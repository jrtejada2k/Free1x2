import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_08 §19 y pantallas 29/30/48/67 (CU-A44…A51).
@Suite("Columnas probables")
struct ColumnasProbablesTests {
    let todos = CasosCondiciones.todosValores(14)

    func cp(_ p: String, aciertos: String? = nil) throws -> ColumnaProbableConfig {
        var c = ColumnaProbableConfig(numeroPartidos: 14)
        try c.ponerPronosticos(p.map(String.init))
        try c.setNoAciertos(aciertos ?? todos)
        try c.setNoAciertosSeguidos(todos)
        try c.setNoFallosSeguidos(todos)
        return c
    }

    func tres() throws -> CondicionColProbables {
        var f = CondicionColProbables()
        f.ponerColumnas([try cp("11111111111111"), try cp("XXXXXXXXXXXXXX"), try cp("22222222222222")])
        return f
    }

    @Test func navegacionGuardadoYAceptar() throws {
        var e = EditorColumnasProbables(numeroPartidos: 14, desplazamiento: 3)
        e.pronostico = "1X21X21X21X21X"
        e.aciertos = "5,6a"
        try e.siguiente()  // sin CPs «>» acota a 0: guarda y se queda en la primera
        #expect(e.columnas.count == 1 && e.actual == 0 && e.paginacion == "1/1" && e.pronostico == "1X21X21X21X21X")
        try e.siguiente()
        #expect(e.columnas.count == 2 && e.actual == 1 && e.paginacion == "2/2" && e.pronostico.isEmpty)
        #expect(e.columnas[0].getAciertos == "5,6a" && e.columnas[0].getAciertosSeguidos == todos && !e.columnas[0].toleranciaLocalActiva)
        e.pronostico = "111"
        e.aciertosTol = "3"
        e.tolerancias = [false, true, false, false]
        try e.anterior()
        let c1 = e.columnas[1]
        #expect(c1.toleranciaLocalActiva && c1.getTolerancias == "1" && c1.getACTol == "3" && c1.getACSTol == todos)
        #expect(c1.textoPronosticos.joined() == "111" && e.paginacion == "1/2" && e.aciertos == "5,6a")
        e.pronostico = ""
        try e.ultima()  // quita la actual y crea otra vacía al final (PRESERVAR)
        #expect(e.columnas.count == 2 && e.actual == 1 && e.paginacion == "2/2" && e.pronostico.isEmpty)
        var f = CondicionColProbables()
        try e.aceptar(en: &f)
        #expect(f.activa && f.contieneDatos && f.columnas.count == 1 && f.columnas[0].getACTol == "3")

        var g = try EditorColumnasProbables(try tres(), numeroPartidos: 14, desplazamiento: 3)
        try g.adelante()
        #expect(g.actual == 3 && g.columnas.count == 4 && g.paginacion == "4/4")
        try g.atras()
        #expect(g.actual == 0 && g.columnas.count == 3 && g.paginacion == "1/3")
        g.todosATriple()
        #expect(g.pronostico.count == 42)
        g.borrarPronosticos()
        #expect(g.pronostico.isEmpty && g.paginacion == "1/3")
        g.eliminarActual()
        #expect(g.columnas.count == 2 && g.pronostico == "XXXXXXXXXXXXXX")
        g.pronostico = "1"
        g.aciertos = "0-14"
        #expect(throws: ErrorDominio.self) { try g.siguiente() }
        #expect(throws: ErrorDominio.self) { try CasosCondiciones.copiaCP(ColumnaProbableConfig(numeroPartidos: 14), puntos: true) }
    }

