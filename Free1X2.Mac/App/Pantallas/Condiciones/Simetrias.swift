import Free1X2CasosDeUso
import SwiftUI

/// «Simetrías» (pantalla 89, `SimetriasFrm`): 30 filas iniciales y «Añadir fila».
struct VistaSimetrias: View {
    @Environment(Sesion.self) private var sesion
    @State private var filas = Array(repeating: "", count: 30)
    @State private var aciertos = ""

    private func marcar(_ f: CondicionSimetrias) {
        filas = filas.map { _ in "" }
        while filas.count < f.simetrias.count { filas.append("") }
        for (i, s) in f.simetrias.enumerated() { filas[i] = s.partidos }
        aciertos = f.aciertosTexto
    }

    /// `ObtenerFiltroTemporal`: si no es válido avisa y no sigue.
    private func temporal(_ n: Int) throws(ErrorDominio) -> CondicionesGrupo? {
        guard let s = try CasosCondiciones.condicionSimetrias(filas: filas, aciertos: aciertos, numeroPartidos: n) else {
            sesion.alertas.error(CasosCondiciones.mensajeSimetriaInvalida)
            return nil
        }
        var c = CondicionesGrupo(numeroPartidos: n)
        c.simetrias = s
        return c
    }

    var body: some View {
        PantallaCondicion(
            destino: .simetrias,
            subtitulo:
                "Se da una simetría entre dos o más partidos cuando su signo es el mismo. Especifica los partidos separados por comas (a,b), guiones (a-b) o una mezcla de ambos (a,b-c).",
            tipo: .simetrias, archivo: ArchivoCondicion("Simetrias", "Simetrias", .simetrias),
            cargar: { c in
                var s = c.simetrias
                if s.contieneDatos() { marcar(c.simetrias) }
            },
            temporal: temporal,
            aceptar: { c throws(ErrorDominio) in
                try CasosCondiciones.aplicarSimetrias(
                    filas: filas, aciertos: aciertos, en: &c.simetrias, numeroPartidos: sesion.numeroPartidos)
            },
            borrar: {
                filas = filas.map { _ in "" }
                aciertos = ""
                try? sesion.combinacion.modificarGrupo(sesion.grupoEnEdicion) { $0.condiciones.simetrias = CondicionSimetrias() }
            },
            marcar: { marcar($0.simetrias) }
        ) {
            Tarjeta("Simetrías") {
                Text("Introduce en cada fila los partidos que deben compartir el mismo signo.")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                LazyVGrid(columns: Array(repeating: GridItem(.flexible()), count: 3), alignment: .leading) {
                    ForEach(filas.indices, id: \.self) { i in
                        HStack {
                            Text("\(i + 1)").monospacedDigit().frame(width: 24, alignment: .trailing)
                            TextField("", text: $filas[i], prompt: Text("p. ej. 1,2-4")).textFieldStyle(.roundedBorder)
                        }
                    }
                }
                Button("Añadir fila") { filas.append("") }
            }
            Tarjeta("Aciertos") {
                Text("Número(s) de aciertos a considerar. Admite valores individuales y rangos separados por comas (p. ej. 1,3,5-7).")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                TextField("", text: $aciertos, prompt: Text("p. ej. 1,3,5-7")).textFieldStyle(.roundedBorder)
            }
        }
    }
}
