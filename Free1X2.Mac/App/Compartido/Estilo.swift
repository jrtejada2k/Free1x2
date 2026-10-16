import AppKit
import Free1X2CasosDeUso
import SwiftUI

// Sistema de diseño (ESPEC_15 §1.1-1.2; `Themes/Tokens.xaml:14-116`). ponytail: colores dinámicos en código en vez de
// Color Sets del Asset Catalog; mismo resultado claro/oscuro.

extension Color {
    static let acento = Color(claro: 0x3D5A80, oscuro: 0x818CF8)
    static let fondo = Color(claro: 0xF4F6F8, oscuro: 0x0B1120)
    static let superficie = Color(claro: 0xFFFFFF, oscuro: 0x0F172A)
    static let superficieAlt = Color(claro: 0xE6ECF2, oscuro: 0x1E293B)
    static let borde = Color(claro: 0xC9D4DF, oscuro: 0x334155)
    static let texto = Color(claro: 0x293241, oscuro: 0xF1F5F9)
    static let textoSecundario = Color(claro: 0x5C6B7A, oscuro: 0x94A3B8)
    static let exito = Color(claro: 0x2E7D32, oscuro: 0x16A34A)
    static let aviso = Color(claro: 0xEE6C4D, oscuro: 0xF59E0B)
    static let error = Color(claro: 0xC0392B, oscuro: 0xEF4444)
    static let secundario = Color(claro: 0x2A9D8F, oscuro: 0x2DD4BF)
    static let terciario = Color(claro: 0xEE6C4D, oscuro: 0xFB7185)
    /// Sexto color de la barra de herramientas (grupo Utilidades); fuera de la paleta de `Tokens.xaml` (DT-77).
    static let violeta = Color(claro: 0x6D28D9, oscuro: 0xA78BFA)

    init(claro: UInt32, oscuro: UInt32) {
        func c(_ v: UInt32) -> NSColor {
            NSColor(
                srgbRed: CGFloat(v >> 16 & 0xFF) / 255, green: CGFloat(v >> 8 & 0xFF) / 255, blue: CGFloat(v & 0xFF) / 255,
                alpha: 1)
        }
        self.init(
            nsColor: NSColor(name: nil) { a in a.bestMatch(from: [.aqua, .darkAqua]) == .darkAqua ? c(oscuro) : c(claro) })
    }
}

/// `AppCardStyle`: superficie, borde 1, radio 12, relleno 10×7 y `ThemeShadow`; título `AccentCardTitle` (o
/// `SecondaryCardTitle` con `color: .secundario`) con chip de icono opcional (`IconChipBorderStyle`), sobre una banda
/// tintada de su color como la cabecera del boleto del original (DT-77).
struct Tarjeta<Contenido: View>: View {
    let titulo: String?
    let icono: String?
    let color: Color
    @ViewBuilder var contenido: Contenido
    @Environment(\.colorScheme) private var esquema

    /// Sin `icono`, se elige por el título (`Tarjeta.estilo`); las de salida van en `secundario` (teal).
    init(_ titulo: String? = nil, icono: String? = nil, color: Color? = nil, @ViewBuilder contenido: () -> Contenido) {
        let auto = titulo.flatMap(Self.estilo)
        (self.titulo, self.icono, self.color) = (titulo, icono ?? auto?.icono, color ?? auto?.color ?? .acento)
        self.contenido = contenido()
    }

