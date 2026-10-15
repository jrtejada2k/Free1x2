import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Free1X2.Domain/EntradaSalida/ArchivoCombinacion.cs (07 F02; 07 §6.2) y la secuencia de apertura y guardado de
// MainPageViewModel.cs:577-648.

/// Lo que devuelve abrir un `.comb`: la combinación, los equipos del boleto y el filtro de columnas.
public struct CombinacionLeida: Sendable {
    public var combinacion: Combinacion
    public var equipos: [String]
    public var filtroColumnas: String
}

/// Ficheros de combinación `.comb` (XML).
public enum ArchivoCombinacion {
    /// `AbrirCombinacionAsync`: equipos → filtro de columnas → `new Analizador()` + grupos y controles → If-Then →
    /// `SetPronostico` de cada pronóstico (más de N ⇒ lanza).
    public static func leer(_ datos: Data, numeroPartidos: Int, cultura: Cultura) throws(ErrorDominio) -> CombinacionLeida {
        let doc = try LectorXML.cargar(datos)
        let equipos = try lista(doc, "equipos", "equipos", "ArchivoCombinacion.cs:53-59")
        var filtro = ""
        if let f = doc.primerElemento("filtroColumnas") { filtro = try f.atributo("nombreArchivo", "ArchivoCombinacion.cs:70") }
        var c = Combinacion(numeroPartidos: numeroPartidos)
        try cargarControladorGrupos(doc, &c, numeroPartidos: numeroPartidos, cultura: cultura)
        c.ifThen = try ArchivoCondiciones.leerIfThen(doc)
        for (i, p) in try lista(doc, "columnaBase", "pronostico", "ArchivoCombinacion.cs:79-84").enumerated() {
            try c.setPronostico(i, p)
        }
        return CombinacionLeida(combinacion: c, equipos: equipos, filtroColumnas: filtro)
    }

    /// `LeeEquipos`/`LeePronosticos`: un atributo por hijo del primer elemento con ese nombre (ausente ⇒ NRE).
    static func lista(_ doc: NodoXML, _ elemento: String, _ atributo: String, _ o: String) throws(ErrorDominio) -> [String] {
        guard let e = doc.primerElemento(elemento) else { throw .referenciaNula(o) }
        var r: [String] = []
        for h in e.hijos { r.append(try h.atributo(atributo, o)) }
        return r
    }

    /// `CargaControladorGrupos` (:97-102): grupos → controles de grupos → controles de conjuntos.
    static func cargarControladorGrupos(
        _ doc: NodoXML, _ c: inout Combinacion, numeroPartidos: Int, cultura: Cultura
    ) throws(ErrorDominio) {
        // LeeGrupos (:147-172): `AddGrupo` pone cada grupo en el control 0 antes de leer los controles.
        guard let gp = doc.primerElemento("gruposPartidos") else { throw .referenciaNula("ArchivoCombinacion.cs:157") }
        c.vaciarGrupos()
        for h in gp.hijos where h.tipo != .comentario {
            var g = try ArchivoCondiciones.grupo(h, numeroPartidos: numeroPartidos, cultura: cultura, combinacion: true)
            if let nombre = h.atributoOpcional("nombre") { g.nombreGrupo = nombre }
            try c.addGrupo(g)
        }
        // LeeControlesGrupos (:183-215): el índice 0 es el de **hijo** (un comentario delante lo desplaza).
        let o = "ArchivoCombinacion.cs:200-209"
        guard let cg = doc.primerElemento("controlGrupos") else { throw .referenciaNula("ArchivoCombinacion.cs:191") }
        for (i, h) in cg.hijos.enumerated() where h.tipo != .comentario {
            var control = i == 0 ? try c.controlesGrupos.leerLista(0, o) : ControlGruposConfig()
            _ = try control.ponerGruposControlados(try h.atributo("grupos", o))
            _ = try control.ponerFallosPermitidos(try h.atributo("fallos", o))
            if i == 0 {
                control.usaControlGrupos = false
                c.controlesGrupos[0] = control
            } else {
                c.controlesGrupos.append(control)
            }
        }
        // LeeControlesConjuntos (:109-145): opcional.
        guard let cc = doc.primerElemento("controlConjuntos") else { return }
        let oc = "ArchivoCombinacion.cs:128-136"
        for (i, h) in cc.hijos.enumerated() where h.tipo != .comentario {
            var conjunto = i == 0 ? try c.controlesConjuntos.leerLista(0, oc) : ControlConjuntosConfig()
            try conjunto.ponerCtrlGruposControlados(try h.atributo("conjuntos", oc))
            try conjunto.ponerFallosPermitidos(try h.atributo("fallos", oc))
            if i == 0 {
                c.controlesConjuntos[0] = conjunto
            } else {
                c.controlesConjuntos.append(conjunto)
                c.usaControlConjuntos = true
            }
        }
    }

    /// `GuardaArchivo` (:299-318). Pronósticos `nil` ⇒ NRE; un pronóstico `nil` ⇒ `""`.
    public static func datos(
        _ c: Combinacion, equipos: [String], filtroColumnas: String, cultura: Cultura
    ) throws(ErrorDominio) -> Data {
        var w = ArchivoCondiciones.cabecera()
        w.iniciarElemento("equipos")
        for (i, e) in equipos.enumerated() { DatosFiltro.elemento(&w, "partido", [("id", String(i)), ("equipos", e)]) }
        w.cerrarElemento()
        guard let pronosticos = c.pronosticos else { throw .referenciaNula("ArchivoCombinacion.cs:355") }
        w.iniciarElemento("columnaBase")
        for (i, p) in pronosticos.enumerated() {
            DatosFiltro.elemento(&w, "partido", [("id", String(i)), ("pronostico", p ?? "")])
        }
        w.cerrarElemento()
        DatosFiltro.elemento(&w, "filtroColumnas", [("nombreArchivo", filtroColumnas)])
        w.iniciarElemento("gruposPartidos")
        w.comentario(" grupo 0 es el boleto base ")
        for (i, grupo) in c.grupos.enumerated() {
            var g = grupo
            w.iniciarElemento("grupo")
            w.atributo("id", String(i))
            w.atributo("nombre", g.nombreGrupo)
            DatosFiltro.elemento(&w, "partidosActivos", [("noPartidos", g.textoPartidosActivos)])
            if !g.esGrupoBase && g.nombreGrupo != "0" {
                DatosFiltro.elemento(&w, "filtroParcial", [("archivo", g.archivoFiltro)])
            }
            try DatosFiltro.escribir(&g.condiciones, &w, cultura: cultura)
            ArchivoCondiciones.escribirTolerancias(g.tolerancias, &w)
            w.cerrarElemento()
        }
        w.cerrarElemento()
        w.iniciarElemento("controlGrupos")
        for (i, x) in c.controlesGrupos.enumerated() {
            DatosFiltro.elemento(
                &w, "control", [("id", String(i)), ("grupos", x.textoGruposControlados), ("fallos", x.textoFallosPermitidos)])
        }
        w.cerrarElemento()
        if c.controlesConjuntos.count > 1 {
            w.iniciarElemento("controlConjuntos")
            for (i, x) in c.controlesConjuntos.enumerated() {
                DatosFiltro.elemento(
                    &w, "control",
                    [("id", String(i)), ("conjuntos", x.textoCtrlGruposControlados), ("fallos", x.textoFallosPermitidos)])
            }
            w.cerrarElemento()
        }
        ArchivoCondiciones.escribirIfThen(c.ifThen, &w)
        return w.cerrar()
    }
}
