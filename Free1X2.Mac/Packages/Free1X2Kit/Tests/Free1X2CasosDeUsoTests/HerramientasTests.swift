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
