import Free1X2Base

// C#: el estado configurable de Analizador.cs, ControladorGrupos.cs, GrupoPartidos.cs y ControladorIfThen.cs (01 §2.2-2.8,
// §2.16-2.18): lo que guarda un `.comb` y edita la pantalla de inicio (ESPEC_11 §4.2).

/// Pronósticos, grupos, controles e If-Then de una combinación.
public struct Combinacion: Sendable, Hashable {
    public let numeroPartidos: Int
    /// `Pronosticos`: `nil` hasta el primer `setPronostico`; entonces N elementos `nil` (`Analizador.cs:439-443`).
    public var pronosticos: [String?]?
    /// Archivo-filtro de columnas (`""` = modo pronóstico) y relleno de líneas cortas (`CompletarCon`).
    public var archivoColumnasBase = ""
    public var completarCon = ""
    public private(set) var grupos: [GrupoConfig] = []
    public var controlesGrupos: [ControlGruposConfig] = []
    public var controlesConjuntos: [ControlConjuntosConfig] = []
    public var usaControlConjuntos = false
    public var ifThen: IfThenConfig?

    /// `new Analizador()` (`Analizador.cs:57-82`) con `new ControladorGrupos()` (`ControladorGrupos.cs:38-62`): control 0 con
    /// el grupo 0, conjunto 0 = `{"0", "0"}` y el grupo base añadido con `AddGrupo`.
    public init(numeroPartidos: Int) {
        self.numeroPartidos = numeroPartidos
        var control = ControlGruposConfig()
        control.gruposControlados = [0]
        controlesGrupos = [control]
        var conjunto = ControlConjuntosConfig()
        // invariante: "0" es un entero válido y el conjunto tiene 1 control, así que ninguna de las dos llamadas lanza.
        try? conjunto.ponerCtrlGruposControlados("0")
        try? conjunto.ponerFallosPermitidos("0")
        controlesConjuntos = [conjunto]
        var base = GrupoConfig(numeroPartidos: numeroPartidos)
        base.esGrupoBase = true
        try? addGrupo(base)
    }

    /// `SetPronostico(i, texto)` (:436-445).
    public mutating func setPronostico(_ partido: Int, _ texto: String) throws(ErrorDominio) {
        var p = pronosticos ?? Array(repeating: nil, count: numeroPartidos)
        try p.escribir(texto, en: partido, "Analizador.cs:444")
        pronosticos = p
    }

    /// Asigna todos los pronósticos (lo que hace la UI desde el boleto).
    public mutating func ponerPronosticos(_ p: [String]) { pronosticos = p }

    /// `ControlesConjuntos = lista` (`ControladorGrupos.cs:298-309`): con más de uno activa el control de conjuntos.
    public mutating func asignarControlesConjuntos(_ lista: [ControlConjuntosConfig]) {
        controlesConjuntos = lista
        if lista.count > 1 { usaControlConjuntos = true }
    }

    // MARK: GrupoPartidos (`GrupoPartidos.cs:37-59`)

    /// `AddGrupo`: calcula la máscara, añade y lo pone en el control 0.
    public mutating func addGrupo(_ grupo: GrupoConfig) throws(ErrorDominio) {
        var g = grupo
        g.calcularMascara()
        grupos.append(g)
        let nuevo = grupos.count - 1
        try controlGrupoLibre { c throws(ErrorDominio) in try c.ponerGrupo(nuevo) }
    }

    /// `InsertarGrupo`: no calcula la máscara y pone `posicion` en el control 0 (`0` lo reinicia a `"0"`).
    public mutating func insertarGrupo(_ grupo: GrupoConfig, en posicion: Int) throws(ErrorDominio) {
        guard (0...grupos.count).contains(posicion) else { throw .fueraDeRangoLista("GrupoPartidos.cs:50") }
        grupos.insert(grupo, at: posicion)
        try controlGrupoLibre { c throws(ErrorDominio) in try c.ponerGrupo(posicion) }
    }

