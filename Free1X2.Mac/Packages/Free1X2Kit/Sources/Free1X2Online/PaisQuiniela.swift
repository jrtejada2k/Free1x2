// C#: Free1X2.WinUI/Services/QuinielaOnlineService.cs, JornadaCache.cs:52 (07 §6.9-6.11; docs/API_CLUBPROGOL.md)

/// País de la jornada online: España (con Pleno al 15) o México.
public enum PaisQuiniela: String, Sendable, Codable, CaseIterable {
    case es
    case mx

    /// Fichero de la caché offline-first en la carpeta base: `jornada-es.json`, `jornada-mx.json`.
    public var ficheroCache: String { "jornada-\(rawValue).json" }

    /// Código que trae el JSON del contrato (`"pais":"ES"`).
    public var codigoContrato: String { rawValue.uppercased() }
}
