import Free1X2CasosDeUso
import SwiftUI

/// «Posibles Premios» (pantalla 79, `PosiblesPremiosFrm`).
struct VistaPosiblesPremios: View {
    @Environment(Sesion.self) private var sesion
    @State private var signos = Array(repeating: "", count: 16)
    @State private var columnas: [String] = []
    @State private var partidos = 0
    @State private var nombre = ""
    @State private var pleno = false
    @State private var generarResumen = false
    @State private var resultado: ResultadoPosiblesPremios?
    @State private var resumen: [PosiblesPremiosContenedor] = []
    @State private var boleto = 1
    @State private var optando = (0..<7).map { "Optando a \(16 - $0): 0" }
    @State private var conPremio = "Columnas con premio: 0"
    @State private var noDefinitivos = 0
    @State private var ocupado = false

    private var premiadas: [String] { resultado?.premiadas ?? [] }
    private var boletos: Int { (premiadas.count + 7) / 8 }

    /// `LlenarColumnas`: las hasta 8 columnas del boleto, recortadas a los partidos del fichero.
    private var visibles: [[String]] {
        guard resultado != nil, !premiadas.isEmpty else { return [] }
        let desde = boleto * 8 - 8
        return premiadas.dropFirst(desde).prefix(8).map { c in c.utf16.prefix(partidos).map { String(decoding: [$0], as: UTF16.self) } }
    }

