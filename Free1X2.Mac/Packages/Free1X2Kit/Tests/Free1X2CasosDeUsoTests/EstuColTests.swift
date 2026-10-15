import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_09 §47 (CU-B11) y ESPEC_10 §101 (CU-C50…C52).
@Suite("EstuCol y visor ABDON")
struct EstuColTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ lineas: [String]) throws -> URL {
        let url = URL(filePath: "/tmp/ec/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(lineas.map { $0 + "\r\n" }.joined().utf8), en: url, atomico: true)
        return url
    }

    func informe(_ modo: AgrupacionEstuCol, total: Int = 3) async throws -> InformeEstuCol {
        let r = try fichero("r.txt", ["111", "1X1", "2X2", "222"])
        let g = try fichero("g.txt", ["111", "2X2", "XXX"])
        #expect(try CasosUtilidades.totalGanadorasEstuCol(g, entorno: entorno) == 3)
        return try await CasosUtilidades.informeEstuCol(reducidas: r, ganadoras: g, total: total, modo: modo, entorno: entorno)
    }

    @Test func emparejarYEscrutar() async throws {
        let a = try await informe(.a)
        #expect(a.columnas.count == 2 && a.porColumnas.map(\.serieAciertos) == [[3, 1, 1], [0, 3, 1]])
        #expect(a.porGanadoras.map(\.serieAciertos) == [[3, 0], [1, 3], [1, 1]])
        let (b, c) = (try await informe(.b), try await informe(.c))
        #expect(b.columnas.count == 3 && c.columnas.count == 2)
        let x = try await informe(.a, total: 4)
        #expect(x.porGanadoras.count == 4 && x.porGanadoras[3].serieAciertos == [0, 0], "si el fichero encoge, ceros")
    }

    @Test func visor() async throws {
        var v = VisorAbdon(try await informe(.a))
        #expect(v.columnaTexto == "1 / 2" && v.aciertosMin == "1" && v.aciertosMax == "3")
        v.moverColumna(1)
        v.moverColumna(1)
        #expect(v.columnaTexto == "2 / 2" && v.aciertosMin == "0")
        try v.mostrarPorGanadoras()
        #expect(v.filasPasoFijo == ["0-1+1+0", "0-1+1+1", "0-1+1+3", "0-1+2+1"] && v.filasSolapadas == v.filasPasoFijo)
        #expect(v.sumaTotal == "2-4" && v.sandwichs == "0-0" && v.escalerasTotales == "0-0")
        try v.mostrarAgrupaciones(solapadas: false, tipo: 2)
        #expect(v.hayPasoFijo == "0-1" && v.elementosPasoFijo == "2" && v.filasPasoFijo == ["0-1+2+1"])
        try v.cambiarModo()
        #expect(v.ganadoraTexto == "1 / 3" && v.hayPasoFijo.isEmpty && v.filasPasoFijo == ["1+1+0", "1+1+3"] && v.sumaTotal == "3")
        try v.moverGanadora(1)
        #expect(v.filasPasoFijo == ["1+1+1", "1+1+3"] && v.sumaTotal == "4")
        try v.mostrarAgrupaciones(solapadas: true, tipo: 1)
        #expect(v.haySolapadas == "2" && v.elementosSolapadas == "1")
        v.calcularSumaOpcional("1-2")
        #expect(v.sumaOpcional == "4")
        v.calcularSumaOpcional("2")
        #expect(v.sumaOpcional == "3")
        #expect(!VisorAbdon(nil).puedeGenerarCondicion && VisorAbdon(nil).columnaTexto == "- / -")
    }

    @Test func condicion() async throws {
        var v = VisorAbdon(try await informe(.a))
        try v.mostrarPorGanadoras()
        let c = try v.condicion(numeroPartidos: 3)
        let url = URL(filePath: "/tmp/ec/c.cps")
        try CasosCondiciones.exportarCondicion(.colProbables, c, en: url, entorno: entorno)
        let leida = try #require(try CasosCondiciones.importarCondicion(url, numeroPartidos: 3, entorno: entorno)).colProbables
        #expect(leida.columnas.count == 2 && leida.columnas[0].getAciertos == "1,2,3")
        #expect(leida.relaciones1.first?.sumaAciertos == "2-4" && leida.relaciones1.first?.columnas == "1-2")
        let r3 = try #require(leida.relaciones3.first)
        #expect(r3.agrupacionesPasoFijoTexto == "0-1+1+0#0-1+1+1#0-1+1+3#0-1+2+1" && r3.escalerasTexto == "0-0" && r3.conceptoTexto == "AC")
    }
}
