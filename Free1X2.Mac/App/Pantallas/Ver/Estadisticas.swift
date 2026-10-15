import Free1X2CasosDeUso
import SwiftUI

/// «Estadísticas 0.3.4» (pantalla 9, `Anastatics`): una de 4 estadísticas sobre un fichero de columnas de 14 signos.
struct VistaAnastatics: View {
    @Environment(Sesion.self) private var sesion
    @State private var modo = Anastatics.Modo.dibujos
    @State private var fichero = "Fichero origen"
    @State private var columnasTexto = "Columnas"
    @State private var columnas: [String] = []
    @State private var origenSeleccionado = false
    @State private var resultado: MatrizEstadistica?

    private func seleccionarOrigen() async {
        guard let url = await SelectorFicheros.abrir(".txt", "*") else { return }
        fichero = url.lastPathComponent
        origenSeleccionado = false
        resultado = nil
        do throws(ErrorDominio) {
            let r = try CasosVer.columnasEstadisticas(url, entorno: sesion.entorno)
            if let e = r.errorEn { return sesion.alertas.error("columna errónea = \(e)") }
            columnas = r.columnas
            columnasTexto = String(columnas.count)
            origenSeleccionado = true
        } catch {
            sesion.alertas.error("No se ha podido leer el fichero: " + error.mensajeUsuario)
        }
    }

    private func calcular() async {
        guard !columnas.isEmpty else { return sesion.alertas.error("No hay columnas que analizar.") }
        do throws(ErrorDominio) {
            resultado = try await CasosVer.calcularEstadistica(modo, columnas: columnas)
        } catch {
            sesion.alertas.error("Error en el cálculo: " + error.mensajeUsuario)
        }
    }

    /// Usa el modo seleccionado **ahora**, aunque se calculara con otro (se replica).
    private func mostrar() {
        guard let resultado else { return }
        sesion.estadisticas[modo] = resultado
        sesion.router.navegar([.dib, .dibRep, .staInter, .staSS][modo.rawValue])
    }

    var body: some View {
        Pagina(destino: .anastatics) {
            Text("Analiza un fichero de columnas (combinaciones de 14 signos 1/X/2) y calcula estadísticas según el modo elegido.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta("Condiciones") {
                Picker("Modo de análisis", selection: $modo) {
                    Text("Variantes, X, 2").tag(Anastatics.Modo.dibujos)
                    Text("Sus coincidencias").tag(Anastatics.Modo.dibRepes)
                    Text("Interrupciones").tag(Anastatics.Modo.staInter)
                    Text("Signos seguidos").tag(Anastatics.Modo.staSigSeg)
                }
                .fixedSize()
                Text("Modo de análisis a aplicar sobre las columnas.").foregroundStyle(Color.textoSecundario)
            }
            Tarjeta("Origen de datos") {
                Button("Selección origen") { Task { await seleccionarOrigen() } }
                Text("Fichero: " + fichero)
                Text("Columnas: " + columnasTexto)
            }
            Tarjeta("Acciones") {
                HStack {
                    Button("Calcular") { Task { await calcular() } }.disabled(!origenSeleccionado)
                    Button("Mostrar resultados", action: mostrar).buttonStyle(.borderedProminent).disabled(resultado == nil)
                }
                Text("Selecciona un origen para calcular. Tras calcular, podrás mostrar los resultados.")
                    .foregroundStyle(Color.textoSecundario)
            }
        }
    }
}

/// Esqueleto común de los cuatro visores: descripción, selector porcentajes/columnas, nº de columnas y matrices.
private struct VisorEstadistica: View {
    let destino: Destino
    let descripcion: String
    let selector: String
    let opciones: (String, String)
    let etiquetaColumnas: String
    let columnas: String
    let titulo: String
    let bloques: (Bool) -> [BloqueAnalisis]
    @State private var porcentajes = true

    var body: some View {
        Pagina(destino: destino) {
            Text(descripcion).foregroundStyle(Color.textoSecundario)
            Tarjeta("Mostrar") {
                Picker(selector, selection: $porcentajes) {
                    Text(opciones.0).tag(true)
                    Text(opciones.1).tag(false)
                }
                .pickerStyle(.segmented).fixedSize()
                Text(etiquetaColumnas + " " + columnas)
            }
            Tarjeta(titulo) {
                ForEach(Array(bloques(porcentajes).enumerated()), id: \.offset) { i, b in
                    if i > 0 && !b.titulo.isEmpty { Text(b.titulo).bold() }
                    MatrizAnalisisView(matriz: b.matriz)
                }
            }
        }
    }
}

