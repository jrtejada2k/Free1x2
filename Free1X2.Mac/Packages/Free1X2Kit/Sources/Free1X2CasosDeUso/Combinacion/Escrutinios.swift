import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/EscrutiniosFrmViewModel.cs (09 pantalla 45; CU-B05…B08) y ColumnasPremiadasFrmViewModel.cs (08 pantalla 20).

/// Contra qué se escrutan los ficheros (pestañas Simple · Contra fichero · Contra jornadas).
public enum ModoEscrutinio: Sendable, Hashable {
    case simple(ganadora: String)
    case contraFichero(URL?)
    /// `digitosTemporada` 4 o 2, `digitosJornada` 2 o 1; sin temporadas marcadas entran todas (filtro vacío).
    case contraJornadas(plantilla: String, carpeta: URL?, digitosTemporada: Int, digitosJornada: Int, temporadas: [Int])
}

public struct EntradaEscrutinio: Sendable, Hashable {
    public var modo: ModoEscrutinio
    public var ficheros: [URL]
    /// `ObtenIntArray(RangoAciertos)`: categorías que cuentan como premio y que se muestran.
    public var rangoAciertos: [Int]
    public var registrarPremiadas: Bool

    public init(modo: ModoEscrutinio, ficheros: [URL], rangoAciertos: [Int], registrarPremiadas: Bool) {
        (self.modo, self.ficheros, self.rangoAciertos, self.registrarPremiadas) = (modo, ficheros, rangoAciertos, registrarPremiadas)
    }
}

/// Filas (con «TOTALES» al final si se escrutó algo), totales por categoría y premiadas.
public struct ResultadoEscrutinio: Sendable, Hashable {
    public var filas: [FilaEscrutinio]
    public var globales: [Int]
    public var premiadas: [ColumnaPremiada]
}

/// Fila de «Columnas Premiadas» (pantalla 20), todo texto.
public struct FilaPremiada: Sendable, Hashable {
    public var archivoColumnas: String
    public var jornada: String
    public var columna: String
    public var premio: String
    public var numeroColumna: String
    public var numeroBoleto: String
}

/// Lo que muestra la rejilla: cabeceras de categoría (de mayor a menor) y, por fila, su conteo y el resumen.
public struct ProyeccionEscrutinio: Sendable, Hashable {
    public struct Fila: Sendable, Hashable {
        public var fila: FilaEscrutinio
        public var conteos: [String]
        public var premios: String
    }

    public struct Barra: Sendable, Hashable {
        public var aciertos: Int
        public var columnas: Int
    }

    public var cabeceras: [Int]
    public var filas: [Fila]
    /// `aciertos -> columnas` de mayor a menor.
    public var histograma: [Barra]
}

extension CasosCombinacion {
    /// Validación de `Escrutar` (:493-527); los textos van como `ErrorDominio(.general)`.
    public static func validarEscrutinio(_ e: EntradaEscrutinio) throws(ErrorDominio) {
        func falta(_ m: String) -> ErrorDominio { ErrorDominio(.general, m) }
        switch e.modo {
        case .simple:
            if e.ficheros.isEmpty { throw falta("Falta fichero a escrutar.") }
        case .contraFichero(let referencia):
            if e.ficheros.isEmpty { throw falta("Falta fichero a escrutar.") }
            if referencia == nil { throw falta("Falta fichero de referencia.") }
        case .contraJornadas(let plantilla, let carpeta, _, _, _):
            if plantilla.isEmpty { throw falta("Falta plantilla de nombre de fichero.") }
            if carpeta == nil { throw falta("Falta la carpeta de los ficheros.") }
            if !plantilla.contains("/t") { throw falta("No se ha puesto el indicador de temporada (/t).") }
            if !plantilla.contains("/j") { throw falta("No se ha puesto el indicador de jornada (/j).") }
        }
    }

    /// CU-B05: `Jornadas/Resultados.txt` (sin fichero, histórico vacío).
    public static func cargarHistoricoJornadas(entorno: Entorno) -> (filas: [ArchivoResultados.Fila], temporadas: [String]) {
        ArchivoResultados.leer(try? entorno.archivos.leer(entorno.rutas.carpeta(.jornadas).appending(path: "Resultados.txt")))
    }

