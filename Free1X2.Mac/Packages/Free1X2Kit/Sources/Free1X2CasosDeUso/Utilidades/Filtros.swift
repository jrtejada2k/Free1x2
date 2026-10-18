import Foundation
import Free1X2Base
import Free1X2Formatos

// C#: Views/Ported/FiltroPorcenJBViewModel.cs (09 pantalla 50; CU-B15…B17) y DiFiltrosViewModel.cs (08 pantalla 33;
// CU-A80…A83). Toda la lógica vive en los VM.

/// `RecuperaPantalla` del separador de Juan Bellas: por banda, el dígito (bits 1 = «1», 2 = «2», 4 = «X») de cada partido.
public struct BandasJB: Sendable {
    let cps: [[Int]]
    /// `_scps`: 7 cadenas de 14 dígitos «0»…«7».
    let scps: [[UInt16]]
    let rangos: [(minimo: Int, maximo: Int)]
}

extension CasosUtilidades {
    /// `nil` = «error en datos de entrada» (rango sin `a-b` enteros o límites decrecientes). Porcentajes y límites se truncan.
    public static func bandasJB(porcentajes: [[Double]], limites: [Double], rangos textos: [String]) -> BandasJB? {
        var rangos: [(Int, Int)] = []
        for t in textos {
            let a = t.components(separatedBy: "-")
            guard a.count >= 2, let x = try? FormatoNET.convertToInt32(a[0]), let y = try? FormatoNET.convertToInt32(a[1]) else {
                return nil
            }
            rangos.append((Int(x), Int(y)))
        }
        let lims = limites.map { Int(FormatoNET.castInt32($0)) }
        guard zip(lims, lims.dropFirst()).allSatisfy({ $1 >= $0 }) else { return nil }
        var cps = [[Int]](repeating: [Int](repeating: 0, count: 7), count: 14)
        for p in 0..<14 {
            for s in 0..<3 {
                let v = Int(FormatoNET.castInt32(porcentajes[p][s]))
                cps[p][lims.firstIndex(where: { v <= $0 }) ?? 6] += s == 0 ? 1 : s == 2 ? 2 : 4
            }
        }
        let scps = (0..<7).map { b in (0..<14).map { UInt16(0x30 + cps[$0][b]) } }
        return BandasJB(cps: cps, scps: scps, rangos: rangos)
    }

    /// Aciertos por banda: dígito ≠ «0» y `(dígito & carácter) > '0'`.
    static func aciertosJB(_ b: BandasJB, _ col: [UInt16]) -> [Int] {
        b.scps.map { banda in (0..<14).count(where: { banda[$0] != 0x30 && banda[$0] & col[$0] > 0x30 }) }
    }

    /// `Valida`: cada banda en su rango y el recorrido (máx − mín) en el 8.º.
    static func validaJB(_ b: BandasJB, _ col: [UInt16]) -> Bool {
        let na = aciertosJB(b, col)
        for (n, r) in zip(na, b.rangos) where n < r.minimo || n > r.maximo { return false }
        let rec = (na.max() ?? 0) - min(98, na.min() ?? 98)
        return rec >= b.rangos[7].minimo && rec <= b.rangos[7].maximo
    }

    /// `CalcularAsync`: líneas `Trim().ToUpper()` no vacías; sin repetir (`S2n`); X ⇒ 4 al validar y guardar. Una línea de
    /// menos de 14 corta y vuelve en `corta` («error longitud=…»).
    @concurrent public static func filtrarJB(
        _ url: URL, bandas: BandasJB, entorno: Entorno
    ) async throws(ErrorDominio) -> (procesadas: Int, admitidas: [String], corta: String?) {
        var vistas = Set<Int>()
        var (procesadas, admitidas) = (0, [String]())
        for l in TextoNET.lineas(try entorno.archivos.leer(url)) {
            if Task.isCancelled { break }
            let col = FormatoNET.mayusculas(FormatoNET.recortar(l))
            if col.isEmpty { continue }
            procesadas += 1
            if col.utf16.count < 14 { return (procesadas, admitidas, col) }
            if vistas.insert(try FiltroPorGrupos.s1n(col)).inserted {
                let c = col.replacingOccurrences(of: "X", with: "4")
                if validaJB(bandas, Array(c.utf16)) { admitidas.append(c) }
            }
        }
        return (procesadas, admitidas, nil)
    }

