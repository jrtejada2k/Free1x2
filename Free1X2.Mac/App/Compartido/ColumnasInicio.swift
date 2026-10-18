import SwiftUI

/// «Columnas de Inicio» (DT-85): junto a cada «abrir fichero de columnas» del Reductor y de los filtros; desarrolla el
/// boleto de Inicio a un temporal y se lo pasa a `usar`, sin que el usuario guarde antes un fichero.
struct BotonColumnasInicio: View {
    @Environment(Sesion.self) private var sesion
    var titulo = "Columnas de Inicio"
    let usar: (URL) -> Void
    @State private var generando = false

    var body: some View {
        Button(generando ? "Generando columnas…" : titulo) {
            Task {
                generando = true
                defer { generando = false }
                if let u = await sesion.columnasDeInicio() { usar(u) }
            }
        }
        .disabled(generando)
        .help("Usar las columnas del boleto de Inicio, con sus condiciones, sin guardar antes un fichero")
        .accessibilityIdentifier("columnas-inicio")
    }
}
