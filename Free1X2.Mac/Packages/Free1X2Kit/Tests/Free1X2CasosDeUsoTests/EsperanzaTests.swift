import Foundation
import Free1X2CasosDeUso
import Testing

@testable import Free1X2CasosDeUso

/// DT-91: EM exacta por categoría, contra el ejemplo del Excel «EM Categorias.xlsm» del dueño. Los valores esperados salen
/// de un cálculo independiente en Python (mismo modelo, filas normalizadas a su suma), no del Swift.
struct EsperanzaTests {
    static let apostados: [[Double]] = [
        [65, 26, 9], [8, 16, 76], [15, 25, 60], [77, 16, 7], [45, 37, 18], [21, 32, 47], [91, 6, 3], [59, 27, 14], [29, 39, 32],
        [68, 22, 10], [31, 35, 34], [43, 35, 22], [76, 17, 7], [13, 24, 63],
    ]
    static let reales: [[Double]] = [
        [50.483, 29.487, 20.03], [17.834, 25.144, 57.022], [19.196, 27.523, 53.282], [61.776, 21.575, 16.649],
        [45.775, 30.493, 23.732], [22.847, 27.438, 49.714], [76.773, 14.886, 8.341], [48.195, 25.625, 26.18],
        [37.232, 33.017, 29.751], [50.901, 33.516, 15.583], [35.146, 33.792, 31.062], [45.181, 30.249, 24.57],
        [56.921, 29.003, 14.077], [22.433, 35.253, 42.314],
    ]
    static let columna = "1121121221112X"
    static let recaudacion = 3_153_494.25

    func em(precio: Double, masUno: Bool = true) throws -> ResultadoEsperanza {
        var p = ParametrosEsperanza(recaudacion: Self.recaudacion, precio: precio)
        p.contarmeComoAcertante = masUno
        return try CasosEsperanza.calcular(columna: Self.columna, apostados: Self.apostados, reales: Self.reales, p)
    }

    func cerca(_ a: [Double], _ b: [Double]) -> Bool { zip(a, b).allSatisfy { abs($0 - $1) < 1e-9 * max(1, abs($1)) } }

    /// El Excel «Probabilidades categorias»: con 60 % en los 14 partidos, 0,6¹⁴ para 14 y 14 · 0,6¹³ · 0,4 para 13.
    @Test func distribucion() {
        let d = CasosEsperanza.distribucion(Array(repeating: 0.6, count: 14))
        #expect(abs(d[14] - pow(0.6, 14)) < 1e-15 && abs(d[13] - 14 * pow(0.6, 13) * 0.4) < 1e-15)
        #expect(abs(d.reduce(0, +) - 1) < 1e-12)
    }

    /// A 1 € la apuesta (3.153.494 apuestas) la columna devuelve el 107 % de lo apostado; a 0,75 € (4.204.659), el 119 %.
    @Test func ejemploDelExcel() throws {
        let uno = try em(precio: 1)
        #expect(cerca(uno.categorias.map(\.esperanza), [0.339387083746, 0.259952860574, 0.199802964982, 0.145122171329, 0.129636490232]))
        #expect(abs(uno.total - 1.073901570862) < 1e-9 && abs(uno.rendimiento - 1.073901570862) < 1e-9)
        let tresCuartos = try em(precio: 0.75)
        #expect(abs(tresCuartos.total - 0.894180459434) < 1e-9 && abs(tresCuartos.apuestas - 4_204_659) < 1e-6)
        #expect(abs(tresCuartos.rendimiento - 0.894180459434 / 0.75) < 1e-9)
        // Sin contarse como acertante (como mínimo 1).
        #expect(abs(try em(precio: 1, masUno: false).total - 1.423042097518) < 1e-9)
        // Probabilidades de la propia columna: fracción apostada (si saliera) y real.
        #expect(
            cerca(
                uno.categorias.map(\.probabilidadApostada),
                [3.154448497271e-07, 1.394103042827e-05, 2.494557948604e-04, 2.385222412527e-03, 1.370617492755e-02]))
        #expect(
            cerca(
                uno.categorias.map(\.probabilidadReal),
                [1.617487413171e-06, 4.323052551039e-05, 5.083017408476e-04, 3.496904024709e-03, 1.577808400922e-02]))
        #expect(uno.categorias.map(\.aciertos) == [14, 13, 12, 11, 10])
    }

    /// Retención: un premio de 140.000 € cobra 40.000 + 100.000 · 0,8 = 120.000. Sin retención, el 14 cobra entero.
    @Test func retencionYMinimos() throws {
        var p = ParametrosEsperanza(recaudacion: Self.recaudacion, precio: 1)
        let con = try CasosEsperanza.calcular(columna: Self.columna, apostados: Self.apostados, reales: Self.reales, p)
        p.retencion = 0
        let sin = try CasosEsperanza.calcular(columna: Self.columna, apostados: Self.apostados, reales: Self.reales, p)
        #expect(sin.categorias[0].esperanza > con.categorias[0].esperanza)
        p.minimo10 = 1_000_000
        let sin10 = try CasosEsperanza.calcular(columna: Self.columna, apostados: Self.apostados, reales: Self.reales, p)
        #expect(sin10.categorias[4].esperanza == 0 && sin10.categorias[0].esperanza == sin.categorias[0].esperanza)
    }

    @Test func errores() {
        let p = ParametrosEsperanza(recaudacion: Self.recaudacion, precio: 1)
        #expect(throws: ErrorDominio.self) {
            try CasosEsperanza.calcular(columna: "112112", apostados: Self.apostados, reales: Self.reales, p)
        }
        var ceros = Self.reales
        ceros[3] = [0, 0, 0]
        let e = #expect(throws: ErrorDominio.self) {
            try CasosEsperanza.calcular(columna: Self.columna, apostados: Self.apostados, reales: ceros, p)
        }
        #expect(e?.mensajeUsuario == "Faltan los porcentajes reales del partido 4.")
        #expect(throws: ErrorDominio.self) {
            try CasosEsperanza.calcular(
                columna: Self.columna, apostados: Self.apostados, reales: Self.reales, ParametrosEsperanza(recaudacion: 1, precio: 0))
        }
    }
}
