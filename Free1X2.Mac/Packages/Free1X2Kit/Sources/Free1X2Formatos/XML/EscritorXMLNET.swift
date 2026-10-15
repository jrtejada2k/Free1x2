import Foundation
import Free1X2Base

// Réplica de `new XmlTextWriter(ruta, null)` con `Formatting.Indented` (ESPEC_16 §2.1; 07 §1.0.1): UTF-8 sin BOM,
// sangría de 2, EOL `\r\n` (DP-02), `<x />` con espacio, sin salto final. `Indent`/`AutoComplete` de .NET.

public struct EscritorXMLNET {
    private struct Marco {
        var nombre: String
        /// `stack[top].mixed`: ya hay texto dentro ⇒ ni sangría ni salto para lo que siga.
        var mixto = false
    }

    private var s = ""
    private var pila: [Marco] = []
    /// Etiqueta de inicio sin cerrar con `>` (`State.Element`/`State.Attribute`).
    private var abierta = false
    /// Nada escrito aún (`State.Start`): el primer nodo no se sangra.
    private var alInicio = true

    public init() {}

    /// `Indent(beforeEndElement)`: en el nivel superior solo salto; dentro, salto y 2 espacios por nivel.
    private mutating func sangrar(antesDeCierre: Bool) {
        guard let marco = pila.last else { return s += "\r\n" }
        guard !marco.mixto else { return }
        s += "\r\n" + String(repeating: " ", count: 2 * (antesDeCierre ? pila.count - 1 : pila.count))
    }

    /// `WriteEndStartTag(false)` pendiente + sangría de un nodo nuevo (`Token.StartElement/Comment/PI`).
    private mutating func antesDeNodo() {
        if abierta {
            s += ">"
            abierta = false
        }
        if !alInicio { sangrar(antesDeCierre: false) }
        alInicio = false
    }

    /// `WriteStartDocument(standalone)`: `<?xml version="1.0"?>` sin `encoding` (codificación `null`).
    public mutating func declaracion(standalone: String? = nil) {
        s += "<?xml version=\"1.0\"" + (standalone.map { " standalone=\"\($0)\"" } ?? "") + "?>"
        alInicio = false
    }

    public mutating func iniciarElemento(_ nombre: String) {
        antesDeNodo()
        s += "<" + nombre
        pila.append(Marco(nombre: nombre))
        abierta = true
    }

    /// `WriteAttributeString`: `& < > "` como entidades y TAB/CR/LF como referencias de carácter.
    public mutating func atributo(_ nombre: String, _ valor: String) {
        s += " " + nombre + "=\"" + Self.escapar(valor, enAtributo: true) + "\""
    }

    /// `WriteString`: vacío no escribe nada (el elemento sigue siendo `<x />`).
    public mutating func texto(_ t: String) {
        guard !t.isEmpty else { return }
        if abierta {
            s += ">"
            abierta = false
        }
        alInicio = false
        if !pila.isEmpty { pila[pila.count - 1].mixto = true }
        s += Self.escapar(t, enAtributo: false)
    }

    /// `WriteComment`: `<!--texto-->` con los espacios que trae.
    public mutating func comentario(_ t: String) {
        antesDeNodo()
        s += "<!--" + t + "-->"
    }

    /// `WriteProcessingInstruction` (distinta de la declaración): `<?destino datos?>`, con el espacio aunque no haya datos.
    public mutating func instruccion(_ destino: String, _ datos: String) {
        antesDeNodo()
        s += "<?" + destino + " " + datos + "?>"
    }

    /// `WriteEndElement` (`completo == false`: `<x />` si no tiene contenido) o `WriteFullEndElement`.
    public mutating func cerrarElemento(completo: Bool = false) {
        guard let marco = pila.last else { return }
        if abierta && !completo {
            s += " />"
        } else {
            if abierta { s += ">" }
            sangrar(antesDeCierre: true)
            s += "</" + marco.nombre + ">"
        }
        abierta = false
        pila.removeLast()
    }

    /// `Close()`: cierra lo abierto; sin salto de línea final.
    public mutating func cerrar() -> Data {
        while !pila.isEmpty { cerrarElemento() }
        return Data(s.utf8)
    }

    /// `XmlDocument.Save(XmlWriter)`: siempre `WriteStartDocument` (con el `standalone` de la declaración, si la hay)
    /// y después cada nodo con `WriteTo`.
    public static func guardar(_ documento: NodoXML) -> Data {
        var w = EscritorXMLNET()
        var hijos = documento.hijos[...]
        if let d = hijos.first, d.tipo == .instruccion, d.nombre == "xml" {
            w.declaracion(standalone: Self.pseudoAtributo("standalone", d.valor))
            hijos = hijos.dropFirst()
        } else {
            w.declaracion()
        }
        for h in hijos { w.escribir(h) }
        return w.cerrar()
    }

    /// `XmlNode.WriteTo`.
    public mutating func escribir(_ n: NodoXML) {
        switch n.tipo {
        case .elemento:
            iniciarElemento(n.nombre)
            for a in n.atributos { atributo(a.nombre, a.valor) }
            for h in n.hijos { escribir(h) }
            cerrarElemento(completo: !n.esVacio)
        case .texto: texto(n.valor)
        case .cdata:
            if abierta {
                s += ">"
                abierta = false
            }
            if !pila.isEmpty { pila[pila.count - 1].mixto = true }
            s += "<![CDATA[" + n.valor + "]]>"
        case .comentario: comentario(n.valor)
        case .instruccion: instruccion(n.nombre, n.valor)
        case .documento: for h in n.hijos { escribir(h) }
        }
    }

    /// Valor de `nombre="…"` dentro de los datos de la declaración.
    static func pseudoAtributo(_ nombre: String, _ datos: String) -> String? {
        guard let r = datos.range(of: nombre + "=") else { return nil }
        let resto = datos[r.upperBound...]
        guard let q = resto.first, q == "\"" || q == "'" else { return nil }
        return String(resto.dropFirst().prefix { $0 != q })
    }

    /// `XmlTextEncoder.Write`: `& < >` siempre; en atributo además `"` y TAB/LF/CR como `&#xH;`; otros controles, `&#xH;`.
    static func escapar(_ t: String, enAtributo: Bool) -> String {
        var r = ""
        for u in t.unicodeScalars {
            switch u {
            case "&": r += "&amp;"
            case "<": r += "&lt;"
            case ">": r += "&gt;"
            case "\"" where enAtributo: r += "&quot;"
            case "\t", "\n", "\r":
                if enAtributo { r += "&#x" + String(u.value, radix: 16, uppercase: true) + ";" } else { r.unicodeScalars.append(u) }
            default:
                if u.value < 0x20 {
                    r += "&#x" + String(u.value, radix: 16, uppercase: true) + ";"
                } else {
                    r.unicodeScalars.append(u)
                }
            }
        }
        return r
    }
}
