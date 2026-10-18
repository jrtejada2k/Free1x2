import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Pruebas
import Testing

/// 07 F09, F10, F12, F13 y F17 con las semillas de `Free1X2.WinUI/Datos`.
@Suite("Ficheros de texto")
struct ArchivosTextoTests {
    @Test func equiposCategoria() throws {
        let e = ArchivoEquiposCategoria.leer(try Golden.bytes("Fixtures/datos/Equipos/equipos1.dat"))
        #expect(e.count == 20 && e.first == "Almería", "Latin-1: «í» = ED")
        #expect(
            [UInt8](ArchivoEquiposCategoria.datos(["Almería", "Ñ€"])) == [
                0x41, 0x6C, 0x6D, 0x65, 0x72, 0xED, 0x61, 0x0D, 0x0A, 0xD1, 0x3F, 0x0D, 0x0A,
            ])
        #expect(ArchivoEquiposCategoria.leer(Data("a\r\n\r\nb".utf8)) == ["a", "", "b"], "las vacías cuentan")
    }

    @Test func equiposBoleto() {
        let d = ArchivoEquiposBoleto.datos(["Madrid - Betis", "Ñandú - Más"])
        #expect([UInt8](d.prefix(3)) == [0xEF, 0xBB, 0xBF])
        #expect(String(decoding: d.dropFirst(3), as: UTF8.self) == "Madrid - Betis\r\nÑandú - Más\r\n")
        #expect(ArchivoEquiposBoleto.leer(d, numeroPartidos: 3) == ["Madrid - Betis", "Ñandú - Más", "? - ?"])
    }

    @Test func resultados() {
        let ok = ArchivoResultados.leer(Data("2020\t1\t1X2111111111111\n2022\t3\t111\ncorta\t1".utf8))
        #expect(ok.filas.count == 2 && ok.temporadas == ["2022", "2021", "2020"])
        let roto = ArchivoResultados.leer(Data("2020\t1\tX\nx\t2\tX\n2021\t1\tX".utf8))
        #expect(roto.filas.count == 1 && roto.temporadas.isEmpty, "el error corta en silencio y no hay temporadas")
        #expect(ArchivoResultados.leer(Data("2020\t1\t1234567890123456".utf8)).filas.isEmpty, "MaxLength 15")
        #expect(ArchivoResultados.leer(nil).filas.isEmpty)
    }

    @Test func impresoras() throws {
        let i = try ArchivoImpresoras.leer(try Golden.bytes("Fixtures/datos/Impresion/impresoras.cfg"))
        #expect(i.count == 18)
        #expect(
            i.contains(ArchivoImpresoras.Impresora(modelo: "HP OFFICEJET 5510", margenSuperior: 102, margenIzquierda: 32, rotar: false)))
    }

    @Test func configImpresion() throws {
        let semilla = try Golden.bytes("Fixtures/datos/Impresion/imprebol.cfg")
        let previo = ArchivoConfigImpresion.Config(margenSuperior: 1, margenIzquierdo: 2, modelo: "x", girar: true)
        let c = try #require(ArchivoConfigImpresion.leer(semilla, previo: previo, cultura: .esES))
        #expect(c == ArchivoConfigImpresion.Config(margenSuperior: 100, margenIzquierdo: 44, modelo: "Por Defecto", girar: false))
        #expect(
            String(decoding: ArchivoConfigImpresion.datos(c), as: UTF8.self)
                == String(decoding: semilla, as: UTF8.self).replacingOccurrences(of: "\n", with: "\r\n"))
        let raro = ArchivoConfigImpresion.leer(Data("a=x\nb\nmodelo=M=2\ngirar=si".utf8), previo: previo, cultura: .esES)
        #expect(raro == ArchivoConfigImpresion.Config(margenSuperior: 1, margenIzquierdo: 2, modelo: "M=2", girar: true))
        #expect(ArchivoConfigImpresion.leer(Data("a=1".utf8), previo: previo, cultura: .esES) == nil)
    }
}
