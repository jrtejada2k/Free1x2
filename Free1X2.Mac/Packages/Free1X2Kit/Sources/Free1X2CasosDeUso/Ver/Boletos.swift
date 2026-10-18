import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/BoletoFrmViewModel.cs:91-310 (08 pantalla 11; CU-A25, CU-A26) y VerBoletosViewModel.cs (10 pantalla 99).

/// `OrdenarMatriz` (`Enums.cs:59-67`).
public enum OrdenBoletos: Sendable, Hashable {
    case signo, variantes, equis, doses, signosSeguidos, interrupciones
}

/// Columnas listas para trocear de 8 en 8.
public struct BoletosPreparados: Sendable, Hashable {
    /// `ObtenNumCols()` entradas (las que faltan, `""`), ya ordenadas.
    public let columnas: [String]
    public let total: Int
}

extension CasosVer {
    /// Métrica de `FiltroNoVariantes.AnalizaColumna(string)` / `FiltroInterrupciones.AnalizaColumna(string)`.
    static func metrica(_ c: String, _ orden: OrdenBoletos) -> Int {
        let u = Array(c.utf16)
        switch orden {
        case .variantes: return u.filter { $0 == 0x58 || $0 == 0x78 || $0 == 0x32 }.count
        case .equis: return u.filter { $0 == 0x58 || $0 == 0x78 }.count
        case .doses: return u.filter { $0 == 0x32 }.count
        case .interrupciones:
            let s = u.filter { $0 != 0x2A }
            return s.indices.dropFirst().filter { s[$0] != s[$0 - 1] }.count
        case .signo, .signosSeguidos: return 0
        }
    }

    /// CU-A26 `OrdenarMatrizColumnas`: «No ordenar» = orden de cadenas (ordinal; equivale a la cultura con `1 X 2`) y se
    /// invierte en descendente; el resto, cubos por métrica `0…longitud(primera)` en orden de fichero; lo que queda fuera
    /// de ese rango se pierde y la lista se completa con `""` (los `null` del C#).
    public static func ordenarColumnas(_ cols: [String], orden: OrdenBoletos, descendente: Bool) -> [String] {
        guard let primera = cols.first else { return cols }
        if orden == .signo {
            let a = cols.sorted { $0.utf16.lexicographicallyPrecedes($1.utf16) }
            return descendente ? a.reversed() : a
        }
        let m = cols.map { metrica($0, orden) }
        let l = primera.utf16.count
        var salida: [String] = []
        for v in descendente ? Array((0...l).reversed()) : Array(0...l) {
            for i in m.indices where m[i] == v { salida.append(cols[i]) }
        }
        return salida + Array(repeating: "", count: cols.count - salida.count)
    }

    /// CU-A25 `Cargar`: `ObtenNumCols()` columnas (`LeeColumnaSinComas`), ordenadas; boletos = ⌈n/8⌉, mínimo 1.
    public static func prepararBoletos(
        _ url: URL, orden: OrdenBoletos, descendente: Bool, entorno: Entorno
    ) throws(ErrorDominio) -> BoletosPreparados {
        let datos = try entorno.archivos.leer(url)
        let n = ArchivoColumnasTexto.numColumnas(datos)
        let lineas = ArchivoColumnasTexto.columnas(datos)
        let cols = (0..<n).map { $0 < lineas.count ? lineas[$0] : "" }
        let boletos = n % 8 == 0 ? n / 8 : n / 8 + 1
        return BoletosPreparados(columnas: ordenarColumnas(cols, orden: orden, descendente: descendente), total: max(boletos, 1))
    }

    /// `LlenarBoleto(b)`: el boleto acotado (0-based) con sus 8 columnas y números (0 = sin columna).
    public static func boleto(_ b: BoletosPreparados, _ indice: Int) -> (indice: Int, signos: [String], numeros: [Int]) {
        let k = min(max(indice, 0), b.total - 1)
        let signos = (0..<8).map { b.columnas[safe: k * 8 + $0] ?? "" }
        return (k, signos, signos.indices.map { signos[$0].isEmpty ? 0 : k * 8 + $0 + 1 })
    }
}

extension CasosVer {
    /// `CargarColumnas` (`VerBoletosEnEditorFrmViewModel.cs`, pantalla 100): boletos verticales en texto. El nº de
    /// boletos es `n/8 + 1` siempre que `n/8 < n` (sobra uno si n es múltiplo de 8) [PRESERVAR]; una columna más larga
    /// que la primera lanza como `colsTransformadas[i, j]`.
    public static func boletosEnTexto(_ columnas: [String], separador: [String]) throws(ErrorDominio) -> String {
        guard let primera = columnas.first else { return "" }
        let largo = primera.utf16.count
        var m: [[String]] = []
        for c in columnas {
            let u = Array(c.utf16)
            guard u.count <= largo else { throw .fueraDeRango("VerBoletosEnEditorFrmViewModel.cs:TransformarColumnas") }
            m.append((0..<largo).map { $0 < u.count ? String(decoding: [u[$0]], as: UTF16.self) : "" })
        }
        var boletos = columnas.count / 8
        if boletos < columnas.count { boletos += 1 }
        var t = ""
        for i in 0..<boletos {
            t += (i == 0 ? "" : "\n\n") + "Boleto \(i + 1)" + "\n__________________"
            for z in 0..<largo {
                t += "\n"
                if separador.count >= 3, separador.prefix(3).contains(String(z + 1)) { t += "\n" }
                for j in (i * 8)..<((i + 1) * 8) where j < columnas.count { t += m[j][z] + " " }
            }
        }
        return t
    }
}
