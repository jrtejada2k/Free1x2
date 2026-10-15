import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroInterrupciones.cs (02 §2) — estado configurable y sus `Set*`/`Get*`.

/// Cambios de signo entre partidos consecutivos y sus rachas, con letras de tolerancia.
public struct CondicionInterrupciones: Sendable, Hashable {
    /// Los 10 contadores, en el orden de `EsColumnaValida` y de `AnalizarFallos`.
    public enum Contador: Int, Sendable, CaseIterable {
        case global, variantes, unos, equis, doses
        case globalSeg, variantesSeg, unosSeg, equisSeg, dosesSeg

        /// Nombre en los mensajes de `AnalizarFallos` (`INT:357-407`).
        var nombre: String {
            switch self {
            case .global: "número de interrupciones"
            case .variantes: "interrupciones de variantes"
            case .unos: "interrupciones de unos"
            case .equis: "interrupciones de equis"
            case .doses: "interrupciones de doses"
            case .globalSeg: "número de interrupciones seguidas"
            case .variantesSeg: "interrupciones de variantes seguidas"
            case .unosSeg: "interrupciones de unos seguidas"
            case .equisSeg: "interrupciones de equis seguidas"
            case .dosesSeg: "interrupciones de doses seguidas"
            }
        }
    }

    public var activa = false
    public var contieneDatos = false
    public private(set) var valores: [ValoresConTolerancia]
    public let numeroPartidos: Int

    public init(numeroPartidos: Int) {
        self.numeroPartidos = numeroPartidos
        valores = Array(repeating: ValoresConTolerancia(tamano: numeroPartidos + 1), count: 10)
    }

    /// `SetNoIntGlobales`, `SetNoIntVar`, …, `SetNoInt2Seg` (`INT:523-1012`).
    public mutating func set(_ c: Contador, _ texto: String) throws(ErrorDominio) {
        try valores[c.rawValue].poner(texto, "FiltroInterrupciones.cs:523-1012")
    }

    /// `GetIntGlobales`, …, `GetInt2Seg`.
    public func get(_ c: Contador) -> String { valores[c.rawValue].texto }

    public mutating func reinicializaValores() {
        for i in valores.indices { valores[i].reinicializar() }
    }

    /// `LlenarTodosValores` (`INT:1036-1055`): `0…N` inclusive en los vacíos.
    public mutating func llenarTodosValores() throws(ErrorDominio) {
        let todos = (0...numeroPartidos).map(String.init).joined(separator: ",")
        for c in Contador.allCases where !valores[c.rawValue].hayValores { try set(c, todos) }
    }
}
