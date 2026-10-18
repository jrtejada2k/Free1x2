import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/TramificarFormViewModel.cs (10 §93; CU-C36…C38), TramificarGraficasFrmViewModel.cs (§94; CU-C39),
// DialogoFiltrarPorLimitesFrmViewModel.cs (08 §35) y DialogoAnalisisMultipleDeTramosFrmViewModel.cs (08 §34).

/// Lo que «Tramificar» deja para «Filtrar», «Ver columna», las gráficas y el modo acumulado.
public struct EstadoTramificar: Sendable {
    static let total = 4_782_969
    /// `Ap14T`, ordenado por |valor − LN|; vacío hasta el primer cálculo.
    public internal(set) var apuestas: [ApuestaProbEscrutada] = []
    /// `Bits` (por índice de columna): las eliminadas por el filtro. Se reserva al primer cálculo.
    var eliminadas: [Bool] = []
    /// `_escrutado`: lo invalidan las valoraciones y la columna premiada, no el LN ni el criterio de orden.
    public var escrutado = false
    /// `MaxiMin[k] = [máx, mín]` de las posiciones premiadas con 14 − k aciertos.
    var maxiMin = [[Int]](repeating: [0, total], count: 5)
    public internal(set) var tramos: [Tramo] = []

    public init() {}

    public var tramificado: Bool { apuestas.count == Self.total }

    /// `PonerMaximosiMinimos(false)`: Mín14, Máx14, Mín13 … Máx10, en posiciones 1-based.
    public var limites: [String] { (0..<5).flatMap { [String(1 + maxiMin[$0][1]), String(1 + maxiMin[$0][0])] } }
}

public struct ParametrosTramificar: Sendable {
    public var columnaInicial, columnaFinal, columnasPorTramo: Double
    public var columnaPremiada: String
    /// Importes de 14 a 10, como texto.
    public var premios: [String]
    public var valoraciones: [[Double]]
    public var ln: Double
    public var porProductos: Bool
    public var acumular: Bool
    /// Precio de la apuesta para el balance de los tramos (DT-93; el original usa 0,50 fijo).
    public var precio = 0.5

    public init(
        columnaInicial: Double, columnaFinal: Double, columnasPorTramo: Double, columnaPremiada: String, premios: [String],
        valoraciones: [[Double]], ln: Double, porProductos: Bool, acumular: Bool
    ) {
        (self.columnaInicial, self.columnaFinal, self.columnasPorTramo) = (columnaInicial, columnaFinal, columnasPorTramo)
        (self.columnaPremiada, self.premios, self.valoraciones) = (columnaPremiada, premios, valoraciones)
        (self.ln, self.porProductos, self.acumular) = (ln, porProductos, acumular)
    }
}

/// Fila de «Filtrar por límites» (`FilaLimite`).
public struct FilaLimite: Sendable, Identifiable {
    public let id: Int
    public let etiqueta: String
    public let posInicial: Int
    public let posFinal: Int
    public var difMin: Double
    public var difMax: Double

    public var rango: String { "\(posInicial) a \(posFinal)" }
}

/// `Combinacion` de la lista «Una combinación por jornada».
public struct CombinacionJornada: Sendable, Hashable, Identifiable {
    public let id = UUID()
    public let temporada: String
    public let jornada: String
    public let ruta: String

    public init(temporada: String, jornada: String, ruta: String) {
        (self.temporada, self.jornada, self.ruta) = (temporada, jornada, ruta)
    }
}

extension CasosUtilidades {
    /// `ToInt` de los límites: vacío → 0; sin «%»; entero o `(int)` del doble; si no, 0.
    static func enteroLimite(_ s: String, _ cultura: Cultura) -> Int {
        let t = FormatoNET.recortar(s).replacingOccurrences(of: "%", with: "")
        return Int(FormatoNET.castInt32(FormatoNET.tryParseDouble(t, cultura: cultura) ?? 0))
    }

