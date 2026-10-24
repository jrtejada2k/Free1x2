import AppKit
import UniformTypeIdentifiers

/// `PickerHelper` (10 §1.6): paneles de abrir/guardar en la carpeta de trabajo o Documentos (`AccesoSandbox`). Extensiones con punto (`".txt"`); `"*"` = todos.
@MainActor
enum SelectorFicheros {
    static func abrir(_ extensiones: String...) async -> URL? {
        await abrirVarios(extensiones, multiple: false).first
    }

    /// `AbrirVariosAsync`.
    static func abrirVarios(_ extensiones: [String], multiple: Bool = true) async -> [URL] {
        let p = NSOpenPanel()
        p.allowsMultipleSelection = multiple
        p.canChooseDirectories = false
        p.directoryURL = AccesoSandbox.carpetaInicial
        if !extensiones.contains("*") { p.allowedContentTypes = tipos(extensiones) }
        guard await p.begin() == .OK else { return [] }
        AccesoSandbox.recordar(p.urls)
        return p.urls
    }

    /// `GuardarAsync(nombreSugerido, (etiqueta, ext)…)`: la primera extensión es la propuesta.
    static func guardar(_ nombre: String, _ tipos: [(etiqueta: String, ext: String)], en carpeta: URL? = nil) async -> URL? {
        let p = NSSavePanel()
        p.nameFieldStringValue = nombre
        p.directoryURL = carpeta ?? AccesoSandbox.carpetaInicial
        if !tipos.contains(where: { $0.ext == "*" }) {
            p.allowedContentTypes = Self.tipos(tipos.map(\.ext))
            p.allowsOtherFileTypes = true
        }
        return await p.begin() == .OK ? p.url : nil
    }

    /// Carpeta (`FolderPicker`).
    static func carpeta() async -> URL? {
        let p = NSOpenPanel()
        p.canChooseFiles = false
        p.canChooseDirectories = true
        p.directoryURL = AccesoSandbox.carpetaInicial
        guard await p.begin() == .OK, let u = p.url else { return nil }
        AccesoSandbox.recordar([u])
        return u
    }

    private static func tipos(_ extensiones: [String]) -> [UTType] {
        extensiones.compactMap { UTType(filenameExtension: String($0.drop { $0 == "." })) }
    }
}
