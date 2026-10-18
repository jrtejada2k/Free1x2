import Free1X2CasosDeUso
import SwiftUI

/// «Simulador de escrutinios (Banco de Pruebas)» (pantalla 10, `BancoPruebasFrm`).
struct VistaBancoPruebas: View {
    @Environment(Sesion.self) private var sesion
    @State private var fichero = ""
    @State private var leidas = "0"
    @State private var apostados: [[Double]] = []
    @State private var reales: [[Double]] = []
    @State private var usarLN = true
    @State private var lnMedia14 = -14.7
    @State private var premioMedio14 = 0.0
    @State private var numColumnas = 50000.0
    @State private var origenAleatorias = true
    @State private var ficheroAleatorias = ""
    @State private var numAleatorias = 1000.0
    @State private var aleatorias: [Int] = []
    @State private var desviacion = "0"
    @State private var lnObtenida = "0"
    @State private var tipo = TipoAnalisisBanco.columnas
    // 14, 13, 12, 11, 10.
    @State private var acumular = [true, false, false, false, false]
    // Nº columnas, gasto, premio total, % recuperado, veces premiada y veces beneficio.
    @State private var globales = Array(repeating: "0", count: 6)
    @State private var filas: [String] = []
    @State private var columnas: [FilaBancoColumna] = []
    @State private var jornadas: [String] = []
    @State private var paraCadaFila = false
    @State private var diferencias = 2.0
    @State private var numFila = 0.0
    @State private var ocupado = false
    @State private var rango: (inicial: Int, final: Int, cuantas: Int)?
    @State private var orden: [OrdenBanco] = []
    @State private var limitesFiltro: [(minimo: Double, maximo: Double)]?
    @State private var seleccionadas = ""

    /// La selección de la tabla es la marca `seleccionado` de cada fila (la usan Grabar y Simplificar).
    private var marcadas: Binding<Set<Int>> {
        Binding {
            Set(columnas.lazy.filter(\.seleccionado).map(\.indice))
        } set: { s in
            for i in columnas.indices { columnas[i].seleccionado = s.contains(columnas[i].indice) }
        }
    }

    private func d(_ v: Double) -> String { FormatoNET.toString(v, cultura: sesion.entorno.cultura) }

    /// Columna de la tabla que se ordena al pulsar su cabecera (WinForms `myDataGrid_MouseDown`).
    private func columna(_ titulo: String, _ cabecera: Int, _ texto: @escaping (FilaBancoColumna) -> String)
        -> TableColumn<FilaBancoColumna, OrdenBanco, Text, Text>
    {
        TableColumn(titulo, sortUsing: OrdenBanco(cabecera: cabecera)) { Text(texto($0)).monospacedDigit() }
    }

    private func leerColumnas() async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        fichero = u.path(percentEncoded: false)
        do throws(ErrorDominio) {
            leidas = String(try CasosUtilidades.columnasBanco(u, entorno: sesion.entorno).count)
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    private func calcularReales() async {
        ocupado = true
        defer { ocupado = false }
        do throws(ErrorDominio) {
            reales = try await CasosUtilidades.valoracionReal(
                apostados: apostados, ln: lnMedia14, columnas: Int(FormatoNET.castInt32(numColumnas)))
        } catch {
            sesion.alertas.error("Error calculando la valoración real: " + error.mensajeUsuario)
        }
    }

    private func generar() {
        var a = sesion.entorno.aleatorio(Int32(truncatingIfNeeded: Int(sesion.entorno.reloj.ahora().timeIntervalSince1970 * 1000)))
        let r = CasosUtilidades.columnasAleatorias(
            reales: reales, apostados: apostados, cantidad: Int(FormatoNET.castInt32(numAleatorias)), aleatorio: &a)
        aleatorias = r.columnas
        desviacion = FormatoNET.toString(r.desviacion, cultura: sesion.entorno.cultura)
        lnObtenida = FormatoNET.toString(r.lnMedia, cultura: sesion.entorno.cultura)
    }

    private func escribir(_ columnas: [String], nombre: String) async -> URL? {
        guard let u = await SelectorFicheros.guardar(nombre, [("Columnas", ".txt")]) else { return nil }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(TextoNET.codificarLineas(columnas), en: u, atomico: false)
            return u
        } catch {
            sesion.alertas.error("No se pudieron grabar las columnas en:\n" + u.path(percentEncoded: false) + "\n\n" + error.mensajeUsuario)
            return nil
        }
    }

