import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/ReductorFrmViewModel.cs (10 pantalla 82; CU-C18, CU-C19).

/// Parámetros de «Reducir» (más de 3 ⇒ `struct`, ESPEC_13 §1).
public struct EntradaReduccion: Sendable, Hashable {
    public var algoritmo: AlgoritmoReduccion
    public var entrada: URL
    public var salida: URL
    public var nivel: Int
    public var maxCol: Int
    public var percent: Int
    public var externas: Bool

    public init(
        algoritmo: AlgoritmoReduccion, entrada: URL, salida: URL, nivel: Int, maxCol: Int, percent: Int, externas: Bool
    ) {
        (self.algoritmo, self.entrada, self.salida, self.nivel) = (algoritmo, entrada, salida, nivel)
        (self.maxCol, self.percent, self.externas) = (maxCol, percent, externas)
    }
}

extension CasosCombinacion {
    public static let mensajeEntradaReductor =
        "El archivo de entrada debe tener 14 partidos.\nCompruebe además que no hay líneas en blanco adicionales al final del archivo."

    /// CU-C18 `EsArchivoEntradaValido` (:115-130): todas las columnas convertibles y 14 signos en la primera; cualquier
    /// error ⇒ no válido.
    public static func validarFicheroEntradaReductor(_ url: URL, entorno: Entorno) -> Bool {
        guard let d = try? entorno.archivos.leer(url) else { return false }
        let conv = ConvertidorDeBases(numeroPartidos: 14)
        for linea in ArchivoColumnasTexto.columnas(d) where (try? conv.numero(linea)) == nil { return false }
        return ArchivoColumnasTexto.numSignos(d) == 14
    }

    /// CU-C19 `ComienzaReduccion`: lee la madre, reduce y graba las reductoras (sin ninguna, la salida no se toca).
    /// Cancelar la tarea deja la cobertura parcial, como `Cancelar()`.
    @concurrent public static func reducirColumnas(
        _ e: EntradaReduccion, numeroPartidos: Int, entorno: Entorno, progreso: ReportadorProgreso = .ninguno
    ) async throws(ErrorDominio) -> ResultadoReduccion {
        let lineas = ArchivoColumnasTexto.columnas(try entorno.archivos.leer(e.entrada))
        let r = try Reduccion.reducir(
            e.algoritmo, lineas: lineas, nivel: e.nivel, maxCol: e.maxCol, percent: e.percent, externas: e.externas,
            numeroPartidos: numeroPartidos, progreso: progreso)
        if let datos = ArchivoColumnasTexto.datos(r.lineas) { try entorno.archivos.escribir(datos, en: e.salida, atomico: false) }
        return r
    }

    /// `RefrescarContadores` (:225-237): `" = n"` y porcentaje con división entera (`"-"` sin iniciales).
    public static func textosReduccion(iniciales: Int, procesadas: Int, finales: Int) -> [String] {
        [" = \(iniciales)", " = \(procesadas)", " = \(finales)", iniciales > 0 ? " = \(procesadas * 100 / iniciales)" : "-"]
    }
}
