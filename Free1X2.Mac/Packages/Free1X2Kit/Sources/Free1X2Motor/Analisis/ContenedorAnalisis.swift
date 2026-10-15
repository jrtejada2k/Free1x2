import Free1X2Base

// C#: Free1X2.Domain/Analisis/ContenedorAnalisisGlobal.cs, ContenedorAnalisis.cs y los contenedores simples
// (06 §3.4-§3.6). Resultado de las rutas C/D: valores, no referencias (lo lee el visor en otro actor).

/// Raíz del análisis: histogramas del grupo base y columnas por nº de fallos de grupos/conjuntos.
public struct ContenedorAnalisisGlobal: Sendable, Hashable {
    public internal(set) var analisisGrupos: ContenedorAnalisis
    /// Tamaño = nº de grupos.
    public internal(set) var columnasPorFallosDeGrupos: [Int]
    /// Tamaño = nº de **controles de grupos** (el C# llama `noConjuntos` al parámetro).
    public internal(set) var columnasPorFallosDeConjuntos: [Int]
    public let noGrupos: Int
    public let noConjuntos: Int
    public internal(set) var esAnalisisExterno = false

    init(noGrupos: Int, noConjuntos: Int, numeroPartidos: Int) {
        self.noGrupos = noGrupos
        self.noConjuntos = noConjuntos
        analisisGrupos = ContenedorAnalisis(numeroPartidos: numeroPartidos)
        columnasPorFallosDeGrupos = Array(repeating: 0, count: noGrupos)
        columnasPorFallosDeConjuntos = Array(repeating: 0, count: noConjuntos)
    }
}

/// Histogramas por condición de las columnas aceptadas (06 §3.5). Matrices `[fila][valor]`.
public struct ContenedorAnalisis: Sendable, Hashable {
    public let numeroPartidos: Int
    /// 3 × (N+1): variantes, equis, doses.
    public internal(set) var vx2: [[Int]]
    /// 4 × N: variantes, unos, equis, doses.
    public internal(set) var distancias: [[Int]]
    /// 10 × (N+1), en el orden de `CondicionInterrupciones.Contador`.
    public internal(set) var interrupciones: [[Int]]
    /// 5 × 10: global, variantes, unos, equis, doses.
    public internal(set) var pesos: [[Int]]
    /// 4 × (N+1): variantes, unos, equis, doses.
    public internal(set) var seguidos: [[Int]]
    /// (N+1) × (N+1): `[equis][doses]`.
    public internal(set) var dibujos: [[Int]]
    /// 10 × N, en el orden de `CondicionContactos.Contacto`.
    public internal(set) var contactos: [[Int]]
    /// `SortedList<long,int>`: se leen ordenadas por clave (`ordenadas`).
    public internal(set) var figurasV: [Int64: Int] = [:]
    public internal(set) var figuras1: [Int64: Int] = [:]
    public internal(set) var figurasX: [Int64: Int] = [:]
    public internal(set) var figuras2: [Int64: Int] = [:]
    public internal(set) var figurasContactos: [Int64: Int] = [:]
    public internal(set) var figurasPesos: [Int64: Int] = [:]
    public internal(set) var valoracionGlobal: [Int] = []
    public internal(set) var valoracionUnos: [Int] = []
    public internal(set) var valoracionEquis: [Int] = []
    public internal(set) var valoracionDoses: [Int] = []
    public internal(set) var noValoresValoracionGlobal: [Int] = []
    public internal(set) var noValoresValoracionUnos: [Int] = []
    public internal(set) var noValoresValoracionEquis: [Int] = []
    public internal(set) var noValoresValoracionDoses: [Int] = []
    public internal(set) var tipoValoracion = ""
    public internal(set) var noAciertosSimetrias: [Int] = []
    public internal(set) var simetrias: [ContenedorSimetrias] = []
    public internal(set) var diferencias: [ContenedorDiferencias] = []
    public internal(set) var columnasProbables: [ContenedorColumnasProbables] = []
    public internal(set) var formatosSignos: [ContenedorFormatos] = []
    public internal(set) var usaSimetrias = false
    public internal(set) var usaValoraciones = false
    public internal(set) var usaCPs = false
    public internal(set) var usaFormatos = false
    /// `true` por defecto, a diferencia de los demás (06 §3.5 P6).
    public internal(set) var usaDiferencias = true

    init(numeroPartidos n: Int) {
        numeroPartidos = n
        func m(_ filas: Int, _ columnas: Int) -> [[Int]] {
            Array(repeating: Array(repeating: 0, count: columnas), count: filas)
        }
        vx2 = m(3, n + 1)
        distancias = m(4, n)
        interrupciones = m(10, n + 1)
        pesos = m(5, 10)
        seguidos = m(4, n + 1)
        dibujos = m(n + 1, n + 1)
        contactos = m(10, n)
    }

    /// Pares de un `SortedList` en orden ascendente de clave.
    public static func ordenadas<K: Comparable>(_ d: [K: Int]) -> [(clave: K, valor: Int)] {
        d.sorted { $0.key < $1.key }.map { (clave: $0.key, valor: $0.value) }
    }

