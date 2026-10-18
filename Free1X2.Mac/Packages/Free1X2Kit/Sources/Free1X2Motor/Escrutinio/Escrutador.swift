import Free1X2Base

// C#: Free1X2.Domain/Escrutinio/Escrutador.cs y ColumnasPremiadas.cs (ESPEC_05 §4.1, §4.3).

/// `ColumnasPremiadas`: una columna con premio y dónde está.
public struct ColumnaPremiada: Sendable, Hashable, Codable {
    public var columna: String
    /// Ruta del fichero de columnas tal como se asignó a `archivoColumnas`.
    public var fichero: String
    public var jornada: Int
    public var premio: Int
    public var noColumna: Int
    public var noBoleto: Int

    public init(columna: String, fichero: String, jornada: Int, premio: Int, noColumna: Int, noBoleto: Int) {
        (self.columna, self.fichero, self.jornada, self.premio) = (columna, fichero, jornada, premio)
        (self.noColumna, self.noBoleto) = (noColumna, noBoleto)
    }
}

/// Fila de la tabla «Resultados» del escrutinio: `LineaID`, `Columna`, `Archivo`, `P0…PN`, «Ac. Totales».
public struct FilaEscrutinio: Sendable, Hashable, Codable {
    public var lineaID: Int
    public var columna: String
    public var archivo: String
    /// `P0 … P<numeroPartidos>`: nº de columnas con `i` aciertos.
    public var premios: [Int]
    public var aciertosTotales: String
    public var seleccionado = false
}

/// Escrutinio de un fichero de columnas contra una ganadora (o contra las ganadoras de una temporada).
/// `premiosTotales` y `listaPremiadas` acumulan entre llamadas, como en el C# (:51,57, peculiaridad 2).
/// Solo se portan las rutas que usa la app (DP-08): `PonerPremios` de 3 argumentos y `LeerPremios` quedan fuera.
public struct Escrutador: Sendable {
    public let numeroPartidos: Int
    /// Categorías que cuentan como premio para `listaPremiadas` (`Premios`).
    public var premios: [Int]
    /// Ruta del fichero de columnas (`ArchivoColumnas`); se copia en `ColumnaPremiada.fichero`.
    public var archivoColumnas: String
    public var anadirAGanadoras: Bool
    public private(set) var premiosTotales: [Int]
    public private(set) var listaPremiadas: [ColumnaPremiada] = []
    private var numJornada = 1

    /// `new Escrutador(int[])`: `numeroPartidos` se fija aquí (T10).
    public init(premios: [Int], numeroPartidos: Int, archivoColumnas: String = "", anadirAGanadoras: Bool = false) {
        self.numeroPartidos = numeroPartidos
        self.premios = premios
        self.archivoColumnas = archivoColumnas
        self.anadirAGanadoras = anadirAGanadoras
        premiosTotales = Array(repeating: 0, count: numeroPartidos + 1)
    }

    /// `EscrutaCombConColumna(ganadora, ds, archivo)` (:94-102). `columnas` = líneas del fichero ya leídas con
    /// `LeeColumnaSinComas` (`ArchivoColumnasTexto.columnas`); `archivo` = nombre que muestra la fila.
    public mutating func escrutarConColumna(_ ganadora: String, columnas: [String], archivo: String) throws(ErrorDominio)
        -> FilaEscrutinio
    {
        numJornada = 1
        return ponerPremios(try escrutarColumna(ganadora, Self.preparar(columnas)), 1, ganadora, archivo)
    }

    /// `EscrutaCombConTemporada` (:117-137): una fila por ganadora. Cancelar (`PararEscrutinio`) corta entre jornadas y
    /// devuelve las filas hechas (DEC-509). El C# relee el fichero en cada jornada; aquí se lee una vez (ESPEC_05 §4.1.6).
    /// `porJornada`: recibe las premiadas de cada ganadora, que salen de `listaPremiadas`, para llevarlas a disco: contra
    /// 1.000 ganadoras, 14 triples dan 19 millones de premiadas (3,7 GB en memoria).
    public mutating func escrutarTemporada(
        ganadoras: [String], columnas: [String], archivo: String,
        porJornada: (([ColumnaPremiada]) throws(ErrorDominio) -> Void)? = nil
    ) throws(ErrorDominio) -> [FilaEscrutinio] {
        var filas: [FilaEscrutinio] = []
        let columnas = Self.preparar(columnas)
        for (i, ganadora) in ganadoras.enumerated() {
            numJornada = i + 1
            if Task.isCancelled { break }
            let ganadora = Self.xMayuscula(ganadora)
            filas.append(ponerPremios(try escrutarColumna(ganadora, columnas), i + 1, ganadora, archivo))
            if let porJornada { try porJornada(sacarPremiadas()) }
        }
        return filas
    }

    /// Devuelve las premiadas acumuladas y vacía `listaPremiadas`.
    public mutating func sacarPremiadas() -> [ColumnaPremiada] {
        let p = listaPremiadas
        listaPremiadas.removeAll(keepingCapacity: true)
        return p
    }

    /// `AñadirPremiosGlobales` (:262-276): fila «TOTALES».
    public func filaTotales(_ globales: [Int]) throws(ErrorDominio) -> FilaEscrutinio {
        guard globales.count > numeroPartidos else {
            throw ErrorDominio(.indiceFueraDeRango, "Índice fuera de los límites de la matriz.", origen: "Escrutador.cs:270")
        }
        return FilaEscrutinio(
            lineaID: 0, columna: "----- TOTALES:", archivo: "-------------", premios: Array(globales[0...numeroPartidos]),
            aciertosTotales: " ")
    }

