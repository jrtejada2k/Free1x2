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

// C#: Views/Ported/ImportExportFrmViewModel.cs (09 pantalla 66; CU-B34).
extension CasosUtilidades {
    /// `AbrirEntrada`: CSV→TXT (`LeerTodasColsSeparadasPorComas`) o TXT→CSV (`LeerTodasCols(true)`) y la salida
    /// propuesta: el nombre en minúsculas con `.csv`↔`.txt`, en la misma carpeta (DT-62).
    public static func leerParaConvertir(_ url: URL, aTxt: Bool, entorno: Entorno) throws(ErrorDominio) -> (columnas: [String], salida: URL)
    {
        let d = try entorno.archivos.leer(url)
        let columnas =
            aTxt
            ? ArchivoColumnasTexto.columnas(d).map { $0.replacingOccurrences(of: ",", with: "") }
            : ArchivoColumnasTexto.columnasConFormato(d)
        let nombre = url.lastPathComponent.lowercased()
            .replacingOccurrences(of: aTxt ? ".csv" : ".txt", with: aTxt ? ".txt" : ".csv")
        return (columnas, url.deletingLastPathComponent().appending(path: nombre))
    }

    /// `Hacer`: `GuardarTodasCols(columnas, conComa: TXT→CSV)` y `ObtenNumCols` de la salida.
    public static func convertirColumnas(_ columnas: [String], aTxt: Bool, salida: URL, entorno: Entorno) throws(ErrorDominio) -> Int {
        guard let d = aTxt ? ArchivoColumnasTexto.datos(columnas) : ArchivoColumnasTexto.datosConComa(columnas) else { return 0 }
        try entorno.archivos.escribir(d, en: salida, atomico: false)
        return ArchivoColumnasTexto.numColumnas(d)
    }
}

// C#: Views/Ported/CompresorViewModel.cs (08 pantalla 22; CU-A58, CU-A59).
extension CasosUtilidades {
    /// CU-A58, lectura: conjunto de 3^L (L = longitud de la 1.ª línea) con cada columna; `nil` si alguna no mide 14 o 15
    /// («Error leyendo columnas»). Fichero vacío ⇒ NRE; índice fuera del conjunto ⇒ error.
    public static func bitmapDeColumnas(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> ConjuntoBits? {
        let lineas = ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url))
        guard let primera = lineas.first else { throw .referenciaNula("CompresorViewModel.cs:76") }
        let conv = ConvertidorDeBases(numeroPartidos: primera.utf16.count)
        var bits = ConjuntoBits(cantidad: try ConvertidorDeBases.tamanoConjunto(partidos: primera.utf16.count))
        for c in lineas {
            guard (14...15).contains(c.utf16.count) else { return nil }
            try bits.poner(Int(try conv.numero(c)))
        }
        return bits
    }

    /// CU-A58: `.z3q` (extensión forzada) cuya entrada se llama como la ruta final; bytes de datos y del fichero.
    @concurrent public static func comprimirZ3Q(
        _ bits: ConjuntoBits, en url: URL, nivel: Int, entorno: Entorno
    ) async throws(ErrorDominio) -> (ruta: URL, datos: Int, fichero: Int) {
        let ruta = url.deletingPathExtension().appendingPathExtension("z3q")
        let r = try ArchivoZ3Q.comprimir(
            bits, tamano: bits.cantidad, nombreEntrada: ruta.path(percentEncoded: false), fecha: entorno.reloj.ahora(), nivel: nivel)
        try entorno.archivos.escribir(r.datos, en: ruta, atomico: false)
        return (ruta, r.tamano, r.datos.count)
    }

    /// CU-A59, lectura del `.z3q`.
    @concurrent public static func descomprimirZ3Q(_ url: URL, entorno: Entorno) async throws(ErrorDominio) -> ConjuntoBits {
        try ArchivoZ3Q.descomprimir(try entorno.archivos.leer(url))
    }

    /// CU-A59, escritura: una columna por bit activo en orden de índice (`ObtenNumeroPartidosColBin`); sin columnas no
    /// se crea el fichero.
    @concurrent public static func columnasDeBitmap(_ bits: ConjuntoBits, en url: URL, entorno: Entorno) async throws(ErrorDominio) -> Int {
        let conv = ConvertidorDeBases(numeroPartidos: ConvertidorDeBases.numeroPartidos(tamanoConjunto: bits.cantidad))
        var columnas: [String] = []
        for i in bits.indices { columnas.append(try conv.columna(Int64(i))) }
        if let d = ArchivoColumnasTexto.datos(columnas) { try entorno.archivos.escribir(d, en: url, atomico: false) }
        return columnas.count
    }
}
