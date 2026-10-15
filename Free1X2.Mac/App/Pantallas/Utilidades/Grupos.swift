import Free1X2CasosDeUso
import SwiftUI

/// Rejilla de campos de texto (`TextBox` de las tablas de niveles).
private struct RejillaTexto: View {
    let cabeceras: [String]
    @Binding var filas: [[String]]
    var ancho: CGFloat = 34

    var body: some View {
        ScrollView(.horizontal) {
            Grid(alignment: .leading, horizontalSpacing: 4, verticalSpacing: 3) {
                GridRow {
                    Text("#").bold()
                    ForEach(cabeceras, id: \.self) { Text($0).font(.caption).bold() }
                }
                ForEach(filas.indices, id: \.self) { r in
                    GridRow {
                        Text(String(r + 1)).monospacedDigit()
                        ForEach(filas[r].indices, id: \.self) { c in
                            TextField("", text: $filas[r][c]).textFieldStyle(.roundedBorder).frame(width: ancho)
                        }
                    }
                }
            }
            .padding(.vertical, 4)
        }
    }
}

/// Contadores y tiempo de «Calcular».
private struct ProcesoGrupos: View {
    let procesadas: String
    let validas: String
    let tiempo: String

    var body: some View {
        HStack(spacing: 20) {
            LabeledContent("Procesadas", value: procesadas)
            LabeledContent("Válidas", value: validas)
            LabeledContent("Tiempo", value: tiempo)
        }
    }
}

/// «Pares» (pantalla 77, `ParejasFrm`).
struct VistaParejas: View {
    @Environment(Sesion.self) private var sesion
    @State private var niveles = Array(repeating: Array(repeating: "0", count: 11), count: 14)
    @State private var condiciones = Array(repeating: ["0", "10"], count: 7)
    @State private var ganadora = "111X11X222X111"
    @State private var recuento = Array(repeating: "-", count: 7)
    @State private var contadores = ("0", "0", "0")
    @State private var bits: ConjuntoBits?
    @State private var tarea: Task<Void, Never>?

    private var filtro: FiltroPorGrupos {
        FiltroPorGrupos(
            k: 2, filas: niveles.map { $0.map(CasosUtilidades.enteroNET) },
            limites: condiciones.map { (CasosUtilidades.enteroNET($0[0]), CasosUtilidades.enteroNET($0[1])) })
    }

    private func calcular() async {
        guard let url = await SelectorFicheros.abrir(".txt", "*") else { return }
        bits = nil
        contadores = ("0", "0", "0")
        let (f, t0) = (filtro, sesion.entorno.reloj.ahora())
        let t = Task {
            do throws(ErrorDominio) {
                let r = try await CasosUtilidades.filtrarPorGrupos(url, filtro: f, entorno: sesion.entorno)
                bits = r.bits
                contadores = (
                    String(r.procesadas), String(r.validas),
                    FormatoNET.toStringTimeSpan(sesion.entorno.reloj.ahora().timeIntervalSince(t0))
                )
            } catch {
                sesion.alertas.error("Error al procesar el fichero de columnas: " + error.mensajeUsuario)
            }
        }
        tarea = t
        await t.value
        tarea = nil
    }

