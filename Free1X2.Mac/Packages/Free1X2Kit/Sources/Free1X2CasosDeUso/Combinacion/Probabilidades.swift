import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/ProbabilidadPremiosViewModel.cs (10 §81; CU-C17).

/// Una fila de «Probabilidades por categoría».
public struct FilaProbabilidad: Sendable, Hashable, Identifiable {
    public let categoria: String
    public let probabilidad: String
    public var id: String { categoria }
}

extension CasosCombinacion {
    /// CU-C17: rango con `ObtenerListaFromTxtAciertos` (vacío o sin valores ⇒ 10-14), comparación madre/hijas y una fila
    /// por categoría con `#,##0.00;0.00` + " %". `errorSignos` = «Los Dos archivos tienen un número distinto de
    /// signos»: el cálculo sigue y da NaN.
    @concurrent public static func probabilidadDePremios(
        madre: URL, hija: URL, rango: String, entorno: Entorno
    ) async throws(ErrorDominio) -> (filas: [FilaProbabilidad], errorSignos: Bool) {
        let lista = rango.isEmpty ? [] : UtilidadesEntradasValores.obtenerListaFromTxtAciertos(rango).sorted()
        let (minimo, maximo) = (lista.first ?? 10, lista.last ?? 14)
        let archivos = entorno.archivos
        let p = try ProbabilidadPremios.comparar(
            madre: ArchivoColumnasTexto.columnas(try archivos.leer(madre)),
            hija: ArchivoColumnasTexto.columnas(try archivos.leer(hija)), premioMinimo: minimo)
        var filas: [FilaProbabilidad] = []
        for i in minimo...maximo {
            let v = try p.probabilidad(i)
            filas.append(
                FilaProbabilidad(
                    categoria: "Premio de \(i)",
                    probabilidad: FormatoNET.toStringMiles(v, decimales: 2, decimalesNegativo: 2, cultura: entorno.cultura) + " %"))
        }
        return (filas, p.errorSignos)
    }
}
