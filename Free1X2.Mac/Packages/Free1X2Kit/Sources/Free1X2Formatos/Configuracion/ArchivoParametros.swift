import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Free1X2.Domain/EntradaSalida/AConfiguracion.cs y la carga de VariablesGlobales.cs:62-175 (07 F01).
// No se portan el diccionario `.lang` (DEC-707), la fecha de notificaciones ni `ObtenEquipos` (vestigiales, DP-08).

/// `parametros.free1x2` cargado: cada `Obten*` navega como el C# y cada `Guardar*` cambia sus atributos y re-serializa
/// el documento entero (los nodos desconocidos y comentarios se conservan).
public struct ArchivoParametros: Sendable {
    public private(set) var documento: NodoXML

    public init(_ datos: Data) throws(ErrorDominio) {
        documento = try LectorXML.cargar(datos)
    }

    /// Bytes del fichero tras un `Guardar*` (`configFile.Save(new XmlTextWriter(ruta, null))`).
    public var datos: Data { EscritorXMLNET.guardar(documento) }

    /// `GetElementsByTagName(tag)[0].ChildNodes[0]` (NRE si falta cualquiera de los dos).
    func nodo(_ tag: String, _ o: String) throws(ErrorDominio) -> NodoXML {
        guard let n = documento.primerElemento(tag)?.hijo(0) else { throw .referenciaNula(o) }
        return n
    }

    func ruta(_ tag: String, _ o: String) throws(ErrorDominio) -> [Int] {
        guard let r = documento.rutaPrimerElemento(tag), documento[ruta: r].hijo(0) != nil else {
            throw .referenciaNula(o)
        }
        return r + [0]
    }

    /// `Attributes[x].Value = v` en el primer hijo de `tag`, en orden; ausente ⇒ NRE (lo ya cambiado se queda).
    mutating func cambiar(_ tag: String, _ pares: [(String, String)], _ o: String) throws(ErrorDominio) {
        let r = try ruta(tag, o)
        for (k, v) in pares { try documento[ruta: r].cambiarAtributo(k, v, o) }
    }

    // MARK: Obten* (:58-453)

    public func puntosCP() throws(ErrorDominio) -> PuntosCP {
        let o = "AConfiguracion.cs:62-66"
        let n = try nodo("CP", o)
        return PuntosCP(
            fijos: Int(try FormatoNET.convertToInt32(try n.atributo("fijos", o))),
            dobles: Int(try FormatoNET.convertToInt32(try n.atributo("dobles", o))),
            triples: Int(try FormatoNET.convertToInt32(try n.atributo("triples", o))))
    }

    /// `ObtenToolBarsVisibles` (:68-98): sin nodo o con atributo ausente o no booleano ⇒ `true`.
    public func barras() -> [Bool] {
        let nombres = ["tsFree", "tsFiltros", "tsCombinacion", "tsOperaciones", "tsArchivo", "tsUtilidades"]
        guard let n = documento.primerElemento("toolbars")?.hijo(0) else { return Array(repeating: true, count: 6) }
        return nombres.map { n.atributoOpcional($0).flatMap { try? FormatoNET.convertToBoolean($0) } ?? true }
    }

    public func actualizarAlInicio() throws(ErrorDominio) -> Bool {
        let o = "AConfiguracion.cs:103-105"
        return try FormatoNET.convertToBoolean(try nodo("actualizador", o).atributo("actualizarAlInicio", o))
    }

    public func confirmacionSalir() throws(ErrorDominio) -> Bool {
        let o = "AConfiguracion.cs:112-114"
        return try FormatoNET.convertToBoolean(try nodo("salir", o).atributo("confirmacionSalir", o))
    }

