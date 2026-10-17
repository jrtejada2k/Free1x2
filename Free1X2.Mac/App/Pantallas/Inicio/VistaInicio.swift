import Free1X2CasosDeUso
import SwiftUI

/// Inicio (`MainPage`, plantilla T0; `MainPageViewModel.cs`, `BoletoBaseControl`).
struct VistaInicio: View {
    @Environment(Sesion.self) private var sesion
    @State private var descargando = false
    @State private var mascaras: [Mascara] = []
    @State private var guardandoMascara = false
    @State private var generando = false

    /// Título, filtro, destino e icono (los glifos de `CondicionItem.Glifo` en SF Symbols).
    static let izquierda: [(String, TipoFiltro, Destino, String)] = [
        ("Variantes X y 2", .noVariantes, .noVariantes, "arrow.left.arrow.right"),
        ("Signos Seguidos", .signosSeguidos, .signosSeguidos, "arrow.up.right"), ("Dibujos", .dibujos, .dibujos, "pencil"),
        ("Interrupciones", .interrupciones, .interrupciones, "square"),
        ("Grupos Equipos", .gruposEquipos, .gruposEquipos, "person.3"), ("Formatos", .formatosSignos, .formatos, "folder"),
        ("Simetrías", .simetrias, .simetrias, "square.grid.2x2"),
    ]
    static let derecha: [(String, TipoFiltro, Destino, String)] = [
        ("Pesos Numéricos", .pesosNumericos, .pesosNum, "scalemass"), ("Valoración", .valoracionSignos, .valoracion, "star"),
        ("Columnas Probables", .colProbables, .colProbables, "chart.xyaxis.line"),
        ("Distancias", .distancias, .distancias, "ruler"),
        ("Contactos", .contactos, .contactos, "person.text.rectangle"),
        ("Formatos 123", .formatos123, .formatos123, "list.number"),
        ("Diferencias", .diferencias, .diferencias, "square.on.square"),
    ]

    var body: some View {
        HStack(alignment: .top, spacing: 12) {
            VStack(spacing: 8) {
                BoletoBaseView()
                navegacionGrupos
            }
            .frame(minWidth: 430, maxWidth: 560)
            ScrollView { columnaDerecha }
        }
        .padding(.horizontal, 12)
        .padding(.vertical, 8)
        .frame(maxWidth: 1180, alignment: .leading)
        .frame(maxWidth: .infinity, maxHeight: .infinity, alignment: .topLeading)
        .background(Color.fondo)
        .navigationTitle("Free1X2")
        .toolbar {
            // Las acciones propias de Inicio (DT-82); Nueva, Abrir y Guardar están en la barra de todas las pantallas.
            ToolbarItemGroup(placement: .primaryAction) {
                Button(action: calcular) { Label("Calcular", systemImage: "function") }
                    .labelStyle(.titleAndIcon).help("Calcular las columnas del boleto").accessibilityIdentifier("Calcular columnas")
                Button {
                    Task {
                        generando = true
                        defer { generando = false }
                        guard let u = await sesion.columnasDeInicio() else { return }
                        sesion.entradaReductor = u
                        sesion.router.navegar(.reductor)
                    }
                } label: {
                    Label(generando ? "Generando…" : "Reducir", systemImage: "arrow.down.right.and.arrow.up.left")
                }
                .labelStyle(.titleAndIcon).disabled(generando)
                .help("Desarrollar las columnas del boleto y abrir el Reductor con ellas, sin guardar antes un fichero")
                .accessibilityIdentifier("Reducir columnas")
                Menu {
                    Button("España") { Task { await cargarJornada(.es) } }
                    Button("México") { Task { await cargarJornada(.mx) } }
                } label: {
                    Label("Jornada", systemImage: "calendar")
                }
                .labelStyle(.titleAndIcon).disabled(descargando)
                .help("Cargar los equipos de la jornada actual").accessibilityIdentifier("cargar-jornada")
                Menu {
                    Button("Guardar boleto como máscara…") { guardandoMascara = true }
                    if !mascaras.isEmpty {
                        Section("Aplicar al boleto") {
                            ForEach(mascaras) { m in Button("\(m.nombre)  ·  \(m.columnas) col.") { sesion.aplicarMascara(m) } }
                        }
                    }
                    Divider()
                    Button("Máscaras y escrutinio histórico…") { sesion.router.navegar(.mascaras) }
                } label: {
                    Label("Máscaras", systemImage: "theatermasks")
                }
                .labelStyle(.titleAndIcon)
                .help("Guardar los signos del boleto como máscara o aplicar una guardada (DT-84)")
                .accessibilityIdentifier("menu-mascaras")
            }
        }
        .modifier(GuardarMascara(visible: $guardandoMascara, alGuardar: recargarMascaras))
        .onAppear(perform: recargarMascaras)
    }

