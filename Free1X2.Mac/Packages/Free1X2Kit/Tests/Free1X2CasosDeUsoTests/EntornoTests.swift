import Foundation
import Free1X2Base
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_17 INF-03: el entorno de pruebas es determinista.
@Test func entornoDePruebasDeterminista() async throws {
    let entorno = Entorno.pruebas()
    #expect(entorno.reloj.ahora() == ISO8601DateFormatter().date(from: "2026-01-01T12:00:00Z"))
    #expect(!entorno.archivos.existe(entorno.rutas.base))
    let peticion = URLRequest(url: try #require(URL(string: "https://clubprogol.com")))
    await #expect(throws: URLError.self) { _ = try await entorno.red.datos(para: peticion) }
}

/// DP-13: las 10 carpetas de trabajo de hoy, bajo Application Support.
@Test func rutasDeProduccion() {
    let rutas = RutasApp.produccion()
    #expect(rutas.base.path(percentEncoded: false).hasSuffix("Application Support/Free1X2/"))
    #expect(RutasApp.Carpeta.allCases.count == 10)
    #expect(rutas.carpeta(.ganadoras).lastPathComponent == "Ganadoras")
}

/// Adaptador de producción de ficheros contra un directorio temporal real (ESPEC_14 §2).
@Suite("Sistema de archivos local")
struct SistemaArchivosLocalTests {
    let archivos = SistemaArchivosLocal()
    let carpeta = FileManager.default.temporaryDirectory.appending(path: "Free1X2-\(UUID().uuidString)")

    @Test func escribeLeeListaYBorra() throws {
        defer { try? FileManager.default.removeItem(at: carpeta) }
        try archivos.crearCarpeta(carpeta)
        for nombre in ["b.txt", "Z.txt", "a1.txt"] {
            try archivos.escribir(Data(nombre.utf8), en: carpeta.appending(path: nombre), atomico: true)
        }
        let fichero = carpeta.appending(path: "a1.txt")
        #expect(try archivos.leer(fichero) == Data("a1.txt".utf8))
        #expect(try archivos.listar(carpeta).map(\.lastPathComponent) == ["a1.txt", "b.txt", "Z.txt"])  // DT-14
        _ = try archivos.fechaModificacion(fichero)
        try archivos.borrar(fichero)
        #expect(!archivos.existe(fichero))
    }

    @Test func traduceLosErroresALasExcepcionesNET() {
        let sinFichero = #expect(throws: ErrorDominio.self) { try archivos.leer(carpeta.appending(path: "no-existe.txt")) }
        #expect(sinFichero?.excepcion == .archivoNoEncontrado)
        let sinCarpeta = #expect(throws: ErrorDominio.self) { try archivos.listar(carpeta) }
        #expect(sinCarpeta?.excepcion == .directorioNoEncontrado)
    }
}
