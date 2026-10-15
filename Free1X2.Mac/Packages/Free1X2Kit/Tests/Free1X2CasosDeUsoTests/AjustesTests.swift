import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// ESPEC_13 §4.2 (pantallas 16, 24, 25).
@Suite("Casos de Ajustes")
struct AjustesTests {
    let entorno = Entorno.pruebas()

    func sembrar() throws {
        try entorno.archivos.escribir(
            try Golden.bytes("Fixtures/datos/parametros.free1x2"),
            en: entorno.rutas.base.appending(path: "parametros.free1x2"), atomico: true)
    }

    @Test func puntosCP() throws {
        #expect(CasosAjustes.leerPuntosCP(entorno: entorno) == PuntosCP(fijos: 0, dobles: 0, triples: 0))
        #expect(throws: ErrorDominio.self) { try CasosAjustes.guardarPuntosCP(1, 2, 3, entorno: entorno) }
        try sembrar()
        #expect(CasosAjustes.leerPuntosCP(entorno: entorno) == PuntosCP(fijos: 6, dobles: 3, triples: 1))
        try CasosAjustes.guardarPuntosCP(7.9, 2, 1, entorno: entorno)
        #expect(CasosAjustes.leerPuntosCP(entorno: entorno) == PuntosCP(fijos: 7, dobles: 2, triples: 1))
    }

    @Test func analisis() throws {
        #expect(!CasosAjustes.leerOpcionesAnalisis(entorno: entorno).todo)
        try sembrar()
        var a = CasosAjustes.leerOpcionesAnalisis(entorno: entorno)
        #expect(a.todo && a.vx2 && a.controlConjuntos)
        a.vx2 = false
        try CasosAjustes.guardarOpcionesAnalisis(a, entorno: entorno)
        #expect(CasosAjustes.leerOpcionesAnalisis(entorno: entorno) == a)
    }

    @Test func configuracion() throws {
        #expect(CasosAjustes.leerConfiguracionEditable(entorno: entorno) == ConfiguracionEditable())
        try sembrar()
        var e = CasosAjustes.leerConfiguracionEditable(entorno: entorno)
        #expect(e.jb == [15, 22, 29, 36, 48, 61] && e.porcentajeAl14 == 12 && e.recaudacion == 14_000_000)
        #expect(e.precioApuesta == Double(Float(0.5)) && e.desplazamiento == 3 && e.separador == "5,9,12")
        e.jb[2] = 22
        #expect {
            _ = try CasosAjustes.guardarConfiguracion(e, entorno: entorno)
        } throws: { ($0 as? ErrorDominio)?.mensajeUsuario == "El valor del campo 2 no puede ser mayor o igual que el siguiente." }
        e.jb[2] = 30
        e.precioApuesta = 0.75
        e.moneda = "$"
        let aviso = try CasosAjustes.guardarConfiguracion(e, entorno: entorno)
        #expect(aviso.hasPrefix("Configuración guardada."))
        let r = CasosAjustes.leerConfiguracionEditable(entorno: entorno)
        #expect(r.jb[2] == 30 && r.precioApuesta == 0.75 && r.moneda == "$")
    }
}
