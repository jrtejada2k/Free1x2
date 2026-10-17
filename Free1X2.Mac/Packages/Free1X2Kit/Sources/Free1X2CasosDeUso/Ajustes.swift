import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/CambioPuntosFrmViewModel.cs, ConfiguracionAnalisisFrmViewModel.cs y ConfiguracionFrmViewModel.cs
// (08 pantallas 16, 24, 25; ESPEC_13 §4.2). Guardar escribe `parametros.free1x2` y no recarga la configuración (DP-04).

/// Valores de la pantalla «Configuración» (`ConfiguracionFrmViewModel.cs:25-90`), con sus defectos antes de cargar.
public struct ConfiguracionEditable: Sendable, Hashable {
    public var numeroPartidos = 14.0
    public var separador = "5,9,12"
    public var desplazamiento = 3.0
    public var fijos = 0.0
    public var dobles = 0.0
    public var triples = 0.0
    public var jb = [15.0, 22, 29, 36, 48, 61]
    public var recaudacion = 15_000_000.0
    public var precioApuesta = 0.75
    public var porcentajeAl14 = 15.0
    public var moneda = "€"
    public var actualizarAlInicio = false
    public var confirmarAlSalir = false
    /// `Idiomas`: `es-ES` y, si falta, el leído.
    public var idiomas = ["es-ES"]
    public var idioma: String? = "es-ES"

    public init() {}
}

public enum CasosAjustes {
    static func archivo(_ entorno: Entorno) throws(ErrorDominio) -> ArchivoParametros {
        try ArchivoParametros(try entorno.archivos.leer(ruta(entorno)))
    }

    static func ruta(_ entorno: Entorno) -> URL { entorno.rutas.base.appending(path: CasosInicio.ficheroParametros) }

    /// Aplica cambios en orden y escribe el fichero; si uno falla, lo ya cambiado se queda escrito (cada `Guardar*` del
    /// C# reescribe el XML completo).
    static func guardar(
        _ entorno: Entorno, _ prefijo: String, _ cambios: (inout ArchivoParametros) throws(ErrorDominio) -> Void
    ) throws(ErrorDominio) {
        try CasosInicio.conPrefijo(prefijo) { () throws(ErrorDominio) in
            var p = try archivo(entorno)
            do throws(ErrorDominio) {
                try cambios(&p)
            } catch {
                try? entorno.archivos.escribir(p.datos, en: ruta(entorno), atomico: true)
                throw error
            }
            try entorno.archivos.escribir(p.datos, en: ruta(entorno), atomico: true)
        }
    }

    // MARK: Pantalla 16 · Cambiar Puntuación

    /// CU-A36 (`CambioPuntosFrmViewModel.cs:35-49`): cualquier fallo ⇒ los tres a 0.
    public static func leerPuntosCP(entorno: Entorno) -> PuntosCP {
        (try? archivo(entorno).puntosCP()) ?? PuntosCP(fijos: 0, dobles: 0, triples: 0)
    }

    /// CU-A37 (:51-70): `(int)` trunca.
    public static func guardarPuntosCP(_ fijos: Double, _ dobles: Double, _ triples: Double, entorno: Entorno) throws(ErrorDominio) {
        try guardar(entorno, "No se pudo guardar la puntuación: ") { (p: inout ArchivoParametros) throws(ErrorDominio) in
            try p.guardarPuntosCP(
                PuntosCP(
                    fijos: Int(FormatoNET.castInt32(fijos)), dobles: Int(FormatoNET.castInt32(dobles)),
                    triples: Int(FormatoNET.castInt32(triples))))
        }
    }

    // MARK: Pantalla 24 · Configurar Análisis

    /// CU-A62 (`ConfiguracionAnalisisFrmViewModel.cs:174-222`): cualquier fallo ⇒ log y las 20 a `false` (también `aTodo`).
    public static func leerOpcionesAnalisis(entorno: Entorno) -> OpcionesAnalisis {
        var leidas = OpcionesAnalisis()
        leidas.todo = false
        let vacias = leidas
        do throws(ErrorDominio) {
            try archivo(entorno).analisis(&leidas)
            return leidas
        } catch {
            entorno.registro.registrar("ConfiguracionAnalisisFrmViewModel.CargarDesdeConfiguracion: \(error.mensajeUsuario)", nivel: .error)
            return vacias
        }
    }

