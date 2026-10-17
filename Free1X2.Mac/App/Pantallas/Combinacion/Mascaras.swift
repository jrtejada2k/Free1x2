import Free1X2CasosDeUso
import SwiftUI

/// Una fila de la clasificación: columnas premiadas por categoría en todo el histórico.
struct FilaMascara: Identifiable {
    let nombre: String
    let columnas: Int
    let p14, p13, p12, p11, p10: Int
    let jornadasConPremio: Int
    let jornadas: Int
    var id: String { nombre }

    init(_ r: ResultadoMascara) {
        (nombre, columnas, jornadasConPremio, jornadas) = (r.nombre, r.columnas, r.jornadasConPremio, r.jornadas)
        (p14, p13, p12, p11, p10) = (r.premios[14] ?? 0, r.premios[13] ?? 0, r.premios[12] ?? 0, r.premios[11] ?? 0, r.premios[10] ?? 0)
    }
}

/// «Máscaras» (nueva, DT-84): las guardadas, aplicarlas al boleto y compararlas contra las ganadoras históricas.
struct VistaMascaras: View {
    @Environment(Sesion.self) private var sesion
    @State private var mascaras: [Mascara] = []
    @State private var filas: [FilaMascara] = []
    @State private var orden = [KeyPathComparator(\FilaMascara.p10, order: .reverse)]
    @State private var pais = PaisQuiniela.es
    @State private var estado = ""
    @State private var ocupado = false
    @State private var guardando = false

    private func recargar() { mascaras = CasosMascaras.listar(numeroPartidos: sesion.numeroPartidos, entorno: sesion.entorno) }

    private func borrar(_ m: Mascara) async {
        guard await sesion.alertas.confirmar("¿Borrar la máscara «\(m.nombre)»?") else { return }
        do throws(ErrorDominio) {
            try CasosMascaras.borrar(m.nombre, entorno: sesion.entorno)
        } catch {
            sesion.alertas.error("No se pudo borrar la máscara: " + error.mensajeUsuario)
        }
        filas.removeAll { $0.nombre == m.nombre }
        recargar()
    }

    private func escrutar() async {
        guard !ocupado else { return }
        ocupado = true
        defer { ocupado = false }
        estado = "Descargando el histórico…"
        do {
            let historico = try await sesion.servicioOnline.obtenerHistorico(pais: pais.rawValue)
            let ganadoras = historico.map(\.ganadora)
            filas = mascaras.map { FilaMascara(CasosMascaras.escrutar($0, ganadoras: ganadoras)) }
            estado = "\(historico.count) jornadas escrutadas."
        } catch {
            filas = []
            estado = ""
            sesion.alertas.error(
                "No se pudo descargar el histórico: " + ((error as? ErrorOnline)?.mensaje ?? error.localizedDescription))
        }
    }

    private func fila(_ m: Mascara) -> some View {
        let r = m.resumen
        return HStack(spacing: 12) {
            VStack(alignment: .leading, spacing: 2) {
                Text(m.nombre).bold()
                Text(m.pronosticos.map { $0.replacing(",", with: "") }.joined(separator: " "))
                    .font(.system(.callout, design: .monospaced)).foregroundStyle(Color.textoSecundario)
            }
            Spacer()
            Text("F \(r.fijos) · D \(r.dobles) · T \(r.triples) · \(m.columnas) col.").monospacedDigit()
                .foregroundStyle(Color.textoSecundario)
            Button("Aplicar al boleto") {
                sesion.aplicarMascara(m)
                sesion.router.ir(.inicio)
            }
            .accessibilityIdentifier("mascara-aplicar-\(m.nombre)")
            Button("Borrar") { Task { await borrar(m) } }
        }
    }

    var body: some View {
        Pagina(destino: .mascaras) {
            Text(
                "Una máscara guarda los signos de cada partido, sin equipos. Aplícala a la jornada cargada o compárala con las "
                    + "ganadoras históricas para ver cuántas columnas de 10 a 14 aciertos habría tenido."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Máscaras guardadas") {
                if mascaras.isEmpty {
                    Text("Aún no hay máscaras: marca los signos en Inicio y pulsa «Guardar boleto como máscara…».")
                        .foregroundStyle(Color.textoSecundario)
                }
                ForEach(mascaras) { fila($0) }
                Button("Guardar boleto como máscara…") { guardando = true }
            }
            Tarjeta("Escrutinio histórico") {
                HStack {
                    Picker("Ganadoras de", selection: $pais) {
                        Text("España").tag(PaisQuiniela.es)
                        Text("México").tag(PaisQuiniela.mx)
                    }
                    .frame(maxWidth: 260)
                    Button("Escrutar las máscaras") { Task { await escrutar() } }.buttonStyle(EstiloPrincipal())
                        .disabled(ocupado || mascaras.isEmpty)
                    Text(estado).foregroundStyle(Color.textoSecundario)
                }
                if !filas.isEmpty {
                    Table(filas.sorted(using: orden), sortOrder: $orden) {
                        TableColumn("Máscara", value: \.nombre)
                        TableColumn("Columnas", value: \.columnas) { Text("\($0.columnas)").monospacedDigit() }
                        TableColumn("14", value: \.p14) { Text("\($0.p14)").monospacedDigit() }
                        TableColumn("13", value: \.p13) { Text("\($0.p13)").monospacedDigit() }
                        TableColumn("12", value: \.p12) { Text("\($0.p12)").monospacedDigit() }
                        TableColumn("11", value: \.p11) { Text("\($0.p11)").monospacedDigit() }
                        TableColumn("10", value: \.p10) { Text("\($0.p10)").monospacedDigit() }
                        TableColumn("Jornadas con premio", value: \.jornadasConPremio) {
                            Text("\($0.jornadasConPremio) de \($0.jornadas)").monospacedDigit()
                        }
                    }
                    .frame(minHeight: 180)
                    Text("Pulsa una cabecera para ordenar. Cada cifra suma las columnas premiadas de todas las jornadas.")
                        .font(.callout).foregroundStyle(Color.textoSecundario)
                }
            }
        }
        .modifier(GuardarMascara(visible: $guardando, alGuardar: recargar))
        .onAppear {
            recargar()
            pais = CasosArchivo.paisInicial(entorno: sesion.entorno)
        }
    }
}

/// «Guardar boleto como máscara…»: pide el nombre, confirma si ya existe y guarda los signos del boleto base.
struct GuardarMascara: ViewModifier {
    @Environment(Sesion.self) private var sesion
    @Binding var visible: Bool
    var alGuardar: () -> Void = {}
    @State private var nombre = ""

    private func guardar() async {
        let n = CasosMascaras.nombreValido(nombre) ?? nombre
        if CasosMascaras.existe(n, entorno: sesion.entorno),
            !(await sesion.alertas.confirmar("Ya hay una máscara «\(n)». ¿Reemplazarla?"))
        {
            return
        }
        do throws(ErrorDominio) {
            try CasosMascaras.guardar(sesion.mascara(n), entorno: sesion.entorno)
            nombre = ""
            alGuardar()
            sesion.alertas.info("Máscara «\(n)» guardada.")
        } catch {
            sesion.alertas.error("No se pudo guardar la máscara: " + error.mensajeUsuario)
        }
    }

    func body(content: Content) -> some View {
        content.alert("Guardar boleto como máscara", isPresented: $visible) {
            TextField("Nombre", text: $nombre)
            Button("Guardar") { Task { await guardar() } }
            Button("Cancelar", role: .cancel) {}
        } message: {
            Text("Se guardan los signos de los \(sesion.numeroPartidos) partidos, sin los equipos.")
        }
    }
}
