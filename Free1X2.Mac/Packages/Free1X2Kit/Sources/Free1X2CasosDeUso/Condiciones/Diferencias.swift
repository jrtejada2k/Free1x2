import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Views/Ported/DiferenciasFrmViewModel.cs (09 pantalla 42; CU-B02). La pantalla trabaja sobre la condición **del
// grupo**: navegar entre conjuntos ya escribe en ella (Cancelar no lo deshace).

/// Lo que muestra la pantalla de un conjunto: líneas de partidos y los 6 conceptos (Variantes, Equis, Doses, Dibujos,
/// Interrupciones, Formatos).
public struct TextosDiferencia: Sendable, Hashable {
    public var lineas: [String]
    public var conceptos: [String]

    public init(lineas: [String] = Array(repeating: "", count: 20), conceptos: [String] = Array(repeating: "", count: 6)) {
        (self.lineas, self.conceptos) = (lineas, conceptos)
    }
}

extension CasosCondiciones {
    /// `ActivarTodos` (:159-164): `bool[n]` con el 0 a `false`.
    static func activarTodos(_ n: Int) -> [Bool] { (0..<max(n, 0)).map { $0 > 0 } }

    /// `CreaDiferencia` (:167-211): cualquier error o ninguna línea ⇒ `nil` (sin aviso).
    public static func crearDiferencia(_ t: TextosDiferencia, numeroPartidos: Int) -> Diferencia? {
        var d = Diferencia()
        do throws(ErrorDominio) {
            for l in t.lineas where !blanco(l) {
                d.partidosSimetricos.append(try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(l, longitud: numeroPartidos + 1))
            }
            let n = d.partidosSimetricos.count + 1
            var ac: [[Bool]?] = []
            for c in 0..<6 { ac.append(try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(t.conceptos[safe: c] ?? "", longitud: n)) }
            guard !d.partidosSimetricos.isEmpty else { return nil }
            d.analizaVX2Dib = !ac[0..<4].allSatisfy { $0 == nil }
            (d.acV, d.acX, d.acDoses, d.acDib) = (
                ac[0] ?? activarTodos(n), ac[1] ?? activarTodos(n), ac[2] ?? activarTodos(n), ac[3] ?? activarTodos(n)
            )
            d.analizaInterrupciones = ac[4] != nil
            d.acInt = ac[4] ?? activarTodos(n)
            d.analizaFormatos = ac[5] != nil
            d.acFormatos = ac[5] ?? activarTodos(n)
            return d
        } catch {
            return nil
        }
    }

    /// `MarcarValores` (:129-157): las líneas crecen hasta los sub-grupos del conjunto.
    public static func textosDiferencia(_ d: Diferencia, lineasMinimas: Int = 20) -> TextosDiferencia {
        var t = TextosDiferencia(lineas: Array(repeating: "", count: max(lineasMinimas, d.partidosSimetricos.count)))
        for (i, p) in d.partidosSimetricos.enumerated() { t.lineas[i] = UtilidadesEntradasValores.obtenerTextoFromBool(p ?? []) }
        func x(_ a: [Bool]?) -> String { UtilidadesEntradasValores.obtenerTextoFromBool(a ?? []) }
        if d.analizaVX2Dib {
            (t.conceptos[0], t.conceptos[1], t.conceptos[2], t.conceptos[3]) = (x(d.acV), x(d.acX), x(d.acDoses), x(d.acDib))
        }
        if d.analizaInterrupciones { t.conceptos[4] = x(d.acInt) }
        if d.analizaFormatos { t.conceptos[5] = x(d.acFormatos) }
        return t
    }

    /// `ActualizarDatos` (:214-226): sustituye el conjunto `indice` o lo añade; un conjunto no válido no borra nada.
    public static func actualizarDiferencias(_ f: inout CondicionDiferencias, indice: Int, _ t: TextosDiferencia, numeroPartidos: Int) {
        guard let s = crearDiferencia(t, numeroPartidos: numeroPartidos) else { return }
        if f.diferencias.count > indice { f.diferencias[indice] = s } else { f.diferencias.append(s) }
    }

    /// `ConjuntoSiguiente` (:338-370): nuevo índice y conjunto a mostrar (`mostrar == nil` = pantalla vacía); `nil` = un
    /// conjunto nuevo no válido no avanza ni cambia la pantalla.
    public static func siguienteDiferencia(_ f: inout CondicionDiferencias, indice: Int, _ t: TextosDiferencia, numeroPartidos: Int)
        -> (indice: Int, mostrar: Diferencia?)?
    {
        let s = crearDiferencia(t, numeroPartidos: numeroPartidos)
        if f.diferencias.count > indice {
            if let s { f.diferencias[indice] = s }
            let i = indice + 1
            return (i, f.diferencias[safe: i])
        }
        guard let s else { return nil }
        f.diferencias.append(s)
        return (indice + 1, nil)
    }

    /// `ConjuntoAnterior` (:373-396): `nil` = no hace nada; un conjunto guardado no válido no retrocede y se repinta.
    public static func anteriorDiferencia(_ f: inout CondicionDiferencias, indice: Int, _ t: TextosDiferencia, numeroPartidos: Int)
        -> (indice: Int, mostrar: Diferencia?)?
    {
        guard indice > 0 else { return nil }
        var i = indice
        if f.diferencias.count > indice {
            if let s = crearDiferencia(t, numeroPartidos: numeroPartidos) {
                f.diferencias[indice] = s
                i -= 1
            }
        } else {
            i -= 1
        }
        return (i, f.diferencias[safe: i])
    }

    /// `EliminarActual` (:399-425): siempre vuelve al conjunto 0.
    public static func eliminarDiferencia(_ f: inout CondicionDiferencias, indice: Int) -> Diferencia? {
        if f.diferencias.count > indice { f.diferencias.remove(at: indice) }
        return f.contieneDatos() ? f.diferencias.first : nil
    }

    /// `GenerarAtajos` (:307-335): solo las líneas vacías, en orden.
    public static func atajosDiferencias(_ atajo: String, pasoFijo: Bool, lineas: [String], numeroPartidos: Int) -> [String] {
        let cantidades = ["Dúos": 2, "Tríos": 3, "Cuartetos": 4, "Quintetos": 5, "Sextetos": 6, "Septetos": 7, "Octetos": 8]
        guard let c = cantidades[atajo] else { return lineas }
        var cadenas = UtilidadesEntradasValores.obtenerGruposDeValores(c, pasoFijo: pasoFijo, numeroPartidos: numeroPartidos)[...]
        return lineas.map { l in blanco(l) && !cadenas.isEmpty ? cadenas.removeFirst() : l }
    }

    /// `AbrirDesde` (:478-512): solo si el leído tiene datos; copia conjuntos, `ContieneDatos` e `IsActive`.
    public static func importarDiferencias(_ leido: CondicionDiferencias, en f: inout CondicionDiferencias) -> Bool {
        var l = leido
        guard l.contieneDatos() else { return false }
        f.diferencias = l.diferencias
        f.ponerContieneDatos(true)
        f.activa = l.activa
        return true
    }
}
