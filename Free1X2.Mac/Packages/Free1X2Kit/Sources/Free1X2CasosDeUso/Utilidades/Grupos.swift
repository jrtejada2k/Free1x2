import Foundation
import Free1X2Base

// C#: Views/Ported/ParejasFrmViewModel.cs (10 §77; CU-C09) y TriosFrmViewModel.cs (10 §96; CU-C41).

/// Filtro por parejas (`k` = 2: 14 filas, 7 niveles) o tríos (`k` = 3: 12 filas, 5 niveles).
public struct FiltroPorGrupos: Sendable {
    public let k: Int
    /// Por fila: P1…Pk (1-based) y el nivel de cada una de las 3^k combinaciones (orden 1, X, 2; el primer partido manda).
    public var filas: [[Int]]
    /// Por nivel: mínimo y máximo.
    public var limites: [(minimo: Int, maximo: Int)]

    public init(k: Int, filas: [[Int]], limites: [(minimo: Int, maximo: Int)]) {
        (self.k, self.filas, self.limites) = (k, filas, limites)
    }

    /// `Valida`: recuento por nivel y si todos caen en su rango. Un nivel mayor que el nº de niveles lanza
    /// (`IndexOutOfRange`).
    public func valida(_ columna: String) throws(ErrorDominio) -> (valida: Bool, recuento: [Int]) {
        var recuento = [Int](repeating: 0, count: limites.count)
        let c = Array(FormatoNET.mayusculas(columna).utf16)
        for f in filas {
            let pos = f.prefix(k).map { $0 - 1 }
            guard pos.allSatisfy({ $0 >= 0 && $0 < c.count }) else { continue }
            var combinacion = 0
            var conocida = true
            for p in pos {
                switch c[p] {
                case 0x31: combinacion = combinacion * 3
                case 0x58: combinacion = combinacion * 3 + 1
                case 0x32: combinacion = combinacion * 3 + 2
                default: conocida = false
                }
            }
            let nv = conocida ? f[k + combinacion] : 0
            guard nv > 0 else { continue }
            guard nv <= recuento.count else { throw .fueraDeRango("ParejasFrmViewModel.cs:351") }
            recuento[nv - 1] += 1
        }
        return (zip(recuento, limites).allSatisfy { $0 >= $1.minimo && $0 <= $1.maximo }, recuento)
    }

    /// `S1n`: base 3 propia de estas pantallas (X y otros = 0, 1 = 1, 2 = 2; primer partido el más significativo); menos de
    /// 14 caracteres lanza.
    static func s1n(_ columna: String) throws(ErrorDominio) -> Int {
        let u = Array(columna.utf16)
        guard u.count >= 14 else { throw .fueraDeRango("ParejasFrmViewModel.cs:383") }
        return u.prefix(14).reduce(0) { $0 * 3 + ($1 == 0x31 ? 1 : $1 == 0x32 ? 2 : 0) }
    }

    /// `N1s`.
    static func n1s(_ n: Int) -> String {
        var (x, s) = (n, "")
        for _ in 0..<14 {
            s = ["X", "1", "2"][x % 3] + s
            x /= 3
        }
        return s
    }
}

extension CasosUtilidades {
    /// `AInt` = `int.TryParse` (inválido ⇒ 0).
    public static func enteroNET(_ s: String) -> Int { (try? FormatoNET.convertToInt32(s)).map(Int.init) ?? 0 }

    /// `Calcular`: cada línea (hasta EOF) que pasa `Valida` marca su `S1n`. La cancelación (`Cancelar`) corta entre líneas
    /// y deja lo marcado.
    @concurrent public static func filtrarPorGrupos(
        _ url: URL, filtro: FiltroPorGrupos, entorno: Entorno
    ) async throws(ErrorDominio) -> (procesadas: Int, validas: Int, bits: ConjuntoBits) {
        var bits = ConjuntoBits(cantidad: 4_782_969)
        var (procesadas, validas) = (0, 0)
        for l in TextoNET.lineas(try entorno.archivos.leer(url)) {
            if Task.isCancelled { break }
            procesadas += 1
            if try filtro.valida(l).valida {
                try bits.poner(try FiltroPorGrupos.s1n(l))
                validas += 1
            }
        }
        return (procesadas, validas, bits)
    }

    /// «Grabar resultado»: `N1s` de cada bit activo, por índice (el fichero se crea aunque no haya ninguno).
    public static func grabarPorGrupos(_ bits: ConjuntoBits, en url: URL, entorno: Entorno) throws(ErrorDominio) {
        try entorno.archivos.escribir(TextoNET.codificarLineas(bits.indices.map(FiltroPorGrupos.n1s)), en: url, atomico: false)
    }

    /// «Leer condiciones» `.par`: 14 líneas `P1,P2,9 niveles` y 7 `min,max`, como texto; una línea con menos campos se
    /// salta (se consume) y el fin del fichero corta.
    public static func aplicarCondicionesPares(_ datos: Data, filas: inout [[String]], limites: inout [[String]]) {
        var lineas = TextoNET.lineas(datos).makeIterator()
        for r in filas.indices {
            guard let l = lineas.next() else { break }
            let aux = l.components(separatedBy: ",")
            if aux.count >= 11 { filas[r] = Array(aux.prefix(11)) }
        }
        for n in limites.indices {
            guard let l = lineas.next() else { break }
            let aux = l.components(separatedBy: ",")
            if aux.count >= 2 { limites[n] = Array(aux.prefix(2)) }
        }
    }

