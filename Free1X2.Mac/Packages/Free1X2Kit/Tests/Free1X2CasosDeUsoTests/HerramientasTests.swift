import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_08 §22-§23 y ESPEC_09 §66.
@Suite("Importar/exportar, compresor y configuración de CPs")
struct HerramientasTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ texto: String) throws -> URL {
        let url = URL(filePath: "/tmp/he/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(texto.utf8), en: url, atomico: true)
        return url
    }

    @Test func importExport() throws {
        let csv = try fichero("Cols.CSV", "1,x,2\r\n")
        let r = try CasosUtilidades.leerParaConvertir(csv, aTxt: true, entorno: entorno)
        #expect(r.columnas == ["1X2"] && r.salida.lastPathComponent == "cols.txt")
        #expect(try CasosUtilidades.convertirColumnas(r.columnas, aTxt: true, salida: r.salida, entorno: entorno) == 1)
        let t = try CasosUtilidades.leerParaConvertir(r.salida, aTxt: false, entorno: entorno)
        #expect(t.columnas == ["1,X,2"] && t.salida.lastPathComponent == "cols.csv")
    }

    @Test func compresor() async throws {
        let txt = try fichero("c.txt", "1X21X21X21X21X\r\n11111111111111\r\n1X21X21X21X21X\r\n")
        let bits = try #require(try CasosUtilidades.bitmapDeColumnas(txt, entorno: entorno))
        #expect(bits.cantidad == 4_782_969 && bits.indices.count == 2)
        let z = try await CasosUtilidades.comprimirZ3Q(bits, en: URL(filePath: "/tmp/he/c.txt"), nivel: 5, entorno: entorno)
        #expect(z.ruta.lastPathComponent == "c.z3q" && z.datos == 597_872)
        let leido = try await CasosUtilidades.descomprimirZ3Q(z.ruta, entorno: entorno)
        let salida = URL(filePath: "/tmp/he/d.txt")
        #expect(try await CasosUtilidades.columnasDeBitmap(leido, en: salida, entorno: entorno) == 2)
        #expect(String(decoding: try entorno.archivos.leer(salida), as: UTF8.self) == "11111111111111\r\n1X21X21X21X21X\r\n")
        #expect(try CasosUtilidades.bitmapDeColumnas(try fichero("m.txt", "1X21X21X21X21X\r\n1X2\r\n"), entorno: entorno) == nil)
    }

    @Test func configCPs() throws {
        let ns = "xmlns:d2p1=\"http://tempuri.org/ColumnasProbables.xsd\""
        let xml = """
            <?xml version="1.0" standalone="yes"?>
            <ColumnasProbables xmlns="http://tempuri.org/ColumnasProbables.xsd">
              <Tipos_x0020_de_x0020_CPs d2p1:Tipo="1" d2p1:Nombre="Normal" \(ns) />
              <Configuracion_x0020_de_x0020_CPs d2p1:Desde="5" \(ns) />
              <Configuracion_x0020_de_x0020_CPs d2p1:Tipo="1" d2p1:Desde="10" d2p1:Hasta="20" d2p1:Forzar_x0020_Fijos="true" \(ns) />
            </ColumnasProbables>
            """
        let url = entorno.rutas.base.appending(path: "ColumnasProbables.xml")
        try entorno.archivos.crearCarpeta(entorno.rutas.base)
        try entorno.archivos.escribir(Data(xml.utf8), en: url, atomico: true)
        var e = try EditorConfigCPs.cargar(entorno: entorno)
        #expect(e.nombres == ["Normal"])
        var filas = e.filas("Normal")
        #expect(filas.count == 1 && filas[0].desde == 10 && filas[0].forzarFijos && filas[0].numFijos == 0)
        filas[0].hasta = 33.9
        try e.guardar(filas, tipo: "Normal", entorno: entorno)
        #expect(try EditorConfigCPs.cargar(entorno: entorno).filas("Normal")[0].hasta == 33)
    }
}

