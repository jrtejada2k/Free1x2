import Free1X2CasosDeUso
import SwiftUI

/// «Grupos de Equipos» (pantalla 62, `GruposEquiposFrm`): botones y mensajes propios (no usa la base de condiciones).
struct VistaGruposEquipos: View {
    @Environment(Sesion.self) private var sesion
    @State private var e = EditorGruposEquipos(CondicionGruposEquipos())
    @State private var cargado = false

    private var equipos: [(casa: String, fuera: String)] { EditorGruposEquipos.equipos(sesion.jornadaActual) }

    /// Mientras `_filtroGE` es el del grupo, lo que `GuardarDatos` escribe en él llega a la combinación.
    private func reflejar() {
        guard e.ligado, e.grupoReal, sesion.combinacion.grupos.indices.contains(sesion.grupoEnEdicion) else { return }
        try? sesion.combinacion.modificarGrupo(sesion.grupoEnEdicion) { $0.condiciones.gruposEquipos = e.filtro }
    }

    private func hacer(_ accion: (inout EditorGruposEquipos) throws -> Void) -> Bool {
        do {
            try accion(&e)
            return true
        } catch let d as ErrorDominio {
            sesion.alertas.inesperado(d)
        } catch {
            sesion.alertas.error(error)
        }
        return false
    }

    private func guardarDatos() -> Bool {
        defer { reflejar() }
        return hacer { try $0.guardarDatos() }
    }

