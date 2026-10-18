import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/GrupoEquipos.cs, RelacionGE1.cs y FiltroGruposEquipos.cs (03 §14-15; 04 §11-12).

/// Un grupo de equipos: en qué partidos juega el equipo y sus límites de puntos, victorias, empates y derrotas.
public struct GrupoEquiposConfig: Sendable, Hashable {
    /// Por partido: `1` local, `2` visitante, `3` ambos, `0` ninguno, `*` sin dato. Al asignar se recalcula
    /// `longPronosticos`, como hacen todos los flujos reales tras fijarlo (03 §15 P1).
    public var pronosticos: String {
        didSet { longPronosticos = Self.codificar(pronosticos) }
    }
    /// El campo que lee el análisis; 0 hasta asignar `pronosticos`.
    public private(set) var longPronosticos: Int64 = 0
    public private(set) var sumaPuntos = ConjuntoRangos()
    public private(set) var victorias = ConjuntoRangos()
    public private(set) var empates = ConjuntoRangos()
    public private(set) var derrotas = ConjuntoRangos()

    /// `char[N]` de `'\0'`.
    public init(numeroPartidos n: Int) {
        pronosticos = String(repeating: "\0", count: n)
    }

    /// `LongPronosticos` (:303-325): `"*213***0".IndexOf(c)` por partido; otro carácter ⇒ `(uint)-1` (B2).
    static func codificar(_ s: String) -> Int64 {
        let codigos: [UInt16: Int64] = [0x2A: 0, 0x32: 1, 0x31: 2, 0x33: 3, 0x30: 7]
        var r: Int64 = 0
        for u in s.utf16.reversed() { r = (r &<< 3) | (codigos[u] ?? 0xFFFF_FFFF) }
        return r
    }

    public mutating func ponerSumaPuntos(_ s: String) throws(ErrorDominio) { try sumaPuntos.poner(s) }
    public mutating func ponerVictorias(_ s: String) throws(ErrorDominio) { try victorias.poner(s) }
    public mutating func ponerEmpates(_ s: String) throws(ErrorDominio) { try empates.poner(s) }
    public mutating func ponerDerrotas(_ s: String) throws(ErrorDominio) { try derrotas.poner(s) }
}

/// Relación I entre grupos de equipos (04 §11): sumas de sus contadores.
public struct RelacionGE1Config: Sendable, Hashable {
    public private(set) var gruposEquipos = ""
    /// Números de grupo 1-based (`ObtenIntArray`, sin deduplicar).
    public private(set) var geRelacionados: [Int]?
    public private(set) var sumaVictorias = ConjuntoRangos()
    public private(set) var sumaEmpates = ConjuntoRangos()
    public private(set) var sumaDerrotas = ConjuntoRangos()
    public private(set) var sumaPuntos = ConjuntoRangos()

    public init() {}

    public mutating func ponerGruposEquipos(_ s: String) throws(ErrorDominio) {
        gruposEquipos = s
        geRelacionados = try RangosHelper.obtenIntArray(s)
    }

    public mutating func ponerSumaVictorias(_ s: String) throws(ErrorDominio) { try sumaVictorias.poner(s) }
    public mutating func ponerSumaEmpates(_ s: String) throws(ErrorDominio) { try sumaEmpates.poner(s) }
    public mutating func ponerSumaDerrotas(_ s: String) throws(ErrorDominio) { try sumaDerrotas.poner(s) }
    public mutating func ponerSumaPuntos(_ s: String) throws(ErrorDominio) { try sumaPuntos.poner(s) }
}

/// Condición «Grupos de equipos» (03 §14).
public struct CondicionGruposEquipos: Sendable, Hashable {
    public var activa = false
    public var contieneDatos = false
    public var grupos: [GrupoEquiposConfig] = []
    public var relaciones: [RelacionGE1Config] = []

    public init() {}

    /// `GruposEquipos = lista` (:85-105).
    public mutating func ponerGrupos(_ lista: [GrupoEquiposConfig]) {
        grupos = lista
        contieneDatos = !lista.isEmpty
    }
}
