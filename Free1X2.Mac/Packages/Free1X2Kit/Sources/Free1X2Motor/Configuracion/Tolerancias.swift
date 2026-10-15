import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/ToleranciaFiltros.cs y ControladorTol.cs (01 §2.9-2.10).

/// Una tolerancia: letras y rango de aciertos permitidos sobre la suma de coincidencias de los filtros activos.
public struct ToleranciaConfig: Sendable, Hashable {
    /// `LetrasTol`: el setter pasa a mayúsculas (DT-13).
    public var letrasTol = "" {
        didSet { letrasTol = FormatoNET.mayusculas(letrasTol) }
    }
    public private(set) var aciertos = ""
    public private(set) var aciertosPermitidos: [Bool]?

    public init() {}

    /// `Aciertos = s` (:148-162): guarda el texto y después lo parsea; si el parseo lanza, el array anterior se queda.
    public mutating func ponerAciertos(_ s: String) throws(ErrorDominio) {
        aciertos = s
        aciertosPermitidos = try ArrayAciertosTol.parsear(s, "ToleranciaFiltros.cs:61-114")
    }
}

/// Las tolerancias de un grupo y el presupuesto de tolerancias falladas.
public struct ControladorTolConfig: Sendable, Hashable {
    public var tolerancias: [ToleranciaConfig] = []
    public private(set) var fallosPermitidos = ""
    public private(set) var arrayFallos: [Bool]?

    public init() {}

    /// `FallosPermitidos = s` (:127-139): `""` no borra el array anterior.
    public mutating func ponerFallosPermitidos(_ s: String) throws(ErrorDominio) {
        fallosPermitidos = s
        if !s.isEmpty { arrayFallos = try ArrayAciertosTol.parsear(s, "ControladorTol.cs:43-81") }
    }
}

/// `InicializaArrayAciertos` / `InicializarFallos`: el array mide `máximo + 2` [PRESERVAR]. Aritmética `int` de C#: `máximo + 2`
/// desborda y da un tamaño negativo (`OverflowException`).
enum ArrayAciertosTol {
    static func parsear(_ s: String, _ origen: String) throws(ErrorDominio) -> [Bool] {
        // `EncuentraSeparador`: el primer `,` o `-`.
        switch s.utf16.first(where: { $0 == 0x2C || $0 == 0x2D }) {
        case 0x2C:
            let partes = trozos(s, ",")
            let maximo = try FormatoNET.convertToInt32(partes.last ?? "")
            var a = try [Bool].net(repeating: false, count: Int(maximo &+ 2), origen)
            for p in partes { try a.escribir(true, en: Int(try FormatoNET.convertToInt32(p)), origen) }
            return a
        case 0x2D:
            let partes = trozos(s, "-")  // al menos 2 trozos: hay un `-`
            let alto = try FormatoNET.convertToInt32(partes[1])
            var a = try [Bool].net(repeating: false, count: Int(alto &+ 2), origen)
            let bajo = try FormatoNET.convertToInt32(partes[0])
            let n = (alto &- bajo) &+ 1
            for i in stride(from: Int32(0), to: n, by: 1) { try a.escribir(true, en: Int(bajo &+ i), origen) }
            return a
        default:
            let n = try FormatoNET.convertToInt32(s)
            var a = try [Bool].net(repeating: false, count: Int(n &+ 2), origen)
            try a.escribir(true, en: Int(n), origen)
            return a
        }
    }
}
