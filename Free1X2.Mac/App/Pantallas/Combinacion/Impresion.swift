import AppKit
import Free1X2CasosDeUso
import SwiftUI

/// «Impresión de boletos» (pantalla 68, `ImprimirBoletoFrm`). Como el port, no imprime: genera la imagen.
struct VistaImprimirBoleto: View {
    @Environment(Sesion.self) private var sesion
    @State private var margenSuperior = 100.0
    @State private var margenIzquierdo = 30.0
    @State private var girar = false
    @State private var impresora = "(ninguna)"
    @State private var fichero = "-"
    @State private var columnas: [String] = []
    @State private var desde = 1.0
    @State private var hasta = 1.0
    @State private var iniciada = false

    private var config: ArchivoConfigImpresion.Config {
        .init(margenSuperior: margenSuperior, margenIzquierdo: margenIzquierdo, modelo: impresora, girar: girar)
    }

    private func aplicar(_ c: ArchivoConfigImpresion.Config) {
        (margenSuperior, margenIzquierdo, impresora, girar) = (c.margenSuperior, c.margenIzquierdo, c.modelo, c.girar)
    }

    private func leerColumnas() async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        (fichero, columnas) = ("-", [])
        do throws(ErrorDominio) {
            columnas = try CasosCombinacion.columnasDeFichero(u, entorno: sesion.entorno)
            fichero = u.lastPathComponent
            (desde, hasta) = (1, Double(max(1, (columnas.count - 1) / 8 + 1)))
        } catch {
            sesion.alertas.error("No se ha podido leer el fichero de columnas: " + error.mensajeUsuario)
        }
    }

    /// Boletos apilados (380 de alto, margen 12) como en el `Canvas` del port.
    struct Lienzo: View {
        let boletos: [BoletoImpresion]

        var ancho: Double { (boletos.flatMap(\.marcas).map { 12 + $0.x + $0.ancho }.max() ?? 0) + 12 }

        var body: some View {
            Canvas { ctx, _ in
                for (i, b) in boletos.enumerated() {
                    let y0 = 12 + Double(i) * 380
                    ctx.draw(
                        Text(b.encabezado).font(.system(size: 14, design: .monospaced)).foregroundStyle(.black),
                        at: CGPoint(x: 12, y: y0), anchor: .topLeading)
                    for m in b.marcas {
                        ctx.fill(Path(CGRect(x: 12 + m.x, y: y0 + m.y, width: m.ancho, height: m.alto)), with: .color(.black))
                    }
                }
            }
            .frame(width: max(ancho, 1), height: 12 + Double(boletos.count) * 380)
            .background(.white)
        }
    }

    @MainActor private func imprimir() async {
        guard !columnas.isEmpty else { return sesion.alertas.error("No se ha seleccionado el archivo") }
        let b = CasosCombinacion.boletosImpresion(
            columnas, fichero: fichero, margenSuperior: margenSuperior, margenIzquierdo: margenIzquierdo,
            desde: Int(FormatoNET.castInt32(desde)), hasta: Int(FormatoNET.castInt32(hasta)))
        guard !b.isEmpty else { return sesion.alertas.error("No hay columnas en el rango de boletos seleccionado.") }
        let r = ImageRenderer(content: Lienzo(boletos: b))
        guard let imagen = r.nsImage, let tiff = imagen.tiffRepresentation, let rep = NSBitmapImageRep(data: tiff),
            let png = rep.representation(using: .png, properties: [:])
        else { return sesion.alertas.error("No se ha podido generar la imagen del boleto.") }
        NSPasteboard.general.clearContents()
        NSPasteboard.general.writeObjects([imagen])
        guard let url = await SelectorFicheros.guardar("boleto", [("Imagen PNG", ".png")], en: URL.picturesDirectory) else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(png, en: url, atomico: true)
        } catch {
            sesion.alertas.error("No se ha podido exportar el boleto: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .imprimirBoleto) {
            Tarjeta("Configuración") {
                CampoNumero(etiqueta: "Margen superior", valor: $margenSuperior)
                CampoNumero(etiqueta: "Margen izquierdo", valor: $margenIzquierdo)
                Toggle("Girar boleto", isOn: $girar).toggleStyle(.switch)
                LabeledContent("Impresora seleccionada", value: impresora)
                HStack {
                    Button("Guardar configuración") {
                        do throws(ErrorDominio) {
                            try CasosCombinacion.guardarConfigImpresion(config, entorno: sesion.entorno)
                            sesion.alertas.info("Configuración guardada")
                        } catch {
                            sesion.alertas.error("No se ha podido guardar la configuración: " + error.mensajeUsuario)
                        }
                    }
                    Button("Recuperar configuración") {
                        guard let c = CasosCombinacion.leerConfigImpresion(previo: config, entorno: sesion.entorno) else {
                            return sesion.alertas.error("No se ha podido leer la configuración guardada")
                        }
                        aplicar(c)
                    }
                    Button("Impresoras conocidas") { sesion.router.navegar(.listaImpresoras) }
                }
            }
            Tarjeta("Columnas") {
                HStack {
                    Button("Leer columnas") { Task { await leerColumnas() } }
                    Text(fichero).foregroundStyle(Color.textoSecundario)
                    Text("\(columnas.count) columnas").monospacedDigit().foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    CampoNumero(etiqueta: "Desde el boleto", valor: $desde, minimo: 1, ancho: 60)
                    CampoNumero(etiqueta: "Hasta el boleto", valor: $hasta, minimo: 1, ancho: 60)
                }
            }
            HStack {
                Spacer()
                Button("Imprimir / Guardar como imagen") { Task { await imprimir() } }.buttonStyle(EstiloPrincipal())
            }
        }
        .onAppear {
            if !iniciada {
                iniciada = true
                if let c = CasosCombinacion.leerConfigImpresion(previo: config, entorno: sesion.entorno) { aplicar(c) }
            }
            if let i = sesion.impresoraSeleccionada {
                sesion.impresoraSeleccionada = nil
                (impresora, margenSuperior, margenIzquierdo, girar) = (
                    i.modelo, Double(i.margenSuperior), Double(i.margenIzquierda), i.rotar
                )
            }
        }
    }
}

