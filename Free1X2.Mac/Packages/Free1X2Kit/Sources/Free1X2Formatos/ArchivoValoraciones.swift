import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Free1X2.Domain/Utils/Porcentajes.cs:81-498 (06 §2.7; 07 F07). Los diálogos de error del C# (`UserDialogs.ShowError`)
// se devuelven como `aviso`; la lectura se abandona y los valores ya leídos se quedan.

/// Resultado de `new Porcentajes(…)` + `Leer()`.
public struct LecturaValoraciones: Sendable, Equatable {
    /// `valores[N,3]`: lo leído; el resto, a 0.
    public var valores: [[Double]]
    /// `FormatoFichero` de `TestFichero`: 1, 3, 42, 43, 44 o 2 (no reconocido).
    public var formato: Int16
    public var separador: Character
    /// Formatos 43/44: se encontró la descripción o la temporada y jornada buscadas.
    public var jornadaEncontrada = true
    /// Formato 43: primer campo de cada línea válida (`DescripcionValoracion`).
    public var descripciones: [String]?
    public var aviso: String?
}

public enum ArchivoValoraciones {
    public static let avisoCorrupto = "El fichero seleccionado no es de valoraciones o está corrupto"
    public static let avisoIncorrecto = "Fichero Incorrecto"
    public static let avisoNoHistorico = "El fichero no es un fichero de Valoraciones históricas"

    /// `Leer()` (:81-101). `temporada`/`jornada` (formato 44) y `descripcion` (43) son las propiedades fijadas antes.
    public static func leer(
        _ datos: Data, numeroPartidos n: Int, cultura: Cultura, temporada: String = "", jornada: String = "",
        descripcion: String = ""
    ) -> LecturaValoraciones {
        let lineas = TextoNET.lineas(datos)
        let (formato, sep) = testFichero(lineas.first.map(FormatoNET.recortar) ?? "", cultura)
        var r = LecturaValoraciones(
            valores: Array(repeating: [0, 0, 0], count: max(n, 0)), formato: formato, separador: sep)
        var valores = r.valores
        func numero(_ s: String) throws(ErrorDominio) -> Double {
            try FormatoNET.convertToDouble(
                FormatoNET.recortar(s.replacingOccurrences(of: ".", with: ",")), cultura: cultura)
        }
        /// `valores[fila, 0..2]` desde `partes[desde…]`; fuera de rango o mal formado lanza.
        func fila(_ fila: Int, _ partes: [String], _ desde: Int) throws(ErrorDominio) {
            for s in 0..<3 {
                let v = try numero(try partes.leer(desde + s, "Porcentajes.cs"))
                guard valores.indices.contains(fila) else { throw .fueraDeRango("Porcentajes.cs") }
                valores[fila][s] = v
            }
        }
        switch formato {
        case 1:
            // LeeUnValorPorFila (:102-119): 3·N líneas sin recortar; fin de fichero ⇒ NRE ⇒ aviso.
            do throws(ErrorDominio) {
                for i in 0..<n {
                    for s in 0..<3 {
                        let l = try lineas.leer(i * 3 + s, "Porcentajes.cs:109")
                        valores[i][s] = try FormatoNET.convertToDouble(l.replacingOccurrences(of: ".", with: ","), cultura: cultura)
                    }
                }
            } catch {
                r.aviso = avisoCorrupto
            }
        case 3:
            // LeeTresValoresPorFila (:121-148): con `i == N` el error se ignora y se sigue contando [PRESERVAR].
            var i = 0
            for l in lineas {
                let partes = trozos(FormatoNET.recortar(l), sep)
                guard partes.count == 3 else { continue }
                do throws(ErrorDominio) {
                    try fila(i, partes, 0)
                } catch {
                    if i != n {
                        r.aviso = avisoCorrupto
                        break
                    }
                }
                i += 1
            }
        case 42, 43, 44:
            // Lee42/43/44ValoresPorFila (:149-247): `indice` sigue entre líneas válidas (una 2.ª desborda).
            let desde = Int(formato) - 42
            var indice = 0
            var encontrado = false
            if formato == 43 { r.descripciones = [] }
            for l in lineas {
                let partes = trozos(FormatoNET.recortar(l), sep)
                guard partes.count == Int(formato) || partes.count == Int(formato) + 3 else { continue }
                if formato == 43 { r.descripciones?.append(partes[0]) }
                let elegida =
                    switch formato {
                    case 43: partes[0] == descripcion
                    case 44: partes[0] == temporada && partes[1] == jornada
                    default: true
                    }
                guard elegida else { continue }
                do throws(ErrorDominio) {
                    for i in stride(from: desde, to: desde + 42, by: 3) {
                        try fila(indice, partes, i)
                        indice += 1
                    }
                    encontrado = true
                } catch {
                    r.aviso = avisoCorrupto
                    break
                }
            }
            if formato != 42 { r.jornadaEncontrada = encontrado }
        default:
            r.aviso = avisoIncorrecto
        }
        r.valores = valores
        return r
    }

