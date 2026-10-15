import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_13 §4.6: patrón genérico y editores de condición.
@Suite("Casos de condiciones")
struct CondicionesTests {
    let entorno = Entorno.pruebas()

    @Test func noVariantesPatronAceptar() throws {
        var f = CondicionNoVariantes(numeroPartidos: 14)
        try CasosCondiciones.aplicarNoVariantes(variantes: "2,3a", equis: "", doses: "", en: &f)
        #expect(f.activa && f.contieneDatos && f.getVariantes == "2,3a" && f.getEquis == CasosCondiciones.todosValores(14))
        f.activa = false  // desactivada desde Inicio: reaceptar con datos no la reactiva
        try CasosCondiciones.aplicarNoVariantes(variantes: "4", equis: "", doses: "", en: &f)
        #expect(!f.activa && f.getVariantes == "4")
        try CasosCondiciones.aplicarNoVariantes(variantes: " ", equis: "", doses: "", en: &f)
        #expect(!f.activa && !f.contieneDatos && f.getVariantes.isEmpty)
        #expect(throws: ErrorDominio.self) { try CasosCondiciones.aplicarNoVariantes(variantes: "0 1", equis: "", doses: "", en: &f) }
    }

    @Test func signosSeguidosFigurasVaciasNoBorran() throws {
        var f = CondicionSignosSeguidos(numeroPartidos: 14)
        f.figuras[1] = [0x21]
        try CasosCondiciones.aplicarSignosSeguidos(["", "3", "", ""], figuras: [nil, [], [0x12], nil], en: &f)
        #expect(f.activa && f.get(.unos) == "3" && f.figuras == [[], [0x21], [0x12], []])
    }

    @Test func simetrias() throws {
        #expect(try CasosCondiciones.simetrias(["1,2-4", "", "5,15"], numeroPartidos: 14) == nil)
        let s = try #require(try CasosCondiciones.simetrias(["1,2-4", " ", "5,6"], numeroPartidos: 14))
        #expect(s.map(\.partidos) == ["1,2,3,4", "5,6"])
        #expect(CasosCondiciones.aciertosSimetrias("1,3,39-42,50") == [1, 3, 39, 40])
        #expect(CasosCondiciones.aciertosSimetrias("x") == nil && CasosCondiciones.aciertosSimetrias("") == [])
        var f = CondicionSimetrias()
        #expect(try CasosCondiciones.aplicarSimetrias(filas: ["1,2"], aciertos: "", en: &f, numeroPartidos: 14))
        #expect(f.activa && f.aciertosTexto == "0" && f.aciertos.isEmpty)
        #expect(try !CasosCondiciones.aplicarSimetrias(filas: ["1,a"], aciertos: "", en: &f, numeroPartidos: 14))
    }

    @Test func figuras() {
        #expect(CasosCondiciones.aceptarFiguras(["3-2-1", "*", "", "3-2-1", "1"]) == [0x321, 1])
        #expect(CasosCondiciones.textoFigura(0x321) == "3-2-1")
        #expect(CasosCondiciones.extensionFiguras("SignosSeguidos") == ".figV1X2")
    }

