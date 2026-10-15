import Free1X2CasosDeUso
import SwiftUI

/// «Descarga de boleto» (pantalla 32, `DescargaBoletoFrm`): jornada online (o la guardada) para los nombres de equipo.
struct VistaDescargaBoleto: View {
    @Environment(Sesion.self) private var sesion
    @State private var pais = PaisQuiniela.es
    @State private var mensaje = ""
    @State private var descargando = false
    @State private var cargado = false

    var body: some View {
        Pagina(destino: .descargaBoleto) {
            Text("Actualiza la jornada de la semana online para rellenar el boleto con los equipos reales.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta {
                Picker("País", selection: $pais) {
                    Text("España").tag(PaisQuiniela.es)
                    Text("México").tag(PaisQuiniela.mx)
                }
                .frame(maxWidth: 260)
                Button("Actualizar jornada") {
                    Task {
                        guard !descargando else { return }
                        mensaje = "Conectando con el servicio online…"
                        descargando = true
                        let r = await CasosArchivo.descargarJornada(pais, servicio: sesion.servicioOnline, entorno: sesion.entorno)
                        if let j = r.jornada { sesion.jornadaActual = j }
                        mensaje = r.mensaje
                        descargando = false
                    }
                }
                .buttonStyle(.borderedProminent).disabled(descargando)
                Text(mensaje).foregroundStyle(Color.textoSecundario)
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            pais = CasosArchivo.paisInicial(entorno: sesion.entorno)
        }
    }
}
