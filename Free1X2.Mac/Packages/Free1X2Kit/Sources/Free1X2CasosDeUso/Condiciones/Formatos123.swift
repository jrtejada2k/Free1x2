import Foundation
import Free1X2Base
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
