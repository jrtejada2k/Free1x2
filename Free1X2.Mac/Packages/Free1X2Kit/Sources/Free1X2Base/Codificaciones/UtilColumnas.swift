// C#: Free1X2.Domain/Utils/UtilColumnas.cs (06 §1.4, §2.1). Solo lo que tiene consumidores.

/// Conversiones texto ↔ `Columna` de 3 bits. El partido 1 ocupa los bits bajos.
public enum UtilColumnas {
    private static let signos: [UInt16] = Array("2X1".utf16)

    /// Peso de un carácter en la columna: `2`→1, `X`→2, `1`→4. Cualquier otro: `IndexOf = −1` y C# hace `1 << −1`
    /// (contador enmascarado a 5 bits) = `Int32.min`, que se extiende con signo al combinarse con `long` [PRESERVAR].
    @inline(__always)
    static func peso(_ c: UInt16) -> Int64 {
        Int64(Int32(1) &<< indice(c))
    }

    @inline(__always)
    private static func indice(_ c: UInt16) -> Int32 {
        switch c {
        case 0x32: 0  // '2'
        case 0x58: 1  // 'X'
        case 0x31: 2  // '1'
        default: -1
        }
    }

    /// `ConvStrToLong(string)` (:196-204).
    public static func convStrToLong(_ s: String) -> Columna {
        var res: Int64 = 0
        for c in s.utf16.reversed() { res = (res &<< 3) ^ peso(c) }
        return res
    }

    /// `ConvStrToLong(s, int)` (:186-195): completa con `faltan` unos (1…15).
    public static func convStrToLong(_ s: String, faltan: Int) throws(ErrorDominio) -> Columna {
        guard (1...15).contains(faltan) else { throw fueraDeRango("UtilColumnas.cs:188") }
        return convStrToLong(s + String(repeating: "1", count: faltan))
    }

    /// `ConvStrToLong(s, string relleno)` (:168-181): relleno vacío ⇒ completa hasta `numeroPartidos` con unos.
    public static func convStrToLong(_ s: String, relleno: String, numeroPartidos: Int) throws(ErrorDominio) -> Columna {
        if relleno.isEmpty { return try convStrToLong(s, faltan: numeroPartidos - s.utf16.count) }
        return convStrToLong(s + relleno)
    }

    /// `ConvStrToLong(s, bool[])` (:205-220): los partidos no marcados quedan a 0.
    public static func convStrToLong(_ s: String, marcados: [Bool]) throws(ErrorDominio) -> Columna {
        let texto = Array(s.utf16)
        guard marcados.count >= texto.count else { throw fueraDeRango("UtilColumnas.cs:210") }
        var res: Int64 = 0
        for i in stride(from: texto.count - 1, through: 0, by: -1) {
            res = marcados[i] ? (res &<< 3) ^ peso(texto[i]) : res &<< 3
        }
        return res
    }

    /// `ConvStrToLong(string[])` (:78-92): pronóstico por partido; solo lee las posiciones pares (`"1,X,2"`→7, `"1X"`→4).
    public static func convStrToLong(pronosticos: [String]) -> Columna {
        var columna: Int64 = 0
        for pronostico in pronosticos.reversed() {
            let texto = Array(pronostico.utf16)
            var b: Int32 = 0
            for j in stride(from: 0, to: texto.count, by: 2) { b |= Int32(1) &<< indice(texto[j]) }
            columna = (columna &<< 3) | Int64(UInt32(bitPattern: b))
        }
        return columna
    }

    /// `ConvLongToStr` (:27-44): slots 6 y 7 y los negativos (más de 22 caracteres) lanzan `IndexOutOfRange`.
    public static func convLongToStr(_ columna: Columna) throws(ErrorDominio) -> String {
        var l = columna
        var salida: [UInt16] = []
        salida.reserveCapacity(16)
        while l != 0 {
            let k = Int(l & 7) >> 1
            guard salida.count < 22, k < 3 else { throw fueraDeRango("UtilColumnas.cs:40") }
            salida.append(signos[k])
            l >>= 3
        }
        return String(decoding: salida, as: UTF16.self)
    }

    /// `ObtenerSignoInt` (:66-76): contador de desplazamiento enmascarado a 6 bits, como C#.
    @inline(__always)
    public static func obtenerSignoInt(_ columna: Columna, partido: Int) -> Int {
        Int((columna &>> Int64((partido - 1) * 3)) & 7)
    }

    /// `ConvPartidoStrToByte` (:94-132): sensible a mayúsculas y al orden; desconocido ⇒ 0.
    public static func convPartidoStrToByte(_ partido: String) -> UInt8 {
        switch partido {
        case "1": 4
        case "X": 2
        case "2": 1
        case "1X", "1,X": 6
        case "12", "1,2": 5
        case "X2", "X,2": 3
        case "1X2", "1,X,2": 7
        default: 0
        }
    }

    /// `ConvPartidoByteToStr` (:134-167).
    public static func convPartidoByteToStr(_ partido: UInt8) -> String {
        switch partido {
        case 4: "1"
        case 2: "X"
        case 1: "2"
        case 6: "1X"
        case 5: "12"
        case 3: "X2"
        case 7: "1X2"
        default: ""
        }
    }

    /// `ObtenerStringFromLongApuestaMultiple` (:379-388): `0` lanza (`Substring(0, −1)`); un negativo no termina en C#.
    public static func obtenerStringFromLongApuestaMultiple(_ columna: Columna) throws(ErrorDominio) -> String {
        guard columna > 0 else {
            throw ErrorDominio(
                columna == 0 ? .argumentoFueraDeRango : .operacionInvalida, "", origen: "UtilColumnas.cs:387")
        }
        var l = columna
        var partes: [String] = []
        while l != 0 {
            partes.append(convPartidoByteToStr(UInt8(truncatingIfNeeded: l & 7)))
            l >>= 3
        }
        return partes.joined(separator: ",")
    }

    /// `IgnorarPartidos` (:429-444): compacta los partidos marcados (`partidos[0]` no se usa; el de menor índice abajo).
    public static func ignorarPartidos(_ partidos: [Bool], _ columna: Columna) -> Columna {
        var mascara: Int64 = 0
        for i in stride(from: partidos.count - 1, to: 0, by: -1) where partidos[i] {
            mascara = (mascara &<< 3) | Int64(obtenerSignoInt(columna, partido: i))
        }
        return mascara
    }

    /// `ContarBitsA1` (:299-309): bits a 1 de los 64.
    @inline(__always)
    public static func contarBitsA1(_ b: Int64) -> Int { b.nonzeroBitCount }

    /// Aciertos entre dos apuestas simples: bits comunes (06 §1.1; ESPEC_17 COL-08).
    @inline(__always)
    public static func aciertos(_ a: Columna, _ b: Columna) -> Int { (a & b).nonzeroBitCount }

    private static func fueraDeRango(_ origen: String) -> ErrorDominio {
        ErrorDominio(.indiceFueraDeRango, "Índice fuera de los límites de la matriz.", origen: origen)
    }
}
