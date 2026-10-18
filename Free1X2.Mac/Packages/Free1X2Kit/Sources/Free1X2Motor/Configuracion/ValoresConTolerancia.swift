import Free1X2Base

// C#: el parser «valores con tolerancia» que repiten los `Set*(string)` de varios filtros (03 §0.4 P6; 02 §0.5).
// Ejemplo: FiltroNoVariantes.cs:231-255.

/// Conjunto de valores permitidos con su letra de tolerancia (`' '` = sin letra), indexado por valor.
public struct ValoresConTolerancia: Sendable, Hashable {
    public private(set) var permitidos: [Bool]
    public private(set) var letras: [UInt16]

    public static let sinLetra: UInt16 = 0x20

    /// `new bool[n]` y `new char[n]` rellenos con `false` y `' '` (o `'\0'` donde el C# no los inicializa).
    public init(tamano: Int, letraInicial: UInt16 = sinLetra) {
        permitidos = Array(repeating: false, count: tamano)
        letras = Array(repeating: letraInicial, count: tamano)
    }

    public var hayValores: Bool { permitidos.contains(true) }

    /// `SetX(int, bool)`: solo la bandera.
    public mutating func poner(_ valor: Int, _ activo: Bool, _ origen: @autoclosure () -> String) throws(ErrorDominio) {
        try permitidos.escribir(activo, en: valor, origen())
    }

    /// `SetX(string)` — `"6,7A,8"`: cada trozo es un entero con una letra final opcional. Acumula; un valor repetido pisa la
    /// letra. Trozo vacío ⇒ `IndexOutOfRange` (`str[-1]`); no numérico ⇒ `Format`; fuera de `0…tamaño−1` ⇒ `IndexOutOfRange`.
    /// Lo aplicado antes del trozo que falla se queda, como en C#.
    public mutating func poner(_ valores: String, _ origen: @autoclosure () -> String) throws(ErrorDominio) {
        for trozo in trozos(valores, ",") {
            let u = Array(trozo.utf16)
            guard let ultima = u.last else { throw .fueraDeRango(origen()) }
            let numero: Int
            let letra: UInt16
            if FormatoNET.esLetra(ultima) {
                numero = Int(try FormatoNET.convertToInt32(String(decoding: u.dropLast(), as: UTF16.self)))
                letra = ultima
            } else {
                numero = Int(try FormatoNET.convertToInt32(trozo))
                letra = Self.sinLetra
            }
            try permitidos.escribir(true, en: numero, origen())
            letras[numero] = letra
        }
    }

    /// `ReinicializaValores`.
    public mutating func reinicializar() {
        permitidos = Array(repeating: false, count: permitidos.count)
        letras = Array(repeating: Self.sinLetra, count: letras.count)
    }

    /// `GetX()`: valores marcados en orden ascendente con su letra (`"6,7A,8"`); `""` si ninguno.
    public var texto: String {
        permitidos.indices.filter { permitidos[$0] }.map { i in
            String(i) + (letras[i] == Self.sinLetra ? "" : String(decoding: [letras[i]], as: UTF16.self))
        }.joined(separator: ",")
    }

    /// `flag[n]` del análisis.
    @inline(__always)
    public func permite(_ n: Int, _ origen: @autoclosure () -> String) throws(ErrorDominio) -> Bool {
        try permitidos.leer(n, origen())
    }

    /// `tol[n] == letra` de `ObtenNoAciertosTolerancias`.
    @inline(__always)
    public func letra(_ n: Int, _ origen: @autoclosure () -> String) throws(ErrorDominio) -> UInt16 {
        try letras.leer(n, origen())
    }
}
