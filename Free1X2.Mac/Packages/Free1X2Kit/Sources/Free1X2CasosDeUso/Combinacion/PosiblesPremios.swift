import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/PosiblesPremiosFrmViewModel.cs (10 §79; CU-C11…C13), VisorPosiblesPremiosViewModel.cs (10 §104;
// CU-C57) y MejoresOpcionesFrmViewModel.cs (09 §72; CU-B40).

/// Resultado de «Calcular» en «Posibles premios».
public struct ResultadoPosiblesPremios: Sendable {
    /// Columnas por categoría 16…10, cada una con el sufijo de aciertos (`columna + "14"`).
    public let categorias: [[String]]
    /// `col16 + col15 + … + col10`.
    public var premiadas: [String] { categorias.flatMap { $0 } }
    public let resumen: [PosiblesPremiosContenedor]
}

/// Una columna premiada en el visor: signos con acierto o fallo, categoría y los 2 últimos caracteres guardados.
public struct ColumnaVisorPremios: Sendable, Hashable {
    public let signos: [String]
    public let aciertos: [Bool]
    public let categoria: Int
    public let aciertosTexto: String
}

extension CasosCombinacion {
    /// CU-C11 «Archivo…»: `N = ObtenNumSignos`; toda columna debe medir N (si no, `nil` = «Error leyendo columnas»).
    public static func columnasJugadas(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> (columnas: [String], partidos: Int)? {
        let d = try entorno.archivos.leer(url)
        let n = ArchivoColumnasTexto.numSignos(d)
        var r: [String] = []
        for c in ArchivoColumnasTexto.columnas(d) {
            guard c.utf16.count == n else { return nil }
            r.append(c)
        }
        return (r, n)
    }

    /// `ObtenGanadora`: 1/x/X/2 en mayúsculas; el resto de los `n` primeros partidos pasa a «*» (también en pantalla).
    public static func ganadoraParcial(_ signos: [String], partidos n: Int) -> (ganadora: String, signos: [String], noDefinitivos: Int) {
        var (g, s, k) = ("", signos, 0)
        for (i, x) in signos.enumerated() {
            if ["1", "x", "X", "2"].contains(x) {
                g += x.uppercased()
            } else if i < n {
                (g, s[i], k) = (g + "*", "*", k + 1)
            }
        }
        return (g, s, k)
    }

    /// CU-C12 `Escrutar`: corta si ya no llega a 10; el último partido cuenta aparte (con pleno, solo si acertó el resto).
    static func escrutarPosible(_ a: [UInt16], _ g: [UInt16], partidos n: Int, pleno: Bool) -> Int {
        let estrella: UInt16 = 0x2A
        var (aciertos, posibles) = (0, n)
        for i in stride(from: 0, to: n - 1, by: 1) {
            if posibles < 10 { break }
            if a[i] == g[i] || g[i] == estrella { aciertos += 1 } else { posibles -= 1 }
        }
        if pleno {
            if aciertos == n - 1 && (a[n - 1] == g[n - 1] || g[n - 1] == estrella) { aciertos += 1 }
        } else if a.last == g.last || g[n - 1] == estrella {
            aciertos += 1
        }
        return aciertos
    }

    static func categorizar(_ columnas: [String], _ g: String, _ n: Int, _ pleno: Bool) -> [[String]] {
        var cat = [[String]](repeating: [], count: 7)
        let gu = Array(g.utf16)
        for c in columnas {
            let a = escrutarPosible(Array(c.utf16), gu, partidos: n, pleno: pleno)
            if (10...16).contains(a) { cat[16 - a].append(c + String(a)) }
        }
        return cat
    }

    /// CU-C13 `Calcular`: categorías con la ganadora parcial y, si se pide, el resumen por cada sustitución de los «*»
    /// (solo las que dan algún premio), ordenado con `PosiblesPremiosComparer`.
    @concurrent public static func posiblesPremios(
        _ columnas: [String], ganadora: String, partidos n: Int, pleno: Bool, generarResumen: Bool
    ) async -> ResultadoPosiblesPremios {
        var resumen: [PosiblesPremiosContenedor] = []
        if generarResumen {
            let g = Array(ganadora.utf16)
            func expandir(_ pre: [UInt16], _ p: Int) {
                for s in g[p] == 0x2A ? [0x31, 0x58, 0x32] : [g[p]] {
                    let nuevo = pre + [s]
                    if p < g.count - 1 && p < n - 1 {
                        expandir(nuevo, p + 1)
                    } else {
                        let gc = String(decoding: nuevo, as: UTF16.self)
                        let cat = categorizar(columnas, gc, n, pleno)
                        guard cat.contains(where: { !$0.isEmpty }) else { continue }
                        var c = PosiblesPremiosContenedor(colGanadora: gc)
                        (c.col16, c.col15, c.col14, c.col13, c.col12, c.col11, c.col10) = (
                            cat[0], cat[1], cat[2], cat[3], cat[4], cat[5], cat[6]
                        )
                        resumen.append(c)
                    }
                }
            }
            if !g.isEmpty { expandir([], 0) }
            PosiblesPremiosContenedor.ordenar(&resumen)
        }
        return ResultadoPosiblesPremios(categorias: categorizar(columnas, ganadora, n, pleno), resumen: resumen)
    }

    /// `MostrarOpciones`: «Optando a 16…10: n»; con pleno el 15 se suma al 14 (se replica).
    public static func textosOptando(_ categorias: [[String]], pleno: Bool) -> [String] {
        var n = categorias.map(\.count)
        if pleno { n[2] += n[1] }
        return n.enumerated().map { "Optando a \(16 - $0): \($1)" }
    }

    /// «Copiar»: cabecera y los «Optando a» (16 y 15 solo si hay tantos partidos), con `\r\n`.
    public static func textoCopiarPosibles(_ optando: [String], noDefinitivos: Int, partidos n: Int) -> String {
        "Posibles premios a falta de \(noDefinitivos) partidos:"
            + optando.enumerated().filter { $0.offset >= 2 || n >= 16 - $0.offset }.map { "\r\n" + $0.element }.joined()
    }

    /// «Guardar Resumen»: categorías 14…10 con los 14 primeros signos, entre líneas de 46 guiones; `AppendLine` = `\r\n`.
    public static func textoResumenPosibles(_ resumen: [PosiblesPremiosContenedor], noDefinitivos: Int) -> String {
        let raya = String(repeating: "-", count: 46)
        var l = ["Posibles Premios a falta de \(noDefinitivos) partidos"]
        for c in resumen {
            l += [raya, "Columna Ganadora: " + c.colGanadora, raya]
            for (cat, k) in [(c.col14, 14), (c.col13, 13), (c.col12, 12), (c.col11, 11), (c.col10, 10)] {
                l += cat.map { String(decoding: $0.utf16.prefix(14), as: UTF16.self) + " \(k) aciertos" }
            }
            l += [raya, " "]
        }
        return l.map { $0 + "\r\n" }.joined()
    }

    /// CU-C57 `MostrarGrupo`: la ganadora signo a signo y cada columna premiada (16…10, en ese orden).
    public static func grupoVisor(_ c: PosiblesPremiosContenedor) -> (ganadora: [String], columnas: [ColumnaVisorPremios]) {
        let g = Array(c.colGanadora.utf16)
        func t(_ u: UInt16) -> String { String(decoding: [u], as: UTF16.self) }
        var r: [ColumnaVisorPremios] = []
        for (lista, k) in [(c.col16, 16), (c.col15, 15), (c.col14, 14), (c.col13, 13), (c.col12, 12), (c.col11, 11), (c.col10, 10)] {
            for col in lista {
                let u = Array(col.utf16)
                let n = min(g.count, u.count)
                r.append(
                    ColumnaVisorPremios(
                        signos: u.prefix(n).map(t), aciertos: (0..<n).map { u[$0] == g[$0] }, categoria: k,
                        aciertosTexto: u.count >= 2 ? String(decoding: u.suffix(2), as: UTF16.self) : ""))
            }
        }
        return (g.map(t), r)
    }
}

extension CasosCombinacion {
    /// CU-B40 `ObtenGanadoras`: cada «*» de un partido involucrado se sustituye por 1, X y 2; los no involucrados siguen «*».
    public static func ganadorasPosibles(_ ganadora: String, involucrados: [Bool]) -> [String] {
        let g = Array(ganadora.utf16)
        var r: [String] = []
        func obtener(_ pre: [UInt16], _ p: Int) {
            let opciones: [UInt16] = g[p] != 0x2A ? [g[p]] : involucrados[p] ? [0x31, 0x58, 0x32] : [0x2A]
            for s in opciones {
                if p < g.count - 1 { obtener(pre + [s], p + 1) } else { r.append(String(decoding: pre + [s], as: UTF16.self)) }
            }
        }
        if !g.isEmpty { obtener([], 0) }
        return r
    }

    /// `Escrutar` de «Mis mejores opciones»: no mira la última posición salvo con pleno.
    static func escrutarMejores(_ a: [UInt16], _ g: [UInt16], partidos n: Int, pleno: Bool) throws(ErrorDominio) -> Int {
        let o = "MejoresOpcionesFrmViewModel.cs:254"
        var (aciertos, posibles) = (0, n)
        for i in stride(from: 0, to: a.count - 1, by: 1) {
            if posibles < 10 { break }
            if a[i] == (try g.leer(i, o)) || g[i] == 0x2A { aciertos += 1 } else { posibles -= 1 }
        }
        if pleno, aciertos == a.count - 1, let ua = a.last, let ug = g.last, ua == ug || ug == 0x2A { aciertos += 1 }
        return aciertos
    }

    /// CU-B40 `Calcular`: un contenedor por ganadora posible (aunque no tenga premios), ordenado; texto de los `limite`
    /// primeros. Con pleno, las columnas cortas que aciertan todo y las de 14 de 14 suman también en el 14 (se replica).
    @concurrent public static func mejoresOpciones(
        ganadora: String, columnas: [String], pleno: Bool, involucrados: [Bool], limite: Int
    ) async throws(ErrorDominio) -> String {
        let n = ganadora.utf16.count
        var lista: [PosiblesPremiosContenedor] = []
        for g in ganadorasPosibles(ganadora, involucrados: involucrados) {
            var c = PosiblesPremiosContenedor(colGanadora: "")
            let gu = Array(g.utf16)
            for col in columnas {
                let a = try escrutarMejores(Array(col.utf16), gu, partidos: n, pleno: pleno)
                guard a > 9 else { continue }
                c.colGanadora = g
                let doble = pleno && col.utf16.count == a && a != 10
                switch a {
                case 10: c.col10.append(col)
                case 11: c.col11.append(col)
                case 12: c.col12.append(col)
                case 13: c.col13.append(col)
                case 14: c.col14.append(col)
                case 15: c.col15.append(col)
                case 16: c.col16.append(col)
                default: break
                }
                if doble && a <= 16 { c.col14.append(col) }
            }
            lista.append(c)
        }
        PosiblesPremiosContenedor.ordenar(&lista)
        var s = "Mis mejores opciones son: \r\n------------------------\r\n\r\n"
        for c in lista.prefix(max(0, limite)) {
            s += c.colGanadora + ": "
            let partes = [(16, c.col16), (15, c.col15), (14, c.col14), (13, c.col13), (12, c.col12), (11, c.col11)]
            for (k, l) in partes where n >= k { s += "\(l.count) de \(k) + " }
            if n >= 10 { s += "\(c.col10.count) de 10" }
            s += "\r\n"
        }
        return s
    }
}
