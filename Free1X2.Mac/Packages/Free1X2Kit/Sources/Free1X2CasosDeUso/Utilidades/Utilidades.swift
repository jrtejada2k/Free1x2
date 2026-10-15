import Foundation
import Free1X2Base

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
