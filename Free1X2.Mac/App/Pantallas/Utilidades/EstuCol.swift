import Free1X2CasosDeUso
import SwiftUI

/// «EstuCol» (pantalla 47, `EstucolFrm`).
struct VistaEstuCol: View {
    @Environment(Sesion.self) private var sesion
    @State private var reducidas: URL?
    @State private var ganadoras: URL?
    @State private var total = 0
    @State private var modo = AgrupacionEstuCol.a
    @State private var estado = ""
    @State private var ocupado = false

    private func elegirGanadoras() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        ganadoras = u
        do throws(ErrorDominio) {
            total = try CasosUtilidades.totalGanadorasEstuCol(u, entorno: sesion.entorno)
        } catch {
            total = 0
            sesion.alertas.error("No se ha podido leer el fichero de ganadoras: " + error.mensajeUsuario)
        }
    }

    private func comenzar() async {
        estado = "Comprobando Entradas"
        guard let reducidas, let ganadoras else {
            sesion.alertas.error("Debe especificar los dos archivos")
            return estado = "Listo"
        }
        ocupado = true
        defer { ocupado = false }
        estado = "Emparejando columnas"
        do throws(ErrorDominio) {
            sesion.informeAbdon = try await CasosUtilidades.informeEstuCol(
                reducidas: reducidas, ganadoras: ganadoras, total: total, modo: modo, entorno: sesion.entorno)
        } catch {
            sesion.alertas.error("Error al generar el informe: " + error.mensajeUsuario)
            return estado = "Listo"
        }
        estado = "Listo"
        sesion.router.navegarSiempre(.visorAnalisisColumnasAbdon)
    }

    private func fila(_ titulo: String, _ url: URL?, _ accion: @escaping () async -> Void) -> some View {
        HStack {
            Text(titulo).frame(width: 220, alignment: .leading)
            Button("Archivo…") { Task { await accion() } }
            Text(url?.lastPathComponent ?? "Sin archivo").foregroundStyle(Color.textoSecundario)
        }
    }

    var body: some View {
        Pagina(destino: .estucol) {
            Tarjeta("Ficheros") {
                fila("Entrada Columnas", reducidas) { if let u = await SelectorFicheros.abrir(".txt", "*") { reducidas = u } }
                fila("Entrada Columnas Ganadoras", ganadoras) { await elegirGanadoras() }
            }
            Tarjeta("Agrupación de Columnas") {
                Picker("", selection: $modo) {
                    Text("1,2 - 3,4...").tag(AgrupacionEstuCol.a)
                    Text("1,2 - 2,3...").tag(AgrupacionEstuCol.b)
                    Text("1,3 - 2,4...").tag(AgrupacionEstuCol.c)
                }
                .pickerStyle(.radioGroup).labelsHidden()
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Obtener Informe") { Task { await comenzar() } }.buttonStyle(EstiloPrincipal())
                    .disabled(ocupado || reducidas == nil || ganadoras == nil)
            }
        }
    }
}

/// «Visor de Informe» ABDON (pantalla 101, `VisorAnalisisColumnasAbdonFrm`).
struct VistaVisorAbdon: View {
    @Environment(Sesion.self) private var sesion
    @State private var visor = VisorAbdon(nil)
    @State private var tipoPasoFijo = 0.0
    @State private var tipoSolapadas = 0.0
    @State private var sumaEntrada = ""

    private func hacer(_ f: (inout VisorAbdon) throws(ErrorDominio) -> Void) {
        do throws(ErrorDominio) {
            try f(&visor)
        } catch {
            sesion.alertas.error(error.mensajeUsuario)
        }
    }

    private func generarCondicion() async {
        guard visor.puedeGenerarCondicion else {
            return sesion.alertas.error("No hay informe de columnas para generar la condición.")
        }
        guard
            let u = await SelectorFicheros.guardar(
                "Condicion", [("Columnas probables", ".cps"), ("Columnas probables (XML)", ".xml")])
        else { return }
        do throws(ErrorDominio) {
            let c = try visor.condicion(numeroPartidos: sesion.numeroPartidos)
            try CasosCondiciones.exportarCondicion(.colProbables, c, en: u, entorno: sesion.entorno)
            sesion.alertas.info("Filtro guardado")
        } catch {
            sesion.alertas.error("No se pudo generar la condición: " + error.mensajeUsuario)
        }
    }

