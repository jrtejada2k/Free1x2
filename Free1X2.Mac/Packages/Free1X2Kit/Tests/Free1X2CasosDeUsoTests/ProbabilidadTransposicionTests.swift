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

/// ESPEC_09 §63 (CU-B31) y §64 (CU-B32).
@Suite("Guardar valoración e histórico")
struct GuardarValoracionTests {
    @Test func guardarEHistorico() throws {
        let entorno = Entorno.pruebas()
        let v = Array(repeating: [45.5, 30, 24.5], count: 14)
        let url = URL(filePath: "/tmp/gv/v.txt")
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try CasosCondiciones.guardarValoracion(v, formato: 3, separador: ",", en: url, numeroPartidos: 14, entorno: entorno)
        #expect(String(decoding: try entorno.archivos.leer(url), as: UTF8.self).hasPrefix("45.5,30,24.5\r\n"))
        let h = URL(filePath: "/tmp/gv/h.txt")
        let r = try CasosCondiciones.guardarEnHistorico(v, temporada: 2004, jornada: 5, en: h, numeroPartidos: 14, entorno: entorno)
        #expect(r.temporada == "2004/2005" && r.jornada == "05")
        #expect(String(decoding: try entorno.archivos.leer(h), as: UTF8.self).hasPrefix("2004/2005\t05\t45,5\t30\t24,5\t"))
        #expect(throws: ErrorDominio.self) {
            try CasosCondiciones.guardarEnHistorico(v, temporada: 2004, jornada: 5, en: url, numeroPartidos: 14, entorno: entorno)
        }
    }
}

/// ESPEC_08 §3 (CU-A03).
@Suite("Álgebra de columnas")
struct AlgebraTests {
    @Test func algebra() async throws {
        let entorno = Entorno.pruebas()
        let a = URL(filePath: "/tmp/al/a.txt")
        let b = URL(filePath: "/tmp/al/b.txt")
        let s = URL(filePath: "/tmp/al/s.txt")
        try entorno.archivos.crearCarpeta(a.deletingLastPathComponent())
        try entorno.archivos.escribir(Data("1X2\r\n1X2\r\n111\r\n".utf8), en: a, atomico: true)
        try entorno.archivos.escribir(Data("111\r\n".utf8), en: b, atomico: true)
        #expect(try CasosUtilidades.resumenColumnas(a, entorno: entorno) == (3, 3))
        let m = try await CasosUtilidades.algebraDeColumnas(.eliminaRepetidas, a, b, salida: s, numeroPartidos: 3, entorno: entorno)
        #expect(m == "Nº columnas final: 2\nNº col. repetidas: 1")
        #expect(String(decoding: try entorno.archivos.leer(s), as: UTF8.self) == "1X2\r\n111\r\n")
        let vacia = URL(filePath: "/tmp/al/v.txt")
        _ = try await CasosUtilidades.algebraDeColumnas(.restaSegunda, b, b, salida: vacia, numeroPartidos: 3, entorno: entorno)
        #expect(!entorno.archivos.existe(vacia))
    }
}
