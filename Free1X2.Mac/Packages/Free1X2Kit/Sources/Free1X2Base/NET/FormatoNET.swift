import Foundation

// Parseo y formato con la semántica de .NET 8 (ESPEC_16 §1; trampas T4, T7, T13, T14). Fuente leída:
// dotnet/runtime release/8.0 — Number.Parsing.cs, Number.Formatting.cs, Convert.cs, Boolean.cs.
// Nada de esto está en el camino caliente: prima la exactitud.

public enum FormatoNET {
    // MARK: Enteros — Convert.ToInt32/ToInt16(string) = Parse con NumberStyles.Integer

    public static func convertToInt32(_ texto: String) throws(ErrorDominio) -> Int32 {
        Int32(try entero(texto, minimo: Int64(Int32.min), maximo: Int64(Int32.max), tipo: "Int32"))
    }

    public static func convertToInt16(_ texto: String) throws(ErrorDominio) -> Int16 {
        Int16(try entero(texto, minimo: Int64(Int16.min), maximo: Int64(Int16.max), tipo: "Int16"))
    }

    /// `[blancos][signo]dígitos[blancos][\0…]`; blancos = U+0009…U+000D y U+0020.
    private static func entero(_ texto: String, minimo: Int64, maximo: Int64, tipo: String) throws(ErrorDominio) -> Int64 {
        let u = Array(texto.utf16)
        var i = 0
        var fin = u.count
        while i < fin, esBlancoNumero(u[i]) { i += 1 }
        while fin > i, u[fin - 1] == 0 { fin -= 1 }
        while fin > i, esBlancoNumero(u[fin - 1]) { fin -= 1 }
        var negativo = false
        if i < fin, u[i] == 0x2D {
            negativo = true
            i += 1
        } else if i < fin, u[i] == 0x2B {
            i += 1
        }
        guard i < fin else { throw errorFormato() }
        var valor: Int64 = 0
        var desborda = false
        while i < fin {
            let c = u[i]
            guard (0x30...0x39).contains(c) else { throw errorFormato() }
            if !desborda {
                valor = valor * 10 + Int64(c - 0x30)
                desborda = valor > maximo + 1
            }
            i += 1
        }
        if negativo { valor = -valor }
        guard !desborda, (minimo...maximo).contains(valor) else {
            throw ErrorDominio(.desbordamiento, "El valor era demasiado grande o demasiado pequeño para un \(tipo).")
        }
        return valor
    }

    // MARK: Booleanos — Convert.ToBoolean(string) = Boolean.Parse

    public static func convertToBoolean(_ texto: String) throws(ErrorDominio) -> Bool {
        let limpio = recortar(texto).trimmingCharacters(in: CharacterSet(charactersIn: "\0"))
        switch limpio.lowercased() {
        case "true": return true
        case "false": return false
        default: throw ErrorDominio(.formato, "La cadena no se reconoció como valor Boolean válido.")
        }
    }

    // MARK: Conversiones double → int

    /// `(int)d` del JIT x64 de .NET 8 (`cvttsd2si`): trunca; NaN, ±∞ o fuera de `Int32` ⇒ `Int32.min` (T13).
    public static func castInt32(_ d: Double) -> Int32 {
        guard d.isFinite else { return .min }
        let truncado = d.rounded(.towardZero)
        guard truncado >= -2_147_483_648.0, truncado <= 2_147_483_647.0 else { return .min }
        return Int32(truncado)
    }

    /// `Convert.ToInt32(double)` (Convert.cs): redondeo a par; NaN o fuera de rango ⇒ `OverflowException`.
    public static func convertToInt32(_ valor: Double) throws(ErrorDominio) -> Int32 {
        if valor >= 0 {
            if valor < 2_147_483_647.5 {
                var resultado = Int32(valor)
                let diferencia = valor - Double(resultado)
                if diferencia > 0.5 || (diferencia == 0.5 && resultado & 1 != 0) { resultado += 1 }
                return resultado
            }
        } else if valor >= -2_147_483_648.5 {
            var resultado = Int32(valor)
            let diferencia = valor - Double(resultado)
            if diferencia < -0.5 || (diferencia == -0.5 && resultado & 1 != 0) { resultado -= 1 }
            return resultado
        }
        throw ErrorDominio(.desbordamiento, "El valor era demasiado grande o demasiado pequeño para un Int32.")
    }

    // MARK: Doubles — Convert.ToDouble(string) = Double.Parse con NumberStyles.Float | AllowThousands

    public static func convertToDouble(_ texto: String, cultura: Cultura) throws(ErrorDominio) -> Double {
        guard let valor = tryParseDouble(texto, cultura: cultura) else { throw errorFormato() }
        return valor
    }

    public static func parseFloat(_ texto: String, cultura: Cultura) throws(ErrorDominio) -> Float {
        Float(try convertToDouble(texto, cultura: cultura))
    }

