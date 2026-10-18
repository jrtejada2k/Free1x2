import Foundation
import Free1X2Base
import Free1X2Formatos

// C#: Views/Ported/aidomnouViewModel.cs (10 §105; CU-C58…C62) y GeneraPimViewModel.cs (09 §57), que llevan el mismo
// motor (DEC-922). `lims` fijos (DEC-923). Columnas con `S2n`/`N2s` = `FiltroPorGrupos.s1n/n1s`.

/// Las 6 columnas probables.
public struct ColumnasAidomnou: Sendable {
    /// `cps[partido][columna]`: máscara 1 = «1», 4 = «X», 2 = «2».
    public let cps: [[Int]]
    /// `scps`: los 14 códigos de cada columna concatenados como dígitos (más de 14 si alguno pasa de 9).
    let scps: [[UInt16]]
}

extension CasosUtilidades {
    static let limsAidomnou = [145, 11, 9, 8, 65, 9, 6, 5]

    /// `Cambia`.
    public static func cambiaAidomnou(_ v: Int) -> String {
        switch v {
        case 0: "-"
        case 1: "1"
        case 2: "2"
        case 3: "12"
        case 4: "X"
        case 5: "1X"
        case 6: "X2"
        default: "1X2"
        }
    }

    /// CU-C58 `PreparaColumnas` con las valoraciones 1/X/2 de los 14 primeros partidos.
    public static func columnasAidomnou(_ valores: [[Double]]) throws(ErrorDominio) -> ColumnasAidomnou {
        var nv: [[Double]] = []
        for p in 0..<14 {
            let f = try valores.leer(p, "aidomnouViewModel.cs:PreparaColumnas")
            nv.append([try f.leer(0, "PreCol"), try f.leer(1, "PreCol"), try f.leer(2, "PreCol")])
        }
        let lims = limsAidomnou
        var cps = [[Int]](repeating: [Int](repeating: 0, count: 6), count: 14)
        let codigo = { (s: Int) in s == 0 ? 1 : s == 1 ? 4 : 2 }
        // PreCol1 / PreCol4: el mayor valor restante («>=»: en empate el último) hasta sumar `tope` puntos.
        func porPuntos(_ c: Int, _ vueltas: Int, _ tope: Int) {
            var pv = nv
            for _ in 0..<vueltas {
                var (q1, q2, v) = (0, 0, 0.0)
                for p in 0..<14 {
                    for s in 0..<3 where pv[p][s] >= v { (v, q1, q2) = (pv[p][s], p, s) }
                }
                cps[q1][c] += codigo(q2)
                pv[q1][q2] = -1
                let puntos = (0..<14).reduce(0) { $0 + ([3, 5, 6].contains(cps[$1][c]) ? 10 : cps[$1][c] == 7 ? 15 : 0) }
                if puntos >= tope { break }
            }
        }
        // PreCol2 / PreCol5: unos («>=»), doses y equis («>»); sin candidato, el partido 0.
        func porCantidades(_ c: Int, _ unos: Int, _ equis: Int, _ doses: Int) {
            for (veces, s, bit, estricto) in [(unos, 0, 1, false), (doses, 2, 2, true), (equis, 1, 4, true)] {
                for _ in 0..<veces {
                    var (q, v) = (0, 0.0)
                    for p in 0..<14 where cps[p][c] < bit && (estricto ? nv[p][s] > v : nv[p][s] >= v) { (v, q) = (nv[p][s], p) }
                    cps[q][c] += bit
                }
            }
        }
        porPuntos(0, 42, lims[0])
        porCantidades(1, lims[1], lims[2], lims[3])
        // PreCol3: 14 dobles sin el menos valorado.
        for p in 0..<14 {
            var (q, v) = (0, 100.0)
            for s in 0..<3 where nv[p][s] < v { (v, q) = (nv[p][s], s) }
            cps[p][2] = [6, 3, 5][q]
        }
        // ponytail: el C# hace `while (true)`; toda vuelta que termina lo hace antes de 50 (42 casillas + 8 del partido 0),
        // así que 64 solo corta el bucle infinito de valoraciones vacías (NaN).
        porPuntos(3, 64, lims[4])
        porCantidades(4, lims[5], lims[6], lims[7])
        // PreCol6: 14 fijos con el más valorado («>», todo 0 → «1»).
        for p in 0..<14 {
            var (q, v) = (0, 0.0)
            for s in 0..<3 where nv[p][s] > v { (v, q) = (nv[p][s], s) }
            cps[p][5] = codigo(q)
        }
        let scps = (0..<6).map { c in Array((0..<14).map { String(cps[$0][c]) }.joined().utf16) }
        return ColumnasAidomnou(cps: cps, scps: scps)
    }

