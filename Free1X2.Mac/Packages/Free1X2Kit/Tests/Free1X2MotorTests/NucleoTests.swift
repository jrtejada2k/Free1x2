import Free1X2Base
import Free1X2Motor
import Free1X2Pruebas
import Testing

/// ESPEC_17 GEN, GRP, TOL, CTL, IFT: núcleo del motor (rutas A/B) — suites `motor/nucleo` y `motor/flujos`.
@Suite("Núcleo del motor")
struct NucleoTests {
    struct Entrada: Decodable, Sendable {
        struct NoVar: Decodable, Sendable {
            var activa: Bool?
            var variantes, equis, doses: String
        }
        struct Grupo: Decodable, Sendable {
            var base: Bool?
            var partidos: String?
            var noVariantes: NoVar?
            var tolerancias: [Tol]?
            var fallosTolerancias: String?
            var colProbables: ColProbablesTests.CondJ?
        }
        struct Tol: Decodable, Sendable {
            var letras, aciertos: String
        }
        struct Control: Decodable, Sendable {
            var grupos: String
            var fallos: String?
        }
        struct Conjunto: Decodable, Sendable {
            var controles, fallos: String
        }
        struct IfThen: Decodable, Sendable {
            struct Cond: Decodable, Sendable {
                var si, entonces: String
            }
            struct Gr: Decodable, Sendable {
                var si, entonces: Int
                var noSi, noEntonces: Bool
            }
            var condiciones: [Cond]?
            var rangoCondiciones: String?
            var grupos: [Gr]?
            var rangoGrupos: String?
        }
        var pronosticos: [String]
        var lineas: [String]?
        var ruta: String
        var grupo0: Grupo?
        var grupos: [Grupo]?
        var controles: [Control]?
        var conjuntos: [Conjunto]?
        var ifThen: IfThen?
        /// Banderas de `OpcionesAnalisis` a `true` (rutas C/D).
        var analisis: [String]?
    }
    struct Salida: Decodable, Sendable {
        var colsAnalizadas, colsAceptadas, lineasFichero: Int?
        var analizadas, aceptadas: Int?
        var columnas: [String]?
        var valores: [String]?
        var sha256: String?
        var vx2: [[Int]]?
        var porFallosGrupos, porFallosConjuntos: [Int]?
    }

    static let n = 14

    /// Arma la combinación como lo hacen la UI y el cargador: grupos con `AddGrupo`, controles y conjuntos.
    static func combinacion(_ e: Entrada) throws -> Combinacion {
        var c = Combinacion(numeroPartidos: n)
        c.ponerPronosticos(e.pronosticos)
        if e.lineas != nil { c.archivoColumnasBase = "base.txt" }
        let grupos = e.grupos ?? [e.grupo0 ?? Entrada.Grupo(base: true)]
        for (i, g) in grupos.enumerated() {
            if i == 0 {
                try c.modificarGrupo(0) { (gc: inout GrupoConfig) throws(ErrorDominio) in try Self.configurar(&gc, g) }
            } else {
                var gc = GrupoConfig(numeroPartidos: n)
                if let p = g.partidos { try gc.ponerPartidosActivos(p) }
                try Self.configurar(&gc, g)
                try c.addGrupo(gc)
            }
        }
        if let controles = e.controles {
            c.controlesGrupos = try controles.map { e in
                var control = ControlGruposConfig()
                let grupos = try control.ponerGruposControlados(e.grupos)
                let fallos = try control.ponerFallosPermitidos(e.fallos ?? "")
                #expect(grupos && fallos)
                return control
            }
        }
        if let conjuntos = e.conjuntos {
            c.asignarControlesConjuntos(
                try conjuntos.map { e throws in
                    var conjunto = ControlConjuntosConfig()
                    try conjunto.ponerCtrlGruposControlados(e.controles)
                    try conjunto.ponerFallosPermitidos(e.fallos)
                    return conjunto
                })
        }
        if let it = e.ifThen {
            var cfg = IfThenConfig()
            for cond in it.condiciones ?? [] {
                var x = CondicionIfThenConfig()
                x.condIf = cond.si
                x.condThen = cond.entonces
                cfg.condiciones.append(x)
            }
            for gr in it.grupos ?? [] {
                var x = GrupoIfThenConfig()
                (x.numGrupoIf, x.numGrupoThen, x.noIf, x.noThen) = (gr.si, gr.entonces, gr.noSi, gr.noEntonces)
                cfg.grupos.append(x)
            }
            if let r = it.rangoCondiciones { try cfg.ponerRangoAciertoCondiciones(r) }
            if let r = it.rangoGrupos { try cfg.ponerRangoAciertoGrupos(r) }
            c.ifThen = cfg
        }
        return c
    }

