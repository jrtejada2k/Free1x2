import Free1X2CasosDeUso
import SwiftUI

/// «Crear nuevos grupos» (pantalla 31, `CrearGruposFrm`).
struct VistaCrearGrupos: View {
    @Environment(Sesion.self) private var sesion
    @State private var numero = 0.0

    var body: some View {
        Pagina(destino: .crearGrupos) {
            Tarjeta("¿Cuántos grupos nuevos?") {
                CampoNumero(etiqueta: "Número de grupos", valor: $numero, minimo: 0, maximo: 9999)
            }
            HStack {
                Button("Crear") {
                    do throws(ErrorDominio) {
                        try CasosInicio.crearGrupos(&sesion.combinacion, cuantos: Int(FormatoNET.castInt32(numero)))
                        sesion.router.atras()
                    } catch {
                        sesion.alertas.inesperado(error)
                    }
                }
                .buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction).disabled(!(numero > 0))
                Button("Cancelar") { sesion.router.atras() }
            }
        }
    }
}
