import Free1X2CasosDeUso
import SwiftUI

/// «Contactos» (pantalla 26, `ContactosFrm`): Guardar y Copiar vuelcan antes en la condición del grupo, como hoy.
struct VistaContactos: View {
    @Environment(Sesion.self) private var sesion
    @State private var filas = Array(repeating: "", count: 10)
    @State private var figuras: [Int64]?

    private func cargar(_ f: CondicionContactos) {
        filas = CondicionContactos.Contacto.allCases.map(f.get)
        figuras = f.figuras
    }

    var body: some View {
        PantallaCondicion(
            destino: .contactos,
            subtitulo:
                "Se da un contacto entre dos signos cuando aparecen juntos en la columna, sin importar cuál aparece en primer lugar.",
            tipo: .contactos, archivo: ArchivoCondicion("Contactos", "Contactos", .contactos),
            cargar: { cargar($0.contactos) },
            temporal: { n throws(ErrorDominio) in
                var c = CondicionesGrupo(numeroPartidos: n)
                try CasosCondiciones.aplicarContactos(filas, figuras: figuras, en: &c.contactos)
                return c
            },
            aceptar: { c throws(ErrorDominio) in
                try CasosCondiciones.aplicarContactos(filas, figuras: figuras, en: &c.contactos)
                return true
            },
            borrar: {
                filas = filas.map { _ in "" }
                figuras = nil
            },
            exportable: { _ throws(ErrorDominio) in
                guard sesion.combinacion.grupos.indices.contains(sesion.grupoEnEdicion) else { return nil }
                try sesion.combinacion.modificarGrupo(sesion.grupoEnEdicion) { g throws(ErrorDominio) in
                    try CasosCondiciones.aplicarContactos(filas, figuras: figuras, en: &g.condiciones.contactos)
                }
                return sesion.combinacion.grupos[sesion.grupoEnEdicion].condiciones
            }
        ) {
            Tarjeta("Número de contactos por pareja de signos") {
                Text("Para cada pareja, indica los valores admitidos (de 0 a 14) separados por comas.")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                ForEach(CondicionContactos.Contacto.allCases, id: \.self) { c in
                    CampoCondicion(etiqueta: c.par, texto: $filas[c.rawValue], ayuda: "p.ej. 0,1,2,3")
                }
            }
            Tarjeta("Figuras") {
                Text("Condición adicional de figuras asociada a este filtro.").font(.callout).foregroundStyle(Color.textoSecundario)
                HStack {
                    SemaforoView(estado: (figuras ?? []).isEmpty ? .neutro : .verde)
                    Button("Editar figuras…") {
                        sesion.figuras = FigurasEnEdicion(lista: figuras ?? [], condicion: "Contactos", clave: 0)
                        sesion.router.navegar(.figurasFiltros)
                    }
                }
            }
        }
        .onAppear {
            guard let f = sesion.figuras, f.condicion == "Contactos" else { return }
            if let r = f.resultado { figuras = r }
            sesion.figuras = nil
        }
    }
}

/// «Dibujos X+2» (pantalla 40, `DibujosFrm`).
struct VistaDibujos: View {
    @Environment(Sesion.self) private var sesion
    @State private var marcadas = Set<String>()

    private var casillas: [String] { CasosCondiciones.casillasDibujos(sesion.numeroPartidos) }
    /// En el orden de la malla.
    private var lista: [String] { casillas.filter(marcadas.contains) }

    var body: some View {
        PantallaCondicion(
            destino: .dibujos, subtitulo: "Filtra las columnas por la figura formada por el número de X y el número de 2.",
            tipo: .dibujos, archivo: ArchivoCondicion("Dibujos", "Dibujos", .dibujos),
            cargar: { c in marcadas = Set(c.dibujos.dibujos.map(FormatoNET.recortar)) },
            temporal: { n throws(ErrorDominio) in
                var c = CondicionesGrupo(numeroPartidos: n)
                try CasosCondiciones.aplicarDibujos(lista, en: &c.dibujos)
                return c
            },
            aceptar: { c throws(ErrorDominio) in
                try CasosCondiciones.aplicarDibujos(lista, en: &c.dibujos)
                return true
            },
            borrar: { marcadas = [] }
        ) {
            Tarjeta {
                Text(
                    "Un dibujo es la figura formada por el número de X y el número de 2. Por ejemplo: 2 equis y 3 doses forman el dibujo 2+3."
                )
                .font(.callout).foregroundStyle(Color.textoSecundario)
                HStack {
                    Button("Marcar todos") { marcadas = Set(casillas) }
                    Button("Desmarcar todos") { marcadas = [] }
                    Spacer()
                    Text("Dibujos seleccionados: \(lista.count)").foregroundStyle(Color.textoSecundario)
                }
                Grid(alignment: .leading, horizontalSpacing: 4, verticalSpacing: 2) {
                    ForEach(0...sesion.numeroPartidos, id: \.self) { x in
                        GridRow {
                            ForEach(0...(sesion.numeroPartidos - x), id: \.self) { y in
                                let e = "\(x)+\(y)"
                                Toggle(
                                    e,
                                    isOn: Binding(
                                        get: { marcadas.contains(e) }, set: { if $0 { marcadas.insert(e) } else { marcadas.remove(e) } })
                                )
                                .toggleStyle(.checkbox).font(.caption.monospacedDigit())
                            }
                        }
                    }
                }
            }
        }
    }
}
