import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Online

// Nuevo (DT-84, a petición del dueño; no existe en el C#): máscaras guardadas y su escrutinio contra el histórico.

/// Una máscara: el signo o signos de cada partido del boleto, con un nombre y sin equipos (se aplica a la jornada cargada).
public struct Mascara: Sendable, Hashable, Identifiable {
    public var nombre: String
    /// Un pronóstico por partido en el formato del boleto (`"1"`, `"1,X"`, `"1,X,2"`).
    public var pronosticos: [String]
    public var id: String { nombre }

    public init(nombre: String, pronosticos: [String]) {
        self.nombre = nombre
        self.pronosticos = pronosticos
    }

    /// Columnas que desarrolla: el producto de los signos de cada partido.
    public var columnas: Int { pronosticos.reduce(1) { $0 * CasosInicio.signos($1).filter { $0 }.count } }

    /// Fijos, dobles y triples.
    public var resumen: (fijos: Int, dobles: Int, triples: Int) {
        let n = pronosticos.map { CasosInicio.signos($0).filter { $0 }.count }
        return (n.filter { $0 == 1 }.count, n.filter { $0 == 2 }.count, n.filter { $0 == 3 }.count)
    }
}

/// Lo que una máscara habría acertado en el histórico: por cada nº de aciertos premiado, las columnas que lo lograron
/// sumando todas las jornadas.
public struct ResultadoMascara: Sendable, Hashable, Identifiable {
    public var nombre: String
    public var columnas: Int
    /// `premios[k]` = columnas con `k` aciertos (k = 10…14).
    public var premios: [Int: Int]
    /// Jornadas en las que al menos una columna tuvo premio.
    public var jornadasConPremio: Int
    public var jornadas: Int
    public var id: String { nombre }
}

public enum CasosMascaras {
    public static let aciertosPremiados = 10...14

    static func carpeta(_ e: Entorno) -> URL { e.rutas.base.appending(path: "Mascaras", directoryHint: .isDirectory) }
    static func ruta(_ nombre: String, _ e: Entorno) -> URL { carpeta(e).appending(path: nombre + ".mas") }

    /// Nombre recortado, no vacío y válido como nombre de fichero en macOS y Windows.
    public static func nombreValido(_ nombre: String) -> String? {
        let n = FormatoNET.recortar(nombre)
        return n.isEmpty || n.contains(where: { "/\\:*?\"<>|".contains($0) }) ? nil : n
    }

    public static func existe(_ nombre: String, entorno: Entorno) -> Bool { entorno.archivos.existe(ruta(nombre, entorno)) }

    /// `Mascaras/<nombre>.mas`: UTF-8 con una línea por partido y su pronóstico (`1,X`); sobrescribe si ya existe.
    public static func guardar(_ m: Mascara, entorno: Entorno) throws(ErrorDominio) {
        guard let nombre = nombreValido(m.nombre) else {
            throw ErrorDominio(.argumento, "El nombre de la máscara no puede estar vacío ni llevar / \\ : * ? \" < > |.")
        }
        try entorno.archivos.crearCarpeta(carpeta(entorno))
        try entorno.archivos.escribir(
            Data((m.pronosticos.joined(separator: "\r\n") + "\r\n").utf8), en: ruta(nombre, entorno), atomico: true)
    }

    /// Las guardadas para `n` partidos, por nombre; se saltan las que no tienen `n` pronósticos válidos.
    public static func listar(numeroPartidos n: Int, entorno: Entorno) -> [Mascara] {
        let ficheros = (try? entorno.archivos.listar(carpeta(entorno))) ?? []
        return ficheros.filter { $0.pathExtension == "mas" }.compactMap { u in
            guard let datos = try? entorno.archivos.leer(u) else { return nil }
            let p = String(decoding: datos, as: UTF8.self).components(separatedBy: .newlines).map(FormatoNET.recortar)
                .filter { !$0.isEmpty }
            let validos = p.allSatisfy { x in
                let s = x.components(separatedBy: ",")
                return !s.isEmpty && Set(s).count == s.count && s.allSatisfy { ["1", "X", "2"].contains($0) }
            }
            return p.count == n && validos ? Mascara(nombre: u.deletingPathExtension().lastPathComponent, pronosticos: p) : nil
        }
    }

    public static func borrar(_ nombre: String, entorno: Entorno) throws(ErrorDominio) {
        try entorno.archivos.borrar(ruta(nombre, entorno))
    }

