import Foundation
import Free1X2Base

// C#: Free1X2.Domain/Utils/Porcentajes.cs (06 §2.7) — la parte numérica y el portapapeles. La lectura y escritura de
// los ficheros de valoraciones es de Free1X2Formatos.

/// Matriz `N×3` de valoraciones (1, X, 2 por partido).
public struct Porcentajes: Sendable, Hashable {
    public var valores: [[Double]]

    public init(valores: [[Double]]) { self.valores = valores }

    /// `ValoresBase100` (:329-341): la suma por partido pasa a `Float` antes de dividir en `Double` (T3).
    public func valoresBase100(numeroPartidos n: Int) throws(ErrorDominio) -> [[Float]] {
        var r: [[Float]] = []
        for i in 0..<n {
            let v = try valores.leer(i, "Porcentajes.cs:333")
            guard v.count >= 3 else { throw .fueraDeRango("Porcentajes.cs:333") }
            let factor = Float(v[0] + v[1] + v[2])
            r.append((0..<3).map { Float(v[$0] / Double(factor)) })
        }
        return r
    }

    /// `ValoresNeperianos` (:343-352): logaritmo natural de la base 100, a `Float`.
    public func valoresNeperianos(numeroPartidos n: Int) throws(ErrorDominio) -> [[Float]] {
        try valoresBase100(numeroPartidos: n).map { $0.map { Float(log(Double($0))) } }
    }

    /// `FormatearParaPortapapeles` (:505-523): tabuladores entre signos y `CR LF` entre partidos, sin salto final.
    public func paraPortapapeles(cultura: Cultura) -> String {
        valores.map { fila in fila.prefix(3).map { FormatoNET.toString($0, cultura: cultura) }.joined(separator: "\t") }
            .joined(separator: "\r\n")
    }
}
