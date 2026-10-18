import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/CondicionIfThen.cs, ControladorIfThen.cs y GrupoIfThen.cs (01 §2.16-2.18).

/// `CondicionIfThen` en ejecución: los dos filtros se construyen en la primera columna (P-01), no antes.
final class CondicionIfThen {
    let config: CondicionIfThenConfig
    let numeroPartidos: Int
    private var filtroIf: (any FiltroColumna)?
    private var filtroThen: (any FiltroColumna)?

    init(_ c: CondicionIfThenConfig, numeroPartidos: Int) {
        config = c
        self.numeroPartidos = numeroPartidos
    }

    /// `CompruebaPronostico` (:284-298): si el «If» no se cumple, la condición se cumple.
    func comprueba(_ columna: Columna) throws(ErrorDominio) -> Bool {
        var valido = try obtenIf().analizar(columna)
        if config.noIf { valido = !valido }
        if !valido { return true }
        valido = try obtenThen().analizar(columna)
        if config.noThen { valido = !valido }
        return valido
    }

    /// `ObtenFiltroIf` / `ObtenFiltroThen` (:263-282): se construye una vez y se reutiliza (cada filtro reinicia sus contadores).
    private func obtenIf() throws(ErrorDominio) -> any FiltroColumna {
        if let f = filtroIf { return f }
        let f = try Self.filtro(config.condIf, numeroPartidos)
        filtroIf = f
        return f
    }

    private func obtenThen() throws(ErrorDominio) -> any FiltroColumna {
        if let f = filtroThen { return f }
        let f = try Self.filtro(config.condThen, numeroPartidos)
        filtroThen = f
        return f
    }

    /// `getFiltro(texto)` (:95-250): ramas en este orden; texto no reconocido ⇒ `NullReferenceException` (:247).
    static func filtro(_ texto: String, _ n: Int) throws(ErrorDominio) -> any FiltroColumna {
        if texto.contains("Cantidad") {
            let valor = try ultimaPalabra(texto)
            var c = CondicionNoVariantes(numeroPartidos: n)
            if texto.contains("Variantes") {
                try c.setNoVariantes(Int(try FormatoNET.convertToInt16(valor)), true)
            } else if texto.contains("X") {
                try c.setNoEquis(Int(try FormatoNET.convertToInt16(valor)), true)
            } else if texto.contains("2") {
                try c.setNoDoses(Int(try FormatoNET.convertToInt16(valor)), true)
            }
            try c.llenarTodosValores()
            c.activa = true
            c.contieneDatos = true
            return FiltroNoVariantes(c, analisisActivo: false)
        }
        if texto.contains("Dibujo") {
            // `Dibujos.Add(valor)` no rehace la tabla: el filtro rechaza siempre (DEC-306) [PRESERVAR].
            var c = CondicionDibujos()
            c.dibujos.append(try ultimaPalabra(texto))
            c.activa = true
            c.contieneDatos = true
            return FiltroDibujos(c, analisisActivo: false)
        }
        if texto.contains("Signos Seguidos") {
            let nombre = try nombreCondicion(texto)
            let valor = try ultimaPalabra(texto)
            var c = CondicionSignosSeguidos(numeroPartidos: n)
            let opciones: [(String, CondicionSignosSeguidos.Clase)] = [
                ("Variantes", .variantes), ("1", .unos), ("X", .equis), ("2", .doses),
            ]
            if let clase = Self.primera(nombre, opciones) {
                try c.set(clase, valor)
            }
            try c.llenarTodosValores()
            c.activa = true
            c.contieneDatos = true
            return FiltroSignosSeguidos(c, analisisActivo: false)
        }
        if texto.contains("Interrupciones") {
            let nombre = try nombreCondicion(texto)
            let valor = try ultimaPalabra(texto)
            var c = CondicionInterrupciones(numeroPartidos: n)
            typealias K = CondicionInterrupciones.Contador
            let opciones: [(String, K)] =
                nombre.contains("Seguidas")
                ? [("Globales", .globalSeg), ("Variantes", .variantesSeg), ("1", .unosSeg), ("X", .equisSeg), ("2", .dosesSeg)]
                : [("Globales", .global), ("Variantes", .variantes), ("1", .unos), ("X", .equis), ("2", .doses)]
            if let k = Self.primera(nombre, opciones) { try c.set(k, valor) }
            try c.llenarTodosValores()
            c.activa = true
            c.contieneDatos = true
            return FiltroInterrupciones(c, analisisActivo: false)
        }
        if texto.contains("Peso numérico") {
            // Sin `PonerTolerancia`: `tolerancias[0]` es falso y el filtro rechaza siempre (DEC-203) [PRESERVAR].
            let nombre = try nombreCondicion(texto)
            let valor = try ultimaPalabra(texto)
            var c = CondicionPesosNumericos(numeroPartidos: n)
            typealias P = CondicionPesosNumericos.Peso
            let opciones: [(String, P)] = [("Global", .global), ("Variantes", .variantes), ("1", .unos), ("X", .equis), ("2", .doses)]
            if let p = Self.primera(nombre, opciones) { try c.setPN(p, valor) }
            try c.llenarTodosValores()
            c.activa = true
            c.contieneDatos = true
            return FiltroPesosNumericos(c, analisisActivo: false)
        }
        if texto.contains("Distancia") {
            let nombre = try nombreCondicion(texto)
            let valor = try ultimaPalabra(texto)
            var c = CondicionDistancias(numeroPartidos: n)
            if nombre.contains("Variantes") {
                try c.setDistVariantes(valor)
            } else if nombre.contains("1") {
                try c.setDistUnos(valor)
            } else if nombre.contains("X") {
                try c.setDistEquis(valor)
            } else if nombre.contains("2") {
                try c.setDistDoses(valor)
            }
            try c.llenarTodosValores()
            c.activa = true
            c.contieneDatos = true
            return FiltroDistancias(c, analisisActivo: false)
        }
        if texto.contains("Contactos") {
            let nombre = try nombreCondicion(texto)
            let valor = try ultimaPalabra(texto)
            var c = CondicionContactos(numeroPartidos: n)
            let opciones = CondicionContactos.Contacto.allCases.map { ($0.par, $0) }
            if let k = Self.primera(nombre, opciones) { try c.set(k, valor) }
            try c.llenarTodosValores()
            c.activa = true
            c.contieneDatos = true
            return FiltroContactos(c, analisisActivo: false)
        }
        throw .referenciaNula("CondicionIfThen.cs:247")
    }

