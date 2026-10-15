import Free1X2CasosDeUso
import SwiftUI

/// «Modificador de columnas» (pantalla 73, `ModificadorFrm`).
struct VistaModificador: View {
    struct Fila {
        var activo = false
        var valores: [Double] = [0, 0, 0]
    }

    @Environment(Sesion.self) private var sesion
    @State private var editor: EditorModificador?
    @State private var url: URL?
    @State private var filas = Array(repeating: Fila(), count: 14)
    @State private var modo = 2
    @State private var ordenarSignos = true

    private func escribirFrecuencias() {
        guard let e = editor else { return }
        for i in filas.indices where i < e.partidos { filas[i].valores = e.frecuencia[i].map(Double.init) }
    }

    private func abrir() async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        do throws(ErrorDominio) {
            let e = try EditorModificador.cargar(u, entorno: sesion.entorno)
            if filas.count != e.partidos { filas = Array(repeating: Fila(), count: e.partidos) }
            (editor, url) = (e, u)
            escribirFrecuencias()
        } catch {
            sesion.alertas.error("Error al leer el archivo: " + error.mensajeUsuario)
        }
    }

    private func aceptar() {
        guard var e = editor else { return }
        // DEC-938: el reloj del C# se sustituye por el generador del entorno sembrado con los milisegundos.
        var r = sesion.entorno.aleatorio(Int32(truncatingIfNeeded: Int(sesion.entorno.reloj.ahora().timeIntervalSince1970 * 1000)))
        do throws(ErrorDominio) {
            for i in filas.indices where filas[i].activo {
                try e.modificar(partido: i, porcentajes: filas[i].valores, modo: modo, ordenarSignos: ordenarSignos, aleatorio: &r)
                filas[i].activo = false
            }
        } catch {
            sesion.alertas.inesperado(error)
        }
        editor = e
        escribirFrecuencias()
    }

    private func grabar() async {
        guard let e = editor, let url else { return }
        guard
            let s = await SelectorFicheros.guardar(url.deletingPathExtension().lastPathComponent + "_modificado", [("Columnas", ".txt")])
        else { return }
        do throws(ErrorDominio) {
            if let d = try e.datos() { try sesion.entorno.archivos.escribir(d, en: s, atomico: false) }
            sesion.alertas.info("Se ha grabado el archivo.")
        } catch {
            sesion.alertas.error("Error al grabar: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .modificador) {
            Text(
                "Carga un fichero de columnas, ajusta el porcentaje 1/X/2 de cada partido y redistribuye los signos según el modo elegido."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Fichero de entrada") {
                HStack {
                    Button("Abrir fichero…") { Task { await abrir() } }
                    Text(url?.lastPathComponent ?? "").foregroundStyle(Color.textoSecundario)
                }
            }
            HStack(alignment: .top) {
                Tarjeta("Partidos") {
                    Grid(alignment: .leading, horizontalSpacing: 10, verticalSpacing: 3) {
                        GridRow {
                            Text("Nº").bold()
                            Text("").bold()
                            Text("% 1").bold()
                            Text("% X").bold()
                            Text("% 2").bold()
                        }
                        ForEach(filas.indices, id: \.self) { i in
                            GridRow {
                                Text(String(i + 1)).monospacedDigit()
                                Toggle("", isOn: $filas[i].activo).labelsHidden()
                                ForEach(0..<3, id: \.self) { j in
                                    CampoNumero(etiqueta: "", valor: $filas[i].valores[j], minimo: 0, maximo: 100, ancho: 56).labelsHidden()
                                        .disabled(!filas[i].activo)
                                }
                            }
                        }
                    }
                }
                Tarjeta("Distribución de los signos") {
                    Picker("Modo", selection: $modo) {
                        Text("Aleatorio").tag(1)
                        Text("Proporcional").tag(2)
                        Text("Ordenado").tag(3)
                    }
                    .pickerStyle(.radioGroup)
                    Toggle("Ordenar signos", isOn: $ordenarSignos).disabled(modo == 1)
                    HStack {
                        Button("Ok", action: aceptar).buttonStyle(.borderedProminent).disabled(editor == nil)
                        Button("Grabar") { Task { await grabar() } }.disabled(editor == nil)
                        // «Cancelar» no hace nada en el C#.
                        Button("Cancelar") {}
                    }
                }
            }
        }
    }
}

/// «Buscador límites» (pantalla 12, `BuscaLimsFrm`).
struct VistaBuscaLims: View {
    @Environment(Sesion.self) private var sesion
    @State private var valores: [[Double]] = []
    @State private var rango = "4-9"
    @State private var resultado: LimitesValoracion?
    @State private var tiempo = ""
    @State private var calculando = false

