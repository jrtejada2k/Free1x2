import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/EscrutarCombinacionesFrmViewModel.cs (09 pantalla 44; CU-B04). El cálculo es `EscrutadorComb` (05 §4.2):
// la primera candidata que cumple lanza (DEC-501) y las premiadas nunca reciben elementos (DEC-502).

extension CasosCombinacion {
    /// Validación de «Escrutar!» (:336-381), textos literales de esta pantalla.
    public static func validarEscrutinioComb(_ e: EntradaEscrutinio) throws(ErrorDominio) {
        func falta(_ m: String) -> ErrorDominio { ErrorDominio(.general, m) }
        switch e.modo {
        case .simple(let g):
            if e.ficheros.isEmpty { throw falta("Falta fichero a escrutar.") }
            if g.replacingOccurrences(of: "*", with: "").isEmpty { throw falta("Falta columna ganadora.") }
        case .contraFichero(let referencia):
            if e.ficheros.isEmpty { throw falta("Falta fichero a escrutar.") }
            if referencia == nil { throw falta("Falta fichero referencia.") }
        case .contraJornadas:
            try validarEscrutinio(e)
        }
    }

    /// `EscrutarAsync`: por fichero de combinación un `EscrutadorComb` con las categorías; sus totales se suman una vez
    /// por fichero (el legacy los sumaba por jornada). Devuelve el histograma «aciertos: columnas» de mayor a menor.
    @concurrent public static func escrutarCombinaciones(
        _ e: EntradaEscrutinio, numeroPartidos n: Int, configuracion: Configuracion, entorno: Entorno
    ) async throws(ErrorDominio) -> [(aciertos: Int, columnas: Int)] {
        var globales: [Int: Int] = [:]
        func escrutar(_ url: URL, _ ganadoras: [String]) throws(ErrorDominio) {
            var esc = EscrutadorComb(premios: e.rangoAciertos, numeroPartidos: n)
            let c = try ArchivoCombinacion.leer(try entorno.archivos.leer(url), numeroPartidos: n, cultura: entorno.cultura).combinacion
            for g in ganadoras {
                try esc.obtenerPosiblesPremios(g)
                try esc.escrutarCombinacion(c, configuracion: configuracion, cultura: entorno.cultura)
            }
            for (cat, total) in zip(esc.premios, esc.premiosTotales) { globales[cat, default: 0] += total }
        }
        switch e.modo {
        case .simple(let g):
            for f in e.ficheros { try escrutar(f, [g]) }
        case .contraFichero(let r):
            for f in e.ficheros {
                guard let r else { break }
                try escrutar(f, ArchivoColumnasTexto.columnas(try entorno.archivos.leer(r)))
            }
        case .contraJornadas(let plantilla, let carpeta, let dt, let dj, let temporadas):
            guard let carpeta else { break }
            for x in try ficherosDeJornadas(
                plantilla: plantilla, carpeta: carpeta, digitosTemporada: dt, digitosJornada: dj, temporadas: temporadas,
                historico: cargarHistoricoJornadas(entorno: entorno).filas, entorno: entorno)
            {
                try escrutar(URL(filePath: x.ruta), [x.quiniela])
            }
        }
        return globales.sorted { $0.key > $1.key }.map { ($0.key, $0.value) }
    }
}