    /// `ObtenConfiguracionAnalisis` (:117-143): asigna bandera a bandera (`ref`); lo leído antes de un error se queda.
    public func analisis(_ a: inout OpcionesAnalisis) throws(ErrorDominio) {
        let o = "AConfiguracion.cs:121-142"
        let n = try nodo("analisis", o)
        func b(_ nombre: String) throws(ErrorDominio) -> Bool {
            try FormatoNET.convertToBoolean(try n.atributo(nombre, o))
        }
        a.todo = try b("aTodo")
        a.vx2 = try b("aVX2")
        a.seguidos = try b("aSeguidos")
        a.figurasV1X2 = try b("aFigurasV1X2")
        a.interrupciones = try b("aInterrupciones")
        a.simetrias = try b("aSimetrias")
        a.formatos = try b("aFormatos")
        a.formatos123 = try b("aFormatos123")
        a.distancias = try b("aDistancias")
        a.contactos = try b("aContactos")
        a.figurasContactos = try b("aFigurasContactos")
        a.pesos = try b("aPesos")
        a.figurasPesos = try b("aFigurasPesos")
        a.valoracion = try b("aValoracion")
        a.cps = try b("aCPs")
        a.dibujos = try b("aDibujos")
        a.gruposEquipos = try b("aGruposEquipos")
        a.controlGrupos = try b("aControlGrupos")
        a.controlConjuntos = try b("aControlConjuntos")
        a.diferencias = try b("aDiferencias")
    }

    /// `ObtenNumPartidos(ref int, ref string[])` (:145-152): `Convert.ToInt16` y `separador.Split(',')`.
    public func partidos() throws(ErrorDominio) -> (numero: Int, separador: [String]) {
        let o = "AConfiguracion.cs:148-151"
        let n = try nodo("partidos", o)
        let numero = Int(try FormatoNET.convertToInt16(try n.atributo("numero", o)))
        return (numero, trozos(try n.atributo("separador", o), ","))
    }

    /// `ObtenNumPartidos(ref int, ref string)` (:153-159): el separador como texto.
    public func separadorTexto() throws(ErrorDominio) -> String {
        let o = "AConfiguracion.cs:156-158"
        return try nodo("partidos", o).atributo("separador", o)
    }

    /// `ObtenDesplazamiento` (:160-171): cualquier fallo se ignora y queda `previo`.
    public func desplazamiento(previo: Int) -> Int {
        guard let v = try? nodo("desplazamiento", "").atributo("valor", ""), let d = try? FormatoNET.convertToInt16(v)
        else { return previo }
        return Int(d)
    }

    public func idioma() throws(ErrorDominio) -> String {
        let o = "AConfiguracion.cs:175-176"
        return try nodo("idioma", o).atributo("valor", o)
    }

    /// `ObtenValoresLAE(ref double…)` (:402-413): `float.Parse(…, es-ES)` ensanchado a `Double` (T3).
    public func lae() throws(ErrorDominio) -> ConfigLAE {
        let o = "AConfiguracion.cs:407-412"
        let n = try nodo("LAE", o)
        var l = ConfigLAE()
        l.precioApuesta = Double(try FormatoNET.parseFloat(try n.atributo("PrecioApuesta", o), cultura: .esES))
        l.porcentaje14 = Double(try FormatoNET.parseFloat(try n.atributo("PorcentajeDestinadoAl14", o), cultura: .esES))
        l.recaudacion = Double(try FormatoNET.parseFloat(try n.atributo("Recaudacion", o), cultura: .esES))
        l.moneda = try n.atributo("Moneda", o)
        return l
    }

    /// DT-97: el Progol en `<Progol><OrdenarApuestas …/></Progol>`, con los mismos atributos que `<LAE>` (el C# solo lee el
    /// primer hijo de `<LAE>`, así que no le afecta). Sin el nodo, o con un valor ilegible, los de fábrica.
    public func laeMexico() -> ConfigLAE {
        var l = ConfigLAE.mexico
        guard let n = documento.primerElemento("Progol")?.hijo(0) else { return l }
        func numero(_ a: String) -> Double? {
            n.atributoOpcional(a).flatMap { try? FormatoNET.parseFloat($0, cultura: .esES) }.map(Double.init)
        }
        l.precioApuesta = numero("PrecioApuesta") ?? l.precioApuesta
        l.porcentaje14 = numero("PorcentajeDestinadoAl14") ?? l.porcentaje14
        l.recaudacion = numero("Recaudacion") ?? l.recaudacion
        l.moneda = n.atributoOpcional("Moneda") ?? l.moneda
        return l
    }

