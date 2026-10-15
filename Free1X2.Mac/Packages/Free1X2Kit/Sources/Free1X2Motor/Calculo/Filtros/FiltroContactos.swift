import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroContactos.cs (02 §3).

final class FiltroContactos: FiltroColumna {
    let condicion: CondicionContactos
    let analisisActivo: Bool
    /// Contadores en el orden de `CondicionContactos.Contacto`.
    private(set) var contadores = [Int](repeating: 0, count: 10)

    init(_ condicion: CondicionContactos, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
    }

    var tipo: TipoFiltro { .contactos }
    var activa: Bool { condicion.activa }
    private var usaFiguras: Bool { condicion.figuras?.isEmpty == false }

    /// `AnalizaColumna(long)` (`CON:155-202`): compacta quitando huecos y clasifica cada par por `a | b`.
    static func contar(_ columna: Columna) -> [Int] {
        var c = columna
        var sin: Int64 = 0
        while c != 0 {
            let s = c & 7
            if s != 0 { sin = (sin &<< 3) | s }
            c >>= 3
        }
        var n = [Int](repeating: 0, count: 6)  // 1X, 12, X2, 11, XX, 22
        var s1 = sin & 7
        while sin != 0 {
            sin >>= 3
            let s2 = sin & 7
            if s2 > 0 {
                switch s1 | s2 {
                case 6: n[0] += 1
                case 5: n[1] += 1
                case 3: n[2] += 1
                case 4: n[3] += 1
                case 2: n[4] += 1
                case 1: n[5] += 1
                default: break
                }
                s1 = s2
            }
        }
        // 1V = 1X + 12; XV = XX + X2; 2V = 22 + X2; VV = XX + X2 + 22.
        return n + [n[0] + n[1], n[4] + n[2], n[5] + n[2], n[4] + n[2] + n[5]]
    }

    /// `ObtenerFiguraLongValor` (`CON:982-1018`): valores > 0 de mayor a menor, un nibble cada uno.
    var figura: Int64 {
        contadores.sorted(by: >).filter { $0 > 0 }.reduce(Int64(0)) { ($0 &<< 4) | Int64($1) }
    }

    func analizar(_ c: Columna) throws(ErrorDominio) -> Bool {
        contadores = Self.contar(c)
        for (i, n) in contadores.enumerated() where !(try condicion.valores[i].permite(n, "FiltroContactos.cs:204-223")) {
            return false
        }
        if usaFiguras { return condicion.figuras?.contains(figura) == true }
        return true
    }

    func analizarFallos(_ c: Columna) throws(ErrorDominio) -> [String]? {
        contadores = Self.contar(c)
        var texto = ""
        let o = "FiltroContactos.cs:225-280"
        for k in CondicionContactos.Contacto.ordenFallos {
            let n = contadores[k.rawValue]
            let v = condicion.valores[k.rawValue]
            if !(try v.permite(n, o)) { texto += "Fallo en contactos \(k.par)  (\(n))#" }
            if try v.letra(n, o) != ValoresConTolerancia.sinLetra { texto += "!Tolerancia en contactos \(k.par)  (\(n))#" }
        }
        if usaFiguras && condicion.figuras?.contains(figura) != true {
            texto += "Fallo en Figuras  (\(try UtilidadesEntradasValores.obtenerTextoFiguraFromLong(figura)))#"
        }
        return Self.fallos(texto)
    }

    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int {
        var n = 0
        for letra in letras.utf16 {
            for (i, valor) in contadores.enumerated()
            where try condicion.valores[i].letra(valor, "FiltroContactos.cs:327-376") == letra {
                n += 1
            }
        }
        return n
    }
}
