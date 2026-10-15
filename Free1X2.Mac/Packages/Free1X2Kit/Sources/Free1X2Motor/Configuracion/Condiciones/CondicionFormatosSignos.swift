import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroFormatosSignos.cs, FormatosSignos.cs y FormatoSignos.cs (03 §9-11).

/// Una línea: un patrón de signos (`1`, `X`, `2`, `V` = X o 2, `*` = cualquiera) y el rango de apariciones.
public struct FormatoSignos: Sendable, Hashable {
    public private(set) var formato = ""
    /// `_Longformato`: códigos del patrón (el 1.er carácter en el grupo más alto) con todos los bits superiores a 1.
    public private(set) var longFormato: Int64 = 0
    /// `_ValMin`: `1 << ((n − 1)·3)` (contador enmascarado a 6 bits).
    public private(set) var valMin: Int64 = 0
    public private(set) var rangoAparicion = ""
    public private(set) var arrayRangos: [Bool]?

    public init() {}

    /// `Formato = v` (:67-75): en mayúsculas.
    public mutating func ponerFormato(_ v: String) {
        formato = FormatoNET.mayusculas(v)
        var res: Int64 = ~0
        for c in formato.utf16 {
            // `" 2XV1  *".IndexOf(c)` como `uint`: un carácter ajeno da `0xFFFFFFFF` (B2).
            let codigo: Int64 =
                switch c {
                case 0x20: 0
                case 0x32: 1
                case 0x58: 2
                case 0x56: 3
                case 0x31: 4
                case 0x2A: 7
                default: 0xFFFF_FFFF
                }
            res = (res &<< 3) | codigo
        }
        longFormato = res
        valMin = Int64(1) &<< Int64((formato.utf16.count - 1) * 3)
    }

    /// `RangoAparicion = s` (:88-101): `""` deja el array como estaba (`nil` ⇒ `analiza` lanza).
    public mutating func ponerRangoAparicion(_ s: String) throws(ErrorDominio) {
        rangoAparicion = s
        if !s.isEmpty { arrayRangos = try RangosHelper.obtenBoolArray(s) }
    }
}

/// Un grupo de líneas con las restricciones opcionales `Lineas` (cuántas se cumplen) y `Global` (suma de apariciones).
public struct FormatosSignos: Sendable, Hashable {
    public var lineasFormatos: [FormatoSignos] = []
    public private(set) var lineas = ""
    public private(set) var global = ""
    public private(set) var arrayLineas: [Bool]?
    public private(set) var arrayGlobal: [Bool]?

    public init() {}

    public mutating func ponerLineas(_ s: String) throws(ErrorDominio) {
        lineas = s
        if !s.isEmpty { arrayLineas = try RangosHelper.obtenBoolArray(s) }
    }

    public mutating func ponerGlobal(_ s: String) throws(ErrorDominio) {
        global = s
        if !s.isEmpty { arrayGlobal = try RangosHelper.obtenBoolArray(s) }
    }
}

/// Grupos de formatos de signos (todos deben cumplirse).
public struct CondicionFormatosSignos: Sendable, Hashable {
    public var activa = false
    public var contieneDatos = false
    public private(set) var formatosSignos: [FormatosSignos] = []

    public init() {}

    /// `FormatosSignos = lista` (:116-133): `contieneDatos` = hay grupos.
    public mutating func ponerFormatosSignos(_ lista: [FormatosSignos]) {
        formatosSignos = lista
        contieneDatos = !lista.isEmpty
    }
}
