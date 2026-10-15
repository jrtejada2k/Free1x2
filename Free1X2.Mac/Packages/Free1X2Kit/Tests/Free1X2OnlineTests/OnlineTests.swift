import Foundation
import Free1X2Base
import Free1X2Pruebas
import Testing
import os

@testable import Free1X2Online

/// JSON genérico para leer las salidas golden de forma libre.
enum J: Decodable, Sendable {
    case o([String: J]), a([J]), s(String), n(Double), b(Bool), nulo

    init(from d: any Decoder) throws {
        let c = try d.singleValueContainer()
        if c.decodeNil() {
            self = .nulo
        } else if let v = try? c.decode(Bool.self) {
            self = .b(v)
        } else if let v = try? c.decode(Double.self) {
            self = .n(v)
        } else if let v = try? c.decode(String.self) {
            self = .s(v)
        } else if let v = try? c.decode([J].self) {
            self = .a(v)
        } else {
            self = .o(try c.decode([String: J].self))
        }
    }

    subscript(_ k: String) -> J? { if case .o(let m) = self { m[k] } else { nil } }
    var s: String? { if case .s(let v) = self { v } else { nil } }
    var n: Int? { if case .n(let v) = self { Int(v) } else { nil } }
    var a: [J] { if case .a(let v) = self { v } else { [] } }
    var esNulo: Bool { if case .nulo = self { true } else { false } }
}

/// 07 §5.4-5.5 (suite `online/parsers`, casos de `JornadaQuinielaParserTests`, `CatalogoEquiposParserTests` y
/// `JornadaCacheRoundTripTests`).
@Suite("Online: parsers y caché")
struct OnlineParsersTests {
    struct Entrada: Decodable, Sendable {
        var fichero: String?
        var json: String?
        var contenido: String?
        var pais: String?
    }

    static func texto(_ e: Entrada) throws -> String {
        if let f = e.fichero { return String(decoding: try Golden.bytes(f), as: UTF8.self) }
        return e.json ?? e.contenido ?? ""
    }

