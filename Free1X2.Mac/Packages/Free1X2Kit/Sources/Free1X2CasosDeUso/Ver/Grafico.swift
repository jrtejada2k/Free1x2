import Foundation
import Free1X2Base
import Free1X2Formatos

// C#: Views/Ported/GraficoColumnasFrmViewModel.cs (09 pantalla 61; CU-B29) y el dibujo de GraficoColumnasFrmPage.xaml.cs.

/// «Código de barras» de una combinación en 10 franjas de 959×25 (lienzo 977×309).
public struct GraficoCombinacion: Sendable {
    public struct Linea: Sendable, Hashable {
        public let x, y0, y1: Int
    }
    public struct Relleno: Sendable, Hashable {
        public let x, y, ancho, alto: Int
    }

    /// ponytail: sin repetir (el C# añade una por apuesta; se ven igual), en orden de primera aparición.
    public var lineas: [Linea] = []
    public var rellenos: [Relleno] = []
    public var minimo = 0
    public var maximo = 0
    public var apuestas = 0

    public init() {}
}

extension CasosVer {
    /// `Convert.ToInt16(double)`: redondeo al par; fuera de rango (o NaN) lanza.
    static func int16NET(_ d: Double) throws(ErrorDominio) -> Int {
        let r = d.rounded(.toNearestOrEven)
        guard r >= -32768 && r <= 32767 else { throw .desbordamiento("GraficoColumnasFrmViewModel.cs:ToInt16") }
        return Int(r)
    }

    /// `ComputarGrafico` (:131-185): índices base 3 de las columnas (línea entera), ordenados. La cancelación solo corta
    /// el bucle de líneas.
    @concurrent public static func graficoCombinacion(_ url: URL, entorno: Entorno) async throws(ErrorDominio) -> GraficoCombinacion {
        let conv = ConvertidorDeBases(numeroPartidos: 14)
        var m: [Int] = []
        for l in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url)) { m.append(Int(try conv.numero(l))) }
        var g = GraficoCombinacion()
        g.apuestas = m.count
        guard let minimo = m.min(), let maximo = m.max() else { return g }
        m.sort()
        (g.minimo, g.maximo) = (minimo, maximo)
        let dif = maximo - minimo
        let escala: Double
        if dif < 9566 {
            escala = 1
            let alto = try int16NET(Double(dif * 10 / 9580))
            // Usa `maximo` absoluto: el ancho sale ≤ 0 si maximo ≥ 958 (no se dibuja).
            g.rellenos.append(.init(x: maximo + 1, y: alto * 25 + 50, ancho: 958 - maximo, alto: 25))
            for i in stride(from: alto + 1, to: 10, by: 1) { g.rellenos.append(.init(x: 9, y: i * 25 + 50, ancho: 958, alto: 25)) }
        } else {
            escala = Double(dif) / 9565.938
        }
        var vistas = Set<GraficoCombinacion.Linea>()
        for v in m {
            if Task.isCancelled { break }
            var num = Double(v - minimo) / escala
            let alto = try int16NET(num / 958 - 0.5)
            num -= Double(alto * 958)
            let l = GraficoCombinacion.Linea(x: try int16NET(num) + 10, y0: alto * 25 + 51, y1: alto * 25 + 74)
            if vistas.insert(l).inserted { g.lineas.append(l) }
        }
        return g
    }

    /// `MostrarGuia` (:218-260): columna del inicio de cada franja en base 3 (partido 14 a la izquierda; 0 ⇒ 1, 1 ⇒ X).
    public static func guiaGrafico(minimo: Int, maximo: Int) -> [String] {
        let dif = maximo - minimo
        return (1...10).map { i in
            let s = String((i - 1) * (dif / 10) + minimo, radix: 3)
            let t = (String(repeating: "0", count: max(0, 14 - s.count)) + s)
                .replacingOccurrences(of: "1", with: "X").replacingOccurrences(of: "0", with: "1")
            return (i < 10 ? "0" : "") + "\(i))  " + t
        }
    }
}
