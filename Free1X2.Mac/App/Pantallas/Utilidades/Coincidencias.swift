import Free1X2CasosDeUso
import SwiftUI

/// «Coincidencias» (pantalla 17, `Coincidencias`).
struct VistaCoincidencias: View {
    @Environment(Sesion.self) private var sesion

    struct Condicion: Identifiable {
        let id = UUID()
        var grupos = ""
        var rango = ""
        var resultado = 0
    }

    /// Ganadoras anteriores o recientes (`colgs`, `limcgs`, `nrfCG`).
    struct Ganadoras {
        var columnas: [String] = []
        var limite = 0
        var actual = 0
        var fichero = "(sin fichero)"
        var cargadas = false
        var columna: String { actual > 0 && actual <= columnas.count ? columnas[actual - 1] : "··············" }
    }

    @State private var ficheroEntrada = "(ningún fichero)"
    @State private var columnasLeidas = "0"
    @State private var entrada = ConjuntoBits(cantidad: 0)
    @State private var hayEntrada = false
    @State private var anterior = Ganadoras()
    @State private var reciente = Ganadoras()
    @State private var condiciones = [Condicion(grupos: "1-9", rango: "0-9")]
    @State private var ficheroCondiciones = "(ninguno)"
    @State private var limGrN = 0
    @State private var condis = [Int](repeating: 0, count: 108)
    @State private var resultado: ResultadoCoincidencias?
    @State private var marcadas = [Bool](repeating: true, count: 109)
    @State private var indiceAmc = "1"
    @State private var procesadas = "0"
    @State private var validas = "0"
    @State private var tiempo = "00:00:00.0"
    @State private var ficheroSalida = "(sin grabar)"
    @State private var grabadas = "0"
    @State private var tarea: Task<Void, Never>?

