import Free1X2Motor
import Testing

/// #13: `OrdenacionPropia` (las dos mitades a la vez) deja exactamente el mismo orden que el quicksort de un hilo de
/// antes (`referencia`, copiado tal cual), con empates, 9E+10 y NaN. 140 000 elementos reparten en paralelo
/// (65 536 como mínimo por mitad); más pesaba en el CI.
@Suite("Ordenación propia")
struct OrdenacionPropiaTests {
    @Test func mismoOrdenQueEnUnHilo() {
        var s: UInt64 = 1
        func siguiente(_ n: Int) -> Int {
            s = s &* 6_364_136_223_846_793_005 &+ 1_442_695_040_888_963_407
            return Int(s >> 33) % n
        }
        var a = (0..<140_000).map { i in
            let p: Float = i % 5 == 0 ? 9E+10 : i % 977 == 0 ? .nan : Float(siguiente(5000)) / 13
            return ApuestaProbable(probabilidad: p, columna: Int32(i))
        }
        var b = a
        Self.referencia(&a) { $0.probabilidad }
        OrdenacionPropia.ordenar(&b) { $0.probabilidad }
        #expect(a.map(\.columna) == b.map(\.columna))
    }

    /// `ordena(izq, der)` de un hilo, como antes de #13.
    static func referencia<T, K: Comparable>(_ a: inout [T], clave: (T) -> K) {
        func ordena(_ izq: Int, _ der: Int) {
            var (i, j) = (izq, der)
            let x = clave(a[(izq + der) / 2])
            repeat {
                while clave(a[i]) < x && j <= der { i += 1 }
                while x < clave(a[j]) && j > izq { j -= 1 }
                if i <= j {
                    a.swapAt(i, j)
                    i += 1
                    j -= 1
                }
            } while i <= j
            if izq < j { ordena(izq, j) }
            if i < der { ordena(i, der) }
        }
        if a.count > 1 { ordena(0, a.count - 1) }
    }
}
