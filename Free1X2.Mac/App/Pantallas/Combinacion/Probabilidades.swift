import Free1X2CasosDeUso
import SwiftUI

/// «Probabilidades de Premios» (pantalla 81, `ProbabilidadPremios`): columnas «madre» frente a «hijas».
struct VistaProbabilidadPremios: View {
    @Environment(Sesion.self) private var sesion
    @State private var madre: URL?
    @State private var hija: URL?
    @State private var rango = ""
    @State private var filas: [FilaProbabilidad] = []

    private func selector(_ titulo: String, _ url: Binding<URL?>) -> some View {
        HStack {
            Text(titulo).frame(width: 140, alignment: .leading)
            Button("Seleccionar...") { Task { if let u = await SelectorFicheros.abrir(".txt", "*") { url.wrappedValue = u } } }
            Text(url.wrappedValue?.deletingPathExtension().lastPathComponent ?? "(seleccionar)").foregroundStyle(Color.textoSecundario)
        }
    }

    private func calcular() async {
        guard let madre, let hija else { return }
        filas = []
        do throws(ErrorDominio) {
            let r = try await CasosCombinacion.probabilidadDePremios(madre: madre, hija: hija, rango: rango, entorno: sesion.entorno)
            if r.errorSignos { sesion.alertas.error("Los Dos archivos tienen un número distinto de signos") }
            filas = r.filas
        } catch {
            sesion.alertas.error("Error al calcular las probabilidades: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .probabilidadPremios) {
            Text(
                "Compara dos conjuntos de columnas (madre frente a hijas) y calcula la probabilidad de premio por cada categoría de aciertos dentro del rango indicado."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Columnas a comparar") {
                selector("Columnas \"madre\"", $madre)
                selector("Columnas \"hijas\"", $hija)
            }
            Tarjeta("Rango de premios") {
                HStack {
                    Text("Rango de aciertos")
                    TextField("Ej. 10-14", text: $rango).frame(width: 160)
                }
                Text("Si se deja vacío se usa el rango por defecto 10-14.").foregroundStyle(Color.textoSecundario)
                Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).disabled(madre == nil || hija == nil)
            }
            Tarjeta("Probabilidades por categoría") {
                Table(filas) {
                    TableColumn("Categoría", value: \.categoria)
                    TableColumn("Probabilidad", value: \.probabilidad)
                }
                .frame(minHeight: 160)
            }
        }
    }
}