    private func texto(_ v: Double?, _ valido: Bool) -> String {
        guard let v, valido else { return "" }
        return FormatoNET.toString(v, cultura: sesion.entorno.cultura)
    }

    var body: some View {
        Pagina(destino: .buscaLims) {
            Text(
                "Recorre todas las columnas y calcula los valores mínimo y máximo de Sumas y Productos para las que aciertan dentro del rango indicado."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                Tarjeta("Valoraciones por partido") {
                    Text("Valoraciones 1 / X / 2 por partido. Son el origen de dv = valors1.RetVals() en el cálculo de límites.")
                        .font(.callout).foregroundStyle(Color.textoSecundario)
                    PorcentajesGrid(valores: $valores)
                }
                VStack(spacing: 10) {
                    Tarjeta("Parámetros") {
                        HStack {
                            Text("Rango")
                            TextField("p. ej. 4-9", text: $rango).textFieldStyle(.roundedBorder).frame(width: 120)
                        }
                        LabeledContent("C.B.", value: resultado?.columnaBase ?? "")
                        Button("Calcular Límites") {
                            Task {
                                calculando = true
                                resultado = nil
                                let t0 = sesion.entorno.reloj.ahora()
                                resultado = await CasosUtilidades.buscarLimites(valores, rango: rango)
                                tiempo = String(
                                    (FormatoNET.toStringTimeSpan(sesion.entorno.reloj.ahora().timeIntervalSince(t0)) + "00000000000")
                                        .prefix(11))
                                calculando = false
                            }
                        }
                        .buttonStyle(.borderedProminent).disabled(calculando)
                    }
                    Tarjeta("Resultados") {
                        Grid(alignment: .leading, horizontalSpacing: 14, verticalSpacing: 4) {
                            GridRow {
                                Text("")
                                Text("Mín").bold()
                                Text("Máx").bold()
                            }
                            GridRow {
                                Text("Sumas")
                                Text(texto(resultado?.sumaMin, (resultado?.sumaMin ?? -1) > -1))
                                Text(texto(resultado?.sumaMax, (resultado?.sumaMax ?? 0) > 0))
                            }
                            GridRow {
                                Text("Productos")
                                Text(texto(resultado?.productoMin, (resultado?.productoMin ?? -1) > -1))
                                Text(texto(resultado?.productoMax, (resultado?.productoMax ?? 0) > 0))
                            }
                        }
                    }
                    Tarjeta("Proceso") {
                        LabeledContent("Procesadas", value: String(resultado?.procesadas ?? 0))
                        LabeledContent("Admitidas", value: String(resultado?.admitidas ?? 0))
                        LabeledContent("Tiempo", value: tiempo)
                    }
                }
            }
        }
        .onAppear { if valores.isEmpty { valores = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos) } }
    }
}

/// «Combinación de Filtros» (pantalla 21, `CombinarFiltros`).
struct VistaCombinarFiltros: View {
    struct Filtro: Identifiable {
        let id = UUID()
        let ruta: String
        var activo = true
        var columnas = 0
    }

    @Environment(Sesion.self) private var sesion
    @State private var filtros: [Filtro] = []
    @State private var partidos = 0
    @State private var cuentas: [Int32] = []
    @State private var activarTodas = true
    @State private var minimo = 1.0
    @State private var maximo = 1.0
    @State private var total = 0
    @State private var salida = ""
    @State private var tarea: Task<Void, Never>?

    private func cargarFiltros() async {
        let urls = await SelectorFicheros.abrirVarios([".txt"])
        guard !urls.isEmpty else { return }
        for u in urls {
            let n = CasosUtilidades.signosDeFiltro(u, entorno: sesion.entorno)
            if partidos == 0 {
                partidos = n
            } else if partidos != n {
                return sesion.alertas.error("Los filtros deben tener el mismo número de partidos")
            }
            filtros.append(Filtro(ruta: u.path(percentEncoded: false)))
        }
        cuentas = []
    }

    private func cargarLista() async {
        guard let u = await SelectorFicheros.abrir(".lst") else { return }
        (filtros, partidos) = ([], 0)
        do throws(ErrorDominio) {
            for (i, r) in try CasosUtilidades.rutasDeLista(u, entorno: sesion.entorno).enumerated() {
                let n = CasosUtilidades.signosDeFiltro(URL(filePath: r), entorno: sesion.entorno)
                if i == 0 {
                    partidos = n
                } else if n != partidos {
                    return sesion.alertas.error("Los archivos no tienen el mismo número de partidos")
                }
                filtros.append(Filtro(ruta: r))
            }
            cuentas = []
        } catch {
            sesion.alertas.error("No se pudo cargar la lista: " + error.mensajeUsuario)
        }
    }

