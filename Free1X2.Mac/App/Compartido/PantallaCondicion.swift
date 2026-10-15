import Free1X2CasosDeUso
import SwiftUI

/// Fichero de una condición (`FiltroArchivoViewModelBase`: `NombreSugerido`, `TiposGuardar`, `ExtensionesAbrir`,
/// `RutaTemporal`).
struct ArchivoCondicion {
    let nombreSugerido: String
    let tipos: [(etiqueta: String, ext: String)]
    let extensionTemporal: String
    var extensionesAbrir: [String] { tipos.map(\.ext) }

    /// Los de las 14 condiciones de filtro: propia + `(XML)`; el temporal suele llevar la propia.
    init(_ nombre: String, _ etiqueta: String, _ tipo: TipoFiltro, temporal: String? = nil) {
        nombreSugerido = nombre
        tipos = [(etiqueta, "." + tipo.extensionCondicion), (etiqueta + " (XML)", ".xml")]
        extensionTemporal = temporal ?? tipo.extensionCondicion
    }
}

/// Patrón de pantalla de condición (10 §1.10; plantilla T1): Aceptar · Estadísticas · Guardar · Abrir · Copiar · Pegar ·
/// Borrar · Cancelar. La pantalla aporta el contenido y la conversión pantalla ↔ condición.
struct PantallaCondicion<Contenido: View>: View {
    @Environment(Sesion.self) private var sesion
    let destino: Destino
    let subtitulo: String
    let tipo: TipoFiltro
    let archivo: ArchivoCondicion
    /// Lee la condición del grupo en edición al entrar (no al volver de Estadísticas).
    let cargar: (CondicionesGrupo) -> Void
    /// El filtro temporal con lo que hay en pantalla; `nil` = la pantalla ya avisó y no se sigue.
    let temporal: (Int) throws(ErrorDominio) -> CondicionesGrupo?
    /// Aplica lo de pantalla sobre la condición del grupo; `false` = no se cierra.
    let aceptar: (inout CondicionesGrupo) throws(ErrorDominio) -> Bool
    let borrar: () -> Void
    /// `MarcarValores` al abrir o pegar, si no es igual que `cargar`.
    var marcar: ((CondicionesGrupo) -> Void)? = nil
    /// Lo que Guardar/Copiar escriben, si no es el filtro temporal (Contactos vuelca antes en el grupo).
    var exportable: ((Int) throws(ErrorDominio) -> CondicionesGrupo?)? = nil
    @ViewBuilder var contenido: Contenido
    @State private var cargado = false

