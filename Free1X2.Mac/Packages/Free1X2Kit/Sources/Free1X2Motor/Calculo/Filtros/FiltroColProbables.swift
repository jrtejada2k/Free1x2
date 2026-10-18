import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/ColumnaProbable.cs, CPControlFallos.cs, ControladorCPControlFallos.cs y
// FiltroColProbables.cs (04 §0-§4).

/// Una columna probable en ejecución. Clase compartida (S3): las relaciones leen los contadores de la última `analizar`.
final class ColumnaProbable {
    let c: ColumnaProbableConfig
    let numeroPartidos: Int
    /// Puntos fijo/doble/triple: `Analizador` los fija en cada cálculo con los de la configuración (04 §0.4).
    let puntos: PuntosCP
    private(set) var noAC = 0
    private(set) var noACS = 0
    private(set) var noFS = 0
    private(set) var noPuntos = 0
    private(set) var noFallosActual = 0
    private var falloAC = false
    private var falloACS = false
    private var falloFS = false
    private let o = "ColumnaProbable.cs:130-360"

    init(_ c: ColumnaProbableConfig, numeroPartidos: Int, puntos: PuntosCP) {
        self.c = c
        self.numeroPartidos = numeroPartidos
        self.puntos = puntos
    }

    /// `Calcular` (:271-323): los partidos con pronóstico 0 no rompen ni reinician rachas (P1).
    private func calcular(_ columna: Columna) throws(ErrorDominio) {
        (noAC, noACS, noFS, noPuntos) = (0, 0, 0, 0)
        (falloAC, falloACS, falloFS) = (false, false, false)
        var col = columna
        var (seguidosA, seguidosF) = (0, 0)
        for i in 0..<numeroPartidos {
            let p = try c.pronosticos.leer(i, o)
            if p != 0 {
                if Int64(p) & (col & 7) > 0 {
                    noAC += 1
                    seguidosA += 1
                    seguidosF = 0
                    noPuntos += p == 6 || p == 5 || p == 3 ? puntos.dobles : p == 7 ? puntos.triples : puntos.fijos
                } else {
                    seguidosF += 1
                    seguidosA = 0
                }
                noACS = max(noACS, seguidosA)
                noFS = max(noFS, seguidosF)
            }
            col >>= 3
        }
    }

    /// `Analizar(long)` y `CumpleCondiciones` (:130-148): puntos antes que tolerancias (P3); con tolerancia local el
    /// veredicto es solo el de ella (P2).
    func analizar(_ col: Columna) throws(ErrorDominio) -> Bool {
        try calcular(col)
        let cumple = try esColumnaValida()
        if c.puntos.activo && !c.puntos.permite(noPuntos) { return false }
        if c.toleranciaLocalActiva { return try toleranciasValidas() }
        return cumple
    }

    /// `esColumnaValida` (:161-183): los tres, sin cortocircuito (P4).
    private func esColumnaValida() throws(ErrorDominio) -> Bool {
        var ok = true
        if !(try c.aciertos.permite(noAC, o)) {
            ok = false
            falloAC = true
        }
        if !(try c.aciertosSeguidos.permite(noACS, o)) {
            ok = false
            falloACS = true
        }
        if !(try c.fallosSeguidos.permite(noFS, o)) {
            ok = false
            falloFS = true
        }
        return ok
    }

    /// `SonToleranciasValidas` (:215-257).
    private func toleranciasValidas() throws(ErrorDominio) -> Bool {
        noFallosActual = 0
        if falloAC {
            guard try c.acTolLocal.leer(noAC, o) else { return false }
            noFallosActual += 1
        }
        if falloACS {
            guard try c.acsTolLocal.leer(noACS, o) else { return false }
            noFallosActual += 1
        }
        if falloFS {
            guard try c.fsTolLocal.leer(noFS, o) else { return false }
            noFallosActual += 1
        }
        return try c.tolLocales.leer(noFallosActual, o)
    }

    /// `Analizar(long, numCol)` (:150-213): concatena sin cortar; ignora las tolerancias locales.
    func texto(_ col: Columna, _ numCol: Int) throws(ErrorDominio) -> String {
        try calcular(col)
        var t = ""
        let familias = [
            (c.aciertos, noAC, "Aciertos"), (c.aciertosSeguidos, noACS, "Aciertos Seguidos"),
            (c.fallosSeguidos, noFS, "Fallos Seguidos"),
        ]
        for (i, (v, n, nombre)) in familias.enumerated() {
            if !(try v.permite(n, o)) {
                t += "Fallo en \(nombre) en la columna \(numCol)  (\(n))#"
                switch i {
                case 0: falloAC = true
                case 1: falloACS = true
                default: falloFS = true
                }
            }
            if try v.letra(n, o) != ValoresConTolerancia.sinLetra {
                t += "!Tolerancias en \(nombre) en la columna \(numCol)  (\(n))#"
            }
        }
        if c.puntos.activo && !c.puntos.permite(noPuntos) {
            t += "Fallo en Puntuación de la columna \(numCol)  (\(noPuntos))#"
        }
        return t
    }

