import Darwin
import Foundation
import Free1X2Base
import Free1X2CasosDeUso
import Free1X2Formatos
import Free1X2Motor
import Free1X2Pruebas

// ESPEC_14 §6: datasets fijos, Release; 2 calentamientos + 10 medidas; mediana, p90 y pico de RSS del proceso.
// Uso: `swift run -c release Free1X2Bench [B1 B5 B6 …]` (sin argumentos: todos).

let pedidos = Set(CommandLine.arguments.dropFirst())

/// Un argumento elige los datasets cuyo nombre empieza por él («B8» o «B8 tramificar»).
func quiere(_ id: String) -> Bool { pedidos.isEmpty || pedidos.contains { $0.hasPrefix(id) || id.hasPrefix($0) } }

func medir(_ id: String, techo: Duration, _ cuerpo: () async throws -> Void) async throws {
    guard pedidos.isEmpty || pedidos.contains(where: { id.hasPrefix($0) }) else { return }
    let reloj = ContinuousClock()
    var t: [Duration] = []
    for i in 0..<12 {
        let d = try await reloj.measure(cuerpo)
        if i >= 2 { t.append(d) }
    }
    t.sort()
    let mediana = (t[4] + t[5]) / 2
    var uso = rusage()
    getrusage(RUSAGE_SELF, &uso)
    let veredicto = mediana <= techo ? "OK" : "SUPERA el techo \(techo)"
    print("\(id)\tmediana \(mediana)\tp90 \(t[8])\tRSS pico \(uso.ru_maxrss / 1_048_576) MB\t\(veredicto)")
}

/// Columnas de la combinación con `triples` triples y el resto «1», por la ruta B.
func universo(triples: Int) throws -> [String] {
    var c = Combinacion(numeroPartidos: 14)
    c.ponerPronosticos((0..<14).map { $0 < triples ? "1,X,2" : "1" })
    var lineas: [String] = []
    _ = try MotorCalculo.calcular(c, configuracion: .porDefecto, leerLineas: { _ in [] }) { col throws(ErrorDominio) in
        lineas.append(try UtilColumnas.convLongToStr(col))
    }
    return lineas
}

/// Columnas pseudoaleatorias reproducibles (semilla fija).
func aleatorias(_ n: Int, semilla: Int32) throws -> [String] {
    var r = AleatorioNET(semilla: semilla)
    let signos: [Character] = ["1", "X", "2"]
    var cols: [String] = []
    for _ in 0..<n {
        var s = ""
        for _ in 0..<14 { s.append(signos[Int(try r.next(3))]) }
        cols.append(s)
    }
    return cols
}

// B1: 14 triples, sin condiciones, ruta A.
var b1 = Combinacion(numeroPartidos: 14)
b1.ponerPronosticos(Array(repeating: "1,X,2", count: 14))
try await medir("B1 14T ruta A", techo: .seconds(1)) {
    let r = try MotorCalculo.calcular(b1, configuracion: .porDefecto, leerLineas: { _ in [] })
    precondition(r.analizadas == 4_782_969)
}

// B5: reductores sobre el universo 9T, nivel 12.
if quiere("B5") {
    let madre = try universo(triples: 9)
    for (nombre, a) in [("JDC", AlgoritmoReduccion.menosTiempo), ("JDCD", .menosColumnas), ("1305", .grandesArchivos)] {
        try await medir("B5 9T nivel 12 \(nombre)", techo: .seconds(10)) {
            _ = try Reduccion.reducir(a, lineas: madre, nivel: 12, numeroPartidos: 14)
        }
    }
}

// B6: escrutinio de 100 000 columnas contra 1 ganadora y contra una temporada de 38.
if quiere("B6") {
    let columnas = try aleatorias(100_000, semilla: 1)
    let ganadoras = try aleatorias(38, semilla: 2)
    try await medir("B6 1 ganadora", techo: .seconds(1)) {
        var e = Escrutador(premios: [14, 13, 12, 11, 10], numeroPartidos: 14)
        _ = try e.escrutarConColumna(ganadoras[0], columnas: columnas, archivo: "")
    }
    try await medir("B6 temporada 38", techo: .seconds(1)) {
        var e = Escrutador(premios: [14, 13, 12, 11, 10], numeroPartidos: 14)
        _ = try e.escrutarTemporada(ganadoras: ganadoras, columnas: columnas, archivo: "")
    }
}

