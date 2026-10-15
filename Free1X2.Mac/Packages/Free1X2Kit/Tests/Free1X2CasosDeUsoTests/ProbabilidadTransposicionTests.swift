import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_10 §81 (CU-C17) y §95 (CU-C40).
@Suite("Probabilidades y transposición")
struct ProbabilidadTransposicionTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ texto: String) throws -> URL {
        let url = URL(filePath: "/tmp/pt/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(texto.utf8), en: url, atomico: true)
        return url
    }

    @Test func probabilidades() async throws {
        let m = String(repeating: "1", count: 14)
        let madre = try fichero("m.txt", m + "\r\n")
        let hija = try fichero("h.txt", "11X11111111111\r\n11X1X111111111\r\n")
        let r = try await CasosCombinacion.probabilidadDePremios(madre: madre, hija: hija, rango: "", entorno: entorno)
        #expect(!r.errorSignos && r.filas.map(\.categoria) == (10...14).map { "Premio de \($0)" })
        #expect(r.filas.map(\.probabilidad) == ["0,00 %", "0,00 %", "0,00 %", "100,00 %", "0,00 %"])
        let uno = try await CasosCombinacion.probabilidadDePremios(madre: madre, hija: hija, rango: "13", entorno: entorno)
        #expect(uno.filas.map(\.categoria) == ["Premio de 13"] && uno.filas.map(\.probabilidad) == ["100,00 %"])
        let corta = try fichero("c.txt", "111\r\n")
        let e = try await CasosCombinacion.probabilidadDePremios(madre: madre, hija: corta, rango: "14-10", entorno: entorno)
        #expect(e.errorSignos && e.filas.count == 5 && e.filas[0].probabilidad == "NaN %")
        await #expect(throws: ErrorDominio.self) {
            try await CasosCombinacion.probabilidadDePremios(madre: madre, hija: hija, rango: "20", entorno: entorno)
        }
    }

    @Test func transposicion() async throws {
        #expect(CasosUtilidades.ordenTransposicion((1...14).map(Double.init)) == Array(0..<14))
        #expect(CasosUtilidades.ordenTransposicion([2, 2] + (3...14).map(Double.init)) == nil)
        #expect(CasosUtilidades.ordenTransposicion([.nan] + (2...14).map(Double.init)) == nil)
        let orden = Array((0..<14).reversed())
        let entrada = try fichero("e.txt", "1X21X21X21X21XEXTRA\n111\n22222222222222\n")
        let salida = URL(filePath: "/tmp/pt/s.txt")
        await #expect(throws: ErrorDominio.self) {
            try await CasosUtilidades.transponerColumnas(entrada, salida: salida, orden: orden, entorno: entorno)
        }
        #expect(String(decoding: try entorno.archivos.leer(salida), as: UTF8.self) == "X12X12X12X12X1\r\n")
        let vacia = try fichero("v.txt", "")
        try await CasosUtilidades.transponerColumnas(vacia, salida: salida, orden: orden, entorno: entorno)
        #expect(try entorno.archivos.leer(salida).isEmpty)
    }
}
