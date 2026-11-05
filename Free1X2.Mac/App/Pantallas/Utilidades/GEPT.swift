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

    /// M8 (#25): caja de texto como el original; pasa a mayúsculas y otra letra avisa y deja la celda vacía.
    private func selector(_ valores: Binding<[String]>, _ i: Int) -> some View {
        TextField("", text: valores[i])
            .labelsHidden().multilineTextAlignment(.center).frame(width: 70)
            .background(Self.fondo(valores.wrappedValue[i]), in: .rect(cornerRadius: 4))
            .onChange(of: valores.wrappedValue[i]) { _, nuevo in
                let v = FormatoNET.mayusculas(FormatoNET.recortar(nuevo))
                if ["", "G", "E", "P"].contains(v) {
                    if v != nuevo { valores.wrappedValue[i] = v }
                } else {
                    sesion.alertas.error("Las letras permitidas son: G,E,P")
                    valores.wrappedValue[i] = ""
                }
            }
    }

    var body: some View {
        Pagina(destino: .gEPT) {
            Text(
                "En la 1.ª columna, el último resultado de liga de cada equipo local; en la 2.ª, el de cada visitante (G = Ganó, E = Empató, P = Perdió). Calcular da el pronóstico."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta {
                Grid(alignment: .leading, horizontalSpacing: 16, verticalSpacing: 4) {
                    GridRow {
                        Text("Última vez").bold().gridCellColumns(3).frame(maxWidth: .infinity)
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
            .buttonStyle(EstiloPrincipal())
        }
        .onAppear {
            guard resultado.isEmpty else { return }
            let n = sesion.numeroPartidos
            (primera, segunda, resultado) = (Array(repeating: "", count: n), Array(repeating: "", count: n), Array(repeating: "", count: n))
        }
    }
}
