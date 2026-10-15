import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroDistancias.cs (03 §1).

final class FiltroDistancias: FiltroColumna {
    /// 21 grupos de 3 bits `100`, `010`, `001` y `011` (:27-30).
    static let mascaraUnos: Int64 = 5_270_498_306_774_157_604
    static let mascaraEquis: Int64 = 2_635_249_153_387_078_802
    static let mascaraDoses: Int64 = 1_317_624_576_693_539_401
    static let mascaraVariantes: Int64 = 3_952_873_730_080_618_203

    let condicion: CondicionDistancias
    let analisisActivo: Bool
    private(set) var distVariantes = 0
    private(set) var distUnos = 0
    private(set) var distEquis = 0
    private(set) var distDoses = 0

    init(_ condicion: CondicionDistancias, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
    }

    var tipo: TipoFiltro { .distancias }
    var activa: Bool { condicion.activa }

    /// `Distancia` (:102-128): mayor salto de posición entre dos apariciones consecutivas; 0 con menos de dos.
    static func distancia(_ c: Int64) -> Int {
        var c = c
        var d = 0
        guard c != 0 else { return 0 }
        while c & 7 == 0 { c >>= 3 }
        repeat {
            c >>= 3
            var actual = 0
            while c & 7 == 0 && c != 0 {
                actual += 1
                c >>= 3
            }
            if c != 0 { d = max(actual + 1, d) }
        } while c != 0
        return d
    }

    private func contar(_ c: Columna) {
        distUnos = Self.distancia(c & Self.mascaraUnos)
        distEquis = Self.distancia(c & Self.mascaraEquis)
        distDoses = Self.distancia(c & Self.mascaraDoses)
        distVariantes = Self.distancia(c & Self.mascaraVariantes)
    }

    func analizar(_ c: Columna) throws(ErrorDominio) -> Bool {
        contar(c)
        let o = "FiltroDistancias.cs:143"
        guard try condicion.variantes.permite(distVariantes, o) else { return false }
        guard try condicion.unos.permite(distUnos, o) else { return false }
        guard try condicion.equis.permite(distEquis, o) else { return false }
        return try condicion.doses.permite(distDoses, o)
    }

    func analizarFallos(_ c: Columna) throws(ErrorDominio) -> [String]? {
        contar(c)
        let o = "FiltroDistancias.cs:146-170"
        var texto = ""
        for (nombre, valores, n) in [
            ("variantes", condicion.variantes, distVariantes), ("unos", condicion.unos, distUnos),
            ("equis", condicion.equis, distEquis), ("doses", condicion.doses, distDoses),
        ] {
            if !(try valores.permite(n, o)) { texto += "Fallo en distancia de \(nombre)  (\(n))#" }
            if try valores.letra(n, o) != ValoresConTolerancia.sinLetra { texto += "!Tolerancia en distancia de \(nombre)  (\(n))#" }
        }
        return Self.fallos(texto)
    }

    /// (:207-237): `noIntGlobal` vale siempre 0 [PRESERVAR].
    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int {
        let o = "FiltroDistancias.cs:207-237"
        var n = 0
        for letra in letras.utf16 {
            if try condicion.global.letra(0, o) == letra { n += 1 }
            if try condicion.variantes.letra(distVariantes, o) == letra { n += 1 }
            if try condicion.unos.letra(distUnos, o) == letra { n += 1 }
            if try condicion.equis.letra(distEquis, o) == letra { n += 1 }
            if try condicion.doses.letra(distDoses, o) == letra { n += 1 }
        }
        return n
    }
}
