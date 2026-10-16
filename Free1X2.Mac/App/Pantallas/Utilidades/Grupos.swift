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
                Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).disabled(tarea != nil)
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
                Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).disabled(tarea != nil)
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

/// `TimeSpan.ToString() + "0…0"` recortado (`FormatearTiempo`).
private func tiempoRecortado(_ desde: Date, _ sesion: Sesion, _ largo: Int = 10) -> String {
    String((FormatoNET.toStringTimeSpan(sesion.entorno.reloj.ahora().timeIntervalSince(desde)) + "00000000000").prefix(largo))
}

/// «Análisis de grupos en combinación» (pantalla 4, `AnaCombi`).
struct VistaAnaCombi: View {
    @Environment(Sesion.self) private var sesion
    @State private var fichero: URL?
    @State private var seleccion = Array(repeating: "-", count: 14)
    @State private var fallos = 0.0
    @State private var filas: [FilaAnaCombi] = []
    @State private var marcados: Set<String> = []
    @State private var procesadas = "0"
    @State private var tiempo = "0"
    @State private var tarea: Task<Void, Never>?

    private func calcular() async {
        guard tarea == nil else { return }
        guard !CasosUtilidades.grupoAnaCombi(seleccion).isEmpty else { return sesion.alertas.error("Falta grupo") }
        guard let url = fichero, sesion.entorno.archivos.existe(url) else { return sesion.alertas.error("Falta fichero") }
        (filas, marcados, procesadas, tiempo) = ([], [], "0", "0")
        let (sel, f, t0) = (seleccion, Int(FormatoNET.castInt32(fallos)), sesion.entorno.reloj.ahora())
        let t = Task {
            do throws(ErrorDominio) {
                let r = try await CasosUtilidades.analizarGrupos(url, seleccion: sel, fallos: f, entorno: sesion.entorno)
                // ponytail: «Procesadas» y «Tiempo» solo al terminar (el C# los refresca cada 1024 líneas).
                (filas, procesadas, tiempo) = (r.filas, String(r.procesadas), tiempoRecortado(t0, sesion))
            } catch {
                sesion.alertas.error("Error al calcular: " + error.mensajeUsuario)
            }
        }
        tarea = t
        await t.value
        tarea = nil
    }

