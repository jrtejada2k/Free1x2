// C#: S14N/N14S, S2n/N2s, S2N/N2S, S1n/N1s y s1n de 9 pantallas; Free1X2.Domain/Utils/Comparador2.cs:56-89
// (ESPEC_16 §6; 08 §1.4; 10:578, 741, 1002, 1064, 1804)

/// Columna ↔ número con `X`→0, `1`→1, `2`→2 (cualquier otro → 0) y el **primer** partido como dígito más
/// significativo. Es otra numeración que la de `ConvertidorDeBases`: no se mezclan (ESPEC_11 §4.4).
public enum CodificacionX0MSB {
    /// Lee los `partidos` primeros caracteres (`Substring`: si faltan, lanza).
    public static func numero(_ columna: String, partidos: Int) throws(ErrorDominio) -> Int32 {
        let texto = Array(columna.utf16)
        guard partidos <= texto.count else {
            throw ErrorDominio(.argumentoFueraDeRango, "La columna tiene menos de \(partidos) signos.", origen: "Comparador2.cs:48")
        }
        var n: Int32 = 0
        for c in texto.prefix(partidos) {
            n = n &* 3 &+ (c == 0x31 ? 1 : c == 0x32 ? 2 : 0)
        }
        return n
    }

    /// Inversa con longitud fija: `partidos` dígitos de menos a más significativo, `0` (y negativos) ⇒ `X`.
    public static func columna(_ numero: Int32, partidos: Int) -> String {
        var n = numero
        var signos = [Character](repeating: "X", count: partidos)
        for posicion in stride(from: partidos - 1, through: 0, by: -1) {
            switch n % 3 {
            case 1: signos[posicion] = "1"
            case 2: signos[posicion] = "2"
            default: break
            }
            n /= 3
        }
        return String(signos)
    }
}
