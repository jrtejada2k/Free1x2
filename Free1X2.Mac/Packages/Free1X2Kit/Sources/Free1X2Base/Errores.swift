/// Tipo de excepción .NET que el C# lanza en el mismo punto lógico (ESPEC_11 §5, DT-03).
/// El `rawValue` es el nombre .NET, que es lo que registran los golden (`"error":"FormatException"`).
public enum ExcepcionNET: String, Sendable, Codable, CaseIterable {
    case formato = "FormatException"
    case indiceFueraDeRango = "IndexOutOfRangeException"
    case argumentoFueraDeRango = "ArgumentOutOfRangeException"
    case argumento = "ArgumentException"
    case argumentoNulo = "ArgumentNullException"
    case referenciaNula = "NullReferenceException"
    case desbordamiento = "OverflowException"
    case conversionInvalida = "InvalidCastException"
    case operacionInvalida = "InvalidOperationException"
    case divisionPorCero = "DivideByZeroException"
    case archivoNoEncontrado = "FileNotFoundException"
    case directorioNoEncontrado = "DirectoryNotFoundException"
    case entradaSalida = "IOException"
    case accesoDenegado = "UnauthorizedAccessException"
    case xml = "XmlException"
    case operacionCancelada = "OperationCanceledException"
    /// `throw new Exception(…)` literal (DEC-104).
    case general = "Exception"
}

/// Error del dominio: reproduce la excepción C# sin abortar nunca (principio A5).
public struct ErrorDominio: Error, Sendable, Equatable {
    public let excepcion: ExcepcionNET
    /// Texto que hoy ve el usuario cuando el C# lo fija; si lo genera .NET, su equivalente en español.
    public let mensajeUsuario: String
    /// Punto del C# que lo origina, para trazabilidad y log: `"Free1X2.Domain/…/Fichero.cs:231"`.
    public let origen: String

    public init(_ excepcion: ExcepcionNET, _ mensajeUsuario: String = "", origen: String = "") {
        self.excepcion = excepcion
        self.mensajeUsuario = mensajeUsuario
        self.origen = origen
    }
}

/// Error del cliente online, equivalente a `QuinielaOnlineException` (07:1123-1133).
public struct ErrorOnline: Error, Sendable, Equatable {
    public let mensaje: String

    public init(_ mensaje: String) {
        self.mensaje = mensaje
    }
}

// Excepciones que genera .NET (no el código del C#): mismo tipo y su texto en español (ESPEC_11 §5.2).
extension ErrorDominio {
    public static func fueraDeRango(_ origen: String) -> ErrorDominio {
        ErrorDominio(.indiceFueraDeRango, "Índice fuera de los límites de la matriz.", origen: origen)
    }

    public static func fueraDeRangoLista(_ origen: String) -> ErrorDominio {
        ErrorDominio(
            .argumentoFueraDeRango,
            "El índice estaba fuera del intervalo. Debe ser un valor no negativo e inferior al tamaño de la colección.",
            origen: origen)
    }

    public static func referenciaNula(_ origen: String) -> ErrorDominio {
        ErrorDominio(.referenciaNula, "Referencia a objeto no establecida como instancia de un objeto.", origen: origen)
    }

    public static func formato(_ origen: String) -> ErrorDominio {
        ErrorDominio(.formato, "La cadena de entrada no tiene el formato correcto.", origen: origen)
    }

    public static func desbordamiento(_ origen: String) -> ErrorDominio {
        ErrorDominio(.desbordamiento, "La operación aritmética ha provocado un desbordamiento.", origen: origen)
    }
}
