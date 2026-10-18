import Foundation

/// Símbolos de número y fecha de una cultura .NET (ESPEC_16 §1.1). `.esES` es la del oráculo y la fija de C1–C2;
/// `.sistema()` equivale a la «cultura actual» de C3–C13 (DP-01).
public struct Cultura: Sendable, Hashable {
    public var separadorDecimal: String
    public var separadorMiles: String
    public var signoNegativo: String
    public var signoPositivo: String
    public var simboloNaN: String
    public var simboloInfinitoPositivo: String
    public var simboloInfinitoNegativo: String
    public var patronFechaCorta: String
    public var patronHoraLarga: String

    public static let esES = Cultura(
        separadorDecimal: ",", separadorMiles: ".", signoNegativo: "-", signoPositivo: "+", simboloNaN: "NaN",
        simboloInfinitoPositivo: "∞", simboloInfinitoNegativo: "-∞", patronFechaCorta: "dd/MM/yyyy",
        patronHoraLarga: "H:mm:ss")

    public static let enUS = Cultura(
        separadorDecimal: ".", separadorMiles: ",", signoNegativo: "-", signoPositivo: "+", simboloNaN: "NaN",
        simboloInfinitoPositivo: "∞", simboloInfinitoNegativo: "-∞", patronFechaCorta: "M/d/yyyy",
        patronHoraLarga: "h:mm:ss tt")

    /// `CultureInfo.InvariantCulture`.
    public static let invariante = Cultura(
        separadorDecimal: ".", separadorMiles: ",", signoNegativo: "-", signoPositivo: "+", simboloNaN: "NaN",
        simboloInfinitoPositivo: "Infinity", simboloInfinitoNegativo: "-Infinity", patronFechaCorta: "MM/dd/yyyy",
        patronHoraLarga: "HH:mm:ss")

    /// Separadores de la región del Mac; el resto, los de `es-ES`.
    public static func sistema(_ locale: Locale = .autoupdatingCurrent) -> Cultura {
        var cultura = Cultura.esES
        if let decimal = locale.decimalSeparator { cultura.separadorDecimal = decimal }
        if let miles = locale.groupingSeparator { cultura.separadorMiles = miles }
        return cultura
    }
}
