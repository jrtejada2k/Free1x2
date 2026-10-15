import Foundation
import Free1X2Base

// C#: Free1X2.WinUI/Services/JornadaCache.cs y QuinielaOnlineService.cs (07 §5.1-5.5, §6.10-6.11; DT-16, DT-17).

/// Caché offline-first: el cuerpo HTTP tal cual en `<base>/jornada-{es|mx}.json`; la fecha es el *mtime*.
public struct CacheJornada: Sendable {
    public let archivos: any SistemaArchivos
    public let carpeta: URL
    public let registro: any Registro

    public init(archivos: any SistemaArchivos, carpeta: URL, registro: any Registro) {
        (self.archivos, self.carpeta, self.registro) = (archivos, carpeta, registro)
    }

    public func ruta(_ pais: PaisQuiniela) -> URL { carpeta.appending(path: pais.ficheroCache) }

    /// `Guardar` (:66-89): ignora lo vacío; escritura atómica; nunca lanza.
    public func guardar(_ pais: PaisQuiniela, _ cuerpo: String) {
        guard !cuerpo.allSatisfy(\.isWhitespace) else { return }
        do throws(ErrorDominio) {
            try archivos.crearCarpeta(carpeta)
            try archivos.escribir(Data(cuerpo.utf8), en: ruta(pais), atomico: true)
        } catch {
            registro.registrar("JornadaCache.Guardar: \(error.mensajeUsuario)", nivel: .aviso)
        }
    }

    /// `FechaGuardado` (:98-111): *mtime* o `nil`; no lee el contenido.
    public func fechaGuardado(_ pais: PaisQuiniela) -> Date? {
        guard archivos.existe(ruta(pais)) else { return nil }
        return try? archivos.fechaModificacion(ruta(pais))
    }

    /// `TryCargar` (:119-146): cualquier fallo ⇒ `nil`.
    public func cargar(_ pais: PaisQuiniela) -> (jornada: JornadaQuiniela, guardado: Date)? {
        do throws(ErrorDominio) {
            let texto = String(decoding: try archivos.leer(ruta(pais)), as: UTF8.self)
            let j = try JornadaQuiniela.analizar(texto)
            return (j, try archivos.fechaModificacion(ruta(pais)))
        } catch {
            registro.registrar("JornadaCache.TryCargar(\(pais.rawValue)): \(error.mensajeUsuario)", nivel: .aviso)
            return nil
        }
    }

    /// `PaisMasReciente` (:153-180): el de *mtime* mayor (empate ⇒ `es`); no comprueba que sea válido.
    public func paisMasReciente() -> PaisQuiniela? {
        var mejor: (PaisQuiniela, Date)?
        for p in PaisQuiniela.allCases {
            guard let f = fechaGuardado(p) else { continue }
            if let m = mejor, f <= m.1 { continue }
            mejor = (p, f)
        }
        return mejor?.0
    }
}

/// `ObtenerJornadaAsync`: la jornada, si viene de la caché y cuándo se guardó.
public struct ResultadoJornada: Sendable {
    public let jornada: JornadaQuiniela
    public let desdeCache: Bool
    public let guardado: Date?
}

