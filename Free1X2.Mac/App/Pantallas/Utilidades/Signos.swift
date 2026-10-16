import Free1X2CasosDeUso
import SwiftUI

/// «Análisis de signos» (pantalla 97, `VSignosFrm`).
struct VistaVSignos: View {
    @Environment(Sesion.self) private var sesion
    @State private var fichero: URL?
    @State private var formato = "% enteros"
    @State private var pleno = true
    @State private var aspiracion = 14.0
    @State private var partidos = 15
    @State private var ganadora = Array(repeating: "-", count: 15)
    @State private var resultado: ResultadoVSignos?
    @State private var procesadas = "Procesadas: -"
    @State private var tiempo = "Tiempo: -"
    @State private var premios = "Premios remanentes: -"

    private static let formatos = ["% enteros", "% con decimales", "columnas"]

    private func abrir() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        fichero = u
        do throws(ErrorDominio) {
            partidos = try CasosUtilidades.partidosVSignos(u, entorno: sesion.entorno)
        } catch {
            return sesion.alertas.error("No se ha podido leer el fichero de columnas: " + error.mensajeUsuario)
        }
        ganadora = Array(repeating: "-", count: partidos)
        await calcular()
    }

    private func calcular() async {
        guard let fichero else { return sesion.alertas.error("Seleccione primero un fichero de columnas.") }
        let inicio = Date()
        do throws(ErrorDominio) {
            let r = try await CasosUtilidades.analizarSignos(
                fichero, ganadora: ganadora, aspiracion: Int(FormatoNET.castInt32(aspiracion)), pleno: pleno, entorno: sesion.entorno)
            resultado = r
            procesadas = "Procesadas: \(r.procesadas)"
            tiempo = "Tiempo: " + CasosUtilidades.tiempoCorto(Date().timeIntervalSince(inicio))
            premios = CasosUtilidades.textoPremiosRemanentes(r.premios)
        } catch {
            resultado?.aceptadas = []
            sesion.alertas.error("Error al procesar el fichero: " + error.mensajeUsuario)
        }
    }

    private func grabar() async {
        guard let r = resultado else { return sesion.alertas.error("Calcule primero el análisis antes de grabar.") }
        let columnas = formato == "columnas"
        guard
            let u = await SelectorFicheros.guardar(
                columnas ? "Columnas" : "Valoracion", [(columnas ? "Columnas" : "Valoración", ".txt")])
        else { return }
        let lineas =
            columnas ? r.aceptadas : CasosUtilidades.valoracionDeSignos(r.recuentos, decimales: formato == "% con decimales")
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(TextoNET.codificarLineas(lineas), en: u, atomico: false)
        } catch {
            sesion.alertas.error("Error al grabar el resultado: " + error.mensajeUsuario)
        }
    }

    private func celda(_ i: Int, _ s: Int) -> some View {
        Button(String(resultado?.recuentos[i][s] ?? 0)) { ganadora[i] = ["1", "X", "2"][s] }
            .buttonStyle(.borderless).monospacedDigit().frame(width: 70, alignment: .trailing)
    }

    var body: some View {
        Pagina(destino: .vSignos) {
            Tarjeta("Fichero") {
                HStack {
                    Text(fichero?.lastPathComponent ?? "Fichero a procesar").foregroundStyle(Color.textoSecundario)
                    Spacer()
                    Button("Examinar…") { Task { await abrir() } }
                }
                HStack(spacing: 16) {
                    Picker("Formato", selection: $formato) { ForEach(Self.formatos, id: \.self) { Text($0).tag($0) } }
                        .frame(width: 240)
                    Toggle("Último partido como Pleno", isOn: $pleno).toggleStyle(.switch)
                    CampoNumero(etiqueta: "Aspirando a…", valor: $aspiracion, minimo: 0, ancho: 60)
                    Button("−") { if aspiracion > 0 { aspiracion -= 1 } }
                    Button("+") { if aspiracion < Double(partidos) { aspiracion += 1 } }
                }
            }
            Tarjeta("Columna ganadora") {
                HStack(spacing: 4) {
                    ForEach(ganadora.indices, id: \.self) { i in
                        VStack(spacing: 2) {
                            Text(String(i + 1)).font(.caption).foregroundStyle(Color.textoSecundario)
                            Picker("", selection: $ganadora[i]) {
                                ForEach(["-", "1", "X", "2"], id: \.self) { Text($0).tag($0) }
                            }
                            .labelsHidden().frame(width: 48)
                        }
                    }
                }
                HStack {
                    Spacer()
                    Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal())
                    Button("Grabar") { Task { await grabar() } }
                    Button("Limpiar") { ganadora = ganadora.map { _ in "-" } }
                }
            }
            Tarjeta("Recuento por partido") {
                Grid(alignment: .trailing, horizontalSpacing: 12, verticalSpacing: 2) {
                    GridRow {
                        Text("Part.").bold()
                        Text("1").bold()
                        Text("X").bold()
                        Text("2").bold()
                    }
                    ForEach(0..<(resultado?.recuentos.count ?? 0), id: \.self) { i in
                        GridRow {
                            Text(String(i + 1)).monospacedDigit()
                            ForEach(0..<3, id: \.self) { s in celda(i, s) }
                        }
                    }
                }
                Text("Pulse un recuento para fijar ese signo en la columna ganadora.").font(.callout)
                    .foregroundStyle(Color.textoSecundario)
            }
            HStack(spacing: 24) {
                Text(procesadas)
                Text(tiempo)
                Text(premios)
            }
            .foregroundStyle(Color.textoSecundario)
        }
    }
}

