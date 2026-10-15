import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/CalculaColumnasFrmViewModel.cs (08 pantalla 13; CU-A28…A31).

/// Modo de «Calcular columnas». «Incluir pleno al 15» no aparece: solo cambia el `numPartidos` del analizador, que el
/// análisis de la combinación no lee (`Analizador.cs:356` usa `VariablesGlobales.NumeroPartidos`).
public enum ModoCalculo: Sendable, Hashable {
    case contar
    case analizar
    case grabar(URL)
}

/// Contadores y, al analizar con alguna aceptada, el contenedor del visor (`AnalisisUi.MostrarVisor`).
public struct ResultadoCalculoCombinacion: Sendable {
    public let calculo: ResultadoCalculo
    public let analisis: ContenedorAnalisisGlobal?
}

/// Tabla «Resultados» de la pantalla 13 con los textos iniciales del C#.
public struct TextosCalculo: Sendable, Hashable {
    public var procesadas = "0"
    public var costeProcesadas = "0.0"
    public var aceptadas = "0"
    public var costeAceptadas = "0.0"
    public var porcentaje = "0.00 %"
    public var estimadas = "0"
    public var costeEstimadas = "0.0"
    public var maximo = "0"
    public var costeMaximo = "0.0"
    /// 0…100.
    public var progreso = 0.0

    public init() {}
}

public enum CasosCombinacion {
    /// Lo que el motor lee de los ficheros de columnas: `Trim().ToUpper()` por línea (`LeerTodasCols(false)`).
    public static func lectorLineas(_ entorno: Entorno) -> LectorLineas {
        let archivos = entorno.archivos
        return { ruta throws(ErrorDominio) in ArchivoColumnasTexto.columnas(try archivos.leer(URL(filePath: ruta))) }
    }

    /// CU-A30 `RecalcularMaximas` (:173-207): líneas del filtro de columnas (error ⇒ 0) o `2^dobles · 3^triples`.
    public static func estimarColumnasMaximas(_ c: Combinacion, entorno: Entorno) -> Int64 {
        if !c.archivoColumnasBase.isEmpty {
            guard let d = try? entorno.archivos.leer(URL(filePath: c.archivoColumnasBase)) else { return 0 }
            return Int64(ArchivoColumnasTexto.numColumnas(d))
        }
        var (dobles, triples) = (0, 0)
        for p in c.pronosticos ?? [] {
            let n = (p ?? "").replacingOccurrences(of: ",", with: "").utf16.count
            if n == 2 { dobles += 1 }
            if n == 3 { triples += 1 }
        }
        return Int64((pow(2, Double(dobles)) * pow(3, Double(triples))).rounded(.toNearestOrEven))
    }

    /// CU-A29 (:209-266): sin fichero, o si ya lo usa la combinación (filtro de columnas del boleto base o filtro
    /// parcial de un grupo; comparación literal de rutas).
    public static func validarArchivoResultados(_ url: URL?, combinacion c: Combinacion) throws(ErrorDominio) {
        guard let ruta = url?.path(percentEncoded: false), !ruta.isEmpty else {
            throw ErrorDominio(.general, "Seleccione un archivo de resultados.")
        }
        if c.grupos.contains(where: { ruta == ($0.esGrupoBase ? c.archivoColumnasBase : $0.archivoFiltro) }) {
            throw ErrorDominio(.general, "No puede usar como archivo de resultados un archivo usado ya en la combinación")
        }
    }

    /// CU-A28: rutas A (contar), B (grabar) y C (analizar). Al grabar, el `.txt` solo se crea si hay alguna aceptada
    /// (el `StreamWriter` del C# se abre con la primera) y al cancelar queda lo grabado.
    /// ponytail: grabar acumula en memoria (~16 B por columna); escritura por bloques si el universo crece.
    @concurrent public static func calcularCombinacion(
        _ c: Combinacion, modo: ModoCalculo, configuracion: Configuracion, entorno: Entorno,
        progreso: ReportadorProgreso = .ninguno
    ) async throws(ErrorDominio) -> ResultadoCalculoCombinacion {
        let leer = lectorLineas(entorno)
        switch modo {
        case .contar:
            let r = try MotorCalculo.calcular(
                c, configuracion: configuracion, cultura: entorno.cultura, leerLineas: leer, progreso: progreso)
            return ResultadoCalculoCombinacion(calculo: r, analisis: nil)
        case .analizar:
            let r = try MotorCalculo.analizar(
                c, configuracion: configuracion, cultura: entorno.cultura, leerLineas: leer, progreso: progreso)
            return ResultadoCalculoCombinacion(calculo: r.calculo, analisis: r.calculo.aceptadas > 0 ? r.contenedor : nil)
        case .grabar(let url):
            var datos = Data()
            let r = try MotorCalculo.calcular(
                c, configuracion: configuracion, cultura: entorno.cultura, leerLineas: leer, progreso: progreso
            ) { col throws(ErrorDominio) in
                datos.append(contentsOf: try UtilColumnas.convLongToStr(col).utf8)
                datos.append(contentsOf: [0x0D, 0x0A])
            }
            if !datos.isEmpty { try entorno.archivos.escribir(datos, en: url, atomico: false) }
            return ResultadoCalculoCombinacion(calculo: r, analisis: nil)
        }
    }

