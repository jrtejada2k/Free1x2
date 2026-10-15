import Free1X2CasosDeUso
import SwiftUI

/// «Analizar fichero de columnas» (pantalla 8, `AnalizarFicheroFrm`): análisis externo con visor.
struct VistaAnalizarFichero: View {
    @Environment(Sesion.self) private var sesion
    @State private var fichero: URL?
    @State private var columnas = 0
    @State private var resumen = ""
    @State private var incluirPleno = false
    @State private var puedeIncluirPleno = false

    private func abrir() async {
        guard let url = await SelectorFicheros.abrir(".txt", ".cols", "*") else { return }
        fichero = url
        do throws(ErrorDominio) {
            let cols = try CasosCombinacion.columnasDeFichero(url, entorno: sesion.entorno)
            columnas = cols.count
            resumen = "\(cols.count) columnas."
            puedeIncluirPleno = cols.first?.utf16.count == 15
            if cols.isEmpty { incluirPleno = false }
        } catch {
            (columnas, resumen) = (0, "")
            sesion.alertas.error("No se ha podido leer el fichero de columnas.")
        }
    }

    private func analizar() async {
        guard let fichero else { return }
        guard columnas > 0 else { return sesion.alertas.error("No se ha cargado el fichero de entrada o no tiene columnas.") }
        var cfg = sesion.configuracion
        cfg.puntosCP = CasosAjustes.leerPuntosCP(entorno: sesion.entorno)
        do throws(ErrorDominio) {
            if let g = try await CasosCombinacion.analizarFicheroDeColumnas(fichero, configuracion: cfg, entorno: sesion.entorno) {
                sesion.visorAnalisis = (g, GrupoConfig(numeroPartidos: sesion.numeroPartidos))
                sesion.router.navegarSiempre(.visorAnalisisColumnas)
            }
        } catch {
            sesion.alertas.error("Error al analizar el fichero: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .analizarFichero) {
            Text("Selecciona un fichero de columnas (.txt / .cols) y lanza el análisis de la combinación.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta {
                LabeledContent("Nombre del fichero de columnas") { Text(fichero?.lastPathComponent ?? "") }
                LabeledContent("Ruta del fichero de columnas") { Text(fichero?.path(percentEncoded: false) ?? "").textSelection(.enabled) }
                HStack {
                    Button("Examinar…") { Task { await abrir() } }
                    Text(resumen).foregroundStyle(Color.textoSecundario)
                }
                Toggle("Incluir pleno al 15", isOn: $incluirPleno).toggleStyle(.switch).disabled(!puedeIncluirPleno)
            }
            HStack {
                Button("Cancelar") { sesion.router.atras() }
                Button("Analizar") { Task { await analizar() } }.buttonStyle(.borderedProminent).keyboardShortcut(.defaultAction)
                    .disabled(columnas == 0)
            }
        }
    }
}
