import Foundation
import Free1X2CasosDeUso
import Free1X2Pruebas
import Testing

/// Plan de pruebas funcionales (`docs/mac/PLAN_PRUEBAS_FUNCIONALES.md`), Fase 1: flujos de punta a punta sin UI, con
/// ficheros reales en una carpeta temporal y el juego de datos JD de §3. Los esperados son los de §3.1, calculados a mano.
@Suite("Flujos funcionales")
struct FlujosTests {
    /// JD-01: partidos 1–6 ∈ {1, X, 2}, 7–14 = «1».
    static let seisTriples: [String] = (0..<729).map { i in
        var (n, s) = (i, "")
        for _ in 0..<6 {
            s += ["1", "X", "2"][n % 3]
            n /= 3
        }
        return s + String(repeating: "1", count: 8)
    }
    /// JD-02: las 64 de JD-01 sin «X».
    static let sinX = seisTriples.filter { !$0.contains("X") }
    static let unos = String(repeating: "1", count: 14)
    static let doses = String(repeating: "2", count: 14)

    let carpeta: URL
    let entorno: Entorno

    init() throws {
        carpeta = FileManager.default.temporaryDirectory.appending(path: "Free1X2Flujos-\(UUID().uuidString)", directoryHint: .isDirectory)
        try FileManager.default.createDirectory(at: carpeta, withIntermediateDirectories: true)
        // Las semillas reales de la app (`Free1X2.Mac/App/Recursos/Datos`).
        let recursos = URL(filePath: #filePath).deletingLastPathComponent().appending(path: "../../../../App/Recursos").standardized
        entorno = Entorno(
            reloj: RelojSistema(), archivos: SistemaArchivosLocal(),
            rutas: RutasApp(base: carpeta, recursos: recursos, logs: carpeta.appending(path: "Logs")),
            red: ClienteHTTPGrabado(), portapapeles: PortapapelesEnMemoria(), registro: RegistroEnMemoria(), cultura: .esES)
        // Como la app al arrancar: carpetas de trabajo y semillas.
        _ = CasosInicio.arrancar(entorno: entorno)
    }

    func fichero(_ nombre: String, _ lineas: [String]) throws -> URL {
        let url = carpeta.appending(path: nombre)
        try FileManager.default.createDirectory(at: url.deletingLastPathComponent(), withIntermediateDirectories: true)
        try Data(lineas.map { $0 + "\r\n" }.joined().utf8).write(to: url)
        return url
    }

    func lineas(_ url: URL) throws -> [String] {
        String(decoding: try Data(contentsOf: url), as: UTF8.self).components(separatedBy: "\r\n").filter { !$0.isEmpty }
    }

    static func aciertos(_ a: String, _ b: String) -> Int { zip(a, b).filter { $0 == $1 }.count }

    /// Boleto de JD-08: partidos 1–6 a triple y 7–14 fijos a «1».
    static func combinacion() -> Combinacion {
        var c = Combinacion(numeroPartidos: 14)
        c.ponerPronosticos((0..<14).map { $0 < 6 ? "1,X,2" : "1" })
        return c
    }

    func calcular(_ c: Combinacion, en nombre: String) async throws -> (aceptadas: Int, salida: URL) {
        let salida = carpeta.appending(path: "Columnas/\(nombre)")
        let r = try await CasosCombinacion.calcularCombinacion(c, modo: .grabar(salida), configuracion: .porDefecto, entorno: entorno)
        return (Int(r.calculo.aceptadas), salida)
    }

    /// F01: carpetas de trabajo y semillas en una carpeta vacía; boleto inicial de 14 triples.
    @Test func f01PrimerArranque() throws {
        for c in RutasApp.Carpeta.allCases { #expect(entorno.archivos.existe(entorno.rutas.carpeta(c)), "\(c)") }
        for s in CasosInicio.semillas { #expect(entorno.archivos.existe(carpeta.appending(path: s)), "\(s)") }
    }

    /// F03 y F04: sin condiciones salen las 729 de JD-01; Variantes acota a 73, sin X a 64 y sin X ni 2 a 1.
    @Test func f03f04Calcular() async throws {
        var c = Self.combinacion()
        let (n, salida) = try await calcular(c, en: "todas.txt")
        #expect(n == 729)
        #expect(Set(try lineas(salida)) == Set(Self.seisTriples))
        // Con espacios («0 1 2») el parser lanza «formato» como el C# (DEC-904); por eso la ayuda pide comas (DT-78).
        try c.modificarGrupo(0) { g throws(ErrorDominio) in
            #expect(throws: ErrorDominio.self) {
                try CasosCondiciones.aplicarNoVariantes(variantes: "0 1 2", equis: "", doses: "", en: &g.condiciones.noVariantes)
            }
        }
        for (v, x, d, esperado) in [("0,1,2", "", "", 73), ("", "0", "", 64), ("", "0", "0", 1)] {
            try c.modificarGrupo(0) { g throws(ErrorDominio) in
                try CasosCondiciones.aplicarNoVariantes(variantes: v, equis: x, doses: d, en: &g.condiciones.noVariantes)
            }
            let r = try await calcular(c, en: "nv.txt")
            #expect(r.aceptadas == esperado, "Var «\(v)» X «\(x)» 2 «\(d)»")
            #expect(try lineas(r.salida).count == esperado)
        }
    }

    /// F52 (DT-85): las columnas de Inicio van a `Temp/ColumnasInicio.txt` sin elegir fichero; si la condición no deja
    /// ninguna, error y sin el fichero de la vez anterior.
    @Test func f52ColumnasDeInicio() async throws {
        var c = Self.combinacion()
        let r = try await CasosCombinacion.columnasDeInicio(c, configuracion: .porDefecto, entorno: entorno)
        #expect(r.columnas == 729 && r.url.lastPathComponent == "ColumnasInicio.txt")
        #expect(Set(try lineas(r.url)) == Set(Self.seisTriples))
        try c.modificarGrupo(0) { g throws(ErrorDominio) in
            try CasosCondiciones.aplicarNoVariantes(variantes: "13", equis: "", doses: "", en: &g.condiciones.noVariantes)
        }
        await #expect(throws: ErrorDominio.self) {
            try await CasosCombinacion.columnasDeInicio(c, configuracion: .porDefecto, entorno: entorno)
        }
        #expect(!entorno.archivos.existe(r.url))
    }

    /// F06: guardar y abrir una combinación conserva boleto, condición y recuento; `completa.comb` se reescribe igual.
    @Test func f06GuardarYAbrir() async throws {
        var c = Self.combinacion()
        try c.modificarGrupo(0) { g throws(ErrorDominio) in
            try CasosCondiciones.aplicarNoVariantes(variantes: "0,1,2", equis: "", doses: "", en: &g.condiciones.noVariantes)
        }
        let url = carpeta.appending(path: "Combinaciones/seis_triples.comb")
        try FileManager.default.createDirectory(at: url.deletingLastPathComponent(), withIntermediateDirectories: true)
        let equipos = (1...14).map { "Local \($0) - Visitante \($0)" }
        try CasosInicio.guardarCombinacion(c, equipos: equipos, filtroColumnas: "", en: url, entorno: entorno)
        let l = try CasosInicio.abrirCombinacion(url, numeroPartidos: 14, entorno: entorno)
        #expect(l.combinacion.pronosticos == c.pronosticos && l.equipos == equipos)
        #expect(l.combinacion.grupos[0].condiciones.noVariantes.activa)
        #expect(try await calcular(l.combinacion, en: "releida.txt").aceptadas == 73)

        let original = try Golden.bytes("Fixtures/corpus/comb/completa.comb")
        let completa = carpeta.appending(path: "Combinaciones/completa.comb")
        try original.write(to: completa)
        let leida = try CasosInicio.abrirCombinacion(completa, numeroPartidos: 14, entorno: entorno)
        let copia = carpeta.appending(path: "Combinaciones/copia.comb")
        try CasosInicio.guardarCombinacion(
            leida.combinacion, equipos: leida.equipos, filtroColumnas: leida.filtroColumnas, en: copia, entorno: entorno)
        #expect(try Data(contentsOf: copia) == original)
        #expect(leida.combinacion.grupos.count == 3 && leida.combinacion.ifThen?.esVacio == false)
    }

    /// F11: reducir JD-01 a 13 da entre 73 (K₃(6,1)) y 729 columnas y garantiza 13 aciertos a todas.
    @Test func f11Reducir() async throws {
        let entrada = try fichero("Columnas/seis_triples.txt", Self.seisTriples)
        let salida = carpeta.appending(path: "Columnas/reducida.txt")
        let e = EntradaReduccion(
            algoritmo: .menosTiempo, entrada: entrada, salida: salida, nivel: 13, maxCol: 0, percent: 100, externas: false)
        let r = try await CasosCombinacion.reducirColumnas(e, numeroPartidos: 14, entorno: entorno)
        let reducidas = try lineas(salida)
        #expect(r.iniciales == 729 && r.finales == reducidas.count && (73...729).contains(reducidas.count))
        let sinCubrir = Self.seisTriples.filter { c in !reducidas.contains { Self.aciertos($0, c) >= 13 } }
        #expect(sinCubrir.isEmpty, "\(sinCubrir.count) columnas sin garantía de 13")
    }

    /// F13: escrutinio simple de JD-01 y JD-02 contra la columna de unos.
    @Test func f13EscrutinioSimple() async throws {
        let rango = Array(10...14)
        for (nombre, cols, esperado) in [
            ("jd01.txt", Self.seisTriples, [64, 192, 240, 160, 60, 12, 1]), ("jd02.txt", Self.sinX, [1, 6, 15, 20, 15, 6, 1]),
        ] {
            let f = try fichero("Columnas/\(nombre)", cols)
            let e = EntradaEscrutinio(modo: .simple(ganadora: Self.unos), ficheros: [f], rangoAciertos: rango, registrarPremiadas: true)
            let r = try await CasosCombinacion.escrutarColumnas(e, numeroPartidos: 14, entorno: entorno)
            #expect(Array(r.globales[8...14]) == esperado, "\(nombre)")
            #expect(r.premiadas?.total == esperado[2...].reduce(0, +), "\(nombre)")
        }
    }

    /// F14: contra un fichero de 3 ganadoras (unos, doses, 1X2…): solo la de unos premia (473); se escrutan 3 × 729.
    @Test func f14EscrutinioContraFichero() async throws {
        let f = try fichero("Columnas/jd01.txt", Self.seisTriples)
        let g = try fichero("Ganadoras/ganadoras.txt", [Self.unos, Self.doses, "1X21X21X21X21X"])
        let e = EntradaEscrutinio(modo: .contraFichero(g), ficheros: [f], rangoAciertos: Array(10...14), registrarPremiadas: true)
        let r = try await CasosCombinacion.escrutarColumnas(e, numeroPartidos: 14, entorno: entorno)
        #expect(r.globales.reduce(0, +) == 3 * 729)
        #expect(Array(r.globales[10...14]) == [240, 160, 60, 12, 1] && r.premiadas?.total == 473)
    }

    /// F15: contra jornadas: la 1 (unos) da 473 premiadas y la 2 (doses) ninguna.
    @Test func f15EscrutinioContraJornadas() async throws {
        _ = try fichero("Jornadas/Resultados.txt", ["2024\t1\t\(Self.unos)", "2024\t2\t\(Self.doses)"])
        _ = try fichero("Quinielas/q2024_01.txt", Self.seisTriples)
        _ = try fichero("Quinielas/q2024_02.txt", Self.seisTriples)
        let e = EntradaEscrutinio(
            modo: .contraJornadas(
                plantilla: "q/t_/j.txt", carpeta: carpeta.appending(path: "Quinielas"), digitosTemporada: 4, digitosJornada: 2,
                temporadas: [2024]),
            ficheros: [], rangoAciertos: Array(10...14), registrarPremiadas: true)
        let r = try await CasosCombinacion.escrutarColumnas(e, numeroPartidos: 14, entorno: entorno)
        #expect(r.filas.map(\.archivo).prefix(2) == ["q2024_01.txt", "q2024_02.txt"])
        #expect(Array(r.globales[10...14]) == [240, 160, 60, 12, 1] && r.premiadas?.total == 473)
    }

    /// F27: JD-01 y JD-02 combinados: 64 columnas en los 2 filtros y 665 en uno.
    @Test func f27CombinarFiltros() async throws {
        let a = try fichero("Filtros/jd01.txt", Self.seisTriples)
        let b = try fichero("Filtros/jd02.txt", Self.sinX)
        let r = try await CasosUtilidades.combinarFiltros([(a, true), (b, true)], partidos: 14, entorno: entorno)
        #expect(r.columnas == [729, 64])
        #expect(r.cuentas.filter { $0 == 2 }.count == 64 && r.cuentas.filter { $0 == 1 }.count == 665)
        let s = carpeta.appending(path: "Filtros/en_dos.txt")
        let rango = CasosUtilidades.rangoFiltros(2, 2, filtros: 2)
        #expect(try CasosUtilidades.grabarFiltros(r.cuentas, rango: rango, en: s, entorno: entorno) == 64)
        #expect(Set(try lineas(s)) == Set(Self.sinX))
    }

    /// F29: álgebra con JD-01, JD-02 y JD-03 (= JD-01 + JD-02).
    @Test func f29Algebra() async throws {
        let a = try fichero("Columnas/jd01.txt", Self.seisTriples)
        let b = try fichero("Columnas/jd02.txt", Self.sinX)
        let ab = try fichero("Columnas/jd03.txt", Self.seisTriples + Self.sinX)
        for (op, c1, c2, esperado) in [
            (AlgebraComb.eliminaRepetidas, ab, nil, Set(Self.seisTriples)), (.sumaEliminaRepetidas, a, b, Set(Self.seisTriples)),
            (.sumaSoloComunes, a, b, Set(Self.sinX)), (.restaSegunda, a, b, Set(Self.seisTriples).subtracting(Self.sinX)),
        ] as [(AlgebraComb, URL, URL?, Set<String>)] {
            let s = carpeta.appending(path: "Columnas/algebra_\(op).txt")
            _ = try await CasosUtilidades.algebraDeColumnas(op, c1, c2, salida: s, numeroPartidos: 14, entorno: entorno)
            let l = try lineas(s)
            #expect(l.count == esperado.count && Set(l) == esperado, "\(op)")
        }
    }

    /// F30: transposición identidad = mismas columnas; 1↔7 pone el «1» fijo del partido 7 en el 1.
    @Test func f30Transponer() async throws {
        let e = try fichero("Columnas/jd01.txt", Self.seisTriples)
        let s = carpeta.appending(path: "Columnas/t.txt")
        // Como la pantalla: las posiciones 1–14 pasan por `ordenTransposicion` (permutación de 0…13).
        func orden(_ p: [Int]) throws -> [Int] { try #require(CasosUtilidades.ordenTransposicion(p.map(Double.init))) }
        try await CasosUtilidades.transponerColumnas(e, salida: s, orden: try orden(Array(1...14)), entorno: entorno)
        #expect(try lineas(s) == Self.seisTriples)
        try await CasosUtilidades.transponerColumnas(
            e, salida: s, orden: try orden([7, 2, 3, 4, 5, 6, 1] + Array(8...14)), entorno: entorno)
        let t = try lineas(s)
        #expect(t.count == 729 && t.allSatisfy { $0.first == "1" } && Set(t).count == 729)
    }

    /// F31 y F32: multiplicar JD-02 por una columna da 64 de 28 signos; fraccionar JD-01 en 3 tramos, 3 × 243.
    @Test func f31f32MultiplicarYFraccionar() async throws {
        let m = try await CasosUtilidades.multiplicarColumnas(Self.sinX, [Self.unos], plantilla: Array(1...28))
        #expect(m.count == 64 && m.allSatisfy { $0.count == 28 } && Set(m) == Set(Self.sinX.map { $0 + Self.unos }))
        let base = carpeta.appending(path: "Columnas/TRAMO")
        #expect(try await CasosUtilidades.fraccionar(Self.seisTriples, base: base, cuotas: nil, tramos: 3, entorno: entorno) == 3)
        let partes = try (1...3).map { try lineas(carpeta.appending(path: "Columnas/TRAMO0\($0).txt")) }
        #expect(partes.map(\.count) == [243, 243, 243] && partes.flatMap { $0 } == Self.seisTriples)
    }

    /// F33 y C2: «% COMBINACIÓN» de JD-01 (34/33/33 en 1–6; 100/0/0 en 7–14), propuesta con una jornada que valora más el
    /// 2 que el 1 (1↔2 en todos los partidos) y rotación: 729 columnas con los partidos 7–14 a «2».
    @Test func f33RotacionDeSignos() async throws {
        let e = try fichero("Columnas/jd01.txt", Self.seisTriples)
        let pct = try CasosUtilidades.porcentajesRotacion(e, partidos: 14, entorno: entorno)
        #expect(pct == [[Double]](repeating: [34, 33, 33], count: 6) + [[Double]](repeating: [100, 0, 0], count: 8))
        let jornada = [[Double]](repeating: [20, 30, 50], count: 14)
        let nuevos = CasosUtilidades.propuestaRotacion(combinacion: pct, jornada: jornada)
        #expect(nuevos == [[Int]](repeating: [2, 1, 0], count: 14))
        #expect(CasosUtilidades.propuestaRotacion(combinacion: [[40, 40, 20]], jornada: [[10, 20, 70]]) == [[2, 1, 0]])
        let s = carpeta.appending(path: "Columnas/rotada.txt")
        let n = try await CasosUtilidades.rotarSignos(e, salida: s, nuevos: nuevos, correccion: false, numeroPartidos: 14, entorno: entorno)
        let esperado = Set(Self.seisTriples.map { String($0.map { $0 == "1" ? "2" : $0 == "2" ? "1" : $0 }) })
        #expect(try n == 729 && Set(lineas(s)) == esperado)
        // Transposición automática del legacy: orden del caso de empates de `TransponedorAutomaticoTests`.
        let orden = try #require(TransponedorAutomatico.trasposicion(valoraciones: jornada, frecuencias: pct))
        let t = carpeta.appending(path: "Columnas/jd01T.txt")
        try await CasosUtilidades.transponerColumnas(e, salida: t, orden: orden, entorno: entorno)
        // Salida i = entrada orden[i]: 0–5 y 12–13 vienen de partidos fijos («1»); 6–11, de los 6 variables.
        let tl = try lineas(t)
        #expect(tl.count == 729 && tl.allSatisfy { $0.prefix(6) == "111111" && $0.suffix(2) == "11" })
        #expect(Set(tl.map { String($0.dropFirst(6).prefix(6)) }).count == 729)
    }

    /// F34: comprimir y descomprimir JD-01 en `.z3q` devuelve las mismas 729 columnas.
    @Test func f34Compresor() async throws {
        let e = try fichero("Columnas/jd01.txt", Self.seisTriples)
        let bits = try #require(try CasosUtilidades.bitmapDeColumnas(e, entorno: entorno))
        let z = try await CasosUtilidades.comprimirZ3Q(bits, en: e, nivel: 5, entorno: entorno)
        let s = carpeta.appending(path: "Columnas/descomprimida.txt")
        #expect(
            try await CasosUtilidades.columnasDeBitmap(
                try await CasosUtilidades.descomprimirZ3Q(z.ruta, entorno: entorno), en: s, entorno: entorno) == 729)
        #expect(Set(try lineas(s)) == Set(Self.seisTriples))
    }

    /// F48: un fichero mal formado (JD-04) da error controlado, no cierre.
    @Test func f48Errores() async throws {
        let malo = try fichero("Columnas/mal.txt", ["1X21X21X21X21", "1X21X21X21X2Z1", ""])
        #expect(try CasosUtilidades.bitmapDeColumnas(malo, entorno: entorno) == nil)
        let e = EntradaEscrutinio(modo: .simple(ganadora: Self.unos), ficheros: [malo], rangoAciertos: [14], registrarPremiadas: false)
        _ = try? await CasosCombinacion.escrutarColumnas(e, numeroPartidos: 14, entorno: entorno)
        let roto = carpeta.appending(path: "Combinaciones/roto.comb")
        try FileManager.default.createDirectory(at: roto.deletingLastPathComponent(), withIntermediateDirectories: true)
        try Data("<?xml version=\"1.0\"?><combinacion><grupo".utf8).write(to: roto)
        #expect(throws: ErrorDominio.self) { try CasosInicio.abrirCombinacion(roto, numeroPartidos: 14, entorno: entorno) }
    }

    /// F07: «grupo siguiente» crea el grupo 1; guardar y abrir conserva los 2 grupos; eliminarlo vuelve a 1.
    @Test func f07Grupos() throws {
        var c = Self.combinacion()
        #expect(try CasosInicio.cambiarGrupo(&c, a: 1) == 1 && c.grupos.count == 2)
        let url = carpeta.appending(path: "Combinaciones/grupos.comb")
        let equipos = (1...14).map { "L\($0) - V\($0)" }
        try CasosInicio.guardarCombinacion(c, equipos: equipos, filtroColumnas: "", en: url, entorno: entorno)
        #expect(try CasosInicio.abrirCombinacion(url, numeroPartidos: 14, entorno: entorno).combinacion.grupos.count == 2)
        _ = try CasosInicio.eliminarGrupo(&c, 1)
        #expect(c.grupos.count == 1)
    }

    /// F07b: con `completa.comb`, añadir un grupo con su propio control y su conjunto y borrarlo deja la combinación
    /// como estaba y Generar da las mismas columnas. Antes el control seguía apuntando al grupo borrado y Generar fallaba.
    @Test func f07bEliminarGrupoConSuControl() throws {
        var original = try ArchivoCombinacion.leer(
            try Golden.bytes("Fixtures/corpus/comb/completa.comb"), numeroPartidos: 14, cultura: .esES
        )
        .combinacion
        original.ponerPronosticos((0..<14).map { $0 < 8 ? "1,X,2" : "1" })
        func generar(_ c: Combinacion) throws -> [UInt8] {
            var texto: [UInt8] = []
            _ = try MotorCalculo.calcular(c, configuracion: .porDefecto, leerLineas: { _ in [] }) { col throws(ErrorDominio) in
                texto.append(contentsOf: try UtilColumnas.convLongToStr(col).utf8)
            }
            return texto
        }
        var c = original
        let nuevo = c.grupos.count
        _ = try CasosInicio.cambiarGrupo(&c, a: nuevo)
        try c.controlesGrupos[0].ponerGruposControlados(original.controlesGrupos[0].textoGruposControlados)
        var control = ControlGruposConfig()
        try control.ponerGruposControlados("\(nuevo)")
        try control.ponerFallosPermitidos("0")
        c.controlesGrupos.append(control)
        var conjunto = ControlConjuntosConfig()
        try conjunto.ponerCtrlGruposControlados("\(c.controlesGrupos.count - 1)")
        try conjunto.ponerFallosPermitidos("0")
        c.controlesConjuntos.append(conjunto)
        _ = try generar(c)
        #expect(try CasosInicio.eliminarGrupo(&c, nuevo) == nuevo - 1)
        #expect(c == original)
        let antes = try generar(original)
        let despues = try generar(c)
        #expect(!antes.isEmpty && despues == antes)
    }

    /// F16: «Ver premiadas ▸ Guardar todas» tras el escrutinio de JD-01: las 473 columnas con 10 o más aciertos.
    @Test func f16ColumnasPremiadas() async throws {
        let f = try fichero("Columnas/jd01.txt", Self.seisTriples)
        let e = EntradaEscrutinio(
            modo: .simple(ganadora: Self.unos), ficheros: [f], rangoAciertos: Array(10...14), registrarPremiadas: true)
        let r = try await CasosCombinacion.escrutarColumnas(e, numeroPartidos: 14, entorno: entorno)
        let s = carpeta.appending(path: "Columnas/premiadas.txt")
        try entorno.archivos.escribir(try #require(r.premiadas).datosColumnas(), en: s, atomico: false)
        let l = try lineas(s)
        #expect(l.count == 473 && Set(l) == Set(Self.seisTriples.filter { Self.aciertos($0, Self.unos) >= 10 }))
    }

    /// F06b: Generar en varios núcleos da el mismo texto y los mismos contadores que en un hilo (DM-21), con
    /// `completa.comb` (todas las condiciones, controles de grupos y conjuntos e If-Then) y 10 partidos triples.
    @Test func f06bGenerarEnParaleloIgualQueEnUnHilo() async throws {
        var c = try ArchivoCombinacion.leer(try Golden.bytes("Fixtures/corpus/comb/completa.comb"), numeroPartidos: 14, cultura: .esES)
            .combinacion
        c.ponerPronosticos((0..<14).map { $0 < 10 ? "1,X,2" : "1" })
        var uno: [UInt8] = []
        let r = try MotorCalculo.calcular(c, configuracion: .porDefecto, leerLineas: { _ in [] }) { col throws(ErrorDominio) in
            uno.append(contentsOf: try UtilColumnas.convLongToStr(col).utf8)
            uno.append(contentsOf: [0x0D, 0x0A])
        }
        let p = try await MotorCalculo.calcularEnParalelo(c, configuracion: .porDefecto, leerLineas: { _ in [] }, grabar: true, hilos: 3)
        #expect(p.calculo == r && r.aceptadas > 0)
        #expect(p.texto.flatMap { $0 } == uno)
    }

    /// F16b: premiadas en disco por páginas: JD-01 contra 3 ganadoras con todas las categorías da 3 × 729 = 2.187 premiadas,
    /// 3 páginas (1.000, 1.000 y 187), en el mismo orden y con los mismos datos que el escrutador en memoria.
    @Test func f16bPremiadasEnDiscoPorPaginas() async throws {
        let f = try fichero("Columnas/jd01.txt", Self.seisTriples)
        let ganadoras = [Self.unos, Self.doses, "1X21X21X21X21X"]
        let g = try fichero("Ganadoras/ganadoras.txt", ganadoras)
        let e = EntradaEscrutinio(modo: .contraFichero(g), ficheros: [f], rangoAciertos: Array(0...14), registrarPremiadas: true)
        let r = try await CasosCombinacion.escrutarColumnas(e, numeroPartidos: 14, entorno: entorno)
        let p = try #require(r.premiadas)
        defer { p.borrar() }
        #expect(p.total == 2187 && p.paginas == 3)
        let leidas = try (0..<p.paginas).map { try p.pagina($0) }
        #expect(leidas.map(\.count) == [1000, 1000, 187])
        var esc = Escrutador(
            premios: Array(0...14), numeroPartidos: 14, archivoColumnas: f.path(percentEncoded: false), anadirAGanadoras: true)
        _ = try esc.escrutarTemporada(ganadoras: ganadoras, columnas: Self.seisTriples, archivo: "jd01.txt")
        #expect(leidas.flatMap { $0 } == esc.listaPremiadas)
    }

    /// F17: posibles premios de JD-02 frente a la columna de unos (categorías 16…10).
    @Test func f17PosiblesPremios() async {
        let r = await CasosCombinacion.posiblesPremios(Self.sinX, ganadora: Self.unos, partidos: 14, pleno: false, generarResumen: false)
        #expect(r.categorias.map(\.count) == [0, 0, 1, 6, 15, 20, 15] && r.premiadas.count == 57)
        let o = CasosCombinacion.textosOptando(r.categorias, pleno: false)
        #expect(o[2] == "Optando a 14: 1" && o[6] == "Optando a 10: 15")
    }

    /// F35: `.txt` → `.csv` → `.txt` devuelve las mismas 729 columnas en el mismo orden.
    @Test func f35ImportarExportar() throws {
        let txt = try fichero("Columnas/jd01.txt", Self.seisTriples)
        let a = try CasosUtilidades.leerParaConvertir(txt, aTxt: false, entorno: entorno)
        #expect(a.columnas.count == 729 && a.salida.pathExtension == "csv")
        #expect(try CasosUtilidades.convertirColumnas(a.columnas, aTxt: false, salida: a.salida, entorno: entorno) == 729)
        let b = try CasosUtilidades.leerParaConvertir(a.salida, aTxt: true, entorno: entorno)
        let vuelta = carpeta.appending(path: "Columnas/vuelta.txt")
        #expect(try CasosUtilidades.convertirColumnas(b.columnas, aTxt: true, salida: vuelta, entorno: entorno) == 729)
        #expect(try lineas(vuelta) == Self.seisTriples)
    }

    /// F41 y F42: «Actualizar jornada» con el servicio grabado deja la jornada en la caché del disco; sin red se usa la
    /// caché, y el arranque la siembra sin tocar la red.
    @Test func f41f42Online() async throws {
        let url = try #require(URL(string: "https://clubprogol.com/wp-json/clubprogol/v1/quiniela/es/actual"))
        var conRed = entorno
        conRed.red = ClienteHTTPGrabado(respuestas: [url: .init(cuerpo: try Golden.bytes("Fixtures/online/quiniela-es-actual.json"))])
        let ok = await CasosArchivo.descargarJornada(.es, servicio: CasosArchivo.servicioOnline(entorno: conRed), entorno: conRed)
        let j = try #require(ok.jornada, "\(ok.mensaje)")
        #expect(ok.mensaje.contains("14 partidos cargados"))
        let cacheada = await CasosArchivo.descargarJornada(.es, servicio: CasosArchivo.servicioOnline(entorno: entorno), entorno: entorno)
        #expect(cacheada.jornada?.jornada == j.jornada && cacheada.mensaje.hasPrefix("Sin conexión: mostrando la jornada guardada"))
        #expect(CasosInicio.arrancar(entorno: entorno).jornada?.jornada == j.jornada)
    }

    /// F43: alta de un equipo con «Ñ» en 1.ª: se guarda en Latin-1 (como la app de Windows) y se relee; repetido ⇒ no.
    @Test func f43Equipos() throws {
        #expect(try CasosArchivo.anadirEquipo("Ñandú Cádiz", categoria: .primera, entorno: entorno))
        #expect(try !CasosArchivo.anadirEquipo("Ñandú Cádiz", categoria: .primera, entorno: entorno))
        let e = EditorEquipos.cargar(entorno: entorno)
        #expect(e.lista(.primera).last == "Ñandú Cádiz" && e.lista(.primera).count > 1)
        #expect(try Data(contentsOf: carpeta.appending(path: "Equipos/equipos1.dat")).contains(0xD1))
    }

    /// F44: la configuración guardada persiste en `parametros.free1x2` y la usa el siguiente arranque.
    @Test func f44Configuracion() throws {
        var e = CasosAjustes.leerConfiguracionEditable(entorno: entorno)
        #expect(e.jb == [15, 22, 29, 36, 48, 61] && e.recaudacion == 14_000_000)
        e.precioApuesta = 0.75
        _ = try CasosAjustes.guardarConfiguracion(e, entorno: entorno)
        #expect(CasosAjustes.leerConfiguracionEditable(entorno: entorno).precioApuesta == 0.75)
        #expect(CasosInicio.arrancar(entorno: entorno).configuracion.lae.precioApuesta == 0.75)
    }

    /// F47: «Borrar archivos temporales» quita los `*_tmp.comb` de `Temp/` y deja lo demás.
    @Test func f47BorrarTemporales() throws {
        _ = try fichero("Temp/a_tmp.comb", ["x"])
        _ = try fichero("Temp/b_tmp.comb", ["y"])
        _ = try fichero("Temp/otro.txt", ["z"])
        #expect(try CasosInicio.borrarTemporales(entorno: entorno) == 2)
        #expect(
            try FileManager.default.contentsOfDirectory(atPath: carpeta.appending(path: "Temp").path(percentEncoded: false)) == ["otro.txt"]
        )
    }

    /// F20: condiciones del grupo sobre JD-08. El esperado sale de un oráculo de fuerza bruta escrito desde la definición
    /// de la spec citada (independiente del motor), contado sobre las 729 columnas de JD-01.
    @Test func f20Condiciones() async throws {
        let cols = Self.seisTriples
        func aceptadas(_ cambio: (inout CondicionesGrupo) throws(ErrorDominio) -> Void) async throws -> Int {
            var c = Self.combinacion()
            try c.modificarGrupo(0) { g throws(ErrorDominio) in try cambio(&g.condiciones) }
            return try await calcular(c, en: "f20.txt").aceptadas
        }
        func esUno(_ c: Character) -> Bool { c == "1" }
        // Interrupciones (ESPEC_02 §2.1): pares consecutivos con signo distinto ∈ {0, 1, 2}.
        let cambios = { (s: String) in zip(s, s.dropFirst()).filter { $0 != $1 }.count }
        let interrupciones = try await aceptadas { c throws(ErrorDominio) in
            try CasosCondiciones.aplicarInterrupciones(["0,1,2"] + Array(repeating: "", count: 9), en: &c.interrupciones)
        }
        #expect(interrupciones == cols.filter { cambios($0) <= 2 }.count)
        // Distancias (ESPEC_03 §1): mayor separación entre unos consecutivos = 1 (todos los unos juntos).
        func distancia(_ s: String) -> Int {
            let p = s.enumerated().filter { esUno($0.element) }.map(\.offset)
            return zip(p, p.dropFirst()).map { $1 - $0 }.max() ?? 0
        }
        let distancias = try await aceptadas { c throws(ErrorDominio) in
            try CasosCondiciones.aplicarDistancias(["", "1", "", ""], en: &c.distancias)
        }
        #expect(distancias == cols.filter { distancia($0) == 1 }.count)
        // Signos seguidos (ESPEC_02 §4.1): racha más larga de «1» = 8 (el partido 6 no es «1»: 2·3⁵ = 486).
        func racha(_ s: String) -> Int {
            var (m, r) = (0, 0)
            for ch in s {
                r = esUno(ch) ? r + 1 : 0
                m = max(m, r)
            }
            return m
        }
        let seguidos = try await aceptadas { c throws(ErrorDominio) in
            try CasosCondiciones.aplicarSignosSeguidos(["", "8", "", ""], figuras: [nil, nil, nil, nil], en: &c.signosSeguidos)
        }
        #expect(seguidos == cols.filter { racha($0) == 8 }.count && seguidos == 486)
        // Simetrías (ESPEC_03 §6): partidos 1 y 2 con el mismo signo (3⁵ = 243).
        let simetrias = try await aceptadas { c throws(ErrorDominio) in
            _ = try CasosCondiciones.aplicarSimetrias(filas: ["1,2"], aciertos: "1", en: &c.simetrias, numeroPartidos: 14)
        }
        #expect(simetrias == cols.filter { Array($0)[0] == Array($0)[1] }.count && simetrias == 243)
        // Dibujos X+2 (ESPEC_03 §8): (nº de X, nº de 2) ∈ {(1, 1), (0, 0)} ⇒ 6·5 + 1 = 31.
        let dibujos = try await aceptadas { c throws(ErrorDominio) in try CasosCondiciones.aplicarDibujos(["1+1", "0+0"], en: &c.dibujos) }
        let dibujo = { (s: String) in "\(s.filter { $0 == "X" }.count)+\(s.filter { $0 == "2" }.count)" }
        #expect(dibujos == cols.filter { ["1+1", "0+0"].contains(dibujo($0)) }.count && dibujos == 31)
    }

    /// F05 y D-04: un valor no válido («a») lanza y **no** deja la condición a medias (el C# la dejaba activa y sin valores,
    /// y el cálculo daba 0): el grupo queda como estaba y Calcular sigue dando 729.
    @Test func f05CondicionNoValidaNoAltera() async throws {
        var c = Self.combinacion()
        let antes = c.grupos[0]
        #expect(throws: ErrorDominio.self) {
            try c.modificarGrupo(0) { g throws(ErrorDominio) in
                try CasosCondiciones.aplicarNoVariantes(variantes: "a", equis: "", doses: "", en: &g.condiciones.noVariantes)
            }
        }
        #expect(c.grupos[0] == antes)
        #expect(try await calcular(c, en: "f05.txt").aceptadas == 729)
    }
}
