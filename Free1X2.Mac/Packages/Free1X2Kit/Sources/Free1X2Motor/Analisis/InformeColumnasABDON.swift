import Free1X2Base

// C#: Free1X2.Domain/Analisis/InformeColumnasABDON_UnaColTodasGan.cs (06 §3.9): fachada de `RelacionCP3(serie)`.

/// Informe ABDON de EstuCol sobre una serie de aciertos: escaleras, sándwiches y agrupaciones (04 §9).
public struct InformeColumnasABDON: Sendable, Hashable {
    public let serieAciertos: [Int]

    public init(serie: [Int]) { serieAciertos = serie }

    /// `MinimoAciertos` / `MaximoAciertos` / `SumaTotalDeAciertos` (`RelacionCP3.cs:216-290`): 0 si la serie está vacía.
    public var minimoAciertos: Int { serieAciertos.min() ?? 0 }
    public var maximoAciertos: Int { serieAciertos.max() ?? 0 }
    public var sumaTotalDeAciertos: Int { serieAciertos.reduce(0, +) }
    /// Escalones de cada escalera, en orden de aparición.
    public var escaleras: [[Int]] { RelacionCP3.escaleras(serieAciertos) }
    public var numeroDeEscaleras: Int { escaleras.count }
    public var numeroDeEscalerasAscendentes: Int { RelacionCP3.ascendentes(escaleras) }
    public var numeroDeEscalerasDescendentes: Int { numeroDeEscaleras - numeroDeEscalerasAscendentes }
    public var sandwichs: [[Int]] { RelacionCP3.sandwichs(serieAciertos) }
    /// `[longitud de racha][valor]`; un valor ≥ 15 lanza como en C#.
    public func agrupacionesPasoFijo() throws(ErrorDominio) -> [[Int]] { try RelacionCP3.pasoFijo(serieAciertos) }
    public func agrupacionesSolapadas() throws(ErrorDominio) -> [[Int]] {
        RelacionCP3.solapadas(try RelacionCP3.pasoFijo(serieAciertos))
    }
}
