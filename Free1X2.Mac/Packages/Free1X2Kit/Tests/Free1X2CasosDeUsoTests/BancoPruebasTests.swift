import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_08 §10 (CU-A16…A24) y §36.
@Suite("Banco de pruebas")
struct BancoPruebasTests {
    let entorno = Entorno.pruebas()
    let uniforme = [[Double]](repeating: [34, 33, 33], count: 14)

    func fichero(_ lineas: [String]) throws -> URL {
        let url = URL(filePath: "/tmp/bp/c.txt")
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(lineas.map { $0 + "\r\n" }.joined().utf8), en: url, atomico: true)
        return url
    }

    @Test func reales() async throws {
        let r = try await CasosUtilidades.valoracionReal(
            apostados: [[Double]](repeating: [90, 5, 5], count: 14), ln: 14 * log(0.9), columnas: 100)
        #expect(r.allSatisfy { $0.reduce(0, +) == 100 } && r[0][0] > 80)
    }

    @Test func aleatorias() {
        var a = entorno.aleatorio(7)
        let r = CasosUtilidades.columnasAleatorias(
            reales: [[Double]](repeating: [100, 0, 0], count: 14), apostados: [[Double]](repeating: [50, 25, 25], count: 14),
            cantidad: 3, aleatorio: &a)
        #expect(r.columnas == [0, 0, 0] && abs(r.lnMedia - 14 * log(0.5)) < 1e-9 && r.desviacion < 1e-9)
    }

    @Test func analisis() async throws {
        let f = try fichero(["11111111111111"])
        let comb = try await CasosUtilidades.analizarBanco(
            .combinacion, fichero: f, apostados: uniforme, aleatorias: [0, 1], acumular: [true, false, false, false, false],
            numeroPartidos: 14, entorno: entorno)
        let filas = try #require(comb.filas)
        #expect(filas[14].hasPrefix("14 aciertos\tveces: 1\t") && filas[13].hasPrefix("13 aciertos\tveces: 1\t"))
        #expect(comb.gasto == "1" && comb.vecesPremiada == "2" && comb.numColumnas == "1")
        let jor = try await CasosUtilidades.analizarBanco(
            .jornadas, fichero: f, apostados: uniforme, aleatorias: [0, 1], acumular: [true, false, false, false, false],
            numeroPartidos: 14, entorno: entorno)
        #expect(
            jor.jornadas?.map { String($0.prefix(41)) } == [
                "Jornada 1    14:1  13:0  12:0  11:0  10:0", "Jornada 2    14:0  13:1  12:0  11:0  10:0",
            ])
        let g = try fichero(["11111111111111", "X1111111111111", "22222222222222"])
        let auto = try await CasosUtilidades.analizarBanco(
            .autoescrutinio, fichero: g, apostados: uniforme, aleatorias: [], acumular: [true, true, false, false, false],
            numeroPartidos: 14, entorno: entorno)
        let cols = try #require(auto.columnas)
        #expect(
            cols.map(\.veces) == [[1, 1, 0, 0, 0], [1, 1, 0, 0, 0], [1, 0, 0, 0, 0]] && auto.gasto == "1,5" && auto.premioTotal == "")
        #expect(cols[2].resumen.hasPrefix("22222222222222    14:1  13:0"))
    }

    @Test func seleccion() {
        let conv = ConvertidorDeBases(numeroPartidos: 14)
        var filas = ["11111111111111", "11111111111112", "22222222222222", "1111111111112X"].enumerated().map { i, c in
            FilaBancoColumna(
                indice: i, columna: c, columnaNumero: Int((try? conv.numero(c)) ?? 0), veces: [0, 0, 0, 0, 0], premio: "0",
                recuperacion: "0 %", seleccionado: true)
        }
        #expect(CasosUtilidades.simplificarBanco(&filas, fila: 9, diferencias: 2) == "El nº de fila está fuera de rango.")
        #expect(CasosUtilidades.simplificarBanco(&filas, fila: 0, diferencias: 2) == nil)
        #expect(filas.map(\.seleccionado) == [true, false, true, true])
        #expect(CasosUtilidades.rangoGrabacionBanco(filas) == (1, 4, 3))
        #expect(CasosUtilidades.columnasGrabacionBanco(filas, inicial: 1, final: 4, maximo: 0, soloSeleccionadas: true).count == 2)
        #expect(CasosUtilidades.columnasGrabacionBanco(filas, inicial: 1, final: 1, maximo: 0, soloSeleccionadas: false).count == 2)
    }
}
