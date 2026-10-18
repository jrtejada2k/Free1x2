import Free1X2Formatos
import Free1X2Motor
import Testing

/// F03 (07:294): cada filtro tiene su extensión de fichero de condición, distinta de las demás y de la de If-Then.
@Test func extensionesDeCondicion() {
    let extensiones = TipoFiltro.allCases.map(\.extensionCondicion)
    #expect(Set(extensiones + [extensionCondicionIfThen]).count == 15)
    #expect(TipoFiltro.noVariantes.extensionCondicion == "vx2")
    #expect(TipoFiltro.formatos123.extensionCondicion == "123")
    #expect(TipoFiltro.colProbables.extensionCondicion == "cps")
}