    /// CU-C36 `Tramifica`: escruta y ordena (si hace falta) y reparte en tramos; `aviso` recibe los estados del C#.
    @concurrent public static func tramificar(
        _ estado: EstadoTramificar, _ p: ParametrosTramificar, cultura: Cultura, aviso: @Sendable (String) -> Void
    ) async throws(ErrorDominio) -> EstadoTramificar {
        let n = EstadoTramificar.total
        var e = estado
        if e.eliminadas.count != n { e.eliminadas = [Bool](repeating: false, count: n) }
        if p.acumular { e.escrutado = false } else { e.maxiMin = [[Int]](repeating: [0, n], count: 5) }
        // `OrdenaPorProductos` / `OrdenaPorSumas`: la resta en `Float` y luego a `Double`.
        let v: [[Float]] =
            p.porProductos
            ? try Porcentajes(valores: p.valoraciones).valoresNeperianos(numeroPartidos: p.valoraciones.count)
            : p.valoraciones.map { $0.map(Float.init) }
        var prob: Float = 0
        var cr = [[Double]](repeating: [0, 0, 0], count: 14)
        for i in 0..<14 {
            let f = try v.leer(i, "TramificarFormViewModel.cs:OrdenaPorProductos")
            prob += f[0]
            (cr[i][1], cr[i][2]) = (Double(f[1] - f[0]), Double(f[2] - f[0]))
        }
        if !e.escrutado {
            aviso("Escrutando ...")
            var ap = (0..<n).map { ApuestaProbEscrutada(probabilidad: 0, columna: Int32($0), aciertos: 0) }
            let num = Int(try ConvertidorDeBases(numeroPartidos: 14).numero(p.columnaPremiada))
            _ = try ap.leer(num, "TramificarFormViewModel.cs:Escrutar")
            ap[num].aciertos = 14
            func distancia1(_ ini: Int, _ desde: Int, _ d: Int) {
                for partido in desde..<14 {
                    let sig = (ini / potencias3[partido]) % 3
                    for z in 0..<3 where z != sig {
                        let idx = ini + potencias3[partido] * (z - sig)
                        ap[idx].aciertos = UInt8(14 - d)
                        if d < 4 { distancia1(idx, partido + 1, d + 1) }
                    }
                }
            }
            distancia1(num, 0, 1)
            e.escrutado = true
            aviso("Calculando probabilidades ...")
            func distantes(_ p0: Double, _ ini: Int, _ desde: Int) {
                for partido in desde..<14 {
                    for z in 1...2 {
                        let idx = ini + potencias3[partido] * z
                        let pr = p0 + cr[partido][z]
                        ap[idx].probabilidad = abs(pr - p.ln)
                        distantes(pr, idx, partido + 1)
                    }
                }
            }
            distantes(Double(prob), 0, 0)
            ap[0].probabilidad = abs(Double(prob) - p.ln)
            aviso("Ordenando apuestas ...")
            OrdenacionPropia.ordenar(&ap) { $0.probabilidad }
            e.apuestas = ap
        }
        aviso("Tramificando apuestas ...")
        let premios = p.premios.map { parseEsp($0, cultura) }
        let (ini, fin) = (Int(try FormatoNET.convertToInt32(p.columnaInicial)), Int(try FormatoNET.convertToInt32(p.columnaFinal)))
        let inc = Int(try FormatoNET.convertToInt32(p.columnasPorTramo))
        // El C# se queda en un bucle infinito.
        guard inc > 0 || ini >= fin else { throw ErrorDominio(.general, "El nº de columnas por tramo debe ser mayor que 0.") }
        let previos = p.acumular ? e.tramos : []
        var tramos: [Tramo] = []
        var acum = 0.0
        for nr in stride(from: ini, to: fin, by: inc) {
            var tr = Tramo(premios: premios)
            var (c, c2) = (0, 0)
            tr.valorIzquierda = nr
            var aciertos = [0, 0, 0, 0, 0]
            var nc = nr
            while Double(nc) < Double(nr) + p.columnasPorTramo && nc <= n - 1 {
                let a = try e.apuestas.leer(nc, "TramificarFormViewModel.cs:TramificarTramos")
                if !p.acumular { acum = -a.probabilidad }
                if !e.eliminadas[Int(a.columna)] {
                    if p.acumular { acum = -a.probabilidad }
                    c += 1
                    if a.aciertos > 9 {
                        let k = 14 - Int(a.aciertos)
                        aciertos[k] += 1
                        tr.columnasPremiadas += 1
                        e.maxiMin[k] = [max(e.maxiMin[k][0], nc), min(e.maxiMin[k][1], nc)]
                    }
                } else {
                    c2 += 1
                }
                nc += 1
            }
            tr.probAcumulada = acum
            tr.numColumnasTramo = c
            tr.valorDerecha += tr.valorIzquierda + c + c2 - 1
            tr.ponerAciertos(aciertos)
            tr.calculaTotalImportePremios(precio: p.precio)
            if tramos.count < previos.count { tr.addTramo(previos[tramos.count]) }
            tr.numeroDeTramo = tramos.count + 1
            tramos.append(tr)
        }
        e.tramos = tramos
        aviso("Finalizado")
        return e
    }

