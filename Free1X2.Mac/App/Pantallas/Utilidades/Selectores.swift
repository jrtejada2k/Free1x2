import Free1X2CasosDeUso
import SwiftUI

/// Ganadoras de los dos selectores: lista, posición (1-based) y columna mostrada.
struct GanadorasSelector {
    var columnas: [String] = []
    var indice = 0
    var columna = ""
    var fichero = "Fichero Ganadoras"

    mutating func mover(_ paso: Int) {
        let n = indice + paso
        guard n >= 1 && n <= columnas.count else { return }
        indice = n
        columna = columnas[n - 1]
    }

    /// Tras cargar queda seleccionada la última.
    mutating func cargar(_ c: [String], fichero f: URL) {
        columnas = c
        guard !c.isEmpty else { return }
        (indice, columna, fichero) = (c.count, c[c.count - 1], f.lastPathComponent)
    }
}

/// Panel «Cargar ganadoras / Columna / − + / Analizar» común.
struct PanelGanadoras: View {
    @Binding var g: GanadorasSelector
    let cargar: () async -> Void
    let analizar: () -> Void

    var body: some View {
        Tarjeta("Columnas ganadoras") {
            HStack {
                Button("Cargar ganadoras…") { Task { await cargar() } }
                Text(g.fichero).foregroundStyle(Color.textoSecundario)
            }
            HStack {
                CampoTexto(etiqueta: "Columna", texto: $g.columna, ancho: 160)
                    .onChange(of: g.columna) { g.columna = String(g.columna.prefix(14)) }
                Button("−") { g.mover(-1) }
                Text(String(g.indice)).monospacedDigit()
                Button("+") { g.mover(1) }
                Button("Analizar", action: analizar).disabled(g.columnas.isEmpty)
            }
        }
    }
}

/// «Selección 5+5+4 de JuanM» (pantalla 86, `SelecJM`).
struct VistaSelecJM: View {
    @Environment(Sesion.self) private var sesion
    @State private var valores: [[Double]] = []
    @State private var grupos = ["1", "1", "1", "1", "1", "2", "2", "2", "2", "2", "3", "3", "3", "3"]
    @State private var limites = ["1-243", "1-243", "1-81", "1-567"]
    @State private var porCorte = true
    @State private var ficheroCondiciones = "(ninguno)"
    @State private var porcentajes = ["%1", "%2", "%3"]
    @State private var procesadas = "Procesadas"
    @State private var validas = "Válidas"
    @State private var tiempo = "Tiempo"
    @State private var ficheroSalida = "(ninguno)"
    @State private var bits = ConjuntoBits(cantidad: 4_782_969)
    @State private var ganadoras = GanadorasSelector()
    @State private var rangos = ["-", "-", "-", "-"]
    @State private var tarea: Task<Void, Never>?

    private func tablas() -> TablasJM? {
        do throws(ErrorDominio) {
            return try CasosUtilidades.tablasJM(valoraciones: valores, grupos: grupos, limites: limites, cultura: sesion.entorno.cultura)
        } catch {
            sesion.alertas.error(error.mensajeUsuario)
            return nil
        }
    }

    private func calcular() async {
        (procesadas, validas, tiempo, ficheroSalida) = (" ", " ", " ", " ")
        let inicio = Date()
        guard let t = tablas() else { return }
        let r = await CasosUtilidades.seleccionJM(t, porCorte: porCorte)
        bits = r.bits
        porcentajes = t.porcentajes
        (validas, procesadas) = (String(r.validas), String(r.procesadas))
        tiempo = CasosUtilidades.tiempoCorto(Date().timeIntervalSince(inicio))
    }

    private func leerCondiciones() async {
        guard let u = await SelectorFicheros.abrir(".cnd", ".txt") else { return }
        do throws(ErrorDominio) {
            guard let c = CasosUtilidades.leerCondicionesJM(try sesion.entorno.archivos.leer(u)) else {
                return sesion.alertas.error("fichero de condiciones erróneo")
            }
            (grupos, limites, porCorte, ficheroCondiciones) = (c.grupos, c.limites, c.porCorte, u.lastPathComponent)
        } catch {
            sesion.alertas.error(error.mensajeUsuario)
        }
    }