    private func recargarMascaras() {
        mascaras = CasosMascaras.listar(numeroPartidos: sesion.numeroPartidos, entorno: sesion.entorno)
    }

    /// Equipos de la jornada actual del país (DT-81); sin red usa la guardada, como «Descarga de boleto».
    private func cargarJornada(_ pais: PaisQuiniela) async {
        guard !descargando else { return }
        descargando = true
        defer { descargando = false }
        let r = await CasosArchivo.descargarJornada(pais, servicio: sesion.servicioOnline, entorno: sesion.entorno)
        guard let j = r.jornada else { return sesion.alertas.error(r.mensaje) }
        sesion.cargarJornada(j)
        sesion.alertas.info(r.mensaje)
    }

    private var estado: EstadoCondiciones { CasosInicio.estadoDeCondiciones(sesion.combinacion, grupo: sesion.grupoActual) }

    private var navegacionGrupos: some View {
        HStack(spacing: 4) {
            boton("backward.end", "Primer grupo (boleto base)") { cambiarGrupo(0) }
            boton("chevron.left", "Grupo anterior") { cambiarGrupo(sesion.grupoPantalla - 1) }
            boton("chevron.right", "Grupo siguiente (crea si no existe)") {
                sesion.volcarPronosticos()
                cambiarGrupo(sesion.grupoPantalla + 1)
            }
            boton("forward.end", "Último grupo") { cambiarGrupo(sesion.combinacion.grupos.count - 1) }
            boton("trash", "Eliminar el grupo actual") {
                do throws(ErrorDominio) {
                    if let g = try CasosInicio.eliminarGrupo(&sesion.combinacion, sesion.grupoPantalla) { sesion.grupoPantalla = g }
                } catch {
                    sesion.alertas.error(error)
                }
            }
            Text(sesion.combinacion.grupos.count == 1 ? "1 grupo" : "\(sesion.combinacion.grupos.count) grupos")
                .font(.callout).foregroundStyle(Color.textoSecundario)
            Spacer()
            Button {
                abrir(.controlGrupos)
            } label: {
                HStack {
                    SemaforoView(estado: estado.controlGrupos)
                    Text("Control de Grupos")
                }
            }
            .help("Control de Grupos")
        }
    }

