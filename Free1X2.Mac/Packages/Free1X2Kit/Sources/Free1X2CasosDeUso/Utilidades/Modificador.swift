import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/ModificadorFrmViewModel.cs (09 pantalla 73).

/// «Modificador de columnas»: las columnas del fichero y los porcentajes enteros 1/X/2 de cada partido.
public struct EditorModificador: Sendable {
    /// `string[ObtenNumCols()]`: los huecos sin línea quedan `nil` (estimación por tamaño, DEC-932).
    public private(set) var columnas: [String?]
    public let partidos: Int
    public private(set) var frecuencia: [[Int]]

    var apuestas: Int { columnas.count }

    /// `Abrir` → `LeeFichero`: un fichero con más líneas que `ObtenNumCols` o columnas cortas lanza.
    public static func cargar(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> EditorModificador {
        let o = "ModificadorFrmViewModel.cs:243"
        let d = try entorno.archivos.leer(url)
        let n = ArchivoColumnasTexto.numSignos(d)
        var cols = [String?](repeating: nil, count: ArchivoColumnasTexto.numColumnas(d))
        var f = [[Int]](repeating: [0, 0, 0], count: n)
        for (k, l) in ArchivoColumnasTexto.columnas(d).enumerated() {
            guard k < cols.count else { throw .fueraDeRango(o) }
            cols[k] = l
            let u = Array(l.utf16)
            for i in 0..<n {
                switch try u.leer(i, o) {
                case 0x31: f[i][0] += 1
                case 0x58: f[i][1] += 1
                case 0x32: f[i][2] += 1
                default: break
                }
            }
        }
        for i in 0..<n {
            for j in 0..<3 { f[i][j] = Int(try FormatoNET.convertToInt32(Double(f[i][j] * 100) / Double(cols.count) + 0.4)) }
            while f[i].reduce(0, +) < 100 { f[i][0] += 1 }
            while f[i].reduce(0, +) > 100 {
                if f[i][2] > 0 { f[i][2] -= 1 } else if f[i][1] > 0 { f[i][1] -= 1 } else { break }
            }
        }
        return EditorModificador(columnas: cols, partidos: n, frecuencia: f)
    }

    /// `Ordenar`: signos de mayor a menor con tres `if` seguidos (el último que se cumple manda).
    static func ordenar(_ p: [Double]) -> [UInt16] {
        var o: [UInt16] = []
        if p[0] >= p[1] && p[0] >= p[2] { o = [0x31] + (p[1] >= p[2] ? [0x58, 0x32] : [0x32, 0x58]) }
        if p[1] >= p[0] && p[1] >= p[2] { o = [0x58] + (p[0] >= p[2] ? [0x31, 0x32] : [0x32, 0x31]) }
        if p[2] >= p[1] && p[2] >= p[0] { o = [0x32] + (p[0] >= p[1] ? [0x31, 0x58] : [0x58, 0x31]) }
        return o.count == 3 ? o : [0x31, 0x58, 0x32]
    }

    /// `CambiarSigno`: la columna con el partido `k` sustituido (`partidos` caracteres).
    func cambiar(_ c: Int, _ k: Int, _ s: UInt16) throws(ErrorDominio) -> String {
        guard let col = try columnas.leer(c, "ModificadorFrmViewModel.cs:395") else {
            throw .referenciaNula("ModificadorFrmViewModel.cs:395")
        }
        let u = Array(col.utf16)
        var r: [UInt16] = []
        for i in 0..<partidos { r.append(i == k ? s : try u.leer(i, "ModificadorFrmViewModel.cs:397")) }
        return String(decoding: r, as: UTF16.self)
    }

    /// `ModificarFrecuencia`: reparte los signos del partido `k` según `porcentajes` (normalizados a 100). Modo 1
    /// aleatorio (siempre 1, X, 2), 2 proporcional, 3 ordenado. El proporcional que no avanza lanza (DT-08).
    public mutating func modificar(
        partido k: Int, porcentajes: [Double], modo: Int, ordenarSignos: Bool, aleatorio: inout AleatorioNET
    ) throws(ErrorDominio) {
        var pct = porcentajes
        let suma = pct.reduce(0, +)
        if suma != 100 && suma != 0 { pct = pct.map { $0 * (100 / suma) } }
        var inicial: [Int] = []
        for p in pct { inicial.append(Int(try FormatoNET.convertToInt32(Double(apuestas) * p / 100))) }
        while inicial.reduce(0, +) < apuestas { inicial[0] += 1 }
        while inicial.reduce(0, +) > apuestas { inicial[2] -= 1 }
        var signo: [UInt16] = [0x31, 0x58, 0x32]
        var total = inicial
        if ordenarSignos && modo > 1 {
            signo = Self.ordenar(pct)
            total = signo.map { inicial[$0 == 0x31 ? 0 : $0 == 0x58 ? 1 : 2] }
        }
        switch modo {
        case 1:
            signo = [0x31, 0x58, 0x32]
            for c in 0..<apuestas {
                var a: Int
                repeat { a = Int(try aleatorio.next(3)) } while total[a] == 0
                columnas[c] = try cambiar(c, k, signo[a])
                total[a] -= 1
            }
        default:
            let grupo =
                modo == 3 ? total : total.map { total[2] == 0 ? (total[1] == 0 ? $0 : $0 / total[1]) : $0 / total[2] }
            var procesados = 0
            while procesados < apuestas {
                let antes = procesados
                for i in 0..<3 {
                    var quedan = min(grupo[i], total[i])
                    while quedan > 0 && total[i] > 0 {
                        columnas[procesados] = try cambiar(procesados, k, signo[i])
                        (quedan, total[i], procesados) = (quedan - 1, total[i] - 1, procesados + 1)
                    }
                }
                guard procesados > antes else {
                    throw ErrorDominio(.operacionInvalida, "El reparto proporcional no avanza con estos porcentajes.")
                }
            }
        }
        for j in 0..<3 { frecuencia[k][j] = Int(try FormatoNET.convertToInt32(pct[j])) }
    }

    /// «Grabar»: `GuardarTodasCols` (sin comas); un hueco sin columna lanza.
    public func datos() throws(ErrorDominio) -> Data? {
        var r: [String] = []
        for c in columnas {
            guard let c else { throw .referenciaNula("ArchivoColumnasTexto.cs:79") }
            r.append(c)
        }
        return ArchivoColumnasTexto.datos(r)
    }
}

// C#: Views/Ported/BuscaLimsFrmViewModel.cs (08 pantalla 12; CU-A27).

/// Resultado de «Calcular Límites» (`-1`/`0` = sin valor, como el C#).
public struct LimitesValoracion: Sendable, Hashable {
    public let columnaBase: String
    public let sumaMin, sumaMax, productoMin, productoMax: Double
    public let procesadas, admitidas: Int
}

extension CasosUtilidades {
    /// CU-A27: columna base (con la condición del 1 que compara contra `dv[0][2]`, se replica), aciertos de cada una de
    /// las 3^N columnas contra ella y mínimos/máximos de suma y producto (·0,03420425138) de las que están en el rango.
    @concurrent public static func buscarLimites(_ dv: [[Double]], rango: String) async -> LimitesValoracion {
        let n = dv.count
        var base: [Int] = []
        for i in 0..<n {
            if dv[i][0] >= dv[i][1] && dv[i][0] >= dv[0][2] {
                base.append(0)
            } else if dv[i][2] >= dv[i][0] && dv[i][2] >= dv[i][1] {
                base.append(2)
            } else {
                base.append(1)
            }
        }
        var rangos = [Bool](repeating: false, count: n + 1)
        for a in UtilidadesEntradasValores.obtenerListaFromTxtAciertos(rango) where a >= 0 && a < rangos.count { rangos[a] = true }
        var (mn, mx, mnP, mxP, nca, ncv) = (-1.0, 0.0, -1.0, 0.0, 0, 0)
        // Odómetro en el orden 1, X, 2 con el primer partido como el más significativo (= la recursión del C#).
        var d = [Int](repeating: 0, count: n)
        var seguir = n > 0
        while seguir {
            nca += 1
            var aciertos = 0
            for i in 0..<n where d[i] == base[i] { aciertos += 1 }
            if rangos[aciertos] {
                ncv += 1
                var (suma, prod) = (0.0, 1.0)
                for i in 0..<n {
                    suma += dv[i][d[i]]
                    prod *= dv[i][d[i]] * 0.03420425138
                }
                if mn > suma || mn == -1 { mn = suma }
                if mx < suma { mx = suma }
                if mnP > prod || mnP == -1 { mnP = prod }
                if mxP < prod { mxP = prod }
            }
            var i = n - 1
            while i >= 0 {
                d[i] += 1
                if d[i] < 3 { break }
                d[i] = 0
                i -= 1
            }
            seguir = i >= 0
        }
        let cb = String(base.map { ["1", "X", "2"][$0] }.joined())
        return LimitesValoracion(
            columnaBase: cb, sumaMin: mn, sumaMax: mx, productoMin: mnP, productoMax: mxP, procesadas: nca, admitidas: ncv)
    }
}

// C#: Views/Ported/CombinarFiltrosViewModel.cs (08 pantalla 21; CU-A53…A57). Codificación propia (la de Pares).
extension CasosUtilidades {
    /// `ObtenNumSignos` de un filtro (0 si no se puede leer).
    public static func signosDeFiltro(_ url: URL, entorno: Entorno) -> Int {
        (try? entorno.archivos.leer(url)).map(ArchivoColumnasTexto.numSignos) ?? 0
    }