    private func grabar() async {
        guard !filas.isEmpty else { return }
        guard let url = fichero, sesion.entorno.archivos.existe(url) else { return sesion.alertas.error("Falta fichero") }
        guard !marcados.isEmpty else { return sesion.alertas.info("No hay ningún grupo marcado para grabar.") }
        guard let s = await SelectorFicheros.guardar("Resultados", [("Resultados", ".txt")]) else { return }
        do throws(ErrorDominio) {
            try CasosUtilidades.grabarGrupos(url, grupos: marcados, seleccion: seleccion, en: s, entorno: sesion.entorno)
            sesion.alertas.info("Resultados grabados en " + s.lastPathComponent)
        } catch {
            sesion.alertas.error("No se pudieron grabar los resultados: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .anaCombi) {
            Text("Define un grupo de partidos, carga un fichero de columnas y cuenta cuántas encajan en cada combinación.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta("Fichero de entrada") {
                HStack {
                    Button("Seleccionar fichero…") { Task { if let u = await SelectorFicheros.abrir(".txt", "*") { fichero = u } } }
                    Text(fichero?.lastPathComponent ?? "Fichero a procesar").foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Patrón del grupo (14 partidos)") {
                Text("Por partido: 1 / X / 2 fija el signo, G lo añade al grupo, - lo ignora.").foregroundStyle(Color.textoSecundario)
                Grid(alignment: .leading, horizontalSpacing: 8, verticalSpacing: 4) {
                    ForEach(0..<2, id: \.self) { f in
                        GridRow {
                            ForEach(0..<7, id: \.self) { c in
                                let i = f * 7 + c
                                Picker(String(i + 1), selection: $seleccion[i]) {
                                    ForEach(["-", "1", "X", "2", "G"], id: \.self) { Text($0).tag($0) }
                                }
                                .frame(width: 90)
                            }
                        }
                    }
                }
                HStack {
                    CampoNumero(etiqueta: "Fallos admitidos", valor: $fallos, minimo: 0, ancho: 60)
                    Button("Limpiar patrón") { seleccion = Array(repeating: "-", count: 14) }
                }
            }
            HStack {
                Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).disabled(tarea != nil)
                Button("Cancelar") { tarea?.cancel() }
                LabeledContent("Procesadas", value: procesadas)
                LabeledContent("Tiempo", value: tiempo)
            }
            Tarjeta("Resultados") {
                List(filas, id: \.grupo) { f in
                    HStack {
                        Toggle(
                            "",
                            isOn: Binding(
                                get: { marcados.contains(f.grupo) },
                                set: { if $0 { marcados.insert(f.grupo) } else { marcados.remove(f.grupo) } })
                        )
                        .labelsHidden()
                        Text(f.grupo).monospaced().frame(width: 160, alignment: .leading)
                        Text(String(f.columnas)).monospacedDigit()
                    }
                }
                .frame(minHeight: 240)
                Button("Grabar resultados") { Task { await grabar() } }.disabled(filas.isEmpty)
            }
        }
    }
}

/// «Sumas Pares Naturales (JPM)» (pantalla 6, `AnalizadorJPM`).
struct VistaAnalizadorJPM: View {
    @Environment(Sesion.self) private var sesion
    @State private var valores: [Double] = [0, 1, 2, 1, 3, 4, 2, 4, 5]
    @State private var marcadas = Array(repeating: false, count: 36)
    @State private var clasificacion: ClasificacionJPM?
    @State private var entrada = "-"
    @State private var salida = "-"
    @State private var tiempo = "-"
    @State private var suma = ""
    @State private var ganadoras: [String] = []
    @State private var fichGanadoras = "-"
    @State private var indice = 0
    @State private var ganadora = "COL.GANADORA"
    @State private var puntuacion = "-"
    @State private var premios = Array(repeating: "0", count: 5)
    @State private var tarea: Task<Void, Never>?

    private static let pares = ["11", "1X", "12", "X1", "XX", "X2", "21", "2X", "22"]

    private func iniciar() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        entrada = u.lastPathComponent
        let v: [Int]
        do throws(ErrorDominio) { v = try CasosUtilidades.valoresJPM(valores) } catch { return sesion.alertas.inesperado(error) }
        let t0 = sesion.entorno.reloj.ahora()
        let t = Task {
            do throws(ErrorDominio) {
                let c = try await CasosUtilidades.clasificarJPM(u, valores: v, entorno: sesion.entorno)
                if let corta = c.columnaCorta { sesion.alertas.error("error de longitud=" + corta) }
                clasificacion = c
                tiempo = tiempoRecortado(t0, sesion, 11)
            } catch {
                sesion.alertas.error("Error al procesar el fichero: " + error.mensajeUsuario)
            }
        }
        tarea = t
        await t.value
        tarea = nil
    }

    private func grabar() async {
        guard let c = clasificacion else { return sesion.alertas.error("Inicia primero el proceso de columnas.") }
        guard let u = await SelectorFicheros.guardar("ColumnasSalida", [("ColumnasSalida", ".txt")]) else { return }
        do throws(ErrorDominio) {
            try CasosUtilidades.grabarJPM(c.validas, marcadas: marcadas, en: u, entorno: sesion.entorno)
            salida = u.lastPathComponent
        } catch {
            sesion.alertas.error("Error al grabar: " + error.mensajeUsuario)
        }
    }

