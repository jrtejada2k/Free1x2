import Foundation
import Free1X2Base

// Ref.: 09 §57. M2 (#18): con los valores 1 / X / 2 de los 14 partidos forma 7 rankings de las 6 casillas de más valor que
// quedan; una columna pasa si sus aciertos con cada ranking y el recorrido (máximo − mínimo de los 7) caen en sus rangos.
// No tiene suma: esa es del filtro Aidomnou.

extension CasosUtilidades {
    /// `rankings[partido][ranking]`: la suma de los signos elegidos en ese partido (1 = «1», 4 = «X», 2 = «2»). Cada casilla
    /// es la de más valor que queda, con «mayor estricto»: en un empate, la primera en P1…P14 y, en cada partido, 1, X, 2.
    public static func rankingsPim(_ valores: [[Double]]) throws(ErrorDominio) -> [[Int]] {
        guard valores.count >= 14, valores.prefix(14).allSatisfy({ $0.count >= 3 }) else {
            throw ErrorDominio(.argumento, "Hacen falta los valores 1 / X / 2 de los 14 partidos.")
        }
        var quedan = valores.prefix(14).map { Array($0.prefix(3)) }
        var r = [[Int]](repeating: Array(repeating: 0, count: 7), count: 14)
        for k in 0..<7 {
            for _ in 0..<6 {
                var (maximo, partido, signo) = (-1.0, 0, 0)
                for p in 0..<14 {
                    for s in 0..<3 where quedan[p][s] > maximo { (maximo, partido, signo) = (quedan[p][s], p, s) }
                }
                quedan[partido][signo] = -1
                r[partido][k] += [1, 4, 2][signo]
            }
        }
        return r
    }

    /// Aciertos de la columna con cada ranking. La «x» vale como «X»; menos de 14 signos es un error.
    public static func aciertosPim(_ rankings: [[Int]], columna: String) throws(ErrorDominio) -> [Int] {
        let u = Array(columna.utf16)
        guard u.count >= 14 else { throw ErrorDominio(.argumento, "La columna debe tener 14 signos.") }
        let bits = u.prefix(14).map { c -> Int in c == 0x31 ? 1 : c == 0x32 ? 2 : c == 0x58 || c == 0x78 ? 4 : 0 }
        return (0..<7).map { k in (0..<14).count(where: { rankings[$0][k] & bits[$0] != 0 }) }
    }

    /// El mayor menos el menor de los 7 aciertos.
    public static func recorridoPim(_ aciertos: [Int]) -> Int { (aciertos.max() ?? 0) - (aciertos.min() ?? 0) }

    /// `rangos`: los 8 pares mínimo-máximo de R1…R7 y del recorrido (`rangosAidomnou`).
    public static func admitePim(_ aciertos: [Int], _ rangos: [[Int]]) -> Bool {
        for k in 0..<7 where aciertos[k] < rangos[k][0] || aciertos[k] > rangos[k][1] { return false }
        let r = recorridoPim(aciertos)
        return r >= rangos[7][0] && r <= rangos[7][1]
    }

    /// «Calcular»: cada línea en mayúsculas; las repetidas no se validan otra vez y las admitidas quedan en el orden del
    /// fichero. Una línea de menos de 14 signos (también una vacía) para la lectura y se devuelve `corta`.
    @concurrent public static func filtrarPim(
        _ datos: Data, rankings: [[Int]], rangos: [[Int]]
    ) async -> (admitidas: [String], procesadas: Int, corta: Bool) {
        var vistas = ConjuntoBits(cantidad: 4_782_969)
        var (admitidas, n) = ([String](), 0)
        for l in TextoNET.lineas(datos) {
            if Task.isCancelled { break }
            let col = FormatoNET.mayusculas(FormatoNET.recortar(l))
            n += 1
            guard col.utf16.count >= 14, let i = try? FiltroPorGrupos.s1n(col) else { return (admitidas, n, true) }
            guard !((try? vistas.contiene(i)) ?? true) else { continue }
            try? vistas.poner(i)
            if let a = try? aciertosPim(rankings, columna: col), admitePim(a, rangos) { admitidas.append(col) }
        }
        return (admitidas, n, false)
    }

    /// «Exportar columnas»: los 7 rankings, una línea cada uno con los 14 partidos («-», «1», «X2»…) separados por comas.
    public static func exportarPim(_ rankings: [[Int]]) -> [String] {
        (0..<7).map { k in (0..<14).map { cambiaAidomnou(rankings[$0][k]) }.joined(separator: ",") }
    }
}