    var body: some View {
        Pagina(destino: .parejas) {
            Text(
                "Filtra columnas según el resultado de parejas de partidos: cada combinación (11, 1x, 12, x1, xx, x2, 21, 2x, 22) recibe un nivel, y cada nivel exige un número de aciertos dentro de un rango."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                Tarjeta("Niveles") {
                    RejillaTexto(cabeceras: ["P1", "P2", "11", "1x", "12", "x1", "xx", "x2", "21", "2x", "22"], filas: $niveles)
                }
                VStack(spacing: 10) {
                    Tarjeta("Condiciones") {
                        Grid(alignment: .leading, horizontalSpacing: 6, verticalSpacing: 3) {
                            GridRow {
                                Text("niv").bold()
                                Text("min").bold()
                                Text("max").bold()
                            }
                            ForEach(0..<7, id: \.self) { n in
                                GridRow {
                                    Text(String(n + 1))
                                    TextField("", text: $condiciones[n][0]).textFieldStyle(.roundedBorder).frame(width: 44)
                                    TextField("", text: $condiciones[n][1]).textFieldStyle(.roundedBorder).frame(width: 44)
                                }
                            }
                        }
                    }
                    Tarjeta("Análisis Resultados") {
                        CampoTexto(etiqueta: "Columna Ganadora", texto: $ganadora, ancho: 150)
                        Button("Analizar") {
                            do throws(ErrorDominio) {
                                recuento = try filtro.valida(ganadora).recuento.map(String.init)
                            } catch {
                                sesion.alertas.inesperado(error)
                            }
                        }
                        HStack { ForEach(0..<7, id: \.self) { Text("\($0 + 1): " + recuento[$0]).monospacedDigit() } }
                    }
                }
            }
            ProcesoGrupos(procesadas: contadores.0, validas: contadores.1, tiempo: contadores.2)
            HStack {
                Button("Leer condiciones") {
                    Task {
                        guard let u = await SelectorFicheros.abrir(".par", "*") else { return }
                        do throws(ErrorDominio) {
                            CasosUtilidades.aplicarCondicionesPares(
                                try sesion.entorno.archivos.leer(u), filas: &niveles, limites: &condiciones)
                        } catch {
                            sesion.alertas.error("No se ha podido leer el fichero de condiciones: " + error.mensajeUsuario)
                        }
                    }
                }
                Button("Salvar condiciones") {
                    Task {
                        guard let u = await SelectorFicheros.guardar("Pares", [("Condiciones", ".par")]) else { return }
                        do throws(ErrorDominio) {
                            try sesion.entorno.archivos.escribir(
                                CasosUtilidades.datosCondicionesPares(filas: niveles, limites: condiciones), en: u, atomico: false)
                        } catch {
                            sesion.alertas.error("No se ha podido guardar el fichero de condiciones: " + error.mensajeUsuario)
                        }
                    }
                }
                Button("Grabar resultado") {
                    Task {
                        guard let bits, let u = await SelectorFicheros.guardar("Resultados", [("Resultados", ".txt")]) else { return }
                        do throws(ErrorDominio) {
                            try CasosUtilidades.grabarPorGrupos(bits, en: u, entorno: sesion.entorno)
                        } catch {
                            sesion.alertas.error("Error al grabar el resultado: " + error.mensajeUsuario)
                        }
                    }
                }
                .disabled(bits == nil || tarea != nil)
                Spacer()
                Button("Cancelar") { tarea?.cancel() }
                Button("Calcular") { Task { await calcular() } }.buttonStyle(.borderedProminent).disabled(tarea != nil)
            }
        }
    }
}

/// «Tríos» (pantalla 96, `TriosFrm`). «Analizar» usa la columna fija del VM: no tiene campo en la pantalla.
struct VistaTrios: View {
    @Environment(Sesion.self) private var sesion
    @State private var niveles = Array(repeating: Array(repeating: "0", count: 30), count: 12)
    @State private var limites = Array(repeating: [0.0, 270], count: 5)
    @State private var recuento = Array(repeating: "-", count: 5)
    @State private var contadores = ("0", "0", "0")
    @State private var bits = ConjuntoBits(cantidad: 4_782_969)
    @State private var tarea: Task<Void, Never>?

    private static let ganadora = "111X11X222X111"
    private static let combinaciones = ["1", "X", "2"].flatMap { a in ["1", "X", "2"].flatMap { b in ["1", "X", "2"].map { a + b + $0 } } }

    private var filtro: FiltroPorGrupos {
        FiltroPorGrupos(
            k: 3, filas: niveles.map { $0.map(CasosUtilidades.enteroNET) },
            limites: limites.map { (Int(FormatoNET.castInt32($0[0])), Int(FormatoNET.castInt32($0[1]))) })
    }

