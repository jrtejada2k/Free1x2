import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor
import Free1X2Online

// C#: Free1X2.WinUI/App.xaml.cs:57-120, Views/MainPageViewModel.cs:202-788 y Controls/BoletoBaseViewModel.cs
// (10 §1.1-1.2, §1.9-1.10; ESPEC_13 §4.1).

/// CU-S01: lo que deja el arranque.
public struct EstadoInicial: Sendable {
    public var configuracion: Configuracion
    public var jornada: JornadaQuiniela?
}

/// `EstadoSemaforo` (`SemaforoControl`, 10 §1.9).
public enum Semaforo: Sendable, Hashable {
    case neutro, verde, rojo
}

/// CU-S09: semáforos de la rejilla de Inicio.
public struct EstadoCondiciones: Sendable, Hashable {
    public var condiciones: [TipoFiltro: Semaforo]
    public var controlGrupos: Semaforo
    public var ifThen: Semaforo
}

public enum CasosInicio {
    /// Semillas editables: del bundle (`Datos/`) a la carpeta base, solo si faltan (ESPEC_11 §9; `aidomnou.cfg` no se porta, DP-08).
    public static let semillas = [
        "parametros.free1x2", "ColumnasProbables.xml", "Equipos/equipos1.dat", "Equipos/equipos2.dat",
        "Equipos/equipos2b.dat", "Equipos/equiposInt.dat", "Impresion/imprebol.cfg",
    ]
    public static let ficheroParametros = "parametros.free1x2"

    /// CU-S01: carpetas de trabajo, semillas, `Configuracion` (todo o nada) y jornada de la caché, **sin red**. Nunca
    /// lanza: los fallos solo se registran (`App.xaml.cs:92,118`).
    public static func arrancar(entorno: Entorno) -> EstadoInicial {
        let (a, r) = (entorno.archivos, entorno.rutas)
        for c in RutasApp.Carpeta.allCases {
            do throws(ErrorDominio) {
                try a.crearCarpeta(r.carpeta(c))
            } catch {
                entorno.registro.registrar("App.AsegurarCarpetasDeTrabajo: \(error.mensajeUsuario)", nivel: .error)
            }
        }
        for s in semillas where !a.existe(r.base.appending(path: s)) {
            let destino = r.base.appending(path: s)
            do throws(ErrorDominio) {
                let datos = try a.leer(r.recursos.appending(path: "Datos/" + s))
                try a.crearCarpeta(destino.deletingLastPathComponent())
                try a.escribir(datos, en: destino, atomico: true)
            } catch {
                entorno.registro.registrar("Semilla \(s): \(error.mensajeUsuario)", nivel: .error)
            }
        }
        let configuracion = ArchivoParametros.cargarConfiguracion(try? a.leer(r.base.appending(path: ficheroParametros)))
        let cache = CacheJornada(archivos: a, carpeta: r.base, registro: entorno.registro)
        return EstadoInicial(configuracion: configuracion, jornada: cache.paisMasReciente().flatMap { cache.cargar($0)?.jornada })
    }

    /// CU-S02 (:568-572): `new Analizador()` + 14 triples volcados + equipos «? - ?».
    public static func nuevaCombinacion(numeroPartidos n: Int) -> CombinacionLeida {
        var c = Combinacion(numeroPartidos: n)
        c.ponerPronosticos(Array(repeating: "1,X,2", count: n))
        return CombinacionLeida(
            combinacion: c, equipos: Array(repeating: ArchivoEquiposBoleto.sinEquipos, count: n), filtroColumnas: "")
    }

    /// CU-S03 (:577-614).
    public static func abrirCombinacion(_ url: URL, numeroPartidos: Int, entorno: Entorno) throws(ErrorDominio) -> CombinacionLeida {
        try conPrefijo("No se pudo abrir la combinación: ") { () throws(ErrorDominio) in
            try ArchivoCombinacion.leer(try entorno.archivos.leer(url), numeroPartidos: numeroPartidos, cultura: entorno.cultura)
        }
    }

    /// CU-S04 (:619-652): la combinación ya lleva volcados los pronósticos del boleto.
    public static func guardarCombinacion(
        _ c: Combinacion, equipos: [String], filtroColumnas: String, en url: URL, entorno: Entorno
    ) throws(ErrorDominio) {
        try conPrefijo("No se pudo guardar la combinación: ") { () throws(ErrorDominio) in
            let datos = try ArchivoCombinacion.datos(c, equipos: equipos, filtroColumnas: filtroColumnas, cultura: entorno.cultura)
            try entorno.archivos.escribir(datos, en: url, atomico: true)
        }
    }

    /// CU-S05 (:708-732): las `N` primeras líneas; faltan ⇒ «? - ?».
    public static func abrirEquipos(_ url: URL, numeroPartidos: Int, entorno: Entorno) throws(ErrorDominio) -> [String] {
        try conPrefijo("No se pudieron abrir los equipos: ") { () throws(ErrorDominio) in
            ArchivoEquiposBoleto.leer(try entorno.archivos.leer(url), numeroPartidos: numeroPartidos)
        }
    }

