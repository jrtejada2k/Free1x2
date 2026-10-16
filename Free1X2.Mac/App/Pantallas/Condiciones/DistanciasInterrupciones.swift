import Free1X2CasosDeUso
import SwiftUI

/// «Distancias» (pantalla 43, `DistanciasFrm`).
struct VistaDistancias: View {
    @State private var textos = ["", "", "", ""]

    var body: some View {
        PantallaCondicion(
            destino: .distancias, subtitulo: "Distancia: número máximo de partidos que separan dos signos iguales.",
            tipo: .distancias, archivo: ArchivoCondicion("Distancias", "Distancias", .distancias),
            cargar: { c in textos = [c.distancias.getIntVar, c.distancias.getInt1, c.distancias.getIntX, c.distancias.getInt2] },
            temporal: { n throws(ErrorDominio) in
                var c = CondicionesGrupo(numeroPartidos: n)
                try CasosCondiciones.aplicarDistancias(textos, en: &c.distancias)
                return c
            },
            aceptar: { c throws(ErrorDominio) in
                try CasosCondiciones.aplicarDistancias(textos, en: &c.distancias)
                return true
            },
            borrar: { textos = ["", "", "", ""] }
        ) {
            Tarjeta("Valores admitidos (0–15)") {
                Text("Introduce las distancias permitidas para cada tipo de signo. Separa varios valores con comas (p. ej. 0,1,2).")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                ForEach(0..<4, id: \.self) { i in
                    CampoCondicion(etiqueta: ["Var", "1", "X", "2"][i], texto: $textos[i], ayuda: "p. ej. 0,1,2")
                }
            }
        }
    }
}

/// «Interrupciones» (pantalla 69, `InterrupcionesFrm`).
struct VistaInterrupciones: View {
    @State private var textos = Array(repeating: "", count: 10)
    private static let nombres = ["Global", "Var", "1", "X", "2"]

    var body: some View {
        PantallaCondicion(
            destino: .interrupciones,
            subtitulo: "Una interrupción es cada cambio de signo que se produce a lo largo de la columna.",
            tipo: .interrupciones, archivo: ArchivoCondicion("Interrupciones", "Interrupciones", .interrupciones),
            cargar: { c in textos = CondicionInterrupciones.Contador.allCases.map { c.interrupciones.get($0) } },
            temporal: { n throws(ErrorDominio) in
                var c = CondicionesGrupo(numeroPartidos: n)
                try CasosCondiciones.aplicarInterrupciones(textos, en: &c.interrupciones)
                return c
            },
            aceptar: { c throws(ErrorDominio) in
                try CasosCondiciones.aplicarInterrupciones(textos, en: &c.interrupciones)
                return true
            },
            borrar: { textos = textos.map { _ in "" } }
        ) {
            HStack(alignment: .top) {
                bloque(
                    "Interrupciones", "Número de interrupciones admitidas (0–14) por tipo. Separa varios valores con comas, p. ej. 0,1,2.",
                    0)
                bloque(
                    "Seguidas",
                    "Número de interrupciones seguidas admitidas (0–14) por tipo. Separa varios valores con comas, p. ej. 0,1,2.",
                    5)
            }
        }
    }

    private func bloque(_ titulo: String, _ ayuda: String, _ desde: Int) -> some View {
        Tarjeta(titulo) {
            Text(ayuda).font(.callout).foregroundStyle(Color.textoSecundario)
            ForEach(0..<5, id: \.self) { i in
                CampoCondicion(etiqueta: Self.nombres[i], texto: $textos[desde + i], ayuda: "p. ej. 0,1,2")
            }
        }
    }
}
