import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/ColProbablesFrmViewModel.cs (08 pantalla 19; CU-A44…A51) y sus satélites CopiarCPFrm (29),
// CopiarDatosCPFrm (30), ImportadorCPsFrm (67) y ExportadorCPsFrm (48).

/// Fila de las tablas de agrupaciones de Relaciones III.
public struct FilaAgrupacion: Sendable, Hashable, Identifiable {
    public var id = UUID()
    public var numero: String
    public var elementos: String
    public var aciertos: String

    public init(numero: String = "", elementos: String = "", aciertos: String = "") {
        (self.numero, self.elementos, self.aciertos) = (numero, elementos, aciertos)
    }
}

/// Fila de la tabla de Control Fallos.
public struct FilaControlFallos: Sendable, Hashable, Identifiable {
    public var id = UUID()
    public var columnas: String
    public var tolerancias: String
    public var aciertos: String

    public init(columnas: String = "", tolerancias: String = "", aciertos: String = "") {
        (self.columnas, self.tolerancias, self.aciertos) = (columnas, tolerancias, aciertos)
    }
}

/// Cómo se mezclan las CPs importadas con las del editor (`AplicarImportacion*`).
public enum ModoImportacionCP: Sendable {
    /// Cambia solo los pronósticos (conserva rangos y tolerancias).
    case sustituir
    case agregar
    case reemplazar
}

/// «Columnas Probables»: buffer de trabajo con las cinco pestañas; nada llega al filtro hasta `aceptar` (tampoco las
/// listas de Relaciones II y III, que en el C# son las del propio filtro; DT-60).
public struct EditorColumnasProbables: Sendable {
    public static let conceptos = ["AC", "ACS", "FS"]
    public static let masMenos = ["Más", "Menos"]

    public let numeroPartidos: Int
    let desplazamiento: Int

    // MARK: Columnas
    public var pronostico = ""
    public var puntos = ""
    public var aciertos = ""
    public var aciertosSeguidos = ""
    public var fallosSeguidos = ""
    public var aciertosTol = ""
    public var aciertosSeguidosTol = ""
    public var fallosSeguidosTol = ""
    /// Casillas «Núm Tolerancias» 0…3.
    public var tolerancias = [false, false, false, false]
    public private(set) var paginacion = "1/1"
    public private(set) var columnas: [ColumnaProbableConfig] = []
    public private(set) var actual = 0
    /// Rangos capturados por «Copiar datos».
    var copia = (ac: "", acs: "", fs: "")

    // MARK: Relaciones I
    public var rel1Columnas = ""
    public var rel1SumaAciertos = ""
    public var rel1Recorrido = ""
    public var rel1CuantasCP = ""
    public var rel1NumeroAciertos = ""
    public private(set) var paginacionRel1 = "1/1"
    public private(set) var relaciones1: [RelacionCP1Config] = []
    public private(set) var rel1 = 0

    // MARK: Relaciones II (índice 1-based)
    public var rel2 = Self.rel2Vacia
    public private(set) var paginacionRel2 = "1 / 1"
    public private(set) var relaciones2: [RelacionCP2Config] = []
    public private(set) var indiceRel2 = 1

    // MARK: Relaciones III (índice 1-based)
    public var rel3Columnas = ""
    public var rel3Concepto = "AC"
    public var rel3EscalerasTotal = ""
    public var rel3EscalerasAsc = ""
    public var rel3EscalerasDesc = ""
    public var rel3Sandwichs = ""
    public var agrupacionesPasoFijo: [FilaAgrupacion] = []
    public var agrupacionesSolapadas: [FilaAgrupacion] = []
    public private(set) var paginacionRel3 = "1 / 1"
    public private(set) var relaciones3: [RelacionCP3Config] = []
    public private(set) var indiceRel3 = 1

    // MARK: Control Fallos
    public var numeroFallosControles = ""
    public var controlesFallo: [FilaControlFallos] = []

    static var rel2Vacia: RelacionCP2Config {
        var r = RelacionCP2Config()
        (r.concepto, r.cantidad, r.concepto2, r.cantidad2) = ("AC", "Más", "AC", "Más")
        return r
    }

    /// Sin grupo en edición: todo vacío.
    public init(numeroPartidos: Int = 14, desplazamiento: Int = 3) {
        self.numeroPartidos = numeroPartidos
        self.desplazamiento = desplazamiento
    }

