import Free1X2Base

// C#: Free1X2.Domain/EntradaSalida/GenerarCPs/CPs.cs y Valoracion.cs (04 §16); salida F06 de
// GenerarCPsViewModel.cs:116-131,239-302 (07 F06). `IO` salvo `GuardarTexto` es código muerto (DEC-409).

/// Valoración 1/X/2 de un partido (`Valoracion.cs`).
public struct Valoracion: Sendable, Equatable {
    public var uno, equis, dos: Double

    public init(_ uno: Double, _ equis: Double, _ dos: Double) {
        (self.uno, self.equis, self.dos) = (uno, equis, dos)
    }
}

/// Fila de «Tipos de CPs» (F05).
public struct TipoCP: Sendable, Equatable {
    public var tipo: Int32
    /// Nulable en el DataSet: `nil` ⇒ `ToString()` vacío.
    public var nombre: String?

    public init(tipo: Int32, nombre: String?) {
        (self.tipo, self.nombre) = (tipo, nombre)
    }
}

/// Fila de «Configuracion de CPs» (F05): toda columna es nulable (`nil` = `DBNull`).
public struct ConfigCP: Sendable, Equatable {
    public var tipo, desde, hasta: Int32?
    public var forzarFijos: Bool?
    public var numFijos: Int32?
    public var forzarDobles: Bool?
    public var numDobles: Int32?
    public var forzarTriples: Bool?

    public init(
        tipo: Int32? = nil, desde: Int32? = nil, hasta: Int32? = nil, forzarFijos: Bool? = nil, numFijos: Int32? = nil,
        forzarDobles: Bool? = nil, numDobles: Int32? = nil, forzarTriples: Bool? = nil
    ) {
        (self.tipo, self.desde, self.hasta) = (tipo, desde, hasta)
        (self.forzarFijos, self.numFijos, self.forzarDobles) = (forzarFijos, numFijos, forzarDobles)
        (self.numDobles, self.forzarTriples) = (numDobles, forzarTriples)
    }
}

/// Generador de columnas probables (`CPs`).
public enum GeneradorCPs {
    /// `Generar` (`GenerarCPsViewModel.cs:116-131`) sin E/S: por cada tipo, en su orden, el fichero
    /// `{Nombre}_j{jornada}.txt` (los espacios se conservan: el `Replace` no se asigna) y su contenido F06.
    public static func generar(
        tipos: [TipoCP], configuraciones: [ConfigCP], valor: [Valoracion], jornada: Int, numeroPartidos n: Int
    ) throws(ErrorDominio) -> [(fichero: String, texto: String)] {
        var r: [(fichero: String, texto: String)] = []
        for t in tipos {
            // `RowFilter = "Tipo = x"`: en el orden de la tabla; las filas sin `Tipo` no entran.
            let vista = configuraciones.filter { $0.tipo == t.tipo }
            let columnas = try crearCPs(vista, valor, numeroPartidos: n)
            r.append(("\(t.nombre ?? "")_j\(jornada).txt", texto(columnas, numeroPartidos: n)))
        }
        return r
    }

    /// `CrearCPs(vista, valor)` (:16-90): matriz `[partido][CP]`, `nil` = sin asignar.
    public static func crearCPs(
        _ vista: [ConfigCP], _ valor: [Valoracion], numeroPartidos n: Int
    ) throws(ErrorDominio) -> [[String?]] {
        var columnas = [[String?]](repeating: [String?](repeating: nil, count: vista.count), count: n)
        for (j, fila) in vista.enumerated() {
            var tipo = 3
            if fila.forzarFijos == true { tipo = 1 }
            if fila.forzarDobles == true { tipo = 2 }  // pisa a «Fijos» (P22 e)
            let desde = Double(try int16(fila.desde, "CPs.cs:45"))
            let hasta = Double(try int16(fila.hasta, "CPs.cs:46"))
            switch tipo {
            case 1, 2:
                let tmp =
                    tipo == 1
                    ? try obtenerFijos(try int16(fila.numFijos, "CPs.cs:50"), valor, numeroPartidos: n)
                    : try obtenerDobles(try int16(fila.numDobles, "CPs.cs:57"), valor, numeroPartidos: n)
                for k in tmp.indices { columnas[k][j] = tmp[k] }
            default:
                for (k, v) in valor.enumerated() {
                    guard k < n else { throw .fueraDeRango("CPs.cs:66") }
                    var s = ""
                    if v.uno >= desde && v.uno <= hasta { s += "1" }
                    if v.equis >= desde && v.equis <= hasta { s += "X" }
                    if v.dos >= desde && v.dos <= hasta { s += "2" }
                    if s.isEmpty && fila.forzarTriples == true { s = "1X2" }
                    columnas[k][j] = s
                }
            }
        }
        return columnas
    }

