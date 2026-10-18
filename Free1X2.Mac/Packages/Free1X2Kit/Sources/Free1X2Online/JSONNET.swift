// `JsonDocument.Parse` de .NET 8 (07 §5.4, §5.6): RFC 8259 estricto (sin comentarios ni comas finales), profundidad ≤ 64,
// números guardados como texto (para `TryGetInt32`) y propiedades en orden; con nombres repetidos gana la última (DEC-704).
// `JSONSerialization` no distingue `38` de `38.0` ni fija qué duplicado gana.

/// Valor de un documento JSON.
public indirect enum ValorJSON: Sendable {
    case objeto([(clave: String, valor: ValorJSON)])
    case array([ValorJSON])
    case texto(String)
    /// El literal tal cual (`-12`, `3.5e2`).
    case numero(String)
    case booleano(Bool)
    case nulo

    /// `TryGetProperty`: la **última** definición del nombre.
    public subscript(_ nombre: String) -> ValorJSON? {
        guard case .objeto(let m) = self else { return nil }
        return m.last { $0.clave == nombre }?.valor
    }

    /// `TryGetInt32`: literal sin fracción ni exponente y dentro de `Int32`.
    public var entero32: Int32? {
        guard case .numero(let s) = self, !s.contains(where: { $0 == "." || $0 == "e" || $0 == "E" }) else { return nil }
        return Int32(s)
    }

    public var texto: String? {
        if case .texto(let s) = self { return s }
        return nil
    }
}

/// Error de `JsonDocument.Parse` (`JsonException`), con un texto al estilo de .NET.
public struct ErrorJSON: Error, Sendable, Equatable {
    public let mensaje: String
}

public enum JSONNET {
    public static func analizar(_ texto: String) throws(ErrorJSON) -> ValorJSON {
        var a = Analizador(Array(texto.utf8))
        a.espacios()
        let v = try a.valor(profundidad: 0)
        a.espacios()
        guard a.i == a.b.count else { throw a.error("'\(a.caracter)' is invalid after a single JSON value. Expected end of data.") }
        return v
    }

    private struct Analizador {
        let b: [UInt8]
        var i = 0

        init(_ b: [UInt8]) { self.b = b }

        var caracter: String { i < b.count ? String(decoding: [b[i]], as: UTF8.self) : "" }

        /// `LineNumber` y `BytePositionInLine` empiezan en 0, como en .NET.
        func error(_ motivo: String) -> ErrorJSON {
            var linea = 0
            var inicio = 0
            for k in 0..<min(i, b.count) where b[k] == 0x0A {
                linea += 1
                inicio = k + 1
            }
            return ErrorJSON(mensaje: "\(motivo) LineNumber: \(linea) | BytePositionInLine: \(i - inicio).")
        }

        mutating func espacios() {
            while i < b.count, [0x20, 0x09, 0x0A, 0x0D].contains(b[i]) { i += 1 }
        }

        mutating func valor(profundidad: Int) throws(ErrorJSON) -> ValorJSON {
            guard i < b.count else {
                throw error(
                    "The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, while isFinalBlock is true."
                )
            }
            switch b[i] {
            case 0x7B, 0x5B:
                guard profundidad < 64 else {
                    throw error("The maximum configured depth of 64 has been exceeded. Cannot read next JSON object.")
                }
                return b[i] == 0x7B ? try objeto(profundidad + 1) : try array(profundidad + 1)
            case 0x22: return .texto(try cadena())
            case 0x74: try literal("true"); return .booleano(true)
            case 0x66: try literal("false"); return .booleano(false)
            case 0x6E: try literal("null"); return .nulo
            case 0x2D, 0x30...0x39: return .numero(try numero())
            default: throw error("'\(caracter)' is an invalid start of a value.")
            }
        }

        mutating func literal(_ s: String) throws(ErrorJSON) {
            let u = Array(s.utf8)
            guard i + u.count <= b.count, Array(b[i..<i + u.count]) == u else {
                throw error("'\(caracter)' is an invalid JSON literal. Expected the literal '\(s)'.")
            }
            i += u.count
        }

