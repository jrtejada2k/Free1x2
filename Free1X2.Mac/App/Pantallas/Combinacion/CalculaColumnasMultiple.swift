import Free1X2CasosDeUso
import SwiftUI

/// «Calcular Múltiples Combinaciones» (pantalla 14, `CalculaColumnasMultipleFrm`): graba en `Columnas/` las columnas de
/// cada combinación, en orden.
struct VistaCalculaColumnasMultiple: View {
    @Environment(Sesion.self) private var sesion
    @State private var combinaciones: [URL] = []
    @State private var seleccionado = -1
    @State private var maximas: Int64 = 0
    @State private var textos = TextosCalculo()
    @State private var enMarcha = false
    @State private var progresoArchivos = 0.0
    @State private var horaComienzo = ""
    @State private var horaFinal = ""
    @State private var tiempoTotal = ""
    @State private var tarea: Task<Void, Never>?

    private var cultura: Cultura { .sistema() }

    private func actualizar(_ procesadas: Int, _ aceptadas: Int) {
        textos = CasosCombinacion.estadisticasDeCalculo(
            procesadas: procesadas, aceptadas: aceptadas, maximas: maximas, lae: sesion.lae, cultura: cultura)
    }

    private func calcular() async {
        guard !enMarcha, !combinaciones.isEmpty else { return }
        let entorno = sesion.entorno
        let carpeta = entorno.rutas.carpeta(.columnas)
        do throws(ErrorDominio) {
            try entorno.archivos.crearCarpeta(carpeta)
        } catch {
            return sesion.alertas.error("No se pudo acceder a la carpeta de salida: " + error.mensajeUsuario)
        }
        let salidas = combinaciones.map { CasosCombinacion.nombreSalidaDeCombinacion($0, carpeta: carpeta) }
        if salidas.contains(where: { entorno.archivos.existe($0) }),
            !(await sesion.alertas.confirmar("Algunos archivos de columnas ya existen. ¿Deseas reemplazarlos?"))
        {
            return
        }
        enMarcha = true
        progresoArchivos = 0
        (horaFinal, tiempoTotal) = ("", "")
        var cfg = sesion.configuracion
        cfg.puntosCP = CasosAjustes.leerPuntosCP(entorno: entorno)
        let (n, origenes) = (sesion.numeroPartidos, combinaciones)
        let inicio = entorno.reloj.ahora()
        horaComienzo = FormatoNET.horaLarga(inicio, cultura: cultura)
        tarea = Task {
            var resultados: [ResumenLote] = []
            var inicioArchivo = inicio
            do throws(ErrorDominio) {
                for (i, origen) in origenes.enumerated() {
                    if Task.isCancelled { break }
                    seleccionado = i
                    progresoArchivos = Double(i + 1) * 100 / Double(origenes.count)
                    let p = try CasosCombinacion.prepararCombinacionDeLote(origen, numeroPartidos: n, entorno: entorno)
                    if let aviso = p.aviso { sesion.alertas.info(aviso) }
                    maximas = p.maximas
                    let (flujo, canal) = AsyncStream.makeStream(of: Progreso.self, bufferingPolicy: .bufferingNewest(1))
                    let reportador = ReportadorProgreso(reloj: entorno.reloj) { _ = canal.yield($0) }
                    let escucha = Task {
                        for await q in flujo { actualizar(q.procesadas, q.aceptadas ?? 0) }
                    }
                    let r = try await CasosCombinacion.calcularCombinacion(
                        p.combinacion, modo: .grabar(salidas[i]), configuracion: cfg, entorno: entorno, progreso: reportador)
                    canal.finish()
                    await escucha.value
                    actualizar(r.calculo.analizadas, r.calculo.aceptadas)
                    let ahora = entorno.reloj.ahora()
                    resultados.append(
                        ResumenLote(
                            archivoCombinacion: origen.lastPathComponent, archivoColumnas: salidas[i].lastPathComponent,
                            analizadas: String(r.calculo.analizadas), aceptadas: String(r.calculo.aceptadas),
                            tiempo: FormatoNET.toStringTimeSpan(ahora.timeIntervalSince(inicioArchivo))))
                    inicioArchivo = ahora
                }
            } catch {
                sesion.alertas.error("Error en el cálculo en lote: " + error.mensajeUsuario)
            }
            let fin = entorno.reloj.ahora()
            horaFinal = FormatoNET.horaLarga(fin, cultura: cultura)
            tiempoTotal = FormatoNET.toStringTimeSpan(fin.timeIntervalSince(inicio))
            enMarcha = false
            sesion.resultadosLote = resultados
            sesion.router.navegar(.resultadosCalculoMultiple)
        }
    }

