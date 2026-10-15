import Free1X2Base

// Árbol de `XmlDocument` cargado con `PreserveWhitespace = false` y su navegación (ESPEC_16 §2.2; 07 §1.0).

/// Atributo de un elemento, en orden de documento.
public struct AtributoXML: Sendable, Equatable {
    public var nombre: String
    public var valor: String

    public init(_ nombre: String, _ valor: String) {
        (self.nombre, self.valor) = (nombre, valor)
    }
}

/// Nodo de un `XmlDocument`: sin nodos de solo espacios en blanco.
public struct NodoXML: Sendable, Equatable {
    public enum Tipo: Sendable, Equatable {
        case documento, elemento, texto, cdata, comentario, instruccion
    }

    public var tipo: Tipo
    /// `Name`: etiqueta, destino de la instrucción (`xml` = declaración) o `#text`/`#cdata-section`/`#comment`/`#document`.
    public var nombre: String
    public var atributos: [AtributoXML] = []
    public var hijos: [NodoXML] = []
    /// `Value` de texto, comentario e instrucción.
    public var valor = ""
    /// `IsEmpty`: elemento escrito `<x />`; al guardar se conserva la forma (`WriteEndElement` frente a `WriteFullEndElement`).
    public var esVacio = false

    public init(tipo: Tipo, nombre: String, valor: String = "") {
        (self.tipo, self.nombre, self.valor) = (tipo, nombre, valor)
    }

    public static func elemento(_ nombre: String, _ atributos: [AtributoXML] = [], hijos: [NodoXML] = []) -> NodoXML {
        var n = NodoXML(tipo: .elemento, nombre: nombre)
        n.atributos = atributos
        n.hijos = hijos
        n.esVacio = hijos.isEmpty
        return n
    }

    /// `ChildNodes[i]`: fuera de rango devuelve `null` (el uso posterior es la `NullReferenceException`). Incluye comentarios.
    public func hijo(_ i: Int) -> NodoXML? {
        hijos.indices.contains(i) ? hijos[i] : nil
    }

    /// `GetElementsByTagName(nombre)`: descendientes en orden de documento, sensible a mayúsculas (`*` = todos).
    public func elementos(_ nombre: String) -> [NodoXML] {
        var r: [NodoXML] = []
        func visitar(_ n: NodoXML) {
            for h in n.hijos where h.tipo == .elemento {
                if nombre == "*" || h.nombre == nombre { r.append(h) }
                visitar(h)
            }
        }
        visitar(self)
        return r
    }

    /// `GetElementsByTagName(nombre)[0]`.
    public func primerElemento(_ nombre: String) -> NodoXML? {
        rutaPrimerElemento(nombre).map { self[ruta: $0] }
    }

    /// Índices de hijo hasta `GetElementsByTagName(nombre)[0]`, para editar el árbol en su sitio (F01).
    public func rutaPrimerElemento(_ nombre: String) -> [Int]? {
        for (i, h) in hijos.enumerated() where h.tipo == .elemento {
            if h.nombre == nombre { return [i] }
            if let r = h.rutaPrimerElemento(nombre) { return [i] + r }
        }
        return nil
    }

    public subscript(ruta ruta: [Int]) -> NodoXML {
        get { ruta.reduce(self) { $0.hijos[$1] } }
        set {
            guard let i = ruta.first else { return self = newValue }
            hijos[i][ruta: Array(ruta.dropFirst())] = newValue
        }
    }

    /// `Attributes[nombre]?.Value` (`nil` si no es un elemento o no lo tiene).
    public func atributoOpcional(_ nombre: String) -> String? {
        atributos.first { $0.nombre == nombre }?.valor
    }

    /// `Attributes[nombre].Value`: ausente (o nodo sin atributos, como un comentario) ⇒ `NullReferenceException`.
    public func atributo(_ nombre: String, _ origen: @autoclosure () -> String) throws(ErrorDominio) -> String {
        guard let v = atributoOpcional(nombre) else { throw .referenciaNula(origen()) }
        return v
    }

    /// `Attributes[nombre].Value = valor`: ausente ⇒ `NullReferenceException`.
    public mutating func cambiarAtributo(_ nombre: String, _ valor: String, _ origen: @autoclosure () -> String)
        throws(ErrorDominio)
    {
        guard let i = atributos.firstIndex(where: { $0.nombre == nombre }) else { throw .referenciaNula(origen()) }
        atributos[i].valor = valor
    }

    /// `InnerText`: el `Value` de texto/CDATA/comentario; en un elemento, el texto de sus descendientes sin comentarios.
    public var textoInterior: String {
        switch tipo {
        case .texto, .cdata, .comentario, .instruccion: return valor
        case .elemento, .documento:
            var r = ""
            func juntar(_ n: NodoXML) {
                for h in n.hijos {
                    if h.tipo == .texto || h.tipo == .cdata { r += h.valor } else { juntar(h) }
                }
            }
            juntar(self)
            return r
        }
    }
}
