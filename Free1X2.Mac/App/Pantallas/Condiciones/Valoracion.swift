import Free1X2CasosDeUso
import SwiftUI

/// «Valoración» (pantalla 98, `ValoracionFrm`).
struct VistaValoracion: View {
    @Environment(Sesion.self) private var sesion
    @State private var matriz: [[Double]] = []
    @State private var modo = "Por sumas"
    @State private var rangos = ["", "", "", ""]
    @State private var minMaxSumas = ""
    @State private var minMaxProductos = ""
    @State private var columna = ""
    @State private var resultado = ""
    @State private var columnasBase = ""

    private var esSuma: Bool { modo == "Por sumas" }

    /// `PrepararValores`: refresca Mín-Máx y rellena los rangos vacíos en pantalla.
    private func preparar() -> ValoracionPreparada {
        let p = CasosCondiciones.prepararValoracion(matriz, esSuma: esSuma, rangos: rangos, cultura: sesion.entorno.cultura)
        (minMaxSumas, minMaxProductos, rangos) = (p.minMaxSumas, p.minMaxProductos, p.rangos)
        return p
    }

    private func aplicar(_ f: inout CondicionValoracionSignos) throws(ErrorDominio) {
        try CasosCondiciones.aplicarValoracion(preparar(), esSuma: esSuma, en: &f, cultura: sesion.entorno.cultura)
    }

    private func inicializar(_ f: CondicionValoracionSignos) {
        modo = f.tipoValoracion == "suma" ? "Por sumas" : "Por productos x 3E7"
        matriz = (0..<sesion.numeroPartidos).map { i in [f.valores1[safe: i] ?? 0, f.valoresX[safe: i] ?? 0, f.valores2[safe: i] ?? 0] }
        rangos = [f.valorGlobal, f.valorUnos, f.valorEquis, f.valorDoses]
    }

    var body: some View {
        PantallaCondicion(
            destino: .valoracion,
            subtitulo:
                "Especifique los porcentajes para cada partido y acote el rango mínimo y máximo para los valores Global, 1, X y 2. Filtra las columnas cuya valoración cae dentro de los rangos indicados.",
            tipo: .valoracionSignos, archivo: ArchivoCondicion("Valoracion", "Valoración de Signos", .valoracionSignos),
            cargar: { c in if c.valoracionSignos.contieneDatos { inicializar(c.valoracionSignos) } },
            temporal: { n throws(ErrorDominio) in
                var c = CondicionesGrupo(numeroPartidos: n)
                try aplicar(&c.valoracionSignos)
                return c
            },
            aceptar: { c throws(ErrorDominio) in
                try aplicar(&c.valoracionSignos)
                return true
            },
            borrar: {
                rangos = ["", "", "", ""]
                (minMaxSumas, minMaxProductos) = ("", "")
            },
            marcar: { inicializar($0.valoracionSignos) }
        ) {
            HStack(alignment: .top) {
                Tarjeta("Porcentajes") {
                    Text("Rejilla de valoraciones 1 / X / 2 por partido.").font(.callout).foregroundStyle(Color.textoSecundario)
                    PorcentajesGrid(valores: $matriz)
                    Button("Guardar valoración") {
                        sesion.valoracionInicial = matriz
                        sesion.router.navegar(.guardarValoracion)
                    }
                }
                VStack(spacing: 10) {
                    Tarjeta("Tipo de valoración") {
                        // Cambiar de modo borra los 4 rangos (`OnTipoValoracionSeleccionadoChanged`).
                        Picker(
                            "Modo", selection: Binding(get: { modo }, set: { if $0 != modo { (modo, rangos) = ($0, ["", "", "", ""]) } })
                        ) {
                            Text("Por sumas").tag("Por sumas")
                            Text("Por productos x 3E7").tag("Por productos x 3E7")
                        }
                        LabeledContent("Mín-Máx Sumas", value: minMaxSumas)
                        LabeledContent("Mín-Máx Productos", value: minMaxProductos)
                    }
                    Tarjeta("Rangos de filtrado") {
                        Text("Use # como separador de rangos (ejemplo: 100-200#200-600).").font(.callout)
                            .foregroundStyle(Color.textoSecundario)
                        CampoCondicion(etiqueta: "Global", texto: $rangos[0], ayuda: "100-200#200-600")
                        CampoCondicion(etiqueta: "1", texto: $rangos[1], ayuda: "0-100")
                        CampoCondicion(etiqueta: "X", texto: $rangos[2], ayuda: "0-100")
                        CampoCondicion(etiqueta: "2", texto: $rangos[3], ayuda: "0-100")
                    }
                    Tarjeta("Calcular valoración") {
                        CampoCondicion(etiqueta: "Columna", texto: $columna, ayuda: "Columna de N signos (1 / X / 2)")
                            .onChange(of: columna) { columna = columna.uppercased() }
                        HStack {
                            Button("Calcula") { calcular() }
                            LabeledContent("Valor", value: resultado)
                        }
                    }
                    Tarjeta("Utilidades") {
                        HStack {
                            Button("Buscar límite") { sesion.router.navegar(.buscaLims) }
                            Button("Columnas base") { columnasBase = CasosCondiciones.columnasBase(matriz) }
                        }
                        TextEditor(text: .constant(columnasBase)).font(.body.monospaced()).frame(height: 60)
                    }
                }
            }
        }
        .onAppear { if matriz.isEmpty { matriz = Array(repeating: [0, 0, 0], count: sesion.numeroPartidos) } }
    }

    /// `Calcular`: escribe en la condición del grupo en edición (como hoy).
    private func calcular() {
        let p = preparar()
        guard sesion.combinacion.grupos.indices.contains(sesion.grupoEnEdicion) else { return }
        do throws(ErrorDominio) {
            try sesion.combinacion.modificarGrupo(sesion.grupoEnEdicion) { g throws(ErrorDominio) in
                if let r = try CasosCondiciones.valorarColumna(
                    columna, p, esSuma: esSuma, en: &g.condiciones.valoracionSignos, numeroPartidos: sesion.numeroPartidos,
                    cultura: sesion.entorno.cultura)
                {
                    resultado = r
                }
            }
        } catch {
            sesion.alertas.inesperado(error)
        }
    }
}