    /// `TramoFilaViewModel`: Nº, Col. Inf., Col. Sup., Nº Cols., Prob. Max., 14…10, NºPremios, Imp.Premios, Ingresos-Gastos.
    public static func filaTramo(_ t: Tramo, cultura: Cultura) -> [String] {
        let d = { (x: Double) in FormatoNET.toString(x, cultura: cultura) }
        return [t.numeroDeTramo, t.valorIzquierda, t.valorDerecha, t.numColumnasTramo].map(String.init) + [d(t.probAcumulada)]
            + [t.p14, t.p13, t.p12, t.p11, t.p10, t.columnasPremiadas].map(String.init) + [d(t.totalImportePremios), d(t.balance)]
    }

    /// `Filtrar` antes del diálogo: nada eliminado y `ObtenerExtremos` (difs a 0) con los límites Mín14, Máx14 … Máx10.
    public static func extremosTramificar(_ e: inout EstadoTramificar, limites: [String], cultura: Cultura) -> [[Int]] {
        e.eliminadas = [Bool](repeating: false, count: EstadoTramificar.total)
        let l = (0..<10).map { enteroLimite($0 < limites.count ? limites[$0] : "", cultura) }
        let (min14, max14, min13, max13, min12, max12, min11, max11, min10, max10) =
            (l[0], l[1], l[2], l[3], l[4], l[5], l[6], l[7], l[8], l[9])
        return [
            [0, min10 - 1], [min10 - 1, min11 - 1], [min11 - 1, min12 - 1], [min12 - 1, min13 - 1], [min13 - 1, min14 - 1],
            [max14, max13], [max13, max12], [max12, max11], [max11, max10], [max10, EstadoTramificar.total - 1],
        ].map { $0 + [0, 0] }
    }

    /// CU-C37 `EjecutarFiltro`: elimina por tramos de posición y vecinos hasta `difMax`; devuelve las que quedan.
    @concurrent public static func filtrarTramos(
        _ estado: EstadoTramificar, extremos: [[Int]]
    ) async throws(ErrorDominio) -> (estado: EstadoTramificar, quedan: Int) {
        var e = estado
        func eliminar(_ ini: Int, _ desde: Int, _ d: Int, _ hasta: Int) {
            for partido in desde..<14 {
                let sig = (ini / potencias3[partido]) % 3
                for z in 0..<3 where z != sig {
                    let idx = ini + potencias3[partido] * (z - sig)
                    e.eliminadas[idx] = true
                    if d < hasta { eliminar(idx, partido + 1, d + 1, hasta) }
                }
            }
        }
        for var x in extremos {
            if x[0] > 0 { x[0] -= 1 }
            for i in stride(from: x[0], to: x[1], by: 1) {
                let c = Int(try e.apuestas.leer(i, "TramificarFormViewModel.cs:EjecutarFiltro").columna)
                if x[2] == 0 { e.eliminadas[c] = true }
                if x[3] > 0 { eliminar(c, 0, 1, x[3]) }
            }
        }
        return (e, e.eliminadas.count { !$0 })
    }

    /// «Grabar filtro»: las que quedan, por índice ascendente.
    @concurrent public static func columnasFiltradas(_ e: EstadoTramificar) async throws(ErrorDominio) -> [String] {
        let conv = ConvertidorDeBases(numeroPartidos: 14)
        var r: [String] = []
        for (nr, fuera) in e.eliminadas.enumerated() where !fuera { r.append(try conv.columna(Int64(nr))) }
        return r
    }

    /// «Siguiente» (`hacia > 0`) y «Anterior»: posición 1-based de la próxima no eliminada con ≥ `aciertos`; el
    /// «Anterior» no mira la posición 0.
    public static func moverTramificar(_ e: EstadoTramificar, desde posicion: Int, aciertos: Int, hacia: Int) -> Int? {
        guard e.tramificado else { return nil }
        let a = e.apuestas
        var i = hacia > 0 ? max(0, posicion) : min(a.count - 1, posicion - 2)
        while hacia > 0 ? i < a.count : i > 0 {
            if !e.eliminadas[Int(a[i].columna)] && Int(a[i].aciertos) >= aciertos { return i + 1 }
            i += hacia > 0 ? 1 : -1
        }
        return nil
    }