    /// Icono y color por palabras del título (DT-77): da a las 273 tarjetas el chip de `IconChipBorderStyle` del original.
    static func estilo(_ titulo: String) -> (icono: String, color: Color)? {
        let t = titulo.lowercased()
        let reglas: [([String], String, Color)] = [
            (["resultado", "resumen", "informe", "análisis"], "chart.bar.doc.horizontal", .secundario),
            (["tiempo"], "clock", .secundario), (["estado"], "info.circle", .secundario),
            (["acierto", "premi"], "trophy", .secundario), (["acciones"], "bolt", .acento),
            (["proceso", "cálculo", "calcul"], "gearshape.2", .acento),
            (["salida", "grabar", "guardar"], "square.and.arrow.down", .acento),
            (["fichero", "archivo", "entrada"], "doc.text", .acento),
            (["condicion", "filtro"], "line.3.horizontal.decrease", .acento),
            (["toleranc"], "plusminus", .acento), (["relacion"], "link", .acento),
            (["valoraci", "porcentaje", "reparto", "probab"], "percent", .acento),
            (["columna", "boleto"], "tablecells", .acento), (["ganadora"], "checkmark.seal", .acento),
            (["figura", "dibujo", "simetr", "sándwich", "signo", "formato"], "square.grid.3x3", .acento),
            (["nivel", "límite", "rango", "cantidad", "valores", "número", "nº"], "number", .acento),
            (["partido", "equipo"], "sportscourt", .acento), (["jornada", "temporada"], "calendar", .acento),
            (["grupo"], "square.stack.3d.up", .acento), (["navegación"], "arrow.left.arrow.right", .acento),
            (["leyenda", "secciones", "lista"], "list.bullet", .acento),
            (["opciones", "configuración", "tipo", "modo", "parámetro", "ajuste", "separador"], "slider.horizontal.3", .acento),
            (["utilidad"], "wrench.and.screwdriver", .acento),
        ]
        return reglas.first { $0.0.contains { t.contains($0) } }.map { ($0.1, $0.2) }
    }

    var body: some View {
        VStack(alignment: .leading, spacing: 0) {
            if let titulo {
                HStack(spacing: 8) {
                    if let icono { ChipIcono(simbolo: icono, color: color, lado: 24) }
                    Text(titulo).font(.headline).foregroundStyle(color)
                }
                .padding(.horizontal, 10)
                .padding(.vertical, 5)
                .frame(maxWidth: .infinity, alignment: .leading)
                // Más ancha que la tarjeta y recortada por ella: llega al borde aunque el contenido desborde el marco pedido.
                .background(alignment: .leading) { color.opacity(esquema == .dark ? 0.16 : 0.09).frame(width: 4000) }
                .overlay(alignment: .bottomLeading) { Rectangle().fill(color.opacity(0.3)).frame(width: 4000, height: 1) }
            }
            VStack(alignment: .leading, spacing: 6) { contenido }
                .padding(.horizontal, 10)
                .padding(.vertical, 7)
                .frame(maxWidth: .infinity, alignment: .leading)
        }
        .background(Color.superficie)
        .clipShape(.rect(cornerRadius: 12))
        .overlay(RoundedRectangle(cornerRadius: 12).stroke(Color.borde))
        .shadow(color: .black.opacity(esquema == .dark ? 0.4 : 0.1), radius: 5, y: 2)
    }
}

extension View {
    /// Cabecera propia de una `Tarjeta` sin título (Boleto Base, Combinación): banda `AppSurfaceAlt` hasta los bordes de la
    /// tarjeta, como el encabezado del boleto del original; el relleno negativo deshace el de la tarjeta.
    func bandaTarjeta() -> some View {
        padding(.horizontal, 10)
            .padding(.vertical, 5)
            .frame(maxWidth: .infinity, alignment: .leading)
            .background(Color.superficieAlt)
            .overlay(alignment: .bottom) { Rectangle().fill(Color.borde).frame(height: 1) }
            .padding(.horizontal, -10)
            .padding(.top, -7)
    }
}

/// Chip de icono: símbolo en `color` sobre su tinte (`AppAccentTintBrush`: 12 % claro, 18 % oscuro), radio 6.
struct ChipIcono: View {
    let simbolo: String
    var color: Color = .acento
    var lado: CGFloat = 20
    @Environment(\.colorScheme) private var esquema

    var body: some View {
        Image(systemName: simbolo)
            .font(.system(size: lado * 0.55, weight: .medium))
            .foregroundStyle(color)
            .frame(width: lado, height: lado)
            .background(color.opacity(esquema == .dark ? 0.18 : 0.12), in: RoundedRectangle(cornerRadius: 6))
    }
}