/// «Diferencias entre columnas» (pantalla 41, `DifCols`).
struct VistaDifCols: View {
    @Environment(Sesion.self) private var sesion
    @State private var triples = true
    @State private var columnaBase = true
    @State private var base = "1X1212X2111121"
    @State private var difBase = "0-5,6,7-14"
    @State private var difResultado = "6-14"
    @State private var limite = 0.0
    @State private var ficheroEntrada = ""
    @State private var ficheroCondiciones = ""
    @State private var aceptadas: [String] = []
    @State private var admitidas = "0"
    @State private var tiempo = "—"
    @State private var puedeGrabar = false
    @State private var tarea: Task<Void, Never>?

    private func calcular() async {
        let e = sesion.entorno
        var condiciones: [UInt64] = []
        do throws(ErrorDominio) {
            if columnaBase {
                condiciones = [try CasosUtilidades.condicionDif(base)]
            } else {
                guard let u = await SelectorFicheros.abrir(".txt") else { return }
                ficheroCondiciones = u.lastPathComponent
                condiciones = try CasosUtilidades.condicionesDif(u, entorno: e)
            }
        } catch {
            return sesion.alertas.error(error.mensajeUsuario)
        }
        var entrada: URL?
        if !triples {
            guard let u = await SelectorFicheros.abrir(".txt") else { return }
            entrada = u
            ficheroEntrada = u.lastPathComponent
        }
        let mascaras: (base: [Bool], resultado: [Bool])
        do throws(ErrorDominio) {
            mascaras = (try CasosUtilidades.mascaraDif(difBase), try CasosUtilidades.mascaraDif(difResultado))
        } catch {
            return sesion.alertas.error("Rango de diferencias inválido: " + error.mensajeUsuario)
        }
        admitidas = "calculando..."
        puedeGrabar = false
        tiempo = " "
        let inicio = Date()
        do throws(ErrorDominio) {
            if let r = try await CasosUtilidades.diferenciasColumnas(
                condiciones: condiciones, difBase: mascaras.base, difResultado: mascaras.resultado, entrada: entrada, entorno: e)
            {
                aceptadas = r
            }
            admitidas = String(aceptadas.count)
            puedeGrabar = !aceptadas.isEmpty
        } catch {
            sesion.alertas.error("Error en el cálculo: " + error.mensajeUsuario)
        }
        tiempo = CasosUtilidades.tiempoCorto(Date().timeIntervalSince(inicio))
    }