    private func salvarCondiciones() async {
        guard let u = await SelectorFicheros.guardar("Condiciones", [("F.Condiciones", ".cnd")]) else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(
                CasosUtilidades.datosCondicionesJM(grupos: grupos, limites: limites, porCorte: porCorte), en: u, atomico: false)
            ficheroCondiciones = u.lastPathComponent
        } catch {
            sesion.alertas.error(error.mensajeUsuario)
        }
    }

    private func cargarGanadoras() async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        ganadoras.columnas = []
        do throws(ErrorDominio) {
            let r = try CasosUtilidades.ganadorasSelector(u, entorno: sesion.entorno)
            if r.erronea != nil { sesion.alertas.error("col.G. errónea=\(r.columnas.count + 1)") }
            ganadoras.cargar(r.columnas, fichero: u)
        } catch {
            sesion.alertas.error(error.mensajeUsuario)
        }
    }

    private func analizar() {
        let c = FormatoNET.mayusculas(ganadoras.columna)
        guard c.utf16.count >= 14 else { return sesion.alertas.error("col.G. errónea") }
        guard let t = tablas() else { return }
        porcentajes = t.porcentajes
        rangos = CasosUtilidades.ubicarGanadoraJM(t, columna: c, porCorte: porCorte).map(String.init)
    }

    var body: some View {
        Pagina(destino: .selecJM) {
            HStack(alignment: .top) {
                Tarjeta("Valoraciones") { PorcentajesGrid(valores: $valores) }
                VStack(alignment: .leading) {
                    Tarjeta("Grupos") {
                        LazyVGrid(columns: Array(repeating: GridItem(.fixed(60)), count: 7), alignment: .leading) {
                            ForEach(0..<14, id: \.self) { i in
                                VStack(spacing: 2) {
                                    Text("P\(i + 1)").font(.caption).foregroundStyle(Color.textoSecundario)
                                    TextField("", text: $grupos[i]).frame(width: 30)
                                        .onChange(of: grupos[i]) { grupos[i] = String(grupos[i].prefix(1)) }
                                }
                            }
                        }
                    }
                    Tarjeta("Límites") {
                        ForEach(0..<4, id: \.self) { i in
                            CampoTexto(
                                etiqueta: ["Grupo 1 (1-243)", "Grupo 2 (1-243)", "Grupo 3 (1-81)", "Total (1-567)"][i], texto: $limites[i],
                                ancho: 160)
                        }
                        HStack(spacing: 16) { ForEach(0..<3, id: \.self) { Text(porcentajes[$0]).monospacedDigit() } }
                    }
                }
            }
            Tarjeta("Proceso") {
                HStack {
                    Picker("Examinar", selection: $porCorte) {
                        Text("Por corte").tag(true)
                        Text("Por valor").tag(false)
                    }
                    .frame(width: 220)
                    Button("Leer condiciones") { Task { await leerCondiciones() } }
                    Button("Salvar condiciones") { Task { await salvarCondiciones() } }
                    Text(ficheroCondiciones).foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    Button("Calcular") {
                        tarea = Task {
                            await calcular()
                            tarea = nil
                        }
                    }
                    .buttonStyle(.borderedProminent)
                    Button("Cancelar") { tarea?.cancel() }
                    Button("Grabar columnas") {
                        Task {
                            guard let u = await SelectorFicheros.guardar("ResultadosJM", [("Resultados", ".txt")]) else { return }
                            do throws(ErrorDominio) {
                                try CasosUtilidades.grabarSeleccion(bits, en: u, entorno: sesion.entorno)
                                ficheroSalida = u.lastPathComponent
                            } catch {
                                sesion.alertas.error(error.mensajeUsuario)
                            }
                        }
                    }
                }
                HStack(spacing: 24) {
                    Text(procesadas)
                    Text(validas)
                    Text(tiempo)
                    Text(ficheroSalida)
                }
                .monospacedDigit().foregroundStyle(Color.textoSecundario)
            }
            PanelGanadoras(g: $ganadoras, cargar: cargarGanadoras, analizar: analizar)
            HStack(spacing: 24) {
                Text("Rango gr.1: " + rangos[0])
                Text("Rango gr.2: " + rangos[1])
                Text("Rango gr.3: " + rangos[2])
                Text("Rango total: " + rangos[3])
            }
            .monospacedDigit()
        }
        .onAppear { if valores.isEmpty { valores = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos) } }
    }
}

/// «Selector MarioSan» (pantalla 87, `SelectorMS`).
struct VistaSelectorMS: View {
    struct Fila: Identifiable {
        let id: Int
        let c: Int
        let p: [Int]
    }

    @Environment(Sesion.self) private var sesion
    @State private var aspecto = 13
    @State private var fichero = ""
    @State private var distribucion: DistribucionMS?
    @State private var resultados: [[Int]] = []
    @State private var seleccion: Int?
    @State private var tiempo = " "
    @State private var suma = "0"
    @State private var ficheroSalida = ""
    @State private var ganadoras = GanadorasSelector()
    @State private var tarea: Task<Void, Never>?

