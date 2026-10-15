import Foundation
import Free1X2Base

// C#: Free1X2.Domain/Online/JornadaQuiniela.cs, JornadaQuinielaParser.cs, CatalogoEquipos.cs y CatalogoEquiposParser.cs
// (07 §5.4, F18, F19). Todos los errores son `FormatException` con el texto exacto del C#.

/// Un partido de la jornada online (`n` = 1…14, o 15 el pleno).
public struct PartidoJornada: Sendable, Hashable {
    public var n: Int32
    public var local: String
    public var visitante: String
}

/// `JornadaQuiniela`: solo los campos conocidos; los demás del JSON se ignoran.
public struct JornadaQuiniela: Sendable, Hashable {
    public static let numeroPartidos = 14

    public var pais: String
    public var temporada: String
    public var jornada: Int32
    public var fecha: String
    public var partidos: [PartidoJornada]
    public var pleno15: PartidoJornada?

    /// `JornadaQuinielaParser.Parse` (:30-95).
    public static func analizar(_ json: String?) throws(ErrorDominio) -> JornadaQuiniela {
        guard let json, !json.allSatisfy(\.isWhitespace) else { throw formato("La respuesta de la jornada está vacía.") }
        let raiz: ValorJSON
        do throws(ErrorJSON) {
            raiz = try JSONNET.analizar(json)
        } catch {
            throw formato("La respuesta de la jornada no es un JSON válido: " + error.mensaje)
        }
        guard case .objeto = raiz else { throw formato("La respuesta de la jornada no es un objeto JSON.") }
        var j = JornadaQuiniela(
            pais: try cadena(raiz, "pais", obligatorio: true), temporada: try cadena(raiz, "temporada", obligatorio: true),
            jornada: try entero(raiz, "jornada", obligatorio: true), fecha: try cadena(raiz, "fecha", obligatorio: false),
            partidos: [], pleno15: nil)
        guard case .array(let lista) = raiz["partidos"] else { throw formato("Falta el array 'partidos' en la jornada.") }
        for (i, p) in lista.enumerated() { j.partidos.append(try partido(p, esperado: Int32(i + 1))) }
        guard j.partidos.count == numeroPartidos else {
            throw formato(
                "La jornada debe tener exactamente \(numeroPartidos) partidos; se recibieron \(j.partidos.count).")
        }
        if let p = raiz["pleno15"], case .objeto = p { j.pleno15 = try partido(p, esperado: 15) }
        return j
    }

    static func partido(_ v: ValorJSON, esperado: Int32) throws(ErrorDominio) -> PartidoJornada {
        guard case .objeto = v else { throw formato("El partido nº \(esperado) no es un objeto JSON.") }
        let n = try entero(v, "n", obligatorio: true)
        guard n == esperado else {
            throw formato("Los partidos deben venir en orden: se esperaba n=\(esperado) y se recibió n=\(n).")
        }
        let local = try cadena(v, "local", obligatorio: true)
        let visitante = try cadena(v, "visitante", obligatorio: true)
        guard !local.allSatisfy(\.isWhitespace) else { throw formato("El equipo local del partido nº \(n) está vacío.") }
        guard !visitante.allSatisfy(\.isWhitespace) else {
            throw formato("El equipo visitante del partido nº \(n) está vacío.")
        }
        return PartidoJornada(n: n, local: local, visitante: visitante)
    }
}

/// Una división del catálogo online.
public struct DivisionEquipos: Sendable, Hashable {
    public var id: String
    public var nombre: String
    public var equipos: [String]
}

/// `CatalogoEquipos`.
public struct CatalogoEquipos: Sendable, Hashable {
    public var pais: String
    public var divisiones: [DivisionEquipos]

