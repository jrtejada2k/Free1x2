import Free1X2Base
import Free1X2Pruebas
import Testing

@testable import Free1X2Motor

/// ESPEC_17 CP-CP-02…11: columnas probables, control de fallos y relaciones I/II/III — suites `motor/cp/*`.
@Suite("Columnas probables")
struct ColProbablesTests {
    /// Una CP en JSON: setters en el orden del cargador (04 §15).
    struct CPJ: Decodable, Sendable {
        var pronosticos: [String?]
        var aciertos, aciertosSeguidos, fallosSeguidos, puntos: String?
        var tolerancias, acTol, acsTol, fsTol: String?
    }
    struct ControlJ: Decodable, Sendable {
        var columnas, tolerancias, aciertos: String
    }
    struct Relacion1J: Decodable, Sendable {
        var columnas: String
        var cantidadCP, cuantosAC, sumaAciertos, recorridos: String?
    }
    struct Relacion2J: Decodable, Sendable {
        var columnasA, columnasB, aciertos, concepto, cantidad: String?
        var columnasA2, columnasB2, aciertos2, concepto2, cantidad2: String?
    }
    /// Arrays permitidos con `ObtenerBoolArrayFromTxt(texto, L)` como el cargador (L = nº CPs / 3; sándwiches K / 4).
    struct Relacion3J: Decodable, Sendable {
        var columnas: String
        var concepto: String?
        var escalerasAsc, escalerasDesc, escalerasTotales, sandwichs: String?
        var pasoFijo, solapadas: [String]?
    }
    struct CondJ: Decodable, Sendable {
        var activa: Bool?
        var cps: [CPJ]
        var controles: [ControlJ]?
        var fallosPermitidos: String?
        var relaciones1: [Relacion1J]?
        var relaciones2: [Relacion2J]?
        var relaciones3: [Relacion3J]?
    }

    static let n = 14

    static func cp(_ j: CPJ) throws(ErrorDominio) -> ColumnaProbableConfig {
        var c = ColumnaProbableConfig(numeroPartidos: n)
        try c.ponerPronosticos(j.pronosticos)
        if let s = j.aciertos { try c.setNoAciertos(s) }
        if let s = j.aciertosSeguidos { try c.setNoAciertosSeguidos(s) }
        if let s = j.fallosSeguidos { try c.setNoFallosSeguidos(s) }
        if let s = j.puntos { try c.setPuntos(s) }
        if let s = j.tolerancias { try c.setTolerancias(s) }
        if let s = j.acTol { try c.setACTol(s) }
        if let s = j.acsTol { try c.setACSTol(s) }
        if let s = j.fsTol { try c.setFSTol(s) }
        return c
    }

    /// «a+b+c» de `ObtenArrayAgrupaciones` (04 §15).
    static func agrupaciones(_ t: [String]?) throws(ErrorDominio) -> [AgrupacionColumnas]? {
        guard let t else { return nil }
        var r: [AgrupacionColumnas] = []
        for x in t {
            let p = x.split(separator: "+").map(String.init)
            r.append(
                AgrupacionColumnas(
                    numero: UtilidadesEntradasValores.obtenerListaFromTxtAciertos(p[0]),
                    noElementos: Int(try FormatoNET.convertToInt32(p[1])),
                    noAciertos: UtilidadesEntradasValores.obtenerListaFromTxtAciertos(p[2])))
        }
        return r
    }

