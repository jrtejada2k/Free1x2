import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/Analizador.cs y GeneradorColumnas.cs (01 §1-§2.2).

/// Las rutas de cálculo de la combinación (01 §1.1).
public enum RutaCalculo: Sendable {
    /// A: contar (`AnalizaCombinacion(false)`).
    case contar
    /// B: contar y entregar cada columna aceptada para grabarla (`AnalizaCombinacion(ruta)`).
    case grabar
    /// C: analizar (`AnalizaCombinacion(true)`): archivo recortado sin relleno.
    case analizar
    /// D: analizar un archivo externo (`AnalizaCombinacion(true, true)`): sin recorte ni relleno.
    case analizarExterno
}

/// Contadores de una ejecución.
public struct ResultadoCalculo: Sendable, Hashable {
    public var analizadas: Int
    public var aceptadas: Int

    public init(analizadas: Int, aceptadas: Int) {
        self.analizadas = analizadas
        self.aceptadas = aceptadas
    }
}

/// Punto de entrada del motor de cálculo de columnas.
public enum MotorCalculo {
    /// Rutas A/B. `aceptada` recibe cada columna aceptada en el orden de generación (ruta B: el llamador escribe
    /// `UtilColumnas.convLongToStr`). Cancelar corta la generación como `PararAnalisis` y devuelve los contadores parciales.
    public static func calcular(
        _ combinacion: Combinacion, configuracion: Configuracion, cultura: Cultura = .esES,
        leerLineas: @escaping LectorLineas, progreso: ReportadorProgreso = .ninguno,
        aceptada: ((Columna) throws(ErrorDominio) -> Void)? = nil
    ) throws(ErrorDominio) -> ResultadoCalculo {
        let a = Analizador(combinacion, configuracion: configuracion, cultura: cultura, leerLineas: leerLineas)
        a.aceptada = aceptada
        a.progreso = progreso
        try a.analizaCombinacion()
        let r = ResultadoCalculo(analizadas: a.analizadas, aceptadas: a.aceptadas)
        progreso.terminar(Progreso(procesadas: r.analizadas, aceptadas: r.aceptadas))
        return r
    }

    /// Rutas C/D: además de contar, acumula los histogramas de las columnas aceptadas. If-Then hace AND (no asigna) y no
    /// se comprueba el pronóstico base (01 §1.3). `numeroPartidosExterno` (ruta D) dimensiona el contenedor como el
    /// `numPartidos` del `Analizador` (por defecto, el de la configuración).
    public static func analizar(
        _ combinacion: Combinacion, configuracion: Configuracion, cultura: Cultura = .esES, externo: Bool = false,
        numeroPartidosExterno: Int? = nil, leerLineas: @escaping LectorLineas, progreso: ReportadorProgreso = .ninguno
    ) throws(ErrorDominio) -> ResultadoAnalisis {
        let a = Analizador(combinacion, configuracion: configuracion, cultura: cultura, leerLineas: leerLineas)
        a.progreso = progreso
        let n = configuracion.boleto.numeroPartidos
        let registro = try a.inicializarContenedorAnalisis(
            externo: externo, numeroPartidos: externo ? numeroPartidosExterno ?? n : n, configuracion: configuracion,
            cultura: cultura)
        a.registro = registro
        try a.analizaCombinacion(externo ? .analizarExterno : .analizar)
        let r = ResultadoCalculo(analizadas: a.analizadas, aceptadas: a.aceptadas)
        progreso.terminar(Progreso(procesadas: r.analizadas, aceptadas: r.aceptadas))
        return ResultadoAnalisis(calculo: r, contenedor: registro.global)
    }
}

/// Contadores y contenedor de un análisis (rutas C/D); el visor se muestra si hay aceptadas.
public struct ResultadoAnalisis: Sendable, Hashable {
    public let calculo: ResultadoCalculo
    public let contenedor: ContenedorAnalisisGlobal
}

/// Grafo de ejecución de una combinación (ESPEC_11 §4.3): vive lo que dura un cálculo.
final class Analizador {
    let combinacion: Combinacion
    let numeroPartidos: Int
    let grupos: [Grupo]
    let controlador: ControladorGrupos
    let ifThen: ControladorIfThen?
    let leerLineas: LectorLineas
    var aceptada: ((Columna) throws(ErrorDominio) -> Void)?
    var progreso = ReportadorProgreso.ninguno
    /// Rutas C/D.
    var registro: RegistroAnalisis?
    private var ruta = RutaCalculo.contar
    private(set) var analizadas = 0
    private(set) var aceptadas = 0
    private var pronosticoBase: Columna = 0
    private var columnaBase: Columna = 0
    private var noPartidos = 0
    private var enMarcha = false

