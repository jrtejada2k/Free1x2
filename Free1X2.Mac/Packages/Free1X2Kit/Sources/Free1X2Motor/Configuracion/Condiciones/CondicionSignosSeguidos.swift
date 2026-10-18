import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroSignosSeguidos.cs (02 §4) — estado configurable y sus `Set*`/`Get*`.

/// Racha máxima de variantes, 1, X y 2 permitida, con letras de tolerancia y figuras de rachas por clase.
public struct CondicionSignosSeguidos: Sendable, Hashable {
    public enum Clase: Int, Sendable, CaseIterable { case variantes, unos, equis, doses }

    public var activa = false
    public var contieneDatos = false
    public private(set) var valores: [ValoresConTolerancia]
    /// `FigurasV`, `Figuras1`, `FigurasX`, `Figuras2`: vacía = sin restricción.
    public var figuras: [[Int64]] = [[], [], [], []]
    public let numeroPartidos: Int

    public init(numeroPartidos: Int) {
        self.numeroPartidos = numeroPartidos
        valores = Array(repeating: ValoresConTolerancia(tamano: numeroPartidos + 1), count: 4)
    }

    /// `SetNoVariantes`, `SetNoUnos`, `SetNoEquis`, `SetNoDoses` (`SEG:414-516`).
    public mutating func set(_ c: Clase, _ texto: String) throws(ErrorDominio) {
        try valores[c.rawValue].poner(texto, "FiltroSignosSeguidos.cs:414-516")
    }

    public func get(_ c: Clase) -> String { valores[c.rawValue].texto }

    public mutating func reinicializaValores() {
        for i in valores.indices { valores[i].reinicializar() }
    }

    /// `LlenarTodosValores` (`SEG:619-632`): `0…N` en los vacíos.
    public mutating func llenarTodosValores() throws(ErrorDominio) {
        let todos = (0...numeroPartidos).map(String.init).joined(separator: ",")
        for c in Clase.allCases where !valores[c.rawValue].hayValores { try set(c, todos) }
    }

    /// `UsaFiguras()`: alguna de las 4 listas no vacía.
    public var usaFiguras: Bool { figuras.contains { !$0.isEmpty } }
}
