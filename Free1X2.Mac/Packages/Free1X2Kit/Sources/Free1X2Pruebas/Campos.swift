import Free1X2Base

/// Campos opcionales que usan las suites golden de valores simples. Decodifica cualquier caso de esas suites: cada test
/// lee los campos que le tocan (así una suite puede mezclar componentes con entradas distintas).
public struct Campos: Decodable, Sendable {
    public var a: String?
    public var b: String?
    public var columna: String?
    public var columnas: String?
    public var cultura: String?
    public var decimales: Int?
    public var entero: Int?
    public var fichero: String?
    public var faltan: Int?
    public var ganadora: String?
    public var grupos: [String]?
    public var cantidad: Int?
    public var pasoFijo: Bool?
    public var indicesVerdaderos: [Int]?
    public var longitud: Int?
    public var marcados: [Bool]?
    public var maximo: Int?
    public var minimo: Int?
    public var nulo: Bool?
    public var numero: Int64?
    public var numeroPartidos: Int?
    public var ordenada: [Int]?
    public var partidos: CampoPartidos?
    public var pronosticos: [String]?
    public var rangos: [Int]?
    public var relleno: String?
    public var resultados: [Bool]?
    public var signos: [Int]?
    public var tamano: Int?
    public var tamanos: [Int]?
    public var texto: String?
    public var textos: [String]?
    public var valor: CampoValor?
    public var valores: CampoValores?
}

/// `partidos` es un entero (nº de partidos) o una lista (de enteros o de booleanos).
public enum CampoPartidos: Decodable, Sendable {
    case numero(Int)
    case enteros([Int])
    case booleanos([Bool])

    public init(from decoder: any Decoder) throws {
        let c = try decoder.singleValueContainer()
        if let n = try? c.decode(Int.self) {
            self = .numero(n)
        } else if let l = try? c.decode([Int].self) {
            self = .enteros(l)
        } else {
            self = .booleanos(try c.decode([Bool].self))
        }
    }

    public var numero: Int? { if case .numero(let n) = self { n } else { nil } }
    public var enteros: [Int]? { if case .enteros(let l) = self { l } else { nil } }
    public var booleanos: [Bool]? { if case .booleanos(let l) = self { l } else { nil } }
}

/// `valor` puede ser un entero, un `Int64` como texto, un booleano o un `double` por bits.
public enum CampoValor: Decodable, Sendable {
    case entero(Int64)
    case booleano(Bool)
    case doble(DoubleGolden)

    public init(from decoder: any Decoder) throws {
        let c = try decoder.singleValueContainer()
        if let b = try? c.decode(Bool.self) {
            self = .booleano(b)
        } else if let n = try? c.decode(Int64.self) {
            self = .entero(n)
        } else if let t = try? c.decode(String.self), let n = Int64(t) {
            self = .entero(n)
        } else {
            self = .doble(try c.decode(DoubleGolden.self))
        }
    }

    public var entero: Int64? { if case .entero(let n) = self { n } else { nil } }
    public var booleano: Bool? { if case .booleano(let b) = self { b } else { nil } }
    public var doble: Double? { if case .doble(let d) = self { d.valor } else { nil } }
}

/// `valores` es una lista de enteros o de booleanos.
public enum CampoValores: Decodable, Sendable {
    case enteros([Int])
    case booleanos([Bool])

    public init(from decoder: any Decoder) throws {
        let c = try decoder.singleValueContainer()
        if let l = try? c.decode([Int].self) { self = .enteros(l) } else { self = .booleanos(try c.decode([Bool].self)) }
    }

    public var enteros: [Int]? { if case .enteros(let l) = self { l } else { nil } }
    public var booleanos: [Bool]? { if case .booleanos(let l) = self { l } else { nil } }
}

/// Compara un resultado con la `salida` (vía `correcto`) o el `error` del caso. `nil` = coincide; si no, el motivo.
/// Uso: `#expect(verificar(caso, Result { … }) { $0 == … } == nil)`.
public func verificar<E, S, T>(
    _ caso: CasoGolden<E, S>, _ resultado: Result<T, ErrorDominio>, _ correcto: (T) -> Bool
) -> String? {
    switch resultado {
    case .success(let valor):
        if let error = caso.error { return "\(caso.id): esperaba \(error) y devolvió \(valor)" }
        return correcto(valor) ? nil : "\(caso.id): devolvió \(valor)"
    case .failure(let error):
        return caso.error == error.excepcion.rawValue ? nil : "\(caso.id): lanzó \(error.excepcion.rawValue)"
    }
}