/// «Lista de impresoras conocidas» (pantalla 70, `ListaImpresoras`).
struct VistaListaImpresoras: View {
    @Environment(Sesion.self) private var sesion
    @State private var impresoras: [ArchivoImpresoras.Impresora] = []
    @State private var seleccion: String?
    @State private var mensaje = "Doble clic sobre una impresora de la lista"

    var body: some View {
        Pagina(destino: .listaImpresoras) {
            Text("Doble clic sobre una impresora de la lista para aplicar su configuración.").foregroundStyle(Color.textoSecundario)
            Tarjeta("Impresoras") {
                List(impresoras, id: \.modelo, selection: $seleccion) { Text($0.modelo).tag($0.modelo) }
                    .frame(minHeight: 240)
                    .contextMenu(forSelectionType: String.self) { _ in
                    } primaryAction: { s in
                        guard let m = s.first, let i = CasosCombinacion.buscarImpresora(m, en: impresoras) else { return }
                        sesion.impresoraSeleccionada = i
                        sesion.router.atras()
                    }
                    .onChange(of: seleccion) {
                        guard !impresoras.isEmpty else { return }
                        mensaje = seleccion.map { "Impresora seleccionada: " + $0 } ?? "Doble clic sobre una impresora de la lista"
                    }
            }
            Text(mensaje).foregroundStyle(Color.textoSecundario)
        }
        .onAppear {
            do throws(ErrorDominio) {
                impresoras = try CasosCombinacion.impresorasConocidas(entorno: sesion.entorno)
                if impresoras.isEmpty { mensaje = "No hay impresoras" }
            } catch {
                mensaje = "No se han encontrado impresoras"
            }
        }
    }
}
