extension Array {
    /// Elemento si el índice existe (lecturas de la UI que Windows protegía con `Count`).
    public subscript(safe i: Int) -> Element? { indices.contains(i) ? self[i] : nil }
}
