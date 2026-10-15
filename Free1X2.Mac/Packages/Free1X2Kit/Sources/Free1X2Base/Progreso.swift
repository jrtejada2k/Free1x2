import Foundation
import os

/// Estado de una operación larga (ESPEC_11 §6.2). Sustituye `UiPump`/`IProgressNotifier` (T12).
public struct Progreso: Sendable, Equatable {
    public var procesadas: Int
    public var total: Int?
    public var aceptadas: Int?
    /// Texto de estado que hoy muestra la pantalla, si lo hay.
    public var fase: String?

    public init(procesadas: Int = 0, total: Int? = nil, aceptadas: Int? = nil, fase: String? = nil) {
        self.procesadas = procesadas
        self.total = total
        self.aceptadas = aceptadas
        self.fase = fase
    }
}

/// Canal de progreso con frecuencia limitada: como mucho un evento por `intervalo` (100 ms), más el final.
/// El bucle caliente solo cuenta en un entero local y llama a `reportar` cada 65 536 columnas (ESPEC_11 §6.2).
public final class ReportadorProgreso: Sendable {
    /// Para llamadas sin pantalla: no emite nada.
    public static let ninguno = ReportadorProgreso(reloj: RelojDetenido(), emitir: { _ in })

    private let reloj: any Reloj
    private let intervalo: Duration
    private let emitir: @Sendable (Progreso) -> Void
    private let ultimo = OSAllocatedUnfairLock<Duration?>(initialState: nil)

    public init(
        reloj: any Reloj,
        intervalo: Duration = .milliseconds(100),
        emitir: @escaping @Sendable (Progreso) -> Void
    ) {
        self.reloj = reloj
        self.intervalo = intervalo
        self.emitir = emitir
    }

    /// Emite si ha pasado al menos `intervalo` desde el último evento (el primero sale siempre).
    public func reportar(_ progreso: Progreso) {
        let ahora = reloj.monotono()
        let toca = ultimo.withLock { ultimo in
            if let anterior = ultimo, ahora - anterior < intervalo { return false }
            ultimo = ahora
            return true
        }
        if toca { emitir(progreso) }
    }

    /// Último evento de la operación: sale siempre.
    public func terminar(_ progreso: Progreso) {
        ultimo.withLock { $0 = reloj.monotono() }
        emitir(progreso)
    }
}

/// Reloj que no avanza; solo lo usa `ReportadorProgreso.ninguno`.
private struct RelojDetenido: Reloj {
    func ahora() -> Date { Date(timeIntervalSince1970: 0) }
    func monotono() -> Duration { .zero }
}
