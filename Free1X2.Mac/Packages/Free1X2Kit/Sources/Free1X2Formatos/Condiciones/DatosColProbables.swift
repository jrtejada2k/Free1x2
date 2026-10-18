import Free1X2Base
import Free1X2Motor

// C#: Free1X2.Domain/EntradaSalida/FColProbablesData.cs (07 F03.b; 04 §15).

/// `<condicion id="ColProbables">`: CPs, relaciones I/II/III y control de fallos.
enum DatosColProbables {
    /// `PonerCondicionesFiltro` (:32-95): despacha por `Name.ToUpper()`; `relacion3` usa las CPs ya leídas.
    static func leer(
        _ n: NodoXML, _ f: inout CondicionColProbables, _ activa: Bool, numeroPartidos: Int
    ) throws(ErrorDominio) {
        f.activa = activa
        f.contieneDatos = true
        var cps: [ColumnaProbableConfig] = []
        var r1: [RelacionCP1Config] = []
        var r2: [RelacionCP2Config] = []
        var r3: [RelacionCP3Config] = []
        var fallos: [CPControlFallosConfig] = []
        var controlFallos = ""
        for h in n.hijos {
            switch FormatoNET.mayusculas(h.nombre) {
            case "CP": cps.append(try cp(h, numeroPartidos))
            case "RELACION1": r1.append(try relacion1(h))
            case "FALLOSCP": fallos.append(try controlFallosCP(h))
            case "CONTROLFALLOS": controlFallos = try h.atributo("noFallos", "FColProbablesData.cs:72")
            case "RELACION2": r2.append(try relacion2(h))
            case "RELACION3": r3.append(try relacion3(h, cps))
            default: break
            }
        }
        f.ponerColumnas(cps)
        f.relaciones1 = r1
        f.relaciones2 = r2
        f.relaciones3 = r3
        f.controlesFallos = fallos
        try f.ponerFallosPermitidos(controlFallos)
    }

    /// `ObtenCP` (:97-121) con `ObtenPronosticosCP` (:342-370): `"1X"` ⇒ `"1,X"`.
    static func cp(_ n: NodoXML, _ numeroPartidos: Int) throws(ErrorDominio) -> ColumnaProbableConfig {
        let o = "FColProbablesData.cs:109-337"
        var cp = ColumnaProbableConfig(numeroPartidos: numeroPartidos)
        for g in n.hijos {
            switch g.nombre {
            case "pronostico":
                let p = trozos(try g.atributo("values", o), ",").map { $0.map(String.init).joined(separator: ",") }
                try cp.ponerPronosticos(p)
            case "standard":
                for x in g.hijos {
                    switch x.nombre {
                    case "aciertos": try cp.setNoAciertos(try x.atributo("values", o))
                    case "aciertosSeguidos": try cp.setNoAciertosSeguidos(try x.atributo("values", o))
                    case "fallosSeguidos": try cp.setNoFallosSeguidos(try x.atributo("values", o))
                    case "puntos": try cp.setPuntos(try x.atributo("values", o))
                    default: break
                    }
                }
            case "tolerancias":
                try cp.setTolerancias(try g.atributo("noTolerancias", o))
                for x in g.hijos {
                    switch x.nombre {
                    case "aciertosTol": try cp.setACTol(try x.atributo("values", o))
                    case "aciertosSeguidosTol": try cp.setACSTol(try x.atributo("values", o))
                    case "fallosSeguidosTol": try cp.setFSTol(try x.atributo("values", o))
                    default: break
                    }
                }
            default: break
            }
        }
        return cp
    }

    /// `ObtenRelacionCP1` (:123-152).
    static func relacion1(_ n: NodoXML) throws(ErrorDominio) -> RelacionCP1Config {
        let o = "FColProbablesData.cs:135-147"
        var r = RelacionCP1Config()
        for g in n.hijos {
            switch g.nombre {
            case "columnas": try r.ponerColumnas(try g.atributo("values", o))
            case "sumas": try r.ponerSumaAciertos(try g.atributo("values", o))
            case "recorridos": try r.ponerRecorridos(try g.atributo("values", o))
            case "gruposCP":
                try r.ponerCantidadCP(try g.atributo("cantidadCP", o))
                try r.ponerCuantosAC(try g.atributo("noAC", o))
            default: break
            }
        }
        return r
    }