    private func guardarAleatorias() async {
        guard !aleatorias.isEmpty else { return sesion.alertas.info("No hay columnas aleatorias generadas que guardar.") }
        let conv = ConvertidorDeBases(numeroPartidos: 14)
        var cols: [String] = []
        for c in aleatorias {
            guard let s = try? conv.columna(Int64(c)) else { continue }
            cols.append(s)
        }
        _ = await escribir(cols, nombre: "ColumnasAleatorias")
    }

    private func analizar() async {
        guard !fichero.isEmpty else { return sesion.alertas.info("Selecciona primero el fichero de columnas (paso 1).") }
        guard tipo == .autoescrutinio || !aleatorias.isEmpty else {
            return sesion.alertas.info("Genera primero las columnas aleatorias (paso 3).")
        }
        ocupado = true
        defer { ocupado = false }
        do throws(ErrorDominio) {
            let r = try await CasosUtilidades.analizarBanco(
                tipo, fichero: URL(filePath: fichero), apostados: apostados, aleatorias: aleatorias, acumular: acumular,
                numeroPartidos: sesion.numeroPartidos, lae: sesion.lae, entorno: sesion.entorno)
            for (i, t) in [r.numColumnas, r.gasto, r.premioTotal, r.recuperado, r.vecesPremiada, r.vecesBeneficio].enumerated() {
                if let t { globales[i] = t }
            }
            (filas, columnas, jornadas) = (r.filas ?? filas, r.columnas ?? columnas, r.jornadas ?? jornadas)
            if r.columnas != nil { (orden, seleccionadas) = ([], "") }
        } catch {
            sesion.alertas.error("Error en el escrutinio: " + error.mensajeUsuario)
        }
    }

    private func grabar() {
        guard !columnas.isEmpty else {
            return sesion.alertas.info(
                "No hay resultado por columnas que grabar. Ejecuta primero un análisis por 'Columnas' o 'Autoescrutinio' (paso 4).")
        }
        rango = CasosUtilidades.rangoGrabacionBanco(columnas)
    }

    private func grabarRango(_ r: (inicial: Int, final: Int, maximo: Int, soloSeleccionadas: Bool)) async {
        let cols = CasosUtilidades.columnasGrabacionBanco(
            columnas, inicial: r.inicial, final: r.final, maximo: r.maximo, soloSeleccionadas: r.soloSeleccionadas)
        guard let u = await escribir(cols, nombre: "columnas") else { return }
        sesion.alertas.info("Grabadas \(cols.count) columna(s) en \(u.lastPathComponent).")
    }

    private func simplificar() {
        guard !columnas.isEmpty else {
            return sesion.alertas.info(
                "No hay resultado por columnas que simplificar. Ejecuta primero un análisis por 'Columnas' o 'Autoescrutinio' (paso 4).")
        }
        let fila = paraCadaFila ? nil : Int(FormatoNET.castInt32(numFila))
        if let aviso = CasosUtilidades.simplificarBanco(&columnas, fila: fila, diferencias: diferencias) { sesion.alertas.info(aviso) }
    }

