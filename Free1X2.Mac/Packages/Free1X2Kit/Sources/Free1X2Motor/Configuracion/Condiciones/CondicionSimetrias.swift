import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroSimetrias.cs y Simetria.cs (03 §6-7).

/// Un grupo de partidos (1-based) que deben llevar el mismo signo.
public struct Simetria: Sendable, Hashable {
    public let partidos: String
    public let partidosSimetricos: [Int]

    /// `new Simetria("1,4,5")`: `Convert.ToInt32` de cada trozo.
    public init(_ partidos: String) throws(ErrorDominio) {
        var p: [Int] = []
        for trozo in trozos(partidos, ",") { p.append(Int(try FormatoNET.convertToInt32(trozo))) }
        partidosSimetricos = p
        self.partidos = partidos
    }
}

/// Nº de simetrías cumplidas permitido.
public struct CondicionSimetrias: Sendable, Hashable {
    public var activa = false
    public var simetrias: [Simetria] = []
    public var aciertos: [Int] = []
    /// `Aciertos` (texto guardado en el fichero).
    public var aciertosTexto = ""

    public init() {}

    /// `ContieneDatos` (getter con efecto, :161-174): sin simetrías **vacía** `aciertos` y devuelve `false`.
    public mutating func contieneDatos() -> Bool {
        if simetrias.isEmpty {
            aciertos = []
            return false
        }
        return true
    }
}
