import Free1X2CasosDeUso
import SwiftUI

/// Lo común de Pim y Aidomnou: escribir un fichero y leer uno de texto.
@MainActor private func escribirLineas(_ sesion: Sesion, _ lineas: [String], en u: URL) throws(ErrorDominio) {
    try sesion.entorno.archivos.escribir(TextoNET.codificarLineas(lineas), en: u, atomico: false)
}

/// «Filtro Pim» (pantalla 57, `GeneraPim`): motor de Aidomnou (DEC-922).
struct VistaGeneraPim: View {
    @Environment(Sesion.self) private var sesion
    @State private var valores = [[Double]](repeating: [0, 0, 0], count: 14)
    // R1…R7 y recorrido.
    @State private var rangos = ["3-6", "2-6", "2-6", "2-6", "2-6", "0-1", "0-1", "0-14"]
    @State private var columnaGanadora = "COL.GANADORA"
    @State private var ficheroRangos = "Fichero"
    @State private var ficheroResultado = "Fichero"
    @State private var ficheroGanadoras = "Fichero Ganadoras"
    @State private var procesadas = "-"
    @State private var admitidas = "-"
    @State private var tiempo = "-"
    @State private var indice = "-"
    @State private var analisis = Array(repeating: "-", count: 8)
    @State private var ganadoras: [String] = []
    @State private var actual = 0
    @State private var validas: ConjuntoBits?
    @State private var calculando = false

    /// `RecuperaPantalla` + `PreparaColumnas`: en el C# un rango mal escrito lanza sin capturar.
    private func preparar() -> (ColumnasAidomnou, [[Int]])? {
        do throws(ErrorDominio) {
            guard let r = CasosUtilidades.rangosAidomnou(rangos) else { throw ErrorDominio(.general, "error en datos de entrada") }
            return (try CasosUtilidades.columnasAidomnou(valores), r)
        } catch {
            sesion.alertas.inesperado(error)
            return nil
        }
    }

    private func calcular() async {
        guard case let (c, r)? = preparar(), let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        let ruta = u.path(percentEncoded: false)
        let inicio = sesion.entorno.reloj.ahora()
        (procesadas, admitidas, tiempo) = ("...", "...", "...")
        calculando = true
        var n = (0, 0)
        do throws(ErrorDominio) {
            let f = await CasosUtilidades.filtrarAidomnou(try sesion.entorno.archivos.leer(u), columnas: c, rangos: r, saltarVacias: false)
            n = (f.procesadas, f.admitidas)
            validas = f.validas
            if f.corta != nil { sesion.alertas.error("error de longitud en una columna de entrada") }
        } catch {
            sesion.alertas.error("No se pudo leer el fichero de columnas:\n" + ruta + "\n\n" + error.mensajeUsuario)
        }
        (procesadas, admitidas) = (String(n.0), String(n.1))
        tiempo = CasosUtilidades.tiempoCorto(sesion.entorno.reloj.ahora().timeIntervalSince(inicio))
        calculando = false
    }

    private func guardar(_ nombre: String, _ tipo: (String, String), _ lineas: () async -> [String], _ prefijo: String) async -> URL? {
        guard let u = await SelectorFicheros.guardar(nombre, [tipo, ("Todos los archivos", ".")]) else { return nil }
        do throws(ErrorDominio) {
            try escribirLineas(sesion, await lineas(), en: u)
            return u
        } catch {
            sesion.alertas.error(prefijo + u.path(percentEncoded: false) + "\n\n" + error.mensajeUsuario)
            return nil
        }
    }

