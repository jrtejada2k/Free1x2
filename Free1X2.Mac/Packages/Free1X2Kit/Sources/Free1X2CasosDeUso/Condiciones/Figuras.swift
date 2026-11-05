import Foundation
import Free1X2Base
import Free1X2Motor

// Ref.: 09 pantalla 49; CU-B13, CU-B14. DT-58: la pantalla edita una copia y la
// devuelve al aceptar (Windows compartía la lista por referencia).

extension CasosCondiciones {
    /// `Aceptar` (:126-150): casillas no vacías convertibles y sin repetir (las no convertibles, p. ej. `*`, se omiten).
    public static func aceptarFiguras(_ casillas: [String], condicion: String = "") -> [Int64] {
        var r: [Int64] = []
        for c in casillas {
            var t = FormatoNET.recortar(c)
            if condicion == "SignosSeguidos" { t = normalizarFiguraSeguidos(t) }
            guard !t.isEmpty, let f = try? UtilidadesEntradasValores.obtenerLongFiguraFromText(t), !r.contains(f) else { continue }
            r.append(f)
        }
        return r
    }

    /// SS-2 (M11, #28): en Signos seguidos la figura son los tramos de mayor a menor; vale sin guiones («32211», una cifra
    /// por tramo) y en cualquier orden («1-1-2-3» ⇒ «3-2-1-1»). Lo que no es una figura se deja como estaba.
    public static func normalizarFiguraSeguidos(_ texto: String) -> String {
        let partes = texto.contains("-") ? trozos(texto, "-").map(FormatoNET.recortar).filter { !$0.isEmpty } : texto.map(String.init)
        var numeros: [Int] = []
        for p in partes {
            guard let n = Int(p) else { return texto }
            numeros.append(n)
        }
        return numeros.sorted(by: >).map(String.init).joined(separator: "-")
    }

    /// `ObtenerTextoFiguraFromLong`.
    public static func textoFigura(_ f: Int64) -> String {
        (try? UtilidadesEntradasValores.obtenerTextoFiguraFromLong(f)) ?? ""
    }

    /// `ExtensionFiguras` (:154-163).
    public static func extensionFiguras(_ condicion: String) -> String {
        switch condicion {
        case "Contactos": ".figContactos"
        case "SignosSeguidos": ".figV1X2"
        default: ".fig"
        }
    }

    /// `Abrir` (:166-199): una figura por línea no en blanco.
    public static func leerFiguras(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> [Int64] {
        try CasosInicio.conPrefijo("No se pudo abrir el archivo de figuras: ") { () throws(ErrorDominio) in
            var r: [Int64] = []
            for l in TextoNET.lineas(try entorno.archivos.leer(url)) where !l.allSatisfy(\.isWhitespace) {
                r.append(try UtilidadesEntradasValores.obtenerLongFiguraFromText(FormatoNET.recortar(l)))
            }
            return r
        }
    }
}
