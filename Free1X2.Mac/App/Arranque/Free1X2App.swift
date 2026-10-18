import AppKit
import Free1X2CasosDeUso
import SwiftUI

/// Una sola ventana, como hoy (DP-05; ESPEC_15 §2): 1020 × 760, mínimo 900 × 600; cerrarla cierra la app.
@main
struct Free1X2App: App {
    @NSApplicationDelegateAdaptor(Delegado.self) private var delegado
    @State private var sesion = Sesion(entorno: .produccion(portapapeles: PortapapelesSistema()))

    var body: some Scene {
        Window("Free1X2", id: "principal") {
            Raiz()
                .frame(minWidth: 900, minHeight: 600)
                .environment(sesion)
        }
        .defaultSize(width: 1020, height: 760)
        .windowResizability(.contentMinSize)
        .commands { Menus(sesion: sesion) }
    }
}

final class Delegado: NSObject, NSApplicationDelegate {
    func applicationShouldTerminateAfterLastWindowClosed(_ sender: NSApplication) -> Bool { true }
}
