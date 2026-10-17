import XCTest

/// Plan de pruebas funcionales (`docs/mac/PLAN_PRUEBAS_FUNCIONALES.md`), Fase 2: la app de verdad, pulsando sus controles,
/// con los datos aislados en una carpeta temporal (`-datos`) y el tema claro.
final class FlujosUITests: XCTestCase {
    /// La app con una carpeta de datos nueva (o la indicada, para comprobar lo que persiste entre arranques).
    @MainActor private func lanzar(datos: String? = nil, entorno: [String: String] = [:]) -> XCUIApplication {
        continueAfterFailure = false
        let app = XCUIApplication()
        let carpeta =
            datos ?? FileManager.default.temporaryDirectory.appending(path: "Free1X2UI-\(UUID().uuidString)").path(percentEncoded: false)
        app.launchArguments = ["-datos", carpeta, "-tema", "Claro"]
        // Sin red por defecto (DT-83): la jornada del arranque no llega y el boleto empieza en blanco, como en la CI de siempre.
        app.launchEnvironment = ["FREE1X2_API_BASE": "http://127.0.0.1:9"].merging(entorno) { $1 }
        app.launch()
        return app
    }

    /// Cierre normal (⌘Q), que guarda la barra como al salir de verdad.
    @MainActor private func salir(_ app: XCUIApplication) {
        app.typeKey("q", modifierFlags: .command)
        XCTAssertTrue(app.wait(for: .notRunning, timeout: 10))
    }

    /// Desplaza la página hasta que `e` se pueda pulsar (en macOS XCUITest no desplaza solo).
    @MainActor private func pulsar(_ app: XCUIApplication, _ e: XCUIElement) {
        XCTAssertTrue(e.waitForExistence(timeout: 10))
        // La zona de la página, no la del panel lateral: la más estrecha que está encima del control.
        let x = e.frame.midX
        let zona = app.scrollViews.allElementsBoundByIndex.filter { $0.frame.minX <= x && x <= $0.frame.maxX }
            .min { $0.frame.width < $1.frame.width }
        for _ in 0..<10 where !e.isHittable { zona?.scroll(byDeltaX: 0, deltaY: -300) }
        e.click()
    }

    /// Panel lateral (DT-82): escribe `texto` en el buscador y pulsa la fila `lateral-<destino>`.
    @MainActor private func ir(_ app: XCUIApplication, _ texto: String, _ destino: String) {
        let buscar = app.searchFields.firstMatch
        XCTAssertTrue(buscar.waitForExistence(timeout: 10))
        buscar.click()
        buscar.typeKey("a", modifierFlags: .command)
        buscar.typeText(texto)
        let fila = app.descendants(matching: .any)["lateral-\(destino)"].firstMatch
        XCTAssertTrue(fila.waitForExistence(timeout: 5), destino)
        fila.click()
    }

    /// Espera un aviso (`.alert` de SwiftUI: hoja o diálogo según la versión) y lo acepta con Return (su botón por defecto).
    @MainActor private func aceptarAviso(_ app: XCUIApplication) -> Bool {
        let hay = NSPredicate { _, _ in app.sheets.count + app.dialogs.count > 0 }
        let ok = XCTWaiter().wait(for: [XCTNSPredicateExpectation(predicate: hay, object: nil)], timeout: 10) == .completed
        app.typeKey(.return, modifierFlags: [])
        return ok
    }

    /// Barra de menús ▸ `ruta`: XCUITest abre solo el menú que contiene cada elemento (abrir antes otro menú a mano
    /// hacía fallar el recorrido con «open menu during menu traversal»).
    @MainActor private func menu(_ app: XCUIApplication, _ ruta: String...) {
        for (i, t) in ruta.enumerated() {
            let item = app.menuBars.menuItems[t].firstMatch
            XCTAssertTrue(item.waitForExistence(timeout: 5), t)
            if i < ruta.count - 1 { item.hover() } else { item.click() }
        }
    }

    /// JD-01 del plan: partidos 1–6 ∈ {1, X, 2}, 7–14 = «1».
    static let seisTriples: [String] = (0..<729).map { i in
        var (n, s) = (i, "")
        for _ in 0..<6 {
            s += ["1", "X", "2"][n % 3]
            n /= 3
        }
        return s + String(repeating: "1", count: 8)
    }

