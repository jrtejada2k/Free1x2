import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_08 §7 y §18 (CU-A12, CU-A13).
@Suite("Análisis de fallos")
struct AnalisisFallosTests {
    let entorno = Entorno.pruebas()

    @Test func arbol() throws {
        #expect(try AnalisisFallos.evaluarPronosticos(UtilColumnas.convStrToLong("1X2"), ["1", "1", "2"]) == [2])
        #expect(try AnalisisFallos.evaluarPronosticos(UtilColumnas.convStrToLong("2X1"), ["1", "1", "2"]) == [3, 2, 1])
        #expect(throws: ErrorDominio(.general, "La columna ganadora debe tener 14 signos.")) {
            try CasosCombinacion.columnaGanadoraParaAnalisis("1X2", numeroPartidos: 14)
        }
        #expect(throws: ErrorDominio(.general, "La columna ganadora debe tener 14 signos.")) {
            try CasosCombinacion.columnaGanadoraParaAnalisis("1X21X21X21X21A", numeroPartidos: 14)
        }

        var c = Combinacion(numeroPartidos: 14)
        c.ponerPronosticos((0..<14).map { $0 == 0 ? "1" : "1,X,2" })
        try c.modificarGrupo(0) { g throws(ErrorDominio) in
            try CasosCondiciones.aplicarNoVariantes(variantes: "0,1,2", equis: "", doses: "", en: &g.condiciones.noVariantes)
        }
        let col = try CasosCombinacion.columnaGanadoraParaAnalisis("X1111111111112", numeroPartidos: 14)
        let e = EntradaAnalisisFallos(
            nombre: "/x/q.comb", columna: col, combinacion: c, pronosticos: ["1"] + Array(repeating: "1X2", count: 13))
        let (titulo, nodos) = try CasosCombinacion.analizarFallosCombinacion(e, configuracion: .porDefecto, entorno: entorno)
        #expect(titulo == "Análisis de fallos: q.comb" && nodos.count == 1 && nodos[0].estado == .fallada)
        let base = try #require(nodos[0].hijos?.first)
        #expect(base.titulo == "Boleto base" && base.hijos?.map(\.titulo) == ["Pronóstico de partidos", "Variantes, X y 2"])
        #expect(base.hijos?[0].hijos?.first?.titulo == "Partido 1  (1)")

        let actual = CasosCombinacion.analisisDeCombinacionActual(Combinacion(numeroPartidos: 14), ruta: nil, numeroPartidos: 14)
        #expect(actual.nombre == "Combinación en pantalla" && actual.pronosticos.allSatisfy { $0 == "1X2" })
    }
}