    /// `TestFichero` (:288-310): prueba `,`, espacio y TAB con la primera línea; un único número ⇒ 1.
    static func testFichero(_ linea: String, _ cultura: Cultura) -> (Int16, Character) {
        for sep: Character in [",", " ", "\t"] {
            let f = testSeparador(trozos(linea, sep), cultura)
            if f != 2 { return (f, sep) }
        }
        return (esNumero(linea, cultura) ? 1 : 2, "\t")
    }

    /// `TestSeparador` (:248-286): 3, 42, 43 o 44 si los campos (salvo los de prefijo) son números; si no, 2.
    static func testSeparador(_ aux: [String], _ cultura: Cultura) -> Int16 {
        func numeros(_ r: Range<Int>) -> Bool { r.allSatisfy { esNumero(aux[$0], cultura) } }
        switch aux.count {
        case 3: return numeros(0..<3) ? 3 : 2
        case 42, 45: return numeros(0..<42) ? 42 : 2
        case 43, 46: return numeros(1..<43) ? 43 : 2
        case 44, 47: return numeros(2..<44) ? 44 : 2
        default: return 2
        }
    }

    /// `EsNumero` (:324-328): `Convert.ToDouble` sin cambiar el punto.
    static func esNumero(_ s: String, _ cultura: Cultura) -> Bool {
        (try? FormatoNET.convertToDouble(s, cultura: cultura)) != nil
    }

    /// `Guardar` (:354-400) tras `GuardarValoraciones(nombre, formato, sep, …)`: 3 (punto decimal), 1 (`ToString()` tal
    /// cual) o 42 (una línea con separador final). Otro formato deja el fichero vacío (`File.CreateText` ya lo creó).
    public static func datos(
        _ valores: [[Double]], formato: Int16, separador: Character, numeroPartidos n: Int, cultura: Cultura
    ) throws(ErrorDominio) -> Data {
        func v(_ i: Int, _ j: Int) throws(ErrorDominio) -> String {
            let fila = try valores.leer(i, "Porcentajes.cs:369")
            return FormatoNET.toString(try fila.leer(j, "Porcentajes.cs:369"), cultura: cultura)
        }
        var lineas: [String] = []
        switch formato {
        case 3:
            for i in 0..<n {
                var l: [String] = []
                for j in 0..<3 { l.append(try v(i, j).replacingOccurrences(of: ",", with: ".")) }
                lineas.append(l.joined(separator: String(separador)))
            }
        case 1:
            for i in 0..<n { for j in 0..<3 { lineas.append(try v(i, j)) } }
        case 42:
            var l = ""
            for i in 0..<n { for j in 0..<3 { l += try v(i, j).replacingOccurrences(of: ",", with: ".") + String(separador) } }
            lineas.append(l)
        default: break
        }
        return TextoNET.codificarLineas(lineas)
    }

    /// `GuardarValoraciones(nombre, sep, valores, temporada, jornada)` (:422-481): histórico (formato 44). Con fichero,
    /// su separador detectado manda; la línea de esa temporada y jornada se sustituye o se añade al final, y se reescribe
    /// todo (líneas recortadas y en mayúsculas). Si el fichero no es de formato 44 devuelve el aviso y no escribe. Con la
    /// guarda `Length >= 2` de la réplica viva (`HistoriaValoracionesFrmViewModel.cs:226`; la de `Porcentajes` no se llama).
    public static func datosHistorico(
        existente: Data?, separador: Character, valores: [[Double]], temporada: String, jornada: String,
        numeroPartidos n: Int, cultura: Cultura
    ) throws(ErrorDominio) -> (datos: Data?, aviso: String?) {
        var sep = separador
        var formato: Int16 = 44
        if let existente {
            (formato, sep) = testFichero(TextoNET.lineas(existente).first.map(FormatoNET.recortar) ?? "", cultura)
        }
        guard formato == 44 else { return (nil, avisoNoHistorico) }
        var nueva = temporada + String(sep) + jornada
        for i in 0..<n {
            let fila = try valores.leer(i, "Porcentajes.cs:494")
            for x in 0..<3 {
                nueva +=
                    String(sep)
                    + FormatoNET.toString(try fila.leer(x, "Porcentajes.cs:494"), cultura: cultura)
                    .replacingOccurrences(of: ".", with: ",")
            }
        }
        var lineas: [String] = []
        var existe = false
        for l in existente.map(ArchivoColumnasTexto.columnas) ?? [] {
            let v = trozos(l, sep)
            if v.count >= 2, v[0] == temporada, v[1] == jornada {
                existe = true
                lineas.append(nueva)
            } else {
                lineas.append(l)
            }
        }
        if !existe { lineas.append(nueva) }
        return (ArchivoColumnasTexto.datosConComa(lineas), nil)
    }
}