    var body: some View {
        Pagina(destino: .calculaColumnasMultiple) {
            Text(
                "Procesa en lote varios archivos de combinaciones (.comb / .xml) y genera sus columnas reducidas en la carpeta Columnas."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Archivos a procesar") {
                Text("Selecciona uno o varios archivos de combinaciones. Se procesarán en orden alfabético.")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                Button("Seleccionar archivos…") {
                    Task {
                        let urls = await SelectorFicheros.abrirVarios([".comb", ".xml", "*"])
                        if !urls.isEmpty { combinaciones = CasosCombinacion.ordenarLote(urls) }
                    }
                }
                ForEach(combinaciones.indices, id: \.self) { i in
                    Text(combinaciones[i].lastPathComponent).font(.callout).fontWeight(i == seleccionado ? .bold : .regular)
                }
            }
            Tarjeta("Estadísticas del cálculo") {
                Grid(alignment: .trailing, horizontalSpacing: 24, verticalSpacing: 6) {
                    GridRow {
                        Text("")
                        Text("Columnas").bold()
                        Text("Coste").bold()
                    }
                    fila("Procesadas", textos.procesadas, textos.costeProcesadas)
                    fila("Aceptadas", textos.aceptadas, textos.costeAceptadas)
                    fila("Porcentaje", textos.porcentaje, "")
                    fila("Estimadas", textos.estimadas, textos.costeEstimadas)
                    fila("Máximo", textos.maximo, textos.costeMaximo)
                }
            }
            Tarjeta("Tiempos") {
                Grid(alignment: .leading, horizontalSpacing: 24, verticalSpacing: 6) {
                    fila("Comienzo", horaComienzo, "")
                    fila("Final", horaFinal, "")
                    fila("Tiempo total", tiempoTotal, "")
                }
            }
            if enMarcha {
                Tarjeta("Progreso") {
                    LabeledContent("Archivos") { ProgressView(value: progresoArchivos, total: 100) }
                    LabeledContent("Columnas del archivo en curso") { ProgressView(value: textos.progreso, total: 100) }
                }
            }
            HStack {
                Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction)
                    .disabled(combinaciones.isEmpty || enMarcha)
                Button("Cancelar") {
                    tarea?.cancel()
                    enMarcha = false
                }
            }
        }
    }

    private func fila(_ titulo: String, _ a: String, _ b: String) -> some View {
        GridRow {
            Text(titulo).gridColumnAlignment(.leading)
            Text(a).monospacedDigit()
            Text(b).monospacedDigit()
        }
    }
}

/// «Resultados de la Generación Múltiple» (`ResultadosCalculoMultipleFrm`).
struct VistaResultadosCalculoMultiple: View {
    @Environment(Sesion.self) private var sesion
    @State private var filas: [ResumenLote] = []
    @State private var cargado = false

    var body: some View {
        Pagina(destino: .resultadosCalculoMultiple) {
            Text(
                "Resumen de cada cálculo realizado en lote: fichero de combinación, fichero de columnas generado, columnas analizadas, columnas aceptadas y tiempo invertido."
            )
            .foregroundStyle(Color.textoSecundario)
            Grid(alignment: .leading, horizontalSpacing: 16, verticalSpacing: 4) {
                GridRow {
                    Text("Arch. Combinación").bold()
                    Text("Arch. Columnas").bold()
                    Text("Cols. Analizadas").bold()
                    Text("Cols. Aceptadas").bold()
                    Text("Tiempo").bold()
                }
                ForEach(filas, id: \.self) { f in
                    GridRow {
                        Text(f.archivoCombinacion)
                        Text(f.archivoColumnas)
                        Text(f.analizadas).monospacedDigit()
                        Text(f.aceptadas).monospacedDigit()
                        Text(f.tiempo).monospacedDigit()
                    }
                }
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            filas = sesion.resultadosLote ?? []
            sesion.resultadosLote = nil
        }
    }
}
