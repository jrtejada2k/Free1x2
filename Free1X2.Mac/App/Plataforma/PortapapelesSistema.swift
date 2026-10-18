import AppKit
import Free1X2Base

/// Portapapeles de producción (ESPEC_11 §7): `NSPasteboard` general, siempre desde el hilo principal.
struct PortapapelesSistema: Portapapeles {
    func leerTexto() async -> String? {
        await MainActor.run { NSPasteboard.general.string(forType: .string) }
    }

    func escribirTexto(_ texto: String) async {
        await MainActor.run {
            NSPasteboard.general.clearContents()
            NSPasteboard.general.setString(texto, forType: .string)
        }
    }
}
