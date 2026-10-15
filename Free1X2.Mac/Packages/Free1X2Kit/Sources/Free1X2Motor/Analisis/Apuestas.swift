// C#: Free1X2.Domain/Analisis/ApuestaProbable.cs, ApuestaProbableCentral.cs y ApuestaProbEscrutada.cs (06 §3.7).
// Registros de las matrices de 3^14 de Rentabilidad, Banco de pruebas y Tramificar; `columna` es el índice base 3.

public struct ApuestaProbable: Sendable, Hashable {
    public var probabilidad: Float
    public var columna: Int32

    public init(probabilidad: Float, columna: Int32) {
        self.probabilidad = probabilidad
        self.columna = columna
    }
}

public struct ApuestaProbableCentral: Sendable, Hashable {
    public var probabilidad: Float
    public var probabilidadDiferencial: Float
    public var columna: Int32

    public init(probabilidad: Float, probabilidadDiferencial: Float, columna: Int32) {
        self.probabilidad = probabilidad
        self.probabilidadDiferencial = probabilidadDiferencial
        self.columna = columna
    }
}

public struct ApuestaProbEscrutada: Sendable, Hashable {
    public var probabilidad: Double
    public var columna: Int32
    public var aciertos: UInt8

    public init(probabilidad: Double, columna: Int32, aciertos: UInt8) {
        self.probabilidad = probabilidad
        self.columna = columna
        self.aciertos = aciertos
    }
}

/// `ordena(izq, der)` de Rentabilidad, Ordenar por probabilidad, Banco de pruebas y Tramificar: quicksort propio
/// ascendente por `clave`, pivote central y las guardas `j <= der` / `j > izq` tal cual; no estable (T5).
public enum OrdenacionPropia {
    public static func ordenar<T>(_ a: inout [T], clave: (T) -> Float) {
        guard a.count > 1 else { return }
        a.withUnsafeMutableBufferPointer { ordena($0, 0, $0.count - 1, clave) }
    }

    static func ordena<T>(_ b: UnsafeMutableBufferPointer<T>, _ izq: Int, _ der: Int, _ clave: (T) -> Float) {
        var (i, j) = (izq, der)
        let x = clave(b[(izq + der) / 2])
        repeat {
            while clave(b[i]) < x && j <= der { i += 1 }
            while x < clave(b[j]) && j > izq { j -= 1 }
            if i <= j {
                b.swapAt(i, j)
                i += 1
                j -= 1
            }
        } while i <= j
        if izq < j { ordena(b, izq, j, clave) }
        if i < der { ordena(b, i, der, clave) }
    }
}
