import Free1X2Base
import Free1X2Pruebas
import Testing

/// ESPEC_12 G-NET-RND: `new System.Random(semilla)` de .NET 8 — suite `net/aleatorio`.
@Suite("AleatorioNET")
struct AleatorioTests {
    struct Entrada: Decodable, Sendable {
        var semilla: Int32
    }
    struct Salida: Decodable, Sendable {
        var next, next100, nextMenos5a5, nextRangoGrande: [Int32]
        var nextDouble: [String]
    }

    @Test func secuenciasDeNET() throws {
        for caso in try Golden.casos("net/aleatorio", Entrada.self, Salida.self) {
            let s = try #require(caso.salida)
            let semilla = caso.entrada.semilla
            var a = AleatorioNET(semilla: semilla)
            #expect((0..<10).map { _ in a.next() } == s.next, "\(caso.id)")
            a = AleatorioNET(semilla: semilla)
            var r: [Int32] = []
            for _ in 0..<10 { r.append(try a.next(100)) }
            #expect(r == s.next100, "\(caso.id)")
            a = AleatorioNET(semilla: semilla)
            r = []
            for _ in 0..<10 { r.append(try a.next(-5, 5)) }
            #expect(r == s.nextMenos5a5, "\(caso.id)")
            a = AleatorioNET(semilla: semilla)
            r = []
            for _ in 0..<6 { r.append(try a.next(Int32.min, Int32.max)) }
            #expect(r == s.nextRangoGrande, "\(caso.id)")
            a = AleatorioNET(semilla: semilla)
            let d = (0..<5).map { _ in "0x" + String(a.nextDouble().bitPattern, radix: 16, uppercase: true) }
            #expect(d.map { "0x" + String(repeating: "0", count: 18 - $0.count) + $0.dropFirst(2) } == s.nextDouble, "\(caso.id)")
        }
    }

    @Test func argumentosFueraDeRango() {
        var a = AleatorioNET(semilla: 1)
        #expect(throws: ErrorDominio.self) { try a.next(-1) }
        #expect(throws: ErrorDominio.self) { try a.next(3, 2) }
    }
}
