import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_08 §1 (CU-A01) y ESPEC_09 §61 (CU-B29).
@Suite("Añadir pleno al 15 y gráfico de columnas")
struct P15GraficoTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ lineas: [String]) throws -> URL {
        let url = URL(filePath: "/tmp/p15/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(lineas.map { $0 + "\r\n" }.joined().utf8), en: url, atomico: true)
        return url
    }

    func p15(_ entrada: [String], _ o: OpcionesP15) async throws -> (lineas: [String], repetidas: Int) {
        let e = try fichero("e.txt", entrada)
        let s = URL(filePath: "/tmp/p15/s.txt")
        if entorno.archivos.existe(s) { try entorno.archivos.borrar(s) }
        let r = try await CasosCombinacion.agregarP15(entrada: e, salida: s, opciones: o, entorno: entorno)
        guard entorno.archivos.existe(s) else { return ([], r) }
        return (TextoNET.lineas(try entorno.archivos.leer(s)), r)
    }

    @Test func comprobar() {
        var o = OpcionesP15()
        #expect(
            CasosCombinacion.comprobarP15(entrada: "", salida: "/b", o)
                == "Uno de los archivos necesarios no ha sido especificado\nNo se ha especificado qué hacer con el Pleno al 15")
        o.copiar = true
        #expect(CasosCombinacion.comprobarP15(entrada: "/a", salida: "/a", o) == "El archivo de salida debe ser distinto al de entrada")
        #expect(CasosCombinacion.comprobarP15(entrada: "/a", salida: "/b", o).isEmpty)
    }

    @Test func modos() async throws {
        let cols = ["11111111111111", "1x111111111111", "21111111111111", "11111111111111"]
        var o = OpcionesP15()
        (o.fijo, o.signoFijo, o.eliminar) = (true, "X", true)
        #expect(try await p15(cols, o).lineas == ["11111111111111X", "1X111111111111X", "21111111111111X", "11111111111111X"])
        o = OpcionesP15()
        (o.condicional, o.signoCondicion, o.signoSi, o.signoNo) = (true, "1", "1", "2")
        #expect(
            try await p15(cols, o).lineas == ["111111111111111", "1X1111111111111", "111111111111111", "211111111111112"])
        o = OpcionesP15()
        o.eliminar = true
        let r = try await p15(cols, o)
        #expect(r.lineas == Array(cols.prefix(3)).map { $0.uppercased() } && r.repetidas == 1)
        o = OpcionesP15()
        (o.copiar, o.partidoCopia) = (true, 2)
        #expect(
            try await p15(cols, o).lineas == ["111111111111111", "211111111111111", "111111111111111", "1X111111111111X"])
        // Bloques de 8: salen antes que la primera columna, que queda en el resto del «2».
        o = OpcionesP15()
        (o.copiar, o.partidoCopia) = (true, 1)
        let nueve = ["21111111111111"] + Array(repeating: "11111111111111", count: 8)
        #expect(try await p15(nueve, o).lineas == Array(repeating: "111111111111111", count: 8) + ["211111111111112"])
        #expect(try await p15([], o).lineas.isEmpty, "sin columnas no se crea el fichero")
        o = OpcionesP15()
        o.condicional = true
        await #expect(throws: ErrorDominio.self) { try await p15(cols, o) }
    }

    @Test func grafico() async throws {
        let g = try await CasosVer.graficoCombinacion(
            try fichero("g.txt", ["X1111111111111", "11111111111111", "x1111111111111"]), entorno: entorno)
        #expect(g.apuestas == 3 && g.minimo == 0 && g.maximo == 1)
        #expect(g.lineas.map { [$0.x, $0.y0, $0.y1] } == [[10, 51, 74], [11, 51, 74]])
        #expect(g.rellenos.count == 10 && g.rellenos.first.map { [$0.x, $0.y, $0.ancho, $0.alto] } == [2, 50, 957, 25])
        let h = try await CasosVer.graficoCombinacion(try fichero("h.txt", ["11111111111111", "22222222222222"]), entorno: entorno)
        #expect(h.maximo == 4_782_968 && h.rellenos.isEmpty && h.lineas.last.map { [$0.x, $0.y0, $0.y1] } == [954, 276, 299])
        let guia = CasosVer.guiaGrafico(minimo: 0, maximo: 4_782_968)
        #expect(guia.count == 10 && guia[0] == "01)  11111111111111" && guia[1] == "02)  11221122112211" && guia[9].hasPrefix("10)  "))
        #expect(try await CasosVer.graficoCombinacion(try fichero("v.txt", []), entorno: entorno).lineas.isEmpty)
    }
}
