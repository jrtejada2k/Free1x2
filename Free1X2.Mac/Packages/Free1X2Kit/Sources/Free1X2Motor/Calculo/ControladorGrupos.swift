import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/ControladorGrupos.cs y ControlGrupos.cs (01 §2.6-2.7). Rutas A/B.

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
    private var reCalcular = true
    private var validoMemoria = true

    init(grupos: [Grupo], _ c: Combinacion) {
        self.grupos = grupos
        controles = c.controlesGrupos.map(ControlGrupos.init)
        conjuntos = c.controlesConjuntos
        usaControlConjuntos = c.usaControlConjuntos
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

    private func sinConjuntos(_ columna: Columna) throws(ErrorDominio) -> Bool {
        for c in controles where !(try c.analiza(columna, grupos)) { return false }
        return true
    }

    /// `AnalizaColumnaConConjuntos` (:95-153): los «libres» del conjunto 0 deben pasar; en los demás se cuentan los
    /// controles que fallan y se mira el presupuesto.
    private func conConjuntos(_ columna: Columna) throws(ErrorDominio) -> Bool {
        let libres = try conjuntos.leerLista(0, "ControladorGrupos.cs:101")
        for i in libres.ctrlGruposControlados where !(try control(i).analiza(columna, grupos)) { return false }
        for conjunto in conjuntos.dropFirst() {
            var fallados = 0
            for i in conjunto.ctrlGruposControlados where !(try control(i).analiza(columna, grupos)) { fallados += 1 }
            guard let fallos = conjunto.fallosPermitidos else { throw .referenciaNula("ControladorGrupos.cs:144") }
            if fallos.count < fallados + 1 || !fallos[fallados] { return false }
        }
        return true
    }

    private func control(_ i: Int) throws(ErrorDominio) -> ControlGrupos {
        try controles.leerLista(i, "ControladorGrupos.cs:110")
    }
}