    /// CU-S06 (:681-699): UTF-8 con BOM y CRLF.
    public static func guardarEquipos(_ equipos: [String], en url: URL, entorno: Entorno) throws(ErrorDominio) {
        try conPrefijo("No se pudieron guardar los equipos: ") { () throws(ErrorDominio) in
            try entorno.archivos.escribir(ArchivoEquiposBoleto.datos(equipos), en: url, atomico: true)
        }
    }

    /// CU-S07 (:744-761): `Temp/` ficheros cuyo nombre contiene `_tmp.comb` (ordinal); el primer fallo corta.
    public static func borrarTemporales(entorno: Entorno) throws(ErrorDominio) -> Int {
        try borrar(en: .temp, "No se pudieron borrar los temporales: ", entorno) { $0.contains("_tmp.comb") }
    }

    /// CU-S08 (:770-788): `Informes/` ficheros de nombre `Informe…` de más de 7 caracteres.
    public static func borrarInformes(entorno: Entorno) throws(ErrorDominio) -> Int {
        try borrar(en: .informes, "No se pudieron borrar los informes: ", entorno) {
            $0.utf16.count > 7 && $0.hasPrefix("Informe")
        }
    }

    /// CU-S09 (`CondicionItem.cs:61-80`; `MainPageViewModel.cs:228-241`).
    public static func estadoDeCondiciones(_ c: Combinacion, grupo: Int) -> EstadoCondiciones {
        let g = c.grupos.indices.contains(grupo) ? c.grupos[grupo].condiciones : c.grupos.first?.condiciones
        var condiciones: [TipoFiltro: Semaforo] = [:]
        for t in TipoFiltro.allCases {
            let e = g?.estado(t) ?? (activa: false, contieneDatos: false)
            condiciones[t] = e.activa ? .verde : e.contieneDatos ? .rojo : .neutro
        }
        let ifThen: Semaforo = c.ifThen.map { $0.esVacio ? .neutro : $0.activo ? .verde : .rojo } ?? .neutro
        return EstadoCondiciones(
            condiciones: condiciones, controlGrupos: c.controlesGrupos.count > 1 ? .verde : .neutro, ifThen: ifThen)
    }

    /// CU-S10: visibilidad de los 6 grupos de la barra en el orden del fichero (`tsFree, tsFiltros, tsCombinacion,
    /// tsOperaciones, tsArchivo, tsUtilidades`); Filtros siempre visible al arrancar (`MainWindow.xaml.cs:445`).
    public static func leerBarrasVisibles(_ configuracion: Configuracion) -> [Bool] {
        var b = configuracion.interfaz.barras
        if b.count > 1 { b[1] = true }
        return b
    }

    /// CU-S10 (`MainWindow.xaml.cs:717-729`): al cerrar la ventana; el que llama solo registra el fallo.
    public static func guardarBarrasVisibles(_ barras: [Bool], entorno: Entorno) throws(ErrorDominio) {
        let url = entorno.rutas.base.appending(path: ficheroParametros)
        var p = try ArchivoParametros(try entorno.archivos.leer(url))
        try p.guardarBarras(barras)
        try entorno.archivos.escribir(p.datos, en: url, atomico: true)
    }

    // MARK: Grupos y filtros de Inicio

    public static let mensajeFiltroParcialBase =
        "El Filtro Parcial se aplica a un grupo. Selecciona o crea un grupo distinto del boleto base."

    /// `CambiarGrupo` (:478-490): por debajo de 0 ⇒ 0; más allá del último añade un grupo vacío (`AddGrupo`).
    public static func cambiarGrupo(_ c: inout Combinacion, a destino: Int) throws(ErrorDominio) -> Int {
        let d = max(destino, 0)
        if d > c.grupos.count - 1 { try c.addGrupo(GrupoConfig(numeroPartidos: c.numeroPartidos)) }
        return d
    }

    /// `EliminarGrupo` (:423-472): borra el grupo, renumera los controles de grupos y va al anterior. `nil` = no hace nada
    /// (boleto base o un solo grupo).
    public static func eliminarGrupo(_ c: inout Combinacion, _ g: Int) throws(ErrorDominio) -> Int? {
        guard g != 0, c.grupos.count > 1 else { return nil }
        try c.borrarGrupo(g)
        for k in c.controlesGrupos.indices {
            let partes = c.controlesGrupos[k].textoGruposControlados.split(
                omittingEmptySubsequences: false, whereSeparator: { $0 == "," || $0 == "-" })
            let n = c.controlesGrupos[k].gruposControlados
            var temp = ""
            for j in partes.indices {
                guard n.indices.contains(j) else { throw .fueraDeRango("MainPageViewModel.cs:446") }
                if n[j] > g { temp += "\(n[j] - 1)," } else if n[j] < g { temp += "\(n[j])," }
            }
            guard temp.utf16.count > 1 else { continue }
            try c.controlesGrupos[k].ponerGruposControlados(String(temp.dropLast()))
        }
        return try cambiarGrupo(&c, a: g - 1)
    }

