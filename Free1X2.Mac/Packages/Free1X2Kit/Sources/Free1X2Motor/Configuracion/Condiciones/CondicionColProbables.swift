import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/ColumnaProbable.cs, CPControlFallos.cs, ControladorCPControlFallos.cs,
// RelacionCP1.cs, RelacionCP2.cs, RelacionCP3.cs y FiltroColProbables.cs (04 §1-§10) — la parte configurable.

/// Una columna probable: pronóstico propio con rangos de aciertos, aciertos seguidos, fallos seguidos y puntos (04 §1).
public struct ColumnaProbableConfig: Sendable, Hashable {
    /// Byte por partido (D1, 0 = ignorado); `N+1` posiciones, la última solo la toca `todosATriple`.
    public private(set) var pronosticos: [UInt8]
    public private(set) var aciertos: ValoresConTolerancia
    public private(set) var aciertosSeguidos: ValoresConTolerancia
    public private(set) var fallosSeguidos: ValoresConTolerancia
    /// `SetPuntos` (D2); `""` = sin restricción.
    public private(set) var puntos = ConjuntoRangos()
    public var toleranciaLocalActiva = false
    public private(set) var acTolLocal: [Bool]
    public private(set) var acsTolLocal: [Bool]
    public private(set) var fsTolLocal: [Bool]
    /// Índice = nº de fallos tolerados (0…3) permitido.
    public private(set) var tolLocales = [Bool](repeating: false, count: 4)
    public let numeroPartidos: Int

    public init(numeroPartidos n: Int) {
        numeroPartidos = n
        pronosticos = Array(repeating: 0, count: n + 1)
        aciertos = ValoresConTolerancia(tamano: n + 1)
        aciertosSeguidos = aciertos
        fallosSeguidos = aciertos
        acTolLocal = Array(repeating: false, count: n + 1)
        acsTolLocal = acTolLocal
        fsTolLocal = acTolLocal
    }

    /// `SetNoAciertos` … `SetNoFallosSeguidos` (:376-452): P6, acumulan (P5).
    public mutating func setNoAciertos(_ s: String) throws(ErrorDominio) {
        try aciertos.poner(s, "ColumnaProbable.cs:376-400")
    }

    public mutating func setNoAciertosSeguidos(_ s: String) throws(ErrorDominio) {
        try aciertosSeguidos.poner(s, "ColumnaProbable.cs:402-426")
    }

    public mutating func setNoFallosSeguidos(_ s: String) throws(ErrorDominio) {
        try fallosSeguidos.poner(s, "ColumnaProbable.cs:428-452")
    }

    public mutating func setPuntos(_ s: String) throws(ErrorDominio) { try puntos.poner(s) }

    /// `SetACTol` … `SetFSTol` (:470-501): activan la tolerancia local **antes** de parsear.
    public mutating func setACTol(_ s: String) throws(ErrorDominio) {
        toleranciaLocalActiva = true
        try Self.marcar(&acTolLocal, s, "ColumnaProbable.cs:470-480")
    }

    public mutating func setACSTol(_ s: String) throws(ErrorDominio) {
        toleranciaLocalActiva = true
        try Self.marcar(&acsTolLocal, s, "ColumnaProbable.cs:482-490")
    }

    public mutating func setFSTol(_ s: String) throws(ErrorDominio) {
        toleranciaLocalActiva = true
        try Self.marcar(&fsTolLocal, s, "ColumnaProbable.cs:492-501")
    }

    /// `SetTolerancias` (:503-511): no activa la tolerancia local.
    public mutating func setTolerancias(_ s: String) throws(ErrorDominio) {
        try Self.marcar(&tolLocales, s, "ColumnaProbable.cs:503-511")
    }

    private static func marcar(_ a: inout [Bool], _ s: String, _ origen: String) throws(ErrorDominio) {
        for t in trozos(s, ",") { try a.escribir(true, en: Int(try FormatoNET.convertToInt32(t)), origen) }
    }

