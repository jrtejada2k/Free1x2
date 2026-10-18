import Free1X2CasosDeUso
import SwiftUI

/// «Separador Porcentajes Juan Bellas» (pantalla 50, `FiltroPorcenJB`).
struct VistaFiltroPorcenJB: View {
    @Environment(Sesion.self) private var sesion
    @State private var valores: [[Double]] = []
    @State private var limites = [15.0, 22, 29, 36, 48, 61]
    @State private var rangos = ["0-3", "0-3", "0-3", "0-3", "0-3", "0-3", "0-3", "0-14"]
    @State private var admitidas: [String] = []
    @State private var procesadasTexto = "procesadas"
    @State private var admitidasTexto = "admitidas"
    @State private var tiempo = "tiempo"
    @State private var ficheroResultado = "fichero"
    @State private var ficheroRangos = "fichero"
    @State private var ganadoras = GanadorasSelector(fichero: "fichero ganadoras")
    @State private var analisis = Array(repeating: "-", count: 8)

    private func bandas() -> BandasJB? {
        guard let b = CasosUtilidades.bandasJB(porcentajes: valores, limites: limites, rangos: rangos) else {
            sesion.alertas.error("error en datos de entrada")
            return nil
        }
        return b
    }

    private func calcular() async {
        (procesadasTexto, admitidasTexto, tiempo) = (" ", " ", " ")
        admitidas = []
        guard let b = bandas(), let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        let inicio = Date()
        do throws(ErrorDominio) {
            let r = try await CasosUtilidades.filtrarJB(u, bandas: b, entorno: sesion.entorno)
            if let c = r.corta { sesion.alertas.error("error longitud=" + c) }
            admitidas = r.admitidas
            (procesadasTexto, admitidasTexto) = (String(r.procesadas), String(r.admitidas.count))
            tiempo = FormatoNET.toStringTimeSpan(Date().timeIntervalSince(inicio))
        } catch {
            sesion.alertas.error("No se ha podido leer el fichero de entrada: " + error.mensajeUsuario)
        }
    }

