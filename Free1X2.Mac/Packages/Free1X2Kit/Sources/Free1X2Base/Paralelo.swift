import Dispatch
import Foundation
import os

/// Hace `trabajo(0..<n)` en todos los núcleos (`DispatchQueue.concurrentPerform`) y devuelve los resultados en orden
/// (DM-22): para repartir un bucle en trozos y juntarlos después como si fuera uno. Con `n` = 1, en el hilo que llama.
public func enParalelo<T: Sendable>(_ n: Int, _ trabajo: @Sendable (Int) -> T) -> [T] {
    if n <= 1 { return (0..<n).map(trabajo) }
    return [T](unsafeUninitializedCapacity: n) { buffer, inicializados in
        guard let base = buffer.baseAddress else { return }
        let destino = Destino(base: base)
        DispatchQueue.concurrentPerform(iterations: n) { i in (destino.base + i).initialize(to: trabajo(i)) }
        inicializados = n
    }
}

/// `elementos.map(f)` en todos los núcleos, cada resultado escrito en su sitio (sin arrays intermedios), por trozos de
/// al menos `minimo` elementos; con menos, en el hilo que llama (DM-22).
public func mapaParalelo<T: Sendable, U: Sendable>(_ elementos: [T], minimo: Int = 65_536, _ f: @Sendable (T) -> U) -> [U] {
    let trozos = trozosParalelos(elementos.count, minimo: minimo)
    if trozos.count <= 1 { return elementos.map(f) }
    return elementos.withUnsafeBufferPointer { origen in
        [U](unsafeUninitializedCapacity: elementos.count) { buffer, inicializados in
            guard let a = origen.baseAddress, let b = buffer.baseAddress else { return }
            let (o, d) = (Origen(base: a), Destino(base: b))
            DispatchQueue.concurrentPerform(iterations: trozos.count) { t in
                for i in trozos[t] { (d.base + i).initialize(to: f(o.base[i])) }
            }
            inicializados = elementos.count
        }
    }
}

/// Cada iteración escribe solo su casilla.
private struct Destino<T>: @unchecked Sendable {
    let base: UnsafeMutablePointer<T>
}

/// Solo se lee.
private struct Origen<T>: @unchecked Sendable {
    let base: UnsafePointer<T>
}

/// Cuántos trozos usar para `n` elementos: cuatro por núcleo (para repartir bien), con al menos `minimo` elementos
/// cada uno; con menos, uno.
public func trozosParalelos(_ n: Int, minimo: Int = 32_768) -> [Range<Int>] {
    let k = max(1, min(ProcessInfo.processInfo.activeProcessorCount * 4, n / minimo))
    return (0..<k).map { n * $0 / k..<n * ($0 + 1) / k }
}

/// Avisa a los bucles repartidos con `enParalelo` de que la tarea que los lanzó se canceló: dentro de
/// `concurrentPerform` no hay tarea y `Task.isCancelled` siempre da `false`.
public final class Cancelacion: Sendable {
    private let estado = OSAllocatedUnfairLock(initialState: false)

    public var activa: Bool { estado.withLock { $0 } }

    fileprivate func activar() { estado.withLock { $0 = true } }
}

/// `trabajo` con una `Cancelacion` que se activa si se cancela la tarea que llama.
public func conCancelacion<T: Sendable>(_ trabajo: @Sendable (Cancelacion) -> T) async -> T {
    let c = Cancelacion()
    return await withTaskCancellationHandler {
        trabajo(c)
    } onCancel: {
        c.activar()
    }
}