    /// CU-B06 `EscrutarContraJornadas` (:659-752): ficheros de la carpeta que casan con la plantilla (uno por coincidencia,
    /// con el **nombre de la plantilla**, como `GetFiles(cons)`), su (temporada, jornada) deducida del nombre y la
    /// quiniela del histórico. ponytail: el patrón usa `fnmatch` sin mayúsculas (corchetes y `\` no son de .NET).
    static func ficherosDeJornadas(
        plantilla: String, carpeta: URL, digitosTemporada dt: Int, digitosJornada dj: Int, temporadas: [Int],
        historico: [ArchivoResultados.Fila], entorno: Entorno
    ) throws(ErrorDominio) -> [(ruta: String, nombre: String, quiniela: String)] {
        let filtradas = temporadas.isEmpty ? historico : historico.filter { temporadas.contains(Int($0.temporada)) }
        let nombres = (try? entorno.archivos.listar(carpeta))?.map(\.lastPathComponent)
        var lista: [String] = []
        for f in filtradas {
            var temp = String(format: "%04d", Int(f.temporada))
            if dt == 2 { temp = String(temp.dropFirst(2)) }
            let jorn = dj == 2 ? String(format: "%02d", Int(f.jornada)) : String(f.jornada)
            let cons = plantilla.replacingOccurrences(of: "/t", with: temp).replacingOccurrences(of: "/j", with: jorn)
            guard let nombres else { continue }
            lista += Array(repeating: cons, count: nombres.filter { fnmatch(cons, $0, FNM_CASEFOLD) == 0 }.count)
        }
        let p = Array(plantilla.utf16)
        func posicion(_ marca: String) -> Int {
            let m = Array(marca.utf16)
            return (0...max(p.count - m.count, 0)).first { p.count >= m.count && Array(p[$0..<$0 + m.count]) == m } ?? -1
        }
        let (posT, posJ) = (posicion("/t"), posicion("/j"))
        let digitos = Set("0123456789".utf16)
        var salida: [(ruta: String, nombre: String, quiniela: String)] = []
        for archivo in lista {
            let u = Array(archivo.utf16)
            func sub(_ i: Int, _ n: Int) throws(ErrorDominio) -> [UInt16] {
                guard i >= 0, n >= 0, i + n <= u.count else { throw .fueraDeRango("EscrutiniosFrmViewModel.cs:Substring") }
                return Array(u[i..<i + n])
            }
            func numero(_ t: [UInt16]) throws(ErrorDominio) -> Int16 { try FormatoNET.convertToInt16(String(decoding: t, as: UTF16.self)) }
            let temporada: Int16
            let jornada: Int16
            if posT > posJ {
                var jorn = try sub(posJ, 2)
                let djLocal = digitos.contains(jorn[1]) ? 2 : 1
                if djLocal == 1 { jorn = [jorn[0]] }
                jornada = try numero(jorn)
                temporada = try numero(try sub(posT + djLocal - 2, dt))
            } else {
                temporada = try numero(try sub(posT, dt))
                var jorn = try sub(posJ + dt - 2, 2)
                if !digitos.contains(jorn[1]) { jorn = [jorn[0]] }
                jornada = try numero(jorn)
            }
            guard let fila = historico.first(where: { $0.temporada == temporada && $0.jornada == jornada }) else { continue }
            salida.append(
                (carpeta.appending(path: archivo).path(percentEncoded: false), Escrutador.nombreDeArchivo(archivo), fila.quiniela))
        }
        return salida
    }

    /// CU-B07 `Escrutar` (:490-652): un `Escrutador` por fichero; la fila «TOTALES» la añade el último.
    @concurrent public static func escrutarColumnas(
        _ e: EntradaEscrutinio, numeroPartidos n: Int, entorno: Entorno
    ) async throws(ErrorDominio) -> ResultadoEscrutinio {
        var r = ResultadoEscrutinio(filas: [], globales: Array(repeating: 0, count: n + 1), premiadas: [])
        var ultimo: Escrutador?
        func columnas(_ ruta: String) throws(ErrorDominio) -> [String] {
            ArchivoColumnasTexto.columnas(try entorno.archivos.leer(URL(filePath: ruta)))
        }
        /// Sin ganadoras (referencia vacía) el C# no llega a abrir el fichero de columnas.
        func escrutar(
            _ ruta: String, leer: Bool = true, _ hacer: (inout Escrutador, [String]) throws(ErrorDominio) -> [FilaEscrutinio]
        ) throws(ErrorDominio) {
            var esc = Escrutador(premios: e.rangoAciertos, numeroPartidos: n, archivoColumnas: ruta, anadirAGanadoras: e.registrarPremiadas)
            r.filas += try hacer(&esc, leer ? try columnas(ruta) : [])
            if e.registrarPremiadas { r.premiadas += esc.listaPremiadas }
            for i in 0...n { r.globales[i] += esc.premiosTotales[i] }
            ultimo = esc
        }
        switch e.modo {
        case .contraJornadas(let plantilla, let carpeta?, let dt, let dj, let temporadas):
            let historico = cargarHistoricoJornadas(entorno: entorno).filas
            for f in try ficherosDeJornadas(
                plantilla: plantilla, carpeta: carpeta, digitosTemporada: dt, digitosJornada: dj, temporadas: temporadas,
                historico: historico, entorno: entorno)
            {
                try escrutar(f.ruta) { x, cols throws(ErrorDominio) in
                    [try x.escrutarConColumna(f.quiniela, columnas: cols, archivo: f.nombre)]
                }
            }
        case .simple(let ganadora):
            for f in e.ficheros {
                let ruta = f.path(percentEncoded: false)
                try escrutar(ruta) { x, cols throws(ErrorDominio) in
                    [try x.escrutarConColumna(ganadora, columnas: cols, archivo: Escrutador.nombreDeArchivo(ruta))]
                }
            }
        case .contraFichero(let referencia?):
            for f in e.ficheros {
                let ruta = f.path(percentEncoded: false)
                let ganadoras = try columnas(referencia.path(percentEncoded: false))
                try escrutar(ruta, leer: !ganadoras.isEmpty) { x, cols throws(ErrorDominio) in
                    try x.escrutarTemporada(ganadoras: ganadoras, columnas: cols, archivo: Escrutador.nombreDeArchivo(ruta))
                }
            }
        case .contraFichero(nil), .contraJornadas(_, nil, _, _, _):
            break  // `validarEscrutinio` ya lo avisa
        }
        if let u = ultimo { r.filas.append(try u.filaTotales(r.globales)) }
        return r
    }

