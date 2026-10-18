import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_10 §93 (CU-C36…C38), §94 (CU-C39) y ESPEC_08 §34, §35.
@Suite("Tramificar")
struct TramificarTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ lineas: [String]) throws -> URL {
        let url = URL(filePath: "/tmp/tr/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(lineas.map { $0 + "\r\n" }.joined().utf8), en: url, atomico: true)
        return url
    }

    @Test func tramos() async throws {
        let p = ParametrosTramificar(
            columnaInicial: 0, columnaFinal: 4_782_969, columnasPorTramo: 531_441, columnaPremiada: "11111111111111",
            premios: ["1000", "100", "10", "1", "0,5"], valoraciones: [[Double]](repeating: [50, 25, 25], count: 14),
            ln: 14 * log(0.5), porProductos: true, acumular: false)
        var e = try await CasosUtilidades.tramificar(EstadoTramificar(), p, cultura: .esES) { _ in }
        #expect(e.tramos.count == 9 && e.tramos[0].p14 == 1 && e.tramos[0].valorDerecha == 531_440)
        #expect(e.tramos.reduce(0) { $0 + $1.numColumnasTramo } == 4_782_969)
        #expect(
            [\Tramo.p13, \Tramo.p12, \Tramo.p11, \Tramo.p10].map { k in e.tramos.reduce(0) { $0 + $1[keyPath: k] } } == [
                28, 364, 2912, 16016,
            ])
        #expect(Array(e.limites.prefix(2)) == ["1", "1"])
        #expect(CasosUtilidades.puntosCurvaTramos(e.tramos, curva: "14 aciertos").first.map { [$0.x, $0.y] } == [0, 1])
        #expect(try CasosUtilidades.buscarTramificar(e, columna: "11111111111111") == 1)
        #expect(CasosUtilidades.moverTramificar(e, desde: 1, aciertos: 14, hacia: 1) == nil)
        #expect(CasosUtilidades.columnaTramificar(e, posicion: 1, premiada: "11111111111111", cultura: .esES)?.aciertos == "14")
        _ = CasosUtilidades.extremosTramificar(&e, limites: e.limites, cultura: .esES)
        let vacio = [[Int]](repeating: [0, 0, 0, 0], count: 10)
        var solo = vacio
        solo[0] = [0, 1, 0, 0]
        #expect(try await CasosUtilidades.filtrarTramos(e, extremos: solo).quedan == 4_782_968)
        solo[0] = [0, 1, 1, 1]
        let f = try await CasosUtilidades.filtrarTramos(e, extremos: solo)
        #expect(f.quedan == 4_782_969 - 28)
        #expect(CasosUtilidades.moverTramificar(f.estado, desde: 1, aciertos: 13, hacia: 1) == nil)
    }

    @Test func limites() {
        let d = CasosUtilidades.filasLimites(nil)
        #expect(d.map(\.difMax) == [4, 3, 2, 1, 0, 0, 1, 2, 0, 0] && d[0].rango == "0 a 0")
        let f = CasosUtilidades.filasLimites((0..<10).map { [$0 == 3 ? 0 : $0 * 10, $0 * 10 + 5, 0, 0] })
        #expect(f[1].rango == "11 a 15" && f[3].posInicial == 22)
        let a = CasosUtilidades.aceptarLimites(f)
        #expect(a[9][1...] == a[8][1...] && a[9][0] == a[8][0] + 1 && a[3][0] == 22)
    }

    @Test func multiple() throws {
        let campos = { (t: String) in ([t] + Array(repeating: "1", count: 43)).joined(separator: "\t") }
        let v = try fichero("v.txt", [campos("2004/2005"), campos("2004/2005"), campos("2005/2006")])
        #expect(CasosUtilidades.temporadasValoraciones(v, entorno: entorno) == ["2004/2005", "2005/2006"])
        #expect(CasosUtilidades.temporadasValoraciones(try fichero("m.txt", ["1\t2"]), entorno: entorno) == nil)
        #expect(CasosUtilidades.esFicheroDeColumnas(try fichero("c.txt", [" 1x2X1111111111 "]), entorno: entorno))
        #expect(!CasosUtilidades.esFicheroDeColumnas(v, entorno: entorno))
        let l = [CombinacionJornada(temporada: "2004/2005", jornada: "07", ruta: "/a,b/c.txt")]
        let r = CasosUtilidades.leerListaCombinaciones(CasosUtilidades.datosListaCombinaciones(l) + Data("corta\r\n".utf8))
        #expect(r.error && r.filas.map(\.ruta) == ["/AB/C.TXT"] && r.filas[0].jornada == "07")
    }

    @Test func jornadaLAE() throws {
        let g = {
            try CasosUtilidades.guardarJornadaTramificar(
                columna: "1X1222X2X12121", temporada: "2004", temporadaFin: "2005", jornada: 7, recaudacion: "7768049",
                premios: ["1165207,35", "29877,11", "1753,51", "165,38", "22,79"], lae: .original, entorno: entorno)
        }
        try g()
        try g()
        let url = entorno.rutas.base.appending(path: "Jornadas/InfoJornadasLAE.txt")
        #expect(
            TextoNET.lineas(try entorno.archivos.leer(url)) == [
                "1X1222X2X12121\t2004/2005\t07\t15536098\t2330414,7\t59754,22\t3507,02\t330,76\t45,58"
            ])
    }
}

extension ConfigLAE {
    /// Los 0,50 € y 14 M € fijos del original (DT-93): con ellos, los resultados son los de siempre.
    static let original: ConfigLAE = {
        var l = ConfigLAE()
        (l.precioApuesta, l.recaudacion) = (0.5, 14_000_000)
        return l
    }()
}

/// DT-93: con el precio de Configuración, lo que guarda Tramificar lo lee Estimación de premios con los mismos importes, y
/// el balance de un tramo descuenta columnas × precio.
struct PrecioConfigurableTests {
    @Test func jornadaGuardadaYLeidaConElMismoPrecio() throws {
        let entorno = Entorno.pruebas()
        var lae = ConfigLAE()
        (lae.precioApuesta, lae.recaudacion) = (0.75, 9_000_000)
        try CasosUtilidades.guardarJornadaTramificar(
            columna: "1X1222X2X12121", temporada: "2026", temporadaFin: "2027", jornada: 7, recaudacion: "",
            premios: ["1165207,35", "29877,11", "1753,51", "165,38", "22,79"], lae: lae, entorno: entorno)
        let d = try #require(
            try CasosUtilidades.datosJornadaLAE(
                temporada: "2026/2027", jornada: 7, recaudacion: 9_000_000, bote: 0, precio: 0.75, porcentajes: [12, 8, 8, 8, 9],
                entorno: entorno))
        #expect(abs(d.recaudacion - 9_000_000) < 1e-6)
        #expect(zip(d.premios, [1_165_207.35, 29_877.11, 1_753.51, 165.38, 22.79]).allSatisfy { abs($0 - $1) < 1e-6 })
    }

    @Test func balanceDelTramo() {
        var t = Tramo(premios: [1000, 0, 0, 0, 0])
        t.numColumnasTramo = 100
        t.ponerAciertos([1, 0, 0, 0, 0])
        t.calculaTotalImportePremios(precio: 0.75)
        #expect(t.totalImportePremios == 1000 && t.balance == 925)
        t.calculaTotalImportePremios(precio: 0.5)
        #expect(t.balance == 950)
    }
}
