import CryptoKit
import Foundation
import Free1X2Pruebas
import Testing

/// ESPEC_17 INF-01 e INF-02: integridad del oráculo por lectura antes de usarlo como criterio de aceptación.
@Suite("Índice golden")
struct GoldenIndiceTests {
    struct Indice: Decodable {
        struct Suite: Decodable {
            let ruta: String
            let casos: Int
        }
        struct Fixture: Decodable {
            let ruta: String
            let origen: String
            let sha256: String
        }
        let suites: [Suite]
        let fixtures: [Fixture]
    }

    let indice: Indice

    init() throws {
        indice = try JSONDecoder().decode(Indice.self, from: Golden.bytes("Golden/indice.json"))
    }

    @Test("INF-01: cada suite tiene sus casos y cada caso es válido")
    func suitesValidas() throws {
        var ids = Set<String>()
        for suite in indice.suites {
            let nombre = String(suite.ruta.dropLast(".jsonl".count))
            let lineas = try Golden.lineas(nombre)
            #expect(lineas.count == suite.casos, "\(suite.ruta): \(lineas.count) casos, el índice dice \(suite.casos)")
            for (n, linea) in lineas.enumerated() {
                let donde = "\(suite.ruta):\(n + 1)"
                let caso = try #require(try JSONSerialization.jsonObject(with: linea) as? [String: Any], "\(donde) no es un objeto")
                let id = try #require(caso["id"] as? String, "\(donde) sin id")
                #expect(ids.insert(id).inserted, "\(donde): id repetido \(id)")
                let fuente = try #require(FuenteGolden(rawValue: caso["fuente"] as? String ?? ""), "\(donde): fuente no válida")
                #expect(caso["componente"] is String, "\(donde): sin componente")
                #expect(caso["entrada"] != nil, "\(donde): sin entrada")
                #expect((caso["salida"] != nil) != (caso["error"] != nil), "\(donde): debe tener salida o error, no ambos")
                if fuente == .test { #expect(caso["cita"] is String, "\(donde): un caso `test` necesita cita") }
                if fuente == .lectura { #expect(caso["traza"] is [String], "\(donde): un caso `lectura` necesita traza") }
                for ruta in Self.rutasDeFixtures(caso["entrada"] as Any) {
                    #expect((try? Golden.url(ruta)) != nil, "\(donde): no existe \(ruta)")
                }
            }
        }
    }

    @Test("INF-03: todo componente con casos `modelo` tiene un caso `lectura` o `test` que lo contrasta (ESPEC_12 §1)")
    func modelosContrastados() throws {
        var fuentes: [String: Set<String>] = [:]
        for suite in indice.suites {
            for linea in try Golden.lineas(String(suite.ruta.dropLast(".jsonl".count))) {
                let caso = try #require(try JSONSerialization.jsonObject(with: linea) as? [String: Any])
                fuentes[caso["componente"] as? String ?? "", default: []].insert(caso["fuente"] as? String ?? "")
            }
        }
        for (componente, f) in fuentes where f.contains("modelo") {
            #expect(f.contains("lectura") || f.contains("test"), "\(componente): solo `modelo`, falta un caso `lectura`")
        }
    }

    @Test("INF-01: todo .jsonl de Golden/ está en el índice")
    func sinSuitesHuerfanas() throws {
        let raiz = try Golden.url("Golden")
        let enDisco = try #require(FileManager.default.enumerator(at: raiz, includingPropertiesForKeys: nil))
            .compactMap { $0 as? URL }
            .filter { $0.pathExtension == "jsonl" }
            .compactMap { $0.path(percentEncoded: false).components(separatedBy: "/Golden/").last }
        #expect(Set(enDisco) == Set(indice.suites.map(\.ruta)))
    }

    @Test("INF-02: los fixtures son copias exactas de los del repo C#")
    func fixturesIntactos() throws {
        #expect(!indice.fixtures.isEmpty)
        for fixture in indice.fixtures {
            let resumen = SHA256.hash(data: try Golden.bytes(fixture.ruta)).map { String(format: "%02x", $0) }.joined()
            #expect(resumen == fixture.sha256, "\(fixture.ruta) (origen \(fixture.origen)): SHA-256 \(resumen)")
        }
    }

    /// Cadenas `"Fixtures/…"` que aparecen en la entrada de un caso.
    static func rutasDeFixtures(_ valor: Any) -> [String] {
        switch valor {
        case let texto as String: texto.hasPrefix("Fixtures/") ? [texto] : []
        case let lista as [Any]: lista.flatMap(rutasDeFixtures)
        case let objeto as [String: Any]: objeto.values.flatMap(rutasDeFixtures)
        default: []
        }
    }
}
