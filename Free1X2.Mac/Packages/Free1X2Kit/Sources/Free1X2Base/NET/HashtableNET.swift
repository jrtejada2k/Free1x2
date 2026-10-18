// .NET 8: dotnet/runtime release/8.0, System/Collections/Hashtable.cs y HashHelpers.cs (ESPEC_16 §6, DEC-501).

/// `System.Collections.Hashtable` con claves `Int32`/`Int64` encajadas y sin borrados: lo justo para reproducir
/// el orden de `Keys` y las búsquedas que no encuentran una clave de otro tipo (`Int32.Equals(Int64)` es falso).
public struct HashtableNET<Valor: Sendable>: Sendable {
    /// Clave encajada: conserva el tipo, que decide la igualdad.
    public enum Clave: Hashable, Sendable, CustomStringConvertible {
        case int32(Int32)
        case int64(Int64)

        /// `GetHashCode()`: `Int32` = el valor; `Int64` = `(int)v ^ (int)(v >> 32)`.
        var hash: Int32 {
            switch self {
            case .int32(let v): v
            case .int64(let v): Int32(truncatingIfNeeded: v) ^ Int32(truncatingIfNeeded: v >> 32)
            }
        }

        public var description: String {
            switch self {
            case .int32(let v): "i32:\(v)"
            case .int64(let v): "i64:\(v)"
            }
        }
    }

    private struct Cubo: Sendable {
        var clave: Clave?
        var valor: Valor?
        var hashColl: UInt32 = 0  // bit 31 = ha habido colisión
    }

    private static var factorCarga: Float { 0.72 }
    private var cubos: [Cubo]
    private var ocupacion = 0
    private var cargaMaxima: Int
    public private(set) var count = 0

    /// `new Hashtable()`: 3 cubos, `loadsize = (int)(0,72f · 3) = 2`.
    public init() {
        cubos = Array(repeating: Cubo(), count: 3)
        cargaMaxima = Self.carga(3)
    }

    public var numeroDeCubos: Int { cubos.count }

    /// `Keys` (`CopyKeys`): de último cubo al primero.
    public var claves: [Clave] { cubos.reversed().compactMap(\.clave) }

    /// Indexador de lectura: `nil` si no está.
    public subscript(clave: Clave) -> Valor? {
        guard let i = buscar(clave) else { return nil }
        return cubos[i].valor
    }

    /// `ContainsKey`.
    public func contiene(_ clave: Clave) -> Bool { buscar(clave) != nil }

    /// Indexador de escritura (`Insert(key, value, add: false)`).
    public mutating func asignar(_ valor: Valor, a clave: Clave) throws(ErrorDominio) {
        try insertar(clave, valor, anadir: false)
    }

    /// `Add` (`Insert(key, value, add: true)`): clave repetida ⇒ `ArgumentException`.
    public mutating func anadir(_ clave: Clave, _ valor: Valor) throws(ErrorDominio) {
        try insertar(clave, valor, anadir: true)
    }

    private static func carga(_ tamano: Int) -> Int { Int(factorCarga * Float(tamano)) }

    /// `InitHash`: `incr = 1 + (seed·101) mod (tamaño − 1)` en `uint` (el producto se desborda y da la vuelta).
    private static func inicio(_ clave: Clave, _ tamano: Int) -> (hash: UInt32, cubo: Int, incremento: Int) {
        let hash = UInt32(bitPattern: clave.hash) & 0x7FFF_FFFF
        let incremento = 1 + Int((hash &* 101) % UInt32(tamano - 1))
        return (hash, Int(hash % UInt32(tamano)), incremento)
    }

    private func buscar(_ clave: Clave) -> Int? {
        let (hash, inicial, incremento) = Self.inicio(clave, cubos.count)
        var i = inicial
        var intentos = 0
        while true {
            let cubo = cubos[i]
            guard let guardada = cubo.clave else { return nil }
            if cubo.hashColl & 0x7FFF_FFFF == hash && guardada == clave { return i }
            i = (i + incremento) % cubos.count
            intentos += 1
            guard cubo.hashColl & 0x8000_0000 != 0 && intentos < cubos.count else { return nil }
        }
    }

