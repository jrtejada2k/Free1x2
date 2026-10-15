import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_08 §11 y ESPEC_10 §99 (CU-A25, CU-A26).
@Suite("Boletos")
struct BoletosTests {
    @Test func ordenYTroceo() throws {
        let cols = ["2X1", "111", "X2X", "121"]
        #expect(CasosVer.ordenarColumnas(cols, orden: .signo, descendente: false) == ["111", "121", "2X1", "X2X"])
        #expect(CasosVer.ordenarColumnas(cols, orden: .signo, descendente: true) == ["X2X", "2X1", "121", "111"])
        #expect(CasosVer.ordenarColumnas(cols, orden: .variantes, descendente: false) == ["111", "121", "2X1", "X2X"])
        #expect(CasosVer.ordenarColumnas(cols, orden: .equis, descendente: true) == ["X2X", "2X1", "111", "121"])
        #expect(CasosVer.ordenarColumnas(cols, orden: .interrupciones, descendente: false) == ["111", "2X1", "X2X", "121"])
        #expect(CasosVer.ordenarColumnas(["1", "1X2"], orden: .variantes, descendente: false) == ["1", ""])

        let entorno = Entorno.pruebas()
        let url = URL(filePath: "/tmp/bol/cols.txt")
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        let texto = (1...9).map { _ in "11111111111111" }.joined(separator: "\r\n") + "\r\n"
        try entorno.archivos.escribir(Data(texto.utf8), en: url, atomico: true)
        let b = try CasosVer.prepararBoletos(url, orden: .signo, descendente: false, entorno: entorno)
        #expect(b.columnas.count == 9 && b.total == 2)
        let segundo = CasosVer.boleto(b, 5)
        #expect(segundo.indice == 1 && segundo.numeros == [9, 0, 0, 0, 0, 0, 0, 0] && segundo.signos[1].isEmpty)
    }

    @Test func boletosEnTexto() throws {
        let t = try CasosVer.boletosEnTexto(["1X", "21"], separador: ["1", "X", "2"])
        #expect(t == "Boleto 1\n__________________\n\n1 2 \n\nX 1 ")
        #expect(throws: ErrorDominio.self) { try CasosVer.boletosEnTexto(["1", "12"], separador: []) }
        #expect(try CasosVer.boletosEnTexto([], separador: []).isEmpty)
    }
}