    /// CU-A31 `ActualizaDatosCalculo` (:367-391) y `RecalcularMaximas`: formatos .NET con la moneda delante.
    public static func estadisticasDeCalculo(
        procesadas p: Int, aceptadas a: Int, maximas: Int64, lae: ConfigLAE, cultura: Cultura
    ) -> TextosCalculo {
        func cols(_ v: Double) -> String { FormatoNET.toStringMiles(v, decimales: 0, decimalesNegativo: 0, cultura: cultura) }
        func coste(_ v: Double) -> String {
            FormatoNET.toStringMiles(v * lae.precioApuesta, decimales: 2, decimalesNegativo: 1, prefijo: lae.moneda, cultura: cultura)
        }
        var t = TextosCalculo()
        (t.procesadas, t.costeProcesadas) = (cols(Double(p)), coste(Double(p)))
        t.progreso = maximas > 0 ? min(100, Double(p) * 100 / Double(maximas)) : 0
        (t.aceptadas, t.costeAceptadas) = (cols(Double(a)), coste(Double(a)))
        let pct = p > 0 ? Double(a) * 100 / Double(p) : 0
        t.porcentaje = FormatoNET.toStringMiles(pct, decimales: 2, decimalesNegativo: 2, cultura: cultura) + " %"
        let estimadas = FormatoNET.round(Double(maximas) * pct / 100, decimales: 0)
        (t.estimadas, t.costeEstimadas) = (cols(estimadas), coste(estimadas))
        (t.maximo, t.costeMaximo) = (cols(Double(maximas)), coste(Double(maximas)))
        return t
    }
}

// C#: Views/Ported/CalculaColumnasMultipleFrmViewModel.cs y ResultadosCalculoMultipleFrmViewModel.cs (08 pantallas 14 y 84;
// CU-A32, CU-A33).

/// Una fila de «Resultados de la Generación Múltiple».
public struct ResumenLote: Sendable, Hashable {
    public var archivoCombinacion: String
    public var archivoColumnas: String
    public var analizadas: String
    public var aceptadas: String
    public var tiempo: String

    public init(archivoCombinacion: String, archivoColumnas: String, analizadas: String, aceptadas: String, tiempo: String) {
        (self.archivoCombinacion, self.archivoColumnas, self.analizadas) = (archivoCombinacion, archivoColumnas, analizadas)
        (self.aceptadas, self.tiempo) = (aceptadas, tiempo)
    }
}

extension CasosCombinacion {
    /// `SeleccionarArchivos`: orden ordinal de las rutas completas.
    public static func ordenarLote(_ urls: [URL]) -> [URL] {
        urls.sorted { $0.path(percentEncoded: false).utf16.lexicographicallyPrecedes($1.path(percentEncoded: false).utf16) }
    }

    /// CU-A33: `Columnas/<nombre>` con `.comb` y `.xml` cambiados por `.txt` (en cualquier parte del nombre).
    public static func nombreSalidaDeCombinacion(_ origen: URL, carpeta: URL) -> URL {
        let nombre = Escrutador.nombreDeArchivo(origen.path(percentEncoded: false))
        return carpeta.appending(
            path: nombre.replacingOccurrences(of: ".comb", with: ".txt").replacingOccurrences(of: ".xml", with: ".txt"))
    }

    /// CU-A32, un elemento (`ActualizaColumnasPrevistas`): abre la combinación con su filtro de columnas; si el filtro no
    /// existe se calcula sin él y se devuelve el aviso.
    public static func prepararCombinacionDeLote(
        _ origen: URL, numeroPartidos: Int, entorno: Entorno
    ) throws(ErrorDominio) -> (combinacion: Combinacion, maximas: Int64, aviso: String?) {
        let leida = try ArchivoCombinacion.leer(try entorno.archivos.leer(origen), numeroPartidos: numeroPartidos, cultura: entorno.cultura)
        var c = leida.combinacion
        c.archivoColumnasBase = leida.filtroColumnas
        var aviso: String?
        if !c.archivoColumnasBase.isEmpty && !entorno.archivos.existe(URL(filePath: c.archivoColumnasBase)) {
            aviso = "No se ha encontrado el filtro " + c.archivoColumnasBase + ". Se calculan las columnas sin filtro."
            c.archivoColumnasBase = ""
        }
        return (c, estimarColumnasMaximas(c, entorno: entorno), aviso)
    }
}

extension CasosCombinacion {
    /// «Examinar…» de la pantalla 8: columnas del fichero (`LeerTodasCols(false)`).
    public static func columnasDeFichero(_ url: URL, entorno: Entorno) throws(ErrorDominio) -> [String] {
        ArchivoColumnasTexto.columnas(try entorno.archivos.leer(url))
    }

    /// CU-A14 `Analizar` (`AnalizarFicheroFrmViewModel.cs:99-135`): ruta D con un analizador de `ObtenNumSignos` partidos
    /// a «1,X,2» y el fichero como columnas base. «Incluir pleno al 15» no interviene (DEC-808). Contenedor solo si hay
    /// aceptadas (el visor se abre entonces).
    @concurrent public static func analizarFicheroDeColumnas(
        _ url: URL, configuracion: Configuracion, entorno: Entorno
    ) async throws(ErrorDominio) -> ContenedorAnalisisGlobal? {
        let partidos = ArchivoColumnasTexto.numSignos(try entorno.archivos.leer(url))
        var c = Combinacion(numeroPartidos: configuracion.boleto.numeroPartidos)
        c.archivoColumnasBase = url.path(percentEncoded: false)
        c.ponerPronosticos(Array(repeating: "1,X,2", count: partidos))
        let r = try MotorCalculo.analizar(
            c, configuracion: configuracion, cultura: entorno.cultura, externo: true, numeroPartidosExterno: partidos,
            leerLineas: lectorLineas(entorno))
        return r.calculo.aceptadas > 0 ? r.contenedor : nil
    }
}
