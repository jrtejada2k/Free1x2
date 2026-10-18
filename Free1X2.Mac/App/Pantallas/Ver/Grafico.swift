import AppKit
import Free1X2CasosDeUso
import SwiftUI

/// «Representación gráfica de la combinación» (pantalla 61, `GraficoColumnasFrm`).
struct VistaGraficoColumnas: View {
    @Environment(Sesion.self) private var sesion
    @State private var fichero: URL?
    @State private var grafico: GraficoCombinacion?
    @State private var guia: [String] = []
    @State private var estado = ""
    @State private var tarea: Task<Void, Never>?

    private func abrir() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        fichero = u
        estado = "Procesando..."
        do throws(ErrorDominio) {
            let g = try await CasosVer.graficoCombinacion(u, entorno: sesion.entorno)
            grafico = g
            estado = "\(g.apuestas) apuestas representadas (mín. \(g.minimo), máx. \(g.maximo))."
        } catch {
            grafico = nil
            estado = "Error al abrir la combinación."
            sesion.alertas.error("No se ha podido representar la combinación: " + error.mensajeUsuario)
        }
    }

    /// Copia nueva del port: imagen al portapapeles y, opcionalmente, PNG.
    @MainActor private func copiar() async {
        guard let grafico else { return estado = "No hay ninguna combinación representada que copiar." }
        let r = ImageRenderer(content: Lienzo(grafico: grafico).background(.white))
        guard let imagen = r.nsImage, let tiff = imagen.tiffRepresentation, let rep = NSBitmapImageRep(data: tiff),
            let png = rep.representation(using: .png, properties: [:])
        else { return sesion.alertas.error("No se ha podido copiar la imagen del gráfico.") }
        NSPasteboard.general.clearContents()
        NSPasteboard.general.writeObjects([imagen])
        estado = "Imagen del gráfico copiada al portapapeles."
        guard let url = await SelectorFicheros.guardar("grafico_columnas", [("Imagen PNG", ".png")], en: URL.picturesDirectory)
        else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(png, en: url, atomico: true)
            estado = "Imagen del gráfico guardada en " + url.lastPathComponent + "."
        } catch {
            sesion.alertas.error("No se ha podido copiar la imagen del gráfico: " + error.mensajeUsuario)
        }
    }

    /// 10 franjas de 959×25 desde (9, 50); rellenos granate; una línea de 1 px por columna.
    struct Lienzo: View {
        let grafico: GraficoCombinacion?

        var body: some View {
            Canvas { ctx, _ in
                for k in 0..<10 {
                    ctx.stroke(Path(CGRect(x: 9, y: 50 + 25 * k, width: 959, height: 25)), with: .color(.borde), lineWidth: 1)
                }
                for r in grafico?.rellenos ?? [] where r.ancho > 0 && r.alto > 0 {
                    ctx.fill(Path(CGRect(x: r.x, y: r.y, width: r.ancho, height: r.alto)), with: .color(Color(red: 0.5, green: 0, blue: 0)))
                }
                var p = Path()
                for l in grafico?.lineas ?? [] {
                    p.move(to: CGPoint(x: l.x, y: l.y0))
                    p.addLine(to: CGPoint(x: l.x, y: l.y1))
                }
                ctx.stroke(p, with: .color(.acento), lineWidth: 1)
            }
            .frame(width: 977, height: 309)
        }
    }

    var body: some View {
        Pagina(destino: .graficoColumnas) {
            HStack {
                Button("Abrir combinación") {
                    tarea = Task {
                        await abrir()
                        tarea = nil
                    }
                }
                .disabled(tarea != nil)
                Text(fichero?.path(percentEncoded: false) ?? "").foregroundStyle(Color.textoSecundario).lineLimit(1)
                Spacer()
                Button("Cancelar") { tarea?.cancel() }.disabled(tarea == nil)
                Button("Copiar imagen") { Task { await copiar() } }.disabled(grafico == nil)
                Button("Guía") { guia = CasosVer.guiaGrafico(minimo: grafico?.minimo ?? 0, maximo: grafico?.maximo ?? 0) }
                    .disabled(grafico == nil)
            }
            Tarjeta("Gráfico") {
                ScrollView(.horizontal) { Lienzo(grafico: grafico) }
                HStack(spacing: 24) {
                    Text("Mínimo: \(grafico?.minimo ?? 0)")
                    Text("Máximo: \(grafico?.maximo ?? 0)")
                }
                .monospacedDigit()
            }
            if !guia.isEmpty {
                Tarjeta("Guía") {
                    Text(guia.joined(separator: "\n")).font(.system(.body, design: .monospaced)).textSelection(.enabled)
                }
            }
            Text(estado).foregroundStyle(Color.textoSecundario)
        }
    }
}
