import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/AnastaticsViewModel.cs (08 pantalla 9; CU-A15) y sus visores DibFormViewModel.cs (38, CU-A88),
// DibRepFrmViewModel.cs (39, CU-A89), StaInterFrmViewModel.cs (90, CU-C32) y StaSSFormViewModel.cs (91).

/// Matriz 15 × 15 de «Calcular» con el nº de columnas analizadas (handoff `MatrizEntrada` / `NumColEntrada`).
public struct MatrizEstadistica: Sendable, Hashable {
    public let rsl: [[Int]]
    public let numColumnas: Int

    public init(rsl: [[Int]], numColumnas: Int) { (self.rsl, self.numColumnas) = (rsl, numColumnas) }

    public static let vacia = MatrizEstadistica(rsl: Array(repeating: Array(repeating: 0, count: 15), count: 15), numColumnas: 0)
}

extension CasosVer {
    /// `SeleccionarOrigen`: columnas validadas por `VerColumna` (tope 500 000, se trunca sin aviso) o el índice
    /// 0-based de la primera errónea («columna errónea = N»).
    public static func columnasEstadisticas(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> (columnas: [String], errorEn: Int?) {
        var r: [String] = []
        for l in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url)) {
            guard let v = Anastatics.verColumna(l) else { return (r, r.count) }
            r.append(v)
            if r.count == Anastatics.maximoColumnas { break }
        }
        return (r, nil)
    }

    /// `Calcular`: `Proceso(modo)` fuera del hilo principal.
    @concurrent public static func calcularEstadistica(
        _ modo: Anastatics.Modo, columnas: [String]
    ) async throws(ErrorDominio) -> MatrizEstadistica {
        MatrizEstadistica(rsl: try Anastatics.proceso(modo, columnas: columnas), numColumnas: columnas.count)
    }

    /// Celda de los visores: recuento o `(v·10000 / numcol) / 1E2` con división entera. En `Int32` con desbordamiento
    /// (T1, se replica) salvo en «Dibujos», que va en `Int64`.
    static func celdaEstadistica(_ v: Int64, porcentaje: Bool, numColumnas n: Int, int64: Bool, cultura: Cultura) -> String {
        guard porcentaje else { return String(v) }
        let q =
            int64
            ? v * 10000 / Int64(n == 0 ? 1 : n)
            : Int64(Int32(truncatingIfNeeded: v) &* 10000 / Int32(truncatingIfNeeded: n == 0 ? 1 : n))
        return FormatoNET.toString(Double(q) / 1E2, cultura: cultura)
    }

    private static func cabecera(_ etiqueta: String, _ n: Int, _ extra: [String] = []) -> MatrizAnalisis.Fila {
        MatrizAnalisis.Fila(etiqueta: etiqueta, celdas: (0..<n).map(String.init) + extra, esCabecera: true)
    }

    /// «Dibujos» (38, `Repintar`): triangular (`nf + nc < 15`) con «Σ fila» y «Σ col», más «Variantes (X + 2)»;
    /// cultura invariante.
    public static func matrizDibujos(_ m: MatrizEstadistica, porcentajes: Bool) -> [BloqueAnalisis] {
        func f(_ v: Int64) -> String {
            celdaEstadistica(v, porcentaje: porcentajes, numColumnas: m.numColumnas, int64: true, cultura: .invariante)
        }
        var filas = [cabecera("X \\ 2", 15, ["Σ fila"])]
        var variantes = [Int64](repeating: 0, count: 29)
        for nf in 0..<15 {
            var celdas: [String] = []
            for nc in 0..<15 {
                celdas.append(nf + nc < 15 ? f(Int64(m.rsl[nf][nc])) : "")
                variantes[nf + nc] += Int64(m.rsl[nf][nc])
            }
            filas.append(.init(etiqueta: String(nf), celdas: celdas + [f(m.rsl[nf].reduce(0) { $0 + Int64($1) })], esCabecera: false))
        }
        let columnas = (0..<15).map { nc in f(m.rsl.reduce(0) { $0 + Int64($1[nc]) }) }
        filas.append(.init(etiqueta: "Σ col", celdas: columnas + [""], esCabecera: false))
        return [
            BloqueAnalisis(titulo: "Distribución (filas = nº de X, columnas = nº de 2)", matriz: MatrizAnalisis(filas: filas)),
            BloqueAnalisis(
                titulo: "Variantes (X + 2)",
                matriz: MatrizAnalisis(filas: [
                    cabecera("", 15), .init(etiqueta: "", celdas: variantes.prefix(15).map(f), esCabecera: false),
                ])),
        ]
    }

    /// «Coincidencias» (39), «Interrupciones» (90) y «Signos seguidos» (91): filas etiquetadas × `columnas`.
    public static func matrizEstadistica(
        _ m: MatrizEstadistica, etiquetas: [String], columnas: Int, porcentajes: Bool, cultura: Cultura
    ) -> MatrizAnalisis {
        var filas = [cabecera("", columnas)]
        for (i, e) in etiquetas.enumerated() {
            let celdas = (0..<columnas).map {
                celdaEstadistica(Int64(m.rsl[i][$0]), porcentaje: porcentajes, numColumnas: m.numColumnas, int64: false, cultura: cultura)
            }
            filas.append(.init(etiqueta: e, celdas: celdas, esCabecera: false))
        }
        return MatrizAnalisis(filas: filas)
    }
}
