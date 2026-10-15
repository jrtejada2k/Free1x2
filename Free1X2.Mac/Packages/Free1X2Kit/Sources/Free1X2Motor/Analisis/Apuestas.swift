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
