import AppKit
import Free1X2CasosDeUso
import SwiftUI

/// «Tramificar» (pantalla 93, `TramificarForm`).
struct VistaTramificar: View {
    @Environment(Sesion.self) private var sesion
    @State private var columnaInicial = 0.0
    @State private var columnaFinal = 4_782_969.0
    @State private var columnasPorTramo = 531_441.0
    @State private var numTramos = "9"
    @State private var columnaPremiada = "1X1222X2X12121"
    @State private var temporada = "2004"
    @State private var jornada = 7.0
    @State private var recaudacion = "7768049"
    // 14…10.
    @State private var premios = ["1165207,35", "29877,11", "1753,51", "165,38", "22,79"]
    // Los «Bloq.» no se usan en ningún cálculo, como en el C#.
    @State private var bloqueados = [false, false, false, false, false]
    @State private var valoraciones: [[Double]] = []
    // Mín14, Máx14, Mín13 … Máx10.
    @State private var limites = Array(repeating: "", count: 10)
    @State private var columnasFiltro = "0"
    @State private var columnaEnPosicion = ""
    @State private var aciertos = "0"
    @State private var probabilidad = "0"
    @State private var aciertosABuscar = "14"
    @State private var posicion = 1
    @State private var lnCentral = "0"
    @State private var ln = 0.0
    @State private var porProductos = true
    @State private var acumular = false
    @State private var estado = "Faltan datos"
    @State private var motor = EstadoTramificar()
    @State private var filas: [[String]] = []
    @State private var calculando = false
    @State private var extremos: [[Int]]?

    private static let total = 4_782_969.0
    private static let anchos: [CGFloat] = [50, 70, 70, 60, 150, 40, 40, 50, 50, 60, 70, 130, 130]

    private func porTramo(_ final: Double) {
        if let n = Int(numTramos), n > 0 { columnasPorTramo = (final / Double(n)).rounded(.down) }
    }

    private func tramificar() async {
        if columnaFinal > Self.total {
            columnaFinal = Self.total
            porTramo(columnaFinal)
        }
        estado = "Calculando ..."
        calculando = true
        var p = ParametrosTramificar(
            columnaInicial: columnaInicial, columnaFinal: columnaFinal, columnasPorTramo: columnasPorTramo,
            columnaPremiada: columnaPremiada, premios: premios, valoraciones: valoraciones, ln: ln, porProductos: porProductos,
            acumular: acumular)
        p.precio = sesion.configuracion.lae.precioApuesta
        let (flujo, canal) = AsyncStream.makeStream(of: String.self, bufferingPolicy: .bufferingNewest(1))
        let escucha = Task { for await t in flujo { estado = t } }
        do throws(ErrorDominio) {
            motor = try await CasosUtilidades.tramificar(motor, p, cultura: sesion.entorno.cultura) { _ = canal.yield($0) }
            canal.finish()
            await escucha.value
            limites = motor.limites
            filas = motor.tramos.map { CasosUtilidades.filaTramo($0, cultura: sesion.entorno.cultura) }
            posicion = 1
            actualizarColumna()
            estado = "Finalizado"
        } catch {
            canal.finish()
            await escucha.value
            sesion.alertas.error("No se pudieron calcular los tramos: " + error.mensajeUsuario)
            estado = "Error en el cálculo de tramos."
        }
        calculando = false
    }

    private func actualizarColumna() {
        guard
            let r = CasosUtilidades.columnaTramificar(
                motor, posicion: posicion, premiada: columnaPremiada, cultura: sesion.entorno.cultura)
        else { return }
        (columnaEnPosicion, probabilidad, aciertos) = (r.columna, r.valoracion, r.aciertos)
    }

    private func mover(_ hacia: Int) {
        guard motor.tramificado else { return estado = "Primero hay que tramificar." }
        if let p = CasosUtilidades.moverTramificar(motor, desde: posicion, aciertos: Int(aciertosABuscar) ?? 14, hacia: hacia) {
            posicion = p
            actualizarColumna()
        }
    }

