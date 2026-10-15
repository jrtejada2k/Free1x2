import Free1X2Base

// C#: Free1X2.Domain/Escrutinio/PosiblesPremiosContenedor.cs y PosiblesPremiosComparer.cs (ESPEC_05 §4.4).

/// Columnas que logran 10…16 aciertos con una ganadora hipotética. `CalcularValorTotal`/`ValorTotal` no tienen
/// llamadores en la app y no se portan (DP-08).
public struct PosiblesPremiosContenedor: Sendable, Hashable {
    public var colGanadora: String
    public var col16: [String] = []
    public var col15: [String] = []
    public var col14: [String] = []
    public var col13: [String] = []
    public var col12: [String] = []
    public var col11: [String] = []
    public var col10: [String] = []

    public init(colGanadora: String) {
        self.colGanadora = colGanadora
    }

    /// `PosiblesPremiosComparer.Compare` (:10-48): descendente por nº de columnas de 16, 15, …, 10; empate total = 0.
    public static func comparar(_ x: Self, _ y: Self) -> Int {
        for (a, b) in zip(x.cuentas, y.cuentas) where a != b { return b < a ? -1 : 1 }
        return 0
    }

    /// `resumen.Sort(new PosiblesPremiosComparer())`: el orden de los empates es el de .NET (DT-15, GE-09).
    public static func ordenar(_ lista: inout [Self]) {
        IntrosortNET.ordenar(&lista, por: comparar)
    }

    private var cuentas: [Int] { [col16.count, col15.count, col14.count, col13.count, col12.count, col11.count, col10.count] }
}
