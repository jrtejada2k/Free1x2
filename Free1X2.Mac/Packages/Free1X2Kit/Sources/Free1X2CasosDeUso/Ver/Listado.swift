import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Views/Ported/ListadoCondicionesFrmViewModel.cs (09 pantalla 71; CU-B39). Textos literales del legacy, erratas incluidas.

/// Nodo del árbol de condiciones; `hijos == nil` en las hojas (para `OutlineGroup`).
public struct NodoListado: Sendable, Hashable, Identifiable {
    public let id: Int
    public let texto: String
    public let hijos: [NodoListado]?
}

extension CasosVer {
    private final class N {
        let texto: String
        var hijos: [N] = []

        init(_ t: String) { texto = t }

        @discardableResult func hijo(_ t: String) -> N {
            let n = N(t)
            hijos.append(n)
            return n
        }

        func siTiene(_ prefijo: String, _ v: String) { if !v.isEmpty { hijo(prefijo + v) } }

        func congelar(_ siguiente: inout Int) -> NodoListado {
            siguiente += 1
            let id = siguiente
            let h = hijos.map { $0.congelar(&siguiente) }
            return NodoListado(id: id, texto: texto, hijos: h.isEmpty ? nil : h)
        }
    }

    /// `Reconstruir`: pronóstico base, filtro de columnas, grupos con sus condiciones activas, controles e If-Then.
    public static func listadoCondiciones(_ c: Combinacion, archivoFiltro: String, cultura: Cultura) -> NodoListado {
        let raiz = N("Listado de Condiciones")
        let pron = raiz.hijo("Pronóstico Base")
        for (i, p) in (c.pronosticos ?? []).enumerated() { pron.hijo("Partido \(i + 1): " + (p ?? "")) }
        raiz.hijo("Filtro de Columnas").hijo(archivoFiltro.isEmpty ? "No se usa ningún Filtro" : archivoFiltro)
        let grupos = raiz.hijo("Grupos")
        for (i, g) in c.grupos.enumerated() {
            let ng = grupos.hijo(i == 0 ? "Boleto Base" : "Grupo \(i)")
            ng.hijo("Partidos Activos: " + g.textoPartidosActivos)
            if !g.archivoFiltro.isEmpty { ng.hijo("Filtro Parcial: " + g.archivoFiltro) }
            let nc = ng.hijo("Condiciones")
            for t in TipoFiltro.allCases where g.condiciones.estado(t).activa {
                detalle(t, g.condiciones, nc.hijo(t.rawValue), cultura)
            }
        }
        let control = raiz.hijo("Control de grupos")
        for k in c.controlesGrupos.dropFirst() {
            let n = control.hijo("Control de Grupos")
            n.hijo("Grupos: " + k.textoGruposControlados)
            n.hijo("Fallos: " + k.textoFallosPermitidos)
        }
        for k in c.controlesConjuntos {
            let n = control.hijo("Control Conjuntos")
            n.hijo("Conjuntos: " + k.textoCtrlGruposControlados)
            n.hijo("Fallos: " + k.textoFallosPermitidos)
        }
        let it = raiz.hijo("IfThen")
        if let f = c.ifThen, f.activo {
            let conds = it.hijo("Condiciones Relacionadas")
            for k in f.condiciones {
                let n = conds.hijo("Condición")
                n.hijo("Si se da: " + k.condIf)
                n.hijo("Entonces: " + k.condThen)
            }
            conds.hijo("Condiciones que se cumplen: " + (f.rangoAciertoCondiciones ?? ""))
            if !f.grupos.isEmpty {
                let gs = it.hijo("Grupos Relacionados")
                for k in f.grupos {
                    let n = gs.hijo("Grupos")
                    n.hijo("Si el grupo \(k.numGrupoIf) es " + (k.noIf ? "True" : "False"))
                    n.hijo("Entonces el grupo: \(k.numGrupoThen) debe ser " + (k.noThen ? "True" : "False"))
                }
                gs.hijo("Grupos que se cumplen: " + (f.rangoAciertoGrupos ?? ""))
            }
        } else {
            it.hijo("IfThen no activado")
        }
        var siguiente = 0
        return raiz.congelar(&siguiente)
    }