    /// `CargarDesdeGrupo` (:256-291): clona las CPs (`ObtenCopiaCP`) y carga relaciones y control de fallos.
    public init(_ f: CondicionColProbables, numeroPartidos: Int, desplazamiento: Int) throws(ErrorDominio) {
        self.init(numeroPartidos: numeroPartidos, desplazamiento: desplazamiento)
        for cp in f.columnas { columnas.append(try CasosCondiciones.copiaCP(cp, puntos: true)) }
        pintar(0)
        relaciones1 = f.relaciones1
        relaciones2 = f.relaciones2
        relaciones3 = f.relaciones3
        // `MostrarPrimeraRelacion2/3` solo refrescan la paginación si no hay relaciones («1 / 0»).
        if let r = relaciones2.first { rel2 = r } else { reinicializarRel2() }
        if let r = relaciones3.first { cargarRel3(r) } else { reinicializarRel3() }
        pintarRel1(0)
        numeroFallosControles = f.fallosPermitidos.texto
        controlesFallo = f.controlesFallos.map {
            FilaControlFallos(columnas: $0.columnas, tolerancias: $0.tolerancias, aciertos: $0.aciertos)
        }
    }

    // MARK: Columnas (:322-575)

    /// `ActualizaDatosPantalla` (:322-360): un partido doble/triple aporta 2–3 caracteres al pronóstico.
    mutating func pintar(_ n: Int) {
        let cp: ColumnaProbableConfig
        if let c = columnas[safe: n] {
            (cp, paginacion) = (c, "\(n + 1)/\(columnas.count)")
        } else {
            (cp, paginacion) = (ColumnaProbableConfig(numeroPartidos: numeroPartidos), "1/1")
        }
        pronostico = cp.textoPronosticos.joined()
        aciertos = cp.getAciertos
        aciertosSeguidos = cp.getAciertosSeguidos
        fallosSeguidos = cp.getFallosSeguidos
        puntos = cp.puntos.texto
        aciertosTol = cp.getACTol
        aciertosSeguidosTol = cp.getACSTol
        fallosSeguidosTol = cp.getFSTol
        let t = trozos(cp.getTolerancias, ",")
        tolerancias = (0..<4).map { t.contains(String($0)) }
    }

    /// `GuardaDatosCP` (:395-437): un carácter por partido (DEC-810); campo vacío = todos los valores.
    func guardaDatos(_ cp: inout ColumnaProbableConfig) throws(ErrorDominio) {
        cp.reinicializaValores()
        let u = Array(pronostico.utf16)
        try cp.ponerPronosticos((0..<numeroPartidos).map { $0 < u.count ? String(decoding: [u[$0]], as: UTF16.self) : "" })
        guard !pronostico.isEmpty else { return }
        let todos = CasosCondiciones.todosValores(numeroPartidos)
        try cp.setPuntos(puntos)
        try cp.setNoAciertos(aciertos.isEmpty ? todos : aciertos)
        try cp.setNoAciertosSeguidos(aciertosSeguidos.isEmpty ? todos : aciertosSeguidos)
        try cp.setNoFallosSeguidos(fallosSeguidos.isEmpty ? todos : fallosSeguidos)
        guard !aciertosTol.isEmpty || !aciertosSeguidosTol.isEmpty || !fallosSeguidosTol.isEmpty else { return }
        try cp.setACTol(aciertosTol.isEmpty ? todos : aciertosTol)
        try cp.setACSTol(aciertosSeguidosTol.isEmpty ? todos : aciertosSeguidosTol)
        try cp.setFSTol(fallosSeguidosTol.isEmpty ? todos : fallosSeguidosTol)
        let tols = tolerancias.indices.filter { tolerancias[$0] }.map(String.init).joined(separator: ",")
        try cp.setTolerancias(tols.isEmpty ? "0" : tols)
    }

    /// `GuardarCPActual` (:380-393): sin pronóstico la CP en pantalla se quita.
    mutating func guardarActual() throws(ErrorDominio) {
        let i: Int
        if actual < columnas.count {
            i = actual
        } else if !pronostico.isEmpty {
            columnas.append(ColumnaProbableConfig(numeroPartidos: numeroPartidos))
            i = columnas.count - 1
        } else {
            return
        }
        do {
            var cp = columnas[i]
            defer { columnas[i] = cp }
            try guardaDatos(&cp)
        }
        if pronostico.isEmpty { columnas.remove(at: i) }
    }

    var ultimaSinPronosticos: Bool { columnas.last?.textoPronosticos.allSatisfy(\.isEmpty) ?? false }

