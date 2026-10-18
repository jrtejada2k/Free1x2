import Free1X2Base
import Testing

@testable import Free1X2Motor

/// ESPEC_06 §3.5 casos (2) y ESPEC_01 G-AN-3: piezas del contenedor de análisis, por lectura.
@Suite("Contenedor de análisis")
struct ContenedorAnalisisTests {
    /// `IncrementarColumnasValoracion`: `double → float → Math.Round` a par (T3, T4).
    @Test func valoracionRedondeaAParTrasPasarPorFloat() throws {
        var c = ContenedorAnalisis(numeroPartidos: 14)
        c.noValoresValoracionGlobal = [4]
        c.noValoresValoracionUnos = [4]
        c.noValoresValoracionEquis = [4]
        c.noValoresValoracionDoses = [4]
        try c.inicializarValoraciones()
        for v in [0.5, 1.5, 2.5, 3.5, -0.5, 2.4999999999, Double(Float(2.5000002))] {
            try c.incrementarColumnasValoracion(v, v, v, v)
        }
        // 0.5→0, 1.5→2, 2.5→2, 3.5→4, −0.5→0, 2.4999999999→2.5f→2, 2.5000002f→3.
        #expect(c.valoracionGlobal == [2, 0, 3, 1, 1])
        let e = #expect(throws: ErrorDominio.self) { try c.incrementarColumnasValoracion(-0.6, 0, 0, 0) }
        #expect(e?.excepcion == .argumentoFueraDeRango)
    }

    @Test func inicializarValoraciones() throws {
        var c = ContenedorAnalisis(numeroPartidos: 14)
        let e = #expect(throws: ErrorDominio.self) { try c.inicializarValoraciones() }
        #expect(e?.excepcion == .argumentoFueraDeRango, "lista vacía: lista[Count-1]")
        c.noValoresValoracionGlobal = [5, 2]
        c.noValoresValoracionUnos = [1]
        c.noValoresValoracionEquis = [1]
        c.noValoresValoracionDoses = [1]
        try c.inicializarValoraciones()
        try c.inicializarValoraciones()
        #expect(c.valoracionGlobal.count == 6, "el último (2) + 1, dos veces: duplica")
    }

    @Test func simetriasYFormatos() {
        var c = ContenedorAnalisis(numeroPartidos: 14)
        c.noAciertosSimetrias = [3, 3]
        c.inicializarSimetrias()
        c.incrementarColumnasSimetrias(3)
        #expect(c.simetrias.map(\.columnas) == [1, 0], "solo el primero con esos aciertos")
        c.inicializaFormatosSignos([2])
        var f = c.formatosSignos[0].aciertosGlobalesFormatos
        for k in [0, 5, 5, 2] { f.incrementar(k) }
        #expect(ContenedorAnalisis.ordenadas(f).map { [$0.clave, $0.valor] } == [[0, 1], [2, 1], [5, 2]])
        #expect(c.formatosSignos[0].aciertosFormatosSignos.count == 3)
    }

    /// G-AN-3: trozos por `#`, rangos con `Math.Round(Convert.ToDouble)` en la cultura actual, ordenada con repetidos.
    @Test func obtenerArrayAciertos() throws {
        #expect(try Analizador.obtenerArrayAciertos("1-3#5#2-2#3", .esES) == [1, 2, 2, 3, 3, 5])
        #expect(try Analizador.obtenerArrayAciertos("", .esES) == [])
        #expect(try Analizador.obtenerArrayAciertos("2.5-4", .enUS) == [2, 3, 4], "2.5 → 2 (a par)")
        #expect(try Analizador.obtenerArrayAciertos("2.5-4", .esES) == [], "es-ES: «2.5» = 25 ⇒ rango vacío")
    }
}
