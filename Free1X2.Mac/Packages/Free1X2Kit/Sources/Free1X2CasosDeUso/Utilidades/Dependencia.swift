import Foundation
import Free1X2Base
import Free1X2Formatos

// C#: Views/Ported/FrmDependenciaLinealViewModel.cs (09 pantalla 54; CU-B21) y FrmReducidasPerfectasViewModel.cs
// (pantalla 55; CU-B22). Toda la lógica vive en los VM.

/// `TestSignosPartidoATratar`: signo nuevo = `(Σ coef·signo) % modulo · corrector + independiente`.
public struct ParametrosDependencia: Sendable, Hashable {
    public let modulo, corrector, independiente: Int
}

extension CasosUtilidades {
    static let potencias3 = (0..<15).map { Int(ConvertidorDeBases.potenciasDe3[$0]) }

    /// `LeerColumnas`: índices (línea entera) y % de 1/X/2 por partido redondeados al par (solo para proponer).
    @concurrent public static func leerDependencia(
        _ url: URL, entorno: Entorno
    ) async throws(ErrorDominio) -> (bits: ConjuntoBits, porcentajes: [[Double]]) {
        var bits = ConjuntoBits(cantidad: 4_782_969)
        var p = [[Double]](repeating: [0, 0, 0], count: 14)
        let conv = ConvertidorDeBases(numeroPartidos: 14)
        for l in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url)) {
            let n = Int(try conv.numero(l))
            try bits.poner(n)
            for i in 0..<14 { p[i][(n / potencias3[i]) % 3] += 1 }
        }
        for i in 0..<14 {
            let suma = p[i].reduce(0, +)
            if suma == 0 { continue }
            p[i][2] = (p[i][2] * 100 / suma).rounded(.toNearestOrEven)
            p[i][1] = (p[i][1] * 100 / suma).rounded(.toNearestOrEven)
            p[i][0] = 100 - p[i][2] - p[i][1]
        }
        return (bits, p)
    }

    /// `ProponerCoeficientes`: 1 si están los tres signos; el partido a tratar es el primero con alguno ausente (o el 1.º).
    public static func proponerCoeficientes(_ p: [[Double]]) -> (coeficientes: [Int], partido: Int) {
        let coef = p.map { $0[0] * $0[1] * $0[2] != 0 ? 1 : 0 }
        return (coef, coef.firstIndex(of: 0) ?? 0)
    }

    /// `nil` = «Debe poner un triple o un doble en el partido a tratar».
    public static func parametrosDependencia(_ s1: Bool, _ sx: Bool, _ s2: Bool) -> ParametrosDependencia? {
        switch (s1, sx, s2) {
        case (true, true, true): .init(modulo: 3, corrector: 1, independiente: 0)
        case (true, true, false): .init(modulo: 2, corrector: 1, independiente: 0)
        case (false, true, true): .init(modulo: 2, corrector: 1, independiente: 1)
        case (true, false, true): .init(modulo: 2, corrector: 2, independiente: 0)
        default: nil
        }
    }

    /// `Aceptar`: recalcula el partido de cada columna y graba por índice ascendente (`ConvNumAColumna` con
    /// `numeroPartidos`); sin columnas no se crea el fichero. Devuelve las grabadas.
    @concurrent public static func aplicarDependencia(
        _ bits: ConjuntoBits, partido ata: Int, coeficientes coef: [Int], parametros q: ParametrosDependencia, salida: URL,
        numeroPartidos: Int, entorno: Entorno
    ) async throws(ErrorDominio) -> Int {
        var cambiados = ConjuntoBits(cantidad: 4_782_969)
        for i in bits.indices {
            let inicial = (i / potencias3[ata]) % 3
            var nuevo = 0
            for p in 0..<14 where p != ata { nuevo += coef[p] * ((i / potencias3[p]) % 3) }
            try cambiados.poner(i + potencias3[ata] * ((nuevo % q.modulo) * q.corrector + q.independiente - inicial))
        }
        let conv = ConvertidorDeBases(numeroPartidos: numeroPartidos)
        var lineas: [String] = []
        for i in cambiados.indices { lineas.append(try conv.columna(Int64(i))) }
        if let d = ArchivoColumnasTexto.datos(lineas) { try entorno.archivos.escribir(d, en: salida, atomico: false) }
        return lineas.count
    }
}

// MARK: - Reducciones perfectas

