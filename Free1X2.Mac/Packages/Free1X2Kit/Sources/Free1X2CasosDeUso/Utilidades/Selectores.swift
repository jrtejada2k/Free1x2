import Foundation
import Free1X2Base
import Free1X2Formatos

// C#: Views/Ported/SelecJMViewModel.cs (10 §86; CU-C23…C27) y SelectorMSViewModel.cs (10 §87; CU-C28, CU-C29). Ambas
// usan la base 3 propia de `FiltroPorGrupos.s1n`/`n1s` (X = 0, 1 = 1, 2 = 2; primer partido el más significativo).

extension CasosUtilidades {
    /// `CargarGanadoras` de los dos selectores: `Trim().ToUpper()`, vacías fuera; corta en la primera de menos de 14 y la
    /// devuelve en `erronea`.
    public static func ganadorasSelector(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> (columnas: [String], erronea: String?) {
        var r: [String] = []
        for l in TextoNET.lineas(try entorno.archivos.leer(url)) {
            let t = FormatoNET.mayusculas(FormatoNET.recortar(l))
            if t.isEmpty { continue }
            if t.utf16.count < 14 { return (r, t) }
            r.append(t)
        }
        return (r, nil)
    }

    /// `s2n(c, n)` con los `n` primeros caracteres.
    static func s2n(_ c: [UInt16], _ n: Int) -> Int {
        c.prefix(n).reduce(0) { $0 * 3 + ($1 == 0x31 ? 1 : $1 == 0x32 ? 2 : 0) }
    }
}

// MARK: - Selector JuanM (5 + 5 + 4)

/// Tablas de `RecuperaPantalla` + `Calgr1/2/3`: por grupo, productos ordenados de mayor a menor (`pg`) y su código (`id`).
public struct TablasJM: Sendable {
    var valores: [[Double]]
    var grupos: [Int]
    var pg: [[Double]] = []
    var id: [[Int]] = []
    /// Límites de los grupos 1, 2, 3 y del total.
    var tablim: [[Bool]]
    public internal(set) var porcentajes: [String] = []
}

extension CasosUtilidades {
    /// `LlenarTablim`: `a`, `a-b` (ordenados) separados por comas, recortados a 1…max; `nil` = «error en limites».
    static func limitesJM(_ texto: String, _ maximo: Int) -> [Bool]? {
        var t = [Bool](repeating: false, count: maximo)
        func n(_ s: String) -> Int? { (try? FormatoNET.convertToInt32(s)).map { min(maximo, max(1, Int($0))) } }
        for parte in texto.components(separatedBy: ",") {
            let a = parte.components(separatedBy: "-")
            switch a.count {
            case 1:
                guard let x = n(a[0]) else { return nil }
                t[x - 1] = true
            case 2:
                guard let x = try? FormatoNET.convertToInt32(a[0]), let y = try? FormatoNET.convertToInt32(a[1]) else { return nil }
                let (i, s) = (min(maximo, max(1, Int(min(x, y)))), min(maximo, max(1, Int(max(x, y)))))
                for k in (i - 1)..<s { t[k] = true }
            default: return nil
            }
        }
        return t
    }

    /// `mxsort`: selección descendente con `>` estricto (se queda con el primer máximo), intercambiando también `id`.
    static func mxsort(_ pg: inout [Double], _ id: inout [Int]) {
        for i in 0..<(pg.count - 1) {
            var (ref, s) = (pg[i], i)
            for nr in (i + 1)..<pg.count where pg[nr] > ref { (s, ref) = (nr, pg[nr]) }
            (pg[s], pg[i]) = (pg[i], ref)
            id.swapAt(i, s)
        }
    }

