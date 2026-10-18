import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroDibujos.cs (03 §8).

/// Dibujos `"e+d"` (nº de X + nº de 2) permitidos.
public struct CondicionDibujos: Sendable, Hashable {
    public var activa = false
    public var contieneDatos = false
    /// `Dibujos`: añadir aquí directamente **no** reconstruye la tabla (el If-Then lo hace y rechaza siempre) [PRESERVAR].
    public var dibujos: [String] = []
    /// `bool[17,17]` aplanado: índice `e·17 + d`.
    public private(set) var tabla = Array(repeating: false, count: 17 * 17)

    public init() {}

    /// `Dibujos = lista` (:113-135): con lista no vacía marca `contieneDatos` y rehace la tabla; vacía no toca nada más.
    public mutating func ponerDibujos(_ lista: [String]) throws(ErrorDominio) {
        dibujos = lista
        guard !lista.isEmpty else { return }
        contieneDatos = true
        tabla = Array(repeating: false, count: 17 * 17)
        for texto in lista {
            let partes = trozos(texto, "+")
            let e = Int(try FormatoNET.convertToInt32(partes[0]))
            let d = Int(try FormatoNET.convertToInt32(try partes.leer(1, "FiltroDibujos.cs:130")))
            guard (0..<17).contains(e), (0..<17).contains(d) else { throw .fueraDeRango("FiltroDibujos.cs:130") }
            tabla[e * 17 + d] = true
        }
    }

    /// `GetDibujos()`.
    public var getDibujos: String { dibujos.joined(separator: ",") }
}
