import Free1X2Base
import Free1X2CasosDeUso
import SwiftUI

/// «Esperanza matemática» (nueva, DT-91): EM exacta de una columna por categoría, con precio, reparto, retención y mínimos
/// configurables. El precio y la recaudación salen de Configuración ▸ Parámetros LAE y se pueden cambiar aquí.
struct VistaEsperanza: View {
    @Environment(Sesion.self) private var sesion
    @State private var columna = ""
    @State private var recaudacion = 0.0
    @State private var precio = 0.0
    @State private var reparto = ParametrosEsperanza.repartoExcel
    @State private var retencion = 20.0
    @State private var exento = 40_000.0
    @State private var minimo = 1.0
    @State private var minimo10 = 1.5
    @State private var masUno = true
    @State private var apostados: [[Double]] = Array(repeating: [0, 0, 0], count: 14)
    @State private var reales: [[Double]] = Array(repeating: [0, 0, 0], count: 14)
    @State private var pestana = 0
    @State private var resultado: ResultadoEsperanza?
    @State private var calculando = false
    @State private var cargado = false

    private enum Reparto: Hashable { case excel, free1x2, personalizado }

    /// El selector muestra el reparto que coincide con los campos; elegir uno los rellena.
    private var repartoElegido: Binding<Reparto> {
        Binding {
            reparto == ParametrosEsperanza.repartoExcel
                ? .excel : reparto == ParametrosEsperanza.repartoFree1X2 ? .free1x2 : .personalizado
        } set: {
            switch $0 {
            case .excel: reparto = ParametrosEsperanza.repartoExcel
            case .free1x2: reparto = ParametrosEsperanza.repartoFree1X2
            case .personalizado: break
            }
        }
    }

    private func calcular() async {
        var p = ParametrosEsperanza(recaudacion: recaudacion, precio: precio, reparto: reparto)
        (p.retencion, p.exento, p.minimo, p.minimo10, p.contarmeComoAcertante) = (retencion, exento, minimo, minimo10, masUno)
        let (c, a, r) = (columna, apostados, reales)
        calculando = true
        defer { calculando = false }
        do {
            resultado = try await Task.detached(priority: .userInitiated) {
                try CasosEsperanza.calcular(columna: c, apostados: a, reales: r, p)
            }.value
        } catch {
            resultado = nil
            sesion.alertas.error(error)
        }
    }

    private func numero(_ v: Double, _ decimales: Int) -> String {
        FormatoNET.toStringMiles(v, decimales: decimales, decimalesNegativo: decimales, cultura: sesion.entorno.cultura)
    }

    private func dinero(_ v: Double, _ decimales: Int = 2) -> String { numero(v, decimales) + " " + sesion.lae.moneda }

    /// «0,0043 % (1 entre 23.142)».
    private func probabilidad(_ p: Double) -> String {
        p > 0 ? numero(p * 100, 4) + " % (1 entre " + numero(1 / p, 0) + ")" : "0 %"
    }

