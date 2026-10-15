import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/Formatos123FrmViewModel.cs (09 pantalla 51; CU-B18).

/// Una fila de la lista de formatos tal como se escribe.
public struct FilaFormato123: Sendable, Hashable {
    public var formato = ""
    public var minimo = ""
    public var maximo = ""

    public init(formato: String = "", minimo: String = "", maximo: String = "") {
        (self.formato, self.minimo, self.maximo) = (formato, minimo, maximo)
    }
}

extension CasosCondiciones {
    /// `int.TryParse` (estilo `Integer`: blancos y signo).
    static func entero(_ s: String) -> Int? { Int(FormatoNET.recortar(s)) }

    /// `ConstruirFormatos` (:194-220): filas no válidas se descartan en silencio; sin deduplicar.
    public static func formatos123(_ filas: [FilaFormato123], ignorarRepeticiones: Bool) -> [Formato123] {
        filas.compactMap { f in
            let t = FormatoNET.recortar(f.formato)
            guard !t.isEmpty, t.utf16.count <= 14, t.allSatisfy({ "123".contains($0) }) else { return nil }
            guard !ignorarRepeticiones else { return Formato123(formato: t) }
            guard let a = entero(f.minimo), let b = entero(f.maximo) else { return nil }
            return Formato123(formato: t, aciertosMin: a, aciertosMax: b)
        }
    }

    /// `ConstruirAciertos` (:223-251): valores y rangos `a-b` de 0…40; lo mal formado se descarta.
    public static func aciertosFormatos123(_ texto: String) -> [Int] {
        guard !blanco(texto) else { return [] }
        var r: [Int] = []
        for parte in texto.split(separator: ",", omittingEmptySubsequences: false) {
            let p = FormatoNET.recortar(String(parte))
            guard !p.isEmpty else { continue }
            if !p.contains("-") {
                if let v = entero(p), (0...40).contains(v) { r.append(v) }
            } else {
                let i = p.split(separator: "-", omittingEmptySubsequences: false).map(String.init)
                if i.count == 2, let a = entero(i[0]), let b = entero(i[1]), a <= b {
                    for j in a...b where (0...40).contains(j) { r.append(j) }
                }
            }
        }
        return r
    }

    /// `Aceptar`/`ObtenerFiltroTemporal` (:306-343, :400-420): `IsActive` = hay formatos o aciertos; `ContieneDatos` no
    /// vuelve a falso (aquí es calculado).
    public static func aplicarFormatos123(
        _ filas: [FilaFormato123], aciertos texto: String, ignorarRepeticiones: Bool, valoracion: [[Double]],
        en f: inout CondicionFormatos123
    ) throws(ErrorDominio) {
        f.valoracionTransformada = try CondicionFormatos123.transformar(valoracion)
        f.valoracion = valoracion
        let formatos = formatos123(filas, ignorarRepeticiones: ignorarRepeticiones)
        let aciertos = aciertosFormatos123(texto).sorted()
        if let a = aciertos.first, let b = aciertos.last { (f.aciertosMin, f.aciertosMax) = (a, b) }
        f.aciertos = aciertos
        f.formatos = formatos
        f.pasoFijo = ignorarRepeticiones
        f.aciertosFiltro = texto
        f.activa = f.contieneDatos
    }

    /// `Traducir` (:265-283): columna de N signos 1/X/2 o «Error!!!!».
    public static func traducirFormatos123(_ columna: String, valoracion: [[Double]], numeroPartidos n: Int) throws(ErrorDominio) -> String
    {
        let c = FormatoNET.mayusculas(columna)
        guard c.utf16.count == n, c.allSatisfy({ "1X2".contains($0) }) else { return "Error!!!!" }
        var f = CondicionFormatos123(numeroPartidos: n)
        f.valoracion = valoracion
        return try f.col1X2ToCol123(c, numeroPartidos: n)
    }
}

// C#: Views/Ported/AnalisisFormatos123FrmViewModel.cs (08 pantalla 5; CU-A06…A08).

/// Una fila del informe «Formato / Apariciones».
public struct ConteoFormato: Sendable, Hashable {
    public let formato: String
    public let apariciones: Int
}

