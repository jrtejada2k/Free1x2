import Foundation
import Free1X2CasosDeUso
import Testing

/// ESPEC_10 §105 (CU-C58…C62).
@Suite("Aidomnou")
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
        let a = await CasosUtilidades.filtrarAidomnou(lineas, columnas: c, rangos: rks)
        #expect(a.procesadas == 4 && a.admitidas == 2 && a.corta == "corta")
        #expect(await CasosUtilidades.columnasValidasAidomnou(a.validas) == ["XXXXXXXXXXXXXX", "11111111111111"])
        rks[1] = [10, 14]
        #expect(await CasosUtilidades.filtrarAidomnou(lineas, columnas: c, rangos: rks).admitidas == 1)
    }
}

/// M2 (#18): el filtro Pim, 7 rankings de 6 casillas sin suma y el recorrido.
@Suite("Filtro Pim")
struct FiltroPimTests {
    let valores = [[Double]](repeating: [60, 25, 15], count: 14)
    let porDefecto = [[3, 6], [2, 6], [2, 6], [2, 6], [2, 6], [0, 1], [0, 1], [0, 14]]

    func datos(_ lineas: [String]) -> Data { Data(lineas.map { $0 + "\r\n" }.joined().utf8) }

    @Test func rankings() throws {
        let r = try CasosUtilidades.rankingsPim(valores)
        func ranking(_ k: Int) -> String { (0..<14).map { CasosUtilidades.cambiaAidomnou(r[$0][k]) }.joined() }
        #expect(ranking(0) == "111111--------")
        #expect(ranking(1) == "------111111--")
        #expect(ranking(2) == "XXXX--------11")
        #expect(ranking(3) == "----XXXXXX----")
        #expect(ranking(4) == "22--------XXXX")
        #expect(ranking(5) == "--222222------")
        #expect(ranking(6) == "--------222222")
        #expect(CasosUtilidades.exportarPim(r).count == 7)
        #expect(throws: ErrorDominio.self) { try CasosUtilidades.rankingsPim(Array(valores.prefix(13))) }
    }

    @Test func aciertosYAdmision() throws {
        let r = try CasosUtilidades.rankingsPim(valores)
        let a = try CasosUtilidades.aciertosPim(r, columna: "111XXX11X1XX12")
        #expect(a == [3, 3, 2, 3, 2, 0, 1] && CasosUtilidades.recorridoPim(a) == 3)
        #expect(try CasosUtilidades.aciertosPim(r, columna: "111xxx11x1xx12") == a)
        #expect(CasosUtilidades.admitePim(a, porDefecto))
        var recorrido4 = porDefecto
        recorrido4[7] = [4, 14]
        #expect(!CasosUtilidades.admitePim(a, recorrido4))
        let unos = try CasosUtilidades.aciertosPim(r, columna: "11111111111111")
        #expect(unos == [6, 6, 2, 0, 0, 0, 0] && !CasosUtilidades.admitePim(unos, porDefecto))
        #expect(throws: ErrorDominio.self) { try CasosUtilidades.aciertosPim(r, columna: "1X21") }
    }

    @Test func filtrar() async throws {
        let r = try CasosUtilidades.rankingsPim(valores)
        let f = await CasosUtilidades.filtrarPim(
            datos(["11111111111111", "111xxx11x1xx12", "111XXX11X1XX12", "corta", "111XXX11X1XX12"]), rankings: r, rangos: porDefecto)
        #expect(f.admitidas == ["111XXX11X1XX12"] && f.procesadas == 4 && f.corta)
        let g = await CasosUtilidades.filtrarPim(datos(["111XXX11X1XX12", "11111111111111"]), rankings: r, rangos: porDefecto)
        #expect(g.admitidas == ["111XXX11X1XX12"] && g.procesadas == 2 && !g.corta)
    }
}
