import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/GrupoEquipos.cs, RelacionGE1.cs, ControladorRelacionesGE1.cs y
// FiltroGruposEquipos.cs (03 §14-15; 04 §11-12).

/// Un grupo de equipos en ejecución: sus contadores los leen las relaciones (B5: clase compartida).
final class GrupoEquipos {
    let c: GrupoEquiposConfig
    private(set) var noSumaPuntos = 0
    private(set) var noVictorias = 0
    private(set) var noEmpates = 0
    private(set) var noDerrotas = 0

    init(_ c: GrupoEquiposConfig) { self.c = c }

    /// `AnalizaColumna` (:75-149): código 7 salta el partido; termina al agotar la columna.
    private func calcular(_ columna: Columna) {
        (noSumaPuntos, noVictorias, noEmpates, noDerrotas) = (0, 0, 0, 0)
        var col = columna
        var lp = c.longPronosticos
        while col != 0 {
            let p = lp & 7
            if p != 7 {
                switch col & 7 {
                case 4:  // 1
                    if p == 1 { noDerrotas += 1 }
                    if p == 2 || p == 3 {
                        noSumaPuntos += 3
                        noVictorias += 1
                    }
                    if p == 3 { noDerrotas += 1 }
                case 1:  // 2
                    if p == 1 || p == 3 {
                        noVictorias += 1
                        noSumaPuntos += 3
                    }
                    if p == 2 || p == 3 { noDerrotas += 1 }
                case 2:  // X
                    if p == 1 || p == 2 {
                        noSumaPuntos += 1
                        noEmpates += 1
                    } else if p == 3 {
                        noSumaPuntos += 2
                        noEmpates += 2
                    }
                default: break
                }
            }
            col >>= 3
            lp >>= 3
        }
    }

    private var limites: [(ConjuntoRangos, Int, String)] {
        [
            (c.sumaPuntos, noSumaPuntos, "Suma de Puntos"), (c.victorias, noVictorias, "Nº de Victorias"),
            (c.empates, noEmpates, "Nº de Empates"), (c.derrotas, noDerrotas, "Nº de Derrotas"),
        ]
    }

    /// `Analizar(long)` y `esColumnaValida` (:160-192): corta en el primer límite que falla.
    func analizar(_ col: Columna) -> Bool {
        calcular(col)
        return limites.allSatisfy { r, n, _ in !r.activo || r.permite(n) }
    }

    /// `Analizar(long, numGrupo)` (:194-218): todos los fallos, sin cortar.
    func texto(_ col: Columna, _ numGrupo: Int) -> String {
        calcular(col)
        return limites.filter { r, n, _ in r.activo && !r.permite(n) }
            .map { _, n, nombre in "Fallo en \(nombre) del grupo \(numGrupo).  (\(n))#" }.joined()
    }
}

/// `RelacionGE1` en ejecución (04 §11).
final class RelacionGE1 {
    let c: RelacionGE1Config
    let grupos: [GrupoEquipos]

    init(_ c: RelacionGE1Config, grupos: [GrupoEquipos]) {
        self.c = c
        self.grupos = grupos
    }

    /// Orden de comprobación: puntos, victorias, empates, derrotas.
    private func limites() throws(ErrorDominio) -> [(ConjuntoRangos, Int, String)] {
        let o = "RelacionGE1.cs:70-84"
        guard let relacionados = c.geRelacionados else { throw .referenciaNula(o) }
        var (v, e, d, p) = (0, 0, 0, 0)
        for n in relacionados {
            let ge = try grupos.leerLista(n - 1, o)
            v += ge.noVictorias
            e += ge.noEmpates
            d += ge.noDerrotas
            p += ge.noSumaPuntos
        }
        return [
            (c.sumaPuntos, p, "Suma de Puntos"), (c.sumaVictorias, v, "Nº de Victorias"),
            (c.sumaEmpates, e, "Nº de Empates"), (c.sumaDerrotas, d, "Nº de Derrotas"),
        ]
    }

    private var sinLimites: Bool {
        !c.sumaPuntos.activo && !c.sumaVictorias.activo && !c.sumaEmpates.activo && !c.sumaDerrotas.activo
    }

    /// `Analiza()` (:46-57, 70-119).
    func analiza() throws(ErrorDominio) -> Bool {
        if sinLimites { return true }
        return try limites().allSatisfy { r, n, _ in !r.activo || r.permite(n) }
    }

    /// `Analiza(numRelacion)` (:59-68, 121-162): todos los fallos, sin cortar.
    func texto(_ numRelacion: Int) throws(ErrorDominio) -> String {
        if sinLimites { return "" }
        return try limites().filter { r, n, _ in r.activo && !r.permite(n) }
            .map { _, n, nombre in "Fallo en \(nombre) de la relación \(numRelacion).  (\(n))#" }.joined()
    }
}

/// `FiltroGruposEquipos` (03 §14).
final class FiltroGruposEquipos: FiltroColumna {
    let condicion: CondicionGruposEquipos
    let analisisActivo: Bool
    let grupos: [GrupoEquipos]
    let relaciones: [RelacionGE1]

    init(_ condicion: CondicionGruposEquipos, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
        let grupos = condicion.grupos.map(GrupoEquipos.init)
        self.grupos = grupos
        relaciones = condicion.relaciones.map { RelacionGE1($0, grupos: grupos) }
    }

    var tipo: TipoFiltro { .gruposEquipos }
    var activa: Bool { condicion.activa }

    /// `Analizar` (:43-54): los grupos posteriores al que falla conservan contadores obsoletos.
    func analizar(_ col: Columna) throws(ErrorDominio) -> Bool {
        for g in grupos where !g.analizar(col) { return false }
        for r in relaciones where !(try r.analiza()) { return false }
        return true
    }

    /// `AnalizarFallos` (:56-77): `ControladorRelacionesGE1.AnalizaRelaciones` **asigna** el texto de cada relación, así
    /// que solo queda el de la última (P20) [PRESERVAR].
    func analizarFallos(_ col: Columna) throws(ErrorDominio) -> [String]? {
        var t = ""
        for (i, g) in grupos.enumerated() { t += g.texto(col, i + 1) }
        var relaciones = ""
        for (i, r) in self.relaciones.enumerated() { relaciones = try r.texto(i + 1) }
        return Self.fallos(t + relaciones)
    }

    /// Sin tolerancias (03 §0.3).
    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int { 0 }
}