    private func grabar() async {
        guard !aceptadas.isEmpty, let u = await SelectorFicheros.guardar("Diferencias", [("F.Salida", ".txt")]) else { return }
        do throws(ErrorDominio) {
            // `(int)LimiteColumnas` solo si > 0.
            let l = limite > 0 ? Int(FormatoNET.castInt32(limite)) : 0
            try CasosUtilidades.grabarDiferencias(aceptadas, limite: l, en: u, entorno: sesion.entorno)
            sesion.alertas.info("Columnas grabadas.")
        } catch {
            sesion.alertas.error("Error al grabar: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .difCols) {
            Tarjeta("Generación por") {
                Picker("", selection: $triples) {
                    Text("14 triples").tag(true)
                    Text("Fichero").tag(false)
                }
                .pickerStyle(.radioGroup).horizontalRadioGroupLayout().labelsHidden()
                if !triples && !ficheroEntrada.isEmpty { Text(ficheroEntrada).foregroundStyle(Color.textoSecundario) }
            }
            Tarjeta("Condicionada por") {
                Picker("", selection: $columnaBase) {
                    Text("Columna Base").tag(true)
                    Text("Fichero").tag(false)
                }
                .pickerStyle(.radioGroup).horizontalRadioGroupLayout().labelsHidden()
                if columnaBase {
                    CampoTexto(etiqueta: "Columna base", texto: $base, ancho: 160)
                        .onChange(of: base) { base = String(FormatoNET.mayusculas(base).prefix(14)) }
                } else if !ficheroCondiciones.isEmpty {
                    Text(ficheroCondiciones).foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Diferencias") {
                CampoTexto(etiqueta: "s/ col. base", texto: $difBase, ancho: 180)
                    .onChange(of: difBase) { difBase = String(difBase.prefix(20)) }
                CampoTexto(etiqueta: "s/ col. result.", texto: $difResultado, ancho: 180)
                    .onChange(of: difResultado) { difResultado = String(difResultado.prefix(20)) }
                Text("Formato «min-max» separado por comas. En la columna, F = partido fijo (no cuenta).").font(.callout)
                    .foregroundStyle(Color.textoSecundario)
            }
            Tarjeta("Proceso") {
                CampoNumero(etiqueta: "Límite de columnas (0 = sin límite)", valor: $limite, minimo: 0)
                HStack(spacing: 24) {
                    Text("Admitidas: " + admitidas)
                    Text("Tiempo: " + tiempo)
                }
                .foregroundStyle(Color.textoSecundario)
                HStack {
                    // «CPs por diferencias…» no hace nada en el C# (`VM:252-256`).
                    Button("CPs por diferencias…") {}
                    Spacer()
                    Button("Cancelar") { tarea?.cancel() }.disabled(tarea == nil)
                    Button("Grabar") { Task { await grabar() } }.disabled(tarea != nil || !puedeGrabar)
                    Button("Calcular") {
                        tarea = Task {
                            await calcular()
                            tarea = nil
                        }
                    }
                    .buttonStyle(EstiloPrincipal()).disabled(tarea != nil)
                }
            }
        }
    }
}

/// «Subir categoría» (pantalla 92, `SubirCategoriaFrm`).
struct VistaSubirCategoria: View {
    @Environment(Sesion.self) private var sesion
    @State private var origen: URL?
    @State private var salida: URL?
    @State private var externas: URL?
    @State private var motor: SubidaCategoria?
    @State private var signos = 14
    struct Casilla: Equatable {
        var habilitada = false
        var marcada = false
    }

    /// 16 partidos y 17 niveles (0…16).
    @State private var partidos = Array(repeating: Casilla(), count: 16)
    @State private var niveles = Array(repeating: Casilla(), count: 17)
    @State private var seguidos = 0.0
    @State private var conteo = ""
    @State private var estado = "Selecciona un archivo de origen"
    @State private var puedeCalcular = false
    @State private var puedeGrabar = false

    /// `AdaptarNiveles`: borra las marcas; nivel 0 y uno más por partido marcado (se replica, DEC-1013).
    private func adaptarNiveles() {
        niveles = niveles.map { _ in Casilla() }
        for k in 0...partidos.filter(\.marcada).count where k < niveles.count { niveles[k].habilitada = true }
    }

    private func seleccionarOrigen() async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        origen = u
        conteo = "leyendo..."
        estado = "Leyendo..."
        do throws(ErrorDominio) {
            let r = try await CasosUtilidades.prepararSubirCategoria(u, entorno: sesion.entorno)
            (motor, signos) = (r.motor, r.signos)
            conteo = "I=\(r.motor.noColumnas)"
            partidos = (0..<16).map { Casilla(habilitada: $0 < r.signos, marcada: $0 < r.signos) }
            adaptarNiveles()
            (puedeCalcular, puedeGrabar) = (false, false)
            estado = "Archivo de origen cargado"
        } catch {
            sesion.alertas.error("Error al leer el archivo: " + error.mensajeUsuario)
            estado = "Error al leer el archivo"
        }
    }

    private func calcular() async {
        guard let m = motor else { return estado = "Selecciona un archivo de origen" }
        guard niveles.contains(where: \.marcada) else { return estado = "No se ha seleccionado ningún nivel" }
        if seguidos > Double(signos) { seguidos = Double(signos) }
        (puedeCalcular, puedeGrabar) = (false, false)
        estado = "Calculando..."
        let r = await CasosUtilidades.calcularSubirCategoria(
            m, involucrados: partidos.map(\.marcada), niveles: niveles.map(\.marcada), seguidos: Int(FormatoNET.castInt32(seguidos)),
            externas: externas, signos: signos, entorno: sesion.entorno)
        motor = r.motor
        if let e = r.error {
            estado = "Error: " + e.mensajeUsuario
        } else {
            conteo = "F=\(r.motor.noColumnas)"
            puedeGrabar = true
            estado = "Calculado"
        }
        puedeCalcular = true
    }

    private func grabar() async {
        guard let m = motor, let salida else { return estado = "Falta el archivo de salida" }
        (puedeCalcular, puedeGrabar) = (false, false)
        conteo = "grabando..."
        estado = "Grabando..."
        do throws(ErrorDominio) {
            try await CasosUtilidades.grabarSubirCategoria(m, en: salida, entorno: sesion.entorno)
            conteo = "G=\(m.noColumnas)"
            estado = "Grabado"
        } catch {
            estado = "Error: " + error.mensajeUsuario
        }
        (puedeCalcular, puedeGrabar) = (true, true)
    }

    private func casilla(_ texto: String, _ c: Binding<Casilla>) -> some View {
        Toggle(texto, isOn: c.marcada).disabled(!c.wrappedValue.habilitada)
    }

    var body: some View {
        Pagina(destino: .subirCategoria) {
            Tarjeta("Ficheros") {
                HStack {
                    Button("Origen (txt)") { Task { await seleccionarOrigen() } }
                    Text(origen?.lastPathComponent ?? "").foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    Button("Salida (txt)") {
                        Task {
                            guard let u = await SelectorFicheros.guardar("columnas", [("Columnas", ".txt")]) else { return }
                            salida = u
                            puedeCalcular = motor != nil
                            estado = "Listo para calcular"
                        }
                    }
                    .disabled(motor == nil)
                    Text(salida?.lastPathComponent ?? "").foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    Button("Usar combinación") { Task { if let u = await SelectorFicheros.abrir(".txt") { externas = u } } }
                    Text(externas?.lastPathComponent ?? "").foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Partidos involucrados") {
                Text("Desmarcando alguno, se arriesga la garantía.").font(.callout).foregroundStyle(Color.textoSecundario)
                LazyVGrid(columns: Array(repeating: GridItem(.fixed(56)), count: 8), alignment: .leading) {
                    ForEach(0..<16, id: \.self) { i in casilla(String(i + 1), $partidos[i]) }
                }
                .onChange(of: partidos.map(\.marcada)) { adaptarNiveles() }
                CampoNumero(etiqueta: "Seguidos:", valor: $seguidos, minimo: 0, maximo: 16, ancho: 60)
            }
            Tarjeta("Niveles") {
                LazyVGrid(columns: Array(repeating: GridItem(.fixed(56)), count: 9), alignment: .leading) {
                    ForEach((0...16).reversed(), id: \.self) { k in casilla(String(k), $niveles[k]) }
                }
            }
            HStack {
                Text(conteo).monospacedDigit()
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).disabled(!puedeCalcular)
                Button("Grabar") { Task { await grabar() } }.disabled(!puedeGrabar)
            }
        }
    }
}
