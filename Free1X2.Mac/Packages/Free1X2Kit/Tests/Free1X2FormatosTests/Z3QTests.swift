import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Pruebas
import Testing

/// 06 §2.14 y ESPEC_16 §3: `.z3q` (suite `formatos/z3q`, CP-Z3Q-01 de `CompresorZipTests.cs`).
@Suite("Ficheros .z3q")
struct Z3QTests {
    struct Entrada: Decodable, Sendable {
        var tamano: Int
        var bitsActivos: [Int]
        var nivel: Int
    }
    struct Salida: Decodable, Sendable {
        var bytesDatos: Int
        var tamanoDescomprimido: Int
        var bitsActivos: [Int]
    }

    static let fecha = Date(timeIntervalSince1970: 1_760_000_000)

    @Test func idaYVuelta() throws {
        for caso in try Golden.casos("formatos/z3q", Entrada.self, Salida.self) {
            let e = caso.entrada
            var bits = ConjuntoBits(cantidad: e.tamano)
            for i in e.bitsActivos { try bits.poner(i) }
            let (zip, n) = try ArchivoZ3Q.comprimir(bits, tamano: e.tamano, fecha: Self.fecha)
            let s = try #require(caso.salida)
            #expect(n == s.bytesDatos, "\(caso.id)")
            let leido = try ArchivoZ3Q.descomprimir(zip)
            #expect(leido.cantidad == s.tamanoDescomprimido, "\(caso.id)")
            #expect(leido.indices == s.bitsActivos, "\(caso.id)")
        }
    }

    /// `SetLevel(nivel)`: 0 guarda sin comprimir, 9 comprime; los dos se leen igual (DT-62).
    @Test func niveles() throws {
        var bits = ConjuntoBits(cantidad: 4_782_969)
        for i in [0, 7, 1234, 4_782_968] { try bits.poner(i) }
        let (z0, _) = try ArchivoZ3Q.comprimir(bits, tamano: bits.cantidad, fecha: Self.fecha, nivel: 0)
        let (z9, _) = try ArchivoZ3Q.comprimir(bits, tamano: bits.cantidad, fecha: Self.fecha, nivel: 9)
        #expect(z0.count > 597_871 && z9.count < 10_000)
        #expect(try ArchivoZ3Q.descomprimir(z0).indices == bits.indices && ArchivoZ3Q.descomprimir(z9).indices == bits.indices)
    }

    /// Compatibilidad con cualquier lector ZIP (ESPEC_16 §3): `unzip -t` del sistema.
    @Test func zipEstandar() throws {
        var bits = ConjuntoBits(cantidad: 4_782_969)
        try bits.poner(5)
        let (zip, _) = try ArchivoZ3Q.comprimir(bits, tamano: bits.cantidad, fecha: Self.fecha)
        let url = FileManager.default.temporaryDirectory.appendingPathComponent("free1x2-\(UUID().uuidString).z3q")
        try zip.write(to: url)
        defer { try? FileManager.default.removeItem(at: url) }
        let p = Process()
        p.executableURL = URL(fileURLWithPath: "/usr/bin/unzip")
        p.arguments = ["-tq", url.path]
        p.standardOutput = FileHandle.nullDevice
        try p.run()
        p.waitUntilExit()
        #expect(p.terminationStatus == 0, "unzip -t acepta el fichero")
    }

    /// `tamanoReal`: más de 3^15 bits leídos ⇒ 3^15; si no, 3^14 (un bitmap de 3^13 se lee como 3^14).
    @Test func tamanoRealYErrores() throws {
        var b13 = ConjuntoBits(cantidad: 1_594_323)
        try b13.poner(1_594_322)
        let (z13, _) = try ArchivoZ3Q.comprimir(b13, tamano: b13.cantidad, fecha: Self.fecha)
        let l13 = try ArchivoZ3Q.descomprimir(z13)
        #expect(l13.cantidad == 4_782_969 && l13.indices == [1_594_322])
        var b15 = ConjuntoBits(cantidad: 14_348_907)
        try b15.poner(14_348_906)
        let (z15, n15) = try ArchivoZ3Q.comprimir(b15, tamano: b15.cantidad, fecha: Self.fecha)
        #expect(n15 == 1_793_614)
        #expect(try ArchivoZ3Q.descomprimir(z15).cantidad == 14_348_907)
        var corrupto = [UInt8](z13)
        corrupto[corrupto.count - 22 - 46 - 12 + 16] ^= 0xFF  // CRC del directorio central
        let e = #expect(throws: ErrorDominio.self) { try ArchivoZ3Q.descomprimir(Data(corrupto)) }
        #expect(e?.excepcion == .entradaSalida)
        let vacio = Data([0x50, 0x4B, 0x05, 0x06] + [UInt8](repeating: 0, count: 18))
        let e2 = #expect(throws: ErrorDominio.self) { try ArchivoZ3Q.descomprimir(vacio) }
        #expect(e2?.excepcion == .referenciaNula, "ZIP sin entradas: GetNextEntry() = null")
    }
}
