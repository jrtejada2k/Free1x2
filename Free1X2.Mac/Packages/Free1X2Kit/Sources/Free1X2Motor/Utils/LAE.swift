import Free1X2Base

// C#: Free1X2.Domain/Utils/LAE.cs (06 §2.8). El fichero `Jornadas/InfoJornadasLAE.txt` lo leen y escriben los casos de
// uso; aquí, el cálculo de premios y la sustitución de la línea de la jornada.

/// Importes de premio por categoría (14…10) a partir de la recaudación y los acertantes de una jornada.
public struct LAE: Sendable, Hashable {
    public var temporada: String
    /// `numJornada.PadLeft(2, '0')`.
    public var jornada: String
    public var columnaPremiada: String
    public let recaudacion: Double
    public let precioApuesta: Double
    /// Tanto por ciento por categoría: el de la 14 de la configuración, 10 el resto.
    public let porcentajes: [Double]
    public var premios = [Double](repeating: 0, count: 5)

    /// `new LAE(temporada, numJornada, columnaPremiada, recaudacion)` (:41-57): `recaudacion ≤ 0` ⇒ la de la
    /// configuración.
    public init(temporada: String, numJornada: String, columnaPremiada: String, recaudacion: Double, configuracion: ConfigLAE) {
        self.temporada = temporada
        let faltan = 2 - numJornada.utf16.count
        jornada = faltan > 0 ? String(repeating: "0", count: faltan) + numJornada : numJornada
        self.columnaPremiada = columnaPremiada
        precioApuesta = configuracion.precioApuesta
        porcentajes = [configuracion.porcentaje14, 10, 10, 10, 10]
        self.recaudacion = recaudacion > 0 ? recaudacion : configuracion.recaudacion
    }

    /// `Math.Round(x, 2)` de .NET: `·100`, redondeo a par y `/100` si `|x| < 1e16` (T4).
    static func redondear2(_ x: Double) -> Double {
        abs(x) < 1e16 ? (x * 100).rounded(.toNearestOrEven) / 100 : x
    }

    /// `CalculaPremios` (:141-148): `Math.Round(premio + 0.005, 2)` (no es «redondear hacia arriba», P1); 0 acertantes ⇒ ∞.
    public mutating func calculaPremios(acertantes: [Int]) throws(ErrorDominio) {
        for i in 0..<5 {
            let a = try acertantes.leer(i, "LAE.cs:145")
            premios[i] = Self.redondear2(recaudacion / precioApuesta * porcentajes[i] / 100 / Double(a) + 0.005)
        }
    }

    /// `MontaLinea` (:120-139): la 4.ª columna es el nº de apuestas (`recaudación/precio`); decimales con coma.
    public func linea(cultura: Cultura) -> String {
        let f = { (d: Double) in FormatoNET.toString(d, cultura: cultura).replacingOccurrences(of: ".", with: ",") }
        return ([columnaPremiada, temporada, jornada, f(recaudacion / precioApuesta)] + premios.map(f)).joined(separator: "\t")
    }

    /// `GrabarJornada(acertantes)` (:77-118) sobre las líneas del fichero leídas con `LeeColumnaSinComas` (recortadas y
    /// en mayúsculas): sustituye la de esta temporada y jornada o la añade al final. Devuelve el fichero entero.
    public mutating func grabarJornada(acertantes: [Int], lineas: [String], cultura: Cultura) throws(ErrorDominio) -> [String] {
        try calculaPremios(acertantes: acertantes)
        var salida: [String] = []
        var existe = false
        for l in lineas {
            let v = trozos(l, "\t")
            if try v.leer(1, "LAE.cs:93") == temporada, try v.leer(2, "LAE.cs:93") == jornada {
                existe = true
                salida.append(linea(cultura: cultura))
            } else {
                salida.append(l)
            }
        }
        if !existe { salida.append(linea(cultura: cultura)) }
        return salida
    }
}
