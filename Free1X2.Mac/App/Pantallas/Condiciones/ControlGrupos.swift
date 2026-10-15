import Free1X2CasosDeUso
import SwiftUI

/// «Control de Grupos» (pantalla 27, `ControlGruposFrm`): trabaja sobre copias y confirma con OK.
struct VistaControlGrupos: View {
    @Environment(Sesion.self) private var sesion
    @State private var e = EditorControlGrupos(Combinacion(numeroPartidos: 14))
    @State private var cargado = false

    /// Ejecuta una acción del editor y muestra los avisos «Hay errores…» que el C# habría sacado.
    private func hacer(_ accion: (inout EditorControlGrupos) throws -> Void) -> Bool {
        let antes = e.errores
        defer { for _ in antes..<max(e.errores, antes) { sesion.alertas.error(ControlGruposConfig.mensajeError) } }
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

    private func seccion(
        _ titulo: String, contador: String, _ texto: Binding<String>, _ ayuda: String, _ etiqueta: String, _ fallos: Binding<String>,
        anterior: @escaping (inout EditorControlGrupos) throws -> Void, siguiente: @escaping (inout EditorControlGrupos) throws -> Void,
        eliminar: @escaping (inout EditorControlGrupos) throws -> Void
    ) -> some View {
        Tarjeta(titulo) {
            HStack {
                Button("Anterior") { _ = hacer(anterior) }
                Text(contador).monospacedDigit()
                Button("Siguiente") { _ = hacer(siguiente) }
                Button("Eliminar Actual") { _ = hacer(eliminar) }
            }
            CampoCondicion(etiqueta: etiqueta, texto: texto, ayuda: ayuda)
            CampoCondicion(etiqueta: "Fallos", texto: fallos, ayuda: "Fallos permitidos")
        }
    }

    var body: some View {
        Pagina(destino: .controlGrupos) {
            Text(
                "Define controles de fallos sobre grupos de partidos y sobre conjuntos de grupos. Cada sección es una lista de controles que se recorre con Anterior y Siguiente; los grupos o conjuntos no incluidos quedan como sueltos en el control base."
            )
            .foregroundStyle(Color.textoSecundario)
            seccion(
                "Control Grupos", contador: e.contadorGrupos, $e.grupos, "Grupos controlados, p. ej. 1,3,5", "Grupos", $e.fallosGrupos,
                anterior: { try $0.gruposAnterior() }, siguiente: { try $0.gruposSiguiente() }, eliminar: { try $0.gruposEliminar() })
            seccion(
                "Control Conjuntos", contador: e.contadorConjuntos, $e.conjuntos, "Conjuntos controlados, p. ej. 1,2", "Conjuntos",
                $e.fallosConjuntos, anterior: { try $0.conjuntosAnterior() }, siguiente: { try $0.conjuntosSiguiente() },
                eliminar: { try $0.conjuntosEliminar() })
            HStack {
                Button("OK") {
                    var c = sesion.combinacion
                    let ok = hacer { try $0.aceptar(en: &c) }
                    sesion.combinacion = c
                    if ok { sesion.router.atras() }
                }
                .buttonStyle(.borderedProminent).keyboardShortcut(.defaultAction)
                Button("Cancelar") { sesion.router.atras() }
                Text(e.estado).font(.callout).foregroundStyle(Color.textoSecundario)
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            e = EditorControlGrupos(sesion.combinacion)
        }
    }
}

/// «Control Tolerancias» (pantalla 28, `ControlTolFrm`): tolerancias del grupo en pantalla.
struct VistaControlTol: View {
    @Environment(Sesion.self) private var sesion
    @State private var filas: [(letras: String, aciertos: String)] = []
    @State private var fallos = ""
    @State private var cargado = false

    var body: some View {
        Pagina(destino: .controlTol) {
            Tarjeta("Fallos Controles") {
                Text("Número de fallos permitidos en los controles.").font(.callout).foregroundStyle(Color.textoSecundario)
                TextField("", text: $fallos, prompt: Text("Fallos permitidos")).textFieldStyle(.roundedBorder).frame(width: 200)
            }
            Tarjeta("Tolerancias") {
                Grid(alignment: .leading, horizontalSpacing: 6, verticalSpacing: 3) {
                    GridRow {
                        Text("Tolerancia").bold()
                        Text("Aciertos").bold()
                    }
                    ForEach(filas.indices, id: \.self) { i in
                        GridRow {
                            TextField("", text: $filas[i].letras, prompt: Text("Tolerancia")).frame(width: 120)
                            TextField("", text: $filas[i].aciertos, prompt: Text("Aciertos")).frame(width: 200)
                        }
                    }
                }
            }
            HStack {
                Button("Cancelar") { sesion.router.atras() }
                Button("Ok") {
                    let g = sesion.grupoActual
                    do throws(ErrorDominio) {
                        try sesion.combinacion.modificarGrupo(g) { grupo throws(ErrorDominio) in
                            try CasosCondiciones.aplicarTolerancias(filas, fallos: fallos, en: &grupo.tolerancias)
                        }
                        sesion.router.atras()
                    } catch {
                        sesion.alertas.inesperado(error)
                    }
                }
                .buttonStyle(.borderedProminent).keyboardShortcut(.defaultAction)
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            let t = sesion.combinacion.grupos[safe: sesion.grupoActual]?.tolerancias ?? ControladorTolConfig()
            filas = CasosCondiciones.filasTolerancias(t)
            fallos = t.fallosPermitidos
        }
    }
}