    private func buscar() {
        guard motor.tramificado else { return estado = "Primero hay que tramificar." }
        guard !columnaEnPosicion.isEmpty else { return }
        do throws(ErrorDominio) {
            if let p = try CasosUtilidades.buscarTramificar(motor, columna: columnaEnPosicion) {
                posicion = p
                actualizarColumna()
            }
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    private func filtrar() {
        guard motor.tramificado else { return estado = "Primero hay que tramificar para escrutar las columnas." }
        extremos = CasosUtilidades.extremosTramificar(&motor, limites: limites, cultura: sesion.entorno.cultura)
    }

    private func aplicarFiltro(_ e: [[Int]]) async {
        guard motor.tramificado else { return estado = "Primero hay que tramificar para escrutar las columnas." }
        estado = "Eliminando columnas ..."
        do throws(ErrorDominio) {
            let r = try await CasosUtilidades.filtrarTramos(motor, extremos: e)
            motor = r.estado
            columnasFiltro = String(r.quedan)
            estado = "Finalizado"
        } catch {
            sesion.alertas.error("No se pudo aplicar el filtro: " + error.mensajeUsuario)
            estado = "Error al filtrar."
        }
    }

    private func grabarFiltro() async {
        guard motor.tramificado else { return estado = "Primero hay que tramificar y filtrar." }
        guard let u = await SelectorFicheros.guardar("Columnas", [("Columnas", ".txt")]) else { return }
        estado = "Guardando columnas ..."
        do throws(ErrorDominio) {
            let cols = try await CasosUtilidades.columnasFiltradas(motor)
            try sesion.entorno.archivos.escribir(TextoNET.codificarLineas(cols), en: u, atomico: false)
            estado = "Grabadas \(cols.count) columnas"
        } catch {
            sesion.alertas.error("No se pudieron grabar las columnas: " + error.mensajeUsuario)
            estado = "Error al grabar el filtro."
        }
    }

    private func guardarLimites() async {
        guard let u = await SelectorFicheros.guardar("Limites", [("Límites", ".txt")]) else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(TextoNET.codificarLineas(limites), en: u, atomico: false)
            estado = "Límites guardados."
        } catch {
            sesion.alertas.error("No se pudieron guardar los límites: " + error.mensajeUsuario)
        }
    }

    private func leerLimites() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        do throws(ErrorDominio) {
            limites = CasosUtilidades.leerLimitesTramificar(try sesion.entorno.archivos.leer(u))
            estado = "Límites cargados."
        } catch {
            sesion.alertas.error("No se pudieron leer los límites: " + error.mensajeUsuario)
        }
    }

    private func guardarJornada() {
        do throws(ErrorDominio) {
            try CasosUtilidades.guardarJornadaTramificar(
                columna: columnaPremiada, temporada: temporada, temporadaFin: "2005", jornada: jornada, recaudacion: recaudacion,
                premios: premios, lae: sesion.configuracion.lae, entorno: sesion.entorno)
            estado = "Datos de la jornada L.A.E. guardados."
        } catch {
            sesion.alertas.error("No se pudo guardar la jornada L.A.E.: " + error.mensajeUsuario)
        }
    }

    private func verGraficas() {
        guard !motor.tramos.isEmpty else { return estado = "No hay tramos calculados que representar." }
        sesion.tramosAnalizados = motor.tramos
        sesion.router.navegar(.tramificarGraficas)
    }

    private func mayusculas14(_ s: Binding<String>) -> Binding<String> {
        Binding(get: { s.wrappedValue }, set: { s.wrappedValue = String(FormatoNET.mayusculas($0).prefix(14)) })
    }

    @ViewBuilder private var tablaResultados: some View {
        HStack(spacing: 4) {
            ForEach(
                Array(
                    [
                        "Nº", "Col. Inf.", "Col. Sup.", "Nº Cols.", "Prob. Max.", "14", "13", "12", "11", "10", "NºPremios",
                        "Imp.Premios",
                        "Ingresos-Gastos",
                    ].enumerated()), id: \.offset
            ) { Text($0.element).frame(width: Self.anchos[$0.offset], alignment: .trailing) }
        }
        .font(.caption).foregroundStyle(Color.textoSecundario)
        List(Array(filas.enumerated()), id: \.offset) { f in
            HStack(spacing: 4) {
                ForEach(Array(f.element.enumerated()), id: \.offset) { c in
                    Text(c.element).frame(width: Self.anchos[c.offset], alignment: .trailing).lineLimit(1)
                }
            }
            .monospacedDigit()
        }
        .frame(width: Self.anchos.reduce(0, +) + 4 * 12 + 40, height: 240)
    }