    init(_ c: Combinacion, configuracion: Configuracion, cultura: Cultura, leerLineas: @escaping LectorLineas) {
        combinacion = c
        numeroPartidos = configuracion.boleto.numeroPartidos
        let contexto = ContextoFiltros(
            numeroPartidos: numeroPartidos, analisis: configuracion.analisis, cultura: cultura,
            puntosCP: configuracion.puntosCP)
        grupos = c.grupos.map { Grupo($0, contexto: contexto, leerLineas: leerLineas) }
        controlador = ControladorGrupos(grupos: grupos, c)
        ifThen = c.ifThen.map { ControladorIfThen($0, numeroPartidos: configuracion.boleto.numeroPartidos) }
        self.leerLineas = leerLineas
    }

    /// `AnalizaCombinacion(bool)` (:256-349).
    func analizaCombinacion(_ ruta: RutaCalculo = .contar) throws(ErrorDominio) {
        self.ruta = ruta
        analizadas = 0
        aceptadas = 0
        pronosticoBase = try Self.columna(combinacion.pronosticos, "Analizador.cs:411")
        enMarcha = true
        defer { enMarcha = false }
        if !combinacion.archivoColumnasBase.isEmpty {
            columnaBase = 281_474_976_710_655  // 2^48 − 1 (GeneradorColumnas.cs:30)
            try usaColumnasArchivo(try leerLineas(combinacion.archivoColumnasBase))
        } else {
            let pronosticos = combinacion.pronosticos ?? []
            noPartidos = pronosticos.count
            columnaBase = try Self.columna(combinacion.pronosticos, "GeneradorColumnas.cs:23")
            let inicial = Self.columnaInicial(numeroPartidos)
            try analizaNueva(inicial)
            try genera(inicial, 0, 1)
        }
    }

    /// `ConvStrToLong(Pronosticos)`: `nil` (o un pronóstico `nil`) ⇒ `NullReferenceException`.
    static func columna(_ pronosticos: [String?]?, _ origen: String) throws(ErrorDominio) -> Columna {
        guard let pronosticos else { throw .referenciaNula(origen) }
        var p: [String] = []
        for x in pronosticos {
            guard let x else { throw .referenciaNula("UtilColumnas.cs:82") }
            p.append(x)
        }
        return UtilColumnas.convStrToLong(pronosticos: p)
    }

    /// `InicializarColumnaInicial` (:36-91): «todo unos» = `4·(8^N − 1)/7` para N en 1…16; si no, 0.
    static func columnaInicial(_ n: Int) -> Columna {
        guard (1...16).contains(n) else { return 0 }
        return 4 * ((Int64(1) << (3 * n)) - 1) / 7
    }

    /// `GeneraColumnas` (:199-228): preorden; en cada partido, primero la `X` con su subárbol y después el `2`.
    private func genera(_ valor: Columna, _ posicion: Int, _ profundidad: Int) throws(ErrorDominio) {
        guard enMarcha else { return }
        var mascara = Int64(7) &<< (3 * posicion)
        var partido = posicion
        while partido < noPartidos {
            let x = (valor & ~mascara) ^ (Int64(1) &<< (partido * 3 + 1))
            try analizaNueva(x)
            if profundidad < noPartidos { try genera(x, partido + 1, profundidad + 1) }
            let d = (valor & ~mascara) ^ (Int64(1) &<< (partido * 3))
            try analizaNueva(d)
            if profundidad < noPartidos { try genera(d, partido + 1, profundidad + 1) }
            mascara = mascara &<< 3
            partido += 1
        }
    }

    /// `UsaColumnasArchivo()` (:272-299): recorta a N o rellena con `CompletarCon`. Ruta C (:300-324) recorta sin
    /// rellenar; ruta D (`UsaColumnasArchivoExterno`, :325-336) ni recorta ni rellena.
    private func usaColumnasArchivo(_ lineas: [String]) throws(ErrorDominio) {
        let n = numeroPartidos
        for linea in lineas {
            guard enMarcha else { break }
            let u = linea.utf16
            let col: Columna
            if ruta == .analizarExterno {
                col = UtilColumnas.convStrToLong(linea)
            } else if u.count > n {
                col = UtilColumnas.convStrToLong(String(decoding: Array(u.prefix(n)), as: UTF16.self))
            } else if u.count < n && ruta != .analizar {
                col = try UtilColumnas.convStrToLong(linea, relleno: combinacion.completarCon, numeroPartidos: n)
            } else {
                col = UtilColumnas.convStrToLong(linea)
            }
            try analizaNueva(col)
        }
    }

    /// `AnalizaNuevaColumna(long)` (:357-364): criba barata con la columna base.
    @inline(__always)
    private func analizaNueva(_ col: Columna) throws(ErrorDominio) {
        guard columnaBase & col == col else { return }
        if let registro { try analizaColumna(col, registro) } else { try analizaColumna(col) }
    }

    /// `AnalizaColumna(long, bool)` (:137-165): rutas C/D, sin criba de pronóstico; If-Then hace AND [PRESERVAR].
    private func analizaColumna(_ col: Columna, _ registro: RegistroAnalisis) throws(ErrorDominio) {
        try controlador.recalcular()
        var valida = try controlador.analiza(col, registro)
        if let ifThen, !ifThen.esVacio, ifThen.activo, !(try ifThen.comprueba(col, grupos)) { valida = false }
        if analizadas & 0xFFF == 0 { bombear() }
        analizadas += 1
        if valida {
            try registro.incrementarContador(col)
            aceptadas += 1
        }
    }

