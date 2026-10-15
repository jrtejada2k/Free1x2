import Free1X2CasosDeUso
import SwiftUI

/// «Configurar Columnas Probables» (pantalla 23, `ConfigCPsFrm`): reglas del generador de CPs por tipo.
struct VistaConfigCPs: View {
    @Environment(Sesion.self) private var sesion
    @State private var editor = EditorConfigCPs()
    @State private var tipo = ""
    @State private var filas: [FilaConfigCP] = []
    @State private var cargado = false

    var body: some View {
        Pagina(destino: .configCPs) {
            Text(
                "Define los tipos de columnas probables y, para cada tramo de puntuación, las reglas de forzado de fijos, dobles y triples."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Tipo de columna") {
                Picker("Tipo de CP", selection: $tipo) { ForEach(editor.nombres, id: \.self) { Text($0).tag($0) } }
                    .fixedSize()
                    .onChange(of: tipo) { filas = editor.filas(tipo) }
            }
            Tarjeta("Configuración de CPs") {
                Text("Cada fila representa un tramo: rango de puntuación admitido y reglas de forzado.").foregroundStyle(
                    Color.textoSecundario)
                Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 6) {
                    GridRow {
                        ForEach(["Desde", "Hasta", "Forzar Fijos", "Nº Fijos", "Forzar Dobles", "Nº Dobles", "Forzar Triples"], id: \.self)
                        {
                            Text($0).bold()
                        }
                    }
                    ForEach(filas.indices, id: \.self) { i in
                        GridRow {
                            CampoNumero(etiqueta: "", valor: $filas[i].desde, minimo: 0, ancho: 60).labelsHidden()
                            CampoNumero(etiqueta: "", valor: $filas[i].hasta, minimo: 0, ancho: 60).labelsHidden()
                            Toggle("", isOn: $filas[i].forzarFijos).labelsHidden()
                            CampoNumero(etiqueta: "", valor: $filas[i].numFijos, minimo: 0, ancho: 60).labelsHidden()
                            Toggle("", isOn: $filas[i].forzarDobles).labelsHidden()
                            CampoNumero(etiqueta: "", valor: $filas[i].numDobles, minimo: 0, ancho: 60).labelsHidden()
                            Toggle("", isOn: $filas[i].forzarTriples).labelsHidden()
                        }
                    }
                }
            }
            Tarjeta("Leyenda") {
                ForEach(
                    [
                        "Desde — Mínima puntuación para incluir el signo.", "Hasta — Máxima puntuación para incluir el signo.",
                        "Forzar Fijos — Sólo columnas de fijos (los más fijos).", "Nº Fijos — Nº de fijos en columna de fijos.",
                        "Forzar Dobles — Sólo columnas de dobles (los más dobles).", "Nº Dobles — Nº de dobles en columna de dobles.",
                        "Forzar Triples — Fuerza el triple si ningún signo se incluye en rango.",
                        "Si se hacen columnas de sólo fijos o dobles, se recomienda poner Desde=0 y Hasta=100.",
                    ], id: \.self
                ) { Text($0).font(.callout).foregroundStyle(Color.textoSecundario) }
            }
            HStack {
                Button("Volver") { sesion.router.atras() }
                Button("Guardar") {
                    do throws(ErrorDominio) {
                        try editor.guardar(filas, tipo: tipo, entorno: sesion.entorno)
                    } catch {
                        sesion.alertas.inesperado(error)
                    }
                }
                .buttonStyle(.borderedProminent)
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            do throws(ErrorDominio) {
                editor = try EditorConfigCPs.cargar(entorno: sesion.entorno)
                tipo = editor.nombres.first ?? ""
                filas = editor.filas(tipo)
            } catch {
                sesion.alertas.inesperado(error)
            }
        }
    }
}
