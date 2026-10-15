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
        return ponerPremios(try escrutarColumna(ganadora, columnas), 1, ganadora, archivo)
    }

    /// `EscrutaCombConTemporada` (:117-137): una fila por ganadora. Cancelar (`PararEscrutinio`) corta entre jornadas y
    /// devuelve las filas hechas (DEC-509). El C# relee el fichero en cada jornada; aquí se lee una vez (ESPEC_05 §4.1.6).
    public mutating func escrutarTemporada(ganadoras: [String], columnas: [String], archivo: String) throws(ErrorDominio)
        -> [FilaEscrutinio]
    {
        var filas: [FilaEscrutinio] = []
        for (i, ganadora) in ganadoras.enumerated() {
            numJornada = i + 1
            if Task.isCancelled { break }
            let ganadora = Self.xMayuscula(ganadora)
            filas.append(ponerPremios(try escrutarColumna(ganadora, columnas), i + 1, ganadora, archivo))
        }
        return filas
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
    /// signos que tienen las dos; las líneas en blanco cuentan (0 aciertos).
    private mutating func escrutarColumna(_ ganadora: String, _ columnas: [String]) throws(ErrorDominio) -> [Int] {
        var nc = Array(repeating: 0, count: numeroPartidos + 1)
        let g = Array(Self.xMayuscula(ganadora).utf16)
        for (i, columna) in columnas.enumerated() {
            let col = Self.xMayuscula(columna)
            let c = Array(col.utf16)
            var na = 0
            for k in 0..<min(c.count, g.count) where g[k] == 0x2A || g[k] == c[k] { na += 1 }
            if anadirAGanadoras { anadirAGanadora(col, i + 1, na) }
            guard na < nc.count else {
                throw ErrorDominio(.indiceFueraDeRango, "Índice fuera de los límites de la matriz.", origen: "Escrutador.cs:199")
            }
            nc[na] += 1
        }
        return nc
    }

    /// `AñadeAGanadoras` + `ObtenBoleto` (:205-226): 8 apuestas por boleto.
    private mutating func anadirAGanadora(_ columna: String, _ numCol: Int, _ aciertos: Int) {
        guard premios.contains(aciertos) else { return }
        listaPremiadas.append(
            ColumnaPremiada(
                columna: columna, fichero: archivoColumnas, jornada: numJornada, premio: aciertos, noColumna: numCol,
                noBoleto: (numCol + 7) / 8))
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

    /// `Replace('x', 'X')`: solo esa letra.
    private static func xMayuscula(_ s: String) -> String {
        s.contains("x") ? String(decoding: s.utf16.map { $0 == 0x78 ? 0x58 : $0 }, as: UTF16.self) : s
    }

    /// `Path.GetFileName`: tras la última `\` o `/`.
    static func nombreDeArchivo(_ ruta: String) -> String {
        guard let i = ruta.utf16.lastIndex(where: { $0 == 0x5C || $0 == 0x2F }) else { return ruta }
        return String(ruta[ruta.utf16.index(after: i)...])
    }
}
