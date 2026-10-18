import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/EstimadorPremiosFrmViewModel.cs (09 pantalla 46; CU-B09, CU-B10).

/// «Previsión de premios»: textos de acertantes y premio por categoría (14…10) y el nº de apuestas.
public struct PrevisionPremios: Sendable, Hashable {
    public let acertantes: [String]
    public let premios: [String]
    public let numApuestas: String

    /// Vacía (antes de calcular).
    public static let vacia = PrevisionPremios(
        acertantes: Array(repeating: "", count: 5), premios: Array(repeating: "", count: 5), numApuestas: "")
}

extension CasosUtilidades {
    /// Temporada por defecto: `año-1/año` hasta junio, `año/año+1` desde julio.
    public static func temporadaActual(_ fecha: Date) -> String {
        let c = Calendar(identifier: .gregorian).dateComponents([.year, .month], from: fecha)
        let (a, m) = (c.year ?? 2000, c.month ?? 1)
        return m < 7 ? "\(a - 1)/\(a)" : "\(a)/\(a + 1)"
    }

    /// `ValoresBase100` del estimador: factor `Float` de la suma, cociente en `Double` y resultado `Float` (T3).
    static func base100(_ v: [Double]) -> [Float] {
        let f = Float(v[0] + v[1] + v[2])
        return v.prefix(3).map { Float($0 / Double(f)) }
    }

    /// CU-B09 `Calcular`: `nil` si la columna tiene menos de 14 signos. El bote solo va al 14; `Math.Round` a par;
    /// premio del 14 limitado a lo destinado y el del 10 a 0 si no llega a 1.
    public static func estimarPremios(
        valores: [[Double]], columna: String, numApuestas: Double, recaudacion: Double, bote: Double, porcentajes: [Double],
        cultura: Cultura
    ) -> PrevisionPremios? {
        let col = Array(columna.utf16)
        guard col.count >= 14, valores.count >= 14 else { return nil }
        var destinado = porcentajes.map { recaudacion * ($0 / 100) }
        destinado[0] += bote
        let n = Int(FormatoNET.castInt32(numApuestas))
        var prob14 = 1.0
        var cr = [Float](repeating: 0, count: 14)
        for p in 0..<14 {
            let s = [0x31, 0x58, 0x32].firstIndex(of: col[p]) ?? 0
            let b = base100(valores[p])[s]
            prob14 *= Double(b)
            cr[p] = (1 - b) / b
        }
        var acertantes = [Double](repeating: 0, count: 5)
        acertantes[0] = Double(n) * prob14
        var premios = [Double](repeating: 0, count: 5)
        premios[0] = FormatoNET.round(destinado[0] / acertantes[0], decimales: 2)
        func rec(_ prob: Double, _ desde: Int, _ nivel: Int) {
            for p in desde..<14 {
                let x = prob * Double(cr[p])
                acertantes[nivel] += x
                if nivel < 4 { rec(x, p + 1, nivel + 1) }
            }
        }
        rec(acertantes[0], 0, 1)
        for i in 1..<5 {
            premios[i] = FormatoNET.round(destinado[i] / acertantes[i], decimales: 2)
            acertantes[i] = FormatoNET.round(acertantes[i], decimales: 0)
        }
        acertantes[0] = FormatoNET.round(acertantes[0], decimales: 1)
        if premios[0] > destinado[0] { premios[0] = destinado[0] }
        if premios[4] < 1 { premios[4] = 0 }
        let t = { (d: Double) in FormatoNET.toString(d, cultura: cultura) }
        return PrevisionPremios(acertantes: acertantes.map(t), premios: premios.map(t), numApuestas: String(n))
    }

    static func rutaInfoJornadas(_ entorno: Entorno) -> URL { entorno.rutas.base.appending(path: "Jornadas/InfoJornadasLAE.txt") }

    /// `ParseEsp`: cultura actual; si falla, con punto decimal invariante; si no, 0.
    static func parseEsp(_ s: String, _ cultura: Cultura) -> Double {
        guard !CasosCondiciones.blanco(s) else { return 0 }
        return FormatoNET.tryParseDouble(s, cultura: cultura)
            ?? FormatoNET.tryParseDouble(s.replacingOccurrences(of: ",", with: "."), cultura: .invariante) ?? 0
    }

