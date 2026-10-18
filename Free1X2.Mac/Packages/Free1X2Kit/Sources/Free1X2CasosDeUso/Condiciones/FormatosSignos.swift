import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Views/Ported/FormatosFrmViewModel.cs (09 pantalla 52; CU-B19). Las relaciones viven en pantalla hasta Aceptar.

/// Una relación de la pantalla: 30 líneas formato + rango y los límites «Líneas» y «Global».
public struct RelacionFormatos: Sendable, Hashable {
    public static let filas = 30
    public var formatos = Array(repeating: "", count: filas)
    public var rangos = Array(repeating: "", count: filas)
    public var lineas = ""
    public var global = ""

    public init() {}
}

extension CasosCondiciones {
    /// `ConstruirFormatos` (:172-210): líneas con formato válido (alfabeto `1 X 2 V *`, ≤ N) y rango no vacío; límites no
    /// numéricos ⇒ `""`; si la **última** relación queda sin líneas se quita.
    public static func construirFormatosSignos(_ relaciones: [RelacionFormatos], numeroPartidos n: Int) throws(ErrorDominio)
        -> [FormatosSignos]
    {
        var r: [FormatosSignos] = []
        for rel in relaciones {
            var fs = FormatosSignos()
            for (formato, rango) in zip(rel.formatos, rel.rangos) {
                let f = FormatoNET.mayusculas(FormatoNET.recortar(formato))
                let g = FormatoNET.recortar(rango)
                guard f.utf16.count <= n, f.allSatisfy({ "1X2V*".contains($0) }), !f.isEmpty, !g.isEmpty else { continue }
                var l = FormatoSignos()
                l.ponerFormato(f)
                try l.ponerRangoAparicion(g)
                fs.lineasFormatos.append(l)
            }
            let lin = FormatoNET.recortar(rel.lineas)
            try fs.ponerLineas(UtilidadesEntradasValores.sonTodosNumeros(lin) ? lin : "")
            let glo = FormatoNET.recortar(rel.global)
            try fs.ponerGlobal(UtilidadesEntradasValores.sonTodosNumeros(glo) ? glo : "")
            r.append(fs)
        }
        if let u = r.last, !u.lineasFormatos.contains(where: { !$0.formato.isEmpty }) { r.removeLast() }
        return r
    }

    /// `CargarDesdeGrupo` (:113-153): una relación por grupo (o una vacía).
    public static func relacionesFormatos(_ f: CondicionFormatosSignos) -> [RelacionFormatos] {
        let r = f.formatosSignos.map { fs in
            var rel = RelacionFormatos()
            for (i, l) in fs.lineasFormatos.enumerated() {
                if i >= rel.formatos.count {
                    rel.formatos.append("")
                    rel.rangos.append("")
                }
                (rel.formatos[i], rel.rangos[i]) = (l.formato, l.rangoAparicion)
            }
            (rel.lineas, rel.global) = (fs.lineas, fs.global)
            return rel
        }
        return r.isEmpty ? [RelacionFormatos()] : r
    }

    /// `Aceptar` (:213-238) y `ObtenerFiltroTemporal`: con relaciones se activa la primera vez; sin ellas, vacía e inactiva.
    public static func aplicarFormatosSignos(_ relaciones: [RelacionFormatos], en f: inout CondicionFormatosSignos, numeroPartidos n: Int)
        throws(ErrorDominio)
    {
        let lista = try construirFormatosSignos(relaciones, numeroPartidos: n)
        if !f.contieneDatos && !lista.isEmpty { (f.contieneDatos, f.activa) = (true, true) }
        if lista.isEmpty { (f.contieneDatos, f.activa) = (false, false) }
        f.ponerFormatosSignos(lista)
    }
}
