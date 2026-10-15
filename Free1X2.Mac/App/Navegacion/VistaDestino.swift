import SwiftUI

/// Una vista por `Destino` (ESPEC_15 §7).
struct VistaDestino: View {
    let destino: Destino

    var body: some View {
        switch destino {
        case .inicio: VistaInicio()
        case .acercaDe: VistaAcercaDe()
        case .ayuda: VistaAyuda()
        case .creditos: VistaCreditos()
        case .configuracion: VistaConfiguracion()
        case .configuracionAnalisis: VistaConfiguracionAnalisis()
        case .cambioPuntos: VistaCambioPuntos()
        default: PantallaPendiente(destino: destino)
        }
    }
}

/// ponytail: hueco de las pantallas que aún no están portadas (M5 en curso); desaparece con la última.
struct PantallaPendiente: View {
    let destino: Destino

    var body: some View {
        Pagina(destino: destino) {
            ContentUnavailableView(destino.titulo, systemImage: "hammer", description: Text(destino.clase))
        }
    }
}
