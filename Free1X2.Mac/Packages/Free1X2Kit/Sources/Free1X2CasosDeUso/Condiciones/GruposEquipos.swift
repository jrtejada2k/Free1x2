import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Views/Ported/GruposEquiposFrmViewModel.cs (09 pantalla 62; CU-B30). Copia de trabajo de grupos y relaciones; el
// filtro `_filtroGE` es el del grupo en edición hasta Borrar o Abrir. DT-59: las listas son valores (el C# compartía la
// lista con el filtro tras `GuardarDatos`, y los cambios posteriores sin Aceptar llegaban al filtro).

/// Estado de la pantalla «Grupos de Equipos».
public struct EditorGruposEquipos: Sendable {
    /// `NumeroPartidos` es una constante de 14 en esta pantalla.
    public static let partidos = 14

    public var casa = Array(repeating: false, count: partidos)
    public var fuera = Array(repeating: false, count: partidos)
    public var victorias = ""
    public var empates = ""
    public var derrotas = ""
    public var sumaPuntos = ""
    public var relGrupos = ""
    public var relVictorias = ""
    public var relEmpates = ""
    public var relDerrotas = ""
    public var relPuntos = ""
    public var estado = "Preparado"
    public private(set) var grupos: [GrupoEquiposConfig]
    public private(set) var relaciones: [RelacionGE1Config]
    public private(set) var noGE = 0
    public private(set) var noRel = 0
    public private(set) var contadorGrupos = (actual: 1, total: 1)
    public private(set) var contadorRelaciones = (actual: 1, total: 1)
    /// `_filtroGE`.
    public private(set) var filtro: CondicionGruposEquipos
    /// `_filtroGE` es el del grupo en edición (deja de serlo tras Borrar o Abrir).
    public private(set) var ligado = true
    /// `_grupo` es el grupo en edición (tras Abrir es el del fichero: Aceptar no llega a la combinación, DEC-927).
    public private(set) var grupoReal = true

    /// `CargarDesdeGrupo` (:196-229).
    public init(_ f: CondicionGruposEquipos) {
        filtro = f
        grupos = f.grupos
        relaciones = f.relaciones
        pintarGrupo(0)
        pintarRelacion(0)
    }

    /// `LlenaEquipos` (:250-262): nombres de la jornada si la hay; si no, «Equipo casa N» / «Equipo fuera N».
    public static func equipos(_ jornada: JornadaQuiniela?) -> [(casa: String, fuera: String)] {
        (0..<partidos).map { i in
            guard let p = jornada?.partidos[safe: i] else { return ("Equipo casa \(i + 1)", "Equipo fuera \(i + 1)") }
            return (p.local, p.visitante)
        }
    }

    // MARK: Grupos

    /// `ActualizaDatosPantalla` (:264-290).
    mutating func pintarGrupo(_ n: Int) {
        let ge: GrupoEquiposConfig
        if let g = grupos[safe: n] {
            ge = g
            contadorGrupos = (n + 1, grupos.count)
        } else {
            ge = GrupoEquiposConfig(numeroPartidos: Self.partidos)
            contadorGrupos = (1, 1)
        }
        let c = Array(ge.pronosticos.utf16)
        for i in 0..<Self.partidos {
            let s = c[safe: i] ?? 0
            casa[i] = s == 0x31 || s == 0x33
            fuera[i] = s == 0x32 || s == 0x33
        }
        (sumaPuntos, victorias, empates, derrotas) = (ge.sumaPuntos.texto, ge.victorias.texto, ge.empates.texto, ge.derrotas.texto)
    }

    /// `ObtenEquiposSeleccionados`: `'0'` + 1 (casa) + 2 (fuera).
    var seleccion: String {
        String((0..<Self.partidos).map { Character(UnicodeScalar(UInt8(48 + (casa[$0] ? 1 : 0) + (fuera[$0] ? 2 : 0)))) })
    }

    var tieneGEDatos: Bool { casa.contains(true) || fuera.contains(true) }

    /// `SonEntradasValidas` (:357-372).
    var entradasValidas: Bool {
        let t = [victorias, empates, derrotas, sumaPuntos]
        return t.contains { !$0.isEmpty } && t.allSatisfy(UtilidadesEntradasValores.sonTodosNumeros)
    }

    /// `GuardaDatosGE` (:403-421).
    mutating func guardarDatos(en ge: inout GrupoEquiposConfig) throws(ErrorDominio) {
        ge.pronosticos = seleccion
        guard ge.pronosticos.utf16.contains(where: { $0 != 0 && $0 != 0x30 }) else { return }
        guard entradasValidas else {
            estado = "Hay errores en la entrada de datos del grupo"
            return
        }
        try ge.ponerVictorias(victorias)
        try ge.ponerEmpates(empates)
        try ge.ponerDerrotas(derrotas)
        try ge.ponerSumaPuntos(sumaPuntos)
    }

