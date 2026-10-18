import Free1X2CasosDeUso
import SwiftUI

/// «Escrutinios» (pantalla 45, `EscrutiniosFrm`): ficheros contra una ganadora, un fichero de referencia o el histórico.
struct VistaEscrutinios: View {
    @Environment(Sesion.self) private var sesion
    @State private var rango = ""
    @State private var registrar = false
    @State private var pestana = 0
    @State private var ficheros: [URL] = []
    @State private var ganadora = ""
    @State private var casillas: [String] = []
    @State private var referencia: URL?
    @State private var plantilla = ""
    @State private var carpeta: URL?
    @State private var digitosTemporada = 4
    @State private var digitosJornada = 2
    @State private var temporadas: [String] = []
    @State private var temporadasMarcadas: Set<String> = []
    @State private var estado = ""
    @State private var resultado: ResultadoEscrutinio?
    @State private var proyeccion: ProyeccionEscrutinio?
    @State private var seleccion: Set<Int> = []
    @State private var premiadas: [FilaPremiada]?
    @State private var escrutando = false
    @State private var tarea: Task<Void, Never>?
    @State private var cargado = false

    private var n: Int { sesion.numeroPartidos }

    private func modo() -> ModoEscrutinio {
        switch pestana {
        case 0: .simple(ganadora: ganadora)
        case 1: .contraFichero(referencia)
        default:
            .contraJornadas(
                plantilla: plantilla, carpeta: carpeta, digitosTemporada: digitosTemporada, digitosJornada: digitosJornada,
                temporadas: temporadasMarcadas.compactMap { Int($0) }.sorted())
        }
    }

    private func escrutar() {
        guard !escrutando else { return }
        let rangoAciertos: [Int]
        do throws(ErrorDominio) {
            rangoAciertos = try RangosHelper.obtenIntArray(rango)
        } catch {
            return sesion.alertas.inesperado(error)
        }
        let e = EntradaEscrutinio(modo: modo(), ficheros: ficheros, rangoAciertos: rangoAciertos, registrarPremiadas: registrar)
        do throws(ErrorDominio) {
            try CasosCombinacion.validarEscrutinio(e)
        } catch {
            return sesion.alertas.error(error.mensajeUsuario)
        }
        estado = "Calculando..."
        (resultado, proyeccion, premiadas, seleccion) = (nil, nil, nil, [])
        escrutando = true
        let (entorno, n) = (sesion.entorno, n)
        let inicio = entorno.reloj.ahora()
        tarea = Task {
            do throws(ErrorDominio) {
                let r = try await CasosCombinacion.escrutarColumnas(e, numeroPartidos: n, entorno: entorno)
                resultado = r
                proyeccion = CasosCombinacion.proyectarEscrutinio(r, rangoAciertos: rangoAciertos)
            } catch {
                sesion.alertas.error("No se pudo completar el escrutinio:\n\n" + error.mensajeUsuario)
            }
            estado = CasosCombinacion.estadoFinalEscrutinio(entorno.reloj.ahora().timeIntervalSince(inicio))
            escrutando = false
        }
    }

