import Free1X2Base
import Free1X2Pruebas
import Testing

/// ESPEC_17 COL-01…16: suites `base/codificaciones` (heredadas) y `base/utilcolumnas` (lectura y modelo).
@Suite("Codificaciones de columnas")
struct CodificacionesTests {
    typealias Caso = CasoGolden<Campos, Campos>

    static func casos(_ componente: String) throws -> [Caso] {
        let todos: [Caso] = try Golden.casos("base/codificaciones") + Golden.casos("base/utilcolumnas")
        let casos = todos.filter { $0.componente == componente }
        try #require(!casos.isEmpty, "sin casos de \(componente)")
        return casos
    }

    @Test func convStrToLong() throws {
        for caso in try Self.casos("UtilColumnas.convStrToLong") {
            let s = try #require(caso.entrada.columna)
            #expect(UtilColumnas.convStrToLong(s) == caso.salida?.valor?.entero, "\(caso.id)")
        }
        for caso in try Self.casos("UtilColumnas.convStrToLongPronosticos") {
            let p = try #require(caso.entrada.pronosticos)
            #expect(UtilColumnas.convStrToLong(pronosticos: p) == caso.salida?.valor?.entero, "\(caso.id)")
        }
        for caso in try Self.casos("UtilColumnas.convStrToLongRelleno") {
            let e = caso.entrada
            let r = Result { () throws(ErrorDominio) in
                try UtilColumnas.convStrToLong(e.columna ?? "", relleno: e.relleno ?? "", numeroPartidos: e.numeroPartidos ?? 14)
            }
            #expect(verificar(caso, r) { $0 == caso.salida?.valor?.entero } == nil)
        }
        for caso in try Self.casos("UtilColumnas.convStrToLongFaltan") {
            let r = Result { () throws(ErrorDominio) in
                try UtilColumnas.convStrToLong(caso.entrada.columna ?? "", faltan: caso.entrada.faltan ?? 0)
            }
            #expect(verificar(caso, r) { $0 == caso.salida?.valor?.entero } == nil)
        }
        for caso in try Self.casos("UtilColumnas.convStrToLongMarcados") {
            let r = Result { () throws(ErrorDominio) in
                try UtilColumnas.convStrToLong(caso.entrada.columna ?? "", marcados: caso.entrada.marcados ?? [])
            }
            #expect(verificar(caso, r) { $0 == caso.salida?.valor?.entero } == nil)
        }
    }

    @Test func convLongToStrYSignos() throws {
        for caso in try Self.casos("UtilColumnas.convLongToStr") {
            let valor = try #require(caso.entrada.valor?.entero)
            #expect(
                verificar(caso, Result { () throws(ErrorDominio) in try UtilColumnas.convLongToStr(valor) }) { $0 == caso.salida?.columna }
                    == nil)
        }
        for caso in try Self.casos("UtilColumnas.idaYVuelta") {
            let columnas = try caso.entrada.fichero.map(Golden.columnas) ?? [try #require(caso.entrada.columna)]
            for columna in columnas {
                #expect(try UtilColumnas.convLongToStr(UtilColumnas.convStrToLong(columna)) == columna, "\(caso.id) \(columna)")
            }
        }
        for caso in try Self.casos("UtilColumnas.obtenerSignoInt") {
            let valor = try #require(caso.entrada.valor?.entero)
            let partidos = try #require(caso.entrada.partidos?.enteros)
            #expect(partidos.map { UtilColumnas.obtenerSignoInt(valor, partido: $0) } == caso.salida?.signos, "\(caso.id)")
        }
        for caso in try Self.casos("UtilColumnas.ignorarPartidos") {
            let valor = try #require(caso.entrada.valor?.entero)
            let partidos = try #require(caso.entrada.partidos?.booleanos)
            #expect(UtilColumnas.ignorarPartidos(partidos, valor) == caso.salida?.valor?.entero, "\(caso.id)")
        }
        for caso in try Self.casos("UtilColumnas.contarBitsA1") {
            #expect(UtilColumnas.contarBitsA1(try #require(caso.entrada.valor?.entero)) == 64, "\(caso.id)")
        }
    }