/// Tile de condición de Inicio (`CondicionTemplate` + `CondicionItem.Fondo`): tinte coral tenue si está vacía, verde si
/// está activa y rojo si tiene datos pero no está activa; radio 8. En oscuro el tinte sube ×1,4 para que se vea (DT-77).
struct EstiloCondicion: ButtonStyle {
    let estado: Semaforo
    @Environment(\.colorScheme) private var esquema

    func makeBody(configuration: Configuration) -> some View {
        let (rgb, alfa): (UInt32, Double) =
            switch estado {
            case .neutro: (0xEE6C4D, 0x29)
            case .verde: (0x2E7D32, 0x3D)
            case .rojo: (0xC0392B, 0x3D)
            }
        configuration.label
            .padding(.horizontal, 7)
            .padding(.vertical, 3)
            .frame(maxWidth: .infinity, alignment: .leading)
            .background(
                Color(claro: rgb, oscuro: rgb).opacity(alfa / 255 * (esquema == .dark ? 1.4 : 1) * (configuration.isPressed ? 1.6 : 1)),
                in: .rect(cornerRadius: 8)
            )
            .contentShape(.rect(cornerRadius: 8))
    }
}

/// Acción principal de cada pantalla (`AccentButtonStyle`, ESPEC_15 §1.3): relleno de acento también con la ventana
/// inactiva (el `.borderedProminent` de macOS se vuelve gris y la pantalla parecía en blanco y negro, DT-77).
struct EstiloPrincipal: ButtonStyle {
    @Environment(\.isEnabled) private var activo

    func makeBody(configuration: Configuration) -> some View {
        configuration.label
            .font(.system(size: 13, weight: .semibold))
            .foregroundStyle(Color(claro: 0xFFFFFF, oscuro: 0x0B1120))
            .padding(.horizontal, 14)
            .padding(.vertical, 5)
            .background(Color.acento.opacity(activo ? (configuration.isPressed ? 0.8 : 1) : 0.4), in: .rect(cornerRadius: 8))
            .contentShape(.rect(cornerRadius: 8))
    }
}

/// `SignoToggleButtonStyle`: signo marcado en `SignoMarcado` (verde lleno, texto blanco); sin marcar, neutro con borde.
struct EstiloSigno: ToggleStyle {
    func makeBody(configuration: Configuration) -> some View {
        Button {
            configuration.isOn.toggle()
        } label: {
            configuration.label
                .font(.system(size: 12, weight: .bold))
                .foregroundStyle(configuration.isOn ? Color.white : Color.textoSecundario)
                .frame(width: 28, height: 22)
                .background(configuration.isOn ? Color.exito : Color.superficieAlt, in: .rect(cornerRadius: 6))
                .overlay(RoundedRectangle(cornerRadius: 6).stroke(configuration.isOn ? Color.clear : Color.borde))
        }
        .buttonStyle(.plain)
    }
}

/// `SemaforoControl` (10 §1.9): punto de 14 pt con halo de 22 pt.
struct SemaforoView: View {
    let estado: Semaforo
    /// Diámetro del punto; el halo y el marco escalan con él (los tiles de Inicio usan 10).
    var tamano: CGFloat = 14

    var body: some View {
        let color: Color =
            switch estado {
            case .neutro: .borde
            case .verde: .exito
            case .rojo: .error
            }
        ZStack {
            if estado != .neutro { Circle().fill(color.opacity(0.35)).frame(width: tamano * 22 / 14, height: tamano * 22 / 14) }
            Circle().fill(color.opacity(estado == .neutro ? 0.55 : 1)).frame(width: tamano, height: tamano)
        }
        .frame(width: tamano * 24 / 14, height: tamano * 24 / 14)
        .accessibilityLabel(estado == .neutro ? "Filtro vacío" : estado == .verde ? "Filtro activo" : "Filtro con error")
    }
}

/// Cabecera de página: título de 17 pt semibold (ESPEC_15 §1.2).
struct Pagina<Contenido: View>: View {
    let destino: Destino
    @ViewBuilder var contenido: Contenido