    var body: some View {
        Pagina(destino: .tramificar) {
            Text(
                "Reparte el universo de columnas (3^14) en tramos según su valoración, usando los datos de premios de la L.A.E. de una jornada, y permite filtrar por posiciones y localizar columnas concretas."
            )
            .foregroundStyle(Color.textoSecundario)
            Text(estado).monospacedDigit().foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top, spacing: 16) {
                Tarjeta("Definición del tramo") {
                    CampoNumero(etiqueta: "Columna inicial", valor: $columnaInicial)
                    CampoNumero(etiqueta: "Columna final", valor: $columnaFinal)
                    CampoNumero(etiqueta: "Columnas / tramo", valor: $columnasPorTramo)
                    Picker("Nº de tramos", selection: $numTramos) {
                        ForEach(["9", "27", "81", "243", "729", "2187", "6561", "19683", "59049", "177147", "531441"], id: \.self) {
                            Text($0).tag($0)
                        }
                    }
                    .frame(width: 220)
                }
                Tarjeta("Datos L.A.E.") {
                    CampoTexto(etiqueta: "Columna premiada", texto: mayusculas14($columnaPremiada), ancho: 160)
                    HStack {
                        CampoTexto(
                            etiqueta: "Temporada", texto: Binding(get: { temporada }, set: { temporada = String($0.prefix(14)) }), ancho: 60
                        )
                        Text("/ 2005").foregroundStyle(Color.textoSecundario)
                    }
                    CampoNumero(etiqueta: "Jornada", valor: $jornada, minimo: 1, maximo: 70, ancho: 60)
                    CampoTexto(etiqueta: "Recaudación", texto: $recaudacion, ancho: 120)
                }
                Tarjeta("Premios") {
                    Text("Importe del premio por categoría de aciertos. El interruptor bloquea esa categoría en el cálculo.")
                        .font(.caption).foregroundStyle(Color.textoSecundario).frame(width: 260, alignment: .leading)
                    ForEach(0..<5, id: \.self) { i in
                        HStack {
                            CampoTexto(etiqueta: String(14 - i), texto: $premios[i], ancho: 110)
                            Toggle("Bloq.", isOn: $bloqueados[i])
                        }
                    }
                    Button("Guardar jornada L.A.E.", action: guardarJornada)
                }
            }
            HStack(alignment: .top, spacing: 16) {
                Tarjeta("Valoración por partido") {
                    PorcentajesGrid(valores: $valoraciones)
                    Button("Guardar en histórico de valoraciones") {
                        sesion.historiaValoraciones = (
                            valoraciones, Int(FormatoNET.recortar(temporada)) ?? 0, Int(FormatoNET.castInt32(jornada)), nil
                        )
                        sesion.router.navegar(.historiaValoraciones)
                    }
                }
                VStack(alignment: .leading, spacing: 16) {
                    Tarjeta("Posiciones mínimas y máximas") {
                        Grid(alignment: .leading) {
                            GridRow {
                                Text("de")
                                Text("Mínima")
                                Text("Máxima")
                            }
                            .font(.caption).foregroundStyle(Color.textoSecundario)
                            ForEach(0..<5, id: \.self) { i in
                                GridRow {
                                    Text(String(14 - i))
                                    TextField("", text: $limites[2 * i]).frame(width: 90)
                                    TextField("", text: $limites[2 * i + 1]).frame(width: 90)
                                }
                            }
                        }
                        Text("Nº columnas filtro: \(columnasFiltro)")
                        HStack {
                            Button("Filtrar", action: filtrar)
                            Button("Grabar filtro") { Task { await grabarFiltro() } }
                            Button("Leer límites") { Task { await leerLimites() } }
                            Button("Guardar límites") { Task { await guardarLimites() } }
                        }
                    }
                    Tarjeta("Valoración") {
                        CampoTexto(etiqueta: "LN central", texto: $lnCentral, ancho: 100)
                        Picker("", selection: $porProductos) {
                            Text("Ordenar por sumas").tag(false)
                            Text("Ordenar por productos (logaritmos neperianos)").tag(true)
                        }
                        .pickerStyle(.radioGroup).labelsHidden()
                        Picker("", selection: $acumular) {
                            Text("No acumular").tag(false)
                            Text("Acumular resultados de jornadas distintas").tag(true)
                        }
                        .pickerStyle(.radioGroup).labelsHidden()
                    }
                }
                Tarjeta("Ver columna") {
                    CampoTexto(etiqueta: "Columna", texto: mayusculas14($columnaEnPosicion), ancho: 160)
                    Text("Aciertos: \(aciertos)")
                    Picker("Aciertos a buscar", selection: $aciertosABuscar) {
                        ForEach(["14", "13", "12", "11", "10"], id: \.self) { Text($0).tag($0) }
                    }
                    .frame(width: 200)
                    Text("Valoración / LN: \(probabilidad)")
                    HStack {
                        Button("Anterior") { mover(-1) }
                        Button("Siguiente") { mover(1) }
                        Button("Buscar columna", action: buscar)
                    }
                }
            }
            Tarjeta("Resultados") {
                // Las 13 columnas miden ~1020 pt: con la ventana estrecha se desplazan en horizontal y no empujan la página.
                ScrollView(.horizontal) { VStack(alignment: .leading) { tablaResultados } }
            }
            HStack {
                Button("Ver gráficas", action: verGraficas)
                Button("Análisis múltiple") { sesion.router.navegar(.dialogoAnalisisMultipleDeTramos) }
                Button("Tramificar") { Task { await tramificar() } }.buttonStyle(EstiloPrincipal()).disabled(calculando)
                Button("Grabar tramos") { sesion.router.navegar(.dialogoGrabarTramos) }
                Button("Cancelar") { sesion.router.atras() }
            }
        }
        .onAppear { if valoraciones.isEmpty { valoraciones = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos) } }
        .onChange(of: numTramos) {
            porTramo(columnaFinal)
            acumular = false
        }
        .onChange(of: columnaFinal) { porTramo(columnaFinal) }
        .onChange(of: columnaPremiada) { motor.escrutado = false }
        .onChange(of: valoraciones) { motor.escrutado = false }
        .onChange(of: lnCentral) {
            if lnCentral != "-",
                let v = FormatoNET.tryParseDouble(lnCentral, cultura: sesion.entorno.cultura)
                    ?? FormatoNET.tryParseDouble(lnCentral.replacingOccurrences(of: ",", with: "."), cultura: .invariante)
            {
                ln = v
            }
        }
        .sheet(isPresented: Binding(get: { extremos != nil }, set: { if !$0 { extremos = nil } })) {
            VistaDialogoFiltrarPorLimites(extremos: extremos) { r in
                extremos = nil
                if let r { Task { await aplicarFiltro(r) } }
            }
            .environment(sesion)
        }
    }
}

