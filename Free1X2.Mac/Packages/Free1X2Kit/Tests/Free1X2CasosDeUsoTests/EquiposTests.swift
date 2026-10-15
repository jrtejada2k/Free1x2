import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_09 §60 y ESPEC_08 §2.
@Suite("Gestión de equipos")
struct EquiposTests {
    let entorno = Entorno.pruebas()

    @Test func editorYAlta() throws {
        var e = EditorEquipos.cargar(entorno: entorno)
        #expect(e.estado.hasPrefix("No se pudo cargar equiposInt.dat: "))
        #expect(e.nuevo("Athletic", en: .primera) && e.lista(.primera) == ["Athletic"])
        #expect(e.nuevo(" athletic ", en: .primera) && e.estado == "El equipo \" athletic \" ya existe en 1ª.")
        #expect(!e.nuevo("  ", en: .primera) && e.estado == "Introduce un nombre de equipo.")
        e.mover("Athletic", de: .primera, a: .segunda)
        #expect(e.lista(.primera).isEmpty && e.lista(.segunda) == ["Athletic"] && e.estado == "Movido \"Athletic\".")
        e.mover(nil, de: .segunda, a: .primera)
        #expect(e.estado == "Selecciona un equipo.")
        e.importar(["ATHLETIC", "Cádiz"], en: .segunda)
        #expect(e.lista(.segunda) == ["Athletic", "Cádiz"] && e.estado.hasPrefix("1 equipos importados (online) a 2ª."))
        e.errorImportacion(ErrorOnline("Sin conexión."))
        #expect(e.estado == "Sin conexión. Los equipos no se modificaron.")
        try e.guardar(entorno: entorno)
        #expect(EditorEquipos.cargar(entorno: entorno).lista(.segunda) == ["Athletic", "Cádiz"] && e.estado == "Equipos guardados.")

        #expect(try CasosArchivo.anadirEquipo("Osasuna", categoria: .segundaB, entorno: entorno))
        #expect(try !CasosArchivo.anadirEquipo("Osasuna", categoria: .segundaB, entorno: entorno))
        #expect(try CasosArchivo.anadirEquipo("osasuna", categoria: .segundaB, entorno: entorno))
    }

    @Test func descargaDeJornada() async throws {
        let json = """
            {"pais":"ES","temporada":"2025/26","jornada":38,"fecha":"2026-06-21","partidos":[\((1...14).map { "{\"n\":\($0),\"local\":\"L\($0)\",\"visitante\":\"V\($0)\"}" }.joined(separator: ","))]}
            """
        let url = try #require(URL(string: "https://clubprogol.com/wp-json/clubprogol/v1/quiniela/es/actual"))
        let conRed = Entorno.pruebas(red: ClienteHTTPGrabado(respuestas: [url: .init(cuerpo: Data(json.utf8))]))
        let ok = await CasosArchivo.descargarJornada(.es, servicio: CasosArchivo.servicioOnline(entorno: conRed), entorno: conRed)
        #expect(ok.jornada?.jornada == 38 && ok.mensaje.hasPrefix("Jornada 38 · 14 partidos cargados · Última actualización: "))
        #expect(CasosArchivo.paisInicial(entorno: conRed) == .es)

        let sinRed = Entorno.pruebas(archivos: conRed.archivos)
        let cacheada = await CasosArchivo.descargarJornada(.es, servicio: CasosArchivo.servicioOnline(entorno: sinRed), entorno: sinRed)
        #expect(cacheada.jornada?.jornada == 38 && cacheada.mensaje.hasPrefix("Sin conexión: mostrando la jornada guardada (actualizada "))
        let nada = await CasosArchivo.descargarJornada(.mx, servicio: CasosArchivo.servicioOnline(entorno: sinRed), entorno: sinRed)
        #expect(nada.jornada == nil && nada.mensaje.hasSuffix(" El boleto sigue en modo manual."))
    }
}