    /// `FiltroColumnasAsync` (:498-529): más signos que partidos ⇒ error; si no se puede leer se asigna igual.
    public static func validarFiltroColumnas(_ url: URL, numeroPartidos: Int, entorno: Entorno) throws(ErrorDominio) {
        guard let d = try? entorno.archivos.leer(url), ArchivoColumnasTexto.numSignos(d) > numeroPartidos else { return }
        throw ErrorDominio(.general, "El filtro tiene más signos que partidos del boleto.")
    }

    /// `AbrirFiltroParcialAsync` (:363-383).
    public static func validarFiltroParcial(_ url: URL, numeroPartidos: Int, entorno: Entorno) throws(ErrorDominio) {
        guard let d = try? entorno.archivos.leer(url) else {
            throw ErrorDominio(.general, "No se pudo leer el archivo de filtro parcial.")
        }
        guard ArchivoColumnasTexto.numSignos(d) == numeroPartidos else {
            throw ErrorDominio(.general, "El Filtro Parcial para los grupos debe tener \(numeroPartidos) partidos.")
        }
    }

    // MARK: Boleto base (`BoletoBaseViewModel.cs`)

    /// `DividirEquipos` (:88-96): «? - ?» o blanco ⇒ vacíos; corta en el primer « - » y recorta cada parte.
    public static func dividirEquipos(_ e: String) -> (local: String, visitante: String) {
        guard !e.allSatisfy(\.isWhitespace), e != ArchivoEquiposBoleto.sinEquipos else { return ("", "") }
        guard let r = e.range(of: " - ") else { return (FormatoNET.recortar(e), "") }
        return (FormatoNET.recortar(String(e[..<r.lowerBound])), FormatoNET.recortar(String(e[r.upperBound...])))
    }

    /// `FijarEquipos` (:100-112): lo que se escribe sin recortar.
    public static func componerEquipos(local: String, visitante: String) -> String {
        FormatoNET.recortar(local).isEmpty && FormatoNET.recortar(visitante).isEmpty
            ? ArchivoEquiposBoleto.sinEquipos : local + " - " + visitante
    }

    /// `EquiposNormalizados` (:120-124): lo que se guarda.
    public static func normalizarEquipos(local: String, visitante: String) -> String {
        let (l, v) = dividirEquipos(componerEquipos(local: local, visitante: visitante))
        return l.isEmpty && v.isEmpty ? ArchivoEquiposBoleto.sinEquipos : l + " - " + v
    }

    /// `SetEquipos` (:291-305): solo los que llegan; vacío ⇒ «? - ?»; los demás no cambian.
    public static func aplicarEquipos(_ nuevos: [String], a previos: [String]) -> [String] {
        var r = previos
        for i in r.indices where i < nuevos.count { r[i] = nuevos[i].isEmpty ? ArchivoEquiposBoleto.sinEquipos : nuevos[i] }
        return r
    }

    /// `CargarDesdeMotor` (:233-258): signos 1/X/2 marcados de un pronóstico (`nil` o vacío ⇒ triple).
    public static func signos(_ pronostico: String?) -> [Bool] {
        let p = pronostico.flatMap { $0.isEmpty ? nil : $0 } ?? "1,X,2"
        return ["1", "X", "2"].map { p.contains($0) }
    }

    /// `Pronostico` (:142-152): los marcados en orden 1, X, 2 separados por coma.
    public static func pronostico(_ signos: [Bool]) -> String {
        zip(["1", "X", "2"], signos).filter(\.1).map(\.0).joined(separator: ",")
    }

    // MARK: -

    static func conPrefijo<T>(_ prefijo: String, _ f: () throws(ErrorDominio) -> T) throws(ErrorDominio) -> T {
        do throws(ErrorDominio) {
            return try f()
        } catch {
            throw ErrorDominio(error.excepcion, prefijo + error.mensajeUsuario, origen: error.origen)
        }
    }

    static func borrar(
        en carpeta: RutasApp.Carpeta, _ prefijo: String, _ entorno: Entorno, _ criterio: (String) -> Bool
    ) throws(ErrorDominio) -> Int {
        let dir = entorno.rutas.carpeta(carpeta)
        guard entorno.archivos.existe(dir) else { return 0 }
        return try conPrefijo(prefijo) { () throws(ErrorDominio) in
            var n = 0
            for f in try entorno.archivos.listar(dir) where criterio(f.lastPathComponent) {
                try entorno.archivos.borrar(f)
                n += 1
            }
            return n
        }
    }
}