    /// `ObtenValoresLAE(ref string…)` (:415-424): los textos tal cual.
    public func laeTextos() throws(ErrorDominio) -> [String] {
        let o = "AConfiguracion.cs:420-423"
        let n = try nodo("LAE", o)
        var r: [String] = []
        for a in ["PrecioApuesta", "PorcentajeDestinadoAl14", "Recaudacion", "Moneda"] { r.append(try n.atributo(a, o)) }
        return r
    }

    /// `ObtenValoresUtilSeparadorJB` (:448-453): el propio `separadorJB`, no su hijo.
    public func separadorJB() throws(ErrorDominio) -> String {
        let o = "AConfiguracion.cs:452"
        guard let n = documento.primerElemento("separadorJB") else { throw .referenciaNula(o) }
        return try n.atributo("valores", o)
    }

    // MARK: Guardar* (:209-400)

    public mutating func guardarPuntosCP(_ p: PuntosCP) throws(ErrorDominio) {
        try cambiar(
            "CP", [("fijos", String(p.fijos)), ("dobles", String(p.dobles)), ("triples", String(p.triples))],
            "AConfiguracion.cs:213-215")
    }

    public mutating func guardarBarras(_ b: [Bool]) throws(ErrorDominio) {
        let nombres = ["tsFree", "tsFiltros", "tsCombinacion", "tsOperaciones", "tsArchivo", "tsUtilidades"]
        try cambiar("toolbars", zip(nombres, b).map { ($0, DatosFiltro.bool($1)) }, "AConfiguracion.cs:228-233")
    }

    public mutating func guardarActualizarAlInicio(_ v: Bool) throws(ErrorDominio) {
        try cambiar("actualizador", [("actualizarAlInicio", DatosFiltro.bool(v))], "AConfiguracion.cs:246")
    }

    public mutating func guardarConfirmacionSalir(_ v: Bool) throws(ErrorDominio) {
        try cambiar("salir", [("confirmacionSalir", DatosFiltro.bool(v))], "AConfiguracion.cs:393")
    }

    /// `GuardarConfiguracionAnalisis` (:254-286): en el orden de escritura del C#.
    public mutating func guardarAnalisis(_ a: OpcionesAnalisis) throws(ErrorDominio) {
        let b = DatosFiltro.bool
        try cambiar(
            "analisis",
            [
                ("aTodo", b(a.todo)), ("aVX2", b(a.vx2)), ("aSeguidos", b(a.seguidos)), ("aFigurasV1X2", b(a.figurasV1X2)),
                ("aDibujos", b(a.dibujos)), ("aInterrupciones", b(a.interrupciones)), ("aFormatos", b(a.formatos)),
                ("aFormatos123", b(a.formatos123)), ("aSimetrias", b(a.simetrias)), ("aContactos", b(a.contactos)),
                ("aFigurasContactos", b(a.figurasContactos)), ("aPesos", b(a.pesos)), ("aFigurasPesos", b(a.figurasPesos)),
                ("aCPs", b(a.cps)), ("aValoracion", b(a.valoracion)), ("aGruposEquipos", b(a.gruposEquipos)),
                ("aDistancias", b(a.distancias)), ("aControlGrupos", b(a.controlGrupos)),
                ("aControlConjuntos", b(a.controlConjuntos)), ("aDiferencias", b(a.diferencias)),
            ], "AConfiguracion.cs:260-279")
    }

    public mutating func guardarBoleto(numeroPartidos: Int, separador: String) throws(ErrorDominio) {
        try cambiar("partidos", [("numero", String(numeroPartidos)), ("separador", separador)], "AConfiguracion.cs:292-293")
    }

    /// `GuardarDesplazamiento` (:301-315): dentro de `try/catch` vacío; si falla no se guarda nada (`false`).
    @discardableResult
    public mutating func guardarDesplazamiento(_ num: String) -> Bool {
        (try? cambiar("desplazamiento", [("valor", num)], "AConfiguracion.cs:307")) != nil
    }

