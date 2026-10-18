import Free1X2CasosDeUso
import SwiftUI

/// «Escrutar combinaciones» (pantalla 44, `EscrutarCombinacionesFrm`).
struct VistaEscrutarCombinaciones: View {
    @Environment(Sesion.self) private var sesion
    @State private var ficheros: [URL] = []
    @State private var noAciertos = "10-14"
    @State private var verPremiadas = false
    @State private var modo = 0
    @State private var signos: [String] = []
    @State private var ganadoraTexto = ""
    @State private var referencia: URL?
    @State private var plantilla = ""
    @State private var carpeta: URL?
    @State private var temporadaDos = false
    @State private var jornadaDos = true
    @State private var temporadas: [String] = []
    @State private var marcadas: Set<String> = []
    @State private var histograma: [String] = []
    @State private var tiempo = ""

    /// `ObtenerColumnaGanadora`: el texto manda si no está en blanco; si no, las casillas («*» sin signo).
    private var ganadora: String {
        let t = FormatoNET.recortar(ganadoraTexto)
        return t.isEmpty ? signos.joined() : FormatoNET.mayusculas(t)
    }

    private func escrutar() async {
        let modoE: ModoEscrutinio =
            switch modo {
            case 0: .simple(ganadora: ganadora)
            case 1: .contraFichero(referencia)
            default:
                .contraJornadas(
                    plantilla: plantilla, carpeta: carpeta, digitosTemporada: temporadaDos ? 2 : 4, digitosJornada: jornadaDos ? 2 : 1,
                    temporadas: marcadas.compactMap { Int($0) }.sorted())
            }
        let previa = EntradaEscrutinio(modo: modoE, ficheros: ficheros, rangoAciertos: [], registrarPremiadas: verPremiadas)
        do throws(ErrorDominio) {
            try CasosCombinacion.validarEscrutinioComb(previa)
        } catch {
            return sesion.alertas.error(error.mensajeUsuario)
        }
        var e = previa
        do throws(ErrorDominio) {
            e.rangoAciertos = try RangosHelper.obtenIntArray(noAciertos)
        } catch {
            return sesion.alertas.inesperado(error)
        }
        tiempo = "Calculando..."
        histograma = []
        let inicio = Date()
        do throws(ErrorDominio) {
            let r = try await CasosCombinacion.escrutarCombinaciones(
                e, numeroPartidos: sesion.numeroPartidos, configuracion: sesion.configuracion, entorno: sesion.entorno)
            histograma = r.map { "\($0.aciertos) aciertos: \($0.columnas)" }
        } catch {
            sesion.alertas.error("No se pudo completar el escrutinio:\n\n" + error.mensajeUsuario)
        }
        tiempo = CasosCombinacion.estadoFinalEscrutinio(Date().timeIntervalSince(inicio))
    }

    var body: some View {
        Pagina(destino: .escrutarCombinaciones) {
            HStack(alignment: .top, spacing: 16) {
                Tarjeta("Ficheros a escrutar") {
                    Button("Fichero a escrutar…") {
                        Task {
                            let u = await SelectorFicheros.abrirVarios([".comb", ".xml"])
                            if !u.isEmpty { ficheros = u }
                        }
                    }
                    ForEach(ficheros, id: \.self) { Text($0.lastPathComponent).font(.callout) }
                }
                .frame(width: 240)
                VStack(alignment: .leading) {
                    HStack(spacing: 16) {
                        CampoTexto(etiqueta: "No Aciertos", texto: $noAciertos, ancho: 100)
                        Toggle("Activar Ver Premiadas", isOn: $verPremiadas).toggleStyle(.switch)
                    }
                    Picker("", selection: $modo) {
                        Text("Escrutinio simple").tag(0)
                        Text("Escrutinio contra fichero").tag(1)
                        Text("Escrutinio contra jornadas").tag(2)
                    }
                    .pickerStyle(.segmented).labelsHidden()
                    switch modo {
                    case 0: simple
                    case 1: contraFichero
                    default: contraJornadas
                    }
                }
            }
            HStack {
                Button("Escrutar!") { Task { await escrutar() } }.buttonStyle(EstiloPrincipal())
                Button("Posibles premios") { sesion.router.navegar(.posiblesPremios) }
                Text(tiempo).monospacedDigit().foregroundStyle(Color.textoSecundario)
            }
            Tarjeta("Resultados") {
                ForEach(histograma, id: \.self) { Text($0).monospacedDigit() }
                // DEC-502: `ListaEscrutadasConPremio` nunca recibe columnas, así que la lista de premiadas queda vacía.
                Text("No hay columnas premiadas.").foregroundStyle(Color.textoSecundario)
                HStack {
                    Button("Seleccionar todas") {}
                    Button("Deseleccionar todas") {}
                    Button("Grabar cols") { sesion.alertas.info("No hay columnas seleccionadas que grabar.") }
                    Button("Ver Premiadas") {
                        sesion.alertas.info("No hay columnas premiadas. Activa «Ver Premiadas» antes de escrutar.")
                    }
                    Button("Ver Premiadas…") {
                        sesion.alertas.info("No hay columnas premiadas. Activa «Ver Premiadas» antes de escrutar.")
                    }
                }
            }
        }
        .onAppear {
            if signos.isEmpty {
                signos = Array(repeating: "*", count: sesion.numeroPartidos)
                temporadas = CasosCombinacion.cargarHistoricoJornadas(entorno: sesion.entorno).temporadas
            }
        }
    }

    private var simple: some View {
        Tarjeta {
            Flujo(espacio: 4) {
                ForEach(signos.indices, id: \.self) { i in
                    VStack(spacing: 2) {
                        Text(String(i + 1)).font(.caption).foregroundStyle(Color.textoSecundario)
                        Picker("", selection: $signos[i]) { ForEach(["*", "1", "X", "2"], id: \.self) { Text($0).tag($0) } }
                            .labelsHidden().frame(width: 44)
                    }
                }
            }
            CampoTexto(etiqueta: "Columna ganadora", texto: $ganadoraTexto, ancho: 180)
                .onChange(of: ganadoraTexto) { ganadoraTexto = String(ganadoraTexto.prefix(14)) }
        }
    }

    private var contraFichero: some View {
        Tarjeta {
            HStack {
                Button("Fichero referencia…") { Task { if let u = await SelectorFicheros.abrir(".txt") { referencia = u } } }
                Text(referencia?.lastPathComponent ?? "(selecciona)").foregroundStyle(Color.textoSecundario)
            }
        }
    }

    private var contraJornadas: some View {
        Tarjeta {
            CampoTexto(etiqueta: "Plantilla", texto: $plantilla, ancho: 260)
            HStack {
                Button("/t") { plantilla += "/t" }
                Button("/j") { plantilla += "/j" }
                Button("Ver archivos") { Task { if let u = await SelectorFicheros.abrir("*") { plantilla = u.lastPathComponent } } }
                Button("Carpeta…") { Task { if let u = await SelectorFicheros.carpeta() { carpeta = u } } }
                Text(carpeta?.path(percentEncoded: false) ?? "").font(.callout).foregroundStyle(Color.textoSecundario)
            }
            HStack {
                Toggle("Temporada con 2 dígitos", isOn: $temporadaDos)
                Toggle("Jornada con 2 dígitos", isOn: $jornadaDos)
            }
            Text("Temporadas").bold()
            List(temporadas, id: \.self, selection: $marcadas) { Text($0) }.frame(height: 120)
        }
    }
}
