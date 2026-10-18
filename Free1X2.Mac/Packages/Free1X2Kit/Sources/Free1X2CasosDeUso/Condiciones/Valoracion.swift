import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/ValoracionFrmViewModel.cs (10 pantalla 98; CU-C44…C47).

/// `PrepararValores`: arrays, Mín-Máx y los rangos con los vacíos rellenos.
public struct ValoracionPreparada: Sendable, Hashable {
    public var valores1: [Double]
    public var valoresX: [Double]
    public var valores2: [Double]
    public var minMaxSumas: String
    public var minMaxProductos: String
    /// Global, 1, X, 2.
    public var rangos: [String]
}

extension CasosCondiciones {
    /// CU-C44 (:225-290): `matriz` N×3 (1, X, 2); `K = 0,03420425138`; solo los valores > 29,236 cuentan en los
    /// productos por signo; `Math.Round(…, 3)` a par; textos con `Double.ToString()` de la cultura.
    public static func prepararValoracion(_ matriz: [[Double]], esSuma: Bool, rangos: [String], cultura: Cultura)
        -> ValoracionPreparada
    {
        let k = 0.03420425138
        func t(_ d: Double) -> String { FormatoNET.toString(d, cultura: cultura) }
        let v1 = matriz.map { $0[safe: 0] ?? 0 }
        let vX = matriz.map { $0[safe: 1] ?? 0 }
        let v2 = matriz.map { $0[safe: 2] ?? 0 }
        var (sMin, sMax, s1, sX, s2) = (0.0, 0.0, 0.0, 0.0, 0.0)
        var (pMin, pMax, p1, pX, p2) = (1.0, 1.0, 1.0, 1.0, 1.0)
        for i in v1.indices {
            let (a, b, c) = (v1[i], vX[i], v2[i])
            let mx = Swift.max(Swift.max(a, b), c)
            let mn = Swift.min(Swift.min(a, b), c)
            sMax += mx
            sMin += mn
            if a + b + c > 0 {
                pMax *= mx * k
                pMin *= mn * k
            }
            s1 += a
            sX += b
            s2 += c
            if a > 29.236 { p1 *= a * k }
            if b > 29.236 { pX *= b * k }
            if c > 29.236 { p2 *= c * k }
        }
        pMax = FormatoNET.round(pMax + 0.000999999, decimales: 3)
        pMin = FormatoNET.round(pMin - 0.000999999, decimales: 3)
        if pMin < 0 { pMin = 0 }
        var r = (0..<4).map { rangos[safe: $0] ?? "" }
        if r[0].isEmpty { r[0] = esSuma ? t(sMin) + "-" + t(sMax) : t(pMin) + "-" + t(pMax) }
        for (i, (s, p)) in [(s1, p1), (sX, pX), (s2, p2)].enumerated() where r[i + 1].isEmpty {
            r[i + 1] = "0-" + (esSuma ? t(s) : t(FormatoNET.round(p + 0.000999999, decimales: 3)))
        }
        return ValoracionPreparada(
            valores1: v1, valoresX: vX, valores2: v2, minMaxSumas: t(sMin) + "-" + t(sMax),
            minMaxProductos: t(pMin) + "-" + t(pMax), rangos: r)
    }

    /// CU-C45 `VolcarA` (:388-406): siempre hay datos; solo se activa la primera vez.
    public static func aplicarValoracion(_ p: ValoracionPreparada, esSuma: Bool, en f: inout CondicionValoracionSignos, cultura: Cultura)
        throws(ErrorDominio)
    {
        try f.reinicializaValores(numeroPartidos: f.valores1.count)
        f.tipoValoracion = esSuma ? "suma" : "multiplo"
        (f.valores1, f.valoresX, f.valores2) = (p.valores1, p.valoresX, p.valores2)
        try f.ponerValorGlobal(p.rangos[0], cultura: cultura)
        try f.ponerValorUnos(p.rangos[1], cultura: cultura)
        try f.ponerValorEquis(p.rangos[2], cultura: cultura)
        try f.ponerValorDoses(p.rangos[3], cultura: cultura)
        if !f.contieneDatos { f.activa = true }
        f.contieneDatos = true
    }

