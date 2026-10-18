import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_08 §13 (CU-A28…A31).
@Suite("Calcular columnas")
struct CalcularTests {
    let entorno = Entorno.pruebas()

    func combinacion() -> Combinacion {
        var c = Combinacion(numeroPartidos: 14)
        c.ponerPronosticos((0..<14).map { $0 < 2 ? "1,X,2" : $0 == 2 ? "1,X" : "1" })
        return c
    }

    @Test func maximasYValidacion() throws {
        var c = combinacion()
        #expect(CasosCombinacion.estimarColumnasMaximas(c, entorno: entorno) == 18)
        let filtro = URL(filePath: "/tmp/filtro.txt")
        c.archivoColumnasBase = filtro.path(percentEncoded: false)
        #expect(CasosCombinacion.estimarColumnasMaximas(c, entorno: entorno) == 0)
        try entorno.archivos.escribir(Data("11111111111111\r\nXXXXXXXXXXXXXX\r\n22222222222222\r\n".utf8), en: filtro, atomico: true)
        #expect(CasosCombinacion.estimarColumnasMaximas(c, entorno: entorno) == 3)
        #expect(throws: ErrorDominio(.general, "Seleccione un archivo de resultados.")) {
            try CasosCombinacion.validarArchivoResultados(nil, combinacion: c)
        }
        #expect(throws: ErrorDominio(.general, "No puede usar como archivo de resultados un archivo usado ya en la combinación")) {
            try CasosCombinacion.validarArchivoResultados(filtro, combinacion: c)
        }
        try CasosCombinacion.validarArchivoResultados(URL(filePath: "/tmp/otro.txt"), combinacion: c)
    }

    @Test func contarGrabarYAnalizar() async throws {
        var c = combinacion()
        let cfg = Configuracion.porDefecto
        let contar = try await CasosCombinacion.calcularCombinacion(c, modo: .contar, configuracion: cfg, entorno: entorno)
        #expect(contar.calculo == ResultadoCalculo(analizadas: 18, aceptadas: 18) && contar.analisis == nil)
        let salida = URL(filePath: "/tmp/salida.txt")
        _ = try await CasosCombinacion.calcularCombinacion(c, modo: .grabar(salida), configuracion: cfg, entorno: entorno)
        let lineas = String(decoding: try entorno.archivos.leer(salida), as: UTF8.self).components(separatedBy: "\r\n")
        #expect(lineas.count == 19 && lineas[0] == "11111111111111" && lineas.last == "")
        let analizar = try await CasosCombinacion.calcularCombinacion(c, modo: .analizar, configuracion: cfg, entorno: entorno)
        #expect(analizar.calculo.aceptadas == 18 && analizar.analisis != nil)

        try c.modificarGrupo(0) { g throws(ErrorDominio) in
            try CasosCondiciones.aplicarNoVariantes(variantes: "5", equis: "", doses: "", en: &g.condiciones.noVariantes)
        }
        let vacia = URL(filePath: "/tmp/vacia.txt")
        let r = try await CasosCombinacion.calcularCombinacion(c, modo: .grabar(vacia), configuracion: cfg, entorno: entorno)
        #expect(r.calculo.aceptadas == 0 && !entorno.archivos.existe(vacia))
        let sinVisor = try await CasosCombinacion.calcularCombinacion(c, modo: .analizar, configuracion: cfg, entorno: entorno)
        #expect(sinVisor.analisis == nil)
    }

    @Test func textosDeEstadisticas() {
        var lae = ConfigLAE()
        lae.precioApuesta = 0.5
        let t = CasosCombinacion.estadisticasDeCalculo(procesadas: 9, aceptadas: 3, maximas: 27, lae: lae, cultura: .esES)
        #expect(t.procesadas == "9" && t.costeProcesadas == "€4,50" && t.aceptadas == "3" && t.costeAceptadas == "€1,50")
        #expect(t.porcentaje == "33,33 %" && t.estimadas == "9" && t.costeEstimadas == "€4,50")
        #expect(t.maximo == "27" && t.costeMaximo == "€13,50" && abs(t.progreso - 100.0 / 3) < 1e-9)
        let cero = CasosCombinacion.estadisticasDeCalculo(procesadas: 0, aceptadas: 0, maximas: 0, lae: lae, cultura: .esES)
        #expect(cero.porcentaje == "0,00 %" && cero.progreso == 0 && cero.costeMaximo == "€0,00")
        #expect(TextosCalculo().costeMaximo == "0.0")
    }
}