    /// `Pronosticos = v` (:711-720): D1 por posición (`nil`/inválido ⇒ 0); no limpia el resto; más de `N+1` ⇒ lanza.
    public mutating func ponerPronosticos(_ valores: [String?]) throws(ErrorDominio) {
        for (i, v) in valores.enumerated() {
            try pronosticos.escribir(v.map(UtilColumnas.convPartidoStrToByte) ?? 0, en: i, "ColumnaProbable.cs:716")
        }
    }

    /// `Pronosticos` get (:695-709): los `N` primeros como texto.
    public var textoPronosticos: [String] { pronosticos.prefix(numeroPartidos).map(UtilColumnas.convPartidoByteToStr) }

    public mutating func borraPronosticos() { pronosticos = Array(repeating: 0, count: numeroPartidos + 1) }

    /// `TodosATriple` (:324-336): también la posición `N`.
    public mutating func todosATriple() { pronosticos = Array(repeating: 7, count: numeroPartidos + 1) }

    /// `ReinicializaValores` (:105-128): no toca pronósticos ni puntos.
    public mutating func reinicializaValores() {
        aciertos.reinicializar()
        aciertosSeguidos.reinicializar()
        fallosSeguidos.reinicializar()
        acTolLocal = Array(repeating: false, count: acTolLocal.count)
        acsTolLocal = acTolLocal
        fsTolLocal = acTolLocal
        tolLocales = [false, false, false, false]
        toleranciaLocalActiva = false
    }

    public var getAciertos: String { aciertos.texto }
    public var getAciertosSeguidos: String { aciertosSeguidos.texto }
    public var getFallosSeguidos: String { fallosSeguidos.texto }
    public var getTolerancias: String { UtilidadesEntradasValores.obtenerTextoFromBool(tolLocales) }
    public var getACTol: String { UtilidadesEntradasValores.obtenerTextoFromBool(acTolLocal) }
    public var getACSTol: String { UtilidadesEntradasValores.obtenerTextoFromBool(acsTolLocal) }
    public var getFSTol: String { UtilidadesEntradasValores.obtenerTextoFromBool(fsTolLocal) }
}

/// Una «línea de tolerancias locales» (04 §3): letras gastadas por las CPs indicadas.
public struct CPControlFallosConfig: Sendable, Hashable {
    public private(set) var columnas = ""
    /// Números de CP 1-based.
    public private(set) var columnasRelacionadas: [Int]?
    /// `Tolerancias = v.ToUpper()` (:214).
    public var tolerancias = "" {
        didSet { tolerancias = FormatoNET.mayusculas(tolerancias) }
    }
    public private(set) var aciertos = ""
    public private(set) var aciertosPermitidos: [Bool]?

    public init() {}

    public mutating func ponerColumnas(_ s: String) throws(ErrorDominio) {
        columnas = s
        try ListasCP.columnas(s, en: &columnasRelacionadas, "CPControlFallos.cs:66-103")
    }

    public mutating func ponerAciertos(_ s: String) throws(ErrorDominio) {
        aciertos = s
        try ListasCP.aciertos(s, en: &aciertosPermitidos, "CPControlFallos.cs:105-158")
    }
}

/// Relación I entre CPs (04 §5): cuántas tienen ciertos aciertos, su suma y su recorrido.
public struct RelacionCP1Config: Sendable, Hashable {
    public private(set) var columnas = ""
    public private(set) var columnasRelacionadas: [Int]?
    public private(set) var sumaAciertos = ""
    public private(set) var sumas: [Bool]?
    public private(set) var recorridos = ""
    /// Siempre 15 posiciones una vez asignado.
    public private(set) var recorridosPermitidos: [Bool]?
    public private(set) var cantidadCP = ConjuntoRangos()
    public private(set) var cuantosAC = ConjuntoRangos()

    public init() {}

    public mutating func ponerColumnas(_ s: String) throws(ErrorDominio) {
        columnas = s
        try ListasCP.columnas(s, en: &columnasRelacionadas, "RelacionCP1.cs:272-309")
    }

