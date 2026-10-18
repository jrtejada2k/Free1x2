import Foundation
import Free1X2Base

// `XmlDocument.Load(new XmlTextReader(ruta))` con `PreserveWhitespace = false` (ESPEC_16 §2.2; 07 §1.0).
// Lector propio: `XMLParser` entrega los atributos en un diccionario y perdería su orden, que se reescribe tal cual (F01).
// ponytail: sin DTD (un `<!DOCTYPE …>` se salta) ni normalización de fin de línea, como `XmlTextReader` por defecto.

public enum LectorXML {
    /// Documento (`tipo == .documento`) con la declaración, comentarios e instrucciones de primer nivel y el elemento raíz.
    public static func cargar(_ datos: Data) throws(ErrorDominio) -> NodoXML {
        var p = Analizador(try decodificar(datos))
        return try p.documento()
    }

    /// Codificación por BOM (UTF-8/16) o por `encoding` de la declaración; sin ellas, UTF-8 estricto.
    static func decodificar(_ datos: Data) throws(ErrorDominio) -> String {
        let b = [UInt8](datos)
        if b.starts(with: [0xEF, 0xBB, 0xBF]) || b.starts(with: [0xFF, 0xFE]) || b.starts(with: [0xFE, 0xFF]) {
            return TextoNET.decodificar(datos)
        }
        var codificacion = "utf-8"
        if b.starts(with: Array("<?xml".utf8)), let fin = b.firstIndex(of: 0x3E) {
            let decl = String(decoding: b[..<fin], as: UTF8.self)
            if let r = decl.range(of: "encoding") {
                let resto = decl[r.upperBound...].drop { $0 == " " || $0 == "=" }
                if let q = resto.first, q == "\"" || q == "'" {
                    codificacion = String(resto.dropFirst().prefix { $0 != q }).lowercased()
                }
            }
        }
        switch codificacion {
        case "iso-8859-1", "latin1", "iso_8859-1", "l1":
            return TextoNET.decodificarLatin1(datos)
        case "utf-8", "utf8", "us-ascii", "ascii":
            guard let s = String(data: datos, encoding: .utf8) else {
                throw ErrorDominio(.xml, "Carácter no válido en la codificación especificada.", origen: "XmlTextReader")
            }
            return s
        default:
            throw ErrorDominio(.xml, "El sistema no admite la codificación '\(codificacion)'.", origen: "XmlTextReader")
        }
    }

    private struct Analizador {
        let c: [Unicode.Scalar]
        var i = 0

        init(_ texto: String) { c = Array(texto.unicodeScalars) }

        func error(_ motivo: String) -> ErrorDominio {
            var linea = 1
            var columna = 1
            for k in 0..<min(i, c.count) {
                if c[k] == "\n" {
                    linea += 1
                    columna = 1
                } else {
                    columna += 1
                }
            }
            return ErrorDominio(.xml, "\(motivo) Línea \(linea), posición \(columna).", origen: "XmlTextReader")
        }

        func mira(_ s: String) -> Bool {
            var k = i
            for u in s.unicodeScalars {
                guard k < c.count, c[k] == u else { return false }
                k += 1
            }
            return true
        }

        static func esEspacio(_ u: Unicode.Scalar) -> Bool { u == " " || u == "\t" || u == "\r" || u == "\n" }

        mutating func saltarEspacios() {
            while i < c.count, Self.esEspacio(c[i]) { i += 1 }
        }

        /// Hasta `fin` (excluido), que se consume.
        mutating func hasta(_ fin: String, _ motivo: String) throws(ErrorDominio) -> String {
            var r = String.UnicodeScalarView()
            while i < c.count {
                if mira(fin) {
                    i += fin.unicodeScalars.count
                    return String(r)
                }
                r.append(c[i])
                i += 1
            }
            throw error(motivo)
        }

        mutating func nombre() throws(ErrorDominio) -> String {
            var r = String.UnicodeScalarView()
            while i < c.count, !Self.esEspacio(c[i]), !"/>=<?\"'".unicodeScalars.contains(c[i]) {
                r.append(c[i])
                i += 1
            }
            guard !r.isEmpty else { throw error("Nombre no válido.") }
            return String(r)
        }

        /// Texto con las referencias `&…;` resueltas.
        func resolver(_ s: String) throws(ErrorDominio) -> String {
            guard s.contains("&") else { return s }
            var r = String.UnicodeScalarView()
            let u = Array(s.unicodeScalars)
            var k = 0
            while k < u.count {
                guard u[k] == "&" else {
                    r.append(u[k])
                    k += 1
                    continue
                }
                guard let fin = u[k...].firstIndex(of: ";") else { throw error("Referencia de entidad sin terminar.") }
                let ref = String(String.UnicodeScalarView(u[(k + 1)..<fin]))
                switch ref {
                case "lt": r.append("<")
                case "gt": r.append(">")
                case "amp": r.append("&")
                case "quot": r.append("\"")
                case "apos": r.append("'")
                default:
                    let hex = ref.hasPrefix("#x")
                    guard ref.hasPrefix("#"), let n = UInt32(ref.dropFirst(hex ? 2 : 1), radix: hex ? 16 : 10),
                        let e = Unicode.Scalar(n)
                    else { throw error("Referencia a la entidad '\(ref)' no declarada.") }
                    r.append(e)
                }
                k = fin + 1
            }
            return String(r)
        }

