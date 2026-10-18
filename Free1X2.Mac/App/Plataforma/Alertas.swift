import Foundation
import Free1X2CasosDeUso
import Observation

/// `AppServices.MostrarError/MostrarInfo/ConfirmarAsync` (10 §1.6; ESPEC_11 §8.4): cola FIFO mostrada de una en una.
@MainActor @Observable
final class Alertas {
    struct Alerta: Identifiable {
        let id = UUID()
        let titulo: String
        let mensaje: String
        /// Con continuación: diálogo Sí/No (botón por defecto «No»).
        var respuesta: CheckedContinuation<Bool, Never>?
    }

    private(set) var cola: [Alerta] = []
    var actual: Alerta? { cola.first }

    func error(_ mensaje: String) { cola.append(Alerta(titulo: "Error", mensaje: mensaje)) }

    func info(_ mensaje: String) { cola.append(Alerta(titulo: "Free1X2", mensaje: mensaje)) }

    /// «Se produjo un error inesperado» para lo que no es un `ErrorDominio` (10 §1.1).
    func error(_ e: any Error) {
        if let d = e as? ErrorDominio {
            error(d.mensajeUsuario)
        } else if let o = e as? ErrorOnline {
            error(o.mensaje)
        } else {
            error("Se produjo un error inesperado:\n\n\(e.localizedDescription)")
        }
    }

    /// Excepción no capturada en el C#: llega al manejador global de `App` (10 §1.1).
    func inesperado(_ e: ErrorDominio) { error("Se produjo un error inesperado:\n\n" + e.mensajeUsuario) }

    /// `ConfirmarAsync`: `true` solo con «Sí».
    func confirmar(_ mensaje: String, titulo: String = "Free1X2") async -> Bool {
        await withCheckedContinuation { c in cola.append(Alerta(titulo: titulo, mensaje: mensaje, respuesta: c)) }
    }

    /// Cierra la alerta actual con la respuesta dada (las informativas la ignoran).
    func cerrar(_ si: Bool) {
        guard !cola.isEmpty else { return }
        cola.removeFirst().respuesta?.resume(returning: si)
    }
}
