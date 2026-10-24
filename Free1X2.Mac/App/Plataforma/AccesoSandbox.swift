import AppKit

/// App Sandbox de la Mac App Store (DP-06 revisada; DT-99): el sandbox solo deja tocar lo elegido en un panel y solo
/// durante la sesión. Cada elección se recuerda con un *security-scoped bookmark* y se reabre al arrancar; la carpeta de
/// trabajo (se pide una vez) cubre las salidas que se escriben junto a la entrada y las rutas guardadas en los `.comb`.
/// Sin sandbox (Debug, CI, `.dmg`) no hace nada.
@MainActor
enum AccesoSandbox {
    private static let claveMarcas = "marcadores"
    private static let claveCarpeta = "carpetaTrabajo"
    private static let clavePedida = "carpetaTrabajoPedida"

    static var activo: Bool { ProcessInfo.processInfo.environment["APP_SANDBOX_CONTAINER_ID"] != nil }

    /// `~/Documents` del usuario: con sandbox `URL.documentsDirectory` apunta al contenedor.
    static var documentos: URL {
        guard activo, let pw = getpwuid(getuid()) else { return .documentsDirectory }
        return URL(filePath: String(cString: pw.pointee.pw_dir), directoryHint: .isDirectory)
            .appending(path: "Documents", directoryHint: .isDirectory)
    }

    static var carpetaTrabajo: URL? {
        UserDefaults.standard.string(forKey: claveCarpeta).map { URL(filePath: $0, directoryHint: .isDirectory) }
    }

    /// Donde abren los paneles: la carpeta de trabajo o Documentos.
    static var carpetaInicial: URL { carpetaTrabajo ?? documentos }

    /// Al arrancar: reabre lo recordado. Las marcas que no se resuelven (disco externo sin montar) se conservan.
    static func restaurar() {
        guard activo else { return }
        var marcas = UserDefaults.standard.dictionary(forKey: claveMarcas) as? [String: Data] ?? [:]
        for (ruta, datos) in marcas {
            var caducada = false
            guard let url = try? URL(resolvingBookmarkData: datos, options: .withSecurityScope, bookmarkDataIsStale: &caducada),
                url.startAccessingSecurityScopedResource()
            else { continue }
            if caducada, let nueva = try? url.bookmarkData(options: .withSecurityScope) { marcas[ruta] = nueva }
        }
        UserDefaults.standard.set(marcas, forKey: claveMarcas)
    }

    /// Recuerda lo elegido en un panel de abrir (ficheros o carpetas) para las próximas sesiones.
    // ponytail: lo elegido en «Guardar» aún no existe y no admite marca; vale para la sesión. Si hiciera falta entre
    // sesiones, marcarlo después de escribirlo.
    static func recordar(_ urls: [URL]) {
        guard activo, !urls.isEmpty else { return }
        var marcas = UserDefaults.standard.dictionary(forKey: claveMarcas) as? [String: Data] ?? [:]
        for u in urls {
            if let d = try? u.bookmarkData(options: .withSecurityScope) { marcas[u.path(percentEncoded: false)] = d }
        }
        UserDefaults.standard.set(marcas, forKey: claveMarcas)
    }

    /// Primer arranque con sandbox: pide la carpeta de trabajo una sola vez (luego, menú «Carpeta de trabajo…»).
    static func pedirSiFalta() async {
        guard activo, !UserDefaults.standard.bool(forKey: clavePedida) else { return }
        UserDefaults.standard.set(true, forKey: clavePedida)
        await elegirCarpetaTrabajo()
    }

    static func elegirCarpetaTrabajo() async {
        let p = NSOpenPanel()
        p.canChooseFiles = false
        p.canChooseDirectories = true
        p.canCreateDirectories = true
        p.directoryURL = carpetaInicial
        p.message = "Elige la carpeta donde guardas tus combinaciones y columnas. Free1X2 podrá leer y escribir en ella."
        p.prompt = "Usar esta carpeta"
        guard await p.begin() == .OK, let u = p.url else { return }
        recordar([u])
        UserDefaults.standard.set(u.path(percentEncoded: false), forKey: claveCarpeta)
    }
}