    static func comprobar(_ j: JornadaQuiniela, _ s: J, _ id: String) {
        if let v = s["pais"]?.s { #expect(j.pais == v, "\(id)") }
        if let v = s["temporada"]?.s { #expect(j.temporada == v, "\(id)") }
        if let v = s["jornada"]?.n { #expect(Int(j.jornada) == v, "\(id)") }
        if let v = s["numeracion"] { #expect(j.partidos.map { Int($0.n) } == v.a.compactMap(\.n), "\(id)") }
        if case .o(let m)? = s["partidos"] {
            for (k, p) in m {
                let partido = j.partidos[(Int(k) ?? 1) - 1]
                if let l = p["local"]?.s { #expect(partido.local == l, "\(id) \(k)") }
                if let v = p["visitante"]?.s { #expect(partido.visitante == v, "\(id) \(k)") }
            }
        }
        if s["nombresNoVacios"] != nil {
            #expect(j.partidos.allSatisfy { !$0.local.isEmpty && !$0.visitante.isEmpty }, "\(id)")
        }
        if let p = s["pleno15"] {
            if p.esNulo {
                #expect(j.pleno15 == nil, "\(id)")
            } else {
                #expect(j.pleno15?.n == 15, "\(id)")
                if let l = p["local"]?.s { #expect(j.pleno15?.local == l, "\(id)") }
                if let v = p["visitante"]?.s { #expect(j.pleno15?.visitante == v, "\(id)") }
            }
        }
    }

    @Test func suite() throws {
        for caso in try Golden.casos("online/parsers", Entrada.self, J.self) {
            let texto = try Self.texto(caso.entrada)
            switch caso.componente {
            case "JornadaQuinielaParser":
                if caso.error != nil {
                    let e = #expect(throws: ErrorDominio.self, "\(caso.id)") { try JornadaQuiniela.analizar(texto) }
                    #expect(e?.excepcion.rawValue == caso.error, "\(caso.id)")
                } else {
                    Self.comprobar(try JornadaQuiniela.analizar(texto), try #require(caso.salida), caso.id)
                }
            case "CatalogoEquiposParser":
                if caso.error != nil {
                    let e = #expect(throws: ErrorDominio.self, "\(caso.id)") { try CatalogoEquipos.analizar(texto) }
                    #expect(e?.excepcion.rawValue == caso.error, "\(caso.id)")
                    continue
                }
                let c = try CatalogoEquipos.analizar(texto)
                let s = try #require(caso.salida)
                if let v = s["pais"]?.s { #expect(c.pais == v, "\(caso.id)") }
                for (i, d) in (s["divisiones"]?.a ?? []).enumerated() {
                    if let id = d["id"]?.s { #expect(c.divisiones[i].id == id, "\(caso.id)") }
                    if let n = d["nombre"]?.s { #expect(c.divisiones[i].nombre == n, "\(caso.id)") }
                }
                for e in s["planoContiene"]?.a ?? [] { #expect(c.equiposPlano.contains(e.s ?? ""), "\(caso.id)") }
                if let p = s["plano"] { #expect(c.equiposPlano == p.a.compactMap(\.s), "\(caso.id)") }
            default:
                let entorno = Entorno.pruebas()
                let cache = CacheJornada(archivos: entorno.archivos, carpeta: entorno.rutas.base, registro: entorno.registro)
                let pais = try #require(PaisQuiniela(rawValue: caso.entrada.pais ?? ""))
                cache.guardar(pais, texto)
                let s = try #require(caso.salida)
                if s["cargada"] != nil {
                    #expect(cache.cargar(pais) == nil, "\(caso.id)")
                } else {
                    let c = try #require(cache.cargar(pais), "\(caso.id)")
                    #expect(cache.ruta(pais).lastPathComponent == s["nombreFichero"]?.s, "\(caso.id)")
                    #expect(c.guardado == RelojManual.fechaInicial, "\(caso.id): mtime")
                    Self.comprobar(c.jornada, s, caso.id)
                }
            }
        }
    }

    /// 07 §5.4: reglas no cubiertas por los tests del C# (lectura de `JornadaQuinielaParser.cs`).
    @Test func reglasJornada() throws {
        func partidos(_ n: Int, desde: Int = 1) -> String {
            (desde..<desde + n).map { "{\"n\":\($0),\"local\":\"L\($0)\",\"visitante\":\"V\($0)\"}" }.joined(separator: ",")
        }
        func json(_ extra: String = "", jornada: String = "1", lista: String? = nil) -> String {
            "{\"pais\":\"ES\",\"temporada\":\"T\",\"jornada\":\(jornada),\"partidos\":[\(lista ?? partidos(14))]\(extra)}"
        }
        func mensaje(_ s: String) -> String? {
            do {
                _ = try JornadaQuiniela.analizar(s)
                return nil
            } catch { return error.mensajeUsuario }
        }
        #expect(mensaje(json(jornada: "38.0")) == "El campo 'jornada' debe ser un número entero.")
        #expect(mensaje(json(jornada: "2147483648")) == "El campo 'jornada' debe ser un número entero.")
        #expect(mensaje(json(jornada: "null")) == "Falta el campo obligatorio 'jornada'.")
        #expect(mensaje(json(lista: partidos(15))) == "La jornada debe tener exactamente 14 partidos; se recibieron 15.")
        #expect(
            mensaje(json(lista: partidos(1) + "," + partidos(1, desde: 3)))
                == "Los partidos deben venir en orden: se esperaba n=2 y se recibió n=3.")
        #expect(mensaje(json(lista: "1")) == "El partido nº 1 no es un objeto JSON.")
        #expect(mensaje("[1]") == "La respuesta de la jornada no es un objeto JSON.")
        #expect(mensaje("  ") == "La respuesta de la jornada está vacía.")
        #expect(mensaje("{\"a\":1,}")?.hasPrefix("La respuesta de la jornada no es un JSON válido: ") == true)
        let j = try JornadaQuiniela.analizar(json(",\"pleno15\":7,\"fecha\":null,\"pais\":\"MX\""))
        #expect(j.pleno15 == nil && j.fecha == "" && j.pais == "MX", "pleno no objeto se ignora; duplicado: gana el último")
        #expect(
            mensaje(json(",\"pleno15\":{\"n\":15,\"local\":\" \",\"visitante\":\"V\"}"))
                == "El equipo local del partido nº 15 está vacío.")
    }

    @Test func equiposPlano() throws {
        let c = try CatalogoEquipos.analizar(
            "{\"pais\":\"ES\",\"divisiones\":[{\"id\":\"a\",\"equipos\":[\"Straße\",\" STRASSE \",\"straße\",\"Éibar\",\"ÉIBAR\"]}]}")
        #expect(c.equiposPlano == ["Straße", "STRASSE", "Éibar"], "OrdinalIgnoreCase sin plegado completo")
        #expect(c.divisiones[0].nombre == "")
    }
}

/// Red con respuestas en secuencia (o un error de transporte) que anota las URL pedidas.
final class RedSecuencial: ClienteHTTP {
    enum Paso: Sendable {
        case respuesta(Int, [String: String], String)
        case fallo(URLError.Code)
    }

    private let estado: OSAllocatedUnfairLock<(pasos: [Paso], urls: [String])>

    init(_ pasos: [Paso]) { estado = OSAllocatedUnfairLock(initialState: (pasos, [])) }

    var urls: [String] { estado.withLock { $0.urls } }

    func datos(para peticion: URLRequest) async throws -> (Data, HTTPURLResponse) {
        let url = try #require(peticion.url)
        let paso = estado.withLock { e -> Paso? in
            e.urls.append(url.absoluteString)
            return e.pasos.isEmpty ? nil : e.pasos.removeFirst()
        }
        switch paso {
        case .respuesta(let c, let h, let cuerpo)?:
            let r = try #require(HTTPURLResponse(url: url, statusCode: c, httpVersion: "HTTP/1.1", headerFields: h))
            return (Data(cuerpo.utf8), r)
        case .fallo(let codigo)?: throw URLError(codigo)
        case nil: throw URLError(.notConnectedToInternet)
        }
    }
}

/// 07 §5.1-5.3 (lectura de `QuinielaOnlineService.cs`).
@Suite("Online: servicio")
struct ServicioQuinielaTests {
    static func servicio(_ red: RedSecuencial, reloj: RelojManual = RelojManual(), base: String? = nil) -> (
        ServicioQuiniela, CacheJornada
    ) {
        let e = Entorno.pruebas(reloj: reloj)
        let cache = CacheJornada(archivos: e.archivos, carpeta: e.rutas.base, registro: e.registro)
        return (ServicioQuiniela(red: red, reloj: reloj, cache: cache, base: base, esperar: { _ in }), cache)
    }

    static func jornadaES() throws -> String {
        String(decoding: try Golden.bytes("Fixtures/online/quiniela-es-actual.json"), as: UTF8.self)
    }

    static func mensaje(_ s: ServicioQuiniela, _ pais: String = "es") async -> String? {
        do {
            _ = try await s.obtenerJornada(pais: pais)
            return nil
        } catch let e as ErrorOnline {
            return e.mensaje
        } catch {
            return "otro: \(error)"
        }
    }

    @Test func guardaAntiDobleYCache() async throws {
        let reloj = RelojManual()
        let red = RedSecuencial([.respuesta(200, [:], "\u{FEFF}" + (try Self.jornadaES())), .respuesta(200, [:], try Self.jornadaES())])
        let (s, cache) = Self.servicio(red, reloj: reloj, base: " http://localhost:8080/ ")
        let r1 = try await s.obtenerJornada(pais: " ES ")
        #expect(!r1.desdeCache && r1.jornada.jornada == 38 && r1.guardado == RelojManual.fechaInicial)
        #expect(red.urls == ["http://localhost:8080/wp-json/clubprogol/v1/quiniela/es/actual"])
        #expect(cache.paisMasReciente() == .es)
        reloj.avanzar(.seconds(30))
        let r2 = try await s.obtenerJornada(pais: "es")
        #expect(r2.desdeCache && red.urls.count == 1, "< 60 s y con caché: sin red")
        reloj.avanzar(.seconds(31))
        _ = try await s.obtenerJornada(pais: "es")
        #expect(red.urls.count == 2)
    }

    @Test func errores() async throws {
        let cuerpo = "{\"error\":\"sin_jornada\",\"mensaje\":\"No hay jornada\"}"
        #expect(await Self.mensaje(Self.servicio(RedSecuencial([.respuesta(404, [:], cuerpo)])).0) == "No hay jornada")
        #expect(
            await Self.mensaje(Self.servicio(RedSecuencial([.respuesta(404, [:], "{\"error\":\"sin_jornada\",\"mensaje\":\" \"}")])).0)
                == "sin_jornada")
        #expect(
            await Self.mensaje(Self.servicio(RedSecuencial([.respuesta(503, [:], "<html>")])).0)
                == "El servicio respondió con código 503 (ServiceUnavailable).")
        #expect(
            await Self.mensaje(Self.servicio(RedSecuencial([.respuesta(418, [:], "")])).0)
                == "El servicio respondió con código 418 (418).")
        #expect(
            await Self.mensaje(Self.servicio(RedSecuencial([.fallo(.timedOut)])).0)
                == "Se agotó el tiempo de espera al contactar con el servicio online.")
        #expect(
            await Self.mensaje(Self.servicio(RedSecuencial([.fallo(.cannotFindHost)])).0)
                == "No se pudo contactar con el servicio online (sin conexión o servidor no disponible).")
        #expect(await Self.mensaje(Self.servicio(RedSecuencial([])).0, " fr") == "País no soportado: ' fr' (use 'es' o 'mx').")
        let invalida = await Self.mensaje(Self.servicio(RedSecuencial([.respuesta(200, [:], "{}")])).0)
        #expect(invalida == "La respuesta del servicio no es una jornada válida: Falta el campo obligatorio 'pais'.")
    }

    /// 429: un solo reintento si `Retry-After` está entre 1 y 3 s.
    @Test func limiteDePeticiones() async throws {
        let ok = try Self.jornadaES()
        let red = RedSecuencial([.respuesta(429, ["Retry-After": "2"], ""), .respuesta(200, [:], ok)])
        #expect(try await Self.servicio(red).0.obtenerJornada(pais: "es").jornada.jornada == 38)
        #expect(red.urls.count == 2)
        let dos429 = RedSecuencial([.respuesta(429, ["Retry-After": "1"], ""), .respuesta(429, ["Retry-After": "1"], "")])
        #expect(
            await Self.mensaje(Self.servicio(dos429).0) == "Demasiadas solicitudes (límite 60/min). Intenta de nuevo en ~1 s.")
        #expect(
            await Self.mensaje(Self.servicio(RedSecuencial([.respuesta(429, ["Retry-After": "5"], "")])).0)
                == "Demasiadas solicitudes (límite 60/min). Intenta de nuevo en ~5 s.")
        #expect(
            await Self.mensaje(Self.servicio(RedSecuencial([.respuesta(429, [:], "")])).0)
                == "Demasiadas solicitudes (límite 60/min). Intenta de nuevo en ~60 s.")
        let ahora = RelojManual.fechaInicial
        #expect(ServicioQuiniela.retryAfter("Thu, 01 Jan 2026 12:00:10 GMT", ahora: ahora) == 10)
        #expect(ServicioQuiniela.retryAfter("Thu, 01 Jan 2026 11:00:00 GMT", ahora: ahora) == 1)
        #expect(ServicioQuiniela.retryAfter("-5", ahora: ahora) == -5)
        #expect(ServicioQuiniela.retryAfter("pronto", ahora: ahora) == nil)
    }

    @Test func catalogo() async throws {
        let cuerpo = String(decoding: try Golden.bytes("Fixtures/online/equipos-mx.json"), as: UTF8.self)
        let red = RedSecuencial([.respuesta(200, [:], cuerpo)])
        let c = try await Self.servicio(red).0.obtenerEquipos(pais: "MX")
        #expect(c.pais == "MX" && red.urls == ["https://clubprogol.com/wp-json/clubprogol/v1/equipos/mx"])
    }
}
