import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroDibujos.cs (03 §8).

final class FiltroDibujos: FiltroColumna {
    /// Aquí las dos máscaras son de 16 partidos (en NoVariantes la de `2` es de 15).
    static let mascaraEquis: Int64 = 80_421_421_917_330
    static let mascaraDoses: Int64 = 40_210_710_958_665

    let condicion: CondicionDibujos
    let analisisActivo: Bool
    private(set) var noEquis = 0
    private(set) var noDoses = 0

    init(_ condicion: CondicionDibujos, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
    }

    var tipo: TipoFiltro { .dibujos }
    var activa: Bool { condicion.activa }

    func analizar(_ c: Columna) throws(ErrorDominio) -> Bool {
        noEquis = UtilColumnas.contarBitsA1(c & Self.mascaraEquis)
        noDoses = UtilColumnas.contarBitsA1(c & Self.mascaraDoses)
        return condicion.tabla[noEquis * 17 + noDoses]  // invariante: los dos recuentos son ≤ 16
    }

    func analizarFallos(_ c: Columna) throws(ErrorDominio) -> [String]? {
        try analizar(c) ? nil : ["Fallo en número de dibujos  (\(noEquis)+\(noDoses))"]
    }

    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int { 0 }
}