    /// `CambiaCPSelecionado` (:472-481): acota con el nº de CPs **antes** de guardar.
    mutating func cambiar(_ n: Int) throws(ErrorDominio) {
        let n = min(max(n, 0), columnas.count)
        try guardarActual()
        actual = n
        if columnas.count < n + 1 { columnas.append(ColumnaProbableConfig(numeroPartidos: numeroPartidos)) }
        pintar(n)
    }

    public mutating func primera() throws(ErrorDominio) { try cambiar(0) }
    public mutating func atras() throws(ErrorDominio) { try cambiar(actual - desplazamiento) }
    public mutating func anterior() throws(ErrorDominio) { try cambiar(actual - 1) }
    public mutating func siguiente() throws(ErrorDominio) { try cambiar(actual + 1) }
    public mutating func adelante() throws(ErrorDominio) { try cambiar(actual + desplazamiento) }
    public mutating func ultima() throws(ErrorDominio) { try cambiar(columnas.isEmpty ? 0 : columnas.count - 1) }

    public mutating func eliminarActual() {
        guard actual < columnas.count else { return }
        columnas.remove(at: actual)
        if actual >= columnas.count { actual = max(0, columnas.count - 1) }
        pintar(actual)
    }

    /// `TodosATriple`: sin CP en pantalla crea una al final; no guarda lo escrito.
    public mutating func todosATriple() {
        if actual < columnas.count {
            columnas[actual].todosATriple()
        } else {
            columnas.append(ColumnaProbableConfig(numeroPartidos: numeroPartidos))
            actual = columnas.count - 1
            columnas[actual].todosATriple()
        }
        pintar(actual)
    }

    public mutating func borrarPronosticos() {
        if actual < columnas.count { columnas[actual].borraPronosticos() }
        pintar(actual)
    }

    /// `Aceptar` (:575-596): la primera vez con datos activa la condición.
    public mutating func aceptar(en f: inout CondicionColProbables) throws(ErrorDominio) {
        try guardarActual()
        if ultimaSinPronosticos { columnas.removeLast() }
        if !f.contieneDatos && !columnas.isEmpty {
            f.contieneDatos = true
            f.activa = true
        }
        f.ponerColumnas(columnas)
        f.relaciones1 = try confirmarRel1()
        try guardarRel2()
        f.relaciones2 = relaciones2
        try guardarRel3()
        f.relaciones3 = relaciones3
        var lista: [CPControlFallosConfig] = []
        for fila in controlesFallo where !fila.columnas.isEmpty {
            var c = CPControlFallosConfig()
            try c.ponerColumnas(fila.columnas)
            c.tolerancias = fila.tolerancias
            try c.ponerAciertos(fila.aciertos)
            lista.append(c)
        }
        f.controlesFallos = lista
        try f.ponerFallosPermitidos(numeroFallosControles)
    }

    // MARK: Copiar datos (pantalla 30), importar (67) y exportar (48)

    /// `CopiarDatos` (:604-617): captura los rangos (vacío = todos); `nil` si no hay CPs. Devuelve el rango inicial.
    public mutating func prepararCopiaDatos() -> (desde: Int, hasta: Int)? {
        guard !columnas.isEmpty else { return nil }
        let todos = CasosCondiciones.todosValores(numeroPartidos)
        copia = (
            aciertos.isEmpty ? todos : aciertos, aciertosSeguidos.isEmpty ? todos : aciertosSeguidos,
            fallosSeguidos.isEmpty ? todos : fallosSeguidos
        )
        return (actual + 1, columnas.count)
    }

    /// `AplicarCopiaDatos` (:619-650): CPs `[desde−1, hasta)` y la actual si cae fuera; `desde < 1` = cancelado.
    public mutating func aplicarCopiaDatos(desde: Int, hasta: Int) throws(ErrorDominio) {
        let minimo = desde - 1
        guard minimo >= 0 else { return }
        var i = minimo
        while i < hasta && i < columnas.count {
            try copiarRangos(i)
            i += 1
        }
        if (actual < minimo || actual > hasta) && actual < columnas.count { try copiarRangos(actual) }
        pintar(actual)
    }

    private mutating func copiarRangos(_ i: Int) throws(ErrorDominio) {
        var cp = columnas[i]
        defer { columnas[i] = cp }
        cp.reinicializaValores()
        try cp.setNoAciertos(copia.ac)
        try cp.setNoAciertosSeguidos(copia.acs)
        try cp.setNoFallosSeguidos(copia.fs)
    }

