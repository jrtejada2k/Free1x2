import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Free1X2.Domain/EntradaSalida/ArchivoCondiciones.cs (07 F03; 07 §6.3).

/// Ficheros de condición sueltos (un filtro, un grupo o un If-Then) y lector del If-Then de las combinaciones.
public enum ArchivoCondiciones {
    /// `AbrirArchivoCombinacion` (:35-51): `nil` si el XML no se puede cargar (los errores de contenido sí se propagan luego).
    public static func abrir(_ datos: Data) -> NodoXML? {
        try? LectorXML.cargar(datos)
    }

    /// `LeeCondicion()` (:77-83): el grupo armado con el nodo `Condiciones` (ausente ⇒ NRE).
    public static func leerCondicion(_ doc: NodoXML, numeroPartidos: Int, cultura: Cultura) throws(ErrorDominio) -> GrupoConfig {
        guard let nodo = doc.primerElemento("Condiciones") else { throw .referenciaNula("ArchivoCondiciones.cs:81") }
        return try grupo(nodo, numeroPartidos: numeroPartidos, cultura: cultura)
    }

    /// `LeeGrupos(ref grupo)` (:53-75): procesa **todos** los `grupo` y devuelve el último (`nil` si no hay ninguno).
    public static func leerGrupo(_ doc: NodoXML, numeroPartidos: Int, cultura: Cultura) throws(ErrorDominio) -> GrupoConfig? {
        guard let gp = doc.primerElemento("gruposPartidos") else { throw .referenciaNula("ArchivoCondiciones.cs:61") }
        var ultimo: GrupoConfig?
        for h in gp.hijos where h.tipo != .comentario {
            var g = try grupo(h, numeroPartidos: numeroPartidos, cultura: cultura)
            if let nombre = h.atributoOpcional("nombre") { g.nombreGrupo = nombre }
            ultimo = g
        }
        return ultimo
    }

    /// `GetGrupoFromXmlNode` (:148-177): despacha por `Name`, salta comentarios; aquí sin `filtroParcial`.
    /// Con `combinacion` (versión de `ArchivoCombinacion.cs:217-261`): `EsGrupoBase = (i == 0)` en cada hijo y `filtroParcial`.
    static func grupo(
        _ nodo: NodoXML, numeroPartidos: Int, cultura: Cultura, combinacion: Bool = false
    ) throws(ErrorDominio) -> GrupoConfig {
        var g = GrupoConfig(numeroPartidos: numeroPartidos)
        for (i, h) in nodo.hijos.enumerated() {
            if combinacion { g.esGrupoBase = i == 0 }
            guard h.tipo != .comentario else { continue }
            switch h.nombre {
            case "condicion":
                // `GetFiltro(id)`: un nombre desconocido devuelve el último filtro del grupo (Diferencias).
                let tipo = TipoFiltro(rawValue: try h.atributo("id", "ArchivoCondiciones.cs:228")) ?? .diferencias
                try DatosFiltro.leer(tipo, h, en: &g.condiciones, numeroPartidos: numeroPartidos, cultura: cultura)
            case "partidosActivos":
                let p = try h.atributo("noPartidos", "ArchivoCondiciones.cs:164")
                if !p.isEmpty { try g.ponerPartidosActivos(p) }
            case "tolerancias":
                g.tolerancias = try tolerancias(h)
            case "filtroParcial" where combinacion:
                g.archivoFiltro = try h.atributo("archivo", "ArchivoCombinacion.cs:254")
            default: break
            }
        }
        return g
    }

    /// `PonerToleranciasGrupo` (:195-223).
    static func tolerancias(_ nodo: NodoXML) throws(ErrorDominio) -> ControladorTolConfig {
        let o = "ArchivoCondiciones.cs:210-217"
        var c = ControladorTolConfig()
        var lista: [ToleranciaConfig] = []
        var noFallos = ""
        for h in nodo.hijos {
            switch h.nombre {
            case "control":
                var t = ToleranciaConfig()
                try t.ponerAciertos(try h.atributo("noAciertos", o))
                t.letrasTol = try h.atributo("letras", o)
                lista.append(t)
            case "controlFallos": noFallos = try h.atributo("noFallos", o)
            default: break
            }
        }
        c.tolerancias = lista
        try c.ponerFallosPermitidos(noFallos)
        return c
    }