    private func abrir() async {
        guard let u = await SelectorFicheros.abrir(".txt", ".cols") else { return }
        columnas = []
        do throws(ErrorDominio) {
            guard let r = try CasosCombinacion.columnasJugadas(u, entorno: sesion.entorno) else {
                nombre = ""
                return sesion.alertas.error("Error leyendo columnas")
            }
            (columnas, partidos, nombre) = (r.columnas, r.partidos, u.lastPathComponent)
            for i in signos.indices where i >= partidos { signos[i] = "" }
            (resultado, boleto) = (nil, 1)
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    private func ganadora() -> String {
        let g = CasosCombinacion.ganadoraParcial(signos, partidos: partidos)
        (signos, noDefinitivos) = (g.signos, g.noDefinitivos)
        return g.ganadora
    }

    private func calcular() async {
        guard !columnas.isEmpty, partidos > 0 else { return sesion.alertas.error("Primero abre un fichero de columnas.") }
        let g = ganadora()
        ocupado = true
        let r = await CasosCombinacion.posiblesPremios(
            columnas, ganadora: g, partidos: partidos, pleno: pleno, generarResumen: generarResumen)
        ocupado = false
        resultado = r
        if generarResumen { resumen = r.resumen }
        boleto = 1
        conPremio = r.premiadas.isEmpty ? "No hay premios :(" : "Columnas con premio: \(r.premiadas.count)"
        optando = CasosCombinacion.textosOptando(r.categorias, pleno: pleno)
    }

    private func guardar() async {
        guard !resumen.isEmpty else { return sesion.alertas.info("No hay resumen que guardar. Marca 'Generar resumen' y calcula.") }
        guard let u = await SelectorFicheros.guardar("Resumen", [("Resumen", ".txt")]) else { return }
        do throws(ErrorDominio) {
            let t = CasosCombinacion.textoResumenPosibles(resumen, noDefinitivos: noDefinitivos)
            try sesion.entorno.archivos.escribir(Data(t.utf8), en: u, atomico: false)
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    var body: some View {
        Pagina(destino: .posiblesPremios) {
            Text("Introduce la columna ganadora, abre un fichero de boletos y calcula a cuántos premios opta cada columna jugada.")
                .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                Tarjeta("Columna ganadora y columnas jugadas") {
                    Text(
                        "Para cada partido elige el signo ganador (1 / X / 2, o * si no es definitivo). Las casillas de la derecha muestran los signos de las hasta 8 columnas leídas del fichero."
                    )
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                    Grid(alignment: .center, horizontalSpacing: 6, verticalSpacing: 2) {
                        GridRow {
                            Text("Part.").bold()
                            Text("Ganadora").bold()
                            ForEach(0..<8, id: \.self) { Text("C\($0 + 1)").font(.caption).bold() }
                        }
                        let v = visibles
                        ForEach(0..<16, id: \.self) { p in
                            GridRow {
                                Text(String(p + 1)).monospacedDigit()
                                Picker("", selection: $signos[p]) { ForEach(["", "1", "X", "2", "*"], id: \.self) { Text($0).tag($0) } }
                                    .labelsHidden().frame(width: 60)
                                ForEach(0..<8, id: \.self) { c in
                                    Text(c < v.count && p < v[c].count ? v[c][p] : " ").monospaced().frame(width: 18)
                                }
                            }
                        }
                    }
                }
                VStack(spacing: 10) {
                    Tarjeta("Fichero de boletos") {
                        HStack {
                            Button("Archivo...") { Task { await abrir() } }
                            Text(nombre).foregroundStyle(Color.textoSecundario)
                        }
                        HStack {
                            Button("◀") { if boleto > 1 { boleto -= 1 } }.accessibilityLabel("Boleto anterior")
                            Text("\(premiadas.isEmpty ? 0 : boleto) / \(boletos)").monospacedDigit()
                            Button("▶") { if boleto < boletos { boleto += 1 } }.accessibilityLabel("Boleto siguiente")
                        }
                    }
                    Tarjeta("Opciones") {
                        Toggle("Considerar último partido como pleno", isOn: $pleno)
                        Toggle("Generar resumen", isOn: $generarResumen)
                    }
                    Tarjeta("Resultados") {
                        ForEach(optando, id: \.self) { Text($0) }
                        Text(conPremio).bold()
                    }
                    HStack {
                        Button("Mis mejores opciones") {
                            guard !columnas.isEmpty, partidos > 0 else {
                                return sesion.alertas.error("Primero abre un fichero de columnas.")
                            }
                            sesion.contextoMejoresOpciones = (ganadora(), columnas, pleno)
                            sesion.router.navegar(.mejoresOpciones)
                        }
                        Button("Ver") {
                            guard !resumen.isEmpty else {
                                return sesion.alertas.info("No hay resumen que mostrar. Marca 'Generar resumen' y calcula.")
                            }
                            sesion.resumenPosiblesPremios = resumen
                            sesion.router.navegar(.visorPosiblesPremios)
                        }
                    }
                    HStack {
                        Button("Copiar") {
                            let t = CasosCombinacion.textoCopiarPosibles(optando, noDefinitivos: noDefinitivos, partidos: partidos)
                            Task { await sesion.entorno.portapapeles.escribirTexto(t) }
                        }
                        Button("Guardar Resumen") { Task { await guardar() } }
                        Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).disabled(ocupado)
                    }
                }
            }
        }
    }
}

/// «Visor de Posibles Premios» (pantalla 104, `VisorPosiblesPremios`).
struct VistaVisorPosiblesPremios: View {
    @Environment(Sesion.self) private var sesion
    @State private var indice = 0

    var body: some View {
        let resumen = sesion.resumenPosiblesPremios
        let grupo = resumen.indices.contains(indice) ? CasosCombinacion.grupoVisor(resumen[indice]) : nil
        Pagina(destino: .visorPosiblesPremios) {
            Text(
                "Recorre los grupos calculados. Cada grupo muestra su columna ganadora y las columnas jugadas que optan a premio (16/15/14/13/12/11/10 aciertos)."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack {
                Button("◀") { if indice > 0 { indice -= 1 } }.accessibilityLabel("Grupo anterior")
                Text(resumen.isEmpty ? "0 de 0" : "\(indice + 1) de \(resumen.count)").monospacedDigit()
                Button("▶") { if indice < resumen.count - 1 { indice += 1 } }.accessibilityLabel("Grupo siguiente")
            }
            HStack(alignment: .top) {
                Tarjeta("Columna ganadora (CG)") {
                    VStack(spacing: 2) {
                        ForEach(Array((grupo?.ganadora ?? []).enumerated()), id: \.offset) { _, s in
                            Text(s).monospaced().frame(width: 28, height: 28).background(Color.superficieAlt, in: .rect(cornerRadius: 4))
                        }
                    }
                }
                Tarjeta("Columnas con premio") {
                    Text("Cada bloque es una columna jugada del grupo; la etiqueta superior indica la categoría de premio a la que opta.")
                        .foregroundStyle(Color.textoSecundario)
                    ScrollView(.horizontal) {
                        LazyHStack(alignment: .top, spacing: 8) {
                            ForEach(Array((grupo?.columnas ?? []).enumerated()), id: \.offset) { _, c in
                                VStack(spacing: 2) {
                                    Text(c.aciertosTexto).font(.caption).bold().help("Aciertos de la columna")
                                    Text(String(c.categoria)).font(.caption2).foregroundStyle(Color.textoSecundario)
                                    ForEach(c.signos.indices, id: \.self) { i in
                                        Text(c.signos[i]).monospaced().frame(width: 24, height: 24)
                                            .foregroundStyle(c.aciertos[i] ? Color.primary : Color.white)
                                            .background(c.aciertos[i] ? Color.superficieAlt : Color.error, in: .rect(cornerRadius: 3))
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

/// «Mis Mejores Opciones» (pantalla 72, `MejoresOpcionesFrm`).
struct VistaMejoresOpciones: View {
    @Environment(Sesion.self) private var sesion
    @State private var ganadora = "**************"
    @State private var columnas: [String] = []
    @State private var pleno = false
    @State private var involucrados = Array(repeating: false, count: 16)
    @State private var limite = 10.0
    @State private var resumen = ""
    @State private var cargado = false

    private func calcular() async {
        if limite < 1 { limite = 10 }
        resumen = "Calculando..."
        do throws(ErrorDominio) {
            resumen = try await CasosCombinacion.mejoresOpciones(
                ganadora: ganadora, columnas: columnas, pleno: pleno, involucrados: involucrados,
                limite: Int(FormatoNET.castInt32(limite)))
        } catch {
            sesion.alertas.error("No se pudo calcular las mejores opciones:\n\n" + error.mensajeUsuario)
            resumen = "No se pudo completar el cálculo."
        }
    }

    var body: some View {
        Pagina(destino: .mejoresOpciones) {
            Text(
                "Indica que partidos pueden variar respecto a la columna ganadora conocida. Se generan todas las columnas ganadoras posibles, se escrutan tus columnas jugadas y se muestran las que mas premios producen."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Partidos Involucrados") {
                Text("Marca los partidos cuyo resultado podria cambiar.").foregroundStyle(Color.textoSecundario)
                HStack {
                    ForEach(0..<min(16, ganadora.utf16.count), id: \.self) { i in
                        Toggle(String(i + 1), isOn: $involucrados[i]).toggleStyle(.checkbox)
                    }
                }
                HStack {
                    Text("Mostrar los")
                    CampoNumero(etiqueta: "", valor: $limite, minimo: 1, ancho: 60).labelsHidden()
                    Text("primeros resultados")
                    Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal())
                }
            }
            Tarjeta("Resumen") {
                ScrollView {
                    Text(resumen).monospaced().textSelection(.enabled).frame(maxWidth: .infinity, alignment: .leading)
                }
                .frame(minHeight: 240)
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            guard let c = sesion.contextoMejoresOpciones else { return }
            sesion.contextoMejoresOpciones = nil
            (ganadora, columnas, pleno) = (c.ganadora.isEmpty ? "**************" : c.ganadora, c.columnas, c.pleno)
        }
    }
}
