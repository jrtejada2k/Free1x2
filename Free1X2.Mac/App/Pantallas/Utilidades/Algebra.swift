import Free1X2CasosDeUso
import SwiftUI

/// «Álgebra» (pantalla 3, `AlgebraColumnasFrm`): repetidas, unión, intersección y resta de ficheros de columnas.
struct VistaAlgebraColumnas: View {
    @Environment(Sesion.self) private var sesion
    @State private var c1: URL?
    @State private var c2: URL?
    @State private var salida: URL?
    @State private var signos = (0, 0)
    @State private var detalle = ("", "")
    @State private var tipo = AlgebraComb.eliminaRepetidas
    @State private var resultado = ""
    @State private var ocupado = false

    private func elegir(_ n: Int) async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        do throws(ErrorDominio) {
            let r = try CasosUtilidades.resumenColumnas(u, entorno: sesion.entorno)
            if n == 1 {
                (c1, signos.0, detalle.0) = (u, r.signos, "Combinación 1: \(r.columnas) columnas.")
            } else {
                (c2, signos.1, detalle.1) = (u, r.signos, "Combinación 2: \(r.columnas) columnas.")
            }
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    private func calcular() async {
        let soloUna = tipo == .eliminaRepetidas
        guard let c1, let salida, soloUna || c2 != nil else { return resultado = "No ha seleccionado los archivos" }
        guard soloUna ? signos.0 != 0 : signos.0 == signos.1 && signos.0 != 0 else {
            (self.c1, c2, self.salida) = (nil, nil, nil)
            return sesion.alertas.error("Los archivos tienen distinto número de partidos")
        }
        resultado = ""
        ocupado = true
        do throws(ErrorDominio) {
            resultado = try await CasosUtilidades.algebraDeColumnas(
                tipo, c1, c2, salida: salida, numeroPartidos: soloUna ? signos.0 : signos.1, entorno: sesion.entorno)
        } catch {
            resultado = "Error: " + error.mensajeUsuario
        }
        ocupado = false
    }

    private func fila(_ titulo: String, _ url: URL?, _ accion: @escaping () async -> Void) -> some View {
        HStack {
            Text(titulo).frame(width: 110, alignment: .leading)
            Button("Seleccionar…") { Task { await accion() } }
            Text(url?.deletingPathExtension().lastPathComponent ?? "(selecciona)").foregroundStyle(Color.textoSecundario)
        }
    }

    var body: some View {
        Pagina(destino: .algebraColumnas) {
            Text("Combina o resta archivos de columnas: elimina repetidas, suma, intersección o resta de combinaciones.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta("Combinaciones") {
                fila("Combinación 1", c1) { await elegir(1) }
                fila("Combinación 2", c2) { await elegir(2) }
                fila("Resultado", salida) {
                    if let u = await SelectorFicheros.guardar("ColumnasResultado", [("Columnas", ".txt")]) { salida = u }
                }
            }
            Tarjeta("Operaciones") {
                Picker("Tipo de operación", selection: $tipo) {
                    Text("Elimina columnas repetidas de Combinación 1").tag(AlgebraComb.eliminaRepetidas)
                    Text("Suma Combinaciones: elimina columnas repetidas").tag(AlgebraComb.sumaEliminaRepetidas)
                    Text("Suma Combinaciones: selecciona columnas comunes").tag(AlgebraComb.sumaSoloComunes)
                    Text("Resta combinaciones: (1)-(2)").tag(AlgebraComb.restaSegunda)
                }
                .fixedSize()
            }
            Tarjeta("Detalle") {
                Text(detalle.0)
                Text(detalle.1)
                Text(resultado).foregroundStyle(Color.exito)
            }
            HStack {
                // «Cancelar» no hace nada en el C# (`VM:205-209`).
                Button("Cancelar") {}
                Button("Calcular") { Task { await calcular() } }.buttonStyle(.borderedProminent).disabled(ocupado)
            }
        }
    }
}
