import Foundation
import Testing

@testable import Free1X2CasosDeUso

/// ESPEC_08 §17 (CU-A38…A42).
@Suite("Coincidencias")
struct CoincidenciasTests {
    func datos(_ lineas: [String]) -> Data { Data(lineas.map { $0 + "\r\n" }.joined().utf8) }

    /// `n1s` de una vez (DM-22) da lo mismo que anteponiendo signo a signo, como el C#.
    @Test func n1sComoAntes() {
        func antes(_ n: Int) -> String {
            var (x, s) = (n, "")
            for _ in 0..<14 {
                s = ["X", "1", "2"][x % 3] + s
                x /= 3
            }
            return s
        }
        for n in [0, 1, 2, 3, 4_782_968] + stride(from: 5, to: 4_782_969, by: 997) {
            #expect(FiltroPorGrupos.n1s(n) == antes(n), "\(n)")
        }
    }

    @Test func radiografia() {
        var e = [Int](repeating: 0, count: 108)
        for (k, v) in [10: 14, 14: 7, 18: 7, 22: 1, 26: 1, 30: 1, 36: 13, 46: 6, 56: 6, 63: 6, 65: 6, 68: 7, 70: 7, 73: 7, 75: 7] {
            e[k] = v
        }
        #expect(CasosUtilidades.radiografia("11111111111111") == e)
        let a = CasosUtilidades.radiografia("1X1X1X1X1X1X1X")
        #expect(Array(a[0..<8]) == [7, 7, 0, 3, 3, 0, 4, 4])
        #expect(Array(a[78..<83]) == [13, 6, 7, 6, 0] && Array(a[93..<98]) == [13, 6, 7, 6, 0])
        #expect(CasosUtilidades.descripcionesRadiografia.count == 108)
    }

    @Test func condiciones() throws {
        let c = try #require(CasosUtilidades.condicionesCoincidencias([("1-9,33", "0-3")]))
        #expect(c.grupos[0].indices.filter { c.grupos[0][$0] } == Array(0..<9) + [32])
        #expect(c.rangos[0].indices.filter { c.rangos[0][$0] } == [0, 1, 2, 3])
        for mala in [("0", "1"), ("1", "109"), ("110", "1"), ("", "1"), ("1-2-3", "1")] {
            #expect(CasosUtilidades.condicionesCoincidencias([mala]) == nil)
        }
        #expect(CasosUtilidades.condicionesCoincidencias([]) == nil)
    }

    @Test func filtrar() async throws {
        let e = await CasosUtilidades.entradaCoincidencias(
            datos(["11111111111111", "11111111111111", "x1111111111111", "1X1X1X1X1X1X1X", "corta"]))
        #expect(e.leidas == 4 && e.entrada.cuenta == 3 && e.error == "Error longitud=CORTA")
        let g = CasosUtilidades.ganadorasCoincidencias(datos(["11111111111111x", "1X1X1X1X1X1X1X", "mal"]))
        #expect(g.columnas == ["11111111111111", "1X1X1X1X1X1X1X"] && g.corta == "MAL" && !g.desborde)
        let c = try #require(CasosUtilidades.condicionesCoincidencias([("1-108", "108")]))
        let ref = CasosUtilidades.radiografia("11111111111111")
        let r = await CasosUtilidades.filtrarCoincidencias(e.entrada, referencia: ref, grupos: c.grupos, rangos: c.rangos) { _, _ in }
        #expect(r.procesadas == 3 && r.validas.map(\.coincidencias) == [108] && r.ac[108] == 1)
        var marcadas = [Bool](repeating: true, count: 109)
        #expect(CasosUtilidades.columnasCoincidencias(r, marcadas: marcadas) == ["11111111111111"])
        marcadas[108] = false
        #expect(CasosUtilidades.columnasCoincidencias(r, marcadas: marcadas).isEmpty)
        let x = CasosUtilidades.excelCoincidencias(r, condiciones: 1)
        #expect(x.first == "N\ts/AC\tN\ts/GR\tN\tAMC1" && x[109] == "108\t1\t108\t1\t108\t1" && x.count == 110)
        #expect(CasosUtilidades.analizarCoincidencias("11111111111111", "1X1X1X1X1X1X1X", grupos: c.grupos) == [39])
    }
}
