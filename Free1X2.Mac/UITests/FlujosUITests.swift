import XCTest

/// Plan de pruebas funcionales (`docs/mac/PLAN_PRUEBAS_FUNCIONALES.md`), Fase 2: la app de verdad, pulsando sus controles,
/// con los datos aislados en una carpeta temporal (`-datos`) y el tema claro.
final class FlujosUITests: XCTestCase {
    /// La app con una carpeta de datos nueva.
    @MainActor private func lanzar() -> XCUIApplication {
        continueAfterFailure = false
        let app = XCUIApplication()
        let datos = FileManager.default.temporaryDirectory.appending(path: "Free1X2UI-\(UUID().uuidString)").path(percentEncoded: false)
        app.launchArguments = ["-datos", datos, "-tema", "Claro"]
        app.launch()
        return app
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
}
