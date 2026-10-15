import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_10 §102 (CU-C53, CU-C54).
@Suite("Visor de análisis")
struct VisorAnalisisTests {
    @Test func secciones() async throws {
        var c = Combinacion(numeroPartidos: 14)
        c.ponerPronosticos((0..<14).map { $0 < 2 ? "1,X,2" : "1" })
        let r = try await CasosCombinacion.calcularCombinacion(
            c, modo: .analizar, configuracion: .porDefecto, entorno: Entorno.pruebas())
        let g = try #require(r.analisis)

        let nada = CasosVer.seccionesDeAnalisis(nil, opciones: OpcionesAnalisis(), numeroPartidos: 14)
        #expect(nada.map(\.clave) == ["SinAnalisis"] && nada[0].tipo == .vacia)
        #expect(nada[0].detalle == "No se ha recibido ningún análisis. Ejecuta un análisis para ver los datos.")
        #expect(
            CasosVer.seccionesDeAnalisis(g, opciones: OpcionesAnalisis(), numeroPartidos: 14)[0].detalle
                == "No hay ningún tipo de análisis activo (VariablesGlobales.Analizar*).")

        var o = OpcionesAnalisis()
        (o.vx2, o.controlGrupos, o.valoracion, o.diferencias) = (true, true, true, true)
        let s = CasosVer.seccionesDeAnalisis(g, opciones: o, numeroPartidos: 14)
        #expect(s.map(\.clave) == ["VX2", "ControlGrupos", "Diferencias", "Valoracion"])
        let vx2 = s[0].bloques[0].matriz.filas
        #expect(s[0].detalle == "VX2 — Variantes, Equis, Doses: 3 conceptos × 15 valores (cada celda = nº de columnas).")
        #expect(vx2.count == 4 && vx2[0].esCabecera && vx2[0].celdas.last == "14" && vx2[1].etiqueta == "Variantes")
        #expect(vx2[1].celdas.count == 15 && vx2[3].etiqueta == "Doses")
        #expect(s[1].bloques[0].matriz.filas.map(\.etiqueta) == ["Fallos", "Columnas"] && s[1].detalle.hasSuffix("(1 valores)."))
        #expect(s[2].tipo == .vacia && s[2].detalle == "Diferencias: No hay datos para analizar")
        #expect(s[3].tipo == .vacia && s[3].detalle == "Valoración: No hay datos para analizar")
        #expect(CasosVer.seleccionTrasRefrescar("Valoracion", en: s) == "Valoracion")
        #expect(CasosVer.seleccionTrasRefrescar("CPs", en: s) == "VX2")
    }
}