        mutating func documento() throws(ErrorDominio) -> NodoXML {
            var doc = NodoXML(tipo: .documento, nombre: "#document")
            var raiz = false
            while true {
                saltarEspacios()
                guard i < c.count else { break }
                guard c[i] == "<" else { throw error("Los datos del nivel de raíz no son válidos.") }
                if mira("<?") {
                    let pi = try instruccion()
                    if pi.nombre.lowercased() == "xml" && !doc.hijos.isEmpty {
                        throw error("Declaración XML inesperada.")
                    }
                    doc.hijos.append(pi)
                } else if mira("<!--") {
                    doc.hijos.append(try comentario())
                } else if mira("<!DOCTYPE") {
                    try saltarDoctype()
                } else {
                    guard !raiz else { throw error("Hay varios elementos raíz.") }
                    doc.hijos.append(try elemento())
                    raiz = true
                }
            }
            guard raiz else { throw error("Falta el elemento raíz.") }
            return doc
        }

        mutating func instruccion() throws(ErrorDominio) -> NodoXML {
            i += 2
            let destino = try nombre()
            saltarEspacios()
            return NodoXML(tipo: .instruccion, nombre: destino, valor: try hasta("?>", "Instrucción sin terminar."))
        }

        mutating func comentario() throws(ErrorDominio) -> NodoXML {
            i += 4
            return NodoXML(tipo: .comentario, nombre: "#comment", valor: try hasta("-->", "Comentario sin terminar."))
        }

        mutating func saltarDoctype() throws(ErrorDominio) {
            var corchetes = 0
            while i < c.count {
                if c[i] == "[" { corchetes += 1 }
                if c[i] == "]" { corchetes -= 1 }
                i += 1
                if c[i - 1] == ">" && corchetes == 0 { return }
            }
            throw error("DOCTYPE sin terminar.")
        }

        mutating func elemento() throws(ErrorDominio) -> NodoXML {
            i += 1
            var e = NodoXML(tipo: .elemento, nombre: try nombre())
            while true {
                saltarEspacios()
                guard i < c.count else { throw error("Fin de archivo inesperado.") }
                if mira("/>") {
                    i += 2
                    e.esVacio = true
                    return e
                }
                if c[i] == ">" {
                    i += 1
                    break
                }
                let n = try nombre()
                saltarEspacios()
                guard i < c.count, c[i] == "=" else { throw error("Falta el signo '=' del atributo '\(n)'.") }
                i += 1
                saltarEspacios()
                guard i < c.count, c[i] == "\"" || c[i] == "'" else { throw error("Falta la comilla del atributo.") }
                let q = String(c[i])
                i += 1
                let crudo = try hasta(q, "Atributo sin terminar.")
                guard !crudo.contains("<") else { throw error("El carácter '<' no puede usarse en un atributo.") }
                guard !e.atributos.contains(where: { $0.nombre == n }) else { throw error("Atributo '\(n)' repetido.") }
                e.atributos.append(AtributoXML(n, try resolver(crudo)))
            }
            while true {
                guard i < c.count else { throw error("Fin de archivo inesperado: falta '</\(e.nombre)>'.") }
                if mira("</") {
                    i += 2
                    let n = try nombre()
                    saltarEspacios()
                    guard n == e.nombre, i < c.count, c[i] == ">" else {
                        throw error("La etiqueta de inicio '\(e.nombre)' no coincide con la etiqueta de fin '\(n)'.")
                    }
                    i += 1
                    return e
                } else if mira("<!--") {
                    e.hijos.append(try comentario())
                } else if mira("<![CDATA[") {
                    i += 9
                    e.hijos.append(
                        NodoXML(tipo: .cdata, nombre: "#cdata-section", valor: try hasta("]]>", "CDATA sin terminar.")))
                } else if mira("<?") {
                    e.hijos.append(try instruccion())
                } else if c[i] == "<" {
                    e.hijos.append(try elemento())
                } else {
                    var t = String.UnicodeScalarView()
                    while i < c.count, c[i] != "<" {
                        t.append(c[i])
                        i += 1
                    }
                    // PreserveWhitespace = false: los nodos de solo espacios no se cargan.
                    if !t.allSatisfy(Self.esEspacio) {
                        e.hijos.append(NodoXML(tipo: .texto, nombre: "#text", valor: try resolver(String(t))))
                    }
                }
            }
        }
    }
}
