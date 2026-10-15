import Free1X2CasosDeUso
import SwiftUI

/// «Saca Formatos (Indeciso)» (pantalla 15, `CalculoFormatosFrm`).
struct VistaCalculoFormatos: View {
    @Environment(Sesion.self) private var sesion
    @State private var salida: URL?
    @State private var columna = ""
    @State private var informe = ""

    private func sacar() {
        guard let salida else { return sesion.alertas.error("Falta seleccionar archivo de salida") }
        if columna.utf16.count != 14 { sesion.alertas.error("numero de caracteres en la columna distinto de 14") }
        informe = CasosCondiciones.informeFormatos(columna)
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(Data(informe.utf8), en: salida, atomico: false)
            sesion.alertas.info("Ya")
        } catch {
            sesion.alertas.error("No se ha podido guardar el informe: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .calculoFormatos) {
            Text(
                "Extrae los formatos (pares, tríos, cuartetos y quintetos) y los contactos de una columna de 14 signos y genera un informe de texto."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Fichero de salida") {
                HStack {
                    Button("Seleccionar fichero…") {
                        Task { if let u = await SelectorFicheros.guardar("Informe", [("Informe", ".txt")]) { salida = u } }
                    }
                    Text(salida?.path(percentEncoded: false) ?? "Falta Fichero Salida").foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Columna") {
                HStack {
                    Text("Columna (14 signos: 1 / X / 2)")
                    // `MaxLength=14`; el port no fuerza mayúsculas.
                    TextField("Ej. 1X21X2111XX221", text: $columna).textFieldStyle(.roundedBorder).monospaced().frame(width: 180)
                        .onChange(of: columna) { _, v in if v.utf16.count > 14 { columna = String(v.prefix(14)) } }
                    Button("Sacar Formato", action: sacar).buttonStyle(.borderedProminent)
                }
            }
            Tarjeta("Informe") {
                ScrollView {
                    Text(informe.isEmpty ? "El informe generado aparecerá aquí." : informe).monospaced().textSelection(.enabled)
                        .foregroundStyle(informe.isEmpty ? Color.textoSecundario : Color.primary)
                        .frame(maxWidth: .infinity, alignment: .leading)
                }
                .frame(minHeight: 260)
            }
        }
    }
}

/// «Analizador Formatos123» (pantalla 5, `AnalisisFormatos123Frm`).
struct VistaAnalisisFormatos123: View {
    @Environment(Sesion.self) private var sesion
    @State private var formatos: [Formato123] = []
    @State private var columnas: [String] = []
    @State private var indice = 0
    @State private var valoracion: [[Double]] = []
    @State private var informe: [ConteoFormato] = []
    @State private var cargado = false

    private var actual: String { columnas.indices.contains(indice) ? columnas[indice] : "" }

    private func leer() async {
        guard let u = await SelectorFicheros.abrir(".txt", ".cols", "*") else { return }
        do throws(ErrorDominio) {
            columnas = try CasosCondiciones.columnasFormatos123(u, entorno: sesion.entorno)
            indice = 0
        } catch {
            sesion.alertas.error("Error leyendo columnas: " + error.mensajeUsuario)
        }
    }

    private func ejecutar(_ todos: Bool) {
        guard !columnas.isEmpty else { return }
        guard todos || !formatos.isEmpty else {
            return sesion.alertas.info(
                "No hay formatos predefinidos para analizar. Abra el análisis desde Formatos 123 o use \"Mostrar todos\".")
        }
        do throws(ErrorDominio) {
            informe =
                todos
                ? try CasosCondiciones.todosFormatos123(actual, valoracion: valoracion)
                : try CasosCondiciones.analizarFormatos123(actual, valoracion: valoracion, formatos: formatos)
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    var body: some View {
        Pagina(destino: .analisisFormatos123) {
            Text(
                "Traduce cada signo de una columna (1/X/2) a la posición que ocupa en la valoración del partido (1 más probable, 3 menos probable) y cuenta cuántas veces aparece cada formato sobre esa columna traducida."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                VStack(spacing: 10) {
                    Tarjeta("Columnas a analizar") {
                        Text("Carga un archivo de columnas y navega por ellas; cada columna se analiza por separado.")
                            .foregroundStyle(Color.textoSecundario)
                        Button("Archivo de columnas") { Task { await leer() } }
                        HStack {
                            Button("<") { indice -= 1 }.disabled(indice <= 0)
                            Text(columnas.isEmpty ? "0/0" : "\(indice + 1)/\(columnas.count)").monospacedDigit()
                            Button(">") { indice += 1 }.disabled(indice >= columnas.count - 1)
                        }
                        HStack {
                            Text("Columna:")
                            Text(actual).monospaced().textSelection(.enabled)
                        }
                    }
                    Tarjeta("Informe") {
                        HStack {
                            Button("Analizar") { ejecutar(false) }.buttonStyle(.borderedProminent)
                            Button("Mostrar todos") { ejecutar(true) }
                        }
                        Grid(alignment: .leading, horizontalSpacing: 20, verticalSpacing: 2) {
                            GridRow {
                                Text("Formato").bold()
                                Text("Apariciones").bold()
                            }
                            ForEach(informe.indices, id: \.self) { i in
                                GridRow {
                                    Text(informe[i].formato).monospaced()
                                    Text(String(informe[i].apariciones)).monospacedDigit()
                                }
                            }
                        }
                    }
                }
                Tarjeta("Valoración de los partidos") {
                    Text(
                        "Define la valoración 1/X/2 de cada partido. El orden de estos valores determina cómo se traduce cada signo a 1, 2 o 3 al construir el formato."
                    )
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                    PorcentajesGrid(valores: $valoracion)
                }
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            valoracion = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos)
            if let f = sesion.formatos123Entrada {
                sesion.formatos123Entrada = nil
                formatos = f
            }
        }
    }
}
