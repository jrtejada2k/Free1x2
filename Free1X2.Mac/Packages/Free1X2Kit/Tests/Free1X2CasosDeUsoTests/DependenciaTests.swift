import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_09 §54 (CU-B21) y §55 (CU-B22).
@Suite("Dependencia lineal y reducciones perfectas")
struct DependenciaTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ lineas: [String]) throws -> URL {
        let url = URL(filePath: "/tmp/dl/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(lineas.map { $0 + "\r\n" }.joined().utf8), en: url, atomico: true)
        return url
    }

    @Test func dependencia() async throws {
        let e = try fichero("e.txt", ["11111111111111", "X1111111111111", "21111111111111", "11111111111111"])
        let l = try await CasosUtilidades.leerDependencia(e, entorno: entorno)
        #expect(l.porcentajes[0] == [50, 25, 25] && l.porcentajes[1] == [100, 0, 0] && l.bits.indices == [0, 1, 2])
        let p = CasosUtilidades.proponerCoeficientes(l.porcentajes)
        #expect(p.coeficientes == [1] + Array(repeating: 0, count: 13) && p.partido == 1)
        let s = URL(filePath: "/tmp/dl/s.txt")
        let triple = try #require(CasosUtilidades.parametrosDependencia(true, true, true))
        #expect(
            try await CasosUtilidades.aplicarDependencia(
                l.bits, partido: 1, coeficientes: p.coeficientes, parametros: triple, salida: s, numeroPartidos: 14, entorno: entorno)
                == 3)
        #expect(TextoNET.lineas(try entorno.archivos.leer(s)) == ["11111111111111", "XX111111111111", "22111111111111"])
        let x2 = try #require(CasosUtilidades.parametrosDependencia(false, true, true))
        _ = try await CasosUtilidades.aplicarDependencia(
            l.bits, partido: 1, coeficientes: p.coeficientes, parametros: x2, salida: s, numeroPartidos: 14, entorno: entorno)
        #expect(TextoNET.lineas(try entorno.archivos.leer(s)) == ["1X111111111111", "2X111111111111", "X2111111111111"])
        #expect(CasosUtilidades.parametrosDependencia(true, false, false) == nil)
        #expect(CasosUtilidades.parametrosDependencia(true, false, true)?.corrector == 2)
    }

    func filas(_ n: Int, pron: [Bool], base: Int) -> [FilaReducidaPerfecta] {
        var f = FilaReducidaPerfecta()
        (f.pronostico, f.base) = (pron, base)
        var resto = FilaReducidaPerfecta()
        resto.pronostico = [true, false, false]
        return Array(repeating: f, count: n) + Array(repeating: resto, count: 15 - n)
    }

    @Test func reduccionesPerfectas() throws {
        let t = try CasosUtilidades.reduccionPerfecta(filas(4, pron: [true, true, true], base: 0), longitud: 15)
        #expect(t.count == 9 && t.prefix(3) == ["111111111111111", "X1XX11111111111", "2X1X11111111111"] && t.last == "212211111111111")
        let d = try CasosUtilidades.reduccionPerfecta(filas(7, pron: [false, true, true], base: 1), longitud: 15)
        #expect(d.count == 16 && d.prefix(2) == ["XXXXXXX11111111", "2XXXX2211111111"] && d.last == "222222211111111")
        #expect(
            try CasosUtilidades.reduccionPerfecta(filas(4, pron: [true, true, true], base: 0), longitud: 14).allSatisfy { $0.count == 14 })
        func error(_ f: [FilaReducidaPerfecta], _ l: Int) -> String {
            do {
                _ = try CasosUtilidades.reduccionPerfecta(f, longitud: l)
                return ""
            } catch { return error.mensajeUsuario }
        }
        #expect(error(filas(5, pron: [true, true, true], base: 0), 15) == "Nº de triples incorrecto")
        #expect(error(filas(4, pron: [true, true, true], base: 0), 3) == "La longitud de la columna es incorrecta")
        #expect(error(filas(4, pron: [false, true, true], base: 0), 15) == "La columna base no está incluida en el pronóstico")
        var mezcla = filas(4, pron: [true, true, true], base: 0)
        mezcla[10].pronostico = [true, true, false]
        #expect(error(mezcla, 15) == "No se permite mezcla de dobles y triples")
        #expect(error(filas(6, pron: [true, true, false], base: 0), 15) == "Nº de dobles incorrecto")
        var fuera = filas(7, pron: [true, true, false], base: 0)
        fuera[0].base = 2
        #expect(error(fuera, 0) == "Los signos base no forman parte del pronostico")
    }
}