    /// `[blancos][signo]dígitos[miles dígitos…][decimal dígitos][e[signo]dígitos][blancos]`. Los separadores de miles
    /// solo se admiten tras un dígito y antes del decimal, sin validar el tamaño de grupo (`"3.5"` es-ES ⇒ 35).
    public static func tryParseDouble(_ texto: String, cultura: Cultura) -> Double? {
        var resto = Substring(recortarNumero(texto))
        if let especial = valorEspecial(resto, cultura: cultura) { return especial }
        var negativo = false
        if resto.hasPrefix(cultura.signoNegativo) {
            negativo = true
            resto = resto.dropFirst(cultura.signoNegativo.count)
        } else if resto.hasPrefix(cultura.signoPositivo) {
            resto = resto.dropFirst(cultura.signoPositivo.count)
        }
        var entera = ""
        var fraccion = ""
        var hayDigitos = false
        while let c = resto.first {
            if c.isASCII, c.isNumber {
                entera.append(c)
                hayDigitos = true
                resto = resto.dropFirst()
            } else if hayDigitos, !cultura.separadorMiles.isEmpty, resto.hasPrefix(cultura.separadorMiles),
                !resto.hasPrefix(cultura.separadorDecimal)
            {
                resto = resto.dropFirst(cultura.separadorMiles.count)
            } else {
                break
            }
        }
        if resto.hasPrefix(cultura.separadorDecimal) {
            resto = resto.dropFirst(cultura.separadorDecimal.count)
            while let c = resto.first, c.isASCII, c.isNumber {
                fraccion.append(c)
                hayDigitos = true
                resto = resto.dropFirst()
            }
        }
        guard hayDigitos else { return nil }
        var exponente = ""
        if let e = resto.first, e == "e" || e == "E" {
            var tras = resto.dropFirst()
            var signo = ""
            if let s = tras.first, s == "+" || s == "-" {
                signo = String(s)
                tras = tras.dropFirst()
            }
            var digitos = ""
            while let c = tras.first, c.isASCII, c.isNumber {
                digitos.append(c)
                tras = tras.dropFirst()
            }
            if !digitos.isEmpty {
                exponente = "e" + signo + digitos
                resto = tras
            }
        }
        guard resto.isEmpty else { return nil }
        let normalizado = (entera.isEmpty ? "0" : entera) + (fraccion.isEmpty ? "" : "." + fraccion) + exponente
        guard let magnitud = Double(normalizado) else { return nil }
        return negativo ? -magnitud : magnitud
    }

    private static func valorEspecial(_ texto: Substring, cultura: Cultura) -> Double? {
        let t = texto.lowercased()
        if t == cultura.simboloNaN.lowercased() || t == "nan" { return .nan }
        if t == cultura.simboloInfinitoPositivo.lowercased() || t == "infinity" || t == "+infinity" { return .infinity }
        if t == cultura.simboloInfinitoNegativo.lowercased() || t == "-infinity" { return -.infinity }
        return nil
    }

    // MARK: Formato de doubles

    /// `double.ToString()` de .NET Core 3.0+: dígitos de ida y vuelta más cortos; notación fija si el exponente
    /// científico está en (−5, 15), si no `d,dddE+XX` (exponente con signo y al menos 2 cifras) (T14).
    public static func toString(_ d: Double, cultura: Cultura) -> String {
        if d.isNaN { return cultura.simboloNaN }
        if d.isInfinite { return d > 0 ? cultura.simboloInfinitoPositivo : cultura.simboloInfinitoNegativo }
        let signo = d.sign == .minus ? cultura.signoNegativo : ""
        if d == 0 { return signo + "0" }
        return signo + general(abs(d).description, cultura: cultura)
    }

    /// `float.ToString()`: igual, con los dígitos más cortos de `Float`.
    public static func toString(_ f: Float, cultura: Cultura) -> String {
        if f.isNaN || f.isInfinite || f == 0 { return toString(Double(f), cultura: cultura) }
        return (f.sign == .minus ? cultura.signoNegativo : "") + general(abs(f).description, cultura: cultura)
    }

    private static func general(_ descripcion: String, cultura: Cultura) -> String {
        let (digitos, exponente) = digitosMasCortos(descripcion)
        if exponente > -5 && exponente < 15 { return fija(digitos, exponente: exponente, cultura: cultura) }
        let mantisa = digitos.count > 1 ? String(digitos.prefix(1)) + cultura.separadorDecimal + digitos.dropFirst() : digitos
        let cifras = String(abs(exponente))
        return mantisa + "E" + (exponente < 0 ? "-" : "+") + (cifras.count < 2 ? "0" + cifras : cifras)
    }

    /// `"F<n>"`, `"{0:f<n>}"`, `"0.00"`: redondeo «mitad lejos de cero» sobre el valor binario **exacto** (T14).
    public static func toStringFijo(_ d: Double, decimales: Int, cultura: Cultura) -> String {
        if d.isNaN { return cultura.simboloNaN }
        if d.isInfinite { return d > 0 ? cultura.simboloInfinitoPositivo : cultura.simboloInfinitoNegativo }
        let (entera, fraccion) = redondeado(d, decimales: decimales)
        let signo = d.sign == .minus ? cultura.signoNegativo : ""
        return signo + entera + (decimales > 0 ? cultura.separadorDecimal + fraccion : "")
    }

