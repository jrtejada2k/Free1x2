import Foundation
import Free1X2Base
import os

extension Entorno {
    /// Entorno real de la app. El portapapeles lo aporta la app, porque AppKit no entra en el paquete (ESPEC_11 §3.2).
    public static func produccion(portapapeles: any Portapapeles) -> Entorno {
        Entorno(
            reloj: RelojSistema(),
            archivos: SistemaArchivosLocal(),
            rutas: .produccion(),
            red: ClienteHTTPSistema(),
            portapapeles: portapapeles,
            registro: RegistroSistema()
        )
    }
}

extension RutasApp {
    /// `~/Library/Application Support/Free1X2/` y `~/Library/Logs/Free1X2/` (DP-13; ESPEC_11 §9).
    public static func produccion(bundle: Bundle = .main) -> RutasApp {
        RutasApp(
            base: URL.applicationSupportDirectory.appending(path: "Free1X2", directoryHint: .isDirectory),
            recursos: bundle.resourceURL ?? bundle.bundleURL,
            logs: URL.libraryDirectory.appending(path: "Logs/Free1X2", directoryHint: .isDirectory)
        )
    }
}

package struct RelojSistema: Reloj {
    private let origen = ContinuousClock.now

    package init() {}

    package func ahora() -> Date { Date() }
    package func monotono() -> Duration { ContinuousClock.now - origen }
}

/// `FileManager` con los errores traducidos a las excepciones .NET equivalentes y el orden de NTFS al listar.
package struct SistemaArchivosLocal: SistemaArchivos {
    package init() {}

    package func leer(_ url: URL) throws(ErrorDominio) -> Data {
        do { return try Data(contentsOf: url) } catch { throw Self.traducir(error, url) }
    }

    package func escribir(_ datos: Data, en url: URL, atomico: Bool) throws(ErrorDominio) {
        do { try datos.write(to: url, options: atomico ? .atomic : []) } catch { throw Self.traducir(error, url) }
    }

    package func listar(_ carpeta: URL) throws(ErrorDominio) -> [URL] {
        do {
            return try FileManager.default.contentsOfDirectory(at: carpeta, includingPropertiesForKeys: nil)
                .sorted { OrdenNTFS.precede($0.lastPathComponent, $1.lastPathComponent) }
        } catch {
            throw Self.traducir(error, carpeta, esCarpeta: true)
        }
    }

    package func borrar(_ url: URL) throws(ErrorDominio) {
        do { try FileManager.default.removeItem(at: url) } catch { throw Self.traducir(error, url) }
    }

    package func existe(_ url: URL) -> Bool {
        FileManager.default.fileExists(atPath: url.path(percentEncoded: false))
    }

    package func crearCarpeta(_ url: URL) throws(ErrorDominio) {
        do {
            try FileManager.default.createDirectory(at: url, withIntermediateDirectories: true)
        } catch {
            throw Self.traducir(error, url, esCarpeta: true)
        }
    }

    package func fechaModificacion(_ url: URL) throws(ErrorDominio) -> Date {
        let atributos: [FileAttributeKey: Any]
        do {
            atributos = try FileManager.default.attributesOfItem(atPath: url.path(percentEncoded: false))
        } catch {
            throw Self.traducir(error, url)
        }
        guard let fecha = atributos[.modificationDate] as? Date else {
            throw ErrorDominio(.entradaSalida, "Sin fecha de modificación: '\(url.path(percentEncoded: false))'.", origen: "FileManager")
        }
        return fecha
    }

    /// Mensajes de .NET en español (ESPEC_11 §5.2: «si .NET lo genera, su equivalente en español»).
    static func traducir(_ error: any Error, _ url: URL, esCarpeta: Bool = false) -> ErrorDominio {
        let ruta = url.path(percentEncoded: false)
        switch (error as? CocoaError)?.code {
        case .fileReadNoSuchFile?, .fileNoSuchFile?:
            return esCarpeta
                ? ErrorDominio(
                    .directorioNoEncontrado, "No se pudo encontrar una parte de la ruta de acceso '\(ruta)'.", origen: "FileManager")
                : ErrorDominio(.archivoNoEncontrado, "No se pudo encontrar el archivo '\(ruta)'.", origen: "FileManager")
        case .fileReadNoPermission?, .fileWriteNoPermission?:
            return ErrorDominio(.accesoDenegado, "Acceso denegado a la ruta de acceso '\(ruta)'.", origen: "FileManager")
        default:
            return ErrorDominio(.entradaSalida, error.localizedDescription, origen: "FileManager")
        }
    }
}

/// Transporte HTTP (DT-16): sesión efímera, 10 s, `Accept: application/json`, `User-Agent: Free1X2/<versión> (macOS)`.
/// Los mensajes para el usuario los compone el cliente online (M4) a partir del `URLError`.
package struct ClienteHTTPSistema: ClienteHTTP {
    private let sesion: URLSession

    package init(bundle: Bundle = .main) {
        let version = bundle.infoDictionary?["CFBundleShortVersionString"] as? String ?? "0"
        let configuracion = URLSessionConfiguration.ephemeral
        configuracion.timeoutIntervalForRequest = 10
        configuracion.requestCachePolicy = .reloadIgnoringLocalCacheData
        configuracion.httpAdditionalHeaders = ["Accept": "application/json", "User-Agent": "Free1X2/\(version) (macOS)"]
        sesion = URLSession(configuration: configuracion)
    }

    package func datos(para peticion: URLRequest) async throws -> (Data, HTTPURLResponse) {
        let (datos, respuesta) = try await sesion.data(for: peticion)
        guard let http = respuesta as? HTTPURLResponse else { throw URLError(.badServerResponse) }
        return (datos, http)
    }
}

/// Registro en `os.Logger`. ponytail: el fichero `~/Library/Logs/Free1X2/log.txt` con el formato y la rotación de
/// 07:1166-1173 llega con su caso de uso en M4.
package struct RegistroSistema: Registro {
    private let logger = Logger(subsystem: "com.free1x2.app", category: "app")

    package init() {}

    package func registrar(_ mensaje: String, nivel: NivelRegistro) {
        switch nivel {
        case .info: logger.info("\(mensaje, privacy: .public)")
        case .aviso: logger.warning("\(mensaje, privacy: .public)")
        case .error: logger.error("\(mensaje, privacy: .public)")
        }
    }
}