    private func grabarColumnas() async {
        let filas = proyeccion?.filas ?? []
        let columnas = filas.indices.filter { seleccion.contains($0) }.map { filas[$0].fila.columna }.filter { !$0.isEmpty }
        guard !columnas.isEmpty else { return sesion.alertas.info("No hay columnas seleccionadas que grabar.") }
        guard let url = await SelectorFicheros.guardar("columnas", [("Columnas", ".txt")]) else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(CasosCombinacion.datosColumnas(columnas, quitarComas: true), en: url, atomico: false)
            sesion.alertas.info("Guardadas \(columnas.count) columna(s) en \(url.lastPathComponent).")
        } catch {
            sesion.alertas.error("No se pudieron grabar las columnas en:\n\(url.path(percentEncoded: false))\n\n\(error.mensajeUsuario)")
        }
    }

    private func ponerGanadora(_ texto: String) {
        ganadora = texto
        casillas = CasosCombinacion.casillasGanadora(texto, numeroPartidos: n)
    }

    private func ponerCasilla(_ i: Int, _ s: String) {
        casillas[i] = s
        ganadora = CasosCombinacion.textoGanadora(casillas)
    }

    var body: some View {
        Pagina(destino: .escrutinios) {
            Text(
                "Escruta tus ficheros de columnas contra una columna ganadora, otro fichero de referencia o las jornadas del histórico, y obtén el número de aciertos."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Opciones del escrutinio") {
                HStack(spacing: 20) {
                    CampoCondicion(etiqueta: "Rango de aciertos a mostrar", texto: $rango, ayuda: "10-14").frame(maxWidth: 380)
                    Toggle("Registrar columnas premiadas", isOn: $registrar).toggleStyle(.switch)
                        .accessibilityIdentifier("registrar-premiadas")
                }
            }
            HStack(alignment: .top, spacing: 16) {
                VStack(alignment: .leading) {
                    Picker("", selection: $pestana) {
                        Text("Escrutinio simple").tag(0)
                        Text("Contra fichero").tag(1)
                        Text("Contra jornadas").tag(2)
                    }
                    .pickerStyle(.segmented).labelsHidden()
                    switch pestana {
                    case 0: simple
                    case 1: contraFichero
                    default: contraJornadas
                    }
                }
                Tarjeta("Ficheros a escrutar") {
                    Text("No aplica al modo «Contra jornadas»: ahí los ficheros se localizan con la plantilla de nombre.")
                        .font(.callout).foregroundStyle(Color.textoSecundario)
                    Button("Seleccionar ficheros…") {
                        Task {
                            let urls = await SelectorFicheros.abrirVarios([".txt"])
                            if !urls.isEmpty { ficheros = urls }
                        }
                    }
                    .disabled(pestana == 2)
                    ScrollView {
                        VStack(alignment: .leading, spacing: 2) {
                            ForEach(ficheros, id: \.self) { Label($0.lastPathComponent, systemImage: "doc.text").font(.callout) }
                        }
                        .frame(maxWidth: .infinity, alignment: .leading)
                        .padding(6)
                    }
                    .frame(height: 140)
                    .background(Color.superficieAlt.opacity(0.5), in: .rect(cornerRadius: 8))
                }
                .frame(width: 300)
            }
            HStack {
                Button("Escrutar", action: escrutar).buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction).disabled(escrutando)
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Posibles premios") {
                    sesion.ganadoraPosiblesPremios = ganadora
                    sesion.router.navegar(.posiblesPremios)
                }
            }
            if let p = proyeccion {
                resultados(p)
            } else {
                Tarjeta("Resultados") {
                    Text("Aún no hay resultados: elige los ficheros y pulsa «Escrutar».")
                        .foregroundStyle(Color.textoSecundario)
                        .frame(maxWidth: .infinity, minHeight: 120)
                        .background(Color.superficieAlt.opacity(0.5), in: .rect(cornerRadius: 8))
                    acciones
                }
            }
            if let premiadas { listaPremiadas(premiadas) }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            rango = "10-\(n)"
            casillas = Array(repeating: "*", count: n)
            temporadas = CasosCombinacion.cargarHistoricoJornadas(entorno: sesion.entorno).temporadas
        }
    }

    /// Botones de la lista de resultados (debajo de ella, como el original).
    private var acciones: some View {
        HStack {
            Button("Seleccionar todas") { seleccion = Set(proyeccion?.filas.indices ?? 0..<0) }
            Button("Deseleccionar todas") { seleccion = [] }
            Button("Grabar columnas…") { Task { await grabarColumnas() } }
            Button("Ver premiadas") { premiadas = CasosCombinacion.filasPremiadas(resultado?.premiadas ?? []) }
            Button("Ver premiadas…") {
                sesion.premiadasEntrada = CasosCombinacion.filasPremiadas(resultado?.premiadas ?? [])
                sesion.router.navegar(.columnasPremiadas)
            }
            .disabled(!(registrar && !(resultado?.premiadas.isEmpty ?? true)))
            Spacer()
            Button("Cancelar") {
                tarea?.cancel()
                sesion.router.atras()
            }
        }
    }

    // MARK: Pestañas

    private var simple: some View {
        Tarjeta {
            Text(
                "Escruta los ficheros contra una columna ganadora. Pulsa la cabecera de cada casilla para ciclar 1 / X / 2, o escribe la columna a mano."
            )
            .font(.callout).foregroundStyle(Color.textoSecundario)
            HStack(spacing: 4) {
                ForEach(casillas.indices, id: \.self) { i in
                    VStack(spacing: 2) {
                        Button(String(i + 1)) { ponerCasilla(i, CasosCombinacion.ciclarSigno(casillas[i])) }.buttonStyle(.plain)
                            .font(.caption).foregroundStyle(Color.textoSecundario)
                        TextField(
                            "",
                            text: Binding(
                                get: { casillas[i] },
                                set: { v in ponerCasilla(i, v.last.map { FormatoNET.mayusculas(String($0)) } ?? "") })
                        )
                        .multilineTextAlignment(.center).frame(width: 30).textFieldStyle(.roundedBorder)
                    }
                }
            }
            CampoCondicion(
                etiqueta: "Columna ganadora", texto: Binding(get: { ganadora }, set: { ponerGanadora(String($0.prefix(14))) }),
                ayuda: "Ej.: 1X21XX2*1X211")
            Button("Actualizar desde Free1X2.com") {
                ponerGanadora(String(repeating: "*", count: n))
                sesion.alertas.info(
                    "El servicio online de Free1X2.com no está disponible sin conexión: no se ha podido obtener la columna ganadora de la jornada actual."
                )
            }
            Text("Signos válidos — 1: Local · X: Empate · 2: Visitante · *: Comodín · S: Suspendido")
                .font(.caption).foregroundStyle(Color.textoSecundario)
        }
    }

    private var contraFichero: some View {
        Tarjeta {
            Text("Escruta los ficheros contra las columnas de otro fichero de referencia.").font(.callout)
                .foregroundStyle(Color.textoSecundario)
            HStack {
                Button("Fichero de referencia…") { Task { if let u = await SelectorFicheros.abrir(".txt") { referencia = u } } }
                Text(referencia?.lastPathComponent ?? "(selecciona)").foregroundStyle(Color.textoSecundario)
            }
        }
    }

    private var contraJornadas: some View {
        Tarjeta {
            Text("Escruta cada fichero contra la temporada y jornada correspondiente del histórico de resultados.").font(.callout)
                .foregroundStyle(Color.textoSecundario)
            CampoCondicion(etiqueta: "Plantilla de nombre de archivo (usa /t y /j)", texto: $plantilla, ayuda: "quiniela_/t_/j.txt")
            HStack {
                // ponytail: añade al final; SwiftUI de macOS 14 no expone la posición del cursor (DT-61).
                Button("Insertar /t") { plantilla += "/t" }
                Button("Insertar /j") { plantilla += "/j" }
                Button("?") { Task { if let u = await SelectorFicheros.abrir("*") { plantilla = u.lastPathComponent } } }
                    .help("Examinar un fichero y usar su nombre como plantilla")
                Button("Carpeta…") { Task { if let u = await SelectorFicheros.carpeta() { carpeta = u } } }
                Text(carpeta?.path(percentEncoded: false) ?? "").font(.callout).foregroundStyle(Color.textoSecundario)
            }
            HStack {
                Picker("Dígitos de temporada", selection: $digitosTemporada) {
                    Text("4 dígitos").tag(4)
                    Text("2 dígitos").tag(2)
                }
                Picker("Dígitos de jornada", selection: $digitosJornada) {
                    Text("2 dígitos").tag(2)
                    Text("1 dígito").tag(1)
                }
            }
            .frame(maxWidth: 520)
            Text("Temporadas").bold()
            List(temporadas, id: \.self, selection: $temporadasMarcadas) { Text($0) }.frame(height: 120)
        }
    }

    // MARK: Resultados

    private func resultados(_ p: ProyeccionEscrutinio) -> some View {
        Tarjeta("Resultados") {
            ScrollView([.horizontal, .vertical]) {
                Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 4) {
                    GridRow {
                        Text("")
                        Text("Archivo").bold()
                        Text("Nº").bold()
                        Text("Columna").bold()
                        ForEach(p.cabeceras.indices, id: \.self) { Text(String(p.cabeceras[$0])).bold() }
                        Text("Ac. Totales").bold()
                    }
                    ForEach(p.filas.indices, id: \.self) { i in
                        let f = p.filas[i]
                        GridRow {
                            Toggle(
                                "",
                                isOn: Binding(
                                    get: { seleccion.contains(i) }, set: { if $0 { seleccion.insert(i) } else { seleccion.remove(i) } })
                            )
                            .labelsHidden()
                            Text(f.fila.archivo)
                            Text(String(f.fila.lineaID)).monospacedDigit()
                            Text(f.fila.columna).monospaced()
                            ForEach(f.conteos.indices, id: \.self) { Text(f.conteos[$0]).monospacedDigit() }
                            Text(f.fila.aciertosTotales).monospacedDigit()
                        }
                    }
                }
            }
            .frame(maxHeight: 360)
            Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 2) {
                ForEach(p.histograma, id: \.self) { b in
                    GridRow {
                        Text(String(b.aciertos)).monospacedDigit()
                        Text("->")
                        Text(String(b.columnas)).monospacedDigit().accessibilityIdentifier("histograma-\(b.aciertos)")
                    }
                }
            }
            acciones
        }
    }

    /// `Table` (solo crea las filas visibles), no `Grid`: con miles de premiadas el Grid las creaba todas y la pantalla (y
    /// cada búsqueda de XCUITest, 9-15 s en F16) se volvía lenta.
    private func listaPremiadas(_ filas: [FilaPremiada]) -> some View {
        Tarjeta("Columnas premiadas") {
            Table(filas.indices.map { FilaIndexada(id: $0, f: filas[$0]) }) {
                TableColumn("Arch. Columnas", value: \.f.archivoColumnas)
                TableColumn("Jorn.", value: \.f.jornada)
                TableColumn("Columna") { Text($0.f.columna).monospaced() }
                TableColumn("Premio", value: \.f.premio)
                TableColumn("Nº Col.", value: \.f.numeroColumna)
                TableColumn("Nº Boleto", value: \.f.numeroBoleto)
            }
            .frame(minHeight: 160, maxHeight: 220)
        }
    }
}

