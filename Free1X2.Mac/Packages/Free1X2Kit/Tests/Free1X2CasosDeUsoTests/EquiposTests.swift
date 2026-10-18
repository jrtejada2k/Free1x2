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

    /// Cliente que no contesta nunca (hasta que lo cancelan).
    struct ClienteMudo: ClienteHTTP {
        func datos(para peticion: URLRequest) async throws -> (Data, HTTPURLResponse) {
            try await Task.sleep(for: .seconds(60))
            throw URLError(.timedOut)
        }
    }

    /// DT-83: al arrancar, la jornada si llega a tiempo; lenta o con error ⇒ `nil` sin usar la caché, y sin esperar de más.
    @Test func jornadaAlArrancar() async throws {
        let json = try Golden.bytes("Fixtures/online/real-es.json")
        let url = try #require(URL(string: "https://clubprogol.com/wp-json/clubprogol/v1/quiniela/es/actual"))
        let conRed = Entorno.pruebas(red: ClienteHTTPGrabado(respuestas: [url: .init(cuerpo: json)]))
        let ok = await CasosArchivo.jornadaAlArrancar(.es, servicio: CasosArchivo.servicioOnline(entorno: conRed))
        #expect(ok?.jornada == 20_260_621 && ok?.partidos.first?.local == "Estados Unidos")

        let cache = CacheJornada(archivos: conRed.archivos, carpeta: conRed.rutas.base, registro: conRed.registro)  // ya guardada
        let lento = ServicioQuiniela(red: ClienteMudo(), reloj: conRed.reloj, cache: cache)
        let inicio = ContinuousClock.now
        let nada = await CasosArchivo.jornadaAlArrancar(.es, servicio: lento, limite: .milliseconds(200))
        #expect(nada == nil && ContinuousClock.now - inicio < .seconds(5))

        let error = Entorno.pruebas(red: ClienteHTTPGrabado(respuestas: [url: .init(estado: 500)]))
        #expect(await CasosArchivo.jornadaAlArrancar(.es, servicio: CasosArchivo.servicioOnline(entorno: error)) == nil)
    }

    /// DT-81: los nombres del boleto salen de la jornada real del backend; el 15º es el pleno (si lo hay).
    @Test func equiposDeJornadaReal() throws {
        func jornada(_ p: String) throws -> JornadaQuiniela {
            try JornadaQuiniela.analizar(String(decoding: try Golden.bytes("Fixtures/online/real-\(p).json"), as: UTF8.self))
        }
        let es = try jornada("es")
        #expect(CasosArchivo.equiposDeJornada(es, partidos: 14).count == 14)
        let es15 = CasosArchivo.equiposDeJornada(es, partidos: 15)
        #expect(es15.first == "Estados Unidos - Australia" && es15[13] == "Finalista 2 Playoff - Finalista 1 Playoff")
        #expect(es15.last == "España - Arabia Saudi" && es15.count == 15)
        let mx = CasosArchivo.equiposDeJornada(try jornada("mx"), partidos: 15)
        #expect(mx.count == 14 && mx.first == "Chequia - Mexico" && mx.last == "Argelia - Austria")
        #expect(CasosInicio.aplicarEquipos(mx, a: Array(repeating: "a - b", count: 15)).last == "a - b")
    }
}
