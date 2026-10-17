import Foundation
import Free1X2Base

// Nuevo (DT-91, a petición del dueño; no existe en el C#): esperanza matemática (EM) de una columna por categoría, como el
// Excel «EM Categorias.xlsm» del dueño (hojas «EM A 1 EURO» y «EM A 0.75 EURO», con y sin «+1 AC»), en su versión exacta.

/// Parámetros de la jornada y del cobro de premios.
public struct ParametrosEsperanza: Sendable, Hashable {
    /// Reparto del Excel del dueño: % de la recaudación para 14, 13, 12, 11 y 10 aciertos.
    public static let repartoExcel = [16, 7.5, 7.5, 7.5, 9]
    /// Reparto que trae «Estimación de premios».
    public static let repartoFree1X2: [Double] = [12, 8, 8, 8, 9]

    public var recaudacion: Double
    /// Precio de una apuesta: nº de apuestas = recaudación ÷ precio.
    public var precio: Double
    /// % de la recaudación para 14, 13, 12, 11 y 10 aciertos.
    public var reparto: [Double]
    /// % que se retiene de la parte del premio que pasa de `exento`.
    public var retencion = 20.0
    public var exento = 40_000.0
    /// Por debajo de estos importes la categoría no se cobra (de 14 a 11 y con 10).
    public var minimo = 1.0
    public var minimo10 = 1.5
    /// «+1 AC» del Excel: si aciertas, eres un acertante más de los estimados.
    public var contarmeComoAcertante = true

    public init(recaudacion: Double, precio: Double, reparto: [Double] = Self.repartoExcel) {
        (self.recaudacion, self.precio, self.reparto) = (recaudacion, precio, reparto)
    }
}

/// Resultado de una categoría (14…10 aciertos).
public struct CategoriaEsperanza: Sendable, Hashable, Identifiable {
    public let aciertos: Int
    /// Probabilidad real de que la columna saque exactamente estos aciertos.
    public let probabilidadReal: Double
    /// Fracción de lo apostado que tendría estos aciertos si la columna fuera la ganadora.
    public let probabilidadApostada: Double
    /// Premio neto medio cuando la columna saca esta categoría (EM ÷ probabilidad real).
    public let premioMedio: Double
    public let esperanza: Double
    public var id: Int { aciertos }
}

public struct ResultadoEsperanza: Sendable, Hashable {
    public let categorias: [CategoriaEsperanza]
    public let apuestas: Double
    public let precio: Double
    public var total: Double { categorias.map(\.esperanza).reduce(0, +) }
    /// EM por cada euro apostado (1 = se recupera lo apostado).
    public var rendimiento: Double { total / precio }
}

public enum CasosEsperanza {
    /// Probabilidad de exactamente k aciertos (k = 0…n) si el partido i se acierta con probabilidad `p[i]`.
    static func distribucion(_ p: [Double]) -> [Double] {
        var d = [1.0]
        for q in p {
            var n = [Double](repeating: 0, count: d.count + 1)
            for (k, v) in d.enumerated() {
                n[k] += v * (1 - q)
                n[k + 1] += v * q
            }
            d = n
        }
        return d
    }

    /// Probabilidad de 1, X y 2 por partido: cada fila dividida por su suma (unas valoraciones que sumen 99 o 101 siguen
    /// siendo una distribución). Se usan los 14 primeros partidos.
    static func normalizar(_ v: [[Double]], _ nombre: String) throws(ErrorDominio) -> [[Double]] {
        guard v.count >= 14 else { throw ErrorDominio(.argumento, "Faltan los porcentajes \(nombre) de los 14 partidos.") }
        return try v.prefix(14).enumerated().map { (i, fila) throws(ErrorDominio) in
            let s = fila.prefix(3).reduce(0, +)
            guard fila.count >= 3, s > 0, fila.prefix(3).allSatisfy({ $0 >= 0 }) else {
                throw ErrorDominio(.argumento, "Faltan los porcentajes \(nombre) del partido \(i + 1).")
            }
            return fila.prefix(3).map { $0 / s }
        }
    }

    /// EM exacta de `columna`: recorre los 19.321 resultados con premio (el propio, los 28 a un signo, 364 a dos, 2.912 a
    /// tres y 16.016 a cuatro). En cada uno: acertantes = fracción apostada con esos aciertos × nº de apuestas (+1 si te
    /// cuentas; como mínimo 1), premio bruto = fondo de la categoría ÷ acertantes, neto tras el mínimo y la retención, y
    /// se suma probabilidad real del resultado × neto.
    public static func calcular(columna: String, apostados: [[Double]], reales: [[Double]], _ p: ParametrosEsperanza)
        throws(ErrorDominio) -> ResultadoEsperanza
    {
        let col = columna.uppercased().compactMap { "1X2".firstIndex(of: $0).map { "1X2".distance(from: "1X2".startIndex, to: $0) } }
        guard col.count == 14, columna.count == 14 else {
            throw ErrorDominio(.argumento, "La columna debe tener 14 signos 1, X o 2.")
        }
        guard p.recaudacion > 0, p.precio > 0, p.reparto.count == 5 else {
            throw ErrorDominio(.argumento, "La recaudación y el precio de la apuesta deben ser mayores que 0.")
        }
        let ap = try normalizar(apostados, "apostados"), re = try normalizar(reales, "reales")
        let apuestas = p.recaudacion / p.precio
        func neto(_ bruto: Double, _ aciertos: Int) -> Double {
            if bruto < (aciertos == 10 ? p.minimo10 : p.minimo) { return 0 }
            return bruto > p.exento ? p.exento + (bruto - p.exento) * (1 - p.retencion / 100) : bruto
        }
        var esperanza = [Double](repeating: 0, count: 5), probabilidad = esperanza
        var w = col
        func visitar(desde: Int, cambios: Int) {
            let aciertos = 14 - cambios
            let x = distribucion((0..<14).map { ap[$0][w[$0]] })[aciertos] * apuestas
            let acertantes = max(1, p.contarmeComoAcertante ? (x + 1.5).rounded(.down) : x.rounded(.toNearestOrAwayFromZero))
            let real = (0..<14).reduce(1.0) { $0 * re[$1][w[$1]] }
            esperanza[cambios] += real * neto(p.reparto[cambios] / 100 * p.recaudacion / acertantes, aciertos)
            probabilidad[cambios] += real
            guard cambios < 4 else { return }
            for i in desde..<14 {
                let original = w[i]
                for d in 1...2 {
                    w[i] = (original + d) % 3
                    visitar(desde: i + 1, cambios: cambios + 1)
                }
                w[i] = original
            }
        }
        visitar(desde: 0, cambios: 0)
        let propia = distribucion((0..<14).map { ap[$0][col[$0]] })
        let categorias = (0..<5).map { m in
            CategoriaEsperanza(
                aciertos: 14 - m, probabilidadReal: probabilidad[m], probabilidadApostada: propia[14 - m],
                premioMedio: probabilidad[m] > 0 ? esperanza[m] / probabilidad[m] : 0, esperanza: esperanza[m])
        }
        return ResultadoEsperanza(categorias: categorias, apuestas: apuestas, precio: p.precio)
    }
}
