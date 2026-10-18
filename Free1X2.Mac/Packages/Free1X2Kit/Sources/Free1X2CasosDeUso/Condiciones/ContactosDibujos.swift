import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Views/Ported/ContactosFrmViewModel.cs y DibujosFrmViewModel.cs (08 pantallas 26 y 40; CU-A67, CU-A90).

extension CasosCondiciones {
    /// `GuardarDatos` de Contactos (:136-171): vacío = `0…15` (con N = 14 el 15 se sale del array y lanza, como hoy);
    /// las figuras cuentan como datos y solo se ponen si hay.
    public static func aplicarContactos(_ filas: [String], figuras: [Int64]?, en f: inout CondicionContactos) throws(ErrorDominio) {
        guard filas.contains(where: { !blanco($0) }) || !(figuras ?? []).isEmpty else {
            f.activa = false
            f.contieneDatos = false
            return
        }
        let todos = (0...15).map(String.init).joined(separator: ",")
        if !f.contieneDatos { f.activa = true }
        f.contieneDatos = true
        f.reinicializaValores()
        for c in CondicionContactos.Contacto.allCases {
            let t = filas[safe: c.rawValue] ?? ""
            try f.set(c, blanco(t) ? todos : t)
        }
        if let figuras, !figuras.isEmpty { f.figuras = figuras }
    }

    /// `GenerarCasillas` (:134-146): `X+Y` con `X = 0…N` e `Y = 0…N−X` (A17: N de la configuración, no 15 fijo).
    public static func casillasDibujos(_ n: Int) -> [String] {
        (0...max(n, 0)).flatMap { x in (0...(n - x)).map { "\(x)+\($0)" } }
    }

    /// `Aceptar` de Dibujos (:178-209): con selección siempre activa; sin ella, vacía e inactiva.
    public static func aplicarDibujos(_ marcadas: [String], en f: inout CondicionDibujos) throws(ErrorDominio) {
        if marcadas.isEmpty {
            f.activa = false
            f.contieneDatos = false
        } else {
            f.contieneDatos = true
            f.activa = true
        }
        try f.ponerDibujos(marcadas)
    }
}