    /// Columnas de la máscara con k aciertos (k = 0…partidos) frente a `ganadora`, sin desarrollarlas: cada partido
    /// multiplica por (fallos + aciertos·x), y el coeficiente de xᵏ cuenta las columnas con k aciertos.
    public static func aciertos(_ pronosticos: [String], ganadora: String) -> [Int] {
        var cuenta = [1]
        for (p, g) in zip(pronosticos, ganadora) {
            let s = CasosInicio.signos(p)
            let acierta = ["1", "X", "2"].firstIndex(of: String(g)).map { s[$0] } ?? false
            let fallos = s.filter { $0 }.count - (acierta ? 1 : 0)
            var nueva = Array(repeating: 0, count: cuenta.count + 1)
            for (k, c) in cuenta.enumerated() {
                nueva[k] += c * fallos
                if acierta { nueva[k + 1] += c }
            }
            cuenta = nueva
        }
        return cuenta
    }

    /// Escruta la máscara contra cada ganadora (los 14 primeros partidos) y suma las columnas premiadas de 10 a 14.
    public static func escrutar(_ m: Mascara, ganadoras: [String]) -> ResultadoMascara {
        var r = ResultadoMascara(nombre: m.nombre, columnas: m.columnas, premios: [:], jornadasConPremio: 0, jornadas: ganadoras.count)
        let catorce = Array(m.pronosticos.prefix(JornadaQuiniela.numeroPartidos))
        for g in ganadoras {
            let a = aciertos(catorce, ganadora: g)
            var premiada = false
            for k in aciertosPremiados where k < a.count && a[k] > 0 {
                r.premios[k, default: 0] += a[k]
                premiada = true
            }
            if premiada { r.jornadasConPremio += 1 }
        }
        return r
    }
}

/// Una ganadora de la base: su jornada (si se conoce) y sus 14 signos.
public struct GanadoraHistorica: Sendable, Hashable {
    public var jornada: String?
    public var ganadora: String

    public init(jornada: String?, ganadora: String) {
        self.jornada = jornada
        self.ganadora = ganadora
    }
}

/// Lo que hizo una importación en la base.
public struct ResumenImportacion: Sendable, Hashable {
    public var anadidas = 0
    public var yaEstaban = 0
    public var actualizadas = 0
    /// Líneas sin jornada: se añaden sin poder comprobar si ya estaban.
    public var sinJornada = 0
    public var total = 0

    public var mensaje: String {
        var p = ["\(anadidas) añadidas", "\(yaEstaban) ya estaban"]
        if actualizadas > 0 { p.append("\(actualizadas) actualizadas (otra columna para la misma jornada)") }
        if sinJornada > 0 { p.append("\(sinJornada) sin jornada (añadidas sin comprobar si ya estaban)") }
        return p.joined(separator: ", ") + ". La liga tiene ahora \(total) jornadas."
    }
}

/// Base de ganadoras por liga (DT-88, a petición del dueño): `Historico/<liga>.txt`, una línea `jornada;columna` (o solo
/// `columna`) por jornada. Las importaciones y las descargas de clubprogol se suman sin duplicar jornadas.
public enum CasosHistorico {
    public static func liga(_ p: PaisQuiniela) -> String { p == .es ? "España" : "México" }

    static func carpeta(_ e: Entorno) -> URL { e.rutas.base.appending(path: "Historico", directoryHint: .isDirectory) }
    static func ruta(_ liga: String, _ e: Entorno) -> URL { carpeta(e).appending(path: liga + ".txt") }

    /// Las ligas de la base con su número de jornadas, por nombre.
    public static func ligas(entorno: Entorno) -> [(nombre: String, jornadas: Int)] {
        let ficheros = (try? entorno.archivos.listar(carpeta(entorno))) ?? []
        return ficheros.filter { $0.pathExtension == "txt" }.map { u in
            let nombre = u.deletingPathExtension().lastPathComponent
            return (nombre, leer(nombre, entorno: entorno).count)
        }
        .sorted { $0.nombre.localizedStandardCompare($1.nombre) == .orderedAscending }
    }

    /// Las ganadoras guardadas de la liga (vacío si no existe o no se puede leer).
    public static func leer(_ liga: String, entorno: Entorno) -> [GanadoraHistorica] {
        guard let datos = try? entorno.archivos.leer(ruta(liga, entorno)) else { return [] }
        return (try? analizar(datos)) ?? []
    }

