import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor
import Free1X2Pruebas
import Testing

/// 07 F01: `parametros.free1x2` (lectura de la semilla, caída a valores por defecto y `Guardar*`).
@Suite("parametros.free1x2")
struct ParametrosTests {
    static func semilla() throws -> Data { try Golden.bytes("Fixtures/datos/parametros.free1x2") }
    static func texto(_ d: Data) -> String { String(decoding: d, as: UTF8.self) }

    @Test func semillaCompleta() throws {
        let c = ArchivoParametros.cargarConfiguracion(try Self.semilla())
        #expect(c.puntosCP == PuntosCP(fijos: 6, dobles: 3, triples: 1))
        #expect(c.boleto.numeroPartidos == 14)
        #expect(c.boleto.separador == ["5", "9", "12"])
        #expect(c.boleto.desplazamiento == 3)
        #expect(c.lae.precioApuesta == Double(Float(0.5)))
        #expect(c.lae.porcentaje14 == 12)
        #expect(c.lae.recaudacion == 14_000_000)
        #expect(c.lae.moneda == "€")
        #expect(c.interfaz.barras == Array(repeating: true, count: 6))
        #expect(c.interfaz.actualizarAlInicio == false)
        #expect(c.interfaz.confirmarAlSalir)
        #expect(c.interfaz.idioma == "es-ES")
        #expect(c.analisis.vx2 && c.analisis.diferencias && c.analisis.controlConjuntos)
        let p = try ArchivoParametros(try Self.semilla())
        #expect(try p.separadorJB() == "15,22,29,36,48,61")
        #expect(try p.laeTextos() == ["0,50", "12", "14000000", "€"])
    }

    /// `SetDefaultValues`: sin fichero, todo por defecto; con un fallo a mitad, las banderas de análisis leídas se quedan.
    @Test func caidaAValoresPorDefecto() throws {
        #expect(ArchivoParametros.cargarConfiguracion(nil) == .porDefecto)
        let roto = Self.texto(try Self.semilla()).replacingOccurrences(of: " aPesos=\"True\"", with: "")
        let c = ArchivoParametros.cargarConfiguracion(Data(roto.utf8))
        #expect(c.puntosCP == Configuracion.porDefecto.puntosCP, "SetDefaultValues pisa lo ya leído")
        #expect(c.boleto == Configuracion.porDefecto.boleto)
        #expect(c.analisis.todo && c.analisis.vx2 && c.analisis.contactos && c.analisis.figurasContactos)
        #expect(!c.analisis.pesos && !c.analisis.cps, "lo posterior al fallo, sin leer")
    }

    @Test func lecturasTolerantes() throws {
        var s = Self.texto(try Self.semilla())
        s = s.replacingOccurrences(of: "<desplazamiento valor=\"3\" />", with: "<desplazamiento valor=\"x\" />")
        s = s.replacingOccurrences(of: "tsFiltros=\"True\"", with: "tsFiltros=\"si\"")
        s = s.replacingOccurrences(of: "tsArchivo=\"True\"", with: "tsArchivo=\"false\"")
        s = s.replacingOccurrences(of: "PorcentajeDestinadoAl14=\"12\"", with: "PorcentajeDestinadoAl14=\"0,55\"")
        let p = try ArchivoParametros(Data(s.utf8))
        #expect(p.desplazamiento(previo: 7) == 7, "try/catch vacío: queda el valor previo")
        #expect(p.barras() == [true, true, true, true, false, true], "TryParse fallido ⇒ true")
        #expect(try p.lae().porcentaje14 == 0.550_000_011_920_928_955, "float.Parse ensanchado a double")
    }

    /// Re-guardar lo mismo deja el fichero igual (CRLF); un cambio solo toca su atributo.
    @Test func guardar() throws {
        let semilla = try Self.semilla()
        var p = try ArchivoParametros(semilla)
        let crlf = Self.texto(semilla).replacingOccurrences(of: "\n", with: "\r\n")
        try p.guardarLAE(try p.lae())
        try p.guardarAnalisis(ArchivoParametros.cargarConfiguracion(semilla).analisis)
        #expect(Self.texto(p.datos) == crlf)
        try p.guardarPuntosCP(PuntosCP(fijos: 7, dobles: 3, triples: 1))
        var l = try p.lae()
        l.precioApuesta = 1.5
        l.recaudacion = 1e16
        try p.guardarLAE(l)
        let idioma = p.guardarIdioma("en-US")
        #expect(idioma)
        let t = Self.texto(p.datos)
        #expect(t.contains("<puntos fijos=\"7\" dobles=\"3\" triples=\"1\" />"))
        #expect(t.contains("PrecioApuesta=\"1,50\""))
        #expect(t.contains("Recaudacion=\"1E+16\""))
        #expect(t.contains("<idioma valor=\"en-US\" />"))
        var sinDesp = try ArchivoParametros(Data("<?xml version=\"1.0\"?><xml><desplazamiento /></xml>".utf8))
        let guardado = sinDesp.guardarDesplazamiento("2")
        #expect(!guardado, "sin hijo: se ignora")
        let e = #expect(throws: ErrorDominio.self) { try sinDesp.guardarPuntosCP(PuntosCP(fijos: 1, dobles: 1, triples: 1)) }
        #expect(e?.excepcion == .referenciaNula)
    }

