import Free1X2CasosDeUso
import SwiftUI

/// «Guardar Valoración» (pantalla 63, `GuardarValoracionFrm`): exporta la rejilla 1/X/2 a `.txt`.
struct VistaGuardarValoracion: View {
    @Environment(Sesion.self) private var sesion
    @State private var matriz: [[Double]] = []
    @State private var formato: Int16 = 3
    @State private var separador: Character = "\t"
    @State private var estado = ""

    private func aceptar() async {
        guard
            let url = await SelectorFicheros.guardar("valoracion", [("Valoraciones", ".txt"), ("Todos los archivos", ".")])
        else { return }
        do throws(ErrorDominio) {
            try CasosCondiciones.guardarValoracion(
                matriz, formato: formato, separador: formato == 1 ? "\t" : separador, en: url, numeroPartidos: sesion.numeroPartidos,
                entorno: sesion.entorno)
            estado = "Valoración guardada en \(url.lastPathComponent)."
            sesion.alertas.info("Valoración guardada correctamente")
        } catch {
            estado = "Error al guardar la valoración: " + error.mensajeUsuario
            sesion.alertas.error(estado)
        }
    }

    var body: some View {
        Pagina(destino: .guardarValoracion) {
            Text("Exporta las valoraciones a un fichero de texto eligiendo el formato y el separador de campos.")
                .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                Tarjeta("Valoraciones (1 / X / 2)") { PorcentajesGrid(valores: $matriz) }
                VStack(spacing: 10) {
                    Tarjeta("Formato del fichero") {
                        Picker("Disposición de los valores", selection: $formato) {
                            Text("3 valores por línea").tag(Int16(3))
                            Text("42 valores en una línea").tag(Int16(42))
                            Text("1 valor por línea").tag(Int16(1))
                        }
                        .pickerStyle(.radioGroup)
                    }
                    Tarjeta("Separador de campos") {
                        Picker("Separador", selection: $separador) {
                            Text("[Tabulador]").tag(Character("\t"))
                            Text("[Coma]").tag(Character(","))
                            Text("[Espacio en blanco]").tag(Character(" "))
                        }
                        .pickerStyle(.radioGroup).disabled(formato == 1)
                        Text("Carácter que separa los valores dentro de cada línea.").foregroundStyle(Color.textoSecundario)
                    }
                }
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Cancelar") { estado = "Cancelado." }
                Button("Aceptar") { Task { await aceptar() } }.buttonStyle(EstiloPrincipal())
            }
        }
        .onAppear {
            if let v = sesion.valoracionInicial {
                sesion.valoracionInicial = nil
                matriz = v
            } else if matriz.isEmpty {
                matriz = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos)
            }
        }
    }
}

/// «Valoraciones históricas» (pantalla 64, `HistoriaValoracionesFrm`): una jornada en el fichero histórico (formato 44).
struct VistaHistoriaValoraciones: View {
    @Environment(Sesion.self) private var sesion
    @State private var matriz: [[Double]] = []
    @State private var temporada = 2004.0
    @State private var jornada = 1.0
    @State private var fichero: URL?
    @State private var estado = ""

    private func guardar() {
        guard let fichero else { return }
        do throws(ErrorDominio) {
            let r = try CasosCondiciones.guardarEnHistorico(
                matriz, temporada: temporada, jornada: jornada, en: fichero, numeroPartidos: sesion.numeroPartidos, entorno: sesion.entorno)
            estado = "Guardada la jornada \(r.jornada) (\(r.temporada)) en \(fichero.lastPathComponent)."
            sesion.alertas.info("Valoraciones guardadas en el histórico")
        } catch {
            estado = "Error al guardar en el histórico: " + error.mensajeUsuario
            sesion.alertas.error(estado)
        }
    }

    var body: some View {
        Pagina(destino: .historiaValoraciones) {
            Text("Guarda las valoraciones 1X2 de la jornada en el fichero histórico.").foregroundStyle(Color.textoSecundario)
            Tarjeta("Jornada y temporada") {
                HStack {
                    Text("Se guardará la jornada")
                    Button("◄") { if jornada > 0 { jornada -= 1 } }
                    CampoNumero(etiqueta: "", valor: $jornada, minimo: 0, maximo: 43, ancho: 50).labelsHidden()
                    Button("►") { if jornada < 43 { jornada += 1 } }
                    Text("de la temporada")
                    Button("◄") { temporada -= 1 }
                    CampoNumero(etiqueta: "", valor: $temporada, ancho: 70).labelsHidden()
                    Button("►") { temporada += 1 }
                    Text("/ " + String(FormatoNET.castInt32(temporada) &+ 1))
                    Text("en el fichero histórico")
                }
            }
            Tarjeta("Valoraciones (1 / X / 2)") { PorcentajesGrid(valores: $matriz) }
            Tarjeta("Fichero histórico") {
                HStack {
                    Button("Abrir…") { Task { if let u = await SelectorFicheros.abrir(".txt", "*") { fichero = u } } }
                    Button("Nuevo…") {
                        Task {
                            if let u = await SelectorFicheros.guardar(
                                "valoraciones_historicas", [("Valoraciones históricas", ".txt"), ("Todos los archivos", ".")])
                            {
                                fichero = u
                            }
                        }
                    }
                    Text(fichero?.lastPathComponent ?? "Ningún fichero seleccionado").foregroundStyle(Color.textoSecundario)
                }
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Cancelar") { estado = "Cancelado." }
                Button("Guardar", action: guardar).buttonStyle(EstiloPrincipal()).disabled(fichero == nil)
            }
        }
        .onAppear {
            if let c = sesion.historiaValoraciones {
                sesion.historiaValoraciones = nil
                (matriz, temporada, jornada) = (c.valores, Double(c.temporada), Double(c.jornada))
                if let a = c.archivo { fichero = a }
            } else if matriz.isEmpty {
                matriz = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos)
            }
        }
    }
}
