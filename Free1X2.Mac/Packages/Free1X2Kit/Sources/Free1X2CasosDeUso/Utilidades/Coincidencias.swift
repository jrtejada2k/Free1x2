import Foundation
import Free1X2Base
import Free1X2Formatos

// C#: Views/Ported/CoincidenciasViewModel.cs (08 §17; CU-A38…A42). Columnas con la codificación `S2N`/`N2S`
// (`FiltroPorGrupos.s1n/n1s`: X = 0, primer partido el más significativo).

/// Resultado de «Calcular»: las válidas (índice y coincidencias de la última condición) y las distribuciones.
public struct ResultadoCoincidencias: Sendable {
    public let procesadas: Int
    public let validas: [(indice: Int, coincidencias: Int)]
    /// `agrabarAC`, `agrabarGR` (109) y `agrabarAMC` (una fila por condición).
    public let ac: [Int]
    public let gr: [Int]
    public let amc: [[Int]]
}

extension CasosUtilidades {
    /// `InitTexs`.
    public static let descripcionesRadiografia = [
        "V14", "X14", "D14", "V7P", "X7P", "D7P", "V7U", "X7U", "D7U", "SSV14", "SSU14", "SSX14", "SSD14", "SSV7P", "SSU7P",
        "SSX7P", "SSD7P", "SSV7U", "SSU7U", "SSX7U", "SSD7U", "disV14", "disU14", "disX14", "disD14", "disV7P", "disU7P",
        "disX7P", "disD7P", "disV7U", "disU7U", "disX7U", "disD7U", "con(1x)14", "con(12)14", "con(x2)14", "con(11)14",
        "con(xx)14", "con(22)14", "con(1v)14", "con(xv)14", "con(2v)14", "con(vv)14", "con(1x)7P", "con(12)7P", "con(x2)7P",
        "con(11)7P", "con(xx)7P", "con(22)7P", "con(1v)7P", "con(xv)7P", "con(2v)7P", "con(vv)7P", "con(1x)7U", "con(12)7U",
        "con(x2)7U", "con(11)7U", "con(xx)7U", "con(22)7U", "con(1v)7U", "con(xv)7U", "con(2v)7U", "con(vv)7U", "PNG14", "PNV14",
        "PNU14", "PNX14", "PND14", "PNG7P", "PNV7P", "PNU7P", "PNX7P", "PND7P", "PNG7U", "PNV7U", "PNU7U", "PNX7U", "PND7U",
        "IG14", "IV14", "IU14", "IX14", "ID14", "IG7P", "IV7P", "IU7P", "IX7P", "ID7P", "IG7U", "IV7U", "IU7U", "IX7U", "ID7U",
        "ISG14", "ISV14", "ISU14", "ISX14", "ISD14", "ISG7P", "ISV7P", "ISU7P", "ISX7P", "ISD7P", "ISG7U", "ISV7U", "ISU7U",
        "ISX7U", "ISD7U",
    ]

