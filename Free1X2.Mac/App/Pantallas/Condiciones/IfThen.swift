import Free1X2CasosDeUso
import SwiftUI

/// Bloque «Si …» / «Entonces …» de una condición sencilla.
private struct EntradaIfThen {
    var generica: String?
    var especifica: String?
    var especificas: [String] = []
    var usaValor = true
    var valor = 0.0
    var negado = false
}

/// «Condiciones relacionadas (if-then)» (pantalla 65, `IfThenFrm`): actúa sobre toda la combinación.
struct VistaIfThen: View {
    @Environment(Sesion.self) private var sesion
    @State private var p = PantallaIfThen()
    @State private var si = EntradaIfThen()
    @State private var entonces = EntradaIfThen()
    @State private var marcadas = Set<UUID>()
    @State private var grupoSi: String?
    @State private var grupoEntonces: String?
    @State private var negadoGrupoSi = false
    @State private var negadoGrupoEntonces = false
    @State private var cargado = false

    private var temporal: URL { CasosCondiciones.rutaTemporal("if", entorno: sesion.entorno) }

    var body: some View {
        Pagina(destino: .ifThen) {
            Text(
                "Define relaciones \"si se produce una condición, entonces debe cumplirse otra\", ya sea entre condiciones de filtro o entre grupos de partidos."
            )
            .foregroundStyle(Color.textoSecundario)
            TabView {
                sencillas.tabItem { Text("Condiciones sencillas") }.padding(8)
                grupos.tabItem { Text("Grupos") }.padding(8).disabled(sesion.combinacion.grupos.count < 2)
            }
            .frame(minHeight: 520)
            HStack {
                Button("Aceptar") { aceptar() }.buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction)
                Button("Guardar") { Task { await guardar() } }
                Button("Abrir") {
                    Task {
                        guard let url = await SelectorFicheros.abrir(".if", ".xml") else { return }
                        abrir(url)
                    }
                }
                Button("Copiar") { copiar() }
                Button("Pegar") { if sesion.entorno.archivos.existe(temporal) { abrir(temporal) } }
                Button("Borrar") { limpiar() }
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            cargar(sesion.combinacion.ifThen)
        }
    }

    private func bloque(_ titulo: String, _ e: Binding<EntradaIfThen>) -> some View {
        Tarjeta(titulo) {
            Picker(
                "Condición genérica",
                selection: Binding(
                    get: { e.wrappedValue.generica },
                    set: { g in
                        let r = CasosCondiciones.especificasIfThen(g, numeroPartidos: sesion.numeroPartidos)
                        e.wrappedValue = EntradaIfThen(
                            generica: g, especifica: nil, especificas: r.items, usaValor: r.usaValor, valor: e.wrappedValue.valor,
                            negado: e.wrappedValue.negado)
                    })
            ) {
                Text("").tag(String?.none)
                ForEach(CasosCondiciones.genericasIfThen, id: \.self) { Text($0).tag(Optional($0)) }
            }
            Picker("Condición específica", selection: e.especifica) {
                Text("").tag(String?.none)
                ForEach(e.wrappedValue.especificas, id: \.self) { Text($0).tag(Optional($0)) }
            }
            CampoNumero(etiqueta: "Valor", valor: e.valor, minimo: 0, maximo: 14)
            Toggle("Negado", isOn: e.negado)
        }
    }

    private func lista(_ relaciones: Binding<[RelacionIfThen]>, rango: Binding<String>) -> some View {
        Tarjeta("Relaciones") {
            Grid(alignment: .leading) {
                GridRow {
                    Text("")
                    Text("Si se produce ...").bold()
                    Text("... debe cumplirse que ...").bold()
                }
                ForEach(relaciones.wrappedValue) { r in
                    GridRow {
                        Toggle(
                            "",
                            isOn: Binding(
                                get: { marcadas.contains(r.id) }, set: { if $0 { marcadas.insert(r.id) } else { marcadas.remove(r.id) } })
                        )
                        .toggleStyle(.checkbox)
                        Text(r.si)
                        Text(r.entonces)
                    }
                }
            }
            HStack {
                Text("Condiciones en lista:")
                Text("\(relaciones.wrappedValue.count)")
                Spacer()
                Button("Borrar relación") { relaciones.wrappedValue.removeAll { marcadas.contains($0.id) } }
            }
            CampoCondicion(etiqueta: "Condiciones que deben cumplirse", texto: rango, ayuda: "p.ej. 1 ó 1-3")
        }
    }