/// «Gráficas de los resultados del análisis de tramos» (pantalla 94, `TramificarGraficasFrm`).
struct VistaTramificarGraficas: View {
    @Environment(Sesion.self) private var sesion
    @State private var curva = "Probabilidad acumulada"
    @State private var tamanoGrid = 50.0
    @State private var curvas: [Curva] = []
    @State private var estado = "Selecciona una curva para dibujarla sobre la gráfica."

    struct Curva: Identifiable {
        var id: String { nombre }
        let nombre: String
        let puntos: [(x: Int, y: Int)]
    }

    /// `ColorDeCurva`: fijos, no siguen el tema.
    static func color(_ curva: String) -> Color {
        let rgb: (Double, Double, Double) =
            switch curva {
            case "14 aciertos": (0x1E, 0x90, 0xFF)
            case "13 aciertos": (0xA5, 0x2A, 0x2A)
            case "12 aciertos": (0x00, 0xFF, 0xFF)
            case "11 aciertos": (0x00, 0x80, 0x00)
            case "10 aciertos": (0xFF, 0x00, 0x00)
            case "Nº de aciertos / columnas premiadas": (0x5F, 0x9E, 0xA0)
            case "Importe de premios": (0x22, 0x8B, 0x22)
            case "Balance (ingresos - gastos)": (0xFF, 0xA5, 0x00)
            default: (0, 0, 0)
            }
        return Color(red: rgb.0 / 255, green: rgb.1 / 255, blue: rgb.2 / 255)
    }

    private func dibujar() {
        guard !sesion.tramosAnalizados.isEmpty else { return estado = "No hay tramos analizados que representar." }
        let puntos = CasosUtilidades.puntosCurvaTramos(sesion.tramosAnalizados, curva: curva)
        curvas.removeAll { $0.nombre == curva }
        curvas.append(Curva(nombre: curva, puntos: puntos))
        estado = "Curva '\(curva)': \(puntos.count) puntos dibujados."
    }

