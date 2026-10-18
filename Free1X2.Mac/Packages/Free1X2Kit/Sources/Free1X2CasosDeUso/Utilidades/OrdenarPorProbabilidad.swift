import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/OrdenarPorProbabilidadFrmViewModel.cs (10 §76; CU-C05…C08). `Float` donde el C# usa `float` (T3).

/// Valores centrales que el VM guarda entre ejecuciones (un texto que no se lee deja el anterior).
public struct ValoresCentrales: Sendable, Hashable {
    public var ln = 0.0
    public var probabilidad = 0.0
    public var premio = 0.0
    public var acertantes = 0.0

    public init() {}
}

/// Radio activo de «Por productos».
public enum ModoCentral: Sendable, Hashable {
    case acertantes, premio, probabilidad, ln, columna
}

/// Parámetros de «Ordenar».
public struct ParametrosOrdenacion: Sendable {
    public var entrada: URL?
    public var salida: URL
    public var valoraciones: [[Double]]
    public var recaudacion = 0.0
    public var precio = 0.0
    public var porcentaje14 = 0.0
    public var maxColumnas = 4_782_969
    public var limiteAcumulada = 1.0
    public var conAcumulada = false
    public var conPremio = false
    /// 0.77.2 «Obtener intervalo»: solo se graban las columnas con el premio de 14 entre estos importes (mínimo, máximo).
    public var premio14Entre: (Double, Double)?
    /// 0 productos, 1 sumas, 2 múltiple.
    public var pestana = 0
    public var valorCentralSumas = 0.0
    public var multiplePorProbabilidad = true
    public var lnMinimo = 0.0
    public var lnMaximo = 0.0
    public var puntos = 1
    public var columnasPorTramo = 0

    public init(entrada: URL?, salida: URL, valoraciones: [[Double]]) {
        (self.entrada, self.salida, self.valoraciones) = (entrada, salida, valoraciones)
    }
}

extension CasosUtilidades {
    /// `TryParseEsp`: recortado, `.` → `,` y `es-ES`; vacío o inválido ⇒ `nil`.
    public static func parseEspOrdenar(_ texto: String) -> Double? {
        guard !CasosCondiciones.blanco(texto) else { return nil }
        return FormatoNET.tryParseDouble(FormatoNET.recortar(texto).replacingOccurrences(of: ".", with: ","), cultura: .esES)
    }

    /// CU-C05 `CalcularValorCentral`.
    public static func valorCentral(
        _ modo: ModoCentral, texto: String, valoraciones: [[Double]], recaudacion r: Double, precio: Double, porcentaje14 pct: Double,
        previo: ValoresCentrales
    ) -> ValoresCentrales {
        var v = previo
        switch modo {
        case .columna:
            v.probabilidad = 1
            for (i, s) in texto.utf16.enumerated() where i < valoraciones.count {
                v.probabilidad *= valoraciones[i][s == 0x31 ? 0 : s == 0x32 ? 2 : 1] / 100
            }
            v.ln = log(v.probabilidad)
            (v.premio, v.acertantes) = (precio * pct / v.probabilidad / 100, r / precio * v.probabilidad)
        case .ln:
            if let x = parseEspOrdenar(texto) { v.ln = x }
            v.probabilidad = exp(v.ln)
            (v.premio, v.acertantes) = (precio * pct / v.probabilidad / 100, r / precio * v.probabilidad)
        case .probabilidad:
            if let x = parseEspOrdenar(texto) { v.probabilidad = x }
            v.ln = log(v.probabilidad)
            (v.premio, v.acertantes) = (precio * pct / v.probabilidad / 100, r / precio * v.probabilidad)
        case .premio:
            if let x = parseEspOrdenar(texto) { v.premio = x }
            v.probabilidad = precio * pct / v.premio / 100
            v.ln = log(v.probabilidad)
            v.acertantes = r / precio * v.probabilidad
        case .acertantes:
            if let x = parseEspOrdenar(texto) { v.acertantes = x }
            v.premio = r * pct / v.acertantes / 100
            v.probabilidad = precio * pct / v.premio / 100
            v.ln = log(v.probabilidad)
        }
        return v
    }

    /// CU-C06…C08 `Ordenar`: productos, sumas o múltiple. Devuelve los valores centrales tal como quedan (el múltiple
    /// por probabilidad cambia `probabilidad` y `ln`).
    @concurrent public static func ordenarPorProbabilidad(
        _ p: ParametrosOrdenacion, centrales: ValoresCentrales, numeroPartidos: Int, entorno: Entorno,
        estado: @escaping @MainActor @Sendable (String) -> Void
    ) async throws(ErrorDominio) -> ValoresCentrales {
        var m = MotorOrdenacion(p: p, c: centrales, numeroPartidos: numeroPartidos, entorno: entorno)
        try await m.ejecutar(estado)
        return m.c
    }
}