    /// `ObtenRelacionCP2` (:154-204): los textos; las listas las deriva la ejecución.
    static func relacion2(_ n: NodoXML) throws(ErrorDominio) -> RelacionCP2Config {
        let o = "FColProbablesData.cs:166-200"
        var r = RelacionCP2Config()
        for g in n.hijos {
            switch g.nombre {
            case "columnasA": r.columnasA = try g.atributo("values", o)
            case "columnasB": r.columnasB = try g.atributo("values", o)
            case "aciertos": r.aciertos = try g.atributo("values", o)
            case "concepto": r.concepto = try g.atributo("value", o)
            case "cantidad": r.cantidad = try g.atributo("value", o)
            case "columnasA2": r.columnasA2 = try g.atributo("values", o)
            case "columnasB2": r.columnasB2 = try g.atributo("values", o)
            case "aciertos2": r.aciertos2 = try g.atributo("values", o)
            case "concepto2": r.concepto2 = try g.atributo("value", o)
            case "cantidad2": r.cantidad2 = try g.atributo("value", o)
            default: break
            }
        }
        return r
    }

    /// `ObtenRelacionCP3` (:206-262): `columnas` toma las **primeras** K CPs (P12) y lanza si K supera las leídas (DT-55).
    static func relacion3(_ n: NodoXML, _ cps: [ColumnaProbableConfig]) throws(ErrorDominio) -> RelacionCP3Config {
        let o = "FColProbablesData.cs:221-256"
        var r = RelacionCP3Config()
        var k: Int?
        let escaleras = cps.count / 3
        for g in n.hijos {
            switch g.nombre {
            case "columnas":
                let cols = try g.atributo("values", o)
                let indices = UtilidadesEntradasValores.obtenerListaFromTxt(cols)
                for i in indices.indices { _ = try cps.leerLista(i, "FColProbablesData.cs:720") }
                k = indices.count
                r.columnasImplicadas = cols
            case "sandwichs":
                guard let k else { continue }
                let t = try g.atributo("values", o)
                r.sandwichsTexto = t
                r.sandwichs = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(t, longitud: k / 4)
            case "escalerasAsc":
                let t = try g.atributo("values", o)
                r.escalerasAscTexto = t
                r.escalerasAsc = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(t, longitud: escaleras)
            case "escalerasDesc":
                let t = try g.atributo("values", o)
                r.escalerasDescTexto = t
                r.escalerasDesc = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(t, longitud: escaleras)
            case "escaleras":
                let t = try g.atributo("values", o)
                r.escalerasTexto = t
                r.escalerasTotales = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(t, longitud: escaleras)
            case "concepto":
                r.concepto = try g.atributo("values", o)
                r.conceptoTexto = r.concepto
            case "agrupacionesPasoFijo":
                let t = try g.atributo("values", o)
                r.agrupacionesPasoFijoTexto = t
                r.agrupacionesPasoFijo = try agrupaciones(t, maximo: cps.count + 1)
            case "agrupacionesSolapadas":
                let t = try g.atributo("values", o)
                r.agrupacionesSolapadasTexto = t
                r.agrupacionesSolapadas = try agrupaciones(t, maximo: cps.count + 1)
            default: break
            }
        }
        return r
    }

    /// `ObtenArrayAgrupaciones` (:724-750): `a+n+b` por `#`; solo con 3 partes, `n < maximo` y ≤ 15 aciertos; ninguna ⇒ `nil`.
    static func agrupaciones(_ texto: String, maximo: Int) throws(ErrorDominio) -> [AgrupacionColumnas]? {
        var r: [AgrupacionColumnas] = []
        for d in trozos(texto, "#") {
            let a = trozos(d, "+").filter { !$0.isEmpty }
            guard a.count == 3 else { continue }
            let valores = UtilidadesEntradasValores.obtenerListaFromTxtAciertos(a[0])
            let elementos = Int(try FormatoNET.convertToInt32(a[1]))
            let ac = UtilidadesEntradasValores.obtenerListaFromTxtAciertos(a[2])
            if elementos < maximo && ac.count <= 15 {
                r.append(AgrupacionColumnas(numero: valores, noElementos: elementos, noAciertos: ac))
            }
        }
        return r.isEmpty ? nil : r
    }