    /// CU-A63 (:231-248).
    public static func guardarOpcionesAnalisis(_ a: OpcionesAnalisis, entorno: Entorno) throws(ErrorDominio) {
        try guardar(entorno, "No se pudo guardar la configuración de análisis: ") { (p: inout ArchivoParametros) throws(ErrorDominio) in
            try p.guardarAnalisis(a)
        }
    }

    // MARK: Pantalla 25 · Configuración

    /// CU-A64 (`ConfiguracionFrmViewModel.cs:98-158`): asigna en orden; el primer fallo corta (log) y el resto queda con los
    /// defectos.
    public static func leerConfiguracionEditable(entorno: Entorno) -> ConfiguracionEditable {
        var e = ConfiguracionEditable()
        do throws(ErrorDominio) {
            let p = try archivo(entorno)
            let jb = try p.separadorJB().split(separator: ",", omittingEmptySubsequences: false).map(String.init)
            if jb.count >= 6 { e.jb = jb.prefix(6).map(parseInv) }
            let lae = try p.lae()
            (e.precioApuesta, e.porcentajeAl14, e.recaudacion, e.moneda) = (
                lae.precioApuesta, lae.porcentaje14, lae.recaudacion, lae.moneda
            )
            let cp = try p.puntosCP()
            (e.fijos, e.dobles, e.triples) = (Double(cp.fijos), Double(cp.dobles), Double(cp.triples))
            e.desplazamiento = Double(p.desplazamiento(previo: 3))
            let n = try p.partidos().numero
            let sep = try p.separadorTexto()
            (e.numeroPartidos, e.separador) = (Double(n), sep)
            e.actualizarAlInicio = try p.actualizarAlInicio()
            e.confirmarAlSalir = try p.confirmacionSalir()
            let idioma = try p.idioma()
            if !idioma.isEmpty {
                if !e.idiomas.contains(idioma) { e.idiomas.append(idioma) }
                e.idioma = idioma
            }
        } catch {
            entorno.registro.registrar("ConfiguracionFrmViewModel.CargarDesdeConfiguracion: \(error.mensajeUsuario)", nivel: .error)
        }
        return e
    }

    /// `ParseInv` (:160-163): invariante, si no `es-ES`, si no 0.
    static func parseInv(_ s: String) -> Double {
        let t = FormatoNET.recortar(s)
        return FormatoNET.tryParseDouble(t, cultura: .invariante) ?? FormatoNET.tryParseDouble(t, cultura: .esES) ?? 0
    }

    /// CU-A66 (:170-179): los 6 límites estrictamente crecientes.
    public static func validarSeparadorJB(_ jb: [Double]) throws(ErrorDominio) {
        for i in jb.indices.dropFirst() where jb[i - 1] >= jb[i] {
            throw ErrorDominio(.general, "El valor del campo \(i) no puede ser mayor o igual que el siguiente.")
        }
    }

    /// CU-A65 (:166-208): valida, guarda en el orden del C# y devuelve el aviso de éxito.
    public static func guardarConfiguracion(_ e: ConfiguracionEditable, entorno: Entorno) throws(ErrorDominio) -> String {
        try validarSeparadorJB(e.jb)
        try guardar(entorno, "No se pudo guardar la configuración: ") { (p: inout ArchivoParametros) throws(ErrorDominio) in
            try p.guardarSeparadorJB(e.jb.map { String(FormatoNET.castInt32($0)) })
            var lae = ConfigLAE()
            (lae.precioApuesta, lae.porcentaje14, lae.recaudacion, lae.moneda) = (
                e.precioApuesta, e.porcentajeAl14, e.recaudacion, e.moneda
            )
            try p.guardarLAE(lae)
            try p.guardarPuntosCP(
                PuntosCP(
                    fijos: Int(FormatoNET.castInt32(e.fijos)), dobles: Int(FormatoNET.castInt32(e.dobles)),
                    triples: Int(FormatoNET.castInt32(e.triples))))
            try p.guardarBoleto(numeroPartidos: Int(FormatoNET.castInt32(e.numeroPartidos)), separador: e.separador)
            p.guardarDesplazamiento(String(FormatoNET.castInt32(e.desplazamiento)))
            try p.guardarActualizarAlInicio(e.actualizarAlInicio)
            if let idioma = e.idioma, !idioma.isEmpty { p.guardarIdioma(idioma) }
            try p.guardarConfirmacionSalir(e.confirmarAlSalir)
        }
        return "Configuración guardada. Algunos cambios pueden requerir reiniciar la aplicación."
    }
}