    /// Escribe un fichero de columnas (CRLF, como los de Windows) y devuelve su ruta.
    private func fichero(_ nombre: String, _ lineas: [String]) throws -> String {
        let url = FileManager.default.temporaryDirectory.appending(path: "Free1X2UI-ficheros-\(UUID().uuidString)/\(nombre)")
        try FileManager.default.createDirectory(at: url.deletingLastPathComponent(), withIntermediateDirectories: true)
        try Data(lineas.map { $0 + "\r\n" }.joined().utf8).write(to: url)
        return url.path(percentEncoded: false)
    }

    /// En el panel de abrir o guardar que acaba de aparecer: ⇧⌘G, la ruta y aceptar (sin depender del idioma del sistema).
    @MainActor private func panel(_ app: XCUIApplication, ruta: String) {
        _ = app.dialogs.firstMatch.waitForExistence(timeout: 10)
        Thread.sleep(forTimeInterval: 1)
        app.typeKey("g", modifierFlags: [.command, .shift])
        Thread.sleep(forTimeInterval: 1)
        app.typeText(ruta + "\r")
        Thread.sleep(forTimeInterval: 1)
        app.typeKey(.return, modifierFlags: [])
        Thread.sleep(forTimeInterval: 1)
    }

    /// Boleto de JD-08: partidos 1–6 a triple y 7–14 fijos a «1».
    @MainActor private func boletoSeisTriples(_ app: XCUIApplication) {
        XCTAssertTrue(app.staticTexts["Fijos: 0 - Dobles: 0 - Triples: 14"].waitForExistence(timeout: 10))
        for p in 7...14 {
            // Los signos son `Toggle` (casilla para la accesibilidad), no botones: se buscan por identificador en cualquier tipo.
            for s in ["X", "2"] { app.descendants(matching: .any)["signo-\(p)-\(s)"].firstMatch.click() }
        }
        XCTAssertTrue(app.staticTexts["Fijos: 8 - Dobles: 0 - Triples: 6"].exists)
    }

    /// Inicio ▸ Calcular columnas ▸ Calcular, y espera las aceptadas.
    @MainActor private func calcular(_ app: XCUIApplication, esperado: String) {
        app.buttons["Calcular columnas"].click()
        let calcular = app.buttons["Calcular"]
        XCTAssertTrue(calcular.waitForExistence(timeout: 10))
        calcular.click()
        let aceptadas = app.staticTexts["calculo-Aceptadas"]
        XCTAssertTrue(aceptadas.waitForExistence(timeout: 10))
        let hecho = NSPredicate(format: "value == %@ OR label == %@", esperado, esperado)
        wait(for: [XCTNSPredicateExpectation(predicate: hecho, object: aceptadas)], timeout: 30)
    }

    /// F02 + F03: el boleto de JD-08 da 729 columnas.
    @MainActor func testF02F03BoletoYCalcular() {
        let app = lanzar()
        defer { app.terminate() }
        boletoSeisTriples(app)
        calcular(app, esperado: "729")
    }

    /// F04: Variantes con «Var = 0,1,2» desde Inicio ⇒ 73 columnas.
    @MainActor func testF04CondicionYCalcular() {
        let app = lanzar()
        defer { app.terminate() }
        boletoSeisTriples(app)
        app.buttons["Variantes X y 2"].click()
        let campo = app.textFields["campo-Var"]
        XCTAssertTrue(campo.waitForExistence(timeout: 10))
        campo.click()
        campo.typeText("0,1,2")
        app.buttons["Aceptar"].click()
        XCTAssertTrue(app.buttons["Calcular columnas"].waitForExistence(timeout: 10))
        calcular(app, esperado: "73")
    }

    /// F07: «Grupo siguiente» crea el grupo 1 y «Eliminar el grupo actual» vuelve al boleto base.
    @MainActor func testF07Grupos() {
        let app = lanzar()
        defer { app.terminate() }
        XCTAssertTrue(app.staticTexts["1 grupo"].waitForExistence(timeout: 10))
        app.buttons["Grupo siguiente (crea si no existe)"].click()
        XCTAssertTrue(app.staticTexts["2 grupos"].waitForExistence(timeout: 5))
        XCTAssertTrue(app.staticTexts["Grupo 1 / 1"].exists)
        app.buttons["Eliminar el grupo actual"].click()
        XCTAssertTrue(app.staticTexts["1 grupo"].waitForExistence(timeout: 5))
    }

