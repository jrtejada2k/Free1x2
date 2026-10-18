import Free1X2CasosDeUso
import SwiftUI

/// «Dependencia lineal» (pantalla 54, `FrmDependenciaLineal`).
struct VistaDependenciaLineal: View {
    struct Fila {
        var tratar = false
        var coeficiente = 0.0
        var signos = [false, false, false]
    }

    @Environment(Sesion.self) private var sesion
    @State private var filas = Array(repeating: Fila(), count: 14)
    @State private var entrada: URL?
    @State private var salida: URL?
    @State private var bits = ConjuntoBits(cantidad: 4_782_969)
    @State private var estado = "Falta seleccionar fichero de columnas"
    @State private var puedeAceptar = false
    @State private var ocupado = false

    private func tratar(_ p: Int) {
        for i in filas.indices {
            filas[i].tratar = i == p
            filas[i].signos = [i == p, i == p, i == p]
        }
    }

    private func seleccionarEntrada() async {
        guard let u = await SelectorFicheros.abrir(".txt", "*") else { return }
        entrada = u
        estado = "Leyendo columnas..."
        let l: (bits: ConjuntoBits, porcentajes: [[Double]])
        do throws(ErrorDominio) {
            l = try await CasosUtilidades.leerDependencia(u, entorno: sesion.entorno)
        } catch {
            bits = ConjuntoBits(cantidad: 4_782_969)
            estado = "Error al leer las columnas: " + error.mensajeUsuario
            return sesion.alertas.error(estado)
        }
        bits = l.bits
        let p = CasosUtilidades.proponerCoeficientes(l.porcentajes)
        for i in filas.indices { filas[i].coeficiente = Double(p.coeficientes[i]) }
        tratar(p.partido)
        if salida == nil { salida = u }
        estado = "Seleccionar partido e indicar coeficientes"
        puedeAceptar = true
    }

    private func aceptar() async {
        guard let ata = filas.firstIndex(where: \.tratar) else { return estado = "Seleccione el partido a tratar" }
        let s = filas[ata].signos
        guard let q = CasosUtilidades.parametrosDependencia(s[0], s[1], s[2]), let salida else {
            return estado = "Debe poner un triple o un doble en el partido a tratar"
        }
        let coef = filas.map { min(2, max(0, Int(FormatoNET.castInt32($0.coeficiente)))) }
        estado = "Calculando columnas..."
        ocupado = true
        defer { ocupado = false }
        do throws(ErrorDominio) {
            let n = try await CasosUtilidades.aplicarDependencia(
                bits, partido: ata, coeficientes: coef, parametros: q, salida: salida, numeroPartidos: sesion.numeroPartidos,
                entorno: sesion.entorno)
            estado = "Se han grabado \(n) columnas"
        } catch {
            estado = "Error al calcular/grabar las columnas: " + error.mensajeUsuario
            return sesion.alertas.error(estado)
        }
        // Como el C#: lo siguiente opera sobre el fichero grabado.
        bits = (try? await CasosUtilidades.leerDependencia(salida, entorno: sesion.entorno).bits) ?? ConjuntoBits(cantidad: 4_782_969)
    }

    var body: some View {
        Pagina(destino: .dependenciaLineal) {
            Tarjeta("Ficheros") {
                HStack {
                    Text("Entrada").frame(width: 60, alignment: .leading)
                    Button("Seleccionar…") { Task { await seleccionarEntrada() } }
                    Text(entrada?.lastPathComponent ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                }
                HStack {
                    Text("Salida").frame(width: 60, alignment: .leading)
                    Button("Seleccionar…") {
                        Task {
                            if let u = await SelectorFicheros.guardar("columnas", [("Columnas", ".txt"), ("Todos los archivos", ".")]) {
                                salida = u
                            }
                        }
                    }
                    Text(salida?.lastPathComponent ?? "(falta selección)").foregroundStyle(Color.textoSecundario)
                }
            }
            Tarjeta("Partidos") {
                Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 4) {
                    GridRow {
                        Text("")
                        Text("1").bold()
                        Text("X").bold()
                        Text("2").bold()
                        Text("Coeficiente").bold()
                        Text("")
                    }
                    ForEach(filas.indices, id: \.self) { i in
                        GridRow {
                            Text("P\(i + 1)").monospacedDigit()
                            ForEach(0..<3, id: \.self) { s in
                                Toggle("", isOn: $filas[i].signos[s]).labelsHidden().disabled(!filas[i].tratar)
                            }
                            CampoNumero(etiqueta: "", valor: $filas[i].coeficiente, minimo: 0, maximo: 2, ancho: 50)
                            Button("Tratar este") {
                                tratar(i)
                                estado = "Seleccionar partido e indicar coeficientes"
                            }
                        }
                    }
                }
            }
            HStack {
                Text(estado).foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Cancelar") { sesion.router.atras() }
                Button("Aceptar") { Task { await aceptar() } }.buttonStyle(EstiloPrincipal()).disabled(!puedeAceptar || ocupado)
            }
        }
    }
}

