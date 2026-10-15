import Free1X2CasosDeUso
import SwiftUI

/// «Número de Variantes» (pantalla 75, `NoVariantesFrm`).
struct VistaNoVariantes: View {
    @State private var variantes = ""
    @State private var equis = ""
    @State private var doses = ""

    private func aplicar(_ f: inout CondicionNoVariantes) throws(ErrorDominio) {
        try CasosCondiciones.aplicarNoVariantes(variantes: variantes, equis: equis, doses: doses, en: &f)
    }

    var body: some View {
        PantallaCondicion(
            destino: .noVariantes,
            subtitulo: "Especifica la cantidad admitida de Variantes, de signos X y de signos 2 por combinación.",
            tipo: .noVariantes, archivo: ArchivoCondicion("CantidadSignos", "Cantidad de signos V, X y 2", .noVariantes),
            cargar: { c in (variantes, equis, doses) = (c.noVariantes.getVariantes, c.noVariantes.getEquis, c.noVariantes.getDoses) },
            temporal: { n throws(ErrorDominio) in
                var c = CondicionesGrupo(numeroPartidos: n)
                try aplicar(&c.noVariantes)
                return c
            },
            aceptar: { c throws(ErrorDominio) in
                try aplicar(&c.noVariantes)
                return true
            },
            borrar: { (variantes, equis, doses) = ("", "", "") }
        ) {
            Tarjeta("Cantidades admitidas (0–15)") {
                Text("Introduce las cantidades permitidas para cada concepto. Separa varios valores con comas o espacios.")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                CampoCondicion(etiqueta: "Var", texto: $variantes, ayuda: "p. ej. 0 1 2")
                CampoCondicion(etiqueta: "X", texto: $equis, ayuda: "p. ej. 0 1 2")
                CampoCondicion(etiqueta: "2", texto: $doses, ayuda: "p. ej. 0 1 2")
            }
        }
    }
}
