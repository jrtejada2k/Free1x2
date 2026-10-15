import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_08 §14 (CU-A32, CU-A33).
@Suite("Calcular varias")
struct LoteTests {
    let entorno = Entorno.pruebas()

    @Test func nombresYPreparacion() async throws {
        let carpeta = URL(filePath: "/tmp/Columnas")
        #expect(CasosCombinacion.nombreSalidaDeCombinacion(URL(filePath: "/x/j38.comb"), carpeta: carpeta).lastPathComponent == "j38.txt")
        #expect(
            CasosCombinacion.nombreSalidaDeCombinacion(URL(filePath: "/x/a.xml.comb"), carpeta: carpeta).lastPathComponent == "a.txt.txt")
        #expect(
            CasosCombinacion.ordenarLote([URL(filePath: "/b"), URL(filePath: "/B"), URL(filePath: "/a")]).map(\.lastPathComponent) == [
                "B", "a", "b",
            ])

        var c = Combinacion(numeroPartidos: 14)
        c.ponerPronosticos((0..<14).map { $0 < 2 ? "1,X,2" : "1" })
        let url = URL(filePath: "/tmp/lote/uno.comb")
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try CasosInicio.guardarCombinacion(c, equipos: [], filtroColumnas: "/tmp/no-existe.txt", en: url, entorno: entorno)
        let p = try CasosCombinacion.prepararCombinacionDeLote(url, numeroPartidos: 14, entorno: entorno)
        #expect(p.maximas == 9 && p.combinacion.archivoColumnasBase.isEmpty)
        #expect(p.aviso == "No se ha encontrado el filtro /tmp/no-existe.txt. Se calculan las columnas sin filtro.")
        let destino = CasosCombinacion.nombreSalidaDeCombinacion(url, carpeta: carpeta)
        let r = try await CasosCombinacion.calcularCombinacion(
            p.combinacion, modo: .grabar(destino), configuracion: .porDefecto, entorno: entorno)
        #expect(r.calculo.aceptadas == 9 && entorno.archivos.existe(destino))
    }
}
