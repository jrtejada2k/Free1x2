import Free1X2CasosDeUso
import SwiftUI

/// «Gestión de Equipos» (pantalla 60, `GestorEquiposFrm`): cuatro listas, alta, importación online y guardado.
struct VistaGestorEquipos: View {
    @Environment(Sesion.self) private var sesion
    @State private var e = EditorEquipos()
    @State private var seleccion: [Int?] = [nil, nil, nil, nil]
    @State private var nombre = ""
    @State private var categoria = CategoriaEquipos.primera
    @State private var pais = PaisQuiniela.es
    @State private var importando = false

    private func elegido(_ c: CategoriaEquipos) -> String? { seleccion[c.rawValue].flatMap { e.lista(c)[safe: $0] } }

    private func importar() async {
        guard !importando else { return }
        importando = true
        e.estado = "Conectando con el servicio online…"
        do {
            let catalogo = try await sesion.servicioOnline.obtenerEquipos(pais: pais.rawValue)
            e.importar(catalogo.equiposPlano, en: categoria)
        } catch {
            e.errorImportacion(error)
        }
        importando = false
    }

    var body: some View {
        Pagina(destino: .gestorEquipos) {
            Text("Organiza los equipos por categoría. Mueve equipos entre 1ª, 2ª y 2ªB, da de alta o elimina equipos y guarda los cambios.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta {
                HStack {
                    TextField("Nombre", text: $nombre, prompt: Text("Nombre del equipo")).textFieldStyle(.roundedBorder)
                        .accessibilityIdentifier("equipo-nombre").frame(
                            maxWidth: 260)
                    Picker("Categoría", selection: $categoria) {
                        ForEach(CategoriaEquipos.allCases, id: \.self) { Text($0.titulo).tag($0) }
                    }
                    .frame(maxWidth: 180)
                    Button("Nuevo equipo") { if e.nuevo(nombre, en: categoria) { nombre = "" } }
                    Button("Formulario de alta...") { sesion.router.navegar(.agregarEquipo) }
                }
                HStack {
                    Picker("País (online)", selection: $pais) {
                        Text("España").tag(PaisQuiniela.es)
                        Text("México").tag(PaisQuiniela.mx)
                    }
                    .frame(maxWidth: 240)
                    Button("Importar equipos online") { Task { await importar() } }.disabled(importando)
                        .help("Descarga la lista de equipos de jornadas recientes y la añade a la categoría seleccionada arriba")
                    Button("Guardar archivos") {
                        do throws(ErrorDominio) {
                            try e.guardar(entorno: sesion.entorno)
                        } catch {
                            sesion.alertas.error("No se pudieron guardar los equipos: " + error.mensajeUsuario)
                        }
                    }
                }
            }
            HStack(alignment: .top, spacing: 8) {
                lista(.primera, "Equipos 1ª")
                VStack {
                    Button {
                        e.mover(elegido(.primera), de: .primera, a: .segunda)
                    } label: {
                        Image(systemName: "chevron.right")
                    }
                    .accessibilityLabel("Mover a 2ª")
                    Button {
                        e.mover(elegido(.segunda), de: .segunda, a: .primera)
                    } label: {
                        Image(systemName: "chevron.left")
                    }
                    .accessibilityLabel("Mover a 1ª")
                }
                .padding(.top, 60)
                lista(.segunda, "Equipos 2ª")
                VStack {
                    Button {
                        e.mover(elegido(.segunda), de: .segunda, a: .segundaB)
                    } label: {
                        Image(systemName: "chevron.right")
                    }
                    .accessibilityLabel("Mover a 2ªB")
                    Button {
                        e.mover(elegido(.segundaB), de: .segundaB, a: .segunda)
                    } label: {
                        Image(systemName: "chevron.left")
                    }
                    .accessibilityLabel("Mover a 2ª")
                }
                .padding(.top, 60)
                lista(.segundaB, "Equipos 2ªB")
                lista(.internacional, "Internacionales")
            }
            Text(e.estado).foregroundStyle(Color.textoSecundario)
        }
        // Al volver del formulario de alta la página se recrea en el C# y relee los ficheros.
        .onAppear { e = EditorEquipos.cargar(entorno: sesion.entorno) }
    }

    private func lista(_ c: CategoriaEquipos, _ titulo: String) -> some View {
        VStack(alignment: .leading) {
            Text(titulo).bold()
            List(selection: $seleccion[c.rawValue]) {
                ForEach(e.lista(c).indices, id: \.self) { Text(e.lista(c)[$0]).tag($0) }
            }
            .frame(minWidth: 160, minHeight: 300)
            Button("Eliminar") { e.eliminar(elegido(c), de: c) }
        }
    }
}

/// «Añadir Equipo» (pantalla 2, `AgregarEquipoFrm`): alta directa en el fichero de la categoría.
struct VistaAgregarEquipo: View {
    @Environment(Sesion.self) private var sesion
    @State private var nombre = ""
    @State private var categoria = CategoriaEquipos.primera
    @State private var estado = "Preparado"

    private func anadir() {
        guard !nombre.allSatisfy(\.isWhitespace) else { return estado = "Introduce un nombre de equipo" }
        do throws(ErrorDominio) {
            if try CasosArchivo.anadirEquipo(nombre, categoria: categoria, entorno: sesion.entorno) {
                estado = "Equipo \"\(nombre)\" añadido a \(categoria.titulo)."
                nombre = ""
            } else {
                estado = "El equipo \"\(nombre)\" ya existe en \(categoria.titulo)."
            }
        } catch {
            estado = "Error al añadir el equipo: " + error.mensajeUsuario
            sesion.alertas.error("No se pudo añadir el equipo: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .agregarEquipo) {
            Text(
                "Da de alta un equipo nuevo indicando su nombre y la categoría a la que pertenece. Si el equipo ya existe en esa categoría no se duplica."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Datos del equipo") {
                CampoCondicion(etiqueta: "Nombre", texto: $nombre, ayuda: "Nombre del equipo")
                Picker("Categoría", selection: $categoria) {
                    ForEach(CategoriaEquipos.allCases, id: \.self) { Text($0.titulo).tag($0) }
                }
                .frame(maxWidth: 260)
            }
            HStack {
                Button("Añadir", action: anadir).buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction)
                Text(estado).foregroundStyle(Color.textoSecundario)
            }
        }
    }
}
