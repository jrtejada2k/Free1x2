import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/RelacionCP1.cs, RelacionCP2.cs, RelacionCP3.cs (+ EscaleraAciertos, SandwichAciertos,
// AgrupacionColumnas) y sus controladores (04 §5-§10). Leen los contadores vivos de las CPs del filtro (S3).

/// Relación I (04 §5).
final class RelacionCP1 {
    let c: RelacionCP1Config
    let cps: [ColumnaProbable]
    /// Posiciones (0-based) dentro de `columnasRelacionadas` de las CPs que cumplen «cuántos AC».
    private(set) var listaCPs: [Int] = []
    private(set) var noColConMismosAC = 0
    private(set) var sumaTemp = 0
    /// No se reinicia entre llamadas.
    private(set) var recorrido = 0
    private let o = "RelacionCP1.cs:72-270"

    init(_ c: RelacionCP1Config, cps: [ColumnaProbable]) {
        self.c = c
        self.cps = cps
    }

    private func relacionadas() throws(ErrorDominio) -> [Int] {
        guard let r = c.columnasRelacionadas else { throw .referenciaNula(o) }
        return r
    }

    private func cp(_ n: Int) throws(ErrorDominio) -> ColumnaProbable { try cps.leerLista(n - 1, o) }

    /// `Analiza()` (:72-89): cantidad → suma → recorrido, encadenados.
    func analiza() throws(ErrorDominio) -> Bool {
        try texto(nil).isEmpty
    }

    /// `Analiza(numRel)` (:91-115): mismo encadenado ⇒ como mucho un mensaje. Con `nil` solo decide (`""` = válida).
    func texto(_ numRel: Int?) throws(ErrorDominio) -> String {
        listaCPs = []
        sumaTemp = 0
        noColConMismosAC = 0
        let n = numRel.map(String.init) ?? ""
        if c.cantidadCP.activo, !(try grupos()) {
            return "Fallo en Cantidad de CPs de la Relación de Columnas \(n)  (\(noColConMismosAC))#"
        }
        if !c.sumaAciertos.isEmpty, !(try sumas()) {
            return "Fallo en Suma de Aciertos de la Relación de Columnas \(n)  (\(sumaTemp))#"
        }
        if !c.recorridos.isEmpty, !(try recorridos()) {
            return "Fallo en Recorrido de la Relación de Columnas \(n)  (\(recorrido))#"
        }
        return ""
    }

    /// `AnalizaGrupos` (:233-270): búferes de 15 (P9).
    private func grupos() throws(ErrorDominio) -> Bool {
        var porAciertos = [Int](repeating: 0, count: 15)
        var cpsPorAciertos = [[Int]](repeating: [], count: 15)
        for (i, n) in try relacionadas().enumerated() {
            let k = try cp(n).noAC
            try porAciertos.escribir(try porAciertos.leer(k, o) + 1, en: k, o)
            cpsPorAciertos[k].append(i)
        }
        guard let cuantos = c.cuantosAC.valores else { throw .referenciaNula(o) }
        for i in cuantos.indices where cuantos[i] {
            noColConMismosAC += try porAciertos.leer(i, o)
            listaCPs += cpsPorAciertos[i]
        }
        return c.cantidadCP.permite(noColConMismosAC)
    }

    /// Las CPs de `listaCPs` o, si está vacía, todas las relacionadas (P8).
    private func fuente() throws(ErrorDominio) -> [ColumnaProbable] {
        let r = try relacionadas()
        var l: [ColumnaProbable] = []
        if listaCPs.isEmpty {
            for n in r { l.append(try cp(n)) }
        } else {
            for p in listaCPs { l.append(try cp(r[p])) }
        }
        return l
    }

    /// `AnalizaSumas` (:124-161).
    private func sumas() throws(ErrorDominio) -> Bool {
        sumaTemp = try fuente().reduce(0) { $0 + $1.noAC }
        guard let s = c.sumas else { throw .referenciaNula(o) }
        return s.indices.contains(sumaTemp) && s[sumaTemp]
    }

    /// `AnalizaRecorridos` (:163-224): sin comprobación de límites.
    private func recorridos() throws(ErrorDominio) -> Bool {
        var (maximo, minimo) = (0, 14)
        for cp in try fuente() {
            minimo = min(minimo, cp.noAC)
            maximo = max(maximo, cp.noAC)
        }
        recorrido = maximo - minimo
        guard let r = c.recorridosPermitidos else { throw .referenciaNula(o) }
        return try r.leer(recorrido, o)
    }
}

/// Relación II (04 §7): índices 0-based con duplicados significativos (S11).
final class RelacionCP2 {
    let cps: [ColumnaProbable]
    let columnasA: [Int], columnasB: [Int], aciertos: [Int]
    let columnasA2: [Int], columnasB2: [Int], aciertos2: [Int]
    let concepto: String, cantidad: String, concepto2: String, cantidad2: String
    private let o = "RelacionCP2.cs:46-150"

    init(_ c: RelacionCP2Config, cps: [ColumnaProbable]) {
        self.cps = cps
        typealias U = UtilidadesEntradasValores
        columnasA = U.obtenerListaFromTxt(c.columnasA)
        columnasB = U.obtenerListaFromTxt(c.columnasB)
        aciertos = U.obtenerListaFromTxtAciertos(c.aciertos)
        columnasA2 = U.obtenerListaFromTxt(c.columnasA2)
        columnasB2 = U.obtenerListaFromTxt(c.columnasB2)
        aciertos2 = U.obtenerListaFromTxtAciertos(c.aciertos2)
        (concepto, cantidad, concepto2, cantidad2) = (c.concepto, c.cantidad, c.concepto2, c.cantidad2)
    }

    var activaGlobales: Bool { !columnasA.isEmpty && !columnasB.isEmpty && !aciertos.isEmpty }
    var activaIndividuales: Bool { !columnasA2.isEmpty && !columnasB2.isEmpty && !aciertos2.isEmpty }

    /// `Analizar()` (:152-166).
    func analizar() throws(ErrorDominio) -> Bool {
        var ok = true
        if activaGlobales { ok = try globales() }
        if activaIndividuales && ok { ok = try individuales() }
        return ok
    }

    /// `Analiza(numRel)` (:168-186): los dos bloques siempre.
    func texto(_ numRel: Int) throws(ErrorDominio) -> String {
        var t = ""
        if activaGlobales, !(try globales()) { t += "Fallo en Relaciones II - Globales#" }
        if activaIndividuales, !(try individuales()) { t += "Fallo en Relaciones II - Individuales#" }
        return t
    }

    private static func valor(_ cp: ColumnaProbable, _ concepto: String) -> Int? {
        switch concepto {
        case "AC": cp.noAC
        case "ACS": cp.noACS
        case "FS": cp.noFS
        default: nil
        }
    }

    private func suma(_ columnas: [Int]) throws(ErrorDominio) -> [Int] {
        var s = [0, 0, 0]
        for i in columnas {
            let cp = try cps.leerLista(i, o)
            s[0] += cp.noAC
            s[1] += cp.noACS
            s[2] += cp.noFS
        }
        return s
    }

    /// `AnalizaGlobales` (:46-91, 188-213): combinación desconocida ⇒ `true` (P11).
    private func globales() throws(ErrorDominio) -> Bool {
        let a = try suma(columnasA)
        let b = try suma(columnasB)
        guard let k = ["AC", "ACS", "FS"].firstIndex(of: concepto) else { return true }
        switch cantidad {
        case "Más": return aciertos.contains(a[k] - b[k])
        case "Menos": return aciertos.contains(b[k] - a[k])
        default: return true
        }
    }

    /// `AnalizaIndividuales` (:92-150): cada A2 contra cada B2; combinación desconocida ⇒ diferencia 0 (P11).
    private func individuales() throws(ErrorDominio) -> Bool {
        for i in columnasA2 {
            let cp = try cps.leerLista(i, o)
            for j in columnasB2 {
                let cp2 = try cps.leerLista(j, o)
                var d = 0
                if let x = Self.valor(cp, concepto2), let y = Self.valor(cp2, concepto2) {
                    switch cantidad2 {
                    case "Más": d = x - y
                    case "Menos": d = y - x
                    default: break
                    }
                }
                if !aciertos2.contains(d) { return false }
            }
        }
        return true
    }
}

/// Relación III (04 §9): escaleras, sándwiches y agrupaciones sobre la serie de las primeras K CPs (P12), o sobre una
/// serie explícita (informe ABDON). Las cachés solo se invalidan en `actualizaValores` (P16).
final class RelacionCP3 {
    let c: RelacionCP3Config
    /// `nil` con serie explícita.
    let columnas: [ColumnaProbable]?
    /// K > nº de CPs: el C# lanza al cargar (`grupoCP[i]`); aquí, al primer uso.
    private let faltanColumnas: Bool
    private var serieCache: [Int]?
    private var escalerasCache: [[Int]]?
    private var sandwichsCache: [[Int]]?
    private var pasoFijoCache: [[Int]]?
    private var solapadasCache: [[Int]]?
    private let o = "RelacionCP3.cs:68-430"

    init(_ c: RelacionCP3Config, cps: [ColumnaProbable]) {
        self.c = c
        let k = UtilidadesEntradasValores.obtenerListaFromTxt(c.columnasImplicadas).count
        columnas = Array(cps.prefix(k))
        faltanColumnas = k > cps.count
    }

    init(serie: [Int], _ c: RelacionCP3Config = RelacionCP3Config()) {
        self.c = c
        columnas = nil
        faltanColumnas = false
        serieCache = serie
    }

    /// `ActualizaValores` (:204-212): recalcula ya la serie (sin CPs ⇒ `NullReference`).
    func actualizaValores() throws(ErrorDominio) {
        serieCache = nil
        _ = try serie()
        escalerasCache = nil
        sandwichsCache = nil
        pasoFijoCache = nil
        solapadasCache = nil
    }

    /// `SerieAciertos` / `ObtenerSerieAciertos` (:233-261): concepto desconocido o `nil` ⇒ ceros.
    func serie() throws(ErrorDominio) -> [Int] {
        if let s = serieCache { return s }
        guard let columnas else { throw .referenciaNula(o) }
        if faltanColumnas { throw .fueraDeRangoLista("FColProbablesData.cs:714-723") }
        let s: [Int] = columnas.map { cp in
            switch c.concepto {
            case "AC": cp.noAC
            case "ACS": cp.noACS
            case "FS": cp.noFS
            default: 0
            }
        }
        serieCache = s
        return s
    }

    /// `ObtenerEscaleras` (:383-430): tramos ±1 sin repetir valor, de 3 o más.
    func escaleras() throws(ErrorDominio) -> [[Int]] {
        if let e = escalerasCache { return e }
        let s = try serie()
        let e = Self.escaleras(s)
        escalerasCache = e
        return e
    }

    static func escaleras(_ s: [Int]) -> [[Int]] {
        var e: [[Int]] = []
        var t: [Int] = []
        if let primero = s.first {
            var previo = primero
            t = [primero]
            for v in s.dropFirst() {
                if abs(v - previo) == 1 && !t.contains(v) {
                    t.append(v)
                } else {
                    if t.count >= 3 { e.append(t) }
                    t = [v]
                }
                previo = v
            }
        }
        if t.count >= 3 { e.append(t) }
        return e
    }

    /// `NumeroDeEscalerasAscendentes` (:521-536): las que no son descendentes (`escalones[0] > último`).
    static func ascendentes(_ e: [[Int]]) -> Int {
        e.count(where: { ($0.first ?? 0) <= ($0.last ?? 0) })
    }

    /// `ObtenerSandwichs` (:431-520): máquina de estados; tras completar uno se descarta el elemento siguiente (P14).
    func sandwichs() throws(ErrorDominio) -> [[Int]] {
        if let r = sandwichsCache { return r }
        let r = Self.sandwichs(try serie())
        sandwichsCache = r
        return r
    }

    static func sandwichs(_ s: [Int]) -> [[Int]] {
        var capa = 1
        var t: [Int] = []
        var r: [[Int]] = []
        for (i, v) in s.enumerated() {
            switch capa {
            case 1:
                t.append(v)
                capa = 2
            case 2:
                if v != t[0] {
                    t.append(v)
                    capa = 3
                } else {
                    t = [v]
                }
            case 3:
                if v == t[1] {
                    t.append(v)
                    capa = 4
                } else {
                    t = [s[i - 1], v]
                }
            case 4:
                if v == t[0] {
                    t.append(v)
                    capa = 5
                } else {
                    t = [s[i - 1], v]
                    capa = 3
                }
            default:
                if t.count == 4 { r.append(t) }
                t = []
                capa = 1
            }
        }
        if t.count == 4 { r.append(t) }
        return r
    }

    /// `ObtenerAgrupaciones` (:291-348): `M[longitud][valor]` = nº de rachas; 15 columnas (valor ≥ 15 lanza).
    func pasoFijo() throws(ErrorDominio) -> [[Int]] {
        if let m = pasoFijoCache { return m }
        let m = try Self.pasoFijo(try serie())
        pasoFijoCache = m
        return m
    }

    static func pasoFijo(_ s: [Int]) throws(ErrorDominio) -> [[Int]] {
        var m = [[Int]](repeating: [Int](repeating: 0, count: 15), count: 1)
        func sumar(_ racha: Int, _ valor: Int) throws(ErrorDominio) {
            while m.count <= racha { m.append([Int](repeating: 0, count: 15)) }
            try m[racha].escribir(try m[racha].leer(valor, "RelacionCP3.cs:291-348") + 1, en: valor, "RelacionCP3.cs:291-348")
        }
        guard var actual = s.first else { return m }
        var racha = 1
        for v in s.dropFirst() {
            if v == actual {
                racha += 1
            } else {
                try sumar(racha, actual)
                racha = 1
            }
            actual = v
        }
        try sumar(racha, actual)
        return m
    }

    /// `ObtenerAgrupacionesSolapadas` (:349-382): ventanas de longitud `k` en una racha de `i`, sin multiplicar por el nº
    /// de rachas (P15).
    func solapadas() throws(ErrorDominio) -> [[Int]] {
        if let m = solapadasCache { return m }
        let m = Self.solapadas(try pasoFijo())
        solapadasCache = m
        return m
    }

    static func solapadas(_ m: [[Int]]) -> [[Int]] {
        var s = m
        for i in m.indices {
            for j in m[i].indices where m[i][j] > 0 {
                var k = i - 1
                while k >= 2 {
                    s[k][j] += i - k + 1
                    k -= 1
                }
            }
        }
        return s
    }

    /// `Analiza()` (:68-83): escaleras → sándwiches → paso fijo → solapadas (estas nunca rechazan, P13).
    func analiza() throws(ErrorDominio) -> Bool {
        guard try analizaEscaleras() else { return false }
        guard try analizaSandwichs() else { return false }
        guard try analizaAgrupaciones(solapadas: false) else { return false }
        _ = try analizaAgrupaciones(solapadas: true)
        return true
    }

    /// `Analiza(numRel)` (:173-202): un solo mensaje, con las cachés tal como estén.
    func texto(_ numRel: Int) throws(ErrorDominio) -> String {
        if !(try analizaEscaleras()) { return "Fallo en Escaleras (\(try escaleras().count))#" }
        if !(try analizaSandwichs()) { return "Fallo en Sandwichs (\(try sandwichs().count))#" }
        if !(try analizaAgrupaciones(solapadas: false)) { return "Fallo en Agrupaciones Paso Fijo#" }
        _ = try analizaAgrupaciones(solapadas: true)  // «Fallo en Agrupaciones Solapadas» nunca sale
        return ""
    }

    /// `AnalizaEscaleras` (:135-162): sin comprobación de límites.
    private func analizaEscaleras() throws(ErrorDominio) -> Bool {
        let e = try escaleras()
        let asc = Self.ascendentes(e)
        if let p = c.escalerasAsc, !(try p.leer(asc, o)) { return false }
        if let p = c.escalerasDesc, !(try p.leer(e.count - asc, o)) { return false }
        if let p = c.escalerasTotales, !(try p.leer(e.count, o)) { return false }
        return true
    }

    /// `AnalizaSandwichs` (:163-170).
    private func analizaSandwichs() throws(ErrorDominio) -> Bool {
        guard let p = c.sandwichs else { return true }
        return try p.leer(try sandwichs().count, o)
    }

    /// `AnalizaAgrupacionesPasoFijo` (:85-109) y `…Solapadas` (:110-133, que descarta el resultado).
    /// La matriz solo se calcula si hay agrupaciones permitidas.
    private func analizaAgrupaciones(solapadas: Bool) throws(ErrorDominio) -> Bool {
        guard let permitidas = solapadas ? c.agrupacionesSolapadas : c.agrupacionesPasoFijo else { return true }
        let m = solapadas ? try self.solapadas() : try pasoFijo()
        for ag in permitidas {
            var n = 0
            if m.count > ag.noElementos {
                let fila = try m.leer(ag.noElementos, o)
                for v in ag.noAciertos { n += try fila.leer(v, o) }
            }
            if !ag.numero.contains(n) { return false }
        }
        return true
    }
}
