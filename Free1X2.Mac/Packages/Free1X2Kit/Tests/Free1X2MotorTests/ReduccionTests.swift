import Free1X2Base
import Free1X2Motor
import Free1X2Pruebas
import Testing

/// ESPEC_17 RED: reductores JDC, JDCDobleContador y Redu1305Xfsf — suites `motor/reduccion/*`.
@Suite("Reducción")
struct ReduccionTests {
    struct Entrada: Decodable, Sendable {
        struct Universo: Decodable, Sendable {
            var triples: Int?
            var pronosticos: [String]?
        }
        var fichero: String?
        var universo: Universo?
        var nivel: Int
        var maxCol: Int
        var percent: Int
        var externas: Bool
    }
    struct Salida: Decodable, Sendable {
        var iniciales, finales, procesadas: Int?
        var lineas: [String]?
        var lineasOrdenadas: [String]?
        var numLineas: Int?
        var sha256Lineas: String?
        var finalesMin, finalesMax: Int?
        var lineasIgualAFinales: Bool?
        var repetida: String?
    }

    static let suites = ["motor/reduccion/jdc", "motor/reduccion/reductores"]

    /// «Universo kT» (`T-JDCD:23-31`): el fichero que graba el `Analizador` (ruta B) con esos pronósticos.
    static func madre(_ e: Entrada) throws -> [String] {
        if let f = e.fichero { return try Golden.columnas(f) }
        let u = try #require(e.universo)
        let pronosticos = u.pronosticos ?? (0..<14).map { $0 < (u.triples ?? 0) ? "1,X,2" : "1" }
        var c = Combinacion(numeroPartidos: 14)
        c.ponerPronosticos(pronosticos)
        var lineas: [String] = []
        _ = try MotorCalculo.calcular(c, configuracion: .porDefecto, leerLineas: { _ in [] }) { col throws(ErrorDominio) in
            lineas.append(try UtilColumnas.convLongToStr(col))
        }
        return lineas
    }

    static func algoritmo(_ componente: String) throws -> AlgoritmoReduccion {
        switch componente {
        case "JDC": .menosTiempo
        case "JDCDobleContador": .menosColumnas
        case "Redu1305Xfsf": .grandesArchivos
        default: throw ErrorDominio(.argumento, "componente \(componente)")
        }
    }

    /// GR-14: con `maxCol = 0` y `percent = 100` toda columna de la madre tiene una reductora a distancia ≤ 14 − nivel.
    static func cubre(_ madre: [String], _ salida: [String], nivel: Int) -> Bool {
        madre.allSatisfy { m in
            salida.contains { r in zip(m.utf16, r.utf16).count(where: { $0 == $1 }) >= nivel }
        }
    }

    @Test(arguments: suites)
    func reducir(_ suite: String) throws {
        for caso in try Golden.casos(suite, Entrada.self, Salida.self) {
            let e = caso.entrada
            let madre = try Self.madre(e)
            let r = try Reduccion.reducir(
                try Self.algoritmo(caso.componente), lineas: madre, nivel: e.nivel, maxCol: e.maxCol, percent: e.percent,
                externas: e.externas, numeroPartidos: 14)
            let s = try #require(caso.salida)
            if let v = s.iniciales { #expect(r.iniciales == v, "\(caso.id) iniciales") }
            if let v = s.finales { #expect(r.finales == v, "\(caso.id) finales") }
            if let v = s.procesadas { #expect(r.procesadas == v, "\(caso.id) procesadas") }
            if let v = s.lineas { #expect(r.lineas == v, "\(caso.id)") }
            if let v = s.lineasOrdenadas { #expect(r.lineas.sorted() == v, "\(caso.id)") }
            if let v = s.numLineas { #expect(r.lineas.count == v, "\(caso.id) nº de líneas") }
            if let v = s.sha256Lineas { #expect(Golden.sha256(r.lineas.joined(separator: "\n")) == v, "\(caso.id)") }
            if let v = s.finalesMin { #expect(r.finales >= v, "\(caso.id)") }
            if let v = s.finalesMax { #expect(r.finales <= v, "\(caso.id)") }
            if s.lineasIgualAFinales == true { #expect(r.lineas.count == r.finales, "\(caso.id)") }
            if let v = s.repetida { #expect(r.lineas.count { $0 == v } == 2, "\(caso.id)") }
            if e.maxCol == 0 && e.percent == 100 {
                #expect(Self.cubre(madre, r.lineas, nivel: e.nivel), "\(caso.id): GR-14 cobertura")
                #expect(r.finales <= r.iniciales || e.externas, "\(caso.id): Finales ≤ Iniciales")
            }
        }
    }

    struct EntradaSubida: Decodable, Sendable {
        var madre: [String]
        /// Partidos 1-based.
        var involucrados: [Int]
        var niveles: [Int]
        var seguidos: Int
        var externas: [String]?
    }
    struct SalidaSubida: Decodable, Sendable {
        var iniciales, noColumnas: Int?
        var lineas: [String]?
    }

    /// GS-*: casillas de la pantalla (16 partidos, niveles 0…16).
    @Test func subirCategoria() throws {
        for caso in try Golden.casos("motor/reduccion/subircategoria", EntradaSubida.self, SalidaSubida.self) {
            let e = caso.entrada
            var sc = try SubidaCategoria(lineas: e.madre)
            let s = try #require(caso.salida)
            if let v = s.iniciales { #expect(sc.noColumnas == v, "\(caso.id) I=") }
            let involucrados = (1...16).map { e.involucrados.contains($0) }
            let niveles = (0...16).map { e.niveles.contains($0) }
            try sc.calcular(
                involucrados: involucrados, niveles: niveles, seguidos: e.seguidos, externas: e.externas, numeroPartidos: 14)
            if let v = s.noColumnas { #expect(sc.noColumnas == v, "\(caso.id) F=") }
            if let v = s.lineas { #expect(try sc.lineas() == v, "\(caso.id)") }
        }
    }

    @Test func cancelarAntesDeEmpezarNoDejaReductoras() async throws {
        let madre = try Golden.columnas("Fixtures/cols_a.txt")
        let t = Task {
            withUnsafeCurrentTask { $0?.cancel() }
            return try Reduccion.reducir(.menosTiempo, lineas: madre, nivel: 13, numeroPartidos: 14)
        }
        let r = try await t.value
        #expect(r.lineas.isEmpty, "05 §2.1 P5: cancelar en la fase 1 ⇒ sin reductoras ni fichero")
    }
}
