// C#: Free1X2.Domain/VariablesGlobales.cs (01 §3) — sustituye al estado global por un valor inyectado.

/// Configuración de la aplicación (`parametros.free1x2`), leída una vez al arrancar (DP-04).
public struct Configuracion: Sendable, Hashable {
    public var boleto = ConfigBoleto()
    public var puntosCP = PuntosCP(fijos: 10, dobles: 15, triples: 20)
    public var lae = ConfigLAE()
    public var analisis = OpcionesAnalisis()
    public var interfaz = PreferenciasInterfaz()

    /// `SetDefaultValues` (`VariablesGlobales.cs:149-175`).
    public static let porDefecto = Configuracion()

    public init() {}
}

public struct ConfigBoleto: Sendable, Hashable {
    public var numeroPartidos = 14
    public var separador = ["1", "X", "2"]
    public var desplazamiento = 0

    public init() {}
}

/// Puntos de las columnas probables: el motor los relee del fichero en cada ejecución (01 §2.2, DP-04).
public struct PuntosCP: Sendable, Hashable {
    public var fijos: Int
    public var dobles: Int
    public var triples: Int

    public init(fijos: Int, dobles: Int, triples: Int) {
        self.fijos = fijos
        self.dobles = dobles
        self.triples = triples
    }
}

/// LAE: valores leídos como `float` con `es-ES` y ensanchados a `Double` (01 §3.2.3).
public struct ConfigLAE: Sendable, Hashable {
    public var precioApuesta = 0.75
    public var porcentaje14 = 0.55
    public var recaudacion = 1_000_000.0
    public var moneda = "€"

    public init() {}
}

/// Las 20 banderas `Analizar*` (01 §3.1 c): deciden el `analisisActivo` de cada filtro.
public struct OpcionesAnalisis: Sendable, Hashable {
    public var todo = true
    public var vx2 = false
    public var seguidos = false
    public var dibujos = false
    public var interrupciones = false
    public var formatos = false
    public var formatos123 = false
    public var contactos = false
    public var figurasContactos = false
    public var figurasV1X2 = false
    public var figurasPesos = false
    public var simetrias = false
    /// `AnalizarSimetriasII`, que se lee del atributo `aDiferencias`.
    public var diferencias = false
    public var gruposEquipos = false
    public var pesos = false
    public var valoracion = false
    public var distancias = false
    public var cps = false
    public var controlGrupos = false
    public var controlConjuntos = false

    public init() {}

    /// `AnalisisActivo` de cada filtro (01 §4.2).
    public func activo(_ tipo: TipoFiltro) -> Bool {
        switch tipo {
        case .noVariantes: vx2
        case .valoracionSignos: valoracion
        case .signosSeguidos: seguidos
        case .interrupciones: interrupciones
        case .dibujos: dibujos
        case .pesosNumericos: pesos
        case .distancias: distancias
        case .gruposEquipos: gruposEquipos
        case .contactos: contactos
        case .formatosSignos: formatos
        case .colProbables: cps
        case .formatos123: formatos123
        case .simetrias: simetrias
        case .diferencias: diferencias
        }
    }
}

public struct PreferenciasInterfaz: Sendable, Hashable {
    public var barras = [true, true, true, true, true, true]
    public var actualizarAlInicio = false
    public var confirmarAlSalir = true
    public var idioma = "es-ES"

    public init() {}
}
