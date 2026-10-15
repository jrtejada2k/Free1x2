import Free1X2Base
import Free1X2Pruebas
import Testing

@testable import Free1X2Motor

/// ESPEC_17 FIL-*: cada filtro por separado — suites `motor/filtros/*`.
/// Entrada: `config` = lista de `[setter, valor]` aplicados en orden; `columnas` a analizar. Salida: veredicto y contadores
/// por columna; tolerancias, fallos y `Get*` tras la última.
@Suite("Filtros")
struct FiltrosTests {
    struct Entrada: Decodable, Sendable {
        var config: [[String]]
        var columnas: [String]
    }
    struct Salida: Decodable, Sendable {
        var validas: [Bool]?
        var contadores: [[Double]]?
        var dobles: [[DoubleGolden]]?
        var tolerancias: [String: Int]?
        var fallos: [String]?
        var get: [String: String]?
    }

    /// Un filtro listo para probar: sus contadores tras `analizar` y los `Get*` de su condición.
    struct Prueba {
        let filtro: any FiltroColumna
        let contadores: () -> [Double]
        let get: [String: String]
    }

    static let suites = [
        "motor/filtros/novariantes", "motor/filtros/distancias", "motor/filtros/valoracion", "motor/filtros/formatos123",
        "motor/filtros/simetrias", "motor/filtros/dibujos", "motor/filtros/formatossignos",
    ]

    /// Columna de texto con `*` = partido fuera del grupo (`000`).
    static func columna(_ texto: String) -> Columna {
        var c: Columna = 0
        for (i, s) in texto.utf16.enumerated() {
            let g: Int64 = s == 0x31 ? 4 : s == 0x58 ? 2 : s == 0x32 ? 1 : 0
            c |= g << (3 * i)
        }
        return c
    }

    static func dobles(_ texto: String) throws -> [Double] {
        try trozosBarra(texto).map { try FormatoNET.convertToDouble($0, cultura: .enUS) }
    }

    static func trozosBarra(_ texto: String) -> [String] { texto.split(separator: "/").map(String.init) }

    static func enteros(_ texto: String) throws -> [Int] {
        try texto.split(separator: ",").map { Int(try FormatoNET.convertToInt32(String($0))) }
    }

