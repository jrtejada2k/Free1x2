import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/ControladorGrupos.cs y ControlGrupos.cs (01 §2.6-2.7).

/// `ControlGrupos` en ejecución.
final class ControlGrupos {
    let usaControlGrupos: Bool
    let fallosPermitidos: [Bool]?
    let gruposControlados: [Int]
    private var reCalcular = true
    private var validoMemoria = true

    init(_ c: ControlGruposConfig) {
        usaControlGrupos = c.usaControlGrupos
        fallosPermitidos = c.fallosPermitidos
        gruposControlados = c.gruposControlados
    }

    /// `AnalizaColumna(long)` (:43-50).
    func analiza(_ columna: Columna, _ grupos: [Grupo]) throws(ErrorDominio) -> Bool {
        if usaControlGrupos {
            // `AnalizaGruposConControlFallos` (:61-90): se evalúan todos, sin corte.
            guard reCalcular else { return validoMemoria }
            var fallos = 0
            for g in gruposControlados where !(try grupos.leerLista(g, "ControlGrupos.cs:69").analiza(columna)) { fallos += 1 }
            guard let a = fallosPermitidos else { throw .referenciaNula("ControlGrupos.cs:78") }
            validoMemoria = try a.leer(fallos, "ControlGrupos.cs:78")
            reCalcular = false
            return validoMemoria
        }
        // `AnalizaGruposSinControlFallos` (:92-103): corte en el primero que falla; sin memo propio.
        for g in gruposControlados where !(try grupos.leerLista(g, "ControlGrupos.cs:97").analiza(columna)) { return false }
        return true
    }

    /// `AnalizaColumna(long, ContenedorAnalisisGlobal)` (:52-59, 105-161): rutas C/D. Con fallos permitidos evalúa sus
    /// grupos **sin** análisis; sin ellos, con análisis y corte.
    func analiza(_ columna: Columna, _ grupos: [Grupo], _ r: RegistroAnalisis) throws(ErrorDominio) -> Bool {
        let o = "ControlGrupos.cs:105-161"
        if usaControlGrupos {
            guard reCalcular else { return validoMemoria }
            try grupos.leerLista(0, o).marcarBase()
            var fallos = 0
            for g in gruposControlados where !(try grupos.leerLista(g, o).analiza(columna)) { fallos += 1 }
            guard let a = fallosPermitidos else { throw .referenciaNula(o) }
            let valida = try a.leer(fallos, o)
            if valida {
                try r.global.columnasPorFallosDeGrupos.incrementar(fallos, o)
            } else {
                r.vaciarInformacion()
            }
            validoMemoria = valida
            reCalcular = false
            return valida
        }
        try grupos.leerLista(0, o).marcarBase()
        for g in gruposControlados where !(try grupos.leerLista(g, o).analiza(columna, r)) { return false }
        return true
    }

    /// `RecalcularControlGrupos` (:283-291).
    func recalcular(_ grupos: [Grupo]) throws(ErrorDominio) {
        reCalcular = true
        for g in gruposControlados { try grupos.leerLista(g, "ControlGrupos.cs:288").reCalcular = true }
    }
}

/// `ControladorGrupos` en ejecución: raíz de la validación por grupos.
final class ControladorGrupos {
    let grupos: [Grupo]
    let controles: [ControlGrupos]
    let conjuntos: [ControlConjuntosConfig]
    let usaControlConjuntos: Bool
    /// Los conjuntos desplegados en arrays propios (`map` copia): leer los de la combinación en cada columna retenía sus
    /// arrays, compartidos por todos los hilos de Generar en paralelo, y los hilos se estorbaban (DM-21).
    private let gruposDeConjuntos: [[Int]]
    private let fallosDeConjuntos: [[Bool]?]
    private var reCalcular = true
    private var validoMemoria = true

