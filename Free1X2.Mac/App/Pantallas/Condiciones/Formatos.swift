import Free1X2CasosDeUso
import SwiftUI

/// «Formatos 123» (pantalla 51, `Formatos123Frm`).
struct VistaFormatos123: View {
    @Environment(Sesion.self) private var sesion
    @State private var filas = Array(repeating: FilaFormato123(), count: 8)
    @State private var aciertos = ""
    @State private var ignorar = false
    @State private var matriz: [[Double]] = []
    @State private var columna1X2 = ""
    @State private var columna123 = ""

    private func aplicar(_ f: inout CondicionFormatos123) throws(ErrorDominio) {
        try CasosCondiciones.aplicarFormatos123(filas, aciertos: aciertos, ignorarRepeticiones: ignorar, valoracion: matriz, en: &f)
    }

    /// `MarcarValores`.
    private func marcar(_ f: CondicionFormatos123) {
        if !f.valoracion.isEmpty { matriz = f.valoracion }
        filas =
            f.formatos.map { FilaFormato123(formato: $0.formato, minimo: String($0.aciertosMin), maximo: String($0.aciertosMax)) }
            + Array(repeating: FilaFormato123(), count: 8)
        aciertos = f.aciertosFiltro ?? ""
        ignorar = f.pasoFijo
    }

    var body: some View {
        PantallaCondicion(
            destino: .formatos123,
            subtitulo:
                "Un Formato 123 es la representación de una secuencia de signos en función de la valoración: el 1 corresponde al signo más valorado, el 2 al segundo más valorado y el 3 al menos valorado.",
            tipo: .formatos123, archivo: ArchivoCondicion("Formatos123", "Formatos 123", .formatos123),
            cargar: { c in if c.formatos123.contieneDatos { marcar(c.formatos123) } },
            temporal: { n throws(ErrorDominio) in
                var c = CondicionesGrupo(numeroPartidos: n)
                try aplicar(&c.formatos123)
                return c
            },
            aceptar: { c throws(ErrorDominio) in
                try aplicar(&c.formatos123)
                return true
            },
            borrar: {
                filas = Array(repeating: FilaFormato123(), count: 8)
                aciertos = ""
                ignorar = false
            },
            marcar: { marcar($0.formatos123) }
        ) {
            HStack(alignment: .top) {
                VStack(spacing: 10) {
                    Tarjeta("Formatos") {
                        Grid(alignment: .leading, horizontalSpacing: 6, verticalSpacing: 3) {
                            GridRow {
                                Text("Nº")
                                Text("Formato")
                                Text("Mín")
                                Text("Máx")
                            }
                            .font(.caption).foregroundStyle(Color.textoSecundario)
                            ForEach(filas.indices, id: \.self) { i in
                                GridRow {
                                    Text("\(i + 1)").monospacedDigit()
                                    TextField("", text: $filas[i].formato, prompt: Text("123…")).frame(minWidth: 140)
                                    TextField("", text: $filas[i].minimo).frame(width: 50)
                                    TextField("", text: $filas[i].maximo).frame(width: 50)
                                }
                            }
                        }
                        Button("Añadir fila") { filas.append(FilaFormato123()) }
                    }
                    Tarjeta("Aciertos") {
                        CampoCondicion(
                            etiqueta: ignorar ? "Aciertos Globales" : "Líneas Acertadas", texto: $aciertos, ayuda: "Ej. 10,12-14")
                        Toggle("Ignorar repeticiones", isOn: $ignorar)
                    }
                }
                VStack(spacing: 10) {
                    Tarjeta("Porcentajes") { PorcentajesGrid(valores: $matriz) }
                    Tarjeta("Utilidades") {
                        Text("Traducir columna 1X2 a 123").font(.callout)
                        HStack {
                            TextField("", text: $columna1X2, prompt: Text("1X2…"))
                            Button("›") {
                                do throws(ErrorDominio) {
                                    columna123 = try CasosCondiciones.traducirFormatos123(
                                        columna1X2, valoracion: matriz, numeroPartidos: sesion.numeroPartidos)
                                } catch {
                                    sesion.alertas.inesperado(error)
                                }
                            }
                            TextField("", text: .constant(columna123), prompt: Text("123…"))
                        }
                        Button("Analizador de formatos") {
                            let f = CasosCondiciones.formatos123(filas, ignorarRepeticiones: ignorar)
                            guard !f.isEmpty else { return sesion.alertas.info("Guarde el Filtro Primero!!!") }
                            sesion.formatos123Entrada = f
                            sesion.router.navegar(.analisisFormatos123)
                        }
                    }
                }
            }
        }
        .onAppear { if matriz.isEmpty { matriz = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos) } }
    }
}

