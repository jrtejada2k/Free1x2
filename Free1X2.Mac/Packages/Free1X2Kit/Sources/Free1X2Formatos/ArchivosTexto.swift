import Foundation
import Free1X2Base

// Formatos de texto sencillos (07 F09, F10, F12, F13, F17). Escritura con CRLF (DP-02).

/// F10 `Equipos/equipos{1,2,2b,Int}.dat` (`GestorEquiposFrmViewModel.cs:230-251,336-348`): un equipo por línea, Latin-1.
public enum ArchivoEquiposCategoria {
    /// `StreamReader(ruta, Latin1)` + `ReadLine` mientras `Peek() != -1`: las líneas vacías cuentan.
    public static func leer(_ datos: Data) -> [String] {
        TextoNET.lineas(TextoNET.decodificarLatin1(datos))
    }

    /// `StreamWriter(ruta, false, Latin1)` + `WriteLine`: lo que no cabe en Latin-1 se escribe `?`.
    public static func datos(_ equipos: [String]) -> Data {
        var d = Data()
        for e in equipos {
            d.append(contentsOf: e.unicodeScalars.map { $0.value <= 0xFF ? UInt8($0.value) : 0x3F })
            d.append(contentsOf: [0x0D, 0x0A])
        }
        return d
    }
}

/// F17 equipos del boleto `*.txt` (`MainPageViewModel.cs:674-732`).
public enum ArchivoEquiposBoleto {
    public static let sinEquipos = "? - ?"

    /// `File.ReadAllLines` (UTF-8, BOM detectado; sin *fallback* a cp1252, DEC-706): las `N` primeras; faltan ⇒ `"? - ?"`.
    public static func leer(_ datos: Data, numeroPartidos n: Int) -> [String] {
        let l = TextoNET.lineas(datos)
        return (0..<max(n, 0)).map { $0 < l.count ? l[$0] : sinEquipos }
    }

    /// `File.WriteAllLines(ruta, equipos, Encoding.UTF8)`: con BOM y `\r\n` tras cada línea.
    public static func datos(_ equipos: [String]) -> Data {
        Data([0xEF, 0xBB, 0xBF]) + TextoNET.codificarLineas(equipos)
    }
}

/// F09 `Jornadas/Resultados.txt` (`EscrutiniosFrmViewModel.cs:323-369`).
public enum ArchivoResultados {
    public struct Fila: Sendable, Hashable {
        public var temporada: Int16
        public var jornada: Int16
        public var quiniela: String
    }

    /// `Temp⇥Jorn⇥Quiniela`; menos de 3 campos se salta. El primer error (número no válido, quiniela de más de 15)
    /// corta la lectura en silencio: quedan las filas ya leídas y **ninguna** temporada. Temporadas de la última fila a la
    /// primera, decrecientes.
    public static func leer(_ datos: Data?) -> (filas: [Fila], temporadas: [String]) {
        guard let datos else { return ([], []) }
        var filas: [Fila] = []
        do throws(ErrorDominio) {
            for linea in TextoNET.lineas(datos) {
                let r = linea.split(separator: "\t", omittingEmptySubsequences: false).map(String.init)
                guard r.count >= 3 else { continue }
                let t = try FormatoNET.convertToInt16(r[0])
                let j = try FormatoNET.convertToInt16(r[1])
                guard r[2].utf16.count <= 15 else { throw ErrorDominio(.argumento, "MaxLength", origen: "DataColumn") }
                filas.append(Fila(temporada: t, jornada: j, quiniela: r[2]))
            }
        } catch {
            return (filas, [])
        }
        guard let primera = filas.first, let ultima = filas.last, ultima.temporada >= primera.temporada else {
            return (filas, [])
        }
        return (filas, stride(from: Int(ultima.temporada), through: Int(primera.temporada), by: -1).map(String.init))
    }
}

/// F12 `Impresion/impresoras.cfg` (`ControladoresImpresion.cs:45-62`): impresoras conocidas, sin deduplicar.
public enum ArchivoImpresoras {
    public struct Impresora: Sendable, Hashable {
        public var modelo: String
        public var margenSuperior: Int32
        public var margenIzquierda: Int32
        public var rotar: Bool

        public init(modelo: String, margenSuperior: Int32, margenIzquierda: Int32, rotar: Bool) {
            (self.modelo, self.margenSuperior, self.margenIzquierda, self.rotar) = (modelo, margenSuperior, margenIzquierda, rotar)
        }
    }

    /// Todos los hijos de `impresoras` (un comentario dentro ⇒ NRE).
    public static func leer(_ datos: Data) throws(ErrorDominio) -> [Impresora] {
        let o = "ControladoresImpresion.cs:53-56"
        guard let lista = try LectorXML.cargar(datos).primerElemento("impresoras") else { throw .referenciaNula(o) }
        var r: [Impresora] = []
        for h in lista.hijos {
            r.append(
                Impresora(
                    modelo: try h.atributo("modelo", o), margenSuperior: try FormatoNET.convertToInt32(try h.atributo("ms", o)),
                    margenIzquierda: try FormatoNET.convertToInt32(try h.atributo("mi", o)),
                    rotar: try FormatoNET.convertToBoolean(try h.atributo("girar", o))))
        }
        return r
    }
}

/// F13 `Impresion/imprebol.cfg` (`ImprimirBoletoFrmViewModel.cs:275-355`).
public enum ArchivoConfigImpresion {
    public struct Config: Sendable, Hashable {
        public var margenSuperior: Double
        public var margenIzquierdo: Double
        public var modelo: String
        public var girar: Bool

        public init(margenSuperior: Double, margenIzquierdo: Double, modelo: String, girar: Bool) {
            (self.margenSuperior, self.margenIzquierdo, self.modelo, self.girar) = (margenSuperior, margenIzquierdo, modelo, girar)
        }
    }

    /// Cuatro líneas `clave=valor` en orden fijo; los márgenes con `(int)` (truncados).
    public static func datos(_ c: Config) -> Data {
        TextoNET.codificarLineas([
            "margen superior=\(FormatoNET.castInt32(c.margenSuperior))",
            "margen izquierdo=\(FormatoNET.castInt32(c.margenIzquierdo))", "modelo=" + c.modelo,
            "girar=" + (c.girar ? "si" : "no"),
        ])
    }

    /// `nil` si faltan líneas (el C# devuelve `false`); cada valor es lo que sigue al primer `=` y, si no se puede leer,
    /// queda el `previo`.
    public static func leer(_ datos: Data, previo: Config, cultura: Cultura) -> Config? {
        let l = TextoNET.lineas(datos)
        guard l.count >= 4 else { return nil }
        func valor(_ s: String) -> String? { s.firstIndex(of: "=").map { String(s[s.index(after: $0)...]) } }
        func numero(_ s: String, _ p: Double) -> Double { FormatoNET.tryParseDouble(valor(s) ?? "", cultura: cultura) ?? p }
        return Config(
            margenSuperior: numero(l[0], previo.margenSuperior), margenIzquierdo: numero(l[1], previo.margenIzquierdo),
            modelo: valor(l[2]) ?? previo.modelo, girar: (valor(l[3]) ?? "no") == "si")
    }
}
