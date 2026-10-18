// C#: Free1X2.Domain/Utils/Tramo.cs (06 §2.10); consumidor: TramificarFormViewModel.cs:582-680

/// Acumulador de un tramo del escrutinio por tramos: aciertos de 14 a 10, importes y balance (0,5 por columna).
/// Valor en vez de clase: el C# solo comparte por referencia `premios`, que nadie vuelve a leer tras calcular.
public struct Tramo: Sendable, Equatable {
    public let premios: [Double]
    public var numeroDeTramo = 0
    public var valorIzquierda = 0
    public var valorDerecha = 0
    public var numColumnasTramo = 0
    public var probAcumulada = 0.0
    public var columnasPremiadas = 0
    public private(set) var aciertos = [0, 0, 0, 0, 0]
    public private(set) var totalImportePremios = 0.0
    public private(set) var balance = 0.0

    public init(premios: [Double]) {
        self.premios = premios
    }

    public var p14: Int { aciertos[0] }
    public var p13: Int { aciertos[1] }
    public var p12: Int { aciertos[2] }
    public var p11: Int { aciertos[3] }
    public var p10: Int { aciertos[4] }

    /// `PonerAciertos`: [P14, P13, P12, P11, P10].
    public mutating func ponerAciertos(_ nuevos: [Int]) {
        aciertos = Array((nuevos + [0, 0, 0, 0, 0]).prefix(5))
    }

    /// `CalculaTotalImportePremios`: Σ aciertos·premio, en el mismo orden; balance = importe − columnas·precio. El original
    /// usa 0,50 fijo; desde DT-93 es el precio de Configuración ▸ Parámetros LAE.
    public mutating func calculaTotalImportePremios(precio: Double) {
        var acumulado = 0.0
        for i in 0..<5 { acumulado += Double(aciertos[i]) * premios[i] }
        totalImportePremios = acumulado
        balance = acumulado - Double(numColumnasTramo) * precio
    }

    /// `AddTramo`: suma aciertos, columnas premiadas, importe y balance de otro tramo.
    public mutating func addTramo(_ otro: Tramo) {
        for i in 0..<5 { aciertos[i] += otro.aciertos[i] }
        columnasPremiadas += otro.columnasPremiadas
        totalImportePremios += otro.totalImportePremios
        balance += otro.balance
    }
}
