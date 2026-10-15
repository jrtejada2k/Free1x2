import Free1X2Base
import Free1X2Motor
import Free1X2Pruebas
import Testing

/// ESPEC_17 ESC-02…06: suite `motor/escrutinio/escrutador` (GE-01…09 de ESPEC_05 §6).
@Suite("Escrutinio")
struct EscrutinioTests {
    struct Entrada: Decodable, Sendable {
        var fichero: String?
        var columnas: [String]?
        var ganadora: String?
        var ganadoras: String?
        var ganadorasTexto: [String]?
        var categorias: [Int]?
        var globales: [Int]?
        var apuestas: [String]?
        var contenedores: [Contenedor]?
    }
    struct Contenedor: Decodable, Sendable {
        var colGanadora: String
        var cuentas: [Int]  // Col10 … Col16
    }
    struct Salida: Decodable, Sendable {
        var premiosPorAciertos: [String: Int]?
        var totalPuntuadas: Int?
        var premios: [Int]?
        var acTotales: String?
        var premiadas: [ColumnaPremiada]?
        var filas: [FilaEscrutinio]?
        var premiosTotales: [Int]?
        var fila: FilaEscrutinio?
        var aciertos: [Int]?
        var etiquetas: [String: Int]?
        var claves: Int?
        var cubos: Int?
        var primeras: [String]?
        var ultimas: [String]?
        var sha256: String?
        var posicionGanadora: Int?
        var orden: [String]?
    }
    typealias Caso = CasoGolden<Entrada, Salida>

    static func casos(_ componente: String) throws -> [Caso] {
        try Golden.casos("motor/escrutinio/escrutador", componente: componente)
    }