    /// CU-A38 `Calcondis`: las 108 características de los 14 primeros signos (cualquier otro carácter cuenta como en el C#).
    public static func radiografia(_ columna: String) -> [Int] {
        var r = [Int](repeating: 0, count: 108)
        let c = Array(columna.utf16.prefix(14))
        guard c.count == 14 else { return r }
        let (uno, equis, dos) = (UInt16(0x31), UInt16(0x58), UInt16(0x32))
        // CalVX2
        for (h, mitad) in [c[0..<7], c[7..<14]].enumerated() {
            for ch in mitad where ch != uno {
                r[0] += 1
                r[3 + 3 * h] += 1
                let k = ch == dos ? 2 : 1
                r[k] += 1
                r[3 + 3 * h + k] += 1
            }
        }
        let pesos: [[Int]] = [
            [7, 5, 3, 1, 8, 6, 4, 2, 9, 7, 5, 3, 1, 8], [5, 1, 6, 2, 7, 3, 8, 4, 9, 5, 1, 6, 2, 7], Array(1...9) + Array(1...5),
        ]
        func peso(_ v: Int) -> Int {
            var p = v
            while p > 9 { p = p / 10 + p % 10 }
            return p
        }
        for (s, t) in [c, Array(c[0..<7]), Array(c[7..<14])].enumerated() {
            // CalSS: rachas V, U, X, D.
            var (n, m) = ([0, 0, 0, 0], [0, 0, 0, 0])
            for ch in t {
                for (k, si) in [ch != uno, ch == uno, ch == equis, ch == dos].enumerated() {
                    n[k] = si ? n[k] + 1 : 0
                    m[k] = max(m[k], n[k])
                }
            }
            for k in 0..<4 { r[9 + 4 * s + k] = m[k] }
            // CalDist: V (X y 2 juntos), U, X, D.
            let criterios: [(UInt16) -> Bool] = [{ $0 == equis || $0 == dos }, { $0 == uno }, { $0 == equis }, { $0 == dos }]
            for (k, es) in criterios.enumerated() {
                let pos = t.indices.filter { es(t[$0]) }
                r[21 + 4 * s + k] = zip(pos, pos.dropFirst()).map { $1 - $0 }.max() ?? 0
            }
            // CalContac: 1X, 12, X2, 11, XX, 22, 1V, XV, 2V, VV.
            var con = [Int](repeating: 0, count: 10)
            for (a, b) in zip(t, t.dropFirst()) {
                let ks: [Int] =
                    switch (a, b) {
                    case (uno, equis), (equis, uno): [0, 6]
                    case (uno, dos), (dos, uno): [1, 6]
                    case (equis, dos), (dos, equis): [2, 9, 7, 8]
                    case (uno, uno): [3]
                    case (equis, equis): [4, 9, 7]
                    case (dos, dos): [5, 9, 8]
                    default: []
                    }
                for k in ks { con[k] += 1 }
            }
            for k in 0..<10 { r[33 + 10 * s + k] = con[k] }
            // CalPN: G, V, U, X, D.
            var suma = [0, 0, 0]
            for (i, ch) in t.enumerated() {
                if let k = [uno, equis, dos].firstIndex(of: ch) { suma[k] += pesos[k][i] }
            }
            let (u, x, d) = (peso(suma[0]), peso(suma[1]), peso(suma[2]))
            let v = peso(x + d)
            for (k, p) in [peso(v + u), v, u, x, d].enumerated() { r[63 + 5 * s + k] = p }
            // CalInterr: nº de interrupciones y rachas de ellas, G, V, U, X, D.
            var (ni, ns, nr) = ([0, 0, 0, 0, 0], [0, 0, 0, 0, 0], [0, 0, 0, 0, 0])
            func cierra(_ ks: [Int]) {
                for k in ks {
                    ns[k] = max(ns[k], nr[k])
                    nr[k] = 0
                }
            }
            var ant = t[0]
            for act in t.dropFirst() {
                if act == ant {
                    cierra([0, 1, 2, 3, 4])
                } else {
                    ni[0] += 1
                    nr[0] += 1
                    if ant == uno {
                        ni[2] += 1
                        nr[2] += 1
                        cierra(act == dos ? [3] : [4])
                    } else if ant == dos {
                        ni[4] += 1
                        nr[4] += 1
                        if act == uno {
                            ni[1] += 1
                            nr[1] += 1
                            cierra([3])
                        } else {
                            cierra([2, 1])
                        }
                    } else {
                        ni[3] += 1
                        nr[3] += 1
                        if act == uno {
                            ni[1] += 1
                            nr[1] += 1
                            cierra([4])
                        } else {
                            cierra([2, 1])
                        }
                    }
                }
                ant = act
            }
            cierra([0, 1, 2, 3, 4])
            for k in 0..<5 {
                r[78 + 5 * s + k] = ni[k]
                r[93 + 5 * s + k] = ns[k]
            }
        }
        return r
    }

    /// CU-A39 `InitConds`: por condición, las características (1-based) y los nº de coincidencias admitidos; `nil` =
    /// «Error en condiciones» (texto no numérico, índices fuera de 1…109 / 0…108 o más de 100 condiciones).
    public static func condicionesCoincidencias(_ filas: [(grupos: String, rango: String)]) -> (grupos: [[Bool]], rangos: [[Bool]])? {
        guard !filas.isEmpty, filas.count <= 100 else { return nil }
        func marcar(_ texto: String, _ desfase: Int, _ hastaIncluido: Bool) -> [Bool]? {
            var m = [Bool](repeating: false, count: 109)
            for parte in texto.components(separatedBy: ",") {
                let ab = parte.components(separatedBy: "-")
                guard ab.count <= 2, let a = try? Int(FormatoNET.convertToInt32(ab[0])) else { return nil }
                let b = ab.count == 2 ? (try? Int(FormatoNET.convertToInt32(ab[1]))) : a
                guard let b else { return nil }
                for k in stride(from: a - desfase, to: b - desfase + (hastaIncluido ? 1 : 0), by: 1) {
                    guard m.indices.contains(k) else { return nil }
                    m[k] = true
                }
            }
            return m
        }
        var (g, r): ([[Bool]], [[Bool]]) = ([], [])
        for f in filas {
            guard let mg = marcar(f.grupos, 1, true), let mr = marcar(f.rango, 0, true) else { return nil }
            g.append(mg)
            r.append(mr)
        }
        return (g, r)
    }

    static func coincidencias(_ referencia: [Int], _ columna: String) -> [Bool] {
        zip(referencia, radiografia(columna)).map { $0 == $1 }
    }