    /// `ObtenCPCrtlFallos` (:264-290).
    static func controlFallosCP(_ n: NodoXML) throws(ErrorDominio) -> CPControlFallosConfig {
        let o = "FColProbablesData.cs:276-284"
        var c = CPControlFallosConfig()
        for g in n.hijos {
            switch g.nombre {
            case "columnas": try c.ponerColumnas(try g.atributo("values", o))
            case "tolerancias": c.tolerancias = try g.atributo("values", o)
            case "aciertos": try c.ponerAciertos(try g.atributo("values", o))
            default: break
            }
        }
        return c
    }

    // MARK: Escritura (:372-712)

    static func escribir(_ f: CondicionColProbables, _ w: inout EscritorXMLNET) throws(ErrorDominio) {
        func valores(_ nombre: String, _ v: String, _ clave: String = "values") {
            DatosFiltro.elemento(&w, nombre, [(clave, v)])
        }
        for (i, cp) in f.columnas.enumerated() {
            w.iniciarElemento("cp")
            w.atributo("id", String(i))
            valores("pronostico", cp.textoPronosticos.map { $0.replacingOccurrences(of: ",", with: "") }.joined(separator: ","))
            w.iniciarElemento("standard")
            valores("aciertos", cp.getAciertos)
            valores("aciertosSeguidos", cp.getAciertosSeguidos)
            valores("fallosSeguidos", cp.getFallosSeguidos)
            valores("puntos", cp.puntos.texto)
            w.cerrarElemento()
            if cp.toleranciaLocalActiva {
                w.iniciarElemento("tolerancias")
                w.atributo("noTolerancias", cp.getTolerancias)
                valores("aciertosTol", cp.getACTol)
                valores("aciertosSeguidosTol", cp.getACSTol)
                valores("fallosSeguidosTol", cp.getFSTol)
                w.cerrarElemento()
            }
            w.cerrarElemento()
        }
        for (i, r) in f.relaciones1.enumerated() {
            w.iniciarElemento("relacion1")
            w.atributo("id", String(i))
            valores("columnas", r.columnas)
            valores("sumas", r.sumaAciertos)
            valores("recorridos", r.recorridos)
            DatosFiltro.elemento(&w, "gruposCP", [("cantidadCP", r.cantidadCP.texto), ("noAC", r.cuantosAC.texto)])
            w.cerrarElemento()
        }
        for (i, r) in f.relaciones2.enumerated() {
            w.iniciarElemento("relacion2")
            w.atributo("id", String(i))
            valores("columnasA", r.columnasA)
            valores("columnasB", r.columnasB)
            valores("aciertos", r.aciertos)
            valores("concepto", r.concepto, "value")
            valores("cantidad", r.cantidad, "value")
            valores("columnasA2", r.columnasA2)
            valores("columnasB2", r.columnasB2)
            valores("aciertos2", r.aciertos2)
            valores("concepto2", r.concepto2, "value")
            valores("cantidad2", r.cantidad2, "value")
            w.cerrarElemento()
        }
        for (i, c) in f.controlesFallos.enumerated() {
            w.iniciarElemento("fallosCP")
            w.atributo("id", String(i))
            valores("columnas", c.columnas)
            valores("tolerancias", c.tolerancias)
            valores("aciertos", c.aciertos)
            w.cerrarElemento()
        }
        DatosFiltro.elemento(&w, "controlFallos", [("noFallos", f.fallosPermitidos.texto)])
        for (i, r) in f.relaciones3.enumerated() {
            w.iniciarElemento("relacion3")
            w.atributo("id", String(i))
            valores("columnas", r.columnasImplicadas)
            valores("sandwichs", r.sandwichsTexto ?? "")
            valores("escalerasAsc", r.escalerasAscTexto ?? "")
            valores("escalerasDesc", r.escalerasDescTexto ?? "")
            valores("escaleras", r.escalerasTexto ?? "")
            valores("concepto", r.conceptoTexto ?? "")
            valores("agrupacionesPasoFijo", r.agrupacionesPasoFijoTexto ?? "")
            valores("agrupacionesSolapadas", r.agrupacionesSolapadasTexto ?? "")
            w.cerrarElemento()
        }
    }
}
