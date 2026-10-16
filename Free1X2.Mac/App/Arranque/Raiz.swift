import AppKit
import SwiftUI

/// Contenido de la ventana: pila de páginas sobre Inicio, barra, alertas y `Esc` = atrás (ESPEC_15 §2).
struct Raiz: View {
    @Environment(Sesion.self) private var sesion

    var body: some View {
        @Bindable var router = sesion.router
        let alertas = sesion.alertas
        VStack(spacing: 0) {
            BarraHerramientas(sesion: sesion)
            NavigationStack(path: $router.pila) {
                VistaInicio().navigationDestination(for: Destino.self) { VistaDestino(destino: $0) }
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
        .onReceive(NotificationCenter.default.publisher(for: NSApplication.willTerminateNotification)) { _ in
            sesion.guardarBarras()
        }
        .task {
            sesion.aplicarTema()
            if CommandLine.arguments.contains("-smoke") { await Smoke.ejecutar(sesion) }
            let args = CommandLine.arguments
            if let i = args.firstIndex(of: "-captura"), i + 1 < args.count {
                await Captura.ejecutar(sesion, en: URL(filePath: args[i + 1]))
            }
        }
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
            sesion.router.pila = d == .inicio ? [] : [d]
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
            sesion.router.pila = d == .inicio ? [] : [d]
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