    @MainActor private func copiar() async {
        guard !curvas.isEmpty else { return estado = "No hay ninguna curva dibujada que copiar." }
        let r = ImageRenderer(content: Lienzo(curvas: curvas, intervalo: tamanoGrid).background(.white))
        guard let imagen = r.nsImage, let tiff = imagen.tiffRepresentation, let rep = NSBitmapImageRep(data: tiff),
            let png = rep.representation(using: .png, properties: [:])
        else { return sesion.alertas.error("No se ha podido copiar la imagen de la gráfica.") }
        NSPasteboard.general.clearContents()
        NSPasteboard.general.writeObjects([imagen])
        estado = "Imagen de la gráfica copiada al portapapeles."
        guard let url = await SelectorFicheros.guardar("grafica_tramos", [("Imagen PNG", ".png")], en: URL.picturesDirectory)
        else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(png, en: url, atomico: true)
            estado = "Imagen de la gráfica guardada en " + url.lastPathComponent + "."
        } catch {
            sesion.alertas.error("No se ha podido copiar la imagen de la gráfica: " + error.mensajeUsuario)
        }
    }

    /// `GraficoLineasControl` (10 §1.9): márgenes 44/12/12/24, escala común, Y hacia arriba, marcadores si ≤ 64 puntos.
    struct Lienzo: View {
        let curvas: [Curva]
        let intervalo: Double

        static func etiqueta(_ v: Double) -> String {
            // ponytail: los puntos son enteros; «0.##» solo con escalas no enteras, que aquí no se dan.
            abs(v - v.rounded()) < 1e-6 ? String(Int64(v.rounded())) : String(format: "%.2f", v)
        }

        var body: some View {
            Canvas { ctx, tam in
                let (izq, der, sup, inf) = (44.0, 12.0, 12.0, 24.0)
                let (ancho, alto) = (tam.width, tam.height)
                let paso = intervalo > 4 ? intervalo : 50
                var rejilla = Path()
                for x in stride(from: izq, through: ancho - der, by: paso) {
                    rejilla.move(to: CGPoint(x: x, y: sup))
                    rejilla.addLine(to: CGPoint(x: x, y: alto - inf))
                }
                for y in stride(from: sup, through: alto - inf, by: paso) {
                    rejilla.move(to: CGPoint(x: izq, y: y))
                    rejilla.addLine(to: CGPoint(x: ancho - der, y: y))
                }
                ctx.stroke(rejilla, with: .color(.gray.opacity(0.25)), lineWidth: 0.5)
                let todos = curvas.flatMap(\.puntos)
                var (xMin, xMax, yMin, yMax) = (0.0, 1.0, 0.0, 1.0)
                if !todos.isEmpty {
                    (xMin, xMax) = (Double(todos.map(\.x).min() ?? 0), Double(todos.map(\.x).max() ?? 0))
                    (yMin, yMax) = (Double(todos.map(\.y).min() ?? 0), Double(todos.map(\.y).max() ?? 0))
                    if xMax - xMin < 1e-9 { xMax = xMin + 1 }
                    if yMax - yMin < 1e-9 { yMax = yMin + 1 }
                }
                var ejes = Path()
                ejes.move(to: CGPoint(x: izq, y: sup))
                ejes.addLine(to: CGPoint(x: izq, y: alto - inf))
                ejes.addLine(to: CGPoint(x: ancho - der, y: alto - inf))
                ctx.stroke(ejes, with: .color(.secondary), lineWidth: 1.5)
                for (v, x, y) in [
                    (yMax, 2, sup - 2), (yMin, 2, alto - inf - 8), (xMin, izq, alto - inf + 4), (xMax, ancho - der - 28, alto - inf + 4),
                ] {
                    ctx.draw(
                        Text(Self.etiqueta(v)).font(.system(size: 11)).foregroundStyle(.secondary), at: CGPoint(x: x, y: y),
                        anchor: .topLeading)
                }
                let (aw, ah) = (ancho - izq - der, alto - sup - inf)
                guard aw > 0, ah > 0 else { return }
                for c in curvas where !c.puntos.isEmpty {
                    let pts = c.puntos.map {
                        CGPoint(
                            x: izq + (Double($0.x) - xMin) / (xMax - xMin) * aw, y: sup + (yMax - Double($0.y)) / (yMax - yMin) * ah)
                    }
                    var linea = Path()
                    linea.addLines(pts)
                    let color = VistaTramificarGraficas.color(c.nombre)
                    ctx.stroke(linea, with: .color(color), style: StrokeStyle(lineWidth: 2, lineJoin: .round))
                    if pts.count <= 64 {
                        for p in pts {
                            ctx.fill(Path(ellipseIn: CGRect(x: p.x - 2.5, y: p.y - 2.5, width: 5, height: 5)), with: .color(color))
                        }
                    }
                }
            }
            .frame(width: 900, height: 360)
        }
    }

    var body: some View {
        Pagina(destino: .tramificarGraficas) {
            Text("Representa las curvas de probabilidad, aciertos, premios y balance sobre los tramos analizados.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta("Curva a representar") {
                HStack(spacing: 16) {
                    Picker("Tipo de curva", selection: $curva) {
                        ForEach(CasosUtilidades.curvasTramos, id: \.self) { Text($0).tag($0) }
                    }
                    .frame(width: 380)
                    CampoNumero(etiqueta: "Tamaño del grid", valor: $tamanoGrid, minimo: 10, maximo: 200, ancho: 60)
                }
                HStack {
                    Button("Dibujar curva", action: dibujar).buttonStyle(EstiloPrincipal())
                    Button("Limpiar imagen") {
                        curvas = []
                        estado = "Imagen limpiada. Selecciona una curva para volver a dibujar."
                    }
                    Button("Copiar al portapapeles") { Task { await copiar() } }
                }
                Text(estado).foregroundStyle(Color.textoSecundario)
            }
            Tarjeta("Leyenda de curvas") {
                HStack(spacing: 12) {
                    ForEach(
                        [
                            ("Probabilidad", "Probabilidad acumulada"), ("14 aciertos", "14 aciertos"), ("13 aciertos", "13 aciertos"),
                            ("12 aciertos", "12 aciertos"), ("11 aciertos", "11 aciertos"), ("10 aciertos", "10 aciertos"),
                            ("Columnas premiadas", "Nº de aciertos / columnas premiadas"), ("Importe premios", "Importe de premios"),
                            ("Balance", "Balance (ingresos - gastos)"),
                        ], id: \.0
                    ) { e in
                        HStack(spacing: 4) {
                            Rectangle().fill(Self.color(e.1)).frame(width: 14, height: 4)
                            Text(e.0).font(.caption)
                        }
                    }
                }
            }
            Lienzo(curvas: curvas, intervalo: tamanoGrid).border(Color.borde)
        }
    }
}

