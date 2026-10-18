import Free1X2CasosDeUso
import SwiftUI

/// «Pesos Numéricos» (pantalla 78, `PesosNumFrm`).
struct VistaPesosNumericos: View {
    @Environment(Sesion.self) private var sesion
    @State private var pesos = ["", "", "", "", ""]
    @State private var tolerados = ["", "", "", "", ""]
    @State private var numTol = Array(repeating: false, count: 6)
    @State private var figuras = Array(repeating: false, count: 5)
    private static let nombres = ["Global", "Var (variantes)", "1", "X", "2"]

    private func aplicar(_ f: inout CondicionPesosNumericos) throws(ErrorDominio) {
        try CasosCondiciones.aplicarPesosNumericos(
            pesos: pesos, tolerados: tolerados, numTolerancias: numTol, figuras: figuras, en: &f,
            numeroPartidos: sesion.numeroPartidos)
    }

    var body: some View {
        PantallaCondicion(
            destino: .pesosNum,
            subtitulo:
                "El Peso Numérico de una columna es una representación numérica de la columna. También se puede expresar el Peso Numérico de Variantes, 1, X y 2.",
            tipo: .pesosNumericos, archivo: ArchivoCondicion("PesosNumericos", "Pesos Numéricos", .pesosNumericos),
            cargar: { c in
                let f = c.pesosNumericos
                pesos = CondicionPesosNumericos.Peso.allCases.map(f.getPN)
                tolerados = CondicionPesosNumericos.Peso.allCases.map(f.getPNTol)
                numTol = CasosCondiciones.toleranciasMarcadasPesos(f)
                figuras = CasosCondiciones.figurasMarcadasPesos(f)
            },
            temporal: { n throws(ErrorDominio) in
                var c = CondicionesGrupo(numeroPartidos: n)
                try aplicar(&c.pesosNumericos)
                return c
            },
            aceptar: { c throws(ErrorDominio) in
                try aplicar(&c.pesosNumericos)
                return true
            },
            borrar: {
                pesos = pesos.map { _ in "" }
                tolerados = tolerados.map { _ in "" }
                numTol = numTol.map { _ in false }
                figuras = figuras.map { _ in false }
            }
        ) {
            HStack(alignment: .top) {
                Tarjeta("Pesos Numéricos") {
                    Text("Dígitos admitidos por cada conjunto: 0 a 9 (separados por comas).").font(.callout).foregroundStyle(
                        Color.textoSecundario)
                    ForEach(0..<5, id: \.self) { i in CampoCondicion(etiqueta: Self.nombres[i], texto: $pesos[i], ayuda: "p. ej. 0,1,2,3") }
                }
                Tarjeta("Tolerancias") {
                    ForEach(0..<5, id: \.self) { i in
                        CampoCondicion(etiqueta: Self.nombres[i], texto: $tolerados[i], ayuda: "p. ej. 0,1,2,3")
                    }
                }
            }
            Tarjeta("Núm. Tolerancias") {
                Text("Selecciona las tolerancias numéricas a aplicar (0 a 5).").font(.callout).foregroundStyle(Color.textoSecundario)
                HStack {
                    ForEach(0..<6, id: \.self) { i in Toggle("\(i)", isOn: $numTol[i]).toggleStyle(.button).tint(Color.exito) }
                }
            }
            Tarjeta("Figuras") {
                HStack {
                    ForEach(0..<5, id: \.self) { i in
                        Toggle(CasosCondiciones.figurasPesos[i], isOn: $figuras[i]).toggleStyle(.button).tint(Color.exito)
                    }
                }
                Text("No se incluyen las figuras 5 y 4-1, que darían 0 columnas.").font(.caption).foregroundStyle(Color.textoSecundario)
            }
        }
    }
}