    /// `Analizar`: aciertos por banda y recorrido de la ganadora (x/X ⇒ 4); `nil` si tiene menos de 14 caracteres.
    public static func analizarJB(_ b: BandasJB, columna: String) -> (bandas: [Int], recorrido: Int)? {
        let c = Array(columna.replacingOccurrences(of: "x", with: "4").replacingOccurrences(of: "X", with: "4").utf16)
        guard c.count >= 14 else { return nil }
        let na = aciertosJB(b, c)
        return (na, max(0, na.max() ?? 0) - min(98, na.min() ?? 98))
    }

    /// `ExportarAsync`: 7 líneas, por partido el `Cambia` del valor de la banda.
    public static func exportarJB(_ b: BandasJB) -> [String] {
        let cambia = ["-", "1", "2", "12", "X", "1X", "X2", "1X2"]
        return (0..<7).map { banda in (0..<14).map { cambia[min(7, b.cps[$0][banda])] }.joined(separator: ",") }
    }

    /// `.jb7`: 7 rangos, recorrido y los 6 límites `(int)` invariantes.
    public static func datosRangosJB(rangos: [String], limites: [Double]) -> Data {
        TextoNET.codificarLineas(rangos + [limites.map { String(FormatoNET.castInt32($0)) }.joined(separator: ",")])
    }

    /// `LeerRangosAsync`: solo lo presente; límites con `double.TryParse` de la cultura (0 si falla).
    public static func leerRangosJB(_ datos: Data, rangos: inout [String], limites: inout [Double], cultura: Cultura) {
        let l = TextoNET.lineas(datos)
        for i in 0..<min(8, l.count) { rangos[i] = l[i] }
        guard l.count >= 9 else { return }
        for (i, s) in l[8].components(separatedBy: ",").prefix(6).enumerated() {
            limites[i] = FormatoNET.tryParseDouble(s, cultura: cultura) ?? 0
        }
    }
}

// MARK: - Diferencias entre filtros

/// Una fila de «Diferencias entre filtros».
public struct FilaDiFiltros: Sendable, Hashable, Identifiable {
    public var id = UUID()
    public var ruta: String
    public var nombre: String
    public var activo = true
    /// Máscara de niveles 0…3 (solo «1» activa).
    public var difs = "1111"
    public var minimo = 1.0
    public var columnas = 0
    public var admitidas = 0
    public var acierta14 = 0
    public var aciertos13 = 0

    public init(ruta: String, nombre: String) { (self.ruta, self.nombre) = (ruta, nombre) }
}

/// Estado que el C# guarda en campos y sobrevive entre «Iniciar»: el conjunto resultante y su contador.
public struct EstadoDiFiltros: Sendable {
    public var validas = [Bool](repeating: false, count: 4_782_969)
    var ctFR = 0

    public init() {}
}

extension CasosUtilidades {
    /// `S2n` = `ConvColumnaANumero` (línea entera, partido 1 el menos significativo); fuera del espacio de 14 lanza.
    static func indiceDiFiltros(_ c: String) throws(ErrorDominio) -> Int {
        let n = Int(try ConvertidorDeBases(numeroPartidos: 14).numero(c))
        guard n >= 0 && n < 4_782_969 else { throw .fueraDeRangoLista("DiFiltrosViewModel.cs:S2n") }
        return n
    }

