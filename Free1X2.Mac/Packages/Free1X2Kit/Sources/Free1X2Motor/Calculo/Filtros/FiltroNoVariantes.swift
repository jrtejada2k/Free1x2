import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroNoVariantes.cs (03 §2).

final class FiltroNoVariantes: FiltroColumna {
    /// `X` en 16 partidos y `2` solo en 15 (:107,111) [PRESERVAR].
    static let mascaraEquis: Int64 = 80_421_421_917_330
    static let mascaraDoses: Int64 = 5_026_338_869_833

    let condicion: CondicionNoVariantes
    let analisisActivo: Bool
    private(set) var noVariantes = 0
    private(set) var noEquis = 0
    private(set) var noDoses = 0

    init(_ condicion: CondicionNoVariantes, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
    }

    var tipo: TipoFiltro { .noVariantes }
    var activa: Bool { condicion.activa }

    /// `AnalizaColumna(long)` (:103-117).
    private func contar(_ c: Columna) {
        noEquis = UtilColumnas.contarBitsA1(c & Self.mascaraEquis)
        noDoses = UtilColumnas.contarBitsA1(c & Self.mascaraDoses)
        noVariantes = noEquis + noDoses
    }

    func analizar(_ c: Columna) throws(ErrorDominio) -> Bool {
        contar(c)
        let o = "FiltroNoVariantes.cs:121"
        // `&&` de C#: corta antes de mirar (y de salirse de) el array siguiente.
        guard try condicion.variantes.permite(noVariantes, o) else { return false }
        guard try condicion.equis.permite(noEquis, o) else { return false }
        return try condicion.doses.permite(noDoses, o)
    }

    func analizarFallos(_ c: Columna) throws(ErrorDominio) -> [String]? {
        contar(c)
        let o = "FiltroNoVariantes.cs:126-146"
        var texto = ""
        for (nombre, valores, n) in [
            ("variantes", condicion.variantes, noVariantes), ("equis", condicion.equis, noEquis), ("doses", condicion.doses, noDoses),
        ] {
            if !(try valores.permite(n, o)) { texto += "Fallo en número de \(nombre)  (\(n))#" }
            if try valores.letra(n, o) != ValoresConTolerancia.sinLetra { texto += "!Tolerancia en número de \(nombre)  (\(n))#" }
        }
        return Self.fallos(texto)
    }

    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int {
        let o = "FiltroNoVariantes.cs:176-199"
        var n = 0
        for letra in letras.utf16 {
            if try condicion.variantes.letra(noVariantes, o) == letra { n += 1 }
            if try condicion.equis.letra(noEquis, o) == letra { n += 1 }
            if try condicion.doses.letra(noDoses, o) == letra { n += 1 }
        }
        return n
    }
}