    /// `ConstruirDetalleFiltro`.
    private static func detalle(_ t: TipoFiltro, _ c: CondicionesGrupo, _ n: N, _ cultura: Cultura) {
        func d(_ v: Double) -> String { FormatoNET.toString(v, cultura: cultura) }
        switch t {
        case .noVariantes:
            n.hijo("Cantidad de V: " + c.noVariantes.getVariantes)
            n.hijo("Cantidad de X: " + c.noVariantes.getEquis)
            n.hijo("Cantidad de 2: " + c.noVariantes.getDoses)
        case .signosSeguidos:
            let s = c.signosSeguidos
            n.hijo("Cantidad de V Seguidas: " + s.get(.variantes))
            n.hijo("Cantidad de 1 Seguidos: " + s.get(.unos))
            n.hijo("Cantidad de X Seguidas: " + s.get(.equis))
            n.hijo("Cantidad de 2 Seguidos: " + s.get(.doses))
        case .dibujos:
            n.hijo("Dibujos : " + c.dibujos.getDibujos)
        case .colProbables:
            columnasProbables(c.colProbables, n)
        case .interrupciones:
            let s = c.interrupciones
            n.hijo("Interrupciones Globales: " + s.get(.global))
            n.hijo("Interrupciones de 1: " + s.get(.unos))
            n.hijo("Interrupciones de X: " + s.get(.equis))
            n.hijo("Interrupciones de 2 " + s.get(.doses))
            n.hijo("Interrupciones Globales Seguidas: " + s.get(.globalSeg))
            n.hijo("Interrupciones de 1 Seguidas: " + s.get(.unosSeg))
            n.hijo("Interrupciones de X Seguidas: " + s.get(.equisSeg))
            n.hijo("Interrupciones de 2 Seguidas: " + s.get(.dosesSeg))
        case .pesosNumericos:
            let p = c.pesosNumericos
            let nombres = ["Global", "Variantes", "1", "X", "2"]
            for (k, peso) in CondicionPesosNumericos.Peso.allCases.enumerated() { n.hijo("Peso \(nombres[k]): " + p.getPN(peso)) }
            for (k, peso) in CondicionPesosNumericos.Peso.allCases.enumerated() {
                n.siTiene("Tolerancias Peso \(nombres[k]): ", p.getPNTol(peso))
            }
            n.siTiene("Número de Tolerancias: ", p.getTolerancias)
        case .valoracionSignos:
            let v = c.valoracionSignos
            let nv = n.hijo("Valoración")
            for z in v.valores1.indices { nv.hijo(d(v.valores1[z]) + "-" + d(v.valoresX[z]) + "-" + d(v.valores2[z])) }
            n.hijo("Límites Valoración Global: " + v.valorGlobal)
            n.hijo("Límites Valoración 1: " + v.valorUnos)
            n.hijo("Límites Valoración X: " + v.valorEquis)
            n.hijo("Límites Valoración 2: " + v.valorDoses)
            n.hijo("Tipo de Valoración: " + v.tipoValoracion)
        case .distancias:
            n.hijo("Distancias de Variantes: " + c.distancias.getIntVar)
            n.hijo("Distancias de 1: " + c.distancias.getInt1)
            n.hijo("Distancias de X: " + c.distancias.getIntX)
            n.hijo("Distancias de 2: " + c.distancias.getInt2)
        case .gruposEquipos:
            let relaciones = N("Relaciones")
            for (z, g) in c.gruposEquipos.grupos.enumerated() {
                let nc = n.hijo("Conjunto \(z + 1)")
                let m = nc.hijo("Equipos Marcados")
                for ch in g.pronosticos {
                    switch ch {
                    case "1": m.hijo("Marcado - Desmarcado")
                    case "2": m.hijo("Desmarcado - Marcado")
                    case "3": m.hijo("Marcado - Marcado")
                    case "0": m.hijo("Desmarcado - Desmarcado")
                    default: break
                    }
                }
                nc.hijo("Victorias: " + g.victorias.texto)
                nc.hijo("Empates: " + g.empates.texto)
                nc.hijo("Derrotas: " + g.derrotas.texto)
                nc.hijo("Suma Puntos: " + g.sumaPuntos.texto)
            }
            for r in c.gruposEquipos.relaciones {
                let nr = relaciones.hijo("Grupos de Equipos: " + r.gruposEquipos)
                nr.hijo("Suma Victorias: " + r.sumaVictorias.texto)
                nr.hijo("Suma Empates: " + r.sumaEmpates.texto)
                nr.hijo("Suma Derrotas: " + r.sumaDerrotas.texto)
                nr.hijo("Suma Puntos: " + r.sumaPuntos.texto)
            }
            n.hijos.append(relaciones)
        case .contactos:
            let nombres = ["1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV"]
            for (k, x) in CondicionContactos.Contacto.allCases.enumerated() { n.hijo("Contactos \(nombres[k]): " + c.contactos.get(x)) }
        case .formatosSignos:
            let nf = n.hijo("Conjuntos")
            for fs in c.formatosSignos.formatosSignos {
                let conj = nf.hijo("Conjunto")
                for f in fs.lineasFormatos { conj.hijo("Formato: " + f.formato).hijo("Rango: " + f.rangoAparicion) }
                conj.hijo("Lineas: " + fs.lineas)
                // [PRESERVAR] «Global» muestra `Lineas`.
                conj.hijo("Global: " + fs.lineas)
            }
        case .formatos123:
            let f = c.formatos123
            let nv = n.hijo("Valoración")
            for v in f.valoracion { nv.hijo(v.prefix(3).map(d).joined(separator: "-")) }
            let nf = n.hijo("Formatos")
            for x in f.formatos {
                let h = nf.hijo("Formato 123: " + x.formato)
                if !f.pasoFijo { h.hijo("Límites: \(x.aciertosMin)-\(x.aciertosMax)") }
            }
            nf.hijo((f.pasoFijo ? "Aciertos Globales: " : "Líneas Acertadas: ") + (f.aciertosFiltro ?? ""))
            nf.hijo(f.pasoFijo ? "Modo: No Admitiendo Repeticiones" : "Modo: Admitiendo Repeticiones")
        case .simetrias:
            let ns = n.hijo("Simetrías")
            for s in c.simetrias.simetrias { ns.hijo("Simetría: " + s.partidos) }
            n.hijo("Aciertos: " + c.simetrias.aciertosTexto)
        case .diferencias:
            func b(_ l: [Bool]?) -> String { UtilidadesEntradasValores.obtenerTextoFromBool(l ?? []) }
            for (z, dif) in c.diferencias.diferencias.enumerated() {
                let nd = n.hijo("Diferencia \(z + 1)")
                for p in dif.partidosSimetricos { nd.hijo("Partidos: " + b(p)) }
                let v = nd.hijo("Diferencias")
                v.hijo("Variantes: " + b(dif.acV))
                v.hijo("Equis: " + b(dif.acX))
                v.hijo("Doses: " + b(dif.acDoses))
                v.hijo("Dibujos: " + b(dif.acDib))
                v.hijo("Interrupciones: " + b(dif.acInt))
                v.hijo("Formatos: " + b(dif.acFormatos))
            }
        }
    }