    private func cargarGanadoras() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        do throws(ErrorDominio) {
            guard let g = try CasosUtilidades.ganadorasJPM(u, entorno: sesion.entorno) else {
                return sesion.alertas.error("col.G. errónea")
            }
            (ganadoras, indice, fichGanadoras) = (g, g.count, u.lastPathComponent)
            if let ultima = g.last { ganadora = ultima }
        } catch {
            sesion.alertas.error("Error al leer ganadoras: " + error.mensajeUsuario)
        }
    }

    private func analizar() async {
        let v: [Int]
        do throws(ErrorDominio) { v = try CasosUtilidades.valoresJPM(valores) } catch { return sesion.alertas.inesperado(error) }
        let col = FormatoNET.mayusculas(FormatoNET.recortar(ganadora))
        guard col.utf16.count >= 14 else { return sesion.alertas.error("error de longitud=" + col) }
        let m = marcadas
        let t = Task {
            do throws(ErrorDominio) {
                let r = try await CasosUtilidades.premiosJPM(col, valores: v, marcadas: m)
                (puntuacion, premios) = (String(r.puntuacion), r.premios.map(String.init))
            } catch {
                sesion.alertas.error("Error al analizar: " + error.mensajeUsuario)
            }
        }
        tarea = t
        await t.value
        tarea = nil
    }

    var body: some View {
        Pagina(destino: .analizadorJPM) {
            Text(
                "Puntúa cada par de signos, clasifica las columnas de un fichero en 36 casillas de suma y analiza los premios de la columna ganadora."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                VStack(spacing: 10) {
                    Tarjeta("Valores a usar (máx. 5)") {
                        Text("Puntos por cada par de signos consecutivos.").foregroundStyle(Color.textoSecundario)
                        Grid(alignment: .leading, horizontalSpacing: 8, verticalSpacing: 4) {
                            ForEach(0..<3, id: \.self) { f in
                                GridRow {
                                    ForEach(0..<3, id: \.self) { c in
                                        CampoNumero(
                                            etiqueta: Self.pares[f * 3 + c], valor: $valores[f * 3 + c], minimo: 0, maximo: 5, ancho: 44)
                                    }
                                }
                            }
                        }
                    }
                    Tarjeta("Proceso de columnas") {
                        HStack {
                            Button("Iniciar") { Task { await iniciar() } }.buttonStyle(EstiloPrincipal()).disabled(tarea != nil)
                            Button("Grabar") { Task { await grabar() } }
                            Button("Cancelar") { tarea?.cancel() }
                        }
                        LabeledContent("Fichero entrada:", value: entrada)
                        LabeledContent("Fichero salida:", value: salida)
                        LabeledContent("Total columnas:", value: String(clasificacion?.total ?? 0))
                        LabeledContent("Tiempo:", value: tiempo)
                    }
                    Tarjeta("Análisis de resultados") {
                        HStack {
                            Button("Cargar ganadoras") { Task { await cargarGanadoras() } }
                            Button("-") {
                                if indice > 1 {
                                    indice -= 1
                                    ganadora = ganadoras[indice - 1]
                                }
                            }
                            Button("+") {
                                if indice < ganadoras.count {
                                    indice += 1
                                    ganadora = ganadoras[indice - 1]
                                }
                            }
                        }
                        LabeledContent("Fichero ganadoras:", value: fichGanadoras)
                        LabeledContent("Índice:", value: String(indice))
                        CampoTexto(etiqueta: "Columna ganadora", texto: $ganadora, ancho: 160)
                        Button("Analizar") { Task { await analizar() } }.disabled(tarea != nil)
                        LabeledContent("Puntuación:", value: puntuacion)
                        ForEach(0..<5, id: \.self) { LabeledContent("\(14 - $0) aciertos:", value: premios[$0]) }
                    }
                }
                Tarjeta("Casillas de suma (0..35)") {
                    Text("Marca las casillas a grabar o sumar. El conteo se rellena tras iniciar.").foregroundStyle(Color.textoSecundario)
                    Grid(alignment: .leading, horizontalSpacing: 14, verticalSpacing: 2) {
                        ForEach(0..<12, id: \.self) { f in
                            GridRow {
                                ForEach(0..<3, id: \.self) { c in
                                    let i = c * 12 + f
                                    Toggle(isOn: $marcadas[i]) {
                                        Text("\(i): " + (clasificacion.map { String($0.casillas[i]) } ?? "0")).monospacedDigit()
                                    }
                                }
                            }
                        }
                    }
                    HStack {
                        Button("Sumar") {
                            suma = String(marcadas.indices.filter { marcadas[$0] }.reduce(0) { $0 + (clasificacion?.casillas[$1] ?? 0) })
                        }
                        Text("Suma: " + suma)
                    }
                }
            }
        }
    }
}
