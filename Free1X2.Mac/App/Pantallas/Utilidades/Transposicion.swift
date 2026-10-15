import Free1X2CasosDeUso
import SwiftUI

/// «Transposición de columnas» (pantalla 95, `TransposicionFrm`): permuta los 14 signos de cada columna.
struct VistaTransposicion: View {
    @Environment(Sesion.self) private var sesion
    @State private var entrada: URL?
    @State private var salida: URL?
    @State private var posiciones = (1...14).map(Double.init)
    @State private var estado = ""
    @State private var ocupado = false

    private func transponer() async {
        guard let orden = CasosUtilidades.ordenTransposicion(posiciones) else {
            estado = "Error en condiciones"
            return sesion.alertas.error("Error en condiciones")
        }
        guard let entrada, let salida else { return estado = "Seleccione los archivos de entrada y salida." }
        ocupado = true
        estado = "Procesando..."
        do throws(ErrorDominio) {
            try await CasosUtilidades.transponerColumnas(entrada, salida: salida, orden: orden, entorno: sesion.entorno)
            estado = "Transposición completada."
        } catch {
            estado = "Error: " + error.mensajeUsuario
        }
        ocupado = false
    }

    var body: some View {
        Pagina(destino: .transposicion) {
            Text(
                "Reordena los 14 signos de cada columna del archivo de entrada. Para cada posición de salida indica de qué posición de la entrada (1 a 14) se toma el signo. Cada valor 1-14 debe usarse exactamente una vez (es una permutación)."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Archivos") {
                HStack {
                    Text("Entrada").frame(width: 60, alignment: .leading)
                    Button("Seleccionar…") { Task { if let u = await SelectorFicheros.abrir(".txt") { entrada = u } } }
                    Text(entrada?.lastPathComponent ?? "(selecciona)").foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    Text("Salida").frame(width: 60, alignment: .leading)
                    Button("Seleccionar…") {
                        Task { if let u = await SelectorFicheros.guardar("ColumnasTranspuestas", [("Columnas", ".txt")]) { salida = u } }
                    }
                    Text(salida?.lastPathComponent ?? "(selecciona)").foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Orden de transposición") {
                Text("Salida ← Entrada: el signo de cada posición de salida (1-14) se toma de la posición de la entrada indicada.")
                    .foregroundStyle(Color.textoSecundario)
                Grid(alignment: .leading, horizontalSpacing: 16, verticalSpacing: 6) {
                    ForEach(0..<2, id: \.self) { f in
                        GridRow {
                            ForEach(0..<7, id: \.self) { c in
                                CampoNumero(
                                    etiqueta: "Salida \(f * 7 + c + 1)", valor: $posiciones[f * 7 + c], minimo: 1, maximo: 14, ancho: 44)
                            }
                        }
                    }
                }
            }
            Tarjeta("Estado") { Text(estado) }
            HStack {
                // «Cancelar» no hace nada en el C# (`VM:186-191`).
                Button("Cancelar") {}
                Button("Transponer") { Task { await transponer() } }.buttonStyle(.borderedProminent).disabled(ocupado)
            }
        }
    }
}