    /// `EscrutaApuestaMultiple` (:104-108): partidos de la ganadora cubiertos por la apuesta (columnas de 3 bits).
    public static func escrutarApuestaMultiple(_ apuesta: Columna, _ ganadora: Columna) -> Int {
        UtilColumnas.contarBitsA1(ganadora & apuesta)
    }

    /// `EscrutaColumna` (:159-203): histograma de aciertos. `*` en la ganadora acierta siempre; solo se comparan los
    /// signos que tienen las dos; las líneas en blanco cuentan (0 aciertos). En varios núcleos (DM-22): cada trozo de
    /// columnas cuenta y apunta sus premiadas por su cuenta, y se juntan en orden: lo mismo que en un hilo.
    private mutating func escrutarColumna(_ ganadora: String, _ columnas: Preparadas) throws(ErrorDominio) -> [Int] {
        let g = Array(Self.xMayuscula(ganadora).utf16)
        let trozos = trozosParalelos(columnas.count)
        let apuntar = anadirAGanadoras ? (premios: premios, fichero: archivoColumnas, jornada: numJornada) : nil
        let n = numeroPartidos
        let parciales = enParalelo(trozos.count) { t in Self.contar(trozos[t], columnas, g, n, apuntar) }
        var nc = Array(repeating: 0, count: numeroPartidos + 1)
        for p in parciales {
            if let e = p.error { throw e }
            for i in nc.indices { nc[i] += p.nc[i] }
            listaPremiadas += p.premiadas
        }
        return nc
    }

    /// Lo que cuenta un trozo de columnas.
    private struct Parcial: Sendable {
        var nc: [Int] = []
        var premiadas: [ColumnaPremiada] = []
        var error: ErrorDominio?
    }

    /// Cuenta un trozo. `AñadeAGanadoras` + `ObtenBoleto` (:205-226): con `apuntar`, la premiada (8 apuestas por boleto).
    /// Al primer índice fuera de rango para, como el C#.
    private static func contar(
        _ r: Range<Int>, _ cols: Preparadas, _ g: [UInt16], _ n: Int,
        _ apuntar: (premios: [Int], fichero: String, jornada: Int)?
    ) -> Parcial {
        var p = Parcial(nc: Array(repeating: 0, count: n + 1))
        cols.signos.withUnsafeBufferPointer { s in
            cols.inicios.withUnsafeBufferPointer { ini in
                g.withUnsafeBufferPointer { g in
                    for i in r {
                        let (a, b) = (ini[i], ini[i + 1])
                        var na = 0
                        for k in 0..<Swift.min(b - a, g.count) where g[k] == 0x2A || g[k] == s[a + k] { na += 1 }
                        if let apuntar, apuntar.premios.contains(na) {
                            p.premiadas.append(
                                ColumnaPremiada(
                                    columna: String(decoding: UnsafeBufferPointer(rebasing: s[a..<b]), as: UTF16.self),
                                    fichero: apuntar.fichero, jornada: apuntar.jornada, premio: na, noColumna: i + 1,
                                    noBoleto: (i + 8) / 8))
                        }
                        guard na < p.nc.count else {
                            p.error = ErrorDominio(
                                .indiceFueraDeRango, "Índice fuera de los límites de la matriz.", origen: "Escrutador.cs:199")
                            return
                        }
                        p.nc[na] += 1
                    }
                }
            }
        }
        return p
    }

    /// `PonerPremios(prems, n, col, archivo)` (:243-260).
    private mutating func ponerPremios(_ prems: [Int], _ linea: Int, _ columna: String, _ archivo: String) -> FilaEscrutinio {
        var total = 0
        for (i, p) in prems.enumerated() {
            premiosTotales[i] += p
            total += p * i
        }
        return FilaEscrutinio(
            lineaID: linea, columna: columna, archivo: Self.nombreDeArchivo(archivo), premios: prems, aciertosTotales: String(total))
    }

    /// Columnas con `Replace('x', 'X')` en UTF-16, una sola vez por escrutinio (no por ganadora), en un solo bloque: la
    /// `i` va de `inicios[i]` a `inicios[i + 1]`. Un array por columna eran 4,8 millones de objetos con 14 triples.
    struct Preparadas: Sendable {
        var signos: [UInt16] = []
        var inicios = [0]
        var count: Int { inicios.count - 1 }
    }

    private static func preparar(_ columnas: [String]) -> Preparadas {
        var p = Preparadas()
        p.signos.reserveCapacity(columnas.count * 14)
        p.inicios.reserveCapacity(columnas.count + 1)
        for c in columnas {
            for u in c.utf16 { p.signos.append(u == 0x78 ? 0x58 : u) }
            p.inicios.append(p.signos.count)
        }
        return p
    }

    /// `Replace('x', 'X')`: solo esa letra.
    private static func xMayuscula(_ s: String) -> String {
        s.contains("x") ? String(decoding: s.utf16.map { $0 == 0x78 ? 0x58 : $0 }, as: UTF16.self) : s
    }

    /// `Path.GetFileName`: tras la última `\` o `/`.
    public static func nombreDeArchivo(_ ruta: String) -> String {
        guard let i = ruta.utf16.lastIndex(where: { $0 == 0x5C || $0 == 0x2F }) else { return ruta }
        return String(ruta[ruta.utf16.index(after: i)...])
    }
}