    /// F44: «Fijos» 6 → 7 en Configuración, Guardar y reabrir la app: sigue en 7.
    @MainActor func testF44ConfiguracionPersiste() {
        let datos = FileManager.default.temporaryDirectory.appending(path: "Free1X2UI-\(UUID().uuidString)").path(percentEncoded: false)
        var app = lanzar(datos: datos)
        ir(app, "Configuración", "configuracion")
        let fijos = app.textFields["Fijos"]
        XCTAssertTrue(fijos.waitForExistence(timeout: 10))
        XCTAssertEqual(fijos.value as? String, "6")
        fijos.doubleClick()
        fijos.typeKey("a", modifierFlags: .command)
        fijos.typeText("7\r")
        pulsar(app, app.scrollViews.buttons["Guardar"].firstMatch)
        // Aviso «Configuración guardada»: «Aceptar» es su botón por defecto (el de la Touch Bar no se puede pulsar).
        XCTAssertTrue(aceptarAviso(app), "se esperaba «Configuración guardada»")
        salir(app)
        app = lanzar(datos: datos)
        defer { app.terminate() }
        ir(app, "Configuración", "configuracion")
        XCTAssertTrue(app.textFields["Fijos"].waitForExistence(timeout: 10))
        XCTAssertEqual(app.textFields["Fijos"].value as? String, "7")
    }

    /// F45 (DT-82): el buscador del panel lateral no distingue tildes ni mayúsculas y la fila abre su pantalla; Inicio vuelve.
    @MainActor func testF45PanelLateral() {
        let app = lanzar()
        defer { app.terminate() }
        ir(app, "algebra", "algebraColumnas")
        XCTAssertFalse(app.descendants(matching: .any)["lateral-tramificar"].exists)
        let titulo = { (t: String) in NSPredicate { _, _ in app.windows.firstMatch.title.hasPrefix(t) } }
        wait(for: [XCTNSPredicateExpectation(predicate: titulo("Álgebra"), object: nil)], timeout: 10)
        ir(app, "inicio", "inicio")
        wait(for: [XCTNSPredicateExpectation(predicate: titulo("Free1X2"), object: nil)], timeout: 10)
    }

    /// F46: con el idioma en inglés, Créditos se muestra en inglés (solo esa pantalla está traducida, DP-07).
    @MainActor func testF46Idioma() {
        let app = lanzar()
        defer { app.terminate() }
        XCTAssertTrue(app.searchFields.firstMatch.waitForExistence(timeout: 10))
        menu(app, "Idioma", "English")
        menu(app, "Créditos…")
        XCTAssertTrue(app.staticTexts["Free1X2 Team"].waitForExistence(timeout: 10))
        menu(app, "Idioma", "Español")
    }

    /// Escrutinios ▸ JD-01 contra la columna de unos (rango 10–14 por defecto) y espera el histograma.
    @MainActor private func escrutarJD01(_ app: XCUIApplication, registrar: Bool = false) throws {
        let ruta = try fichero("jd01.txt", Self.seisTriples)
        app.buttons["Escrutinio"].firstMatch.click()
        let elegir = app.buttons["Seleccionar ficheros…"]
        XCTAssertTrue(elegir.waitForExistence(timeout: 10))
        elegir.click()
        panel(app, ruta: ruta)
        XCTAssertTrue(app.staticTexts["jd01.txt"].waitForExistence(timeout: 10))
        // «Ver premiadas» solo funciona con «Registrar columnas premiadas» activado (como el original).
        if registrar { app.switches["registrar-premiadas"].click() }
        let ganadora = app.textFields["campo-Columna ganadora"]
        ganadora.click()
        ganadora.typeText(String(repeating: "1", count: 14))
        app.buttons["Escrutar"].firstMatch.click()
        XCTAssertTrue(app.staticTexts["histograma-14"].waitForExistence(timeout: 30))
    }

    /// F13: Escrutinios ▸ JD-01 contra la columna de unos, rango 10–14 ⇒ 14: 1, 13: 12, 12: 60, 11: 160, 10: 240.
    @MainActor func testF13Escrutinio() throws {
        let app = lanzar()
        defer { app.terminate() }
        try escrutarJD01(app)
        for (aciertos, columnas) in [(14, "1"), (13, "12"), (12, "60"), (11, "160"), (10, "240")] {
            let t = app.staticTexts["histograma-\(aciertos)"]
            XCTAssertTrue(t.waitForExistence(timeout: 30), "\(aciertos)")
            XCTAssertEqual(t.value as? String ?? t.label, columnas, "\(aciertos) aciertos")
        }
    }