    private mutating func insertar(_ clave: Clave, _ valor: Valor, anadir: Bool) throws(ErrorDominio) {
        if count >= cargaMaxima {
            rehacer(HashHelpersNET.ampliarPrimo(cubos.count))
        } else if ocupacion > cargaMaxima && count > 100 {
            rehacer(cubos.count)
        }
        let (hash, inicial, incremento) = Self.inicio(clave, cubos.count)
        var i = inicial
        for _ in 0..<cubos.count {
            guard let guardada = cubos[i].clave else {
                cubos[i].valor = valor
                cubos[i].clave = clave
                cubos[i].hashColl |= hash
                count += 1
                return
            }
            if cubos[i].hashColl & 0x7FFF_FFFF == hash && guardada == clave {
                if anadir {
                    throw ErrorDominio(
                        .argumento, "Item has already been added. Key in dictionary: '\(guardada)'", origen: "Hashtable.cs Insert")
                }
                cubos[i].valor = valor
                return
            }
            if cubos[i].hashColl & 0x8000_0000 == 0 {
                cubos[i].hashColl |= 0x8000_0000
                ocupacion += 1
            }
            i = (i + incremento) % cubos.count
        }
        throw ErrorDominio(.operacionInvalida, "Hashtable insert failed. Load factor too high.", origen: "Hashtable.cs Insert")
    }

    /// `rehash(newsize)` + `putEntry`: recoloca en el orden de los cubos viejos y reinicia la ocupación.
    private mutating func rehacer(_ tamano: Int) {
        ocupacion = 0
        var nuevos = Array(repeating: Cubo(), count: tamano)
        for viejo in cubos {
            guard let clave = viejo.clave else { continue }
            let hash = viejo.hashColl & 0x7FFF_FFFF
            let incremento = 1 + Int((hash &* 101) % UInt32(tamano - 1))
            var i = Int(hash % UInt32(tamano))
            while nuevos[i].clave != nil {
                if nuevos[i].hashColl & 0x8000_0000 == 0 {
                    nuevos[i].hashColl |= 0x8000_0000
                    ocupacion += 1
                }
                i = (i + incremento) % tamano
            }
            nuevos[i].valor = viejo.valor
            nuevos[i].clave = clave
            nuevos[i].hashColl |= hash
        }
        cubos = nuevos
        cargaMaxima = Self.carga(tamano)
    }
}

/// `HashHelpers` de .NET 8.
enum HashHelpersNET {
    private static let primos: [Int] = [
        3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597,
        1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023, 25229, 30293, 36353, 43627,
        52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827,
        807403, 968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369,
    ]

    /// `ExpandPrime`: primo ≥ 2·tamaño (tope `0x7FFFFFC3`).
    static func ampliarPrimo(_ viejo: Int) -> Int {
        let nuevo = 2 * viejo
        if nuevo > 0x7FFF_FFC3 && 0x7FFF_FFC3 > viejo { return 0x7FFF_FFC3 }
        return primo(minimo: nuevo)
    }

    /// `GetPrime`: de la tabla; fuera de ella, el primer primo impar con `(p − 1) mod 101 ≠ 0`.
    static func primo(minimo: Int) -> Int {
        if let p = primos.first(where: { $0 >= minimo }) { return p }
        var i = minimo | 1
        while i < Int(Int32.max) {
            if esPrimo(i) && (i - 1) % 101 != 0 { return i }
            i += 2
        }
        return minimo
    }

    private static func esPrimo(_ n: Int) -> Bool {
        guard n & 1 != 0 else { return n == 2 }
        var d = 3
        while d * d <= n {
            if n % d == 0 { return false }
            d += 2
        }
        return true
    }
}