    /// `Valida`: sobrevive si está en el filtro (nivel 0) o si tiene al menos `minimo` columnas del filtro a distancia
    /// exacta 1, 2 o 3 (niveles marcados). ponytail: cuenta los vecinos a distancia exacta con salida temprana, como los
    /// caminos ordenados del C#.
    static func validaDiFiltros(_ n: Int, _ filtro: [Bool], _ t: [Bool], _ minimo: Int) -> Bool {
        if t[0] && filtro[n] { return true }
        guard t[1] || t[2] || t[3] else { return false }
        var cuenta = 0
        let ultimo = t.lastIndex(of: true) ?? 0
        func paso(_ x: Int, _ desde: Int, _ nivel: Int) -> Bool {
            for p in stride(from: desde, to: 14, by: 1) {
                let pw = Int(ConvertidorDeBases.potenciasDe3[p])
                let s = (x / pw) % 3
                for z in 0..<3 where z != s {
                    let y = x + pw * (z - s)
                    if t[nivel] && filtro[y] {
                        cuenta += 1
                        if cuenta >= minimo { return true }
                    }
                    if nivel < ultimo && paso(y, p + 1, nivel + 1) { return true }
                }
            }
            return false
        }
        return paso(n, 0, 1)
    }

    /// `IniciarProcesoAsync` (+ `Escrutar` si hay ganadora): la 1.ª fila activa abre el conjunto; cada siguiente descarta
    /// lo que no cumpla `Valida` con ella. Una fila que no se puede abrir se desactiva. Al cancelar queda lo hecho.
    @concurrent public static func procesoDiFiltros(
        _ entrada: [FilaDiFiltros], estado: EstadoDiFiltros, ganadora: String?, entorno: Entorno
    ) async throws(ErrorDominio) -> (filas: [FilaDiFiltros], estado: EstadoDiFiltros) {
        var filas = entrada
        var e = estado
        e.validas = [Bool](repeating: false, count: 4_782_969)
        var prm: (catorce: Int, trece: [Int])?
        if let ganadora {
            let g = try indiceDiFiltros(ganadora)
            var trece: [Int] = []
            for p in 0..<14 {
                let pw = Int(ConvertidorDeBases.potenciasDe3[p])
                let s = (g / pw) % 3
                for z in 0..<3 where z != s { trece.append(g + pw * (z - s)) }
            }
            prm = (g, trece)
        }
        var filtro = [Bool](repeating: false, count: 4_782_969)
        for nf in filas.indices {
            if Task.isCancelled { break }
            filas[nf].columnas = 0
            guard filas[nf].activo else { continue }
            guard let d = try? entorno.archivos.leer(URL(filePath: filas[nf].ruta)) else {
                entorno.registro.registrar("DiFiltrosViewModel: no se pudo abrir " + filas[nf].ruta, nivel: .error)
                filas[nf].activo = false
                continue
            }
            if nf > 0 { filtro = [Bool](repeating: false, count: 4_782_969) }
            var ct = 0
            // DM-22: los índices en varios núcleos (−1: menos de 14 signos; −2: no es una columna, y en el recorrido se
            // repite para lanzar el mismo error); el recorrido, en orden como siempre.
            let lineas = ArchivoColumnasTexto.columnas(d)
            let indices = mapaParalelo(lineas) { c -> Int in c.utf16.count < 14 ? -1 : ((try? indiceDiFiltros(c)) ?? -2) }
            for (k, i) in indices.enumerated() {
                if Task.isCancelled { break }
                ct += 1
                if i == -1 { break }
                let i = i == -2 ? try indiceDiFiltros(lineas[k]) : i
                if nf == 0 { e.validas[i] = true } else { filtro[i] = true }
            }
            filas[nf].columnas = ct
            if nf == 0 {
                e.ctFR = ct
                filas[nf].admitidas = ct
            } else {
                let minimo = min(3305, max(1, Int(FormatoNET.castInt32(filas[nf].minimo))))
                let difs = Array((filas[nf].difs.utf16.count < 4 ? filas[nf].difs + "0000" : filas[nf].difs).utf16.prefix(4))
                let t = difs.map { $0 == 0x31 }
                // DM-22: cada posición se valida sola; en varios núcleos por tramos, y las caídas se aplican en orden. Al
                // cancelar, los tramos enteros hasta el primero a medias (incluido), como en un hilo.
                let (validas, filtro) = (e.validas, filtro)
                let tramos = trozosParalelos(4_782_969, minimo: 2187 * 16)
                let caidas = await conCancelacion { cancelado in
                    enParalelo(tramos.count) { k -> (posiciones: [Int32], completo: Bool) in
                        var p: [Int32] = []
                        for n in tramos[k] {
                            if n % 2187 == 0 && cancelado.activa { return (p, false) }
                            if validas[n] && !validaDiFiltros(n, filtro, t, minimo) { p.append(Int32(n)) }
                        }
                        return (p, true)
                    }
                }
                for c in caidas {
                    for n in c.posiciones { e.validas[Int(n)] = false }
                    e.ctFR -= c.posiciones.count
                    if !c.completo { break }
                }
                filas[nf].admitidas = e.ctFR
            }
            if let prm {
                filas[nf].acierta14 = e.validas[prm.catorce] ? 1 : 0
                filas[nf].aciertos13 = prm.trece.count(where: { e.validas[$0] })
            }
        }
        return (filas, e)
    }

