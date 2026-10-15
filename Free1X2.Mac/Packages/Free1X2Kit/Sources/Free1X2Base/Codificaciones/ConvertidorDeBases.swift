// C#: Free1X2.Domain/Utils/ConvertidorDeBases.cs (06 §1.7, §2.4)

/// Columna ↔ número en base 3 que indexa los conjuntos de 3^N columnas: `1`→0, `X`→1, `2`→2;
/// el partido 1 es el dígito **menos** significativo. Cualquier carácter distinto de `1`/`2` pesa como `X`.
public struct ConvertidorDeBases: Sendable {
    /// 3^0 … 3^16 (:28).
    public static let potenciasDe3: [Int32] = [
        1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907, 43046721,
    ]
    private static let signos: [Character] = ["1", "X", "2", " "]

    public let numeroPartidos: Int

    public init(numeroPartidos: Int) {
        self.numeroPartidos = numeroPartidos
    }

    /// `ConvColumnaANumero(columna)` (:43-54).
    public func numero(_ columna: String) throws(ErrorDominio) -> IndiceBase3 {
        try Self.numero(Array(columna.utf16), hasta: columna.utf16.count)
    }

    /// `ConvColumnaANumero(columna, numPartidos)` (:55-66): lee los `partidos` primeros caracteres.
    public func numero(_ columna: String, partidos: Int) throws(ErrorDominio) -> IndiceBase3 {
        try Self.numero(Array(columna.utf16), hasta: partidos)
    }

    private static func numero(_ texto: [UInt16], hasta n: Int) throws(ErrorDominio) -> IndiceBase3 {
        var b: Int32 = 0
        for i in 0..<n {
            guard i < texto.count else { throw fueraDeRango(":59") }
            switch texto[i] {
            case 0x31: break  // '1'
            case 0x32:  // '2'
                guard i < potenciasDe3.count else { throw fueraDeRango(":50") }
                b &+= 2 &* potenciasDe3[i]
            default:
                guard i < potenciasDe3.count else { throw fueraDeRango(":51") }
                b &+= potenciasDe3[i]
            }
        }
        return b
    }

    /// `ConvNumAColumna` (:96-117): relleno con `1` a la derecha hasta `numeroPartidos`.
    public func columna(_ numero: Int64) throws(ErrorDominio) -> String {
        var n = numero
        var texto = ""
        repeat {
            let resto = Int(n % 3)
            guard resto >= 0 else { throw Self.fueraDeRango(":104") }
            texto.append(Self.signos[resto])
            n /= 3
        } while n != 0
        let faltan = numeroPartidos - texto.count
        guard faltan >= 0 else {
            throw ErrorDominio(
                .argumentoFueraDeRango, "El número no cabe en \(numeroPartidos) partidos.", origen: "ConvertidorDeBases.cs:107")
        }
        return texto + String(repeating: "1", count: faltan)
    }

    /// `ObtenTamañoBitArray` (:118-121) = 3^longitud.
    public static func tamanoConjunto(partidos: Int) throws(ErrorDominio) -> Int {
        guard potenciasDe3.indices.contains(partidos) else { throw fueraDeRango(":120") }
        return Int(potenciasDe3[partidos])
    }

    /// `ObtenNumeroPartidosColBin` (:122-…): k si `longitud` = 3^k; si no, 14.
    public static func numeroPartidos(tamanoConjunto longitud: Int) -> Int {
        potenciasDe3.firstIndex { Int($0) == longitud } ?? 14
    }

    private static func fueraDeRango(_ linea: String) -> ErrorDominio {
        ErrorDominio(.indiceFueraDeRango, "Índice fuera de los límites de la matriz.", origen: "ConvertidorDeBases.cs\(linea)")
    }
}