    private func cargarGanadoras() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        let ruta = u.path(percentEncoded: false)
        do throws(ErrorDominio) {
            guard let g = CasosUtilidades.ganadorasAidomnou(try sesion.entorno.archivos.leer(u)) else {
                return sesion.alertas.error("col.G. errónea")
            }
            guard g.count <= 3000 else {
                return sesion.alertas.error(
                    "No se pudo leer el fichero de columnas ganadoras:\n" + ruta + "\n\nIndex was outside the bounds of the array.")
            }
            (ganadoras, actual, ficheroGanadoras, indice) = (g, g.count, u.lastPathComponent, String(g.count))
            if let c = g.last { columnaGanadora = c }
        } catch {
            sesion.alertas.error("No se pudo leer el fichero de columnas ganadoras:\n" + ruta + "\n\n" + error.mensajeUsuario)
        }
    }

    private func mover(_ d: Int) {
        guard (d > 0 && actual < ganadoras.count) || (d < 0 && actual > 1) else { return }
        actual += d
        (indice, columnaGanadora) = (String(actual), ganadoras[actual - 1])
    }

    private func analizar() {
        let col = columnaGanadora.replacingOccurrences(of: "x", with: "4").replacingOccurrences(of: "X", with: "4")
        guard case let (c, _)? = preparar() else { return }
        do throws(ErrorDominio) {
            let na = try CasosUtilidades.aciertosAidomnou(c, columna: col)
            analisis = na.map(String.init) + [String(na.reduce(0, +)), String((na.max() ?? 0) - (na.min() ?? 0))]
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    private static let etiquetas = ["R1", "R2", "R3", "R4", "R5", "R6", "R7", "Recorr."]

    var body: some View {
        Pagina(destino: .generaPim) {
            Text("Genera y reduce columnas probables a partir de los pronósticos 1/X/2 de los 14 partidos.")
                .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top, spacing: 16) {
                Tarjeta("Valores por partido") {
                    Text("Probabilidad de cada signo (1 / X / 2) en los 14 partidos.").font(.caption).foregroundStyle(Color.textoSecundario)
                    PorcentajesGrid(valores: $valores)
                }
                VStack(alignment: .leading, spacing: 16) {
                    Tarjeta("Rangos de aciertos") {
                        Text("Mínimo-máximo de aciertos admitidos por cada ranking (formato min-max).")
                            .font(.caption).foregroundStyle(Color.textoSecundario)
                        Grid(alignment: .leading) {
                            ForEach(0..<8, id: \.self) { i in
                                GridRow {
                                    Text(Self.etiquetas[i])
                                    TextField("", text: $rangos[i]).frame(width: 80)
                                }
                            }
                        }
                        HStack {
                            Button("Salvar rangos") {
                                Task {
                                    if let u = await guardar(
                                        "rangos", ("Rangos", ".cnd"), { rangos }, "No se pudieron grabar los rangos en:\n")
                                    {
                                        ficheroRangos = u.lastPathComponent
                                    }
                                }
                            }
                            Button("Recuperar rangos") {
                                Task {
                                    guard let u = await SelectorFicheros.abrir(".cnd", "*") else { return }
                                    do throws(ErrorDominio) {
                                        let l = TextoNET.lineas(try sesion.entorno.archivos.leer(u))
                                        for i in 0..<min(8, l.count) { rangos[i] = l[i] }
                                        ficheroRangos = u.lastPathComponent
                                    } catch {
                                        sesion.alertas.error(
                                            "No se pudo leer el fichero de rangos:\n" + u.path(percentEncoded: false) + "\n\n"
                                                + error.mensajeUsuario)
                                    }
                                }
                            }
                        }
                        Text(ficheroRangos).font(.caption).foregroundStyle(Color.textoSecundario)
                    }
                    Tarjeta("Cálculo de columnas") {
                        HStack {
                            Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).disabled(calculando)
                            Button("Grabar resultado") {
                                Task {
                                    guard let v = validas else {
                                        return sesion.alertas.error("No hay resultado calculado. Pulsa Calcular primero.")
                                    }
                                    if let u = await guardar(
                                        "resultado", ("Resultados", ".txt"), { await CasosUtilidades.columnasValidasAidomnou(v) },
                                        "No se pudo grabar el fichero de resultado:\n")
                                    {
                                        ficheroResultado = u.lastPathComponent
                                    }
                                }
                            }
                            Button("Exportar columnas") {
                                Task {
                                    guard case let (c, _)? = preparar() else { return }
                                    if let u = await guardar(
                                        "columnas", ("F.Salida", ".txt"), { CasosUtilidades.exportarAidomnou(c) },
                                        "No se pudieron exportar las columnas a:\n")
                                    {
                                        ficheroResultado = u.lastPathComponent
                                    }
                                }
                            }
                        }
                        Text("Procesadas: \(procesadas)   Admitidas: \(admitidas)   Tiempo: \(tiempo)").monospacedDigit()
                        Text("Fichero resultado: \(ficheroResultado)").font(.caption).foregroundStyle(Color.textoSecundario)
                    }
                }
            }
            Tarjeta("Análisis Resultados") {
                Text("Cuenta aciertos por ranking de una columna ganadora cargada desde fichero.")
                    .font(.caption).foregroundStyle(Color.textoSecundario)
                HStack {
                    Button("Cargar ganadoras") { Task { await cargarGanadoras() } }
                    Text(ficheroGanadoras).foregroundStyle(Color.textoSecundario)
                    Button("-") { mover(-1) }
                    Text(indice).monospacedDigit()
                    Button("+") { mover(1) }
                    Button("Analizar", action: analizar)
                }
                CampoTexto(etiqueta: "Columna ganadora", texto: $columnaGanadora, ancho: 160)
                Grid(alignment: .leading) {
                    GridRow { ForEach(Self.etiquetas, id: \.self) { Text($0).font(.caption).foregroundStyle(Color.textoSecundario) } }
                    GridRow { ForEach(0..<8, id: \.self) { Text(analisis[$0]).monospacedDigit() } }
                }
            }
        }
    }
}

