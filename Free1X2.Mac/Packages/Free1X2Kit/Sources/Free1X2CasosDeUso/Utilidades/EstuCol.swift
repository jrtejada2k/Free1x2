import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Views/Ported/EstucolFrmViewModel.cs (09 pantalla 47; CU-B11) y VisorAnalisisColumnasAbdonFrmViewModel.cs (10 §101;
// CU-C50…C52).

/// «Agrupación de Columnas»: A = (1,2)(3,4)…, B = (1,2)(2,3)…, C = (1,3)(2,4)…
public enum AgrupacionEstuCol: Sendable, Hashable, CaseIterable {
    case a, b, c
}

/// `UltimoInforme`: un informe por columna emparejada (frente a todas las ganadoras) y otro por ganadora.
public struct InformeEstuCol: Sendable {
    public var porColumnas: [InformeColumnasABDON] = []
    public var porGanadoras: [InformeColumnasABDON] = []
    public var columnas: [Columna] = []
}

extension CasosUtilidades {
    /// `AbrirArchivoGanadorasAsync`: todas las líneas, vacías incluidas.
    public static func totalGanadorasEstuCol(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> Int {
        TextoNET.lineas(try entorno.archivos.leer(url)).count
    }

    /// `EmparejarColumnasReducidas`: OR de cada pareja; se descarta si la anterior no añade nada.
    static func emparejar(_ lineas: [String], _ modo: AgrupacionEstuCol) -> [Columna] {
        var r: [Columna] = []
        var (t1, t2): (Columna, Columna) = (0, 0)
        func anadir(_ previa: Columna, _ c: Columna) { if previa | c != c { r.append(previa | c) } }
        for (i, l) in lineas.enumerated() {
            let c = UtilColumnas.convStrToLong(l)
            switch modo {
            case .a:
                if i % 2 == 1 { anadir(t1, c) }
                t1 = c
            case .b:
                anadir(t1, c)
                t1 = c
            case .c:
                if i % 2 == 0 {
                    anadir(t1, c)
                    t1 = c
                } else {
                    anadir(t2, c)
                    t2 = c
                }
            }
        }
        return r
    }

    /// `ComenzarAsync`: emparejar, escrutar contra las `total` primeras ganadoras (las que falten quedan a 0) e informes.
    @concurrent public static func informeEstuCol(
        reducidas: URL, ganadoras: URL, total: Int, modo: AgrupacionEstuCol, entorno: Entorno
    ) async throws(ErrorDominio) -> InformeEstuCol {
        let cols = emparejar(TextoNET.lineas(try entorno.archivos.leer(reducidas)), modo)
        guard !cols.isEmpty else { return InformeEstuCol() }
        var m = [[Int]](repeating: [Int](repeating: 0, count: total), count: cols.count)
        for (g, l) in TextoNET.lineas(try entorno.archivos.leer(ganadoras)).prefix(total).enumerated() {
            let gl = UtilColumnas.convStrToLong(l)
            for i in cols.indices { m[i][g] = UtilColumnas.aciertos(cols[i], gl) }
        }
        return InformeEstuCol(
            porColumnas: m.map(InformeColumnasABDON.init(serie:)),
            porGanadoras: (0..<total).map { g in InformeColumnasABDON(serie: m.map { $0[g] }) }, columnas: cols)
    }
}

/// Estado y textos del visor ABDON (`VisorAnalisisColumnasAbdonFrmViewModel`).
public struct VisorAbdon: Sendable {
    /// `List<int>[elementos, aciertos]`: valores distintos; `nil` = celda sin tocar.
    public typealias Contenedor = [[[Int]?]]

    public let informe: InformeEstuCol
    public private(set) var global = true
    public private(set) var columna = 0
    public private(set) var ganadora = 0
    var pasoFijo: Contenedor
    var solapadas: Contenedor
    var pasoFijoGlobal: Contenedor
    var solapadasGlobal: Contenedor

    public private(set) var columnaTexto = "- / -"
    public private(set) var ganadoraTexto = "- / -"
    public private(set) var aciertosMin = "-"
    public private(set) var aciertosMax = "-"
    public private(set) var hayPasoFijo = ""
    public private(set) var elementosPasoFijo = ""
    public private(set) var haySolapadas = ""
    public private(set) var elementosSolapadas = ""
    public private(set) var sumaTotal = "-"
    public private(set) var sumaOpcional = "-"
    public private(set) var escalerasAsc = "-"
    public private(set) var escalerasDesc = "-"
    public private(set) var escalerasTotales = "-"
    public private(set) var sandwichs = "-"
    /// Filas `"número+elementos+aciertos"` (`ObtenTextoAgrupaciones`).
    public private(set) var filasPasoFijo: [String] = []
    public private(set) var filasSolapadas: [String] = []

    var vacio: Contenedor { Array(repeating: Array(repeating: nil, count: 15), count: informe.columnas.count + 1) }

    /// El ctor: textos por columna; los de ganadora llegan con `mostrarPorGanadoras()`.
    public init(_ informe: InformeEstuCol?) {
        self.informe = informe ?? InformeEstuCol()
        let v: Contenedor = Array(repeating: Array(repeating: nil, count: 15), count: self.informe.columnas.count + 1)
        (pasoFijo, solapadas, pasoFijoGlobal, solapadasGlobal) = (v, v, v, v)
        mostrarPorColumnas()
    }

    mutating func mostrarPorColumnas() {
        let p = informe.porColumnas
        guard columna < p.count else { return (columnaTexto, aciertosMin, aciertosMax) = ("- / -", "-", "-") }
        columnaTexto = "\(columna + 1) / \(p.count)"
        (aciertosMin, aciertosMax) = (String(p[columna].minimoAciertos), String(p[columna].maximoAciertos))
    }

    /// `MostrarInformePorGans`.
    public mutating func mostrarPorGanadoras() throws(ErrorDominio) {
        let g = informe.porGanadoras
        guard !g.isEmpty else { return ganadoraTexto = "- / -" }
        if !global {
            ganadoraTexto = "\(ganadora + 1) / \(g.count)"
            _ = try individual(solapadas: false, tipo: 0)
            filasPasoFijo = Self.textoAgrupaciones(pasoFijo)
            _ = try individual(solapadas: true, tipo: 0)
            filasSolapadas = Self.textoAgrupaciones(solapadas)
            let inf = g[ganadora]
            sumaTotal = String(inf.sumaTotalDeAciertos)
            escalerasDesc = String(inf.numeroDeEscalerasDescendentes)
            escalerasAsc = String(inf.numeroDeEscalerasAscendentes)
            escalerasTotales = String(inf.numeroDeEscaleras)
            sandwichs = String(inf.sandwichs.count)
        } else {
            _ = try deTodas(solapadas: false, tipo: 0)
            filasPasoFijo = Self.textoAgrupaciones(pasoFijoGlobal)
            _ = try deTodas(solapadas: true, tipo: 0)
            filasSolapadas = Self.textoAgrupaciones(solapadasGlobal)
            func rango(_ v: (InformeColumnasABDON) -> Int) -> String {
                let x = g.map(v)
                return "\(x.min() ?? 0)-\(x.max() ?? 0)"
            }
            escalerasAsc = rango(\.numeroDeEscalerasAscendentes)
            escalerasDesc = rango(\.numeroDeEscalerasDescendentes)
            escalerasTotales = rango(\.numeroDeEscaleras)
            sandwichs = rango { $0.sandwichs.count }
            let s = g.map(\.sumaTotalDeAciertos).sorted()
            sumaTotal = s.first.map { "\($0)-\(s[s.count - 1])" } ?? "-"
        }
    }

    static func matriz(_ inf: InformeColumnasABDON, _ solapadas: Bool) throws(ErrorDominio) -> [[Int]] {
        solapadas ? try inf.agrupacionesSolapadas() : try inf.agrupacionesPasoFijo()
    }

    static func anadir(_ c: inout Contenedor, _ j: Int, _ k: Int, _ v: Int) throws(ErrorDominio) -> Bool {
        guard c.indices.contains(j), c[j].indices.contains(k) else { throw .fueraDeRango("VisorAnalisisColumnasAbdonFrmViewModel.cs") }
        var l = c[j][k] ?? []
        defer { c[j][k] = l }
        guard !l.contains(v) else { return false }
        l.append(v)
        return true
    }

    /// `ObtenerListaAgrupaciones{PasoFijo,Solapadas}` (desde la fila 0): suma de los valores nuevos de la ganadora actual.
    mutating func individual(solapadas sol: Bool, tipo: Int) throws(ErrorDominio) -> Int {
        guard informe.porGanadoras.indices.contains(ganadora) else { return 0 }
        let m = try Self.matriz(informe.porGanadoras[ganadora], sol)
        var c = sol ? solapadas : pasoFijo
        defer { if sol { solapadas = c } else { pasoFijo = c } }
        var n = 0
        for j in m.indices where j == tipo || tipo == 0 {
            for k in m[j].indices {
                if try Self.anadir(&c, j, k, m[j][k]) { n += m[j][k] }
            }
        }
        return n
    }

    /// `...Global`: desde la fila 1, un total por ganadora; las filas que la ganadora no alcanza garantizan un 0.
    mutating func deTodas(solapadas sol: Bool, tipo: Int) throws(ErrorDominio) -> [Int] {
        var c = sol ? solapadasGlobal : pasoFijoGlobal
        defer { if sol { solapadasGlobal = c } else { pasoFijoGlobal = c } }
        var valores: [Int] = []
        for inf in informe.porGanadoras {
            let m = try Self.matriz(inf, sol)
            var n = 0
            for j in m.indices.dropFirst() where j == tipo || tipo == 0 {
                for k in m[j].indices {
                    n += m[j][k]
                    _ = try Self.anadir(&c, j, k, m[j][k])
                }
            }
            for j in stride(from: m.count, to: c.count, by: 1) {
                for k in 0..<(m.first?.count ?? 0) { _ = try Self.anadir(&c, j, k, 0) }
            }
            valores.append(n)
        }
        return UtilidadesEntradasValores.obtenerRangos(&valores)
    }

    /// `ObtenTextoAgrupaciones`: desde la fila 1; un único valor 0 no sale.
    public static func textoAgrupaciones(_ c: Contenedor) -> [String] {
        var r: [String] = []
        for i in c.indices.dropFirst() {
            for (j, celda) in c[i].enumerated() {
                guard let l = celda?.sorted(), let a = l.first else { continue }
                if l.count > 1 {
                    r.append("\(a)-\(l[l.count - 1])+\(i)+\(j)")
                } else if a != 0 {
                    r.append("\(a)+\(i)+\(j)")
                }
            }
        }
        return r
    }

    /// `CambiarModo`: la pantalla vuelve a 0 sus dos «Mostrar sólo agrupaciones de».
    public mutating func cambiarModo() throws(ErrorDominio) {
        global.toggle()
        (hayPasoFijo, haySolapadas, elementosPasoFijo, elementosSolapadas) = ("", "", "", "")
        try mostrarPorGanadoras()
    }

    public mutating func moverColumna(_ paso: Int) {
        let n = columna + paso
        guard n >= 0 && n < informe.porColumnas.count else { return }
        columna = n
        mostrarPorColumnas()
    }

    public mutating func moverGanadora(_ paso: Int) throws(ErrorDominio) {
        let n = ganadora + paso
        guard n >= 0 && n < informe.porGanadoras.count else { return }
        ganadora = n
        (solapadas, pasoFijo) = (vacio, vacio)
        try mostrarPorGanadoras()
    }

    /// «Mostrar» (`MostrarAgrupaciones{PasoFijo,Solapadas}`): rehace el contenedor del modo activo.
    public mutating func mostrarAgrupaciones(solapadas sol: Bool, tipo: Int) throws(ErrorDominio) {
        let hay: String
        if !global {
            if sol { solapadas = vacio } else { pasoFijo = vacio }
            hay = String(try individual(solapadas: sol, tipo: tipo))
        } else {
            if sol { solapadasGlobal = vacio } else { pasoFijoGlobal = vacio }
            let r = try deTodas(solapadas: sol, tipo: tipo)
            hay = r.count >= 2 ? "\(r[0])-\(r[1])" : ""
        }
        if sol {
            (filasSolapadas, elementosSolapadas, haySolapadas) = (
                Self.textoAgrupaciones(global ? solapadasGlobal : solapadas), String(tipo), hay
            )
        } else {
            (filasPasoFijo, elementosPasoFijo, hayPasoFijo) = (
                Self.textoAgrupaciones(global ? pasoFijoGlobal : pasoFijo), String(tipo), hay
            )
        }
    }

    /// `CalcularSumaOpcional`: columnas 1-based del texto (error ⇒ ninguna).
    public mutating func calcularSumaOpcional(_ texto: String) {
        let cols = UtilidadesEntradasValores.obtenerListaFromTxt(texto)
        let serie = informe.porGanadoras.indices.contains(ganadora) ? informe.porGanadoras[ganadora].serieAciertos : []
        sumaOpcional = String(serie.enumerated().reduce(0) { cols.contains($1.offset) ? $0 + $1.element : $0 })
    }

    public var puedeGenerarCondicion: Bool {
        !informe.columnas.isEmpty && informe.porColumnas.count >= informe.columnas.count
    }

    /// CU-C52 `GenerarCondicion`: con los textos y contenedores globales. Una lista de agrupaciones vacía lanza al
    /// grabar en el C# (`Substring(0, -1)`).
    public func condicion(numeroPartidos n: Int) throws(ErrorDominio) -> CondicionesGrupo {
        let todos = CasosCondiciones.todosValores(n)
        let k = informe.columnas.count
        var cps: [ColumnaProbableConfig] = []
        for (i, col) in informe.columnas.enumerated() {
            var cp = ColumnaProbableConfig(numeroPartidos: n)
            try cp.ponerPronosticos(
                try UtilColumnas.obtenerStringFromLongApuestaMultiple(col).components(separatedBy: ",").map(Optional.some))
            let inf = informe.porColumnas[i]
            try cp.setNoAciertos(try UtilidadesEntradasValores.obtenerTodosValores(inf.minimoAciertos, inf.maximoAciertos))
            try cp.setNoAciertosSeguidos(todos)
            try cp.setNoFallosSeguidos(todos)
            try cp.setPuntos("")
            cps.append(cp)
        }
        var f = CondicionColProbables()
        f.ponerColumnas(cps)
        var r1 = RelacionCP1Config()
        try r1.ponerColumnas("1-\(k)")
        try r1.ponerSumaAciertos(sumaTotal)
        try r1.ponerRecorridos("")
        try r1.ponerCantidadCP("")
        try r1.ponerCuantosAC("")
        f.relaciones1.append(r1)
        var r3 = RelacionCP3Config()
        r3.columnasImplicadas = "1-\(k)"
        (r3.concepto, r3.conceptoTexto) = ("AC", "AC")
        r3.escalerasAsc = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(escalerasAsc, longitud: k / 3)
        r3.escalerasAscTexto = escalerasAsc
        r3.escalerasDesc = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(escalerasDesc, longitud: k / 3)
        r3.escalerasDescTexto = escalerasDesc
        r3.escalerasTotales = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(escalerasTotales, longitud: k / 3)
        r3.escalerasTexto = escalerasTotales
        r3.sandwichs = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(sandwichs, longitud: k / 4)
        r3.sandwichsTexto = sandwichs
        func unir(_ t: [String]) throws(ErrorDominio) -> String {
            guard !t.isEmpty else { throw .fueraDeRangoLista("FColProbablesData.cs:569") }
            return t.joined(separator: "#")
        }
        r3.agrupacionesPasoFijoTexto = try unir(Self.textoAgrupaciones(pasoFijoGlobal))
        r3.agrupacionesSolapadasTexto = try unir(Self.textoAgrupaciones(solapadasGlobal))
        f.relaciones3.append(r3)
        f.contieneDatos = true
        var c = CondicionesGrupo(numeroPartidos: n)
        c.colProbables = f
        return c
    }
}
