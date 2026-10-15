// .NET 8: dotnet/runtime release/8.0, ArraySortHelper.cs (IntrospectiveSort) y SortUtils.MoveNansToFront (DT-15).

/// `Array.Sort` / `List<T>.Sort` de .NET 8. No es estable: el orden de los empates es el de .NET y forma parte
/// de la salida (ESPEC_16 §6, golden `net/colecciones`).
public enum IntrosortNET {
    /// Con `Comparison<T>` / `IComparer<T>`: `comparar(a, b)` < 0, 0 o > 0.
    public static func ordenar<T>(_ a: inout [T], por comparar: (T, T) -> Int) {
        guard a.count > 1 else { return }
        introSort(&a, 0, a.count, 2 * (Int.bitWidth - a.count.leadingZeroBitCount), comparar)
    }

    /// Comparador por defecto de enteros.
    public static func ordenar<T: BinaryInteger>(_ a: inout [T]) {
        ordenar(&a) { $0 < $1 ? -1 : ($0 > $1 ? 1 : 0) }
    }

    /// `double[]`/`float[]` con el comparador por defecto: primero los `NaN` (en el orden que deja el intercambio)
    /// y después el resto ordenado.
    public static func ordenar<T: BinaryFloatingPoint>(_ a: inout [T]) {
        var nan = 0
        for i in a.indices where a[i].isNaN {
            a.swapAt(nan, i)
            nan += 1
        }
        guard a.count - nan > 1 else { return }
        let n = a.count - nan
        introSort(&a, nan, n, 2 * (Int.bitWidth - n.leadingZeroBitCount)) { $0 < $1 ? -1 : ($0 > $1 ? 1 : 0) }
    }

    /// `IntroSort(keys[lo..<lo+tamano], depthLimit)`.
    private static func introSort<T>(_ a: inout [T], _ lo: Int, _ tamano: Int, _ profundidad: Int, _ cmp: (T, T) -> Int) {
        var tamano = tamano
        var profundidad = profundidad
        while tamano > 1 {
            if tamano <= 16 {
                if tamano == 2 {
                    cambiarSiMayor(&a, lo, lo + 1, cmp)
                } else if tamano == 3 {
                    cambiarSiMayor(&a, lo, lo + 1, cmp)
                    cambiarSiMayor(&a, lo, lo + 2, cmp)
                    cambiarSiMayor(&a, lo + 1, lo + 2, cmp)
                } else {
                    insercion(&a, lo, tamano, cmp)
                }
                return
            }
            if profundidad == 0 {
                heapSort(&a, lo, tamano, cmp)
                return
            }
            profundidad -= 1
            let p = particion(&a, lo, tamano, cmp)
            introSort(&a, lo + p + 1, tamano - (p + 1), profundidad, cmp)
            tamano = p
        }
    }

    private static func cambiarSiMayor<T>(_ a: inout [T], _ i: Int, _ j: Int, _ cmp: (T, T) -> Int) {
        if cmp(a[i], a[j]) > 0 { a.swapAt(i, j) }
    }

    /// `PickPivotAndPartition`: mediana de tres con el pivote en `hi − 1`. Los topes `left < hi − 1` y `right > 0` no
    /// cambian nada con un comparador coherente y evitan salirse del array con uno incoherente.
    private static func particion<T>(_ a: inout [T], _ lo: Int, _ tamano: Int, _ cmp: (T, T) -> Int) -> Int {
        let hi = tamano - 1
        let medio = hi >> 1
        cambiarSiMayor(&a, lo, lo + medio, cmp)
        cambiarSiMayor(&a, lo, lo + hi, cmp)
        cambiarSiMayor(&a, lo + medio, lo + hi, cmp)
        let pivote = a[lo + medio]
        a.swapAt(lo + medio, lo + hi - 1)
        var izquierda = 0
        var derecha = hi - 1
        while izquierda < derecha {
            repeat { izquierda += 1 } while izquierda < hi - 1 && cmp(a[lo + izquierda], pivote) < 0
            repeat { derecha -= 1 } while derecha > 0 && cmp(pivote, a[lo + derecha]) < 0
            if izquierda >= derecha { break }
            a.swapAt(lo + izquierda, lo + derecha)
        }
        if izquierda != hi - 1 { a.swapAt(lo + izquierda, lo + hi - 1) }
        return izquierda
    }

    private static func heapSort<T>(_ a: inout [T], _ lo: Int, _ n: Int, _ cmp: (T, T) -> Int) {
        var i = n >> 1
        while i >= 1 {
            bajar(&a, lo, i, n, cmp)
            i -= 1
        }
        i = n
        while i > 1 {
            a.swapAt(lo, lo + i - 1)
            bajar(&a, lo, 1, i - 1, cmp)
            i -= 1
        }
    }

    private static func bajar<T>(_ a: inout [T], _ lo: Int, _ inicio: Int, _ n: Int, _ cmp: (T, T) -> Int) {
        var i = inicio
        let d = a[lo + i - 1]
        while i <= n >> 1 {
            var hijo = 2 * i
            if hijo < n && cmp(a[lo + hijo - 1], a[lo + hijo]) < 0 { hijo += 1 }
            if !(cmp(d, a[lo + hijo - 1]) < 0) { break }
            a[lo + i - 1] = a[lo + hijo - 1]
            i = hijo
        }
        a[lo + i - 1] = d
    }

    private static func insercion<T>(_ a: inout [T], _ lo: Int, _ n: Int, _ cmp: (T, T) -> Int) {
        for i in 0..<(n - 1) {
            let t = a[lo + i + 1]
            var j = i
            while j >= 0 && cmp(t, a[lo + j]) < 0 {
                a[lo + j + 1] = a[lo + j]
                j -= 1
            }
            a[lo + j + 1] = t
        }
    }
}