    private var columnaDerecha: some View {
        VStack(spacing: 10) {
            Tarjeta("Condiciones del grupo") {
                HStack(alignment: .top) {
                    columna(Self.izquierda)
                    columna(Self.derecha)
                }
                HStack {
                    Button {
                        abrir(.controlTol)
                    } label: {
                        Label("Tolerancias", systemImage: "plusminus").frame(maxWidth: .infinity)
                    }
                    Button {
                        abrir(.ifThen)
                    } label: {
                        HStack {
                            SemaforoView(estado: estado.ifThen)
                            Text("Condiciones Relacionadas (If-Then)").lineLimit(1)
                        }
                        .frame(maxWidth: .infinity)
                    }
                }
            }
            Tarjeta("Filtro de columnas", icono: "line.3.horizontal.decrease") {
                HStack {
                    SemaforoView(estado: estadoFichero(sesion.archivoFiltroCols))
                    Text(nombre(sesion.archivoFiltroCols) ?? "(sin filtro)").foregroundStyle(Color.textoSecundario)
                        .lineLimit(1).truncationMode(.middle)
                    Spacer()
                    Button("Filtro…") { Task { await filtroColumnas() } }
                        .accessibilityLabel("Seleccionar o quitar el filtro de columnas")
                }
            }
            Tarjeta("Filtro Parcial del Grupo", icono: "line.3.horizontal.decrease", color: .secundario) {
                let g = sesion.combinacion.grupos[sesion.grupoActual].archivoFiltro
                let parcial = sesion.grupoPantalla != 0 && !g.isEmpty
                HStack {
                    SemaforoView(estado: parcial ? estadoFichero(g) : .neutro)
                    Text(parcial ? nombre(g) ?? "" : "(Selecciona)").foregroundStyle(Color.textoSecundario)
                        .lineLimit(1).truncationMode(.middle)
                    Spacer()
                    Button("Seleccionar…") { Task { await filtroParcial() } }
                        .accessibilityLabel("Seleccionar el filtro parcial del grupo")
                }
            }
        }
    }

    private func columna(_ items: [(String, TipoFiltro, Destino, String)]) -> some View {
        VStack(spacing: 3) {
            ForEach(items, id: \.0) { titulo, tipo, destino, icono in
                let e = estado.condiciones[tipo] ?? .neutro
                Button {
                    abrir(destino)
                } label: {
                    HStack(spacing: 6) {
                        SemaforoView(estado: e, tamano: 10)
                        ChipIcono(simbolo: icono)
                        Text(titulo).font(.system(size: 12)).foregroundStyle(Color.texto).lineLimit(1)
                        Spacer(minLength: 0)
                    }
                }
                .buttonStyle(EstiloCondicion(estado: e))
                .accessibilityLabel(titulo)
            }
        }
        .frame(maxWidth: .infinity)
    }

    private func boton(_ simbolo: String, _ ayuda: String, _ accion: @escaping () -> Void) -> some View {
        Button(action: accion) { Image(systemName: simbolo) }.help(ayuda).accessibilityLabel(ayuda)
    }

    /// `AbrirCondicion`: fija el grupo en edición y navega.
    private func abrir(_ d: Destino) {
        sesion.grupoEnEdicion = sesion.grupoActual
        sesion.router.navegar(d)
    }

    private func cambiarGrupo(_ g: Int) {
        do throws(ErrorDominio) {
            sesion.grupoPantalla = try CasosInicio.cambiarGrupo(&sesion.combinacion, a: g)
        } catch {
            sesion.alertas.error(error)
        }
    }

    private func nombre(_ ruta: String) -> String? {
        ruta.isEmpty ? nil : URL(filePath: ruta).deletingPathExtension().lastPathComponent
    }

    private func estadoFichero(_ ruta: String) -> Semaforo {
        ruta.isEmpty ? .neutro : sesion.entorno.archivos.existe(URL(filePath: ruta)) ? .verde : .rojo
    }

    private func calcular() {
        sesion.volcarPronosticos()
        let f = sesion.archivoFiltroCols
        sesion.combinacion.archivoColumnasBase = estadoFichero(f) == .verde ? f : ""
        sesion.usarCombinacionCompartida = true
        sesion.router.navegar(.calculaColumnas)
    }

    private func filtroColumnas() async {
        guard sesion.archivoFiltroCols.isEmpty else {
            sesion.archivoFiltroCols = ""
            return
        }
        guard let url = await SelectorFicheros.abrir(".txt", "*") else { return }
        do throws(ErrorDominio) {
            try CasosInicio.validarFiltroColumnas(url, numeroPartidos: sesion.numeroPartidos, entorno: sesion.entorno)
            sesion.archivoFiltroCols = url.path(percentEncoded: false)
        } catch {
            sesion.alertas.error(error)
        }
    }

