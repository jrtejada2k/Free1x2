import Free1X2CasosDeUso
import SwiftUI

/// «Ordenación de columnas por probabilidad» (pantalla 76, `OrdenarPorProbabilidadFrm`).
struct VistaOrdenarPorProbabilidad: View {
    @Environment(Sesion.self) private var sesion
    @State private var deFichero = false
    @State private var entrada: URL?
    @State private var salida: URL?
    @State private var maxColumnas = 4_782_969.0
    @State private var conAcumulada = false
    @State private var limite = 1.0
    @State private var conPremio = false
    @State private var intervalo = false
    @State private var premioMinimo = 0.0
    @State private var premioMaximo = 0.0
    @State private var valoraciones: [[Double]] = []
    @State private var pestana = 0
    @State private var modo = ModoCentral.acertantes
    @State private var textos: [ModoCentral: String] = [.acertantes: "1,5"]
    @State private var aciertos = "14"
    @State private var recaudacion = ""
    @State private var precio = ""
    @State private var porcentaje14 = ""
    @State private var centralSumas = 0.0
    @State private var porProbabilidad = true
    @State private var lnMin = 0.0
    @State private var lnMax = 0.0
    @State private var puntos = 1.0
    @State private var porTramo = 0.0
    @State private var centrales = ValoresCentrales()
    @State private var estado = "Faltan datos"
    @State private var tarea: Task<Void, Never>?
    @State private var cargado = false

    private func actualizar() { estado = salida != nil && (!deFichero || entrada != nil) ? "Preparado" : "Faltan datos" }

    private func texto(_ m: ModoCentral) -> Binding<String> { Binding(get: { textos[m] ?? "" }, set: { textos[m] = $0 }) }

    private func ordenar() async {
        guard let salida, !deFichero || entrada != nil else { return estado = "Faltan datos" }
        let r = CasosUtilidades.parseEspOrdenar(recaudacion) ?? 0
        let pr = CasosUtilidades.parseEspOrdenar(precio) ?? 0
        let pc = CasosUtilidades.parseEspOrdenar(porcentaje14) ?? 0
        var p = ParametrosOrdenacion(entrada: deFichero ? entrada : nil, salida: salida, valoraciones: valoraciones)
        (p.recaudacion, p.precio, p.porcentaje14, p.maxColumnas) = (r, pr, pc, Int(FormatoNET.castInt32(maxColumnas)))
        (p.limiteAcumulada, p.conAcumulada, p.conPremio, p.pestana, p.valorCentralSumas) = (
            limite, conAcumulada, conPremio, pestana, centralSumas
        )
        (p.multiplePorProbabilidad, p.lnMinimo, p.lnMaximo) = (porProbabilidad, lnMin, lnMax)
        p.premio14Entre = intervalo ? (premioMinimo, premioMaximo) : nil
        (p.puntos, p.columnasPorTramo) = (Int(FormatoNET.castInt32(puntos)), Int(FormatoNET.castInt32(porTramo)))
        centrales = CasosUtilidades.valorCentral(
            modo, texto: textos[modo] ?? "", valoraciones: valoraciones, recaudacion: r, precio: pr, porcentaje14: pc, previo: centrales)
        let (c, n) = (centrales, sesion.numeroPartidos)
        let t = Task {
            do throws(ErrorDominio) {
                centrales = try await CasosUtilidades.ordenarPorProbabilidad(p, centrales: c, numeroPartidos: n, entorno: sesion.entorno) {
                    estado = $0
                }
            } catch {
                estado = "Error: " + error.mensajeUsuario
            }
        }
        tarea = t
        await t.value
        tarea = nil
    }

    private func radio(_ m: ModoCentral, _ titulo: String) -> some View {
        HStack {
            Toggle(titulo, isOn: Binding(get: { modo == m }, set: { if $0 { modo = m } })).toggleStyle(.checkbox).frame(
                width: 130, alignment: .leading)
            TextField("", text: texto(m)).textFieldStyle(.roundedBorder).frame(width: 150).disabled(modo != m)
                .onChange(of: textos[.columna] ?? "") { _, v in
                    guard m == .columna else { return }
                    let u = String(v.uppercased().prefix(14))
                    if u != v { textos[.columna] = u }
                }
            if m == .ln && modo == .ln {
                Picker("", selection: $aciertos) { ForEach(["0", "10", "11", "12", "13", "14"], id: \.self) { Text($0).tag($0) } }
                    .labelsHidden().frame(width: 70)
            }
        }
    }

