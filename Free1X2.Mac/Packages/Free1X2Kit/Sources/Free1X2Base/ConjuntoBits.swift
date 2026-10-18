/// Conjunto de 3^N columnas como bitset contiguo (ESPEC_11 §6.4): sustituye a `BitArray`.
/// Un índice fuera de rango lanza `ArgumentOutOfRange`, como el indexador de `BitArray`.
public struct ConjuntoBits: Sendable, Equatable {
    public let cantidad: Int
    private var palabras: [UInt64]

    public init(cantidad: Int) {
        self.cantidad = cantidad
        palabras = [UInt64](repeating: 0, count: (cantidad + 63) / 64)
    }

    public func contiene(_ i: Int) throws(ErrorDominio) -> Bool {
        try comprobar(i)
        return palabras[i >> 6] & (1 << UInt64(i & 63)) != 0
    }

    public mutating func poner(_ i: Int, _ valor: Bool = true) throws(ErrorDominio) {
        try comprobar(i)
        if valor { palabras[i >> 6] |= 1 << UInt64(i & 63) } else { palabras[i >> 6] &= ~(1 << UInt64(i & 63)) }
    }

    /// Índices a 1 en orden creciente.
    public var indices: [Int] {
        var salida: [Int] = []
        for (n, palabra) in palabras.enumerated() where palabra != 0 {
            var p = palabra
            while p != 0 {
                salida.append(n << 6 | p.trailingZeroBitCount)
                p &= p - 1
            }
        }
        return salida
    }

    public var cuenta: Int { palabras.reduce(0) { $0 + $1.nonzeroBitCount } }

    private func comprobar(_ i: Int) throws(ErrorDominio) {
        guard i >= 0, i < cantidad else {
            throw ErrorDominio(.argumentoFueraDeRango, "El índice estaba fuera del intervalo.", origen: "BitArray")
        }
    }
}