    /// `ImportarCPs` (:711-721): antes de ir al importador quita la última CP si no tiene pronósticos (no guarda).
    public mutating func prepararImportacion() { if ultimaSinPronosticos { columnas.removeLast() } }

    /// Pregunta «¿Sustituir…?» si hay tantas importadas como CPs.
    public func confirmarSustituir(_ importadas: [ColumnaProbableConfig]) -> Bool {
        !importadas.isEmpty && importadas.count == columnas.count
    }

    /// Pregunta «¿Añadir…?» si ya hay CPs.
    public func confirmarAgregar(_ importadas: [ColumnaProbableConfig]) -> Bool { !importadas.isEmpty && !columnas.isEmpty }

    /// `AplicarImportacion*` (:744-787). «Sustituir» recorre las CPs **del filtro** y luego las del editor (como el C#).
    public mutating func importar(_ importadas: [ColumnaProbableConfig], modo: ModoImportacionCP, delFiltro: [ColumnaProbableConfig])
        throws(ErrorDominio)
    {
        if !importadas.isEmpty {
            switch modo {
            case .sustituir:
                var base: [ColumnaProbableConfig] = []
                for cp in delFiltro { base.append(try CasosCondiciones.copiaCP(cp, puntos: true)) }
                base += columnas
                columnas = []
                for i in 0..<min(importadas.count, base.count) {
                    var cp = base[i]
                    try cp.ponerPronosticos(importadas[i].textoPronosticos)
                    columnas.append(cp)
                }
            case .agregar: columnas += importadas
            case .reemplazar: columnas = importadas
            }
        }
        pintar(actual)
    }

    /// `ExportarCPs` (:788-796): guarda la CP en pantalla solo si hay rangos escritos.
    public mutating func prepararExportacion() throws(ErrorDominio) -> [ColumnaProbableConfig] {
        if !aciertos.isEmpty || !aciertosSeguidos.isEmpty || !fallosSeguidos.isEmpty { try guardarActual() }
        return columnas
    }

    // MARK: Relaciones I (:797-1078)

    mutating func pintarRel1(_ n: Int) {
        if let r = relaciones1[safe: n] {
            (rel1Columnas, rel1Recorrido, rel1SumaAciertos) = (r.columnas, r.recorridos, r.sumaAciertos)
            (rel1CuantasCP, rel1NumeroAciertos) = (r.cantidadCP.texto, r.cuantosAC.texto)
            paginacionRel1 = "\(n + 1)/\(relaciones1.count)"
        } else {
            (rel1Columnas, rel1Recorrido, rel1SumaAciertos, rel1CuantasCP, rel1NumeroAciertos) = ("", "", "", "", "")
            paginacionRel1 = "1/1"
        }
    }

    var tieneRel1Datos: Bool { !rel1Columnas.isEmpty && !(rel1SumaAciertos.isEmpty && rel1Recorrido.isEmpty && rel1CuantasCP.isEmpty) }

    mutating func guardarRel1Actual() throws(ErrorDominio) {
        guard tieneRel1Datos else { return }
        if rel1 >= relaciones1.count { relaciones1.append(RelacionCP1Config()) }
        let i = min(rel1, relaciones1.count - 1)
        var r = relaciones1[i]
        defer { relaciones1[i] = r }
        try r.ponerColumnas(rel1Columnas)
        try r.ponerSumaAciertos(rel1SumaAciertos)
        try r.ponerRecorridos(rel1Recorrido)
        try r.ponerCantidadCP(rel1CuantasCP)
        try r.ponerCuantosAC(rel1NumeroAciertos)
    }

    mutating func cambiarRel1(_ n: Int) throws(ErrorDominio) {
        try guardarRel1Actual()
        rel1 = n
        if relaciones1.count < n + 1 { relaciones1.append(RelacionCP1Config()) }
        pintarRel1(n)
    }

    /// En la primera no hace nada (el WinForms deshabilitaba el botón; ESPEC_08 §19.2).
    public mutating func rel1Anterior() throws(ErrorDominio) { if rel1 > 0 { try cambiarRel1(rel1 - 1) } }
    public mutating func rel1Siguiente() throws(ErrorDominio) { if tieneRel1Datos { try cambiarRel1(rel1 + 1) } }

