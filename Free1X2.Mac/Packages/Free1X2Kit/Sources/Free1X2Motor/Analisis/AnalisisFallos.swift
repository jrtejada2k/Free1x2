import Foundation
import Free1X2Base

// C#: Free1X2.WinUI/Views/Ported/AnalizarCombinacionFrmViewModel.cs:188-467 (08 pantalla 7; CU-A12, CU-A13) sobre el
// grafo de ejecución del motor: `AnalizarFallos` de cada filtro activo, tolerancias, controles e If-Then.

/// Estado de una condición en el árbol: verde, neutro (aceptada por tolerancias) o rojo.
public enum EstadoNodo: Sendable, Hashable {
    case acertada, tolerancias, fallada
}

/// Nodo del árbol de fallos; `hijos == nil` en las hojas (para `OutlineGroup`).
public struct NodoAnalisis: Sendable, Hashable, Identifiable {
    public let id: Int
    public let titulo: String
    public let estado: EstadoNodo
    public let hijos: [NodoAnalisis]?
}

public enum AnalisisFallos {
    /// Construcción mutable (el C# cambia estados de nodos ya añadidos).
    private final class N {
        var titulo: String
        var estado: EstadoNodo
        var hijos: [N] = []

        init(_ titulo: String, _ estado: EstadoNodo = .acertada) { (self.titulo, self.estado) = (titulo, estado) }

        func congelar(_ siguiente: inout Int) -> NodoAnalisis {
            siguiente += 1
            let id = siguiente
            let h = hijos.map { $0.congelar(&siguiente) }
            return NodoAnalisis(id: id, titulo: titulo, estado: estado, hijos: h.isEmpty ? nil : h)
        }
    }

    /// `BuscarFiltro`.
    static func nombreVisible(_ t: TipoFiltro) -> String {
        switch t {
        case .colProbables: "Columnas probables"
        case .formatosSignos: "Formatos de Signos"
        case .gruposEquipos: "Grupos de Equipos"
        case .interrupciones: "Interrupciones"
        case .noVariantes: "Variantes, X y 2"
        case .pesosNumericos: "Pesos Numericos"
        case .signosSeguidos: "Signos Seguidos"
        case .valoracionSignos: "Valoracion de Signos"
        default: t.rawValue
        }
    }

    /// CU-A13 `EvaluarPronosticos`: partidos (1-based, de mayor a menor) cuyo signo no está en el pronóstico; `nil` si
    /// ninguno. Recorre la columna hasta su último grupo de 3 bits no nulo.
    public static func evaluarPronosticos(_ columna: Columna, _ pronosticos: [String]) throws(ErrorDominio) -> [Int]? {
        var c = columna
        var partido = 0
        var fallos: [Int] = []
        while c != 0 {
            let signo = UInt8(truncatingIfNeeded: c & 7)
            let p = UtilColumnas.convPartidoStrToByte(
                try pronosticos.leer(partido, "AnalizarCombinacionFrmViewModel.cs:EvaluarPronosticos"))
            if signo & p != signo { fallos.insert(partido + 1, at: 0) }
            c >>= 3
            partido += 1
        }
        return fallos.isEmpty ? nil : fallos
    }

