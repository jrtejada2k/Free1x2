import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Free1X2.Domain/EntradaSalida/GenerarCPs/DatosHelper.cs y el DataSet tipado ColumnasProbables.cs (07 F05; DEC-718).

/// Tipos y configuración del generador de CPs (`ColumnasProbables.xml`).
public enum ArchivoColumnasProbables {
    static let espacio = "http://tempuri.org/ColumnasProbables.xsd"
    static let tablaTipos = "Tipos de CPs"
    static let tablaConfig = "Configuracion de CPs"

    /// `ObtenerDatos` (:16-27): fichero ausente ⇒ tablas vacías. `ReadXml` con esquema: solo las tablas y columnas
    /// conocidas; atributos por nombre local con `_xHHHH_` decodificado; valores con `XmlConvert`.
    public static func leer(_ datos: Data?) throws(ErrorDominio) -> (tipos: [TipoCP], configuraciones: [ConfigCP]) {
        guard let datos else { return ([], []) }
        let doc = try LectorXML.cargar(datos)
        var tipos: [TipoCP] = []
        var configuraciones: [ConfigCP] = []
        for e in doc.elementos("*") {
            let a = Dictionary(
                e.atributos.filter { !$0.nombre.hasPrefix("xmlns") }.map { (decodificar(local($0.nombre)), $0.valor) },
                uniquingKeysWith: { _, ultimo in ultimo })
            switch decodificar(local(e.nombre)) {
            case tablaTipos:
                guard let t = a["Tipo"] else {
                    throw ErrorDominio(.argumento, "La columna 'Tipo' no admite valores NULL.", origen: "ColumnasProbables.cs:280")
                }
                let tipo = try entero(t)
                guard !tipos.contains(where: { $0.tipo == tipo }) else {
                    throw ErrorDominio(.argumento, "La columna 'Tipo' está restringida a ser única.", origen: "ColumnasProbables.cs:287")
                }
                tipos.append(TipoCP(tipo: tipo, nombre: a["Nombre"]))
            case tablaConfig:
                configuraciones.append(
                    ConfigCP(
                        tipo: try a["Tipo"].map(entero), desde: try a["Desde"].map(entero), hasta: try a["Hasta"].map(entero),
                        forzarFijos: try a["Forzar Fijos"].map(booleano), numFijos: try a["Num Fijos"].map(entero),
                        forzarDobles: try a["Forzar Dobles"].map(booleano), numDobles: try a["Num Dobles"].map(entero),
                        forzarTriples: try a["Forzar Triples"].map(booleano)))
            default: break
            }
        }
        return (tipos, configuraciones)
    }

    /// `GuardarDatos` (:29-35) = `DataSet.WriteXml`: declaración `standalone="yes"`, tablas planas en orden, solo los
    /// atributos no nulos, prefijo `d2p1` redeclarado en cada fila y booleanos en minúscula.
    public static func datos(tipos: [TipoCP], configuraciones: [ConfigCP]) -> Data {
        var w = EscritorXMLNET()
        w.declaracion(standalone: "yes")
        w.iniciarElemento("ColumnasProbables")
        w.atributo("xmlns", espacio)
        func fila(_ tabla: String, _ columnas: [(String, String?)]) {
            w.iniciarElemento(codificar(tabla))
            for case let (c, v?) in columnas { w.atributo("d2p1:" + codificar(c), v) }
            w.atributo("xmlns:d2p1", espacio)
            w.cerrarElemento()
        }
        for t in tipos { fila(tablaTipos, [("Tipo", String(t.tipo)), ("Nombre", t.nombre)]) }
        let b: (Bool?) -> String? = { $0.map { $0 ? "true" : "false" } }
        let n: (Int32?) -> String? = { $0.map(String.init) }
        for c in configuraciones {
            fila(
                tablaConfig,
                [
                    ("Tipo", n(c.tipo)), ("Desde", n(c.desde)), ("Hasta", n(c.hasta)), ("Forzar Fijos", b(c.forzarFijos)),
                    ("Num Fijos", n(c.numFijos)), ("Forzar Dobles", b(c.forzarDobles)), ("Num Dobles", n(c.numDobles)),
                    ("Forzar Triples", b(c.forzarTriples)),
                ])
        }
        return w.cerrar()
    }

    static func local(_ nombre: String) -> String {
        nombre.split(separator: ":", maxSplits: 1).last.map(String.init) ?? nombre
    }

    /// `XmlConvert.DecodeName`: `_xHHHH_` ⇒ carácter.
    static func decodificar(_ s: String) -> String {
        guard s.contains("_x") else { return s }
        var r = ""
        var resto = Substring(s)
        while let i = resto.range(of: "_x") {
            let h = resto[i.upperBound...].prefix(5)
            if h.count == 5, h.last == "_", let v = UInt32(h.prefix(4), radix: 16), let u = Unicode.Scalar(v) {
                r += resto[..<i.lowerBound] + String(u)
                resto = resto[h.endIndex...]
            } else {
                r += resto[..<i.upperBound]
                resto = resto[i.upperBound...]
            }
        }
        return r + resto
    }

    /// `XmlConvert.EncodeName` de los nombres de este esquema: solo hay espacios.
    static func codificar(_ s: String) -> String { s.replacingOccurrences(of: " ", with: "_x0020_") }

    /// `XmlConvert.ToInt32`: cultura invariante, espacios laterales admitidos.
    static func entero(_ s: String) throws(ErrorDominio) -> Int32 {
        guard let v = Int32(s.trimmingCharacters(in: .whitespacesAndNewlines)) else { throw .formato("XmlConvert.ToInt32") }
        return v
    }

    /// `XmlConvert.ToBoolean`: `true`/`false`/`1`/`0` (recortados, sensible a mayúsculas).
    static func booleano(_ s: String) throws(ErrorDominio) -> Bool {
        switch s.trimmingCharacters(in: .whitespacesAndNewlines) {
        case "true", "1": return true
        case "false", "0": return false
        default: throw .formato("XmlConvert.ToBoolean")
        }
    }
}