    var body: some View {
        Pagina(destino: .ordenarPorProbabilidad) {
            Text("Ordena o filtra las columnas de la quiniela en torno a un valor central y las escribe a un fichero de salida.")
                .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                VStack(spacing: 10) {
                    Tarjeta("Origen de las columnas a ordenar") {
                        Picker("", selection: $deFichero) {
                            Text("14 Triples").tag(false)
                            Text("Fichero").tag(true)
                        }
                        .pickerStyle(.radioGroup).horizontalRadioGroupLayout().labelsHidden()
                        .onChange(of: deFichero) { actualizar() }
                        HStack {
                            Button("Seleccionar archivo...") {
                                Task {
                                    if let u = await SelectorFicheros.abrir(".txt") { entrada = u }
                                    actualizar()
                                }
                            }
                            .disabled(!deFichero)
                            Text(entrada?.path(percentEncoded: false) ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                        }
                    }
                    Tarjeta("Fichero de salida") {
                        HStack {
                            Button("Seleccionar archivo...") {
                                Task {
                                    if let u = await SelectorFicheros.guardar("ColumnasOrdenadas", [("Columnas", ".txt")]) { salida = u }
                                    actualizar()
                                }
                            }
                            Text(salida?.path(percentEncoded: false) ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                        }
                        CampoNumero(etiqueta: "Nº máximo de columnas", valor: $maxColumnas, minimo: 1, ancho: 100)
                        HStack {
                            Toggle("Añadir probabilidad acumulada", isOn: $conAcumulada)
                            CampoNumero(etiqueta: "Límite de la prob. acumulada", valor: $limite, minimo: 0, ancho: 70)
                        }
                        Toggle("Añadir Premio de 14 aciertos", isOn: $conPremio)
                        // 0.77.2 «Obtener intervalo»: solo se graban las columnas con el premio de 14 en el intervalo.
                        HStack {
                            Toggle("Obtener intervalo: premio de 14 entre", isOn: $intervalo)
                            CampoNumero(etiqueta: "", valor: $premioMinimo, minimo: 0, ancho: 100).disabled(!intervalo)
                            Text("y")
                            CampoNumero(etiqueta: "", valor: $premioMaximo, minimo: 0, ancho: 100).disabled(!intervalo)
                        }
                    }
                    Tarjeta("Criterio de ordenación") {
                        Picker("", selection: $pestana) {
                            Text("Por productos").tag(0)
                            Text("Por Sumas").tag(1)
                            Text("Multiple").tag(2)
                        }
                        .pickerStyle(.segmented).labelsHidden()
                        switch pestana {
                        case 0:
                            Text("Valor central de ordenación").bold()
                            radio(.acertantes, "Nº acertantes")
                            radio(.premio, "Premio")
                            radio(.probabilidad, "Probabilidad")
                            radio(.ln, "Log. neperiano")
                            radio(.columna, "Columna")
                            Text("Configuración L.A.E.").bold()
                            CampoTexto(etiqueta: "Recaudación", texto: $recaudacion, ancho: 120)
                            CampoTexto(etiqueta: "Precio de una apuesta", texto: $precio, ancho: 80)
                            CampoTexto(etiqueta: "% para los 14 aciertos", texto: $porcentaje14, ancho: 80)
                        case 1:
                            CampoNumero(etiqueta: "Valor central de ordenación", valor: $centralSumas, ancho: 100)
                        default:
                            Text("Ordenación por múltiples puntos").bold()
                            Picker("Tipo de valoración", selection: $porProbabilidad) {
                                Text("Por probabilidad (LN)").tag(true)
                                Text("Por sumas").tag(false)
                            }
                            .pickerStyle(.radioGroup)
                            CampoNumero(etiqueta: "LN mínimo", valor: $lnMin, ancho: 90)
                            CampoNumero(etiqueta: "LN máximo", valor: $lnMax, ancho: 90)
                            CampoNumero(etiqueta: "Nº puntos centrales", valor: $puntos, minimo: 1, ancho: 70)
                            CampoNumero(etiqueta: "Nº columnas por punto", valor: $porTramo, minimo: 0, ancho: 90)
                        }
                    }
                }
                Tarjeta("Valoraciones (porcentajes)") {
                    Text("Porcentajes por partido (1/X/2) que alimentan el cálculo de probabilidad / suma.").font(.callout)
                        .foregroundStyle(Color.textoSecundario)
                    PorcentajesGrid(valores: $valoraciones)
                }
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Cancelar") {
                    tarea?.cancel()
                    estado = "Faltan datos"
                    sesion.router.atras()
                }
                Button("Ordenar") { Task { await ordenar() } }.buttonStyle(EstiloPrincipal()).disabled(tarea != nil)
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            valoraciones = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos)
            let (l, c) = (sesion.configuracion.lae, sesion.entorno.cultura)
            (recaudacion, precio, porcentaje14) = (
                FormatoNET.toString(l.recaudacion, cultura: c), FormatoNET.toString(l.precioApuesta, cultura: c),
                FormatoNET.toString(l.porcentaje14, cultura: c)
            )
        }
    }
}
