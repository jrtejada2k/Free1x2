/// Orden en que NTFS devuelve los ficheros de una carpeta, del que depende `Directory.GetFiles` en el C# (DT-14):
/// nombre en mayúsculas, comparado por unidades UTF-16. APFS no garantiza orden, así que se impone siempre.
public enum OrdenNTFS {
    public static func precede(_ a: String, _ b: String) -> Bool {
        a.uppercased().utf16.lexicographicallyPrecedes(b.uppercased().utf16)
    }
}