    /// `ConstruirDetalleColProbables`. [PRESERVAR] «Aciertos» de Relaciones muestra `CantidadCP`.
    private static func columnasProbables(_ f: CondicionColProbables, _ n: N) {
        let ncp = n.hijo("Columnas Probables")
        for cp in f.columnas {
            let h = ncp.hijo("Columna Probable: " + cp.textoPronosticos.map { $0.isEmpty ? "*" : $0 }.joined(separator: ","))
            h.siTiene("Aciertos: ", cp.getAciertos)
            h.siTiene("Aciertos Seguidos: ", cp.getAciertosSeguidos)
            h.siTiene("Fallos Seguidos: ", cp.getFallosSeguidos)
            h.siTiene("Tolerancias Aciertos: ", cp.getACTol)
            h.siTiene("Tolerancias Aciertos Seguidos: ", cp.getACSTol)
            h.siTiene("Tolerancias Fallos Seguidos: ", cp.getFSTol)
            h.siTiene("Aciertos en Tolerancias: ", cp.getTolerancias)
            h.siTiene("Puntos: ", cp.puntos.texto)
        }
        let r1 = n.hijo("Relaciones")
        for r in f.relaciones1 {
            let h = r1.hijo("Columnas: " + r.columnas)
            h.hijo("Cuantas: " + r.cantidadCP.texto)
            h.hijo("Aciertos: " + r.cantidadCP.texto)
            h.hijo("Suma Aciertos: " + r.sumaAciertos)
            h.hijo("Recorrido: " + r.recorridos)
        }
        let r2 = n.hijo("Relaciones II")
        for r in f.relaciones2 {
            let h = r2.hijo("Relación II")
            h.hijo("Las columnas: \(r.columnasA) tendrán \(r.aciertos) \(r.concepto) \(r.cantidad) que las columnas \(r.columnasB)")
            h.hijo("Las columnas: \(r.columnasA2) tendrán \(r.aciertos2) \(r.concepto2) \(r.cantidad2) que las columnas \(r.columnasB2)")
        }
        let r3 = n.hijo("Relaciones III")
        for r in f.relaciones3 {
            let h = r3.hijo("Relación III")
            h.hijo("Columnas: " + r.columnasImplicadas)
            h.hijo("Concepto: " + (r.conceptoTexto ?? ""))
            h.hijo("Sandwichs: " + (r.sandwichsTexto ?? ""))
            let e = h.hijo("Escaleras")
            e.hijo("Totales: " + (r.escalerasTexto ?? ""))
            e.hijo("Ascendentes: " + (r.escalerasAscTexto ?? ""))
            e.hijo("Descendentes: " + (r.escalerasDescTexto ?? ""))
            for (titulo, lista) in [
                ("Agrupaciones Paso Fijo", r.agrupacionesPasoFijo), ("Agrupaciones Solapadas", r.agrupacionesSolapadas),
            ] {
                let a = h.hijo(titulo).hijo("Agrupación")
                for g in lista ?? [] {
                    a.hijo("Número: " + UtilidadesEntradasValores.obtenerTextoFromListAciertos(g.numero))
                    a.hijo("Elementos: \(g.noElementos)")
                    a.hijo("Aciertos: " + UtilidadesEntradasValores.obtenerTextoFromListAciertos(g.noAciertos))
                }
            }
        }
        let cf = n.hijo("Control de Fallos")
        for k in f.controlesFallos {
            let h = cf.hijo("Control Fallos")
            h.hijo("Columnas: " + k.columnas)
            h.hijo("Tolerancia: " + k.tolerancias)
            h.hijo("Aciertos: " + k.aciertos)
        }
        cf.hijo("Numero de Fallos de Controles: " + f.fallosPermitidos.texto)
    }