    private func guardar(_ nombre: String, _ tipo: (String, String), _ datos: Data, _ prefijo: String) async -> URL? {
        guard let u = await SelectorFicheros.guardar(nombre, [tipo]) else { return nil }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(datos, en: u, atomico: false)
            return u
        } catch {
            sesion.alertas.error(prefijo + error.mensajeUsuario)
            return nil
        }
    }

    private func leerRangos() async {
        guard let u = await SelectorFicheros.abrir(".jb7", "*") else { return }
        do throws(ErrorDominio) {
            CasosUtilidades.leerRangosJB(
                try sesion.entorno.archivos.leer(u), rangos: &rangos, limites: &limites, cultura: sesion.entorno.cultura)
            ficheroRangos = u.lastPathComponent
        } catch {
            sesion.alertas.error("No se han podido leer los rangos: " + error.mensajeUsuario)
        }
    }

    private func cargarGanadoras() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        ganadoras.columnas = []
        do throws(ErrorDominio) {
            // Sin saltar vacías: una línea corta (o vacía) aborta.
            var leidas: [String] = []
            for l in TextoNET.lineas(try sesion.entorno.archivos.leer(u)) {
                let t = FormatoNET.mayusculas(FormatoNET.recortar(l))
                guard t.utf16.count >= 14 else {
                    ganadoras.columnas = leidas
                    return sesion.alertas.error("col.G. errónea=" + t)
                }
                leidas.append(t)
            }
            guard !leidas.isEmpty else { return sesion.alertas.error("El fichero de ganadoras está vacío.") }
            ganadoras.cargar(leidas, fichero: u)
        } catch {
            sesion.alertas.error("No se ha podido leer el fichero de ganadoras: " + error.mensajeUsuario)
        }
    }

    private func analizar() {
        guard let b = bandas() else { return }
        guard let r = CasosUtilidades.analizarJB(b, columna: ganadoras.columna) else {
            let c = ganadoras.columna.replacingOccurrences(of: "x", with: "4").replacingOccurrences(of: "X", with: "4")
            return sesion.alertas.error("col.G. errónea=" + c)
        }
        analisis = r.bandas.map(String.init) + [String(r.recorrido)]
    }

    var body: some View {
        Pagina(destino: .filtroPorcenJB) {
            HStack(alignment: .top) {
                Tarjeta("Porcentajes") { PorcentajesGrid(valores: $valores) }
                VStack(alignment: .leading) {
                    Tarjeta("Límites") {
                        LazyVGrid(columns: Array(repeating: GridItem(.fixed(150)), count: 3), alignment: .leading) {
                            ForEach(0..<6, id: \.self) { i in
                                CampoNumero(etiqueta: "Límite \(i + 1)", valor: $limites[i], minimo: 0, ancho: 50)
                            }
                        }
                    }
                    Tarjeta("Rangos") {
                        LazyVGrid(columns: Array(repeating: GridItem(.fixed(150)), count: 4), alignment: .leading) {
                            ForEach(0..<8, id: \.self) { i in
                                CampoTexto(etiqueta: i < 7 ? "Banda \(i + 1)" : "Recorrido", texto: $rangos[i], ancho: 60)
                            }
                        }
                        HStack {
                            Button("Salvar Rangos") {
                                Task {
                                    if let u = await guardar(
                                        "RangosJB", ("Condiciones", ".jb7"),
                                        CasosUtilidades.datosRangosJB(rangos: rangos, limites: limites),
                                        "No se han podido guardar los rangos: ")
                                    {
                                        ficheroRangos = u.lastPathComponent
                                    }
                                }
                            }
                            Button("Leer Rangos") { Task { await leerRangos() } }
                            Text(ficheroRangos).foregroundStyle(Color.textoSecundario)
                        }
                    }
                }
            }
            Tarjeta("Filtrado") {
                HStack {
                    Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal())
                    // DEC-915: como hoy, «Cancelar» no interrumpe el cálculo.
                    Button("Cancelar") {}
                    Button("Grabar Resultado") {
                        guard !admitidas.isEmpty else {
                            return sesion.alertas.info("No hay columnas admitidas que grabar. Ejecute primero Calcular.")
                        }
                        Task {
                            let l = admitidas.map { $0.replacingOccurrences(of: "4", with: "X") }
                            if let u = await guardar(
                                "Resultados", ("Resultados", ".txt"), TextoNET.codificarLineas(l), "No se ha podido grabar el resultado: ")
                            {
                                ficheroResultado = u.lastPathComponent
                            }
                        }
                    }
                    Button("Exportar") {
                        guard let b = bandas() else { return }
                        Task {
                            _ = await guardar(
                                "SignosPorBanda", ("Salida", ".txt"), TextoNET.codificarLineas(CasosUtilidades.exportarJB(b)),
                                "No se ha podido exportar: ")
                        }
                    }
                }
                HStack(spacing: 24) {
                    Text(procesadasTexto)
                    Text(admitidasTexto)
                    Text(tiempo)
                    Text(ficheroResultado)
                }
                .monospacedDigit().foregroundStyle(Color.textoSecundario)
            }
            PanelGanadoras(g: $ganadoras, cargar: cargarGanadoras, analizar: analizar)
            HStack(spacing: 16) {
                ForEach(0..<7, id: \.self) { Text("B\($0 + 1): " + analisis[$0]) }
                Text("Recorrido: " + analisis[7])
            }
            .monospacedDigit()
        }
        .onAppear {
            if valores.isEmpty {
                valores = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos)
                ganadoras.columna = "COL.GANADORA"
            }
        }
    }
}

/// «Diferencias entre filtros» (pantalla 33, `DiFiltros`).
struct VistaDiFiltros: View {
    @Environment(Sesion.self) private var sesion
    @State private var filas: [FilaDiFiltros] = []
    @State private var estado = EstadoDiFiltros()
    @State private var activarTodas = true
    @State private var tiempo = ""
    @State private var archivoSalida = ""
    @State private var ganadoras = GanadorasSelector(columna: "Col. Ganadora", fichero: "Fichero ganadoras")
    @State private var tarea: Task<Void, Never>?

    private func iniciar(analisis: Bool) {
        guard !filas.isEmpty, tarea == nil else { return }
        let (f, e, g) = (filas, estado, analisis ? ganadoras.columna : nil)
        let inicio = Date()
        tarea = Task {
            do throws(ErrorDominio) {
                (filas, estado) = try await CasosUtilidades.procesoDiFiltros(f, estado: e, ganadora: g, entorno: sesion.entorno)
            } catch {
                sesion.alertas.error(error.mensajeUsuario)
            }
            tiempo = CasosUtilidades.tiempoCorto(Date().timeIntervalSince(inicio))
            tarea = nil
        }
    }

