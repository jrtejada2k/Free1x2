// Acceso a arrays con la semántica de C#: fuera de rango lanza `IndexOutOfRangeException` en vez de abortar (DT-03).

extension Array {
    /// `a[i]`.
    @inline(__always)
    public func leer(_ i: Int, _ origen: @autoclosure () -> String) throws(ErrorDominio) -> Element {
        guard i >= 0 && i < count else { throw .fueraDeRango(origen()) }
        return self[i]
    }

    /// `lista[i]` de un `List<T>`: fuera de rango lanza `ArgumentOutOfRangeException`.
    @inline(__always)
    public func leerLista(_ i: Int, _ origen: @autoclosure () -> String) throws(ErrorDominio) -> Element {
        guard i >= 0 && i < count else { throw .fueraDeRangoLista(origen()) }
        return self[i]
    }

    /// `a[i] = valor`.
    @inline(__always)
    public mutating func escribir(_ valor: Element, en i: Int, _ origen: @autoclosure () -> String) throws(ErrorDominio) {
        guard i >= 0 && i < count else { throw .fueraDeRango(origen()) }
        self[i] = valor
    }

    /// `new T[n]` con `n` negativo lanza `OverflowException`.
    public static func net(repeating valor: Element, count n: Int, _ origen: @autoclosure () -> String) throws(ErrorDominio) -> [Element] {
        guard n >= 0 else { throw .desbordamiento(origen()) }
        return Array(repeating: valor, count: n)
    }
}