    static func condicion(_ j: CondJ) throws(ErrorDominio) -> CondicionColProbables {
        var c = CondicionColProbables()
        c.activa = j.activa ?? true
        var cps: [ColumnaProbableConfig] = []
        for x in j.cps { cps.append(try cp(x)) }
        c.ponerColumnas(cps)
        for x in j.controles ?? [] {
            var k = CPControlFallosConfig()
            try k.ponerColumnas(x.columnas)
            k.tolerancias = x.tolerancias
            try k.ponerAciertos(x.aciertos)
            c.controlesFallos.append(k)
        }
        try c.ponerFallosPermitidos(j.fallosPermitidos ?? "")
        for x in j.relaciones1 ?? [] {
            var r = RelacionCP1Config()
            try r.ponerColumnas(x.columnas)
            if let s = x.sumaAciertos { try r.ponerSumaAciertos(s) }
            if let s = x.recorridos { try r.ponerRecorridos(s) }
            if let s = x.cantidadCP { try r.ponerCantidadCP(s) }
            if let s = x.cuantosAC { try r.ponerCuantosAC(s) }
            c.relaciones1.append(r)
        }
        for x in j.relaciones2 ?? [] {
            var r = RelacionCP2Config()
            r.columnasA = x.columnasA ?? ""
            r.columnasB = x.columnasB ?? ""
            r.aciertos = x.aciertos ?? ""
            r.concepto = x.concepto ?? ""
            r.cantidad = x.cantidad ?? ""
            r.columnasA2 = x.columnasA2 ?? ""
            r.columnasB2 = x.columnasB2 ?? ""
            r.aciertos2 = x.aciertos2 ?? ""
            r.concepto2 = x.concepto2 ?? ""
            r.cantidad2 = x.cantidad2 ?? ""
            c.relaciones2.append(r)
        }
        for x in j.relaciones3 ?? [] {
            var r = RelacionCP3Config()
            r.columnasImplicadas = x.columnas
            r.concepto = x.concepto
            let l = cps.count / 3
            let k = UtilidadesEntradasValores.obtenerListaFromTxt(x.columnas).count
            typealias U = UtilidadesEntradasValores
            r.escalerasAsc = try x.escalerasAsc.flatMap { s throws(ErrorDominio) in try U.obtenerBoolArrayFromTxt(s, longitud: l) }
            r.escalerasDesc = try x.escalerasDesc.flatMap { s throws(ErrorDominio) in try U.obtenerBoolArrayFromTxt(s, longitud: l) }
            r.escalerasTotales = try x.escalerasTotales.flatMap { s throws(ErrorDominio) in
                try U.obtenerBoolArrayFromTxt(s, longitud: l)
            }
            r.sandwichs = try x.sandwichs.flatMap { s throws(ErrorDominio) in try U.obtenerBoolArrayFromTxt(s, longitud: k / 4) }
            r.agrupacionesPasoFijo = try agrupaciones(x.pasoFijo)
            r.agrupacionesSolapadas = try agrupaciones(x.solapadas)
            c.relaciones3.append(r)
        }
        return c
    }

    // MARK: ColumnaProbable (G04-CP-*)

    struct EntradaCP: Decodable, Sendable {
        var cp: CPJ
        var puntos: [Int]?
        var columnas: [String]?
    }
    struct SalidaCP: Decodable, Sendable {
        var validas: [Bool]?
        var contadores: [[Int]]?
        var tolerancias: [String: Int]?
        var texto: String?
        var bytes: [Int]?
        var error: String?
    }

