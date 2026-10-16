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
                Button("Hacer") { Task { await hacer() } }.buttonStyle(EstiloPrincipal())
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

/// «Multiplicador» (pantalla 74, `MultiplicadorFrm`): producto de dos ficheros de columnas con plantilla de 28 cifras.
struct VistaMultiplicador: View {
    @Environment(Sesion.self) private var sesion
    @State private var entradas: [[String]] = [[], []]
    @State private var nombres = ["(selecciona)", "(selecciona)"]
    @State private var cuentas = ["0", "0"]
    @State private var plantilla: [Double] = [1, 2, 3, 4, 5, 15, 16, 17, 18, 19, 20, 21, 13, 13]
    @State private var resultado: [String] = []
    @State private var columnasResultado = "0"
    @State private var mensaje = ""
    @State private var ocupado = false

    private func seleccionar(_ i: Int) async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        entradas[i] = []
        do throws(ErrorDominio) {
            entradas[i] = try CasosUtilidades.columnasMultiplicador(u, entorno: sesion.entorno)
            nombres[i] = u.lastPathComponent
            cuentas[i] = String(entradas[i].count)
        } catch {
            sesion.alertas.inesperado(error)
        }
    }

    private func multiplicar() async {
        guard let p = CasosUtilidades.plantillaMultiplicador(plantilla) else {
            mensaje = "error en plantilla"
            return sesion.alertas.error("error en plantilla")
        }
        mensaje = ""
        ocupado = true
        do throws(ErrorDominio) {
            resultado = try await CasosUtilidades.multiplicarColumnas(entradas[0], entradas[1], plantilla: p)
            columnasResultado = String(resultado.count)
        } catch {
            mensaje = "Error: " + error.mensajeUsuario
        }
        ocupado = false
    }

    private func grabar() async {
        guard let url = await SelectorFicheros.guardar("Resultado", [("Fichero resultados", ".txt")]) else { return }
        ocupado = true
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(TextoNET.codificarLineas(resultado), en: url, atomico: false)
            mensaje = "Resultado grabado."
        } catch {
            mensaje = "Error: " + error.mensajeUsuario
        }
        ocupado = false
    }

    private func entrada(_ i: Int, _ titulo: String, _ detalle: String) -> some View {
        Tarjeta(titulo) {
            Text(detalle).foregroundStyle(Color.textoSecundario)
            HStack {
                Button("Seleccionar archivo…") { Task { await seleccionar(i) } }.disabled(ocupado)
                Text(nombres[i]).foregroundStyle(Color.textoSecundario)
            }
            Text("Columnas: " + cuentas[i])
        }
    }

    var body: some View {
        Pagina(destino: .multiplicador) {
            Text(
                "Multiplica (producto cartesiano) dos combinaciones de 14 signos en columnas de 28 cifras, reordenadas según una plantilla."
            )
            .foregroundStyle(Color.textoSecundario)
            HStack(alignment: .top) {
                entrada(0, "Entrada Comb-1", "Columnas numeradas de 1 a 14.")
                entrada(1, "Entrada Comb-2", "Columnas numeradas de 15 a 28.")
            }
            Tarjeta("Plantilla de estructura") {
                Text(
                    "Plantilla de estructura de las columnas resultantes (cifras de ejemplo). El cambio de orden de estas cifras (1 a 28) permite transponer columnas al mismo tiempo."
                )
                .foregroundStyle(Color.textoSecundario)
                Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 6) {
                    ForEach(0..<2, id: \.self) { f in
                        GridRow {
                            ForEach(0..<7, id: \.self) { c in
                                CampoNumero(etiqueta: String(f * 7 + c + 1), valor: $plantilla[f * 7 + c], minimo: 1, maximo: 28, ancho: 44)
                            }
                        }
                    }
                }
            }
            Tarjeta("Resultado") {
                Text("Columnas resultantes: " + columnasResultado)
                if !mensaje.isEmpty { Text(mensaje).foregroundStyle(mensaje.hasPrefix("Error") ? Color.error : Color.textoSecundario) }
                HStack {
                    Button("Grabar resultado") { Task { await grabar() } }.disabled(ocupado || resultado.isEmpty)
                    Button("Multiplicar") { Task { await multiplicar() } }.buttonStyle(EstiloPrincipal()).disabled(ocupado)
                }
            }
        }
    }
}

