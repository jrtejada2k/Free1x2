import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/VSignosFrmViewModel.cs (10 §97), DifColsViewModel.cs (09 §41) y SubirCategoriaFrmViewModel.cs
// (10 §92; CU-C33…C35).

/// «Valoración de signos»: recuentos por partido (1, X, 2) de las columnas que llegan a la aspiración.
public struct ResultadoVSignos: Sendable {
    public var procesadas = 0
    public var recuentos: [[Int]]
    /// `premis[noPartidos - aciertos]` para 10…noPartidos aciertos.
    public var premios: [Int]
    public var aceptadas: [String] = []

    public init(partidos: Int) {
        recuentos = Array(repeating: [0, 0, 0], count: partidos)
        premios = Array(repeating: 0, count: max(1, partidos - 9))
    }
}

extension CasosUtilidades {
    /// `TimeSpan` + «0000000000» cortado a 10 (en DifCols el C# corta sin rellenar; solo difiere con 0 ticks).
    public static func tiempoCorto(_ segundos: TimeInterval) -> String {
        String((FormatoNET.toStringTimeSpan(segundos) + "0000000000").prefix(10))
    }

    /// `AbrirFicheroAsync`: `ObtenNumSignos`; menos de 10 ⇒ 15.
    public static func partidosVSignos(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> Int {
        let n = ArchivoColumnasTexto.numSignos(try entorno.archivos.leer(url))
        return n < 10 ? 15 : n
    }

    /// `AcCb`: acierta el signo igual o el comodín (todo lo que no es 1, X ni 2). El último partido se compara con el
    /// último carácter de la columna y, con pleno, solo cuenta si acertaron todos los anteriores.
    static func aciertosVSignos(_ c: [UInt16], _ part: [UInt16], pleno: Bool) -> Int {
        func acierta(_ p: UInt16, _ s: UInt16) -> Bool { p == s || (p != 0x31 && p != 0x58 && p != 0x32) }
        let n = part.count
        var r = 0
        for i in 0..<max(0, n - 1) where acierta(part[i], c[i]) { r += 1 }
        if (!pleno || r == n - 1), let p = part.last, let s = c.last, acierta(p, s) { r += 1 }
        return r
    }

    /// `CalcularAsync`: cada línea `Trim().ToUpper()` de al menos `ganadora.count` signos. `ganadora`: un carácter por
    /// partido (vacío = «-»).
    @concurrent public static func analizarSignos(
        _ url: URL, ganadora: [String], aspiracion: Int, pleno: Bool, entorno: Entorno
    ) async throws(ErrorDominio) -> ResultadoVSignos {
        let n = ganadora.count
        let part = ganadora.map { $0.utf16.first ?? 0x2D }
        var r = ResultadoVSignos(partidos: n)
        for l in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url)) {
            let c = Array(l.utf16)
            guard c.count >= n else { continue }
            r.procesadas += 1
            let a = aciertosVSignos(c, part, pleno: pleno)
            if a >= aspiracion {
                for i in 0..<n { r.recuentos[i][c[i] == 0x31 ? 0 : c[i] == 0x32 ? 2 : 1] += 1 }
                r.aceptadas.append(l)
            }
            if a > 9 && a <= n { r.premios[n - a] += 1 }
        }
        return r
    }

    public static func textoPremiosRemanentes(_ premios: [Int]) -> String {
        "Premios remanentes: " + premios.map(String.init).joined(separator: "-")
    }

    /// `GrabarAsync` en modo porcentaje: `"v1, vx, v2"` por partido, invariante `f0`/`f2` (suma 0 ⇒ 1).
    public static func valoracionDeSignos(_ recuentos: [[Int]], decimales: Bool) -> [String] {
        recuentos.map { v in
            let suma = Double(v.reduce(0, +))
            return v.map {
                FormatoNET.toStringFijo(Double($0) / (suma == 0 ? 1 : suma) * 1e2, decimales: decimales ? 2 : 0, cultura: .invariante)
            }.joined(separator: ", ")
        }
    }
}

// MARK: - Diferencias entre columnas (DifCols)

extension CasosUtilidades {
    /// `VerColumna`: exactamente 14 signos de «124xXF» (x/X ⇒ 4), empaquetados en grupos de 3 bits (1 = 001, 2 = 010,
    /// 4 = 100, F = 111; partido i en los bits 3i…3i+2). `nil` = «columna errónea».
    static func columnaDif(_ s: String) -> UInt64? {
        let u = Array(s.utf16)
        guard u.count == 14 else { return nil }
        var c: UInt64 = 0
        for (i, x) in u.enumerated() {
            let v: UInt64
            switch x {
            case 0x31: v = 1
            case 0x32: v = 2
            case 0x34, 0x78, 0x58: v = 4
            case 0x46: v = 7
            default: return nil
            }
            c |= v << (3 * i)
        }
        return c
    }

    /// Condición «columna base».
    public static func condicionDif(_ s: String) throws(ErrorDominio) -> UInt64 {
        guard let c = columnaDif(s) else { throw ErrorDominio(.general, "columna errónea") }
        return c
    }