    /// `InicializarContenedorAnalisis(esArchivo)` (:351-408): en el análisis de la combinación mira las condiciones del
    /// grupo base; el externo no consulta filtros.
    func inicializarContenedorAnalisis(
        externo: Bool, numeroPartidos n: Int, configuracion: Configuracion, cultura: Cultura
    ) throws(ErrorDominio) -> RegistroAnalisis {
        var g = ContenedorAnalisisGlobal(noGrupos: grupos.count, noConjuntos: controlador.controles.count, numeroPartidos: n)
        if externo {
            g.esAnalisisExterno = true
            return RegistroAnalisis(g, analisis: configuracion.analisis)
        }
        let c = try combinacion.grupos.leerLista(0, "Analizador.cs:355").condiciones
        g.analisisGrupos.usaSimetrias = c.simetrias.activa
        g.analisisGrupos.usaDiferencias = c.diferencias.activa
        g.analisisGrupos.usaValoraciones = c.valoracionSignos.activa
        g.analisisGrupos.usaCPs = c.colProbables.activa
        g.analisisGrupos.usaFormatos = c.formatosSignos.activa
        if c.simetrias.activa {
            g.analisisGrupos.noAciertosSimetrias = c.simetrias.aciertos
            g.analisisGrupos.inicializarSimetrias()
        }
        if c.diferencias.activa {
            g.analisisGrupos.diferencias = c.diferencias.diferencias.map { ContenedorDiferencias($0.partidosSimetricos.count) }
        }
        if c.valoracionSignos.activa {
            let v = c.valoracionSignos
            g.analisisGrupos.noValoresValoracionGlobal = try Self.obtenerArrayAciertos(v.valorGlobal, cultura)
            g.analisisGrupos.noValoresValoracionUnos = try Self.obtenerArrayAciertos(v.valorUnos, cultura)
            g.analisisGrupos.noValoresValoracionEquis = try Self.obtenerArrayAciertos(v.valorEquis, cultura)
            g.analisisGrupos.noValoresValoracionDoses = try Self.obtenerArrayAciertos(v.valorDoses, cultura)
            try g.analisisGrupos.inicializarValoraciones()
        }
        if c.colProbables.activa {
            // `cp.Pronosticos.Length` = N de la CP.
            g.analisisGrupos.columnasProbables += c.colProbables.columnas.map {
                _ in ContenedorColumnasProbables(partidos: n)
            }
        }
        if c.formatosSignos.activa {
            g.analisisGrupos.inicializaFormatosSignos(c.formatosSignos.formatosSignos.map(\.lineasFormatos.count))
        }
        return RegistroAnalisis(g, analisis: configuracion.analisis)
    }

    /// `ObtenerArrayAciertos` (:229-255): trozos por `#`; `a-b` con `Math.Round(Convert.ToDouble)` (a par, cultura
    /// actual); ordenada, con repetidos.
    static func obtenerArrayAciertos(_ valores: String, _ cultura: Cultura) throws(ErrorDominio) -> [Int] {
        var r: [Int] = []
        guard !valores.isEmpty else { return r }
        for t in trozos(valores, "#") {
            if !t.contains("-") {
                r.append(Int(try FormatoNET.convertToInt32(t)))
            } else {
                let p = trozos(t, "-")
                func redondear(_ s: String) throws(ErrorDominio) -> Int {
                    let v = try FormatoNET.convertToDouble(s, cultura: cultura).rounded(.toNearestOrEven)
                    guard let i = Int32(exactly: v) else { throw .desbordamiento("Analizador.cs:244") }
                    return Int(i)
                }
                let a = try redondear(p[0])
                let b = try redondear(try p.leer(1, "Analizador.cs:244"))
                if a <= b { r += Array(a...b) }
            }
        }
        return r.sorted()
    }

    /// `AnalizaColumna(long)` (:84-119): ruta A/B.
    private func analizaColumna(_ col: Columna) throws(ErrorDominio) {
        var valida = false
        if pronosticoBase & col == col {
            try controlador.recalcular()
            valida = try controlador.analiza(col)
        }
        if let ifThen, !ifThen.esVacio, ifThen.activo {
            valida = try ifThen.comprueba(col, grupos)  // asigna, no hace AND [PRESERVAR] (:97-103)
        }
        if analizadas & 0xFFF == 0 { bombear() }
        analizadas += 1
        if valida {
            aceptadas += 1
            try aceptada?(col)
        }
    }

    /// `BombearUiSiToca` (:127-136): cada 4 096 columnas, progreso y cancelación cooperativa (`PararAnalisis`).
    private func bombear() {
        progreso.reportar(Progreso(procesadas: analizadas, aceptadas: aceptadas))
        if Task.isCancelled { enMarcha = false }
    }
}
