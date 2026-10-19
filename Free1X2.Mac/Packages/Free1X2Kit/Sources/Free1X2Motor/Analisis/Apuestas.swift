// C#: Free1X2.Domain/Analisis/ApuestaProbable.cs, ApuestaProbableCentral.cs y ApuestaProbEscrutada.cs (06 §3.7).
// Registros de las matrices de 3^14 de Rentabilidad, Banco de pruebas y Tramificar; `columna` es el índice base 3.

import Free1X2Base

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
/// ascendente por `clave`, pivote central y las guardas `j <= der` / `j > izq` tal cual; no estable (T5), así que no vale
/// otra ordenación. #13: tras cada partición las dos mitades no se tocan y se ordenan a la vez (6 niveles, en un hilo por
/// debajo de 65 536), lo que deja el mismo orden que una tras otra.
public enum OrdenacionPropia {
    @inlinable public static func ordenar<T: Sendable, K: Comparable & Sendable>(_ a: inout [T], clave: (T) -> K) {
        guard a.count > 1 else { return }
        withoutActuallyEscaping(clave) { clave in
            a.withUnsafeMutableBufferPointer { ordena($0, 0, $0.count - 1, clave, 6) }
        }
    }

    /// Lo que necesita cada mitad en su hilo. Las dos tocan zonas distintas del buffer.
    @usableFromInline struct Mitades<T, K>: @unchecked Sendable {
        @usableFromInline let b: UnsafeMutableBufferPointer<T>
        @usableFromInline let clave: (T) -> K
        @inlinable init(_ b: UnsafeMutableBufferPointer<T>, _ clave: @escaping (T) -> K) { (self.b, self.clave) = (b, clave) }
    }

    @inlinable static func ordena<T: Sendable, K: Comparable & Sendable>(
        _ b: UnsafeMutableBufferPointer<T>, _ izq: Int, _ der: Int, _ clave: @escaping (T) -> K, _ niveles: Int
    ) {
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
        if niveles > 0 && der - izq >= 65_536 {
            let (m, i0, j0) = (Mitades(b, clave), i, j)
            _ = enParalelo(2) { k in
                if k == 0 {
                    if izq < j0 { ordena(m.b, izq, j0, m.clave, niveles - 1) }
                } else if i0 < der {
                    ordena(m.b, i0, der, m.clave, niveles - 1)
                }
            }
        } else {
            if izq < j { ordena(b, izq, j, clave, 0) }
            if i < der { ordena(b, i, der, clave, 0) }
        }
    }
}