    init(grupos: [Grupo], _ c: Combinacion) {
        self.grupos = grupos
        controles = c.controlesGrupos.map(ControlGrupos.init)
        conjuntos = c.controlesConjuntos
        usaControlConjuntos = c.usaControlConjuntos
        gruposDeConjuntos = c.controlesConjuntos.map { $0.ctrlGruposControlados.map { $0 } }
        fallosDeConjuntos = c.controlesConjuntos.map { $0.fallosPermitidos?.map { $0 } }
    }

    /// `RecalcularControladorGrupos` (:270-278).
    func recalcular() throws(ErrorDominio) {
        reCalcular = true
        for c in controles { try c.recalcular(grupos) }
    }

    /// `AnalizaColumna(long)` (:64-81): el memo se marca **antes** de evaluar.
    func analiza(_ columna: Columna) throws(ErrorDominio) -> Bool {
        guard reCalcular else { return validoMemoria }
        reCalcular = false
        validoMemoria = usaControlConjuntos ? try conConjuntos(columna) : try sinConjuntos(columna)
        return validoMemoria
    }

    /// `AnalizaColumna(long, ContenedorAnalisisGlobal)` (:155-179): el memo se marca **después** de evaluar.
    func analiza(_ columna: Columna, _ r: RegistroAnalisis) throws(ErrorDominio) -> Bool {
        guard reCalcular else { return validoMemoria }
        let valida = usaControlConjuntos ? try conConjuntos(columna, r) : try sinConjuntos(columna, r)
        validoMemoria = valida
        reCalcular = false
        return valida
    }

    private func sinConjuntos(_ columna: Columna, _ r: RegistroAnalisis) throws(ErrorDominio) -> Bool {
        for c in controles where !(try c.analiza(columna, grupos, r)) { return false }
        return true
    }

    /// (:201-269): antes de cada libre salvo el primero se vacía lo registrado; los demás conjuntos evalúan sin análisis
    /// y cuentan las columnas por nº de controles fallados.
    private func conConjuntos(_ columna: Columna, _ r: RegistroAnalisis) throws(ErrorDominio) -> Bool {
        let o = "ControladorGrupos.cs:201-269"
        let libres = try conjuntos.leerLista(0, o)
        for (j, i) in libres.ctrlGruposControlados.enumerated() {
            let c = try control(i)
            if j > 0 { r.vaciarInformacion() }
            if !(try c.analiza(columna, grupos, r)) { return false }
        }
        for conjunto in conjuntos.dropFirst() {
            var fallados = 0
            for i in conjunto.ctrlGruposControlados where !(try control(i).analiza(columna, grupos)) { fallados += 1 }
            guard let fallos = conjunto.fallosPermitidos else { throw .referenciaNula(o) }
            if fallos.count < fallados + 1 || !fallos[fallados] { return false }
            try r.global.columnasPorFallosDeConjuntos.incrementar(fallados, o)
            try r.global.columnasPorFallosDeGrupos.incrementar(fallados, o)
        }
        return true
    }

    private func sinConjuntos(_ columna: Columna) throws(ErrorDominio) -> Bool {
        for c in controles where !(try c.analiza(columna, grupos)) { return false }
        return true
    }

    /// `AnalizaColumnaConConjuntos` (:95-153): los «libres» del conjunto 0 deben pasar; en los demás se cuentan los
    /// controles que fallan y se mira el presupuesto.
    private func conConjuntos(_ columna: Columna) throws(ErrorDominio) -> Bool {
        let libres = try gruposDeConjuntos.leerLista(0, "ControladorGrupos.cs:101")
        for i in libres where !(try control(i).analiza(columna, grupos)) { return false }
        for k in gruposDeConjuntos.indices.dropFirst() {
            var fallados = 0
            for i in gruposDeConjuntos[k] where !(try control(i).analiza(columna, grupos)) { fallados += 1 }
            guard let fallos = fallosDeConjuntos[k] else { throw .referenciaNula("ControladorGrupos.cs:144") }
            if fallos.count < fallados + 1 || !fallos[fallados] { return false }
        }
        return true
    }

    private func control(_ i: Int) throws(ErrorDominio) -> ControlGrupos {
        try controles.leerLista(i, "ControladorGrupos.cs:110")
    }
}
