import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroContactos.cs (02 §3) — estado configurable y sus `Set*`/`Get*`.

/// Contactos (pares de partidos consecutivos) por tipo, con letras de tolerancia y figuras.
public struct CondicionContactos: Sendable, Hashable {
    /// Los 10 contadores en el orden de `EsColumnaValida` (`CON:204-223`).
    public enum Contacto: Int, Sendable, CaseIterable {
        case unoX, unoDos, xDos, unoUno, xx, dosDos, unoV, xV, dosV, vv

        public var par: String {
            ["1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV"][rawValue]
        }

        /// Orden de `AnalizarFallos` (`CON:225-280`).
        static let ordenFallos: [Contacto] = [.unoUno, .unoX, .unoDos, .unoV, .xx, .xDos, .xV, .dosDos, .dosV, .vv]
    }

    public var activa = false
    public var contieneDatos = false
    public private(set) var valores: [ValoresConTolerancia]
    /// `Figuras`: `nil` si el fichero no las trae.
    public var figuras: [Int64]?
    public let numeroPartidos: Int

    /// El constructor no inicializa las letras: quedan a `'\0'` (C4) [PRESERVAR].
    public init(numeroPartidos: Int) {
        self.numeroPartidos = numeroPartidos
        valores = Array(repeating: ValoresConTolerancia(tamano: numeroPartidos + 1, letraInicial: 0), count: 10)
    }

    /// `SetNum1X` … `SetNumVV`.
    public mutating func set(_ c: Contacto, _ texto: String) throws(ErrorDominio) {
        try valores[c.rawValue].poner(texto, "FiltroContactos.cs:411-899")
    }

    public func get(_ c: Contacto) -> String { valores[c.rawValue].texto }

    /// `Inicializa()` / `ReinicializaValores()`: todo a `false` y letras a `' '`.
    public mutating func reinicializaValores() {
        valores = Array(repeating: ValoresConTolerancia(tamano: numeroPartidos + 1), count: 10)
    }

    /// `LlenarTodosValores` (`CON:922-941`): `0…N−1` en los vacíos (C5).
    public mutating func llenarTodosValores() throws(ErrorDominio) {
        let todos = (0..<numeroPartidos).map(String.init).joined(separator: ",")
        for c in Contacto.allCases where !valores[c.rawValue].hayValores { try set(c, todos) }
    }
}
