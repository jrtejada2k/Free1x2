import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor
import Free1X2Pruebas
import Testing

/// ESPEC_17 CP-13 y 07 F02/F03: leer y volver a escribir condiciones, grupos, If-Then y combinaciones (`formatos/condiciones`).
@Suite("Condiciones y combinaciones XML")
struct CondicionesXMLTests {
    struct Entrada: Decodable, Sendable {
        var archivo: String
        var tipo: String?
        var xml: String
    }
    struct Salida: Decodable, Sendable {
        var igual: Bool?
        var xml: String?
        var error: String?
    }

    static func crlf(_ s: String) -> String { s.replacingOccurrences(of: "\n", with: "\r\n") }

    /// Lee con el lector del C# que corresponde y vuelve a guardar.
    static func idaYVuelta(_ e: Entrada) throws(ErrorDominio) -> Data {
        let datos = Data(crlf(e.xml).utf8)
        switch e.archivo {
        case "condicion":
            let g = try ArchivoCondiciones.leerCondicion(try LectorXML.cargar(datos), numeroPartidos: 14, cultura: .esES)
            let tipo = TipoFiltro(rawValue: e.tipo ?? "") ?? .diferencias
            return try ArchivoCondiciones.datos(filtro: tipo, de: g.condiciones, cultura: .esES)
        case "grupo":
            let doc = try LectorXML.cargar(datos)
            guard let g = try ArchivoCondiciones.leerGrupo(doc, numeroPartidos: 14, cultura: .esES) else {
                throw ErrorDominio(.referenciaNula, "sin grupo")
            }
            return try ArchivoCondiciones.datos(grupo: g, cultura: .esES)
        case "ifthen":
            return ArchivoCondiciones.datos(ifThen: try ArchivoCondiciones.leerIfThen(try LectorXML.cargar(datos)))
        default:
            let l = try ArchivoCombinacion.leer(datos, numeroPartidos: 14, cultura: .esES)
            return try ArchivoCombinacion.datos(l.combinacion, equipos: l.equipos, filtroColumnas: l.filtroColumnas, cultura: .esES)
        }
    }

    @Test func suite() throws {
        for caso in try Golden.casos("formatos/condiciones", Entrada.self, Salida.self) {
            let s = try #require(caso.salida)
            let datos: Data
            do {
                datos = try Self.idaYVuelta(caso.entrada)
            } catch {
                #expect(error.excepcion.rawValue == s.error, "\(caso.id): \(error)")
                continue
            }
            #expect(s.error == nil, "\(caso.id): se esperaba \(s.error ?? "")")
            let esperado = Self.crlf(s.xml ?? caso.entrada.xml)
            let obtenido = String(decoding: datos, as: UTF8.self)
            #expect(obtenido == esperado, "\(caso.id)")
            if obtenido != esperado {
                let a = obtenido.components(separatedBy: "\r\n")
                let b = esperado.components(separatedBy: "\r\n")
                if let i = a.indices.first(where: { $0 >= b.count || a[$0] != b[$0] }) {
                    Issue.record("\(caso.id) línea \(i + 1): «\(a[i])» ≠ «\(i < b.count ? b[i] : "")»")
                }
            }
        }
    }

    /// La combinación leída lleva lo que el motor necesita: grupos con máscara, controles y pronósticos.
    @Test func combinacionLeida() throws {
        let caso = try #require(
            try Golden.casos("formatos/condiciones", Entrada.self, Salida.self).first { $0.id == "F02-COMB-RT" })
        let l = try ArchivoCombinacion.leer(Data(Self.crlf(caso.entrada.xml).utf8), numeroPartidos: 14, cultura: .esES)
        let c = l.combinacion
        #expect(l.equipos[1] == "A & B - \"C\" <D>")
        #expect(l.filtroColumnas == "C:\\Datos\\filtro.txt")
        #expect(c.pronosticos?.prefix(4).map { $0 ?? "nil" } == ["1,X,2", "1,X", "X", ""])
        #expect(c.grupos.count == 2)
        #expect(c.grupos[1].archivoFiltro == "parcial.txt")
        #expect(c.grupos[1].mascara == 0o77, "AddGrupo calcula la máscara (partidos 1 y 2)")
        #expect(c.grupos[1].condiciones.noVariantes.activa)
        #expect(c.controlesGrupos.map(\.textoGruposControlados) == ["0", "0,1"])
        #expect(c.controlesGrupos[0].usaControlGrupos == false)
        #expect(c.usaControlConjuntos)
        #expect(c.ifThen?.condiciones.first?.noThen == true, "«(NO)» en Then")
    }
}