    /// Archivo para importar (y el formato de la base): una jornada por línea, `jornada;columna` o solo `columna`; las
    /// líneas en blanco se saltan; de la columna cuentan sus 14 primeros signos (lo de detrás, p. ej. el pleno, se
    /// ignora). Una línea mal formada detiene la lectura con su número: mejor avisar que guardar datos rotos.
    public static func analizar(_ datos: Data) throws(ErrorDominio) -> [GanadoraHistorica] {
        var r: [GanadoraHistorica] = []
        for (i, l) in ArchivoColumnasTexto.columnas(datos).enumerated() where !l.isEmpty {
            var jornada: String?
            var columna = l
            if let pc = l.firstIndex(of: ";") {
                jornada = FormatoNET.recortar(String(l[..<pc]))
                columna = FormatoNET.recortar(String(l[l.index(after: pc)...]))
            }
            let g = String(columna.prefix(JornadaQuiniela.numeroPartidos))
            guard jornada?.isEmpty != true, g.count == JornadaQuiniela.numeroPartidos, g.allSatisfy({ "1X2".contains($0) }) else {
                throw ErrorDominio(
                    .formato, "La línea \(i + 1) no es «jornada;columna» ni una columna de 14 signos 1, X o 2: «\(l)».")
            }
            r.append(GanadoraHistorica(jornada: jornada, ganadora: g))
        }
        return r
    }

    /// Crea la liga vacía (si no existe) para poder importar en ella.
    public static func crear(_ liga: String, entorno: Entorno) throws(ErrorDominio) -> String {
        guard let n = CasosMascaras.nombreValido(liga) else {
            throw ErrorDominio(.argumento, "El nombre de la liga no puede estar vacío ni llevar / \\ : * ? \" < > |.")
        }
        if !entorno.archivos.existe(ruta(n, entorno)) { try guardar([], en: n, entorno: entorno) }
        return n
    }

    /// Suma `nuevas` a la liga: una jornada que ya estaba no se duplica (si trae otra columna, se actualiza); las
    /// líneas sin jornada se añaden siempre.
    public static func anadir(_ nuevas: [GanadoraHistorica], a liga: String, entorno: Entorno) throws(ErrorDominio)
        -> ResumenImportacion
    {
        var base = leer(liga, entorno: entorno)
        var indice: [String: Int] = [:]
        for (i, g) in base.enumerated() { if let j = g.jornada { indice[j] = i } }
        var r = ResumenImportacion()
        for g in nuevas {
            guard let j = g.jornada else {
                base.append(g)
                r.sinJornada += 1
                continue
            }
            if let i = indice[j] {
                if base[i].ganadora == g.ganadora { r.yaEstaban += 1 } else { (base[i], r.actualizadas) = (g, r.actualizadas + 1) }
            } else {
                indice[j] = base.count
                base.append(g)
                r.anadidas += 1
            }
        }
        try guardar(base, en: liga, entorno: entorno)
        r.total = base.count
        return r
    }

    /// «Añadir ganadoras desde archivo…»: un archivo sin ninguna ganadora es un error, no una importación vacía.
    public static func importar(_ datos: Data, a liga: String, entorno: Entorno) throws(ErrorDominio) -> ResumenImportacion {
        let nuevas = try analizar(datos)
        guard !nuevas.isEmpty else { throw ErrorDominio(.formato, "El archivo no tiene ninguna ganadora.") }
        return try anadir(nuevas, a: liga, entorno: entorno)
    }

    /// Las ganadoras descargadas de clubprogol se suman a la liga «España» o «México» (decisión del dueño).
    public static func sumarDescarga(_ h: [ResultadoHistorico], pais: PaisQuiniela, entorno: Entorno) throws(ErrorDominio)
        -> ResumenImportacion
    {
        try anadir(h.map { GanadoraHistorica(jornada: String($0.jornada), ganadora: $0.ganadora) }, a: liga(pais), entorno: entorno)
    }

    public static func borrar(_ liga: String, entorno: Entorno) throws(ErrorDominio) {
        try entorno.archivos.borrar(ruta(liga, entorno))
    }

    static func guardar(_ g: [GanadoraHistorica], en liga: String, entorno: Entorno) throws(ErrorDominio) {
        try entorno.archivos.crearCarpeta(carpeta(entorno))
        let texto = g.map { ($0.jornada.map { $0 + ";" } ?? "") + $0.ganadora + "\r\n" }.joined()
        try entorno.archivos.escribir(Data(texto.utf8), en: ruta(liga, entorno), atomico: true)
    }
}