    private func agrupaciones(_ titulo: String, filas: [String], tipo: Binding<Double>, hay: String, elementos: String, solapadas: Bool)
        -> some View
    {
        Tarjeta(titulo) {
            Grid(alignment: .trailing, horizontalSpacing: 16, verticalSpacing: 2) {
                GridRow {
                    Text("Número").bold()
                    Text("Elementos").bold()
                    Text("Aciertos").bold()
                }
                ForEach(filas, id: \.self) { f in
                    let p = f.components(separatedBy: "+")
                    GridRow { ForEach(p.indices, id: \.self) { Text(p[$0]).monospacedDigit() } }
                }
            }
            HStack {
                CampoNumero(etiqueta: "Mostrar sólo agrupaciones de", valor: tipo, minimo: 0, ancho: 60)
                Button("Mostrar") {
                    hacer { (v: inout VisorAbdon) throws(ErrorDominio) in
                        try v.mostrarAgrupaciones(solapadas: solapadas, tipo: Int(FormatoNET.castInt32(tipo.wrappedValue)))
                    }
                }
            }
            if !hay.isEmpty { Text("Hay \(hay) agrupaciones de \(elementos) elementos").foregroundStyle(Color.textoSecundario) }
        }
    }

    var body: some View {
        Pagina(destino: .visorAnalisisColumnasAbdon) {
            Tarjeta("Modo de análisis") {
                HStack {
                    Button(visor.global ? "Cambiar a modo individual" : "Cambiar a modo global") {
                        (tipoPasoFijo, tipoSolapadas) = (0, 0)
                        hacer { (v: inout VisorAbdon) throws(ErrorDominio) in try v.cambiarModo() }
                    }
                    if visor.global {
                        Text("En modo global se muestran rangos sobre todas las ganadoras.").foregroundStyle(Color.textoSecundario)
                    }
                }
            }
            Tarjeta("Rango de aciertos de la columna generada") {
                HStack(spacing: 12) {
                    Button("◀") { visor.moverColumna(-1) }.accessibilityLabel("Columna anterior")
                    Text(visor.columnaTexto).monospacedDigit()
                    Button("▶") { visor.moverColumna(1) }.accessibilityLabel("Columna siguiente")
                    Text("Mín \(visor.aciertosMin)")
                    Text("Máx \(visor.aciertosMax)")
                }
            }
            if !visor.global {
                Tarjeta("Columna ganadora (modo individual)") {
                    HStack(spacing: 12) {
                        Button("◀") { hacer { (v: inout VisorAbdon) throws(ErrorDominio) in try v.moverGanadora(-1) } }
                        Text(visor.ganadoraTexto).monospacedDigit()
                        Button("▶") { hacer { (v: inout VisorAbdon) throws(ErrorDominio) in try v.moverGanadora(1) } }
                    }
                }
            }
            agrupaciones(
                "Agrupaciones Paso Fijo", filas: visor.filasPasoFijo, tipo: $tipoPasoFijo, hay: visor.hayPasoFijo,
                elementos: visor.elementosPasoFijo, solapadas: false)
            agrupaciones(
                "Agrupaciones Solapadas", filas: visor.filasSolapadas, tipo: $tipoSolapadas, hay: visor.haySolapadas,
                elementos: visor.elementosSolapadas, solapadas: true)
            Tarjeta("Aciertos") {
                Text("Suma Total de la serie de Aciertos: " + visor.sumaTotal)
                if !visor.global {
                    HStack {
                        CampoTexto(etiqueta: "Suma de Aciertos de las columnas", texto: $sumaEntrada, ancho: 120)
                        Button("Sumar") { visor.calcularSumaOpcional(sumaEntrada) }
                        Text(visor.sumaOpcional).monospacedDigit()
                    }
                }
                HStack(spacing: 24) {
                    Text("Escaleras Ascendentes: " + visor.escalerasAsc)
                    Text("Descendentes: " + visor.escalerasDesc)
                    Text("Totales: " + visor.escalerasTotales)
                    Text("Sandwichs: " + visor.sandwichs)
                }
            }
            if visor.global {
                HStack {
                    Spacer()
                    Button("Generar Condición") { Task { await generarCondicion() } }.buttonStyle(EstiloPrincipal())
                }
            }
        }
        .onAppear {
            guard let i = sesion.informeAbdon else { return }
            sesion.informeAbdon = nil
            visor = VisorAbdon(i)
            hacer { (v: inout VisorAbdon) throws(ErrorDominio) in try v.mostrarPorGanadoras() }
        }
    }
}
