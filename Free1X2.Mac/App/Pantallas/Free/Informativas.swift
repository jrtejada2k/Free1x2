import AppKit
import SwiftUI

// Páginas sin ViewModel (10 §1.11; plantilla T9): `AcercaDeFrmPage`, `AyudaFrmPage`, `CreditosFrmPage`.

/// Páginas de clubprogol.com que existen (DT-86): las antiguas (`DocWK` wiki, `foros/index.php`, `Descargas.aspx`) dan 404.
enum Web {
    static let sitio = "https://clubprogol.com"
    static let manual = "https://clubprogol.com/docwk/"
    static let faq = "https://clubprogol.com/docwk/faq/"
    static let foro = "https://clubprogol.com/foro/"
}

private func abrirWeb(_ s: String) {
    if let u = URL(string: s) { NSWorkspace.shared.open(u) }
}

/// «Acerca de»: logo, versión del bundle y enlaces (`AcercaDeFrmPage.xaml.cs:19-175`).
struct VistaAcercaDe: View {
    @Environment(Sesion.self) private var sesion

    /// `VersionProducto`: `CFBundleShortVersionString`; si falta, «(desconocida)».
    private var version: String {
        "Versión " + (Bundle.main.infoDictionary?["CFBundleShortVersionString"] as? String ?? "(desconocida)")
    }

    /// Documento local del bundle con la app asociada; si falta o falla, la web (:97-111).
    private func documento(_ nombre: String) {
        let url = sesion.entorno.rutas.recursos.appending(path: "Datos/Documentacion/" + nombre)
        if !FileManager.default.fileExists(atPath: url.path(percentEncoded: false)) || !NSWorkspace.shared.open(url) {
            abrirWeb(Web.sitio)
        }
    }

    var body: some View {
        Pagina(destino: .acercaDe) {
            Text("El primer programa de quinielas libre y gratuito").foregroundStyle(Color.textoSecundario)
            Tarjeta {
                HStack(spacing: 16) {
                    if let logo = NSImage(contentsOf: sesion.entorno.rutas.recursos.appending(path: "logo.jpg")) {
                        Image(nsImage: logo).resizable().frame(width: 110, height: 110)
                    }
                    VStack(alignment: .leading, spacing: 6) {
                        Text("Free1X2").font(.title2.bold())
                        Text(version)
                        Button("www.clubprogol.com") { abrirWeb(Web.sitio) }.buttonStyle(.link)
                    }
                }
            }
            Tarjeta("Enlaces") {
                Button("Información de licencia") { documento("licencia.txt") }.buttonStyle(.link)
                Button("Sobre el software libre") { documento("GPL.txt") }.buttonStyle(.link)
                Button("Manual") { abrirWeb(Web.manual) }.buttonStyle(.link)
                Button("Foro sobre el programa") { abrirWeb(Web.foro) }.buttonStyle(.link)
                Button("Créditos") { sesion.router.navegar(.creditos) }.buttonStyle(.link)
            }
        }
    }
}

/// «Recursos de ayuda» (DT-86, a petición del dueño): solo los enlaces que existen. El original (`AyudaFrmPage.xaml.cs:16-81`)
/// tenía además Artículos, Recursos, Notificaciones y Facebook, que solo avisaban en inglés de que estaban desactivados.
struct VistaAyuda: View {
    var body: some View {
        Pagina(destino: .ayuda) {
            Text("Documentación, comunidad y enlaces de soporte de Free1X2").foregroundStyle(Color.textoSecundario)
            Tarjeta("Documentación") {
                Button("Manual") { abrirWeb(Web.manual) }.buttonStyle(.link)
                Button("Preguntas frecuentes (FAQ)") { abrirWeb(Web.faq) }.buttonStyle(.link)
            }
            Tarjeta("Comunidad") {
                Button("Foro de Free1X2") { abrirWeb(Web.foro) }.buttonStyle(.link)
            }
        }
    }
}

/// «Créditos» (DT-89, a petición del dueño): la app Mac la escribió solo él; queda el aviso de origen que pide la GPLv3.
struct VistaCreditos: View {
    @Environment(Sesion.self) private var sesion

    private func t(_ es: String, _ en: String) -> String { sesion.idioma == "Ingles" ? en : es }

    var body: some View {
        Pagina(destino: .creditos) {
            Tarjeta(t("Autor", "Author")) {
                HStack {
                    Text("jrtejada2k").bold()
                    Button("elrevisor2k@gmail.com") { abrirWeb("mailto:elrevisor2k@gmail.com") }.buttonStyle(.link)
                        .accessibilityLabel(t("Contactar con jrtejada2k", "Contact jrtejada2k"))
                }
                Button("www.clubprogol.com") { abrirWeb(Web.sitio) }.buttonStyle(.link)
                    .accessibilityLabel(t("Visitar el sitio web clubprogol.com", "Visit the clubprogol.com website"))
            }
            Tarjeta(t("Licencia", "License")) {
                Text(
                    t(
                        "Programa libre bajo la licencia GPLv3, basado en Free1X2 de Joan Duatis.",
                        "Free software under the GPLv3 license, based on Free1X2 by Joan Duatis."))
            }
        }
    }
}