    @Test func convertidorDeBases() throws {
        let conversor = ConvertidorDeBases(numeroPartidos: 14)
        for caso in try Self.casos("ConvertidorDeBases.numero") {
            #expect(try conversor.numero(try #require(caso.entrada.columna)) == IndiceBase3(caso.salida?.numero ?? -1), "\(caso.id)")
        }
        for caso in try Self.casos("ConvertidorDeBases.idaYVuelta") {
            if let fichero = caso.entrada.fichero {
                for columna in try Golden.columnas(fichero) {
                    let n = try conversor.numero(columna)
                    #expect(try conversor.numero(conversor.columna(Int64(n))) == n, "\(caso.id) \(columna)")
                    #expect((0...4_782_968).contains(n), "\(caso.id) \(columna)")
                }
            } else {
                let n = try #require(caso.entrada.numero)
                let columna = try conversor.columna(n)
                #expect(try columna.count == 14 && Int64(conversor.numero(columna)) == n, "\(caso.id)")
            }
        }
        for caso in try Self.casos("ConvertidorDeBases.columna") {
            let n = try #require(caso.entrada.numero)
            #expect(verificar(caso, Result { () throws(ErrorDominio) in try conversor.columna(n) }) { $0 == caso.salida?.columna } == nil)
        }
        for caso in try Self.casos("ConvertidorDeBases.tamanoConjunto") {
            #expect(try ConvertidorDeBases.tamanoConjunto(partidos: caso.entrada.partidos?.numero ?? 0) == caso.salida?.tamano)
        }
        for caso in try Self.casos("ConvertidorDeBases.numeroPartidos") {
            let tamanos = try #require(caso.entrada.tamanos)
            #expect(tamanos.map { ConvertidorDeBases.numeroPartidos(tamanoConjunto: $0) } == caso.salida?.partidos?.enteros)
        }
    }

    @Test func aciertos() throws {
        for caso in try Golden.casos("base/codificaciones", componente: "Aciertos", Campos.self, CamposAciertos.self) {
            if let fichero = caso.entrada.columnas, let rutaGanadora = caso.entrada.ganadora {
                let ganadora = try #require(try Golden.columnas(rutaGanadora).first)
                let columnas = try Golden.columnas(fichero)
                for columna in columnas {
                    #expect(Self.aciertos(ganadora, columna) + Self.diferencias(ganadora, columna) == 14, "\(caso.id)")
                }
                #expect(Self.aciertos(ganadora, try #require(columnas.first)) == 14, "\(caso.id)")
                continue
            }
            let a = try #require(caso.entrada.a)
            let b = try #require(caso.entrada.b)
            if let n = caso.salida?.aciertos { #expect(Self.aciertos(a, b) == n, "\(caso.id)") }
            if let n = caso.salida?.aciertosMasFallos { #expect(Self.aciertos(a, b) + Self.diferencias(a, b) == n, "\(caso.id)") }
        }
    }

    @Test func codificacionX0MSB() throws {
        for caso in try Self.casos("CodificacionX0MSB.idaYVuelta") {
            let columna = try #require(caso.entrada.columna)
            let partidos = caso.entrada.partidos?.numero ?? 14
            #expect(CodificacionX0MSB.columna(try CodificacionX0MSB.numero(columna, partidos: partidos), partidos: partidos) == columna)
        }
        for caso in try Self.casos("CodificacionX0MSB.numero") {
            let n = try CodificacionX0MSB.numero(try #require(caso.entrada.columna), partidos: caso.entrada.partidos?.numero ?? 14)
            #expect(Int64(n) == caso.salida?.numero, "\(caso.id)")
        }
        for caso in try Self.casos("CodificacionX0MSB.columna") {
            let n = Int32(try #require(caso.entrada.numero))
            #expect(CodificacionX0MSB.columna(n, partidos: caso.entrada.partidos?.numero ?? 14) == caso.salida?.columna)
        }
        let caso = try Golden.caso("COL-16", en: "base/utilcolumnas", Campos.self, CamposComparacion.self)
        let columna = try #require(caso.entrada.columna)
        #expect(Int(try ConvertidorDeBases(numeroPartidos: 14).numero(columna)) == caso.salida?.convertidorDeBases)
        #expect(Int(try CodificacionX0MSB.numero(columna, partidos: 14)) == caso.salida?.x0msb)
    }

    struct CamposAciertos: Decodable, Sendable {
        var aciertos: Int?
        var aciertosMasFallos: Int?
    }

    struct CamposComparacion: Decodable, Sendable {
        var convertidorDeBases: Int
        var x0msb: Int
    }

    static func aciertos(_ a: String, _ b: String) -> Int {
        UtilColumnas.aciertos(UtilColumnas.convStrToLong(a), UtilColumnas.convStrToLong(b))
    }

    /// Diferencias contadas carácter a carácter, independiente de la codificación de bits.
    static func diferencias(_ a: String, _ b: String) -> Int {
        zip(a.utf16, b.utf16).filter { $0 != $1 }.count
    }
}
