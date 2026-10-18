import Free1X2Base
import Free1X2Pruebas
import Testing

@testable import Free1X2Motor

/// ESPEC_17 CP-14: generador de CPs (04 §16) y su salida F06 — suite `motor/cp/generador`.
@Suite("Generador de CPs")
struct GeneradorCPsTests {
    struct Fila: Decodable, Sendable {
        var tipo, desde, hasta: Int32?
        var forzarFijos: Bool?
        var numFijos: Int32?
        var forzarDobles: Bool?
        var numDobles: Int32?
        var forzarTriples: Bool?
    }
    struct Tipo: Decodable, Sendable {
        var tipo: Int32
        var nombre: String?
    }
    struct Entrada: Decodable, Sendable {
        var llamada: String
        var n: Int
        var cuantos: Int?
        var valoracion: [[Double]]?
        var filas: [Fila]?
        var tipos: [Tipo]?
        var columnas: [[String?]]?
        var jornada: Int?
    }
    struct Salida: Decodable, Sendable {
        var signos: [String?]?
        var columnas: [[String?]]?
        var texto: String?
        var ficheros: [[String]]?
        var error: String?
    }

    static func config(_ f: Fila) -> ConfigCP {
        ConfigCP(
            tipo: f.tipo, desde: f.desde, hasta: f.hasta, forzarFijos: f.forzarFijos, numFijos: f.numFijos,
            forzarDobles: f.forzarDobles, numDobles: f.numDobles, forzarTriples: f.forzarTriples)
    }

    static func ejecutar(_ e: Entrada, _ s: inout Salida) throws(ErrorDominio) {
        let v = (e.valoracion ?? []).map { Valoracion($0[0], $0[1], $0[2]) }
        let filas = (e.filas ?? []).map(config)
        switch e.llamada {
        case "fijos": s.signos = try GeneradorCPs.obtenerFijos(e.cuantos ?? 0, v, numeroPartidos: e.n)
        case "dobles": s.signos = try GeneradorCPs.obtenerDobles(e.cuantos ?? 0, v, numeroPartidos: e.n)
        case "crear": s.columnas = try GeneradorCPs.crearCPs(filas, v, numeroPartidos: e.n)
        case "texto": s.texto = GeneradorCPs.texto(e.columnas ?? [], numeroPartidos: e.n)
        case "generar":
            s.ficheros = try GeneradorCPs.generar(
                tipos: (e.tipos ?? []).map { TipoCP(tipo: $0.tipo, nombre: $0.nombre) }, configuraciones: filas,
                valor: v, jornada: e.jornada ?? 0, numeroPartidos: e.n
            ).map { [$0.fichero, $0.texto] }
        default: throw ErrorDominio(.argumento, "llamada \(e.llamada)")
        }
    }

    @Test func generador() throws {
        for caso in try Golden.casos("motor/cp/generador", Entrada.self, Salida.self) {
            let esperado = try #require(caso.salida)
            var s = Salida()
            do {
                try Self.ejecutar(caso.entrada, &s)
            } catch {
                #expect(error.excepcion.rawValue == esperado.error, "\(caso.id) \(error)")
                continue
            }
            #expect(esperado.error == nil, "\(caso.id): se esperaba error")
            if let x = esperado.signos { #expect(s.signos == x, "\(caso.id)") }
            if let x = esperado.columnas { #expect(s.columnas == x, "\(caso.id)") }
            if let x = esperado.texto { #expect(s.texto == x, "\(caso.id)") }
            if let x = esperado.ficheros { #expect(s.ficheros == x, "\(caso.id)") }
        }
    }

    /// DT-15 (DEC-408): `Array.Sort` deja los `NaN` primero ⇒ al invertir para los fijos quedan los últimos.
    @Test func nanVaPrimeroAlOrdenar() throws {
        let v = [Valoracion(.nan, 10, 20)]
        #expect(GeneradorCPs.ordenarValoracion(v, numeroPartidos: 1).map(\.description) == ["nan", "10.0", "20.0"])
        #expect(try GeneradorCPs.obtenerFijos(2, v, numeroPartidos: 1) == ["X"], "20 ⇒ «2», 10 ⇒ «X»")
        #expect(try GeneradorCPs.obtenerFijos(3, v, numeroPartidos: 1) == ["X"], "NaN no coincide con nada")
    }
}
