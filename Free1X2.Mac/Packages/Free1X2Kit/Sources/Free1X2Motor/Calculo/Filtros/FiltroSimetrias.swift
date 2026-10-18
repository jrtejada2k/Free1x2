import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroSimetrias.cs (03 §6).

final class FiltroSimetrias: FiltroColumna {
    let condicion: CondicionSimetrias
    let analisisActivo: Bool
    /// `AciertosSimetria`: lo actualiza `analizar`, no `analizarFallos`.
    private(set) var aciertos = 0

    init(_ condicion: CondicionSimetrias, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
    }

    var tipo: TipoFiltro { .simetrias }
    var activa: Bool { condicion.activa }

    /// Compara el grupo crudo de 3 bits (dos asteriscos son iguales).
    private func cuenta(_ c: Columna, enFallos: Bool) throws(ErrorDominio) -> Int {
        var n = 0
        var valida = false
        for s in condicion.simetrias {
            let p = s.partidosSimetricos
            if let primero = p.first {
                let signo = UtilColumnas.obtenerSignoInt(c, partido: primero)
                valida = p.dropFirst().allSatisfy { UtilColumnas.obtenerSignoInt(c, partido: $0) == signo }
            } else if !enFallos {
                throw .fueraDeRangoLista("FiltroSimetrias.cs:71")  // `PartidosSimetricos[0]`
            }
            // En `AnalizarFallosComb` una simetría vacía conserva el `esValida` de la anterior (:95-120).
            if valida { n += 1 }
        }
        return n
    }

    func analizar(_ c: Columna) throws(ErrorDominio) -> Bool {
        aciertos = 0
        aciertos = try cuenta(c, enFallos: false)
        return condicion.aciertos.contains(aciertos)
    }

    func analizarFallos(_ c: Columna) throws(ErrorDominio) -> [String]? {
        let ac = try cuenta(c, enFallos: true)
        return condicion.aciertos.contains(ac) ? nil : ["Fallo en Número de Aciertos (\(ac))"]
    }

    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int { 0 }
}