    private var sencillas: some View {
        VStack(alignment: .leading, spacing: 10) {
            HStack(alignment: .top) {
                bloque("Si ...", $si)
                bloque("Entonces ...", $entonces)
            }
            Button("Añadir") {
                do throws(ErrorDominio) {
                    let a = CasosCondiciones.componerCondicionIfThen(
                        generica: si.generica, especifica: si.especifica, valor: si.valor, usaValor: si.usaValor, negado: si.negado)
                    let b = CasosCondiciones.componerCondicionIfThen(
                        generica: entonces.generica, especifica: entonces.especifica, valor: entonces.valor, usaValor: entonces.usaValor,
                        negado: entonces.negado)
                    guard let r = try CasosCondiciones.relacionIfThen(a, b) else { return }
                    p.relaciones.append(r)
                    (si, entonces) = (EntradaIfThen(), EntradaIfThen())
                } catch {
                    sesion.alertas.error(error.mensajeUsuario)
                }
            }
            lista($p.relaciones, rango: $p.rangoCondiciones)
        }
    }

    private var grupos: some View {
        VStack(alignment: .leading, spacing: 10) {
            Tarjeta("Relación entre grupos de partidos") {
                Text("Selecciona el grupo \"Si\" y el grupo \"Entonces\". Ambos deben ser distintos.").font(.callout)
                    .foregroundStyle(Color.textoSecundario)
                let opciones = CasosCondiciones.gruposIfThen(sesion.combinacion)
                HStack {
                    Picker("Si grupo ...", selection: $grupoSi) {
                        Text("").tag(String?.none)
                        ForEach(opciones, id: \.self) { Text($0).tag(Optional($0)) }
                    }
                    Toggle("Negado", isOn: $negadoGrupoSi)
                }
                HStack {
                    Picker("... entonces grupo ...", selection: $grupoEntonces) {
                        Text("").tag(String?.none)
                        ForEach(opciones, id: \.self) { Text($0).tag(Optional($0)) }
                    }
                    Toggle("Negado", isOn: $negadoGrupoEntonces)
                }
                Button("Añadir") {
                    do throws(ErrorDominio) {
                        guard
                            let r = try CasosCondiciones.relacionGruposIfThen(
                                grupoSi, negadoSi: negadoGrupoSi, grupoEntonces, negadoEntonces: negadoGrupoEntonces)
                        else { return }
                        p.grupos.append(r)
                        (grupoSi, grupoEntonces, negadoGrupoSi, negadoGrupoEntonces) = (nil, nil, false, false)
                    } catch {
                        sesion.alertas.error(error.mensajeUsuario)
                    }
                }
            }
            lista($p.grupos, rango: $p.rangoGrupos)
        }
    }

    private func cargar(_ it: IfThenConfig?) {
        guard let it else { return }
        let n = CasosCondiciones.cargarIfThen(it, combinacion: sesion.combinacion)
        // `CargarDatos` limpia las listas pero conserva los rangos si el fichero no trae elementos.
        p.relaciones = n.relaciones
        p.grupos = n.grupos
        if !it.condiciones.isEmpty { p.rangoCondiciones = n.rangoCondiciones }
        if !it.grupos.isEmpty { p.rangoGrupos = n.rangoGrupos }
        for _ in 0..<n.gruposQuitados { sesion.alertas.error(CasosCondiciones.mensajeGruposQuitados) }
    }

    private func limpiar() {
        (si, entonces) = (EntradaIfThen(), EntradaIfThen())
        p = PantallaIfThen()
    }

    /// `GuardarCondicion`: los avisos de rango se muestran; otros errores no se capturaban en el C#.
    private func construir() -> IfThenConfig? {
        do throws(ErrorDominio) {
            return try CasosCondiciones.construirIfThen(p, combinacion: sesion.combinacion)
        } catch {
            if error.excepcion == .general { sesion.alertas.error(error.mensajeUsuario) } else { sesion.alertas.inesperado(error) }
            return nil
        }
    }

    private func aceptar() {
        guard let it = construir() else { return }
        sesion.combinacion.ifThen = it
        sesion.router.atras()
    }

    private func guardar() async {
        guard let it = construir(), !it.esVacio else { return }
        guard
            let url = await SelectorFicheros.guardar(
                "Condiciones", [("Condiciones relacionadas", ".if"), ("Condiciones relacionadas (XML)", ".xml")])
        else { return }
        do throws(ErrorDominio) {
            try CasosCondiciones.exportarIfThen(it, en: url, entorno: sesion.entorno)
        } catch {
            sesion.alertas.error("No se pudo guardar: " + error.mensajeUsuario)
        }
    }

    private func abrir(_ url: URL) {
        limpiar()
        do throws(ErrorDominio) {
            cargar(try CasosCondiciones.importarIfThen(url, entorno: sesion.entorno))
        } catch {
            sesion.alertas.error("No se pudo abrir: " + error.mensajeUsuario)
        }
    }

    private func copiar() {
        guard let it = construir(), !it.esVacio else { return }
        do throws(ErrorDominio) {
            try CasosCondiciones.exportarIfThen(it, en: temporal, entorno: sesion.entorno)
        } catch {
            sesion.alertas.error("No se pudo copiar: " + error.mensajeUsuario)
        }
    }
}