    /// Primera opción cuyo texto aparece en `nombre` (cadena de `else if` del C#).
    static func primera<T>(_ nombre: String, _ opciones: [(String, T)]) -> T? {
        opciones.first { nombre.contains($0.0) }?.1
    }

    /// `texto.Substring(texto.LastIndexOf(" ")).Trim()`; sin espacio ⇒ `Substring(-1)` lanza.
    static func ultimaPalabra(_ texto: String) throws(ErrorDominio) -> String {
        guard let i = texto.utf16.lastIndex(of: 0x20) else {
            throw ErrorDominio(
                .argumentoFueraDeRango, "El índice de inicio no puede ser menor que cero.", origen: "CondicionIfThen.cs:104")
        }
        return FormatoNET.recortar(String(texto[i...]))
    }

    /// `texto.Substring(0, LastIndexOf(" ") - 1)`: el nombre sin el `:` final.
    static func nombreCondicion(_ texto: String) throws(ErrorDominio) -> String {
        guard let i = texto.utf16.lastIndex(of: 0x20), texto.utf16.distance(from: texto.utf16.startIndex, to: i) >= 1 else {
            throw ErrorDominio(.argumentoFueraDeRango, "La longitud no puede ser menor que cero.", origen: "CondicionIfThen.cs:126")
        }
        return String(texto[..<texto.utf16.index(before: i)])
    }
}

/// `ControladorIfThen` en ejecución.
final class ControladorIfThen {
    let condiciones: [CondicionIfThen]
    let grupos: [GrupoIfThenConfig]
    let aciertoCondiciones: [Bool]?
    let aciertoGrupos: [Bool]?
    let activo: Bool
    let esVacio: Bool

    init(_ c: IfThenConfig, numeroPartidos: Int) {
        condiciones = c.condiciones.map { CondicionIfThen($0, numeroPartidos: numeroPartidos) }
        // Arrays propios (`map` copia): los de la combinación los comparten todos los hilos de Generar en paralelo, y
        // retenerlos en cada columna hacía que se estorbaran (DM-21).
        grupos = c.grupos.map { $0 }
        aciertoCondiciones = c.aciertoCondiciones?.map { $0 }
        aciertoGrupos = c.aciertoGrupos?.map { $0 }
        activo = c.activo
        esVacio = c.esVacio
    }

    /// `CompruebaPronostico(col, grupos)` (:277-298): todas las condiciones se evalúan (sin corte).
    func comprueba(_ columna: Columna, _ gruposPartidos: [Grupo]) throws(ErrorDominio) -> Bool {
        var fallos = 0
        for c in condiciones where !(try c.comprueba(columna)) { fallos += 1 }
        if !condiciones.isEmpty {
            guard let a = aciertoCondiciones else { throw .referenciaNula("ControladorIfThen.cs:287") }
            if !(try a.leer(condiciones.count - fallos, "ControladorIfThen.cs:287")) { return false }
        }
        fallos = 0
        for g in grupos where !(try Self.comprueba(g, columna, gruposPartidos)) { fallos += 1 }
        if !grupos.isEmpty {
            guard let a = aciertoGrupos else { throw .referenciaNula("ControladorIfThen.cs:296") }
            return try a.leer(grupos.count - fallos, "ControladorIfThen.cs:296")
        }
        return true
    }

    /// `GrupoIfThen.CompruebaPronostico` (:67-81): el veredicto memoizado del grupo en la columna en curso.
    static func comprueba(_ g: GrupoIfThenConfig, _ columna: Columna, _ grupos: [Grupo]) throws(ErrorDominio) -> Bool {
        var valido = try grupos.leerLista(g.numGrupoIf, "GrupoIfThen.cs:69").analiza(columna)
        if g.noIf { valido = !valido }
        if !valido { return true }
        valido = try grupos.leerLista(g.numGrupoThen, "GrupoIfThen.cs:74").analiza(columna)
        if g.noThen { valido = !valido }
        return valido
    }
}
