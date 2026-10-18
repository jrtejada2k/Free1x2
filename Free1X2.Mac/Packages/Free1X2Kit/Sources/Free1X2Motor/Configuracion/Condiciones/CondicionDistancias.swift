import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroDistancias.cs (03 §1) — estado configurable y sus `Set*`/`Get*`.

/// Mayor separación entre apariciones consecutivas de `1`, `X`, `2` y variantes, con letras de tolerancia.
public struct CondicionDistancias: Sendable, Hashable {
    public var activa = false
    public var contieneDatos = false
    /// `intGlobal`: se rellena pero no se evalúa; en tolerancias se consulta siempre su posición 0 (03 §1.1-1.2).
    public private(set) var global: ValoresConTolerancia
    public private(set) var variantes: ValoresConTolerancia
    public private(set) var unos: ValoresConTolerancia
    public private(set) var equis: ValoresConTolerancia
    public private(set) var doses: ValoresConTolerancia
    public let numeroPartidos: Int

    public init(numeroPartidos: Int) {
        self.numeroPartidos = numeroPartidos
        global = ValoresConTolerancia(tamano: numeroPartidos + 1)
        (variantes, unos, equis, doses) = (global, global, global, global)
    }

    public mutating func setNoIntGlobales(_ v: String) throws(ErrorDominio) { try global.poner(v, "FiltroDistancias.cs:260") }
    /// `SetdistVar` = `SetNoIntVar`.
    public mutating func setDistVariantes(_ v: String) throws(ErrorDominio) { try variantes.poner(v, "FiltroDistancias.cs:360") }
    public mutating func setDistUnos(_ v: String) throws(ErrorDominio) { try unos.poner(v, "FiltroDistancias.cs:285") }
    public mutating func setDistEquis(_ v: String) throws(ErrorDominio) { try equis.poner(v, "FiltroDistancias.cs:310") }
    public mutating func setDistDoses(_ v: String) throws(ErrorDominio) { try doses.poner(v, "FiltroDistancias.cs:335") }

    public var getIntVar: String { variantes.texto }
    public var getInt1: String { unos.texto }
    public var getIntX: String { equis.texto }
    public var getInt2: String { doses.texto }

    public mutating func reinicializaValores() {
        for k in [\Self.global, \.variantes, \.unos, \.equis, \.doses] as [WritableKeyPath<Self, ValoresConTolerancia>] {
            self[keyPath: k].reinicializar()
        }
    }

    /// `LlenarTodosValores` (:597-610): `0…N−1` (sin N) en los vacíos, en el orden unos, equis, doses, variantes.
    public mutating func llenarTodosValores() throws(ErrorDominio) {
        let todos = (0..<numeroPartidos).map(String.init).joined(separator: ",")
        if !unos.hayValores { try setDistUnos(todos) }
        if !equis.hayValores { try setDistEquis(todos) }
        if !doses.hayValores { try setDistDoses(todos) }
        if !variantes.hayValores { try setDistVariantes(todos) }
    }
}