    var body: some View {
        Pagina(destino: .bancoPruebas) {
            Text("Asistente de 4 pasos para generar, valorar y escrutar columnas de quiniela.").foregroundStyle(Color.textoSecundario)
            Tarjeta("Paso 1 · Ficheros") {
                Text("Selección del fichero de columnas a analizar").bold()
                HStack {
                    CampoTexto(etiqueta: "Nombre del fichero (ruta completa incluida)", texto: $fichero)
                    Button("Examinar…") { Task { await leerColumnas() } }
                }
                Text("Columnas leídas: \(leidas)")
            }
            Tarjeta("Paso 2 · Valoraciones") {
                Text(
                    "Porcentajes por partido (1 / X / 2). La rejilla apostada alimenta el cálculo y la generación; la real se obtiene del cálculo o se edita manualmente."
                )
                .foregroundStyle(Color.textoSecundario)
                HStack(alignment: .top, spacing: 24) {
                    VStack(alignment: .leading) {
                        Text("Porcentajes apostados").bold()
                        PorcentajesGrid(valores: $apostados)
                    }
                    VStack(alignment: .leading) {
                        Text("Porcentajes reales").bold()
                        PorcentajesGrid(valores: $reales)
                    }
                    VStack(alignment: .leading) {
                        Text("Obtención de la valoración real a partir de la apostada").bold()
                        Picker("Parámetros", selection: $usarLN) {
                            Text("LN de la Prob. media del 14").tag(true)
                            Text("Premio medio de 14").tag(false)
                        }
                        .pickerStyle(.radioGroup)
                        CampoNumero(etiqueta: "LN de la Prob. media del 14", valor: $lnMedia14).disabled(!usarLN)
                        CampoNumero(etiqueta: "Premio medio de 14", valor: $premioMedio14).disabled(usarLN)
                        CampoNumero(etiqueta: "Nº columnas a considerar", valor: $numColumnas)
                        Button("Calcular") { Task { await calcularReales() } }.disabled(ocupado)
                    }
                }
            }
            Tarjeta("Paso 3 · Simular 14's") {
                HStack(alignment: .top, spacing: 24) {
                    VStack(alignment: .leading) {
                        Picker("Origen de las columnas", selection: $origenAleatorias) {
                            Text("Columnas aleatorias").tag(true)
                            Text("Fichero").tag(false)
                        }
                        .pickerStyle(.radioGroup)
                        CampoTexto(etiqueta: "Fichero de columnas aleatorias", texto: $ficheroAleatorias, ancho: 260)
                            .disabled(origenAleatorias)
                    }
                    VStack(alignment: .leading) {
                        Text("Parámetros de generación").bold()
                        CampoNumero(etiqueta: "Número de columnas", valor: $numAleatorias)
                        HStack {
                            Button("Generar columnas", action: generar)
                            Button("Guardar columnas") { Task { await guardarAleatorias() } }
                        }
                    }
                    VStack(alignment: .leading) {
                        Text("Resultados de la generación").bold()
                        Text("Desviación Típica obtenida: \(desviacion)")
                        Text("LN Probabilidad Media obtenida: \(lnObtenida)")
                    }
                }
            }
            Tarjeta("Paso 4 · Escrutinios") {
                HStack(spacing: 16) {
                    Picker("Tipo de análisis", selection: $tipo) {
                        ForEach(TipoAnalisisBanco.allCases, id: \.self) { Text($0.rawValue).tag($0) }
                    }
                    .frame(width: 260)
                    Text("Premios acumulados que se consideran")
                    ForEach([4, 3, 2, 1, 0], id: \.self) { i in Toggle(String(14 - i), isOn: $acumular[i]) }
                }
                HStack {
                    Button("Analizar") { Task { await analizar() } }.buttonStyle(EstiloPrincipal()).disabled(ocupado)
                    // El C# no cablea «Seleccionar jornadas…» (botón oculto y sin handler en el original).
                    Button("Seleccionar jornadas…") {}
                    Button("Grabar", action: grabar)
                    Button("Filtrar…") { limitesFiltro = CasosUtilidades.limitesBanco(columnas) }
                        .help("Filtro de selección de filas").disabled(columnas.isEmpty)
                    Text(seleccionadas).foregroundStyle(Color.textoSecundario)
                }
                Grid(alignment: .leading, horizontalSpacing: 16) {
                    GridRow {
                        ForEach(
                            ["Nº columnas", "Gasto total", "Premio total", "% recuperado", "Veces premiada", "Veces beneficio"], id: \.self
                        ) {
                            Text($0).font(.caption).foregroundStyle(Color.textoSecundario)
                        }
                    }
                    GridRow { ForEach(globales.indices, id: \.self) { Text(globales[$0]).monospacedDigit() } }
                }
            }
            HStack(alignment: .top, spacing: 16) {
                Tarjeta("Combinación (por nº de aciertos)") {
                    List(Array(filas.enumerated()), id: \.offset) { Text($0.element).monospacedDigit() }.frame(height: 220)
                }
                Tarjeta("Jornadas (por columna aleatoria)") {
                    List(Array(jornadas.enumerated()), id: \.offset) { Text($0.element).monospacedDigit() }
                        .frame(height: 220)
                }
            }
            Tarjeta("Columnas / Autoescrutinio (por columna)") {
                Text("Pulsa una cabecera para ordenar por esa columna; selecciona filas con ⌘ o ⇧ para grabarlas.")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                Table(columnas, selection: marcadas, sortOrder: $orden) {
                    Group {
                        columna("Nºcol.", 0) { String($0.indice + 1) }
                        TableColumn("Columna") { Text($0.columna).monospaced() }
                        columna("Nº14", 1) { String($0.veces[0]) }
                        columna("Nº13", 2) { String($0.veces[1]) }
                        columna("Nº12", 3) { String($0.veces[2]) }
                        columna("Nº11", 4) { String($0.veces[3]) }
                        columna("Nº10", 5) { String($0.veces[4]) }
                        columna("Nºacumulado", 6) { String($0.vecesAcumulado) }
                    }
                    Group {
                        columna("Premio 14", 7) { d(FormatoNET.round($0.premios[0], decimales: 0)) }
                        columna("Premio 13", 8) { d(FormatoNET.round($0.premios[1], decimales: 0)) }
                        columna("Premio 12", 9) { d(FormatoNET.round($0.premios[2], decimales: 0)) }
                        columna("Premio 11", 10) { d(FormatoNET.round($0.premios[3], decimales: 0)) }
                        columna("Premio 10", 11) { d(FormatoNET.round($0.premios[4], decimales: 0)) }
                        columna("Premio Acumulado", 12) { $0.premio }
                    }
                    Group {
                        columna("14 medio", 13) { d($0.premioMedio(0)) }
                        columna("13 medio", 14) { d($0.premioMedio(1)) }
                        columna("12 medio", 15) { d($0.premioMedio(2)) }
                        columna("11 medio", 16) { d($0.premioMedio(3)) }
                        columna("10 medio", 17) { d($0.premioMedio(4)) }
                        columna("Recuperación", 18) { $0.recuperacion }
                    }
                }
                .onChange(of: orden) { columnas.sort(using: orden) }
                .frame(height: 260)
                .accessibilityIdentifier("banco-columnas")
            }
            .sheet(isPresented: Binding(get: { limitesFiltro != nil }, set: { if !$0 { limitesFiltro = nil } })) {
                VistaFiltroBanco(limites: limitesFiltro ?? []) { criterios in
                    limitesFiltro = nil
                    guard let criterios else { return }
                    seleccionadas = "\(CasosUtilidades.seleccionarBanco(&columnas, criterios)) filas seleccionadas"
                }
            }
            Tarjeta("Eliminación de filas") {
                HStack {
                    Text("Eliminar las apuestas con menos de")
                    CampoNumero(etiqueta: "", valor: $diferencias, ancho: 60).labelsHidden()
                    Text("diferencias")
                }
                Picker("", selection: $paraCadaFila) {
                    Text("Sólo respecto de la fila actual").tag(false)
                    Text("Respecto a cada fila seleccionada").tag(true)
                }
                .pickerStyle(.radioGroup).labelsHidden()
                CampoNumero(etiqueta: "Fila Nº", valor: $numFila)
                HStack {
                    Button("Simplificar", action: simplificar)
                    // El C# no cablea este «Cancelar» (DEC-915).
                    Button("Cancelar") {}
                }
            }
        }
        .onAppear {
            if apostados.isEmpty {
                apostados = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos)
                reales = apostados
            }
        }
        .sheet(
            isPresented: Binding(get: { rango != nil }, set: { if !$0 { rango = nil } })
        ) {
            VistaDialogoGrabarBancoPruebas(inicial: rango ?? (0, 0, 0)) { r in
                rango = nil
                if let r { Task { await grabarRango(r) } }
            }
            .environment(sesion)
        }
    }
}

