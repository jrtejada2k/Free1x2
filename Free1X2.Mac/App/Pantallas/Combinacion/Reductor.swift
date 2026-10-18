import Free1X2CasosDeUso
import SwiftUI

/// «Reductor» (pantalla 82, `ReductorFrm`): reduce un fichero de columnas con el método y nivel elegidos.
struct VistaReductor: View {
    @Environment(Sesion.self) private var sesion
    @State private var entrada: URL?
    @State private var salida: URL?
    @State private var nivel = 13
    @State private var metodo = AlgoritmoReduccion.menosTiempo
    @State private var externas = false
    @State private var maxColumnas = 0.0
    @State private var maxPorcentaje = 100.0
    @State private var contadores = ["-", "-", "-", "-"]
    @State private var tiempo = "-"
    @State private var reduciendo = false
    @State private var tarea: Task<Void, Never>?
    /// Lo último reducido, para «Guardar resultado…» (DT-85).
    @State private var reducido: URL?

    private static let metodos: [(AlgoritmoReduccion, String)] = [
        (.menosTiempo, "menos tiempo"), (.menosColumnas, "menos columnas"), (.grandesArchivos, "grandes archivos"),
    ]

    private func seleccionarEntrada() async {
        guard let url = await SelectorFicheros.abrir(".txt") else { return }
        usarEntrada(url)
    }

    private func usarEntrada(_ url: URL) {
        guard CasosCombinacion.validarFicheroEntradaReductor(url, entorno: sesion.entorno) else {
            return sesion.alertas.error(CasosCombinacion.mensajeEntradaReductor)
        }
        entrada = url
    }

    /// Sin «Archivo salida» elegido, la reducción va a `Temp/Reducido.txt` y se guarda después si se quiere (DT-85).
    private var salidaTemporal: URL { sesion.entorno.rutas.carpeta(.temp).appending(path: "Reducido.txt") }

    private func guardarResultado() async {
        guard let reducido, let destino = await SelectorFicheros.guardar("Reducido", [("Columnas", ".txt")]) else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(try sesion.entorno.archivos.leer(reducido), en: destino, atomico: true)
        } catch {
            sesion.alertas.error("No se pudo guardar la reducción: " + error.mensajeUsuario)
        }
    }

    /// `RefrescarContadores`.
    private func refrescar(_ p: Progreso, desde inicio: Date) {
        contadores = CasosCombinacion.textosReduccion(iniciales: p.total ?? 0, procesadas: p.procesadas, finales: p.aceptadas ?? 0)
        tiempo = FormatoNET.toStringTimeSpan(sesion.entorno.reloj.ahora().timeIntervalSince(inicio))
    }

    private func reducir() {
        guard !reduciendo, let entrada else { return }
        let salida = salida ?? salidaTemporal
        reducido = nil
        let e = EntradaReduccion(
            algoritmo: metodo, entrada: entrada, salida: salida, nivel: nivel, maxCol: Int(FormatoNET.castInt32(maxColumnas)),
            percent: Int(FormatoNET.castInt32(maxPorcentaje)), externas: externas)
        let (entorno, n) = (sesion.entorno, sesion.numeroPartidos)
        reduciendo = true
        contadores = ["-", "-", "-", "-"]
        tiempo = "-"
        let inicio = entorno.reloj.ahora()
        tarea = Task {
            let (flujo, canal) = AsyncStream.makeStream(of: Progreso.self, bufferingPolicy: .bufferingNewest(1))
            let reportador = ReportadorProgreso(reloj: entorno.reloj) { _ = canal.yield($0) }
            let escucha = Task {
                for await p in flujo { refrescar(p, desde: inicio) }
            }
            do throws(ErrorDominio) {
                try entorno.archivos.crearCarpeta(salida.deletingLastPathComponent())
                _ = try await CasosCombinacion.reducirColumnas(e, numeroPartidos: n, entorno: entorno, progreso: reportador)
                reducido = salida
            } catch {
                sesion.alertas.error("Error en la reducción: " + error.mensajeUsuario)
            }
            canal.finish()
            await escucha.value
            tiempo = FormatoNET.toStringTimeSpan(entorno.reloj.ahora().timeIntervalSince(inicio))
            reduciendo = false
        }
    }

    var body: some View {
        Pagina(destino: .reductor) {
            Text("Reduce un archivo de columnas aplicando un método y nivel de reducción.").foregroundStyle(Color.textoSecundario)
            Tarjeta("Datos de Entrada") {
                HStack {
                    Button("Archivo entrada") { Task { await seleccionarEntrada() } }
                    BotonColumnasInicio { usarEntrada($0) }
                    Text(entrada?.lastPathComponent ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                        .accessibilityIdentifier("reductor-entrada")
                }
                HStack {
                    Button("Archivo salida") {
                        Task { if let url = await SelectorFicheros.guardar("Reducido", [("Columnas", ".txt")]) { salida = url } }
                    }
                    Text(salida?.lastPathComponent ?? "(temporal: guárdalo después con «Guardar resultado…»)")
                        .foregroundStyle(Color.textoSecundario)
                }
                Picker("Nivel Reducción", selection: $nivel) {
                    ForEach((1...13).reversed(), id: \.self) { Text(String($0)).tag($0) }
                }
                .frame(maxWidth: 260)
                Picker("Método Reducción", selection: $metodo) {
                    ForEach(Self.metodos, id: \.0) { Text($0.1).tag($0.0) }
                }
                .frame(maxWidth: 320)
                if metodo != .grandesArchivos {
                    Toggle("Admitir columnas externas", isOn: $externas).toggleStyle(.switch)
                }
            }
            Tarjeta("Opciones") {
                CampoNumero(etiqueta: "Máx. Columnas", valor: $maxColumnas, minimo: 0)
                CampoNumero(etiqueta: "Máx. %", valor: $maxPorcentaje, minimo: 0, maximo: 100)
            }
            Tarjeta("Resultados") {
                Grid(alignment: .leading, horizontalSpacing: 16, verticalSpacing: 4) {
                    ForEach(Array(["Col. Iniciales", "Procesadas", "Admitidas", "Porcentaje"].enumerated()), id: \.offset) { i, t in
                        GridRow {
                            Text(t)
                            Text(contadores[i]).monospacedDigit().accessibilityIdentifier("reduccion-\(t)")
                        }
                    }
                    GridRow {
                        Text("Tiempo")
                        Text(tiempo).monospacedDigit()
                    }
                }
                if reduciendo { ProgressView().controlSize(.small) }
            }
            HStack {
                Button("Reducir", action: reducir).buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction)
                    .accessibilityIdentifier("accion-reducir")
                    .disabled(entrada == nil || reduciendo)
                Button("Cancelar") {
                    tarea?.cancel()
                    reduciendo = false
                }
                Button("Guardar resultado…") { Task { await guardarResultado() } }.disabled(reducido == nil || reduciendo)
            }
        }
        .onAppear {
            // Inicio ▸ Reducir deja aquí sus columnas (DT-85).
            if let u = sesion.entradaReductor {
                sesion.entradaReductor = nil
                usarEntrada(u)
            }
        }
    }
}
