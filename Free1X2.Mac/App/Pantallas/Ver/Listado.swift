import AppKit
import Free1X2CasosDeUso
import SwiftUI

/// «Listado de condiciones» (pantalla 71, `ListadoCondicionesFrm`).
struct VistaListadoCondiciones: View {
    @Environment(Sesion.self) private var sesion
    @State private var raiz: NodoListado?
    @State private var abiertos: Set<Int> = []

    private func todos(_ n: NodoListado) -> [Int] { [n.id] + (n.hijos ?? []).flatMap(todos) }

    private func rama(_ n: NodoListado) -> AnyView {
        guard let hijos = n.hijos else { return AnyView(Text(n.texto).textSelection(.enabled)) }
        let abierto = Binding(
            get: { abiertos.contains(n.id) }, set: { if $0 { abiertos.insert(n.id) } else { abiertos.remove(n.id) } })
        return AnyView(
            DisclosureGroup(isExpanded: abierto) {
                ForEach(hijos) { rama($0) }
            } label: {
                Text(n.texto)
            })
    }

    /// `GuardarYAbrirAsync`: UTF-8 sin BOM (DEC-935) y lo abre con la aplicación asociada.
    private func exportar(_ texto: String, _ ext: String) async {
        guard let u = await SelectorFicheros.guardar("Listados", [("Listados", "." + ext)]) else { return }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(Data(texto.utf8), en: u, atomico: true)
            NSWorkspace.shared.open(u)
        } catch {
            sesion.alertas.error("No se pudo exportar: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .listadoCondiciones) {
            HStack {
                Button("Expandir Todo") { abiertos = Set(raiz.map(todos) ?? []) }
                Button("Colapsar Todo") { abiertos = [] }
                Button("Exportar a Texto") { if let raiz { Task { await exportar(CasosVer.listadoTexto(raiz), "txt") } } }
                Button("Exportar a Html") { if let raiz { Task { await exportar(CasosVer.listadoHtml(raiz), "html") } } }
            }
            Tarjeta("Condiciones") {
                if let raiz { rama(raiz) }
            }
        }
        .onAppear {
            let r = CasosVer.listadoCondiciones(
                sesion.combinacion, archivoFiltro: sesion.archivoFiltroCols, cultura: sesion.entorno.cultura)
            raiz = r
            abiertos = [r.id]
        }
    }
}
