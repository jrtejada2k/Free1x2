// C#/.NET: `new System.Random(int semilla)` = `Random.Net5CompatSeedImpl` (ESPEC_16 §5): generador sustractivo de Knuth.
// Aritmética `int` sin `checked` de C#: `&-`/`&+` (T1).

/// Secuencia de `System.Random(semilla)`, bit a bit. Sin semilla, los casos de uso usan el generador del sistema.
public struct AleatorioNET: Sendable {
    static let mbig = Int32.max
    static let mseed: Int32 = 161_803_398

    private var tabla = [Int32](repeating: 0, count: 56)
    private var inext = 0
    private var inextp = 21

    public init(semilla: Int32) {
        let resta = semilla == Int32.min ? Int32.max : abs(semilla)
        var mj = Self.mseed &- resta
        tabla[55] = mj
        var mk: Int32 = 1
        var ii = 0
        for _ in 1..<55 {
            ii += 21
            if ii >= 55 { ii -= 55 }
            tabla[ii] = mk
            mk = mj &- mk
            if mk < 0 { mk &+= Self.mbig }
            mj = tabla[ii]
        }
        for _ in 1..<5 {
            for i in 1..<56 {
                var n = i + 30
                if n >= 55 { n -= 55 }
                tabla[i] &-= tabla[1 + n]
                if tabla[i] < 0 { tabla[i] &+= Self.mbig }
            }
        }
    }

    /// `InternalSample`.
    private mutating func muestraInterna() -> Int32 {
        inext = inext + 1 >= 56 ? 1 : inext + 1
        inextp = inextp + 1 >= 56 ? 1 : inextp + 1
        var r = tabla[inext] &- tabla[inextp]
        if r == Self.mbig { r -= 1 }
        if r < 0 { r &+= Self.mbig }
        tabla[inext] = r
        return r
    }

    /// `Sample()`: `[0, 1)`.
    private mutating func muestra() -> Double { Double(muestraInterna()) * (1.0 / Double(Self.mbig)) }

    /// `GetSampleForLargeRange`.
    private mutating func muestraRangoGrande() -> Double {
        var r = muestraInterna()
        if muestraInterna() % 2 == 0 { r = 0 &- r }
        return (Double(r) + Double(Int32.max - 1)) / Double(2 * UInt32(Int32.max) - 1)
    }

    /// `Next()`: `[0, Int32.max)`.
    public mutating func next() -> Int32 { muestraInterna() }

    /// `Next(max)`: `(int)(Sample() * max)`, por truncamiento. `max < 0` lanza en .NET.
    public mutating func next(_ maximo: Int32) throws(ErrorDominio) -> Int32 {
        guard maximo >= 0 else {
            throw ErrorDominio(.argumentoFueraDeRango, "'maxValue' debe ser mayor que cero.", origen: "Random.Next")
        }
        return Int32(muestra() * Double(maximo))
    }

    /// `Next(min, max)`: rangos de más de `Int32.max` con la muestra de rango grande. `min > max` lanza en .NET.
    public mutating func next(_ minimo: Int32, _ maximo: Int32) throws(ErrorDominio) -> Int32 {
        guard minimo <= maximo else {
            throw ErrorDominio(.argumentoFueraDeRango, "'minValue' no puede ser mayor que maxValue.", origen: "Random.Next")
        }
        let rango = Int64(maximo) - Int64(minimo)
        if rango <= Int64(Int32.max) { return Int32(muestra() * Double(rango)) + minimo }
        return Int32(Int64(muestraRangoGrande() * Double(rango)) + Int64(minimo))
    }

    /// `NextDouble()`.
    public mutating func nextDouble() -> Double { muestra() }
}
