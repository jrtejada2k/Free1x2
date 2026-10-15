import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_13 §4.1 (CU-S01…S10).
@Suite("Casos de Inicio")
struct InicioTests {
    let entorno = Entorno.pruebas()
    var base: URL { entorno.rutas.base }

    func sembrarBundle() throws {
        for s in CasosInicio.semillas {
            try entorno.archivos.escribir(
                try Golden.bytes("Fixtures/datos/" + s), en: entorno.rutas.recursos.appending(path: "Datos/" + s), atomico: true)
        }
    }

    @Test func arrancarCreaCarpetasYSiembraSinPisar() throws {
        try sembrarBundle()
        try entorno.archivos.escribir(Data("mío".utf8), en: base.appending(path: "Equipos/equipos1.dat"), atomico: true)
        let e = CasosInicio.arrancar(entorno: entorno)
        for c in RutasApp.Carpeta.allCases { #expect(entorno.archivos.existe(entorno.rutas.carpeta(c))) }
        #expect(try entorno.archivos.leer(base.appending(path: "Equipos/equipos1.dat")) == Data("mío".utf8))
        #expect(entorno.archivos.existe(base.appending(path: "Impresion/imprebol.cfg")))
        #expect(e.configuracion.puntosCP == PuntosCP(fijos: 6, dobles: 3, triples: 1))
        #expect(e.configuracion.boleto.desplazamiento == 3)
        #expect(e.jornada == nil)
    }

    @Test func arrancarSinSemillasUsaValoresPorDefectoYCache() throws {
        try entorno.archivos.escribir(
            try Golden.bytes("Fixtures/online/quiniela-es-actual.json"), en: base.appending(path: "jornada-es.json"), atomico: true)
        let e = CasosInicio.arrancar(entorno: entorno)
        #expect(e.configuracion == .porDefecto)
        #expect(e.jornada?.partidos.count == 14)
    }

    @Test func combinacionIdaYVuelta() throws {
        var nueva = CasosInicio.nuevaCombinacion(numeroPartidos: 14)
        #expect(nueva.combinacion.pronosticos == Array(repeating: "1,X,2", count: 14))
        #expect(nueva.equipos.allSatisfy { $0 == "? - ?" })
        try nueva.combinacion.setPronostico(0, "1")
        nueva.equipos[0] = "Betis - Sevilla"
        let url = base.appending(path: "Combinaciones/j.comb")
        try CasosInicio.guardarCombinacion(
            nueva.combinacion, equipos: nueva.equipos, filtroColumnas: "/x/f.txt", en: url, entorno: entorno)
        let leida = try CasosInicio.abrirCombinacion(url, numeroPartidos: 14, entorno: entorno)
        // `GetGrupoFromXmlNode` reescribe `EsGrupoBase` en cada hijo (ArchivoCombinacion.cs:225-232): no es igualdad total.
        #expect(leida.combinacion.pronosticos == nueva.combinacion.pronosticos)
        #expect(leida.combinacion.grupos.count == 1 && leida.combinacion.controlesGrupos == nueva.combinacion.controlesGrupos)
        #expect(leida.equipos == nueva.equipos)
        #expect(leida.filtroColumnas == "/x/f.txt")
    }

    @Test func abrirCombinacionInexistenteLlevaElLiteral() {
        #expect {
            _ = try CasosInicio.abrirCombinacion(base.appending(path: "no.comb"), numeroPartidos: 14, entorno: entorno)
        } throws: { ($0 as? ErrorDominio)?.mensajeUsuario.hasPrefix("No se pudo abrir la combinación: ") == true }
    }

    @Test func equiposIdaYVuelta() throws {
        let url = base.appending(path: "e.txt")
        try CasosInicio.guardarEquipos(["A - B", "C - D"], en: url, entorno: entorno)
        #expect(try entorno.archivos.leer(url) == Data("\u{FEFF}A - B\r\nC - D\r\n".utf8))
        #expect(try CasosInicio.abrirEquipos(url, numeroPartidos: 3, entorno: entorno) == ["A - B", "C - D", "? - ?"])
    }

    @Test func borraTemporalesEInformes() throws {
        #expect(try CasosInicio.borrarTemporales(entorno: entorno) == 0)
        try entorno.archivos.crearCarpeta(entorno.rutas.carpeta(.temp))
        try entorno.archivos.crearCarpeta(entorno.rutas.carpeta(.informes))
        for n in ["a_tmp.comb", "b_tmp.comb.bak", "c.comb", "tmp.vx2"] {
            try entorno.archivos.escribir(Data(), en: entorno.rutas.carpeta(.temp).appending(path: n), atomico: true)
        }
        for n in ["Informe", "Informe1.txt", "informe2.txt", "XInforme.txt"] {
            try entorno.archivos.escribir(Data(), en: entorno.rutas.carpeta(.informes).appending(path: n), atomico: true)
        }
        #expect(try CasosInicio.borrarTemporales(entorno: entorno) == 2)
        #expect(try CasosInicio.borrarInformes(entorno: entorno) == 1)
        #expect(try entorno.archivos.listar(entorno.rutas.carpeta(.temp)).map(\.lastPathComponent) == ["c.comb", "tmp.vx2"])
    }

