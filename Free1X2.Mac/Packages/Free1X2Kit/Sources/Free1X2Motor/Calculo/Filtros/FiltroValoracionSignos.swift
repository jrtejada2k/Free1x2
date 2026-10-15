import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroValoracionSignos.cs (03 §5).

final class FiltroValoracionSignos: FiltroColumna {
    static let k = 0.03420425138

    let condicion: CondicionValoracionSignos
    let analisisActivo: Bool
    let cultura: Cultura
    private let esSuma: Bool
    private let esMultiplo: Bool
    private(set) var valoracion = 0.0
    private(set) var valoracionUnos = 0.0
    private(set) var valoracionEquis = 0.0
    private(set) var valoracionDoses = 0.0

    init(_ condicion: CondicionValoracionSignos, analisisActivo: Bool, cultura: Cultura) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
        self.cultura = cultura
        esSuma = condicion.tipoValoracion == "suma"
        esMultiplo = condicion.tipoValoracion == "multiplo"
    }

    var tipo: TipoFiltro { .valoracionSignos }
    var activa: Bool { condicion.activa }

    /// `AnalizaPorSumas` / `AnalizaPorMultiplos` (:76-158): orden de partido ascendente; los asteriscos no aportan pero
    /// avanzan el índice.
    private func calcular(_ columna: Columna) throws(ErrorDominio) {
        (valoracion, valoracionUnos, valoracionEquis, valoracionDoses) = (0, 0, 0, 0)
        guard esSuma || esMultiplo else { return }
        if esMultiplo { (valoracion, valoracionUnos, valoracionEquis, valoracionDoses) = (1, 1, 1, 1) }
        var c = columna
        var i = 0
        let o = "FiltroValoracionSignos.cs:76-158"
        while c != 0 {
            switch c & 7 {
            case 4:
                let v = try condicion.valores1.leer(i, o)
                if esSuma {
                    valoracion += v
                    valoracionUnos += v
                } else {
                    valoracion *= v * Self.k
                    valoracionUnos *= v * Self.k
                }
            case 2:
                let v = try condicion.valoresX.leer(i, o)
                if esSuma {
                    valoracion += v
                    valoracionEquis += v
                } else {
                    valoracion *= v * Self.k
                    valoracionEquis *= v * Self.k
                }
            case 1:
                let v = try condicion.valores2.leer(i, o)
                if esSuma {
                    valoracion += v
                    valoracionDoses += v
                } else {
                    valoracion *= v * Self.k
                    valoracionDoses *= v * Self.k
                }
            default: break
            }
            c >>= 3
            i += 1
        }
        if esMultiplo {
            if valoracion == 1 { valoracion = 0 }
            if valoracionUnos == 1 { valoracionUnos = 0 }
            if valoracionEquis == 1 { valoracionEquis = 0 }
            if valoracionDoses == 1 { valoracionDoses = 0 }
        }
    }

    func analizar(_ columna: Columna) throws(ErrorDominio) -> Bool {
        try calcular(columna)
        let c = condicion
        return c.rangosGlobales.contiene(valoracion) && c.rangosUnos.contiene(valoracionUnos)
            && c.rangosEquis.contiene(valoracionEquis) && c.rangosDoses.contiene(valoracionDoses)
    }

    /// (:169-186): el valor con `double.ToString()` de la cultura actual.
    func analizarFallos(_ columna: Columna) throws(ErrorDominio) -> [String]? {
        try calcular(columna)
        let c = condicion
        var texto = ""
        for (nombre, rangos, v) in [
            ("global", c.rangosGlobales, valoracion), ("de 1", c.rangosUnos, valoracionUnos),
            ("de X", c.rangosEquis, valoracionEquis), ("de 2", c.rangosDoses, valoracionDoses),
        ] where !rangos.contiene(v) {
            texto += "Fallo en valoración \(nombre)  (\(FormatoNET.toString(v, cultura: cultura)))#"
        }
        return Self.fallos(texto)
    }

    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int { 0 }
}

extension CondicionValoracionSignos {
    /// `Analizar` + `ValoracionResultado` (la valoración global de la columna; pantalla 98, CU-C46).
    public func valoracionResultado(_ columna: Columna, cultura: Cultura) throws(ErrorDominio) -> Double {
        let f = FiltroValoracionSignos(self, analisisActivo: false, cultura: cultura)
        _ = try f.analizar(columna)
        return f.valoracion
    }
}
