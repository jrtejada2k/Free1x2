import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/IfThenFrmViewModel.cs (09 pantalla 65; CU-B33). Las relaciones se guardan como texto (contrato `.if`).

/// Una relación «si … entonces …» tal como se lista.
public struct RelacionIfThen: Sendable, Hashable, Identifiable {
    public let id = UUID()
    public var si: String
    public var entonces: String

    public init(_ si: String, _ entonces: String) { (self.si, self.entonces) = (si, entonces) }

    public static func == (a: Self, b: Self) -> Bool { a.si == b.si && a.entonces == b.entonces }
    public func hash(into h: inout Hasher) {
        h.combine(si)
        h.combine(entonces)
    }
}

/// Lo que muestra la pantalla tras cargar un If-Then.
public struct PantallaIfThen: Sendable, Hashable {
    public var relaciones: [RelacionIfThen] = []
    public var rangoCondiciones = ""
    public var grupos: [RelacionIfThen] = []
    public var rangoGrupos = ""
    /// Grupos del fichero que no existen en la combinación (uno por aviso).
    public var gruposQuitados = 0

    public init() {}
}

extension CasosCondiciones {
    public static let genericasIfThen = [
        "Cantidad de signos", "Dibujos", "Signos Seguidos", "Interrupciones", "Pesos numéricos", "Distancias", "Contactos",
    ]
    public static let mensajeGruposQuitados =
        "Uno de los grupos del fichero no existe en la combinación actual. Se quita de la lista."

    /// `RellenarEspecificas` (:237-326): los dibujos no llevan valor.
    public static func especificasIfThen(_ generica: String?, numeroPartidos n: Int) -> (items: [String], usaValor: Bool) {
        switch generica {
        case "Cantidad de signos": (["Cantidad de Variantes", "Cantidad de X", "Cantidad de 2"], true)
        case "Dibujos": ((0...n).flatMap { i in (0...n).filter { i + $0 <= n }.map { "Dibujo \(i)+\($0)" } }, false)
        case "Signos Seguidos": (["Variantes", "1", "X", "2"].map { "Signos Seguidos de " + $0 }, true)
        case "Interrupciones":
            (
                ["Interrupciones Globales"] + ["Variantes", "1", "X", "2"].map { "Interrupciones de " + $0 }
                    + ["Interrupciones Seguidas Globales"] + ["Variantes", "1", "X", "2"].map { "Interrupciones Seguidas de " + $0 },
                true
            )
        case "Pesos numéricos": (["Peso numérico Global"] + ["Variantes", "1", "X", "2"].map { "Peso numérico de " + $0 }, true)
        case "Distancias": (["Variantes", "1", "X", "2"].map { "Distancia de " + $0 }, true)
        case "Contactos": (CondicionContactos.Contacto.allCases.map { "Contactos de " + $0.par }, true)
        default: ([], true)
        }
    }

    /// `ComponerCondicion` (:362-388): «Cantidad de Variantes: 5», «(NO) Interrupciones de 1: 3», «Dibujo 3+4».
    public static func componerCondicionIfThen(generica: String?, especifica: String?, valor: Double, usaValor: Bool, negado: Bool)
        -> String?
    {
        guard let generica, !generica.isEmpty else { return nil }
        var t: String
        if let especifica, !especifica.isEmpty {
            t = especifica
            if !especifica.contains("Dibujo") { t += ": " }
        } else {
            t = generica + ": "
        }
        if usaValor || (especifica ?? "").isEmpty { t += String(FormatoNET.castInt32(valor)) }
        return negado ? "(NO) " + t : t
    }

    /// `AnadirCondicion` (:331-355): `nil` si falta alguna genérica.
    public static func relacionIfThen(_ si: String?, _ entonces: String?) throws(ErrorDominio) -> RelacionIfThen? {
        guard let si, let entonces else { return nil }
        guard si != entonces, !(si.contains("Dibujo") && entonces.contains("Dibujo")) else {
            throw ErrorDominio(.general, "Las condiciones deben ser diferentes")
        }
        return RelacionIfThen(si, entonces)
    }

    /// `LlenarGrupos` (:94-106): «i - NombreGrupo» para i = 1…n−1.
    public static func gruposIfThen(_ c: Combinacion) -> [String] {
        c.grupos.indices.dropFirst().map { "\($0) - \(c.grupos[$0].nombreGrupo)" }
    }