    var body: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: 12) {
                Text(destino.titulo).font(.system(size: 17, weight: .semibold)).foregroundStyle(Color.texto)
                contenido
            }
            .padding(16)
            .frame(maxWidth: .infinity, alignment: .leading)
        }
        .background(Color.fondo)
        .navigationTitle(destino.titulo)
    }
}

/// Campo de texto con etiqueta a la izquierda, como los formularios actuales.
struct CampoTexto: View {
    let etiqueta: String
    @Binding var texto: String
    var ancho: CGFloat? = nil

    var body: some View {
        HStack {
            Text(etiqueta)
            TextField(etiqueta, text: $texto).labelsHidden().frame(width: ancho)
        }
    }
}

/// Selector de fichero: ruta (editable) + «…».
struct CampoFichero: View {
    let etiqueta: String
    @Binding var ruta: String
    var extensiones: [String] = ["*"]
    var guardar: (nombre: String, ext: String)? = nil

    var body: some View {
        HStack {
            Text(etiqueta)
            TextField(etiqueta, text: $ruta).labelsHidden().textFieldStyle(.roundedBorder)
            Button("…") {
                Task {
                    let url: URL? =
                        if let guardar {
                            await SelectorFicheros.guardar(guardar.nombre, [(guardar.nombre, guardar.ext)])
                        } else {
                            await SelectorFicheros.abrirVarios(extensiones, multiple: false).first
                        }
                    if let url { ruta = url.path(percentEncoded: false) }
                }
            }
        }
    }
}

/// `NumberBox` (ESPEC_15 §1.3): cultura del sistema; vacío ⇒ `NaN`; texto no válido ⇒ se queda el valor anterior;
/// fuera de rango ⇒ se ajusta a `minimo…maximo`.
struct CampoNumero: View {
    let etiqueta: String
    @Binding var valor: Double
    var minimo = -Double.infinity
    var maximo = Double.infinity
    var ancho: CGFloat = 110
    @State private var texto = ""
    @FocusState private var foco: Bool

    var body: some View {
        LabeledContent(etiqueta) {
            TextField(etiqueta, text: $texto)
                .labelsHidden()
                .multilineTextAlignment(.trailing)
                .frame(width: ancho)
                .focused($foco)
                .onSubmit(confirmar)
                .onChange(of: foco) { if !foco { confirmar() } }
                .onAppear { texto = Self.mostrar(valor) }
                .onChange(of: valor) { texto = Self.mostrar(valor) }
        }
    }

    static func mostrar(_ v: Double) -> String { v.isNaN ? "" : FormatoNET.toString(v, cultura: .sistema()) }

    private func confirmar() {
        if FormatoNET.recortar(texto).isEmpty {
            valor = .nan
        } else if let v = FormatoNET.tryParseDouble(texto, cultura: .sistema()) {
            valor = min(max(v, minimo), maximo)
        }
        texto = Self.mostrar(valor)
    }
}

/// `PorcentajesControl` (10 §1.9): «Part. / 1 / X / 2», una fila por partido, `NumberBox` ≥ 0 sin máximo; alto máx. 260.
struct PorcentajesGrid: View {
    @Binding var valores: [[Double]]

    var body: some View {
        ScrollView {
            Grid(alignment: .trailing, horizontalSpacing: 6, verticalSpacing: 3) {
                GridRow {
                    Text("Part.")
                    Text("1")
                    Text("X")
                    Text("2")
                }
                .font(.caption).foregroundStyle(Color.textoSecundario)
                ForEach(valores.indices, id: \.self) { i in
                    GridRow {
                        Text("\(i + 1)").monospacedDigit()
                        ForEach(0..<3, id: \.self) { j in
                            CampoNumero(etiqueta: "", valor: $valores[i][j], minimo: 0, ancho: 64).labelsHidden()
                        }
                    }
                }
            }
            .controlSize(.small)
        }
        .frame(maxHeight: 260)
    }
}
