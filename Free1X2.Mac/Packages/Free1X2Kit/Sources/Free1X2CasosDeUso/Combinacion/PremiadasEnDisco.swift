import Foundation
import Free1X2Base
import Free1X2Motor

/// Las columnas premiadas de un escrutinio, en un fichero temporal y no en memoria: 14 triples contra 1.000 ganadoras dan
/// 19 millones (3,7 GB en memoria). Una premiada por línea (`columna⇥fichero⇥jornada⇥premio⇥nº columna⇥nº boleto`,
/// el fichero por su posición en `ficheros`); se guarda dónde empieza cada página para leerlas por páginas.
public struct PremiadasEnDisco: Sendable, Hashable {
    public static let porPagina = 1000
    public let url: URL
    public let total: Int
    /// El byte donde empieza cada página y, al final, el tamaño del fichero.
    let inicios: [UInt64]
    let ficheros: [String]

    public var paginas: Int { inicios.count - 1 }

    /// Las premiadas de la página `p` (desde 0), en el orden del escrutinio.
    public func pagina(_ p: Int) throws(ErrorDominio) -> [ColumnaPremiada] {
        guard p >= 0, p < paginas else { return [] }
        let datos: Data
        do {
            let h = try FileHandle(forReadingFrom: url)
            defer { try? h.close() }
            try h.seek(toOffset: inicios[p])
            datos = try h.read(upToCount: Int(inicios[p + 1] - inicios[p])) ?? Data()
        } catch {
            throw ErrorDominio(.entradaSalida, "No se pudieron leer las premiadas: \(error.localizedDescription)")
        }
        return datos.split(separator: 0x0A).map { linea in
            // La columna va tal cual (puede llevar tabuladores): los 5 últimos campos son los números.
            let c = linea.split(separator: 0x09, omittingEmptySubsequences: false)
            func numero(_ i: Int) -> Int { Int(String(decoding: c[c.count - 5 + i], as: UTF8.self)) ?? 0 }
            return ColumnaPremiada(
                columna: String(decoding: Data(c[..<(c.count - 5)].joined(separator: [0x09])), as: UTF8.self),
                fichero: ficheros[numero(0)], jornada: numero(1), premio: numero(2), noColumna: numero(3), noBoleto: numero(4))
        }
    }

    /// «Grabar todas»: las columnas, una por línea, leídas página a página.
    /// ponytail: el fichero de salida se arma entero en memoria (el sistema de archivos escribe ficheros enteros); con
    /// decenas de millones de premiadas son cientos de MB. Por bloques cuando se haga «guardar por bloques».
    public func datosColumnas() throws(ErrorDominio) -> Data {
        var d = Data()
        for p in 0..<paginas { d.append(CasosCombinacion.datosColumnas(try pagina(p).map(\.columna), quitarComas: false)) }
        return d
    }

    public func borrar() { try? FileManager.default.removeItem(at: url) }
}

/// Escribe las premiadas en el fichero mientras se escruta, de MB en MB. Si no se cierra (error o cancelación), lo borra.
final class EscritorPremiadas {
    private let url = FileManager.default.temporaryDirectory.appending(path: "Free1X2-premiadas-\(UUID().uuidString).txt")
    private let fichero: FileHandle
    private var buffer: [UInt8] = []
    private var escritos: UInt64 = 0
    private var total = 0
    private var inicios: [UInt64] = [0]
    private var ficheros: [String] = []
    private var cerrado = false

    init() throws(ErrorDominio) {
        guard FileManager.default.createFile(atPath: url.path(percentEncoded: false), contents: nil),
            let f = try? FileHandle(forWritingTo: url)
        else { throw ErrorDominio(.entradaSalida, "No se pudo crear el fichero temporal de las premiadas.") }
        fichero = f
    }

    deinit {
        try? fichero.close()
        if !cerrado { try? FileManager.default.removeItem(at: url) }
    }

    func anadir(_ premiadas: [ColumnaPremiada]) throws(ErrorDominio) {
        for p in premiadas {
            if total > 0, total % PremiadasEnDisco.porPagina == 0 { inicios.append(escritos + UInt64(buffer.count)) }
            // Las premiadas llegan por fichero: basta mirar el último.
            if ficheros.last != p.fichero { ficheros.append(p.fichero) }
            buffer.append(contentsOf: p.columna.utf8)
            campo(ficheros.count - 1)
            campo(p.jornada)
            campo(p.premio)
            campo(p.noColumna)
            campo(p.noBoleto)
            buffer.append(0x0A)
            total += 1
        }
        if buffer.count >= 1 << 20 { try volcar() }
    }

    func cerrar() throws(ErrorDominio) -> PremiadasEnDisco {
        try volcar()
        try? fichero.close()
        cerrado = true
        return PremiadasEnDisco(url: url, total: total, inicios: total == 0 ? [0] : inicios + [escritos], ficheros: ficheros)
    }

    /// Tabulador y el número. ponytail: solo enteros >= 0 (jornada, premio y números de columna y boleto).
    private func campo(_ n: Int) {
        buffer.append(0x09)
        let i = buffer.count
        var n = n
        repeat {
            buffer.append(UInt8(48 + n % 10))
            n /= 10
        } while n > 0
        buffer[i...].reverse()
    }

    private func volcar() throws(ErrorDominio) {
        do {
            try fichero.write(contentsOf: buffer)
        } catch {
            throw ErrorDominio(.entradaSalida, "No se pudieron guardar las premiadas: \(error.localizedDescription)")
        }
        escritos += UInt64(buffer.count)
        buffer.removeAll(keepingCapacity: true)
    }
}