/// «Filtrar por límites» (pantalla 35, `DialogoFiltrarPorLimitesFrm`): hoja de Tramificar o destino suelto (vuelve atrás).
struct VistaDialogoFiltrarPorLimites: View {
    var extremos: [[Int]]?
    var cerrar: (([[Int]]?) -> Void)?
    @Environment(Sesion.self) private var sesion
    @State private var filas: [FilaLimite] = []

    private func salir(_ r: [[Int]]?) {
        if let cerrar { cerrar(r) } else { sesion.router.atras() }
    }

    var body: some View {
        Pagina(destino: .dialogoFiltrarPorLimites) {
            Text("Por cada tramo de aciertos, elimina las columnas cuyo número de diferencias quede dentro del rango indicado.")
                .foregroundStyle(Color.textoSecundario)
            Grid(alignment: .leading, horizontalSpacing: 16) {
                GridRow {
                    Text("Rango de aciertos")
                    Text("Posiciones")
                    Text("Difs. desde")
                    Text("Difs. hasta")
                }
                .font(.caption).foregroundStyle(Color.textoSecundario)
                ForEach($filas) { $f in
                    GridRow {
                        Text(f.etiqueta)
                        Text(f.rango).monospacedDigit()
                        CampoNumero(etiqueta: "", valor: $f.difMin, minimo: 0, ancho: 60).labelsHidden()
                        CampoNumero(etiqueta: "", valor: $f.difMax, minimo: 0, ancho: 60).labelsHidden()
                    }
                }
            }
            HStack {
                Button("Cancelar") { salir(nil) }
                Button("Aceptar") { if filas.count == 10 { salir(CasosUtilidades.aceptarLimites(filas)) } }.buttonStyle(EstiloPrincipal())
            }
        }
        .frame(minWidth: 520, minHeight: 460)
        .onAppear { if filas.isEmpty { filas = CasosUtilidades.filasLimites(extremos) } }
    }
}

/// «Grabar Tramos» (pantalla 37, `DialogoGrabarTramosFrm`): como en el C#, nadie consume el resultado (DEC-815).
struct VistaDialogoGrabarTramos: View {
    @State private var inicial = 0.0
    @State private var final = 0.0
    @State private var n = 1.0
    @State private var p = 1.0