/// `QuinielaOnlineService` (07 §5.2-5.3).
public actor ServicioQuiniela {
    public static let basePorDefecto = "https://clubprogol.com"
    static let prefijo = "/wp-json/clubprogol/v1"
    static let retryAfterMaximo = 3
    static let ventanaAntiDoble: TimeInterval = 60

    let red: any ClienteHTTP
    let reloj: any Reloj
    let cache: CacheJornada
    let base: String
    let esperar: @Sendable (Int) async throws -> Void
    /// `ultimaDescargaUtc`: solo en memoria.
    var ultimaDescarga: [PaisQuiniela: Date] = [:]

    /// `base`: el `FREE1X2_API_BASE` de desarrollo si no está vacío (tras recortar); si no, el de producción.
    public init(
        red: any ClienteHTTP, reloj: any Reloj, cache: CacheJornada, base: String? = nil,
        esperar: @escaping @Sendable (Int) async throws -> Void = { try await Task.sleep(for: .seconds($0)) }
    ) {
        let b = base?.trimmingCharacters(in: .whitespacesAndNewlines) ?? ""
        var limpia = b.isEmpty ? Self.basePorDefecto : b
        while limpia.hasSuffix("/") { limpia.removeLast() }
        (self.red, self.reloj, self.cache, self.base, self.esperar) = (red, reloj, cache, limpia, esperar)
    }

    /// `Normalizar` (:132-136): `Trim().ToLowerInvariant()`; el mensaje cita el valor original.
    static func pais(_ texto: String) throws(ErrorOnline) -> PaisQuiniela {
        guard let p = PaisQuiniela(rawValue: texto.trimmingCharacters(in: .whitespacesAndNewlines).lowercased()) else {
            throw ErrorOnline("País no soportado: '\(texto)' (use 'es' o 'mx').")
        }
        return p
    }

    /// `ObtenerJornadaAsync` (:130-192): guarda anti-doble de 60 s con caché; tras éxito guarda el cuerpo.
    public func obtenerJornada(pais texto: String) async throws -> ResultadoJornada {
        let p = try Self.pais(texto)
        if let u = ultimaDescarga[p], reloj.ahora().timeIntervalSince(u) < Self.ventanaAntiDoble,
            cache.fechaGuardado(p) != nil, let c = cache.cargar(p)
        {
            return ResultadoJornada(jornada: c.jornada, desdeCache: true, guardado: c.guardado)
        }
        let cuerpo = try await descargar("\(base)\(Self.prefijo)/quiniela/\(p.rawValue)/actual")
        let jornada: JornadaQuiniela
        do throws(ErrorDominio) {
            jornada = try JornadaQuiniela.analizar(cuerpo)
        } catch {
            throw ErrorOnline("La respuesta del servicio no es una jornada válida: " + error.mensajeUsuario)
        }
        cache.guardar(p, cuerpo)
        ultimaDescarga[p] = reloj.ahora()
        return ResultadoJornada(jornada: jornada, desdeCache: false, guardado: cache.fechaGuardado(p))
    }

    /// `ObtenerEquiposAsync` (:205-227): sin guarda ni caché.
    public func obtenerEquipos(pais texto: String) async throws -> CatalogoEquipos {
        let p = try Self.pais(texto)
        let cuerpo = try await descargar("\(base)\(Self.prefijo)/equipos/\(p.rawValue)")
        do throws(ErrorDominio) {
            return try CatalogoEquipos.analizar(cuerpo)
        } catch {
            throw ErrorOnline("La respuesta del servicio no es un catálogo de equipos válido: " + error.mensajeUsuario)
        }
    }

    /// `DescargarConReintentoAsync` (:235-302): un solo reintento ante 429 con `Retry-After` de 1 a 3 s.
    func descargar(_ direccion: String) async throws -> String {
        guard let url = URL(string: direccion) else { throw URLError(.badURL) }
        var peticion = URLRequest(url: url)
        peticion.setValue("application/json", forHTTPHeaderField: "Accept")
        var reintentado = false
        while true {
            let (datos, respuesta): (Data, HTTPURLResponse)
            do {
                (datos, respuesta) = try await red.datos(para: peticion)
            } catch let e as URLError where e.code == .cancelled && Task.isCancelled {
                throw CancellationError()
            } catch let e as URLError where e.code == .timedOut {
                throw ErrorOnline("Se agotó el tiempo de espera al contactar con el servicio online.")
            } catch is URLError {
                throw ErrorOnline("No se pudo contactar con el servicio online (sin conexión o servidor no disponible).")
            }
            var texto = String(decoding: datos, as: UTF8.self)
            if texto.first == "\u{FEFF}" { texto.removeFirst() }
            let estado = respuesta.statusCode
            if (200...299).contains(estado) { return texto }
            if estado == 429 {
                let ra = Self.retryAfter(respuesta.value(forHTTPHeaderField: "Retry-After"), ahora: reloj.ahora())
                if !reintentado, let ra, ra > 0, ra <= Self.retryAfterMaximo {
                    reintentado = true
                    try await esperar(ra)
                    continue
                }
                throw ErrorOnline(
                    Self.mensajeDelCuerpo(texto)
                        ?? "Demasiadas solicitudes (límite 60/min). Intenta de nuevo en ~\(ra ?? 60) s.")
            }
            throw ErrorOnline(
                Self.mensajeDelCuerpo(texto) ?? "El servicio respondió con código \(estado) (\(Self.nombreEstado(estado))).")
        }
    }

    /// `LeerRetryAfter` (:309-332): segundos; fecha HTTP (≤ 1 s o pasada ⇒ 1); o el entero crudo.
    static func retryAfter(_ valor: String?, ahora: Date) -> Int? {
        guard let v = valor?.trimmingCharacters(in: .whitespaces), !v.isEmpty else { return nil }
        if v.allSatisfy(\.isASCII), v.allSatisfy(\.isNumber), let s = Int(v) { return s }
        let f = DateFormatter()
        f.locale = Locale(identifier: "en_US_POSIX")
        f.timeZone = TimeZone(identifier: "GMT")
        f.dateFormat = "EEE, dd MMM yyyy HH:mm:ss 'GMT'"
        if let fecha = f.date(from: v) {
            let s = Int(fecha.timeIntervalSince(ahora).rounded(.up))
            return s <= 1 ? 1 : s
        }
        return Int(v)
    }

    /// `MensajeDelCuerpo` (:339-365): `mensaje` y si no `error`, si son texto no blanco.
    static func mensajeDelCuerpo(_ cuerpo: String) -> String? {
        guard let v = try? JSONNET.analizar(cuerpo), case .objeto = v else { return nil }
        for clave in ["mensaje", "error"] {
            if let s = v[clave]?.texto, !s.allSatisfy(\.isWhitespace) { return s }
        }
        return nil
    }

    /// `HttpStatusCode.ToString()` de .NET 8 (DT-17); sin nombre ⇒ el número.
    static func nombreEstado(_ c: Int) -> String {
        let nombres: [Int: String] = [
            100: "Continue", 101: "SwitchingProtocols", 102: "Processing", 103: "EarlyHints", 200: "OK", 201: "Created",
            202: "Accepted", 203: "NonAuthoritativeInformation", 204: "NoContent", 205: "ResetContent",
            206: "PartialContent", 207: "MultiStatus", 208: "AlreadyReported", 226: "IMUsed", 300: "Ambiguous",
            301: "Moved", 302: "Redirect", 303: "RedirectMethod", 304: "NotModified", 305: "UseProxy", 306: "Unused",
            307: "RedirectKeepVerb", 308: "PermanentRedirect", 400: "BadRequest", 401: "Unauthorized",
            402: "PaymentRequired", 403: "Forbidden", 404: "NotFound", 405: "MethodNotAllowed", 406: "NotAcceptable",
            407: "ProxyAuthenticationRequired", 408: "RequestTimeout", 409: "Conflict", 410: "Gone", 411: "LengthRequired",
            412: "PreconditionFailed", 413: "RequestEntityTooLarge", 414: "RequestUriTooLong", 415: "UnsupportedMediaType",
            416: "RequestedRangeNotSatisfiable", 417: "ExpectationFailed", 421: "MisdirectedRequest",
            422: "UnprocessableEntity", 423: "Locked", 424: "FailedDependency", 426: "UpgradeRequired",
            428: "PreconditionRequired", 429: "TooManyRequests", 431: "RequestHeaderFieldsTooLarge",
            451: "UnavailableForLegalReasons", 500: "InternalServerError", 501: "NotImplemented", 502: "BadGateway",
            503: "ServiceUnavailable", 504: "GatewayTimeout", 505: "HttpVersionNotSupported", 506: "VariantAlsoNegotiates",
            507: "InsufficientStorage", 508: "LoopDetected", 510: "NotExtended", 511: "NetworkAuthenticationRequired",
        ]
        return nombres[c] ?? String(c)
    }
}