    /// «Grabar» (`GuardarTodasCols`): por índice ascendente con `numeroPartidos` signos; sin ninguna no se crea el fichero.
    public static func grabarDiFiltros(_ e: EstadoDiFiltros, numeroPartidos: Int, en url: URL, entorno: Entorno) throws(ErrorDominio) {
        let conv = ConvertidorDeBases(numeroPartidos: numeroPartidos)
        var l: [String] = []
        for (i, v) in e.validas.enumerated() where v { l.append(try conv.columna(Int64(i))) }
        if let d = ArchivoColumnasTexto.datos(l) { try entorno.archivos.escribir(d, en: url, atomico: false) }
    }

    /// `.lst`: `ruta;nombre;True|False;difs;mínimo entero`.
    public static func datosListaDiFiltros(_ filas: [FilaDiFiltros]) -> Data {
        TextoNET.codificarLineas(
            filas.map { "\($0.ruta);\($0.nombre);\($0.activo ? "True" : "False");\($0.difs);\(FormatoNET.castInt32($0.minimo))" })
    }

    /// `CargarLista`: líneas con al menos 5 campos; `Activo` = «True» (sin mayúsculas) o «1»; mínimo inválido ⇒ 1.
    public static func leerListaDiFiltros(_ datos: Data, cultura: Cultura) -> [FilaDiFiltros] {
        TextoNET.lineas(datos).compactMap { l in
            let a = l.components(separatedBy: ";")
            guard !l.allSatisfy(\.isWhitespace), a.count >= 5 else { return nil }
            var f = FilaDiFiltros(ruta: a[0], nombre: a[1])
            f.activo = a[2].lowercased() == "true" || a[2] == "1"
            f.difs = a[3]
            f.minimo = FormatoNET.tryParseDouble(a[4], cultura: cultura) ?? 1
            return f
        }
    }

    /// `CargarGanadoras`: `Trim()`, 14 primeros de «12xX» (hasta 3000); `erronea` = alguna inválida (se para en ella).
    public static func ganadorasDiFiltros(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> (columnas: [String], erronea: Bool) {
        var r: [String] = []
        for l in TextoNET.lineas(try entorno.archivos.leer(url)) {
            let t = Array(FormatoNET.recortar(l).utf16)
            guard t.count >= 14, t.prefix(14).allSatisfy({ "12xX".utf16.contains($0) }) else { return (r, true) }
            if r.count >= 3000 { break }
            r.append(String(decoding: t.prefix(14), as: UTF16.self))
        }
        return (r, false)
    }
}
