import Free1X2Base

// C#: Free1X2.Domain/Utils/RangosHelper.cs (06 §2.5)

/// Texto de rangos `"1,3-5,7"` → enteros o `[Bool]` indexado por valor.
public enum RangosHelper {
    /// Límite de memoria razonable para `obtenBoolArray` (el C# pediría `bool[10^9]`; 06 §2.5-6).
    static let maximoValor = 10_000_000

    /// `ObtenIntArray` (:32-74): `"a-b"` ⇒ a…b (invertido ⇒ nada); con 3 o más partes, solo la primera; sin deduplicar.
    public static func obtenIntArray(_ texto: String) throws(ErrorDominio) -> [Int] {
        var salida: [Int] = []
        for parte in trozos(texto, ",") {
            let t = trozos(parte, "-")
            if t.count == 2 {
                let a = Int(try FormatoNET.convertToInt32(t[0]))
                let b = Int(try FormatoNET.convertToInt32(t[1]))
                if b >= a { salida.append(contentsOf: a...b) }
            } else {
                salida.append(Int(try FormatoNET.convertToInt32(t[0])))
            }
        }
        return salida
    }

    /// `ObtenBoolArray` (:76-141): longitud = máximo de **todas** las subpartes + 2; el último elemento siempre `false`.
    public static func obtenBoolArray(_ texto: String) throws(ErrorDominio) -> [Bool] {
        let partes = trozos(texto, ",")
        var maximo = 0
        for parte in partes {
            for sub in trozos(parte, "-") { maximo = max(maximo, Int(try FormatoNET.convertToInt32(sub))) }
        }
        guard maximo <= maximoValor else {
            throw ErrorDominio(.argumentoFueraDeRango, "Valor demasiado grande: \(maximo).", origen: "RangosHelper.cs:113")
        }
        var valores = [Bool](repeating: false, count: maximo + 2)
        for parte in partes {
            if parte.contains("-") {
                let t = trozos(parte, "-")
                let a = Int(try FormatoNET.convertToInt32(t[0]))
                let b = Int(try FormatoNET.convertToInt32(t[1]))
                if b >= a {
                    for i in a...b { try marcar(&valores, i) }
                }
            } else {
                try marcar(&valores, Int(try FormatoNET.convertToInt32(parte)))
            }
        }
        return valores
    }

    private static func marcar(_ valores: inout [Bool], _ i: Int) throws(ErrorDominio) {
        guard valores.indices.contains(i) else {
            throw ErrorDominio(.argumentoFueraDeRango, "El índice estaba fuera del intervalo.", origen: "RangosHelper.cs:132")
        }
        valores[i] = true
    }
}

/// Texto de rangos y su `[Bool]` (D2/P4): el par `strX`/`x` de CP, GE y controles. `""` ⇒ `nil` = sin restricción.
public struct ConjuntoRangos: Sendable, Hashable {
    public private(set) var texto = ""
    public private(set) var valores: [Bool]?

    public init() {}

    /// El texto se fija antes de parsear: si lanza, queda el texto nuevo con el array anterior, como en C#.
    public mutating func poner(_ s: String) throws(ErrorDominio) {
        texto = s
        if s.isEmpty {
            valores = nil
        } else {
            valores = try RangosHelper.obtenBoolArray(s)
        }
    }

    public var activo: Bool { !texto.isEmpty }

    /// `arr.Length >= n+1 && arr[n]`.
    func permite(_ n: Int) -> Bool {
        guard let v = valores, v.indices.contains(n) else { return false }
        return v[n]
    }
}

/// `string.Split(char)` de .NET: conserva los trozos vacíos.
@inline(__always)
func trozos(_ texto: some StringProtocol, _ separador: Character) -> [String] {
    texto.split(separator: separador, maxSplits: .max, omittingEmptySubsequences: false).map { String($0) }
}