extension CasosCondiciones {
    /// «Archivo de columnas»: en mayúsculas; una de más de 16 signos deja la lista vacía sin aviso.
    public static func columnasFormatos123(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> [String] {
        var r: [String] = []
        for c in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url)) {
            guard c.utf16.count <= 16 else { return [] }
            r.append(c)
        }
        return r
    }

    /// CU-A06 `TransformarValoracion` + `TraducirColumna`: rango de cada signo (1 = el más valorado; empates: 1 > X > 2).
    /// Sin rango (NaN) el signo no aporta nada; una columna más larga que la valoración lanza.
    public static func columnaEnRangos(_ columna: String, valoracion: [[Double]]) throws(ErrorDominio) -> String {
        func rangos(_ v: [Double]) -> [String]? {
            let (a, b, c) = (v[0], v[1], v[2])
            if a >= b && a >= c { return b >= c ? ["1", "2", "3"] : c > b ? ["1", "3", "2"] : nil }
            if b > a && b >= c { return a >= c ? ["2", "1", "3"] : ["3", "1", "2"] }
            if c > a && c > b { return a >= b ? ["2", "3", "1"] : ["3", "2", "1"] }
            return nil
        }
        var r = ""
        for (i, s) in columna.utf16.enumerated() {
            let v = try valoracion.leer(i, "AnalisisFormatos123FrmViewModel.cs:301")
            guard v.count >= 3 else { throw .fueraDeRango("AnalisisFormatos123FrmViewModel.cs:301") }
            r += rangos(v)?[s == 0x31 ? 0 : s == 0x58 ? 1 : 2] ?? ""
        }
        return r
    }

    /// `ConvStrToLong` de la pantalla: el primer carácter en los bits bajos (`1 << -1` = `Int32.min`, B3).
    static func binarioFormato123(_ s: String) -> Int64 {
        Formato123(formato: String(decoding: s.utf16.reversed(), as: UTF16.self)).formatoLong
    }

    /// CU-A07 `Analizar`: apariciones solapadas de cada formato en la columna traducida.
    public static func analizarFormatos123(
        _ columna: String, valoracion: [[Double]], formatos: [Formato123]
    ) throws(ErrorDominio) -> [ConteoFormato] {
        let col = binarioFormato123(try columnaEnRangos(columna, valoracion: valoracion))
        return formatos.map { f in
            let b = binarioFormato123(f.formato)
            var (c, n) = (col, 0)
            while c != 0 {
                if c & b == b { n += 1 }
                c >>= 3
            }
            return ConteoFormato(formato: f.formato, apariciones: n)
        }
    }

    /// CU-A08 `MostrarTodos`: todas las subcadenas (por longitud y posición), agrupadas en orden de primera aparición.
    public static func todosFormatos123(_ columna: String, valoracion: [[Double]]) throws(ErrorDominio) -> [ConteoFormato] {
        let t = Array(try columnaEnRangos(columna, valoracion: valoracion).utf16)
        let n = columna.utf16.count
        var orden: [String] = []
        var cuenta: [String: Int] = [:]
        for l in stride(from: 1, through: n, by: 1) {
            for p in 0...(n - l) {
                guard p + l <= t.count else { throw .fueraDeRango("AnalisisFormatos123FrmViewModel.cs:262") }
                let s = String(decoding: t[p..<p + l], as: UTF16.self)
                if cuenta[s] == nil { orden.append(s) }
                cuenta[s, default: 0] += 1
            }
        }
        return orden.map { ConteoFormato(formato: $0, apariciones: cuenta[$0] ?? 0) }
    }
}

// C#: Views/Ported/CalculoFormatosFrmViewModel.cs (08 pantalla 15; CU-A34, CU-A35).
extension CasosCondiciones {
    /// CU-A34/A35 `SacarFormato`: ventanas de 2…5 signos en orden de primera aparición (capacidades 9/12/11/10: lo que no
    /// cabe se pierde) y contactos (sobre la columna sin `*`); `AppendLine` = `\r\n`.
    public static func informeFormatos(_ columna: String) -> String {
        let u = Array(columna.utf16)
        var lineas: [String] = []
        for (k, capacidad, nombre) in [(2, 9, "Pares"), (3, 12, "Trios"), (4, 11, "Cuartetos"), (5, 10, "Quintetos")] {
            var vistos: [(String, Int)] = []
            for i in u.indices where u.count - i >= k {
                let v = String(decoding: u[i..<i + k], as: UTF16.self)
                if let j = vistos.firstIndex(where: { $0.0 == v }) {
                    vistos[j].1 += 1
                } else if vistos.count < capacidad {
                    vistos.append((v, 1))
                }
            }
            lineas.append("* \(nombre) que aparecen")
            lineas += vistos.map { "\($0.0)-\($0.1)" }
            lineas.append("\(nombre) distintos = \(vistos.count)")
        }
        // `AnalizaColumna`: 1X, 12, X2, 11, XX, 22, 1V, XV, 2V, VV.
        var n = [Int](repeating: 0, count: 10)
        let c = Array(columna.replacingOccurrences(of: "*", with: "").utf16)
        for i in 0..<max(0, c.count - 1) {
            switch String(decoding: c[i...i + 1], as: UTF16.self) {
            case "1X", "X1", "1x", "x1": (n[0], n[6]) = (n[0] + 1, n[6] + 1)
            case "12", "21": (n[1], n[6]) = (n[1] + 1, n[6] + 1)
            case "X2", "2X", "x2", "2x": for j in [2, 9, 7, 8] { n[j] += 1 }
            case "11": n[3] += 1
            case "XX", "xx": for j in [4, 9, 7] { n[j] += 1 }
            case "22": for j in [5, 9, 8] { n[j] += 1 }
            default: break
            }
        }
        lineas.append("* Contactos")
        lineas += zip(["1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV"], n).map { "\($0)-\($1)" }
        return lineas.map { $0 + "\r\n" }.joined()
    }
}