    /// «Cargar Lista»: rutas no vacías del `.lst`, tal cual.
    public static func rutasDeLista(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> [String] {
        TextoNET.lineas(try entorno.archivos.leer(url)).filter { !CasosCondiciones.blanco($0) }
    }

    /// CU-A53 `Iniciar`: por cada filtro activo (en orden), +1 a cada columna distinta; lectura cortada por una columna
    /// normalizada de menos de 14 signos. Devuelve el recuento por columna y las líneas leídas por filtro (0 si inactivo).
    @concurrent public static func combinarFiltros(
        _ filtros: [(url: URL, activo: Bool)], partidos: Int, entorno: Entorno
    ) async throws(ErrorDominio) -> (cuentas: [Int32], columnas: [Int]) {
        let tam = Int(pow(3.0, Double(partidos)))
        var cuentas = [Int32](repeating: 0, count: tam)
        var leidas: [Int] = []
        for f in filtros {
            if Task.isCancelled { break }
            var ct = 0
            defer { leidas.append(ct) }
            guard f.activo, let d = try? entorno.archivos.leer(f.url) else { continue }
            var repes = [Bool](repeating: false, count: tam)
            for l in ArchivoColumnasTexto.columnas(d) {
                if Task.isCancelled { break }
                let c = normalizaAnaCombi(l)
                ct += 1
                if c.count < 14 { break }
                let i = c.reduce(0) { $0 * 3 + ($1 == 0x31 ? 1 : $1 == 0x32 ? 2 : 0) }
                guard i < tam else { throw .fueraDeRango("CombinarFiltrosViewModel.cs:294") }
                if !repes[i] {
                    cuentas[i] += 1
                    repes[i] = true
                }
            }
        }
        return (cuentas, leidas)
    }

    /// Rango efectivo de «Sumar» y «Grabar»: mínimo al menos 1 y máximo como mucho el nº de filtros.
    public static func rangoFiltros(_ minimo: Double, _ maximo: Double, filtros: Int) -> ClosedRange<Int>? {
        let (a, b) = (max(1, Int(FormatoNET.castInt32(minimo))), min(Int(FormatoNET.castInt32(maximo)), filtros))
        return a <= b ? a...b : nil
    }

    /// CU-A55 «Grabar cols.»: `N2s` (siempre 14 signos, DEC-812) de las columnas en rango, por índice.
    public static func grabarFiltros(_ cuentas: [Int32], rango: ClosedRange<Int>?, en url: URL, entorno: Entorno) throws(ErrorDominio)
        -> Int
    {
        let cols = cuentas.indices.filter { rango?.contains(Int(cuentas[$0])) ?? false }.map(FiltroPorGrupos.n1s)
        if let d = ArchivoColumnasTexto.datos(cols) { try entorno.archivos.escribir(d, en: url, atomico: false) }
        return cols.count
    }
}
