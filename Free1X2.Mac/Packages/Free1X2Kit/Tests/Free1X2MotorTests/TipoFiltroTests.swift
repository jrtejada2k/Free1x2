import Free1X2Motor
import Testing

/// 01 §4.2: orden fijo de `Grupo.filtros` y `NombreFiltro` del C# (`MC/Grupo.cs:94-133`).
@Test func losCatorceFiltrosEnElOrdenDeGrupo() {
    #expect(
        TipoFiltro.allCases.map(\.rawValue) == [
            "NoVariantes", "ValoracionSignos", "SignosSeguidos", "NoInterrupciones", "Dibujos", "PesosNumericos",
            "Distancias", "GruposEquipos", "Contactos", "FormatosSignos", "ColProbables", "Formatos123", "Simetrias",
            "Diferencias",
        ])
}