    /// «Buscar columna»: posición 1-based de la columna.
    public static func buscarTramificar(_ e: EstadoTramificar, columna: String) throws(ErrorDominio) -> Int? {
        let num = Int32(try ConvertidorDeBases(numeroPartidos: 14).numero(columna))
        return e.apuestas.firstIndex { $0.columna == num }.map { $0 + 1 }
    }

    /// `ActualizarColumnaEnPosicion`: columna, valoración (−|valor − LN|) y aciertos contra la premiada, signo a signo.
    public static func columnaTramificar(
        _ e: EstadoTramificar, posicion: Int, premiada: String, cultura: Cultura
    ) -> (columna: String, valoracion: String, aciertos: String)? {
        guard e.apuestas.indices.contains(posicion - 1) else { return nil }
        let a = e.apuestas[posicion - 1]
        guard let col = try? ConvertidorDeBases(numeroPartidos: 14).columna(Int64(a.columna)) else { return nil }
        let aciertos = zip(premiada.utf16, col.utf16).count { $0 == $1 }
        return (col, FormatoNET.toString(-a.probabilidad, cultura: cultura), String(aciertos))
    }

    /// «Leer límites»: 10 líneas; las que falten, vacías.
    public static func leerLimitesTramificar(_ datos: Data) -> [String] {
        let l = TextoNET.lineas(datos)
        return (0..<10).map { $0 < l.count ? l[$0] : "" }
    }