    @Test func semaforos() throws {
        var c = Combinacion(numeroPartidos: 14)
        var e = CasosInicio.estadoDeCondiciones(c, grupo: 0)
        #expect(e.condiciones.values.allSatisfy { $0 == .neutro })
        #expect(e.controlGrupos == .neutro && e.ifThen == .neutro)
        try c.modificarGrupo(0) { g in
            g.condiciones.noVariantes.activa = true
            g.condiciones.dibujos.contieneDatos = true
        }
        var it = IfThenConfig()
        it.condiciones = [CondicionIfThenConfig()]
        it.activo = false
        c.ifThen = it
        c.controlesGrupos.append(ControlGruposConfig())
        e = CasosInicio.estadoDeCondiciones(c, grupo: 7)  // fuera de rango ⇒ grupo 0
        #expect(e.condiciones[.noVariantes] == .verde && e.condiciones[.dibujos] == .rojo)
        #expect(e.condiciones[.contactos] == .neutro)
        #expect(e.controlGrupos == .verde && e.ifThen == .rojo)
    }

    @Test func barrasVisibles() throws {
        try sembrarBundle()
        _ = CasosInicio.arrancar(entorno: entorno)
        try CasosInicio.guardarBarrasVisibles([false, false, true, true, false, true], entorno: entorno)
        let c = ArchivoParametros.cargarConfiguracion(try entorno.archivos.leer(base.appending(path: "parametros.free1x2")))
        #expect(c.interfaz.barras == [false, false, true, true, false, true])
        #expect(CasosInicio.leerBarrasVisibles(c) == [false, true, true, true, false, true])
    }

    @Test func gruposYControles() throws {
        var c = Combinacion(numeroPartidos: 14)
        #expect(try CasosInicio.cambiarGrupo(&c, a: -1) == 0 && c.grupos.count == 1)
        #expect(try CasosInicio.cambiarGrupo(&c, a: 1) == 1)
        #expect(try CasosInicio.cambiarGrupo(&c, a: 2) == 2)
        #expect(c.grupos.count == 3 && c.controlesGrupos[0].textoGruposControlados == "0,1,2")
        #expect(try CasosInicio.eliminarGrupo(&c, 0) == nil)
        #expect(try CasosInicio.eliminarGrupo(&c, 1) == 0)
        #expect(c.grupos.count == 2 && c.controlesGrupos[0].gruposControlados == [0, 1])
    }

    @Test func filtros() throws {
        let url = base.appending(path: "f.txt")
        try entorno.archivos.escribir(Data("1X21X21X21X21X2\r\n".utf8), en: url, atomico: true)
        #expect(throws: ErrorDominio.self) { try CasosInicio.validarFiltroColumnas(url, numeroPartidos: 14, entorno: entorno) }
        try CasosInicio.validarFiltroColumnas(base.appending(path: "no.txt"), numeroPartidos: 14, entorno: entorno)
        try CasosInicio.validarFiltroParcial(url, numeroPartidos: 15, entorno: entorno)
        #expect(throws: ErrorDominio.self) { try CasosInicio.validarFiltroParcial(url, numeroPartidos: 14, entorno: entorno) }
    }

    @Test func boleto() {
        #expect(CasosInicio.dividirEquipos("? - ?") == ("", ""))
        #expect(CasosInicio.dividirEquipos(" Real Madrid  -  Betis ") == ("Real Madrid", "Betis"))
        #expect(CasosInicio.dividirEquipos("Solo") == ("Solo", ""))
        #expect(CasosInicio.componerEquipos(local: "Real ", visitante: "") == "Real  - ")
        #expect(CasosInicio.componerEquipos(local: " ", visitante: "") == "? - ?")
        #expect(CasosInicio.normalizarEquipos(local: "Real ", visitante: " Betis") == "Real - Betis")
        #expect(CasosInicio.aplicarEquipos(["", "A - B"], a: ["x", "y", "z"]) == ["? - ?", "A - B", "z"])
        #expect(CasosInicio.signos(nil) == [true, true, true])
        #expect(CasosInicio.signos("X,1") == [true, true, false])
        #expect(CasosInicio.pronostico([true, false, true]) == "1,2")
    }
}