    /// CU-C23 `PrepararTablasJM`: valoraciones (0 ⇒ 0,1), grupos 5+5+4 y límites; los % con `{0:f2}` de la cultura.
    public static func tablasJM(
        valoraciones: [[Double]], grupos textos: [String], limites: [String], cultura: Cultura
    ) throws(ErrorDominio) -> TablasJM {
        let v = valoraciones.prefix(14).map { $0.map { $0 == 0 ? 1e-1 : $0 } }
        let g = textos.map(enteroNET)
        guard g.count(where: { $0 == 1 }) == 5, g.count(where: { $0 == 2 }) == 5, g.count(where: { $0 == 3 }) == 4 else {
            throw ErrorDominio(.general, "error en grupos")
        }
        var tablim: [[Bool]] = []
        for (texto, m) in zip(limites, [243, 243, 81, 567]) {
            guard let t = limitesJM(texto, m) else { throw ErrorDominio(.general, "error en limites") }
            tablim.append(t)
        }
        var t = TablasJM(valores: v, grupos: g, tablim: tablim)
        for grupo in 1...3 {
            // w[k] = (X, 1, 2) del k-ésimo partido del grupo; productos con a (primer partido) el más externo.
            let w = (0..<14).filter { g[$0] == grupo }.map { [v[$0][1], v[$0][0], v[$0][2]] }
            var pg: [Double] = [1]
            for wk in w { pg = pg.flatMap { p in wk.map { p * $0 } } }
            var id = Array(pg.indices)
            mxsort(&pg, &id)
            let pc = zip(pg, tablim[grupo - 1]).reduce(0.0) { $1.1 ? $0 + $1.0 : $0 }
            t.pg.append(pg)
            t.id.append(id)
            t.porcentajes.append(FormatoNET.toStringFijo(pc / (grupo == 3 ? 1e6 : 1e8), decimales: 2, cultura: cultura))
        }
        return t
    }

    /// Índice de 14 partidos (`s2n`) de cada código de grupo: suma de sus dígitos en las posiciones de su grupo.
    static func aportacionesJM(_ t: TablasJM) -> [[Int]] {
        (1...3).map { grupo in
            let pos = (0..<14).filter { t.grupos[$0] == grupo }
            return (0..<(grupo == 3 ? 81 : 243)).map { c in
                var (x, r) = (c, 0)
                for p in pos.reversed() {
                    r += (x % 3) * Int(ConvertidorDeBases.potenciasDe3[13 - p])
                    x /= 3
                }
                return r
            }
        }
    }