    /// F11: Reductor, JD-01 a nivel 13 (menos tiempo) ⇒ entre 73 (K₃(6,1)) y 729 columnas, y todas las de JD-01 con 13
    /// aciertos frente a alguna reducida.
    @MainActor func testF11Reducir() throws {
        let entrada = try fichero("jd01.txt", Self.seisTriples)
        let carpeta = (entrada as NSString).deletingLastPathComponent
        let app = lanzar()
        defer { app.terminate() }
        ir(app, "Reducir", "reductor")
        let elegir = app.buttons["Archivo entrada"]
        XCTAssertTrue(elegir.waitForExistence(timeout: 10))
        elegir.click()
        panel(app, ruta: entrada)
        XCTAssertTrue(app.staticTexts["jd01.txt"].waitForExistence(timeout: 10))
        app.buttons["Archivo salida"].click()
        panel(app, ruta: carpeta)
        XCTAssertTrue(app.staticTexts["Reducido.txt"].waitForExistence(timeout: 10))
        app.buttons["accion-reducir"].click()
        let admitidas = app.staticTexts["reduccion-Admitidas"]
        let hecho = NSPredicate(format: "value BEGINSWITH ' = '")
        wait(for: [XCTNSPredicateExpectation(predicate: hecho, object: admitidas)], timeout: 60)
        let n = Int(((admitidas.value as? String) ?? "").replacingOccurrences(of: " = ", with: "")) ?? -1
        XCTAssertTrue((73...729).contains(n), "admitidas \(n)")
        let reducidas = try String(contentsOfFile: carpeta + "/Reducido.txt", encoding: .utf8).components(separatedBy: "\r\n").filter {
            !$0.isEmpty
        }
        XCTAssertEqual(reducidas.count, n)
        let aciertos = { (a: String, b: String) in zip(a, b).filter { $0 == $1 }.count }
        XCTAssertTrue(Self.seisTriples.allSatisfy { c in reducidas.contains { aciertos($0, c) >= 13 } })
    }

    /// F06: Guardar la combinación, «Nueva» (vuelve a 14 triples) y Abrir lo guardado (vuelve el boleto de JD-08).
    @MainActor func testF06GuardarNuevaAbrir() throws {
        let carpeta = (try fichero("vacio.txt", []) as NSString).deletingLastPathComponent
        let app = lanzar()
        defer { app.terminate() }
        boletoSeisTriples(app)
        app.buttons["Guardar combinación"].firstMatch.click()
        panel(app, ruta: carpeta)
        let comb = carpeta + "/Combinacion.comb"
        XCTAssertTrue(FileManager.default.fileExists(atPath: comb))
        app.buttons["Nueva combinación"].firstMatch.click()
        XCTAssertTrue(app.staticTexts["Fijos: 0 - Dobles: 0 - Triples: 14"].waitForExistence(timeout: 10))
        app.buttons["Abrir combinación"].firstMatch.click()
        panel(app, ruta: comb)
        XCTAssertTrue(app.staticTexts["Fijos: 8 - Dobles: 0 - Triples: 6"].waitForExistence(timeout: 10))
    }

    /// F43: alta de un equipo en Gestión de Equipos y «Guardar archivos»: queda al final de `equipos1.dat` (Latin-1, como
    /// en Windows) y la pantalla lo vuelve a cargar al reabrir. La lista es larga: se comprueba en el fichero.
    @MainActor func testF43EquipoPersiste() throws {
        let datos = FileManager.default.temporaryDirectory.appending(path: "Free1X2UI-\(UUID().uuidString)").path(percentEncoded: false)
        let app = lanzar(datos: datos)
        defer { app.terminate() }
        ir(app, "gestion", "gestorEquipos")
        let nombre = app.textFields["equipo-nombre"]
        XCTAssertTrue(nombre.waitForExistence(timeout: 10))
        nombre.click()
        nombre.typeText("Equipo Prueba UI")
        app.buttons["Nuevo equipo"].click()
        pulsar(app, app.buttons["Guardar archivos"])
        let fichero = datos + "/Equipos/equipos1.dat"
        let hecho = NSPredicate { _, _ in
            (try? String(contentsOfFile: fichero, encoding: .isoLatin1))?.contains("Equipo Prueba UI") ?? false
        }
        wait(for: [XCTNSPredicateExpectation(predicate: hecho, object: nil)], timeout: 10)
    }

