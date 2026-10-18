import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/ColGanadoraFrmViewModel.cs (08 pantalla 18) y AnalizarCombinacionFrmViewModel.cs (pantalla 7).

/// Lo que analiza el árbol de fallos (`UltimoAnalisis`).
public struct EntradaAnalisisFallos: Sendable {
    public var nombre: String
    public var columna: Columna
    public var combinacion: Combinacion
    /// Pronósticos sin comas (`pronosticosBase`).
    public var pronosticos: [String]

    public init(nombre: String, columna: Columna, combinacion: Combinacion, pronosticos: [String]) {
        (self.nombre, self.columna, self.combinacion, self.pronosticos) = (nombre, columna, combinacion, pronosticos)
    }
}

extension CasosCombinacion {
    /// `AnalizarCombinacionActual` (:146-181): la combinación en pantalla contra «todo lo pronosticado».
    public static func analisisDeCombinacionActual(_ c: Combinacion, ruta: URL?, numeroPartidos n: Int) -> EntradaAnalisisFallos {
        let p = c.pronosticos.flatMap { $0.isEmpty ? nil : $0.map { $0 ?? "" } } ?? Array(repeating: "1,X,2", count: n)
        return EntradaAnalisisFallos(
            nombre: ruta?.path(percentEncoded: false) ?? "Combinación en pantalla",
            columna: UtilColumnas.convStrToLong(pronosticos: p), combinacion: c,
            pronosticos: p.map { $0.replacingOccurrences(of: ",", with: "") })
    }

    /// Pantalla 18: N signos de `1 X 2` (la pantalla ya pone mayúsculas; ESPEC_08 §18 pide rechazar el resto con el mismo
    /// mensaje).
    public static func columnaGanadoraParaAnalisis(_ texto: String, numeroPartidos n: Int) throws(ErrorDominio) -> Columna {
        guard texto.utf16.count == n, texto.allSatisfy({ "1X2".contains($0) }) else {
            throw ErrorDominio(.general, "La columna ganadora debe tener \(n) signos.")
        }
        return UtilColumnas.convStrToLong(texto)
    }

    /// «Abrir combinación para analizar»: solo grupos y controles (`CargaControladorGrupos`), sin filtro de columnas ni
    /// If-Then; pronósticos sin comas.
    public static func analisisDeCombinacionAbierta(
        _ url: URL, columna: Columna, numeroPartidos: Int, entorno: Entorno
    ) throws(ErrorDominio) -> EntradaAnalisisFallos {
        let leida = try ArchivoCombinacion.leer(try entorno.archivos.leer(url), numeroPartidos: numeroPartidos, cultura: entorno.cultura)
        var c = leida.combinacion
        c.ifThen = nil
        c.archivoColumnasBase = ""
        let p = (c.pronosticos ?? []).map { ($0 ?? "").replacingOccurrences(of: ",", with: "") }
        return EntradaAnalisisFallos(nombre: url.path(percentEncoded: false), columna: columna, combinacion: c, pronosticos: p)
    }

    /// CU-A12: el árbol y el título «Análisis de fallos: <fichero>».
    public static func analizarFallosCombinacion(
        _ e: EntradaAnalisisFallos, configuracion: Configuracion, entorno: Entorno
    ) throws(ErrorDominio) -> (titulo: String, nodos: [NodoAnalisis]) {
        let nodos = try AnalisisFallos.analizar(
            e.combinacion, columna: e.columna, pronosticosBase: e.pronosticos, configuracion: configuracion,
            cultura: entorno.cultura, leerLineas: lectorLineas(entorno))
        return ("Análisis de fallos: " + Escrutador.nombreDeArchivo(e.nombre), nodos)
    }
}
