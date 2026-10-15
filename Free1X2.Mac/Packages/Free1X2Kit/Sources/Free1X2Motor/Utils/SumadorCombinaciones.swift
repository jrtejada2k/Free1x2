import Free1X2Base

// C#: Free1X2.Domain/Utils/SumadorCombinaciones.cs (06 §2.9); pantalla «Operaciones con combinaciones» (CU-A03)

/// Operación del álgebra de combinaciones (`AlgebraCombTipo`).
public enum AlgebraComb: String, Sendable, Codable, CaseIterable {
    case eliminaRepetidas, sumaEliminaRepetidas, sumaSoloComunes, restaSegunda
}

public struct ResultadoAlgebra: Sendable, Equatable {
    /// Columnas a grabar, en orden; vacío ⇒ el C# no llega a crear el fichero de salida.
    public var columnas: [String]
    public var columnasFinal: Int
    public var columnasRepetidas: Int
    /// `MensajeFinCalculo`, con `\n` literal como en el C#.
    public var mensaje: String
}

/// Unión, intersección, resta y deduplicado de ficheros de columnas, sobre un conjunto de 3^15 bits (:41).
/// Las columnas llegan ya leídas como `LeeColumnaSinComas` (recortadas y en mayúsculas).
public enum SumadorCombinaciones {
    static let tamano = 14_348_907

    public static func calcular(
        _ tipo: AlgebraComb, columnas1: [String], columnas2: [String] = [], numeroPartidos: Int
    ) throws(ErrorDominio) -> ResultadoAlgebra {
        let conversor = ConvertidorDeBases(numeroPartidos: numeroPartidos)
        var bits = ConjuntoBits(cantidad: tamano)
        var salida: [String] = []
        var final = 0
        var repetidas = 0

        switch tipo {
        case .eliminaRepetidas, .sumaEliminaRepetidas:
            for columna in columnas1 + (tipo == .sumaEliminaRepetidas ? columnas2 : []) {
                let n = Int(try conversor.numero(columna))
                if try bits.contiene(n) {
                    repetidas += 1
                } else {
                    try bits.poner(n)
                    salida.append(columna)
                    final += 1
                }
            }
        case .sumaSoloComunes:
            for columna in columnas1 { try bits.poner(Int(try conversor.numero(columna))) }
            for columna in columnas2 {
                guard try bits.contiene(Int(try conversor.numero(columna))) else { continue }
                salida.append(columna)
                final += 1
                repetidas += 1
            }
        case .restaSegunda:
            for columna in columnas1 {
                let n = Int(try conversor.numero(columna))
                if try bits.contiene(n) {
                    repetidas += 1
                } else {
                    try bits.poner(n)
                    final += 1
                }
            }
            for columna in columnas2 {
                let n = Int(try conversor.numero(columna))
                if try bits.contiene(n) {
                    try bits.poner(n, false)
                    final -= 1
                    repetidas += 1
                }
            }
            // Se graban en orden numérico y en forma canónica (`ConvNumAColumna`), no el texto leído (:116-123).
            for n in bits.indices { salida.append(try conversor.columna(Int64(n))) }
        }

        let mensaje: String
        switch tipo {
        case .eliminaRepetidas, .sumaEliminaRepetidas:
            mensaje = "Nº columnas final: \(final)\nNº col. repetidas: \(repetidas)"
        case .sumaSoloComunes, .restaSegunda:
            mensaje = "Nº columnas final: \(final)"
        }
        return ResultadoAlgebra(columnas: salida, columnasFinal: final, columnasRepetidas: repetidas, mensaje: mensaje)
    }
}
