import Free1X2CasosDeUso
import SwiftUI

/// «Rotación de signos» (pantalla 85, `RotacionDeSignosFrm`).
struct VistaRotacionDeSignos: View {
    struct Fila {
        var base = "1"
        var cambios = ["1", "X", "2"]
    }

    @Environment(Sesion.self) private var sesion
    @State private var entrada: URL?
    @State private var salida: URL?
    @State private var partidos = 0
    @State private var filas: [Fila] = []
    @State private var correccion = false
    @State private var estado = "Faltan datos"
    @State private var ocupado = false

    private static let signos = ["1", "X", "2"]

    private func actualizar() { estado = entrada != nil && salida != nil ? "Preparado" : "Faltan datos" }

    private func seleccionarEntrada() async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        entrada = u
        do throws(ErrorDominio) {
            let r = try CasosUtilidades.entradaRotacion(u, numeroPartidos: sesion.numeroPartidos, entorno: sesion.entorno)
            partidos = r.partidos
            filas = Array(repeating: Fila(), count: r.partidos)
            for (i, s) in Array((r.primera ?? "").utf16).prefix(filas.count).enumerated() {
                filas[i].base = String(decoding: [s], as: UTF16.self)
            }
        } catch {
            sesion.alertas.error("Error al leer el archivo: " + error.mensajeUsuario)
        }
        actualizar()
    }

    private func aceptar() async {
        guard let entrada, let salida else { return estado = "Faltan datos" }
        let nuevos = filas.prefix(partidos).map { $0.cambios.map(CasosUtilidades.indiceSigno) }
        ocupado = true
        estado = "Procesando..."
        do throws(ErrorDominio) {
            let n = try await CasosUtilidades.rotarSignos(
                entrada, salida: salida, nuevos: nuevos, correccion: correccion, numeroPartidos: sesion.numeroPartidos,
                entorno: sesion.entorno)
            estado = "Se han grabado \(n) columnas"
        } catch {
            estado = "Error: " + error.mensajeUsuario
        }
        ocupado = false
        // DEC-1012: el `finally` del C# reescribe el estado con «Preparado»/«Faltan datos».
        actualizar()
    }

    private func selector(_ valor: Binding<String>) -> some View {
        Picker("", selection: valor) { ForEach(Self.signos, id: \.self) { Text($0).tag($0) } }.labelsHidden().frame(width: 60)
    }

    var body: some View {
        Pagina(destino: .rotacionDeSignos) {
            Tarjeta("Ficheros") {
                HStack {
                    Text("Entrada").frame(width: 60, alignment: .leading)
                    Button("Examinar…") { Task { await seleccionarEntrada() } }
                    Text(entrada?.lastPathComponent ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    Text("Salida").frame(width: 60, alignment: .leading)
                    Button("Examinar…") {
                        Task {
                            if let u = await SelectorFicheros.guardar("ColumnasRotadas", [("Columnas", ".txt")]) { salida = u }
                            actualizar()
                        }
                    }
                    Text(salida?.lastPathComponent ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Cambios de signo") {
                Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 4) {
                    GridRow {
                        Text("Nº").bold()
                        Text("Base").bold()
                        Text("1 →").bold()
                        Text("X →").bold()
                        Text("2 →").bold()
                    }
                    ForEach(filas.indices, id: \.self) { i in
                        GridRow {
                            Text(String(i + 1)).monospacedDigit()
                            selector($filas[i].base)
                            ForEach(0..<3, id: \.self) { s in selector($filas[i].cambios[s]) }
                        }
                    }
                }
                HStack {
                    Toggle("Corrección de fallos", isOn: $correccion).toggleStyle(.switch)
                    Button("Transponer") {
                        for i in filas.indices {
                            filas[i].cambios = filas[i].cambios.map { $0 == "1" ? "X" : $0 == "X" ? "2" : "1" }
                        }
                    }
                }
                Text(
                    "Nota: las valoraciones de JORNADA y COMBINACIÓN (ControlPorcentajes legacy) se portarán como control propio en una fase posterior."
                )
                .font(.callout).foregroundStyle(Color.textoSecundario)
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                // «Cancelar» no hace nada en el C# (`VM:206-210`).
                Button("Cancelar") {}
                Button("Aceptar") { Task { await aceptar() } }.buttonStyle(EstiloPrincipal())
                    .disabled(ocupado || entrada == nil || salida == nil)
            }
        }
        .onAppear {
            if filas.isEmpty {
                partidos = sesion.numeroPartidos
                filas = Array(repeating: Fila(), count: partidos)
            }
        }
    }
}
