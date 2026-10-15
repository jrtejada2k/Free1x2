import Free1X2CasosDeUso
import SwiftUI

/// «Columna estadística de ABDON (GEPT)» (pantalla 56, `GEPTFrm`): pronóstico por los dos últimos enfrentamientos.
struct VistaGEPT: View {
    @Environment(Sesion.self) private var sesion
    @State private var primera: [String] = []
    @State private var segunda: [String] = []
    @State private var resultado: [String] = []

    /// `Fondo`: G verde-amarillo, E amarillo, P rosa.
    private static func fondo(_ v: String) -> Color {
        switch v.uppercased() {
        case "G": Color(red: 173 / 255, green: 1, blue: 47 / 255)
        case "E": .yellow
        case "P": Color(red: 1, green: 192 / 255, blue: 203 / 255)
        default: .clear
        }
    }

    private func selector(_ valores: Binding<[String]>, _ i: Int) -> some View {
        Picker("", selection: valores[i]) { ForEach(["", "G", "E", "P"], id: \.self) { Text($0).tag($0) } }
            .labelsHidden().frame(width: 70).background(Self.fondo(valores.wrappedValue[i]), in: .rect(cornerRadius: 4))
    }

    var body: some View {
        Pagina(destino: .gEPT) {
            Text(
                "Para cada partido indica las dos últimas veces que se repitió el enfrentamiento (G = Ganó, E = Empató, P = Perdió). El sistema deriva el pronóstico de signos."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta {
                Grid(alignment: .leading, horizontalSpacing: 16, verticalSpacing: 4) {
                    GridRow {
                        Text("Nº").bold()
                        Text("Última vez").bold()
                        Text("Penúltima vez").bold()
                        Text("Pronóstico").bold()
                    }
                    ForEach(resultado.indices, id: \.self) { i in
                        GridRow {
                            Text(String(format: "%02d", i + 1)).monospacedDigit()
                            selector($primera, i)
                            selector($segunda, i)
                            Text(resultado[i]).monospaced()
                        }
                    }
                }
            }
            Button("Calcular") {
                resultado = resultado.indices.map { CasosUtilidades.traducirGEPT(primera[$0], segunda[$0]) }
            }
            .buttonStyle(.borderedProminent)
        }
        .onAppear {
            guard resultado.isEmpty else { return }
            let n = sesion.numeroPartidos
            (primera, segunda, resultado) = (Array(repeating: "", count: n), Array(repeating: "", count: n), Array(repeating: "", count: n))
        }
    }
}