/// «Reducciones perfectas» (pantalla 55, `FrmReducidasPerfectas`).
struct VistaReducidasPerfectas: View {
    @Environment(Sesion.self) private var sesion
    @State private var filas = Array(repeating: FilaReducidaPerfecta(), count: 15)
    @State private var longitud = 15.0
    @State private var salida: URL?
    @State private var estado = "Indique el archivo de salida y pulse Generar"
    @State private var estadoSalida = ""

    private func generar() {
        estadoSalida = ""
        guard let salida else { return estado = "Es necesario indicar el archivo de salida" }
        // Síncrono como el C# (como mucho 3^11 vueltas).
        let columnas: [String]
        do throws(ErrorDominio) {
            columnas = try CasosUtilidades.reduccionPerfecta(filas, longitud: Int(FormatoNET.castInt32(longitud)))
        } catch {
            return estado = error.mensajeUsuario
        }
        do throws(ErrorDominio) {
            try sesion.entorno.archivos.escribir(TextoNET.codificarLineas(columnas), en: salida, atomico: false)
        } catch {
            estado = "Error al grabar las columnas: " + error.mensajeUsuario
            return sesion.alertas.error(estado)
        }
        estado = "Se han grabado \(columnas.count) columnas"
        estadoSalida = "Fichero de salida: " + salida.lastPathComponent
    }

    var body: some View {
        Pagina(destino: .reducidasPerfectas) {
            Tarjeta("Pronóstico y columna base") {
                Grid(alignment: .leading, horizontalSpacing: 12, verticalSpacing: 4) {
                    GridRow {
                        Text("")
                        Text("1").bold()
                        Text("X").bold()
                        Text("2").bold()
                        Text("Base").bold()
                    }
                    ForEach(filas.indices, id: \.self) { i in
                        GridRow {
                            Text(String(i + 1)).monospacedDigit()
                            ForEach(0..<3, id: \.self) { s in Toggle("", isOn: $filas[i].pronostico[s]).labelsHidden() }
                            Button(["1", "X", "2"][filas[i].base]) { filas[i].base = (filas[i].base + 1) % 3 }.frame(width: 40)
                        }
                    }
                }
            }
            Tarjeta("Salida") {
                CampoNumero(etiqueta: "Número de partidos", valor: $longitud, minimo: 1, maximo: 15, ancho: 50)
                HStack {
                    Button("Seleccionar archivo…") {
                        Task {
                            if let u = await SelectorFicheros.guardar("reducida", [("Columnas", ".txt"), ("Todos los archivos", ".")]) {
                                salida = u
                            }
                        }
                    }
                    Text(salida?.lastPathComponent ?? "(Sin archivo seleccionado)").foregroundStyle(Color.textoSecundario)
                }
                if let u = URL(string: "http://www.foro1x2.com/viewtopic.php?t=4445") { Link("Hilo del método en foro1x2", destination: u) }
            }
            HStack {
                VStack(alignment: .leading) {
                    Text(estado)
                    if !estadoSalida.isEmpty { Text(estadoSalida) }
                }
                .foregroundStyle(Color.textoSecundario)
                Spacer()
                Button("Generar") { generar() }.buttonStyle(EstiloPrincipal())
            }
        }
    }
}