    /// `GuardarIdioma` (:332-346): como `GuardarDesplazamiento`.
    @discardableResult
    public mutating func guardarIdioma(_ idioma: String) -> Bool {
        (try? cambiar("idioma", [("valor", idioma)], "AConfiguracion.cs:338")) != nil
    }

    /// `GuardarValoresSeparadorJB` (:349-365): sobre el primer hijo de `utilidades`.
    public mutating func guardarSeparadorJB(_ valores: [String]) throws(ErrorDominio) {
        try cambiar("utilidades", [("valores", valores.joined(separator: ","))], "AConfiguracion.cs:359")
    }

    /// `GuardarValoresLAE` (:367-385): precio con `"0.00"` y el resto con `ToString()`, todo en `es-ES`.
    public mutating func guardarLAE(_ l: ConfigLAE) throws(ErrorDominio) {
        try cambiar(
            "LAE",
            [
                ("PrecioApuesta", FormatoNET.toStringFijo(l.precioApuesta, decimales: 2, cultura: .esES)),
                ("PorcentajeDestinadoAl14", FormatoNET.toString(l.porcentaje14, cultura: .esES)),
                ("Recaudacion", FormatoNET.toString(l.recaudacion, cultura: .esES)), ("Moneda", l.moneda),
            ], "AConfiguracion.cs:373-376")
    }

    /// DT-97: como `guardarLAE`, en `<Progol>`; si el fichero aún no lo tiene, se añade detrás de `<LAE>` (al final de
    /// `<configuracionONLAE>`, o de la raíz `<xml>` si falta).
    public mutating func guardarLAEMexico(_ l: ConfigLAE) {
        let atributos = [
            AtributoXML("PrecioApuesta", FormatoNET.toStringFijo(l.precioApuesta, decimales: 2, cultura: .esES)),
            AtributoXML("PorcentajeDestinadoAl14", FormatoNET.toString(l.porcentaje14, cultura: .esES)),
            AtributoXML("Recaudacion", FormatoNET.toString(l.recaudacion, cultura: .esES)), AtributoXML("Moneda", l.moneda),
        ]
        if let r = documento.rutaPrimerElemento("Progol"), !documento[ruta: r].hijos.isEmpty {
            documento[ruta: r + [0]].atributos = atributos
            return
        }
        guard let padre = documento.rutaPrimerElemento("configuracionONLAE") ?? documento.rutaPrimerElemento("xml") else { return }
        documento[ruta: padre].hijos.append(.elemento("Progol", hijos: [.elemento("OrdenarApuestas", atributos)]))
    }

    // MARK: Carga de VariablesGlobales

    /// `InicializarVariables` (:72-100): idioma → CP → desplazamiento → partidos → barras → actualizador → análisis → LAE →
    /// salir. Fichero ausente o cualquier error ⇒ `SetDefaultValues` (:149-175), que **no** restaura las banderas de
    /// análisis ya leídas salvo `aTodo` (vuelve a `true`).
    public static func cargarConfiguracion(_ datos: Data?) -> Configuracion {
        var analisis = OpcionesAnalisis()
        do throws(ErrorDominio) {
            guard let datos else { throw ErrorDominio(.archivoNoEncontrado) }
            let p = try ArchivoParametros(datos)
            var c = Configuracion()
            c.interfaz.idioma = try p.idioma()
            c.puntosCP = try p.puntosCP()
            c.boleto.desplazamiento = p.desplazamiento(previo: 0)
            (c.boleto.numeroPartidos, c.boleto.separador) = try p.partidos()
            c.interfaz.barras = p.barras()
            c.interfaz.actualizarAlInicio = try p.actualizarAlInicio()
            try p.analisis(&analisis)
            c.analisis = analisis
            c.lae = try p.lae()
            c.laeMexico = p.laeMexico()
            c.interfaz.confirmarAlSalir = try p.confirmacionSalir()
            return c
        } catch {
            var c = Configuracion.porDefecto
            analisis.todo = true
            c.analisis = analisis
            return c
        }
    }
}