    /// `AnadirGrupo` (:407-426).
    public static func relacionGruposIfThen(_ si: String?, negadoSi: Bool, _ entonces: String?, negadoEntonces: Bool)
        throws(ErrorDominio) -> RelacionIfThen?
    {
        guard let si, !blanco(si), let entonces, !blanco(entonces) else { return nil }
        guard si != entonces else { throw ErrorDominio(.general, "Los grupos deben ser diferentes") }
        return RelacionIfThen((negadoSi ? "(NO) " : "") + si, (negadoEntonces ? "(NO) " : "") + entonces)
    }

    /// `GuardarCondicion` (:448-510): los dos avisos de rango vacío salen como `ErrorDominio(.general)`.
    public static func construirIfThen(_ p: PantallaIfThen, combinacion c: Combinacion) throws(ErrorDominio) -> IfThenConfig {
        if p.rangoCondiciones.isEmpty && !p.relaciones.isEmpty {
            throw ErrorDominio(.general, "Debe indicarse la cantidad de condiciones que se deben cumplir.")
        }
        if p.rangoGrupos.isEmpty && !p.grupos.isEmpty {
            throw ErrorDominio(.general, "Debe indicarse la cantidad de grupos que se deben cumplir.")
        }
        var it = IfThenConfig()
        for r in p.relaciones {
            var x = CondicionIfThenConfig()
            x.condIf = r.si
            x.condThen = r.entonces
            it.condiciones.append(x)
        }
        if !p.relaciones.isEmpty { try it.ponerRangoAciertoCondiciones(p.rangoCondiciones) }
        func grupo(_ texto: String) throws(ErrorDominio) -> (Int, Bool) {
            var t = texto
            let no = t.hasPrefix("(NO)")
            if no { t = String(t.dropFirst(5)) }
            guard let e = t.firstIndex(of: " ") else { throw .fueraDeRangoLista("IfThenFrmViewModel.cs:479") }
            let n = Int(try FormatoNET.convertToInt16(String(t[..<e])))
            guard c.grupos.indices.contains(n) else { throw .fueraDeRangoLista("IfThenFrmViewModel.cs:481") }
            return (n, no)
        }
        for r in p.grupos {
            var g = GrupoIfThenConfig()
            (g.numGrupoIf, g.noIf) = try grupo(r.si)
            (g.numGrupoThen, g.noThen) = try grupo(r.entonces)
            it.grupos.append(g)
        }
        if !p.grupos.isEmpty { try it.ponerRangoAciertoGrupos(p.rangoGrupos) }
        return it
    }

    /// `CargarDatos` (:108-148): rangos solo si hay elementos; grupos inexistentes se quitan con aviso.
    public static func cargarIfThen(_ it: IfThenConfig, combinacion c: Combinacion) -> PantallaIfThen {
        var p = PantallaIfThen()
        p.relaciones = it.condiciones.map { RelacionIfThen($0.condIf, $0.condThen) }
        if !it.condiciones.isEmpty { p.rangoCondiciones = it.rangoAciertoCondiciones ?? "" }
        for g in it.grupos {
            guard c.grupos.indices.contains(g.numGrupoIf), c.grupos.indices.contains(g.numGrupoThen) else {
                p.gruposQuitados += 1
                continue
            }
            let si = (g.noIf ? "(NO) " : "") + "\(g.numGrupoIf) - \(c.grupos[g.numGrupoIf].nombreGrupo)"
            let entonces = (g.noThen ? "(NO) " : "") + "\(g.numGrupoThen) - \(c.grupos[g.numGrupoThen].nombreGrupo)"
            p.grupos.append(RelacionIfThen(si, entonces))
        }
        if !it.grupos.isEmpty { p.rangoGrupos = it.rangoAciertoGrupos ?? "" }
        return p
    }

    /// Guardar/Copiar `.if` (`ArchivoCondiciones.GuardaArchivo(ifThen)`).
    public static func exportarIfThen(_ it: IfThenConfig, en url: URL, entorno: Entorno) throws(ErrorDominio) {
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(ArchivoCondiciones.datos(ifThen: it), en: url, atomico: true)
    }

    /// Abrir/Pegar: `nil` si el XML no carga o no trae If-Then.
    public static func importarIfThen(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> IfThenConfig? {
        guard let doc = ArchivoCondiciones.abrir(try entorno.archivos.leer(url)) else { return nil }
        return try ArchivoCondiciones.leerIfThen(doc)
    }
}
