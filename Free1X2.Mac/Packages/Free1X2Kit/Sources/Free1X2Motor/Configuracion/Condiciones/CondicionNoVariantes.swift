import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroNoVariantes.cs (03 §2) — estado configurable y sus `Set*`/`Get*`.

/// Nº de variantes (X o 2), de X y de 2 permitidos, con letras de tolerancia.
public struct CondicionNoVariantes: Sendable, Hashable {
    public var activa = false
    public var contieneDatos = false
    public private(set) var variantes: ValoresConTolerancia
    public private(set) var equis: ValoresConTolerancia
    public private(set) var doses: ValoresConTolerancia
    public let numeroPartidos: Int

    /// Arrays de `N + 1` (:46-70).
    public init(numeroPartidos: Int) {
        self.numeroPartidos = numeroPartidos
        variantes = ValoresConTolerancia(tamano: numeroPartidos + 1)
        equis = variantes
        doses = variantes
    }

    public mutating func setNoVariantes(_ n: Int, _ activo: Bool) throws(ErrorDominio) {
        try variantes.poner(n, activo, "FiltroNoVariantes.cs:220")
    }

    public mutating func setNoEquis(_ n: Int, _ activo: Bool) throws(ErrorDominio) {
        try equis.poner(n, activo, "FiltroNoVariantes.cs:259")
    }

    public mutating func setNoDoses(_ n: Int, _ activo: Bool) throws(ErrorDominio) {
        try doses.poner(n, activo, "FiltroNoVariantes.cs:298")
    }

    public mutating func setNoVariantes(_ valores: String) throws(ErrorDominio) {
        try variantes.poner(valores, "FiltroNoVariantes.cs:231-255")
    }

    public mutating func setNoEquis(_ valores: String) throws(ErrorDominio) {
        try equis.poner(valores, "FiltroNoVariantes.cs:270-294")
    }

    public mutating func setNoDoses(_ valores: String) throws(ErrorDominio) {
        try doses.poner(valores, "FiltroNoVariantes.cs:309-333")
    }

    public var getVariantes: String { variantes.texto }
    public var getEquis: String { equis.texto }
    public var getDoses: String { doses.texto }

    /// `ReinicializaValores`.
    public mutating func reinicializaValores() {
        variantes.reinicializar()
        equis.reinicializar()
        doses.reinicializar()
    }

    /// `LlenarTodosValores` (:404-416): `0…N` **inclusive** en los conjuntos vacíos.
    public mutating func llenarTodosValores() throws(ErrorDominio) {
        let todos = (0...numeroPartidos).map(String.init).joined(separator: ",")
        if !variantes.hayValores { try setNoVariantes(todos) }
        if !equis.hayValores { try setNoEquis(todos) }
        if !doses.hayValores { try setNoDoses(todos) }
    }
}
