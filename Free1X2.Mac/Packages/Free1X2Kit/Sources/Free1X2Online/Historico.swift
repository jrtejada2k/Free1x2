import Foundation
import Free1X2Base

// Nuevo (DT-84, a petición del dueño; no existe en el C#): `GET /quiniela/{pais}/historico`, contrato propuesto en
// `docs/API_CLUBPROGOL.md` §«Histórico (propuesto)».

/// La columna ganadora de una jornada pasada.
public struct ResultadoHistorico: Sendable, Hashable {
    public var jornada: Int32
    public var fecha: String
    /// Los 14 signos ganadores (`1`, `X` o `2`), en mayúsculas.
    public var ganadora: String
}

public enum HistoricoQuiniela {
    /// `{"pais": "ES", "resultados": [{"jornada": 20261011, "fecha": "2026-10-11", "ganadora": "1X21…"}, …]}`: un
    /// resultado sin 14 signos 1/X/2 invalida toda la respuesta (mejor avisar que escrutar con datos rotos).
    public static func analizar(_ json: String?) throws(ErrorDominio) -> [ResultadoHistorico] {
        guard let json, !json.allSatisfy(\.isWhitespace) else { throw formato("La respuesta del histórico está vacía.") }
        let raiz: ValorJSON
        do throws(ErrorJSON) {
            raiz = try JSONNET.analizar(json)
        } catch {
            throw formato("La respuesta del histórico no es un JSON válido: " + error.mensaje)
        }
        guard case .objeto = raiz, case .array(let lista) = raiz["resultados"] else {
            throw formato("Falta el array 'resultados' en el histórico.")
        }
        var r: [ResultadoHistorico] = []
        for v in lista {
            guard case .objeto = v else { throw formato("Un resultado del histórico no es un objeto JSON.") }
            let jornada = try entero(v, "jornada", obligatorio: true)
            let ganadora = FormatoNET.mayusculas(try cadena(v, "ganadora", obligatorio: true))
            guard ganadora.count == JornadaQuiniela.numeroPartidos, ganadora.allSatisfy({ "1X2".contains($0) }) else {
                throw formato("La ganadora de la jornada \(jornada) debe tener 14 signos 1, X o 2: «\(ganadora)».")
            }
            r.append(ResultadoHistorico(jornada: jornada, fecha: try cadena(v, "fecha", obligatorio: false), ganadora: ganadora))
        }
        return r
    }
}
