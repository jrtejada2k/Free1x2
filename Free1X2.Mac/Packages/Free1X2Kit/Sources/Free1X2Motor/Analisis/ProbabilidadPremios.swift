import Free1X2Base

// C#: Free1X2.Domain/Analisis/Analizador.cs y ProbabilidadesPremios.cs (06 §3.1-§3.2). `IteraCols` y `premios[]` del
// analizador son código muerto (DP-08).

/// Recuentos por categoría (0…16) y su porcentaje sobre las columnas de la madre.
public struct ProbabilidadesPremios: Sendable, Hashable {
    public internal(set) var premios = [Int](repeating: 0, count: 17)
    public internal(set) var probabilidades = [Double](repeating: 0, count: 17)
    public internal(set) var noColumnasBase = 0
    /// Líneas de la hija (con repetidas y en blanco); se guarda y no se usa.
    public internal(set) var noColumnasComb = 0
    /// «Los Dos archivos tienen un número distinto de signos»: el cálculo sigue y da NaN en las 17 categorías.
    public internal(set) var errorSignos = false

    /// `CalculaProbabilidades` (:49-54): `premios·100 / base` (0/0 = NaN).
    mutating func calculaProbabilidades() {
        for i in 0..<17 { probabilidades[i] = Double(premios[i]) * 100.0 / Double(noColumnasBase) }
    }

    /// `ObtenProbabilidadPremioDirecto(c)`.
    public func probabilidad(_ categoria: Int) throws(ErrorDominio) -> Double {
        try probabilidades.leer(categoria, "ProbabilidadesPremios.cs:69")
    }
}

public enum ProbabilidadPremios {
    static let pot = (0...16).map { Int(ConvertidorDeBases.potenciasDe3[$0]) }

    /// `ComparaCombinaciones(madre, hija)` (:73-131): para cada columna de la madre, las categorías en las que tiene
    /// alguna hija buscando por vecindad hasta `N − premioMinimo` cambios; una hija encontrada **poda** su rama (P1) y
    /// los vecinos a distancia 1 se miran siempre (P2). Líneas ya leídas con `LeeColumnaSinComas`.
    public static func comparar(madre: [String], hija: [String], premioMinimo: Int) throws(ErrorDominio)
        -> ProbabilidadesPremios
    {
        var r = ProbabilidadesPremios()
        let a = madre.first?.utf16.count ?? 0
        guard a == hija.first?.utf16.count ?? 0 else {
            r.errorSignos = true
            r.calculaProbabilidades()
            return r
        }
        let o = "Analizador.cs:80-159"
        var bits = [Bool](repeating: false, count: try pot.leer(a, o))
        let conv = ConvertidorDeBases(numeroPartidos: a)
        for l in hija {
            let k = Int(try conv.numero(l))
            guard bits.indices.contains(k) else { throw .fueraDeRango(o) }
            bits[k] = true
        }
        // #11: la madre se convierte en orden, como antes (el primer error salta en la misma línea), y cada columna se
        // busca sola (solo lee `bits`) por trozos en varios núcleos. Se suman enteros: lo mismo en cualquier orden.
        var nums: [Int] = []
        nums.reserveCapacity(madre.count)
        for l in madre {
            let num = Int(try conv.numero(l))
            guard bits.indices.contains(num) else { throw .fueraDeRango(o) }
            nums.append(num)
        }
        let profundidad = a - premioMinimo
        let (columnas, mapa) = (nums, bits)
        let trozos = trozosParalelos(columnas.count)
        let parciales = enParalelo(trozos.count) { t in contar(columnas[trozos[t]], mapa, a, profundidad) }
        for p in parciales { for i in 0..<17 { r.premios[i] += p[i] } }
        r.noColumnasBase = nums.count
        r.noColumnasComb = hija.count
        r.calculaProbabilidades()
        return r
    }

    /// Lo que cuenta un trozo de la madre: por categoría, cuántas de sus columnas tienen alguna hija en ella.
    private static func contar(_ cols: ArraySlice<Int>, _ bits: [Bool], _ a: Int, _ profundidad: Int) -> [Int] {
        var premios = [Int](repeating: 0, count: 17)
        var temp = [Int](repeating: 0, count: 17)
        /// `EncontrarDistantes1` (:133-159).
        func distantes(_ idx: Int, _ desde: Int, _ nivel: Int) {
            for partido in stride(from: desde, to: a, by: 1) {
                let p = pot[partido]
                let sig = (idx / p) % 3
                for z in 0..<3 where z != sig {
                    let j = idx + p * (z - sig)
                    if bits[j] {
                        temp[a - nivel] += 1
                    } else if nivel < profundidad {
                        distantes(j, partido + 1, nivel + 1)
                    }
                }
            }
        }
        for num in cols {
            for i in temp.indices { temp[i] = 0 }
            if bits[num] { temp[a] += 1 } else { distantes(num, 0, 1) }
            for i in 0..<17 where temp[i] != 0 { premios[i] += 1 }
        }
        return premios
    }
}