    var body: some View {
        Pagina(destino: destino) {
            Text(subtitulo).foregroundStyle(Color.textoSecundario)
            contenido
            Tarjeta("Acciones") {
                HStack {
                    Button("Aceptar", action: botonAceptar).buttonStyle(.borderedProminent).keyboardShortcut(.defaultAction)
                    Button("Estadísticas") { estadisticas() }
                    Divider().frame(height: 16)
                    Button("Guardar") { Task { await guardar() } }
                    Button("Abrir") { Task { await abrir() } }
                    Button("Copiar", action: copiar)
                    Button("Pegar", action: pegar)
                    Divider().frame(height: 16)
                    Button("Borrar", action: borrar)
                    Button("Cancelar") { sesion.router.atras() }
                }
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            if let g = sesion.combinacion.grupos[safe: sesion.grupoEnEdicion] { cargar(g.condiciones) }
        }
    }

    private var n: Int { sesion.numeroPartidos }

    private func exportar() throws(ErrorDominio) -> CondicionesGrupo? {
        if let exportable { return try exportable(n) }
        return try temporal(n)
    }

    private func botonAceptar() {
        guard sesion.combinacion.grupos.indices.contains(sesion.grupoEnEdicion) else { return sesion.router.atras() }
        do throws(ErrorDominio) {
            var cerrar = false
            try sesion.combinacion.modificarGrupo(sesion.grupoEnEdicion) { g throws(ErrorDominio) in
                cerrar = try aceptar(&g.condiciones)
            }
            if cerrar { sesion.router.atras() }
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    private func estadisticas() {
        do throws(ErrorDominio) {
            guard let t = try temporal(n) else { return }
            sesion.ultimasEstadisticas = try CasosCondiciones.estadisticasDeCondicion(
                tipo, t, configuracion: sesion.configuracion, entorno: sesion.entorno)
            sesion.router.navegar(.visorEstadisticas)
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    private func guardar() async {
        guard let url = await SelectorFicheros.guardar(archivo.nombreSugerido, archivo.tipos) else { return }
        do throws(ErrorDominio) {
            guard let t = try exportar() else { return }
            try CasosCondiciones.exportarCondicion(tipo, t, en: url, entorno: sesion.entorno)
        } catch {
            sesion.alertas.error("No se pudo guardar: " + error.mensajeUsuario)
        }
    }

    private func abrirDesde(_ url: URL) throws(ErrorDominio) {
        if let c = try CasosCondiciones.importarCondicion(url, numeroPartidos: n, entorno: sesion.entorno) { (marcar ?? cargar)(c) }
    }

    private func abrir() async {
        guard let url = await SelectorFicheros.abrirVarios(archivo.extensionesAbrir, multiple: false).first else { return }
        do throws(ErrorDominio) {
            try abrirDesde(url)
        } catch {
            sesion.alertas.error("No se pudo abrir: " + error.mensajeUsuario)
        }
    }

    private func copiar() {
        do throws(ErrorDominio) {
            guard let t = try exportar() else { return }
            try CasosCondiciones.exportarCondicion(
                tipo, t, en: CasosCondiciones.rutaTemporal(archivo.extensionTemporal, entorno: sesion.entorno),
                entorno: sesion.entorno)
        } catch {
            sesion.alertas.error("No se pudo copiar: " + error.mensajeUsuario)
        }
    }

    /// Sin fichero temporal no hace nada; sus errores no se capturan en el C# (llegan al manejador global).
    private func pegar() {
        let url = CasosCondiciones.rutaTemporal(archivo.extensionTemporal, entorno: sesion.entorno)
        guard sesion.entorno.archivos.existe(url) else { return }
        do throws(ErrorDominio) {
            try abrirDesde(url)
        } catch {
            sesion.alertas.inesperado(error)
        }
    }
}

/// Campo de texto de una condición con su etiqueta y *placeholder*.
struct CampoCondicion: View {
    let etiqueta: String
    @Binding var texto: String
    var ayuda = ""

    var body: some View {
        LabeledContent(etiqueta) {
            TextField(etiqueta, text: $texto, prompt: Text(ayuda)).labelsHidden().textFieldStyle(.roundedBorder)
        }
    }
}

/// «Estadísticas» (pantalla 103, `VisorEstadisticas`): archivo, barra 0–100 y `Cumplimiento + "%"`.
struct VistaVisorEstadisticas: View {
    @Environment(Sesion.self) private var sesion

    var body: some View {
        Pagina(destino: .visorEstadisticas) {
            Text("Porcentaje de cumplimiento de cada fichero analizado.").foregroundStyle(Color.textoSecundario)
            let lista = sesion.ultimasEstadisticas ?? []
            if lista.isEmpty {
                Text("No hay estadísticas para mostrar.").foregroundStyle(Color.textoSecundario)
            } else {
                Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 6) {
                    GridRow {
                        Text("Archivo").bold()
                        Text("")
                        Text("Cumplimiento").bold()
                    }
                    ForEach(Array(lista.enumerated()), id: \.offset) { _, e in
                        GridRow {
                            Text(e.archivo)
                            ProgressView(value: min(max(Double(e.cumplimiento), 0), 100), total: 100).frame(width: 160)
                            Text(CasosCondiciones.textoCumplimiento(e, cultura: sesion.entorno.cultura) + "%").monospacedDigit()
                        }
                    }
                }
            }
        }
    }
}