    /// Columnas del caso: en línea o de un fixture.
    static func columnas(_ e: Entrada) throws -> [String] {
        if let columnas = e.columnas { return columnas }
        return try Golden.columnas(try #require(e.fichero))
    }

    /// Escruta las columnas con la ganadora (texto o fichero cuya 1.ª línea es la ganadora).
    static func escrutar(_ caso: Caso, anadir: Bool = false) throws -> (Escrutador, FilaEscrutinio) {
        let e = caso.entrada
        var ganadora = try #require(e.ganadora)
        if ganadora.hasPrefix("Fixtures/") { ganadora = try #require(try Golden.columnas(ganadora).first) }
        var esc = Escrutador(
            premios: e.categorias ?? [], numeroPartidos: 14, archivoColumnas: e.fichero ?? "", anadirAGanadoras: anadir)
        let fila = try esc.escrutarConColumna(ganadora, columnas: try columnas(e), archivo: e.fichero ?? "")
        return (esc, fila)
    }

    @Test func heredadoDelFixture() throws {
        let caso = try Golden.caso("CP-ESC-02", en: "motor/escrutinio/escrutador", Entrada.self, Salida.self)
        let (esc, _) = try Self.escrutar(caso)
        let esperado = try #require(caso.salida?.premiosPorAciertos)
        for (aciertos, n) in esc.premiosTotales.enumerated() { #expect(n == esperado[String(aciertos)] ?? 0, "\(aciertos)") }
        #expect(esc.premiosTotales.reduce(0, +) == caso.salida?.totalPuntuadas)
    }

    @Test func columnaGanadora() throws {
        for caso in try Self.casos("Escrutador.columna") {
            let (esc, fila) = try Self.escrutar(caso)
            #expect(fila.premios == caso.salida?.premios && esc.premiosTotales == caso.salida?.premios, "\(caso.id)")
            #expect(fila.aciertosTotales == caso.salida?.acTotales, "\(caso.id)")
            #expect(fila.lineaID == 1 && fila.archivo == "\(caso.entrada.fichero?.split(separator: "/").last ?? "")", "\(caso.id)")
            #expect(fila.columna == caso.entrada.ganadora || caso.entrada.ganadora?.hasPrefix("Fixtures/") == true)
        }
    }

    @Test func columnasPremiadas() throws {
        for caso in try Self.casos("Escrutador.premiadas") {
            let (esc, _) = try Self.escrutar(caso, anadir: true)
            #expect(esc.listaPremiadas == caso.salida?.premiadas, "\(caso.id)")
        }
    }

    @Test func temporada() throws {
        for caso in try Self.casos("Escrutador.temporada") {
            let e = caso.entrada
            let ganadoras = try e.ganadorasTexto ?? Golden.columnas(try #require(e.ganadoras))
            var esc = Escrutador(premios: e.categorias ?? [], numeroPartidos: 14, archivoColumnas: e.fichero ?? "")
            let filas = try esc.escrutarTemporada(
                ganadoras: ganadoras, columnas: try Self.columnas(e), archivo: "Escrutinios/" + (e.fichero ?? "f.txt"))
            #expect(filas == caso.salida?.filas, "\(caso.id)")
            #expect(esc.premiosTotales == caso.salida?.premiosTotales, "\(caso.id)")
        }
    }

    @Test func filaDeTotales() throws {
        let caso = try Golden.caso("GE-05", en: "motor/escrutinio/escrutador", Entrada.self, Salida.self)
        let esc = Escrutador(premios: [], numeroPartidos: 14)
        #expect(try esc.filaTotales(try #require(caso.entrada.globales)) == caso.salida?.fila)
        #expect(throws: ErrorDominio.self) { try esc.filaTotales([1, 2]) }
    }

    @Test func apuestaMultiple() throws {
        let caso = try Golden.caso("GE-06", en: "motor/escrutinio/escrutador", Entrada.self, Salida.self)
        let texto = try #require(caso.entrada.ganadora)
        let ganadora = try #require(Columna(texto))
        let aciertos = (caso.entrada.apuestas ?? []).compactMap(Columna.init).map {
            Escrutador.escrutarApuestaMultiple($0, ganadora)
        }
        #expect(aciertos == caso.salida?.aciertos)
    }

    @Test func masAciertosQuePartidosEsFueraDeRango() {
        var esc = Escrutador(premios: [], numeroPartidos: 14)
        let e = #expect(throws: ErrorDominio.self) {
            try esc.escrutarConColumna(String(repeating: "1", count: 15), columnas: [String(repeating: "1", count: 15)], archivo: "")
        }
        #expect(e?.excepcion == .indiceFueraDeRango)
    }

    @Test func candidatasDeEscrutadorComb() throws {
        for caso in try Self.casos("EscrutadorComb.posiblesPremios") {
            var esc = EscrutadorComb(premios: caso.entrada.categorias ?? [], numeroPartidos: 14)
            try esc.obtenerPosiblesPremios(try #require(caso.entrada.ganadora))
            let claves = esc.aciertos.claves
            var etiquetas: [String: Int] = [:]
            for clave in claves { etiquetas[String(esc.aciertos[clave]?.valor ?? -1), default: 0] += 1 }
            let s = try #require(caso.salida)
            #expect(etiquetas == s.etiquetas && claves.count == s.claves, "\(caso.id)")
            #expect(esc.premios == (caso.entrada.categorias ?? []).sorted(by: >), "\(caso.id): Premios queda descendente")
            guard let sha256 = s.sha256 else { continue }  // los casos `lectura` no fijan el orden
            #expect(esc.aciertos.numeroDeCubos == s.cubos, "\(caso.id)")
            #expect(Array(claves.prefix(8)).map(\.description) == s.primeras, "\(caso.id)")
            #expect(Array(claves.suffix(4)).map(\.description) == s.ultimas, "\(caso.id)")
            #expect(Golden.sha256(claves.map(\.description).joined(separator: ";")) == sha256, "\(caso.id)")
            let posicion = claves.firstIndex { if case .int64 = $0 { true } else { false } } ?? -1
            #expect(posicion == s.posicionGanadora, "\(caso.id)")
        }
    }

    @Test func comparadorDePosiblesPremios() throws {
        for caso in try Self.casos("PosiblesPremiosComparer") {
            var lista = (caso.entrada.contenedores ?? []).map { c in
                var p = PosiblesPremiosContenedor(colGanadora: c.colGanadora)
                let listas = c.cuentas.map { Array(repeating: "", count: $0) }
                (p.col10, p.col11, p.col12, p.col13, p.col14, p.col15, p.col16) =
                    (listas[0], listas[1], listas[2], listas[3], listas[4], listas[5], listas[6])
                return p
            }
            PosiblesPremiosContenedor.ordenar(&lista)
            #expect(lista.map(\.colGanadora) == caso.salida?.orden, "\(caso.id)")
        }
    }
}