/// Un partido de «Reducciones perfectas»: signos del pronóstico (1, X, 2) y signo base (0 = 1, 1 = X, 2 = 2).
public struct FilaReducidaPerfecta: Sendable, Hashable {
    public var pronostico = [false, false, false]
    public var base = 0

    public init() {}
}

extension CasosUtilidades {
    /// Matrices de «Fortuna» (`VM:96-118`): filas = signos dependientes, columnas = independientes.
    static let m4TR13 = [[1, 1], [1, 2]]
    static let m13TR13 = [[0, 0, 1, 1, 1, 1, 1, 1, 1, 1], [1, 1, 0, 0, 1, 1, 1, 2, 2, 2], [1, 2, 1, 2, 0, 1, 2, 0, 1, 2]]
    static let m11TR12 = [[1, 0, 1, 2, 2, 2], [1, 1, 0, 1, 1, 2], [1, 1, 2, 0, 2, 1], [1, 2, 1, 1, 0, 1], [1, 2, 2, 2, 1, 0]]
    static let m7DR13 = [[0, 1, 1, 1], [1, 0, 1, 1], [1, 1, 0, 1]]
    static let m15DR13 = [
        [1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1], [1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1], [1, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0],
        [1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0],
    ]

    /// `Generar` (sin grabar): los mensajes de validación van en el error. Los dígitos se sacan con
    /// `Convert.ToInt32(i / 3^z) % 3` (redondeo al par, no truncado), como el legacy.
    public static func reduccionPerfecta(_ filas: [FilaReducidaPerfecta], longitud: Int) throws(ErrorDominio) -> [String] {
        func error(_ m: String) -> ErrorDominio { ErrorDominio(.general, m) }
        func activo(_ f: FilaReducidaPerfecta, _ s: Int) -> Bool { f.pronostico[min(s, 2)] }
        for i in stride(from: 0, to: longitud, by: 1) {
            let f = try filas.leer(i, "FrmReducidasPerfectasViewModel.cs:TestSignos")
            if !activo(f, f.base) { throw error("La columna base no está incluida en el pronóstico") }
        }
        let marcados = filas.map { $0.pronostico.count(where: \.self) }
        let involucrados = marcados.map { $0 >= 2 }
        let (triples, dobles) = (marcados.count(where: { $0 == 3 }), marcados.count(where: { $0 == 2 }))
        if triples != 0 && dobles != 0 { throw error("No se permite mezcla de dobles y triples") }
        let m: [[Int]]
        if dobles == 0 {
            switch triples {
            case 4: m = m4TR13
            case 13: m = m13TR13
            case 11: m = m11TR12
            default: throw error("Nº de triples incorrecto")
            }
        } else {
            switch dobles {
            case 7, 15:
                guard filas.indices.allSatisfy({ !involucrados[$0] || activo(filas[$0], filas[$0].base) }) else {
                    throw error("Los signos base no forman parte del pronostico")
                }
                m = dobles == 7 ? m7DR13 : m15DR13
            default: throw error("Nº de dobles incorrecto")
            }
        }
        let esTriples = dobles == 0
        let (r, c) = (m.count, m[0].count)
        guard r + c <= longitud else { throw error("La longitud de la columna es incorrecta") }
        let s = ["1", "X", "2"]
        var columnas: [String] = []
        for i in 0..<Int(pow(3, Double(c))) {
            var signos = [Int](repeating: 0, count: longitud)
            var suma = [Int](repeating: 0, count: 5)
            var descartar = false
            for z in 0..<c {
                signos[z] = Int(try FormatoNET.convertToInt32(Double(i) / pow(3, Double(z)))) % 3
                if !esTriples && signos[z] == 2 {
                    descartar = true
                    break
                }
                for j in 0..<r { suma[j] += m[j][z] * signos[z] }
            }
            if descartar { continue }
            for j in 0..<r { signos[c + j] = suma[j] % (esTriples ? 3 : 2) }
            var columna = ""
            var pv = 0
            for (p, f) in filas.enumerated() {
                guard involucrados[p] else {
                    columna += s[f.base]
                    continue
                }
                if esTriples {
                    columna += s[(f.base + signos[pv]) % 3]
                } else {
                    var d = [f.base, (f.base + 1) % 3]
                    if !activo(f, d[1]) { d[1] = (d[1] + 1) % 3 }
                    columna += s[d[signos[pv]]]
                }
                pv += 1
            }
            columnas.append(String(columna.prefix(longitud)))
        }
        return columnas
    }
}
