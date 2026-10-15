import Foundation
import Free1X2Base
import Free1X2Pruebas
import Testing

/// ESPEC_17 NET-01…11: suite `net/formato` (comportamiento .NET 8 leído del fuente de dotnet/runtime).
@Suite("Formato .NET")
struct FormatoNETTests {
    static func casos(_ componente: String) throws -> [CasoGolden<Campos, Campos>] {
        try Golden.casos("net/formato", componente: componente)
    }

    @Test func conversionesDeDouble() throws {
        for caso in try Self.casos("FormatoNET.castInt32") {
            let d = try #require(caso.entrada.valor?.doble)
            #expect(Int(FormatoNET.castInt32(d)) == caso.salida?.entero, "\(caso.id)")
        }
        for caso in try Self.casos("FormatoNET.convertToInt32Double") {
            let d = try #require(caso.entrada.valor?.doble)
            #expect(
                verificar(caso, Result { () throws(ErrorDominio) in try FormatoNET.convertToInt32(d) }) {
                    Int($0) == caso.salida?.entero
                } == nil)
        }
    }

    @Test func formatoDeDouble() throws {
        for caso in try Self.casos("FormatoNET.toString") {
            let d = try #require(caso.entrada.valor?.doble)
            #expect(FormatoNET.toString(d, cultura: .esES) == caso.salida?.texto, "\(caso.id)")
        }
        for caso in try Self.casos("FormatoNET.toStringFijo") {
            let d = try #require(caso.entrada.valor?.doble)
            let n = try #require(caso.entrada.decimales)
            #expect(FormatoNET.toStringFijo(d, decimales: n, cultura: .esES) == caso.salida?.texto, "\(caso.id)")
        }
    }

    @Test func parseo() throws {
        for caso in try Self.casos("FormatoNET.convertToInt32") {
            let t = try #require(caso.entrada.texto)
            #expect(
                verificar(caso, Result { () throws(ErrorDominio) in try FormatoNET.convertToInt32(t) }) {
                    Int($0) == caso.salida?.entero
                } == nil)
        }
        for caso in try Self.casos("FormatoNET.convertToDouble") {
            let t = try #require(caso.entrada.texto)
            #expect(
                verificar(caso, Result { () throws(ErrorDominio) in try FormatoNET.convertToDouble(t, cultura: .esES) }) {
                    $0.bitPattern == caso.salida?.valor?.doble?.bitPattern
                } == nil)
        }
        for caso in try Self.casos("FormatoNET.convertToBoolean") {
            let t = try #require(caso.entrada.texto)
            #expect(
                verificar(caso, Result { () throws(ErrorDominio) in try FormatoNET.convertToBoolean(t) }) {
                    $0 == caso.salida?.valor?.booleano
                } == nil)
        }
    }

    @Test func texto() {
        #expect(FormatoNET.mayusculas("ñandú straße") == "ÑANDÚ STRAßE")  // mapeo simple: ß se queda
        #expect(FormatoNET.recortar("\u{A0} 1X2\t\r\n") == "1X2")
        #expect(FormatoNET.esLetra(0x00F1) && !FormatoNET.esLetra(0x0031))
    }
}

@Suite("FormatoNET · pantallas de cálculo")
struct FormatoCalculoTests {
    @Test func milesYTiempos() throws {
        #expect(FormatoNET.toStringMiles(4_782_969, decimales: 0, decimalesNegativo: 0, cultura: .esES) == "4.782.969")
        #expect(FormatoNET.toStringMiles(0, decimales: 0, decimalesNegativo: 0, cultura: .esES) == "0")
        #expect(FormatoNET.toStringMiles(2_391_484.5, decimales: 2, decimalesNegativo: 1, prefijo: "€", cultura: .esES) == "€2.391.484,50")
        #expect(FormatoNET.toStringMiles(0.125, decimales: 2, decimalesNegativo: 2, cultura: .esES) == "0,13")
        #expect(FormatoNET.toStringMiles(-3.25, decimales: 2, decimalesNegativo: 1, prefijo: "€", cultura: .esES) == "3,3")
        #expect(FormatoNET.toStringTimeSpan(3.1234567) == "00:00:03.1234567")
        #expect(FormatoNET.toStringTimeSpan(90_061) == "1.01:01:01")
        let fecha = try #require(Calendar.current.date(from: DateComponents(year: 2026, month: 1, day: 1, hour: 9, minute: 5, second: 7)))
        #expect(FormatoNET.horaLarga(fecha, cultura: .esES) == "9:05:07")
        #expect(FormatoNET.horaLarga(fecha, cultura: .enUS) == "9:05:07 AM")
    }
}
