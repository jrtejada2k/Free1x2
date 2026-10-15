import SwiftUI

/// Pantalla de inicio. ponytail: hoy solo prueba el arranque; su contenido (boleto, rejilla de condiciones,
/// semáforos) es la plantilla T0 de ESPEC_15 §5 y llega en M5.
struct VistaInicio: View {
    var body: some View {
        ContentUnavailableView("Free1X2", systemImage: "soccerball", description: Text("Esqueleto de la app Mac (M2)."))
    }
}