    @Test func columnaProbable() throws {
        for caso in try Golden.casos("motor/cp/columnaprobable", EntradaCP.self, SalidaCP.self) {
            let s = try #require(caso.salida)
            let config: ColumnaProbableConfig
            do {
                config = try Self.cp(caso.entrada.cp)
            } catch {
                #expect(error.excepcion.rawValue == s.error, "\(caso.id)")
                continue
            }
            #expect(s.error == nil, "\(caso.id): se esperaba error")
            if let b = s.bytes { #expect(config.pronosticos.map(Int.init) == b, "\(caso.id)") }
            let p = caso.entrada.puntos ?? [6, 3, 1]
            let cp = ColumnaProbable(config, numeroPartidos: Self.n, puntos: PuntosCP(fijos: p[0], dobles: p[1], triples: p[2]))
            var validas: [Bool] = []
            var contadores: [[Int]] = []
            for t in caso.entrada.columnas ?? [] {
                validas.append(try cp.analizar(FiltrosTests.columna(t)))
                contadores.append([cp.noAC, cp.noACS, cp.noFS, cp.noPuntos])
            }
            if let v = s.validas { #expect(validas == v, "\(caso.id)") }
            if let c = s.contadores { #expect(contadores == c, "\(caso.id)") }
            for (letras, k) in s.tolerancias ?? [:] {
                #expect(try cp.obtenNoAciertosTolerancias(letras) == k, "\(caso.id) \(letras)")
            }
            if let t = s.texto, let ultima = caso.entrada.columnas?.last {
                #expect(try cp.texto(FiltrosTests.columna(ultima), 1) == t, "\(caso.id)")
            }
        }
    }

    // MARK: FiltroColProbables (control de fallos y relaciones)

    struct EntradaF: Decodable, Sendable {
        var condicion: CondJ
        var columnas: [String]
        /// `"fallos"`: solo `analizarFallos` por columna (P16).
        var modo: String?
    }
    struct SalidaF: Decodable, Sendable {
        var validas: [Bool]?
        var contadores: [[Int]]?
        var fallos: [String]?
        var sinFallos: Bool?
        var fallosPorColumna: [[String]?]?
        var acumuladas: [Int]?
        var textosRelaciones: [String]?
        var relacion1: [String: Int]?
        var listaCPs: [Int]?
        var error: String?
    }

    @Test func filtro() throws {
        for caso in try Golden.casos("motor/cp/filtro", componente: "FiltroColProbables", EntradaF.self, SalidaF.self) {
            let s = try #require(caso.salida)
            let e = caso.entrada
            do {
                let f = FiltroColProbables(
                    try Self.condicion(e.condicion), analisisActivo: false, numeroPartidos: Self.n,
                    puntos: PuntosCP(fijos: 6, dobles: 3, triples: 1))
                if e.modo == "fallos" {
                    let r = try e.columnas.map { t throws(ErrorDominio) in try f.analizarFallos(FiltrosTests.columna(t)) }
                    #expect(r == s.fallosPorColumna, "\(caso.id)")
                    continue
                }
                var validas: [Bool] = []
                var contadores: [[Int]] = []
                for t in e.columnas {
                    validas.append(try f.analizar(FiltrosTests.columna(t)))
                    contadores.append(f.cps.flatMap { [$0.noAC, $0.noACS, $0.noFS, $0.noPuntos] })
                }
                #expect(s.error == nil, "\(caso.id): se esperaba error")
                if let v = s.validas { #expect(validas == v, "\(caso.id)") }
                if let c = s.contadores { #expect(contadores == c, "\(caso.id)") }
                if let a = s.acumuladas { #expect(f.controles.map(\.toleranciasAcumuladas) == a, "\(caso.id)") }
                if let r = s.relacion1, let r1 = f.relaciones1.first {
                    let estado = [
                        "noColConMismosAC": r1.noColConMismosAC, "sumaTemp": r1.sumaTemp, "recorrido": r1.recorrido,
                    ]
                    #expect(estado.filter { r[$0.key] != nil } == r, "\(caso.id)")
                }
                if let l = s.listaCPs { #expect(f.relaciones1.first?.listaCPs == l, "\(caso.id)") }
                if let t = s.textosRelaciones {
                    var r: [String] = []
                    for (i, x) in f.relaciones1.enumerated() { r.append(try x.texto(i + 1)) }
                    for (i, x) in f.relaciones2.enumerated() { r.append(try x.texto(i + 1)) }
                    #expect(r == t, "\(caso.id)")
                }
                if let ultima = e.columnas.last {
                    if let fallos = s.fallos { #expect(try f.analizarFallos(FiltrosTests.columna(ultima)) == fallos, "\(caso.id)") }
                    if s.sinFallos == true { #expect(try f.analizarFallos(FiltrosTests.columna(ultima)) == nil, "\(caso.id)") }
                }
            } catch {
                #expect(error.excepcion.rawValue == s.error, "\(caso.id) \(error)")
            }
        }
    }

    // MARK: Relación I heredada (CP-CP-02, 03): 243 columnas sintéticas

    struct EntradaR1: Decodable, Sendable {
        var cps: [CPJ]
        var relaciones: [Relacion1J]
    }
    struct SalidaR1: Decodable, Sendable {
        var validez: String
        var sha256Lineas: String
        var textosDistintos: [String]
    }

    /// `ColumnaSintetica(indice, 5)` de `RelacionCP1GoldenTests.cs:84-96`.
    static func sintetica(_ indice: Int) -> Columna {
        var (col, i): (Columna, Int) = (0, indice)
        for p in 0..<n {
            var d = 0
            if p < 5 {
                d = i % 3
                i /= 3
            }
            col |= Int64(d == 0 ? 4 : d == 1 ? 2 : 1) << (3 * p)
        }
        return col
    }

    @Test func relacionesIHeredadas() throws {
        for caso in try Golden.casos("motor/cp/relacioncp1", EntradaR1.self, SalidaR1.self) {
            let s = try #require(caso.salida)
            let j = CondJ(cps: caso.entrada.cps, relaciones1: caso.entrada.relaciones)
            let f = FiltroColProbables(
                try Self.condicion(j), analisisActivo: false, numeroPartidos: Self.n,
                puntos: PuntosCP(fijos: 6, dobles: 3, triples: 1))
            var lineas: [String] = []
            var validez = ""
            var distintos = Set<String>()
            for idx in 0..<243 {
                let valido = try f.analizar(Self.sintetica(idx))
                var txt = ""
                for (i, r) in f.relaciones1.enumerated() { txt += try r.texto(i + 1) }
                lineas.append("\(idx);\(valido ? "1" : "0");\(txt)")
                validez += valido ? "1" : "0"
                if !txt.isEmpty { distintos.insert(txt) }
            }
            #expect(validez == s.validez, "\(caso.id)")
            #expect(Golden.sha256(lineas.joined(separator: "\n")).uppercased() == s.sha256Lineas, "\(caso.id)")
            #expect(distintos.sorted { $0.utf16.lexicographicallyPrecedes($1.utf16) } == s.textosDistintos, "\(caso.id)")
        }
    }

    // MARK: Relación III sobre una serie explícita (G04-R3-*)

    struct EntradaR3: Decodable, Sendable {
        var serie: [Int]
        var escalerasAsc, escalerasDesc, escalerasTotales, sandwichs: String?
        /// L de los arrays de escaleras y sándwiches.
        var l: Int?
        var pasoFijo, solapadas: [String]?
    }
    struct SalidaR3: Decodable, Sendable {
        var escaleras: [[Int]]?
        var ascendentes: Int?
        var sandwichs: [[Int]]?
        /// Celdas no nulas `[longitud, valor, n]`.
        var pasoFijo, solapadas: [[Int]]?
        var valida: Bool?
    }

    static func celdas(_ m: [[Int]]) -> [[Int]] {
        m.indices.flatMap { i in m[i].indices.filter { m[i][$0] != 0 }.map { [i, $0, m[i][$0]] } }
    }

    @Test func relacionIIISerie() throws {
        for caso in try Golden.casos("motor/cp/relacioncp3", componente: "RelacionCP3", EntradaR3.self, SalidaR3.self) {
            let s = try #require(caso.salida)
            let e = caso.entrada
            var c = RelacionCP3Config()
            let l = e.l ?? 0
            typealias U = UtilidadesEntradasValores
            c.escalerasAsc = try e.escalerasAsc.flatMap { x throws(ErrorDominio) in try U.obtenerBoolArrayFromTxt(x, longitud: l) }
            c.escalerasDesc = try e.escalerasDesc.flatMap { x throws(ErrorDominio) in try U.obtenerBoolArrayFromTxt(x, longitud: l) }
            c.escalerasTotales = try e.escalerasTotales.flatMap { x throws(ErrorDominio) in
                try U.obtenerBoolArrayFromTxt(x, longitud: l)
            }
            c.sandwichs = try e.sandwichs.flatMap { x throws(ErrorDominio) in try U.obtenerBoolArrayFromTxt(x, longitud: l) }
            c.agrupacionesPasoFijo = try Self.agrupaciones(e.pasoFijo)
            c.agrupacionesSolapadas = try Self.agrupaciones(e.solapadas)
            let r = RelacionCP3(serie: e.serie, c)
            if let x = s.escaleras { #expect(try r.escaleras() == x, "\(caso.id)") }
            if let x = s.ascendentes { #expect(RelacionCP3.ascendentes(try r.escaleras()) == x, "\(caso.id)") }
            if let x = s.sandwichs { #expect(try r.sandwichs() == x, "\(caso.id)") }
            if let x = s.pasoFijo { #expect(Self.celdas(try r.pasoFijo()) == x, "\(caso.id)") }
            if let x = s.solapadas { #expect(Self.celdas(try r.solapadas()) == x, "\(caso.id)") }
            if let x = s.valida { #expect(try r.analiza() == x, "\(caso.id)") }
        }
    }
}

extension ColProbablesTests.CondJ {
    init(cps: [ColProbablesTests.CPJ], relaciones1: [ColProbablesTests.Relacion1J]) {
        self.cps = cps
        self.relaciones1 = relaciones1
    }
}
