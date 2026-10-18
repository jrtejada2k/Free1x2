import Foundation
import Free1X2Base
import Free1X2Motor

extension CasosCondiciones {
    // MARK: Pantalla 88 · Signos Seguidos (CU-C30)

    /// `Aceptar` (`SignosSeguidosFrmViewModel.cs:141-178`): «todos» es la constante 0…14; las figuras no cuentan como
    /// datos y una lista vacía deja la que tuviera la condición.
    public static func aplicarSignosSeguidos(
        _ textos: [String], figuras: [[Int64]?], en f: inout CondicionSignosSeguidos
    ) throws(ErrorDominio) {
        let todos = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14"
        f.reinicializaValores()
        guard textos.contains(where: { !blanco($0) }) else {
            f.activa = false
            f.contieneDatos = false
            return
        }
        if !f.contieneDatos { f.activa = true }
        f.contieneDatos = true
        for c in CondicionSignosSeguidos.Clase.allCases {
            let t = textos[safe: c.rawValue] ?? ""
            try f.set(c, blanco(t) ? todos : t)
        }
        for (i, l) in figuras.enumerated() where i < f.figuras.count {
            if let l, !l.isEmpty { f.figuras[i] = l }
        }
    }

    // MARK: Pantalla 89 · Simetrías (CU-C31)

    public static let mensajeSimetriaInvalida =
        "Verifique que ha introducido una simetría correcta: (a,b) ó (a-b) ó (a,b-c), y que ha introducido un número válido de aciertos"

    /// `EntradaValida` (`SimetriasFrmViewModel.cs:96-117`): cada número en 1…N.
    static func simetriaValida(_ t: String, _ n: Int) -> Bool {
        guard !blanco(t) else { return false }
        var valida = false
        for parte in t.split(separator: ",", omittingEmptySubsequences: false) {
            for p in parte.split(separator: "-", omittingEmptySubsequences: false) {
                guard let a = try? FormatoNET.convertToInt32(FormatoNET.recortar(String(p))), a > 0, a <= n else { return false }
                valida = true
            }
        }
        return valida
    }

    /// `ObtenerSimetrias` (:120-131): `nil` si una fila no vacía no es válida; la expansión de `a-b` con `a > b` lanza.
    public static func simetrias(_ filas: [String], numeroPartidos n: Int) throws(ErrorDominio) -> [Simetria]? {
        var r: [Simetria] = []
        for t in filas where !blanco(t) {
            guard simetriaValida(t, n) else { return nil }
            r.append(try Simetria(try UtilidadesEntradasValores.obtenerValoresSeparadosPorComas(t)))
        }
        return r
    }

    /// `ObtenerAciertos` (:134-163): valores y rangos 0…40 (fuera se ignoran); cualquier error ⇒ `nil`.
    public static func aciertosSimetrias(_ texto: String) -> [Int]? {
        var r: [Int] = []
        guard !blanco(texto) else { return r }
        for parte in texto.split(separator: ",", omittingEmptySubsequences: false) {
            let p = FormatoNET.recortar(String(parte))
            if !p.contains("-") {
                guard let v = try? FormatoNET.convertToInt32(p) else { return nil }
                if (0...40).contains(v) { r.append(Int(v)) }
            } else {
                let i = p.split(separator: "-", omittingEmptySubsequences: false).map(String.init)
                guard i.count >= 2, let a = try? FormatoNET.convertToInt32(i[0]), let b = try? FormatoNET.convertToInt32(i[1])
                else { return nil }
                if a <= b { for j in a...b where (0...40).contains(j) { r.append(Int(j)) } }
            }
        }
        return r
    }

    /// Filtro con lo de pantalla (`ObtenerFiltroTemporal`, :216-236) o `nil` si la entrada no es válida.
    public static func condicionSimetrias(filas: [String], aciertos: String, numeroPartidos n: Int) throws(ErrorDominio)
        -> CondicionSimetrias?
    {
        guard let s = try simetrias(filas, numeroPartidos: n), let a = aciertosSimetrias(aciertos) else { return nil }
        var f = CondicionSimetrias()
        f.simetrias = s
        f.aciertos = a
        f.aciertosTexto = blanco(aciertos) ? "0" : aciertos
        f.activa = f.contieneDatos()
        return f
    }

    /// `Aceptar` (:188-209): sustituye la condición y **siempre** la reactiva si tiene simetrías; `false` = entrada no
    /// válida (no se cierra la pantalla ni se avisa).
    public static func aplicarSimetrias(filas: [String], aciertos: String, en f: inout CondicionSimetrias, numeroPartidos n: Int)
        throws(ErrorDominio) -> Bool
    {
        guard let nueva = try condicionSimetrias(filas: filas, aciertos: aciertos, numeroPartidos: n) else { return false }
        f = nueva
        return true
    }
}
