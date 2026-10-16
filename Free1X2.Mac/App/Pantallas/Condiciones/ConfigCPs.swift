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
                .buttonStyle(EstiloPrincipal())
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

/// «Generador de Columnas Probables» (pantalla 59, `GenerarCPs`): un fichero de CPs por tipo configurado.
struct VistaGenerarCPs: View {
    @Environment(Sesion.self) private var sesion
    @State private var valoracion: [[Double]] = []
    @State private var jornada = 0.0
    @State private var estado = ""

    private func generar() async {
        let j = Int(FormatoNET.castInt32(jornada))
        guard j > 0 else {
            estado = "Especifique un número de jornada."
            return sesion.alertas.error("Especifique un número de jornada")
        }
        estado = "Generando columnas..."
        do throws(ErrorDominio) {
            let n = try await CasosCondiciones.generarCPs(valoracion, jornada: j, entorno: sesion.entorno)
            estado = "Fichero(s) guardado(s) correctamente en la carpeta Condiciones (\(n))."
            sesion.alertas.info("Fichero(s) guardado(s) correctamente en la carpeta Condiciones")
        } catch {
            estado = "Error al generar las columnas: " + error.mensajeUsuario
            sesion.alertas.error(estado)
        }
    }

    private func importar() async {
        guard let url = await SelectorFicheros.abrir(".txt", "*") else { return }
        do throws(ErrorDominio) {
            let r = try CasosCondiciones.importarValoracion(url, numeroPartidos: sesion.numeroPartidos, entorno: sesion.entorno)
            if let aviso = r.aviso { sesion.alertas.error(aviso) }
            valoracion = r.valores
            estado = "Valoración importada de \(url.lastPathComponent)."
        } catch {
            estado = "Error al importar la valoración: " + error.mensajeUsuario
            sesion.alertas.error(estado)
        }
    }

    var body: some View {
        Pagina(destino: .generarCPs) {
            Text(
                "Introduce la valoración 1 / X / 2 (en porcentaje) de cada partido y genera las columnas probables para una jornada."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                Tarjeta("Valoración") {
                    PorcentajesGrid(valores: $valoracion)
                    Button("Importar %") { Task { await importar() } }
                }
                VStack(spacing: 10) {
                    Tarjeta("Generar columnas") {
                        CampoNumero(etiqueta: "Jornada", valor: $jornada, minimo: 0, ancho: 70)
                        Button("Generar (OK)") { Task { await generar() } }.buttonStyle(EstiloPrincipal())
                    }
                    Tarjeta("Herramientas") {
                        Button("Configurar columnas") { sesion.router.navegar(.configCPs) }
                        Button("Separador de porcentajes") { sesion.router.navegar(.filtroPorcenJB) }
                        Button("CPs por diferencias") { sesion.router.navegar(.generadorCPSDiferencias) }
                    }
                }
            }
            Text(estado).foregroundStyle(Color.textoSecundario)
        }
        .onAppear { if valoracion.isEmpty { valoracion = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos) } }
    }
}

/// «Generador de CPs por diferencias» (pantalla 58, `GeneradorCPSDiferencias`).
struct VistaGeneradorCPSDiferencias: View {
    @Environment(Sesion.self) private var sesion
    @State private var inicial = ""
    @State private var alternativa = ""
    @State private var dobles = true
    @State private var diferencias = 2
    @State private var destino = ""
    @State private var estado = ""

    private func columna(_ titulo: String, _ ejemplo: String, _ texto: Binding<String>) -> some View {
        HStack {
            Text(titulo).frame(width: 260, alignment: .leading)
            TextField(ejemplo, text: texto).textFieldStyle(.roundedBorder).monospaced().frame(width: 180)
                .onChange(of: texto.wrappedValue) { _, v in
                    let u = String(v.uppercased().prefix(14))
                    if u != v { texto.wrappedValue = u }
                }
        }
    }

    private func generar() {
        guard CasosCondiciones.columnaCPValida(inicial) else {
            estado = "La cadena inicial no es válida."
            return sesion.alertas.error("La cadena inicial no es válida")
        }
        guard CasosCondiciones.columnaCPValida(alternativa) else {
            estado = "La cadena alternativa no es válida."
            return sesion.alertas.error("La cadena alternativa no es válida")
        }
        guard !destino.isEmpty else {
            estado = "No se ha indicado el fichero."
            return sesion.alertas.error("No se ha indicado el fichero.")
        }
        estado = "Generando..."
        let filas = CasosCondiciones.cpsPorDiferencias(inicial, alternativa, dobles: dobles, diferencias: diferencias)
        do throws(ErrorDominio) {
            if let d = ArchivoColumnasTexto.datosConComa(filas) {
                try sesion.entorno.archivos.escribir(d, en: URL(filePath: destino), atomico: false)
            }
            estado = "Columnas creadas."
            sesion.alertas.info("Columnas creadas")
        } catch {
            sesion.alertas.error("No se pudieron generar las columnas en:\n" + destino + "\n\n" + error.mensajeUsuario)
            estado = "No se pudieron crear las columnas: " + error.mensajeUsuario
        }
    }

    var body: some View {
        Pagina(destino: .generadorCPSDiferencias) {
            Text("Genera columnas a partir de una columna inicial y una alternativa, según el tipo y el número de diferencias.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta("Columnas") {
                columna("Columna inicial (14 signos 1/X/2)", "Ej. 1X21X21X21X21", $inicial)
                columna("Columna alternativa (14 signos 1/X/2)", "Ej. X12X12X12X12X", $alternativa)
            }
            Tarjeta("Opciones") {
                Picker("Tipo de columna", selection: $dobles) {
                    Text("Fijos").tag(false)
                    Text("Dobles").tag(true)
                }
                .fixedSize()
                Picker("Número de diferencias", selection: $diferencias) {
                    Text("1").tag(1)
                    Text("2").tag(2)
                }
                .fixedSize()
            }
            Tarjeta("Archivo destino") {
                HStack {
                    TextField("Ruta del archivo de salida (.txt)", text: .constant(destino)).textFieldStyle(.roundedBorder).disabled(true)
                    Button("Examinar…") {
                        Task {
                            if let u = await SelectorFicheros.guardar("doc1", [("Archivos de texto", ".txt"), ("Todos los archivos", ".")])
                            {
                                destino = u.path(percentEncoded: false)
                            }
                        }
                    }
                }
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Cancelar") { estado = "" }
                Button("Generar", action: generar).buttonStyle(EstiloPrincipal())
            }
        }
    }
}
