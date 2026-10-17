import AppKit
import Free1X2CasosDeUso
import SwiftUI

/// Contenido de la ventana: panel lateral, pila de páginas sobre Inicio, barra de la ventana, alertas y `Esc` = atrás
/// (ESPEC_15 §2; DT-82).
struct Raiz: View {
    @Environment(Sesion.self) private var sesion

    var body: some View {
        @Bindable var router = sesion.router
        let alertas = sesion.alertas
        NavigationSplitView {
            PanelLateral()
        } detail: {
            NavigationStack(path: $router.pila) {
                VistaInicio().navigationDestination(for: Destino.self) { VistaDestino(destino: $0) }
            }
            .toolbar {
                // Las acciones de siempre, con texto; cada pantalla añade las suyas (Inicio: Calcular, Reducir, Jornada).
                ToolbarItemGroup(placement: .primaryAction) {
                    accion("Nueva", "doc.badge.plus", "Nueva combinación", .nuevaCombinacion)
                    accion("Abrir", "folder", "Abrir combinación", .abrirCombinacion)
                    accion("Guardar", "square.and.arrow.down", "Guardar combinación", .guardarCombinacion)
                    accion("Escrutinio", "checklist", "Escrutinio", .navegar(.escrutinios))
                }
            }
        }
        .tint(Color.acento)
        .background {
            Button("Atrás") { sesion.router.atras() }.keyboardShortcut(.escape, modifiers: []).opacity(0)
        }
        .alert(
            alertas.actual?.titulo ?? "", isPresented: Binding(get: { alertas.actual != nil }, set: { _ in }),
            presenting: alertas.actual
        ) { a in
            if a.respuesta != nil {
                Button("Sí") { alertas.cerrar(true) }
                Button("No", role: .cancel) { alertas.cerrar(false) }.keyboardShortcut(.defaultAction)
            } else {
                Button("Aceptar") { alertas.cerrar(false) }
            }
        } message: { a in
            Text(a.mensaje)
        }
        .task {
            sesion.aplicarTema()
            if CommandLine.arguments.contains("-smoke") { await Smoke.ejecutar(sesion) }
            let args = CommandLine.arguments
            if let i = args.firstIndex(of: "-captura"), i + 1 < args.count {
                await Captura.ejecutar(sesion, en: URL(filePath: args[i + 1]))
            }
            // DT-83: la jornada actual al arrancar, 3 s como mucho; si no llega, el boleto queda en blanco y sin aviso. No
            // pisa lo que el usuario haya escrito o abierto mientras tanto.
            let pais = CasosArchivo.paisInicial(entorno: sesion.entorno)
            if let j = await CasosArchivo.jornadaAlArrancar(pais, servicio: sesion.servicioOnline),
                (sesion.locales + sesion.visitantes).allSatisfy(\.isEmpty)
            {
                sesion.cargarJornada(j)
            }
        }
    }

    /// Botón de la barra con icono y texto; `nombre` es el accesible (el de los menús).
    private func accion(_ titulo: String, _ simbolo: String, _ nombre: String, _ c: Comando) -> some View {
        Button {
            sesion.ejecutar(c)
        } label: {
            Label(titulo, systemImage: simbolo)
        }
        .labelStyle(.titleAndIcon).help(nombre).accessibilityLabel(nombre)
    }
}

/// Smoke (10 §1.8): recorre las 109 superficies cada 120 ms y escribe `$TMPDIR/free1x2_smoke.log`.
@MainActor
enum Smoke {
    static func ejecutar(_ sesion: Sesion) async {
        let log = FileManager.default.temporaryDirectory.appending(path: "free1x2_smoke.log")
        var texto = "SMOKE START\n"
        var ok = 0
        for d in Destino.allCases {
            sesion.router.ir(d)
            try? await Task.sleep(for: .milliseconds(120))
            ok += 1
            texto += "OK \(d.clase)\n"
        }
        texto += "SMOKE DONE total=\(Destino.allCases.count) ok=\(ok) fail=\(Destino.allCases.count - ok)\n"
        try? Data(texto.utf8).write(to: log)
        NSApplication.shared.terminate(nil)
    }
}

/// Captura de desarrollo (como el smoke, inerte sin su argumento): `-captura <carpeta>` pinta la ventana entera (barra
/// incluida) de unas pantallas representativas (o de las 109 con `-todas`) a PNG con `cacheDisplay`, sin necesitar el permiso de grabación de pantalla.
@MainActor
enum Captura {
    static func ejecutar(_ sesion: Sesion, en carpeta: URL) async {
        try? await Task.sleep(for: .seconds(1))
        let todas = CommandLine.arguments.contains("-todas")
        for d in todas ? Destino.allCases : [.inicio, .noVariantes, .calculaColumnas, .escrutinios, .tramificar] {
            sesion.router.ir(d)
            try? await Task.sleep(for: .milliseconds(800))
            guard let vista = NSApp.windows.first(where: \.isVisible)?.contentView?.superview,
                let rep = vista.bitmapImageRepForCachingDisplay(in: vista.bounds)
            else { continue }
            vista.cacheDisplay(in: vista.bounds, to: rep)
            try? rep.representation(using: .png, properties: [:])?.write(to: carpeta.appending(path: "\(d.clase).png"))
        }
        NSApplication.shared.terminate(nil)
    }
}
