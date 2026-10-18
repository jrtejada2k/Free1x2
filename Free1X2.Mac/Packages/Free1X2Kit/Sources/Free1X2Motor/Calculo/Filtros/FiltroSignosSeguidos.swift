import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroSignosSeguidos.cs (02 §4).

final class FiltroSignosSeguidos: FiltroColumna {
    /// Máscaras de clase (16 celdas): variantes `011`, unos `100`, equis `010`, doses `001`.
    static let mascaras: [Int64] = [120_632_132_875_995, 160_842_843_834_660, 80_421_421_917_330, 40_210_710_958_665]

    let condicion: CondicionSignosSeguidos
    let analisisActivo: Bool
    /// Rachas máximas en el orden de `CondicionSignosSeguidos.Clase`.
    private(set) var rachas = [0, 0, 0, 0]

    init(_ condicion: CondicionSignosSeguidos, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
    }

    var tipo: TipoFiltro { .signosSeguidos }
    var activa: Bool { condicion.activa }

    /// `AnalizaColumna` (`SEG:150-206`): las celdas que no son 1, X o 2 no cambian ninguna racha.
    static func contar(_ columna: Columna) -> [Int] {
        var c = columna
        var (v, u, x, d) = (0, 0, 0, 0)
        var m = [0, 0, 0, 0]
        while c != 0 {
            switch c & 7 {
            case 4: (u, v, x, d) = (u + 1, 0, 0, 0)
            case 2: (x, v, u, d) = (x + 1, v + 1, 0, 0)
            case 1: (d, v, u, x) = (d + 1, v + 1, 0, 0)
            default: break
            }
            m = [max(m[0], v), max(m[1], u), max(m[2], x), max(m[3], d)]
            c >>= 3
        }
        return m
    }

    /// `ObtenFiguraV/1/X/2(col)` (`SEG:226-295`): aquí una celda 0 **corta** la racha (S2).
    static func figura(_ columna: Columna, _ clase: CondicionSignosSeguidos.Clase) -> Int64 {
        var c = columna & mascaras[clase.rawValue]
        var longitudes: [Int] = []
        var cuantas = 0
        while c != 0 {
            if c & 7 != 0 {
                cuantas += 1
            } else if cuantas > 0 {
                longitudes.append(cuantas)
                cuantas = 0
            }
            c >>= 3
        }
        if cuantas > 0 { longitudes.append(cuantas) }
        return longitudes.sorted(by: >).reduce(Int64(0)) { ($0 &<< 4) | Int64($1) }
    }

    private func figurasValidas(_ c: Columna) -> [Bool] {
        CondicionSignosSeguidos.Clase.allCases.map { k in
            let lista = condicion.figuras[k.rawValue]
            return lista.isEmpty || lista.contains(Self.figura(c, k))
        }
    }

    func analizar(_ c: Columna) throws(ErrorDominio) -> Bool {
        rachas = Self.contar(c)
        for (i, n) in rachas.enumerated() where !(try condicion.valores[i].permite(n, "FiltroSignosSeguidos.cs:77-80")) {
            return false
        }
        // `AnalizaFiguras` (`SEG:387-394`): V ∧ 1 ∧ X ∧ 2 con corte.
        guard condicion.usaFiguras else { return true }
        for k in CondicionSignosSeguidos.Clase.allCases {
            let lista = condicion.figuras[k.rawValue]
            if !lista.isEmpty && !lista.contains(Self.figura(c, k)) { return false }
        }
        return true
    }

    func analizarFallos(_ c: Columna) throws(ErrorDominio) -> [String]? {
        rachas = Self.contar(c)
        let nombres = ["variantes seguidas", "unos seguidos", "equis seguidas", "doses seguidos"]
        let tolerancia = ["!Tolerancia  en", "!Tolerancia en", "!Tolerancia en", "!Tolerancia en"]  // S5: dos espacios
        var texto = ""
        let o = "FiltroSignosSeguidos.cs:82-100"
        for i in 0..<4 {
            let n = rachas[i]
            if !(try condicion.valores[i].permite(n, o)) { texto += "Fallo en \(nombres[i])  (\(n))#" }
            if try condicion.valores[i].letra(n, o) != ValoresConTolerancia.sinLetra {
                texto += "\(tolerancia[i]) \(nombres[i])  (\(n))#"
            }
        }
        let clases = ["Variantes", "Unos", "Equis", "Doses"]
        for (k, valida) in zip(CondicionSignosSeguidos.Clase.allCases, figurasValidas(c)) where !valida {
            let f = try UtilidadesEntradasValores.obtenerTextoFiguraFromLong(Self.figura(c, k))
            texto += "Fallo en Figuras de \(clases[k.rawValue])  (\(f))#"
        }
        return Self.fallos(texto)
    }

    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int {
        var n = 0
        for letra in letras.utf16 {
            for (i, valor) in rachas.enumerated()
            where try condicion.valores[i].letra(valor, "FiltroSignosSeguidos.cs:319-347") == letra {
                n += 1
            }
        }
        return n
    }
}
