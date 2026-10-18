// C#: Free1X2.Domain/Utils/TablaDeSeleccion.cs, Combinacion.cs y Pral.cs (06 §2.11-§2.13).

/// Fila de mínimos/máximos del diálogo de selección del banco de pruebas: asignar `minimo` o `maximo` la marca.
public struct TablaDeSeleccion: Sendable, Hashable {
    public let concepto: String
    public var minimo: Double { didSet { checked = true } }
    public var maximo: Double { didSet { checked = true } }
    public var checked = false

    /// El constructor no marca la fila.
    public init(concepto: String, minimo: Double, maximo: Double) {
        self.concepto = concepto
        self.minimo = minimo
        self.maximo = maximo
    }
}

/// `Utils.Combinacion`: una combinación del análisis múltiple de tramos (no confundir con `Combinacion` del motor).
public struct CombinacionJornada: Sendable, Hashable {
    public var temporada: String
    public var jornada: String
    public var path: String

    public init(temporada: String, jornada: String, path: String) {
        self.temporada = temporada
        self.jornada = jornada
        self.path = path
    }
}

public enum Pral {
    /// `Normaliza` (:26-37): solo los signos `1`, `X`, `2` (con `x` a mayúscula).
    public static func normaliza(_ columna: String) -> String {
        String(
            columna.unicodeScalars.compactMap { s -> Character? in
                switch s {
                case "1", "2", "X": Character(s)
                case "x": "X"
                default: nil
                }
            })
    }
}
