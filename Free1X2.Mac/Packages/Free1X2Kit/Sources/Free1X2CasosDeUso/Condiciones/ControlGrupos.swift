import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Views/Ported/ControlGruposFrmViewModel.cs y ControlTolFrmViewModel.cs (08 pantallas 27 y 28; CU-A72…A74).

/// «Control de Grupos»: copias de los controles de grupos y de conjuntos; el control 0 (libres) no se muestra.
public struct EditorControlGrupos: Sendable {
    public var grupos = ""
    public var fallosGrupos = ""
    public var conjuntos = ""
    public var fallosConjuntos = ""
    public var estado = "Preparado"
    public private(set) var controles: [ControlGruposConfig]
    public private(set) var controlesConjuntos: [ControlConjuntosConfig]
    public private(set) var cg = 1
    public private(set) var cc = 1
    public private(set) var contadorGrupos = "1/1"
    public private(set) var contadorConjuntos = "1/1"
    /// Veces que el C# habría mostrado `ControlGruposConfig.mensajeError` («Hay errores en los valores introducidos»).
    public var errores = 0
    let numeroGrupos: Int

    /// `CargarDesdeMotor` (:80-90).
    public init(_ c: Combinacion) {
        controles = c.controlesGrupos
        controlesConjuntos = c.controlesConjuntos
        numeroGrupos = c.grupos.count
        pintarGrupos(1)
        pintarConjuntos(1)
    }

    // MARK: Controles de grupos

    /// `ActualizaDatosPantalla` (:118-133).
    mutating func pintarGrupos(_ n: Int) {
        if controles.count > 1, let c = controles[safe: n] {
            (grupos, fallosGrupos, contadorGrupos) = (c.textoGruposControlados, c.textoFallosPermitidos, "\(n)/\(controles.count - 1)")
        } else {
            (grupos, fallosGrupos, contadorGrupos) = ("", "", "1/1")
        }
    }

    var tieneDatosGrupos: Bool { !grupos.isEmpty && !fallosGrupos.isEmpty }

    /// `GuardaDatosCG`: cada `Poner*` que falla avisa (y se sigue).
    mutating func guardar(en c: inout ControlGruposConfig) throws(ErrorDominio) {
        if !(try c.ponerGruposControlados(grupos)) { errores += 1 }
        if !(try c.ponerFallosPermitidos(fallosGrupos)) { errores += 1 }
    }

    /// `GuardarCGActual` (:141-155).
    mutating func guardarGrupoActual() throws(ErrorDominio) {
        guard tieneDatosGrupos else { return }
        if cg >= controles.count { controles.append(ControlGruposConfig()) }
        let i = cg < controles.count ? cg : controles.count - 1
        var c = controles[i]
        defer { controles[i] = c }
        try guardar(en: &c)
    }

    mutating func cambiarGrupo(_ n: Int) throws(ErrorDominio) {
        try guardarGrupoActual()
        cg = n
        if controles.count < n + 1 { controles.append(ControlGruposConfig()) }
        pintarGrupos(n)
    }

    public mutating func gruposAnterior() throws(ErrorDominio) { if cg > 1 { try cambiarGrupo(cg - 1) } }
    public mutating func gruposSiguiente() throws(ErrorDominio) { if tieneDatosGrupos { try cambiarGrupo(cg + 1) } }

    /// `GruposEliminarActual` (:369-383): fuera del primero, `RemoveAt` sin comprobar.
    public mutating func gruposEliminar() throws(ErrorDominio) {
        if cg == 1 {
            if controles.count > 1 { controles.remove(at: cg) }
        } else {
            guard controles.indices.contains(cg) else { throw .fueraDeRangoLista("ControlGruposFrmViewModel.cs:377") }
            controles.remove(at: cg)
            cg -= 1
        }
        pintarGrupos(cg)
    }

    /// `GuardarDatos` (:207-214) con `GuardarGruposLibres` (:179-205).
    mutating func confirmarGrupos() throws(ErrorDominio) {
        try guardarGrupoActual()
        if controles.count > 1, let u = controles.last, u.textoGruposControlados.isEmpty || u.textoFallosPermitidos.isEmpty {
            controles.removeLast()
        }
        var libres = "0"
        for g in 1..<max(numeroGrupos, 1) where !controles.dropFirst().contains(where: { $0.contieneGrupo(g) }) { libres += ",\(g)" }
        guard !controles.isEmpty else { throw .fueraDeRangoLista("ControlGruposFrmViewModel.cs:201") }
        if !(try controles[0].ponerGruposControlados(libres)) { errores += 1 }
        controles[0].usaControlGrupos = false
    }

    // MARK: Controles de conjuntos