    /// Inicio ▸ condición `tile` ▸ `ajustar` ▸ Aceptar ▸ Calcular ⇒ `esperado` (boleto de JD-08).
    @MainActor private func condicion(_ tile: String, esperado: String, _ ajustar: (XCUIApplication) -> Void) {
        let app = lanzar()
        defer { app.terminate() }
        boletoSeisTriples(app)
        app.buttons[tile].click()
        ajustar(app)
        pulsar(app, app.buttons["Aceptar"].firstMatch)
        XCTAssertTrue(app.buttons["Calcular columnas"].waitForExistence(timeout: 10))
        calcular(app, esperado: esperado)
    }

    /// F20 por la UI: Distancias «1 = 1» (todos los unos juntos): los partidos 1…j distintos de «1» y j+1…6 a «1»,
    /// Σ 2ʲ (j = 0…6) = 127.
    @MainActor func testF20Distancias() {
        condicion("Distancias", esperado: "127") { app in
            let campo = app.textFields["campo-1"]
            XCTAssertTrue(campo.waitForExistence(timeout: 10))
            campo.click()
            campo.typeText("1")
        }
    }

    /// F20 por la UI: Dibujos X+2 «1+1» y «0+0» ⇒ 6·5 + 1 = 31.
    @MainActor func testF20Dibujos() {
        condicion("Dibujos", esperado: "31") { app in
            for d in ["1+1", "0+0"] {
                let casilla = app.checkBoxes[d]
                XCTAssertTrue(casilla.waitForExistence(timeout: 10), d)
                casilla.click()
            }
        }
    }

    /// F48: abrir un `.comb` truncado muestra un error y la app sigue (el boleto no cambia).
    @MainActor func testF48CombinacionCorrupta() throws {
        let roto = try fichero("roto.comb", ["<?xml version=\"1.0\"?><combinacion><grupo"])
        let app = lanzar()
        defer { app.terminate() }
        XCTAssertTrue(app.staticTexts["Fijos: 0 - Dobles: 0 - Triples: 14"].waitForExistence(timeout: 10))
        app.buttons["Abrir combinación"].firstMatch.click()
        panel(app, ruta: roto)
        XCTAssertTrue(aceptarAviso(app), "se esperaba el aviso de error")
        XCTAssertTrue(app.staticTexts["Fijos: 0 - Dobles: 0 - Triples: 14"].waitForExistence(timeout: 10))
        XCTAssertTrue(app.buttons["Calcular columnas"].isHittable)
    }

    /// F16: tras el escrutinio de F13, «Ver premiadas…» ▸ «Guardar Todas» graba las 473 columnas con 10 o más aciertos.
    @MainActor func testF16GuardarPremiadas() throws {
        let carpeta = (try fichero("vacio.txt", []) as NSString).deletingLastPathComponent
        let app = lanzar()
        defer { app.terminate() }
        try escrutarJD01(app, registrar: true)
        pulsar(app, app.buttons["Ver premiadas…"])
        pulsar(app, app.buttons["Guardar Todas"])
        panel(app, ruta: carpeta)
        let salida = carpeta + "/columnas.txt"
        let hecho = NSPredicate { _, _ in FileManager.default.fileExists(atPath: salida) }
        wait(for: [XCTNSPredicateExpectation(predicate: hecho, object: nil)], timeout: 10)
        let lineas = try String(contentsOfFile: salida, encoding: .utf8).components(separatedBy: "\r\n").filter { !$0.isEmpty }
        XCTAssertEqual(lineas.count, 473)
        XCTAssertEqual(Set(lineas), Set(Self.seisTriples.filter { c in c.filter { $0 == "1" }.count >= 10 }))
    }

    /// F05: un valor no válido en Variantes («a») da error y no se aplica: el semáforo sigue vacío y Calcular da 729.
    @MainActor func testF05CondicionNoValida() {
        let app = lanzar()
        defer { app.terminate() }
        boletoSeisTriples(app)
        app.buttons["Variantes X y 2"].click()
        let campo = app.textFields["campo-Var"]
        XCTAssertTrue(campo.waitForExistence(timeout: 10))
        campo.click()
        campo.typeText("a")
        pulsar(app, app.buttons["Aceptar"].firstMatch)
        XCTAssertTrue(aceptarAviso(app), "se esperaba el aviso de error")
        pulsar(app, app.buttons["Cancelar"].firstMatch)
        XCTAssertTrue(app.buttons["Calcular columnas"].waitForExistence(timeout: 10))
        calcular(app, esperado: "729")
    }

