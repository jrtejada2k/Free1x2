import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/GEPTFrmViewModel.cs (09 pantalla 56; CU-B23).

public enum CasosUtilidades {
    /// `Traductor(primera, segunda)`: pronóstico por los dos últimos enfrentamientos (G/E/P); otro ⇒ `" "`.
    public static func traducirGEPT(_ primera: String, _ segunda: String) -> String {
        switch (primera + segunda).uppercased() {
        case "GG", "PP": "12"
        case "GE", "EP": "1X"
        case "GP": "1"
        case "EG", "PE": "X2"
        case "EE": "X"
        case "PG": "2"
        default: " "
        }
    }
}

// C#: Views/Ported/TransposicionFrmViewModel.cs (10 §95; CU-C40).
extension CasosUtilidades {
    /// `orde[i] = (int)Pos_i − 1`; `nil` («Error en condiciones») si no es una permutación de 0…13.
    public static func ordenTransposicion(_ posiciones: [Double]) -> [Int]? {
        let orden = posiciones.map { Int(FormatoNET.castInt32($0)) - 1 }
        return orden.count == 14 && Set(orden) == Set(0..<14) ? orden : nil
    }

    /// `Transponer`: cada línea reordenada con `orden`, `\r\n`, UTF-8 sin BOM. Una línea de menos de 14 caracteres
    /// aborta dejando la salida parcial (el `StreamWriter` ya estaba abierto); el bucle `Peek() > 0` también se detiene
    /// en una línea que empieza por NUL.
    @concurrent public static func transponerColumnas(
        _ entrada: URL, salida: URL, orden: [Int], entorno: Entorno
    ) async throws(ErrorDominio) {
        let lineas = TextoNET.lineas(try entorno.archivos.leer(entrada))
        var datos = Data()
        var fallo: ErrorDominio?
        for l in lineas {
            let u = Array(l.utf16)
            if u.first == 0 { break }
            guard u.count >= 14 else {
                fallo = .fueraDeRango("TransposicionFrmViewModel.cs:166")
                break
            }
            datos.append(contentsOf: String(decoding: orden.map { u[$0] }, as: UTF16.self).utf8)
            datos.append(contentsOf: [0x0D, 0x0A])
        }
        try entorno.archivos.escribir(datos, en: salida, atomico: false)
        if let fallo { throw fallo }
    }
}

// C#: Views/Ported/AlgebraColumnasFrmViewModel.cs (08 pantalla 3; CU-A03).
extension CasosUtilidades {
    /// Al elegir un fichero: `ObtenNumSignos` (1.ª línea recortada) y `NumColumnas` (`ObtenNumCols`).
    public static func resumenColumnas(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> (signos: Int, columnas: Int) {
        let d = try entorno.archivos.leer(url)
        return (ArchivoColumnasTexto.numSignos(d), ArchivoColumnasTexto.numColumnas(d))
    }

    /// CU-A03 `Calcular`: `SumadorCombinaciones` (la combinación 2 solo si la operación la usa); sin columnas no se crea la
    /// salida. Devuelve `MensajeFinCalculo`.
    @concurrent public static func algebraDeColumnas(
        _ tipo: AlgebraComb, _ c1: URL, _ c2: URL?, salida: URL, numeroPartidos: Int, entorno: Entorno
    ) async throws(ErrorDominio) -> String {
        let columnas1 = ArchivoColumnasTexto.columnas(try entorno.archivos.leer(c1))
        var columnas2: [String] = []
        if tipo != .eliminaRepetidas, let c2 { columnas2 = ArchivoColumnasTexto.columnas(try entorno.archivos.leer(c2)) }
        let r = try SumadorCombinaciones.calcular(tipo, columnas1: columnas1, columnas2: columnas2, numeroPartidos: numeroPartidos)
        if let d = ArchivoColumnasTexto.datos(r.columnas) { try entorno.archivos.escribir(d, en: salida, atomico: false) }
        return r.mensaje
    }
}