    /// `GuardarGEActual` (:423-437).
    mutating func guardarGEActual() throws(ErrorDominio) {
        if noGE >= grupos.count {
            guard tieneGEDatos else { return }
            grupos.append(GrupoEquiposConfig(numeroPartidos: Self.partidos))
        }
        let i = noGE < grupos.count ? noGE : grupos.count - 1
        var g = grupos[i]
        defer { grupos[i] = g }  // lo ya escrito se queda aunque lance (objeto compartido en el C#)
        try guardarDatos(en: &g)
    }

    mutating func cambiarGrupo(_ n: Int) throws(ErrorDominio) {
        try guardarGEActual()
        noGE = n
        if grupos.count < n + 1 { grupos.append(GrupoEquiposConfig(numeroPartidos: Self.partidos)) }
        pintarGrupo(n)
    }

    public mutating func grupoAnterior() throws(ErrorDominio) { if noGE > 0 { try cambiarGrupo(noGE - 1) } }
    /// Solo si el grupo en pantalla tiene algún equipo marcado.
    public mutating func grupoSiguiente() throws(ErrorDominio) { if tieneGEDatos { try cambiarGrupo(noGE + 1) } }

    /// `EliminarGrupo` (:749-771).
    public mutating func eliminarGrupo() throws(ErrorDominio) {
        if noGE == 0 {
            if !grupos.isEmpty { grupos.remove(at: 0) }
        } else {
            guard grupos.indices.contains(noGE) else { throw .fueraDeRangoLista("GruposEquiposFrmViewModel.cs:762") }
            grupos.remove(at: noGE)
            noGE -= 1
        }
        if grupos.isEmpty { grupos.append(GrupoEquiposConfig(numeroPartidos: Self.partidos)) }
        pintarGrupo(noGE)
    }

    static func necesitaBorrar(_ ge: GrupoEquiposConfig) -> Bool { !ge.pronosticos.utf16.contains { (0x31...0x33).contains($0) } }

    /// `GuardarDatos` (:483-526): copia de trabajo → filtro; tras tener datos queda activo siempre.
    public mutating func guardarDatos() throws(ErrorDominio) {
        try guardarGEActual()
        if let u = grupos.last {
            if Self.necesitaBorrar(u) { grupos.removeLast() }
        } else {
            filtro.activa = false
        }
        if !filtro.contieneDatos {
            if !grupos.isEmpty { (filtro.contieneDatos, filtro.activa) = (true, true) } else { filtro.activa = false }
        } else {
            (filtro.contieneDatos, filtro.activa) = (true, true)
        }
        filtro.ponerGrupos(grupos)
    }

    // MARK: Relaciones

    /// `ActualizaDatosPantRel1` (:560-580).
    mutating func pintarRelacion(_ n: Int) {
        let r: RelacionGE1Config
        if let x = relaciones[safe: n] {
            r = x
            contadorRelaciones = (n + 1, relaciones.count)
        } else {
            r = RelacionGE1Config()
            contadorRelaciones = (1, 1)
        }
        (relGrupos, relVictorias, relEmpates, relDerrotas, relPuntos) =
            (r.gruposEquipos, r.sumaVictorias.texto, r.sumaEmpates.texto, r.sumaDerrotas.texto, r.sumaPuntos.texto)
    }

    var tieneRelacionDatos: Bool {
        !relGrupos.isEmpty && ![relVictorias, relEmpates, relDerrotas, relPuntos].allSatisfy(\.isEmpty)
    }

    var relacionValida: Bool {
        let t = [relVictorias, relEmpates, relDerrotas, relPuntos]
        return t.contains { !$0.isEmpty } && t.allSatisfy(UtilidadesEntradasValores.sonTodosNumeros)
    }

    /// `GuardaDatosRel1` (:611-624).
    func guardarDatos(en r: inout RelacionGE1Config) throws(ErrorDominio) {
        guard tieneRelacionDatos && relacionValida else { return }
        try r.ponerGruposEquipos(relGrupos)
        try r.ponerSumaVictorias(relVictorias)
        try r.ponerSumaEmpates(relEmpates)
        try r.ponerSumaDerrotas(relDerrotas)
        try r.ponerSumaPuntos(relPuntos)
    }

    mutating func guardarRelacionActual() throws(ErrorDominio) {
        if noRel >= relaciones.count {
            guard tieneRelacionDatos && relacionValida else { return }
            relaciones.append(RelacionGE1Config())
        }
        let i = noRel < relaciones.count ? noRel : relaciones.count - 1
        var r = relaciones[i]
        defer { relaciones[i] = r }
        try guardarDatos(en: &r)
    }