    /// `ProyectarResultados` (:756-811) y el histograma (:625-632): categorías del rango de mayor a menor.
    public static func proyectarEscrutinio(_ r: ResultadoEscrutinio, rangoAciertos: [Int]) -> ProyeccionEscrutinio {
        let orden = rangoAciertos.sorted(by: >)
        let filas = r.filas.map { f in
            let conteos = orden.map { f.premios.indices.contains($0) ? String(f.premios[$0]) : "" }
            let premios = zip(orden, conteos).filter { !$0.1.isEmpty }.map { "\($0.0): \($0.1)  " }.joined()
            return ProyeccionEscrutinio.Fila(fila: f, conteos: conteos, premios: FormatoNET.recortar(premios))
        }
        let histograma = orden.filter { r.globales.indices.contains($0) }.map {
            ProyeccionEscrutinio.Barra(aciertos: $0, columnas: r.globales[$0])
        }
        return ProyeccionEscrutinio(cabeceras: orden, filas: filas, histograma: histograma)
    }

    /// CU-B08 «Ver premiadas»: `NoBoleto (orden)` con orden = `NoColumna % 8` (8 si 0).
    public static func filasPremiadas(_ premiadas: [ColumnaPremiada]) -> [FilaPremiada] {
        premiadas.map { p in
            let orden = p.noColumna % 8 == 0 ? 8 : p.noColumna % 8
            return FilaPremiada(
                archivoColumnas: Escrutador.nombreDeArchivo(p.fichero), jornada: String(p.jornada), columna: p.columna,
                premio: String(p.premio), numeroColumna: String(p.noColumna), numeroBoleto: "\(p.noBoleto) (\(orden))")
        }
    }

    /// `Final = <TimeSpan>` recortado a 18 caracteres (:647-650).
    public static func estadoFinalEscrutinio(_ segundos: TimeInterval) -> String {
        String(("Final = " + FormatoNET.toStringTimeSpan(segundos)).prefix(18))
    }

    /// «Grabar columnas…» (45) y «Guardar…» (20): una columna por línea; la 45 quita comas (`GuardarCols`) y la 20 escribe
    /// la columna tal cual (`StreamWriter.WriteLine`).
    public static func datosColumnas(_ columnas: [String], quitarComas: Bool) -> Data {
        Data(columnas.map { (quitarComas ? $0.replacingOccurrences(of: ",", with: "") : $0) + "\r\n" }.joined().utf8)
    }
}

extension CasosCombinacion {
    /// `OnColumnaGanadoraChanged` (:183-198): un signo por casilla en mayúsculas; lo que falta o está en blanco, `*`.
    public static func casillasGanadora(_ texto: String, numeroPartidos n: Int) -> [String] {
        let u = Array(FormatoNET.mayusculas(texto).utf16)
        return (0..<n).map { i in
            guard i < u.count else { return "*" }
            let s = String(decoding: [u[i]], as: UTF16.self)
            return FormatoNET.recortar(s).isEmpty ? "*" : s
        }
    }

    /// `VolcarCasillasATexto` (:164-181): primer carácter de cada casilla (vacía = `*`).
    public static func textoGanadora(_ casillas: [String]) -> String {
        casillas.map { c in
            let s = FormatoNET.mayusculas(FormatoNET.recortar(c))
            return s.isEmpty ? "*" : String(s.prefix(1))
        }.joined()
    }

    /// `CiclarSigno` (:200-211): `* → 1 → X → 2 → *`; lo desconocido pasa a `1`.
    public static func ciclarSigno(_ s: String) -> String {
        let secuencia = ["*", "1", "X", "2"]
        let actual = FormatoNET.mayusculas(FormatoNET.recortar(s))
        return secuencia[((secuencia.firstIndex(of: actual.isEmpty ? "*" : actual) ?? 0) + 1) % 4]
    }
}
