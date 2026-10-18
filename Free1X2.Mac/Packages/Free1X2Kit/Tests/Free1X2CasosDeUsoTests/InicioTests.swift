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

    /// DT-96: jornadas L.A.E. de la distribución original (apuestas de 0,50 €) pasadas al precio de Configuración.
    @Test func jornadasLAEAlPrecio() throws {
        let original =
            "111111XX211221\t1988/1989\t01\t13894332,67\t1042074,97\t18946,83\t1053,67\t150,52\t21,50\r\n"
            + "21X121XXX22X2X\t1995/1996\t23\t47426967,00\t0,00\t278982,15\t14503,65\t1097,85\t156,84\r\n"
        #expect(CasosInicio.jornadasLAEAlPrecio(Data(original.utf8), precio: 0.5) == Data(original.utf8))
        let a075 = CasosInicio.jornadasLAEAlPrecio(Data(original.utf8), precio: 0.75)
        #expect(
            String(decoding: a075, as: UTF8.self)
                == "111111XX211221\t1988/1989\t01\t9262888,45\t694716,65\t12631,22\t702,45\t100,35\t14,33\r\n"
                + "21X121XXX22X2X\t1995/1996\t23\t31617978\t0,00\t185988,1\t9669,1\t731,9\t104,56\r\n")
        // Leída con Estimación a 0,75 € da los euros de la jornada (0,50 × lo guardado en el original).
        let url = base.appending(path: "Jornadas/InfoJornadasLAE.txt")
        try entorno.archivos.escribir(a075, en: url, atomico: true)
        let r = try #require(
            try CasosUtilidades.datosJornadaLAE(
                temporada: "1995/1996", jornada: 23, recaudacion: 1, bote: 0, precio: 0.75, porcentajes: [12, 8, 8, 8, 9],
                entorno: entorno))
        #expect(abs(r.recaudacion - 0.5 * 47_426_967) < 0.01 && abs(r.premios[1] - 0.5 * 278_982.15) < 0.01)
    }

    /// DT-96: los históricos se siembran si faltan y la liga «España» solo si nunca hubo carpeta `Historico`.
    @Test func arrancarSiembraHistoricos() throws {
        try sembrarBundle()
        let recursos = entorno.rutas.recursos.appending(path: "Datos")
        for s in CasosInicio.semillasHistoricas {
            try entorno.archivos.escribir(Data("1\r\n".utf8), en: recursos.appending(path: s), atomico: true)
        }
        let resultados = "1970\t1\tX11121212X11X1\r\n2006\t38\t1X21X21X21X21X1\r\n"
        try entorno.archivos.escribir(Data(resultados.utf8), en: recursos.appending(path: "Jornadas/Resultados.txt"), atomico: true)
        let progol =
            "NPRODUCTO,CONCURSO,R1,R2,R3,R4,R5,R6,R7,R8,R9,R10,R11,R12,R13,R14,BOLSA,FECHA\r\n"
            + "10,2354,L,L,E,E,E,E,E,L,V,L,E,L,L,L,8000000,12/10/2026\r\n10,767,E,L,V,V,L,V,L,V,E,E,E,L,E,L,0,15/08/1993\r\n"
            + "10,700,L,L,E\r\n"
        try entorno.archivos.escribir(Data(progol.utf8), en: recursos.appending(path: CasosInicio.semillaProgol), atomico: true)
        let web = "2019/2020-01;1X21X21X21X21X\r\n20261011;X21X11XX111121\r\n"
        try entorno.archivos.escribir(Data(web.utf8), en: recursos.appending(path: CasosInicio.semillaEspanaWeb), atomico: true)
        let lae = "111111XX211221\t1988/1989\t01\t1000\t0,00\t0\t0\t0\t0\r\n"
        try entorno.archivos.escribir(Data(lae.utf8), en: recursos.appending(path: CasosInicio.semillaJornadasLAE), atomico: true)
        let e = CasosInicio.arrancar(entorno: entorno)
        #expect(CasosInicio.semillasHistoricas.count == 31 && CasosInicio.semillasHistoricas.last == "Ganadoras/2008-09.txt")
        #expect(CasosInicio.semillasHistoricas.contains("Ganadoras/1999-00.txt"))
        for s in CasosInicio.semillasHistoricas { #expect(entorno.archivos.existe(base.appending(path: s))) }
        let guardada = try entorno.archivos.leer(base.appending(path: CasosInicio.semillaJornadasLAE))
        #expect(guardada == CasosInicio.jornadasLAEAlPrecio(Data(lae.utf8), precio: e.configuracion.lae.precioApuesta))
        #expect(
            CasosHistorico.leer("España", entorno: entorno) == [
                GanadoraHistorica(jornada: "1970/1971-01", ganadora: "X11121212X11X1"),
                GanadoraHistorica(jornada: "2006/2007-38", ganadora: "1X21X21X21X21X"),
                GanadoraHistorica(jornada: "2019/2020-01", ganadora: "1X21X21X21X21X"),
                GanadoraHistorica(jornada: "20261011", ganadora: "X21X11XX111121"),
            ])
        // DT-98: la misma jornada bajada de clubprogol (clave = fecha) no se repite.
        let descarga = try HistoricoQuiniela.analizar(#"{"resultados":[{"jornada":20261011,"ganadora":"X21X11XX111121"}]}"#)
        #expect(try CasosHistorico.sumarDescarga(descarga, pais: .es, entorno: entorno).yaEstaban == 1)
        #expect(CasosHistorico.leer("España", entorno: entorno).count == 4)
        // DT-97: «México» del CSV de Lotería Nacional, del concurso más antiguo al más reciente; las filas rotas se saltan.
        #expect(
            CasosHistorico.leer("México", entorno: entorno) == [
                GanadoraHistorica(jornada: "767", ganadora: "X1221212XXX1X1"),
                GanadoraHistorica(jornada: "2354", ganadora: "11XXXXX121X111"),
            ])
        // Con la carpeta ya creada (aunque se borre la liga) no se vuelve a sembrar.
        try CasosHistorico.borrar("España", entorno: entorno)
        _ = CasosInicio.arrancar(entorno: entorno)
        #expect(CasosHistorico.leer("España", entorno: entorno).isEmpty)
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

    @Test func crearGrupos() throws {
        var c = Combinacion(numeroPartidos: 14)
        try CasosInicio.crearGrupos(&c, cuantos: 2)
        #expect(c.grupos.count == 3 && c.grupos[2].partidosActivos.allSatisfy { $0 } && !c.grupos[2].esGrupoBase)
        try CasosInicio.crearGrupos(&c, cuantos: -1)
        #expect(c.grupos.count == 3)
    }
}
