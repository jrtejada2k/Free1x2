import Foundation

/// `TransponedorAutomatico` (ESPEC_06 §3.10; `A/TransponedorAutomatico.cs:27-89`): la transposición automática de la
/// rotación de signos del WinForms (C2 del plan de pruebas). Empareja los 14 partidos ordenados por concentración de la
/// valoración con los ordenados por concentración de la frecuencia.
public enum TransponedorAutomatico {
    /// `Trasposicion()`: `resultado[partido por frecuencia] = partido por valoración` (0…13); `nil` si `valoraciones` o
    /// `frecuencias` no traen 14 filas de 3 valores.
    public static func trasposicion(valoraciones v: [[Double]], frecuencias f: [[Double]]) -> [Int]? {
        guard v.count >= 14, f.count >= 14, (v.prefix(14) + f.prefix(14)).allSatisfy({ $0.count >= 3 }) else { return nil }
        var pv = pmedia(base100(v))
        var pf = pmedia(base100(f))
        ordenar(&pv)
        ordenar(&pf)
        var r = [Int](repeating: 0, count: 14)
        for i in 0..<14 { r[pf[i].indice] = pv[i].indice }
        return r
    }

    /// `base100`: cada fila entre su suma (suma 0 ⇒ NaN, como el C#).
    static func base100(_ x: [[Double]]) -> [[Double]] {
        (0..<14).map { i in
            let factor = x[i][0] + x[i][1] + x[i][2]
            return (0..<3).map { x[i][$0] / factor }
        }
    }

    /// `pmedia`: suma de cuadrados de la fila (`Math.Pow(x, 2)` = `x * x` en IEEE) y el índice del partido.
    static func pmedia(_ p: [[Double]]) -> [(valor: Double, indice: Int)] {
        (0..<14).map { i in (p[i][0] * p[i][0] + p[i][1] * p[i][1] + p[i][2] * p[i][2], i) }
    }

    /// Ordenación por intercambio, descendente y estricta (no estable): los empates quedan como los deja el C#.
    static func ordenar(_ m: inout [(valor: Double, indice: Int)]) {
        for x in 0..<13 {
            for y in (x + 1)..<14 where m[y].valor > m[x].valor { m.swapAt(x, y) }
        }
    }
}
