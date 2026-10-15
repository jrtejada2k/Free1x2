import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_10 §97 y §92 (CU-C33…C35); ESPEC_09 §41.
@Suite("Valoración de signos, diferencias y subir categoría")
struct SignosTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ texto: String) throws -> URL {
        let url = URL(filePath: "/tmp/sg/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(texto.utf8), en: url, atomico: true)
        return url
    }

    func texto(_ url: URL) throws -> String { String(decoding: try entorno.archivos.leer(url), as: UTF8.self) }

    @Test func vSignos() async throws {
        let f = try fichero("v.txt", "11111111111112\r\nx1111111111112\r\n11111111111111\r\n111\r\n")
        #expect(try CasosUtilidades.partidosVSignos(f, entorno: entorno) == 14)
        #expect(try CasosUtilidades.partidosVSignos(try fichero("c.txt", "111\r\n"), entorno: entorno) == 15)
        var g = Array(repeating: "-", count: 14)
        (g[0], g[13]) = ("1", "2")
        // Con pleno la 2.ª (12 aciertos) no mira el último partido.
        let r = try await CasosUtilidades.analizarSignos(f, ganadora: g, aspiracion: 13, pleno: true, entorno: entorno)
        #expect(r.procesadas == 3 && r.aceptadas == ["11111111111112", "11111111111111"])
        #expect(r.recuentos[0] == [2, 0, 0] && r.recuentos[13] == [1, 0, 1] && r.premios == [1, 1, 1, 0, 0])
        #expect(CasosUtilidades.textoPremiosRemanentes(r.premios) == "Premios remanentes: 1-1-1-0-0")
        let s = try await CasosUtilidades.analizarSignos(f, ganadora: g, aspiracion: 13, pleno: false, entorno: entorno)
        #expect(s.aceptadas.count == 3 && s.premios == [1, 2, 0, 0, 0] && s.recuentos[0] == [2, 1, 0])
        #expect(
            CasosUtilidades.valoracionDeSignos([[2, 0, 0], [1, 1, 1], [0, 0, 0]], decimales: false) == [
                "100, 0, 0", "33, 33, 33", "0, 0, 0",
            ])
        #expect(CasosUtilidades.valoracionDeSignos([[1, 0, 1]], decimales: true) == ["50.00, 0.00, 50.00"])
        #expect(CasosUtilidades.tiempoCorto(0) == "00:00:0000" && CasosUtilidades.tiempoCorto(1.5) == "00:00:01.5")
    }

    @Test func difColsMascaras() throws {
        #expect(try CasosUtilidades.mascaraDif("0-5,6,7-14") == Array(repeating: true, count: 15))
        #expect(try CasosUtilidades.mascaraDif("6-14") == Array(repeating: false, count: 6) + Array(repeating: true, count: 9))
        #expect(try CasosUtilidades.mascaraDif("5-3").count == 16, "a > b hace crecer la máscara")
        #expect(try CasosUtilidades.mascaraDif("3,20") == (0..<15).map { $0 == 3 || $0 == 14 })
        #expect(throws: ErrorDominio.self) { try CasosUtilidades.mascaraDif("1,") }
        #expect(throws: ErrorDominio.self) { try CasosUtilidades.mascaraDif("-3") }
        #expect(try CasosUtilidades.condicionDif("1X1212x2111121") == CasosUtilidades.condicionDif("14121242111121"))
        #expect(throws: ErrorDominio.self) { try CasosUtilidades.condicionDif("1X121") }
        #expect(throws: ErrorDominio.self) { try CasosUtilidades.condicionDif("1X12123X111121") }
    }

    @Test func difColsFichero() async throws {
        let base = [try CasosUtilidades.condicionDif("11111111111111")]
        let e = try fichero(
            "e.txt", "11111111111111\r\n1111111111111X\r\n111111111111XX\r\n\r\nFF1111111111XX\r\nFF11111111122X\r\nFF111111111122\r\n")
        let r = try await CasosUtilidades.diferenciasColumnas(
            condiciones: base, difBase: try CasosUtilidades.mascaraDif("0-2"), difResultado: try CasosUtilidades.mascaraDif("2-14"),
            entrada: e, entorno: entorno)
        #expect(r == ["11111111111111", "11111111111144", "FF111111111122"])
        let s = URL(filePath: "/tmp/sg/s.txt")
        try CasosUtilidades.grabarDiferencias(r ?? [], limite: 2, en: s, entorno: entorno)
        #expect(try texto(s) == "11111111111111\r\n111111111111XX\r\n")
        await #expect(throws: ErrorDominio.self) {
            try await CasosUtilidades.diferenciasColumnas(
                condiciones: base, difBase: [], difResultado: [], entrada: try fichero("m.txt", "123\r\n"), entorno: entorno)
        }
    }

    @Test func difColsTriples() async throws {
        let r = try await CasosUtilidades.diferenciasColumnas(
            condiciones: [try CasosUtilidades.condicionDif("11111111111111")], difBase: try CasosUtilidades.mascaraDif("0-1"),
            difResultado: try CasosUtilidades.mascaraDif("0-14"), entrada: nil, entorno: entorno)
        #expect(r?.count == 29 && r?.prefix(3) == ["11111111111111", "11111111111112", "11111111111114"])
        #expect(r?.last == "41111111111111")
    }

    @Test func subirCategoria() async throws {
        let f = try fichero("m.txt", "11111111111111\r\n11111111111111\r\n")
        let p = try await CasosUtilidades.prepararSubirCategoria(f, entorno: entorno)
        #expect(p.signos == 14 && p.motor.noColumnas == 2)
        let involucrados = (0..<16).map { $0 < 3 }
        let r = await CasosUtilidades.calcularSubirCategoria(
            p.motor, involucrados: involucrados, niveles: (0...16).map { $0 <= 2 }, seguidos: 1, externas: nil, signos: 14,
            entorno: entorno)
        #expect(r.error == nil && r.motor.noColumnas == 19)
        let s = URL(filePath: "/tmp/sg/sc.txt")
        try await CasosUtilidades.grabarSubirCategoria(r.motor, en: s, entorno: entorno)
        #expect(try texto(s).components(separatedBy: "\r\n").count == 20)
        let x = await CasosUtilidades.calcularSubirCategoria(
            p.motor, involucrados: involucrados, niveles: Array(repeating: false, count: 17), seguidos: 0, externas: nil,
            signos: 14, entorno: entorno)
        #expect(x.error != nil)
    }
}