    /// `BorrarGrupo`: no reindexa los controles (lo hace la UI).
    public mutating func borrarGrupo(_ posicion: Int) throws(ErrorDominio) {
        guard grupos.indices.contains(posicion) else {
            throw .fueraDeRangoLista("GrupoPartidos.cs:58")
        }
        grupos.remove(at: posicion)
    }

    /// Edita un grupo en su sitio.
    public mutating func modificarGrupo(_ i: Int, _ cambio: (inout GrupoConfig) throws(ErrorDominio) -> Void) throws(ErrorDominio) {
        guard grupos.indices.contains(i) else {
            throw .fueraDeRangoLista("GrupoPartidos.cs:66")
        }
        try cambio(&grupos[i])
    }

    private mutating func controlGrupoLibre(_ cambio: (inout ControlGruposConfig) throws(ErrorDominio) -> Void) throws(ErrorDominio) {
        guard !controlesGrupos.isEmpty else {
            throw .fueraDeRangoLista("GrupoPartidos.cs:44")
        }
        try cambio(&controlesGrupos[0])
    }

    /// `HayDatos` (`Analizador.cs:463-479`).
    public var hayDatos: Bool {
        get throws(ErrorDominio) {
            if !archivoColumnasBase.isEmpty || controlesGrupos.count > 1 || grupos.count > 1 { return true }
            if grupos.first?.condiciones.hayActivas == true { return true }
            if let ifThen, !ifThen.esVacio { return true }
            guard let pronosticos else { throw .referenciaNula("Analizador.cs:474") }
            return pronosticos.contains { $0 != "1,X,2" }
        }
    }
}

/// `ControladorIfThen`: condiciones y grupos «Si… entonces…» con el rango de cuántos deben cumplirse.
public struct IfThenConfig: Sendable, Hashable {
    public var condiciones: [CondicionIfThenConfig] = []
    public var grupos: [GrupoIfThenConfig] = []
    public private(set) var rangoAciertoCondiciones: String?
    public private(set) var rangoAciertoGrupos: String?
    public private(set) var aciertoCondiciones: [Bool]?
    public private(set) var aciertoGrupos: [Bool]?
    public var activo = true

    public init() {}

    public var esVacio: Bool { condiciones.isEmpty && grupos.isEmpty }

    /// `RangoAciertoCondiciones = s` (:57-72): el array mide `condiciones.count + 1` **ahora** [PRESERVAR].
    public mutating func ponerRangoAciertoCondiciones(_ s: String) throws(ErrorDominio) {
        rangoAciertoCondiciones = s
        aciertoCondiciones = try Self.rango(s, condiciones.count, "ControladorIfThen.cs:68")
    }

    public mutating func ponerRangoAciertoGrupos(_ s: String) throws(ErrorDominio) {
        rangoAciertoGrupos = s
        aciertoGrupos = try Self.rango(s, grupos.count, "ControladorIfThen.cs:85")
    }

    private static func rango(_ s: String, _ n: Int, _ origen: String) throws(ErrorDominio) -> [Bool] {
        let valores = try RangosHelper.obtenIntArray(s)
        var a = Array(repeating: false, count: n + 1)
        for v in valores { try a.escribir(true, en: v, origen) }
        return a
    }
}

/// `CondicionIfThen`: «si `condIf` entonces `condThen`», dos filtros descritos como texto.
public struct CondicionIfThenConfig: Sendable, Hashable {
    /// `CondIf = s` también fija `noIf` si el texto contiene `(NO)` en cualquier posición (:57-68).
    public var condIf = "" {
        didSet { noIf = condIf.contains("(NO)") }
    }
    public var condThen = "" {
        didSet { noThen = condThen.contains("(NO)") }
    }
    public var noIf = false
    public var noThen = false

    public init() {}
}

/// `GrupoIfThen`: «si el grupo `numGrupoIf` se cumple entonces el grupo `numGrupoThen`».
public struct GrupoIfThenConfig: Sendable, Hashable {
    public var numGrupoIf = 0
    public var numGrupoThen = 0
    public var noIf = false
    public var noThen = false

    public init() {}
}
