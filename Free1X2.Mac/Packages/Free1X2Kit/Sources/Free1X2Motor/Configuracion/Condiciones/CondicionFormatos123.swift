import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroFormatos123.cs y Formato123.cs (03 §3-4); transformación de FFormatos123Data.cs:69-122.

/// Un formato (cadena de dígitos 1-3) con su rango de apariciones.
public struct Formato123: Sendable, Hashable {
    public var formato: String
    public var aciertosMin: Int
    public var aciertosMax: Int

    public init(formato: String, aciertosMin: Int = 0, aciertosMax: Int = 0) {
        self.formato = formato
        self.aciertosMin = aciertosMin
        self.aciertosMax = aciertosMax
    }

    /// `FormatoLong` (`Formato123.cs:61-70`): `res = (res << 3) ^ (1 << "321".IndexOf(c))`; un carácter ajeno da `1 << -1`
    /// = `Int32.min`, que se extiende de signo (B3).
    public var formatoLong: Int64 {
        var res: Int64 = 0
        for c in formato.utf16 {
            let i: Int32 =
                switch c {
                case 0x33: 0
                case 0x32: 1
                case 0x31: 2
                default: -1
                }
            res = (res &<< 3) ^ Int64(Int32(1) &<< i)
        }
        return res
    }
}

/// Formatos de la cadena 1-2-3 de la columna (según una valoración por partido) con sus rangos de apariciones.
public struct CondicionFormatos123: Sendable, Hashable {
    public var activa = false
    public var formatos: [Formato123] = []
    /// `ArrayAciertos`: nº de formatos acertados permitidos.
    public var aciertos: [Int] = []
    public var aciertosFiltro: String?
    public var pasoFijo = false
    /// No se guardan en el `.comb`: solo los fija la pantalla (03 §3.1) [PRESERVAR].
    public var aciertosMax = 0
    public var aciertosMin = 0
    /// `Valoracion` `double[filas, 3]` (fila = partido, columnas `1`, `X`, `2`).
    public var valoracion: [[Double]]
    /// `ValoracionTranformada` `byte[filas, 3]`: puesto de cada signo, 1.º=4, 2.º=2, 3.º=1.
    public var valoracionTransformada: [[UInt8]]

    public init(numeroPartidos: Int) {
        valoracion = Array(repeating: [0, 0, 0], count: numeroPartidos)
        valoracionTransformada = Array(repeating: [0, 0, 0], count: numeroPartidos)
    }

    /// `ContieneDatos` = `NecesitaGuardar()`: hay formatos o aciertos (el setter no hace nada).
    public var contieneDatos: Bool { !formatos.isEmpty || !aciertos.isEmpty }

    /// `TransformarValoracion` de `FFormatos123Data` (:69-122): siempre 14 filas; con un `NaN` la fila queda `0,0,0`.
    public static func transformar(_ valoracion: [[Double]]) throws(ErrorDominio) -> [[UInt8]] {
        var t: [[UInt8]] = []
        for i in 0..<14 {
            let v = try valoracion.leer(i, "FFormatos123Data.cs:74")
            guard v.count == 3 else { throw .fueraDeRango("FFormatos123Data.cs:74") }
            t.append(puestos(v, bits: [4, 2, 1]))
        }
        return t
    }

    /// `Col1X2ToCol123` (pantalla, :93-96): mayúsculas y cadena de puestos `"1"`, `"2"`, `"3"`; lo que no es `1`/`X` cuenta
    /// como `2`.
    public func col1X2ToCol123(_ columna: String, numeroPartidos: Int) throws(ErrorDominio) -> String {
        var texto = ""
        for (i, c) in FormatoNET.mayusculas(columna).utf16.enumerated() {
            let fila = try valoracion.leer(i, "FiltroFormatos123.cs:318")
            guard fila.count == 3, i < numeroPartidos else { throw .fueraDeRango("FiltroFormatos123.cs:318") }
            let p = Self.puestos(fila, bits: [1, 2, 3])
            // Con un `NaN` la fila queda en `null` y concatenarla no añade nada.
            if p != [0, 0, 0] { texto += String(c == 0x31 ? p[0] : c == 0x58 ? p[1] : p[2]) }
        }
        return texto
    }

    /// Puestos de los tres signos por valor descendente (empates: `1` antes que `X` antes que `2`).
    static func puestos(_ v: [Double], bits b: [UInt8]) -> [UInt8] {
        if v[0] >= v[1] && v[0] >= v[2] {
            if v[1] >= v[2] { return [b[0], b[1], b[2]] }
            if v[2] > v[1] { return [b[0], b[2], b[1]] }
        } else if v[1] > v[0] && v[1] >= v[2] {
            return v[0] >= v[2] ? [b[1], b[0], b[2]] : [b[2], b[0], b[1]]
        } else if v[2] > v[0] && v[2] > v[1] {
            return v[0] >= v[1] ? [b[1], b[2], b[0]] : [b[2], b[1], b[0]]
        }
        return [0, 0, 0]
    }
}