    private func iniciar() async {
        guard !filtros.isEmpty else { return }
        let lista = filtros.map { (url: URL(filePath: $0.ruta), activo: $0.activo) }
        let p = partidos
        let t = Task {
            do throws(ErrorDominio) {
                let r = try await CasosUtilidades.combinarFiltros(lista, partidos: p, entorno: sesion.entorno)
                cuentas = r.cuentas
                for (i, n) in r.columnas.enumerated() where i < filtros.count { filtros[i].columnas = n }
            } catch {
                sesion.alertas.error("No se pudo combinar los filtros:\n\n" + error.mensajeUsuario)
            }
        }
        tarea = t
        await t.value
        tarea = nil
    }

    var body: some View {
        Pagina(destino: .combinarFiltros) {
            Text("Combina varios archivos de columnas y cuenta en cuántos filtros aparece cada una.").foregroundStyle(Color.textoSecundario)
            Tarjeta("Filtros") {
                HStack {
                    Button("Cargar Filtro") { Task { await cargarFiltros() } }
                    Button("Salvar Lista") {
                        Task {
                            guard let u = await SelectorFicheros.guardar("Lista", [("SalvarLista", ".lst")]) else { return }
                            do throws(ErrorDominio) {
                                try sesion.entorno.archivos.escribir(TextoNET.codificarLineas(filtros.map(\.ruta)), en: u, atomico: false)
                            } catch {
                                sesion.alertas.error("No se pudo guardar la lista: " + error.mensajeUsuario)
                            }
                        }
                    }
                    Button("Cargar Lista") { Task { await cargarLista() } }
                    Toggle("Activa / Desactiva", isOn: $activarTodas)
                        .onChange(of: activarTodas) { for i in filtros.indices { filtros[i].activo = activarTodas } }
                }
                ScrollView {
                    Grid(alignment: .leading, horizontalSpacing: 14, verticalSpacing: 3) {
                        GridRow {
                            Text("N").bold()
                            Text("Filtro").bold()
                            Text("Activo").bold()
                            Text("Cols.").bold()
                        }
                        ForEach(filtros.indices, id: \.self) { i in
                            GridRow {
                                Text(String(i + 1)).monospacedDigit()
                                Text(URL(filePath: filtros[i].ruta).lastPathComponent)
                                Toggle("", isOn: $filtros[i].activo).labelsHidden()
                                Text(String(filtros[i].columnas)).monospacedDigit()
                            }
                        }
                    }
                }
                .frame(minHeight: 200)
                Text("Cualquier variación en el cuadro \"Filtros\" requiere repetir el proceso antes de sumar o grabar columnas.")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                HStack {
                    Button("Iniciar Proceso") { Task { await iniciar() } }.buttonStyle(.borderedProminent).disabled(tarea != nil)
                    Button("Cancelar") { tarea?.cancel() }
                    Button("Reiniciar Todo") { (filtros, total, salida, partidos, cuentas) = ([], 0, "", 0, []) }
                }
            }
            Tarjeta("Filtros acertados") {
                HStack {
                    CampoNumero(etiqueta: "Mínimo", valor: $minimo, minimo: 1, ancho: 60)
                    CampoNumero(etiqueta: "Máximo", valor: $maximo, minimo: 1, ancho: 60)
                }
                HStack {
                    Button("Sumar cols.") {
                        let r = CasosUtilidades.rangoFiltros(minimo, maximo, filtros: filtros.count)
                        total = cuentas.filter { r?.contains(Int($0)) ?? false }.count
                    }
                    LabeledContent("Total columnas", value: String(total))
                }
                HStack {
                    Button("Grabar cols.") {
                        Task {
                            guard let u = await SelectorFicheros.guardar("Columnas", [("Columnas", ".txt")]) else { return }
                            do throws(ErrorDominio) {
                                total = try CasosUtilidades.grabarFiltros(
                                    cuentas, rango: CasosUtilidades.rangoFiltros(minimo, maximo, filtros: filtros.count), en: u,
                                    entorno: sesion.entorno)
                                salida = u.lastPathComponent
                            } catch {
                                sesion.alertas.error("No se pudo grabar: " + error.mensajeUsuario)
                            }
                        }
                    }
                    LabeledContent("Archivo de salida", value: salida)
                }
            }
        }
    }
}
