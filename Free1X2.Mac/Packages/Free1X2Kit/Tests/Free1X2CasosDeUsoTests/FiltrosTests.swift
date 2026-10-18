import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_09 §50 (CU-B15…B17) y ESPEC_08 §33 (CU-A80…A83).
@Suite("Separador Juan Bellas y diferencias entre filtros")
struct FiltrosTests {
    let entorno = Entorno.pruebas()

    func fichero(_ nombre: String, _ lineas: [String]) throws -> URL {
        let url = URL(filePath: "/tmp/fil/" + nombre)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(Data(lineas.map { $0 + "\r\n" }.joined().utf8), en: url, atomico: true)
        return url
    }

    let limites = [15.0, 22, 29, 36, 48, 61]
    let porcentajes = Array(repeating: [50.0, 30, 20], count: 14)

    @Test func porcenJB() async throws {
        var rangos = ["0-3", "0-3", "0-3", "0-3", "0-3", "0-14", "0-3", "0-14"]
        let b = try #require(CasosUtilidades.bandasJB(porcentajes: porcentajes, limites: limites, rangos: rangos))
        let e = try fichero("e.txt", ["11111111111111", "x1111111111111", "", "11111111111111", "111", "22222222222222"])
        let r = try await CasosUtilidades.filtrarJB(e, bandas: b, entorno: entorno)
        #expect(r.procesadas == 4 && r.admitidas == ["11111111111111", "41111111111111"] && r.corta == "111")
        let a = try #require(CasosUtilidades.analizarJB(b, columna: "x1111111111111"))
        #expect(a.bandas == [0, 0, 0, 1, 0, 13, 0] && a.recorrido == 13 && CasosUtilidades.analizarJB(b, columna: "111") == nil)
        let x = CasosUtilidades.exportarJB(b)
        #expect(x[1] == Array(repeating: "2", count: 14).joined(separator: ",") && x[3].hasPrefix("X,X") && x[0].hasPrefix("-,-"))
        #expect(CasosUtilidades.bandasJB(porcentajes: porcentajes, limites: limites, rangos: ["3"] + rangos.dropFirst()) == nil)
        #expect(CasosUtilidades.bandasJB(porcentajes: porcentajes, limites: [15, 10, 29, 36, 48, 61], rangos: rangos) == nil)
        let d = CasosUtilidades.datosRangosJB(rangos: rangos, limites: [15.7, 22, 29, 36, 48, 61])
        var (r2, l2) = (Array(repeating: "", count: 8), Array(repeating: 0.0, count: 6))
        CasosUtilidades.leerRangosJB(d, rangos: &r2, limites: &l2, cultura: .esES)
        #expect(r2 == rangos && l2 == [15, 22, 29, 36, 48, 61])
        rangos[5] = "0-3"
        let c = try #require(CasosUtilidades.bandasJB(porcentajes: porcentajes, limites: limites, rangos: rangos))
        #expect(try await CasosUtilidades.filtrarJB(e, bandas: c, entorno: entorno).admitidas.isEmpty)
    }

    func fila(_ url: URL, _ difs: String, _ minimo: Double = 1) -> FilaDiFiltros {
        var f = FilaDiFiltros(ruta: url.path(percentEncoded: false), nombre: url.lastPathComponent)
        (f.difs, f.minimo) = (difs, minimo)
        return f
    }

    @Test func diFiltros() async throws {
        let base = try fichero("b.txt", ["11111111111111", "X1111111111111", "22222222222222", "X1111111111111"])
        let f1 = try fichero("f1.txt", ["1X111111111111"])
        var r = try await CasosUtilidades.procesoDiFiltros(
            [fila(base, "1111"), fila(f1, "0100")], estado: EstadoDiFiltros(), ganadora: nil, entorno: entorno)
        #expect(r.filas.map(\.columnas) == [4, 1] && r.filas.map(\.admitidas) == [4, 2], "4 líneas con duplicado: 4 − 2")
        r = try await CasosUtilidades.procesoDiFiltros(
            [fila(base, "1111"), fila(f1, "011")], estado: r.estado, ganadora: "11111111111111", entorno: entorno)
        #expect(r.filas[1].admitidas == 3 && r.filas[1].acierta14 == 1 && r.filas[1].aciertos13 == 1)
        let s = URL(filePath: "/tmp/fil/s.txt")
        try CasosUtilidades.grabarDiFiltros(r.estado, numeroPartidos: 14, en: s, entorno: entorno)
        #expect(TextoNET.lineas(try entorno.archivos.leer(s)) == ["11111111111111", "X1111111111111"])
        let mal = try await CasosUtilidades.procesoDiFiltros(
            [fila(URL(filePath: "/no/existe.txt"), "1111"), fila(f1, "1000", 2)], estado: r.estado, ganadora: nil, entorno: entorno)
        #expect(!mal.filas[0].activo && mal.filas[1].admitidas == 3, "sin base, el contador queda el de la vez anterior")
        let l = CasosUtilidades.leerListaDiFiltros(CasosUtilidades.datosListaDiFiltros([fila(f1, "01", 7)]), cultura: .esES)
        #expect(l.count == 1 && l[0].difs == "01" && l[0].minimo == 7 && l[0].activo && l[0].nombre == "f1.txt")
        let g = try CasosUtilidades.ganadorasDiFiltros(try fichero("g.txt", [" 1x1X1111111111 ", "1Z11111111111111"]), entorno: entorno)
        #expect(g.columnas == ["1x1X1111111111"] && g.erronea)
    }
}
