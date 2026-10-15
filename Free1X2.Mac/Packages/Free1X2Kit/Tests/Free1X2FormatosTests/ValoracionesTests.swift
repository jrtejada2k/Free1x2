import Foundation
import Free1X2Base
import Free1X2Formatos
import Testing

/// 07 F07 y 06 §2.7: ficheros de valoraciones (detección de formato, lectores y escritores), cultura `es-ES`.
@Suite("Valoraciones")
struct ValoracionesTests {
    static func leer(
        _ lineas: [String], temporada: String = "", jornada: String = "", descripcion: String = ""
    ) -> LecturaValoraciones {
        ArchivoValoraciones.leer(
            Data(lineas.joined(separator: "\r\n").utf8), numeroPartidos: 14, cultura: .esES, temporada: temporada,
            jornada: jornada, descripcion: descripcion)
    }

    static let fila: [Double] = [1.5, 2, 3.5]
    static let catorce = [[Double]](repeating: fila, count: 14)
    static let ceros = [[Double]](repeating: [0, 0, 0], count: 14)
    static let numeros42 = Array(repeating: ["1.5", "2", "3.5"], count: 14).flatMap { $0 }

    @Test func formatos() {
        let f3 = Self.leer(Array(repeating: "1,5 2 3,5", count: 14))
        #expect(f3.formato == 3 && f3.separador == " " && f3.valores == Self.catorce && f3.aviso == nil)
        let f3c = Self.leer(Array(repeating: "1.5,2,3.5", count: 14))
        #expect(f3c.formato == 3 && f3c.separador == ",", "EsNumero(\"1.5\") en es-ES: el punto es de miles")
        #expect(f3c.valores == Self.catorce)
        let f1 = Self.leer(Array(repeating: ["1.5", " 2 ", "3,5"], count: 14).flatMap { $0 })
        #expect(f1.formato == 1 && f1.valores == Self.catorce)
        let f42 = Self.leer([(Self.numeros42 + ["9", "9", "9"]).joined(separator: "\t")])
        #expect(f42.formato == 42 && f42.separador == "\t" && f42.valores == Self.catorce, "45 campos: sobran 3")
        let f43 = Self.leer(
            ["A " + Self.numeros42.joined(separator: " "), "B " + Self.numeros42.joined(separator: " ")], descripcion: "B")
        #expect(f43.formato == 43 && f43.descripciones == ["A", "B"] && f43.jornadaEncontrada && f43.valores == Self.catorce)
        let f43no = Self.leer(["A " + Self.numeros42.joined(separator: " ")], descripcion: "Z")
        #expect(!f43no.jornadaEncontrada && f43no.valores == Self.ceros)
        let f44 = Self.leer(
            ["2023/2024;1".replacingOccurrences(of: ";", with: ",") + "," + Self.numeros42.joined(separator: ",")],
            temporada: "2023/2024", jornada: "1")
        #expect(f44.formato == 44 && f44.jornadaEncontrada && f44.valores == Self.catorce)
    }

    @Test func errores() {
        #expect(Self.leer(["hola"]).aviso == ArchivoValoraciones.avisoIncorrecto)
        var lineas = Array(repeating: "1 2 3", count: 14)
        lineas[4] = "1 x 3"
        let roto = Self.leer(lineas)
        #expect(roto.aviso == ArchivoValoraciones.avisoCorrupto)
        #expect(roto.valores[3] == [1, 2, 3] && roto.valores[4] == [1, 0, 0], "lo leído antes del fallo se queda")
        #expect(Self.leer(Array(repeating: "1 2 3", count: 15)).aviso == nil, "con i == N el error se ignora")
        #expect(Self.leer(Array(repeating: "1 2 3", count: 16)).aviso == ArchivoValoraciones.avisoCorrupto)
        #expect(Self.leer(["5"]).aviso == ArchivoValoraciones.avisoCorrupto, "formato 1 sin las 42 líneas")
        let dos42 = Self.leer([Self.numeros42.joined(separator: " "), Self.numeros42.joined(separator: " ")])
        #expect(dos42.aviso == ArchivoValoraciones.avisoCorrupto && dos42.valores == Self.catorce, "la 2.ª línea desborda")
    }

    @Test func escritura() throws {
        let t3 = String(
            decoding: try ArchivoValoraciones.datos(Self.catorce, formato: 3, separador: " ", numeroPartidos: 14, cultura: .esES),
            as: UTF8.self)
        #expect(t3 == String(repeating: "1.5 2 3.5\r\n", count: 14))
        let t1 = String(
            decoding: try ArchivoValoraciones.datos(Self.catorce, formato: 1, separador: " ", numeroPartidos: 14, cultura: .esES),
            as: UTF8.self)
        #expect(t1.hasPrefix("1,5\r\n2\r\n3,5\r\n"))
        let t42 = String(
            decoding: try ArchivoValoraciones.datos(Self.catorce, formato: 42, separador: ",", numeroPartidos: 14, cultura: .esES),
            as: UTF8.self)
        #expect(t42 == Self.numeros42.map { $0 + "," }.joined() + "\r\n", "separador final")
        #expect(
            try ArchivoValoraciones.datos(Self.catorce, formato: 43, separador: ",", numeroPartidos: 14, cultura: .esES).isEmpty)
    }

    @Test func historico() throws {
        let linea = "2023/2024\t1\t" + Array(repeating: ["1,5", "2", "3,5"], count: 14).flatMap { $0 }.joined(separator: "\t")
        let nuevo = try ArchivoValoraciones.datosHistorico(
            existente: nil, separador: "\t", valores: Self.catorce, temporada: "2023/2024", jornada: "1", numeroPartidos: 14,
            cultura: .esES)
        #expect(nuevo.datos.map { String(decoding: $0, as: UTF8.self) } == linea + "\r\n")
        let otra = linea.replacingOccurrences(of: "2023/2024\t1", with: "2023/2024\t2")
        let previo = Data((otra + "\n" + linea.replacingOccurrences(of: "1,5", with: "9")).utf8)
        let r = try ArchivoValoraciones.datosHistorico(
            existente: previo, separador: ",", valores: Self.catorce, temporada: "2023/2024", jornada: "1", numeroPartidos: 14,
            cultura: .esES)
        #expect(r.datos.map { String(decoding: $0, as: UTF8.self) } == otra + "\r\n" + linea + "\r\n", "se sustituye en su sitio")
        let no = try ArchivoValoraciones.datosHistorico(
            existente: Data("1 2 3".utf8), separador: "\t", valores: Self.catorce, temporada: "x", jornada: "1",
            numeroPartidos: 14, cultura: .esES)
        #expect(no.datos == nil && no.aviso == ArchivoValoraciones.avisoNoHistorico)
    }
}