/// «Dibujos» (pantalla 38, `DibForm`): nº de X × nº de 2, triangular, con totales.
struct VistaDibForm: View {
    @Environment(Sesion.self) private var sesion

    var body: some View {
        let m = sesion.estadisticas[.dibujos] ?? .vacia
        VisorEstadistica(
            destino: .dib,
            descripcion:
                "Distribución por nº de X (filas) y nº de 2 (columnas): tabla triangular con totales por fila, por columna y por nº de variantes (X+2). Elige ver porcentajes o número de columnas.",
            selector: "Modo de visualización", opciones: ("Porcentajes", "Columnas"), etiquetaColumnas: "Columnas analizadas:",
            columnas: String(m.numColumnas), titulo: "Distribución (filas = nº de X, columnas = nº de 2)"
        ) { CasosVer.matrizDibujos(m, porcentajes: $0) }
    }
}

/// «Coincidencias» (pantalla 39, `DibRepFrm`): 5 × 15, cultura invariante.
struct VistaDibRep: View {
    @Environment(Sesion.self) private var sesion

    var body: some View {
        let m = sesion.estadisticas[.dibRepes] ?? .vacia
        VisorEstadistica(
            destino: .dibRep, descripcion: "Distribución de coincidencias por posición de la quiniela.",
            selector: "Modo de visualización", opciones: ("Porcentajes", "Columnas (conteo)"), etiquetaColumnas: "Columnas totales:",
            columnas: String(m.numColumnas), titulo: "Posiciones"
        ) {
            let e = ["posiciones", "cant. 1", "cant. X", "cant. 2", "cant. V"]
            return [
                BloqueAnalisis(
                    titulo: "", matriz: CasosVer.matrizEstadistica(m, etiquetas: e, columnas: 15, porcentajes: $0, cultura: .invariante))
            ]
        }
    }
}

/// «Interrupciones» (pantalla 90, `StaInterFrm`): 5 agregados × 14 partidos.
struct VistaStaInter: View {
    @Environment(Sesion.self) private var sesion

    var body: some View {
        let m = sesion.estadisticas[.staInter] ?? .vacia
        VisorEstadistica(
            destino: .staInter, descripcion: "Tabla de interrupciones de la reducción (5 agregados x 14 partidos).",
            selector: "Valores", opciones: ("porcentajes", "columnas"), etiquetaColumnas: "Nº columnas", columnas: String(m.numColumnas),
            titulo: "Detalle por partido"
        ) {
            let e = ["globales", "cant. 1", "cant. X", "cant. 2", "cant. V"]
            return [
                BloqueAnalisis(
                    titulo: "",
                    matriz: CasosVer.matrizEstadistica(m, etiquetas: e, columnas: 14, porcentajes: $0, cultura: sesion.entorno.cultura))
            ]
        }
    }
}

/// «Signos seguidos 0.1» (pantalla 91, `StaSSForm`): rachas de 1, X, 2 y V por longitud.
struct VistaStaSS: View {
    @Environment(Sesion.self) private var sesion

    var body: some View {
        let m = sesion.estadisticas[.staSigSeg]
        VisorEstadistica(
            destino: .staSS,
            descripcion:
                "Frecuencia de signos seguidos (rachas de 1, X, 2 y total V) por longitud de racha, sobre el conjunto de columnas analizadas.",
            selector: "Modo de visualización", opciones: ("porcentajes", "columnas"), etiquetaColumnas: "Columnas analizadas",
            columnas: m.map { String($0.numColumnas) } ?? "-", titulo: "Distribución por longitud de racha"
        ) {
            let e = ["cant. 1", "cant. X", "cant. 2", "cant. V"]
            return [
                BloqueAnalisis(
                    titulo: "",
                    matriz: CasosVer.matrizEstadistica(
                        m ?? .vacia, etiquetas: e, columnas: 15, porcentajes: $0, cultura: sesion.entorno.cultura))
            ]
        }
    }
}
