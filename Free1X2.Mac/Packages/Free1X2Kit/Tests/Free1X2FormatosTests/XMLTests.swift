import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Pruebas
import Testing

/// ESPEC_16 §2 y 07 §1.0: lector `XmlDocument` sin espacios y escritor `XmlTextWriter` indentado.
@Suite("XML .NET")
struct XMLTests {
    static func texto(_ d: Data) -> String { String(decoding: d, as: UTF8.self) }

    /// `GuardarX` re-serializa el documento entero: la semilla (LF en el repo) sale idéntica con CRLF y sin salto final.
    @Test func semillaParametrosIdaYVuelta() throws {
        let semilla = try Golden.bytes("Fixtures/datos/parametros.free1x2")
        let doc = try LectorXML.cargar(semilla)
        #expect(Self.texto(EscritorXMLNET.guardar(doc)) == Self.texto(semilla).replacingOccurrences(of: "\n", with: "\r\n"))
        let lae = try #require(doc.primerElemento("OrdenarApuestas"))
        #expect(try lae.atributo("Moneda", "") == "€")
        #expect(doc.primerElemento("partidos")?.hijo(0)?.nombre == "partidos", "partidos externo → ChildNodes[0] interno")
    }

    @Test func semillaColumnasProbables() throws {
        let doc = try LectorXML.cargar(try Golden.bytes("Fixtures/datos/ColumnasProbables.xml"))
        let config = doc.elementos("Configuracion_x0020_de_x0020_CPs")
        #expect(config.count == 2)
        #expect(config[0].atributos.map(\.nombre).first == "d2p1:Desde", "la 1.ª fila no tiene Tipo")
        #expect(doc.hijos.first?.valor == "version=\"1.0\" standalone=\"yes\"")
    }

    /// 07 F03, ejemplo mínimo de NoVariantes, por la API en streaming.
    @Test func condicionEnStreaming() {
        var w = EscritorXMLNET()
        w.iniciarElemento("xml")
        w.iniciarElemento("file")
        w.atributo("version", "0.1")
        w.cerrarElemento()
        w.iniciarElemento("Condiciones")
        w.iniciarElemento("condicion")
        w.atributo("id", "NoVariantes")
        w.atributo("activa", "True")
        w.iniciarElemento("standard")
        for t in ["var", "equis"] {
            w.iniciarElemento("valores")
            w.atributo("tipo", t)
            w.atributo("values", "0,1")
            w.cerrarElemento()
        }
        let esperado = """
            <xml>
              <file version="0.1" />
              <Condiciones>
                <condicion id="NoVariantes" activa="True">
                  <standard>
                    <valores tipo="var" values="0,1" />
                    <valores tipo="equis" values="0,1" />
                  </standard>
                </condicion>
              </Condiciones>
            </xml>
            """
        #expect(Self.texto(w.cerrar()) == esperado.replacingOccurrences(of: "\n", with: "\r\n"))
    }

    @Test func textoComentariosYEscapes() {
        var w = EscritorXMLNET()
        w.iniciarElemento("condicion")
        w.comentario(" Para cada partido:")
        w.iniciarElemento("dibujo")
        w.texto("3+2")
        w.cerrarElemento()
        w.iniciarElemento("dibujo")
        w.texto("")
        w.cerrarElemento()
        w.iniciarElemento("v")
        w.atributo("a", "a&b<c>\"d'\te\nf\r")
        w.texto("x<y & \"z\" > 'w'")
        w.cerrarElemento()
        let esperado = [
            "<condicion>", "  <!-- Para cada partido:-->", "  <dibujo>3+2</dibujo>", "  <dibujo />",
            "  <v a=\"a&amp;b&lt;c&gt;&quot;d'&#x9;e&#xA;f&#xD;\">x&lt;y &amp; \"z\" &gt; 'w'</v>", "</condicion>",
        ]
        #expect(Self.texto(w.cerrar()) == esperado.joined(separator: "\r\n"))
    }