/// «Formatos (1, X, 2, V, *)» (pantalla 52, `FormatosFrm`): las relaciones viven en pantalla hasta Aceptar; Abrir y
/// Borrar cambian ya la condición del grupo.
struct VistaFormatos: View {
    @Environment(Sesion.self) private var sesion
    @State private var relaciones = [RelacionFormatos()]
    @State private var indice = 0

    private func aplicar(_ f: inout CondicionFormatosSignos) throws(ErrorDominio) {
        try CasosCondiciones.aplicarFormatosSignos(relaciones, en: &f, numeroPartidos: sesion.numeroPartidos)
    }

    private func recargar() {
        relaciones = CasosCondiciones.relacionesFormatos(
            sesion.combinacion.grupos[safe: sesion.grupoEnEdicion]?.condiciones.formatosSignos ?? CondicionFormatosSignos())
        indice = 0
    }

    private func enGrupo(_ cambio: (inout CondicionFormatosSignos) -> Void) {
        guard sesion.combinacion.grupos.indices.contains(sesion.grupoEnEdicion) else { return }
        try? sesion.combinacion.modificarGrupo(sesion.grupoEnEdicion) { cambio(&$0.condiciones.formatosSignos) }
    }

    var body: some View {
        PantallaCondicion(
            destino: .formatos,
            subtitulo:
                "Un formato es una determinada secuencia de signos; esta condición controla la repetición o aparición de los distintos formatos.",
            tipo: .formatosSignos, archivo: ArchivoCondicion("Formatos", "Formatos", .formatosSignos),
            cargar: { _ in recargar() },
            temporal: { n throws(ErrorDominio) in
                var c = CondicionesGrupo(numeroPartidos: n)
                try aplicar(&c.formatosSignos)
                return c
            },
            aceptar: { c throws(ErrorDominio) in
                try aplicar(&c.formatosSignos)
                return true
            },
            borrar: {
                enGrupo {
                    $0.ponerFormatosSignos([])
                    ($0.contieneDatos, $0.activa) = (false, false)
                }
                recargar()
            },
            marcar: { leido in
                enGrupo {
                    $0.ponerFormatosSignos(leido.formatosSignos.formatosSignos)
                    ($0.contieneDatos, $0.activa) = (leido.formatosSignos.contieneDatos, leido.formatosSignos.activa)
                }
                recargar()
            }
        ) {
            Tarjeta {
                HStack {
                    Button {
                        if indice > 0 { indice -= 1 }
                    } label: {
                        Image(systemName: "chevron.left")
                    }
                    .disabled(indice < 1)
                    .accessibilityLabel("Formato anterior")
                    Text("\(indice + 1)/\(relaciones.count)").monospacedDigit()
                    Button {
                        if indice + 1 >= relaciones.count { relaciones.append(RelacionFormatos()) }
                        indice += 1
                    } label: {
                        Image(systemName: "chevron.right")
                    }
                    .accessibilityLabel("Formato siguiente")
                    Button("Eliminar actual") {
                        relaciones.remove(at: indice)
                        if relaciones.isEmpty { relaciones.append(RelacionFormatos()) }
                        if indice >= relaciones.count { indice = relaciones.count - 1 }
                    }
                }
            }
            HStack(alignment: .top) {
                Tarjeta("Líneas de formato") {
                    Text("Cada línea es una secuencia de signos (1, X, 2, V, *) y su rango de apariciones Mín-Máx.").font(.callout)
                        .foregroundStyle(Color.textoSecundario)
                    Grid(alignment: .leading, horizontalSpacing: 6, verticalSpacing: 3) {
                        GridRow {
                            Text("Formato")
                            Text("Mín-Máx")
                        }
                        .font(.caption).foregroundStyle(Color.textoSecundario)
                        ForEach(relaciones[indice].formatos.indices, id: \.self) { i in
                            GridRow {
                                TextField("", text: $relaciones[indice].formatos[i], prompt: Text("p. ej. 1X2")).frame(minWidth: 160)
                                TextField("", text: $relaciones[indice].rangos[i], prompt: Text("mín-máx")).frame(width: 80)
                            }
                        }
                    }
                }
                VStack(spacing: 10) {
                    Tarjeta {
                        CampoCondicion(etiqueta: "Líneas", texto: $relaciones[indice].lineas)
                        CampoCondicion(etiqueta: "Global", texto: $relaciones[indice].global)
                    }
                    Tarjeta("Utilidades de formatos") {
                        Text("Herramientas auxiliares para generar o analizar formatos.").font(.callout).foregroundStyle(
                            Color.textoSecundario)
                        HStack {
                            Button("Saca formatos") { sesion.router.navegar(.calculoFormatos) }
                            Button("Pares") { sesion.router.navegar(.parejas) }
                            Button("Tríos") { sesion.router.navegar(.trios) }
                            Button("Sumas pares nat.") { sesion.router.navegar(.analizadorJPM) }
                        }
                    }
                }
            }
        }
    }
}
