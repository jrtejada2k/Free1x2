import Free1X2CasosDeUso
import SwiftUI

/// `MatrizAnalisisView` (10 §1.9): rejilla de enteros con fila de cabecera y etiqueta por fila; desplazable en horizontal.
struct MatrizAnalisisView: View {
    let matriz: MatrizAnalisis

    var body: some View {
        ScrollView(.horizontal) {
            Grid(alignment: .trailing, horizontalSpacing: 10, verticalSpacing: 3) {
                ForEach(Array(matriz.filas.enumerated()), id: \.offset) { _, f in
                    GridRow {
                        Text(f.etiqueta).bold().gridColumnAlignment(.leading)
                        ForEach(Array(f.celdas.enumerated()), id: \.offset) { _, c in
                            Text(c).monospacedDigit().fontWeight(f.esCabecera ? .semibold : .regular)
                                .foregroundStyle(f.esCabecera ? Color.textoSecundario : Color.primary)
                        }
                    }
                }
            }
            .padding(.vertical, 4)
        }
    }
}
