import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/ControlGrupos.cs y ControlConjuntos.cs (01 §2.7-2.8).

/// Un control de grupos: los grupos que agrupa y, opcionalmente, cuántos pueden fallar.
public struct ControlGruposConfig: Sendable, Hashable {
    /// Mensaje que el C# muestra con `UserDialogs.ShowError` al poner valores incorrectos (:194,222).
    public static let mensajeError = "Hay errores en los valores introducidos"

    public var usaControlGrupos = false
    public var fallosPermitidos: [Bool]?
    public var gruposControlados: [Int] = []
    public private(set) var textoFallosPermitidos = ""
    public private(set) var textoGruposControlados = ""

    public init() {}

    /// `PonerGruposControlados` (:179-197). Devuelve `false` si el C# habría mostrado `mensajeError`: el array queda
    /// parcial (ceros desde el valor que falló). `ObtenerValoresSeparadosPorComas` lanza fuera del `try`.
    @discardableResult
    public mutating func ponerGruposControlados(_ texto: String) throws(ErrorDominio) -> Bool {
        textoGruposControlados = texto
        let partes = trozos(try UtilidadesEntradasValores.obtenerValoresSeparadosPorComas(texto), ",")
        gruposControlados = Array(repeating: 0, count: partes.count)
        for (i, parte) in partes.enumerated() {
            guard let n = try? FormatoNET.convertToInt32(parte) else { return false }
            gruposControlados[i] = Int(n)
        }
        return true
    }

    /// `PonerFallosPermitidos` (:199-232). `""` ⇒ sin control de fallos (el array anterior se queda). Devuelve `false` si el
    /// C# habría mostrado `mensajeError`: el array queda parcial y `usaControlGrupos` sin cambiar.
    @discardableResult
    public mutating func ponerFallosPermitidos(_ texto: String) throws(ErrorDominio) -> Bool {
        textoFallosPermitidos = texto
        guard !texto.isEmpty else {
            usaControlGrupos = false
            return true
        }
        let partes = trozos(try UtilidadesEntradasValores.obtenerValoresSeparadosPorComas(texto), ",")
        var fallos = Array(repeating: false, count: gruposControlados.count + 1)
        defer { fallosPermitidos = fallos }
        for parte in partes {
            guard let n = try? FormatoNET.convertToInt32(parte), (try? fallos.escribir(true, en: Int(n), "")) != nil else {
                return false
            }
        }
        usaControlGrupos = true
        return true
    }

    /// `PonerGrupo(n)` (:262-281): `0` reinicia a `"0"`; si no, añade `",n"`.
    @discardableResult
    public mutating func ponerGrupo(_ n: Int) throws(ErrorDominio) -> Bool {
        var texto = textoGruposControlados
        if n == 0 {
            texto = "0"
        } else {
            if !texto.isEmpty { texto += "," }
            texto += String(n)
        }
        return try ponerGruposControlados(texto)
    }

    public func contieneGrupo(_ n: Int) -> Bool { gruposControlados.contains(n) }
}

/// Un conjunto de controles con su presupuesto de fallos (el conjunto 0 son los «libres»: deben pasar todos).
public struct ControlConjuntosConfig: Sendable, Hashable {
    public private(set) var fallosPermitidos: [Bool]?
    public private(set) var ctrlGruposControlados: [Int] = []
    public private(set) var textoFallosPermitidos = ""
    public private(set) var textoCtrlGruposControlados = ""

    public init() {}

    /// `PonerCtrlGruposControlados` (:41-48): `RangosHelper.ObtenIntArray`.
    public mutating func ponerCtrlGruposControlados(_ texto: String) throws(ErrorDominio) {
        textoCtrlGruposControlados = texto
        ctrlGruposControlados = try RangosHelper.obtenIntArray(texto)
    }

    /// `PonerFallosPermitidos` (:34-39): `ObtenerBoolArrayFromTxt(texto, nº de controles)`; `""` ⇒ `nil`.
    public mutating func ponerFallosPermitidos(_ texto: String) throws(ErrorDominio) {
        textoFallosPermitidos = texto
        fallosPermitidos = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(texto, longitud: ctrlGruposControlados.count)
    }

    public func contieneConjunto(_ n: Int) -> Bool { ctrlGruposControlados.contains(n) }
}