/// «Selección de columnas» (WinForms `DialogoSeleccionBancoPruebasFrm`, que el port WinUI no trae): mínimo y máximo por
/// concepto, con los límites de la tabla como valores iniciales y ningún concepto marcado.
struct VistaFiltroBanco: View {
    let alTerminar: ([(concepto: Int, minimo: Double, maximo: Double)]?) -> Void
    @State private var marcados: [Bool]
    @State private var minimos: [Double]
    @State private var maximos: [Double]

    init(limites: [(minimo: Double, maximo: Double)], alTerminar: @escaping ([(concepto: Int, minimo: Double, maximo: Double)]?) -> Void) {
        self.alTerminar = alTerminar
        let n = CasosUtilidades.conceptosBanco.count
        let l = limites.count == n ? limites : Array(repeating: (0, 0), count: n)
        _marcados = State(initialValue: Array(repeating: false, count: n))
        _minimos = State(initialValue: l.map(\.minimo))
        _maximos = State(initialValue: l.map(\.maximo))
    }

    var body: some View {
        VStack(alignment: .leading, spacing: 10) {
            Text("Selección de columnas").font(.headline)
            Text("Se seleccionarán las columnas comprendidas entre los valores mínimo y máximo de los conceptos marcados.")
                .font(.callout).foregroundStyle(Color.textoSecundario)
            Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 4) {
                GridRow {
                    Text("Concepto").bold()
                    Text("Mínimo").bold()
                    Text("Máximo").bold()
                }
                ForEach(CasosUtilidades.conceptosBanco.indices, id: \.self) { i in
                    GridRow {
                        Toggle(CasosUtilidades.conceptosBanco[i], isOn: $marcados[i])
                        CampoNumero(etiqueta: "", valor: $minimos[i], ancho: 100)
                        CampoNumero(etiqueta: "", valor: $maximos[i], ancho: 100)
                    }
                }
            }
            HStack {
                Spacer()
                Button("Cancelar") { alTerminar(nil) }.keyboardShortcut(.cancelAction)
                Button("Aceptar") { alTerminar(marcados.indices.filter { marcados[$0] }.map { ($0, minimos[$0], maximos[$0]) }) }
                    .buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction)
            }
        }
        .padding(20)
    }
}

