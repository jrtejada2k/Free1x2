import XCTest

/// Plan de pruebas funcionales (`docs/mac/PLAN_PRUEBAS_FUNCIONALES.md`), Fase 2: la app de verdad, pulsando sus controles,
/// con los datos aislados en una carpeta temporal (`-datos`) y el tema claro.
final class FlujosUITests: XCTestCase {
    /// La app con una carpeta de datos nueva (o la indicada, para comprobar lo que persiste entre arranques).
    @MainActor private func lanzar(datos: String? = nil) -> XCUIApplication {
        continueAfterFailure = false
        let app = XCUIApplication()
        let carpeta =
            datos ?? FileManager.default.temporaryDirectory.appending(path: "Free1X2UI-\(UUID().uuidString)").path(percentEncoded: false)
        app.launchArguments = ["-datos", carpeta, "-tema", "Claro"]
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
        for _ in 0..<10 where !e.isHittable { app.scrollViews.firstMatch.scroll(byDeltaX: 0, deltaY: -300) }
        e.click()
    }

    /// Menú de la app (Free1X2) ▸ `ruta`.
    @MainActor private func menu(_ app: XCUIApplication, _ ruta: String...) {
        app.menuBars.menuBarItems["Free1X2"].click()
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
        app.buttons["Configuración"].firstMatch.click()
        let fijos = app.textFields["Fijos"]
        XCTAssertTrue(fijos.waitForExistence(timeout: 10))
        XCTAssertEqual(fijos.value as? String, "6")
        fijos.doubleClick()
        fijos.typeKey("a", modifierFlags: .command)
        fijos.typeText("7\r")
        pulsar(app, app.buttons["Guardar"])
        // Aviso «Configuración guardada»: «Aceptar» es su botón por defecto (el de la Touch Bar no se puede pulsar).
        _ = app.dialogs.firstMatch.waitForExistence(timeout: 5)
        app.typeKey(.return, modifierFlags: [])
        salir(app)
        app = lanzar(datos: datos)
        defer { app.terminate() }
        app.buttons["Configuración"].firstMatch.click()
        XCTAssertTrue(app.textFields["Fijos"].waitForExistence(timeout: 10))
        XCTAssertEqual(app.textFields["Fijos"].value as? String, "7")
    }

    /// F45: ocultar la barra «Utilidades» y reabrir: sigue oculta (se guarda en `parametros.free1x2` al salir).
    @MainActor func testF45BarrasPersisten() {
        let datos = FileManager.default.temporaryDirectory.appending(path: "Free1X2UI-\(UUID().uuidString)").path(percentEncoded: false)
        var app = lanzar(datos: datos)
        XCTAssertTrue(app.buttons["Tramificar"].waitForExistence(timeout: 10))
        menu(app, "Barras de herramientas", "Utilidades")
        XCTAssertFalse(app.buttons["Tramificar"].waitForExistence(timeout: 3))
        salir(app)
        app = lanzar(datos: datos)
        defer { app.terminate() }
        XCTAssertTrue(app.buttons["Configuración"].firstMatch.waitForExistence(timeout: 10))
        XCTAssertFalse(app.buttons["Tramificar"].exists)
    }

    /// F46: con el idioma en inglés, Créditos se muestra en inglés (solo esa pantalla está traducida, DP-07).
    @MainActor func testF46Idioma() {
        let app = lanzar()
        defer { app.terminate() }
        XCTAssertTrue(app.buttons["Configuración"].firstMatch.waitForExistence(timeout: 10))
        menu(app, "Idioma", "English")
        menu(app, "Créditos…")
        XCTAssertTrue(app.staticTexts["Free1X2 Team"].waitForExistence(timeout: 10))
        menu(app, "Idioma", "Español")
    }

    /// F13: Escrutinios ▸ JD-01 contra la columna de unos, rango 10–14 ⇒ 14: 1, 13: 12, 12: 60, 11: 160, 10: 240.
    @MainActor func testF13Escrutinio() throws {
        let ruta = try fichero("jd01.txt", Self.seisTriples)
        let app = lanzar()
        defer { app.terminate() }
        app.buttons["Escrutinio"].firstMatch.click()
        let elegir = app.buttons["Seleccionar ficheros…"]
        XCTAssertTrue(elegir.waitForExistence(timeout: 10))
        elegir.click()
        panel(app, ruta: ruta)
        XCTAssertTrue(app.staticTexts["jd01.txt"].waitForExistence(timeout: 10))
        let ganadora = app.textFields["campo-Columna ganadora"]
        ganadora.click()
        ganadora.typeText(String(repeating: "1", count: 14))
        app.buttons["Escrutar"].firstMatch.click()
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
        app.buttons["Reducir"].firstMatch.click()
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
        app.buttons["Gestión de Equipos"].firstMatch.click()
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
        XCTAssertTrue(app.dialogs.firstMatch.waitForExistence(timeout: 10), "se esperaba el aviso de error")
        app.typeKey(.return, modifierFlags: [])
        XCTAssertTrue(app.staticTexts["Fijos: 0 - Dobles: 0 - Triples: 14"].waitForExistence(timeout: 10))
        XCTAssertTrue(app.buttons["Calcular columnas"].isHittable)
    }
}
