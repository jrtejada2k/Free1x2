import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/ConfigCPsFrmViewModel.cs (08 pantalla 23; CU-A60, CU-A61).

/// Fila de «Configuración de CPs» como la muestra la pantalla (nulos ⇒ 0 / `false`).
public struct FilaConfigCP: Sendable, Hashable {
    public var desde, hasta: Double
    public var forzarFijos: Bool
    public var numFijos: Double
    public var forzarDobles: Bool
    public var numDobles: Double
    public var forzarTriples: Bool
}

/// «Configurar Columnas Probables»: el `DataSet` de `ColumnasProbables.xml` en memoria.
public struct EditorConfigCPs: Sendable {
    public private(set) var tipos: [TipoCP] = []
    public private(set) var configuraciones: [ConfigCP] = []
    /// Nombres de tipo sin repetir (el primero manda), en orden.
    public private(set) var nombres: [String] = []
    var tipoPorNombre: [String: Int32] = [:]

    public init() {}

    static func ruta(_ entorno: Entorno) -> URL { entorno.rutas.base.appending(path: "ColumnasProbables.xml") }

    /// CU-A60 (constructor): fichero ausente ⇒ tablas vacías.
    public static func cargar(entorno: Entorno) throws(ErrorDominio) -> EditorConfigCPs {
        let url = ruta(entorno)
        var e = EditorConfigCPs()
        (e.tipos, e.configuraciones) = try ArchivoColumnasProbables.leer(
            entorno.archivos.existe(url) ? try entorno.archivos.leer(url) : nil)
        for t in e.tipos where e.tipoPorNombre[t.nombre ?? ""] == nil {
            e.tipoPorNombre[t.nombre ?? ""] = t.tipo
            e.nombres.append(t.nombre ?? "")
        }
        return e
    }

    /// Índices de las filas con `Tipo = n` (`RowFilter`; las de tipo nulo no salen).
    func indices(_ nombre: String) -> [Int] {
        guard let t = tipoPorNombre[nombre] else { return [] }
        return configuraciones.indices.filter { configuraciones[$0].tipo == t }
    }

    /// `CargarFilasDelTipo`.
    public func filas(_ nombre: String) -> [FilaConfigCP] {
        indices(nombre).map {
            let c = configuraciones[$0]
            return FilaConfigCP(
                desde: Double(c.desde ?? 0), hasta: Double(c.hasta ?? 0), forzarFijos: c.forzarFijos ?? false,
                numFijos: Double(c.numFijos ?? 0), forzarDobles: c.forzarDobles ?? false, numDobles: Double(c.numDobles ?? 0),
                forzarTriples: c.forzarTriples ?? false)
        }
    }

    /// CU-A61 `Guardar`: las filas existentes del tipo, en orden y truncadas a entero; reescribe todo el XML.
    public mutating func guardar(_ filas: [FilaConfigCP], tipo nombre: String, entorno: Entorno) throws(ErrorDominio) {
        for (i, f) in zip(indices(nombre), filas) {
            configuraciones[i].desde = FormatoNET.castInt32(f.desde)
            configuraciones[i].hasta = FormatoNET.castInt32(f.hasta)
            configuraciones[i].forzarFijos = f.forzarFijos
            configuraciones[i].numFijos = FormatoNET.castInt32(f.numFijos)
            configuraciones[i].forzarDobles = f.forzarDobles
            configuraciones[i].numDobles = FormatoNET.castInt32(f.numDobles)
            configuraciones[i].forzarTriples = f.forzarTriples
        }
        try entorno.archivos.escribir(
            ArchivoColumnasProbables.datos(tipos: tipos, configuraciones: configuraciones), en: Self.ruta(entorno), atomico: false)
    }
}
