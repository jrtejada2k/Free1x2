import Free1X2Base

// C#: Free1X2.Domain/Analisis/AnalizadorColumnaCombinacion.cs (06 §3.3).

/// ¿Cumple esta columna la combinación? Sin memo ni filtro parcial; corta al primer fallo y **nunca** marca grupos ni
/// conjuntos como válidos, así que los controles cuentan todos sus grupos como fallados (P1) [PRESERVAR].
enum AnalizadorColumnaCombinacion {
    /// `AnalizaColumna(long, Analizador, string[])` (:38-110). ponytail: sin If-Then; su único llamador
    /// (`EscrutadorComb`) usa un `Analizador` que solo carga grupos, controles y conjuntos.
    static func analizaColumna(_ columna: Columna, _ a: Analizador, _ pronosticos: [String]) throws(ErrorDominio) -> Bool {
        let o = "AnalizadorColumnaCombinacion.cs:38-160"
        let grupos = [Bool](repeating: false, count: a.grupos.count)
        for (i, g) in a.grupos.enumerated() {
            if i == 0, try hayFallosPronostico(columna, pronosticos) { return false }
            let col = columna & g.mascara  // `ColumnaGrupo`: el grupo base (máscara 0) analiza la columna 0
            for f in g.filtros where f.activa {
                if !(try f.analizar(col)) { return false }
            }
            if g.hayTolerancias, !(try g.toleranciasValidas()) { return false }
        }
        let controles = a.controlador.controles
        let conjuntos = [Bool](repeating: false, count: controles.count)
        guard conjuntos.count > 1 else { return true }
        for c in controles.dropFirst() {
            var fallos = 0
            for n in c.gruposControlados where !(try grupos.leer(n, o)) { fallos += 1 }
            guard let p = c.fallosPermitidos else { throw .referenciaNula(o) }
            if !(try p.leer(fallos, o)) { return false }
        }
        let cc = a.controlador.conjuntos
        guard cc.count > 1 else { return true }
        for c in cc.dropFirst() {
            var fallos = 0
            for n in c.ctrlGruposControlados where !(try conjuntos.leer(n, o)) { fallos += 1 }
            guard let p = c.fallosPermitidos else { throw .referenciaNula(o) }
            if !(try p.leer(fallos, o)) { return false }
        }
        return true
    }

    /// `evaluarPronosticos` (:112-134): solo hasta el último grupo de 3 bits no nulo; cada uno exige su pronóstico.
    static func hayFallosPronostico(_ columna: Columna, _ pronosticos: [String]) throws(ErrorDominio) -> Bool {
        var c = columna
        var partido = 0
        var fallo = false
        while c != 0 {
            let signo = UInt8(truncatingIfNeeded: c & 7)
            let p = UtilColumnas.convPartidoStrToByte(try pronosticos.leer(partido, "AnalizadorColumnaCombinacion.cs:119"))
            if signo & p != signo { fallo = true }
            c >>= 3
            partido += 1
        }
        return fallo
    }
}