    var body: some View {
        Pagina(destino: .gruposEquipos) {
            Text(
                "Marca los equipos elegidos y especifica las victorias / empates / derrotas y/o la suma de puntos que conseguirán. La pestaña Relaciones combina varios grupos."
            )
            .foregroundStyle(Color.textoSecundario)
            TabView {
                grupos.tabItem { Text("Grupos Equipos") }.padding(8)
                relaciones.tabItem { Text("Relaciones") }.padding(8)
            }
            .frame(minHeight: 520)
            HStack {
                Button("Estadísticas") { estadisticas() }
                Button("Abrir") { Task { await abrir() } }
                Button("Guardar") { Task { await guardar() } }
                Button("Copiar") { copiar() }
                Button("Pegar") { pegar() }
                Button("Borrar") {
                    _ = hacer { try $0.borrar() }
                    reflejar()
                }
                Button("Cancelar") { sesion.router.atras() }
                Button("Aceptar") { aceptar() }.buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction)
            }
            Text(e.estado).font(.callout).foregroundStyle(Color.textoSecundario)
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            e = EditorGruposEquipos(
                sesion.combinacion.grupos[safe: sesion.grupoEnEdicion]?.condiciones.gruposEquipos ?? CondicionGruposEquipos())
        }
    }

    private var grupos: some View {
        VStack(alignment: .leading, spacing: 10) {
            HStack {
                Text("Grupo")
                Button("◄ Anterior") { _ = hacer { try $0.grupoAnterior() } }.disabled(e.contadorGrupos.actual < 2)
                Text("\(e.contadorGrupos.actual)/\(e.contadorGrupos.total)").monospacedDigit()
                Button("Siguiente ►") { _ = hacer { try $0.grupoSiguiente() } }
                Button("Eliminar actual") { _ = hacer { try $0.eliminarGrupo() } }
            }
            HStack(alignment: .top) {
                Tarjeta("Equipos del grupo") {
                    Text("Pulsa el equipo de casa (1) o de fuera (2) de cada partido. Marcar ambos equivale al empate (X).")
                        .font(.callout).foregroundStyle(Color.textoSecundario)
                    Grid(alignment: .leading, horizontalSpacing: 6, verticalSpacing: 3) {
                        ForEach(0..<EditorGruposEquipos.partidos, id: \.self) { i in
                            GridRow {
                                Text(String(format: "%02d", i + 1)).monospacedDigit()
                                Toggle(equipos[i].casa, isOn: $e.casa[i]).toggleStyle(.button).tint(Color.exito)
                                Toggle(equipos[i].fuera, isOn: $e.fuera[i]).toggleStyle(.button).tint(Color.exito)
                            }
                        }
                    }
                    .controlSize(.small)
                }
                Tarjeta("Objetivos del grupo") {
                    CampoCondicion(etiqueta: "Victorias", texto: $e.victorias)
                    CampoCondicion(etiqueta: "Empates", texto: $e.empates)
                    CampoCondicion(etiqueta: "Derrotas", texto: $e.derrotas)
                    CampoCondicion(etiqueta: "Suma Puntos", texto: $e.sumaPuntos)
                }
            }
        }
    }

    private var relaciones: some View {
        VStack(alignment: .leading, spacing: 10) {
            HStack {
                Text("Relación")
                Button("◄ Anterior") { _ = hacer { try $0.relacionAnterior() } }.disabled(e.contadorRelaciones.actual < 2)
                Text("\(e.contadorRelaciones.actual)/\(e.contadorRelaciones.total)").monospacedDigit()
                Button("Siguiente ►") { _ = hacer { try $0.relacionSiguiente() } }
                Button("Eliminar actual") { _ = hacer { try $0.eliminarRelacion() } }
            }
            Tarjeta("Datos de la relación") {
                CampoCondicion(etiqueta: "Grupos Equipos", texto: $e.relGrupos)
                CampoCondicion(etiqueta: "Suma Victorias", texto: $e.relVictorias)
                CampoCondicion(etiqueta: "Suma Empates", texto: $e.relEmpates)
                CampoCondicion(etiqueta: "Suma Derrotas", texto: $e.relDerrotas)
                CampoCondicion(etiqueta: "Suma Puntos", texto: $e.relPuntos)
            }
            Spacer()
        }
    }

    private func aceptar() {
        var f: CondicionGruposEquipos?
        guard hacer({ f = try $0.aceptar() }) else { return }
        if let f, sesion.combinacion.grupos.indices.contains(sesion.grupoEnEdicion) {
            try? sesion.combinacion.modificarGrupo(sesion.grupoEnEdicion) { $0.condiciones.gruposEquipos = f }
        }
        sesion.router.atras()
    }

    private func estadisticas() {
        do throws(ErrorDominio) {
            var c = CondicionesGrupo(numeroPartidos: sesion.numeroPartidos)
            c.gruposEquipos = try e.temporal()
            sesion.ultimasEstadisticas = try CasosCondiciones.estadisticasDeCondicion(
                .gruposEquipos, c, configuracion: sesion.configuracion, entorno: sesion.entorno)
            sesion.router.navegar(.visorEstadisticas)
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    private func exportar(_ url: URL) throws(ErrorDominio) {
        var c = CondicionesGrupo(numeroPartidos: sesion.numeroPartidos)
        c.gruposEquipos = e.paraGuardar()
        reflejar()
        try CasosCondiciones.exportarCondicion(.gruposEquipos, c, en: url, entorno: sesion.entorno)
    }

    private func importar(_ url: URL) throws(ErrorDominio) {
        if let c = try CasosCondiciones.importarCondicion(url, numeroPartidos: sesion.numeroPartidos, entorno: sesion.entorno) {
            e.abrir(c.gruposEquipos)
        }
    }

    private var temporal: URL { CasosCondiciones.rutaTemporal("geq", entorno: sesion.entorno) }

    private func guardar() async {
        guard guardarDatos() else { return }
        guard
            let url = await SelectorFicheros.guardar(
                "Grupos de equipos", [("Grupos de equipos", ".geq"), ("Grupos de equipos (XML)", ".xml")])
        else { return }
        do throws(ErrorDominio) {
            try exportar(url)
            e.estado = "Condición guardada en " + url.lastPathComponent
        } catch {
            sesion.alertas.error("No se pudo guardar: " + error.mensajeUsuario)
        }
    }

    private func abrir() async {
        guard guardarDatos() else { return }
        guard let url = await SelectorFicheros.abrir(".geq", ".xml") else { return }
        do throws(ErrorDominio) {
            try importar(url)
            e.estado = "Condición abierta de " + url.lastPathComponent
        } catch {
            sesion.alertas.error("No se pudo abrir: " + error.mensajeUsuario)
        }
    }

    private func copiar() {
        guard guardarDatos() else { return }
        do throws(ErrorDominio) {
            try exportar(temporal)
            e.estado = "Condición copiada"
        } catch {
            sesion.alertas.error("No se pudo copiar: " + error.mensajeUsuario)
        }
    }

    private func pegar() {
        guard guardarDatos(), sesion.entorno.archivos.existe(temporal) else { return }
        do throws(ErrorDominio) {
            try importar(temporal)
            e.estado = "Condición pegada"
        } catch {
            sesion.alertas.error("No se pudo pegar: " + error.mensajeUsuario)
        }
    }
}
