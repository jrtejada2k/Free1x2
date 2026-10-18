import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_09 §71 (CU-B39).
@Suite("Listado de condiciones")
struct ListadoTests {
    @Test func arbolYExportacion() throws {
        var c = Combinacion(numeroPartidos: 14)
        c.ponerPronosticos(Array(repeating: "1X", count: 14))
        try c.modificarGrupo(0) { (g: inout GrupoConfig) throws(ErrorDominio) in
            g.condiciones.noVariantes.activa = true
            try g.condiciones.noVariantes.setNoVariantes("3,4")
            g.condiciones.dibujos.activa = true
            try g.condiciones.dibujos.ponerDibujos(["1+2"])
        }
        let r = CasosVer.listadoCondiciones(c, archivoFiltro: "", cultura: .esES)
        let h = try #require(r.hijos)
        #expect(h.map(\.texto) == ["Pronóstico Base", "Filtro de Columnas", "Grupos", "Control de grupos", "IfThen"])
        #expect(h[0].hijos?.first?.texto == "Partido 1: 1X" && h[1].hijos?.first?.texto == "No se usa ningún Filtro")
        let condiciones = try #require(h[2].hijos?.first?.hijos?.last)
        #expect(condiciones.texto == "Condiciones" && condiciones.hijos?.map(\.texto) == ["NoVariantes", "Dibujos"])
        #expect(condiciones.hijos?.first?.hijos?.first?.texto == "Cantidad de V: 3,4")
        #expect(condiciones.hijos?.last?.hijos?.first?.texto == "Dibujos : 1+2")
        #expect(h[3].hijos?.map(\.texto) == ["Control Conjuntos"] && h[3].hijos?.first?.hijos?.first?.texto == "Conjuntos: 0")
        #expect(h[4].hijos?.first?.texto == "IfThen no activado")
        let texto = CasosVer.listadoTexto(r)
        #expect(texto.hasPrefix("Pronóstico Base\r\n\r\n Partido 1: 1X\r\n Partido 2: 1X\r\n"))
        #expect(texto.contains(" Partido 14: 1X\r\n\r\nFiltro de Columnas\r\n\r\n No se usa ningún Filtro\r\n\r\n"))
        #expect(texto.hasSuffix("IfThen\r\n\r\n IfThen no activado\r\n\r\n"))
        let html = CasosVer.listadoHtml(r)
        #expect(
            html.hasPrefix(
                "<html><head><title>Listado de Condiciones</title></head><body bgcolor=\"#DBFEBC\"><br><b>Pronóstico Base</b><br><br>Partido 1: 1X<br>"
            ))
        #expect(html.contains("<i>Boleto Base</i><br>Partidos Activos: "))
    }
}
