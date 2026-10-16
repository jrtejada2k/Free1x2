import Free1X2CasosDeUso
import SwiftUI

/// «Añade P15 (Indeciso)» (pantalla 1, `AgregaP15Frm`).
struct VistaAgregaP15: View {
    @Environment(Sesion.self) private var sesion
    @State private var entrada: URL?
    @State private var salida: URL?
    @State private var o = OpcionesP15()
    @State private var partidoCondicion = 1.0
    @State private var partidoCopia = 1.0
    @State private var repetidas = 0
    @State private var estado = "Preparado"
    @State private var ocupado = false

    private func signo(_ valor: Binding<String>) -> some View {
        Picker("", selection: valor) {
            Text("—").tag("")
            ForEach(["1", "X", "2"], id: \.self) { Text($0).tag($0) }
        }
        .labelsHidden().frame(width: 64)
    }

    private func calcular() async {
        let error = CasosCombinacion.comprobarP15(
            entrada: entrada?.path(percentEncoded: false) ?? "", salida: salida?.path(percentEncoded: false) ?? "", o)
        guard error.isEmpty, let entrada, let salida else { return estado = error }
        var opciones = o
        opciones.partidoCondicion = Int(FormatoNET.castInt32(partidoCondicion))
        opciones.partidoCopia = Int(FormatoNET.castInt32(partidoCopia))
        ocupado = true
        estado = "Calculando"
        do throws(ErrorDominio) {
            repetidas = try await CasosCombinacion.agregarP15(entrada: entrada, salida: salida, opciones: opciones, entorno: sesion.entorno)
            estado = "Terminado"
        } catch {
            sesion.alertas.error("Error al añadir el P15: " + error.mensajeUsuario)
            estado = "Error"
        }
        ocupado = false
    }

    var body: some View {
        Pagina(destino: .agregaP15) {
            Tarjeta("Ficheros") {
                HStack {
                    Text("Entrada").frame(width: 60, alignment: .leading)
                    Button("Examinar…") { Task { if let u = await SelectorFicheros.abrir(".txt") { entrada = u } } }
                    Text(entrada?.lastPathComponent ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    Text("Salida").frame(width: 60, alignment: .leading)
                    Button("Examinar…") {
                        Task { if let u = await SelectorFicheros.guardar("ColumnasP15", [("Columnas", ".txt")]) { salida = u } }
                    }
                    Text(salida?.lastPathComponent ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Pleno al 15") {
                HStack {
                    Toggle("Partido 15 fijo a", isOn: $o.fijo)
                    signo($o.signoFijo)
                }
                HStack {
                    Toggle("Si partido", isOn: $o.condicional)
                    CampoNumero(etiqueta: "", valor: $partidoCondicion, minimo: 1, maximo: 14, ancho: 50)
                    Text("=")
                    signo($o.signoCondicion)
                    Text("entonces P15 =")
                    signo($o.signoSi)
                    Text("; si no")
                    signo($o.signoNo)
                }
                HStack {
                    Toggle("Elimina partido 15", isOn: $o.eliminar)
                    if o.eliminar { Text("Nº Columnas Repetidas: \(repetidas)").foregroundStyle(Color.textoSecundario) }
                }
                HStack {
                    Toggle("Resultado P15 igual que partido", isOn: $o.copiar)
                    CampoNumero(etiqueta: "", valor: $partidoCopia, minimo: 1, maximo: 14, ancho: 50)
                }
                Text("Los modos se pueden combinar: se aplican en este orden sobre la misma columna.").font(.callout)
                    .foregroundStyle(Color.textoSecundario)
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Calcular") { Task { await calcular() } }.buttonStyle(EstiloPrincipal()).disabled(ocupado)
            }
        }
    }
}
