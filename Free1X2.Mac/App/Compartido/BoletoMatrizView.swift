import SwiftUI

/// `BoletoMatrizControl` (10 §1.9): 8 columnas × 16 apuestas; cada apuesta con las ranuras 1 · X · 2 y solo la signada
/// con texto; cabecera = nº de columna; separación extra tras los partidos 4, 8, 11 y 14.
struct BoletoMatrizView: View {
    let signos: [String]
    let numeros: [Int]

    private static let apuestas = 16
    private static let grupos: Set<Int> = [4, 8, 11, 14]

    var body: some View {
        ScrollView(.horizontal) {
            HStack(alignment: .top, spacing: 10) {
                ForEach(0..<8, id: \.self) { columna(signos[safe: $0] ?? "", numeros[safe: $0] ?? 0) }
            }
            .padding(6)
        }
    }

    private func columna(_ s: String, _ numero: Int) -> some View {
        let u = Array(s.utf16)
        return VStack(spacing: 2) {
            Text(numero > 0 ? String(numero) : " ").font(.caption).bold().foregroundStyle(Color.accentColor)
            ForEach(0..<Self.apuestas, id: \.self) { j in
                if j < u.count {
                    HStack(spacing: 2) {
                        ranura("1", u[j] == 0x31)
                        ranura("X", u[j] == 0x58 || u[j] == 0x78)
                        ranura("2", u[j] == 0x32)
                    }
                    .padding(.bottom, Self.grupos.contains(j + 1) ? 3 : 0)
                }
            }
        }
    }

    private func ranura(_ t: String, _ marcada: Bool) -> some View {
        Text(marcada ? t : " ").font(.caption.monospaced()).frame(width: 16, height: 14)
            .background(marcada ? Color.accentColor.opacity(0.25) : Color.textoSecundario.opacity(0.08), in: .rect(cornerRadius: 2))
    }
}