/// Estado del VM durante el cálculo (`Ap14T`, `Bits`, `Admitidas`…).
struct MotorOrdenacion {
    static let total = 4_782_969
    static let centinela = Float(9e10)
    let p: ParametrosOrdenacion
    var c: ValoresCentrales
    let numeroPartidos: Int
    let entorno: Entorno
    var ap: [ApuestaProbableCentral] = []
    var bits: [Bool] = []
    var admitidas: [Bool] = []
    var numApuestas = 0
    var maxColumnas = 0
    var multiple = false
    var iter = ""
    var cr = [[Float]](repeating: [0, 0, 0], count: 14)

    init(p: ParametrosOrdenacion, c: ValoresCentrales, numeroPartidos: Int, entorno: Entorno) {
        (self.p, self.c, self.numeroPartidos, self.entorno) = (p, c, numeroPartidos, entorno)
        maxColumnas = min(p.maxColumnas, Self.total)
    }

    mutating func ejecutar(_ estado: @escaping @MainActor @Sendable (String) -> Void) async throws(ErrorDominio) {
        switch p.pestana {
        case 0: try await ordenar(productos: true, ln: nil, estado)
        case 1: try await ordenar(productos: false, ln: p.valorCentralSumas, estado)
        default:
            multiple = true
            try await multiplePuntos(estado)
        }
    }

    /// `OrdenaPorProductos` / `OrdenaPorSumas` + `Calcula14Triples` / `CalculaFichero`.
    mutating func ordenar(productos: Bool, ln sumas: Double?, _ estado: @escaping @MainActor @Sendable (String) -> Void)
        async throws(ErrorDominio)
    {
        let pv: [[Float]]
        maxColumnas = min(maxColumnas, Self.total)
        if productos {
            c.ln = c.probabilidad > 0 ? log(c.probabilidad) : -99999
            pv = try Porcentajes(valores: p.valoraciones).valoresNeperianos(numeroPartidos: 14)
        } else {
            c.ln = sumas ?? 0
            pv = (0..<14).map { i in (0..<3).map { Float(p.valoraciones[i][$0]) } }
        }
        ap = (0..<Self.total).map { ApuestaProbableCentral(probabilidad: 0, probabilidadDiferencial: 0, columna: Int32($0)) }
        var prob: Float = 0
        for i in 0..<14 {
            prob += pv[i][0]
            cr[i] = [0, pv[i][1] - pv[i][0], pv[i][2] - pv[i][0]]
        }
        let lnF = Float(c.ln)
        if let entrada = p.entrada {
            let d = try entorno.archivos.leer(entrada)
            numApuestas = ArchivoColumnasTexto.numColumnas(d)
            bits = [Bool](repeating: false, count: Self.total)
            let conv = ConvertidorDeBases(numeroPartidos: numeroPartidos)
            for col in ArchivoColumnasTexto.columnas(d) {
                let n = Int(try conv.numero(col, partidos: 14))
                guard n < Self.total else { throw .fueraDeRango("ArchivoColumnasTexto.cs:285") }
                bits[n] = true
            }
            await estado("Calculando probabilidades ...")
        } else {
            bits = [Bool](repeating: true, count: Self.total)
            numApuestas = Self.total
            await estado(iter + "Calculando probabilidades ...")
        }
        distantes(prob, lnF)
        ap[0].probabilidad = prob
        ap[0].probabilidadDiferencial = p.entrada == nil || bits[0] ? abs(prob - lnF) : Self.centinela
        await estado(iter + "Ordenando apuestas ...")
        OrdenacionPropia.ordenar(&ap) { $0.probabilidadDiferencial }
        await estado(iter + "Guardando apuestas ...")
        if multiple {
            for nr in 0..<max(0, maxColumnas) { admitidas[Int(ap[nr].columna)] = true }
        } else {
            await estado(try grabar(pestanaProductos: p.pestana == 0, admitidasMultiples: false))
        }
    }

