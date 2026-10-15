import Free1X2Online
import Free1X2Pruebas
import Testing

/// CP-ONL-17 (golden `test`, `JornadaCacheRoundTripTests.cs:26-63`): nombre del fichero de caché por país.
@Test func ficheroDeCacheComoElGolden() throws {
    struct Entrada: Decodable, Sendable { let pais: String }
    struct Salida: Decodable, Sendable { let nombreFichero: String }

    let caso = try Golden.caso("CP-ONL-17", en: "online/parsers", Entrada.self, Salida.self)
    let pais = try #require(PaisQuiniela(rawValue: caso.entrada.pais))
    #expect(pais.ficheroCache == caso.salida?.nombreFichero)
    #expect(pais.codigoContrato == "ES")
}