    /// «Salvar condiciones» `.par`: los textos tal cual.
    public static func datosCondicionesPares(filas: [[String]], limites: [[String]]) -> Data {
        TextoNET.codificarLineas(filas.map { $0.joined(separator: ",") } + limites.map { $0.joined(separator: ",") })
    }

    /// «Abrir Condiciones» `.tri`: 12 líneas de hasta 30 enteros (inválido ⇒ 0) y los límites de las **5 últimas** líneas
    /// (`Double.TryParse`; faltan ⇒ 0).
    public static func aplicarCondicionesTrios(_ datos: Data, filas: inout [[String]], limites: inout [[Double]], cultura: Cultura) {
        let lineas = TextoNET.lineas(datos)
        for r in filas.indices where r < lineas.count {
            for (c, v) in lineas[r].components(separatedBy: ",").prefix(30).enumerated() {
                filas[r][c] = String(enteroNET(FormatoNET.recortar(v)))
            }
        }
        let inicio = max(0, lineas.count - 5)
        for i in limites.indices {
            let li = inicio + i
            let aux = li < lineas.count ? lineas[li].components(separatedBy: ",") : []
            limites[i] =
                aux.count >= 2
                ? [FormatoNET.tryParseDouble(aux[0], cultura: cultura) ?? 0, FormatoNET.tryParseDouble(aux[1], cultura: cultura) ?? 0]
                : [0, 0]
        }
    }

    /// «Salvar Condiciones» `.tri`: 12 líneas de 30 enteros y 5 `min,max` truncados.
    public static func datosCondicionesTrios(filas: [[String]], limites: [[Double]]) -> Data {
        TextoNET.codificarLineas(
            filas.map { $0.map { String(enteroNET($0)) }.joined(separator: ",") }
                + limites.map { "\(FormatoNET.castInt32($0[0])),\(FormatoNET.castInt32($0[1]))" })
    }
}

// C#: Views/Ported/PremiadasFrmViewModel.cs (10 §80; CU-C14…C16). Misma codificación propia que Pares.
extension CasosUtilidades {
    /// Columnas a distancia exacta `d` (1…4 ⇔ 13…10 aciertos) de `n`, en el orden de los bucles anidados (`Genera`).
    static func distantes(_ n: Int, _ d: Int, _ accion: (Int) -> Void) {
        let pot = (0..<14).map { Int(pow(3.0, Double($0))) }
        func visitar(_ col: Int, _ desde: Int, _ nivel: Int) {
            for nr in desde..<14 {
                let s = col / pot[nr] % 3
                for z in 0..<3 where z != s {
                    let c = col + pot[nr] * (z - s)
                    if nivel == d { accion(c) } else { visitar(c, nr + 1, nivel + 1) }
                }
            }
        }
        visitar(n, 0, 1)
    }

    /// Ganadoras del fichero: las líneas de al menos 14 caracteres, codificadas con `S1n`.
    static func ganadoras(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> [Int] {
        var r: [Int] = []
        for l in TextoNET.lineas(try entorno.archivos.leer(url)) where l.utf16.count >= 14 { r.append(try FiltroPorGrupos.s1n(l)) }
        return r
    }

    /// CU-C14 `Calcular`: veces que cada columna tiene `premio` aciertos con alguna ganadora y la lista «qdc = n veces».
    @concurrent public static func frecuenciasPremiadas(
        _ url: URL, premio: Int, entorno: Entorno
    ) async throws(ErrorDominio) -> (validas: [Int32], frecuencias: [String], procesadas: Int) {
        let g = try ganadoras(url, entorno: entorno)
        var validas = [Int32](repeating: 0, count: 4_782_969)
        var nmax: Int32 = 0
        for n in g {
            distantes(n, 14 - premio) {
                validas[$0] &+= 1
                nmax = max(nmax, validas[$0])
            }
        }
        var qdc = [Int](repeating: 0, count: Int(nmax))
        for v in validas where v > 0 { qdc[Int(v) - 1] += 1 }
        return (validas, qdc.enumerated().map { "\($1) = \($0 + 1) veces" }, g.count)
    }

    /// CU-C15 `Analizar`: «sem.k» (k = nº de ganadora) por cada columna a esa distancia con frecuencia `n`.
    @concurrent public static func jornadasDeFrecuencia(
        _ url: URL, validas: [Int32], premio: Int, n: Int, entorno: Entorno
    ) async throws(ErrorDominio) -> [String] {
        var r: [String] = []
        for (j, g) in try ganadoras(url, entorno: entorno).enumerated() {
            distantes(g, 14 - premio) { if validas[$0] == n { r.append("sem.\(j + 1)") } }
        }
        return r
    }

    /// CU-C16 `Grabar`: las columnas con frecuencia `n`, por índice (el fichero se crea aunque no haya ninguna).
    public static func grabarColumnasConFrecuencia(_ validas: [Int32], n: Int, en url: URL, entorno: Entorno) throws(ErrorDominio) {
        let cols = validas.indices.filter { validas[$0] == n }.map(FiltroPorGrupos.n1s)
        try entorno.archivos.escribir(TextoNET.codificarLineas(cols), en: url, atomico: false)
    }
}