    private var filas: [Fila] {
        guard let d = distribucion else { return [] }
        let conteo = d.conteo
        return (0..<d.filas).map { q in Fila(id: q, c: conteo[q], p: q < resultados.count ? resultados[q] : [0, 0, 0, 0, 0]) }
    }

    private func iniciar() async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        fichero = u.lastPathComponent
        tiempo = " "
        let inicio = Date()
        do throws(ErrorDominio) {
            if let d = try await CasosUtilidades.distribucionMS(u, limite: aspecto, entorno: sesion.entorno) {
                distribucion = d
                resultados = Array(repeating: [0, 0, 0, 0, 0], count: d.filas)
            }
        } catch {
            sesion.alertas.error("Error: " + error.mensajeUsuario)
        }
        tiempo = FormatoNET.toStringTimeSpan(Date().timeIntervalSince(inicio))
    }

    private func analizar() {
        guard let d = distribucion, !d.columnas.isEmpty, ganadoras.columna.utf16.count >= 14 else { return }
        do throws(ErrorDominio) {
            resultados = try CasosUtilidades.analizarMS(d, ganadora: ganadoras.columna)
        } catch {
            sesion.alertas.error(error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .selectorMS) {
            Tarjeta("Cálculo") {
                HStack {
                    Picker("Aspecto", selection: $aspecto) { ForEach([11, 12, 13], id: \.self) { Text(String($0)).tag($0) } }
                        .frame(width: 140)
                    Button("−") { if aspecto > 11 { aspecto -= 1 } }
                    Button("+") { if aspecto < 13 { aspecto += 1 } }
                    Button("Iniciar") {
                        tarea = Task {
                            await iniciar()
                            tarea = nil
                        }
                    }
                    .buttonStyle(.borderedProminent)
                    Button("Cancelar") { tarea?.cancel() }
                }
                HStack(spacing: 24) {
                    Text("Fichero de entrada: " + fichero)
                    Text("Columnas leídas: \(distribucion?.columnas.count ?? 0)")
                    Text("Tiempo: " + tiempo)
                }
                .monospacedDigit().foregroundStyle(Color.textoSecundario)
            }
            Tarjeta("Distribución") {
                Table(filas, selection: $seleccion) {
                    TableColumn("Q") { Text(String($0.id)).monospacedDigit() }
                    TableColumn("cols.") { Text(String($0.c)).monospacedDigit() }
                    TableColumn("14") { Text(String($0.p[0])).monospacedDigit() }
                    TableColumn("13") { Text(String($0.p[1])).monospacedDigit() }
                    TableColumn("12") { Text(String($0.p[2])).monospacedDigit() }
                    TableColumn("11") { Text(String($0.p[3])).monospacedDigit() }
                    TableColumn("10") { Text(String($0.p[4])).monospacedDigit() }
                }
                .frame(minHeight: 240)
                HStack {
                    Button("Sumar") { suma = seleccion.flatMap { q in filas.first { $0.id == q }.map { String($0.c) } } ?? "0" }
                    Text("Suma de selección: " + suma).monospacedDigit()
                    Spacer()
                    Button("Grabar") {
                        Task {
                            guard let d = distribucion, !d.columnas.isEmpty, let q = seleccion,
                                let u = await SelectorFicheros.guardar("SeleccionMS", [("Columnas", ".txt")])
                            else { return }
                            do throws(ErrorDominio) {
                                try CasosUtilidades.grabarMS(d, grupo: q, en: u, entorno: sesion.entorno)
                                ficheroSalida = u.lastPathComponent
                            } catch {
                                sesion.alertas.error("Error al grabar: " + error.mensajeUsuario)
                            }
                        }
                    }
                    Text(ficheroSalida).foregroundStyle(Color.textoSecundario)
                }
            }
            PanelGanadoras(
                g: $ganadoras,
                cargar: {
                    guard let u = await SelectorFicheros.abrir(".txt") else { return }
                    do throws(ErrorDominio) {
                        let r = try CasosUtilidades.ganadorasSelector(u, entorno: sesion.entorno)
                        if let e = r.erronea {
                            // Como el C#: avisa y deja las leídas sin actualizar el índice.
                            ganadoras.columnas = r.columnas
                            return sesion.alertas.error("col.G. errónea=" + e)
                        }
                        ganadoras.cargar(r.columnas, fichero: u)
                    } catch {
                        ganadoras.columnas = []
                        sesion.alertas.error(error.mensajeUsuario)
                    }
                }, analizar: analizar)
        }
    }
}