/// «Filtro Aidomnou» (pantalla 105, `aidomnou`).
struct VistaAidomnou: View {
    @Environment(Sesion.self) private var sesion
    @State private var valores: [[Double]] = []
    // 6 columnas, suma y recorrido.
    @State private var limites = Array(repeating: "0-14", count: 6) + ["0-84", "0-14"]
    @State private var cps: [[Int]] = []
    @State private var ficheroRangos = "Fichero"
    @State private var procesadas = "-"
    @State private var validasTexto = "-"
    @State private var tiempo = "-"
    @State private var ficheroResultado = "Fichero"
    @State private var columna = "2222XXXX222XXX"
    @State private var ficheroGanadoras = "Fichero Ganadoras"
    @State private var paginacion = "-"
    @State private var analisis = Array(repeating: "-", count: 8)
    @State private var ganadoras: [String] = []
    @State private var actual = 0
    @State private var validas = ConjuntoBits(cantidad: 0)
    @State private var tarea: Task<Void, Never>?

    /// `RecuperaPantalla` + `PreparaColumnas` + `PintaPantalla`.
    private func preparar() -> (ColumnasAidomnou, [[Int]])? {
        guard let r = CasosUtilidades.rangosAidomnou(limites) else {
            sesion.alertas.error("error en datos de entrada")
            return nil
        }
        do throws(ErrorDominio) {
            let c = try CasosUtilidades.columnasAidomnou(valores)
            cps = c.cps
            return (c, r)
        } catch {
            sesion.alertas.inesperado(error)
            return nil
        }
    }

    private func calcular() {
        (procesadas, validasTexto, tiempo) = (" ", " ", " ")
        validas = ConjuntoBits(cantidad: 0)
        guard case let (c, r)? = preparar() else { return }
        let entorno = sesion.entorno
        tarea = Task {
            defer { tarea = nil }
            guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
            let inicio = entorno.reloj.ahora()
            do throws(ErrorDominio) {
                let f = await CasosUtilidades.filtrarAidomnou(try entorno.archivos.leer(u), columnas: c, rangos: r, saltarVacias: true)
                if let corta = f.corta { sesion.alertas.error("error de longitud=" + corta) }
                validas = f.validas
                (procesadas, validasTexto) = (String(f.procesadas), String(f.admitidas))
                tiempo = FormatoNET.toStringTimeSpan(entorno.reloj.ahora().timeIntervalSince(inicio))
            } catch {
                sesion.alertas.error("No se ha podido leer el fichero de entrada: " + error.mensajeUsuario)
            }
        }
    }

