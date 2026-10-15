import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/Grupo.cs, ToleranciaFiltros.cs y ControladorTol.cs (01 §2.4, §2.9-2.10).

/// Lee las líneas de un fichero (`LeerTodasCols(false)`: `Trim().ToUpper()` por línea) cuando el motor lo necesita, para
/// conservar el momento del error de E/S del C# (el filtro parcial se carga en la primera columna que llega a él).
public typealias LectorLineas = @Sendable (String) throws(ErrorDominio) -> [String]

/// Lo que los filtros leen del entorno al construirse: N, banderas de análisis, cultura de los `Convert.ToDouble` y
/// puntos CP (`InicializaPuntosCP` en cada cálculo, 04 §0.4).
struct ContextoFiltros {
    let numeroPartidos: Int
    let analisis: OpcionesAnalisis
    let cultura: Cultura
    let puntosCP: PuntosCP
}

/// `ToleranciaFiltros` en ejecución: acumula coincidencias de letras.
final class Tolerancia {
    let letras: String
    let aciertosPermitidos: [Bool]?
    var acumulados = 0

    init(_ c: ToleranciaConfig) {
        letras = c.letrasTol
        aciertosPermitidos = c.aciertosPermitidos
    }

    /// `CumpleTolerancia` (:43-59).
    func cumple() throws(ErrorDominio) -> Bool {
        guard let a = aciertosPermitidos else { throw .referenciaNula("ToleranciaFiltros.cs:46") }
        return acumulados < a.count && a[acumulados]
    }
}

/// Un grupo en ejecución: sus 14 filtros en el orden fijo, tolerancias, filtro parcial y memo de la columna.
final class Grupo {
    var esGrupoBase: Bool
    /// `EsGrupoBase = true` (desde `ControlGrupos` en el análisis) también lo pone a `true` (:565-578).
    var todosPartidosActivos: Bool
    let mascara: Columna
    let partidosActivos: [Bool]
    let filtros: [any FiltroColumna]
    /// `filtros` con `activa`, en el mismo orden: la ruta A y las tolerancias solo miran estos.
    /// ponytail: llamada dinámica por filtro activo; si el benchmark lo pide, propiedades tipadas (ESPEC_11 §10.1).
    private let activos: [any FiltroColumna]
    private let tolerancias: [Tolerancia]
    private let fallosTolerancias: String
    private let arrayFallosTolerancias: [Bool]?
    private let archivoFiltro: String
    private let leerLineas: LectorLineas
    private var bits: Set<Columna>?
    var reCalcular = true
    private var validoMemoria = true

    init(_ g: GrupoConfig, contexto x: ContextoFiltros, leerLineas: @escaping LectorLineas) {
        esGrupoBase = g.esGrupoBase
        todosPartidosActivos = g.todosPartidosActivos
        mascara = g.mascara
        partidosActivos = g.partidosActivos
        let c = g.condiciones
        // Orden fijo de `Grupo.filtros` (:94-134).
        let a = x.analisis
        filtros = [
            FiltroNoVariantes(c.noVariantes, analisisActivo: a.activo(.noVariantes)),
            FiltroValoracionSignos(c.valoracionSignos, analisisActivo: a.activo(.valoracionSignos), cultura: x.cultura),
            FiltroSignosSeguidos(c.signosSeguidos, analisisActivo: a.activo(.signosSeguidos)),
            FiltroInterrupciones(c.interrupciones, analisisActivo: a.activo(.interrupciones)),
            FiltroDibujos(c.dibujos, analisisActivo: a.activo(.dibujos)),
            FiltroPesosNumericos(c.pesosNumericos, analisisActivo: a.activo(.pesosNumericos)),
            FiltroDistancias(c.distancias, analisisActivo: a.activo(.distancias)),
            FiltroGruposEquipos(c.gruposEquipos, analisisActivo: a.activo(.gruposEquipos)),
            FiltroContactos(c.contactos, analisisActivo: a.activo(.contactos)),
            FiltroFormatosSignos(c.formatosSignos, analisisActivo: a.activo(.formatosSignos)),
            FiltroColProbables(
                c.colProbables, analisisActivo: a.activo(.colProbables), numeroPartidos: x.numeroPartidos,
                puntos: x.puntosCP),
            FiltroFormatos123(c.formatos123, analisisActivo: a.activo(.formatos123), numeroPartidos: x.numeroPartidos),
            FiltroSimetrias(c.simetrias, analisisActivo: a.activo(.simetrias)),
            FiltroDiferencias(c.diferencias, analisisActivo: a.activo(.diferencias)),
        ]
        activos = filtros.filter(\.activa)
        tolerancias = g.tolerancias.tolerancias.map(Tolerancia.init)
        fallosTolerancias = g.tolerancias.fallosPermitidos
        arrayFallosTolerancias = g.tolerancias.arrayFallos
        archivoFiltro = g.archivoFiltro
        self.leerLineas = leerLineas
    }

    /// `GetFiltro(nombre)` (:415-431): si ninguno coincide, el último (`Diferencias`).
    func filtro(_ nombre: String) -> (any FiltroColumna)? {
        filtros.first { $0.tipo.rawValue == nombre } ?? filtros.last
    }

    /// `AnalizaColumna(long)` (:165-225): ruta A/B.
    func analiza(_ columna: Columna) throws(ErrorDominio) -> Bool {
        guard reCalcular else { return validoMemoria }
        let col = todosPartidosActivos ? columna : columna & mascara
        var valida = true
        for f in activos where !(try f.analizar(col)) {
            valida = false
            break
        }
        if valida {
            valida = try toleranciasValidas()
            if valida && !archivoFiltro.isEmpty {
                if bits == nil {
                    var b = Set<Columna>()
                    for linea in try leerLineas(archivoFiltro) {
                        b.insert(try UtilColumnas.convStrToLong(linea, marcados: partidosActivos))
                    }
                    bits = b
                }
                valida = bits?.contains(col) == true
            }
        }
        validoMemoria = valida
        reCalcular = false
        return valida
    }

    /// `FallosPermitidos == ""` elige la rama (:192-201).
    func toleranciasValidas() throws(ErrorDominio) -> Bool {
        if fallosTolerancias.isEmpty {
            // `SonTolGrupoValidas` (:367-408): atajo P-09 sin tolerancias.
            if tolerancias.isEmpty { return true }
            try acumular()
            for t in tolerancias where !(try t.cumple()) { return false }
            return true
        }
        // `SonTolGrupoValidasConFallos` (:331-365).
        try acumular()
        var falladas = 0
        for t in tolerancias where !(try t.cumple()) { falladas += 1 }
        guard let a = arrayFallosTolerancias else { throw .referenciaNula("ControladorTol.cs:113") }
        return falladas < a.count && a[falladas]
    }

    private func acumular() throws(ErrorDominio) {
        for t in tolerancias { t.acumulados = 0 }
        for f in activos {
            for t in tolerancias { t.acumulados += try f.obtenNoAciertosTolerancias(t.letras) }
        }
    }
}