    var body: some View {
        Pagina(destino: .dialogoGrabarTramos) {
            Tarjeta("Rango de columnas") {
                CampoNumero(etiqueta: "Columna inicial", valor: $inicial, minimo: 0)
                CampoNumero(etiqueta: "Columna final", valor: $final, minimo: 0)
            }
            Tarjeta("Grabar") {
                Text("Grabar N columnas de cada P columnas.").foregroundStyle(Color.textoSecundario)
                CampoNumero(etiqueta: "Columnas a grabar (N)", valor: $n, minimo: 1)
                CampoNumero(etiqueta: "De cada (P) columnas", valor: $p, minimo: 1)
            }
            Button("Aceptar") {}.buttonStyle(EstiloPrincipal())
        }
    }
}

/// «Definición múltiple de jornadas y ficheros» (pantalla 34, `DialogoAnalisisMultipleDeTramosFrm`): como en el C#,
/// nadie consume el resultado (DEC-815).
struct VistaDialogoAnalisisMultipleDeTramos: View {
    @Environment(Sesion.self) private var sesion
    @State private var ficheroValoraciones = ""
    @State private var rutaValoraciones: URL?
    @State private var temporadas: [String] = []
    @State private var marcadas: Set<String> = []
    @State private var pestana = 0
    @State private var usar14Triples = true
    @State private var ficheroCombinacion = ""
    @State private var temporada = 2004.0
    @State private var jornada = 1.0
    @State private var analizarEn14Triples = false
    @State private var combinaciones: [CombinacionJornada] = []
    @State private var seleccion: Set<UUID> = []
    // Recaudación y premios de 14 a 10: mínimo y máximo.
    @State private var criterio: [[Double]] = [[0, 25_000_000], [0, 6_000_000]] + Array(repeating: [0, 3_000_000], count: 4)

    private var temporadaTexto: String {
        let t = FormatoNET.castInt32(temporada)
        return "\(t)/\(t &+ 1)"
    }