    /// CU-A12 `AnalizarCombinacion`: hasta cuatro raíces (combinación, control de grupos, control de conjuntos y
    /// condiciones relacionadas). El grupo con algún filtro con líneas vuelve a `acertada` antes de fallar (A4, se replica).
    public static func analizar(
        _ c: Combinacion, columna: Columna, pronosticosBase: [String], configuracion: Configuracion, cultura: Cultura,
        leerLineas: @escaping LectorLineas
    ) throws(ErrorDominio) -> [NodoAnalisis] {
        let o = "AnalizarCombinacionFrmViewModel.cs"
        let a = Analizador(c, configuracion: configuracion, cultura: cultura, leerLineas: leerLineas)
        var raices: [N] = []
        let base = N("Combinación")
        raices.append(base)
        var grupos = [Bool](repeating: false, count: a.grupos.count)
        var hayFallos = false
        for (i, g) in a.grupos.enumerated() {
            var texto = "Grupo \(i)"
            let nombre = c.grupos[i].nombreGrupo
            if i == 0 { texto = "Boleto base" } else if !nombre.isEmpty { texto += " (\(nombre))" }
            let p = N(texto)
            base.hijos.append(p)
            if i == 0 {
                let s = N("Pronóstico de partidos")
                if let fallos = try evaluarPronosticos(columna, pronosticosBase) {
                    (s.estado, base.estado, p.estado) = (.fallada, .fallada, .fallada)
                    p.hijos.append(s)
                    for n in fallos { s.hijos.append(N("Partido \(n)  (" + (try pronosticosBase.leer(n - 1, o)) + ")", .fallada)) }
                } else {
                    p.hijos.append(s)
                }
            }
            let col = columna & g.mascara
            for f in g.filtros where f.activa {
                let s = N(nombreVisible(f.tipo))
                let lineas = try f.analizarFallos(col)
                p.hijos.append(s)
                guard let lineas else { continue }
                (s.estado, p.estado) = (.acertada, .acertada)
                for m in lineas {
                    guard let marca = m.first else { throw .fueraDeRango(o + ":Substring") }
                    switch marca {
                    case "*":
                        s.hijos.append(N(String(m.dropFirst())))
                        if f.tipo == .pesosNumericos || f.tipo == .colProbables, s.estado == .tolerancias { s.estado = .acertada }
                    case "!":
                        s.hijos.append(N(String(m.dropFirst()), .tolerancias))
                        if s.estado != .fallada { s.estado = .tolerancias }
                    default:
                        s.hijos.append(N(m, .fallada))
                        (s.estado, p.estado, base.estado) = (.fallada, .fallada, .fallada)
                    }
                }
            }
            if g.hayTolerancias && !g.esGrupoBase {
                if try g.toleranciasValidas() {
                    p.estado = .acertada
                } else {
                    p.estado = .fallada
                    p.hijos.append(N("Tolerancias Globales - Fallo en tolerancias Globales", .fallada))
                }
            }
            grupos[i] = p.estado != .fallada
            if !grupos[i] { hayFallos = true }
        }
        if hayFallos { base.estado = .fallada }

        let controles = a.controlador.controles
        if controles.count > 1 {
            let cg = N("Control de grupos")
            raices.append(cg)
            var conjuntos = [Bool](repeating: false, count: controles.count)
            hayFallos = false
            for i in 1..<controles.count {
                let k = controles[i]
                var nf = 0
                for g in k.gruposControlados where !(try grupos.leer(g, o)) { nf += 1 }
                guard let permitidos = k.fallosPermitidos else { throw .referenciaNula(o + ":AnalizaFallosGrupos") }
                let estado: EstadoNodo
                if try permitidos.leer(nf, o) {
                    conjuntos[i] = true
                    estado = .acertada
                    for g in k.gruposControlados {
                        let h = try base.hijos.leerLista(g, o)
                        if h.estado == .fallada { h.estado = .tolerancias }
                    }
                } else {
                    hayFallos = true
                    estado = .fallada
                }
                cg.hijos.append(N("Control de Grupos \(i)", estado))
            }
            if hayFallos { cg.estado = .fallada } else if grupos.first == true { base.estado = .tolerancias }

            let cc = a.controlador.conjuntos
            if cc.count > 1 {
                let nc = N("Control de conjuntos")
                raices.append(nc)
                hayFallos = false
                for i in 1..<cc.count {
                    let k = cc[i]
                    var nf = 0
                    for x in k.ctrlGruposControlados where !(try conjuntos.leer(x, o)) { nf += 1 }
                    guard let permitidos = k.fallosPermitidos else { throw .referenciaNula(o + ":AnalizaFallosConjuntos") }
                    let estado: EstadoNodo
                    if try permitidos.leer(nf, o) {
                        estado = .acertada
                        for x in k.ctrlGruposControlados {
                            let h = try cg.hijos.leerLista(x - 1, o)
                            if h.estado == .fallada { h.estado = .tolerancias }
                        }
                    } else {
                        hayFallos = true
                        estado = .fallada
                    }
                    nc.hijos.append(N("Control de Conjuntos \(i)", estado))
                }
                if hayFallos {
                    nc.estado = .fallada
                } else {
                    if grupos.first == true { base.estado = .tolerancias }
                    cg.estado = .tolerancias
                }
            }
        }

        if let it = a.ifThen, !it.esVacio, it.activo {
            let r = N("Condiciones relacionadas")
            raices.append(r)
            func anadir(_ titulo: String, _ errores: [String]) throws(ErrorDominio) {
                let q = N(titulo)
                r.hijos.append(q)
                var falla = false
                for e in errores {
                    guard let marca = e.first else { throw .fueraDeRango(o + ":AgregarNodosIfThen") }
                    switch marca {
                    case "*": q.hijos.append(N(String(e.dropFirst())))
                    case "!": q.hijos.append(N(String(e.dropFirst()), .tolerancias))
                    default:
                        falla = true
                        q.hijos.append(N(e, .fallada))
                    }
                }
                if falla { (q.estado, r.estado) = (.fallada, .fallada) }
            }
            if !it.condiciones.isEmpty {
                var errores: [String] = []
                for (i, cond) in it.condiciones.enumerated() where !(try cond.comprueba(columna)) {
                    errores.append("!Fallo en condición relacionada \(i + 1)")
                }
                if !errores.isEmpty {
                    guard let ac = it.aciertoCondiciones else { throw .referenciaNula("ControladorIfThen.cs:313") }
                    errores.append(
                        try ac.leer(it.condiciones.count - errores.count, "ControladorIfThen.cs:313")
                            ? "*Rango de aciertos de condiciones acertado" : "Rango de aciertos de condiciones fallado")
                }
                try anadir("Condiciones sencillas", errores)
            }
            if !it.grupos.isEmpty {
                var errores: [String] = []
                for (i, g) in it.grupos.enumerated() where !(try ControladorIfThen.comprueba(g, columna, a.grupos)) {
                    errores.append("!Fallo en grupo relacionado \(i + 1)")
                }
                if !errores.isEmpty {
                    guard let ac = it.aciertoGrupos else { throw .referenciaNula("ControladorIfThen.cs:340") }
                    errores.append(
                        try ac.leer(it.grupos.count - errores.count, "ControladorIfThen.cs:340")
                            ? "*Rango de aciertos de grupos acertado" : "Rango de aciertos de grupos fallado")
                }
                try anadir("Grupos relacionados", errores)
            }
        }
        var siguiente = 0
        return raices.map { $0.congelar(&siguiente) }
    }
}
