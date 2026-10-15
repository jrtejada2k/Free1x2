import AppKit
import Free1X2CasosDeUso
import SwiftUI

/// «Ver Boletos» (pantalla 99, `VerBoletos`): fichero y orden; «Ok» abre «Boleto» (el visor embebido no llega a verse).
struct VistaVerBoletos: View {
    @Environment(Sesion.self) private var sesion
    @State private var fichero: URL?
    @State private var orden = OrdenBoletos.signo
    @State private var descendente = false
    @State private var estado = "Seleccione un fichero de columnas"

    var body: some View {
        Pagina(destino: .verBoletos) {
            Text("Abre un fichero de columnas y muestra el boleto, eligiendo el criterio y el sentido de ordenación.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta("Fichero de columnas") {
                HStack {
                    Button("Seleccionar archivo...") {
                        Task {
                            guard let u = await SelectorFicheros.abrir(".txt", ".cols", "*") else { return }
                            fichero = u
                            estado = "Fichero seleccionado"
                        }
                    }
                    Text(fichero?.path(percentEncoded: false) ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                }
            }
            HStack(alignment: .top) {
                Tarjeta("Ordenar por ...") {
                    Picker("", selection: $orden) {
                        Text("No ordenar").tag(OrdenBoletos.signo)
                        Text("Variantes").tag(OrdenBoletos.variantes)
                        Text("X").tag(OrdenBoletos.equis)
                        Text("2").tag(OrdenBoletos.doses)
                        Text("Interrupciones").tag(OrdenBoletos.interrupciones)
                    }
                    .pickerStyle(.radioGroup).labelsHidden()
                    Toggle("Signos seguidos", isOn: .constant(false)).disabled(true)
                }
                Tarjeta("Tipo de ordenamiento") {
                    Picker("", selection: $descendente) {
                        Text("ascendente").tag(false)
                        Text("descendente").tag(true)
                    }
                    .pickerStyle(.radioGroup).labelsHidden()
                }
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Cancelar") { sesion.router.atras() }
                Button("Ok") {
                    guard let fichero else { return estado = "Seleccione un fichero de columnas" }
                    estado = "Mostrando boleto"
                    sesion.parametrosBoleto = (fichero, orden, descendente)
                    sesion.router.navegar(.boleto)
                }
                .buttonStyle(.borderedProminent).keyboardShortcut(.defaultAction).disabled(fichero == nil)
            }
        }
    }
}

/// «Boleto» (pantalla 11, `BoletoFrm`): columnas de un fichero de 8 en 8 sobre el boleto oficial.
struct VistaBoleto: View {
    @Environment(Sesion.self) private var sesion
    @State private var fichero: URL?
    @State private var orden = OrdenBoletos.signo
    @State private var descendente = false
    @State private var boletos: BoletosPreparados?
    @State private var actual = 0
    @State private var irA = 1.0
    @State private var estado = "Selecciona un archivo de combinación para ver sus boletos."
    @State private var cargado = false

    private var total: Int { boletos?.total ?? 1 }

    private func cargar() {
        guard let fichero else { return estado = "No se ha seleccionado ningún archivo." }
        estado = "Cargando boletos..."
        do throws(ErrorDominio) {
            boletos = try CasosVer.prepararBoletos(fichero, orden: orden, descendente: descendente, entorno: sesion.entorno)
            llenar(0)
            estado = "\(fichero.lastPathComponent): \(total) boleto(s)."
        } catch {
            estado = "Error al cargar el boleto: " + error.mensajeUsuario
            sesion.alertas.error("No se pudo cargar el boleto: " + error.mensajeUsuario)
        }
    }

    private func llenar(_ k: Int) {
        actual = boletos.map { CasosVer.boleto($0, k).indice } ?? 0
        irA = Double(actual + 1)
    }

