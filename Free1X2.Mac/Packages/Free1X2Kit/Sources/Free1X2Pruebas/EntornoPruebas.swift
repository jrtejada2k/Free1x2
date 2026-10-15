import Foundation
import Free1X2Base
import os

// Dobles de prueba de `Entorno` (ESPEC_14 §2). Deterministas por construcción.

extension Entorno {
    /// Reloj fijo 2026-01-01 12:00 UTC, ficheros en memoria, red sin respuestas, portapapeles y registro en memoria.
    public static func pruebas(
        reloj: RelojManual = RelojManual(),
        archivos: (any SistemaArchivos)? = nil,
        red: ClienteHTTPGrabado = ClienteHTTPGrabado(),
        portapapeles: PortapapelesEnMemoria = PortapapelesEnMemoria(),
        registro: RegistroEnMemoria = RegistroEnMemoria()
    ) -> Entorno {
        Entorno(
            reloj: reloj,
            archivos: archivos ?? ArchivosEnMemoria(reloj: reloj),
            rutas: .pruebas,
            red: red,
            portapapeles: portapapeles,
            registro: registro
        )
    }
}

extension RutasApp {
    /// Rutas ficticias para `ArchivosEnMemoria`.
    public static let pruebas = RutasApp(
        base: URL(filePath: "/pruebas/Free1X2", directoryHint: .isDirectory),
        recursos: URL(filePath: "/pruebas/Recursos", directoryHint: .isDirectory),
        logs: URL(filePath: "/pruebas/Logs", directoryHint: .isDirectory)
    )
}

/// Reloj que solo avanza cuando el test lo pide.
public final class RelojManual: Reloj {
    /// 2026-01-01 12:00:00 UTC.
    public static let fechaInicial = Date(timeIntervalSince1970: 1_767_268_800)

    private let transcurrido = OSAllocatedUnfairLock<Duration>(initialState: .zero)

    public init() {}

    public func avanzar(_ duracion: Duration) {
        transcurrido.withLock { $0 += duracion }
    }

    public func ahora() -> Date {
        let t = transcurrido.withLock { $0 }
        return Self.fechaInicial.addingTimeInterval(Double(t.components.seconds) + Double(t.components.attoseconds) / 1e18)
    }

    public func monotono() -> Duration { transcurrido.withLock { $0 } }
}

/// Árbol de ficheros en memoria; la fecha de modificación es la del reloj del entorno.
public final class ArchivosEnMemoria: SistemaArchivos {
    private struct Estado {
        var ficheros: [String: (datos: Data, fecha: Date)] = [:]
        var carpetas: Set<String> = []
    }

    private let reloj: any Reloj
    private let estado = OSAllocatedUnfairLock(initialState: Estado())

    public init(reloj: any Reloj = RelojManual()) {
        self.reloj = reloj
    }

    public func leer(_ url: URL) throws(ErrorDominio) -> Data {
        guard let fichero = estado.withLock({ $0.ficheros[Self.clave(url)] }) else { throw Self.noEncontrado(url) }
        return fichero.datos
    }

    public func escribir(_ datos: Data, en url: URL, atomico: Bool) throws(ErrorDominio) {
        let fecha = reloj.ahora()
        estado.withLock { $0.ficheros[Self.clave(url)] = (datos, fecha) }
    }

    public func listar(_ carpeta: URL) throws(ErrorDominio) -> [URL] {
        let padre = Self.clave(carpeta)
        let (existe, nombres) = estado.withLock { e -> (Bool, [String]) in
            let hijos =
                e.ficheros.keys.filter { ($0 as NSString).deletingLastPathComponent == padre }
                + e.carpetas.filter { ($0 as NSString).deletingLastPathComponent == padre }
            return (e.carpetas.contains(padre), hijos)
        }
        guard existe else {
            throw ErrorDominio(.directorioNoEncontrado, "No se pudo encontrar una parte de la ruta de acceso '\(padre)'.")
        }
        return nombres.sorted { OrdenNTFS.precede(($0 as NSString).lastPathComponent, ($1 as NSString).lastPathComponent) }
            .map { URL(filePath: $0) }
    }

    public func borrar(_ url: URL) throws(ErrorDominio) {
        let borrado = estado.withLock { $0.ficheros.removeValue(forKey: Self.clave(url)) != nil }
        if !borrado { throw Self.noEncontrado(url) }
    }

    public func existe(_ url: URL) -> Bool {
        let c = Self.clave(url)
        return estado.withLock { $0.ficheros[c] != nil || $0.carpetas.contains(c) }
    }

    public func crearCarpeta(_ url: URL) throws(ErrorDominio) {
        var rutas: [String] = []
        var ruta = Self.clave(url)
        while ruta != "/" && !ruta.isEmpty {
            rutas.append(ruta)
            ruta = (ruta as NSString).deletingLastPathComponent
        }
        let nuevas = rutas
        estado.withLock { $0.carpetas.formUnion(nuevas) }
    }

    public func fechaModificacion(_ url: URL) throws(ErrorDominio) -> Date {
        guard let fichero = estado.withLock({ $0.ficheros[Self.clave(url)] }) else { throw Self.noEncontrado(url) }
        return fichero.fecha
    }

    private static func clave(_ url: URL) -> String {
        url.standardizedFileURL.path(percentEncoded: false).trimmingSuffix("/")
    }

    private static func noEncontrado(_ url: URL) -> ErrorDominio {
        ErrorDominio(.archivoNoEncontrado, "No se pudo encontrar el archivo '\(clave(url))'.")
    }
}

/// Red grabada: respuestas por URL; una URL sin respuesta se comporta como «sin conexión».
public struct ClienteHTTPGrabado: ClienteHTTP {
    public struct Respuesta: Sendable {
        public var estado: Int
        public var cabeceras: [String: String]
        public var cuerpo: Data

        public init(estado: Int = 200, cabeceras: [String: String] = [:], cuerpo: Data = Data()) {
            self.estado = estado
            self.cabeceras = cabeceras
            self.cuerpo = cuerpo
        }
    }

    public var respuestas: [URL: Respuesta]

    public init(respuestas: [URL: Respuesta] = [:]) {
        self.respuestas = respuestas
    }

    public func datos(para peticion: URLRequest) async throws -> (Data, HTTPURLResponse) {
        guard let url = peticion.url, let r = respuestas[url],
            let http = HTTPURLResponse(url: url, statusCode: r.estado, httpVersion: "HTTP/1.1", headerFields: r.cabeceras)
        else { throw URLError(.notConnectedToInternet) }
        return (r.cuerpo, http)
    }
}

public final class PortapapelesEnMemoria: Portapapeles {
    private let texto = OSAllocatedUnfairLock<String?>(initialState: nil)

    public init() {}

    public func leerTexto() async -> String? { texto.withLock { $0 } }
    public func escribirTexto(_ nuevo: String) async { texto.withLock { $0 = nuevo } }
}

public final class RegistroEnMemoria: Registro {
    private let almacen = OSAllocatedUnfairLock<[String]>(initialState: [])

    public init() {}

    /// Líneas registradas, como `"<nivel>: <mensaje>"`.
    public var lineas: [String] { almacen.withLock { $0 } }

    public func registrar(_ mensaje: String, nivel: NivelRegistro) {
        almacen.withLock { $0.append("\(nivel.rawValue): \(mensaje)") }
    }
}

extension String {
    fileprivate func trimmingSuffix(_ sufijo: String) -> String {
        count > 1 && hasSuffix(sufijo) ? String(dropLast(sufijo.count)) : self
    }
}
