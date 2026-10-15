import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_08 §9, §38, §39; ESPEC_09 §56; ESPEC_10 §90, §91.
@Suite("Estadísticas y GEPT")
struct EstadisticasTests {
    @Test func origenYProceso() async throws {
        let entorno = Entorno.pruebas()
        let url = URL(filePath: "/tmp/est/cols.txt")
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data("11111111111111\r\nx2x2x2x2x2x2x2\r\n111\r\n".utf8), en: url, atomico: true)
        let r = try CasosVer.columnasEstadisticas(url, entorno: entorno)
        #expect(r.columnas == ["11111111111111", "X2X2X2X2X2X2X2"] && r.errorEn == 2)
        let m = try await CasosVer.calcularEstadistica(.dibujos, columnas: r.columnas)
        #expect(m.numColumnas == 2 && m.rsl[0][0] == 1 && m.rsl[7][7] == 1)
        let b = CasosVer.matrizDibujos(m, porcentajes: true)
        #expect(b[0].matriz.filas[1].celdas.first == "50" && b[0].matriz.filas[1].celdas.last == "50")
        #expect(b[0].matriz.filas[8].celdas[7] == "50" && b[0].matriz.filas[8].celdas[8] == "" && b[1].matriz.filas[1].celdas[14] == "50")
        #expect(b[0].matriz.filas[16].celdas[7] == "50")
    }

    @Test func porcentajeDesbordaEnInt32() {
        var rsl = MatrizEstadistica.vacia.rsl
        (rsl[0][0], rsl[0][1]) = (300_000, 1)
        let m = MatrizEstadistica(rsl: rsl, numColumnas: 500_000)
        let p = CasosVer.matrizEstadistica(m, etiquetas: ["globales"], columnas: 14, porcentajes: true, cultura: .esES)
        #expect(p.filas[1].celdas[0] == "-25,89" && p.filas[1].celdas[1] == "0")
        let c = CasosVer.matrizEstadistica(m, etiquetas: ["globales"], columnas: 14, porcentajes: false, cultura: .esES)
        #expect(c.filas[1].celdas[0] == "300000" && c.filas[0].celdas.last == "13")
        var d = MatrizEstadistica.vacia.rsl
        d[0][0] = 300_000
        #expect(
            CasosVer.matrizDibujos(MatrizEstadistica(rsl: d, numColumnas: 500_000), porcentajes: true)[0].matriz.filas[1].celdas[0] == "60")
    }

    @Test func traductorGEPT() {
        let esperado = ["GG": "12", "GE": "1X", "GP": "1", "EG": "X2", "EE": "X", "EP": "1X", "PG": "2", "PE": "X2", "PP": "12"]
        for (k, v) in esperado { #expect(CasosUtilidades.traducirGEPT(String(k.prefix(1)), String(k.suffix(1))) == v) }
        #expect(CasosUtilidades.traducirGEPT("g", "e") == "1X" && CasosUtilidades.traducirGEPT("", "G") == " ")
    }
}