    /// CU-C24 / CU-C25: columnas válidas «por corte» (posiciones) o «por valor» (productos iguales a alguno dentro de los
    /// límites). La cancelación corta y deja lo calculado.
    /// ponytail: «por valor» no compara cada columna con las 243·243·81 posiciones como el C#: agrupa por código de grupo
    /// (la columna ↔ sus tres códigos) y comprueba el total con conjuntos de sumas de posiciones; mismo resultado.
    @concurrent public static func seleccionJM(
        _ t: TablasJM, porCorte: Bool
    ) async -> (bits: ConjuntoBits, procesadas: Int, validas: Int) {
        var bits = ConjuntoBits(cantidad: 4_782_969)
        var (procesadas, validas) = (0, 0)
        let x = aportacionesJM(t)
        let total = t.tablim[3]
        if porCorte {
            for r1 in 0..<243 {
                if Task.isCancelled { break }
                guard t.tablim[0][r1] else {
                    procesadas += 19683
                    continue
                }
                for r2 in 0..<243 {
                    guard t.tablim[1][r2] else {
                        procesadas += 81
                        continue
                    }
                    let x12 = x[0][t.id[0][r1]] + x[1][t.id[1][r2]]
                    for r3 in 0..<81 {
                        if t.tablim[2][r3] && total[r1 + r2 + r3 + 2] {
                            try? bits.poner(x12 + x[2][t.id[2][r3]])
                            validas += 1
                        }
                        procesadas += 1
                    }
                }
            }
        } else {
            // Por grupo y código: posiciones (con límite) cuyo producto es igual al del código, como máscara de bits.
            let posiciones: [[[Int]]] = (0..<3).map { g in
                var orig = [Double](repeating: 0, count: t.pg[g].count)
                for (r, c) in t.id[g].enumerated() { orig[c] = t.pg[g][r] }
                return orig.map { v in t.pg[g].indices.filter { t.tablim[g][$0] && t.pg[g][$0] == v } }
            }
            let mascaras2: [[UInt64]] = posiciones[1].map { l in
                var m = [UInt64](repeating: 0, count: 9)
                for r in l { m[r >> 6] |= 1 << UInt64(r & 63) }
                return m
            }
            // Por código del grupo 3: sumas s = r1 + r2 con las que alguna de sus posiciones r3 cumple el total.
            let sumas3: [[UInt64]] = posiciones[2].map { l3 in
                var s = [UInt64](repeating: 0, count: 9)
                for suma in 0...484 {
                    for r3 in l3 where total[suma + r3 + 2] {
                        s[suma >> 6] |= 1 << UInt64(suma & 63)
                        break
                    }
                }
                return s
            }
            for c1 in 0..<243 {
                if Task.isCancelled { break }
                let l1 = posiciones[0][c1]
                for c2 in 0..<243 {
                    // Sumas r1 + r2: desplazar la máscara del grupo 2 por cada r1.
                    var s12 = [UInt64](repeating: 0, count: 9)
                    let m2 = mascaras2[c2]
                    for r1 in l1 {
                        let (w, b) = (r1 >> 6, UInt64(r1 & 63))
                        for k in 0..<(9 - w) {
                            s12[k + w] |= m2[k] << b
                            if b > 0 && k + w + 1 < 9 { s12[k + w + 1] |= m2[k] >> (64 - b) }
                        }
                    }
                    let x12 = x[0][c1] + x[1][c2]
                    procesadas += 81
                    guard s12.contains(where: { $0 != 0 }) else { continue }
                    for c3 in 0..<81 {
                        let s3 = sumas3[c3]
                        var k = 0
                        while k < 9 && s12[k] & s3[k] == 0 { k += 1 }
                        if k < 9 {
                            try? bits.poner(x12 + x[2][c3])
                            validas += 1
                        }
                    }
                }
            }
        }
        return (bits, procesadas, validas)
    }

    /// CU-C26 `UbicarGanadoraJM`: posiciones (1-based) en cada grupo y total.
    public static func ubicarGanadoraJM(_ t: TablasJM, columna: String, porCorte: Bool) -> [Int] {
        let c = Array(columna.utf16)
        var i = [0, 0, 0]
        for g in 0..<3 {
            let pos = (0..<14).filter { t.grupos[$0] == g + 1 }
            if porCorte {
                let code = s2n(pos.map { c[$0] }, pos.count)
                i[g] = t.id[g].firstIndex(of: code) ?? code
            } else {
                var n = 1.0
                for p in pos {
                    let nv = t.valores[p][c[p] == 0x31 ? 0 : c[p] == 0x32 ? 2 : 1]
                    n *= nv == 0 ? 1 : nv
                }
                i[g] = t.pg[g].firstIndex(of: n) ?? 0
            }
        }
        return [i[0] + 1, i[1] + 1, i[2] + 1, i[0] + i[1] + i[2] + 3]
    }

    /// CU-C27: 6 líneas (grupos, 3 límites, total, modo «1» = corte / «0» = valor). `nil` = «fichero de condiciones erróneo».
    public static func leerCondicionesJM(_ datos: Data) -> (grupos: [String], limites: [String], porCorte: Bool)? {
        let l = TextoNET.lineas(datos)
        guard l.count >= 6, l[0].utf16.count >= 14 else { return nil }
        let g = Array(l[0].utf16.prefix(14)).map { String(decoding: [$0], as: UTF16.self) }
        return (g, Array(l[1...4]), l[5] != "0")
    }

    public static func datosCondicionesJM(grupos: [String], limites: [String], porCorte: Bool) -> Data {
        TextoNET.codificarLineas([grupos.joined()] + limites + [porCorte ? "1" : "0"])
    }