    /// CU-B10 `RecargarDatosJornada` (◀ ▶): la primera línea de `Jornadas/InfoJornadasLAE.txt` de esa temporada y
    /// jornada; `nil` sin fichero o sin línea. `destinado` usa la recaudación anterior a la de la línea (se replica).
    public static func datosJornadaLAE(
        temporada: String, jornada: Double, recaudacion: Double, bote: Double, precio: Double, porcentajes: [Double],
        entorno: Entorno
    ) throws(ErrorDominio) -> (columna: String, recaudacion: Double, acertantes: [Double], premios: [Double])? {
        let url = rutaInfoJornadas(entorno)
        guard entorno.archivos.existe(url) else { return nil }
        let c = entorno.cultura
        var destinado = porcentajes.map { recaudacion * ($0 / 100) }
        destinado[0] += bote
        let j = String(FormatoNET.castInt32(jornada))
        let buscada = String(repeating: "0", count: max(0, 2 - j.count)) + j
        for l in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url)) {
            let v = l.components(separatedBy: "\t")
            guard v.count >= 9, v[1] == temporada, v[2] == buscada else { continue }
            let num = (4...8).map { parseEsp(v[$0], c) }
            var premios = num.map { precio * $0 }
            if v[4] == "0,00" { premios[0] = precio * parseEsp(v[3], c) * (porcentajes[0] / 100) }
            var acertantes = (0..<5).map { num[$0] != 0 ? FormatoNET.round(destinado[$0] / num[$0] / precio, decimales: 0) : 0 }
            if num[0] == 0 { acertantes[0] = 0 }
            if precio * num[4] < 1 { premios[4] = 0 }
            return (v[0], precio * parseEsp(v[3], c), acertantes, premios)
        }
        return nil
    }

    /// «Guardar escrutinio»: `LAE.GrabarJornada` con los acertantes truncados (los premios los recalcula LAE).
    public static func guardarEscrutinioLAE(
        temporada: String, jornada: Double, columna: String, recaudacion: Double, acertantes: [Double], lae: ConfigLAE,
        entorno: Entorno
    ) throws(ErrorDominio) {
        let url = rutaInfoJornadas(entorno)
        var j = LAE(
            temporada: temporada, numJornada: FormatoNET.toString(jornada, cultura: entorno.cultura),
            columnaPremiada: String(decoding: columna.utf16.prefix(14), as: UTF16.self), recaudacion: recaudacion, configuracion: lae)
        let lineas = ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url))
        let nuevas = try j.grabarJornada(
            acertantes: acertantes.map { Int(FormatoNET.castInt32($0)) }, lineas: lineas, cultura: entorno.cultura)
        if let d = ArchivoColumnasTexto.datosConComa(nuevas) { try entorno.archivos.escribir(d, en: url, atomico: false) }
    }
}

// C#: Views/Ported/RentabilidadFrmViewModel.cs (10 §83; CU-C20, CU-C21). Todo el cálculo masivo en `Float` (T3).
extension CasosUtilidades {
    static func base100Porcentajes(_ v: [[Double]]) throws(ErrorDominio) -> [[Float]] {
        try Porcentajes(valores: v).valoresBase100(numeroPartidos: 14)
    }

    /// CU-C20 «Valorar columna»: premio estimado (`Math.Round` en `Double`, sin tope), probabilidad real y EM (`Float`).
    public static func valorarColumnaRentabilidad(
        _ columna: String, apostados: [[Double]], reales: [[Double]], recaudacion: Double, lae: ConfigLAE, cultura: Cultura
    ) throws(ErrorDominio) -> (premio: String, probabilidad: String, esperanza: String) {
        let premioDe14 = Float(lae.precioApuesta) * Float(lae.porcentaje14) / 100
        let tope = Float(recaudacion) * Float(lae.porcentaje14) / 100
        let (pa, pr) = (try base100Porcentajes(apostados), try base100Porcentajes(reales))
        var (pA, pR): (Float, Float) = (1, 1)
        for (i, s) in columna.utf16.prefix(14).enumerated() {
            let c = s == 0x31 ? 0 : s == 0x32 ? 2 : 1
            pA *= pa[i][c]
            pR *= pr[i][c]
        }
        let esperanza = min(premioDe14 / pA, tope) * pR
        let premio = FormatoNET.round(lae.precioApuesta * lae.porcentaje14 / 100 / Double(pA), decimales: 2)
        return (
            FormatoNET.toString(premio, cultura: cultura), FormatoNET.toString(pR, cultura: cultura),
            FormatoNET.toString(esperanza, cultura: cultura)
        )
    }

