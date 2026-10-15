import Foundation

// Inyección de dependencias por parámetro (ESPEC_11 §7, DT-05): cada protocolo tiene exactamente dos
// implementaciones, la de producción (Free1X2CasosDeUso o la app) y el doble de prueba (Free1X2Pruebas).

/// Hora de pared y tiempo monótono.
public protocol Reloj: Sendable {
    /// Lo que hoy da `DateTime.Now`.
    func ahora() -> Date
    /// Tiempo desde un origen fijo del reloj, para medir duraciones y limitar el progreso.
    func monotono() -> Duration
}

/// Ficheros. `listar` devuelve el orden de NTFS: por nombre, ordinal sin distinguir mayúsculas (DT-14).
public protocol SistemaArchivos: Sendable {
    func leer(_ url: URL) throws(ErrorDominio) -> Data
    func escribir(_ datos: Data, en url: URL, atomico: Bool) throws(ErrorDominio)
    func listar(_ carpeta: URL) throws(ErrorDominio) -> [URL]
    func borrar(_ url: URL) throws(ErrorDominio)
    func existe(_ url: URL) -> Bool
    func crearCarpeta(_ url: URL) throws(ErrorDominio)
    func fechaModificacion(_ url: URL) throws(ErrorDominio) -> Date
}

/// Red: una petición y su respuesta HTTP (`URLSession` en producción). Es transporte puro: lanza el `URLError`
/// tal cual y el cliente online lo traduce a `ErrorOnline` con los textos del C# (07 §6.10).
public protocol ClienteHTTP: Sendable {
    func datos(para peticion: URLRequest) async throws -> (Data, HTTPURLResponse)
}

/// Portapapeles de texto (01 §2.23). La implementación de producción vive en la app (`NSPasteboard`).
public protocol Portapapeles: Sendable {
    func leerTexto() async -> String?
    func escribirTexto(_ texto: String) async
}

/// Registro (log). Nunca lanza (07:1166-1173).
public protocol Registro: Sendable {
    func registrar(_ mensaje: String, nivel: NivelRegistro)
}

public enum NivelRegistro: String, Sendable {
    case info, aviso, error
}

/// Carpetas de la app (DP-13, ESPEC_11 §9).
public struct RutasApp: Sendable, Equatable {
    /// Las 10 carpetas de trabajo que hoy se crean junto al `.exe` (10:33; `Free1X2.WinUI/App.xaml.cs:102-120`).
    public enum Carpeta: String, Sendable, CaseIterable {
        case columnas = "Columnas"
        case combinaciones = "Combinaciones"
        case condiciones = "Condiciones"
        case filtros = "Filtros"
        case ganadoras = "Ganadoras"
        case informes = "Informes"
        case jornadas = "Jornadas"
        case temp = "Temp"
        case comunicaciones = "Comunicaciones"
        case documentacion = "Documentacion"
    }

    /// `~/Library/Application Support/Free1X2/` en producción.
    public var base: URL
    /// Recursos de solo lectura del bundle (semillas, `Impresion/impresoras.cfg`, documentación).
    public var recursos: URL
    /// `~/Library/Logs/Free1X2/` en producción.
    public var logs: URL

    public init(base: URL, recursos: URL, logs: URL) {
        self.base = base
        self.recursos = recursos
        self.logs = logs
    }

    public func carpeta(_ carpeta: Carpeta) -> URL {
        base.appending(path: carpeta.rawValue, directoryHint: .isDirectory)
    }
}

/// Todo lo que un caso de uso necesita del mundo exterior. El bucle caliente del motor no lo toca.
/// ponytail: `aleatorio` (AleatorioNET) y `cultura` (Cultura) se añaden con sus tipos en M3 (ESPEC_16 §1, §5).
public struct Entorno: Sendable {
    public var reloj: any Reloj
    public var archivos: any SistemaArchivos
    public var rutas: RutasApp
    public var red: any ClienteHTTP
    public var portapapeles: any Portapapeles
    public var registro: any Registro

    public init(
        reloj: any Reloj,
        archivos: any SistemaArchivos,
        rutas: RutasApp,
        red: any ClienteHTTP,
        portapapeles: any Portapapeles,
        registro: any Registro
    ) {
        self.reloj = reloj
        self.archivos = archivos
        self.rutas = rutas
        self.red = red
        self.portapapeles = portapapeles
        self.registro = registro
    }
}
