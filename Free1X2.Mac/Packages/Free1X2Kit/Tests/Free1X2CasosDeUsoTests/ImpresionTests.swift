import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_09 §68 (CU-B36) y §70 (CU-B38).
@Suite("Impresión de boletos e impresoras conocidas")
struct ImpresionTests {
    let entorno = Entorno.pruebas()

    @Test func boletos() {
        let unos = Array(repeating: "11111111111111", count: 9)
        let b = CasosCombinacion.boletosImpresion(
            ["1X2111111111112"] + unos, fichero: "f.txt", margenSuperior: 100, margenIzquierdo: 30, desde: 1, hasta: 2)
        #expect(b.map(\.encabezado) == ["f.txt   Boleto 1", "f.txt   Boleto 2"] && b.map(\.marcas.count) == [113, 29])
        let m = b[0].marcas
        #expect(abs(m[0].x - 276.59) < 1e-9 && m[0].y == 183 && abs(m[1].y - 197.764) < 1e-9 && abs(m[2].y - 212.528) < 1e-9)
        #expect(m.last?.y == 405 && b[1].marcas.last?.y == 345, "pleno de la 1.ª columna: 2 ⇒ +305, 1 ⇒ +245")
        // 9 columnas: el primer boleto corta en 7 para no dejar uno de una sola.
        #expect(
            CasosCombinacion.boletosImpresion(unos, fichero: "f", margenSuperior: 0, margenIzquierdo: 0, desde: 1, hasta: 2)
                .map(\.marcas.count) == [99, 29])
        let ultimo = CasosCombinacion.boletosImpresion(
            Array(unos.prefix(8)), fichero: "f", margenSuperior: 0, margenIzquierdo: 0, desde: 2, hasta: 2)
        #expect(ultimo.map(\.marcas.count) == [15] && ultimo.first?.encabezado == "f   Boleto 2")
        #expect(CasosCombinacion.boletosImpresion(unos, fichero: "f", margenSuperior: 0, margenIzquierdo: 0, desde: 5, hasta: 5).isEmpty)
    }

    @Test func configuracion() throws {
        let previo = ArchivoConfigImpresion.Config(margenSuperior: 100, margenIzquierdo: 30, modelo: "(ninguna)", girar: false)
        #expect(CasosCombinacion.leerConfigImpresion(previo: previo, entorno: entorno) == nil)
        try CasosCombinacion.guardarConfigImpresion(
            .init(margenSuperior: 98.7, margenIzquierdo: 31, modelo: "HP", girar: true), entorno: entorno)
        #expect(
            CasosCombinacion.leerConfigImpresion(previo: previo, entorno: entorno)
                == .init(margenSuperior: 98, margenIzquierdo: 31, modelo: "HP", girar: true))
    }

    @Test func impresoras() throws {
        let xml = """
            <xml><impresoras><c modelo="Zeta" ms="1" mi="2" girar="false" /><c modelo="alfa" ms="3" mi="4" girar="true" />\
            <c modelo="Beta" ms="5" mi="6" girar="False" /></impresoras></xml>
            """
        let url = entorno.rutas.recursos.appending(path: "Datos/Impresion/impresoras.cfg")
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(xml.utf8), en: url, atomico: true)
        let l = try CasosCombinacion.impresorasConocidas(entorno: entorno)
        #expect(l.map(\.modelo) == ["alfa", "Beta", "Zeta"] && l[0].rotar)
        #expect(CasosCombinacion.buscarImpresora("Beta", en: l)?.margenSuperior == 5)
        #expect(CasosCombinacion.buscarImpresora("Otra", en: l)?.modelo == "Zeta", "no encontrada ⇒ la última")
    }
}
