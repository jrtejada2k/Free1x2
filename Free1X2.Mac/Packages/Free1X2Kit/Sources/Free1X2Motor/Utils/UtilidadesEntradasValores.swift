import Free1X2Base

// C#: Free1X2.Domain/Utils/UtilidadesEntradasValores.cs (06 §2.6). Sin consumidores, no se portan:
// ValidarCamposDeTexto, ObtenerRangosSeparadosPorGuion (salvo como paso interno) y PonerEnMayuscula.

/// Conversiones texto ↔ listas, `[Bool]` y figuras de los campos de condiciones.
public enum UtilidadesEntradasValores {
    /// Rangos de más de un millón de valores: en C# el bucle no termina (06 §2.6-4).
    static let maximoRango = 1_000_000

    /// `ObtenerArrayAciertos` (:26-36).
    public static func obtenerArrayAciertos(_ aciertos: String) throws(ErrorDominio) -> [Int] {
        var salida: [Int] = []
        for parte in trozos(aciertos, ",") { salida.append(Int(try FormatoNET.convertToInt32(parte))) }
        return salida
    }

    /// `ObtenerListaFromTxt` (:42-108): índices 0-based **con el fallo de deduplicado del C#** [PRESERVAR, G-D2]:
    /// compara el valor sin restar 1 contra índices ya restados. Cualquier error ⇒ lista vacía.
    public static func obtenerListaFromTxt(_ entrada: String) -> [Int] {
        lista(entrada, desplazamiento: 1)
    }

    /// `ObtenerListaFromTxtAciertos` (:115-181): valores tal cual, deduplicados. Cualquier error ⇒ lista vacía.
    public static func obtenerListaFromTxtAciertos(_ entrada: String) -> [Int] {
        lista(entrada, desplazamiento: 0)
    }

    private static func lista(_ entrada: String, desplazamiento: Int) -> [Int] {
        var indices: [Int] = []
        do {
            let partes = entrada.contains(",") ? trozos(entrada, ",") : [entrada]
            for parte in partes {
                if parte.contains("-") {
                    let p = trozos(parte, "-")
                    let minimo = Int(try FormatoNET.convertToInt32(p[0]))
                    let maximo = Int(try FormatoNET.convertToInt32(p[1]))
                    guard maximo - minimo < maximoRango else { throw ErrorDominio(.operacionInvalida) }
                    if maximo >= minimo {
                        for j in minimo...maximo where !indices.contains(j) { indices.append(j - desplazamiento) }
                    }
                } else {
                    let valor = Int(try FormatoNET.convertToInt32(parte))
                    if !indices.contains(valor) { indices.append(valor - desplazamiento) }
                }
            }
        } catch {
            return []
        }
        return indices
    }

    /// `ObtenerTextoFromListAciertos` (:183-195).
    public static func obtenerTextoFromListAciertos(_ lista: [Int]) -> String {
        lista.map(String.init).joined(separator: ",")
    }

    /// `ObtenerTextoFromBool` (:197-214): índices a `true`, separados por comas.
    public static func obtenerTextoFromBool(_ lista: [Bool]) -> String {
        lista.indices.filter { lista[$0] }.map(String.init).joined(separator: ",")
    }

    /// `ObtenerLongFiguraFromText` (:220-231): `"x-y-z"` ⇒ un nibble por valor (`(uint)` sin signo).
    public static func obtenerLongFiguraFromText(_ texto: String) throws(ErrorDominio) -> Int64 {
        var figura: Int64 = 0
        for valor in trozos(texto, "-") {
            figura = (figura &<< 4) | Int64(UInt32(bitPattern: try FormatoNET.convertToInt32(valor)))
        }
        return figura
    }

    /// `ObtenerTextoFiguraFromLong` (:238-253): los nibbles 0 de la izquierda se pierden; 0 ⇒ `"0"`.
    public static func obtenerTextoFiguraFromLong(_ figura: Int64) throws(ErrorDominio) -> String {
        guard figura >= 0 else { throw ErrorDominio(.operacionInvalida, "", origen: "UtilidadesEntradasValores.cs:241") }
        var f = figura
        var partes: [String] = []
        while f != 0 {
            partes.insert(String(f & 15), at: 0)
            f >>= 4
        }
        return partes.isEmpty ? "0" : partes.joined(separator: "-")
    }