    /// `EncontrarDistantes1`: suma acumulada por la ruta DFS; las columnas ausentes del fichero llevan el centinela.
    mutating func distantes(_ prob0: Float, _ lnF: Float) {
        let pot = ConvertidorDeBases.potenciasDe3.map(Int.init)
        let (cr, bits) = (cr, bits)
        ap.withUnsafeMutableBufferPointer { b in
            func rec(_ prob: Float, _ indice: Int, _ desde: Int, _ nivel: Int) {
                for partido in desde..<14 {
                    for z in 1...2 {
                        let i = indice + pot[partido] * z
                        let pr = prob + cr[partido][z]
                        if bits[i] {
                            b[i].columna = Int32(i)
                            b[i].probabilidadDiferencial = abs(pr - lnF)
                        } else {
                            b[i].probabilidadDiferencial = Self.centinela
                        }
                        b[i].probabilidad = pr
                        if nivel < 14 { rec(pr, i, partido + 1, nivel + 1) }
                    }
                }
            }
            rec(prob0, 0, 0, 1)
        }
    }

    /// CU-C08 `OrdenacionMultiple`.
    mutating func multiplePuntos(_ estado: @escaping @MainActor @Sendable (String) -> Void) async throws(ErrorDominio) {
        var (a, b) = (p.lnMinimo, p.lnMaximo)
        if b < a { (a, b) = (b, a) }
        if p.multiplePorProbabilidad && b > 0 { return await estado("Los LN deben ser valores negativos") }
        let paso = (b - a) / Double(p.puntos - 1)
        maxColumnas = p.columnasPorTramo
        admitidas = [Bool](repeating: false, count: Self.total)
        var i = 0
        var central = a
        while central <= b {
            if Task.isCancelled { return }
            i += 1
            iter = "\(i) "
            if p.multiplePorProbabilidad { c.probabilidad = exp(central) }
            if i == 1 {
                try await ordenar(productos: p.multiplePorProbabilidad, ln: central, estado)
            } else {
                let f = Float(central)
                for x in ap.indices where ap[x].probabilidadDiferencial != Self.centinela {
                    ap[x].probabilidadDiferencial = abs(ap[x].probabilidad - f)
                }
                await estado(iter + "Ordenando apuestas ...")
                OrdenacionPropia.ordenar(&ap) { $0.probabilidadDiferencial }
                for nr in 0..<max(0, min(maxColumnas, Self.total)) { admitidas[Int(ap[nr].columna)] = true }
            }
            central += paso
        }
        await estado(try grabar(pestanaProductos: false, admitidasMultiples: true))
    }

    /// CU-C07 `GrabacionColumnas` / `GrabarAdmitidasMultiples` (este recorre por columna pero lee `Ap14T[nr]` ya
    /// ordenado: se replica). Devuelve el estado final.
    func grabar(pestanaProductos: Bool, admitidasMultiples: Bool) throws(ErrorDominio) -> String {
        let pr = Float(p.precio * p.porcentaje14 / 100)
        let premioMax = Float(p.recaudacion * p.porcentaje14 / 100)
        let limite = Float(p.limiteAcumulada)
        let porExp = admitidasMultiples ? p.multiplePorProbabilidad : pestanaProductos
        let conv = ConvertidorDeBases(numeroPartidos: numeroPartidos)
        let t = { (f: Float) in FormatoNET.toString(f, cultura: entorno.cultura) }
        var acum: Float = 0
        var lineas: [String] = []
        let indices = admitidasMultiples ? admitidas.indices.filter { admitidas[$0] } : Array(0..<max(0, min(numApuestas, maxColumnas)))
        for nr in indices {
            guard ap.indices.contains(nr) else { throw .fueraDeRango("OrdenarPorProbabilidadFrmViewModel.cs:735") }
            var linea = try conv.columna(Int64(admitidasMultiples ? nr : Int(ap[nr].columna)))
            let prob: Float
            if porExp {
                prob = Float(exp(Double(ap[nr].probabilidad)))
                acum += prob
                if acum > limite { break }
            } else {
                prob = ap[nr].probabilidad
                acum = prob
            }
            if p.conAcumulada { linea += "\t" + t(acum) }
            let premio = min(pr / prob, premioMax)
            if p.conPremio { linea += "\t" + t(premio) }
            // 0.77.2 `GrabacionColumnas`: el intervalo no frena la probabilidad acumulada; solo deja de grabar la columna.
            if !admitidasMultiples, let (a, b) = p.premio14Entre, !(Double(premio) >= a && Double(premio) <= b) { continue }
            lineas.append(linea)
        }
        if let d = ArchivoColumnasTexto.datosConComa(lineas) { try entorno.archivos.escribir(d, en: p.salida, atomico: false) }
        return p.pestana == 0 ? "Finalizado \(lineas.count) columnas Pr. acum. =" + t(100 * acum) : "Finalizado \(lineas.count) columnas"
    }
}
