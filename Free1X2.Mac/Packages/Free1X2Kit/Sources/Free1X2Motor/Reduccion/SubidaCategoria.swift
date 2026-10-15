import Free1X2Base

// C#: Free1X2.Domain/SubirCategoria/Calculos.cs (05 §3). `SubirUnaCategoria` es código muerto: no se porta.

/// «Subir categoría»: todas las columnas que salen de la madre cambiando signos en los partidos involucrados, en los
/// niveles (nº de cambios) elegidos y con `seguidos` cambios consecutivos de la lista circular de involucrados.
/// Con estado, como el C#: `listaInvolucrados` (29 posiciones) conserva valores de la llamada anterior (05 §3 P2).
public struct SubidaCategoria: Sendable {
    /// Índices base 3 de la madre (con duplicados), leídos con los signos de la primera línea.
    public let columnas: [Int]
    /// «I=» al construir; «F=» (columnas distintas generadas) tras `calcular`.
    public var noColumnas: Int
    public private(set) var numeroPartidos = 0
    private var listaInvolucrados = [Int](repeating: 0, count: 29)
    /// `Bits`: 3^15 posiciones.
    private var bits: [Bool] = []

    static let tamano = 14_348_907
    static let pot = [
        1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177_147, 531_441, 1_594_323, 4_782_969, 14_348_907,
    ]

    /// `new Calculos(archivo)` (:53-71): `ConvColumnaANumero(línea, noSignos)`.
    public init(lineas: [String]) throws(ErrorDominio) {
        let n = lineas.first?.utf16.count ?? 0
        let conv = ConvertidorDeBases(numeroPartidos: n)
        var c: [Int] = []
        for l in lineas { c.append(Int(try conv.numero(l, partidos: n))) }
        columnas = c
        noColumnas = c.count
    }

    /// `Calcular(involucrados, niveles, seguidos, ficheroExternas, noPartidos)` (:116-254). `externas` = columnas del
    /// fichero de externas (`nil` = sin fichero: todas admitidas). Índices de partido 0-based; `niveles[k]` = k cambios.
    public mutating func calcular(
        involucrados: [Bool], niveles: [Bool], seguidos: Int, externas: [String]?, numeroPartidos noPartidos: Int
    ) throws(ErrorDominio) {
        let o = "Calculos.cs:116-254"
        var invariante = [Bool](repeating: false, count: noPartidos)
        numeroPartidos = noPartidos
        var admitidas = [Bool](repeating: externas == nil, count: Self.tamano)
        for l in externas ?? [] {
            let k = Int(try ConvertidorDeBases(numeroPartidos: noPartidos).numero(l))
            guard admitidas.indices.contains(k) else { throw .fueraDeRangoLista("Calculos.cs:178") }
            admitidas[k] = true
        }
        var num = 0
        for (i, v) in involucrados.enumerated() where v {
            try listaInvolucrados.escribir(i, en: num, o)
            num += 1
        }
        var i = 0
        while i < seguidos + 1 {
            try listaInvolucrados.escribir(try listaInvolucrados.leer(i, o), en: num + i, o)
            i += 1
        }
        var profMaxima = noPartidos + 1
        repeat { profMaxima -= 1 } while !(try niveles.leer(profMaxima, o))
        // Estado local que se vuelca al salir, también si se lanza a medias (como los campos del C#).
        var cuenta = 0
        var b = [Bool](repeating: false, count: Self.tamano)
        defer {
            noColumnas = cuenta
            bits = b
        }
        let lista = listaInvolucrados

        func marcar(_ idx: Int) throws(ErrorDominio) {
            guard b.indices.contains(idx) else { throw .fueraDeRangoLista("Calculos.cs:205") }
            if !b[idx] && admitidas[idx] {
                b[idx] = true
                cuenta += 1
            }
        }
        func poner(_ p: Int) throws(ErrorDominio) {
            guard invariante.indices.contains(p) else { throw .fueraDeRangoLista("Calculos.cs:190") }
            invariante[p] = true
        }
        /// `SubirUnaCategoriaExceptoSeguidos` (:221-254): recursa **antes** de marcar.
        func excepto(_ ini: Int, _ desde: Int, _ nivel: Int) throws(ErrorDominio) {
            for p in stride(from: desde, to: noPartidos, by: 1) {
                guard try involucrados.leer(p, o), !invariante[p] else { continue }
                let pot = try Self.pot.leer(p, o)
                let sig = (ini / pot) % 3
                for z in 0..<3 where z != sig {
                    let idx = ini + pot * (z - sig)
                    if nivel < profMaxima { try excepto(idx, p + 1, nivel + 1) }
                    if try niveles.leer(nivel, o) { try marcar(idx) }
                }
            }
        }
        /// `SubirUnaCategoriaSeguidos` (:184-220): la cadena de `seguidos` partidos y, al final, las derivadas.
        func encadenar(_ ini: Int, _ i: Int, _ nivel: Int) throws(ErrorDominio) {
            let partido = try lista.leer(i, o)
            try poner(partido)
            let pot = try Self.pot.leer(partido, o)
            let sig = (ini / pot) % 3
            for z in 0..<3 where z != sig {
                let idx = ini + pot * (z - sig)
                if nivel < seguidos {
                    try encadenar(idx, i + 1, nivel + 1)
                } else {
                    try excepto(idx, 0, nivel + 1)
                    if try niveles.leer(nivel, o) { try marcar(idx) }
                }
            }
        }

        for c in columnas {
            if try niveles.leer(0, o) { try marcar(c) }
            for n in 0..<num {
                for k in invariante.indices { invariante[k] = false }
                try poner(lista[n])
                try encadenar(c, n, 1)
            }
        }
    }

    /// `Grabar` (:73-79): en orden de índice, con anchura `numeroPartidos`; vacía ⇒ no se crea el fichero.
    public func lineas() throws(ErrorDominio) -> [String] {
        let conv = ConvertidorDeBases(numeroPartidos: numeroPartidos)
        var r: [String] = []
        for i in bits.indices where bits[i] { r.append(try conv.columna(Int64(i))) }
        return r
    }
}
