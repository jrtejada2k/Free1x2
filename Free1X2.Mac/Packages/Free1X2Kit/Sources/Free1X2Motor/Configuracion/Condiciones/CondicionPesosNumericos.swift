import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroPesosNumericos.cs (02 §1) — estado configurable y sus `Set*`/`Get*`.

/// Conjunto de enteros permitidos sin letras (`bool[]`), con los setters `SetX(string)` de Pesos numéricos.
public struct ValoresPermitidos: Sendable, Hashable {
    public private(set) var permitidos: [Bool]

    public init(tamano: Int) { permitidos = Array(repeating: false, count: tamano) }

    public var hayValores: Bool { permitidos.contains(true) }

    /// `SetX(int, bool)`.
    public mutating func poner(_ n: Int, _ activo: Bool, _ origen: @autoclosure () -> String) throws(ErrorDominio) {
        try permitidos.escribir(activo, en: n, origen())
    }

    /// `SetX(string)` (:528-529…): `""` no hace nada; si no, `Convert.ToInt32` de cada trozo.
    public mutating func poner(_ valores: String, _ origen: @autoclosure () -> String) throws(ErrorDominio) {
        guard !valores.isEmpty else { return }
        for trozo in trozos(valores, ",") { try poner(Int(try FormatoNET.convertToInt32(trozo)), true, origen()) }
    }

    /// `GetX()`: índices marcados separados por comas.
    public var texto: String { permitidos.indices.filter { permitidos[$0] }.map(String.init).joined(separator: ",") }

    @inline(__always)
    public func permite(_ n: Int, _ origen: @autoclosure () -> String) throws(ErrorDominio) -> Bool {
        try permitidos.leer(n, origen())
    }
}

/// Pesos numéricos (raíz digital por signo) permitidos y tolerados, con tolerancias propias y figuras.
public struct CondicionPesosNumericos: Sendable, Hashable {
    public enum Peso: Int, Sendable, CaseIterable { case global, variantes, unos, equis, doses }

    public var activa = false
    public var contieneDatos = false
    /// `pn*` (permitidos) y `pn*Tol` (tolerados), en el orden de `Peso`.
    public private(set) var permitidos: [ValoresPermitidos]
    public private(set) var tolerados: [ValoresPermitidos]
    /// `tolerancias[k]`: se admiten `k` fallos tolerados (k = 0…5).
    public private(set) var tolerancias = ValoresPermitidos(tamano: 6)
    /// `Figuras` (`List<long>`, vacía por defecto).
    public var figuras: [Int64] = []

    public init(numeroPartidos: Int) {
        permitidos = Array(repeating: ValoresPermitidos(tamano: numeroPartidos + 1), count: 5)
        tolerados = permitidos
    }

    public mutating func setPN(_ p: Peso, _ valores: String) throws(ErrorDominio) {
        try permitidos[p.rawValue].poner(valores, "FiltroPesosNumericos.cs:526-764")
    }

    public mutating func setPNTol(_ p: Peso, _ valores: String) throws(ErrorDominio) {
        try tolerados[p.rawValue].poner(valores, "FiltroPesosNumericos.cs:526-764")
    }

    public func getPN(_ p: Peso) -> String { permitidos[p.rawValue].texto }
    public func getPNTol(_ p: Peso) -> String { tolerados[p.rawValue].texto }

    /// `PonerTolerancia(string)`.
    public mutating func ponerTolerancia(_ valores: String) throws(ErrorDominio) {
        try tolerancias.poner(valores, "FiltroPesosNumericos.cs:520")
    }

    public mutating func ponerTolerancia(_ n: Int, _ activa: Bool) throws(ErrorDominio) {
        try tolerancias.poner(n, activa, "FiltroPesosNumericos.cs:516")
    }

    public var getTolerancias: String { tolerancias.texto }

    /// `ReinicializaValores` (:487-511): todo salvo las figuras.
    public mutating func reinicializaValores() {
        let n = permitidos[0].permitidos.count
        permitidos = Array(repeating: ValoresPermitidos(tamano: n), count: 5)
        tolerados = permitidos
        tolerancias = ValoresPermitidos(tamano: 6)
    }

    /// `LlenarTodosValores` (:1051-1070): `0…9` en cada uno de los 10 arrays vacíos; no toca `tolerancias`.
    public mutating func llenarTodosValores() throws(ErrorDominio) {
        let todos = (0..<10).map(String.init).joined(separator: ",")
        for p in Peso.allCases where !permitidos[p.rawValue].hayValores { try setPN(p, todos) }
        for p in Peso.allCases where !tolerados[p.rawValue].hayValores { try setPNTol(p, todos) }
    }
}