/// «Columnas Premiadas» (pantalla 20, `ColumnasPremiadasFrm`): resumen exportable.
/// Una columna premiada con su posición, para las tablas.
private struct FilaIndexada: Identifiable {
    let id: Int
    let f: FilaPremiada
}

struct VistaColumnasPremiadas: View {
    @Environment(Sesion.self) private var sesion
    @State private var filas: [FilaPremiada] = []
    @State private var seleccion: Set<Int> = []
    @State private var cargado = false

    private func guardar(_ lista: [FilaPremiada]) async {
        guard !lista.isEmpty else { return sesion.alertas.info("No hay columnas que guardar.") }
        guard let url = await SelectorFicheros.guardar("columnas", [("Columnas", ".txt")]) else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(
                CasosCombinacion.datosColumnas(lista.map(\.columna), quitarComas: false), en: url, atomico: false)
            sesion.alertas.info("Guardadas \(lista.count) columna(s) en \(url.lastPathComponent).")
        } catch {
            sesion.alertas.error("No se pudieron guardar las columnas: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .columnasPremiadas) {
            Text(
                "Resumen de las columnas premiadas encontradas. Puedes exportar todas las columnas o solo las seleccionadas a un fichero de texto."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Columnas premiadas (\(filas.count))", icono: "trophy", color: .secundario) {
                Table(filas.indices.map { FilaIndexada(id: $0, f: filas[$0]) }, selection: $seleccion) {
                    TableColumn("Arch. Columnas", value: \.f.archivoColumnas)
                    TableColumn("Jorn.", value: \.f.jornada)
                    TableColumn("Columna", value: \.f.columna)
                    TableColumn("Premio", value: \.f.premio)
                    TableColumn("Nº Col.", value: \.f.numeroColumna)
                    TableColumn("Nº Boleto", value: \.f.numeroBoleto)
                }
                .frame(minHeight: 320)
                HStack {
                    Button("Guardar Todas") { Task { await guardar(filas) } }.buttonStyle(EstiloPrincipal())
                    Button("Guardar Seleccionadas") {
                        Task { await guardar(filas.indices.filter { seleccion.contains($0) }.map { filas[$0] }) }
                    }
                }
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            if let e = sesion.premiadasEntrada { filas = e }
            sesion.premiadasEntrada = nil
        }
    }
}