    /// «Grabar columnas»: las válidas por código ascendente (el fichero se crea aunque no haya ninguna).
    public static func grabarSeleccion(_ bits: ConjuntoBits, en url: URL, entorno: Entorno) throws(ErrorDominio) {
        try entorno.archivos.escribir(TextoNET.codificarLineas(bits.indices.map(FiltroPorGrupos.n1s)), en: url, atomico: false)
    }
}

// MARK: - Selector MarioSan

/// CU-C28: columnas distintas (orden de fichero) y, de cada una, cuántas otras están a distancia exacta 14 − aspecto.
public struct DistribucionMS: Sendable {
    public let limite: Int
    public let columnas: [Int]
    public let vecinos: [Int]

    /// `LimSh`: filas Q de la tabla.
    public var filas: Int { limite == 13 ? 29 : limite == 12 ? 365 : 2913 }

    /// `lbtab`: columnas por nº de vecinos.
    public var conteo: [Int] {
        var r = [Int](repeating: 0, count: 2913)
        for v in vecinos { r[v] += 1 }
        return r
    }
}

extension CasosUtilidades {
    /// `LeerYCalcular` + `Calcular13/12/11`; `nil` = cancelado.
    /// ponytail: enumera directamente los vecinos a distancia exacta d (28, 364 o 2912) en vez de los caminos con
    /// `repes` del C#; cuenta lo mismo.
    @concurrent public static func distribucionMS(
        _ url: URL, limite: Int, entorno: Entorno
    ) async throws(ErrorDominio) -> DistribucionMS? {
        var existe = [Bool](repeating: false, count: 4_782_969)
        var cols: [Int] = []
        for l in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url)) {
            if Task.isCancelled { return nil }
            let n = try FiltroPorGrupos.s1n(l)
            if !existe[n] {
                existe[n] = true
                cols.append(n)
            }
        }
        let d = 14 - limite
        var vecinos: [Int] = []
        vecinos.reserveCapacity(cols.count)
        for c in cols {
            if Task.isCancelled { return nil }
            var n = 0
            func paso(_ x: Int, _ desde: Int, _ resto: Int) {
                if resto == 0 {
                    if existe[x] { n += 1 }
                    return
                }
                for p in stride(from: desde, to: 14, by: 1) {
                    let pw = Int(ConvertidorDeBases.potenciasDe3[p])
                    let s = (x / pw) % 3
                    for z in 0..<3 where z != s { paso(x + pw * (z - s), p + 1, resto - 1) }
                }
            }
            paso(c, 0, d)
            vecinos.append(n)
        }
        return DistribucionMS(limite: limite, columnas: cols, vecinos: vecinos)
    }

    /// CU-C29 `Analizar`: por fila Q, columnas con 14…10 signos iguales a la ganadora (sus 14 primeros caracteres).
    public static func analizarMS(_ d: DistribucionMS, ganadora: String) throws(ErrorDominio) -> [[Int]] {
        var r = [[Int]](repeating: [0, 0, 0, 0, 0], count: d.filas)
        let g = try FiltroPorGrupos.s1n(ganadora)
        for (c, v) in zip(d.columnas, d.vecinos) {
            var (a, b, na) = (g, c, 0)
            for _ in 0..<14 {
                if a % 3 == b % 3 { na += 1 }
                (a, b) = (a / 3, b / 3)
            }
            if na > 9 && v < r.count { r[v][14 - na] += 1 }
        }
        return r
    }

    /// `Grabar`: las columnas del grupo Q en orden de fichero; sin ninguna no se crea el fichero.
    public static func grabarMS(_ d: DistribucionMS, grupo: Int, en url: URL, entorno: Entorno) throws(ErrorDominio) {
        let l = zip(d.columnas, d.vecinos).filter { $0.1 == grupo }.map { FiltroPorGrupos.n1s($0.0) }
        if let datos = ArchivoColumnasTexto.datos(l) { try entorno.archivos.escribir(datos, en: url, atomico: false) }
    }
}
