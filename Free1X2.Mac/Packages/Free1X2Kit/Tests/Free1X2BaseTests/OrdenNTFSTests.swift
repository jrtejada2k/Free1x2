import Free1X2Base
import Testing

/// DT-14: orden de NTFS (mayúsculas + UTF-16), del que depende `Directory.GetFiles` en el C#.
@Test func ordenNTFSIgnoraMayusculas() {
    // El orden ordinal pondría «Z» antes que «a»; NTFS no.
    #expect(["b", "Z", "a1", "B2", "A"].sorted(by: OrdenNTFS.precede) == ["A", "a1", "b", "B2", "Z"])
}
