import Free1X2CasosDeUso
import SwiftUI

/// «Rotación de signos» (pantalla 85, `RotacionDeSignosFrm`).
struct VistaRotacionDeSignos: View {
    struct Fila {
        var base = "1"
        var cambios = ["1", "X", "2"]
    }

    @Environment(Sesion.self) private var sesion
    @State private var entrada: URL?
    @State private var salida: URL?
    @State private var partidos = 0
    @State private var filas: [Fila] = []
    @State private var correccion = false
    @State private var estado = "Faltan datos"
    @State private var ocupado = false
    /// Paneles del legacy (C2 del plan): «% COMBINACIÓN» (de la entrada) y «Valoraciones JORNADA» (del usuario).
    @State private var porcentajes: [[Double]] = []
    @State private var jornada: [[Double]] = []

    private static let signos = ["1", "X", "2"]

    private func actualizar() { estado = entrada != nil && salida != nil ? "Preparado" : "Faltan datos" }

    private func seleccionarEntrada() async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        leerEntrada(u)
    }

    /// `LeerColumnas` del legacy: base, partidos y «% COMBINACIÓN».
    private func leerEntrada(_ u: URL) {
        entrada = u
        do throws(ErrorDominio) {
            let r = try CasosUtilidades.entradaRotacion(u, numeroPartidos: sesion.numeroPartidos, entorno: sesion.entorno)
            partidos = r.partidos
            filas = Array(repeating: Fila(), count: r.partidos)
            for (i, s) in Array((r.primera ?? "").utf16).prefix(filas.count).enumerated() {
                filas[i].base = String(decoding: [s], as: UTF16.self)
            }
            porcentajes = try CasosUtilidades.porcentajesRotacion(u, partidos: r.partidos, entorno: sesion.entorno)
            if jornada.count != r.partidos { jornada = Array(repeating: [0, 0, 0], count: r.partidos) }
        } catch {
            sesion.alertas.error("Error al leer el archivo: " + error.mensajeUsuario)
        }
        actualizar()
    }

    private func aceptar() async {
        guard let entrada, let salida else { return estado = "Faltan datos" }
        let nuevos = filas.prefix(partidos).map { $0.cambios.map(CasosUtilidades.indiceSigno) }
        ocupado = true
        estado = "Procesando..."
        do throws(ErrorDominio) {
            let n = try await CasosUtilidades.rotarSignos(
                entrada, salida: salida, nuevos: nuevos, correccion: correccion, numeroPartidos: sesion.numeroPartidos,
                entorno: sesion.entorno)
            estado = "Se han grabado \(n) columnas"
        } catch {
            estado = "Error: " + error.mensajeUsuario
        }
        ocupado = false
        // DEC-1012: el `finally` del C# reescribe el estado con «Preparado»/«Faltan datos».
        actualizar()
    }

    /// `PropuestaAutomatica`: cada partido rota sus signos según la valoración de la jornada.
    private func proponer() {
        // Como el legacy, solo al modificar la jornada: con la valoración vacía se deja la rotación del usuario.
        guard !porcentajes.isEmpty, jornada.joined().contains(where: { $0 != 0 }) else { return }
        for (i, n) in CasosUtilidades.propuestaRotacion(combinacion: porcentajes, jornada: jornada).enumerated() where i < filas.count {
            filas[i].cambios = n.map { Self.signos[$0] }
        }
    }

    private func abrirValoracion() async {
        guard let url = await SelectorFicheros.abrir(".txt", "*") else { return }
        do throws(ErrorDominio) {
            let r = try CasosCondiciones.importarValoracion(url, numeroPartidos: max(partidos, 1), entorno: sesion.entorno)
            if let aviso = r.aviso { sesion.alertas.error(aviso) }
            jornada = r.valores
        } catch {
            sesion.alertas.error("Error al leer la valoración: " + error.mensajeUsuario)
        }
    }

    /// Botón «Transponer» del legacy (`RotacionDeSignosFrm.cs:2264-2286`): `TransponedorAutomatico` con la jornada y la
    /// combinación, transposición a la salida (o a «<entrada>T») y esta pasa a ser la entrada.
    private func transponerAutomatico() async {
        guard let entrada else { return }
        guard let orden = TransponedorAutomatico.trasposicion(valoraciones: jornada, frecuencias: porcentajes) else {
            return sesion.alertas.error("La transposición automática necesita 14 partidos con valoración y columnas leídas.")
        }
        let destino =
            salida
            ?? entrada.deletingLastPathComponent().appending(
                path: entrada.deletingPathExtension().lastPathComponent + "T." + entrada.pathExtension)
        guard destino != entrada else { return }
        let lista = orden.map { String($0 + 1) }.joined(separator: ", ")
        guard await sesion.alertas.confirmar("Orden de transposición: \(lista).\nSe grabará en \(destino.lastPathComponent). ¿Transponer?")
        else { return }
        do throws(ErrorDominio) {
            try await CasosUtilidades.transponerColumnas(entrada, salida: destino, orden: orden, entorno: sesion.entorno)
            salida = destino
            leerEntrada(destino)
            proponer()
        } catch {
            sesion.alertas.error("Error al transponer: " + error.mensajeUsuario)
        }
    }

    /// Tabla de solo lectura «Part. / 1 / X / 2».
    private func tabla(_ v: [[Double]]) -> some View {
        Grid(alignment: .trailing, horizontalSpacing: 10, verticalSpacing: 3) {
            GridRow {
                Text("Part.")
                Text("1")
                Text("X")
                Text("2")
            }
            .font(.caption).foregroundStyle(Color.textoSecundario)
            ForEach(v.indices, id: \.self) { i in
                GridRow {
                    Text("\(i + 1)")
                    ForEach(0..<3, id: \.self) { j in Text(CampoNumero.mostrar(v[i][j])).monospacedDigit() }
                }
            }
        }
    }

    private func selector(_ valor: Binding<String>) -> some View {
        Picker("", selection: valor) { ForEach(Self.signos, id: \.self) { Text($0).tag($0) } }.labelsHidden().frame(width: 60)
    }

    var body: some View {
        Pagina(destino: .rotacionDeSignos) {
            Tarjeta("Ficheros") {
                HStack {
                    Text("Entrada").frame(width: 60, alignment: .leading)
                    Button("Examinar…") { Task { await seleccionarEntrada() } }
                    Text(entrada?.lastPathComponent ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    Text("Salida").frame(width: 60, alignment: .leading)
                    Button("Examinar…") {
                        Task {
                            if let u = await SelectorFicheros.guardar("ColumnasRotadas", [("Columnas", ".txt")]) { salida = u }
                            actualizar()
                        }
                    }
                    Text(salida?.lastPathComponent ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                }
            }
            HStack(alignment: .top, spacing: 12) {
                Tarjeta("% Combinación") {
                    Text("Frecuencia de cada signo en las columnas de la entrada.").font(.caption).foregroundStyle(Color.textoSecundario)
                    if porcentajes.isEmpty {
                        Text("Elige un fichero de entrada.").foregroundStyle(Color.textoSecundario)
                    } else {
                        tabla(porcentajes)
                    }
                }
                Tarjeta("Valoraciones jornada") {
                    Text("Al cambiarlas se propone la rotación de cada partido.").font(.caption).foregroundStyle(Color.textoSecundario)
                    PorcentajesGrid(valores: $jornada)
                    HStack {
                        Button("Abrir valoración…") { Task { await abrirValoracion() } }
                        Button("Transposición automática") { Task { await transponerAutomatico() } }
                            .disabled(entrada == nil || porcentajes.isEmpty || ocupado)
                    }
                }
            }
            Tarjeta("Cambios de signo") {
                Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 4) {
                    GridRow {
                        Text("Nº").bold()
                        Text("Base").bold()
                        Text("1 →").bold()
                        Text("X →").bold()
                        Text("2 →").bold()
                    }
                    ForEach(filas.indices, id: \.self) { i in
                        GridRow {
                            Text(String(i + 1)).monospacedDigit()
                            selector($filas[i].base)
                            ForEach(0..<3, id: \.self) { s in selector($filas[i].cambios[s]) }
                        }
                    }
                }
                HStack {
                    Toggle("Corrección de fallos", isOn: $correccion).toggleStyle(.switch)
                    Button("Transponer") {
                        for i in filas.indices {
                            filas[i].cambios = filas[i].cambios.map { $0 == "1" ? "X" : $0 == "X" ? "2" : "1" }
                        }
                    }
                }
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                // «Cancelar» no hace nada en el C# (`VM:206-210`).
                Button("Cancelar") {}
                Button("Aceptar") { Task { await aceptar() } }.buttonStyle(EstiloPrincipal())
                    .disabled(ocupado || entrada == nil || salida == nil)
            }
        }
        .onAppear {
            if filas.isEmpty {
                partidos = sesion.numeroPartidos
                filas = Array(repeating: Fila(), count: partidos)
                jornada = Array(repeating: [0, 0, 0], count: partidos)
            }
        }
        .onChange(of: jornada) { proponer() }
    }
}