    static func prueba(_ componente: String, _ config: [[String]]) throws -> Prueba {
        let n = 14
        switch componente {
        case "FiltroNoVariantes":
            var c = CondicionNoVariantes(numeroPartidos: n)
            c.activa = true
            for p in config {
                switch p[0] {
                case "variantes": try c.setNoVariantes(p[1])
                case "equis": try c.setNoEquis(p[1])
                case "doses": try c.setNoDoses(p[1])
                case "llenar": try c.llenarTodosValores()
                default: Issue.record("setter desconocido \(p[0])")
                }
            }
            let f = FiltroNoVariantes(c, analisisActivo: false)
            return Prueba(
                filtro: f, contadores: { [f.noVariantes, f.noEquis, f.noDoses].map(Double.init) },
                get: ["variantes": c.getVariantes, "equis": c.getEquis, "doses": c.getDoses])
        case "FiltroDistancias":
            var c = CondicionDistancias(numeroPartidos: n)
            c.activa = true
            for p in config {
                switch p[0] {
                case "var": try c.setDistVariantes(p[1])
                case "unos": try c.setDistUnos(p[1])
                case "equis": try c.setDistEquis(p[1])
                case "doses": try c.setDistDoses(p[1])
                case "global": try c.setNoIntGlobales(p[1])
                case "llenar": try c.llenarTodosValores()
                default: Issue.record("setter desconocido \(p[0])")
                }
            }
            let f = FiltroDistancias(c, analisisActivo: false)
            return Prueba(
                filtro: f, contadores: { [f.distVariantes, f.distUnos, f.distEquis, f.distDoses].map(Double.init) },
                get: ["var": c.getIntVar, "unos": c.getInt1, "equis": c.getIntX, "doses": c.getInt2])
        case "FiltroValoracionSignos":
            var c = CondicionValoracionSignos(numeroPartidos: n)
            c.activa = true
            for p in config {
                switch p[0] {
                case "tipo": c.tipoValoracion = p[1]
                case "valores":
                    let v = try dobles(p[1])
                    c.valores1 = stride(from: 0, to: v.count, by: 3).map { v[$0] }
                    c.valoresX = stride(from: 1, to: v.count, by: 3).map { v[$0] }
                    c.valores2 = stride(from: 2, to: v.count, by: 3).map { v[$0] }
                case "global": try c.ponerValorGlobal(p[1], cultura: .esES)
                case "unos": try c.ponerValorUnos(p[1], cultura: .esES)
                case "equis": try c.ponerValorEquis(p[1], cultura: .esES)
                case "doses": try c.ponerValorDoses(p[1], cultura: .esES)
                default: Issue.record("setter desconocido \(p[0])")
                }
            }
            let f = FiltroValoracionSignos(c, analisisActivo: false, cultura: .esES)
            return Prueba(
                filtro: f, contadores: { [f.valoracion, f.valoracionUnos, f.valoracionEquis, f.valoracionDoses] }, get: [:])
        case "FiltroFormatos123":
            var c = CondicionFormatos123(numeroPartidos: n)
            c.activa = true
            for p in config {
                switch p[0] {
                case "valoracion":
                    let v = try dobles(p[1])
                    c.valoracion = stride(from: 0, to: v.count, by: 3).map { Array(v[$0..<$0 + 3]) }
                    c.valoracionTransformada = try CondicionFormatos123.transformar(c.valoracion)
                case "formato":
                    let t = p[1].split(separator: ",").map(String.init)
                    c.formatos.append(
                        Formato123(
                            formato: t[0], aciertosMin: Int(try FormatoNET.convertToInt32(t[1])),
                            aciertosMax: Int(try FormatoNET.convertToInt32(t[2]))))
                case "aciertos": c.aciertos = try enteros(p[1])
                case "aciertosMax": c.aciertosMax = Int(try FormatoNET.convertToInt32(p[1]))
                case "pasoFijo": c.pasoFijo = try FormatoNET.convertToBoolean(p[1])
                default: Issue.record("setter desconocido \(p[0])")
                }
            }
            let f = FiltroFormatos123(c, analisisActivo: false, numeroPartidos: n)
            var ultima: Columna = 0
            return Prueba(filtro: Espia(f) { ultima = $0 }, contadores: { [Double((try? f.traducir(ultima)) ?? -1)] }, get: [:])
        case "FiltroSimetrias":
            var c = CondicionSimetrias()
            c.activa = true
            for p in config {
                switch p[0] {
                case "simetria": c.simetrias.append(try Simetria(p[1]))
                case "aciertos": c.aciertos = try enteros(p[1])
                default: Issue.record("setter desconocido \(p[0])")
                }
            }
            let f = FiltroSimetrias(c, analisisActivo: false)
            return Prueba(filtro: f, contadores: { [Double(f.aciertos)] }, get: [:])
        case "FiltroDibujos":
            var c = CondicionDibujos()
            c.activa = true
            for p in config {
                switch p[0] {
                case "dibujos": try c.ponerDibujos(p[1].split(separator: ",").map(String.init))
                case "añadir": c.dibujos.append(p[1])
                default: Issue.record("setter desconocido \(p[0])")
                }
            }
            let f = FiltroDibujos(c, analisisActivo: false)
            return Prueba(filtro: f, contadores: { [f.noEquis, f.noDoses].map(Double.init) }, get: ["dibujos": c.getDibujos])
        case "FiltroFormatosSignos":
            var grupos: [FormatosSignos] = []
            for p in config {
                switch p[0] {
                case "grupo": grupos.append(FormatosSignos())
                case "linea":
                    let t = p[1].split(separator: "|").map(String.init)
                    var l = FormatoSignos()
                    l.ponerFormato(t[0])
                    try l.ponerRangoAparicion(t[1])
                    grupos[grupos.count - 1].lineasFormatos.append(l)
                case "lineas": try grupos[grupos.count - 1].ponerLineas(p[1])
                case "global": try grupos[grupos.count - 1].ponerGlobal(p[1])
                default: Issue.record("setter desconocido \(p[0])")
                }
            }
            var c = CondicionFormatosSignos()
            c.activa = true
            c.ponerFormatosSignos(grupos)
            let f = FiltroFormatosSignos(c, analisisActivo: false)
            return Prueba(filtro: f, contadores: { f.aparicionesGrupo.map(Double.init) }, get: [:])
        default:
            throw ErrorDominio(.argumento, "componente \(componente)")
        }
    }

