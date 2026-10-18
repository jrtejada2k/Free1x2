import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroFormatosSignos.cs, FormatosSignos.cs y FormatoSignos.cs (03 §9-11).

final class FiltroFormatosSignos: FiltroColumna {
    let condicion: CondicionFormatosSignos
    let analisisActivo: Bool
    /// Del último grupo válido de la última llamada a `analizar`.
    private(set) var noLineasValidas = 0
    private(set) var noAparicionesGlobal = 0
    /// Contadores de cada grupo (los lee el análisis), actualizados hasta donde se evaluó.
    private(set) var lineasValidasGrupo: [Int]
    private(set) var aparicionesGrupo: [Int]

    init(_ condicion: CondicionFormatosSignos, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
        lineasValidasGrupo = Array(repeating: 0, count: condicion.formatosSignos.count)
        aparicionesGrupo = lineasValidasGrupo
    }

    var tipo: TipoFiltro { .formatosSignos }
    var activa: Bool { condicion.activa }

    /// Quita los asteriscos: el 1.er partido queda en el grupo más alto (:41-66).
    static func sinAsteriscos(_ columna: Columna) -> Int64 {
        var c = columna
        var r: Int64 = 0
        while c != 0 {
            let s = c & 7
            if s != 0 { r = (r &<< 3) | s }
            c >>= 3
        }
        return r
    }

    /// `FormatoSignos.Analiza` (:39-65): nº de ventanas en las que la columna encaja con el patrón.
    static func apariciones(_ f: FormatoSignos, _ columna: Int64) -> Int {
        var c = columna
        var n = 0
        while c != 0 && c >= f.valMin {
            if c & f.longFormato == c { n += 1 }
            c >>= 3
        }
        return n
    }

    static func cumple(_ f: FormatoSignos, _ n: Int) throws(ErrorDominio) -> Bool {
        guard let a = f.arrayRangos else { throw .referenciaNula("FormatoSignos.cs:58") }
        return a.count > n && a[n]
    }

    /// `FormatosSignos.Analizar(long)` (:38-85): modo «Y» si no hay `Lineas` ni `Global`; si no, contado.
    private func grupo(_ g: FormatosSignos, _ indice: Int, _ col: Int64) throws(ErrorDominio) -> Bool {
        var valida = true
        var lineas = 0
        var global = 0
        defer { (lineasValidasGrupo[indice], aparicionesGrupo[indice]) = (lineas, global) }
        let contado = !g.lineas.isEmpty || !g.global.isEmpty
        for f in g.lineasFormatos {
            let n = Self.apariciones(f, col)
            valida = try Self.cumple(f, n)
            global += n
            if valida {
                lineas += 1
            } else if !contado {
                break
            }
        }
        guard contado else { return valida }
        var ok = true
        if !g.lineas.isEmpty {
            guard let a = g.arrayLineas else { throw .referenciaNula("FormatosSignos.cs:66") }
            ok = a.count > lineas && a[lineas]
        }
        if !g.global.isEmpty && ok {
            guard let a = g.arrayGlobal else { throw .referenciaNula("FormatosSignos.cs:74") }
            ok = a.count > global && a[global]
        }
        return ok
    }

    func analizar(_ c: Columna) throws(ErrorDominio) -> Bool {
        let col = Self.sinAsteriscos(c)
        for (i, g) in condicion.formatosSignos.enumerated() {
            guard try grupo(g, i, col) else { return false }
            noLineasValidas = lineasValidasGrupo[i]
            noAparicionesGlobal = aparicionesGrupo[i]
        }
        return true
    }

    /// (:68-94) y `FormatosSignos.Analizar(long, numGrupo)` (:87-127): todos los grupos, sin cortar.
    func analizarFallos(_ c: Columna) throws(ErrorDominio) -> [String]? {
        let col = Self.sinAsteriscos(c)
        var texto = ""
        for (i, g) in condicion.formatosSignos.enumerated() {
            let numGrupo = i + 1
            let contado = !g.lineas.isEmpty || !g.global.isEmpty
            var lineas = 0
            var global = 0
            for (j, f) in g.lineasFormatos.enumerated() {
                let n = Self.apariciones(f, col)
                let valida = try Self.cumple(f, n)
                if !valida { texto += "Fallo en Linea nº \(j + 1) (\(f.formato)) en grupo \(numGrupo)  (\(n))#" }
                global += n
                if valida {
                    lineas += 1
                } else if !contado {
                    break
                }
            }
            (lineasValidasGrupo[i], aparicionesGrupo[i]) = (lineas, global)
            if !g.lineas.isEmpty {
                guard let a = g.arrayLineas else { throw .referenciaNula("FormatosSignos.cs:114") }
                if !(a.count > lineas && a[lineas]) { texto += "Fallo en Líneas en grupo \(numGrupo)  (\(lineas))#" }
            }
            if !g.global.isEmpty {
                guard let a = g.arrayGlobal else { throw .referenciaNula("FormatosSignos.cs:119") }
                if !(a.count > global && a[global]) { texto += "Fallo en Global en grupo \(numGrupo)  (\(global))#" }
            }
        }
        return Self.fallos(texto)
    }

    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int { 0 }
}
