import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/IFiltro.cs:25-40 (01 §4.1).

/// Un filtro en ejecución: guarda el estado de su último `analizar`, que leen las tolerancias y el análisis.
protocol FiltroColumna: AnyObject {
    var tipo: TipoFiltro { get }
    var activa: Bool { get }
    var analisisActivo: Bool { get }
    /// `Analizar`: reinicia los contadores y decide.
    func analizar(_ columna: Columna) throws(ErrorDominio) -> Bool
    /// `AnalizarFallos`: `nil` = sin fallos; `*` acierto, `!` tolerancia, otro = fallo (01 §4.1.3).
    func analizarFallos(_ columna: Columna) throws(ErrorDominio) -> [String]?
    /// `ObtenNoAciertosTolerancias(letras)` sobre el estado del último `analizar`.
    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int
}

extension FiltroColumna {
    /// Texto de fallos `"a#b#"` → `["a", "b"]`; vacío ⇒ `nil` (patrón de todos los `AnalizarFallos`).
    static func fallos(_ texto: String) -> [String]? {
        guard !texto.isEmpty else { return nil }
        return trozos(texto.dropLast(), "#")
    }
}
