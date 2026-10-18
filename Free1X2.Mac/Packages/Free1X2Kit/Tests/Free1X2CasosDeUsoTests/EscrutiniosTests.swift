import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_09 §45 y ESPEC_08 §20 (CU-B05…B08).
@Suite("Escrutinios")
struct EscrutiniosTests {
    let entorno = Entorno.pruebas()

    func escribir(_ texto: String, _ ruta: String) throws -> URL {
        let url = URL(filePath: ruta)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(texto.utf8), en: url, atomico: true)
        return url
    }

    @Test func validacion() {
        func mensaje(_ m: ModoEscrutinio, _ f: [URL] = []) -> String? {
            do {
                try CasosCombinacion.validarEscrutinio(
                    EntradaEscrutinio(modo: m, ficheros: f, rangoAciertos: [14], registrarPremiadas: false))
                return nil
            } catch { return error.mensajeUsuario }
        }
        let a = URL(filePath: "/tmp/a.txt")
        #expect(mensaje(.simple(ganadora: "")) == "Falta fichero a escrutar.")
        #expect(mensaje(.simple(ganadora: ""), [a]) == nil)
        #expect(mensaje(.contraFichero(nil), [a]) == "Falta fichero de referencia.")
        #expect(
            mensaje(.contraJornadas(plantilla: "q", carpeta: nil, digitosTemporada: 4, digitosJornada: 2, temporadas: []))
                == "Falta la carpeta de los ficheros.")
        let c = URL(filePath: "/tmp")
        #expect(
            mensaje(.contraJornadas(plantilla: "", carpeta: c, digitosTemporada: 4, digitosJornada: 2, temporadas: []))
                == "Falta plantilla de nombre de fichero.")
        #expect(
            mensaje(.contraJornadas(plantilla: "q/j", carpeta: c, digitosTemporada: 4, digitosJornada: 2, temporadas: []))
                == "No se ha puesto el indicador de temporada (/t).")
        #expect(
            mensaje(.contraJornadas(plantilla: "q/t", carpeta: c, digitosTemporada: 4, digitosJornada: 2, temporadas: []))
                == "No se ha puesto el indicador de jornada (/j).")
    }

    @Test func simpleYPremiadas() async throws {
        let a = try escribir("11111111111111\r\n1111111111111X\r\nXXXXXXXXXXXXXX\r\n", "/tmp/esc/a.txt")
        let b = try escribir("11111111111111\r\n", "/tmp/esc/b.txt")
        let e = EntradaEscrutinio(
            modo: .simple(ganadora: "11111111111111"), ficheros: [a, b], rangoAciertos: [13, 14], registrarPremiadas: true)
        let r = try await CasosCombinacion.escrutarColumnas(e, numeroPartidos: 14, entorno: entorno)
        #expect(r.filas.map(\.archivo) == ["a.txt", "b.txt", "-------------"] && r.filas.last?.columna == "----- TOTALES:")
        #expect(r.globales[14] == 2 && r.globales[13] == 1 && r.globales[0] == 1 && r.premiadas?.total == 3)
        let p = CasosCombinacion.proyectarEscrutinio(r, rangoAciertos: [13, 14])
        #expect(p.cabeceras == [14, 13] && p.filas[0].conteos == ["1", "1"] && p.filas[0].premios == "14: 1  13: 1")
        #expect(p.histograma.map(\.columnas) == [2, 1])
        let f = CasosCombinacion.filasPremiadas(try #require(r.premiadas).pagina(0))
        #expect(
            f[0].archivoColumnas == "a.txt" && f[0].numeroBoleto == "1 (1)" && f[1].numeroColumna == "2" && f[1].numeroBoleto == "1 (2)")
        #expect(f.count == 3 && f[2].archivoColumnas == "b.txt" && f[2].columna == "11111111111111")
        #expect(CasosCombinacion.estadoFinalEscrutinio(0.5) == "Final = 00:00:00.5")
        #expect(
            String(decoding: CasosCombinacion.datosColumnas(["1,X", "----- TOTALES:"], quitarComas: true), as: UTF8.self)
                == "1X\r\n----- TOTALES:\r\n")
    }

    @Test func contraJornadas() async throws {
        _ = try escribir(
            "2024\t1\t11111111111111\r\n2024\t2\tXXXXXXXXXXXXXX\r\n2023\t1\t22222222222222\r\n",
            entorno.rutas.carpeta(.jornadas).appending(path: "Resultados.txt").path(percentEncoded: false))
        _ = try escribir("11111111111111\r\n", "/tmp/jor/q2024_01.txt")
        _ = try escribir("XXXXXXXXXXXXXX\r\n", "/tmp/jor/q2024_02.txt")
        let e = EntradaEscrutinio(
            modo: .contraJornadas(
                plantilla: "q/t_/j.txt", carpeta: URL(filePath: "/tmp/jor"), digitosTemporada: 4, digitosJornada: 2, temporadas: [2024]),
            ficheros: [], rangoAciertos: [14], registrarPremiadas: false)
        let r = try await CasosCombinacion.escrutarColumnas(e, numeroPartidos: 14, entorno: entorno)
        #expect(r.filas.map(\.archivo) == ["q2024_01.txt", "q2024_02.txt", "-------------"] && r.globales[14] == 2)
        #expect(r.filas[1].columna == "XXXXXXXXXXXXXX")
    }

    @Test func casillas() {
        #expect(CasosCombinacion.casillasGanadora("1x 2", numeroPartidos: 6) == ["1", "X", "*", "2", "*", "*"])
        #expect(CasosCombinacion.textoGanadora(["1", "", "x", "S"]) == "1*XS")
        #expect(
            CasosCombinacion.ciclarSigno("*") == "1" && CasosCombinacion.ciclarSigno("2") == "*" && CasosCombinacion.ciclarSigno("S") == "1"
        )
    }
}