    /// `ObtenerBoolArrayFromTxt(txt, longitud)` (:255-281): `""` ⇒ `nil`; texto inválido (lista vacía) ⇒ lanza.
    public static func obtenerBoolArrayFromTxt(_ texto: String, longitud: Int) throws(ErrorDominio) -> [Bool]? {
        guard !texto.isEmpty else { return nil }
        let lista = obtenerListaFromTxtAciertos(texto).sorted()
        guard let mayor = lista.last else {
            throw ErrorDominio(.argumentoFueraDeRango, "El índice estaba fuera del intervalo.", origen: "UtilidadesEntradasValores.cs:263")
        }
        let tamano = longitud > mayor ? longitud + 1 : mayor + 1
        guard tamano <= RangosHelper.maximoValor else {
            throw ErrorDominio(.argumentoFueraDeRango, "Valor demasiado grande: \(mayor).", origen: "UtilidadesEntradasValores.cs:269")
        }
        let conjunto = Set(lista)
        return (0..<max(tamano, 0)).map { conjunto.contains($0) }
    }

    /// `ObtenerBoolArrayFromTxt(txt)` (:282-302): tamaño `numeroPartidos + 1`; `""` ⇒ `nil`.
    public static func obtenerBoolArrayFromTxt(_ texto: String, numeroPartidos: Int) -> [Bool]? {
        guard !texto.isEmpty else { return nil }
        let conjunto = Set(obtenerListaFromTxtAciertos(texto))
        return (0...numeroPartidos).map { conjunto.contains($0) }
    }

    /// `ObtenerTodosValores()` (:322-330): `"0,1,…,N"`.
    public static func obtenerTodosValores(numeroPartidos: Int) -> String {
        (0...max(numeroPartidos, 0)).map(String.init).joined(separator: ",")
    }

    /// `ObtenerTodosValores(min, max)` (:332-340): `min > max` ⇒ lanza (`Substring(0, −1)`).
    public static func obtenerTodosValores(_ minimo: Int, _ maximo: Int) throws(ErrorDominio) -> String {
        guard minimo <= maximo else {
            throw ErrorDominio(
                .argumentoFueraDeRango, "La longitud no puede ser menor que cero.", origen: "UtilidadesEntradasValores.cs:339")
        }
        guard maximo - minimo < maximoRango else { throw ErrorDominio(.operacionInvalida) }
        return (minimo...maximo).map(String.init).joined(separator: ",")
    }

    /// `ObtenerValoresSeparadosPorComas` (:341-361): expande los rangos; las partes sueltas se copian sin validar.
    public static func obtenerValoresSeparadosPorComas(_ cadena: String) throws(ErrorDominio) -> String {
        var salida: [String] = []
        for parte in trozos(cadena, ",") {
            if parte.contains("-") {
                let p = trozos(parte, "-")
                salida.append(
                    try obtenerTodosValores(
                        Int(try FormatoNET.convertToInt32(p[0])), Int(try FormatoNET.convertToInt32(p[1]))))
            } else {
                salida.append(parte)
            }
        }
        return salida.joined(separator: ",")
    }

    /// `SonTodosNumeros` (:362-382): `""` ⇒ `true`.
    public static func sonTodosNumeros(_ aciertos: String) -> Bool {
        guard !aciertos.isEmpty else { return true }
        do {
            for parte in trozos(try obtenerValoresSeparadosPorComas(aciertos), ",") {
                _ = try FormatoNET.convertToInt32(parte)
            }
            return true
        } catch {
            return false
        }
    }

    /// `ObtenerRangos` (:383-393): **ordena la lista del llamador** [PRESERVAR]; vacía ⇒ `[0, 0]`.
    public static func obtenerRangos(_ valores: inout [Int]) -> [Int] {
        guard !valores.isEmpty else { return [0, 0] }
        valores.sort()
        return [valores[0], valores[valores.count - 1]]
    }

    /// `ObtenerGruposDeValores` (:404-426): grupos `"i-j"` de `cantidad` partidos; con `pasoFijo`, disjuntos.
    public static func obtenerGruposDeValores(_ cantidad: Int, pasoFijo: Bool, numeroPartidos: Int) -> [String] {
        var grupos: [String] = []
        var i = 1
        while i <= numeroPartidos {
            var j = i + 1
            while j <= numeroPartidos {
                if j - i == cantidad - 1 {
                    grupos.append("\(i)-\(j)")
                    if pasoFijo { i = j }
                    break
                }
                j += 1
            }
            i += 1
        }
        return grupos
    }
}
