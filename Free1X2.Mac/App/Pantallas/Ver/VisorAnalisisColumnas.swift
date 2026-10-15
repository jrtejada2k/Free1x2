import Free1X2CasosDeUso
import SwiftUI

/// «Visor de análisis de columnas» (pantalla 102, `VisorAnalisisColumnasFrm`): solo lectura, por secciones.
struct VistaVisorAnalisisColumnas: View {
    @Environment(Sesion.self) private var sesion
    @State private var contenedor: ContenedorAnalisisGlobal?
    @State private var secciones: [SeccionAnalisis] = []
    @State private var seleccion: String?
    @State private var cargado = false

    private var actual: SeccionAnalisis? { secciones.first { $0.clave == seleccion } }

    private func construir() {
        secciones = CasosVer.seccionesDeAnalisis(
            contenedor, opciones: sesion.configuracion.analisis, numeroPartidos: sesion.numeroPartidos)
    }

    var body: some View {
        Pagina(destino: .visorAnalisisColumnas) {
            Text("Visor de análisis de columnas por secciones").foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top, spacing: 16) {
                Tarjeta("Secciones") {
                    ForEach(secciones) { s in
                        Button {
                            seleccion = s.clave
                        } label: {
                            HStack {
                                Text(s.titulo)
                                Spacer()
                                Text("●").foregroundStyle(Color.exito)
                            }
                            .contentShape(Rectangle())
                        }
                        .buttonStyle(.plain)
                        .padding(4)
                        .background(seleccion == s.clave ? Color.accentColor.opacity(0.15) : .clear, in: .rect(cornerRadius: 4))
                    }
                    Button("Refrescar") {
                        let clave = seleccion
                        construir()
                        seleccion = CasosVer.seleccionTrasRefrescar(clave, en: secciones)
                    }
                }
                .frame(width: 260)
                Tarjeta(actual?.titulo ?? "Selecciona un análisis") {
                    Text(actual?.detalle ?? "No hay ningún análisis seleccionado.").foregroundStyle(Color.textoSecundario)
                    if let s = actual, s.tipo == .matrices {
                        ForEach(Array(s.bloques.enumerated()), id: \.offset) { _, b in
                            if !b.titulo.isEmpty { Text(b.titulo).bold() }
                            MatrizAnalisisView(matriz: b.matriz)
                        }
                    }
                    if let s = actual, s.tipo == .valoracion {
                        HStack(alignment: .top, spacing: 20) {
                            ForEach(s.valoracion, id: \.titulo) { c in
                                VStack(alignment: .leading) {
                                    Text(c.titulo).bold()
                                    Grid(alignment: .trailing, horizontalSpacing: 12, verticalSpacing: 2) {
                                        GridRow {
                                            Text("Valor").foregroundStyle(Color.textoSecundario)
                                            Text("Columnas").foregroundStyle(Color.textoSecundario)
                                        }
                                        ForEach(c.entradas, id: \.self) { e in
                                            GridRow {
                                                Text(e.valor).monospacedDigit()
                                                Text(e.columnas).monospacedDigit()
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            contenedor = sesion.visorAnalisis?.contenedor
            sesion.visorAnalisis = nil
            construir()
            seleccion = secciones.first?.clave
        }
    }
}
