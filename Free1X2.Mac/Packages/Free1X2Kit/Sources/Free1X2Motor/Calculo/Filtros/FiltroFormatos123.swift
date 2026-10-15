import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroFormatos123.cs (03 §3).

final class FiltroFormatos123: FiltroColumna {
    let condicion: CondicionFormatos123
    let analisisActivo: Bool
    let numeroPartidos: Int
    private let formatosLong: [Int64]

    init(_ condicion: CondicionFormatos123, analisisActivo: Bool, numeroPartidos: Int) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
        self.numeroPartidos = numeroPartidos
        formatosLong = condicion.formatos.map(\.formatoLong)
    }

    var tipo: TipoFiltro { .formatos123 }
    var activa: Bool { condicion.activa }

    /// `TraducirLongColumnaALong123` (:267-289): el partido 1 queda en el grupo más alto; los asteriscos se saltan.
    func traducir(_ columna: Columna) throws(ErrorDominio) -> Int64 {
        var c = columna
        var r: Int64 = 0
        var i = 0
        while c != 0 && i < numeroPartidos {
            let k: Int? =
                switch c & 7 {
                case 4: 0
                case 2: 1
                case 1: 2
                default: nil
                }
            if let k {
                let fila = try condicion.valoracionTransformada.leer(i, "FiltroFormatos123.cs:276")
                r = (r &<< 3) | Int64(try fila.leer(k, "FiltroFormatos123.cs:276"))
            }
            c >>= 3
            i += 1
        }
        return r
    }

    /// Apariciones (solapadas) del formato como subcadena de la cadena 1-2-3.
    private static func cuenta(_ col: Int64, _ formato: Int64, hastaUna: Bool) -> Int {
        var t = col
        var n = 0
        while t != 0 {
            if t & formato == formato {
                n += 1
                if hastaUna { break }
            }
            t >>= 3
        }
        return n
    }

    func analizar(_ columna: Columna) throws(ErrorDominio) -> Bool {
        let col = try traducir(columna)
        var globales = 0
        if condicion.pasoFijo {
            // `CumpleCondicionesPasoFijo` (:159-181).
            for f in formatosLong where Self.cuenta(col, f, hastaUna: true) >= 1 { globales += 1 }
        } else {
            // `CumpleCondicionesPasoLibre` (:115-157): `globales` satura en `aciertosMax + 1`.
            for (f, largo) in zip(condicion.formatos, formatosLong) {
                let a = Self.cuenta(col, largo, hastaUna: false)
                if a >= f.aciertosMin && a <= f.aciertosMax && globales <= condicion.aciertosMax {
                    globales += 1
                } else if globales > condicion.aciertosMax {
                    break
                }
            }
        }
        return condicion.aciertos.contains(globales)
    }

    /// (:106-113,182-256): en paso libre puede devolver texto aunque la columna pase [PRESERVAR].
    func analizarFallos(_ columna: Columna) throws(ErrorDominio) -> [String]? {
        let col = try traducir(columna)
        var globales = 0
        var texto = ""
        if condicion.pasoFijo {
            for f in formatosLong where Self.cuenta(col, f, hastaUna: true) >= 1 { globales += 1 }
            if !condicion.aciertos.contains(globales) { texto = "Fallo en Aciertos Globales (\(globales))" }
        } else {
            for (f, largo) in zip(condicion.formatos, formatosLong) {
                let a = Self.cuenta(col, largo, hastaUna: false)
                if a >= f.aciertosMin && a <= f.aciertosMax && globales <= condicion.aciertosMax {
                    globales += 1
                } else {
                    texto = "Columna Fuera de Límites (Formato \(f.formato) Apariciones: \(a))"
                    if globales > condicion.aciertosMax { break }
                }
            }
            if !condicion.aciertos.contains(globales) { texto = "Fallo en Líneas Acertadas (\(globales))" }
        }
        return texto.isEmpty ? nil : trozos(texto, "#")
    }

    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int { 0 }
}