    public mutating func rel1Eliminar() {
        if rel1 == 0 {
            if !relaciones1.isEmpty { relaciones1.remove(at: 0) }
        } else if rel1 < relaciones1.count {
            relaciones1.remove(at: rel1)
            rel1 -= 1
        }
        if relaciones1.isEmpty { relaciones1.append(RelacionCP1Config()) }
        pintarRel1(rel1)
    }

    /// `GuardarDatosRelacionesCP1` (:1040-1055).
    mutating func confirmarRel1() throws(ErrorDominio) -> [RelacionCP1Config] {
        try guardarRel1Actual()
        if let u = relaciones1.last, u.columnas.isEmpty || (u.sumaAciertos.isEmpty && u.recorridos.isEmpty && u.cantidadCP.texto.isEmpty) {
            relaciones1.removeLast()
        }
        return relaciones1.filter { !$0.columnas.isEmpty }
    }

    // MARK: Relaciones II (:1080-1257)

    mutating func reinicializarRel2() {
        rel2 = Self.rel2Vacia
        paginacionRel2 = "\(indiceRel2) / \(relaciones2.count)"
    }

    mutating func mostrarRel2() {
        if relaciones2.count >= indiceRel2 { rel2 = relaciones2[indiceRel2 - 1] } else { reinicializarRel2() }
    }

    /// `ComprobarRelacion2`: hace falta un bloque completo (columnas A y B y aciertos).
    static func completa(_ r: RelacionCP2Config) -> Bool {
        func vacio(_ a: String, _ b: String, _ ac: String) -> Bool {
            UtilidadesEntradasValores.obtenerListaFromTxt(a).isEmpty || UtilidadesEntradasValores.obtenerListaFromTxt(b).isEmpty
                || UtilidadesEntradasValores.obtenerListaFromTxtAciertos(ac).isEmpty
        }
        return !(vacio(r.columnasA, r.columnasB, r.aciertos) && vacio(r.columnasA2, r.columnasB2, r.aciertos2))
    }

    /// `GuardarRelacion2EnPantalla`.
    mutating func guardarRel2() throws(ErrorDominio) {
        guard Self.completa(rel2) else { return }
        if relaciones2.count >= indiceRel2 { relaciones2[indiceRel2 - 1] = rel2 } else { relaciones2.append(rel2) }
    }

    public mutating func rel2Anterior() throws(ErrorDominio) {
        guard indiceRel2 > 1 else { return }
        try guardarRel2()
        indiceRel2 -= 1
        paginacionRel2 = "\(indiceRel2) / \(relaciones2.count)"
        mostrarRel2()
    }

    public mutating func rel2Siguiente() throws(ErrorDominio) {
        try guardarRel2()
        indiceRel2 += 1
        mostrarRel2()
        paginacionRel2 = "\(indiceRel2) / \(relaciones2.count)"
    }

    public mutating func rel2Eliminar() {
        if relaciones2.isEmpty {
            reinicializarRel2()
        } else if relaciones2.count >= indiceRel2 {
            relaciones2.remove(at: indiceRel2 - 1)
            mostrarRel2()
        }
        paginacionRel2 = "\(indiceRel2) / \(relaciones2.count)"
    }

    // MARK: Relaciones III (:1259-1577)

    mutating func reinicializarRel3() {
        (rel3Columnas, rel3Sandwichs, rel3EscalerasTotal, rel3EscalerasAsc, rel3EscalerasDesc) = ("", "", "", "", "")
        rel3Concepto = "AC"
        agrupacionesPasoFijo = []
        agrupacionesSolapadas = []
        paginacionRel3 = "\(indiceRel3) / \(relaciones3.count)"
    }

    mutating func mostrarRel3() {
        if relaciones3.count >= indiceRel3 { cargarRel3(relaciones3[indiceRel3 - 1]) } else { reinicializarRel3() }
    }

    mutating func cargarRel3(_ r: RelacionCP3Config) {
        rel3Columnas = r.columnasImplicadas
        rel3Sandwichs = r.sandwichsTexto ?? ""
        rel3EscalerasTotal = r.escalerasTexto ?? ""
        rel3EscalerasAsc = r.escalerasAscTexto ?? ""
        rel3EscalerasDesc = r.escalerasDescTexto ?? ""
        rel3Concepto = r.conceptoTexto ?? ""
        agrupacionesPasoFijo = Self.filasAgrupacion(r.agrupacionesPasoFijoTexto)
        agrupacionesSolapadas = Self.filasAgrupacion(r.agrupacionesSolapadasTexto)
    }