    /// `ObtenNoAciertosTolerancias` (:337-360): cada carácter contra las tres familias; los repetidos cuentan doble.
    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int {
        var n = 0
        for l in letras.utf16 {
            if try c.aciertos.letra(noAC, o) == l { n += 1 }
            if try c.aciertosSeguidos.letra(noACS, o) == l { n += 1 }
            if try c.fallosSeguidos.letra(noFS, o) == l { n += 1 }
        }
        return n
    }
}

/// `CPControlFallos` en ejecución (04 §3).
final class CPControlFallos {
    let c: CPControlFallosConfig
    let cps: [ColumnaProbable]
    private(set) var toleranciasAcumuladas = 0

    init(_ c: CPControlFallosConfig, cps: [ColumnaProbable]) {
        self.c = c
        self.cps = cps
    }

    /// `Analiza` (:41-64): usa las letras globales de la CP, no las tolerancias locales.
    func analiza() throws(ErrorDominio) -> Bool {
        let o = "CPControlFallos.cs:41-64"
        toleranciasAcumuladas = 0
        guard let columnas = c.columnasRelacionadas else { throw .referenciaNula(o) }
        for n in columnas {
            toleranciasAcumuladas += try cps.leerLista(n - 1, o).obtenNoAciertosTolerancias(c.tolerancias)
        }
        guard let a = c.aciertosPermitidos else { throw .referenciaNula(o) }
        return a.indices.contains(toleranciasAcumuladas) && a[toleranciasAcumuladas]
    }
}

/// `FiltroColProbables` (04 §2): las CPs, su control de fallos y las relaciones I, II y III.
final class FiltroColProbables: FiltroColumna {
    let condicion: CondicionColProbables
    let analisisActivo: Bool
    let cps: [ColumnaProbable]
    let controles: [CPControlFallos]
    let relaciones1: [RelacionCP1]
    let relaciones2: [RelacionCP2]
    let relaciones3: [RelacionCP3]

    init(_ condicion: CondicionColProbables, analisisActivo: Bool, numeroPartidos: Int, puntos: PuntosCP) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
        let cps = condicion.columnas.map { ColumnaProbable($0, numeroPartidos: numeroPartidos, puntos: puntos) }
        self.cps = cps
        controles = condicion.controlesFallos.map { CPControlFallos($0, cps: cps) }
        relaciones1 = condicion.relaciones1.map { RelacionCP1($0, cps: cps) }
        relaciones2 = condicion.relaciones2.map { RelacionCP2($0, cps: cps) }
        relaciones3 = condicion.relaciones3.map { RelacionCP3($0, cps: cps) }
    }

    var tipo: TipoFiltro { .colProbables }
    var activa: Bool { condicion.activa }

    /// `Analizar` (:54-92, 04 §0.3): corta en la 1.ª CP fallida (P7); el resultado de las relaciones III es el final.
    func analizar(_ col: Columna) throws(ErrorDominio) -> Bool {
        for cp in cps where !(try cp.analizar(col)) { return false }
        guard try controlFallosValido() else { return false }
        for r in relaciones1 where !(try r.analiza()) { return false }
        for r in relaciones2 where !(try r.analizar()) { return false }
        if !relaciones3.isEmpty {
            // `ControladorRelacionesCP3.AnalizaRelaciones` (:21-33): refresca las cachés antes de cada una.
            for r in relaciones3 {
                try r.actualizaValores()
                guard try r.analiza() else { return false }
            }
        }
        return true
    }

    /// `ControladorCPControlFallos.Analiza()` (:40-71): con fallos permitidos evalúa todos (P18).
    private func controlFallosValido() throws(ErrorDominio) -> Bool {
        if !condicion.fallosPermitidos.activo {
            for c in controles where !(try c.analiza()) { return false }
            return true
        }
        var fallos = 0
        for c in controles where !(try c.analiza()) { fallos += 1 }
        return condicion.fallosPermitidos.permite(fallos)
    }

    /// `AnalizarFallos` (:114-148): todas las CPs y relaciones, sin cortar; las III con sus cachés (P16).
    func analizarFallos(_ col: Columna) throws(ErrorDominio) -> [String]? {
        var t = ""
        for (i, cp) in cps.enumerated() { t += try cp.texto(col, i + 1) }
        // `ControladorCPControlFallos.Analiza(ref txt, …)` (:73-107).
        if !condicion.fallosPermitidos.activo {
            for (i, c) in controles.enumerated() {
                if try c.analiza() {
                    t += "*\(i + 1)ª linea de Tolerancias Locales aceptada.#"
                } else {
                    t += "Fallo en \(i + 1)ª linea de Tolerancias Locales  (\(c.toleranciasAcumuladas))#"
                }
            }
        } else {
            var fallos = 0
            for c in controles where !(try c.analiza()) { fallos += 1 }
            if !condicion.fallosPermitidos.permite(fallos) {
                t += "Fallo en Fallos de Controles permitidos  (\(fallos))#"
            }
        }
        for (i, r) in relaciones1.enumerated() { t += try r.texto(i + 1) }
        for (i, r) in relaciones2.enumerated() { t += try r.texto(i + 1) }
        for (i, r) in relaciones3.enumerated() { t += try r.texto(i + 1) }
        return Self.fallos(t)
    }

    /// (:223-233): suma sobre las CPs.
    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int {
        var n = 0
        for cp in cps { n += try cp.obtenNoAciertosTolerancias(letras) }
        return n
    }
}