    /// `ObtenerFijos` (:92-136): los `cuantos` valores mayores, cada uno al primer partido que lo tenga; la casilla
    /// usada pasa a `0`. Un partido puede recibir varios y quedarse con el último (P22 b, c).
    public static func obtenerFijos(
        _ cuantos: Int, _ valor: [Valoracion], numeroPartidos n: Int
    ) throws(ErrorDominio) -> [String?] {
        if cuantos == n { return try obtenerTodosFijos(valor, numeroPartidos: n) }
        var t: [Valoracion] = []
        for i in 0..<n { t.append(try valor.leer(i, "CPs.cs:105")) }
        let m = Array(ordenarValoracion(t, numeroPartidos: n).reversed())
        var s = [String?](repeating: nil, count: n)
        for i in stride(from: 0, to: cuantos, by: 1) {
            for j in t.indices {
                let x = try m.leer(i, "CPs.cs:115")
                if t[j].uno == x {
                    (s[j], t[j].uno) = ("1", 0)
                    break
                }
                if t[j].equis == x {
                    (s[j], t[j].equis) = ("X", 0)
                    break
                }
                if t[j].dos == x {
                    (s[j], t[j].dos) = ("2", 0)
                    break
                }
            }
        }
        return s
    }

    /// `ObtenerDobles` (:162-205): los `cuantos` valores menores; el signo excluido es el de ese valor y la casilla
    /// usada pasa a `99.99`.
    public static func obtenerDobles(
        _ cuantos: Int, _ valor: [Valoracion], numeroPartidos n: Int
    ) throws(ErrorDominio) -> [String?] {
        if cuantos == n { return try obtenerTodosDobles(valor, numeroPartidos: n) }
        var t: [Valoracion] = []
        for i in 0..<n { t.append(try valor.leer(i, "CPs.cs:175")) }
        let m = ordenarValoracion(t, numeroPartidos: n)
        var s = [String?](repeating: nil, count: n)
        for i in stride(from: 0, to: cuantos, by: 1) {
            for j in t.indices {
                let x = try m.leer(i, "CPs.cs:184")
                if t[j].uno == x {
                    (s[j], t[j].uno) = ("X2", 99.99)
                    break
                }
                if t[j].equis == x {
                    (s[j], t[j].equis) = ("12", 99.99)
                    break
                }
                if t[j].dos == x {
                    (s[j], t[j].dos) = ("1X", 99.99)
                    break
                }
            }
        }
        return s
    }

    /// `ObtenerTodosFijos` (:138-148): el signo estrictamente mayor; empate de máximos ⇒ `nil`.
    public static func obtenerTodosFijos(_ valor: [Valoracion], numeroPartidos n: Int) throws(ErrorDominio) -> [String?] {
        var s = [String?](repeating: nil, count: n)
        for (i, v) in valor.enumerated() {
            if v.uno > v.equis && v.uno > v.dos { try s.escribir("1", en: i, "CPs.cs:143") }
            if v.equis > v.uno && v.equis > v.dos { try s.escribir("X", en: i, "CPs.cs:144") }
            if v.dos > v.equis && v.dos > v.uno { try s.escribir("2", en: i, "CPs.cs:145") }
        }
        return s
    }

    /// `ObtenerTodosDobles` (:150-160): excluye el signo estrictamente menor; empate de mínimos ⇒ `nil`.
    public static func obtenerTodosDobles(_ valor: [Valoracion], numeroPartidos n: Int) throws(ErrorDominio) -> [String?] {
        var s = [String?](repeating: nil, count: n)
        for (i, v) in valor.enumerated() {
            if v.uno < v.equis && v.uno < v.dos { try s.escribir("X2", en: i, "CPs.cs:155") }
            if v.equis < v.uno && v.equis < v.dos { try s.escribir("12", en: i, "CPs.cs:156") }
            if v.dos < v.equis && v.dos < v.uno { try s.escribir("1X", en: i, "CPs.cs:157") }
        }
        return s
    }

    /// `OrdenarValoracion` (:207-218): `N·3` valores ascendentes con `Array.Sort` (NaN primero, DT-15).
    static func ordenarValoracion(_ valor: [Valoracion], numeroPartidos n: Int) -> [Double] {
        var m = [Double](repeating: 0, count: n * 3)
        for (i, v) in valor.enumerated() { (m[i * 3], m[i * 3 + 1], m[i * 3 + 2]) = (v.uno, v.equis, v.dos) }
        IntrosortNET.ordenar(&m)
        return m
    }

    /// F06 (`LlenarDataset` + `LlenarTxtColumnas` + `IO.GuardarTexto`): una línea por CP con los `N` tokens
    /// (`nil` ⇒ `" "`) separados por `,` y `\r\n`; el `WriteLine` final añade otro `\r\n`.
    public static func texto(_ columnas: [[String?]], numeroPartidos n: Int) -> String {
        var txt = ""
        for i in 0..<(columnas.first?.count ?? 0) {
            txt += (0..<n).map { columnas[$0][i] ?? " " }.joined(separator: ",") + "\r\n"
        }
        return txt + "\r\n"
    }

    /// `Convert.ToInt16(celda)`: `DBNull` ⇒ `InvalidCastException`; fuera de `Int16` ⇒ `OverflowException`.
    static func int16(_ v: Int32?, _ origen: String) throws(ErrorDominio) -> Int {
        guard let v else {
            throw ErrorDominio(.conversionInvalida, "No se puede convertir un objeto de DBNull a otros tipos.", origen: origen)
        }
        guard let r = Int16(exactly: v) else {
            throw ErrorDominio(.desbordamiento, "El valor era demasiado grande o demasiado pequeño para Int16.", origen: origen)
        }
        return Int(r)
    }
}
