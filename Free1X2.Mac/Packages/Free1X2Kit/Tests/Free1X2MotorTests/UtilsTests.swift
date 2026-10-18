import Free1X2Base
import Free1X2Motor
import Free1X2Pruebas
import Testing

/// ESPEC_17 RNG, TRA y SUM: utilidades de cálculo del motor.
@Suite("Utilidades del motor")
struct UtilsTests {
    typealias Caso = CasoGolden<Campos, Campos>

    static func casos(_ suite: String, _ componente: String) throws -> [Caso] {
        try Golden.casos(suite, componente: componente)
    }

    @Test func rangosHelper() throws {
        for suite in ["motor/utils/rangos", "motor/utils/entradas"] {
            for caso in try Self.casos(suite, "RangosHelper.obtenIntArray") {
                let t = try #require(caso.entrada.texto)
                #expect(
                    verificar(caso, Result { () throws(ErrorDominio) in try RangosHelper.obtenIntArray(t) }) {
                        $0 == caso.salida?.valores?.enteros
                    } == nil)
            }
            for caso in try Self.casos(suite, "RangosHelper.obtenBoolArray") {
                let t = try #require(caso.entrada.texto)
                #expect(
                    verificar(caso, Result { () throws(ErrorDominio) in try RangosHelper.obtenBoolArray(t) }) { r in
                        if let esperado = caso.salida?.valores?.booleanos { return r == esperado }
                        return r.count == caso.salida?.longitud && r.indices.filter { r[$0] } == caso.salida?.indicesVerdaderos
                    } == nil)
            }
        }
    }

    @Test func entradasDeValores() throws {
        let s = "motor/utils/entradas"
        typealias U = UtilidadesEntradasValores
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerListaFromTxt") {
            #expect(U.obtenerListaFromTxt(caso.entrada.texto ?? "") == caso.salida?.valores?.enteros, "\(caso.id)")
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerListaFromTxtAciertos") {
            #expect(U.obtenerListaFromTxtAciertos(caso.entrada.texto ?? "") == caso.salida?.valores?.enteros, "\(caso.id)")
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerLongFiguraFromText") {
            #expect(try U.obtenerLongFiguraFromText(caso.entrada.texto ?? "") == caso.salida?.valor?.entero, "\(caso.id)")
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerTextoFiguraFromLong") {
            #expect(try U.obtenerTextoFiguraFromLong(caso.entrada.valor?.entero ?? -1) == caso.salida?.texto, "\(caso.id)")
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerBoolArrayFromTxtLongitud") {
            let r = Result { () throws(ErrorDominio) in
                try U.obtenerBoolArrayFromTxt(caso.entrada.texto ?? "", longitud: caso.entrada.longitud ?? 0)
            }
            #expect(verificar(caso, r) { $0 == nil ? caso.salida?.nulo == true : $0 == caso.salida?.valores?.booleanos } == nil)
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerBoolArrayFromTxtPartidos") {
            let r = U.obtenerBoolArrayFromTxt(caso.entrada.texto ?? "", numeroPartidos: caso.entrada.numeroPartidos ?? 14)
            #expect(r == caso.salida?.valores?.booleanos, "\(caso.id)")
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerTodosValores") {
            let r = Result { () throws(ErrorDominio) in try U.obtenerTodosValores(caso.entrada.minimo ?? 0, caso.entrada.maximo ?? 0) }
            #expect(verificar(caso, r) { $0 == caso.salida?.texto } == nil)
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerValoresSeparadosPorComas") {
            #expect(try U.obtenerValoresSeparadosPorComas(caso.entrada.texto ?? "") == caso.salida?.texto, "\(caso.id)")
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.sonTodosNumeros") {
            #expect((caso.entrada.textos ?? []).map(U.sonTodosNumeros) == caso.salida?.resultados, "\(caso.id)")
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerGruposDeValores") {
            let e = caso.entrada
            let grupos = U.obtenerGruposDeValores(e.cantidad ?? 0, pasoFijo: e.pasoFijo ?? false, numeroPartidos: e.numeroPartidos ?? 14)
            #expect(grupos == caso.salida?.grupos, "\(caso.id)")
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerTextoFromBool") {
            #expect(U.obtenerTextoFromBool(caso.entrada.valores?.booleanos ?? []) == caso.salida?.texto, "\(caso.id)")
        }
        for caso in try Self.casos(s, "UtilidadesEntradasValores.obtenerRangos") {
            var lista = caso.entrada.valores?.enteros ?? []
            #expect(U.obtenerRangos(&lista) == caso.salida?.rangos, "\(caso.id)")
            #expect(lista == caso.salida?.ordenada, "\(caso.id): la lista del llamador queda ordenada")
        }
    }

    @Test func tramo() throws {
        struct Entrada: Decodable, Sendable {
            struct Parte: Decodable, Sendable {
                var aciertos: [Int]
                var numColumnasTramo: Int?
                var columnasPremiadas: Int?
                var calcular: Bool?
            }
            var premios: [Double]
            var aciertos: [Int]?
            var numColumnasTramo: Int?
            var numeroDeTramo: Int?
            var valorIzquierda: Int?
            var valorDerecha: Int?
            var probAcumulada: DoubleGolden?
            var columnasPremiadas: Int?
            var origen: Parte?
            var destino: Parte?
        }
        struct Salida: Decodable, Sendable {
            var p14, p13, p12, p11, p10: Int?
            var totalImportePremios, balance, probAcumulada: DoubleGolden?
            var numeroDeTramo, valorIzquierda, valorDerecha, numColumnasTramo, columnasPremiadas: Int?
        }
        for caso in try Golden.casos("motor/utils/tramo", Entrada.self, Salida.self) {
            let e = caso.entrada
            let s = try #require(caso.salida)
            var tramo = Tramo(premios: e.premios)
            if let origen = e.origen, let destino = e.destino {
                var o = Tramo(premios: e.premios)
                o.ponerAciertos(origen.aciertos)
                o.numColumnasTramo = origen.numColumnasTramo ?? 0
                o.columnasPremiadas = origen.columnasPremiadas ?? 0
                if origen.calcular == true { o.calculaTotalImportePremios(precio: 0.5) }
                tramo.ponerAciertos(destino.aciertos)
                tramo.addTramo(o)
            } else {
                tramo.ponerAciertos(e.aciertos ?? [0, 0, 0, 0, 0])
                tramo.numColumnasTramo = e.numColumnasTramo ?? 0
                tramo.numeroDeTramo = e.numeroDeTramo ?? 0
                tramo.valorIzquierda = e.valorIzquierda ?? 0
                tramo.valorDerecha = e.valorDerecha ?? 0
                tramo.probAcumulada = e.probAcumulada?.valor ?? 0
                tramo.columnasPremiadas = e.columnasPremiadas ?? 0
                if caso.componente == "Tramo.calculaTotalImportePremios" { tramo.calculaTotalImportePremios(precio: 0.5) }
            }
            if let p = s.p14 {
                #expect([tramo.p14, tramo.p13, tramo.p12, tramo.p11, tramo.p10] == [p, s.p13, s.p12, s.p11, s.p10].compactMap { $0 })
            }
            if let t = s.totalImportePremios { #expect(tramo.totalImportePremios.bitPattern == t.valor?.bitPattern, "\(caso.id)") }
            if let b = s.balance { #expect(tramo.balance.bitPattern == b.valor?.bitPattern, "\(caso.id)") }
            if let p = s.probAcumulada { #expect(tramo.probAcumulada == p.valor, "\(caso.id)") }
            if let n = s.numeroDeTramo { #expect(tramo.numeroDeTramo == n && tramo.valorIzquierda == s.valorIzquierda) }
            if let n = s.columnasPremiadas { #expect(tramo.columnasPremiadas == n, "\(caso.id)") }
        }
    }

    @Test func sumadorDeCombinaciones() throws {
        struct Entrada: Decodable, Sendable {
            var a: String
            var b: String?
            var operacion: String
        }
        struct Salida: Decodable, Sendable {
            var columnasDistintas, lineas, a, b, union, interseccion, resta: Int?
        }
        for caso in try Golden.casos("motor/utils/sumador", Entrada.self, Salida.self) {
            let a = try Golden.columnas(caso.entrada.a)
            let b = try caso.entrada.b.map(Golden.columnas) ?? []
            let s = try #require(caso.salida)
            if let tipo = AlgebraComb(rawValue: caso.entrada.operacion) {
                let r = try SumadorCombinaciones.calcular(tipo, columnas1: a, columnas2: b, numeroPartidos: 14)
                if let n = s.columnasDistintas { #expect(Set(r.columnas).count == n, "\(caso.id)") }
                if let n = s.lineas { #expect(r.columnas.count == n, "\(caso.id)") }
            } else {
                let union = try SumadorCombinaciones.calcular(.sumaEliminaRepetidas, columnas1: a, columnas2: b, numeroPartidos: 14)
                let inter = try SumadorCombinaciones.calcular(.sumaSoloComunes, columnas1: a, columnas2: b, numeroPartidos: 14)
                let resta = try SumadorCombinaciones.calcular(.restaSegunda, columnas1: a, columnas2: b, numeroPartidos: 14)
                #expect([Set(a).count, Set(b).count] == [s.a, s.b].compactMap { $0 })
                #expect(Set(union.columnas).count == s.union && Set(inter.columnas).count == s.interseccion)
                #expect(Set(resta.columnas).count == s.resta)
            }
        }
    }
}
