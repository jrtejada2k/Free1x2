import Free1X2Base
import Observation

/// Estado de la app (≡ `AppState`, ESPEC_11 §8.1). ponytail: `configuracion`, `combinacion`, `grupoPantalla`,
/// `rutaCombinacion` y `jornadaActual` llegan con sus tipos en M3-M5.
@MainActor @Observable
final class Sesion {
    let entorno: Entorno

    init(entorno: Entorno) {
        self.entorno = entorno
    }
}
