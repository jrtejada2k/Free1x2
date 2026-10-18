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

// C#: Views/Ported/GenerarCPsViewModel.cs (09 pantalla 59; CU-B27).
extension CasosCondiciones {
    /// «Importar %»: `Porcentajes(ruta).valores`; el aviso del lector se muestra y los valores se usan igual.
    public static func importarValoracion(
        _ url: URL, numeroPartidos n: Int, entorno: Entorno
    ) throws(ErrorDominio) -> (valores: [[Double]], aviso: String?) {
        let r = ArchivoValoraciones.leer(try entorno.archivos.leer(url), numeroPartidos: n, cultura: entorno.cultura)
        return (r.valores, r.aviso)
    }

    /// CU-B27 `Generar`: un `Condiciones/<Nombre>_j<jornada>.txt` por tipo de CP de `ColumnasProbables.xml`; devuelve
    /// cuántos.
    @concurrent public static func generarCPs(_ valoracion: [[Double]], jornada: Int, entorno: Entorno) async throws(ErrorDominio) -> Int {
        let url = EditorConfigCPs.ruta(entorno)
        let (tipos, configuraciones) = try ArchivoColumnasProbables.leer(
            entorno.archivos.existe(url) ? try entorno.archivos.leer(url) : nil)
        let carpeta = entorno.rutas.base.appending(path: "Condiciones")
        try entorno.archivos.crearCarpeta(carpeta)
        let ficheros = try GeneradorCPs.generar(
            tipos: tipos, configuraciones: configuraciones, valor: valoracion.map { Valoracion($0[0], $0[1], $0[2]) },
            jornada: jornada, numeroPartidos: valoracion.count)
        for f in ficheros { try entorno.archivos.escribir(Data(f.texto.utf8), en: carpeta.appending(path: f.fichero), atomico: false) }
        return ficheros.count
    }
}

// C#: Views/Ported/GeneradorCPSDiferenciasViewModel.cs (09 pantalla 58; CU-B26).
extension CasosCondiciones {
    /// `EsValida`: exactamente 14 signos de «1X2».
    public static func columnaCPValida(_ c: String) -> Bool {
        c.utf16.count == 14 && c.utf16.allSatisfy { [0x31, 0x58, 0x32].contains($0) }
    }

    /// `CombinarFijos1/2` y `CombinarDobles1/2`: 15 filas con 1 diferencia (la última = la inicial) o 92 con 2 (la
    /// inicial primero). En dobles el segundo signo sale de `cM`, que acumula 2 caracteres donde las columnas coinciden y
    /// desalinea las posiciones siguientes (se replica).
    public static func cpsPorDiferencias(_ c1: String, _ c2: String, dobles: Bool, diferencias: Int) -> [String] {
        let (a, b) = (Array(c1.utf16), Array(c2.utf16))
        var cM: [UInt16] = []
        for i in 0..<14 { for s: UInt16 in [0x31, 0x58, 0x32] where a[i] != s && b[i] != s { cM.append(s) } }
        func fila(_ cambia: (Int) -> Bool) -> String {
            (0..<14).map { k in String(decoding: [cambia(k) ? b[k] : a[k]] + (dobles ? [cM[k]] : []), as: UTF16.self) }
                .joined(separator: ",")
        }
        if diferencias == 1 { return (0..<15).map { i in fila { $0 == i } } }
        var r = [fila { _ in false }]
        for i in 0..<14 { for j in i + 1..<14 { r.append(fila { $0 == i || $0 == j }) } }
        return r
    }
}