    private func filtroParcial() async {
        let g = sesion.grupoActual
        guard sesion.grupoPantalla != 0 else { return sesion.alertas.error(CasosInicio.mensajeFiltroParcialBase) }
        guard sesion.combinacion.grupos[g].archivoFiltro.isEmpty else {
            try? sesion.combinacion.modificarGrupo(g) { $0.archivoFiltro = "" }
            return
        }
        guard let url = await SelectorFicheros.abrir(".txt", "*") else { return }
        do throws(ErrorDominio) {
            try CasosInicio.validarFiltroParcial(url, numeroPartidos: sesion.numeroPartidos, entorno: sesion.entorno)
            try sesion.combinacion.modificarGrupo(g) { $0.archivoFiltro = url.path(percentEncoded: false) }
        } catch {
            sesion.alertas.error(error)
        }
    }
}

/// `BoletoBaseControl`: título, resumen, reinicio y una fila por partido.
struct BoletoBaseView: View {
    @Environment(Sesion.self) private var sesion
    var body: some View {
        @Bindable var s = sesion
        let marcados = (0..<sesion.numeroPartidos).map { sesion.signos($0).filter { $0 }.count }
        Tarjeta {
            HStack {
                Text(sesion.grupoPantalla == 0 ? "Boleto Base" : "Grupo \(sesion.grupoPantalla) / \(sesion.combinacion.grupos.count - 1)")
                    .font(.headline).foregroundStyle(Color.error)
                Text(sesion.rutaCombinacion.map { $0.deletingPathExtension().lastPathComponent } ?? "combinación nueva")
                    .font(.callout).foregroundStyle(Color.textoSecundario).lineLimit(1).truncationMode(.middle)
                    .help("Fichero de la combinación")
                Spacer()
                Text(
                    "Fijos: \(marcados.filter { $0 == 1 }.count) - Dobles: \(marcados.filter { $0 == 2 }.count) - Triples: \(marcados.filter { $0 == 3 }.count)"
                )
                .font(.callout).foregroundStyle(Color.textoSecundario)
                Button {
                    sesion.reiniciarTriples()
                } label: {
                    Image(systemName: "arrow.counterclockwise")
                }
                .help("Reiniciar a 14 triples")
                .accessibilityLabel("Reiniciar a 14 triples")
            }
            .bandaTarjeta()
            Grid(alignment: .leading, horizontalSpacing: 6, verticalSpacing: 3) {
                GridRow {
                    Text("Nº")
                    Text("Local")
                    Text("Visitante")
                    Text("1")
                    Text("X")
                    Text("2")
                }
                .font(.caption).foregroundStyle(Color.textoSecundario)
                ForEach(0..<sesion.numeroPartidos, id: \.self) { i in
                    GridRow {
                        Text(String(format: "%02d", i + 1)).monospacedDigit()
                        TextField("Local", text: $s.locales[i]).labelsHidden().accessibilityIdentifier("local-\(i + 1)")
                        TextField("Visitante", text: $s.visitantes[i]).labelsHidden().accessibilityIdentifier("visitante-\(i + 1)")
                        ForEach(0..<3, id: \.self) { k in
                            Toggle(
                                ["1", "X", "2"][k],
                                isOn: Binding(get: { sesion.signos(i)[k] }, set: { _ in sesion.alternar(i, k) })
                            )
                            .toggleStyle(EstiloSigno())
                            .accessibilityIdentifier("signo-\(i + 1)-\(["1", "X", "2"][k])")
                            .accessibilityHint(["Signo 1 (gana el local)", "Signo X (empate)", "Signo 2 (gana el visitante)"][k])
                        }
                    }
                    .controlSize(.small)
                }
            }
        }
    }
}