/// ESPEC_09 §58 (CU-B26) y §59 (CU-B27).
@Suite("Generador de CPs")
struct GeneradorCPsTests {
    @Test func porDiferencias() {
        let c1 = "11111111111111"
        let c2 = "X1111111111111"
        #expect(
            CasosCondiciones.columnaCPValida(c1) && !CasosCondiciones.columnaCPValida("1111")
                && !CasosCondiciones.columnaCPValida("A1111111111111"))
        let f1 = CasosCondiciones.cpsPorDiferencias(c1, c2, dobles: false, diferencias: 1)
        #expect(f1.count == 15 && f1[0].hasPrefix("X,1,") && f1[14] == Array(repeating: "1", count: 14).joined(separator: ","))
        let f2 = CasosCondiciones.cpsPorDiferencias(c1, c2, dobles: false, diferencias: 2)
        #expect(f2.count == 92 && f2[0] == f1[14])
        // cM = "2" + "X2"·13: desde el partido 2 el segundo signo se desalinea.
        let d1 = CasosCondiciones.cpsPorDiferencias(c1, c2, dobles: true, diferencias: 1)
        #expect(d1[14].hasPrefix("12,1X,12,1X"))
    }

    @Test func generar() async throws {
        let entorno = Entorno.pruebas()
        let ns = "xmlns:d2p1=\"http://tempuri.org/ColumnasProbables.xsd\""
        let xml = """
            <?xml version="1.0" standalone="yes"?>
            <ColumnasProbables xmlns="http://tempuri.org/ColumnasProbables.xsd">
              <Tipos_x0020_de_x0020_CPs d2p1:Tipo="1" d2p1:Nombre="Mi tipo" \(ns) />
              <Configuracion_x0020_de_x0020_CPs d2p1:Tipo="1" d2p1:Desde="40" d2p1:Hasta="100" \(ns) />
            </ColumnasProbables>
            """
        try entorno.archivos.crearCarpeta(entorno.rutas.base)
        try entorno.archivos.escribir(Data(xml.utf8), en: entorno.rutas.base.appending(path: "ColumnasProbables.xml"), atomico: true)
        let v = Array(repeating: [50.0, 30, 20], count: 14)
        #expect(try await CasosCondiciones.generarCPs(v, jornada: 7, entorno: entorno) == 1)
        let t = String(decoding: try entorno.archivos.leer(entorno.rutas.base.appending(path: "Condiciones/Mi tipo_j7.txt")), as: UTF8.self)
        #expect(t == Array(repeating: "1", count: 14).joined(separator: ",") + "\r\n\r\n")
    }
}

/// ESPEC_10 §74 (CU-C01) y ESPEC_09 §53 (CU-B20).
@Suite("Multiplicador y fraccionador")
struct MultiplicadorFraccionadorTests {
    let entorno = Entorno.pruebas()

    @Test func multiplicador() async throws {
        let url = URL(filePath: "/tmp/mf/e.txt")
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data("1X2\r\n\r\n2222222222222222\r\n".utf8), en: url, atomico: true)
        let e = try CasosUtilidades.columnasMultiplicador(url, entorno: entorno)
        #expect(e == ["1X211111111111", "11111111111111", "22222222222222"])
        #expect(CasosUtilidades.plantillaMultiplicador([1, 2, 3, 4, 5, 15, 16, 17, 18, 19, 20, 21, 13, 13]) != nil)
        #expect(CasosUtilidades.plantillaMultiplicador([0] + Array(repeating: 1, count: 13)) == nil)
        #expect(CasosUtilidades.plantillaMultiplicador([.nan] + Array(repeating: 1, count: 13)) == nil)
        let p = Array(1...14).reversed().map { $0 + 14 }
        let ok = try await CasosUtilidades.multiplicarColumnas(["11111111111111"], ["1X21X21X21X21X"], plantilla: Array(p))
        #expect(ok == ["X12X12X12X12X1"])
        await #expect(throws: ErrorDominio.self) {
            try await CasosUtilidades.multiplicarColumnas(["1"], ["2"], plantilla: Array(p))
        }
    }

    @Test func fraccionador() async throws {
        let cols = (0..<10).map(String.init)
        let base = URL(filePath: "/tmp/mf/BASE")
        try entorno.archivos.crearCarpeta(base.deletingLastPathComponent())
        func leer(_ k: String) throws -> String {
            String(decoding: try entorno.archivos.leer(URL(filePath: "/tmp/mf/BASE\(k).txt")), as: UTF8.self)
        }
        #expect(try await CasosUtilidades.fraccionar(cols, base: base, cuotas: [3, 20, 0, 5], tramos: 0, entorno: entorno) == 2)
        #expect(try leer("01") == "0\r\n1\r\n2\r\n" && leer("02").hasPrefix("3\r\n") && leer("02").hasSuffix("9\r\n"))
        #expect(try await CasosUtilidades.fraccionar(cols, base: base, cuotas: nil, tramos: 4, entorno: entorno) == 4)
        #expect(try leer("04") == "6\r\n7\r\n", "N=10, q=4: se pierden 8 y 9")
    }
}
