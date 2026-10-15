import Free1X2CasosDeUso
import SwiftUI

/// «Importar / Exportar columnas» (pantalla 66, `ImportExportFrm`): CSV ↔ TXT.
struct VistaImportExport: View {
    @Environment(Sesion.self) private var sesion
    @State private var aTxt = true
    @State private var entrada = ""
    @State private var salida = ""
    @State private var columnas: [String]?
    @State private var colsEntrada = ""
    @State private var colsSalida = ""
    @State private var estado = ""

    private func abrirEntrada() async {
        guard let u = await SelectorFicheros.abrir(aTxt ? ".csv" : ".txt") else { return }
        (entrada, colsEntrada, estado) = (u.path(percentEncoded: false), "", "")
        do throws(ErrorDominio) {
            let r = try CasosUtilidades.leerParaConvertir(u, aTxt: aTxt, entorno: sesion.entorno)
            columnas = r.columnas
            salida = r.salida.path(percentEncoded: false)
            colsEntrada = "\(r.columnas.count) columnas."
        } catch {
            (columnas, colsEntrada) = (nil, "")
            sesion.alertas.error("No se ha podido leer el fichero de entrada")
        }
    }

    private func hacer() async {
        guard !salida.isEmpty else { return }
        guard let columnas, !columnas.isEmpty else {
            return sesion.alertas.error("No se ha cargado el fichero de entrada o no tiene columnas.")
        }
        guard entrada != salida else { return sesion.alertas.error("Los archivos de entrada y salida deben ser distintos") }
        colsSalida = ""
        let url = URL(filePath: salida)
        if sesion.entorno.archivos.existe(url), !(await sesion.alertas.confirmar("Ya existe el fichero de salida. ¿Sobreescribir?")) {
            return
        }
        do throws(ErrorDominio) {
            let n = try CasosUtilidades.convertirColumnas(columnas, aTxt: aTxt, salida: url, entorno: sesion.entorno)
            colsSalida = "\(n) columnas."
            estado = "Transformación finalizada"
            sesion.alertas.info("Transformación finalizada")
        } catch {
            estado = "Error: " + error.mensajeUsuario
            sesion.alertas.error("No se ha podido guardar el fichero de salida")
        }
    }

    var body: some View {
        Pagina(destino: .importExport) {
            Tarjeta("Tipo de conversión") {
                Picker("", selection: $aTxt) {
                    Text("Separadas por comas (*.csv) a TXT (*.txt)").tag(true)
                    Text("TXT (*.txt) a Separadas por comas (*.csv)").tag(false)
                }
                .labelsHidden().fixedSize()
            }
            Tarjeta("Fichero de entrada") {
                HStack {
                    TextField("Ruta del fichero de entrada", text: $entrada).textFieldStyle(.roundedBorder)
                    Button("Examinar...") { Task { await abrirEntrada() } }
                }
                Text(colsEntrada).foregroundStyle(Color.textoSecundario)
            }
            Tarjeta("Fichero de salida") {
                HStack {
                    TextField("Ruta del fichero de salida", text: $salida).textFieldStyle(.roundedBorder)
                    Button("Examinar...") {
                        Task {
                            if let u = await SelectorFicheros.guardar("Columnas", [("Columnas", aTxt ? ".txt" : ".csv")]) {
                                salida = u.path(percentEncoded: false)
                            }
                        }
                    }
                }
                Text(colsSalida).foregroundStyle(Color.textoSecundario)
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                // «Cancelar» no hace nada en el C# (`VM:184-188`).
                Button("Cancelar") {}
                Button("Hacer") { Task { await hacer() } }.buttonStyle(.borderedProminent)
            }
        }
    }
}

/// «Compresor *.z3q» (pantalla 22, `Compresor`).
struct VistaCompresor: View {
    @Environment(Sesion.self) private var sesion
    @State private var nivel = 5
    @State private var nombreTexto = "(ningún archivo)"
    @State private var nombreComprimido = "(ningún archivo)"
    @State private var estado = "Preparado"

