import Free1X2CasosDeUso
import SwiftUI

/// «Análisis Premiadas» (pantalla 80, `PremiadasFrm`).
struct VistaPremiadas: View {
    @Environment(Sesion.self) private var sesion
    @State private var fichero: URL?
    @State private var premio = 10
    @State private var validas: [Int32] = []
    @State private var frecuencias: [String] = []
    @State private var seleccion: Int?
    @State private var jornadas: [String] = []
    @State private var procesadas = "0"
    @State private var tiempo = "00:00:00.0"
    @State private var ocupado = false

    private var ficheroValido: URL? { fichero.flatMap { sesion.entorno.archivos.existe($0) ? $0 : nil } }

    private func calcular() async {
        guard let url = ficheroValido else { return sesion.alertas.error("Selecciona primero un fichero de columnas ganadoras.") }
        let t0 = sesion.entorno.reloj.ahora()
        (frecuencias, jornadas, procesadas, ocupado) = ([], [], "0", true)
        do throws(ErrorDominio) {
            let r = try await CasosUtilidades.frecuenciasPremiadas(url, premio: premio, entorno: sesion.entorno)
            (validas, frecuencias, procesadas) = (r.validas, r.frecuencias, String(r.procesadas))
        } catch {
            sesion.alertas.error(
                "No se pudo calcular sobre el fichero de columnas ganadoras:\n" + url.path(percentEncoded: false) + "\n\n"
                    + error.mensajeUsuario)
        }
        tiempo = String(
            (FormatoNET.toStringTimeSpan(sesion.entorno.reloj.ahora().timeIntervalSince(t0)) + "0000000000").prefix(10))
        ocupado = false
    }

    /// Selección y cálculo previos; `n` = frecuencia elegida.
    private func requisitos(fichero: Bool) -> Int? {
        guard let seleccion else {
            sesion.alertas.error("Selecciona una frecuencia en la lista.")
            return nil
        }
        guard !validas.isEmpty else {
            sesion.alertas.error("Calcula primero las frecuencias.")
            return nil
        }
        guard !fichero || ficheroValido != nil else {
            sesion.alertas.error("Selecciona primero un fichero de columnas ganadoras.")
            return nil
        }
        return seleccion + 1
    }

    private func grabar() async {
        guard let n = requisitos(fichero: false),
            let u = await SelectorFicheros.guardar("columnas", [("Cols.Salida", ".txt")])
        else { return }
        do throws(ErrorDominio) {
            try CasosUtilidades.grabarColumnasConFrecuencia(validas, n: n, en: u, entorno: sesion.entorno)
            sesion.alertas.info("Columnas grabadas en \(u.lastPathComponent).")
        } catch {
            sesion.alertas.error("No se pudieron grabar las columnas en:\n" + u.path(percentEncoded: false) + "\n\n" + error.mensajeUsuario)
        }
    }

    private func analizar() async {
        guard let n = requisitos(fichero: true), let url = ficheroValido else { return }
        jornadas = []
        do throws(ErrorDominio) {
            jornadas = try await CasosUtilidades.jornadasDeFrecuencia(url, validas: validas, premio: premio, n: n, entorno: sesion.entorno)
        } catch {
            sesion.alertas.error(
                "No se pudo analizar el fichero de columnas ganadoras:\n" + url.path(percentEncoded: false) + "\n\n" + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .premiadas) {
            Text(
                "Cuenta cuántas columnas resultarían premiadas (10/11/12/13 aciertos) a partir de un fichero de columnas ganadoras, y permite grabar o analizar las frecuencias obtenidas."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Configuración") {
                HStack {
                    Text("Fichero de columnas ganadoras")
                    Button("Seleccionar fichero") { Task { if let u = await SelectorFicheros.abrir(".txt", "*") { fichero = u } } }
                    Text(fichero?.lastPathComponent ?? "Seleccionar Previamente").foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    Picker("Premios a estudiar", selection: $premio) {
                        ForEach([13, 12, 11, 10], id: \.self) { Text("\($0) aciertos").tag($0) }
                    }
                    .fixedSize()
                    Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).disabled(ocupado)
                }
                HStack(spacing: 20) {
                    LabeledContent("Procesadas", value: procesadas)
                    LabeledContent("Tiempo", value: tiempo)
                }
            }
            HStack(alignment: .top) {
                Tarjeta("Frecuencias") {
                    List(frecuencias.indices, id: \.self, selection: $seleccion) { Text(frecuencias[$0]).monospacedDigit() }
                        .frame(minHeight: 220)
                    HStack {
                        Button("Grabar Selección") { Task { await grabar() } }
                        Button("Analiza Selección") { Task { await analizar() } }
                    }
                }
                Tarjeta("Jornadas") {
                    List(jornadas.indices, id: \.self) { Text(jornadas[$0]) }.frame(minHeight: 220)
                }
            }
        }
    }
}
