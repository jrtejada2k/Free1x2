import Darwin
import Free1X2Base
import Free1X2Motor

// ESPEC_14 §6: datasets fijos, Release; 2 calentamientos + 10 medidas; mediana, p90 y pico de RSS del proceso.
// Uso: `swift run -c release Free1X2Bench [B1 B5 B6 …]` (sin argumentos: todos).
// ponytail: B2–B4, B7 y B8 llegan con el cargador `.comb` y los formatos (M4).

let pedidos = Set(CommandLine.arguments.dropFirst())

func medir(_ id: String, techo: Duration, _ cuerpo: () throws -> Void) throws {
    guard pedidos.isEmpty || pedidos.contains(String(id.prefix(2))) else { return }
    let reloj = ContinuousClock()
    var t: [Duration] = []
    for i in 0..<12 {
        let d = try reloj.measure(cuerpo)
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
try medir("B1 14T ruta A", techo: .seconds(1)) {
    let r = try MotorCalculo.calcular(b1, configuracion: .porDefecto, leerLineas: { _ in [] })
    precondition(r.analizadas == 4_782_969)
}

// B5: reductores sobre el universo 9T, nivel 12.
if pedidos.isEmpty || pedidos.contains("B5") {
    let madre = try universo(triples: 9)
    for (nombre, a) in [("JDC", AlgoritmoReduccion.menosTiempo), ("JDCD", .menosColumnas), ("1305", .grandesArchivos)] {
        try medir("B5 9T nivel 12 \(nombre)", techo: .seconds(10)) {
            _ = try Reduccion.reducir(a, lineas: madre, nivel: 12, numeroPartidos: 14)
        }
    }
}

// B6: escrutinio de 100 000 columnas contra 1 ganadora y contra una temporada de 38.
if pedidos.isEmpty || pedidos.contains("B6") {
    let columnas = try aleatorias(100_000, semilla: 1)
    let ganadoras = try aleatorias(38, semilla: 2)
    try medir("B6 1 ganadora", techo: .seconds(1)) {
        var e = Escrutador(premios: [14, 13, 12, 11, 10], numeroPartidos: 14)
        _ = try e.escrutarConColumna(ganadoras[0], columnas: columnas, archivo: "")
    }
    try medir("B6 temporada 38", techo: .seconds(1)) {
        var e = Escrutador(premios: [14, 13, 12, 11, 10], numeroPartidos: 14)
        _ = try e.escrutarTemporada(ganadoras: ganadoras, columnas: columnas, archivo: "")
    }
}
