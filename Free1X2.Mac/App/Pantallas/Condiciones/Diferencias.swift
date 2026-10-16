import Free1X2CasosDeUso
import SwiftUI

/// «Diferencias» (pantalla 42, `DiferenciasFrm`): trabaja sobre la condición del grupo en edición.
struct VistaDiferencias: View {
    @Environment(Sesion.self) private var sesion
    @State private var t = TextosDiferencia()
    @State private var indice = 0
    @State private var atajo = "Dúos"
    @State private var pasoFijo = true
    private static let conceptos = ["Variantes", "Equis", "Doses", "Dibujos", "Interrupciones", "Formatos"]

    private var n: Int { sesion.numeroPartidos }
    private var guardados: Int { sesion.combinacion.grupos[safe: sesion.grupoEnEdicion]?.condiciones.diferencias.diferencias.count ?? 0 }

    /// Ejecuta `cambio` sobre la condición del grupo en edición (sin grupo no hace nada).
    private func enGrupo<T>(_ cambio: (inout CondicionDiferencias) -> T) -> T? {
        guard sesion.combinacion.grupos.indices.contains(sesion.grupoEnEdicion) else { return nil }
        var r: T?
        try? sesion.combinacion.modificarGrupo(sesion.grupoEnEdicion) { r = cambio(&$0.condiciones.diferencias) }
        return r
    }

    private func mostrar(_ d: Diferencia?) {
        t =
            d.map { CasosCondiciones.textosDiferencia($0, lineasMinimas: t.lineas.count) }
            ?? TextosDiferencia(lineas: t.lineas.map { _ in "" })
    }

    /// `ActualizarDatos` sobre el grupo y su condición resultante.
    private func actualizar() -> CondicionesGrupo? {
        _ = enGrupo { CasosCondiciones.actualizarDiferencias(&$0, indice: indice, t, numeroPartidos: n) }
        return sesion.combinacion.grupos[safe: sesion.grupoEnEdicion]?.condiciones
    }

    var body: some View {
        PantallaCondicion(
            destino: .diferencias,
            subtitulo:
                "Especifica en cada línea un grupo de partidos separados por \",\" o por \"-\". Para cada concepto indica la cantidad o el intervalo de valores DISTINTOS entre los grupos.",
            tipo: .diferencias, archivo: ArchivoCondicion("Diferencias", "Diferencias", .diferencias, temporal: "rep"),
            cargar: { c in
                indice = 0
                var d = c.diferencias
                mostrar(d.contieneDatos() ? d.diferencias.first : nil)
            },
            temporal: { _ in actualizar() },
            aceptar: { c in
                CasosCondiciones.actualizarDiferencias(&c.diferencias, indice: indice, t, numeroPartidos: n)
                c.diferencias.activa = c.diferencias.contieneDatos()
                return true
            },
            borrar: {
                _ = enGrupo { $0.diferencias = [] }
                indice = 0
                mostrar(nil)
            },
            marcar: { leido in
                guard enGrupo({ CasosCondiciones.importarDiferencias(leido.diferencias, en: &$0) }) == true else { return }
                indice = 0
                mostrar(enGrupo { $0.contieneDatos() ? $0.diferencias.first : nil } ?? nil)
            }
        ) {
            Tarjeta("Conjuntos — actual / guardados") {
                HStack {
                    Button("◀") {
                        if let r = enGrupo({ CasosCondiciones.anteriorDiferencia(&$0, indice: indice, t, numeroPartidos: n) }) ?? nil {
                            indice = r.indice
                            mostrar(r.mostrar)
                        }
                    }
                    .disabled(indice < 1)
                    .accessibilityLabel("Conjunto anterior")
                    Text("\(indice + 1)/\(guardados)").monospacedDigit()
                    Button("▶") {
                        if let r = enGrupo({ CasosCondiciones.siguienteDiferencia(&$0, indice: indice, t, numeroPartidos: n) }) ?? nil {
                            indice = r.indice
                            mostrar(r.mostrar)
                        }
                    }
                    .accessibilityLabel("Conjunto siguiente")
                    Button("Eliminar actual") {
                        let d = enGrupo { CasosCondiciones.eliminarDiferencia(&$0, indice: indice) } ?? nil
                        indice = 0
                        mostrar(d)
                    }
                }
            }
            HStack(alignment: .top) {
                Tarjeta("Grupos de partidos") {
                    Text("Una línea por grupo. Partidos separados por \",\" o \"-\".").font(.callout).foregroundStyle(Color.textoSecundario)
                    ForEach(t.lineas.indices, id: \.self) { i in
                        HStack {
                            Text("\(i + 1)").monospacedDigit().frame(width: 24, alignment: .trailing)
                            TextField("", text: $t.lineas[i], prompt: Text("p. ej. 1,2-5")).textFieldStyle(.roundedBorder)
                        }
                    }
                }
                VStack(spacing: 10) {
                    Tarjeta("Valores distintos por concepto") {
                        Text("Cantidad o intervalo de valores distintos entre los grupos.").font(.callout)
                            .foregroundStyle(Color.textoSecundario)
                        ForEach(0..<6, id: \.self) { i in
                            CampoCondicion(etiqueta: Self.conceptos[i], texto: $t.conceptos[i], ayuda: "p. ej. 2 o 1-3")
                        }
                    }
                    Tarjeta("Atajos") {
                        Text("Genera automáticamente los grupos de partidos por agrupación.").font(.callout)
                            .foregroundStyle(Color.textoSecundario)
                        Picker("Agrupación", selection: $atajo) {
                            ForEach(["Dúos", "Tríos", "Cuartetos", "Quintetos", "Sextetos", "Septetos", "Octetos"], id: \.self) { Text($0) }
                        }
                        Toggle("Paso fijo", isOn: $pasoFijo)
                        Button("Generar") {
                            t.lineas = CasosCondiciones.atajosDiferencias(atajo, pasoFijo: pasoFijo, lineas: t.lineas, numeroPartidos: n)
                        }
                    }
                }
            }
        }
    }
}
