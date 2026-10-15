import Free1X2CasosDeUso
import SwiftUI

/// Inicio (`MainPage`, plantilla T0; `MainPageViewModel.cs`, `BoletoBaseControl`).
struct VistaInicio: View {
    @Environment(Sesion.self) private var sesion

    static let izquierda: [(String, TipoFiltro, Destino)] = [
        ("Variantes X y 2", .noVariantes, .noVariantes), ("Signos Seguidos", .signosSeguidos, .signosSeguidos),
        ("Dibujos", .dibujos, .dibujos), ("Interrupciones", .interrupciones, .interrupciones),
        ("Grupos Equipos", .gruposEquipos, .gruposEquipos), ("Formatos", .formatosSignos, .formatos),
        ("Simetrías", .simetrias, .simetrias),
    ]
    static let derecha: [(String, TipoFiltro, Destino)] = [
        ("Pesos Numéricos", .pesosNumericos, .pesosNum), ("Valoración", .valoracionSignos, .valoracion),
        ("Columnas Probables", .colProbables, .colProbables), ("Distancias", .distancias, .distancias),
        ("Contactos", .contactos, .contactos), ("Formatos 123", .formatos123, .formatos123),
        ("Diferencias", .diferencias, .diferencias),
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
            Tarjeta("Filtro de columnas") {
                HStack {
                    SemaforoView(estado: estadoFichero(sesion.archivoFiltroCols))
                    Text(nombre(sesion.archivoFiltroCols) ?? "(sin filtro)").foregroundStyle(Color.textoSecundario)
                        .lineLimit(1).truncationMode(.middle)
                    Spacer()
                    Button("Filtro…") { Task { await filtroColumnas() } }
                        .accessibilityLabel("Seleccionar o quitar el filtro de columnas")
                }
            }
            Tarjeta("Filtro Parcial del Grupo") {
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
            Tarjeta {
                HStack {
                    Text("Combinación").font(.headline).foregroundStyle(Color.acento)
                    Spacer()
                    Text(sesion.rutaCombinacion.map { $0.deletingPathExtension().lastPathComponent } ?? "(combinación nueva)")
                        .font(.callout).foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    Button("Calcular") { calcular() }.buttonStyle(.borderedProminent)
                        .accessibilityLabel("Calcular columnas")
                    Button("Reducir") {
                        sesion.volcarPronosticos()
                        sesion.router.navegar(.reductor)
                    }
                    .accessibilityLabel("Reducir columnas")
                    Button("Nueva") { sesion.ejecutar(.nuevaCombinacion) }.accessibilityLabel("Nueva combinación")
                    Button("Abrir…") { sesion.ejecutar(.abrirCombinacion) }.accessibilityLabel("Abrir combinación")
                    Button("Guardar…") { sesion.ejecutar(.guardarCombinacion) }.accessibilityLabel("Guardar combinación")
                }
            }
        }
    }

    private func columna(_ items: [(String, TipoFiltro, Destino)]) -> some View {
        VStack(spacing: 4) {
            ForEach(items, id: \.0) { titulo, tipo, destino in
                Button {
                    abrir(destino)
                } label: {
                    HStack {
                        SemaforoView(estado: estado.condiciones[tipo] ?? .neutro)
                        Text(titulo).lineLimit(1)
                        Spacer()
                    }
                }
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
            }
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
                        TextField("Local", text: $s.locales[i]).labelsHidden()
                        TextField("Visitante", text: $s.visitantes[i]).labelsHidden()
                        ForEach(0..<3, id: \.self) { k in
                            Toggle(
                                ["1", "X", "2"][k],
                                isOn: Binding(get: { sesion.signos(i)[k] }, set: { _ in sesion.alternar(i, k) })
                            )
                            .toggleStyle(.button)
                            .tint(Color.exito)
                            .accessibilityHint(["Signo 1 (gana el local)", "Signo X (empate)", "Signo 2 (gana el visitante)"][k])
                        }
                    }
                    .controlSize(.small)
                }
            }
        }
    }
}
