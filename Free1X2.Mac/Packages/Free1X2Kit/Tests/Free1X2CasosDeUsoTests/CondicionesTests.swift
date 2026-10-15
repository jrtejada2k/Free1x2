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
}
