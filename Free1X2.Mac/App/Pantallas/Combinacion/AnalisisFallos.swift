import Free1X2CasosDeUso
import SwiftUI

/// «Análisis de fallos en combinación» (pantalla 18, `ColGanadoraFrm`): columna ganadora y combinación a analizar.
struct VistaColGanadora: View {
    @Environment(Sesion.self) private var sesion
    @State private var columna = ""
    @State private var enPantalla = true

    private func analizar() async {
        let n = sesion.numeroPartidos
        let col: Columna
        do throws(ErrorDominio) {
            col = try CasosCombinacion.columnaGanadoraParaAnalisis(columna, numeroPartidos: n)
        } catch {
            return sesion.alertas.error(error.mensajeUsuario)
        }
        // El port no recibe nunca el contexto de la pantalla (`ColGanadoraContexto`): esta rama siempre avisa (08 §18).
        guard !enPantalla else {
            return sesion.alertas.error(
                "No hay una combinación en pantalla disponible para analizar. Selecciona \"Abrir combinación para analizar\".")
        }
        guard let url = await SelectorFicheros.abrir(".comb", ".xml") else { return }
        do throws(ErrorDominio) {
            sesion.analisisFallos = try CasosCombinacion.analisisDeCombinacionAbierta(
                url, columna: col, numeroPartidos: n, entorno: sesion.entorno)
            sesion.router.navegar(.analizarCombinacion)
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    var body: some View {
        Pagina(destino: .colGanadora) {
            Text("Introduce la columna ganadora y elige qué combinación analizar para detectar sus fallos frente a esa columna.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta("Columna ganadora") {
                TextField(
                    "", text: Binding(get: { columna }, set: { columna = String(FormatoNET.mayusculas($0).prefix(16)) }),
                    prompt: Text("Signos de la columna ganadora")
                )
                .textFieldStyle(.roundedBorder).monospaced().frame(maxWidth: 320)
                Text("La columna debe tener exactamente \(sesion.numeroPartidos) signos.").font(.callout)
                    .foregroundStyle(Color.textoSecundario)
            }
            Tarjeta("Combinación a analizar") {
                Picker("", selection: $enPantalla) {
                    Text("Analizar combinación en pantalla").tag(true)
                    Text("Abrir combinación para analizar").tag(false)
                }
                .pickerStyle(.radioGroup).labelsHidden()
            }
            Button("Analizar") { Task { await analizar() } }.buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction)
        }
    }
}

/// «Analizar combinación» (pantalla 7, `AnalizarCombinacionFrm`): árbol de condiciones evaluadas.
struct VistaAnalizarCombinacion: View {
    @Environment(Sesion.self) private var sesion
    @State private var titulo = "Analizar combinación"
    @State private var nodos: [NodoAnalisis] = []
    @State private var cargado = false

    var body: some View {
        Pagina(destino: .analizarCombinacion) {
            Text(
                "Resultado del análisis de la combinación frente a las condiciones y filtros definidos. Cada condición se marca como acertada, aceptada por tolerancias o fallada."
            )
            .foregroundStyle(Color.textoSecundario)
            Text(titulo).font(.headline)
            Tarjeta("Condiciones evaluadas") {
                if nodos.isEmpty {
                    Text("Sin resultados todavía. El árbol se rellena al analizar una combinación.").foregroundStyle(Color.textoSecundario)
                } else {
                    ForEach(nodos) { NodoArbol(nodo: $0) }
                }
            }
            Tarjeta("Leyenda") {
                HStack(spacing: 16) {
                    Label {
                        Text("Condición acertada")
                    } icon: {
                        SemaforoView(estado: .verde)
                    }
                    Label {
                        Text("Condición aceptada por tolerancias")
                    } icon: {
                        SemaforoView(estado: .neutro)
                    }
                    Label {
                        Text("Condición fallada")
                    } icon: {
                        SemaforoView(estado: .rojo)
                    }
                }
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            let e =
                sesion.analisisFallos
                ?? CasosCombinacion.analisisDeCombinacionActual(
                    sesion.combinacion, ruta: sesion.rutaCombinacion, numeroPartidos: sesion.numeroPartidos)
            sesion.analisisFallos = nil
            titulo = "Análisis de fallos: " + e.nombre
            do throws(ErrorDominio) {
                (titulo, nodos) = try CasosCombinacion.analizarFallosCombinacion(
                    e, configuracion: sesion.configuracion, entorno: sesion.entorno)
            } catch {
                sesion.alertas.inesperado(error)
            }
        }
    }
}

/// Un nodo del árbol, siempre desplegado (`IsExpanded=True`).
private struct NodoArbol: View {
    let nodo: NodoAnalisis

    private var semaforo: Semaforo {
        switch nodo.estado {
        case .acertada: .verde
        case .tolerancias: .neutro
        case .fallada: .rojo
        }
    }

    var body: some View {
        VStack(alignment: .leading, spacing: 4) {
            HStack(spacing: 6) {
                SemaforoView(estado: semaforo)
                Text(nodo.titulo)
            }
            ForEach(nodo.hijos ?? []) { NodoArbol(nodo: $0).padding(.leading, 20) }
        }
    }
}