    @Test func exportarImportarYEstadisticas() throws {
        var c = CondicionesGrupo(numeroPartidos: 14)
        try CasosCondiciones.aplicarNoVariantes(variantes: "0,1,2", equis: "", doses: "", en: &c.noVariantes)
        let url = CasosCondiciones.rutaTemporal("vx2", entorno: entorno)
        try CasosCondiciones.exportarCondicion(.noVariantes, c, en: url, entorno: entorno)
        let leida = try #require(try CasosCondiciones.importarCondicion(url, numeroPartidos: 14, entorno: entorno))
        #expect(leida.noVariantes.getVariantes == "0,1,2")
        try entorno.archivos.escribir(Data("no xml".utf8), en: url, atomico: true)
        #expect(try CasosCondiciones.importarCondicion(url, numeroPartidos: 14, entorno: entorno) == nil)

        #expect(throws: ErrorDominio.self) {
            _ = try CasosCondiciones.estadisticasDeCondicion(.noVariantes, c, configuracion: .porDefecto, entorno: entorno)
        }
        let g = entorno.rutas.carpeta(.ganadoras)
        try entorno.archivos.crearCarpeta(g)
        try entorno.archivos.escribir(
            Data("11111111111111\r\nXXXXXXXXXXXXXX\r\n111111111111XX\r\n".utf8), en: g.appending(path: "b.TXT"), atomico: true)
        try entorno.archivos.escribir(Data(), en: g.appending(path: "a.txt"), atomico: true)
        try entorno.archivos.escribir(Data("1".utf8), en: g.appending(path: "c.dat"), atomico: true)
        let e = try CasosCondiciones.estadisticasDeCondicion(.noVariantes, c, configuracion: .porDefecto, entorno: entorno)
        #expect(e.map(\.archivo) == ["a.txt", "b.TXT"])
        #expect(e.map(\.cumplimiento) == [0, Float(2) / Float(3) * 100])
        #expect(CasosCondiciones.textoCumplimiento(e[1], cultura: .esES) == "66,66667")
    }

    @Test func pesosNumericos() throws {
        var f = CondicionPesosNumericos(numeroPartidos: 14)
        try CasosCondiciones.aplicarPesosNumericos(
            pesos: ["", "", "", "", ""], tolerados: ["1", "", "", "", ""], numTolerancias: [false, true, false, false, false, false],
            figuras: [false, true, false, false, false], en: &f, numeroPartidos: 14)
        #expect(f.activa && f.figuras == [0x311] && f.getPN(.global) == CasosCondiciones.todosValores(14))
        #expect(f.getPNTol(.global) == "1" && f.getTolerancias == "1")
        #expect(CasosCondiciones.figurasMarcadasPesos(f) == [false, true, false, false, false])
        #expect(CasosCondiciones.toleranciasMarcadasPesos(f) == [false, true, false, false, false, false])
        try CasosCondiciones.aplicarPesosNumericos(
            pesos: ["3"], tolerados: [], numTolerancias: [], figuras: [], en: &f, numeroPartidos: 14)
        #expect(f.getTolerancias == "0" && f.figuras.isEmpty && f.getPN(.global) == "3")
    }

    @Test func valoracion() throws {
        var m = Array(repeating: [0.0, 0.0, 0.0], count: 14)
        m[0] = [50, 30, 20]
        m[1] = [10, 60, 30]
        let p = CasosCondiciones.prepararValoracion(m, esSuma: true, rangos: ["", "5-9", "", ""], cultura: .esES)
        #expect(p.minMaxSumas == "30-110" && p.rangos == ["30-110", "5-9", "0-90", "0-50"])
        let q = CasosCondiciones.prepararValoracion(m, esSuma: false, rangos: [], cultura: .esES)
        #expect(q.minMaxProductos == "0,233-3,511" && q.rangos[1] == "0-1,711")
        var f = CondicionValoracionSignos(numeroPartidos: 14)
        try CasosCondiciones.aplicarValoracion(p, esSuma: true, en: &f, cultura: .esES)
        #expect(f.activa && f.contieneDatos && f.valorGlobal == "30-110")
        #expect(try CasosCondiciones.valorarColumna("1X", p, esSuma: true, en: &f, numeroPartidos: 14, cultura: .esES) == nil)
        #expect(try CasosCondiciones.valorarColumna("1X222222222222", p, esSuma: true, en: &f, numeroPartidos: 14, cultura: .esES) == "110")
        #expect(CasosCondiciones.columnasBase([[50, 30, 20], [10, 60, 30], [1, 1, 5], [5, 5, 5]]) == "1X21\r\nX21X\r\n21X2")
    }

    @Test func distanciasEInterrupciones() throws {
        var d = CondicionDistancias(numeroPartidos: 14)
        try CasosCondiciones.aplicarDistancias(["", "2,3b", "", ""], en: &d)
        #expect(d.activa && d.getInt1 == "2,3b" && d.getIntVar == CasosCondiciones.todosValores(14))
        var i = CondicionInterrupciones(numeroPartidos: 14)
        try CasosCondiciones.aplicarInterrupciones(["", "", "", "", "", "1"], en: &i)
        #expect(i.activa && i.get(.globalSeg) == "1" && i.get(.global) == CasosCondiciones.todosValores(14))
        try CasosCondiciones.aplicarInterrupciones([], en: &i)
        #expect(!i.activa && !i.contieneDatos)
    }

    @Test func contactosYDibujos() throws {
        var c = CondicionContactos(numeroPartidos: 15)
        try CasosCondiciones.aplicarContactos(["0,1", "", "", "", "", "", "", "", "", ""], figuras: [0x21], en: &c)
        #expect(c.activa && c.get(.unoX) == "0,1" && c.get(.vv) == CasosCondiciones.todosValores(15) && c.figuras == [0x21])
        var c14 = CondicionContactos(numeroPartidos: 14)
        #expect(throws: ErrorDominio.self) { try CasosCondiciones.aplicarContactos(["0"], figuras: nil, en: &c14) }
        #expect(CasosCondiciones.casillasDibujos(2) == ["0+0", "0+1", "0+2", "1+0", "1+1", "2+0"])
        #expect(CasosCondiciones.casillasDibujos(14).count == 120)
        var d = CondicionDibujos()
        try CasosCondiciones.aplicarDibujos(["2+3", "0+0"], en: &d)
        #expect(d.activa && d.contieneDatos && d.getDibujos == "2+3,0+0")
        try CasosCondiciones.aplicarDibujos([], en: &d)
        #expect(!d.activa && !d.contieneDatos && d.dibujos.isEmpty)
    }

    @Test func diferencias() throws {
        var t = TextosDiferencia()
        #expect(CasosCondiciones.crearDiferencia(t, numeroPartidos: 14) == nil)
        t.lineas[0] = "1,2"
        t.lineas[1] = "3-4"
        t.conceptos[1] = "1-2"
        let d = try #require(CasosCondiciones.crearDiferencia(t, numeroPartidos: 14))
        #expect(d.partidosSimetricos.count == 2 && d.analizaVX2Dib && !d.analizaInterrupciones && d.acV == [false, true, true])
        #expect(d.acX == [false, true, true, false])  // tamaño longitud + 1 si longitud > mayor
        let v = CasosCondiciones.textosDiferencia(d)
        #expect(v.lineas[0] == "1,2" && v.lineas[1] == "3,4" && v.conceptos[0] == "1,2" && v.conceptos[4].isEmpty)
        var f = CondicionDiferencias()
        let r = try #require(CasosCondiciones.siguienteDiferencia(&f, indice: 0, t, numeroPartidos: 14))
        #expect(r.indice == 1 && r.mostrar == nil && f.diferencias.count == 1)
        #expect(CasosCondiciones.siguienteDiferencia(&f, indice: 1, TextosDiferencia(), numeroPartidos: 14) == nil)
        let a = try #require(CasosCondiciones.anteriorDiferencia(&f, indice: 1, TextosDiferencia(), numeroPartidos: 14))
        #expect(a.indice == 0 && a.mostrar == d)
        #expect(CasosCondiciones.eliminarDiferencia(&f, indice: 0) == nil && f.diferencias.isEmpty)
        #expect(
            CasosCondiciones.atajosDiferencias("Tríos", pasoFijo: true, lineas: ["x", "", ""], numeroPartidos: 7) == ["x", "1-3", "4-6"])
    }
}