    private func cargarGanadoras() async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        do throws(ErrorDominio) {
            let r = try CasosUtilidades.ganadorasDiFiltros(u, entorno: sesion.entorno)
            if r.erronea {
                ganadoras.columnas = r.columnas
                return sesion.alertas.error("col.G. errónea")
            }
            ganadoras.cargar(r.columnas, fichero: u)
        } catch {
            sesion.alertas.error("No se pudo cargar el fichero de ganadoras: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .diFiltros) {
            HStack {
                Button("Cargar Filtro") {
                    Task {
                        for u in await SelectorFicheros.abrirVarios([".txt"]) {
                            filas.append(FilaDiFiltros(ruta: u.path(percentEncoded: false), nombre: u.lastPathComponent))
                        }
                    }
                }
                BotonColumnasInicio { filas.append(FilaDiFiltros(ruta: $0.path(percentEncoded: false), nombre: $0.lastPathComponent)) }
                Button("Salvar Lista") {
                    Task {
                        guard let u = await SelectorFicheros.guardar("Lista", [("SalvarLista", ".lst")]) else { return }
                        do throws(ErrorDominio) {
                            try sesion.entorno.archivos.escribir(CasosUtilidades.datosListaDiFiltros(filas), en: u, atomico: false)
                        } catch {
                            sesion.alertas.error("No se pudo guardar la lista: " + error.mensajeUsuario)
                        }
                    }
                }
                Button("Cargar Lista") {
                    Task {
                        guard let u = await SelectorFicheros.abrir(".lst") else { return }
                        filas = []
                        do throws(ErrorDominio) {
                            filas = CasosUtilidades.leerListaDiFiltros(try sesion.entorno.archivos.leer(u), cultura: sesion.entorno.cultura)
                        } catch {
                            sesion.alertas.error("No se pudo cargar la lista: " + error.mensajeUsuario)
                        }
                    }
                }
                Toggle("Activa / Desactiva", isOn: $activarTodas)
                    .onChange(of: activarTodas) { for i in filas.indices { filas[i].activo = activarTodas } }
            }
            Tarjeta("Filtros") {
                Grid(alignment: .leading, horizontalSpacing: 10, verticalSpacing: 3) {
                    GridRow {
                        ForEach(["N", "Filtro", "Activo", "Cols.", "Difs", "Mín.", "Adm.", "14", "13"], id: \.self) { Text($0).bold() }
                    }
                    ForEach(filas.indices, id: \.self) { i in
                        GridRow {
                            Text(String(i + 1)).monospacedDigit()
                            Text(filas[i].nombre).lineLimit(1)
                            Toggle("", isOn: $filas[i].activo).labelsHidden()
                            Text(String(filas[i].columnas)).monospacedDigit()
                            TextField("", text: $filas[i].difs).frame(width: 50)
                                .onChange(of: filas[i].difs) { filas[i].difs = String(filas[i].difs.prefix(4)) }
                            CampoNumero(etiqueta: "", valor: $filas[i].minimo, minimo: 1, maximo: 3305, ancho: 60)
                            Text(String(filas[i].admitidas)).monospacedDigit()
                            Text(String(filas[i].acierta14)).monospacedDigit()
                            Text(String(filas[i].aciertos13)).monospacedDigit()
                        }
                    }
                }
            }
            HStack {
                Button("Iniciar Proceso") { iniciar(analisis: false) }.buttonStyle(EstiloPrincipal())
                Button("Cancelar") { tarea?.cancel() }
                Button("Grabar") {
                    Task {
                        guard let u = await SelectorFicheros.guardar("Columnas", [("Columnas", ".txt")]) else { return }
                        do throws(ErrorDominio) {
                            try CasosUtilidades.grabarDiFiltros(
                                estado, numeroPartidos: sesion.numeroPartidos, en: u, entorno: sesion.entorno)
                            archivoSalida = u.lastPathComponent
                        } catch {
                            sesion.alertas.error("No se pudo grabar: " + error.mensajeUsuario)
                        }
                    }
                }
                Text("Tiempo: " + tiempo).monospacedDigit()
                Text(archivoSalida).foregroundStyle(Color.textoSecundario)
            }
            PanelGanadoras(g: $ganadoras, cargar: cargarGanadoras, analizar: { iniciar(analisis: true) })
        }
    }
}
