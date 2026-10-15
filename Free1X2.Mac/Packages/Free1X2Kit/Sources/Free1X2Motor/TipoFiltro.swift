// C#: Free1X2.Domain/MotorCalculo/Grupo.cs:94-133 (01 §4.2)

/// Los 14 filtros de un grupo, en el orden fijo de `Grupo.filtros`. El `rawValue` es el `NombreFiltro` del C#.
public enum TipoFiltro: String, Sendable, Codable, CaseIterable {
    case noVariantes = "NoVariantes"
    case valoracionSignos = "ValoracionSignos"
    case signosSeguidos = "SignosSeguidos"
    case interrupciones = "NoInterrupciones"
    case dibujos = "Dibujos"
    case pesosNumericos = "PesosNumericos"
    case distancias = "Distancias"
    case gruposEquipos = "GruposEquipos"
    case contactos = "Contactos"
    case formatosSignos = "FormatosSignos"
    case colProbables = "ColProbables"
    case formatos123 = "Formatos123"
    case simetrias = "Simetrias"
    case diferencias = "Diferencias"
}