    /// DT-97: el Progol en `<Progol>`; sin el nodo, los de fábrica; al guardar se añade tras `<LAE>` y luego se cambia.
    @Test func laeMexico() throws {
        let semilla = try Self.semilla()
        var p = try ArchivoParametros(semilla)
        #expect(p.laeMexico() == ConfigLAE.mexico && ArchivoParametros.cargarConfiguracion(semilla).laeMexico == .mexico)
        #expect(ConfigLAE.mexico.precioApuesta == 15 && ConfigLAE.mexico.moneda == "$")
        var l = ConfigLAE.mexico
        (l.precioApuesta, l.porcentaje14, l.recaudacion) = (20, 30.5, 18_000_000)
        p.guardarLAEMexico(l)
        let t = Self.texto(p.datos)
        #expect(
            t.contains(
                "</LAE>\r\n    <Progol>\r\n      <OrdenarApuestas PrecioApuesta=\"20,00\" PorcentajeDestinadoAl14=\"30,5\" "
                    + "Recaudacion=\"18000000\" Moneda=\"$\" />\r\n    </Progol>"))
        let leido = try ArchivoParametros(p.datos)
        #expect(leido.laeMexico() == l)
        #expect(try leido.lae() == p.lae(), "España no cambia")
        var otra = leido
        l.precioApuesta = 15
        otra.guardarLAEMexico(l)
        #expect(Self.texto(otra.datos).components(separatedBy: "<Progol>").count == 2 && otra.laeMexico().precioApuesta == 15)
        #expect(ArchivoParametros.cargarConfiguracion(otra.datos).laeMexico == l)
    }
}

/// 07 F05: `ColumnasProbables.xml` de la semilla (DEC-718).
@Suite("ColumnasProbables.xml")
struct ColumnasProbablesXMLTests {
    @Test func semillaIdaYVuelta() throws {
        let semilla = try Golden.bytes("Fixtures/datos/ColumnasProbables.xml")
        let (tipos, config) = try ArchivoColumnasProbables.leer(semilla)
        #expect(tipos == [TipoCP(tipo: 1, nombre: "Normal")])
        #expect(config.count == 2)
        #expect(config[0] == ConfigCP(desde: 0, hasta: 100, forzarFijos: true, numFijos: 14), "la 1.ª fila no tiene Tipo")
        #expect(config[1].tipo == 1 && config[1].desde == 1)
        let escrito = ArchivoColumnasProbables.datos(tipos: tipos, configuraciones: config)
        #expect(
            String(decoding: escrito, as: UTF8.self)
                == String(decoding: semilla, as: UTF8.self).replacingOccurrences(of: "\n", with: "\r\n"))
        #expect(try ArchivoColumnasProbables.leer(nil) == ([], []) as ([TipoCP], [ConfigCP]))
    }

    @Test func valoresXmlConvert() throws {
        let xml = """
            <ColumnasProbables xmlns="http://tempuri.org/ColumnasProbables.xsd">
              <Configuracion_x0020_de_x0020_CPs d2p1:Tipo=" 2 " d2p1:Forzar_x0020_Dobles="1" d2p1:Otro="x" \
            xmlns:d2p1="http://tempuri.org/ColumnasProbables.xsd" />
            </ColumnasProbables>
            """
        let (_, c) = try ArchivoColumnasProbables.leer(Data(xml.utf8))
        #expect(c == [ConfigCP(tipo: 2, forzarDobles: true)])
        let malo = xml.replacingOccurrences(of: "\"1\"", with: "\"True\"")
        let e = #expect(throws: ErrorDominio.self) { try ArchivoColumnasProbables.leer(Data(malo.utf8)) }
        #expect(e?.excepcion == .formato, "XmlConvert.ToBoolean no admite «True»")
    }
}