        mutating func numero() throws(ErrorJSON) -> String {
            let inicio = i
            func digitos() -> Int {
                let d = i
                while i < b.count, (0x30...0x39).contains(b[i]) { i += 1 }
                return i - d
            }
            if b[i] == 0x2D { i += 1 }
            guard i < b.count, (0x30...0x39).contains(b[i]) else {
                throw error("'-' is an invalid end of a number. Expected a digit ('0'-'9').")
            }
            if b[i] == 0x30 {
                i += 1
            } else {
                _ = digitos()
            }
            if i < b.count, b[i] == 0x2E {
                i += 1
                guard digitos() > 0 else { throw error("'.' is an invalid end of a number. Expected a digit ('0'-'9').") }
            }
            if i < b.count, b[i] == 0x65 || b[i] == 0x45 {
                i += 1
                if i < b.count, b[i] == 0x2B || b[i] == 0x2D { i += 1 }
                guard digitos() > 0 else { throw error("'e' is an invalid end of a number. Expected a digit ('0'-'9').") }
            }
            if i < b.count, !([0x20, 0x09, 0x0A, 0x0D, 0x2C, 0x5D, 0x7D] as [UInt8]).contains(b[i]) {
                throw error(
                    "'\(caracter)' is invalid within a number, immediately after a sign character ('+' or '-'). Expected a digit ('0'-'9')."
                )
            }
            return String(decoding: b[inicio..<i], as: UTF8.self)
        }

        mutating func cadena() throws(ErrorJSON) -> String {
            i += 1
            var u: [UInt16] = []
            while i < b.count {
                let c = b[i]
                switch c {
                case 0x22:
                    i += 1
                    return String(decoding: u, as: UTF16.self)
                case 0x5C:
                    i += 1
                    guard i < b.count else { break }
                    switch b[i] {
                    case 0x22: u.append(0x22)
                    case 0x5C: u.append(0x5C)
                    case 0x2F: u.append(0x2F)
                    case 0x62: u.append(0x08)
                    case 0x66: u.append(0x0C)
                    case 0x6E: u.append(0x0A)
                    case 0x72: u.append(0x0D)
                    case 0x74: u.append(0x09)
                    case 0x75:
                        guard i + 4 < b.count, let v = UInt16(String(decoding: b[i + 1...i + 4], as: UTF8.self), radix: 16)
                        else { throw error("'\(caracter)' is not a valid hex digit following '\\u' within a JSON string.") }
                        u.append(v)
                        i += 4
                    default:
                        throw error(
                            "'\(caracter)' is an invalid escapable character within a JSON string. The string should be correctly escaped.")
                    }
                    i += 1
                case 0x00..<0x20:
                    throw error("'\(caracter)' is invalid within a JSON string. The string should be correctly escaped.")
                default:
                    // Secuencia UTF-8 completa (el texto ya es UTF-8 válido: viene de un `String`).
                    var n = 1
                    if c >= 0xF0 { n = 4 } else if c >= 0xE0 { n = 3 } else if c >= 0xC0 { n = 2 }
                    u.append(contentsOf: String(decoding: b[i..<min(i + n, b.count)], as: UTF8.self).utf16)
                    i += n
                }
            }
            throw error("Expected end of string, but instead reached end of data.")
        }

        mutating func objeto(_ profundidad: Int) throws(ErrorJSON) -> ValorJSON {
            i += 1
            var m: [(clave: String, valor: ValorJSON)] = []
            espacios()
            if i < b.count, b[i] == 0x7D {
                i += 1
                return .objeto(m)
            }
            while true {
                espacios()
                guard i < b.count, b[i] == 0x22 else {
                    throw error("'\(caracter)' is an invalid start of a property name. Expected a '\"'.")
                }
                let clave = try cadena()
                espacios()
                guard i < b.count, b[i] == 0x3A else { throw error("'\(caracter)' is invalid after a property name. Expected a ':'.") }
                i += 1
                espacios()
                m.append((clave, try valor(profundidad: profundidad)))
                espacios()
                guard i < b.count else {
                    throw error(
                        "Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed."
                    )
                }
                if b[i] == 0x2C {
                    i += 1
                    continue
                }
                guard b[i] == 0x7D else { throw error("'\(caracter)' is invalid after a value. Expected either ',', '}', or ']'.") }
                i += 1
                return .objeto(m)
            }
        }

        mutating func array(_ profundidad: Int) throws(ErrorJSON) -> ValorJSON {
            i += 1
            var a: [ValorJSON] = []
            espacios()
            if i < b.count, b[i] == 0x5D {
                i += 1
                return .array(a)
            }
            while true {
                espacios()
                a.append(try valor(profundidad: profundidad))
                espacios()
                guard i < b.count else {
                    throw error(
                        "Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed."
                    )
                }
                if b[i] == 0x2C {
                    i += 1
                    continue
                }
                guard b[i] == 0x5D else { throw error("'\(caracter)' is invalid after a value. Expected either ',', '}', or ']'.") }
                i += 1
                return .array(a)
            }
        }
    }
}