// B2–B4: 14 triples + `corpus/comb/completa.comb` (los 14 filtros con datos, grupos, controles e If-Then), por las rutas A
// (contar), B (grabar `.txt`) y C (análisis).
let b2 = try ArchivoCombinacion.leer(
    try Golden.bytes("Fixtures/corpus/comb/completa.comb"), numeroPartidos: 14, cultura: .esES
).combinacion
try await medir("B2 completa ruta A", techo: .seconds(10)) {
    let r = try MotorCalculo.calcular(b2, configuracion: .porDefecto, leerLineas: { _ in [] })
    precondition(r.analizadas == 4_782_969)
}
let temporal = FileManager.default.temporaryDirectory.appending(path: "free1x2-bench.txt")
try await medir("B3 completa ruta B", techo: .seconds(13)) {
    FileManager.default.createFile(atPath: temporal.path, contents: nil)
    let h = try FileHandle(forWritingTo: temporal)
    var buf: [UInt8] = []
    buf.reserveCapacity(1 << 20)
    _ = try MotorCalculo.calcular(b2, configuracion: .porDefecto, leerLineas: { _ in [] }) { col throws(ErrorDominio) in
        buf.append(contentsOf: try UtilColumnas.convLongToStr(col).utf8)
        buf.append(contentsOf: [13, 10])
        if buf.count >= 1 << 20 {
            h.write(Data(buf))
            buf.removeAll(keepingCapacity: true)
        }
    }
    h.write(Data(buf))
    try h.close()
}
try await medir("B4 completa ruta C", techo: .seconds(20)) {
    _ = try MotorCalculo.analizar(b2, configuracion: .porDefecto, leerLineas: { _ in [] })
}

// B7: escribir y leer un `.txt` de 1 000 000 columnas.
if quiere("B7") {
    let millon = try aleatorias(1_000_000, semilla: 3)
    try await medir("B7 1M columnas escribir+leer", techo: .seconds(3)) {
        try TextoNET.codificarLineas(millon).write(to: temporal)
        let leido = try Data(contentsOf: temporal)
        precondition(ArchivoColumnasTexto.columnas(leido).count == 1_000_000)
    }
}

// B8: tres CU pesados de pantalla sobre las 3^14 columnas.
if quiere("B8") {
    let entorno = Entorno.pruebas()
    let valores = (0..<14).map { [40.0 + Double($0), 35, 25 - Double($0)] }
    var p = ParametrosOrdenacion(entrada: nil, salida: URL(filePath: "/tmp/b8/s.txt"), valoraciones: valores)
    (p.recaudacion, p.precio, p.porcentaje14, p.maxColumnas) = (15_000_000, 0.5, 15, 1000)
    try entorno.archivos.crearCarpeta(URL(filePath: "/tmp/b8"))
    try await medir("B8 ordenar por probabilidad", techo: .seconds(5)) {
        _ = try await CasosUtilidades.ordenarPorProbabilidad(
            p, centrales: ValoresCentrales(), numeroPartidos: 14, entorno: entorno
        ) { _ in }
    }
    let t = ParametrosTramificar(
        columnaInicial: 0, columnaFinal: 4_782_969, columnasPorTramo: 531_441, columnaPremiada: "1X1222X2X12121",
        premios: ["1000", "100", "10", "1", "0,5"], valoraciones: valores, ln: -14.7, porProductos: true, acumular: false)
    try await medir("B8 tramificar", techo: .seconds(5)) {
        _ = try await CasosUtilidades.tramificar(EstadoTramificar(), t, cultura: .esES) { _ in }
    }
    try await medir("B8 banco valoración real", techo: .seconds(5)) {
        _ = try await CasosUtilidades.valoracionReal(apostados: valores, ln: -14.7, columnas: 50_000)
    }
}

try? FileManager.default.removeItem(at: temporal)