    /// `SumaAciertos` (:311-351): `""` no toca el array.
    public mutating func ponerSumaAciertos(_ s: String) throws(ErrorDominio) {
        sumaAciertos = s
        if !s.isEmpty { try ListasCP.aciertos(s, en: &sumas, "RelacionCP1.cs:311-351") }
    }

    /// `Recorridos` (:353-393): siempre `bool[15]`; un valor ≥ 15 lanza.
    public mutating func ponerRecorridos(_ s: String) throws(ErrorDominio) {
        recorridos = s
        let o = "RelacionCP1.cs:353-393"
        var r = [Bool](repeating: false, count: 15)
        defer { recorridosPermitidos = r }
        guard !s.isEmpty else { return }
        switch ListasCP.separador(s) {
        case ",":
            for t in trozos(s, ",") { try r.escribir(true, en: Int(try FormatoNET.convertToInt32(t)), o) }
        case "-":
            let (a, b) = try ListasCP.extremos(s)
            if b >= a {
                for i in a...b { try r.escribir(true, en: i, o) }
            }
        default:
            try r.escribir(true, en: Int(try FormatoNET.convertToInt32(s)), o)
        }
    }

    public mutating func ponerCantidadCP(_ s: String) throws(ErrorDominio) { try cantidadCP.poner(s) }
    public mutating func ponerCuantosAC(_ s: String) throws(ErrorDominio) { try cuantosAC.poner(s) }
}

/// Relación II (04 §7): diferencias de AC/ACS/FS entre dos grupos de CPs, global e individual. Los textos de columnas
/// son 1-based y se convierten con `ObtenerListaFromTxt` (D3, P10); los de aciertos con `ObtenerListaFromTxtAciertos`.
public struct RelacionCP2Config: Sendable, Hashable {
    public var columnasA = "", columnasB = "", aciertos = ""
    /// `"AC"`, `"ACS"` o `"FS"`.
    public var concepto = ""
    /// `"Más"` o `"Menos"`.
    public var cantidad = ""
    public var columnasA2 = "", columnasB2 = "", aciertos2 = ""
    public var concepto2 = "", cantidad2 = ""

    public init() {}
}

/// `AgrupacionColumnas` (`AgrupacionColumnas.cs:6-30`): `numero` = nº de rachas permitido de longitud `noElementos` con
/// los valores `noAciertos`.
public struct AgrupacionColumnas: Sendable, Hashable {
    public var numero: [Int]
    public var noElementos: Int
    public var noAciertos: [Int]

    public init(numero: [Int], noElementos: Int, noAciertos: [Int]) {
        self.numero = numero
        self.noElementos = noElementos
        self.noAciertos = noAciertos
    }
}

/// Relación III (04 §9): escaleras, sándwiches y agrupaciones en la serie AC/ACS/FS de las **primeras K** CPs (P12).
/// Los arrays permitidos los construye el cargador (04 §15); `nil` = sin restricción.
public struct RelacionCP3Config: Sendable, Hashable {
    /// K = nº de índices de `ObtenerListaFromTxt(columnasImplicadas)`.
    public var columnasImplicadas = ""
    public var concepto: String?
    public var escalerasAsc: [Bool]?
    public var escalerasDesc: [Bool]?
    public var escalerasTotales: [Bool]?
    public var sandwichs: [Bool]?
    public var agrupacionesPasoFijo: [AgrupacionColumnas]?
    public var agrupacionesSolapadas: [AgrupacionColumnas]?
    /// Textos tal cual se leyeron (`*String` del C#), que son los que se reescriben; `nil` ⇒ `values=""`.
    public var sandwichsTexto: String?
    public var escalerasAscTexto: String?
    public var escalerasDescTexto: String?
    public var escalerasTexto: String?
    public var conceptoTexto: String?
    public var agrupacionesPasoFijoTexto: String?
    public var agrupacionesSolapadasTexto: String?

    public init() {}
}