    @Test(arguments: suites)
    func filtro(_ suite: String) throws {
        for caso in try Golden.casos(suite, Entrada.self, Salida.self) {
            let p = try Self.prueba(caso.componente, caso.entrada.config)
            let s = try #require(caso.salida)
            var validas: [Bool] = []
            var contadores: [[Double]] = []
            for texto in caso.entrada.columnas {
                validas.append(try p.filtro.analizar(Self.columna(texto)))
                contadores.append(p.contadores())
            }
            if let v = s.validas { #expect(validas == v, "\(caso.id)") }
            if let c = s.contadores { #expect(contadores == c, "\(caso.id)") }
            if let d = s.dobles {
                #expect(contadores.map { $0.map(\.bitPattern) } == d.map { $0.compactMap { $0.valor?.bitPattern } }, "\(caso.id)")
            }
            for (letras, n) in s.tolerancias ?? [:] {
                #expect(try p.filtro.obtenNoAciertosTolerancias(letras) == n, "\(caso.id) \(letras)")
            }
            if let fallos = s.fallos, let ultima = caso.entrada.columnas.last {
                #expect(try p.filtro.analizarFallos(Self.columna(ultima)) == fallos, "\(caso.id)")
            }
            for (k, v) in s.get ?? [:] { #expect(p.get[k] == v, "\(caso.id) get \(k)") }
        }
    }

    /// Envuelve un filtro para recordar la última columna analizada (contadores derivados de ella).
    final class Espia: FiltroColumna {
        let f: any FiltroColumna
        let ver: (Columna) -> Void
        init(_ f: any FiltroColumna, _ ver: @escaping (Columna) -> Void) {
            self.f = f
            self.ver = ver
        }
        var tipo: TipoFiltro { f.tipo }
        var activa: Bool { f.activa }
        var analisisActivo: Bool { f.analisisActivo }
        func analizar(_ c: Columna) throws(ErrorDominio) -> Bool {
            ver(c)
            return try f.analizar(c)
        }
        func analizarFallos(_ c: Columna) throws(ErrorDominio) -> [String]? { try f.analizarFallos(c) }
        func obtenNoAciertosTolerancias(_ l: String) throws(ErrorDominio) -> Int { try f.obtenNoAciertosTolerancias(l) }
    }

    @Test func parserDeValoresConTolerancia() throws {
        var v = ValoresConTolerancia(tamano: 15)
        try v.poner("6,7A,8", "")
        try v.poner("7B", "")
        #expect(v.texto == "6,7B,8", "acumula y una letra repetida pisa la anterior")
        for (texto, error) in [("", ExcepcionNET.indiceFueraDeRango), ("x", .formato), ("15", .indiceFueraDeRango), ("A", .formato)] {
            var w = ValoresConTolerancia(tamano: 15)
            let e = #expect(throws: ErrorDominio.self) { try w.poner(texto, "") }
            #expect(e?.excepcion == error, "\(texto)")
        }
    }
}