    /// `CargarAgrupaciones`: cada `Número+Elementos+Aciertos` (sin trozos vacíos) de exactamente 3 partes.
    static func filasAgrupacion(_ texto: String?) -> [FilaAgrupacion] {
        guard let texto else { return [] }
        return trozos(texto, "#").compactMap { e in
            let p = trozos(e, "+").filter { !$0.isEmpty }
            return p.count == 3 ? FilaAgrupacion(numero: p[0], elementos: p[1], aciertos: p[2]) : nil
        }
    }

    /// `ObtenArrayAgrupaciones` (:1463-1486): filas completas con `elementos < CPs + 1` y ≤ 15 aciertos; ninguna ⇒ `nil`.
    func agrupaciones(_ filas: [FilaAgrupacion]) throws(ErrorDominio) -> [AgrupacionColumnas]? {
        var r: [AgrupacionColumnas] = []
        for f in filas where !f.numero.isEmpty && !f.elementos.isEmpty && !f.aciertos.isEmpty {
            let valores = UtilidadesEntradasValores.obtenerListaFromTxtAciertos(f.numero)
            let elementos = Int(try FormatoNET.convertToInt32(f.elementos))
            let ac = UtilidadesEntradasValores.obtenerListaFromTxtAciertos(f.aciertos)
            if elementos < columnas.count + 1 && ac.count <= 15 {
                r.append(AgrupacionColumnas(numero: valores, noElementos: elementos, noAciertos: ac))
            }
        }
        return r.isEmpty ? nil : r
    }

    /// `ObtenTextoAgrupaciones`: filas con número, unidas por `#` como al escribir el fichero.
    static func textoAgrupaciones(_ filas: [FilaAgrupacion]) -> String? {
        let t = filas.filter { !$0.numero.isEmpty }.map { $0.numero + "+" + $0.elementos + "+" + $0.aciertos }
        return t.isEmpty ? nil : t.joined(separator: "#")
    }

    /// `GuardarRelacion3EnPantalla` (:1416-1450): K = primeras CPs (posición, no valor del índice).
    mutating func guardarRel3() throws(ErrorDominio) {
        let k = min(UtilidadesEntradasValores.obtenerListaFromTxt(rel3Columnas).count, columnas.count)
        var r = RelacionCP3Config()
        (r.concepto, r.conceptoTexto, r.columnasImplicadas) = (rel3Concepto, rel3Concepto, rel3Columnas)
        r.escalerasAsc = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(rel3EscalerasAsc, longitud: k / 3)
        r.escalerasAscTexto = rel3EscalerasAsc
        r.escalerasDesc = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(rel3EscalerasDesc, longitud: k / 3)
        r.escalerasDescTexto = rel3EscalerasDesc
        r.escalerasTotales = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(rel3EscalerasTotal, longitud: k / 3)
        r.escalerasTexto = rel3EscalerasTotal
        r.sandwichs = try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(rel3Sandwichs, longitud: k / 4)
        r.sandwichsTexto = rel3Sandwichs
        r.agrupacionesPasoFijo = try agrupaciones(agrupacionesPasoFijo)
        r.agrupacionesPasoFijoTexto = Self.textoAgrupaciones(agrupacionesPasoFijo)
        r.agrupacionesSolapadas = try agrupaciones(agrupacionesSolapadas)
        r.agrupacionesSolapadasTexto = Self.textoAgrupaciones(agrupacionesSolapadas)
        // `ComprobarRelacion3`: sin ningún dato no se guarda.
        guard
            r.agrupacionesPasoFijo != nil || r.agrupacionesSolapadas != nil || !r.columnasImplicadas.isEmpty || r.escalerasAsc != nil
                || r.escalerasDesc != nil || r.escalerasTotales != nil || r.sandwichs != nil
        else { return }
        if relaciones3.count >= indiceRel3 { relaciones3[indiceRel3 - 1] = r } else { relaciones3.append(r) }
    }

    public mutating func rel3Anterior() throws(ErrorDominio) {
        guard indiceRel3 > 1 else { return }
        try guardarRel3()
        indiceRel3 -= 1
        paginacionRel3 = "\(indiceRel3) / \(relaciones3.count)"
        mostrarRel3()
    }

    public mutating func rel3Siguiente() throws(ErrorDominio) {
        try guardarRel3()
        indiceRel3 += 1
        mostrarRel3()
        paginacionRel3 = "\(indiceRel3) / \(relaciones3.count)"
    }

