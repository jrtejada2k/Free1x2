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

/// `AppCardStyle`: superficie, borde 1, radio 12, relleno 10×7.
struct Tarjeta<Contenido: View>: View {
    let titulo: String?
    @ViewBuilder var contenido: Contenido

    init(_ titulo: String? = nil, @ViewBuilder contenido: () -> Contenido) {
        self.titulo = titulo
        self.contenido = contenido()
    }

    var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            if let titulo { Text(titulo).font(.headline).foregroundStyle(Color.acento) }
            contenido
        }
        .padding(.horizontal, 10)
        .padding(.vertical, 7)
        .frame(maxWidth: .infinity, alignment: .leading)
        .background(Color.superficie, in: RoundedRectangle(cornerRadius: 12))
        .overlay(RoundedRectangle(cornerRadius: 12).stroke(Color.borde))
    }
}

/// `SemaforoControl` (10 §1.9): punto de 14 pt con halo de 22 pt.
struct SemaforoView: View {
    let estado: Semaforo

    var body: some View {
        let color: Color =
            switch estado {
            case .neutro: .borde
            case .verde: .exito
            case .rojo: .error
            }
        ZStack {
            if estado != .neutro { Circle().fill(color.opacity(0.35)).frame(width: 22, height: 22) }
            Circle().fill(color.opacity(estado == .neutro ? 0.55 : 1)).frame(width: 14, height: 14)
        }
        .frame(width: 24, height: 24)
        .accessibilityLabel(estado == .neutro ? "Filtro vacío" : estado == .verde ? "Filtro activo" : "Filtro con error")
    }
}

/// Cabecera de página: título de 17 pt semibold (ESPEC_15 §1.2).
struct Pagina<Contenido: View>: View {
    let destino: Destino
    @ViewBuilder var contenido: Contenido

    var body: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: 12) { contenido }
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
