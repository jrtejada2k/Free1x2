import Foundation
import Free1X2CasosDeUso
import Free1X2Motor
import Free1X2Pruebas
import Testing

/// ESPEC_08 §5 y §15; ESPEC_10 §80.
@Suite("Formatos 123, saca formatos y premiadas")
struct FormatosPremiadasTests {
    @Test func formatos123() throws {
        let v = [[50.0, 30, 20], [20, 30, 50], [30, 50, 20]]
        #expect(try CasosCondiciones.columnaEnRangos("1X2", valoracion: v) == "123")
        #expect(try CasosCondiciones.columnaEnRangos("1", valoracion: [[.nan, 1, 1]]) == "")
        #expect(throws: ErrorDominio.self) { try CasosCondiciones.columnaEnRangos("1111", valoracion: v) }
        let a = try CasosCondiciones.analizarFormatos123(
            "1X2", valoracion: v, formatos: [Formato123(formato: "1"), Formato123(formato: "23"), Formato123(formato: "32")])
        #expect(a.map(\.apariciones) == [1, 1, 0])
        let t = try CasosCondiciones.todosFormatos123("1X2", valoracion: v)
        #expect(t.map(\.formato) == ["1", "2", "3", "12", "23", "123"] && t.allSatisfy { $0.apariciones == 1 })
    }

    @Test func sacaFormatos() {
        let i = CasosCondiciones.informeFormatos("1X1X")
        #expect(i.hasPrefix("* Pares que aparecen\r\n1X-2\r\nX1-1\r\nPares distintos = 2\r\n* Trios que aparecen\r\n1X1-1\r\nX1X-1\r\n"))
        #expect(i.hasSuffix("* Contactos\r\n1X-3\r\n12-0\r\nX2-0\r\n11-0\r\nXX-0\r\n22-0\r\n1V-3\r\nXV-0\r\n2V-0\r\nVV-0\r\n"))
    }

    @Test func premiadas() async throws {
        let entorno = Entorno.pruebas()
        let url = URL(filePath: "/tmp/pr/g.txt")
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data("11111111111111\r\ncorta\r\n1111111111111X\r\n".utf8), en: url, atomico: true)
        let r = try await CasosUtilidades.frecuenciasPremiadas(url, premio: 13, entorno: entorno)
        // 28 columnas a distancia 1 de cada ganadora; «11111111111112» lo está de las dos.
        #expect(r.procesadas == 2 && r.frecuencias.first?.hasSuffix("= 1 veces") == true)
        let total = r.frecuencias.enumerated().reduce(0) { $0 + Int($1.element.split(separator: " ")[0])! * ($1.offset + 1) }
        #expect(total == 56)
        let j = try await CasosUtilidades.jornadasDeFrecuencia(url, validas: r.validas, premio: 13, n: 2, entorno: entorno)
        #expect(j == ["sem.1", "sem.2"])
        let s = URL(filePath: "/tmp/pr/s.txt")
        try CasosUtilidades.grabarColumnasConFrecuencia(r.validas, n: 2, en: s, entorno: entorno)
        #expect(String(decoding: try entorno.archivos.leer(s), as: UTF8.self) == "11111111111112\r\n")
    }
}
