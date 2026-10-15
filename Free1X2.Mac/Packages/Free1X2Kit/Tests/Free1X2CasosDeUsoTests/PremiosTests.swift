import Foundation
import Free1X2CasosDeUso
import Free1X2Motor
import Free1X2Pruebas
import Testing

/// ESPEC_09 §46 (CU-B09, CU-B10) y ESPEC_10 §83 (CU-C20, CU-C21).
@Suite("Estimación de premios y rentabilidad")
struct PremiosTests {
    let entorno = Entorno.pruebas()

    @Test func estimacion() throws {
        #expect(CasosUtilidades.temporadaActual(Date(timeIntervalSince1970: 1_760_000_000)) == "2025/2026")
        let v = Array(repeating: [50.0, 30, 20], count: 14)
        #expect(
            CasosUtilidades.estimarPremios(
                valores: v, columna: "111", numApuestas: 1, recaudacion: 1, bote: 0, porcentajes: [12, 8, 8, 8, 9], cultura: .esES) == nil)
        let p = try #require(
            CasosUtilidades.estimarPremios(
                valores: v, columna: "11111111111111", numApuestas: 28_000_000, recaudacion: 13_825_260.5, bote: 0,
                porcentajes: [12, 8, 8, 8, 9], cultura: .esES))
        // 0,5^14 · 28 M = 1708,98…; 13 ⇒ 14·1708,98·1 = 23925,8 → 23926.
        #expect(p.acertantes[0] == "1709" && p.acertantes[1] == "23926" && p.numApuestas == "28000000")
        #expect(p.premios[0] == "970,77")
    }

    @Test func historicoLAE() throws {
        let url = entorno.rutas.base.appending(path: "Jornadas/InfoJornadasLAE.txt")
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data("11111111111111\t2025/2026\t05\t1000\t0,00\t2\t4\t0\t1\r\n".utf8), en: url, atomico: true)
        let r = try #require(
            try CasosUtilidades.datosJornadaLAE(
                temporada: "2025/2026", jornada: 5, recaudacion: 100, bote: 0, precio: 0.5, porcentajes: [12, 8, 8, 8, 9],
                entorno: entorno))
        #expect(r.columna == "11111111111111" && r.recaudacion == 500 && r.premios[0] == 60 && r.premios[1] == 1 && r.premios[4] == 0)
        #expect(r.acertantes == [0, 8, 4, 0, 18])
        try CasosUtilidades.guardarEscrutinioLAE(
            temporada: "2025/2026", jornada: 6, columna: "1X21X21X21X21X", recaudacion: 1000, acertantes: [1, 2, 3, 4, 5],
            lae: ConfigLAE(), entorno: entorno)
        let t = String(decoding: try entorno.archivos.leer(url), as: UTF8.self)
        #expect(t.components(separatedBy: "\r\n").count == 3 && t.contains("1X21X21X21X21X\t2025/2026\t06\t"))
    }

    @Test func rentabilidad() async throws {
        let v = Array(repeating: [50.0, 30, 20], count: 14)
        let c = try CasosUtilidades.valorarColumnaRentabilidad(
            "11111111111111", apostados: v, reales: v, recaudacion: 8_000_000, lae: ConfigLAE(), cultura: .esES)
        #expect(c.probabilidad == "6,1035156E-05" && c.esperanza == c.esperanza)
        let s = URL(filePath: "/tmp/re/s.txt")
        try entorno.archivos.crearCarpeta(s.deletingLastPathComponent())
        let e = URL(filePath: "/tmp/re/e.txt")
        try entorno.archivos.escribir(Data("11111111111111\r\n2222222222222X\r\n".utf8), en: e, atomico: true)
        let n = try await CasosUtilidades.calcularRentabilidad(
            entrada: e, salida: s, apostados: v, reales: v, recaudacion: 8_000_000, lae: ConfigLAE(), emMin: 0, emMax: 50,
            ordenar: true, conEM: false, numeroPartidos: 14, entorno: entorno, estado: { _ in })
        #expect(n == 2)
        #expect(String(decoding: try entorno.archivos.leer(s), as: UTF8.self).hasPrefix("11111111111111\r\n"))
        var a = [3, 1, 2, 1].map { ApuestaProbable(probabilidad: Float($0), columna: Int32($0)) }
        OrdenacionPropia.ordenar(&a) { $0.probabilidad }
        #expect(a.map(\.probabilidad) == [1, 1, 2, 3])
    }
}

/// ESPEC_10 §76 (CU-C05…C08).
@Suite("Ordenar por probabilidad")
struct OrdenarPorProbabilidadTests {
    let entorno = Entorno.pruebas()

    @Test func valorCentral() {
        let v = Array(repeating: [50.0, 30, 20], count: 14)
        var c = CasosUtilidades.valorCentral(
            .acertantes, texto: "1.5", valoraciones: v, recaudacion: 15_000_000, precio: 0.5, porcentaje14: 15, previo: ValoresCentrales())
        #expect(c.acertantes == 1.5 && c.premio == 1_500_000 && abs(c.probabilidad - 5e-8) < 1e-20)
        c = CasosUtilidades.valorCentral(.ln, texto: "x", valoraciones: v, recaudacion: 1, precio: 1, porcentaje14: 1, previo: c)
        #expect(abs(c.probabilidad - 5e-8) < 1e-18, "texto ilegible: se arrastra el LN anterior")
        let col = CasosUtilidades.valorCentral(.columna, texto: "1", valoraciones: v, recaudacion: 1, precio: 1, porcentaje14: 1, previo: c)
        #expect(col.probabilidad == 0.5)
    }

    @Test func ordenarFichero() async throws {
        let e = URL(filePath: "/tmp/op/e.txt")
        try entorno.archivos.crearCarpeta(e.deletingLastPathComponent())
        try entorno.archivos.escribir(Data("11111111111111\r\n2222222222222X\r\n".utf8), en: e, atomico: true)
        var p = ParametrosOrdenacion(
            entrada: e, salida: URL(filePath: "/tmp/op/s.txt"), valoraciones: Array(repeating: [50.0, 30, 20], count: 14))
        (p.recaudacion, p.precio, p.porcentaje14, p.pestana, p.valorCentralSumas) = (15_000_000, 0.5, 15, 1, 700)
        _ = try await CasosUtilidades.ordenarPorProbabilidad(p, centrales: ValoresCentrales(), numeroPartidos: 14, entorno: entorno) { _ in
        }
        // Sumas: «1…1» = 700 (dif 0); «2…2X» = 13·20+30 = 290. Se graban las 2 del fichero primero.
        let t = String(decoding: try entorno.archivos.leer(p.salida), as: UTF8.self).components(separatedBy: "\r\n")
        #expect(t.prefix(2) == ["11111111111111", "2222222222222X"] && t.count == 3)
    }
}
