import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Views/Ported/PesosNumFrmViewModel.cs (10 pantalla 78; CU-C10).

extension CasosCondiciones {
    /// Las 5 casillas de figuras de la pantalla (sin «5» ni «4-1», que darían 0 columnas).
    public static let figurasPesos = ["3-2", "3-1-1", "2-2-1", "2-1-1-1", "1-1-1-1-1"]

    /// `VolcarA` (:229-272): las figuras (solo las 5 casillas) cuentan como datos; las tolerancias no. Con datos el
    /// nº de tolerancias nunca queda vacío («0»).
    public static func aplicarPesosNumericos(
        pesos: [String], tolerados: [String], numTolerancias: [Bool], figuras: [Bool], en f: inout CondicionPesosNumericos,
        numeroPartidos n: Int
    ) throws(ErrorDominio) {
        var figs: [Int64] = []
        for (i, m) in figuras.enumerated() where m {
            if let t = figurasPesos[safe: i] { figs.append(try UtilidadesEntradasValores.obtenerLongFiguraFromText(t)) }
        }
        let todos = todosValores(n)
        f.reinicializaValores()
        guard pesos.contains(where: { !blanco($0) }) || !figs.isEmpty else {
            f.activa = false
            f.contieneDatos = false
            return
        }
        if !f.contieneDatos { f.activa = true }
        f.contieneDatos = true
        for p in CondicionPesosNumericos.Peso.allCases {
            let t = pesos[safe: p.rawValue] ?? ""
            try f.setPN(p, blanco(t) ? todos : t)
        }
        if tolerados.contains(where: { !blanco($0) }) {
            for p in CondicionPesosNumericos.Peso.allCases { try f.setPNTol(p, tolerados[safe: p.rawValue] ?? "") }
            let tol = numTolerancias.indices.filter { numTolerancias[$0] }.map(String.init).joined(separator: ",")
            try f.ponerTolerancia(blanco(tol) ? "0" : tol)
        } else {
            try f.ponerTolerancia("0")
        }
        f.figuras = figs
    }

    /// `MarcarValores` (:103-128): casillas de figuras marcadas según `Figuras`.
    public static func figurasMarcadasPesos(_ f: CondicionPesosNumericos) -> [Bool] {
        figurasPesos.map { t in (try? UtilidadesEntradasValores.obtenerLongFiguraFromText(t)).map(f.figuras.contains) ?? false }
    }

    /// `GetTolerancias().Split(',')`: botones «0»…«5» marcados.
    public static func toleranciasMarcadasPesos(_ f: CondicionPesosNumericos) -> [Bool] {
        let t = Set(f.getTolerancias.split(separator: ",", omittingEmptySubsequences: false).map(String.init))
        return (0...5).map { t.contains(String($0)) }
    }
}