    /// CU-C59 `ParseaRango` ×8 (6 columnas, suma y recorrido): `nil` = «error en datos de entrada».
    public static func rangosAidomnou(_ textos: [String]) -> [[Int]]? {
        var r: [[Int]] = []
        for t in textos {
            let a = t.components(separatedBy: "-")
            guard a.count >= 2, let x = try? FormatoNET.convertToInt32(a[0]), let y = try? FormatoNET.convertToInt32(a[1]) else {
                return nil
            }
            r.append([Int(x), Int(y)])
        }
        return r
    }

    /// Aciertos de una columna (con «X» ya como «4») contra cada una de las 6: AND de códigos de carácter, como el C#.
    public static func aciertosAidomnou(_ c: ColumnasAidomnou, columna: String) throws(ErrorDominio) -> [Int] {
        let u = Array(columna.utf16)
        var r: [Int] = []
        for banda in c.scps {
            var na = 0
            for p in 0..<14 where banda[p] != 48 {
                if banda[p] & (try u.leer(p, "aidomnouViewModel.cs:Analizar")) > 48 { na += 1 }
            }
            r.append(na)
        }
        return r
    }

    /// `Valida`: cada columna, la suma y el recorrido (máx − mín) dentro de su rango.
    static func validaAidomnou(_ na: [Int], _ rks: [[Int]]) -> Bool {
        for (c, n) in na.enumerated() where n < rks[c][0] || n > rks[c][1] { return false }
        let (suma, reco) = (na.reduce(0, +), (na.max() ?? 0) - (na.min() ?? 0))
        return suma >= rks[6][0] && suma <= rks[6][1] && reco >= rks[7][0] && reco <= rks[7][1]
    }

    /// CU-C60 `Calcular`: las válidas sin repetir, por índice. Aidomnou salta las líneas en blanco y Pim no; una línea
    /// de menos de 14 signos (devuelta en `corta`) para la lectura.
    @concurrent public static func filtrarAidomnou(
        _ datos: Data, columnas c: ColumnasAidomnou, rangos: [[Int]], saltarVacias: Bool
    ) async -> (validas: ConjuntoBits, procesadas: Int, admitidas: Int, corta: String?) {
        var v = ConjuntoBits(cantidad: 4_782_969)
        var (n, adm) = (0, 0)
        for l in TextoNET.lineas(datos) {
            if Task.isCancelled { break }
            var col = FormatoNET.recortar(l)
            if saltarVacias && col.isEmpty { continue }
            n += 1
            guard col.utf16.count >= 14 else { return (v, n, adm, col) }
            col = col.replacingOccurrences(of: "x", with: "4").replacingOccurrences(of: "X", with: "4")
            guard let na = try? aciertosAidomnou(c, columna: col), validaAidomnou(na, rangos),
                let i = try? FiltroPorGrupos.s1n(col), !((try? v.contiene(i)) ?? true)
            else { continue }
            try? v.poner(i)
            adm += 1
        }
        return (v, n, adm, nil)
    }

    /// «Grabar resultado»: las válidas por índice ascendente.
    @concurrent public static func columnasValidasAidomnou(_ v: ConjuntoBits) async -> [String] { v.indices.map(FiltroPorGrupos.n1s) }

    /// «Exportar columnas»: una línea por columna con los 14 `Cambia` separados por comas.
    public static func exportarAidomnou(_ c: ColumnasAidomnou) -> [String] {
        (0..<6).map { col in (0..<14).map { cambiaAidomnou(c.cps[$0][col]) }.joined(separator: ",") }
    }

    /// CU-C62 `VerColumna` en cada línea: los 14 primeros signos de «12xX» (la x tal cual); `nil` = «col.G. errónea».
    public static func ganadorasAidomnou(_ datos: Data) -> [String]? {
        var r: [String] = []
        for l in TextoNET.lineas(datos) {
            let u = Array(FormatoNET.recortar(l).utf16.prefix(14))
            guard u.count == 14, u.allSatisfy({ "12xX".utf16.contains($0) }) else { return nil }
            r.append(String(decoding: u, as: UTF16.self))
        }
        return r
    }
}