    /// CU-C21 `Calcular`: EM de cada columna (las 14 triples o las del fichero), ordenadas por EM descendente si se pide
    /// (quicksort propio), y graba las de `emMin ≤ EM ≤ emMax` (con la EM tras un tabulador si se pide). Devuelve cuántas.
    @concurrent public static func calcularRentabilidad(
        entrada: URL?, salida: URL, apostados: [[Double]], reales: [[Double]], recaudacion: Double, lae: ConfigLAE,
        emMin: Double, emMax: Double, ordenar: Bool, conEM: Bool, numeroPartidos: Int, entorno: Entorno,
        estado: @escaping @MainActor @Sendable (String) -> Void
    ) async throws(ErrorDominio) -> Int {
        let total = 4_782_969
        await estado("Inicializando...")
        var ap = (0..<total).map { ApuestaProbable(probabilidad: 0, columna: Int32($0)) }
        var bits = [Bool](repeating: entrada == nil, count: total)
        if let entrada {
            await estado("Leyendo columnas...")
            let conv = ConvertidorDeBases(numeroPartidos: numeroPartidos)
            for c in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(entrada)) {
                let n = Int(try conv.numero(c, partidos: 14))
                guard n < total else { throw .fueraDeRango("ArchivoColumnasTexto.cs:285") }
                bits[n] = true
            }
        }
        await estado("Calculando...")
        let premioDe14 = Float(lae.precioApuesta) * Float(lae.porcentaje14) / 100
        let tope = Float(recaudacion) * Float(lae.porcentaje14) / 100
        let (pa, pr) = (try base100Porcentajes(apostados), try base100Porcentajes(reales))
        var (pA, pR): (Float, Float) = (1, 1)
        var cra = [[Float]](repeating: [0, 0], count: 14)
        var crp = cra
        for p in 0..<14 {
            pA *= pa[p][0]
            pR *= pr[p][0]
            cra[p] = [pa[p][1] / pa[p][0], pa[p][2] / pa[p][0]]
            crp[p] = [pr[p][1] / pr[p][0], pr[p][2] / pr[p][0]]
        }
        let pot = ConvertidorDeBases.potenciasDe3.map(Int.init)
        if bits[0] { ap[0].probabilidad = -(min(premioDe14 / pA, tope) * pR) } else { ap[0].probabilidad = 3e7 }
        ap.withUnsafeMutableBufferPointer { b in
            func distantes(_ probA: Float, _ probR: Float, _ indice: Int, _ desde: Int, _ nivel: Int) {
                for p in desde..<14 {
                    for z in 0..<2 {
                        let i = indice + pot[p] * (z + 1)
                        let (a, r) = (probA * cra[p][z], probR * crp[p][z])
                        if bits[i] { b[i].probabilidad = -(min(premioDe14 / a, tope) * r) }
                        if nivel < 14 { distantes(a, r, i, p + 1, nivel + 1) }
                    }
                }
            }
            distantes(pA, pR, 0, 0, 1)
        }
        if ordenar {
            await estado("Ordenando...")
            OrdenacionPropia.ordenar(&ap) { $0.probabilidad }
        }
        await estado("Grabando...")
        let (minimo, maximo) = (-emMin, -emMax)
        let conv = ConvertidorDeBases(numeroPartidos: numeroPartidos)
        var lineas: [String] = []
        for a in ap where bits[Int(a.columna)] && Double(a.probabilidad) >= maximo && Double(a.probabilidad) <= minimo {
            let col = try conv.columna(Int64(a.columna))
            lineas.append(conEM ? col + "\t" + FormatoNET.toString(-a.probabilidad, cultura: entorno.cultura) : col)
        }
        if let d = conEM ? ArchivoColumnasTexto.datosConComa(lineas) : ArchivoColumnasTexto.datos(lineas) {
            try entorno.archivos.escribir(d, en: salida, atomico: false)
        }
        return lineas.count
    }
}