    /// `WriteFullEndElement` de un elemento sin contenido (`<b></b>` al cargar): `>` + sangría + `</b>`.
    @Test func elementoVacioNoAutocerrado() throws {
        let doc = try LectorXML.cargar(Data("<a>\n  <b></b><c/>  <!--k-->\n</a>".utf8))
        let esperado = ["<?xml version=\"1.0\"?>", "<a>", "  <b>", "  </b>", "  <c />", "  <!--k-->", "</a>"]
        #expect(Self.texto(EscritorXMLNET.guardar(doc)) == esperado.joined(separator: "\r\n"))
    }

    @Test func lectorNavegacion() throws {
        let xml =
            "<?xml version='1.0' encoding='ISO-8859-1'?>\r\n<r>\r\n  <!-- c -->\r\n"
            + "  <e x='&lt;1&#x41;&#66;'> a <![CDATA[<b>]]><i>c</i><!--no--></e>\r\n</r>"
        var bytes = Data(xml.utf8)
        bytes.append(contentsOf: [0x3C, 0x6E, 0x3E, 0xF1, 0x3C, 0x2F, 0x6E, 0x3E])  // <n>ñ</n> en Latin-1 tras la raíz
        let e1 = #expect(throws: ErrorDominio.self) { try LectorXML.cargar(bytes) }
        #expect(e1?.excepcion == .xml, "dos elementos raíz")
        let doc = try LectorXML.cargar(Data(xml.utf8))
        let r = try #require(doc.primerElemento("r"))
        #expect(r.hijos.map(\.nombre) == ["#comment", "e"], "sin nodos de espacios; el comentario cuenta")
        let e = try #require(r.hijo(1))
        #expect(try e.atributo("x", "") == "<1AB")
        #expect(e.textoInterior == " a <b>c", "InnerText: texto, CDATA y descendientes, sin comentarios")
        #expect(r.hijo(0)?.textoInterior == " c ")
        #expect(r.hijo(5) == nil)
        let e2 = #expect(throws: ErrorDominio.self) { try e.atributo("y", "prueba") }
        #expect(e2?.excepcion == .referenciaNula)
        let e3 = #expect(throws: ErrorDominio.self) { try r.hijo(0)?.atributo("x", "") }
        #expect(e3?.excepcion == .referenciaNula, "un comentario no tiene atributos")
    }

    @Test func codificaciones() throws {
        var latin1 = Data("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><n a=\"".utf8)
        latin1.append(contentsOf: [0xF1, 0x22, 0x2F, 0x3E])
        #expect(try LectorXML.cargar(latin1).primerElemento("n")?.atributo("a", "") == "ñ")
        var malo = Data("<n a=\"".utf8)
        malo.append(contentsOf: [0xF1, 0x22, 0x2F, 0x3E])
        let e = #expect(throws: ErrorDominio.self) { try LectorXML.cargar(malo) }
        #expect(e?.excepcion == .xml, "UTF-8 no válido")
        let bom = Data([0xEF, 0xBB, 0xBF] + Array("<n a=\"ñ\"/>".utf8))
        #expect(try LectorXML.cargar(bom).primerElemento("n")?.atributo("a", "") == "ñ")
        for roto in ["<a><b></a>", "", "<a>", "<a b=\"1\" b=\"2\"/>", "<a>&nada;</a>", " <?xml version=\"1.0\"?><a/>x"] {
            let e = #expect(throws: ErrorDominio.self, "\(roto)") { try LectorXML.cargar(Data(roto.utf8)) }
            #expect(e?.excepcion == .xml, "\(roto)")
        }
    }

    @Test func edicionEnSitio() throws {
        var doc = try LectorXML.cargar(try Golden.bytes("Fixtures/datos/parametros.free1x2"))
        let ruta = try #require(doc.rutaPrimerElemento("puntos"))
        try doc[ruta: ruta].cambiarAtributo("fijos", "7", "")
        #expect(try doc.primerElemento("puntos")?.atributo("fijos", "") == "7")
        let e = #expect(throws: ErrorDominio.self) { try doc[ruta: ruta].cambiarAtributo("nada", "1", "") }
        #expect(e?.excepcion == .referenciaNula)
    }
}