/// «Grabar apuestas» (pantalla 36, `DialogoGrabarBancoPruebasFrm`): hoja del Banco o destino suelto (sin quien lo cierre).
struct VistaDialogoGrabarBancoPruebas: View {
    var inicial: (inicial: Int, final: Int, cuantas: Int) = (0, 0, 0)
    var cerrar: ((inicial: Int, final: Int, maximo: Int, soloSeleccionadas: Bool)?) -> Void = { _ in }
    @Environment(Sesion.self) private var sesion
    @State private var filaInicial = 0.0
    @State private var filaFinal = 0.0
    @State private var maximo = 0.0
    @State private var solo = true
    @State private var estado = "Preparado"
    @State private var listo = false

    var body: some View {
        Pagina(destino: .dialogoGrabarBancoPruebas) {
            Text(
                "Exporta a un archivo las apuestas del Banco de Pruebas. Indica el rango de filas a grabar y el número máximo de apuestas."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Rango de apuestas") {
                CampoNumero(etiqueta: "Fila de la apuesta inicial", valor: $filaInicial, minimo: 0)
                CampoNumero(etiqueta: "Fila de la apuesta final", valor: $filaFinal, minimo: 0)
                CampoNumero(etiqueta: "Nº máximo de apuestas", valor: $maximo, minimo: 0)
            }
            Tarjeta("Opciones") {
                Picker("Grabar solo si están seleccionadas", selection: $solo) {
                    Text("Sí").tag(true)
                    Text("No").tag(false)
                }
                .pickerStyle(.radioGroup)
            }
            HStack {
                Button("Aceptar") {
                    guard !(filaInicial > filaFinal) else {
                        return sesion.alertas.error("La fila inicial no puede ser mayor que la fila final.")
                    }
                    estado = "Aceptado"
                    cerrar(
                        (
                            Int(FormatoNET.castInt32(filaInicial)), Int(FormatoNET.castInt32(filaFinal)), Int(FormatoNET.castInt32(maximo)),
                            solo
                        ))
                }
                .buttonStyle(EstiloPrincipal())
                Button("Cancelar") {
                    estado = "Cancelado"
                    cerrar(nil)
                }
                Text(estado).foregroundStyle(Color.textoSecundario)
            }
        }
        .frame(minWidth: 480, minHeight: 380)
        .onAppear {
            guard !listo else { return }
            listo = true
            (filaInicial, filaFinal, maximo) = (Double(inicial.inicial), Double(inicial.final), Double(inicial.cuantas))
        }
    }
}