    /// F49 (DT-81): Inicio ▸ «Jornada» ▸ España rellena local y visitante de los 14 partidos. Sin red (la API apunta a un
    /// puerto cerrado), así que sale de la jornada guardada, como haría la app sin conexión.
    @MainActor func testF49CargarJornada() throws {
        let datos = FileManager.default.temporaryDirectory.appending(path: "Free1X2UI-\(UUID().uuidString)")
        try FileManager.default.createDirectory(at: datos, withIntermediateDirectories: true)
        let partidos = (1...14).map { n in
            n == 1 ? #"{"n":1,"local":"Villarreal","visitante":"Nápoles"}"# : #"{"n":\#(n),"local":"L\#(n)","visitante":"V\#(n)"}"#
        }
        let json =
            #"{"pais":"ES","temporada":"2026/27","jornada":20261014,"fecha":"2026-10-14","partidos":["#
            + partidos.joined(separator: ",") + "]}"
        try Data(json.utf8).write(to: datos.appending(path: "jornada-es.json"))
        let app = lanzar(datos: datos.path(percentEncoded: false), entorno: ["FREE1X2_API_BASE": "http://127.0.0.1:9"])
        defer { app.terminate() }
        let menuJornada = app.descendants(matching: .any)["cargar-jornada"].firstMatch
        XCTAssertTrue(menuJornada.waitForExistence(timeout: 10))
        menuJornada.click()
        app.menuItems["España"].firstMatch.click()
        XCTAssertTrue(aceptarAviso(app))
        for (id, valor) in [("local-1", "Villarreal"), ("visitante-1", "Nápoles"), ("local-14", "L14"), ("visitante-14", "V14")] {
            XCTAssertEqual(app.textFields[id].value as? String, valor, id)
        }
    }

    /// F51 (DT-84): guardar el boleto como máscara, empezar una combinación nueva y aplicar la máscara desde la barra.
    @MainActor func testF51Mascaras() {
        let app = lanzar()
        defer { app.terminate() }
        boletoSeisTriples(app)
        let menuMascaras = app.descendants(matching: .any)["menu-mascaras"].firstMatch
        XCTAssertTrue(menuMascaras.waitForExistence(timeout: 10))
        menuMascaras.click()
        app.menuItems["Guardar boleto como máscara…"].firstMatch.click()
        // El campo del diálogo no lleva identificador (macOS lo descarta): se busca por su texto de ayuda.
        let nombre = app.textFields.matching(NSPredicate(format: "placeholderValue == %@", "Nombre")).firstMatch
        XCTAssertTrue(nombre.waitForExistence(timeout: 10))
        nombre.typeText("JD01\r")
        XCTAssertTrue(nombre.waitForNonExistence(timeout: 10))
        XCTAssertTrue(aceptarAviso(app), "se esperaba «Máscara «JD01» guardada.»")
        app.buttons["Nueva combinación"].firstMatch.click()
        XCTAssertTrue(app.staticTexts["Fijos: 0 - Dobles: 0 - Triples: 14"].waitForExistence(timeout: 10))
        menuMascaras.click()
        app.menuItems["JD01  ·  729 col."].firstMatch.click()
        XCTAssertTrue(app.staticTexts["Fijos: 8 - Dobles: 0 - Triples: 6"].waitForExistence(timeout: 10))
    }

    /// F52 (DT-85): Inicio ▸ Reducir con el boleto JD-01 abre el Reductor con sus columnas ya cargadas (sin elegir
    /// fichero), reduce a un temporal y deja «Guardar resultado…» activo.
    @MainActor func testF52ReducirSinFichero() {
        let app = lanzar()
        defer { app.terminate() }
        boletoSeisTriples(app)
        app.buttons["Reducir columnas"].firstMatch.click()
        let entrada = app.staticTexts["reductor-entrada"]
        XCTAssertTrue(entrada.waitForExistence(timeout: 30))
        let cargada = NSPredicate(format: "value == %@ OR label == %@", "ColumnasInicio.txt", "ColumnasInicio.txt")
        wait(for: [XCTNSPredicateExpectation(predicate: cargada, object: entrada)], timeout: 30)
        app.buttons["accion-reducir"].click()
        let guardar = app.buttons["Guardar resultado…"]
        wait(for: [XCTNSPredicateExpectation(predicate: NSPredicate(format: "isEnabled == true"), object: guardar)], timeout: 60)
        let admitidas = Int((app.staticTexts["reduccion-Admitidas"].value as? String) ?? "") ?? 0
        XCTAssertTrue((73...729).contains(admitidas), "admitidas \(admitidas)")
    }
}
