import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/BancoPruebasFrmViewModel.cs (08 pantalla 10; CU-A16…A24) y DialogoGrabarBancoPruebasFrmViewModel.cs
// (pantalla 36). Columnas como índices de `ConvertidorDeBases` (partido 1 el dígito menos significativo, 1 = 0, X = 1, 2 = 2).

/// Una fila del resultado por columnas (modos «Columnas» y «Autoescrutinio»).
public struct FilaBancoColumna: Sendable, Hashable, Identifiable {
    public var id: Int { indice }
    public let indice: Int
    public let columna: String
    public let columnaNumero: Int
    /// Veces con 14…10.
    public let veces: [Int]
    public let premio: String
    public let recuperacion: String
    public var seleccionado = false

    public init(
        indice: Int, columna: String, columnaNumero: Int, veces: [Int], premio: String, recuperacion: String, seleccionado: Bool = false
    ) {
        (self.indice, self.columna, self.columnaNumero, self.veces) = (indice, columna, columnaNumero, veces)
        (self.premio, self.recuperacion, self.seleccionado) = (premio, recuperacion, seleccionado)
    }

    public var resumen: String {
        FormatoNET.recortar(
            "\(columna)    14:\(veces[0])  13:\(veces[1])  12:\(veces[2])  11:\(veces[3])  10:\(veces[4])    premio:\(premio)  \(recuperacion)"
        )
    }
}

/// Lo que cambia un análisis; `nil` = el C# no toca ese texto o esa lista.
public struct ResultadoBanco: Sendable {
    public var numColumnas: String?
    public var gasto: String?
    public var premioTotal: String?
    public var recuperado: String?
    public var vecesPremiada: String?
    public var vecesBeneficio: String?
    public var filas: [String]?
    public var columnas: [FilaBancoColumna]?
    public var jornadas: [String]?
}

public enum TipoAnalisisBanco: String, Sendable, CaseIterable {
    case combinacion = "Combinación"
    case columnas = "Columnas"
    case jornadas = "Jornadas"
    case autoescrutinio = "Autoescrutinio"
}

extension CasosUtilidades {
    static let pctBanco = [0.12, 0.08, 0.08, 0.08, 0.09]
    static let recaudacionBanco = 14_000_000.0
    static let precioBanco = 0.5