    /// CU-C38 `GuardarJornadaLAE`: sustituye la línea de esa temporada y jornada en `Jornadas/InfoJornadasLAE.txt` o la
    /// añade; importes ÷ precio con coma decimal; recaudación vacía = la de Configuración. El original usa 0,50 y 14 000 000
    /// fijos; desde DT-93, el precio y la recaudación de Configuración ▸ Parámetros LAE (como Escrutinio y Estimación).
    public static func guardarJornadaTramificar(
        columna: String, temporada: String, temporadaFin: String, jornada: Double, recaudacion: String, premios: [String],
        lae: ConfigLAE, entorno: Entorno
    ) throws(ErrorDominio) {
        let c = entorno.cultura
        let j = String(FormatoNET.castInt32(jornada))
        let (jor, temp) = (String(repeating: "0", count: max(0, 2 - j.count)) + j, temporada + "/" + temporadaFin)
        let f = { (v: Double) in FormatoNET.toString(v / lae.precioApuesta, cultura: c).replacingOccurrences(of: ".", with: ",") }
        let rec = recaudacion.isEmpty ? lae.recaudacion : parseEsp(recaudacion, c)
        let nueva = ([columna, temp, jor, f(rec)] + premios.map { f(parseEsp($0, c)) }).joined(separator: "\t")
        let url = rutaInfoJornadas(entorno)
        var lineas: [String] = []
        var existe = false
        if entorno.archivos.existe(url) {
            for l in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url)) {
                let v = l.components(separatedBy: "\t")
                let es = v.count > 2 && v[1] == temp && v[2] == jor
                existe = existe || es
                lineas.append(es ? nueva : l)
            }
        }
        if !existe { lineas.append(nueva) }
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(TextoNET.codificarLineas(lineas), en: url, atomico: false)
    }

    // MARK: - Gráficas (94)

    public static let curvasTramos = [
        "Probabilidad acumulada", "14 aciertos", "13 aciertos", "12 aciertos", "11 aciertos", "10 aciertos",
        "Nº de aciertos / columnas premiadas", "Importe de premios", "Balance (ingresos - gastos)",
    ]

    /// CU-C39: x = nº de tramo − 1; y truncado a entero.
    public static func puntosCurvaTramos(_ tramos: [Tramo], curva: String) -> [(x: Int, y: Int)] {
        tramos.map { t in
            let y: Int =
                switch curva {
                case "Probabilidad acumulada": Int(FormatoNET.castInt32(35 + t.probAcumulada))
                case "14 aciertos": t.p14
                case "13 aciertos": t.p13
                case "12 aciertos": t.p12
                case "11 aciertos": t.p11
                case "10 aciertos": t.p10
                case "Nº de aciertos / columnas premiadas": t.columnasPremiadas
                case "Importe de premios": Int(FormatoNET.castInt32(t.totalImportePremios))
                case "Balance (ingresos - gastos)": Int(FormatoNET.castInt32(t.balance))
                default: 0
                }
            return (t.numeroDeTramo - 1, y)
        }
    }

    // MARK: - Filtrar por límites (35)

    static let etiquetasLimites = [
        "< 10", "de 10 a < 11", "de 11 a < 12", "de 12 a < 13", "de 13 a < 14", "de < 13 a 12", "de < 12 a 11", "de < 11 a 10",
        "< 10", "fila 10",
    ]

    static func coherenciar(_ e: inout [[Int]]) {
        for i in 0..<9 where e[i + 1][0] < e[i][0] + 1 { e[i + 1][0] = e[i][0] + 1 }
    }

    /// Filas del diálogo: sin extremos, las difs por defecto y posiciones a 0; con ellos (`Inicializar`), coherentes y la
    /// posición inicial + 1 salvo en la primera.
    public static func filasLimites(_ extremos: [[Int]]?) -> [FilaLimite] {
        guard var e = extremos else {
            let difs: [(Double, Double)] = [(0, 4), (1, 3), (1, 2), (1, 1), (1, 0), (1, 0), (1, 1), (1, 2), (0, 0), (0, 0)]
            return (0..<10).map {
                FilaLimite(id: $0, etiqueta: etiquetasLimites[$0], posInicial: 0, posFinal: 0, difMin: difs[$0].0, difMax: difs[$0].1)
            }
        }
        coherenciar(&e)
        return (0..<10).map { i in
            FilaLimite(
                id: i, etiqueta: etiquetasLimites[i], posInicial: i == 0 ? e[i][0] : e[i][0] + 1, posFinal: e[i][1],
                difMin: Double(e[i][2]), difMax: Double(e[i][3]))
        }
    }

    /// «Aceptar»: la fila 10 copia la 9 (DEC-816) y se vuelve a hacer coherente.
    public static func aceptarLimites(_ filas: [FilaLimite]) -> [[Int]] {
        var e = filas.map { [$0.posInicial, $0.posFinal, Int(FormatoNET.castInt32($0.difMin)), Int(FormatoNET.castInt32($0.difMax))] }
        e[9] = e[8]
        coherenciar(&e)
        return e
    }

    // MARK: - Análisis múltiple (34)

    /// `CargarListaDeTemporadas`: temporadas consecutivas distintas; `nil` si alguna línea no tiene 44 campos.
    public static func temporadasValoraciones(_ url: URL, entorno: Entorno) -> [String]? {
        guard let d = try? entorno.archivos.leer(url) else { return nil }
        var r: [String] = []
        for l in ArchivoColumnasTexto.columnas(d) {
            let v = l.components(separatedBy: "\t")
            guard v.count == 44 else { return nil }
            if v[0] != (r.last ?? "") { r.append(v[0]) }
        }
        return r
    }

    /// `EsFicheroDeColumnas`: la primera línea, recortada, empieza por 14 signos de «1xX2».
    public static func esFicheroDeColumnas(_ url: URL, entorno: Entorno) -> Bool {
        guard let d = try? entorno.archivos.leer(url), let l = TextoNET.lineas(d).first else { return false }
        let u = Array(FormatoNET.recortar(l).utf16)
        return u.count >= 14 && u.prefix(14).allSatisfy { "1xX2".utf16.contains($0) }
    }

    /// «Leer» `.lst`: «temporada(9) jornada(2) ruta», en mayúsculas; si una línea es corta, lo leído y el error.
    public static func leerListaCombinaciones(_ datos: Data) -> (filas: [CombinacionJornada], error: Bool) {
        var r: [CombinacionJornada] = []
        for l in ArchivoColumnasTexto.columnas(datos) {
            let u = Array(l.utf16)
            guard u.count >= 13 else { return (r, true) }
            let s = { (a: Int, b: Int) in String(decoding: u[a..<b], as: UTF16.self) }
            r.append(CombinacionJornada(temporada: s(0, 9), jornada: s(10, 12), ruta: s(13, u.count)))
        }
        return (r, false)
    }

    /// «Guardar» `.lst` (`GuardarCols` quita las comas).
    public static func datosListaCombinaciones(_ filas: [CombinacionJornada]) -> Data {
        TextoNET.codificarLineas(filas.map { ($0.temporada + " " + $0.jornada + " " + $0.ruta).replacingOccurrences(of: ",", with: "") })
    }
}
