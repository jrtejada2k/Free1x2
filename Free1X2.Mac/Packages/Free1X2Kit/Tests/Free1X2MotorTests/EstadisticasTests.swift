import Free1X2Base
import Free1X2Motor
import Free1X2Pruebas
import Testing

/// ESPEC_17 EST: estadísticas de filtro y de Anastatics (ESPEC_03 §16-§20) — suite `motor/estadisticas/anastatics`.
@Suite("Estadísticas")
struct EstadisticasTests {
    struct Fichero: Decodable, Sendable {
        var nombre: String
        var lineas: [String]
    }
    struct Entrada: Decodable, Sendable {
        var columnas: [String]?
        var variantes: String?
        var ficheros: [Fichero]?
    }
    struct Salida: Decodable, Sendable {
        var dibujos, staSigSeg, staInter, dibRepes: [[Int]]?
        var archivos: [String]?
        var bits: [String]?
    }

    @Test func anastatics() throws {
        for caso in try Golden.casos("motor/estadisticas/anastatics", componente: "Anastatics", Entrada.self, Salida.self) {
            let cols = try #require(caso.entrada.columnas)
            let s = try #require(caso.salida)
            if let v = s.dibujos { #expect(try cols.map { c throws(ErrorDominio) in try Anastatics.dibujos(c) } == v, "\(caso.id)") }
            if let v = s.staSigSeg {
                #expect(try cols.map { c throws(ErrorDominio) in try Anastatics.staSigSeg(c) } == v, "\(caso.id)")
            }
            if let v = s.staInter { #expect(try cols.map { c throws(ErrorDominio) in try Anastatics.staInter(c) } == v, "\(caso.id)") }
            if let v = s.dibRepes {
                var rep = Anastatics.DibRepes()
                var r: [[Int]] = []
                for c in cols { r.append(try rep.procesar(c)) }
                #expect(r == v, "\(caso.id)")
            }
        }
    }

    /// 03 §20: `mV` es la mayor racha de signos distintos de `1`; `m1/mX/m2`, las de cada signo.
    @Test func staSigSegCumpleSuDefinicion() throws {
        var g = SystemRandomNumberGenerator()
        for _ in 0..<2000 {
            let c = String((0..<14).map { _ in "1X2".randomElement(using: &g) ?? "1" })
            func racha(_ p: (Character) -> Bool) -> Int {
                var (m, n) = (0, 0)
                for ch in c {
                    n = p(ch) ? n + 1 : 0
                    m = max(m, n)
                }
                return m
            }
            #expect(
                try Anastatics.staSigSeg(c) == [0, racha { $0 != "1" }, racha { $0 == "1" }, racha { $0 == "X" }, racha { $0 == "2" }],
                "\(c)")
        }
    }

    @Test func procesoYValidacion() throws {
        #expect(Anastatics.verColumna("1x2111111111XX") == "1X2111111111XX")
        #expect(Anastatics.verColumna("1X21111111111") == nil)
        #expect(Anastatics.verColumna("1X2111111111X*") == nil)
        let m = try Anastatics.proceso(.dibujos, columnas: ["XXXXXXXXXXXXXX", "1X2112X1X2211X"])
        #expect(m[14][0] == 1 && m[4][4] == 1)
    }

    @Test func cumplimientoDeFiltro() throws {
        let caso = try Golden.caso("EST-L1", en: "motor/estadisticas/anastatics", Entrada.self, Salida.self)
        var c = CondicionesGrupo(numeroPartidos: 14)
        try c.noVariantes.setNoVariantes(try #require(caso.entrada.variantes))
        try c.noVariantes.llenarTodosValores()
        let r = try CalculadorEstadisticas.estadisticas(
            .noVariantes, condiciones: c, configuracion: .porDefecto,
            ficheros: try #require(caso.entrada.ficheros).map { (nombre: $0.nombre, lineas: $0.lineas) })
        let s = try #require(caso.salida)
        #expect(r.map(\.archivo) == s.archivos)
        #expect(r.map { "0x" + String($0.cumplimiento.bitPattern, radix: 16, uppercase: true).leftPad(8) } == s.bits)
    }
}

extension String {
    fileprivate func leftPad(_ n: Int) -> String { String(repeating: "0", count: max(0, n - count)) + self }
}
