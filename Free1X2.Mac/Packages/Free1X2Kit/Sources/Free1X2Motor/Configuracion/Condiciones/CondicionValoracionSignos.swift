import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroValoracionSignos.cs y RangosOpciones.cs (03 §5; 01 §2.11).

/// Intervalos cerrados `[min, max]` de `Double` (`RangosOpciones`).
public struct RangosOpciones: Sendable, Hashable {
    public private(set) var rangos: [ClosedRangeNET] = []

    public init() {}

    /// `PonerRangos("a-b#c#d-e")` (:30-56): añade (no limpia); `Convert.ToDouble` con la cultura actual; un 3.er trozo se
    /// ignora; negativos no admitidos (`"-3"` ⇒ `ToDouble("")` lanza).
    public mutating func poner(_ valores: String, cultura: Cultura) throws(ErrorDominio) {
        for trozo in trozos(valores, "#") {
            let partes = trozos(trozo, "-")
            let min = try FormatoNET.convertToDouble(partes[0], cultura: cultura)
            let max = partes.count == 1 ? min : try FormatoNET.convertToDouble(partes[1], cultura: cultura)
            rangos.append(ClosedRangeNET(min: min, max: max))
        }
    }

    public mutating func reinicializar() { rangos = [] }

    /// `ValorEnRangoValido`: algún intervalo con `v <= max && v >= min`; vacío ⇒ `false`.
    public func contiene(_ v: Double) -> Bool { rangos.contains { v <= $0.max && v >= $0.min } }
}

/// Un intervalo de `RangosOpciones` (puede tener `min > max`, y entonces no contiene nada).
public struct ClosedRangeNET: Sendable, Hashable {
    public var min: Double
    public var max: Double
}

/// Puntuación por signo y partido; la suma (o el producto escalado) global y por signo debe caer en sus rangos.
public struct CondicionValoracionSignos: Sendable, Hashable {
    public var activa = false
    public var contieneDatos = false
    /// `TipoValoracion`: `"suma"` o `"multiplo"` (comparación exacta); otro texto no calcula nada.
    public var tipoValoracion = "suma"
    public var valores1: [Double]
    public var valoresX: [Double]
    public var valores2: [Double]
    public private(set) var valorGlobal = ""
    public private(set) var valorUnos = ""
    public private(set) var valorEquis = ""
    public private(set) var valorDoses = ""
    public private(set) var rangosGlobales = RangosOpciones()
    public private(set) var rangosUnos = RangosOpciones()
    public private(set) var rangosEquis = RangosOpciones()
    public private(set) var rangosDoses = RangosOpciones()

    public init(numeroPartidos: Int) {
        valores1 = Array(repeating: 0, count: numeroPartidos)
        (valoresX, valores2) = (valores1, valores1)
    }

    /// `ValorGlobal = s` (:256-291): guarda el texto, vacía los rangos y, si no es `""`, los pone.
    public mutating func ponerValorGlobal(_ s: String, cultura: Cultura) throws(ErrorDominio) {
        valorGlobal = s
        rangosGlobales.reinicializar()
        if !s.isEmpty { try rangosGlobales.poner(s, cultura: cultura) }
    }

    public mutating func ponerValorUnos(_ s: String, cultura: Cultura) throws(ErrorDominio) {
        valorUnos = s
        rangosUnos.reinicializar()
        if !s.isEmpty { try rangosUnos.poner(s, cultura: cultura) }
    }

    public mutating func ponerValorEquis(_ s: String, cultura: Cultura) throws(ErrorDominio) {
        valorEquis = s
        rangosEquis.reinicializar()
        if !s.isEmpty { try rangosEquis.poner(s, cultura: cultura) }
    }

    public mutating func ponerValorDoses(_ s: String, cultura: Cultura) throws(ErrorDominio) {
        valorDoses = s
        rangosDoses.reinicializar()
        if !s.isEmpty { try rangosDoses.poner(s, cultura: cultura) }
    }

    /// `ReinicializaValores` (:225-233): pone a 0 los N primeros de cada array.
    public mutating func reinicializaValores(numeroPartidos: Int) throws(ErrorDominio) {
        for i in 0..<numeroPartidos {
            try valores1.escribir(0, en: i, "FiltroValoracionSignos.cs:229")
            try valoresX.escribir(0, en: i, "FiltroValoracionSignos.cs:230")
            try valores2.escribir(0, en: i, "FiltroValoracionSignos.cs:231")
        }
    }
}
