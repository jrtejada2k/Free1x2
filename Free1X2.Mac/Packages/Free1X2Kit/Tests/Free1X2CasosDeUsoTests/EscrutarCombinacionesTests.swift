import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_09 §44 (CU-B04); DEC-501/502.
@Suite("Escrutar combinaciones")
struct EscrutarCombinacionesTests {
    let entorno = Entorno.pruebas()

    func combinacion(_ nombre: String, _ pronostico: String, variantes: String? = nil) throws -> URL {
        var c = Combinacion(numeroPartidos: 14)
        c.ponerPronosticos(Array(repeating: pronostico, count: 14))
        if let variantes {
            try c.modificarGrupo(0) { (g: inout GrupoConfig) throws(ErrorDominio) in
                (g.condiciones.noVariantes.activa, g.condiciones.noVariantes.contieneDatos) = (true, true)
                try g.condiciones.noVariantes.setNoVariantes(variantes)
                try g.condiciones.noVariantes.setNoEquis(CasosCondiciones.todosValores(14))
                try g.condiciones.noVariantes.setNoDoses(CasosCondiciones.todosValores(14))
            }
        }
        let url = URL(filePath: "/tmp/ec2/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(
            try ArchivoCombinacion.datos(c, equipos: [], filtroColumnas: "", cultura: entorno.cultura), en: url, atomico: true)
        return url
    }

    @Test func escrutar() async throws {
        // GE-08-L2: con «1 variante» ninguna candidata cumple y los premios quedan a 0.
        let doses = try combinacion("d.comb", "1,X,2", variantes: "1")
        func entrada(_ m: ModoEscrutinio, _ f: [URL]) -> EntradaEscrutinio {
            EntradaEscrutinio(modo: m, ficheros: f, rangoAciertos: [13, 14], registrarPremiadas: false)
        }
        #expect(throws: ErrorDominio.self) { try CasosCombinacion.validarEscrutinioComb(entrada(.simple(ganadora: "**"), [doses])) }
        #expect(throws: ErrorDominio.self) { try CasosCombinacion.validarEscrutinioComb(entrada(.contraFichero(nil), [doses])) }
        let r = try await CasosCombinacion.escrutarCombinaciones(
            entrada(.simple(ganadora: "11111111111111"), [doses]), numeroPartidos: 14, configuracion: .porDefecto, entorno: entorno)
        #expect(r.map(\.aciertos) == [14, 13] && r.allSatisfy { $0.columnas == 0 })
        let ref = URL(filePath: "/tmp/ec2/ref.txt")
        try entorno.archivos.escribir(Data("11111111111111\r\nX1111111111111\r\n".utf8), en: ref, atomico: true)
        let f = try await CasosCombinacion.escrutarCombinaciones(
            entrada(.contraFichero(ref), [doses, doses]), numeroPartidos: 14, configuracion: .porDefecto, entorno: entorno)
        #expect(f.count == 2 && f.allSatisfy { $0.columnas == 0 })
        // DEC-501: la primera candidata que cumple lanza.
        let triples = try combinacion("t.comb", "1,X,2")
        await #expect(throws: ErrorDominio.self) {
            try await CasosCombinacion.escrutarCombinaciones(
                entrada(.simple(ganadora: "11111111111111"), [triples]), numeroPartidos: 14, configuracion: .porDefecto,
                entorno: entorno)
        }
    }
}