/// «Fraccionador» (pantalla 53, `FraccionadorFrm`): reparte un fichero de columnas en `BASE01.txt`, `BASE02.txt`…
struct VistaFraccionador: View {
    @Environment(Sesion.self) private var sesion
    @State private var columnas: [String] = []
    @State private var nombre = ""
    @State private var cargada = false
    @State private var porColumnas = true
    @State private var cuotas = Array(repeating: 0.0, count: 20)
    @State private var tramos = 0.0
    @State private var archivos = 0
    @State private var tiempo = ""

    private func lapso(_ desde: Date) -> String {
        String((FormatoNET.toStringTimeSpan(sesion.entorno.reloj.ahora().timeIntervalSince(desde)) + "0000000000").prefix(10))
    }

    private func entrada() async {
        guard let u = await SelectorFicheros.abrir(".txt") else { return }
        let t0 = sesion.entorno.reloj.ahora()
        do throws(ErrorDominio) {
            columnas = try CasosUtilidades.lineasFraccionador(u, entorno: sesion.entorno)
            (nombre, cargada, tiempo) = (u.lastPathComponent, true, lapso(t0))
        } catch {
            sesion.alertas.error("Error al leer el archivo: " + error.mensajeUsuario)
        }
    }

    private func fraccionar() async {
        guard let u = await SelectorFicheros.guardar("BASE", [("Nombre BASE salida", ".txt")]) else { return }
        let t0 = sesion.entorno.reloj.ahora()
        do throws(ErrorDominio) {
            archivos = try await CasosUtilidades.fraccionar(
                columnas, base: u.deletingPathExtension(), cuotas: porColumnas ? cuotas.map { Int(FormatoNET.castInt32($0)) } : nil,
                tramos: Int(FormatoNET.castInt32(tramos)), entorno: sesion.entorno)
            tiempo = lapso(t0)
        } catch {
            sesion.alertas.error("Error al fraccionar: " + error.mensajeUsuario)
        }
    }

    var body: some View {
        Pagina(destino: .fraccionador) {
            Text(
                "Carga un archivo de columnas (*.txt) y divídelo en varios archivos de salida numerados, ya sea por cuotas fijas por columnas o en tramos iguales."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Archivo de entrada") {
                HStack {
                    Button("Entrada…") { Task { await entrada() } }
                    Text(nombre).foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Fraccionar por") {
                Picker("", selection: $porColumnas) {
                    Text("Columnas").tag(true)
                    Text("Tramos").tag(false)
                }
                .pickerStyle(.radioGroup).horizontalRadioGroupLayout().labelsHidden()
                CampoNumero(etiqueta: "¿Cuántos tramos?", valor: $tramos, minimo: 0, ancho: 70).disabled(porColumnas)
            }
            Tarjeta("Por columnas") {
                Text(
                    "Cantidad de columnas que recibe cada archivo de salida (1 a 20). El reparto se detiene en la primera cuota igual a 0."
                )
                .foregroundStyle(Color.textoSecundario)
                Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 6) {
                    ForEach(0..<4, id: \.self) { f in
                        GridRow {
                            ForEach(0..<5, id: \.self) { c in
                                CampoNumero(etiqueta: String(f * 5 + c + 1), valor: $cuotas[f * 5 + c], minimo: 0, ancho: 70)
                            }
                        }
                    }
                }
                .disabled(!porColumnas)
            }
            Tarjeta("Resultado") {
                Text("Columnas: \(columnas.count)")
                Text("Archivos: \(archivos)")
                Text("Tiempo: " + tiempo)
            }
            HStack {
                // «Cerrar» no hace nada en el C# (`VM:228-231`).
                Button("Cerrar") {}
                Button("Fraccionar") { Task { await fraccionar() } }.buttonStyle(EstiloPrincipal()).disabled(!cargada)
            }
        }
    }
}