    private func calcular() async {
        guard let url = await SelectorFicheros.abrir(".txt", "*") else { return }
        contadores = ("0", "0", "0")
        let (f, t0) = (filtro, sesion.entorno.reloj.ahora())
        let t = Task {
            do throws(ErrorDominio) {
                let r = try await CasosUtilidades.filtrarPorGrupos(url, filtro: f, entorno: sesion.entorno)
                bits = r.bits
                contadores = (
                    String(r.procesadas), String(r.validas),
                    FormatoNET.toStringTimeSpan(sesion.entorno.reloj.ahora().timeIntervalSince(t0))
                )
            } catch {
                sesion.alertas.error("Error al procesar el fichero de columnas: " + error.mensajeUsuario)
            }
        }
        tarea = t
        await t.value
        tarea = nil
    }

    var body: some View {
        Pagina(destino: .trios) {
            Text(
                "Analiza un fichero de columnas de entrada validando cada columna contra una tabla de límites de cinco niveles. Cuenta las columnas procesadas y válidas, y muestra un resultado por nivel."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Límites") {
                Text("Mínimo y máximo de aciertos admitidos por nivel (campos legacy c11/c12 … c51/c52).").foregroundStyle(
                    Color.textoSecundario)
                Grid(alignment: .leading, horizontalSpacing: 10, verticalSpacing: 3) {
                    GridRow {
                        Text("Niv").bold()
                        Text("Mín").bold()
                        Text("Máx").bold()
                        Text("Resultado").bold()
                    }
                    ForEach(0..<5, id: \.self) { n in
                        GridRow {
                            Text(String(n + 1))
                            CampoNumero(etiqueta: "", valor: $limites[n][0], ancho: 60).labelsHidden()
                            CampoNumero(etiqueta: "", valor: $limites[n][1], ancho: 60).labelsHidden()
                            Text(recuento[n]).monospacedDigit()
                        }
                    }
                }
            }
            Tarjeta("Niveles") {
                Text(
                    "12 tríos: las 3 primeras columnas son los partidos que forman el trío (1-based); las 27 restantes asignan el nivel (1-5) a cada combinación de signos del trío."
                )
                .foregroundStyle(Color.textoSecundario)
                RejillaTexto(cabeceras: ["P1", "P2", "P3"] + Self.combinaciones, filas: $niveles, ancho: 30)
            }
            Tarjeta("Proceso") { ProcesoGrupos(procesadas: contadores.0, validas: contadores.1, tiempo: contadores.2) }
            HStack {
                Button("Calcular") { Task { await calcular() } }.buttonStyle(.borderedProminent).disabled(tarea != nil)
                Button("Analizar") {
                    do throws(ErrorDominio) {
                        recuento = try filtro.valida(Self.ganadora).recuento.map(String.init)
                    } catch {
                        sesion.alertas.inesperado(error)
                    }
                }
                Button("Grabar Resultado") {
                    Task {
                        guard let u = await SelectorFicheros.guardar("Resultados", [("Resultados", ".txt")]) else { return }
                        do throws(ErrorDominio) {
                            try CasosUtilidades.grabarPorGrupos(bits, en: u, entorno: sesion.entorno)
                        } catch {
                            sesion.alertas.error("Error al grabar el resultado: " + error.mensajeUsuario)
                        }
                    }
                }
                Button("Abrir Condiciones") {
                    Task {
                        guard let u = await SelectorFicheros.abrir(".tri", "*") else { return }
                        do throws(ErrorDominio) {
                            CasosUtilidades.aplicarCondicionesTrios(
                                try sesion.entorno.archivos.leer(u), filas: &niveles, limites: &limites, cultura: sesion.entorno.cultura)
                        } catch {
                            sesion.alertas.error("No se ha podido leer el fichero de condiciones: " + error.mensajeUsuario)
                        }
                    }
                }
                Button("Salvar Condiciones") {
                    Task {
                        guard let u = await SelectorFicheros.guardar("Trios", [("Condiciones", ".tri")]) else { return }
                        do throws(ErrorDominio) {
                            try sesion.entorno.archivos.escribir(
                                CasosUtilidades.datosCondicionesTrios(filas: niveles, limites: limites), en: u, atomico: false)
                        } catch {
                            sesion.alertas.error("No se ha podido guardar el fichero de condiciones: " + error.mensajeUsuario)
                        }
                    }
                }
                Button("Cancelar") { tarea?.cancel() }
            }
        }
    }
}