    var body: some View {
        Pagina(destino: .boleto) {
            Text("Visor de boletos imprimibles de un archivo de combinación. Abre el archivo y navega entre los boletos que contiene.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta("Archivo de combinación") {
                HStack {
                    Button("Abrir archivo") { Task { if let u = await SelectorFicheros.abrir(".txt") { fichero = u } } }
                    Button("Cargar boletos", action: cargar)
                    Text(fichero?.path(percentEncoded: false) ?? "").foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Navegación") {
                HStack {
                    Button("|<") { llenar(0) }
                    Button("<") { llenar(actual - 1) }
                    Text("Boleto nº")
                    CampoNumero(etiqueta: "", valor: $irA, minimo: 1, ancho: 60).labelsHidden()
                    Button("Ir") {
                        let d = Int(FormatoNET.castInt32(irA))
                        guard d >= 1, d <= total else { return estado = "El número de boleto está fuera de rango." }
                        llenar(d - 1)
                    }
                    Text("de \(total)")
                    Button(">") { llenar(actual + 1) }
                    Button(">|") { llenar(total - 1) }
                }
            }
            Tarjeta("Boleto actual") {
                let b = boletos.map { CasosVer.boleto($0, actual) }
                BoletoMatrizView(signos: b?.signos ?? [], numeros: b?.numeros ?? [])
            }
            Text(estado).foregroundStyle(Color.textoSecundario)
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            guard let p = sesion.parametrosBoleto else { return }
            sesion.parametrosBoleto = nil
            (fichero, orden, descendente) = (p.fichero, p.orden, p.descendente)
            cargar()
        }
    }
}

/// «Ver Boletos En Editor de Texto» (pantalla 100, `VerBoletosEnEditorFrm`): boletos verticales en texto, exportables
/// como imagen (portapapeles y PNG).
struct VistaVerBoletosEnEditor: View {
    @Environment(Sesion.self) private var sesion
    @State private var texto = ""

    private func cargar() async {
        guard let url = await SelectorFicheros.abrir(".txt", "*") else { return }
        do throws(ErrorDominio) {
            let cols = try CasosCombinacion.columnasDeFichero(url, entorno: sesion.entorno)
            texto = try CasosVer.boletosEnTexto(cols, separador: sesion.configuracion.boleto.separador)
        } catch {
            sesion.alertas.error("No se ha podido leer el fichero de columnas: " + error.mensajeUsuario)
        }
    }

    @MainActor private func exportar() async {
        guard !texto.isEmpty else { return }
        let r = ImageRenderer(
            content: Text(texto).font(.system(size: 13, design: .monospaced)).foregroundStyle(.black).padding(4).background(.white))
        guard let imagen = r.nsImage, let tiff = imagen.tiffRepresentation, let rep = NSBitmapImageRep(data: tiff),
            let png = rep.representation(using: .png, properties: [:])
        else { return sesion.alertas.error("No se ha podido generar la imagen de los boletos.") }
        NSPasteboard.general.clearContents()
        NSPasteboard.general.writeObjects([imagen])
        guard let url = await SelectorFicheros.guardar("boletos", [("Imagen PNG", ".png")], en: URL.picturesDirectory) else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(png, en: url, atomico: true)
        } catch {
            sesion.alertas.error("No se ha podido exportar los boletos: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .verBoletosEnEditor) {
            Text("Columnas de apuesta formateadas en boletos verticales.").foregroundStyle(Color.textoSecundario)
            HStack {
                Button("Cargar columnas…") { Task { await cargar() } }
                Button("Imprimir / Exportar imagen") { Task { await exportar() } }.disabled(texto.isEmpty)
            }
            Tarjeta("Boletos") {
                ScrollView([.horizontal, .vertical]) {
                    Text(texto).font(.system(.body, design: .monospaced)).textSelection(.enabled).frame(
                        maxWidth: .infinity, alignment: .leading)
                }
                .frame(minHeight: 360)
            }
        }
    }
}
