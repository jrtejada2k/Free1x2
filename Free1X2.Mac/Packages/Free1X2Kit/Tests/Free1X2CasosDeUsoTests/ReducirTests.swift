import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_10 §82 (CU-C18, CU-C19).
@Suite("Reductor")
struct ReducirTests {
    let entorno = Entorno.pruebas()

    @Test func validarYReducir() async throws {
        let madre = URL(filePath: "/tmp/madre.txt")
        var lineas: [String] = []
        for a in ["1", "X", "2"] { for b in ["1", "X", "2"] { lineas.append(a + b + String(repeating: "1", count: 12)) } }
        try entorno.archivos.escribir(Data((lineas.joined(separator: "\r\n") + "\r\n").utf8), en: madre, atomico: true)
        #expect(CasosCombinacion.validarFicheroEntradaReductor(madre, entorno: entorno))
        let corta = URL(filePath: "/tmp/corta.txt")
        try entorno.archivos.escribir(Data("1X2\r\n".utf8), en: corta, atomico: true)
        #expect(!CasosCombinacion.validarFicheroEntradaReductor(corta, entorno: entorno))
        #expect(!CasosCombinacion.validarFicheroEntradaReductor(URL(filePath: "/tmp/no.txt"), entorno: entorno))

        let salida = URL(filePath: "/tmp/reducido.txt")
        let e = EntradaReduccion(
            algoritmo: .menosTiempo, entrada: madre, salida: salida, nivel: 13, maxCol: 0, percent: 100, externas: false)
        let r = try await CasosCombinacion.reducirColumnas(e, numeroPartidos: 14, entorno: entorno)
        #expect(r.iniciales == 9 && r.finales > 0 && r.finales < 9)
        let escritas = String(decoding: try entorno.archivos.leer(salida), as: UTF8.self).components(separatedBy: "\r\n")
        #expect(escritas.count == r.finales + 1 && escritas[0].count == 14)
        #expect(CasosCombinacion.textosReduccion(iniciales: 9, procesadas: 5, finales: 3) == [" = 9", " = 5", " = 3", " = 55"])
        #expect(CasosCombinacion.textosReduccion(iniciales: 0, procesadas: 0, finales: 0).last == "-")
    }
}
