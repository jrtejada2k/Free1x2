import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// DT-84: máscaras guardadas y su escrutinio contra el histórico.
struct MascarasTests {
    /// JD-01 del plan: partidos 1–6 triples y 7–14 = «1» (729 columnas).
    static let jd01 = Mascara(nombre: "JD-01", pronosticos: Array(repeating: "1,X,2", count: 6) + Array(repeating: "1", count: 8))

    /// Contra la columna de unos, los valores del plan calculados a mano (§3): 1, 12, 60, 160, 240, 192 y 64.
    @Test func aciertosSinDesarrollar() {
        let a = CasosMascaras.aciertos(Self.jd01.pronosticos, ganadora: String(repeating: "1", count: 14))
        #expect(Array(a[8...14]) == [64, 192, 240, 160, 60, 12, 1] && a.reduce(0, +) == 729)
        #expect(Self.jd01.columnas == 729 && Self.jd01.resumen == (8, 0, 6))
    }

    /// Suma por categoría en todas las jornadas; la de doses (6 aciertos como mucho) no premia.
    @Test func escrutarContraHistorico() {
        let r = CasosMascaras.escrutar(Self.jd01, ganadoras: [String(repeating: "1", count: 14), String(repeating: "2", count: 14)])
        #expect(r.premios == [14: 1, 13: 12, 12: 60, 11: 160, 10: 240])
        #expect(r.jornadasConPremio == 1 && r.jornadas == 2 && r.columnas == 729)
    }

    @Test func guardarListarYBorrar() throws {
        let entorno = Entorno.pruebas()
        try CasosMascaras.guardar(Mascara(nombre: "  Mi 4D-2T ", pronosticos: Self.jd01.pronosticos), entorno: entorno)
        #expect(CasosMascaras.existe("Mi 4D-2T", entorno: entorno))
        #expect(
            CasosMascaras.listar(numeroPartidos: 14, entorno: entorno) == [Mascara(nombre: "Mi 4D-2T", pronosticos: Self.jd01.pronosticos)])
        #expect(CasosMascaras.listar(numeroPartidos: 15, entorno: entorno).isEmpty)
        #expect(throws: ErrorDominio.self) { try CasosMascaras.guardar(Mascara(nombre: "a/b", pronosticos: []), entorno: entorno) }
        #expect(CasosMascaras.nombreValido("   ") == nil)
        try CasosMascaras.borrar("Mi 4D-2T", entorno: entorno)
        #expect(CasosMascaras.listar(numeroPartidos: 14, entorno: entorno).isEmpty)
    }

    @Test func historicoDelServicio() async throws {
        let json =
            #"{"pais":"ES","resultados":[{"jornada":20261011,"fecha":"2026-10-11","ganadora":"1x21X1121X1X21"},"#
            + #"{"jornada":20261004,"fecha":"2026-10-04","ganadora":"11111111111111"}]}"#
        let url = try #require(URL(string: "https://clubprogol.com/wp-json/clubprogol/v1/quiniela/es/historico"))
        let e = Entorno.pruebas(red: ClienteHTTPGrabado(respuestas: [url: .init(cuerpo: Data(json.utf8))]))
        let h = try await CasosArchivo.servicioOnline(entorno: e).obtenerHistorico(pais: "es")
        #expect(h.map(\.ganadora) == ["1X21X1121X1X21", "11111111111111"] && h.first?.jornada == 20_261_011)
        #expect(throws: ErrorDominio.self) {
            try HistoricoQuiniela.analizar(#"{"resultados":[{"jornada":1,"ganadora":"1X2"}]}"#)
        }
    }

    /// Respuestas reales de clubprogol (16-oct-2026): España 19 jornadas y México 24 (con `fecha: null`). JD-01 contra
    /// las de España da lo mismo que desarrollar sus 729 columnas en Python: 3 de 11 y 43 de 10, en 10 de 19 jornadas.
    @Test func historicoReal() throws {
        func leer(_ p: String) throws -> [ResultadoHistorico] {
            try HistoricoQuiniela.analizar(
                String(decoding: try Golden.bytes("Fixtures/online/historico-\(p)-2026-10-16.json"), as: UTF8.self))
        }
        let es = try leer("es")
        let mx = try leer("mx")
        #expect(es.count == 19 && es.first?.jornada == 20_261_014 && es.first?.ganadora == "11122X12112X2X")
        #expect(mx.count == 24 && mx.last?.jornada == 2331 && mx.last?.fecha == "")
        let r = CasosMascaras.escrutar(Self.jd01, ganadoras: es.map(\.ganadora))
        #expect(r.premios == [11: 3, 10: 43] && r.jornadasConPremio == 10 && r.jornadas == 19)
    }
}