    private func n0(_ v: Int) -> String {
        FormatoNET.toStringMiles(Double(v), decimales: 0, decimalesNegativo: 0, cultura: sesion.entorno.cultura)
    }

    private func comprimir() async {
        guard let entrada = await SelectorFicheros.abrir(".txt", "*") else { return }
        nombreTexto = entrada.lastPathComponent
        let bits: ConjuntoBits
        do throws(ErrorDominio) {
            guard let b = try CasosUtilidades.bitmapDeColumnas(entrada, entorno: sesion.entorno) else {
                nombreTexto = "(ningún archivo)"
                return sesion.alertas.error("Error leyendo columnas")
            }
            bits = b
        } catch {
            return sesion.alertas.error("Error al leer el archivo de columnas: " + error.mensajeUsuario)
        }
        estado = "Guardando"
        guard
            let salida = await SelectorFicheros.guardar(
                entrada.deletingPathExtension().lastPathComponent, [("Columnas Comprimidas", ".z3q")])
        else { return estado = "Preparado" }
        do throws(ErrorDominio) {
            let r = try await CasosUtilidades.comprimirZ3Q(bits, en: salida, nivel: nivel, entorno: sesion.entorno)
            estado =
                "Guardado: \(r.ruta.lastPathComponent) (\(n0(r.datos)) bytes de datos, \(n0(r.fichero)) bytes en disco, nivel \(nivel))."
            sesion.alertas.info(
                "Archivo comprimido correctamente:\n\(r.ruta.path(percentEncoded: false))\n\nDatos: \(n0(r.datos)) bytes — Archivo: \(n0(r.fichero)) bytes."
            )
        } catch {
            estado = "Error al comprimir."
            sesion.alertas.error("Error al comprimir el archivo: " + error.mensajeUsuario)
        }
    }

    private func descomprimir() async {
        guard let entrada = await SelectorFicheros.abrir(".z3q", "*") else { return }
        nombreComprimido = entrada.lastPathComponent
        let bits: ConjuntoBits
        do throws(ErrorDominio) {
            estado = "Descomprimiendo…"
            bits = try await CasosUtilidades.descomprimirZ3Q(entrada, entorno: sesion.entorno)
        } catch {
            nombreComprimido = "(ningún archivo)"
            estado = "Error al descomprimir."
            return sesion.alertas.error("Error al descomprimir el archivo: " + error.mensajeUsuario)
        }
        estado = "Guardando"
        guard let salida = await SelectorFicheros.guardar("Columnas", [("Columnas", ".txt")]) else { return estado = "Preparado" }
        do throws(ErrorDominio) {
            let n = try await CasosUtilidades.columnasDeBitmap(bits, en: salida, entorno: sesion.entorno)
            estado = "Guardado: \(salida.lastPathComponent) (\(n0(n)) columnas)."
            sesion.alertas.info(
                "Archivo descomprimido correctamente:\n\(salida.path(percentEncoded: false))\n\n\(n0(n)) columnas escritas.")
        } catch {
            estado = "Error al guardar las columnas."
            sesion.alertas.error("Error al guardar las columnas descomprimidas: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .compresor) {
            Text("Comprime archivos de columnas (*.txt) al formato propio *.z3q y descomprímelos de vuelta a texto.")
                .foregroundStyle(Color.textoSecundario)
            Tarjeta("Comprimir") {
                HStack {
                    Button("Archivo…") { Task { await comprimir() } }
                    Text(nombreTexto).foregroundStyle(Color.textoSecundario)
                }
                Picker("Nivel de compresión", selection: $nivel) { ForEach(0..<10, id: \.self) { Text(String($0)).tag($0) } }
                    .fixedSize()
            }
            Tarjeta("Descomprimir") {
                HStack {
                    Button("Archivo…") { Task { await descomprimir() } }
                    Text(nombreComprimido).foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Estado") { Text(estado) }
            Button("Cerrar") { sesion.router.atras() }
        }
    }
}