    mutating func pintarConjuntos(_ n: Int) {
        if controlesConjuntos.count > 1, let c = controlesConjuntos[safe: n] {
            (conjuntos, fallosConjuntos, contadorConjuntos) =
                (c.textoCtrlGruposControlados, c.textoFallosPermitidos, "\(n)/\(controlesConjuntos.count - 1)")
        } else {
            (conjuntos, fallosConjuntos, contadorConjuntos) = ("", "", "1/1")
        }
    }

    var tieneDatosConjuntos: Bool { !conjuntos.isEmpty && !fallosConjuntos.isEmpty }

    mutating func guardarConjuntoActual() throws(ErrorDominio) {
        guard tieneDatosConjuntos else { return }
        if cc >= controlesConjuntos.count { controlesConjuntos.append(ControlConjuntosConfig()) }
        let i = cc < controlesConjuntos.count ? cc : controlesConjuntos.count - 1
        var c = controlesConjuntos[i]
        defer { controlesConjuntos[i] = c }
        try c.ponerCtrlGruposControlados(conjuntos)
        try c.ponerFallosPermitidos(fallosConjuntos)
    }

    mutating func cambiarConjunto(_ n: Int) throws(ErrorDominio) {
        try guardarConjuntoActual()
        cc = n
        if controlesConjuntos.count < n + 1 { controlesConjuntos.append(ControlConjuntosConfig()) }
        pintarConjuntos(n)
    }

    public mutating func conjuntosAnterior() throws(ErrorDominio) { if cc > 1 { try cambiarConjunto(cc - 1) } }
    public mutating func conjuntosSiguiente() throws(ErrorDominio) { if tieneDatosConjuntos { try cambiarConjunto(cc + 1) } }

    public mutating func conjuntosEliminar() throws(ErrorDominio) {
        if cc == 1 {
            if controlesConjuntos.count > 1 { controlesConjuntos.remove(at: cc) }
        } else {
            guard controlesConjuntos.indices.contains(cc) else { throw .fueraDeRangoLista("ControlGruposFrmViewModel.cs:403") }
            controlesConjuntos.remove(at: cc)
            cc -= 1
        }
        pintarConjuntos(cc)
    }

    /// `GuardarDatosCConj` (:315-322) con `GuardarGruposLibresCConj` (:290-313): usa el nuevo nº de controles de grupos.
    mutating func confirmarConjuntos(numeroControles: Int) throws(ErrorDominio) {
        try guardarConjuntoActual()
        if controlesConjuntos.count > 1, let u = controlesConjuntos.last,
            u.textoCtrlGruposControlados.isEmpty || u.textoFallosPermitidos.isEmpty
        {
            controlesConjuntos.removeLast()
        }
        var libres = "0"
        for k in 1..<max(numeroControles, 1) where !controlesConjuntos.dropFirst().contains(where: { $0.contieneConjunto(k) }) {
            libres += ",\(k)"
        }
        guard !controlesConjuntos.isEmpty else { throw .fueraDeRangoLista("ControlGruposFrmViewModel.cs:310") }
        try controlesConjuntos[0].ponerCtrlGruposControlados(libres)
        try controlesConjuntos[0].ponerFallosPermitidos("0")
    }

    /// `Aceptar` (:412-420).
    public mutating func aceptar(en c: inout Combinacion) throws(ErrorDominio) {
        try confirmarGrupos()
        c.controlesGrupos = controles
        try confirmarConjuntos(numeroControles: c.controlesGrupos.count)
        c.asignarControlesConjuntos(controlesConjuntos)
        estado = "Guardado"
    }
}

extension CasosCondiciones {
    /// «Control Tolerancias» al entrar (`ControlTolFrmViewModel.cs:60-80`): al menos 30 filas.
    public static func filasTolerancias(_ t: ControladorTolConfig) -> [(letras: String, aciertos: String)] {
        var f = t.tolerancias.map { ($0.letrasTol, $0.aciertos) }
        while f.count < 30 { f.append(("", "")) }
        return f
    }

    /// `Ok` (:83-110): solo las filas con aciertos; luego los fallos permitidos.
    public static func aplicarTolerancias(_ filas: [(letras: String, aciertos: String)], fallos: String, en t: inout ControladorTolConfig)
        throws(ErrorDominio)
    {
        var lista: [ToleranciaConfig] = []
        for (letras, aciertos) in filas where !aciertos.isEmpty {
            var x = ToleranciaConfig()
            x.letrasTol = letras
            try x.ponerAciertos(aciertos)
            lista.append(x)
        }
        t.tolerancias = lista
        try t.ponerFallosPermitidos(fallos)
    }
}