    private func leerEntrada() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        await usarEntrada(u)
    }

    private func usarEntrada(_ u: URL) async {
        do throws(ErrorDominio) {
            let r = await CasosUtilidades.entradaCoincidencias(try sesion.entorno.archivos.leer(u))
            entrada = r.entrada
            if let e = r.error { return sesion.alertas.error("No se pudo leer el fichero de entrada: " + e) }
            ficheroEntrada = u.lastPathComponent
            columnasLeidas = String(r.leidas)
            hayEntrada = true
        } catch {
            sesion.alertas.error("No se pudo leer el fichero de entrada: " + error.mensajeUsuario)
        }
    }

    private func leerGanadoras(_ g: inout Ganadoras, _ nombre: String) async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        do throws(ErrorDominio) {
            let r = CasosUtilidades.ganadorasCoincidencias(try sesion.entorno.archivos.leer(u))
            // Las leídas pisan las primeras del fichero anterior, como el array fijo del C#.
            for (i, c) in r.columnas.enumerated() {
                if i < g.columnas.count { g.columnas[i] = c } else { g.columnas.append(c) }
            }
            g.limite = r.columnas.count
            if let c = r.corta { return sesion.alertas.error("col.G. errónea=" + c) }
            if r.desborde {
                return sesion.alertas.error(
                    "No se pudo leer el fichero de ganadoras \(nombre): Index was outside the bounds of the array.")
            }
            g.actual = g.limite
            g.fichero = u.lastPathComponent
            g.cargadas = true
        } catch {
            sesion.alertas.error("No se pudo leer el fichero de ganadoras \(nombre): " + error.mensajeUsuario)
        }
    }

    private func referencia() -> [Int]? {
        guard anterior.actual >= 1 else {
            sesion.alertas.error("Faltan ganadoras anteriores.")
            return nil
        }
        condis = CasosUtilidades.radiografia(anterior.columna.replacingOccurrences(of: "x", with: "X"))
        return condis
    }

    private func calcular() {
        limGrN = condiciones.count
        guard hayEntrada && anterior.cargadas && limGrN > 0 else {
            return sesion.alertas.error("Faltan entradas (fichero, ganadora anterior y condiciones).")
        }
        guard anterior.actual >= 1 else { return sesion.alertas.error("Faltan ganadoras anteriores.") }
        guard let c = CasosUtilidades.condicionesCoincidencias(condiciones.map { ($0.grupos, $0.rango) }) else {
            return sesion.alertas.error("Error en condiciones")
        }
        guard let ref = referencia() else { return }
        let (e, entorno) = (entrada, sesion.entorno)
        let inicio = entorno.reloj.ahora()
        tarea = Task {
            let (flujo, canal) = AsyncStream.makeStream(of: (Int, Int).self, bufferingPolicy: .bufferingNewest(1))
            let escucha = Task {
                for await (p, v) in flujo {
                    (procesadas, validas) = (String(p), String(v))
                    tiempo = CasosUtilidades.tiempoCorto(entorno.reloj.ahora().timeIntervalSince(inicio))
                }
            }
            let r = await CasosUtilidades.filtrarCoincidencias(e, referencia: ref, grupos: c.grupos, rangos: c.rangos) {
                _ = canal.yield(($0, $1))
            }
            canal.finish()
            await escucha.value
            resultado = r
            (procesadas, validas) = (String(r.procesadas), String(r.validas.count))
            tiempo = CasosUtilidades.tiempoCorto(entorno.reloj.ahora().timeIntervalSince(inicio))
            tarea = nil
        }
    }

    private func analizar() {
        limGrN = condiciones.count
        guard anterior.limite > 0 && reciente.limite > 0 else { return sesion.alertas.error("Faltan ganadoras") }
        guard let c = CasosUtilidades.condicionesCoincidencias(condiciones.map { ($0.grupos, $0.rango) }) else {
            return sesion.alertas.error("Error en condiciones")
        }
        do throws(ErrorDominio) {
            let a = try anterior.columnas.leer(anterior.actual - 1, "CoincidenciasViewModel.cs:Analizar")
            let r = try reciente.columnas.leer(reciente.actual - 1, "CoincidenciasViewModel.cs:Analizar")
            let (ax, rx) = (a.replacingOccurrences(of: "x", with: "X"), r.replacingOccurrences(of: "x", with: "X"))
            condis = CasosUtilidades.radiografia(ax)
            let n = CasosUtilidades.analizarCoincidencias(ax, rx, grupos: c.grupos)
            for i in n.indices where i < condiciones.count { condiciones[i].resultado = n[i] }
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    private func escribir(_ nombre: String, _ ext: String, _ lineas: [String], _ prefijo: String) async -> URL? {
        guard let u = await SelectorFicheros.guardar(nombre, [("Archivo", ext)]) else { return nil }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(TextoNET.codificarLineas(lineas), en: u, atomico: false)
            return u
        } catch {
            sesion.alertas.error(prefijo + error.mensajeUsuario)
            return nil
        }
    }

    private func leerCondiciones() async {
        guard let u = await SelectorFicheros.abrir(".cnd", "*") else { return }
        condiciones = []
        do throws(ErrorDominio) {
            condiciones = TextoNET.lineas(try sesion.entorno.archivos.leer(u)).map {
                let a = $0.components(separatedBy: ";")
                return Condicion(grupos: a[0], rango: a.count > 1 ? a[1] : "")
            }
            ficheroCondiciones = u.lastPathComponent
        } catch {
            sesion.alertas.error("No se pudieron leer las condiciones: " + error.mensajeUsuario)
        }
    }

    private var amc: Int { max(0, (Int(indiceAmc) ?? 1) - 1) }

    var body: some View {
        Pagina(destino: .coincidencias) {
            Text(
                "Calcula la radiografía de 108 características de las columnas y filtra las de entrada según las coincidencias con las columnas ganadoras de referencia."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top, spacing: 16) {
                Tarjeta("Fichero de entrada") {
                    HStack {
                        Button("Seleccionar fichero…") { Task { await leerEntrada() } }
                        BotonColumnasInicio { u in Task { await usarEntrada(u) } }
                    }
                    Text(ficheroEntrada).foregroundStyle(Color.textoSecundario)
                    Text("Columnas leídas: \(columnasLeidas)")
                }
                Tarjeta("Columnas ganadoras de referencia") {
                    ganadoras("Ganadora anterior", $anterior, "anteriores")
                    ganadoras("Ganadora reciente", $reciente, "recientes")
                }
                Tarjeta("Cálculo") {
                    Flujo(espacio: 6) {
                        Button("Calcular", action: calcular).buttonStyle(EstiloPrincipal()).disabled(tarea != nil)
                        Button("Cancelar") { tarea?.cancel() }
                        Button("Analizar", action: analizar)
                        Button("Calcular radiografía") { _ = referencia() }
                    }
                    Grid(alignment: .leading) {
                        GridRow {
                            Text("Procesadas")
                            Text(procesadas).monospacedDigit()
                        }
                        GridRow {
                            Text("Válidas")
                            Text(validas).monospacedDigit()
                        }
                        GridRow {
                            Text("Tiempo")
                            Text(tiempo).monospacedDigit()
                        }
                    }
                }
            }
            HStack(alignment: .top, spacing: 16) {
                Tarjeta("Condiciones") {
                    Text(
                        "Cada condición define un grupo de características (1-108, p.ej. «1-9,33») y el rango de coincidencias admitidas (p.ej. «0-3»)."
                    )
                    .font(.caption).foregroundStyle(Color.textoSecundario).frame(width: 320, alignment: .leading)
                    Grid(alignment: .leading) {
                        GridRow {
                            Text("Grupos")
                            Text("Rango")
                            Text("Rsl.")
                        }
                        .font(.caption).foregroundStyle(Color.textoSecundario)
                        ForEach($condiciones) { $c in
                            GridRow {
                                TextField("1-9,33", text: $c.grupos).frame(width: 150)
                                TextField("0-3", text: $c.rango).frame(width: 90)
                                Text(String(c.resultado)).monospacedDigit()
                            }
                        }
                    }
                    HStack {
                        Button("Añadir") { condiciones.append(Condicion()) }
                        Button("Quitar") { if !condiciones.isEmpty { condiciones.removeLast() } }
                        Button("Leer…") { Task { await leerCondiciones() } }
                        Button("Guardar…") {
                            Task {
                                if let u = await escribir(
                                    "Condiciones", ".cnd", condiciones.map { $0.grupos + ";" + $0.rango },
                                    "No se pudieron guardar las condiciones: ")
                                {
                                    ficheroCondiciones = u.lastPathComponent
                                }
                            }
                        }
                    }
                    Text(ficheroCondiciones).font(.caption).foregroundStyle(Color.textoSecundario)
                }
                Tarjeta("Radiografía") {
                    Text("Valor de las 108 características de la columna ganadora anterior.")
                        .font(.caption).foregroundStyle(Color.textoSecundario)
                    List(0..<108, id: \.self) { i in
                        HStack {
                            Text(String(i + 1)).frame(width: 36, alignment: .trailing)
                            Text(CasosUtilidades.descripcionesRadiografia[i]).frame(width: 90, alignment: .leading)
                            Text(String(condis[i])).frame(width: 40, alignment: .trailing)
                        }
                        .monospacedDigit()
                    }
                    .frame(width: 220, height: 300)
                    Button("Imprimir radiografía") {
                        Task {
                            let l =
                                ["Radiografia de la columna = " + String(anterior.actual)]
                                + (0..<108).map { CasosUtilidades.descripcionesRadiografia[$0] + " = " + String(condis[$0]) }
                            if let u = await escribir("radiografia", ".txt", l, "No se pudo imprimir la radiografía: ") {
                                sesion.alertas.info("Radiografía guardada en " + u.lastPathComponent)
                            }
                        }
                    }
                }
                Tarjeta("Distribución de aceptadas") {
                    Picker("Índice AMC", selection: $indiceAmc) { Text("1").tag("1") }.frame(width: 160)
                    List(0..<109, id: \.self) { n in
                        HStack {
                            Text(String(n)).frame(width: 36, alignment: .trailing)
                            Toggle("", isOn: $marcadas[n]).labelsHidden()
                            Text(String(resultado?.ac[n] ?? 0)).frame(width: 60, alignment: .trailing)
                            Text(String(resultado?.gr[n] ?? 0)).frame(width: 60, alignment: .trailing)
                            Text(String(resultado.map { amc < $0.amc.count ? $0.amc[amc][n] : 0 } ?? 0))
                                .frame(width: 60, alignment: .trailing)
                        }
                        .monospacedDigit()
                    }
                    .frame(width: 320, height: 300)
                }
            }
            Tarjeta("Grabar resultados") {
                HStack {
                    Button("Grabar resultado") {
                        Task {
                            let cols = resultado.map { CasosUtilidades.columnasCoincidencias($0, marcadas: marcadas) } ?? []
                            if let u = await escribir("Resultados", ".txt", cols, "No se pudieron grabar los resultados: ") {
                                (ficheroSalida, grabadas) = (u.lastPathComponent, String(cols.count))
                            }
                        }
                    }
                    Button("Grabar distribuciones en Excel") {
                        Task {
                            // ponytail: con más de 100 condiciones el C# rompe a mitad de fichero; aquí solo avisa.
                            guard limGrN <= 100 else {
                                return sesion.alertas.error("No se pudo exportar a Excel: Index was outside the bounds of the array.")
                            }
                            let l = CasosUtilidades.excelCoincidencias(resultado, condiciones: limGrN)
                            if let u = await escribir("Resultados", ".xls", l, "No se pudo exportar a Excel: ") {
                                sesion.alertas.info("Distribuciones guardadas en " + u.lastPathComponent)
                            }
                        }
                    }
                    Text("Fichero salida: \(ficheroSalida)")
                    Text("Grabadas: \(grabadas)")
                }
            }
        }
    }

    private func ganadoras(_ titulo: String, _ g: Binding<Ganadoras>, _ nombre: String) -> some View {
        VStack(alignment: .leading) {
            HStack {
                Text(titulo).bold()
                Button("Fichero…") {
                    Task {
                        var copia = g.wrappedValue
                        await leerGanadoras(&copia, nombre)
                        g.wrappedValue = copia
                    }
                }
                Text(g.wrappedValue.fichero).foregroundStyle(Color.textoSecundario)
            }
            HStack {
                Button("−") { if g.wrappedValue.actual > 1 { g.wrappedValue.actual -= 1 } }
                Text(String(g.wrappedValue.actual)).monospacedDigit()
                Button("+") { if g.wrappedValue.actual < g.wrappedValue.limite { g.wrappedValue.actual += 1 } }
                Text(g.wrappedValue.columna).monospaced()
            }
        }
    }
}
