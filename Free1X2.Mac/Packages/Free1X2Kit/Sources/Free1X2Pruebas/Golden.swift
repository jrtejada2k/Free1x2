import CryptoKit
import Foundation
import Free1X2Formatos

// Cargador del oráculo por lectura (ESPEC_12 §2-3). Los datos son recursos de este target (DT-28).

/// De dónde sale el esperado de un caso (ESPEC_12 §1).
public enum FuenteGolden: String, Sendable, Codable, CaseIterable {
    case test, fixture, lectura, modelo, bcl, propiedad, instantanea
}

/// Un caso golden: una línea de una suite JSONL.
public struct CasoGolden<Entrada: Decodable & Sendable, Salida: Decodable & Sendable>: Decodable, Sendable,
    CustomStringConvertible
{
    public let id: String
    public let fuente: FuenteGolden
    public let componente: String
    public let entrada: Entrada
    public let salida: Salida?
    /// Nombre .NET de la excepción esperada (`ExcepcionNET.rawValue`).
    public let error: String?
    /// Obligatoria si `fuente == .test`: `fichero:línea` del test C# leído.
    public let cita: String?
    /// Obligatoria si `fuente == .lectura`: pasos con `fichero:línea`.
    public let traza: [String]?

    public var description: String { id }
}

/// `double`/`float` de un golden: se compara por bits, nunca por texto (ESPEC_12 §2).
public struct DoubleGolden: Decodable, Sendable, Equatable {
    public let bits: String
    public let texto: String

    public var valor: Double? {
        UInt64(bits.dropFirst(2), radix: 16).map(Double.init(bitPattern:))
    }
}

public enum Golden {
    public enum Fallo: Error, CustomStringConvertible {
        case recursoNoEncontrado(String)
        case lineaInvalida(suite: String, linea: Int, causa: any Error)

        public var description: String {
            switch self {
            case .recursoNoEncontrado(let ruta): "No existe el recurso golden «\(ruta)»."
            case .lineaInvalida(let suite, let linea, let causa): "\(suite):\(linea): \(causa)"
            }
        }
    }

    /// URL de un recurso: `"Golden/…"` o `"Fixtures/…"`.
    public static func url(_ rutaRelativa: String) throws -> URL {
        let url = (Bundle.module.resourceURL ?? Bundle.module.bundleURL).appending(path: rutaRelativa)
        guard FileManager.default.fileExists(atPath: url.path(percentEncoded: false)) else {
            throw Fallo.recursoNoEncontrado(rutaRelativa)
        }
        return url
    }

    public static func bytes(_ rutaRelativa: String) throws -> Data {
        try Data(contentsOf: url(rutaRelativa))
    }

    /// Columnas de un fixture leídas como `LeeColumnaSinComas` (lector real de Free1X2Formatos).
    public static func columnas(_ rutaRelativa: String) throws -> [String] {
        ArchivoColumnasTexto.columnas(try bytes(rutaRelativa))
    }

    /// SHA-256 en hexadecimal (minúsculas) de los bytes UTF-8 de `texto`: resume salidas largas en el golden.
    public static func sha256(_ texto: String) -> String {
        SHA256.hash(data: Data(texto.utf8)).map { String(format: "%02x", $0) }.joined()
    }

    /// Líneas no vacías de una suite (`"motor/flujos"` → `Golden/motor/flujos.jsonl`).
    public static func lineas(_ suite: String) throws -> [Data] {
        try bytes("Golden/\(suite).jsonl").split(separator: UInt8(ascii: "\n")).map { Data($0) }
    }

    /// Casos de una suite, opcionalmente solo los de un `componente` (las suites mezclan componentes con entradas distintas).
    public static func casos<E, S>(
        _ suite: String,
        componente: String? = nil,
        _: E.Type = E.self,
        _: S.Type = S.self
    ) throws -> [CasoGolden<E, S>] {
        let casos: [CasoGolden<E, S>] = try decodificar(suite) { componente == nil || $0.componente == componente }
        // Un componente mal escrito daría un bucle vacío y un test verde en falso.
        if casos.isEmpty { throw Fallo.recursoNoEncontrado("\(suite) → \(componente ?? "*")") }
        return casos
    }

    /// Un caso concreto por su `id`.
    public static func caso<E, S>(_ id: String, en suite: String, _: E.Type = E.self, _: S.Type = S.self) throws
        -> CasoGolden<E, S>
    {
        // Swift 6.2 no infiere E y S a través de `.first`: primero la lista con su tipo.
        let casos: [CasoGolden<E, S>] = try decodificar(suite) { $0.id == id }
        guard let caso = casos.first else {
            throw Fallo.recursoNoEncontrado("\(suite) → \(id)")
        }
        return caso
    }

    private struct Cabecera: Decodable {
        let id: String
        let componente: String
    }

    private static func decodificar<E, S>(_ suite: String, _ incluir: (Cabecera) -> Bool) throws -> [CasoGolden<E, S>] {
        let decodificador = JSONDecoder()
        return try lineas(suite).enumerated().compactMap { indice, linea in
            do {
                guard incluir(try decodificador.decode(Cabecera.self, from: linea)) else { return nil }
                return try decodificador.decode(CasoGolden<E, S>.self, from: linea)
            } catch {
                throw Fallo.lineaInvalida(suite: suite, linea: indice + 1, causa: error)
            }
        }
    }
}
