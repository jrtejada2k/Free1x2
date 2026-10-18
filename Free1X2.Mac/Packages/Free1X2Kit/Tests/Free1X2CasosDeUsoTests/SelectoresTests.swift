import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_10 §86 (CU-C23…C27) y §87 (CU-C28, CU-C29).
@Suite("Selectores JuanM y MarioSan")
struct SelectoresTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ texto: String) throws -> URL {
        let url = URL(filePath: "/tmp/sel/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(texto.utf8), en: url, atomico: true)
        return url
    }

    func columnas(_ b: ConjuntoBits) throws -> [String] {
        let u = URL(filePath: "/tmp/sel/b.txt")
        try CasosUtilidades.grabarSeleccion(b, en: u, entorno: entorno)
        return TextoNET.lineas(try entorno.archivos.leer(u))
    }

    let grupos = ["1", "1", "1", "1", "1", "2", "2", "2", "2", "2", "3", "3", "3", "3"]
    let valores = Array(repeating: [50.0, 30, 20], count: 14)

    @Test func selecJM() async throws {
        let t = try CasosUtilidades.tablasJM(valoraciones: valores, grupos: grupos, limites: ["1", "1", "1", "1-567"], cultura: .esES)
        #expect(t.porcentajes == ["3,13", "3,13", "6,25"], "50⁵ / 1e8 = 3,125: al alejarse de cero")
        let c = await CasosUtilidades.seleccionJM(t, porCorte: true)
        #expect(c.procesadas == 4_782_969 && c.validas == 1)
        #expect(try columnas(c.bits) == ["11111111111111"])
        // Posición 2 del grupo 1: cinco códigos empatan; por corte solo el primero de la ordenación, por valor todos.
        let u = try CasosUtilidades.tablasJM(valoraciones: valores, grupos: grupos, limites: ["2", "1", "1", "1-567"], cultura: .esES)
        let corte = await CasosUtilidades.seleccionJM(u, porCorte: true)
        let valor = await CasosUtilidades.seleccionJM(u, porCorte: false)
        #expect(try columnas(corte.bits) == ["X1111111111111"])
        #expect(valor.validas == 5 && valor.procesadas == 4_782_969 && valor.bits.indices.count == 5)
        #expect(CasosUtilidades.ubicarGanadoraJM(u, columna: "X1111111111111", porCorte: true) == [2, 1, 1, 4])
        #expect(CasosUtilidades.ubicarGanadoraJM(u, columna: "1X111111111111", porCorte: true) == [3, 1, 1, 5])
        #expect(CasosUtilidades.ubicarGanadoraJM(u, columna: "1X111111111111", porCorte: false) == [2, 1, 1, 4])
        func error(_ g: [String], _ l: [String]) -> String {
            do {
                _ = try CasosUtilidades.tablasJM(valoraciones: valores, grupos: g, limites: l, cultura: .esES)
                return ""
            } catch { return error.mensajeUsuario }
        }
        #expect(error(["1"] + grupos.dropFirst(), ["1", "1", "1", "1"]).isEmpty)
        #expect(error(["2"] + grupos.dropFirst(), ["1", "1", "1", "1"]) == "error en grupos")
        #expect(
            error(grupos, ["1", "-5", "1", "1"]) == "error en limites" && error(grupos, ["1", "5-3,", "1", "1"]) == "error en limites")
        #expect(error(grupos, ["300-1", "1,2", "7-9", "1"]).isEmpty)
        let d = CasosUtilidades.datosCondicionesJM(grupos: grupos, limites: ["1-3", "2", "3", "4"], porCorte: false)
        let r = try #require(CasosUtilidades.leerCondicionesJM(d))
        #expect(r.grupos == grupos && r.limites == ["1-3", "2", "3", "4"] && !r.porCorte)
        #expect(CasosUtilidades.leerCondicionesJM(Data("11111\r\n1\r\n1\r\n1\r\n1\r\n1\r\n".utf8)) == nil)
    }

    @Test func ganadoras() throws {
        let g = try CasosUtilidades.ganadorasSelector(
            try fichero("g.txt", " 1x1111111111111 \r\n\r\n111\r\n22222222222222\r\n"), entorno: entorno)
        #expect(g.columnas == ["1X1111111111111"] && g.erronea == "111")
    }

    @Test func selectorMS() async throws {
        let f = try fichero(
            "m.txt", "11111111111111\r\nX1111111111111\r\n21111111111111\r\nXX111111111111\r\n11111111111111\r\n")
        let d13 = try #require(try await CasosUtilidades.distribucionMS(f, limite: 13, entorno: entorno))
        #expect(d13.vecinos == [2, 3, 2, 1] && d13.filas == 29 && Array(d13.conteo.prefix(4)) == [0, 1, 2, 1])
        let d12 = try #require(try await CasosUtilidades.distribucionMS(f, limite: 12, entorno: entorno))
        let d11 = try #require(try await CasosUtilidades.distribucionMS(f, limite: 11, entorno: entorno))
        #expect(d12.vecinos == [1, 0, 1, 2] && d11.vecinos == [0, 0, 0, 0] && d11.filas == 2913)
        let a = try CasosUtilidades.analizarMS(d13, ganadora: "11111111111111")
        #expect(a[2] == [1, 1, 0, 0, 0] && a[3] == [0, 1, 0, 0, 0] && a[1] == [0, 0, 1, 0, 0])
        let s = URL(filePath: "/tmp/sel/s.txt")
        try CasosUtilidades.grabarMS(d13, grupo: 2, en: s, entorno: entorno)
        #expect(TextoNET.lineas(try entorno.archivos.leer(s)) == ["11111111111111", "21111111111111"])
        await #expect(throws: ErrorDominio.self) {
            try await CasosUtilidades.distribucionMS(try fichero("c.txt", "111\r\n"), limite: 13, entorno: entorno)
        }
    }
}
