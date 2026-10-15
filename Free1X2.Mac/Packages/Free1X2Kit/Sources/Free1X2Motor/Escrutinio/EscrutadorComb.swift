import Free1X2Base

// C#: Free1X2.Domain/Escrutinio/EscrutadorComb.cs y ColumnasPremiadasComb.cs (ESPEC_05 §4.2, DEC-501, DEC-502).

/// Valor encajado del `Hashtable` de aciertos: la ganadora guarda un `byte`, los vecinos un `int` (DEC-501).
public enum EtiquetaAciertos: Sendable, Hashable {
    case byte(UInt8)
    case int(Int)

    public var valor: Int {
        switch self {
        case .byte(let b): Int(b)
        case .int(let n): n
        }
    }
}

/// Escrutinio de combinaciones: conjunto de columnas candidatas a premio alrededor de una ganadora.
public struct EscrutadorComb: Sendable {
    public let numeroPartidos: Int
    /// `Premios`: `obtenerPosiblesPremios` lo deja ordenado de mayor a menor (el C# ordena el mismo array, §4.2.3).
    public private(set) var premios: [Int]
    /// `PremiosTotales[i]` = columnas con la categoría `premios[i]` (por posición, no por aciertos: ESC-06).
    public internal(set) var premiosTotales: [Int]
    /// Candidatas (clave → etiqueta) en un `Hashtable` de .NET: su orden de `Keys` es el del recorrido (DEC-501).
    public private(set) var aciertos = HashtableNET<EtiquetaAciertos>()

    /// 3^0 … 3^13: solo 14 partidos (§4.2.6).
    private static let pot: [Int64] = (0..<14).map { Int64(ConvertidorDeBases.potenciasDe3[$0]) }

    /// `new EscrutadorComb(int[] categorias)`.
    public init(premios: [Int], numeroPartidos: Int) {
        self.premios = premios
        self.numeroPartidos = numeroPartidos
        premiosTotales = Array(repeating: 0, count: premios.count)
    }

    /// `ObtenerPosiblesPremios(columna, categorias)` (:232-247), con `categorias` = `Premios` como hace la app.
    public mutating func obtenerPosiblesPremios(_ columna: String) throws(ErrorDominio) {
        aciertos = HashtableNET()
        IntrosortNET.ordenar(&premios)
        premios.reverse()
        guard let primera = premios.first, let ultima = premios.last else {
            throw ErrorDominio(.indiceFueraDeRango, "Índice fuera de los límites de la matriz.", origen: "EscrutadorComb.cs:240")
        }
        var profundidad = UInt8(truncatingIfNeeded: primera)
        let col = Int64(try ConvertidorDeBases(numeroPartidos: numeroPartidos).numero(columna))
        if primera == numeroPartidos {
            try aciertos.asignar(.byte(UInt8(truncatingIfNeeded: primera)), a: .int64(col))
            profundidad &-= 1
        }
        try vecinos(col, 0, UInt8(truncatingIfNeeded: ultima), Int(profundidad))
    }

    /// `columnasCon1SignoDiferente` (:209-231): vecinos a un signo, recursivo por partidos crecientes (sin repetidos).
    private mutating func vecinos(_ inicial: Int64, _ posicion: Int, _ ultimo: UInt8, _ actual: Int) throws(ErrorDominio) {
        guard posicion < numeroPartidos else { return }
        for partido in posicion..<numeroPartidos {
            guard partido < Self.pot.count else {
                throw ErrorDominio(.indiceFueraDeRango, "Índice fuera de los límites de la matriz.", origen: "EscrutadorComb.cs:213")
            }
            let signo = (inicial / Self.pot[partido]) % 3
            for z in Int64(0)..<3 where z != signo {
                let indice = Int32(truncatingIfNeeded: inicial + Self.pot[partido] * (z - signo))
                if let previa = aciertos[.int32(indice)] {
                    guard case .int(let n) = previa else {
                        throw ErrorDominio(
                            .conversionInvalida, "Unable to cast object of type 'System.Byte' to type 'System.Int32'.",
                            origen: "EscrutadorComb.cs:222")
                    }
                    if n < actual { try aciertos.asignar(.int(actual), a: .int32(indice)) }
                } else {
                    try aciertos.anadir(.int32(indice), .int(actual))
                }
                if actual > Int(ultimo) { try vecinos(Int64(indice), partido + 1, ultimo, actual - 1) }
            }
        }
    }
}