    /// `LeeIfThen()` (:85-146): `nil` sin `If-Then`; el **último** hijo de `Condiciones`/`Grupos` es siempre `Rangos`.
    public static func leerIfThen(_ doc: NodoXML) throws(ErrorDominio) -> IfThenConfig? {
        guard let it = doc.primerElemento("If-Then") else { return nil }
        let o = "ArchivoCondiciones.cs:111-140"
        var cfg = IfThenConfig()
        var condiciones: NodoXML?
        var grupos: NodoXML?
        for h in it.hijos {
            if h.nombre == "Condiciones" {
                condiciones = h
            } else if h.nombre == "Grupos" {
                grupos = h
            }
        }
        if let condiciones {
            for (i, x) in condiciones.hijos.enumerated() {
                if i == condiciones.hijos.count - 1 {
                    try cfg.ponerRangoAciertoCondiciones(try x.atributo("valor", o))
                } else {
                    var c = CondicionIfThenConfig()
                    c.condIf = try x.atributo("If", o)
                    c.condThen = try x.atributo("Then", o)
                    cfg.condiciones.append(c)
                }
            }
        }
        if let grupos {
            for (i, x) in grupos.hijos.enumerated() {
                if i == grupos.hijos.count - 1 {
                    try cfg.ponerRangoAciertoGrupos(try x.atributo("valor", o))
                } else {
                    var g = GrupoIfThenConfig()
                    g.numGrupoIf = Int(try FormatoNET.convertToInt16(try x.atributo("if", o)))
                    g.numGrupoThen = Int(try FormatoNET.convertToInt16(try x.atributo("then", o)))
                    g.noIf = try FormatoNET.convertToBoolean(try x.atributo("no-if", o))
                    g.noThen = try FormatoNET.convertToBoolean(try x.atributo("no-then", o))
                    cfg.grupos.append(g)
                }
            }
        }
        return cfg
    }

    // MARK: Escritura (:251-406)

    /// `GuardaArchivo(IFiltro)`: `<Condiciones>` con la condición si tiene datos.
    public static func datos(
        filtro tipo: TipoFiltro, de condiciones: CondicionesGrupo, cultura: Cultura
    ) throws(ErrorDominio) -> Data {
        var c = condiciones
        var w = cabecera()
        w.iniciarElemento("Condiciones")
        try DatosFiltro.escribir(tipo, &c, &w, cultura: cultura)
        return w.cerrar()
    }

    /// `GuardaArchivo(Grupo)`: un `grupo id="0"` con partidos activos, condiciones y tolerancias.
    public static func datos(grupo: GrupoConfig, cultura: Cultura) throws(ErrorDominio) -> Data {
        var g = grupo
        var w = cabecera()
        w.iniciarElemento("gruposPartidos")
        w.iniciarElemento("grupo")
        w.atributo("id", "0")
        w.atributo("nombre", g.nombreGrupo)
        DatosFiltro.elemento(&w, "partidosActivos", [("noPartidos", g.textoPartidosActivos)])
        try DatosFiltro.escribir(&g.condiciones, &w, cultura: cultura)
        escribirTolerancias(g.tolerancias, &w)
        return w.cerrar()
    }

    /// `GuardaArchivo(ControladorIfThen)`: sin If-Then solo queda la cabecera.
    public static func datos(ifThen: IfThenConfig?) -> Data {
        var w = cabecera()
        escribirIfThen(ifThen, &w)
        return w.cerrar()
    }

    /// `InicializaArchivo` + `EscribeVersionArchivo`: `<xml>` sin declaración y `<file version="0.1" />`.
    static func cabecera() -> EscritorXMLNET {
        var w = EscritorXMLNET()
        w.iniciarElemento("xml")
        DatosFiltro.elemento(&w, "file", [("version", "0.1")])
        return w
    }

    /// `EscribirToleranciasGrupo` (:383-407).
    static func escribirTolerancias(_ t: ControladorTolConfig, _ w: inout EscritorXMLNET) {
        w.iniciarElemento("tolerancias")
        for (i, x) in t.tolerancias.enumerated() {
            DatosFiltro.elemento(&w, "control", [("id", String(i)), ("letras", x.letrasTol), ("noAciertos", x.aciertos)])
        }
        DatosFiltro.elemento(&w, "controlFallos", [("noFallos", t.fallosPermitidos)])
        w.cerrarElemento()
    }

    /// `EscribeIfThen` (:329-374): `Rangos` solo si hay elementos.
    static func escribirIfThen(_ it: IfThenConfig?, _ w: inout EscritorXMLNET) {
        guard let it else { return }
        w.iniciarElemento("If-Then")
        w.iniciarElemento("Condiciones")
        for (i, c) in it.condiciones.enumerated() {
            DatosFiltro.elemento(&w, "Condicion", [("id", String(i)), ("If", c.condIf), ("Then", c.condThen)])
        }
        if !it.condiciones.isEmpty { DatosFiltro.elemento(&w, "Rangos", [("valor", it.rangoAciertoCondiciones ?? "")]) }
        w.cerrarElemento()
        w.iniciarElemento("Grupos")
        for (i, g) in it.grupos.enumerated() {
            DatosFiltro.elemento(
                &w, "Grupo",
                [
                    ("id", String(i)), ("if", String(g.numGrupoIf)), ("then", String(g.numGrupoThen)),
                    ("no-if", DatosFiltro.bool(g.noIf)), ("no-then", DatosFiltro.bool(g.noThen)),
                ])
        }
        if !it.grupos.isEmpty { DatosFiltro.elemento(&w, "Rangos", [("valor", it.rangoAciertoGrupos ?? "")]) }
        w.cerrarElemento()
        w.cerrarElemento()
    }
}