    static func configurar(_ gc: inout GrupoConfig, _ g: Entrada.Grupo) throws(ErrorDominio) {
        if let cp = g.colProbables { gc.condiciones.colProbables = try ColProbablesTests.condicion(cp) }
        if let nv = g.noVariantes {
            gc.condiciones.noVariantes.activa = nv.activa ?? true
            try gc.condiciones.noVariantes.setNoVariantes(nv.variantes)
            try gc.condiciones.noVariantes.setNoEquis(nv.equis)
            try gc.condiciones.noVariantes.setNoDoses(nv.doses)
        }
        var tols = ControladorTolConfig()
        for t in g.tolerancias ?? [] {
            var tc = ToleranciaConfig()
            try tc.ponerAciertos(t.aciertos)
            tc.letrasTol = t.letras
            tols.tolerancias.append(tc)
        }
        try tols.ponerFallosPermitidos(g.fallosTolerancias ?? "")
        gc.tolerancias = tols
    }

    static func ejecutar(_ e: Entrada) throws -> (ResultadoCalculo, [Columna]) {
        let lineas = e.lineas ?? []
        var aceptadas: [Columna] = []
        let r = try MotorCalculo.calcular(
            try combinacion(e), configuracion: .porDefecto, leerLineas: { _ in lineas },
            aceptada: e.ruta == "B" ? { aceptadas.append($0) } : nil)
        return (r, aceptadas)
    }

    @Test func nucleo() throws {
        for caso in try Golden.casos("motor/nucleo", Entrada.self, Salida.self) {
            let (r, cols) = try Self.ejecutar(caso.entrada)
            let s = try #require(caso.salida)
            #expect(r == ResultadoCalculo(analizadas: s.analizadas ?? -1, aceptadas: s.aceptadas ?? -1), "\(caso.id)")
            let textos = try cols.map { c throws(ErrorDominio) in try UtilColumnas.convLongToStr(c) }
            if let esperadas = s.columnas { #expect(textos == esperadas, "\(caso.id)") }
            if let valores = s.valores { #expect(cols.map(String.init) == valores, "\(caso.id)") }
            if let sha = s.sha256 { #expect(Golden.sha256(textos.joined(separator: "\n")) == sha, "\(caso.id)") }
        }
    }

    /// ESPEC_17 ANA: rutas C/D (`motor/analisis`).
    @Test func analisis() throws {
        for caso in try Golden.casos("motor/analisis", Entrada.self, Salida.self) {
            let e = caso.entrada
            var config = Configuracion.porDefecto
            for bandera in e.analisis ?? [] {
                switch bandera {
                case "vx2": config.analisis.vx2 = true
                default: Issue.record("bandera desconocida \(bandera)")
                }
            }
            let lineas = e.lineas ?? []
            let r = try MotorCalculo.analizar(
                try Self.combinacion(e), configuracion: config, externo: e.ruta == "D", leerLineas: { _ in lineas })
            let s = try #require(caso.salida)
            #expect(r.calculo == ResultadoCalculo(analizadas: s.analizadas ?? -1, aceptadas: s.aceptadas ?? -1), "\(caso.id)")
            if let v = s.vx2 { #expect(r.contenedor.analisisGrupos.vx2 == v, "\(caso.id)") }
            if let v = s.porFallosGrupos { #expect(r.contenedor.columnasPorFallosDeGrupos == v, "\(caso.id)") }
            if let v = s.porFallosConjuntos { #expect(r.contenedor.columnasPorFallosDeConjuntos == v, "\(caso.id)") }
        }
    }

    @Test func flujosHeredados() throws {
        for caso in try Golden.casos("motor/flujos", componente: "Analizador", Entrada.self, Salida.self)
        where caso.id.hasPrefix("CP-GEN") || caso.id.hasPrefix("CP-CP") {
            let (r, cols) = try Self.ejecutar(caso.entrada)
            let s = try #require(caso.salida)
            if let n = s.colsAnalizadas { #expect(r.analizadas == n, "\(caso.id)") }
            if let n = s.colsAceptadas { #expect(r.aceptadas == n, "\(caso.id)") }
            if let n = s.lineasFichero { #expect(cols.count == n, "\(caso.id)") }
        }
    }

    @Test func columnaInicialYErrores() throws {
        var c = Combinacion(numeroPartidos: 14)
        let e = #expect(throws: ErrorDominio.self) {
            try MotorCalculo.calcular(c, configuracion: .porDefecto, leerLineas: { _ in [] })
        }
        #expect(e?.excepcion == .referenciaNula, "sin pronósticos ⇒ NullReferenceException")
        try c.setPronostico(0, "1")
        let e2 = #expect(throws: ErrorDominio.self) {
            try MotorCalculo.calcular(c, configuracion: .porDefecto, leerLineas: { _ in [] })
        }
        #expect(e2?.excepcion == .referenciaNula, "un pronóstico nil ⇒ NullReferenceException")
        #expect(try c.hayDatos)
    }
}
