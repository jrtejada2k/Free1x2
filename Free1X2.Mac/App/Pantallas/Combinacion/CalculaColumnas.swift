import Free1X2CasosDeUso
import SwiftUI

/// «Calcular columnas» (pantalla 13, `CalculaColumnasFrm`): contar, analizar o grabar con contadores en vivo.
struct VistaCalculaColumnas: View {
    @Environment(Sesion.self) private var sesion

    enum Modo: String, CaseIterable {
        case calcular = "Calcular"
        case analizar = "Calcular y analizar"
        case grabar = "Grabar archivo"
    }

    @State private var modo = Modo.calcular
    @State private var incluirPleno = false
    @State private var archivo: URL?
    /// Combinación compartida desde Inicio o una propia con todo a «1,X,2» (`UsarAnalizadorCompartido`).
    @State private var combinacion: Combinacion?
    @State private var maximas: Int64 = 0
    @State private var textos = TextosCalculo()
    @State private var calculando = false
    @State private var progresoVisible = false
    @State private var estado = "Preparado"
    @State private var horaComienzo = "00:00:00"
    @State private var horaFinal = "00:00:00"
    @State private var transcurrido = "0.0"
    @State private var tarea: Task<Void, Never>?

    private var cultura: Cultura { .sistema() }

    private func cargar() {
        guard combinacion == nil else { return }
        if sesion.usarCombinacionCompartida {
            combinacion = sesion.combinacion
        } else {
            var c = Combinacion(numeroPartidos: sesion.numeroPartidos)
            c.ponerPronosticos(Array(repeating: "1,X,2", count: sesion.numeroPartidos))
            combinacion = c
        }
        sesion.usarCombinacionCompartida = false
        recalcularMaximas()
    }

    /// `RecalcularMaximas`: solo cambian «Máximo» y su coste.
    private func recalcularMaximas() {
        guard let c = combinacion else { return }
        maximas = CasosCombinacion.estimarColumnasMaximas(c, entorno: sesion.entorno)
        let t = CasosCombinacion.estadisticasDeCalculo(
            procesadas: 0, aceptadas: 0, maximas: maximas, lae: sesion.configuracion.lae, cultura: cultura)
        (textos.maximo, textos.costeMaximo) = (t.maximo, t.costeMaximo)
    }

    /// `ActualizaDatosCalculo`.
    private func actualizar(_ procesadas: Int, _ aceptadas: Int) {
        let t = CasosCombinacion.estadisticasDeCalculo(
            procesadas: procesadas, aceptadas: aceptadas, maximas: maximas, lae: sesion.configuracion.lae, cultura: cultura)
        textos = t
    }

    private func calcular() {
        guard !calculando, let c = combinacion else { return }
        recalcularMaximas()
        let m: ModoCalculo
        switch modo {
        case .calcular: m = .contar
        case .analizar: m = .analizar
        case .grabar:
            do throws(ErrorDominio) {
                try CasosCombinacion.validarArchivoResultados(archivo, combinacion: c)
            } catch {
                return sesion.alertas.error(error.mensajeUsuario)
            }
            m = .grabar(archivo ?? URL(filePath: ""))
        }
        calculando = true
        progresoVisible = true
        estado = "Calculando..."
        let entorno = sesion.entorno
        var cfg = sesion.configuracion
        cfg.puntosCP = CasosAjustes.leerPuntosCP(entorno: entorno)  // DP-04: los puntos CP se releen en cada cálculo
        let inicio = entorno.reloj.ahora()
        horaComienzo = FormatoNET.horaLarga(inicio, cultura: cultura)
        tarea = Task {
            let (flujo, canal) = AsyncStream.makeStream(of: Progreso.self, bufferingPolicy: .bufferingNewest(1))
            let reportador = ReportadorProgreso(reloj: entorno.reloj) { _ = canal.yield($0) }
            let escucha = Task {
                for await p in flujo { actualizar(p.procesadas, p.aceptadas ?? 0) }
            }
            var resultado: ResultadoCalculoCombinacion?
            do throws(ErrorDominio) {
                resultado = try await CasosCombinacion.calcularCombinacion(
                    c, modo: m, configuracion: cfg, entorno: entorno, progreso: reportador)
            } catch {
                sesion.alertas.error("Error en el cálculo: " + error.mensajeUsuario)
            }
            canal.finish()
            await escucha.value
            if let a = resultado?.analisis, let base = c.grupos.first {
                sesion.visorAnalisis = (a, base)
                sesion.router.navegarSiempre(.visorAnalisisColumnas)
            }
            let fin = entorno.reloj.ahora()
            horaFinal = FormatoNET.horaLarga(fin, cultura: cultura)
            transcurrido = FormatoNET.toStringTimeSpan(fin.timeIntervalSince(inicio))
            if let r = resultado?.calculo { actualizar(r.analizadas, r.aceptadas) }
            calculando = false
            progresoVisible = false
            estado = "Listo"
        }
    }

    var body: some View {
        Pagina(destino: .calculaColumnas) {
            Text(
                "Genera y cuenta las columnas de la combinación actual, opcionalmente analizándolas o grabándolas a un archivo."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Modo de cálculo") {
                Picker("", selection: $modo) {
                    ForEach(Modo.allCases, id: \.self) { Text($0.rawValue).tag($0) }
                }
                .pickerStyle(.radioGroup).labelsHidden()
                Toggle("Incluir pleno al 15", isOn: $incluirPleno).disabled(modo != .analizar)
                HStack {
                    Button("Seleccionar archivo...") {
                        Task {
                            if let url = await SelectorFicheros.guardar("Columnas", [("Columnas", ".txt")]) { archivo = url }
                        }
                    }
                    .disabled(modo != .grabar)
                    Text(archivo?.lastPathComponent ?? "(Seleccionar)").foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Resultados") {
                Grid(alignment: .trailing, horizontalSpacing: 24, verticalSpacing: 6) {
                    GridRow {
                        Text("")
                        Text("Columnas").bold()
                        Text("Coste").bold()
                    }
                    fila("Procesadas", textos.procesadas, textos.costeProcesadas)
                    fila("Aceptadas", textos.aceptadas, textos.costeAceptadas)
                    fila("% aceptación", textos.porcentaje, "")
                    fila("Estimadas", textos.estimadas, textos.costeEstimadas)
                    fila("Máximo", textos.maximo, textos.costeMaximo)
                }
                if progresoVisible { ProgressView(value: textos.progreso, total: 100) }
            }
            Tarjeta("Tiempos") {
                Grid(alignment: .leading, horizontalSpacing: 24, verticalSpacing: 6) {
                    GridRow {
                        Text("Comienzo")
                        Text(horaComienzo).monospacedDigit()
                    }
                    GridRow {
                        Text("Final")
                        Text(horaFinal).monospacedDigit()
                    }
                    GridRow {
                        Text("Transcurrido")
                        Text(transcurrido).monospacedDigit()
                    }
                }
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Cancelar") {
                    tarea?.cancel()
                    progresoVisible = false
                    estado = "Cancelado"
                    sesion.router.atras()
                }
                Button("Calcular", action: calcular).buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction)
                    .disabled(calculando)
            }
        }
        .onAppear(perform: cargar)
        .onChange(of: modo) { if modo != .analizar { incluirPleno = false } }
    }

    private func fila(_ titulo: String, _ columnas: String, _ coste: String) -> some View {
        GridRow {
            Text(titulo).gridColumnAlignment(.leading)
            Text(columnas).monospacedDigit().accessibilityIdentifier("calculo-\(titulo)")
            Text(coste).monospacedDigit()
        }
    }
}