    public mutating func rel3Eliminar() {
        if relaciones3.isEmpty {
            reinicializarRel3()
        } else if relaciones3.count >= indiceRel3 {
            relaciones3.remove(at: indiceRel3 - 1)
            mostrarRel3()
        }
        paginacionRel3 = "\(indiceRel3) / \(relaciones3.count)"
    }
}

extension CasosCondiciones {
    /// `ObtenCopiaCP` (`ColProbablesFrmViewModel.cs:295-319`) y la copia de `CopiarCPFrm` (sin puntos): rehace la CP con
    /// sus *setters*. Un rango vacío lanza como en el C# (`str[str.Length-1]`).
    public static func copiaCP(_ cp: ColumnaProbableConfig, puntos: Bool) throws(ErrorDominio) -> ColumnaProbableConfig {
        var c = ColumnaProbableConfig(numeroPartidos: cp.numeroPartidos)
        try c.ponerPronosticos(cp.textoPronosticos)
        try c.setNoAciertos(cp.getAciertos)
        try c.setNoAciertosSeguidos(cp.getAciertosSeguidos)
        try c.setNoFallosSeguidos(cp.getFallosSeguidos)
        if puntos { try c.setPuntos(cp.puntos.texto) }
        if cp.toleranciaLocalActiva {
            try c.setACTol(cp.getACTol)
            try c.setACSTol(cp.getACSTol)
            try c.setFSTol(cp.getFSTol)
            try c.setTolerancias(cp.getTolerancias)
        }
        return c
    }

    // MARK: Pantalla 29 · Copiar Columnas

    /// `ObtenCP` + `IndicesValidos` (`CopiarCPFrmViewModel.cs`): `5` · `1,3,5` · `1-5` (el primer separador manda).
    /// Lanza si la expresión no vale; `nil` si algún índice no está en `1…disponibles`.
    static func indicesCP(_ s: String, disponibles: Int) throws(ErrorDominio) -> [Int]? {
        let o = "CopiarCPFrmViewModel.cs"
        var r: [Int]
        switch s.first(where: { $0 == "," || $0 == "-" }) {
        case ",":
            r = []
            for t in trozos(s, ",") { r.append(Int(try FormatoNET.convertToInt32(t))) }
        case "-":
            let t = trozos(s, "-")
            let a = try FormatoNET.convertToInt32(t[0])
            let n = Int(try FormatoNET.convertToInt32(t[1]) &- a &+ 1)
            // `new int[n]`: negativo ⇒ OverflowException; por encima del máximo de .NET ⇒ OutOfMemoryException.
            guard n >= 0, n <= 0x7FFF_FFC7 else { throw .desbordamiento(o) }
            // ponytail: valida antes de materializar el rango (el C# reserva int[n]).
            if n > 0 && (a < 1 || Int(a) + n - 1 > disponibles) { return nil }
            r = Array(Int(a)..<Int(a) + n)
        default:
            r = [Int(try FormatoNET.convertToInt32(s))]
        }
        return r.allSatisfy { $0 >= 1 && $0 <= disponibles } ? r : nil
    }

    /// `Copiar` (`CopiarCPFrmViewModel.cs`): copia CPs del grupo en edición a los grupos marcados; el grupo en pantalla
    /// recibe las copias en la lista de origen y sin activar nada. Devuelve el estado y si se cierra la pantalla.
    public static func copiarColumnasCP(
        _ texto: String, destinos: Set<Int>, grupoEnEdicion: Int, grupoPantalla: Int, en c: inout Combinacion
    ) throws(ErrorDominio) -> (estado: String, cerrar: Bool) {
        if blanco(texto) { return ("Introduce al menos una columna a copiar.", false) }
        if destinos.isEmpty { return ("Selecciona al menos un grupo destino.", false) }
        let origen = c.grupos.indices.contains(grupoEnEdicion) ? grupoEnEdicion : grupoPantalla
        guard c.grupos.indices.contains(origen) else {
            return ("No hay columnas de origen (abra primero el editor de Columnas Probables).", false)
        }
        let disponibles = c.grupos[origen].condiciones.colProbables.columnas.count
        let indices: [Int]
        do throws(ErrorDominio) {
            guard let i = try indicesCP(texto, disponibles: disponibles) else {
                return ("Algún índice supera el número de columnas disponibles (\(disponibles)).", false)
            }
            indices = i
        } catch {
            return ("Expresión de columnas no válida (usa \"5\", \"1,3,5\" o \"1-5\").", false)
        }
        var copiadas = 0
        for g in c.grupos.indices where destinos.contains(g) {
            for i in indices {
                let cp = try copiaCP(c.grupos[origen].condiciones.colProbables.columnas[i - 1], puntos: false)
                if g == grupoPantalla {
                    try c.modificarGrupo(origen) { x throws(ErrorDominio) in x.condiciones.colProbables.columnas.append(cp) }
                } else {
                    try c.modificarGrupo(g) { x throws(ErrorDominio) in
                        x.condiciones.colProbables.columnas.append(cp)
                        x.condiciones.colProbables.contieneDatos = true
                        x.condiciones.colProbables.activa = true
                    }
                }
                copiadas += 1
            }
        }
        return ("Copiadas \(copiadas) columna(s) a \(destinos.count) grupo(s).", true)
    }