    @Test func copiaDeDatosEImportacion() throws {
        let f = try tres()
        var e = try EditorColumnasProbables(f, numeroPartidos: 14, desplazamiento: 3)
        #expect(e.paginacion == "1/3" && e.paginacionRel2 == "1 / 0" && e.paginacionRel3 == "1 / 0")
        e.aciertos = "7"
        let rango = e.prepararCopiaDatos()
        let r = try #require(rango)
        #expect(r.desde == 1 && r.hasta == 3)
        try e.aplicarCopiaDatos(desde: 2, hasta: 3)
        #expect(e.columnas.map(\.getAciertos) == ["7", "7", "7"] && e.aciertos == "7")
        try e.aplicarCopiaDatos(desde: 0, hasta: 3)

        let simples = try #require(
            CasosCondiciones.importarCPsSimples(Data("1x21x21x21x21x\r\n\r\n1,X,12,1X2\r\n".utf8), numeroPartidos: 14))
        #expect(simples.count == 2 && simples[0].textoPronosticos.joined() == "1X21X21X21X21X")
        #expect(simples[1].textoPronosticos.prefix(5) == ["1", "X", "12", "1X2", ""] && simples[1].getAciertos == todos)
        #expect(CasosCondiciones.importarCPsSimples(Data("1X2\r\n".utf8), numeroPartidos: 14) == nil)
        let clm = try #require(
            CasosCondiciones.importarCPsClm(Data("1X21X21X21X21X#3,4#0#1\r\nbasura\r\n".utf8), numeroPartidos: 14))
        #expect(clm.count == 1 && clm[0].getAciertos == "3,4" && clm[0].getFallosSeguidos == "1")
        #expect(CasosCondiciones.importarCPsClm(Data("1X21X21X21X21X#3-4#0#1".utf8), numeroPartidos: 14) == nil)

        let base = try EditorColumnasProbables(f, numeroPartidos: 14, desplazamiento: 3)
        let tresImportadas = simples + [simples[0]]
        #expect(!base.confirmarSustituir(simples) && base.confirmarSustituir(tresImportadas) && base.confirmarAgregar(simples))
        var s = base
        try s.importar(tresImportadas, modo: .sustituir, delFiltro: f.columnas)
        #expect(s.columnas.count == 3 && s.columnas[0].textoPronosticos.joined() == "1X21X21X21X21X" && s.pronostico == "1X21X21X21X21X")
        var a = base
        try a.importar(simples, modo: .agregar, delFiltro: [])
        #expect(a.columnas.count == 5)
        var z = base
        try z.importar(simples, modo: .reemplazar, delFiltro: [])
        #expect(z.columnas == simples)
        var p = base
        try p.ultima()
        try p.siguiente()
        p.prepararImportacion()
        #expect(p.columnas.count == 3)

        #expect(CasosCondiciones.datosCPsSimples([]) == nil)
        let d = try #require(CasosCondiciones.datosCPsSimples([simples[1]]))
        #expect(String(decoding: d, as: UTF8.self) == "1,X,12,1X2" + String(repeating: ",", count: 10) + "\r\n")
        #expect(String(decoding: CasosCondiciones.datosCPsClm(clm), as: UTF8.self) == "1,X,2,1,X,2,1,X,2,1,X,2,1,X#3,4#0#1\r\n")
    }

    @Test func relacionesYControlFallos() throws {
        let f = try tres()
        var e = try EditorColumnasProbables(f, numeroPartidos: 14, desplazamiento: 3)
        e.rel1Columnas = "1-2"
        e.rel1SumaAciertos = "3"
        try e.rel1Siguiente()
        #expect(e.relaciones1.count == 2 && e.paginacionRel1 == "2/2" && e.rel1Columnas.isEmpty)
        try e.rel1Anterior()
        try e.rel1Anterior()
        #expect(e.rel1 == 0 && e.rel1Columnas == "1-2" && e.paginacionRel1 == "1/2")

        e.rel2.columnasA = "1"
        e.rel2.columnasB = "2"
        e.rel2.aciertos = "3"
        try e.rel2Siguiente()
        #expect(e.relaciones2.count == 1 && e.paginacionRel2 == "2 / 1" && e.rel2.columnasA.isEmpty && e.rel2.cantidad == "Más")
        try e.rel2Anterior()
        #expect(e.rel2.columnasA == "1" && e.paginacionRel2 == "1 / 1")

        e.rel3Columnas = "1-3"
        e.rel3EscalerasTotal = "1"
        e.agrupacionesPasoFijo = [
            FilaAgrupacion(numero: "0-1", elementos: "2", aciertos: "3,4"), FilaAgrupacion(numero: "1", elementos: "", aciertos: ""),
        ]
        e.controlesFallo = [FilaControlFallos(columnas: "1,2", tolerancias: "ac", aciertos: "0-1"), FilaControlFallos()]
        e.numeroFallosControles = "0-1"
        var g = f
        try e.aceptar(en: &g)
        #expect(g.relaciones1.count == 1 && g.relaciones1[0].sumaAciertos == "3" && g.relaciones2.count == 1)
        let r3 = try #require(g.relaciones3.first)
        #expect(r3.escalerasTotales == [false, true] && r3.agrupacionesPasoFijo?.count == 1)
        #expect(r3.agrupacionesPasoFijoTexto == "0-1+2+3,4#1++" && r3.agrupacionesSolapadasTexto == nil && r3.conceptoTexto == "AC")
        #expect(g.controlesFallos.count == 1 && g.controlesFallos[0].tolerancias == "AC" && g.fallosPermitidos.texto == "0-1")

        var e2 = try EditorColumnasProbables(g, numeroPartidos: 14, desplazamiento: 3)
        #expect(e2.paginacionRel2 == "1 / 1" && e2.rel2.columnasA == "1" && e2.rel1Columnas == "1-2")
        #expect(e2.agrupacionesPasoFijo.count == 1 && e2.controlesFallo.count == 1 && e2.numeroFallosControles == "0-1")
        e2.rel2Eliminar()
        #expect(e2.relaciones2.isEmpty && e2.paginacionRel2 == "1 / 0" && e2.rel2.columnasA.isEmpty)
        e2.rel1Eliminar()
        #expect(e2.relaciones1.count == 1 && e2.rel1Columnas.isEmpty)
    }

    @Test func copiarColumnas() throws {
        var c = Combinacion(numeroPartidos: 14)
        for g in 1...2 { _ = try CasosInicio.cambiarGrupo(&c, a: g) }
        let cols = try tres().columnas
        try c.modificarGrupo(1) { x throws(ErrorDominio) in x.condiciones.colProbables.ponerColumnas(cols) }
        func copiar(_ t: String, _ d: Set<Int>) throws -> (estado: String, cerrar: Bool) {
            try CasosCondiciones.copiarColumnasCP(t, destinos: d, grupoEnEdicion: 1, grupoPantalla: 1, en: &c)
        }
        #expect(try copiar(" ", [2]).estado == "Introduce al menos una columna a copiar.")
        #expect(try copiar("1", []).estado == "Selecciona al menos un grupo destino.")
        #expect(try copiar("1-x", [2]).estado == "Expresión de columnas no válida (usa \"5\", \"1,3,5\" o \"1-5\").")
        #expect(try copiar("2-4", [2]).estado == "Algún índice supera el número de columnas disponibles (3).")
        #expect(try copiar("0", [2]).estado == "Algún índice supera el número de columnas disponibles (3).")
        #expect(try copiar("5-4", [2]) == ("Copiadas 0 columna(s) a 1 grupo(s).", true))
        #expect(try copiar("1,3", [1, 2]) == ("Copiadas 4 columna(s) a 2 grupo(s).", true))
        let destino = c.grupos[2].condiciones.colProbables
        #expect(c.grupos[1].condiciones.colProbables.columnas.count == 5 && destino.columnas.count == 2 && destino.activa)
        #expect(destino.columnas.map { $0.textoPronosticos[0] } == ["1", "2"])
    }
}