    mutating func cambiarRelacion(_ n: Int) throws(ErrorDominio) {
        try guardarRelacionActual()
        noRel = n
        if relaciones.count < n + 1 { relaciones.append(RelacionGE1Config()) }
        pintarRelacion(n)
    }

    public mutating func relacionAnterior() throws(ErrorDominio) { if noRel > 0 { try cambiarRelacion(noRel - 1) } }
    public mutating func relacionSiguiente() throws(ErrorDominio) { if tieneRelacionDatos { try cambiarRelacion(noRel + 1) } }

    public mutating func eliminarRelacion() throws(ErrorDominio) {
        if noRel == 0 {
            if !relaciones.isEmpty { relaciones.remove(at: 0) }
        } else {
            guard relaciones.indices.contains(noRel) else { throw .fueraDeRangoLista("GruposEquiposFrmViewModel.cs:807") }
            relaciones.remove(at: noRel)
            noRel -= 1
        }
        if relaciones.isEmpty { relaciones.append(RelacionGE1Config()) }
        pintarRelacion(noRel)
    }

    static func necesitaBorrar(_ r: RelacionGE1Config) -> Bool {
        r.gruposEquipos.isEmpty || [r.sumaVictorias, r.sumaEmpates, r.sumaDerrotas, r.sumaPuntos].allSatisfy { $0.texto.isEmpty }
    }

    /// `GuardarDatosRelacionesGE1` (:702-724).
    public mutating func guardarDatosRelaciones() throws(ErrorDominio) {
        try guardarRelacionActual()
        if let u = relaciones.last, Self.necesitaBorrar(u) { relaciones.removeLast() }
        filtro.relaciones = relaciones.filter { !$0.gruposEquipos.isEmpty }
    }

    // MARK: Acciones

    /// `Aceptar` (:823-842): el filtro a aplicar al grupo, o `nil` si `_grupo` ya no es el de la combinación.
    public mutating func aceptar() throws(ErrorDominio) -> CondicionGruposEquipos? {
        try guardarDatos()
        try guardarDatosRelaciones()
        return grupoReal ? filtro : nil
    }

    /// `Borrar` (:845-862): el filtro nuevo queda desligado del grupo hasta Aceptar.
    public mutating func borrar() throws(ErrorDominio) {
        try guardarDatos()
        filtro = CondicionGruposEquipos()
        ligado = false
        (grupos, relaciones, noGE, noRel) = ([], [], 0, 0)
        pintarGrupo(0)
        pintarRelacion(0)
        estado = "Datos del filtro borrados"
    }

    /// `GuardarEn` (:878-889): con grupos fuerza `ContieneDatos = IsActive = true` en `_filtroGE`.
    public mutating func paraGuardar() -> CondicionGruposEquipos {
        if !filtro.grupos.isEmpty { (filtro.contieneDatos, filtro.activa) = (true, true) }
        return filtro
    }

    /// `AbrirDesde` (:896-909): `_grupo` y `_filtroGE` pasan a ser los del fichero.
    public mutating func abrir(_ leido: CondicionGruposEquipos) {
        filtro = leido
        (ligado, grupoReal) = (false, false)
        (grupos, noGE, noRel) = (leido.grupos, 0, 0)
        pintarGrupo(0)
        relaciones = leido.relaciones
        pintarRelacion(0)
    }

    /// `ObtenerFiltroTemporal` (:916-993): el grupo en pantalla también se vuelca en la copia de trabajo y, si el último
    /// queda vacío, se quita de ella (efecto lateral del C#).
    public mutating func temporal() throws(ErrorDominio) -> CondicionGruposEquipos {
        var t = CondicionGruposEquipos()
        var g = grupos
        if noGE < g.count {
            try guardarDatos(en: &g[noGE])
            grupos[noGE] = g[noGE]
        } else if tieneGEDatos {
            g.append(GrupoEquiposConfig(numeroPartidos: Self.partidos))
            try guardarDatos(en: &g[g.count - 1])
        }
        if let u = g.last, Self.necesitaBorrar(u) {
            guard grupos.indices.contains(g.count - 1) else { throw .fueraDeRangoLista("GruposEquiposFrmViewModel.cs:942") }
            grupos.remove(at: g.count - 1)
        }
        if !g.isEmpty { (t.contieneDatos, t.activa) = (true, true) }
        t.ponerGrupos(g)
        var r = relaciones
        if noRel < r.count {
            try guardarDatos(en: &r[noRel])
            relaciones[noRel] = r[noRel]
        } else if tieneRelacionDatos {
            r.append(RelacionGE1Config())
            try guardarDatos(en: &r[r.count - 1])
        }
        if let u = r.last, Self.necesitaBorrar(u) { r.removeLast() }
        t.relaciones = r.filter { !$0.gruposEquipos.isEmpty }
        return t
    }
}
