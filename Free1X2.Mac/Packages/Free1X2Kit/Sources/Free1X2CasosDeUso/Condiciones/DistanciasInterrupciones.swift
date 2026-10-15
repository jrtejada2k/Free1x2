import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Views/Ported/DistanciasFrmViewModel.cs y InterrupcionesFrmViewModel.cs (09 pantallas 43 y 69; CU-B03, CU-B37).

extension CasosCondiciones {
    /// `Aceptar` de Distancias (:94-129): Var, 1, X, 2; vacío = `0…N`.
    public static func aplicarDistancias(_ textos: [String], en f: inout CondicionDistancias) throws(ErrorDominio) {
        let todos = todosValores(f.numeroPartidos)
        f.reinicializaValores()
        guard textos.contains(where: { !blanco($0) }) else {
            f.activa = false
            f.contieneDatos = false
            return
        }
        if !f.contieneDatos { f.activa = true }
        f.contieneDatos = true
        func t(_ i: Int) -> String { textos[safe: i].flatMap { blanco($0) ? nil : $0 } ?? todos }
        try f.setDistVariantes(t(0))
        try f.setDistUnos(t(1))
        try f.setDistEquis(t(2))
        try f.setDistDoses(t(3))
    }

    /// `Aceptar` de Interrupciones (:137-177): los 10 contadores en orden; vacío = la constante 0…14.
    public static func aplicarInterrupciones(_ textos: [String], en f: inout CondicionInterrupciones) throws(ErrorDominio) {
        let todos = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14"
        f.reinicializaValores()
        guard textos.contains(where: { !blanco($0) }) else {
            f.activa = false
            f.contieneDatos = false
            return
        }
        if !f.contieneDatos { f.activa = true }
        f.contieneDatos = true
        for c in CondicionInterrupciones.Contador.allCases {
            let t = textos[safe: c.rawValue] ?? ""
            try f.set(c, blanco(t) ? todos : t)
        }
    }
}