    /// Condiciones de fichero (`File.ReadAllLines`): las líneas vacías se saltan.
    public static func condicionesDif(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> [UInt64] {
        var r: [UInt64] = []
        for l in TextoNET.lineas(try entorno.archivos.leer(url)) where !l.isEmpty { r.append(try condicionDif(l)) }
        return r
    }

    /// `PreparaDif`: `a[-b]` por trozo (recortados a 0…14); admite a…b. Con a > b la máscara crece (se replica).
    public static func mascaraDif(_ rangos: String) throws(ErrorDominio) -> [Bool] {
        var dif = [Bool](repeating: false, count: 15)
        for trozo in rangos.components(separatedBy: ",") {
            let mgg = trozo.components(separatedBy: "-")
            func valor(_ s: String) throws(ErrorDominio) -> Int { min(14, max(0, Int(try FormatoNET.convertToInt32(s)))) }
            let a = try valor(mgg[0])
            let b = mgg.count == 2 ? try valor(mgg[1]) : a
            dif = Array(dif[0..<a]) + [Bool](repeating: true, count: max(0, b - a + 1)) + Array(dif[(b + 1)..<15])
        }
        return dif
    }

    /// `Compara`: el C# cuenta `(x & y) == '0'`: solo 1, 2 y 4 distintos; «F» (la fija de cualquiera de las dos) nunca
    /// cuenta. Con los grupos de 3 bits es «AND del grupo = 0».
    static func diferencias(_ x: UInt64, _ y: UInt64) -> Int {
        let m = x & y
        return 14 - ((m | m >> 1 | m >> 2) & 0o11111111111111).nonzeroBitCount
    }

    static func textoDif(_ c: UInt64) -> String {
        String((0..<14).map { i -> Character in ["F", "1", "2", "F", "4", "F", "F", "F"][Int(c >> (3 * i) & 7)] })
    }

    /// `Procesar`: universo de triples (orden 1, 2, 4; el primer partido el más externo) o líneas de `entrada` (vacías
    /// fuera; inválida ⇒ «columna errónea»). Acepta si cumple las diferencias con las condiciones y con las ya
    /// aceptadas; se guarda con «F» en las fijas. `nil` = cancelado (la pantalla conserva las anteriores).
    @concurrent public static func diferenciasColumnas(
        condiciones: [UInt64], difBase: [Bool], difResultado: [Bool], entrada: URL?, entorno: Entorno
    ) async throws(ErrorDominio) -> [String]? {
        var aceptadas: [UInt64] = []
        func proceso(_ c: UInt64) {
            for k in condiciones where !difBase[diferencias(c, k)] { return }
            for k in aceptadas where !difResultado[diferencias(c, k)] { return }
            aceptadas.append(c)
        }
        if let entrada {
            for l in TextoNET.lineas(try entorno.archivos.leer(entrada)) {
                if Task.isCancelled { return nil }
                if l.isEmpty { continue }
                guard let c = columnaDif(l) else { throw ErrorDominio(.operacionInvalida, "columna errónea") }
                proceso(c)
            }
        } else {
            // ponytail: en vez de la recursión, n = alto·3^7 + bajo (mismo orden) con una tabla por cada 7 partidos.
            func tabla(_ desde: Int) -> [UInt64] {
                (0..<2187).map { n in
                    var (c, x): (UInt64, Int) = (0, n)
                    for i in stride(from: desde + 6, through: desde, by: -1) {
                        c |= UInt64(1 << (x % 3)) << (3 * i)
                        x /= 3
                    }
                    return c
                }
            }
            let (alto, bajo) = (tabla(0), tabla(7))
            for a in alto {
                if Task.isCancelled { return nil }
                for b in bajo { proceso(a | b) }
            }
        }
        return aceptadas.map(textoDif)
    }

    /// `Grabar`: «4» ⇒ «X», hasta `limite` (0 ⇒ 4 782 969).
    public static func grabarDiferencias(_ aceptadas: [String], limite: Int, en url: URL, entorno: Entorno) throws(ErrorDominio) {
        let l = aceptadas.prefix(limite > 0 ? limite : 4_782_969).map { $0.replacingOccurrences(of: "4", with: "X") }
        try entorno.archivos.escribir(TextoNET.codificarLineas(l), en: url, atomico: false)
    }
}

// MARK: - Subir categoría

extension CasosUtilidades {
    /// CU-C33: motor con las columnas del fichero (`I=`) y su nº de signos.
    @concurrent public static func prepararSubirCategoria(
        _ url: URL, entorno: Entorno
    ) async throws(ErrorDominio) -> (motor: SubidaCategoria, signos: Int) {
        let d = try entorno.archivos.leer(url)
        return (try SubidaCategoria(lineas: ArchivoColumnasTexto.columnas(d)), ArchivoColumnasTexto.numSignos(d))
    }

    /// CU-C34: el motor vuelve también con error (como el C#, conserva el estado que dejó la llamada).
    @concurrent public static func calcularSubirCategoria(
        _ motor: SubidaCategoria, involucrados: [Bool], niveles: [Bool], seguidos: Int, externas: URL?, signos: Int,
        entorno: Entorno
    ) async -> (motor: SubidaCategoria, error: ErrorDominio?) {
        var m = motor
        do throws(ErrorDominio) {
            var ext: [String]?
            if let externas { ext = ArchivoColumnasTexto.columnas(try entorno.archivos.leer(externas)) }
            try m.calcular(involucrados: involucrados, niveles: niveles, seguidos: seguidos, externas: ext, numeroPartidos: signos)
            return (m, nil)
        } catch {
            return (m, error)
        }
    }

    /// CU-C35: en orden de índice; sin columnas no se crea el fichero.
    @concurrent public static func grabarSubirCategoria(_ motor: SubidaCategoria, en url: URL, entorno: Entorno) async throws(ErrorDominio)
    {
        if let d = ArchivoColumnasTexto.datos(try motor.lineas()) { try entorno.archivos.escribir(d, en: url, atomico: false) }
    }
}