    /// `InicializaFormatosSignos` (:366-374): no vacía la lista previa.
    mutating func inicializaFormatosSignos(_ noFormatos: [Int]) {
        for n in noFormatos {
            formatosSignos.append(
                ContenedorFormatos(aciertosFormatosSignos: Array(repeating: 0, count: n + 1), aciertosGlobalesFormatos: [0: 0]))
        }
    }

    /// `InicializarSimetrias` (:376-383).
    mutating func inicializarSimetrias() {
        simetrias += noAciertosSimetrias.map { ContenedorSimetrias(aciertos: $0, columnas: 0) }
    }

    /// `InicializarValoraciones` (:385-403): con el **último** valor de cada lista; añade (dos llamadas duplican).
    mutating func inicializarValoraciones() throws(ErrorDominio) {
        let o = "ContenedorAnalisis.cs:385-403"
        valoracionGlobal += Array(
            repeating: 0, count: max(0, try noValoresValoracionGlobal.leerLista(noValoresValoracionGlobal.count - 1, o) + 1))
        valoracionUnos += Array(
            repeating: 0, count: max(0, try noValoresValoracionUnos.leerLista(noValoresValoracionUnos.count - 1, o) + 1))
        valoracionEquis += Array(
            repeating: 0, count: max(0, try noValoresValoracionEquis.leerLista(noValoresValoracionEquis.count - 1, o) + 1))
        valoracionDoses += Array(
            repeating: 0, count: max(0, try noValoresValoracionDoses.leerLista(noValoresValoracionDoses.count - 1, o) + 1))
    }

    /// `IncrementarColumnasSimetrias` (:412-422): el primero con esos aciertos.
    mutating func incrementarColumnasSimetrias(_ aciertos: Int) {
        if let i = simetrias.firstIndex(where: { $0.aciertos == aciertos }) { simetrias[i].columnas += 1 }
    }

    /// `IncrementarColumnasValoracion` (:424-435): `double → float → Math.Round` (a par) `→ int` (T3, T4).
    mutating func incrementarColumnasValoracion(_ g: Double, _ u: Double, _ x: Double, _ d: Double) throws(ErrorDominio) {
        let o = "ContenedorAnalisis.cs:430-433"
        func clave(_ v: Double) -> Int {
            // `(int)` de un valor no representable da `int.MinValue` en C#: índice negativo.
            Int(exactly: Double(Float(v)).rounded(.toNearestOrEven)) ?? Int(Int32.min)
        }
        try valoracionGlobal.incrementarLista(clave(g), o)
        try valoracionUnos.incrementarLista(clave(u), o)
        try valoracionEquis.incrementarLista(clave(x), o)
        try valoracionDoses.incrementarLista(clave(d), o)
    }
}

public struct ContenedorColumnasProbables: Sendable, Hashable {
    public internal(set) var noAC: [Int]
    public internal(set) var noACS: [Int]
    public internal(set) var noFS: [Int]

    init(partidos: Int) {
        noAC = Array(repeating: 0, count: partidos + 1)
        noACS = noAC
        noFS = noAC
    }
}

public struct ContenedorDiferencias: Sendable, Hashable {
    public internal(set) var variantes, equis, doses, dibujos, interrupciones, formatos: [Int]
    /// `n + 1`, no `n` (06 §3.6).
    public var numDiferenciasPosibles: Int { variantes.count }

    init(_ n: Int) {
        variantes = Array(repeating: 0, count: n + 1)
        (equis, doses, dibujos, interrupciones, formatos) = (variantes, variantes, variantes, variantes, variantes)
    }
}

public struct ContenedorFormatos: Sendable, Hashable {
    public internal(set) var aciertosFormatosSignos: [Int]
    /// `SortedList<int,int>`.
    public internal(set) var aciertosGlobalesFormatos: [Int: Int]
}

public struct ContenedorSimetrias: Sendable, Hashable {
    public internal(set) var aciertos: Int
    public internal(set) var columnas: Int
}

extension Array where Element == Int {
    /// `arr[i]++` de un `int[]`/`int[,]`.
    mutating func incrementar(_ i: Int, _ origen: @autoclosure () -> String) throws(ErrorDominio) {
        guard indices.contains(i) else { throw .fueraDeRango(origen()) }
        self[i] += 1
    }

    /// `lista[i]++` de un `List<int>`.
    mutating func incrementarLista(_ i: Int, _ origen: @autoclosure () -> String) throws(ErrorDominio) {
        guard indices.contains(i) else { throw .fueraDeRangoLista(origen()) }
        self[i] += 1
    }
}

extension Array where Element == [Int] {
    /// `m[f, c]++` de un `int[,]`.
    mutating func incrementar(_ f: Int, _ c: Int, _ origen: @autoclosure () -> String) throws(ErrorDominio) {
        guard indices.contains(f) else { throw .fueraDeRango(origen()) }
        try self[f].incrementar(c, origen())
    }
}

extension Dictionary where Value == Int {
    mutating func incrementar(_ k: Key) { self[k, default: 0] += 1 }
}
