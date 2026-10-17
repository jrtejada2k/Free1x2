import AppKit
import Free1X2CasosDeUso
import SwiftUI

/// «Estimación de premios» (pantalla 46, `EstimadorPremiosFrm`).
struct VistaEstimadorPremios: View {
    @Environment(Sesion.self) private var sesion
    @State private var temporada = ""
    @State private var jornada = 0.0
    @State private var recaudacion = 13_825_260.5
    @State private var bote = 0.0
    @State private var precio = 0.5
    @State private var destinadoPremios = 55.0
    @State private var reparto: [Double] = [12, 8, 8, 8, 9]
    @State private var porcentajes: [[Double]] = Array(repeating: [0, 0, 0], count: 14)
    @State private var columna = "11111111111111"
    @State private var numApuestas = 28_000_000.0
    @State private var prevision = PrevisionPremios.vacia
    @State private var numApuestasTexto = ""
    @State private var real: [[Double]] = Array(repeating: [0, 0], count: 5)
    @State private var cargado = false

    private static let categorias = ["14", "13", "12", "11", "10"]

    private func recargar() {
        do throws(ErrorDominio) {
            guard
                let r = try CasosUtilidades.datosJornadaLAE(
                    temporada: temporada, jornada: jornada, recaudacion: recaudacion, bote: bote, precio: precio, porcentajes: reparto,
                    entorno: sesion.entorno)
            else { return }
            (columna, recaudacion) = (r.columna, r.recaudacion)
            real = (0..<5).map { [r.acertantes[$0], r.premios[$0]] }
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    var body: some View {
        Pagina(destino: .estimadorPremios) {
            Text(
                "Estima los acertantes y premios por categoría a partir de la recaudación, el bote y el reparto de porcentajes de la jornada."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                VStack(spacing: 10) {
                    Tarjeta("Datos económicos") {
                        CampoTexto(etiqueta: "Temporada", texto: $temporada, ancho: 100)
                        HStack {
                            Button("◀") {
                                if jornada > 1 { jornada -= 1 }
                                recargar()
                            }
                            .accessibilityLabel("Jornada anterior")
                            CampoNumero(etiqueta: "Jornada", valor: $jornada, minimo: 0, ancho: 60)
                            Button("▶") {
                                if jornada < 44 { jornada += 1 }
                                recargar()
                            }
                            .accessibilityLabel("Jornada siguiente")
                        }
                        CampoNumero(etiqueta: "Recaudación", valor: $recaudacion, minimo: 0, ancho: 140)
                        CampoNumero(etiqueta: "Bote", valor: $bote, minimo: 0, ancho: 140)
                        CampoNumero(etiqueta: "Precio apuesta", valor: $precio, minimo: 0, ancho: 80)
                    }
                    Tarjeta("Reparto de premios (%)") {
                        CampoNumero(etiqueta: "Destinado a premios", valor: $destinadoPremios, minimo: 0, maximo: 100, ancho: 60)
                        ForEach(0..<5, id: \.self) { i in
                            CampoNumero(
                                etiqueta: "Para \(Self.categorias[i]) aciertos", valor: $reparto[i], minimo: 0, maximo: 100, ancho: 60)
                        }
                    }
                }
                Tarjeta("Porcentajes y columna premiada") {
                    Text(
                        "Porcentajes por partido (1/X/2) de la jornada y columna premiada (14 signos). Alimentan la previsión de acertantes y premios."
                    )
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                    PorcentajesGrid(valores: $porcentajes)
                    HStack {
                        Text("Columna premiada")
                        TextField("14 signos (1 X 2)", text: $columna).textFieldStyle(.roundedBorder).monospaced().frame(width: 170)
                            .onChange(of: columna) { _, v in
                                let u = String(v.uppercased().prefix(14))
                                if u != v { columna = u }
                            }
                    }
                    CampoNumero(etiqueta: "Nº apuestas", valor: $numApuestas, minimo: 0, ancho: 120)
                    Button("Calcular") {
                        guard
                            let p = CasosUtilidades.estimarPremios(
                                valores: porcentajes, columna: columna, numApuestas: numApuestas, recaudacion: recaudacion, bote: bote,
                                porcentajes: reparto, cultura: sesion.entorno.cultura)
                        else { return numApuestasTexto = "Columna premiada inválida (14 signos)" }
                        (prevision, numApuestasTexto) = (p, p.numApuestas)
                    }
                    .buttonStyle(EstiloPrincipal())
                }
            }
            HStack(alignment: .top) {
                Tarjeta("Previsión de premios") {
                    LabeledContent("Nº columnas", value: numApuestasTexto)
                    Grid(alignment: .trailing, horizontalSpacing: 20, verticalSpacing: 3) {
                        GridRow {
                            Text("Categoría").bold()
                            Text("Acertantes").bold()
                            Text("Premio").bold()
                        }
                        ForEach(0..<5, id: \.self) { i in
                            GridRow {
                                Text(Self.categorias[i])
                                Text(prevision.acertantes[i]).monospacedDigit()
                                Text(prevision.premios[i]).monospacedDigit()
                            }
                        }
                    }
                    Button("Copiar previsión al escrutinio real") {
                        let c = sesion.entorno.cultura
                        real = (0..<5).map {
                            [
                                FormatoNET.tryParseDouble(prevision.acertantes[$0], cultura: c) ?? 0,
                                FormatoNET.tryParseDouble(prevision.premios[$0], cultura: c) ?? 0,
                            ]
                        }
                    }
                }
                Tarjeta("Escrutinio real") {
                    Grid(alignment: .trailing, horizontalSpacing: 12, verticalSpacing: 3) {
                        GridRow {
                            Text("Categoría").bold()
                            Text("Acertantes").bold()
                            Text("Premio").bold()
                        }
                        ForEach(0..<5, id: \.self) { i in
                            GridRow {
                                Text(Self.categorias[i])
                                CampoNumero(etiqueta: "", valor: $real[i][0], minimo: 0, ancho: 100).labelsHidden()
                                CampoNumero(etiqueta: "", valor: $real[i][1], minimo: 0, ancho: 110).labelsHidden()
                            }
                        }
                    }
                    HStack {
                        Button("Guardar escrutinio") {
                            guard columna.utf16.count >= 14 else { return numApuestasTexto = "Columna premiada inválida (14 signos)" }
                            do throws(ErrorDominio) {
                                try CasosUtilidades.guardarEscrutinioLAE(
                                    temporada: temporada, jornada: jornada, columna: columna, recaudacion: recaudacion,
                                    acertantes: real.map { $0[0] }, lae: sesion.configuracion.lae, entorno: sesion.entorno)
                            } catch {
                                sesion.alertas.error("No se ha podido grabar el escrutinio: " + error.mensajeUsuario)
                            }
                        }
                        Button("Resultados escrutinio oficial") {
                            if let u = URL(string: "https://www.loteriasyapuestas.es/es/la-quiniela") { NSWorkspace.shared.open(u) }
                        }
                    }
                }
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            temporada = CasosUtilidades.temporadaActual(sesion.entorno.reloj.ahora())
            // DT-92 (dueño): el precio de partida es el de Configuración ▸ Parámetros LAE, no 0,50 fijo.
            precio = sesion.configuracion.lae.precioApuesta
        }
    }
}

/// «Rentabilidad» (pantalla 83, `RentabilidadFrm`).
struct VistaRentabilidad: View {
    @Environment(Sesion.self) private var sesion
    @State private var deFichero = false
    @State private var entrada = ""
    @State private var salida = ""
    @State private var recaudacion = 8_000_000.0
    @State private var emMin = 0.133
    @State private var emMax = 50.0
    @State private var ordenar = false
    @State private var conEM = false
    @State private var apostados: [[Double]] = []
    @State private var reales: [[Double]] = []
    @State private var pestana = 0
    @State private var columna = ""
    @State private var valoracion = ("-", "-", "-")
    @State private var estado = "Listo"
    @State private var calculando = false

    private func calcular() async {
        guard !salida.isEmpty, !deFichero || !entrada.isEmpty else { return estado = "Faltan datos" }
        calculando = true
        do throws(ErrorDominio) {
            let n = try await CasosUtilidades.calcularRentabilidad(
                entrada: deFichero ? URL(filePath: entrada) : nil, salida: URL(filePath: salida), apostados: apostados, reales: reales,
                recaudacion: recaudacion, lae: sesion.configuracion.lae, emMin: emMin, emMax: emMax, ordenar: ordenar, conEM: conEM,
                numeroPartidos: sesion.numeroPartidos, entorno: sesion.entorno
            ) { estado = $0 }
            estado = "Finalizado (\(n) columnas)"
        } catch {
            estado = "Error: " + error.mensajeUsuario
        }
        calculando = false
    }

    var body: some View {
        Pagina(destino: .rentabilidad) {
            Text(
                "Obtiene la Esperanza Matemática de premio (EM) de las columnas a partir de la probabilidad real y las frecuencias apostadas."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                VStack(spacing: 10) {
                    Tarjeta("Ficheros") {
                        Picker("Origen de las columnas", selection: $deFichero) {
                            Text("14 Triples").tag(false)
                            Text("Fichero").tag(true)
                        }
                        .pickerStyle(.radioGroup).horizontalRadioGroupLayout()
                        HStack {
                            TextField("Fichero de columnas de entrada", text: $entrada).textFieldStyle(.roundedBorder)
                            Button("…") {
                                Task { if let u = await SelectorFicheros.abrir(".txt") { entrada = u.path(percentEncoded: false) } }
                            }
                        }
                        .disabled(!deFichero)
                        Text("Salida de resultados").bold()
                        HStack {
                            TextField("Fichero de salida", text: $salida).textFieldStyle(.roundedBorder)
                            Button("…") {
                                Task {
                                    if let u = await SelectorFicheros.guardar("Rentabilidad", [("Columnas", ".txt")]) {
                                        salida = u.path(percentEncoded: false)
                                    }
                                }
                            }
                        }
                    }
                    Tarjeta("Límites Esperanza Matemática") {
                        CampoNumero(etiqueta: "Recaudación considerada", valor: $recaudacion, minimo: 0, ancho: 120)
                        CampoNumero(etiqueta: "Mínimo", valor: $emMin, ancho: 80)
                        CampoNumero(etiqueta: "Máximo", valor: $emMax, ancho: 80)
                    }
                    Tarjeta("Opciones de salida") {
                        Toggle("Ordenar columnas por EM", isOn: $ordenar)
                        Toggle("Añadir EM al fichero de salida", isOn: $conEM)
                        HStack {
                            Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).disabled(calculando)
                            Text(estado).foregroundStyle(Color.textoSecundario)
                        }
                    }
                    Tarjeta("Calcular Valoración de una columna") {
                        HStack {
                            Text("Columna")
                            TextField("14 signos (1 X 2)", text: $columna).textFieldStyle(.roundedBorder).monospaced().frame(width: 170)
                                .onChange(of: columna) { _, v in
                                    let u = String(v.uppercased().prefix(14))
                                    if u != v { columna = u }
                                }
                            Button("Calcular") {
                                do throws(ErrorDominio) {
                                    valoracion = try CasosUtilidades.valorarColumnaRentabilidad(
                                        columna, apostados: apostados, reales: reales, recaudacion: recaudacion,
                                        lae: sesion.configuracion.lae, cultura: sesion.entorno.cultura)
                                    estado = "Valoración calculada"
                                } catch {
                                    sesion.alertas.inesperado(error)
                                }
                            }
                            .disabled(columna.utf16.count != 14)
                        }
                        LabeledContent("Premio estimado de 14", value: valoracion.0)
                        LabeledContent("Probabilidad real", value: valoracion.1)
                        LabeledContent("Esperanza matemática", value: valoracion.2)
                    }
                }
                Tarjeta("Valoraciones (porcentajes)") {
                    Text("Porcentajes apostados y reales por partido (1/X/2). Alimentan el cálculo de la Esperanza Matemática.")
                        .font(.callout).foregroundStyle(Color.textoSecundario)
                    Picker("", selection: $pestana) {
                        Text("Apostados").tag(0)
                        Text("Reales").tag(1)
                    }
                    .pickerStyle(.segmented).labelsHidden()
                    if pestana == 0 { PorcentajesGrid(valores: $apostados) } else { PorcentajesGrid(valores: $reales) }
                }
            }
        }
        .onAppear {
            if apostados.isEmpty {
                apostados = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos)
                reales = apostados
            }
        }
    }
}
