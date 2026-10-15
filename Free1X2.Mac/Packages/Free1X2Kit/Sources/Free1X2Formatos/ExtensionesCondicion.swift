import Free1X2Motor

// C#: Free1X2.WinUI/Views/Ported/*FrmViewModel.cs (`ExtensionesAbrir`/`TiposGuardar`); 07 §1 F03 (07:294)

extension TipoFiltro {
    /// Extensión propia del fichero de condición (F03). Cada pantalla ofrece además `.xml`, con el mismo contenido.
    public var extensionCondicion: String {
        switch self {
        case .noVariantes: "vx2"
        case .signosSeguidos: "seg"
        case .interrupciones: "int"
        case .dibujos: "dbj"
        case .pesosNumericos: "pes"
        case .valoracionSignos: "valor"
        case .distancias: "dist"
        case .contactos: "cont"
        case .gruposEquipos: "geq"
        case .formatosSignos: "fmt"
        case .formatos123: "123"
        case .simetrias: "sim"
        case .diferencias: "dif"
        case .colProbables: "cps"
        }
    }
}

/// Extensión de las condiciones If-Then (F03), que no son un filtro de grupo.
public let extensionCondicionIfThen = "if"