    /// `CatalogoEquiposParser.Parse` (:30-80): las divisiones se comprueban **después** del bucle.
    public static func analizar(_ json: String?) throws(ErrorDominio) -> CatalogoEquipos {
        guard let json, !json.allSatisfy(\.isWhitespace) else {
            throw formato("La respuesta del catálogo de equipos está vacía.")
        }
        let raiz: ValorJSON
        do throws(ErrorJSON) {
            raiz = try JSONNET.analizar(json)
        } catch {
            throw formato("La respuesta del catálogo de equipos no es un JSON válido: " + error.mensaje)
        }
        guard case .objeto = raiz else { throw formato("La respuesta del catálogo de equipos no es un objeto JSON.") }
        var c = CatalogoEquipos(pais: try cadena(raiz, "pais", obligatorio: true), divisiones: [])
        guard case .array(let lista) = raiz["divisiones"] else {
            throw formato("Falta el array 'divisiones' en el catálogo de equipos.")
        }
        for d in lista {
            guard case .objeto = d else { throw formato("Una división del catálogo no es un objeto JSON.") }
            let id = try cadena(d, "id", obligatorio: true)
            let nombre = try cadena(d, "nombre", obligatorio: false)
            guard case .array(let e) = d["equipos"] else { throw formato("Falta el array 'equipos' en la división '\(id)'.") }
            var equipos: [String] = []
            for x in e {
                guard let s = x.texto else { throw formato("Los equipos de la división '\(id)' deben ser texto.") }
                guard !s.allSatisfy(\.isWhitespace) else {
                    throw formato("La división '\(id)' contiene un nombre de equipo vacío.")
                }
                equipos.append(s)
            }
            guard !equipos.isEmpty else { throw formato("La división '\(id)' no tiene equipos.") }
            c.divisiones.append(DivisionEquipos(id: id, nombre: nombre, equipos: equipos))
        }
        guard !c.divisiones.isEmpty else { throw formato("El catálogo de equipos debe tener al menos una división.") }
        return c
    }

    /// `EquiposPlano()` (:35-52): en orden, sin blancos, recortados y sin repetir (`OrdinalIgnoreCase`: gana el primero).
    public var equiposPlano: [String] {
        var vistos = Set<String>()
        var r: [String] = []
        for d in divisiones {
            for e in d.equipos where !e.allSatisfy(\.isWhitespace) {
                let clave = e.trimmingCharacters(in: .whitespacesAndNewlines)
                if vistos.insert(Self.mayusculaSimple(clave)).inserted { r.append(clave) }
            }
        }
        return r
    }

    /// Clave de `OrdinalIgnoreCase`: mayúscula **simple** por escalar (`ß` no pasa a `SS`).
    static func mayusculaSimple(_ s: String) -> String {
        var r = String.UnicodeScalarView()
        for u in s.unicodeScalars {
            let m = u.properties.uppercaseMapping.unicodeScalars
            r.append(m.count == 1 ? m.first ?? u : u)
        }
        return String(r)
    }
}

func formato(_ mensaje: String) -> ErrorDominio { ErrorDominio(.formato, mensaje, origen: "Free1X2.Domain/Online") }

/// `LeerCadena`: ausente o `null` ⇒ obligatorio lanza, si no `""`; no texto ⇒ lanza.
func cadena(_ obj: ValorJSON, _ nombre: String, obligatorio: Bool) throws(ErrorDominio) -> String {
    guard let v = obj[nombre], !isNulo(v) else {
        if obligatorio { throw formato("Falta el campo obligatorio '\(nombre)'.") }
        return ""
    }
    guard let s = v.texto else { throw formato("El campo '\(nombre)' debe ser texto.") }
    return s
}

/// `LeerEntero`: como `LeerCadena`, con `TryGetInt32`.
func entero(_ obj: ValorJSON, _ nombre: String, obligatorio: Bool) throws(ErrorDominio) -> Int32 {
    guard let v = obj[nombre], !isNulo(v) else {
        if obligatorio { throw formato("Falta el campo obligatorio '\(nombre)'.") }
        return 0
    }
    guard let n = v.entero32 else { throw formato("El campo '\(nombre)' debe ser un número entero.") }
    return n
}

func isNulo(_ v: ValorJSON) -> Bool {
    if case .nulo = v { return true }
    return false
}