    // MARK: Pantalla 67 · Importador de CPs

    public static let mensajeFormatoCPs = "Las columnas no tienen el formato correcto"

    /// `ObtenPronostico`: con comas, un partido por trozo; sin comas, un carácter por partido (línea corta ⇒ lanza).
    static func pronosticoImportado(_ linea: String, numeroPartidos n: Int) throws(ErrorDominio) -> [String] {
        let l = FormatoNET.mayusculas(linea)
        if l.contains(",") { return trozos(l, ",") }
        let u = Array(l.utf16)
        guard u.count >= n else { throw .fueraDeRango("ImportadorCPsFrmViewModel.cs:ObtenPronostico") }
        return (0..<n).map { String(decoding: [u[$0]], as: UTF16.self) }
    }

    /// `ImportarSimples`: cada línea no vacía es una CP con todos los rangos; `nil` = formato incorrecto.
    public static func importarCPsSimples(_ datos: Data, numeroPartidos n: Int) -> [ColumnaProbableConfig]? {
        let todos = todosValores(n)
        var r: [ColumnaProbableConfig] = []
        for linea in ArchivoColumnasTexto.columnas(datos) where !linea.isEmpty {
            do throws(ErrorDominio) {
                var cp = ColumnaProbableConfig(numeroPartidos: n)
                try cp.ponerPronosticos(try pronosticoImportado(linea, numeroPartidos: n))
                try cp.setNoAciertos(todos)
                try cp.setNoAciertosSeguidos(todos)
                try cp.setNoFallosSeguidos(todos)
                r.append(cp)
            } catch {
                return nil
            }
        }
        return r
    }

    /// `ImportarClm`: líneas `pronóstico#AC#ACS#FS` (las demás se saltan); `nil` = formato incorrecto.
    public static func importarCPsClm(_ datos: Data, numeroPartidos n: Int) -> [ColumnaProbableConfig]? {
        var r: [ColumnaProbableConfig] = []
        for linea in TextoNET.lineas(datos) {
            let p = trozos(linea, "#")
            guard p.count == 4 else { continue }
            do throws(ErrorDominio) {
                var cp = ColumnaProbableConfig(numeroPartidos: n)
                try cp.ponerPronosticos(try pronosticoImportado(p[0], numeroPartidos: n))
                try cp.setNoAciertos(p[1])
                try cp.setNoAciertosSeguidos(p[2])
                try cp.setNoFallosSeguidos(p[3])
                r.append(cp)
            } catch {
                return nil
            }
        }
        return r
    }

    // MARK: Pantalla 48 · Exportador de CPs

    /// `PronosticosString`: los N partidos separados por comas.
    public static func textoCP(_ cp: ColumnaProbableConfig) -> String { cp.textoPronosticos.joined(separator: ",") }

    /// `.txt`: una CP por línea tal cual (`GuardarTodasCols(…, true)`); sin CPs no se crea el fichero.
    public static func datosCPsSimples(_ lista: [ColumnaProbableConfig]) -> Data? {
        ArchivoColumnasTexto.datosConComa(lista.map(textoCP))
    }

    /// `.clm`: `pronóstico#AC#ACS#FS` + CRLF (`StreamWriter`, UTF-8 sin BOM).
    public static func datosCPsClm(_ lista: [ColumnaProbableConfig]) -> Data {
        Data(
            lista.map { textoCP($0) + "#" + $0.getAciertos + "#" + $0.getAciertosSeguidos + "#" + $0.getFallosSeguidos + "\r\n" }
                .joined().utf8)
    }
}