    static func cuenta(_ grupo: [Bool], _ coin: [Bool]) -> Int { (0..<108).count { grupo[$0] && coin[$0] } }

    /// `LeerFileIn`: columnas sin repetir (por índice); si una línea tiene < 14 signos, lo leído y el aviso.
    @concurrent public static func entradaCoincidencias(_ datos: Data) async -> (entrada: ConjuntoBits, leidas: Int, error: String?) {
        var e = ConjuntoBits(cantidad: 4_782_969)
        var n = 0
        for l in TextoNET.lineas(datos) {
            let c = FormatoNET.mayusculas(FormatoNET.recortar(l))
            guard let i = try? FiltroPorGrupos.s1n(c) else { return (e, n, "Error longitud=" + c) }
            n += 1
            try? e.poner(i)
        }
        return (e, n, nil)
    }

    /// `EntraCGsA/R`: hasta 3000 ganadoras (sus 14 primeros signos); `corta` = la línea con < 14 signos (lo leído antes
    /// queda), `desborde` = más de 3000.
    public static func ganadorasCoincidencias(_ datos: Data) -> (columnas: [String], corta: String?, desborde: Bool) {
        var r: [String] = []
        for l in TextoNET.lineas(datos) {
            let t = FormatoNET.mayusculas(FormatoNET.recortar(l))
            guard t.utf16.count >= 14 else { return (r, t, false) }
            guard r.count < 3000 else { return (r, nil, true) }
            r.append(String(decoding: t.utf16.prefix(14), as: UTF16.self))
        }
        return (r, nil, false)
    }

    /// CU-A40 `Calcular`: valida cada columna de la entrada (por índice ascendente) contra la radiografía de referencia;
    /// al cancelar, lo procesado hasta entonces.
    @concurrent public static func filtrarCoincidencias(
        _ entrada: ConjuntoBits, referencia: [Int], grupos: [[Bool]], rangos: [[Bool]],
        progreso: @Sendable (_ procesadas: Int, _ validas: Int) -> Void
    ) async -> ResultadoCoincidencias {
        var validas: [(indice: Int, coincidencias: Int)] = []
        var (ac, gr) = ([Int](repeating: 0, count: 109), [Int](repeating: 0, count: 109))
        var amc = [[Int]](repeating: [Int](repeating: 0, count: 109), count: grupos.count)
        var procesadas = 0
        for idx in entrada.indices {
            if Task.isCancelled { break }
            procesadas += 1
            let coin = coincidencias(referencia, FiltroPorGrupos.n1s(idx))
            var nc = 0
            var valida = true
            for ng in grupos.indices where valida {
                nc = cuenta(grupos[ng], coin)
                valida = rangos[ng][nc]
            }
            if valida {
                validas.append((idx, nc))
                ac[nc] += 1
                for k in 0..<108 where coin[k] { gr[k + 1] += 1 }
                for ng in grupos.indices { amc[ng][cuenta(grupos[ng], coin)] += 1 }
            }
            if procesadas & 0xFFF == 0 { progreso(procesadas, validas.count) }
        }
        return ResultadoCoincidencias(procesadas: procesadas, validas: validas, ac: ac, gr: gr, amc: amc)
    }

    /// CU-A41 `Analizar`: coincidencias por condición entre las radiografías de dos columnas.
    public static func analizarCoincidencias(_ anterior: String, _ reciente: String, grupos: [[Bool]]) -> [Int] {
        let coin = coincidencias(radiografia(anterior), reciente)
        return grupos.map { cuenta($0, coin) }
    }

    /// «Grabar resultado»: las válidas cuya fila de la distribución está marcada.
    public static func columnasCoincidencias(_ r: ResultadoCoincidencias, marcadas: [Bool]) -> [String] {
        r.validas.filter { marcadas.indices.contains($0.coincidencias) && marcadas[$0.coincidencias] }.map {
            FiltroPorGrupos.n1s($0.indice)
        }
    }

    /// CU-A42 `GrabExcel` (texto tabulado): N, s/AC, N, s/GR y N, AMCk por cada una de las `condiciones`.
    public static func excelCoincidencias(_ r: ResultadoCoincidencias?, condiciones: Int) -> [String] {
        let t = "\t"
        var l = ["N\ts/AC\tN\ts/GR" + (0..<condiciones).map { "\(t)N\(t)AMC\($0 + 1)" }.joined()]
        for n in 0..<109 {
            let amc = (0..<condiciones).map { ng in "\(t)\(n)\(t)\(r.map { ng < $0.amc.count ? $0.amc[ng][n] : 0 } ?? 0)" }
            l.append("\(n)\(t)\(r?.ac[n] ?? 0)\(t)\(n)\(t)\(r?.gr[n] ?? 0)" + amc.joined())
        }
        return l
    }
}