/// Condición «Columnas probables» (04 §2).
public struct CondicionColProbables: Sendable, Hashable {
    public var activa = false
    public var contieneDatos = false
    /// `ColProbables.Add` no toca `contieneDatos`; `ponerColumnas` sí.
    public var columnas: [ColumnaProbableConfig] = []
    public var controlesFallos: [CPControlFallosConfig] = []
    /// `ControladorCPControlFallos.FallosPermitidos`: `""` = todos los controles deben pasar.
    public private(set) var fallosPermitidos = ConjuntoRangos()
    public var relaciones1: [RelacionCP1Config] = []
    public var relaciones2: [RelacionCP2Config] = []
    public var relaciones3: [RelacionCP3Config] = []

    public init() {}

    /// `ColProbables = lista` (:159-181).
    public mutating func ponerColumnas(_ lista: [ColumnaProbableConfig]) {
        columnas = lista
        contieneDatos = !lista.isEmpty
    }

    public mutating func ponerFallosPermitidos(_ s: String) throws(ErrorDominio) { try fallosPermitidos.poner(s) }
}

/// Parseo propio de CPControlFallos y RelacionCP1: el **primer** `,` o `-` decide la forma; no admite mezclas. Escriben
/// en el destino en cuanto el C# asigna el array, para dejar el mismo estado parcial si un trozo lanza.
enum ListasCP {
    /// `EncuentraSeparador`.
    static func separador(_ s: String) -> Character? { s.first { $0 == "," || $0 == "-" } }

    static func extremos(_ s: String) throws(ErrorDominio) -> (Int, Int) {
        let t = trozos(s, "-")
        return (Int(try FormatoNET.convertToInt32(t[0])), Int(try FormatoNET.convertToInt32(t[1])))
    }

    /// `InicializarColumnas` (`a,b,c` · `a-b` · `a`): `b < a − 1` ⇒ `new int[negativo]` ⇒ `Overflow`.
    static func columnas(_ s: String, en destino: inout [Int]?, _ o: String) throws(ErrorDominio) {
        switch separador(s) {
        case ",":
            let t = trozos(s, ",")
            var r = [Int](repeating: 0, count: t.count)
            defer { destino = r }
            for (i, x) in t.enumerated() { r[i] = Int(try FormatoNET.convertToInt32(x)) }
        case "-":
            let (a, b) = try extremos(s)
            let n = b - a + 1
            guard n >= 0 else { throw .desbordamiento(o) }
            destino = Array(a..<a + n)
        default:
            var r = [0]
            defer { destino = r }
            r[0] = Int(try FormatoNET.convertToInt32(s))
        }
    }

    /// `InicializarAciertos`/`InicializarSumas`: `bool[último + 2]` (lista ascendente, P17).
    static func aciertos(_ s: String, en destino: inout [Bool]?, _ o: String) throws(ErrorDominio) {
        switch separador(s) {
        case ",":
            let t = trozos(s, ",")
            var r = try nuevo(Int(try FormatoNET.convertToInt32(t[t.count - 1])) + 2, o)
            defer { destino = r }
            for x in t { try r.escribir(true, en: Int(try FormatoNET.convertToInt32(x)), o) }
        case "-":
            let (a, b) = try extremos(s)
            var r = try nuevo(b + 2, o)
            defer { destino = r }
            if b >= a {
                for i in a...b { try r.escribir(true, en: i, o) }
            }
        default:
            let v = Int(try FormatoNET.convertToInt32(s))
            var r = try nuevo(v + 2, o)
            defer { destino = r }
            try r.escribir(true, en: v, o)
        }
    }

    /// Mismo tope de memoria que `RangosHelper.obtenBoolArray`.
    private static func nuevo(_ n: Int, _ o: String) throws(ErrorDominio) -> [Bool] {
        guard n <= RangosHelper.maximoValor + 2 else {
            throw ErrorDominio(.argumentoFueraDeRango, "Valor demasiado grande: \(n - 2).", origen: o)
        }
        return try [Bool].net(repeating: false, count: n, o)
    }
}