    /// `LeerColumnas` / `CargarFicheroDeColumnas`: `ConvColumnaANumero` de la línea entera.
    public static func columnasBanco(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> [Int] {
        let conv = ConvertidorDeBases(numeroPartidos: 14)
        var r: [Int] = []
        for l in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url)) { r.append(Int(try conv.numero(l))) }
        return r
    }

    /// `Aciertos`: dígitos base 3 iguales en los 14 partidos.
    static func aciertosBanco(_ a: Int, _ b: Int) -> Int {
        (0..<14).count { (a / potencias3[$0]) % 3 == (b / potencias3[$0]) % 3 }
    }

    /// CU-A16 `CalcularReales` + `Calcula14Triples`: las 3^14 columnas por |ln P − LN| (quicksort propio, no estable) y
    /// el % de cada signo en las `columnas` primeras (dividido por `columnas / 100` entera: con menos de 100, ∞/NaN).
    @concurrent public static func valoracionReal(
        apostados: [[Double]], ln: Double, columnas numCol: Int
    ) async throws(ErrorDominio) -> [[Double]] {
        let va = try Porcentajes(valores: apostados).valoresNeperianos(numeroPartidos: apostados.count)
        var prob: Float = 0
        var cra = [[Float]](repeating: [0, 0, 0], count: 14)
        for p in 0..<14 {
            let f = try va.leer(p, "BancoPruebasFrmViewModel.cs:CalcularReales")
            prob += f[0]
            (cra[p][1], cra[p][2]) = (f[1] - f[0], f[2] - f[0])
        }
        let lnF = Float(ln)
        // ponytail: solo diferencia y columna (8 B) en vez de `ApuestaProbableCentral` (12 B); la probabilidad no se lee.
        var ap = [(dif: Float, col: Int32)](repeating: (0, 0), count: 4_782_969)
        func encontrar(_ p0: Float, _ ini: Int, _ desde: Int) {
            for p in desde..<14 {
                for z in 1...2 {
                    let idx = ini + potencias3[p] * z
                    let pr = p0 + cra[p][z]
                    ap[idx] = (abs(pr - lnF), Int32(idx))
                    encontrar(pr, idx, p + 1)
                }
            }
        }
        encontrar(prob, 0, 0)
        ap[0] = (abs(prob - lnF), 0)
        OrdenacionPropia.ordenar(&ap) { $0.dif }
        var r = [[Double]](repeating: [0, 0, 0], count: 14)
        for i in stride(from: 0, to: numCol, by: 1) {
            let c = Int(try ap.leer(i, "BancoPruebasFrmViewModel.cs:Calcula14Triples").col)
            for p in 0..<14 { r[p][(c / potencias3[p]) % 3] += 1 }
        }
        let divisor = Double(numCol / 100)
        return r.map { $0.map { $0 / divisor } }
    }

    /// CU-A17 `GenerarAleatorias`: un signo por partido según los % reales; LN medio y su desviación con los apostados.
    public static func columnasAleatorias(
        reales p: [[Double]], apostados v: [[Double]], cantidad: Int, aleatorio: inout AleatorioNET
    ) -> (columnas: [Int], desviacion: Double, lnMedia: Double) {
        var (mpx, lnMedia, lnVari, lnDT) = (0.0, 0.0, 0.0, 0.0)
        var cols: [Int] = []
        for z in 0..<max(0, cantidad) {
            var prob = 1.0
            var b = 0
            for j in 0..<14 {
                let num = aleatorio.nextDouble()
                if num < p[j][0] / 100 {
                    prob *= v[j][0] / 100
                } else if num < (p[j][0] + p[j][1]) / 100 {
                    prob *= v[j][1] / 100
                    b += potencias3[j]
                } else {
                    prob *= v[j][2] / 100
                    b += 2 * potencias3[j]
                }
            }
            let zd = Double(z)
            mpx = (mpx * zd + prob) / (zd + 1)
            let (l, lm) = (log(prob), log(mpx))
            lnMedia = (lnMedia * zd + l) / (zd + 1)
            lnVari = (lnVari * zd + (l - lm) * (l - lm)) / (zd + 1)
            lnDT = lnVari.squareRoot()
            cols.append(b)
        }
        return (cols, lnDT, lnMedia)
    }

    /// CU-A18 `CalcularPremios`: reparto del L.A.E. (14, 13, 12, 11, 10) para un 14 virtual con las valoraciones `pa`.
    static func premiosBanco(_ ganadora: Int, _ pa: [[Float]]) -> [Double] {
        var prob14 = 1.0
        var cr = [Float](repeating: 0, count: 14)
        for p in 0..<14 {
            let s = (ganadora / potencias3[p]) % 3
            prob14 *= Double(pa[p][s])
            cr[p] = (1 - pa[p][s]) / pa[p][s]
        }
        var suma = [Double](repeating: 0, count: 5)
        func sumar(_ p0: Double, _ desde: Int, _ nivel: Int) {
            for p in desde..<14 {
                let pr = p0 * Double(cr[p])
                suma[nivel] += pr
                if nivel < 4 { sumar(pr, p + 1, nivel + 1) }
            }
        }
        sumar(prob14, 0, 1)
        var premios = (0..<5).map { FormatoNET.round(pctBanco[$0] * precioBanco / ($0 == 0 ? prob14 : suma[$0]), decimales: 2) }
        for i in 0..<5 where premios[i] > recaudacionBanco * pctBanco[i] { premios[i] = recaudacionBanco * pctBanco[i] }
        if premios[4] < 1 { premios[4] = 0 }
        return premios
    }

    /// CU-A19…A22 `Analizar`.
    @concurrent public static func analizarBanco(
        _ tipo: TipoAnalisisBanco, fichero: URL, apostados: [[Double]], aleatorias: [Int], acumular: [Bool], numeroPartidos: Int,
        entorno: Entorno
    ) async throws(ErrorDominio) -> ResultadoBanco {
        let cultura = entorno.cultura
        func d(_ v: Double) -> String { FormatoNET.toString(v, cultura: cultura) }
        let cols = try columnasBanco(fichero, entorno: entorno)
        let pa = try Porcentajes(valores: apostados).valoresBase100(numeroPartidos: apostados.count)
        let coste = Double(cols.count) * precioBanco
        var r = ResultadoBanco(numColumnas: String(cols.count))
        switch tipo {
        case .combinacion:
            var aciertos = [Int](repeating: 0, count: 15)
            var ingresos = [Double](repeating: 0, count: 5)
            var (premiada, beneficio) = (0, 0)
            let gasto = coste * Double(aleatorias.count)
            for i in aleatorias {
                let premios = premiosBanco(i, pa)
                var (gana, saldo) = (false, -coste)
                for j in cols {
                    let n = aciertosBanco(i, j)
                    aciertos[n] += 1
                    if n > 9 {
                        ingresos[14 - n] += premios[14 - n]
                        gana = true
                        saldo += premios[14 - n]
                    }
                }
                if gana { premiada += 1 }
                if saldo > 0 { beneficio += 1 }
            }
            r.filas = (0..<15).map { i in
                let cat = i >= 10 ? FormatoNET.round(ingresos[14 - i], decimales: 0) : 0
                let media = aciertos[i] > 0 ? FormatoNET.round(cat / Double(aciertos[i]), decimales: 2) : 0
                return "\(i) aciertos\tveces: \(aciertos[i])\tpremios: \(d(cat))\tmedia: \(d(media))"
            }
            let total = FormatoNET.round((10..<15).reduce(0.0) { $0 + ingresos[14 - $1] }, decimales: 0)
            (r.gasto, r.premioTotal, r.vecesBeneficio, r.vecesPremiada) = (d(gasto), d(total), String(beneficio), String(premiada))
            r.recuperado = d(FormatoNET.round(100 * total / gasto, decimales: 0)) + "%"
        case .columnas, .autoescrutinio:
            let auto = tipo == .autoescrutinio
            let escrutinios = auto ? cols : aleatorias
            var veces = [[Int]](repeating: [0, 0, 0, 0, 0], count: cols.count)
            var premiosCol = [[Double]](repeating: [0, 0, 0, 0, 0], count: cols.count)
            var (premiada, beneficio) = (0, 0)
            for (k, i) in escrutinios.enumerated() {
                let premios = premiosBanco(i, pa)
                var (gana, saldo) = (false, -coste)
                for (jj, j) in cols.enumerated() {
                    let n = aciertosBanco(i, j)
                    guard n > 9 else { continue }
                    // En el autoescrutinio se acumula en la columna que hace de 14.
                    let destino = auto ? k : jj
                    veces[destino][14 - n] += 1
                    premiosCol[destino][14 - n] += premios[14 - n]
                    gana = true
                    saldo += premios[14 - n]
                }
                if gana { premiada += 1 }
                if saldo > 0 { beneficio += 1 }
            }
            let conv = ConvertidorDeBases(numeroPartidos: numeroPartidos)
            var filas: [FilaBancoColumna] = []
            for (k, c) in cols.enumerated() {
                let suma = (0..<5).reduce(0.0) { acumular[$1] ? $0 + premiosCol[k][$1] : $0 }
                let n = Double(escrutinios.count)
                let rec = escrutinios.isEmpty ? 0 : FormatoNET.round(suma * 100 / n / 0.5, decimales: 0)
                filas.append(
                    FilaBancoColumna(
                        indice: k, columna: try conv.columna(Int64(c)), columnaNumero: c, veces: veces[k],
                        premio: d(FormatoNET.round(suma, decimales: 1)), recuperacion: d(rec) + " %"))
            }
            r.columnas = filas
            r.vecesBeneficio = String(beneficio)
            if auto {
                (r.gasto, r.premioTotal, r.vecesPremiada, r.recuperado) = (d(coste), "", "", "")
            } else {
                let gasto = coste * Double(aleatorias.count)
                let total = FormatoNET.round(
                    premiosCol.reduce(0.0) { $0 + $1.reduce(0.0) { $0 + FormatoNET.round($1, decimales: 0) } }, decimales: 0)
                (r.gasto, r.premioTotal, r.vecesPremiada) = (d(gasto), d(total), String(premiada))
                r.recuperado = d(gasto > 0 ? FormatoNET.round(100 * total / gasto, decimales: 0) : 0) + "%"
            }
        case .jornadas:
            var saldo = 0.0
            var lineas: [String] = []
            for (k, i) in aleatorias.enumerated() {
                let premios = premiosBanco(i, pa)
                var aciertos = [0, 0, 0, 0, 0]
                var ingresos = [0.0, 0, 0, 0, 0]
                for j in cols {
                    let n = aciertosBanco(i, j)
                    if n > 9 {
                        aciertos[14 - n] += 1
                        ingresos[14 - n] += premios[14 - n]
                    }
                }
                let importe = ingresos.reduce(0, +)
                saldo = saldo - coste + importe
                lineas.append(
                    FormatoNET.recortar(
                        "Jornada \(k + 1)    14:\(aciertos[0])  13:\(aciertos[1])  12:\(aciertos[2])  11:\(aciertos[3])  10:\(aciertos[4])    premios:\(d(FormatoNET.round(importe, decimales: 2)))  saldo:\(d(FormatoNET.round(saldo, decimales: 2)))"
                    ))
            }
            r.jornadas = lineas
        }
        return r
    }

    /// CU-A24, valores iniciales del diálogo: primera y última marcadas (1-based) y cuántas.
    public static func rangoGrabacionBanco(_ filas: [FilaBancoColumna]) -> (inicial: Int, final: Int, cuantas: Int) {
        let marcadas = filas.indices.filter { filas[$0].seleccionado }
        return ((marcadas.first ?? 0) + 1, (marcadas.last ?? 0) + 1, marcadas.count)
    }

    /// CU-A24 `GrabarAsync` tras el diálogo (el cálculo de la fila final mezcla 1-based con `n − inicial`, se replica).
    public static func columnasGrabacionBanco(
        _ filas: [FilaBancoColumna], inicial: Int, final: Int, maximo: Int, soloSeleccionadas: Bool
    ) -> [String] {
        let n = filas.count
        var c2 = final + 1
        if c2 > n - inicial { c2 = n - inicial }
        let (ini, fin) = (max(0, inicial - 1), min(n, c2))
        var r: [String] = []
        for f in filas[min(ini, n)..<max(min(ini, n), fin)] where !soloSeleccionadas || f.seleccionado {
            r.append(f.columna)
            if maximo > 0 && r.count == maximo { break }
        }
        return r
    }

    /// CU-A23 `SimplificarFilas`: `fila` (0-based) para «sólo la fila actual», `nil` para «cada fila seleccionada» (que
    /// compara siempre con la columna 0, «todo 1»: se replica). Devuelve el aviso si la fila no existe.
    public static func simplificarBanco(_ filas: inout [FilaBancoColumna], fila: Int?, diferencias: Double) -> String? {
        let dif = Int(UInt8(truncatingIfNeeded: 14 - Int(FormatoNET.castInt32(diferencias))))
        func deselecciona(_ c: Int, _ desde: Int) {
            for i in stride(from: desde, to: filas.count, by: 1) where filas[i].seleccionado && c != filas[i].columnaNumero {
                if aciertosBanco(c, filas[i].columnaNumero) > dif { filas[i].seleccionado = false }
            }
        }
        if let fila {
            guard filas.indices.contains(fila) else { return "El nº de fila está fuera de rango." }
            deselecciona(filas[fila].columnaNumero, 0)
            filas[fila].seleccionado = true
        } else {
            for i in filas.indices where filas[i].seleccionado { deselecciona(0, i) }
        }
        return nil
    }
}