    /// Partes entera y decimal de `|d|` redondeadas a `decimales` cifras, mitad lejos de cero, sin separadores.
    static func redondeado(_ d: Double, decimales: Int) -> (entera: String, fraccion: String) {
        // Darwin imprime la expansión decimal exacta con precisión suficiente (1100 > 1074 cifras del subnormal mínimo).
        let exacta = String(format: "%.1100f", abs(d))
        let partes = exacta.split(separator: ".", maxSplits: 1)
        var digitos = Array(partes[0].utf8).map { Int($0) - 48 }
        let corte = digitos.count + decimales
        digitos += Array((partes.count > 1 ? partes[1] : "").utf8).map { Int($0) - 48 }
        let redondear = corte < digitos.count && digitos[corte] >= 5
        digitos = Array(digitos.prefix(corte))
        if redondear {
            var i = digitos.count - 1
            while i >= 0 {
                if digitos[i] == 9 {
                    digitos[i] = 0
                    i -= 1
                } else {
                    digitos[i] += 1
                    break
                }
            }
            if i < 0 { digitos.insert(1, at: 0) }
        }
        let texto = digitos.map(String.init).joined()
        let nEntera = texto.count - decimales
        var entera = String(texto.prefix(nEntera))
        while entera.count > 1, entera.hasPrefix("0") { entera.removeFirst() }
        return (entera.isEmpty ? "0" : entera, String(texto.suffix(decimales)))
    }

    /// Dígitos significativos (sin ceros a los lados) y exponente científico a partir de `description` de Swift,
    /// que ya da la representación de ida y vuelta más corta.
    private static func digitosMasCortos(_ descripcion: String) -> (String, Int) {
        let partes = descripcion.lowercased().split(separator: "e", maxSplits: 1)
        let exponenteTexto = partes.count > 1 ? Int(partes[1]) ?? 0 : 0
        let mantisa = partes[0]
        let puntoEn = mantisa.firstIndex(of: ".").map { mantisa.distance(from: mantisa.startIndex, to: $0) } ?? mantisa.count
        var digitos = Array(mantisa.filter { $0 != "." })
        var exponente = puntoEn - 1 + exponenteTexto
        while digitos.first == "0", digitos.count > 1 {
            digitos.removeFirst()
            exponente -= 1
        }
        while digitos.last == "0", digitos.count > 1 { digitos.removeLast() }
        return (String(digitos), exponente)
    }

    private static func fija(_ digitos: String, exponente: Int, cultura: Cultura) -> String {
        if exponente < 0 {
            return "0" + cultura.separadorDecimal + String(repeating: "0", count: -exponente - 1) + digitos
        }
        let enteros = exponente + 1
        if digitos.count <= enteros { return digitos + String(repeating: "0", count: enteros - digitos.count) }
        return String(digitos.prefix(enteros)) + cultura.separadorDecimal + digitos.dropFirst(enteros)
    }

    // MARK: Texto

    /// `string.ToUpper()`: mapeo simple carácter a carácter (sin expansiones: `ß` se queda) (04 S7; 06 A5).
    public static func mayusculas(_ texto: String) -> String {
        var salida = String.UnicodeScalarView()
        for escalar in texto.unicodeScalars {
            let mapeo = escalar.properties.uppercaseMapping.unicodeScalars
            if mapeo.count == 1, let unico = mapeo.first { salida.append(unico) } else { salida.append(escalar) }
        }
        return String(salida)
    }

    /// `string.Trim()`: quita `Char.IsWhiteSpace` de los dos extremos.
    public static func recortar(_ texto: String) -> String {
        let escalares = texto.unicodeScalars
        guard let inicio = escalares.firstIndex(where: { !$0.properties.isWhitespace }),
            let fin = escalares.lastIndex(where: { !$0.properties.isWhitespace })
        else { return "" }
        return String(escalares[inicio...fin])
    }

    /// `Char.IsLetter` sobre una unidad UTF-16: categorías Lu, Ll, Lt, Lm, Lo.
    public static func esLetra(_ unidad: UInt16) -> Bool {
        guard let escalar = Unicode.Scalar(unidad) else { return false }
        switch escalar.properties.generalCategory {
        case .uppercaseLetter, .lowercaseLetter, .titlecaseLetter, .modifierLetter, .otherLetter: return true
        default: return false
        }
    }

    // MARK: Auxiliares

    private static func esBlancoNumero(_ c: UInt16) -> Bool { c == 0x20 || (0x09...0x0D).contains(c) }

    private static func recortarNumero(_ texto: String) -> String {
        let u = Array(texto.utf16)
        var i = 0
        var fin = u.count
        while i < fin, esBlancoNumero(u[i]) { i += 1 }
        while fin > i, u[fin - 1] == 0 { fin -= 1 }
        while fin > i, esBlancoNumero(u[fin - 1]) { fin -= 1 }
        return String(decoding: u[i..<fin], as: UTF16.self)
    }

    private static func errorFormato() -> ErrorDominio {
        ErrorDominio(.formato, "La cadena de entrada no tiene el formato correcto.")
    }
}
