import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_10 §85 (CU-C22), §77 (CU-C09) y §96 (CU-C41).
@Suite("Rotación, parejas y tríos")
struct RotacionGruposTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ texto: String) throws -> URL {
        let url = URL(filePath: "/tmp/rg/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(texto.utf8), en: url, atomico: true)
        return url
    }

    @Test func rotacion() async throws {
        let e = try fichero("e.txt", "111\r\n1X2\r\n")
        #expect(try CasosUtilidades.entradaRotacion(e, numeroPartidos: 14, entorno: entorno) == (3, "111"))
        let s = URL(filePath: "/tmp/rg/s.txt")
        // 1→X en todos los partidos, X y 2 sin cambio.
        let n = Array(repeating: [1, 1, 2], count: 3)
        #expect(
            try await CasosUtilidades.rotarSignos(e, salida: s, nuevos: n, correccion: false, numeroPartidos: 14, entorno: entorno) == 2)
        #expect(String(decoding: try entorno.archivos.leer(s), as: UTF8.self) == "XXX\r\nXX2\r\n")
        // Corrección: un partido cada vez; «111» da X11, 1X1, 11X y «1X2» da XX2, 1X2.
        #expect(try await CasosUtilidades.rotarSignos(e, salida: s, nuevos: n, correccion: true, numeroPartidos: 14, entorno: entorno) == 5)
        #expect(CasosUtilidades.indiceSigno("2") == 2 && CasosUtilidades.indiceSigno("?") == 0)
    }

    @Test func parejas() async throws {
        // Fila 1: partidos 1 y 2; «11» nivel 1. Nivel 1 exige exactamente 1.
        var filas = Array(repeating: Array(repeating: 0, count: 11), count: 14)
        filas[0] = [1, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0]
        var limites = Array(repeating: (minimo: 0, maximo: 10), count: 7)
        limites[0] = (1, 1)
        let f = FiltroPorGrupos(k: 2, filas: filas, limites: limites)
        #expect(try f.valida("11X").valida && !(try f.valida("1X1").valida) && (try f.valida("11").recuento[0]) == 1)
        let e = try fichero("p.txt", "11111111111111\r\nX1111111111111\r\n11111111111111\r\n")
        let r = try await CasosUtilidades.filtrarPorGrupos(e, filtro: f, entorno: entorno)
        #expect(r.procesadas == 3 && r.validas == 2 && r.bits.cuenta == 1)
        let s = URL(filePath: "/tmp/rg/ps.txt")
        try CasosUtilidades.grabarPorGrupos(r.bits, en: s, entorno: entorno)
        #expect(String(decoding: try entorno.archivos.leer(s), as: UTF8.self) == "11111111111111\r\n")
        var mal = filas
        mal[0][2] = 8
        #expect(throws: ErrorDominio.self) { try FiltroPorGrupos(k: 2, filas: mal, limites: limites).valida("11") }
        await #expect(throws: ErrorDominio.self) {
            try await CasosUtilidades.filtrarPorGrupos(try fichero("c.txt", "11\r\n"), filtro: f, entorno: entorno)
        }
    }

    @Test func ficherosDeCondiciones() {
        var filas = Array(repeating: Array(repeating: "0", count: 11), count: 14)
        var lims = Array(repeating: ["0", "10"], count: 7)
        let texto =
            "1,2,3,4,5,6,7,1,2,3,4\ncorta\n" + Array(repeating: "0,0,0,0,0,0,0,0,0,0,0", count: 12).joined(separator: "\n") + "\n2,5\n"
        CasosUtilidades.aplicarCondicionesPares(Data(texto.utf8), filas: &filas, limites: &lims)
        #expect(filas[0][2] == "3" && filas[1][0] == "0" && lims[0] == ["2", "5"] && lims[1] == ["0", "10"])
        #expect(String(decoding: CasosUtilidades.datosCondicionesPares(filas: filas, limites: lims), as: UTF8.self).hasPrefix("1,2,3,"))

        var tf = Array(repeating: Array(repeating: "0", count: 30), count: 12)
        var tl = Array(repeating: [0.0, 270], count: 5)
        CasosUtilidades.aplicarCondicionesTrios(Data("1, 2 ,x\n3,4\n".utf8), filas: &tf, limites: &tl, cultura: .esES)
        #expect(tf[0][0] == "1" && tf[0][1] == "2" && tf[0][2] == "0" && tl[0] == [1, 2] && tl[1] == [3, 4] && tl[2] == [0, 0])
        let d = String(decoding: CasosUtilidades.datosCondicionesTrios(filas: tf, limites: [[1.9, 270]]), as: UTF8.self)
        #expect(d.hasSuffix("1,270\r\n"))
    }
}