    private func guardar(_ nombre: String, _ tipo: (String, String), _ lineas: [String], _ prefijo: String) async -> URL? {
        guard let u = await SelectorFicheros.guardar(nombre, [tipo]) else { return nil }
        do throws(ErrorDominio) {
            try escribirLineas(sesion, lineas, en: u)
            return u
        } catch {
            sesion.alertas.error(prefijo + error.mensajeUsuario)
            return nil
        }
    }

    private func abrirGanadoras() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        do throws(ErrorDominio) {
            guard let g = CasosUtilidades.ganadorasAidomnou(try sesion.entorno.archivos.leer(u)) else {
                return sesion.alertas.error("col.G. errónea")
            }
            guard let ultima = g.last else { return sesion.alertas.error("El fichero de ganadoras está vacío.") }
            (ganadoras, actual, ficheroGanadoras, paginacion, columna) = (g, g.count, u.lastPathComponent, String(g.count), ultima)
        } catch {
            sesion.alertas.error("No se ha podido leer el fichero de ganadoras: " + error.mensajeUsuario)
        }
    }

    private func mover(_ d: Int) {
        guard (d > 0 && actual < ganadoras.count) || (d < 0 && actual > 1) else { return }
        actual += d
        (paginacion, columna) = (String(actual), ganadoras[actual - 1])
    }

    private func analizar() {
        guard case let (c, _)? = preparar() else { return }
        let col = columna.replacingOccurrences(of: "x", with: "4").replacingOccurrences(of: "X", with: "4")
        guard col.utf16.count >= 14, let na = try? CasosUtilidades.aciertosAidomnou(c, columna: col) else {
            return sesion.alertas.error("col. errónea=" + col)
        }
        analisis = na.map(String.init) + [String(na.reduce(0, +)), String((na.max() ?? 0) - (na.min() ?? 0))]
    }

    var body: some View {
        Pagina(destino: .aidomnou) {
            Text(
                "Genera 6 columnas probables a partir de las valoraciones de los 14 partidos, define límites de aciertos por columna y filtra un fichero de columnas que cumplan esas condiciones."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top, spacing: 16) {
                Tarjeta("Valoraciones (1 / X / 2)") {
                    Text("Valoración de cada signo por partido. Genera las 6 columnas probables.")
                        .font(.caption).foregroundStyle(Color.textoSecundario)
                    PorcentajesGrid(valores: $valores)
                }
                Tarjeta("Columnas Probables") {
                    Grid(alignment: .center, horizontalSpacing: 10) {
                        GridRow {
                            Text("P")
                            ForEach(1...6, id: \.self) { Text("C\($0)") }
                        }
                        .font(.caption).foregroundStyle(Color.textoSecundario)
                        ForEach(cps.indices, id: \.self) { p in
                            GridRow {
                                Text(String(format: "%02d", p + 1)).monospacedDigit()
                                ForEach(0..<6, id: \.self) { Text(CasosUtilidades.cambiaAidomnou(cps[p][$0])).monospaced() }
                            }
                        }
                        GridRow {
                            Text("lím:")
                            ForEach(0..<6, id: \.self) { TextField("", text: $limites[$0]).frame(width: 56) }
                        }
                        GridRow {
                            Text("ac:")
                            ForEach(0..<6, id: \.self) { Text(analisis[$0]).monospacedDigit() }
                        }
                    }
                    HStack {
                        Text("suma:")
                        TextField("", text: $limites[6]).frame(width: 60)
                        Text("recorrido:")
                        TextField("", text: $limites[7]).frame(width: 60)
                    }
                    Text("suma: \(analisis[6])   recorrido: \(analisis[7])").monospacedDigit()
                    ForEach(0..<(cps.isEmpty ? 0 : 6), id: \.self) { c in
                        Text("Col \(c + 1): " + (0..<14).map { CasosUtilidades.cambiaAidomnou(cps[$0][c]) }.joined(separator: " "))
                            .font(.caption).monospaced()
                    }
                    // El original prometía `aidomnou.cfg` editable; el port usa los límites fijos (DEC-923, DP-08; C5 del plan).
                    Text("NOTA: Las condiciones de generación de las 6 columnas son fijas; no se leen de ningún fichero.")
                        .font(.caption).foregroundStyle(Color.textoSecundario)
                }
            }
            HStack(alignment: .top, spacing: 16) {
                Tarjeta {
                    HStack {
                        Button("Salvar Límites") {
                            Task {
                                if let u = await guardar("Aidomnou", ("Rangos", ".cnd"), limites, "No se han podido guardar los límites: ")
                                {
                                    ficheroRangos = u.lastPathComponent
                                }
                            }
                        }
                        Button("Recuperar Límites") {
                            Task {
                                guard let u = await SelectorFicheros.abrir(".cnd", "*") else { return }
                                do throws(ErrorDominio) {
                                    let l = TextoNET.lineas(try sesion.entorno.archivos.leer(u))
                                    for i in 0..<min(8, l.count) { limites[i] = l[i] }
                                    ficheroRangos = u.lastPathComponent
                                } catch {
                                    sesion.alertas.error("No se han podido leer los límites: " + error.mensajeUsuario)
                                }
                            }
                        }
                        Text(ficheroRangos).foregroundStyle(Color.textoSecundario)
                    }
                    HStack {
                        Button("Calcular", action: calcular).buttonStyle(EstiloPrincipal()).disabled(tarea != nil)
                        Text("Procesadas: \(procesadas)   Válidas: \(validasTexto)   Tiempo: \(tiempo)").monospacedDigit()
                        Button("Cancelar") { tarea?.cancel() }
                    }
                    HStack {
                        Button("Exportar Columnas") {
                            Task {
                                guard case let (c, _)? = preparar() else { return }
                                _ = await guardar(
                                    "ColumnasProbables", ("Salida", ".txt"), CasosUtilidades.exportarAidomnou(c),
                                    "No se ha podido exportar: ")
                            }
                        }
                        Button("Grabar Resultado") {
                            Task {
                                guard validas.cuenta > 0 else {
                                    return sesion.alertas.info("No hay columnas válidas que grabar. Ejecute primero Calcular.")
                                }
                                let l = await CasosUtilidades.columnasValidasAidomnou(validas)
                                if let u = await guardar("Resultados", ("Resultados", ".txt"), l, "No se ha podido grabar el resultado: ") {
                                    ficheroResultado = u.lastPathComponent
                                }
                            }
                        }
                        Text(ficheroResultado).foregroundStyle(Color.textoSecundario)
                    }
                }
                Tarjeta("Análisis Resultados") {
                    HStack {
                        Button("Fichero Ganadoras") { Task { await abrirGanadoras() } }
                        Text(ficheroGanadoras).foregroundStyle(Color.textoSecundario)
                        Button("-") { mover(-1) }
                        Text(paginacion).monospacedDigit()
                        Button("+") { mover(1) }
                    }
                    CampoTexto(
                        etiqueta: "Columna a analizar (14 signos: 1 / X / 2)",
                        texto: Binding(get: { columna }, set: { columna = String(FormatoNET.mayusculas($0).prefix(14)) }), ancho: 160)
                    Button("Analizar", action: analizar)
                }
            }
        }
        .onAppear { if valores.isEmpty { valores = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos) } }
    }
}
