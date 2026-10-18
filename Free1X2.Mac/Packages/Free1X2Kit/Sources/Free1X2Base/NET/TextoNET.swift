import Foundation

// Lectura y escritura de texto como `StreamReader`/`StreamWriter` de .NET (ESPEC_16 §4; trampa T8; DP-02).

public enum TextoNET {
    /// Fin de línea al escribir: `Environment.NewLine` de Windows (DP-02).
    public static let finDeLinea = "\r\n"

    /// `new StreamReader(ruta)`: detecta el BOM (UTF-8, UTF-16 LE/BE); sin BOM, UTF-8. Lo inválido se sustituye por U+FFFD.
    public static func decodificar(_ datos: Data) -> String {
        let bytes = [UInt8](datos)
        if bytes.starts(with: [0xEF, 0xBB, 0xBF]) { return String(decoding: bytes.dropFirst(3), as: UTF8.self) }
        if bytes.starts(with: [0xFF, 0xFE]) { return utf16(bytes.dropFirst(2), bigEndian: false) }
        if bytes.starts(with: [0xFE, 0xFF]) { return utf16(bytes.dropFirst(2), bigEndian: true) }
        return String(decoding: bytes, as: UTF8.self)
    }

    /// Bytes en Latin-1 (ISO-8859-1): cada byte es el carácter U+00XX (`.dat`, T8).
    public static func decodificarLatin1(_ datos: Data) -> String {
        String(String.UnicodeScalarView(datos.compactMap { Unicode.Scalar(UInt32($0)) }))
    }

    /// Líneas como las devuelve `ReadLine()` en bucle: cortes `\r\n`, `\n` o `\r`; las vacías cuentan; el terminador
    /// final no genera una línea vacía de más.
    public static func lineas(_ texto: String) -> [String] {
        var lineas: [String] = []
        var actual: [UInt16] = []
        var anteriorCR = false
        var pendiente = false
        for u in texto.utf16 {
            if anteriorCR && u == 0x0A {
                anteriorCR = false
                continue
            }
            anteriorCR = false
            if u == 0x0D || u == 0x0A {
                lineas.append(String(decoding: actual, as: UTF16.self))
                actual.removeAll(keepingCapacity: true)
                anteriorCR = u == 0x0D
                pendiente = false
            } else {
                actual.append(u)
                pendiente = true
            }
        }
        if pendiente { lineas.append(String(decoding: actual, as: UTF16.self)) }
        return lineas
    }

    public static func lineas(_ datos: Data) -> [String] { lineas(decodificar(datos)) }

    /// `WriteLine` de cada línea con `\r\n`, en UTF-8 sin BOM (`File.CreateText`).
    public static func codificarLineas(_ lineas: [String]) -> Data {
        var datos = Data()
        datos.reserveCapacity(lineas.reduce(0) { $0 + $1.utf8.count + 2 })
        for linea in lineas {
            datos.append(contentsOf: linea.utf8)
            datos.append(contentsOf: [0x0D, 0x0A])
        }
        return datos
    }

    private static func utf16(_ bytes: ArraySlice<UInt8>, bigEndian: Bool) -> String {
        var unidades: [UInt16] = []
        unidades.reserveCapacity(bytes.count / 2)
        var i = bytes.startIndex
        while i + 1 < bytes.endIndex {
            let (a, b) = (UInt16(bytes[i]), UInt16(bytes[i + 1]))
            unidades.append(bigEndian ? a << 8 | b : b << 8 | a)
            i += 2
        }
        return String(decoding: unidades, as: UTF16.self)
    }
}
