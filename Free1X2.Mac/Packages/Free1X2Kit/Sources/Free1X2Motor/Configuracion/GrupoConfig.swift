import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/Grupo.cs (01 §2.4) — la parte configurable de un grupo.

/// Las condiciones de los 14 filtros de un grupo (orden fijo de `Grupo.filtros`).
public struct CondicionesGrupo: Sendable, Hashable {
    public var noVariantes: CondicionNoVariantes
    public var valoracionSignos: CondicionValoracionSignos
    public var signosSeguidos: CondicionSignosSeguidos
    public var interrupciones: CondicionInterrupciones
    public var dibujos = CondicionDibujos()
    public var pesosNumericos: CondicionPesosNumericos
    public var distancias: CondicionDistancias
    public var gruposEquipos = CondicionGruposEquipos()
    public var contactos: CondicionContactos
    public var formatosSignos = CondicionFormatosSignos()
    public var colProbables = CondicionColProbables()
    public var formatos123: CondicionFormatos123
    public var simetrias = CondicionSimetrias()
    public var diferencias = CondicionDiferencias()

    public init(numeroPartidos n: Int) {
        noVariantes = CondicionNoVariantes(numeroPartidos: n)
        valoracionSignos = CondicionValoracionSignos(numeroPartidos: n)
        signosSeguidos = CondicionSignosSeguidos(numeroPartidos: n)
        interrupciones = CondicionInterrupciones(numeroPartidos: n)
        pesosNumericos = CondicionPesosNumericos(numeroPartidos: n)
        distancias = CondicionDistancias(numeroPartidos: n)
        contactos = CondicionContactos(numeroPartidos: n)
        formatos123 = CondicionFormatos123(numeroPartidos: n)
    }

    /// `EsActivo`: algún filtro activo.
    public var hayActivas: Bool {
        noVariantes.activa || valoracionSignos.activa || signosSeguidos.activa || interrupciones.activa || dibujos.activa
            || pesosNumericos.activa || distancias.activa || gruposEquipos.activa || contactos.activa
            || formatosSignos.activa || colProbables.activa || formatos123.activa || simetrias.activa || diferencias.activa
    }
}

/// Un grupo de partidos (el 0 es el boleto base) con sus filtros, tolerancias y filtro parcial.
public struct GrupoConfig: Sendable, Hashable {
    public var nombreGrupo = ""
    /// `EsGrupoBase = true` también activa `todosPartidosActivos`; `false` no lo revierte (:565-578).
    public var esGrupoBase = false {
        didSet { if esGrupoBase { todosPartidosActivos = true } }
    }
    public internal(set) var todosPartidosActivos = false
    public private(set) var partidosActivos: [Bool]
    /// Valor inicial 7 (solo el partido 1) hasta `calcularMascara` [PRESERVAR].
    public private(set) var mascara: Columna = 7
    public var condiciones: CondicionesGrupo
    public var tolerancias = ControladorTolConfig()
    /// `ArchivoFiltroGrupo`: `""` = sin filtro parcial.
    public var archivoFiltro = ""
    public let numeroPartidos: Int

    public init(numeroPartidos: Int) {
        self.numeroPartidos = numeroPartidos
        partidosActivos = Array(repeating: false, count: numeroPartidos)
        condiciones = CondicionesGrupo(numeroPartidos: numeroPartidos)
    }

    /// `Partidos = v`: asigna y recalcula la máscara.
    public mutating func ponerPartidos(_ partidos: [Bool]) {
        partidosActivos = partidos
        calcularMascara()
    }

    /// `CalcularMascara` (:69-89): el partido `i` ocupa los bits `3i…3i+2`.
    public mutating func calcularMascara() {
        var m: Columna = 0
        for activo in partidosActivos.reversed() { m = (m &<< 3) | (activo ? 7 : 0) }
        mascara = m
    }

    /// `PonerPartidosActivos("1,3,5")` (:470-493): acumula, no recalcula la máscara; `todosPartidosActivos` compara el nº
    /// de trozos con N. `0` o no numérico lanzan; valores mayores que N se ignoran.
    public mutating func ponerPartidosActivos(_ texto: String) throws(ErrorDominio) {
        let partes = trozos(texto, ",")
        todosPartidosActivos = partes.count == numeroPartidos
        for parte in partes {
            let n = Int(try FormatoNET.convertToInt32(parte))
            if n <= partidosActivos.count { try partidosActivos.escribir(true, en: n - 1, "Grupo.cs:488") }
        }
    }

    /// `ObtenPartidosActivos`: `"1,3,5"`.
    public var textoPartidosActivos: String {
        partidosActivos.indices.filter { partidosActivos[$0] }.map { String($0 + 1) }.joined(separator: ",")
    }

    public var hayPartidosActivos: Bool { partidosActivos.contains(true) }
}
