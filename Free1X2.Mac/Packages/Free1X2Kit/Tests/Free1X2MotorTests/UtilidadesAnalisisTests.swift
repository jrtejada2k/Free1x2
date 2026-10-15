import Foundation
import Free1X2Base
import Free1X2Motor
import Free1X2Pruebas
import Testing

/// ESPEC_06 §2.7-§2.13 y §3.1-§3.2, §3.9: utilidades y análisis sin ficheros, por lectura.
@Suite("Utilidades de análisis")
struct UtilidadesAnalisisTests {
    struct EntradaProb: Decodable, Sendable {
        var madre, hija: [String]
        var minimo: Int
    }
    struct SalidaProb: Decodable, Sendable {
        var probabilidades: [Double]?
        var errorSignos, nan: Bool?
        var noColumnasComb: Int?
    }

    @Test func probabilidadDePremios() throws {
        for caso in try Golden.casos("motor/utils/probabilidades", EntradaProb.self, SalidaProb.self) {
            let e = caso.entrada
            let r = try ProbabilidadPremios.comparar(madre: e.madre, hija: e.hija, premioMinimo: e.minimo)
            let s = try #require(caso.salida)
            if let v = s.probabilidades { #expect(r.probabilidades == v, "\(caso.id)") }
            if let v = s.errorSignos { #expect(r.errorSignos == v, "\(caso.id)") }
            let todosNaN = r.probabilidades.allSatisfy { $0.isNaN }
            if s.nan == true { #expect(todosNaN, "\(caso.id)") }
            if let v = s.noColumnasComb { #expect(r.noColumnasComb == v, "\(caso.id)") }
        }
    }

    /// 06 §2.8: `Math.Round(premio + 0.005, 2)`; 0 acertantes ⇒ ∞; jornada `"7"` ⇒ `"07"`.
    @Test func premiosLAE() throws {
        var config = ConfigLAE()
        config.precioApuesta = 0.5
        config.porcentaje14 = 15
        var lae = LAE(temporada: "2008", numJornada: "7", columnaPremiada: "1X2", recaudacion: 1_000_000, configuracion: config)
        #expect(lae.jornada == "07")
        try lae.calculaPremios(acertantes: [1, 10, 100, 1000, 10000])
        #expect(lae.premios == [300_000, 20000, 2000, 200, 20])
        config.porcentaje14 = 12
        lae = LAE(temporada: "2008", numJornada: "12", columnaPremiada: "1X2", recaudacion: 2_500_000, configuracion: config)
        try lae.calculaPremios(acertantes: [3, 57, 1500, 14000, 120_000])
        #expect(lae.premios == [200_000, 8771.93, 333.34, 35.72, 4.17])
        try lae.calculaPremios(acertantes: [3, 57, 0, 14000, 120_000])
        #expect(lae.premios[2] == .infinity)
        #expect(
            LAE(temporada: "", numJornada: "", columnaPremiada: "", recaudacion: 0, configuracion: config).recaudacion
                == config.recaudacion, "recaudación ≤ 0 ⇒ la de la configuración")
    }

    @Test func grabarJornadaSustituyeOAnade() throws {
        var config = ConfigLAE()
        config.precioApuesta = 0.5
        config.porcentaje14 = 15
        var lae = LAE(temporada: "2008", numJornada: "7", columnaPremiada: "1", recaudacion: 1_000_000, configuracion: config)
        let previas = ["X\t2008\t06\t1", "2\t2008\t07\t9", "1\tOTRA", "SOLO"]
        let e = #expect(throws: ErrorDominio.self) {
            _ = try lae.grabarJornada(acertantes: [1, 1, 1, 1, 1], lineas: previas, cultura: .esES)
        }
        #expect(e?.excepcion == .indiceFueraDeRango, "«1\tOTRA» no llega a mirar v[2]; «SOLO» no tiene v[1] (05 §2.8 P6)")
        let r = try lae.grabarJornada(acertantes: [1, 10, 100, 1000, 10000], lineas: Array(previas.prefix(2)), cultura: .esES)
        #expect(r == ["X\t2008\t06\t1", "1\t2008\t07\t2000000\t300000\t20000\t2000\t200\t20"])
        let nueva = try lae.grabarJornada(acertantes: [1, 10, 100, 1000, 10000], lineas: [], cultura: .esES)
        #expect(nueva == ["1\t2008\t07\t2000000\t300000\t20000\t2000\t200\t20"])
    }

    /// 06 §2.7 A): la suma pasa por `Float`; fila `[0,0,0]` ⇒ NaN; un cero ⇒ −∞ en neperianos.
    @Test func porcentajesBase100YNeperianos() throws {
        var v = [[Double]](repeating: [1, 1, 1], count: 14)
        v[0] = [2, 3, 5]
        v[1] = [0, 0, 0]
        v[2] = [0.5, 0.25, 0.25]
        v[3] = [0, 1, 1]
        let p = Porcentajes(valores: v)
        let b = try p.valoresBase100(numeroPartidos: 14)
        #expect(b[0] == [Float(0.2), Float(0.3), Float(0.5)])
        let filaNaN = b[1].allSatisfy { $0.isNaN }
        #expect(filaNaN)
        #expect(b[2] == [0.5, 0.25, 0.25])
        let n = try p.valoresNeperianos(numeroPartidos: 14)
        #expect(n[3][0] == -.infinity && n[3][1] == Float(log(Double(Float(0.5)))))
        #expect(throws: ErrorDominio.self) { try Porcentajes(valores: [[1, 1, 1]]).valoresBase100(numeroPartidos: 14) }
        #expect(Porcentajes(valores: [[1.5, 2, 3], [4, 5, 6]]).paraPortapapeles(cultura: .esES) == "1,5\t2\t3\r\n4\t5\t6")
    }

    @Test func pralTablaYABDON() throws {
        #expect(Pral.normaliza("1,x,2 *") == "1X2")
        #expect(Pral.normaliza("abc") == "")
        #expect(Pral.normaliza(" 1X2\t") == "1X2")
        var t = TablaDeSeleccion(concepto: "x", minimo: 1, maximo: 5)
        #expect(!t.checked)
        t.minimo = 2
        #expect(t.checked)
        let abdon = InformeColumnasABDON(serie: [3, 4, 5, 5, 2, 7, 7, 3, 3, 3, 0, 1, 2, 3])
        #expect(abdon.minimoAciertos == 0 && abdon.maximoAciertos == 7 && abdon.sumaTotalDeAciertos == 48)
        #expect(abdon.numeroDeEscaleras == 2 && abdon.numeroDeEscalerasAscendentes == 2)
        #expect(InformeColumnasABDON(serie: []).minimoAciertos == 0)
    }
}
