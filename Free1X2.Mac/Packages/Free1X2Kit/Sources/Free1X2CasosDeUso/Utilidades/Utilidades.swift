// C#: Views/Ported/GEPTFrmViewModel.cs (09 pantalla 56; CU-B23).

public enum CasosUtilidades {
    /// `Traductor(primera, segunda)`: pronóstico por los dos últimos enfrentamientos (G/E/P); otro ⇒ `" "`.
    public static func traducirGEPT(_ primera: String, _ segunda: String) -> String {
        switch (primera + segunda).uppercased() {
        case "GG", "PP": "12"
        case "GE", "EP": "1X"
        case "GP": "1"
        case "EG", "PE": "X2"
        case "EE": "X"
        case "PG": "2"
        default: " "
        }
    }
}