    /// `ExportarATexto`: sangría = profundidad; un nodo con hijos lleva línea en blanco detrás y otra al cerrar.
    public static func listadoTexto(_ nodo: NodoListado, profundidad: Int = 0) -> String {
        var s = ""
        for h in nodo.hijos ?? [] {
            s += String(repeating: " ", count: profundidad) + h.texto
            if h.hijos != nil {
                s += "\r\n\r\n" + listadoTexto(h, profundidad: profundidad + 1) + "\r\n"
            } else {
                s += "\r\n"
            }
        }
        return s
    }

    /// `ExportarAHtml`, envuelto en la página del C#.
    public static func listadoHtml(_ raiz: NodoListado) -> String {
        func cuerpo(_ nodo: NodoListado, _ p: Int) -> String {
            var s = ""
            for h in nodo.hijos ?? [] {
                if h.hijos != nil {
                    s += (p == 0 ? "<br><b>" : p == 1 ? "<i>" : "") + h.texto + (p == 0 ? "</b><br>" : p == 1 ? "</i>" : "") + "<br>"
                    s += cuerpo(h, p + 1)
                } else {
                    s += h.texto + "<br>"
                }
            }
            return s
        }
        return "<html><head><title>Listado de Condiciones</title></head><body bgcolor=\"#DBFEBC\">" + cuerpo(raiz, 0) + "</body></html>"
    }
}
