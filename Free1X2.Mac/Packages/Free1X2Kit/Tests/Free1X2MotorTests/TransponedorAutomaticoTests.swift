import Free1X2Motor
import Testing

/// ESPEC_06 §3.10, los 4 casos golden propuestos, con los esperados calculados a mano.
@Suite("Transponedor automático")
struct TransponedorAutomaticoTests {
    /// Fila i con concentración estrictamente decreciente: [1 − a, a/2, a/2], a = 0,05·i (a ≤ 0,65 < 2/3).
    static let decreciente: [[Double]] = (0..<14).map { i in
        let a = 0.05 * Double(i)
        return [1 - a, a / 2, a / 2]
    }

    @Test func identidadYPermutacion() {
        #expect(TransponedorAutomatico.trasposicion(valoraciones: Self.decreciente, frecuencias: Self.decreciente) == Array(0..<14))
        // Frecuencias al revés: el más concentrado de una es el 0 y de la otra el 13 ⇒ r[13 − i] = i.
        let r = TransponedorAutomatico.trasposicion(valoraciones: Self.decreciente, frecuencias: Self.decreciente.reversed())
        #expect(r == Array((0..<14).reversed()))
    }

    /// Empates: la ordenación por intercambio no es estable. Jornada uniforme ⇒ valoraciones en su orden; frecuencias con
    /// 6 filas [34,33,33] (0,3334) y 8 filas [100,0,0] (1,0) ⇒ pmediaF = 6…13, 2, 3, 4, 5, 0, 1 (a mano, bucle a bucle).
    @Test func empatesNoEstables() {
        let v = [[Double]](repeating: [20, 30, 50], count: 14)
        let f = [[Double]](repeating: [34, 33, 33], count: 6) + [[Double]](repeating: [100, 0, 0], count: 8)
        #expect(TransponedorAutomatico.trasposicion(valoraciones: v, frecuencias: f) == [12, 13, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5, 6, 7])
        let uniforme = [[Double]](repeating: [1, 1, 1], count: 14)
        #expect(TransponedorAutomatico.trasposicion(valoraciones: uniforme, frecuencias: uniforme) == Array(0..<14))
    }

    /// Fila [0,0,0] ⇒ NaN; las comparaciones con NaN son falsas y no mueve nada. Menos de 14 filas ⇒ `nil`.
    @Test func nanYTamano() {
        let uniforme = [[Double]](repeating: [1, 1, 1], count: 14)
        var v = uniforme
        v[0] = [0, 0, 0]
        #expect(TransponedorAutomatico.trasposicion(valoraciones: v, frecuencias: uniforme) == Array(0..<14))
        #expect(TransponedorAutomatico.trasposicion(valoraciones: Array(uniforme.prefix(13)), frecuencias: uniforme) == nil)
    }
}
