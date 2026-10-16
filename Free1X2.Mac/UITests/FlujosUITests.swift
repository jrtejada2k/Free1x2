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

    /// Menú de la app (Free1X2) ▸ `ruta`.
    @MainActor private func menu(_ app: XCUIApplication, _ ruta: String...) {
        app.menuBars.menuBarItems["Free1X2"].click()
        for (i, t) in ruta.enumerated() {
            let item = app.menuBars.menuItems[t].firstMatch
            XCTAssertTrue(item.waitForExistence(timeout: 5), t)
            if i < ruta.count - 1 { item.hover() } else { item.click() }
        }
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
        app.buttons["Guardar"].click()
        let aceptar = app.buttons["Aceptar"].firstMatch
        if aceptar.waitForExistence(timeout: 5) { aceptar.click() }
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
}