    /// CU-C46 `Calcular` (:171-196): con la columna completa escribe valores y tipo en la condición **del grupo** (como
    /// hoy) y devuelve `ValoracionResultado.ToString("r")`; columna corta ⇒ `nil`.
    public static func valorarColumna(
        _ columna: String, _ p: ValoracionPreparada, esSuma: Bool, en f: inout CondicionValoracionSignos, numeroPartidos n: Int,
        cultura: Cultura
    ) throws(ErrorDominio) -> String? {
        guard columna.utf16.count >= n else { return nil }
        (f.valores1, f.valoresX, f.valores2) = (p.valores1, p.valoresX, p.valores2)
        f.tipoValoracion = esSuma ? "suma" : "multiplo"
        return FormatoNET.toString(try f.valoracionResultado(UtilColumnas.convStrToLong(columna), cultura: cultura), cultura: cultura)
    }

    /// CU-C47 `ObtenerColsBase` (:293-339): mayor, medio y menor de cada partido, separados por CRLF.
    public static func columnasBase(_ matriz: [[Double]]) -> String {
        var c = ["", "", ""]
        for f in matriz {
            let (v1, vX, v2) = (f[safe: 0] ?? 0, f[safe: 1] ?? 0, f[safe: 2] ?? 0)
            let orden: String =
                if v1 >= v2 && v1 >= vX {
                    vX >= v2 ? "1X2" : "12X"
                } else if vX >= v2 {
                    v1 >= v2 ? "X12" : "X21"
                } else {
                    vX > v1 ? "2X1" : "21X"
                }
            for (j, s) in orden.enumerated() { c[j].append(s) }
        }
        return c.joined(separator: "\r\n")
    }
}

// C#: Views/Ported/GuardarValoracionFrmViewModel.cs (09 pantalla 63; CU-B31) y HistoriaValoracionesFrmViewModel.cs
// (09 pantalla 64; CU-B32).
extension CasosCondiciones {
    /// CU-B31 `Aceptar`: disposición 3, 42 o 1 (en la 1 el separador no se usa).
    public static func guardarValoracion(
        _ valores: [[Double]], formato: Int16, separador: Character, en url: URL, numeroPartidos n: Int, entorno: Entorno
    ) throws(ErrorDominio) {
        let datos = try ArchivoValoraciones.datos(
            valores, formato: formato, separador: separador, numeroPartidos: n, cultura: entorno.cultura)
        try entorno.archivos.escribir(datos, en: url, atomico: false)
    }

    /// CU-B32 `Guardar`: «T/T+1» y jornada a 2 cifras; un fichero existente fija el separador y debe ser de formato 44.
    public static func guardarEnHistorico(
        _ valores: [[Double]], temporada: Double, jornada: Double, en url: URL, numeroPartidos n: Int, entorno: Entorno
    ) throws(ErrorDominio) -> (temporada: String, jornada: String) {
        let t = FormatoNET.castInt32(temporada)
        let tempo = "\(t)/\(t &+ 1)"
        let j = String(FormatoNET.castInt32(jornada))
        let jorna = String(repeating: "0", count: max(0, 2 - j.count)) + j
        let existente = entorno.archivos.existe(url) ? try entorno.archivos.leer(url) : nil
        let r = try ArchivoValoraciones.datosHistorico(
            existente: existente, separador: "\t", valores: valores, temporada: tempo, jornada: jorna, numeroPartidos: n,
            cultura: entorno.cultura)
        guard let datos = r.datos else { throw ErrorDominio(.general, r.aviso ?? ArchivoValoraciones.avisoNoHistorico) }
        try entorno.archivos.escribir(datos, en: url, atomico: false)
        return (tempo, jorna)
    }
}
