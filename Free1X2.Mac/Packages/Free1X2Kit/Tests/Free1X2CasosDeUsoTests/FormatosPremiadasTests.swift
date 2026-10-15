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

/// ESPEC_10 §79 (CU-C11…C13), §104 (CU-C57) y ESPEC_09 §72 (CU-B40).
@Suite("Posibles premios y mejores opciones")
struct PosiblesPremiosTests {
    let cols = ["11111111111111", "X1111111111111"]
    let unos = String(repeating: "1", count: 13)

    @Test func posibles() async throws {
        let p = CasosCombinacion.ganadoraParcial(Array(repeating: "1", count: 13) + ["", "x", ""], partidos: 14)
        #expect(p.ganadora == unos + "*X" && p.noDefinitivos == 1 && p.signos[13] == "*" && p.signos[15] == "")
        let r = await CasosCombinacion.posiblesPremios(cols, ganadora: unos + "*", partidos: 14, pleno: false, generarResumen: true)
        #expect(r.categorias[2] == [cols[0] + "14"] && r.categorias[3] == [cols[1] + "13"] && r.premiadas.count == 2)
        #expect(r.resumen.map(\.colGanadora) == [unos + "1", unos + "X", unos + "2"])
        let o = CasosCombinacion.textosOptando(r.categorias, pleno: false)
        #expect(o[2] == "Optando a 14: 1" && o[3] == "Optando a 13: 1")
        let c = CasosCombinacion.textoCopiarPosibles(o, noDefinitivos: 1, partidos: 14)
        #expect(c.hasPrefix("Posibles premios a falta de 1 partidos:\r\nOptando a 14: 1\r\n") && c.hasSuffix("Optando a 10: 0"))
        let g = CasosCombinacion.textoResumenPosibles(Array(r.resumen.prefix(1)), noDefinitivos: 1)
        #expect(g.contains("Columna Ganadora: " + unos + "1\r\n") && g.contains(cols[0] + " 14 aciertos\r\n") && g.hasSuffix(" \r\n"))
        let v = CasosCombinacion.grupoVisor(r.resumen[0])
        #expect(v.ganadora.count == 14 && v.columnas.map(\.categoria) == [14, 13] && v.columnas[0].aciertosTexto == "14")
        #expect(v.columnas[1].aciertos.first == false && v.columnas[1].signos.count == 14)
    }

    @Test func mejores() async throws {
        let inv = Array(repeating: false, count: 13) + [true, false, false]
        #expect(CasosCombinacion.ganadorasPosibles(unos + "*", involucrados: inv).count == 3)
        #expect(CasosCombinacion.ganadorasPosibles("*1", involucrados: [false, false]) == ["*1"])
        let t = try await CasosCombinacion.mejoresOpciones(ganadora: unos + "*", columnas: cols, pleno: false, involucrados: inv, limite: 2)
        #expect(
            t == "Mis mejores opciones son: \r\n------------------------\r\n\r\n" + unos
                + "1: 0 de 14 + 1 de 13 + 1 de 12 + 0 de 11 + 0 de 10\r\n"
                + unos + "X: 0 de 14 + 1 de 13 + 1 de 12 + 0 de 11 + 0 de 10\r\n")
        let p = try await CasosCombinacion.mejoresOpciones(ganadora: unos + "1", columnas: cols, pleno: true, involucrados: inv, limite: 1)
        #expect(p.contains(": 2 de 14 + 0 de 13 + 1 de 12"), "pleno: 14 de 14 cuenta dos veces")
    }
}
