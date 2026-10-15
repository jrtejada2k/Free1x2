import Observation

/// Pila de páginas (≡ `Frame`, 10 §1.2; ESPEC_11 §8.2). Inicio es la raíz y no está en la pila.
@MainActor @Observable
final class Router {
    var pila: [Destino] = []

    var actual: Destino { pila.last ?? .inicio }

    /// `Navegar(page)`: no apila si ya es la página actual.
    func navegar(_ d: Destino) {
        guard d != actual else { return }
        pila.append(d)
    }

    /// `NavegarA(page)`: siempre apila (visor de análisis).
    func navegarSiempre(_ d: Destino) { pila.append(d) }

    /// `GoBack` si se puede.
    func atras() {
        if !pila.isEmpty { pila.removeLast() }
    }
}
