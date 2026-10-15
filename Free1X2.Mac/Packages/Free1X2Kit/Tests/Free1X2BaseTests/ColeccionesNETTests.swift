import Free1X2Base
import Free1X2Pruebas
import Testing

/// ESPEC_17 NET-12 y NET-15, G-NET-SORT: `Array.Sort` y `Hashtable.Keys` de .NET 8.
@Suite("Colecciones .NET")
struct ColeccionesNETTests {
    struct Entrada: Decodable, Sendable {
        var dobles: [DoubleGolden]?
        var claves: Claves?
    }
    struct Salida: Decodable, Sendable {
        var dobles: [DoubleGolden]?
        var orden: Claves?
        var cubos: Int?
    }
    /// `claves`/`orden` son enteros (NET-SORT) o claves encajadas `"i32:5"` (NET-15).
    enum Claves: Decodable, Sendable {
        case enteros([Int])
        case textos([String])

        init(from decoder: any Decoder) throws {
            let c = try decoder.singleValueContainer()
            if let l = try? c.decode([Int].self) { self = .enteros(l) } else { self = .textos(try c.decode([String].self)) }
        }

        var enteros: [Int] { if case .enteros(let l) = self { l } else { [] } }
        var textos: [String] { if case .textos(let l) = self { l } else { [] } }
    }

    static func casos(_ componente: String) throws -> [CasoGolden<Entrada, Salida>] {
        try Golden.casos("net/colecciones", componente: componente)
    }

    @Test func dobles() throws {
        for caso in try Self.casos("IntrosortNET.doubles") {
            var a = (caso.entrada.dobles ?? []).compactMap(\.valor)
            IntrosortNET.ordenar(&a)
            #expect(a.map(\.bitPattern) == caso.salida?.dobles?.compactMap { $0.valor?.bitPattern }, "\(caso.id)")
        }
    }

    @Test func empatesEnElOrdenDeNET() throws {
        for caso in try Self.casos("IntrosortNET.empates") {
            var pares = (caso.entrada.claves?.enteros ?? []).enumerated().map { ($1, $0) }
            IntrosortNET.ordenar(&pares) { $0.0 < $1.0 ? -1 : ($0.0 > $1.0 ? 1 : 0) }
            #expect(pares.map(\.1) == caso.salida?.orden?.enteros, "\(caso.id)")
        }
    }

    @Test func ordenaComoLaBiblioteca() {
        var generador = SystemRandomNumberGenerator()
        for n in [0, 1, 2, 3, 16, 17, 100, 5000] {
            var a = (0..<n).map { _ in Int.random(in: -50...50, using: &generador) }
            let esperado = a.sorted()
            IntrosortNET.ordenar(&a)
            #expect(a == esperado, "n=\(n)")
        }
    }

    @Test func clavesDelHashtable() throws {
        for caso in try Self.casos("HashtableNET.claves") {
            var t = HashtableNET<Int>()
            for texto in caso.entrada.claves?.textos ?? [] {
                let clave = try #require(Self.clave(texto))
                try t.anadir(clave, 0)
            }
            #expect(t.claves.map(\.description) == caso.salida?.orden?.textos, "\(caso.id)")
            #expect(t.numeroDeCubos == caso.salida?.cubos, "\(caso.id)")
        }
    }

    @Test func clavesDeOtroTipoNoSeEncuentran() throws {
        var t = HashtableNET<String>()
        try t.asignar("largo", a: .int64(5))
        try t.anadir(.int32(5), "corto")
        #expect(t[.int64(5)] == "largo" && t[.int32(5)] == "corto" && t.count == 2)
        #expect(!t.contiene(.int32(6)) && t[.int64(6)] == nil)
        #expect(throws: ErrorDominio.self) { try t.anadir(.int32(5), "otra vez") }
    }

    static func clave(_ texto: String) -> HashtableNET<Int>.Clave? {
        let partes = texto.split(separator: ":")
        guard partes.count == 2 else { return nil }
        switch partes[0] {
        case "i32": return Int32(partes[1]).map { .int32($0) }
        case "i64": return Int64(partes[1]).map { .int64($0) }
        default: return nil
        }
    }
}
