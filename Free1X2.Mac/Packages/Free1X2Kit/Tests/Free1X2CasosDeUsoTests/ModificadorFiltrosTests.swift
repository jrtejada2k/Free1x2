import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_09 §73; ESPEC_08 §12 (CU-A27) y §21 (CU-A53…A57).
@Suite("Modificador, buscador de límites y combinar filtros")
struct ModificadorFiltrosTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ texto: String) throws -> URL {
        let url = URL(filePath: "/tmp/mf2/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(texto.utf8), en: url, atomico: true)
        return url
    }

    @Test func modificador() throws {
        let cols = ["11111111111111", "11111111111111", "X1111111111111", "21111111111111"]
        var e = try EditorModificador.cargar(try fichero("m.txt", cols.joined(separator: "\r\n") + "\r\n"), entorno: entorno)
        #expect(e.partidos == 14 && e.frecuencia[0] == [50, 25, 25] && e.frecuencia[1] == [100, 0, 0])
        var r = AleatorioNET(semilla: 1)
        // Ordenado: 2 «X» y 2 «1» en el partido 2 (los dos primeros X por ser el más frecuente).
        try e.modificar(partido: 1, porcentajes: [50, 50, 0], modo: 3, ordenarSignos: true, aleatorio: &r)
        #expect(e.columnas.map { $0.map { String($0.dropFirst().prefix(1)) } } == ["X", "X", "1", "1"] && e.frecuencia[1] == [50, 50, 0])
        try e.modificar(partido: 2, porcentajes: [10, 5, 20], modo: 2, ordenarSignos: true, aleatorio: &r)
        #expect(throws: ErrorDominio.self) {
            try e.modificar(partido: 3, porcentajes: [30, 20, 50], modo: 2, ordenarSignos: false, aleatorio: &r)
        }
        try e.modificar(partido: 4, porcentajes: [0, 0, 100], modo: 1, ordenarSignos: true, aleatorio: &r)
        #expect(e.columnas.allSatisfy { $0?.utf16.dropFirst(4).first == 0x32 })
        #expect(try e.datos() != nil)
    }

    @Test func buscaLims() async {
        let dv = [[60.0, 30, 10], [20, 30, 50]]
        let r = await CasosUtilidades.buscarLimites(dv, rango: "1-2")
        // Base «12»: 9 columnas, admitidas las de 1 o 2 aciertos (5).
        #expect(r.columnaBase == "12" && r.procesadas == 9 && r.admitidas == 5 && r.sumaMax == 110 && r.sumaMin == 60)
        let q = await CasosUtilidades.buscarLimites([[50.0, 30, 20], [40, 30, 50]], rango: "0")
        #expect(q.columnaBase == "11", "la condición del 1 compara con dv[0][2]")
    }

    @Test func combinar() async throws {
        let a = try fichero("a.txt", "11111111111111\r\n11111111111111\r\nX1111111111111\r\n")
        let b = try fichero("b.txt", "11111111111111\r\n12\r\n2X111111111111\r\n")
        #expect(CasosUtilidades.signosDeFiltro(a, entorno: entorno) == 14)
        let r = try await CasosUtilidades.combinarFiltros([(a, true), (b, true)], partidos: 14, entorno: entorno)
        #expect(r.columnas == [3, 2] && r.cuentas.reduce(0, +) == 3)
        let rango = CasosUtilidades.rangoFiltros(2, 5, filtros: 2)
        #expect(rango == 2...2)
        let s = URL(filePath: "/tmp/mf2/s.txt")
        #expect(try CasosUtilidades.grabarFiltros(r.cuentas, rango: rango, en: s, entorno: entorno) == 1)
        #expect(String(decoding: try entorno.archivos.leer(s), as: UTF8.self) == "11111111111111\r\n")
        let lst = try fichero("l.lst", "/x/a.txt\r\n\r\n /y/b.txt\r\n")
        #expect(try CasosUtilidades.rutasDeLista(lst, entorno: entorno) == ["/x/a.txt", " /y/b.txt"])
    }
}
