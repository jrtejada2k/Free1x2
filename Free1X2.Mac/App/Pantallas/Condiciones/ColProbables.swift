import Free1X2CasosDeUso
import SwiftUI

/// «Columnas Probables» (pantalla 19, `ColProbablesFrm`): editor con buffer, cinco pestañas y un único «Aceptar».
struct VistaColProbables: View {
    @Environment(Sesion.self) private var sesion
    @State private var e = EditorColumnasProbables()
    @State private var pestana = 0
    /// Retorno de Copiar datos o del Importador: entonces no se recarga del grupo (ESPEC_08 §19.1; DT-60).
    @State private var pendiente = Pendiente.ninguno

    enum Pendiente { case ninguno, copia, importacion }

    private static let pestanas = ["Columnas", "Relaciones I", "Relaciones II", "Relaciones III", "Control Fallos"]

    @discardableResult
    private func hacer(_ accion: (inout EditorColumnasProbables) throws -> Void) -> Bool {
        do {
            try accion(&e)
            return true
        } catch let d as ErrorDominio {
            sesion.alertas.inesperado(d)
        } catch {
            sesion.alertas.error(error)
        }
        return false
    }

    private func cargar() {
        let (n, d) = (sesion.numeroPartidos, sesion.configuracion.boleto.desplazamiento)
        guard let g = sesion.combinacion.grupos[safe: sesion.grupoEnEdicion] else {
            e = EditorColumnasProbables(numeroPartidos: n, desplazamiento: d)
            return
        }
        do throws(ErrorDominio) {
            e = try EditorColumnasProbables(g.condiciones.colProbables, numeroPartidos: n, desplazamiento: d)
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    /// `AplicarImportacionAsync` (`ColProbablesFrmPage.xaml.cs`): «Sí» es el botón por defecto.
    private func importar() async {
        let imp = sesion.traspasoCP.importadas ?? []
        sesion.traspasoCP.importadas = nil
        var modo = ModoImportacionCP.reemplazar
        if e.confirmarSustituir(imp),
            await sesion.alertas.confirmar(
                "¿Sustituir las columnas existentes por las importadas (manteniendo los rangos de aciertos y tolerancias)?",
                titulo: "Importar columnas")
        {
            modo = .sustituir
        } else if e.confirmarAgregar(imp),
            await sesion.alertas.confirmar(
                "¿Añadir las columnas importadas al final de las existentes?\n(Si se selecciona No, se sustituirán TODAS las columnas y sus rangos por las columnas importadas)",
                titulo: "Importar columnas")
        {
            modo = .agregar
        }
        let delFiltro = sesion.combinacion.grupos[safe: sesion.grupoEnEdicion]?.condiciones.colProbables.columnas ?? []
        hacer { try $0.importar(imp, modo: modo, delFiltro: delFiltro) }
    }

    private func aceptar() {
        guard sesion.combinacion.grupos.indices.contains(sesion.grupoEnEdicion) else { return sesion.router.atras() }
        do throws(ErrorDominio) {
            try sesion.combinacion.modificarGrupo(sesion.grupoEnEdicion) { g throws(ErrorDominio) in
                try e.aceptar(en: &g.condiciones.colProbables)
            }
            sesion.router.atras()
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    var body: some View {
        Pagina(destino: .colProbables) {
            Text(
                "Una Columna Probable es un segundo pronóstico sobre un grupo de partidos, expresando la posibilidad de que se den ciertos resultados."
            )
            .foregroundStyle(Color.textoSecundario)
            Picker("", selection: $pestana) {
                ForEach(Self.pestanas.indices, id: \.self) { Text(Self.pestanas[$0]).tag($0) }
            }
            .pickerStyle(.segmented).labelsHidden()
            switch pestana {
            case 0: columnas
            case 1: relaciones1
            case 2: relaciones2
            case 3: relaciones3
            default: controlFallos
            }
            Button("Aceptar", action: aceptar).buttonStyle(.borderedProminent).keyboardShortcut(.defaultAction)
        }
        .onAppear {
            switch pendiente {
            case .ninguno:
                cargar()
            case .copia:
                pendiente = .ninguno
                if let r = sesion.traspasoCP.copia { hacer { try $0.aplicarCopiaDatos(desde: r.desde, hasta: r.hasta) } }
                sesion.traspasoCP.copia = nil
            case .importacion:
                pendiente = .ninguno
                Task { await importar() }
            }
        }
        // `MaxLength=14` de la caja del pronóstico (lo cargado puede ser más largo, DEC-810).
        .onChange(of: e.pronostico) { viejo, nuevo in
            if nuevo.count > 14 && nuevo.count > viejo.count { e.pronostico = String(nuevo.prefix(max(14, viejo.count))) }
        }
    }

    // MARK: Pestañas

    private var columnas: some View {
        Group {
            Tarjeta("Columna probable") {
                CampoCondicion(etiqueta: "Pronóstico (14 signos: 1 / X / 2)", texto: $e.pronostico, ayuda: "Ej. 1X21X2111XX221")
                CampoCondicion(etiqueta: "Puntos", texto: $e.puntos)
            }
            Tarjeta("Rangos (valores 0..14)") {
                CampoCondicion(etiqueta: "Aciertos", texto: $e.aciertos, ayuda: "Ej. 0-14 ó 3,5,7")
                CampoCondicion(etiqueta: "Ac. Seguidos", texto: $e.aciertosSeguidos, ayuda: "Ej. 0-14 ó 3,5,7")
                CampoCondicion(etiqueta: "Fallos Seguidos", texto: $e.fallosSeguidos, ayuda: "Ej. 0-14 ó 3,5,7")
            }
            Tarjeta("Tolerancias locales") {
                CampoCondicion(etiqueta: "Aciertos", texto: $e.aciertosTol)
                CampoCondicion(etiqueta: "Ac. Seguidos", texto: $e.aciertosSeguidosTol)
                CampoCondicion(etiqueta: "Fallos Seguidos", texto: $e.fallosSeguidosTol)
                LabeledContent("Núm Tolerancias") {
                    HStack {
                        ForEach(0..<4, id: \.self) { Toggle(String($0), isOn: $e.tolerancias[$0]) }
                    }
                }
            }
            Tarjeta("Navegación") {
                HStack {
                    Button("|<") { hacer { try $0.primera() } }
                    Button("<<") { hacer { try $0.atras() } }
                    Button("<") { hacer { try $0.anterior() } }
                    Text(e.paginacion).monospacedDigit().frame(minWidth: 50)
                    Button(">") { hacer { try $0.siguiente() } }
                    Button(">>") { hacer { try $0.adelante() } }
                    Button(">|") { hacer { try $0.ultima() } }
                }
            }
            Tarjeta("Acciones") {
                HStack {
                    Button("Todos a Triple") { e.todosATriple() }
                    Button("Borrar Pronósticos") { e.borrarPronosticos() }
                    Button("Eliminar actual") { e.eliminarActual() }
                }
                HStack {
                    Button("Copiar datos") {
                        guard let r = e.prepararCopiaDatos() else { return }
                        sesion.traspasoCP.rango = r
                        sesion.traspasoCP.copia = nil
                        pendiente = .copia
                        sesion.router.navegar(.copiarDatosCP)
                    }
                    Button("Copiar Columnas") { sesion.router.navegar(.copiarCP) }
                    Button("Cambiar Puntuación") { sesion.router.navegar(.cambioPuntos) }
                    Button("Importador CPs") {
                        e.prepararImportacion()
                        sesion.traspasoCP.importadas = nil
                        pendiente = .importacion
                        sesion.router.navegar(.importadorCPs)
                    }
                    Button("Exportador CPs") {
                        var lista: [ColumnaProbableConfig] = []
                        guard hacer({ lista = try $0.prepararExportacion() }) else { return }
                        sesion.traspasoCP.exportar = lista
                        sesion.router.navegar(.exportadorCPs)
                    }
                }
            }
        }
    }

    private func navegacion(
        _ paginacion: String, anterior: @escaping () -> Void, siguiente: @escaping () -> Void, eliminar: @escaping () -> Void,
        primera: Bool
    ) -> some View {
        HStack {
            Text("Actual / Guardadas").foregroundStyle(Color.textoSecundario)
            Button("<", action: anterior).disabled(primera)
            Text(paginacion).monospacedDigit().frame(minWidth: 50)
            Button(">", action: siguiente)
            Button("Eliminar Actual", action: eliminar)
        }
    }

    private var relaciones1: some View {
        Tarjeta("Relaciones") {
            Text("Introducir datos en ambas casillas.").font(.callout).foregroundStyle(Color.textoSecundario)
            CampoCondicion(etiqueta: "Columnas", texto: $e.rel1Columnas)
            CampoCondicion(etiqueta: "Suma Aciertos", texto: $e.rel1SumaAciertos)
            CampoCondicion(etiqueta: "Recorrido", texto: $e.rel1Recorrido)
            CampoCondicion(etiqueta: "¿Cuántas Columnas?", texto: $e.rel1CuantasCP)
            CampoCondicion(etiqueta: "Número de Aciertos", texto: $e.rel1NumeroAciertos)
            navegacion(
                e.paginacionRel1, anterior: { hacer { try $0.rel1Anterior() } }, siguiente: { hacer { try $0.rel1Siguiente() } },
                eliminar: { e.rel1Eliminar() }, primera: e.rel1 == 0)
        }
    }

    private func selector(_ titulo: String, _ opciones: [String], _ valor: Binding<String>) -> some View {
        Picker(titulo, selection: valor) {
            ForEach(opciones, id: \.self) { Text($0).tag($0) }
        }
        .frame(maxWidth: 260)
    }

    private func bloqueRel2(_ titulo: String, _ ayuda: String, individual: Bool) -> some View {
        Tarjeta(titulo) {
            Text(ayuda).font(.callout).foregroundStyle(Color.textoSecundario)
            CampoCondicion(etiqueta: "Las columnas", texto: individual ? $e.rel2.columnasA2 : $e.rel2.columnasA)
            selector("Concepto", EditorColumnasProbables.conceptos, individual ? $e.rel2.concepto2 : $e.rel2.concepto)
            CampoCondicion(etiqueta: "Tendrán (nº aciertos)", texto: individual ? $e.rel2.aciertos2 : $e.rel2.aciertos)
            selector("Más / Menos", EditorColumnasProbables.masMenos, individual ? $e.rel2.cantidad2 : $e.rel2.cantidad)
            CampoCondicion(etiqueta: "Que las columnas", texto: individual ? $e.rel2.columnasB2 : $e.rel2.columnasB)
        }
    }

    private var relaciones2: some View {
        Group {
            bloqueRel2(
                "Sumas de AC/ACS/FS",
                "Los aciertos se refieren a la SUMA de AC, ACS o FS entre todas las columnas. (Introducir intervalo o columnas individuales: a-b ó a,c,d)",
                individual: false)
            bloqueRel2(
                "AC/ACS/FS individuales",
                "Los aciertos se refieren a los AC, ACS o FS INDIVIDUALES de CADA columna. (Introducir intervalo o columnas individuales: a-b ó a,c,d)",
                individual: true)
            navegacion(
                e.paginacionRel2, anterior: { hacer { try $0.rel2Anterior() } }, siguiente: { hacer { try $0.rel2Siguiente() } },
                eliminar: { hacer { $0.rel2Eliminar() } }, primera: e.indiceRel2 <= 1)
        }
    }

    private func agrupaciones(_ titulo: String, _ filas: Binding<[FilaAgrupacion]>) -> some View {
        Tarjeta(titulo) {
            Text("Número de agrupaciones permitidas, nº de elementos de la agrupación y aciertos.")
                .font(.callout).foregroundStyle(Color.textoSecundario)
            Grid(alignment: .leading, horizontalSpacing: 6, verticalSpacing: 4) {
                GridRow {
                    Text("Número").bold()
                    Text("Elementos").bold()
                    Text("Aciertos").bold()
                    Text("")
                }
                ForEach(filas) { $f in
                    GridRow {
                        TextField("", text: $f.numero, prompt: Text("0-2 ó 1,3")).textFieldStyle(.roundedBorder)
                        TextField("", text: $f.elementos, prompt: Text("2")).textFieldStyle(.roundedBorder).frame(width: 70)
                        TextField("", text: $f.aciertos, prompt: Text("0-14 ó 3,5")).textFieldStyle(.roundedBorder)
                        Button("Eliminar") { filas.wrappedValue.removeAll { $0.id == f.id } }
                    }
                }
            }
            Button("Añadir agrupación") { filas.wrappedValue.append(FilaAgrupacion()) }
        }
    }

    private var relaciones3: some View {
        Group {
            Tarjeta("Columnas") {
                CampoCondicion(etiqueta: "Columnas implicadas", texto: $e.rel3Columnas)
                selector("Concepto", EditorColumnasProbables.conceptos, $e.rel3Concepto)
            }
            Tarjeta("Escaleras") {
                CampoCondicion(etiqueta: "Total", texto: $e.rel3EscalerasTotal)
                CampoCondicion(etiqueta: "Ascendentes", texto: $e.rel3EscalerasAsc)
                CampoCondicion(etiqueta: "Descendentes", texto: $e.rel3EscalerasDesc)
            }
            Tarjeta("Sándwichs") { CampoCondicion(etiqueta: "Total", texto: $e.rel3Sandwichs) }
            agrupaciones("Agrupaciones Paso Fijo", $e.agrupacionesPasoFijo)
            agrupaciones("Agrupaciones Solapadas", $e.agrupacionesSolapadas)
            navegacion(
                e.paginacionRel3, anterior: { hacer { try $0.rel3Anterior() } }, siguiente: { hacer { try $0.rel3Siguiente() } },
                eliminar: { hacer { $0.rel3Eliminar() } }, primera: e.indiceRel3 <= 1)
        }
    }

    private var controlFallos: some View {
        Group {
            Tarjeta("Control de fallos") {
                CampoCondicion(
                    etiqueta: "Número de Fallos de Controles permitidos", texto: $e.numeroFallosControles, ayuda: "Ej. 0-2 ó 1,2")
            }
            Tarjeta("Controles (Columnas / Tolerancias / Aciertos)") {
                Text("Cada control comprueba, sobre las columnas indicadas, el nº de aciertos de las tolerancias dadas.")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                Grid(alignment: .leading, horizontalSpacing: 6, verticalSpacing: 4) {
                    GridRow {
                        Text("Columnas").bold()
                        Text("Tolerancias").bold()
                        Text("Aciertos").bold()
                        Text("")
                    }
                    ForEach($e.controlesFallo) { $f in
                        GridRow {
                            TextField("", text: $f.columnas, prompt: Text("1-3 ó 1,2")).textFieldStyle(.roundedBorder)
                            TextField("", text: $f.tolerancias, prompt: Text("AC, ACS, FS")).textFieldStyle(.roundedBorder)
                            TextField("", text: $f.aciertos, prompt: Text("0-2 ó 1")).textFieldStyle(.roundedBorder)
                            Button("Eliminar") { e.controlesFallo.removeAll { $0.id == f.id } }
                        }
                    }
                }
                Button("Añadir control") { e.controlesFallo.append(FilaControlFallos()) }
            }
        }
    }
}

/// «Copiar Datos» (pantalla 30, `CopiarDatosCPFrm`): rango `desde…hasta` encadenado.
struct VistaCopiarDatosCP: View {
    @Environment(Sesion.self) private var sesion
    @State private var desde = 1.0
    @State private var hasta = 1.0
    @State private var maximo = 1.0
    @State private var cargado = false

    var body: some View {
        Pagina(destino: .copiarDatosCP) {
            Text(
                "Selecciona el rango de columnas a copiar indicando desde qué columna hasta qué columna. Los límites están encadenados para que el rango siempre sea válido."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Columnas") {
                HStack(spacing: 20) {
                    CampoNumero(etiqueta: "desde", valor: $desde, minimo: 1, maximo: hasta)
                    CampoNumero(etiqueta: "hasta", valor: $hasta, minimo: desde, maximo: maximo)
                }
            }
            HStack {
                Button("Copiar") {
                    sesion.traspasoCP.copia = (Int(FormatoNET.castInt32(desde)), Int(FormatoNET.castInt32(hasta)))
                    sesion.router.atras()
                }
                .buttonStyle(.borderedProminent).keyboardShortcut(.defaultAction)
                Button("Cancelar") {
                    sesion.traspasoCP.copia = nil
                    sesion.router.atras()
                }
                Text("Preparado").font(.callout).foregroundStyle(Color.textoSecundario)
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            let r = sesion.traspasoCP.rango
            (maximo, hasta) = (Double(r.hasta), Double(r.hasta))
            desde = Double(min(r.desde, r.hasta))
        }
    }
}

/// «Copiar Columnas» (pantalla 29, `CopiarCPFrm`): CPs del grupo en edición a otros grupos.
struct VistaCopiarCP: View {
    @Environment(Sesion.self) private var sesion
    @State private var columnas = ""
    @State private var seleccion: Set<Int> = []
    @State private var estado = "Indica las columnas y el grupo destino."

    private func copiar() {
        do throws(ErrorDominio) {
            let r = try CasosCondiciones.copiarColumnasCP(
                columnas, destinos: seleccion, grupoEnEdicion: sesion.grupoEnEdicion, grupoPantalla: sesion.grupoPantalla,
                en: &sesion.combinacion)
            estado = r.estado
            if r.cerrar { sesion.router.atras() }
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    var body: some View {
        Pagina(destino: .copiarCP) {
            Text(
                "Copia columnas probables a uno o varios grupos del boleto. Indica las columnas por índice (5), lista (1,3,5) o rango (1-5) y elige los grupos destino."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Columnas a copiar") { CampoCondicion(etiqueta: "Columnas", texto: $columnas, ayuda: "Ej.: 5  |  1,3,5  |  1-5") }
            Tarjeta("Grupos destino") {
                Text("Selecciona uno o varios grupos a los que copiar las columnas.").font(.callout).foregroundStyle(Color.textoSecundario)
                ForEach(sesion.combinacion.grupos.indices, id: \.self) { g in
                    Toggle(
                        g == 0 ? "Boleto Base" : String(g),
                        isOn: Binding(
                            get: { seleccion.contains(g) },
                            set: { if $0 { seleccion.insert(g) } else { seleccion.remove(g) } }))
                }
            }
            HStack {
                Button("Copiar", action: copiar).buttonStyle(.borderedProminent).keyboardShortcut(.defaultAction)
                Button("Crear grupos") { sesion.router.navegar(.crearGrupos) }
                Button("Cancelar") { sesion.router.atras() }
                Text(estado).font(.callout).foregroundStyle(Color.textoSecundario)
            }
        }
    }
}

/// «Importador de Columnas Probables» (pantalla 67, `ImportadorCPsFrm`): deja las CPs para «Columnas Probables».
struct VistaImportadorCPs: View {
    @Environment(Sesion.self) private var sesion
    @State private var estado = "Sin importaciones realizadas."
    @State private var numero = "0 columnas importadas."

    private func importar(clm: Bool) async {
        guard let url = await SelectorFicheros.abrir(clm ? ".clm" : ".txt", "*") else { return }
        let datos: Data
        do throws(ErrorDominio) {
            datos = try sesion.entorno.archivos.leer(url)
        } catch {
            return sesion.alertas.inesperado(error)
        }
        let n = sesion.numeroPartidos
        let leidas =
            clm ? CasosCondiciones.importarCPsClm(datos, numeroPartidos: n) : CasosCondiciones.importarCPsSimples(datos, numeroPartidos: n)
        guard let lista = leidas else {
            sesion.alertas.error(CasosCondiciones.mensajeFormatoCPs)
            estado = "Error: formato de columnas incorrecto."
            return
        }
        sesion.traspasoCP.importadas = lista
        numero = lista.count == 1 ? "1 columna importada." : "\(lista.count) columnas importadas."
        estado = "Importadas \(lista.count) columnas \(clm ? "con aciertos" : "simples") desde \(url.lastPathComponent)."
        sesion.router.atras()
    }

    var body: some View {
        Pagina(destino: .importadorCPs) {
            Text("Importa Columnas Probables (CPs) desde un archivo a un grupo temporal.").foregroundStyle(Color.textoSecundario)
            Tarjeta("Columnas simples") {
                Text(
                    "Archivos de texto que contienen únicamente columnas (*.txt). Cada columna se importa como una CP con los aciertos calculados sobre el histórico."
                )
                .font(.callout).foregroundStyle(Color.textoSecundario)
                Button("Importar CPs Simples (sólo columnas *.txt)") { Task { await importar(clm: false) } }
            }
            Tarjeta("Columnas con aciertos") {
                Text(
                    "Archivos *.clm con formato pronóstico#aciertos#aciertos seguidos#fallos seguidos. Se importan las CPs respetando sus estadísticas guardadas."
                )
                .font(.callout).foregroundStyle(Color.textoSecundario)
                Button("Importar CPs con Ac, Acs, Fs (*.clm)") { Task { await importar(clm: true) } }
            }
            Tarjeta("Estado") {
                Text(estado)
                Text(numero).foregroundStyle(Color.textoSecundario)
            }
            Button("Cancelar") {
                sesion.traspasoCP.importadas = nil
                sesion.router.atras()
            }
        }
    }
}

/// «Exportador de Columnas Probables» (pantalla 48, `ExportadorCPsFrm`): `.txt` o `.clm`.
struct VistaExportadorCPs: View {
    @Environment(Sesion.self) private var sesion
    @State private var estado = "Elige el formato de exportación de las columnas probables."

    private func exportar(clm: Bool) async {
        let lista = sesion.traspasoCP.exportar
        let tipo = clm ? (etiqueta: "Columnas Con Aciertos", ext: ".clm") : (etiqueta: "Columnas Simples", ext: ".txt")
        guard let url = await SelectorFicheros.guardar("columnas", [tipo, (etiqueta: "Todos los archivos", ext: ".")]) else { return }
        let clase = clm ? "con aciertos" : "simples"
        do throws(ErrorDominio) {
            // Sin CPs el `.txt` no se crea (el C# solo abre el fichero al escribir la primera).
            if let datos = clm ? CasosCondiciones.datosCPsClm(lista) : CasosCondiciones.datosCPsSimples(lista) {
                try sesion.entorno.archivos.escribir(datos, en: url, atomico: true)
            }
            estado = "Exportadas \(lista.count) columnas \(clase) a \(url.lastPathComponent)."
        } catch {
            sesion.alertas.error("No se pudieron exportar las columnas \(clase) a:\n\(url.path)\n\n\(error.mensajeUsuario)")
            estado = "No se pudo exportar: " + error.mensajeUsuario
        }
    }

    var body: some View {
        let n = sesion.traspasoCP.exportar.count
        Pagina(destino: .exportadorCPs) {
            Text(
                "Exporta las columnas probables a fichero. Elige un formato: sólo las columnas (.txt) o las columnas con sus aciertos, aciertos seguidos y fallos seguidos (.clm)."
            )
            .foregroundStyle(Color.textoSecundario)
            Text(n == 1 ? "1 columna para exportar." : "\(n) columnas para exportar.")
            Tarjeta("Exportar simples (*.txt)") {
                Text("Guarda únicamente los pronósticos de cada columna probable en un fichero de texto.")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                Button("Exportar CPs simples (sólo columnas *.txt)") { Task { await exportar(clm: false) } }
            }
            Tarjeta("Exportar con aciertos (*.clm)") {
                Text("Guarda cada columna con sus aciertos (Ac), aciertos seguidos (Acs) y fallos seguidos (Fs).")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                Button("Exportar CPs con Ac, Acs, Fs (*.clm)") { Task { await exportar(clm: true) } }
            }
            HStack {
                Button("Cancelar") { sesion.router.atras() }
                Text(estado).font(.callout).foregroundStyle(Color.textoSecundario)
            }
        }
    }
}