    private func seleccionarValoraciones() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        rutaValoraciones = u
        ficheroValoraciones = u.lastPathComponent
        marcadas = []
        guard let t = CasosUtilidades.temporadasValoraciones(u, entorno: sesion.entorno) else {
            rutaValoraciones = nil
            temporadas = []
            return sesion.alertas.error("El fichero no es de valoraciones históricas")
        }
        temporadas = t
        if let a = t.last, let n = Int(String(a.prefix(4))), a.count >= 4 { temporada = Double(n) }
        jornada = 1
    }

    private func anadir() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        guard CasosUtilidades.esFicheroDeColumnas(u, entorno: sesion.entorno) else {
            return sesion.alertas.error("El fichero seleccionado no es un fichero de columnas válido")
        }
        let j = String(FormatoNET.castInt32(jornada))
        combinaciones.append(
            CombinacionJornada(
                temporada: temporadaTexto, jornada: String(repeating: "0", count: max(0, 2 - j.count)) + j,
                ruta: u.path(percentEncoded: false)))
        jornada += 1
    }

    private func guardarLista() async {
        guard let u = await SelectorFicheros.guardar("Lista", [("Lista", ".lst")]) else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(CasosUtilidades.datosListaCombinaciones(combinaciones), en: u, atomico: false)
        } catch {
            sesion.alertas.error("No se pudo guardar la lista: " + error.mensajeUsuario)
        }
    }

    private func leerLista() async {
        guard let u = await SelectorFicheros.abrir(".lst", "*") else { return }
        combinaciones = []
        do throws(ErrorDominio) {
            let r = CasosUtilidades.leerListaCombinaciones(try sesion.entorno.archivos.leer(u))
            combinaciones = r.filas
            if r.error { sesion.alertas.error("No se pudo leer la lista: hay una línea demasiado corta.") }
        } catch {
            sesion.alertas.error("No se pudo leer la lista: " + error.mensajeUsuario)
        }
    }

    private func aceptar() {
        let completo = rutaValoraciones != nil && (pestana == 1 || !marcadas.isEmpty)
        if !completo { sesion.alertas.error("No se ha introducido toda la información necesaria") }
    }

    var body: some View {
        Pagina(destino: .dialogoAnalisisMultipleDeTramos) {
            Text(
                "Selecciona el fichero de valoraciones históricas y define qué combinaciones analizar: una misma combinación sobre varias temporadas, o una combinación distinta por jornada. Añade además el criterio de selección de jornadas por recaudación e importes de premio."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Fichero de valoraciones históricas") {
                HStack {
                    Text(ficheroValoraciones.isEmpty ? "Ningún fichero seleccionado" : ficheroValoraciones)
                    Button("Seleccionar") { Task { await seleccionarValoraciones() } }
                }
            }
            Picker("", selection: $pestana) {
                Text("Combinación única").tag(0)
                Text("Una combinación por jornada").tag(1)
            }
            .pickerStyle(.segmented).labelsHidden().frame(width: 420)
            if pestana == 0 { unica } else { porJornada }
            Tarjeta("Criterio de selección de jornadas") {
                Text(
                    "Las jornadas se filtran por el rango de recaudación y por el importe (mínimo y máximo) de los premios de 14, 13, 12, 11 y 10 aciertos."
                )
                .font(.caption).foregroundStyle(Color.textoSecundario)
                Grid(alignment: .leading) {
                    GridRow {
                        Text("")
                        Text("Mínimo")
                        Text("Máximo")
                    }
                    .font(.caption).foregroundStyle(Color.textoSecundario)
                    ForEach(0..<6, id: \.self) { i in
                        GridRow {
                            Text(i == 0 ? "Recaudación" : "Importe premio de \(15 - i)")
                            CampoNumero(etiqueta: "", valor: $criterio[i][0], minimo: 0, ancho: 120).labelsHidden()
                            CampoNumero(etiqueta: "", valor: $criterio[i][1], minimo: 0, ancho: 120).labelsHidden()
                        }
                    }
                }
            }
            HStack {
                Button("Aceptar", action: aceptar).buttonStyle(EstiloPrincipal())
                Button("Cancelar") {
                    rutaValoraciones = nil
                    ficheroValoraciones = ""
                }
            }
        }
    }

    private var unica: some View {
        Tarjeta("Combinación única") {
            Toggle("Usar el desarrollo de 14 triples", isOn: $usar14Triples).toggleStyle(.switch)
            Text(usar14Triples ? "14 triples" : "Fichero de combinación").font(.caption).foregroundStyle(Color.textoSecundario)
            HStack {
                Text(ficheroCombinacion.isEmpty ? "Fichero de combinación (si no se usan 14 triples)" : ficheroCombinacion)
                Button("Seleccionar") {
                    Task {
                        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
                        ficheroCombinacion = u.lastPathComponent
                        usar14Triples = false
                    }
                }
            }
            Text("Temporadas").bold()
            Text("Marca las temporadas leídas del fichero de valoraciones sobre las que aplicar la combinación.")
                .font(.caption).foregroundStyle(Color.textoSecundario)
            List(temporadas, id: \.self) { t in
                Toggle(t, isOn: Binding(get: { marcadas.contains(t) }, set: { if $0 { marcadas.insert(t) } else { marcadas.remove(t) } }))
            }
            .frame(height: 160)
            HStack {
                Button("Todas") { marcadas = Set(temporadas) }
                Button("Ninguna") { marcadas = [] }
            }
        }
    }

    private var porJornada: some View {
        Tarjeta("Una combinación por jornada") {
            HStack {
                Text("Temporada")
                Button("◄") { temporada -= 1 }
                CampoNumero(etiqueta: "", valor: $temporada, minimo: 1900, maximo: 2100, ancho: 70).labelsHidden()
                Button("►") { temporada += 1 }
                Text(temporadaTexto).foregroundStyle(Color.textoSecundario)
                Text("Jornada")
                Button("◄") { if jornada > 0 { jornada -= 1 } }.disabled(!(jornada > 0))
                CampoNumero(etiqueta: "", valor: $jornada, minimo: 0, maximo: 43, ancho: 50).labelsHidden()
                Button("►") { if jornada < 43 { jornada += 1 } }.disabled(!(jornada < 43))
            }
            HStack {
                Button("Añadir") { Task { await anadir() } }
                Button("Eliminar") {
                    combinaciones.removeAll { seleccion.contains($0.id) }
                    seleccion = []
                }
                Button("Guardar") { Task { await guardarLista() } }
                Button("Leer") { Task { await leerLista() } }
            }
            Toggle("Analizar ficheros dentro del desarrollo de 14 triples", isOn: $analizarEn14Triples)
            Text("Combinaciones por jornada").bold()
            List(combinaciones, selection: $seleccion) { c in
                HStack {
                    Text(c.temporada).frame(width: 90, alignment: .leading)
                    Text(c.jornada).frame(width: 40, alignment: .leading)
                    Text(c.ruta).lineLimit(1)
                }
                .monospacedDigit()
            }
            .frame(height: 180)
        }
    }
}
