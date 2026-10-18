import Foundation
import Free1X2Base

// C#: Free1X2.Domain/EntradaSalida/ArchivoColumnasTexto.cs (07 §6.5; F04). Sin estado: lee y escribe valores.

/// Ficheros de columnas `.txt`: una columna por línea.
public enum ArchivoColumnasTexto {
    /// `LeeColumnaSinComas` de cada línea (`ReadLine().Trim().ToUpper()`); las líneas vacías cuentan (:221-224).
    /// En varios núcleos, cada línea en su sitio (DM-22).
    public static func columnas(_ datos: Data) -> [String] {
        mapaParalelo(TextoNET.lineas(datos)) { FormatoNET.mayusculas(FormatoNET.recortar($0)) }
    }

    /// `LeerTodasCols(true)`: con comas entre signos (`DarFormatoColumna`).
    public static func columnasConFormato(_ datos: Data) -> [String] {
        columnas(datos).map(darFormatoColumna)
    }

    /// `DarFormatoColumna` (:314-334): intercala comas; una línea que **ya** tiene comas devuelve `""` [PRESERVAR].
    public static func darFormatoColumna(_ linea: String) -> String {
        guard !linea.contains(",") else { return "" }
        return linea.map(String.init).joined(separator: ",")
    }

    /// `ObtenNumCols` (:335-356): tamaño del fichero ÷ (longitud de la 1.ª línea recortada + 2). Supone CRLF:
    /// con LF cuenta de menos (`cols_a.txt` ⇒ 9 de 10) [PRESERVAR].
    public static func numColumnas(_ datos: Data) -> Int {
        guard let primera = columnas(datos).first else { return 0 }
        return datos.count / (primera.utf16.count + 2)
    }

    /// `ObtenNumSignos` (:357-375): longitud de la primera línea recortada.
    public static func numSignos(_ datos: Data) -> Int {
        columnas(datos).first?.utf16.count ?? 0
    }

    /// `GuardarCols` en bucle (:62-71): quita las comas; `\r\n`; UTF-8 sin BOM. Sin columnas ⇒ `nil`: el C# solo
    /// crea el fichero al escribir la primera.
    public static func datos(_ columnas: [String]) -> Data? {
        columnas.isEmpty ? nil : TextoNET.codificarLineas(columnas.map { $0.replacingOccurrences(of: ",", with: "") })
    }

    /// `GuardarColsComa` en bucle (:84-93): la columna tal cual.
    public static func datosConComa(_ columnas: [String]) -> Data? {
        columnas.isEmpty ? nil : TextoNET.codificarLineas(columnas)
    }
}
