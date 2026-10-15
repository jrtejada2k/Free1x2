import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroDiferencias.cs y Diferencia.cs (03 §12-13).

/// Sub-grupos de partidos y cuántos valores distintos pueden tomar entre ellos variantes, X, 2, dibujos, interrupciones y
/// formatos.
public struct Diferencia: Sendable, Hashable {
    /// Cada sub-grupo es un `bool[]` 1-based (`N + 1`).
    public var partidosSimetricos: [[Bool]] = []
    public var acV: [Bool]?
    public var acX: [Bool]?
    public var acDoses: [Bool]?
    public var acDib: [Bool]?
    public var acInt: [Bool]?
    public var acFormatos: [Bool]?
    public var analizaVX2Dib = true
    public var analizaFormatos = true
    public var analizaInterrupciones = true

    public init() {}
}

/// Todas las diferencias deben cumplirse.
public struct CondicionDiferencias: Sendable, Hashable {
    public var activa = false
    public var diferencias: [Diferencia] = []
    private var contieneDatosGuardado = false

    public init() {}

    /// `ContieneDatos` (getter con efecto, :107-122): sin diferencias lo pone a `false`; si no, a `true`.
    public mutating func contieneDatos() -> Bool {
        contieneDatosGuardado = !diferencias.isEmpty
        return contieneDatosGuardado
    }

    public mutating func ponerContieneDatos(_ v: Bool) { contieneDatosGuardado = v }
}