    private var datos: some View {
        VStack(spacing: 10) {
            Tarjeta("Columna") {
                TextField("14 signos (1 X 2)", text: $columna).textFieldStyle(.roundedBorder).monospaced().frame(width: 170)
                    .onChange(of: columna) { _, v in
                        let u = String(v.uppercased().prefix(14))
                        if u != v { columna = u }
                    }
                    .accessibilityIdentifier("esperanza-columna")
            }
            Tarjeta("Jornada") {
                HStack(spacing: 16) {
                    CampoNumero(etiqueta: "Recaudación", valor: $recaudacion, minimo: 0, ancho: 130)
                    CampoNumero(etiqueta: "Precio apuesta", valor: $precio, minimo: 0, ancho: 70)
                }
                Text("Apuestas: recaudación ÷ precio = " + (precio > 0 ? numero(recaudacion / precio, 0) : "—"))
                    .font(.callout).foregroundStyle(Color.textoSecundario)
            }
            Tarjeta("Reparto del fondo (% de la recaudación)") {
                Picker("Reparto", selection: repartoElegido) {
                    Text("Excel: 16 · 7,5 · 7,5 · 7,5 · 9").tag(Reparto.excel)
                    Text("Free1X2: 12 · 8 · 8 · 8 · 9").tag(Reparto.free1x2)
                    Text("Personalizado").tag(Reparto.personalizado)
                }
                .frame(maxWidth: 320)
                HStack(spacing: 8) {
                    ForEach(0..<5, id: \.self) { i in
                        CampoNumero(etiqueta: "\(14 - i)", valor: $reparto[i], minimo: 0, maximo: 100, ancho: 50)
                    }
                }
            }
            Tarjeta("Cobro") {
                HStack(spacing: 16) {
                    CampoNumero(etiqueta: "Retención %", valor: $retencion, minimo: 0, maximo: 100, ancho: 50)
                    CampoNumero(etiqueta: "de lo que pase de", valor: $exento, minimo: 0, ancho: 90)
                }
                HStack(spacing: 16) {
                    CampoNumero(etiqueta: "No se cobra de 14 a 11 por debajo de", valor: $minimo, minimo: 0, ancho: 60)
                    CampoNumero(etiqueta: "ni el 10 por debajo de", valor: $minimo10, minimo: 0, ancho: 60)
                }
                Toggle("Contarme como un acertante más (+1)", isOn: $masUno)
            }
            HStack {
                Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal())
                    .disabled(calculando || columna.count != 14)
                if calculando { ProgressView().controlSize(.small) }
            }
        }
    }

    private func tabla(_ r: ResultadoEsperanza) -> some View {
        Tarjeta("Resultado") {
            Grid(alignment: .trailing, horizontalSpacing: 18, verticalSpacing: 6) {
                GridRow {
                    Text("Aciertos")
                    Text("Probabilidad real")
                    Text("Apostado con esos aciertos")
                    Text("Premio neto medio")
                    Text("EM")
                }
                .font(.caption.bold()).foregroundStyle(Color.textoSecundario)
                ForEach(r.categorias) { c in
                    GridRow {
                        Text("\(c.aciertos)").bold()
                        Text(probabilidad(c.probabilidadReal))
                        Text(probabilidad(c.probabilidadApostada))
                        Text(dinero(c.premioMedio))
                        Text(dinero(c.esperanza, 4))
                    }
                    .monospacedDigit()
                }
            }
            Text(
                "EM total: \(dinero(r.total, 4)) por cada apuesta de \(dinero(r.precio)), el \(numero(r.rendimiento * 100, 1)) % de "
                    + "lo apostado (\(numero(r.apuestas, 0)) apuestas)."
            )
            .bold()
            .accessibilityIdentifier("esperanza-total")
            Text(
                "Por encima del 100 % la columna devuelve de media más de lo que cuesta, según tus porcentajes reales. «Apostado "
                    + "con esos aciertos» es la parte de las apuestas que tendría esos aciertos si tu columna fuera la ganadora."
            )
            .font(.callout).foregroundStyle(Color.textoSecundario)
        }
    }

    var body: some View {
        Pagina(destino: .esperanza) {
            Text(
                "Esperanza matemática de una columna por categoría (14 a 10): con los porcentajes apostados se estiman los "
                    + "acertantes y el premio; con los reales, la probabilidad de cada resultado."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                datos
                Tarjeta("Valoraciones (porcentajes)") {
                    Text("Porcentajes apostados y reales por partido (1/X/2). Cada fila se toma sobre su suma.")
                        .font(.callout).foregroundStyle(Color.textoSecundario)
                    Picker("", selection: $pestana) {
                        Text("Apostados").tag(0)
                        Text("Reales").tag(1)
                    }
                    .pickerStyle(.segmented).labelsHidden()
                    if pestana == 0 { PorcentajesGrid(valores: $apostados) } else { PorcentajesGrid(valores: $reales) }
                }
            }
            if let r = resultado { tabla(r) }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            (recaudacion, precio) = (sesion.lae.recaudacion, sesion.lae.precioApuesta)
        }
    }
}
