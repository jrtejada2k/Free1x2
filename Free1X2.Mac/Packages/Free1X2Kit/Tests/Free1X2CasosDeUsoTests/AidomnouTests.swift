import Foundation
import Free1X2CasosDeUso
import Testing

/// ESPEC_10 §105 (CU-C58…C62) y ESPEC_09 §57 (mismo motor, DEC-922).
@Suite("Aidomnou y Pim")
struct AidomnouTests {
    let valores = [[Double]](repeating: [60, 25, 15], count: 14)

    func datos(_ lineas: [String]) -> Data { Data(lineas.map { $0 + "\r\n" }.joined().utf8) }

    @Test func columnas() throws {
        let c = try CasosUtilidades.columnasAidomnou(valores)
        #expect(c.cps.map { $0[0] } == Array(repeating: 5, count: 13) + [7])
        #expect(c.cps.map { $0[1] } == [6, 6, 6, 7, 7, 7, 7, 7, 5, 1, 1, 1, 1, 1])
        #expect(c.cps.map { $0[2] } == Array(repeating: 5, count: 14))
        #expect(c.cps.map { $0[3] } == Array(repeating: 1, count: 7) + Array(repeating: 5, count: 7))
        #expect(c.cps.map { $0[4] } == [6, 6, 6, 6, 6, 5, 1, 1, 1, 1, 1, 1, 1, 1])
        #expect(c.cps.map { $0[5] } == Array(repeating: 1, count: 14))
        #expect(CasosUtilidades.exportarAidomnou(c)[0] == Array(repeating: "1X", count: 13).joined(separator: ",") + ",1X2")
        #expect(try CasosUtilidades.aciertosAidomnou(c, columna: "11111111111111") == [14, 11, 14, 14, 9, 14])
        #expect(try CasosUtilidades.aciertosAidomnou(c, columna: "44444444444444") == [14, 9, 14, 7, 6, 0])
        #expect(throws: ErrorDominio.self) { try CasosUtilidades.columnasAidomnou(Array(valores.prefix(13))) }
        #expect(throws: ErrorDominio.self) { try CasosUtilidades.aciertosAidomnou(c, columna: "COL.GANADORA") }
    }

    @Test func rangos() {
        #expect(CasosUtilidades.rangosAidomnou(["0-14", "3-5-7", " 2 -6 "]) == [[0, 14], [3, 5], [2, 6]])
        for malo in ["a-b", "-3-5", "5", ""] { #expect(CasosUtilidades.rangosAidomnou([malo]) == nil) }
        #expect(CasosUtilidades.ganadorasAidomnou(datos(["  1x2X1111111111 extra"])) == ["1x2X1111111111"])
        #expect(CasosUtilidades.ganadorasAidomnou(datos(["11111111111111", "1X2"])) == nil)
    }

    @Test func filtrar() async throws {
        let c = try CasosUtilidades.columnasAidomnou(valores)
        var rks = [[Int]](repeating: [0, 14], count: 6) + [[0, 84], [0, 14]]
        let lineas = datos(["11111111111111", "XXXXXXXXXXXXXX", "11111111111111", "", "corta"])
        let a = await CasosUtilidades.filtrarAidomnou(lineas, columnas: c, rangos: rks, saltarVacias: true)
        #expect(a.procesadas == 4 && a.admitidas == 2 && a.corta == "corta")
        #expect(await CasosUtilidades.columnasValidasAidomnou(a.validas) == ["XXXXXXXXXXXXXX", "11111111111111"])
        let p = await CasosUtilidades.filtrarAidomnou(lineas, columnas: c, rangos: rks, saltarVacias: false)
        #expect(p.procesadas == 4 && p.corta == "")
        rks[1] = [10, 14]
        #expect(await CasosUtilidades.filtrarAidomnou(lineas, columnas: c, rangos: rks, saltarVacias: true).admitidas == 1)
    }
}
