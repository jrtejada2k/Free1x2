import Free1X2CasosDeUso
import SwiftUI

/// «Signos Seguidos» (pantalla 88, `SignosSeguidosFrm`).
struct VistaSignosSeguidos: View {
    @Environment(Sesion.self) private var sesion
    @State private var textos = ["", "", "", ""]
    /// `_figurasV/_figuras1/_figurasX/_figuras2` (`nil` tras Borrar).
    @State private var figuras: [[Int64]?] = [nil, nil, nil, nil]

    private func aplicar(_ f: inout CondicionSignosSeguidos) throws(ErrorDominio) {
        try CasosCondiciones.aplicarSignosSeguidos(textos, figuras: figuras, en: &f)
    }

    var body: some View {
        PantallaCondicion(
            destino: .signosSeguidos,
            subtitulo: "Especifica la cantidad de Variantes seguidas, de signos 1, X y 2 seguidos por combinación.",
            tipo: .signosSeguidos, archivo: ArchivoCondicion("SignosSeguidos", "Signos seguidos", .signosSeguidos),
            cargar: { c in
                textos = CondicionSignosSeguidos.Clase.allCases.map { c.signosSeguidos.get($0) }
                figuras = c.signosSeguidos.figuras.map { Optional($0) }
            },
            temporal: { n throws(ErrorDominio) in
                var c = CondicionesGrupo(numeroPartidos: n)
                try aplicar(&c.signosSeguidos)
                return c
            },
            aceptar: { c throws(ErrorDominio) in
                try CasosCondiciones.conCantidadesValidas(numeroPartidos: sesion.numeroPartidos) { () throws(ErrorDominio) in
                    try aplicar(&c.signosSeguidos)
                }
                return true
            },
            borrar: {
                textos = ["", "", "", ""]
                figuras = [nil, nil, nil, nil]
            }
        ) {
            Tarjeta("Cantidades admitidas (0–14)") {
                Text("Introduce las cantidades permitidas para cada concepto. Separa varios valores con comas (p. ej. 0,1,2).")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                ForEach(0..<4, id: \.self) { i in
                    CampoCondicion(etiqueta: ["Var", "1", "X", "2"][i], texto: $textos[i], ayuda: "p. ej. 0,1,2")
                }
            }
            Tarjeta("Figuras") {
                Text("Define las figuras asociadas a cada concepto. Cuando un concepto tiene figuras definidas se muestra como activo.")
                    .font(.callout).foregroundStyle(Color.textoSecundario)
                HStack {
                    ForEach(0..<4, id: \.self) { i in
                        VStack {
                            Button(["Figuras Variantes", "Figuras Unos", "Figuras Equis", "Figuras Doses"][i]) {
                                sesion.figuras = FigurasEnEdicion(lista: figuras[i] ?? [], condicion: "SignosSeguidos", clave: i)
                                sesion.router.navegar(.figurasFiltros)
                            }
                            if !(figuras[i] ?? []).isEmpty { Text("Activas").font(.caption).foregroundStyle(Color.exito) }
                        }
                    }
                }
            }
        }
        .onAppear {
            guard let f = sesion.figuras, f.condicion == "SignosSeguidos" else { return }
            if let r = f.resultado { figuras[f.clave] = r }
            sesion.figuras = nil
        }
    }
}

/// «Figuras» (pantalla 49, `FigurasFiltrosFrm`): 12 casillas o más (figuras + 2).
struct VistaFigurasFiltros: View {
    @Environment(Sesion.self) private var sesion
    @State private var casillas: [String] = Array(repeating: "", count: 12)
    @State private var cargado = false

    private func pintar(_ lista: [Int64]) {
        while casillas.count < lista.count + 2 { casillas.append("") }
        for i in casillas.indices { casillas[i] = i < lista.count ? CasosCondiciones.textoFigura(lista[i]) : "" }
    }

    var body: some View {
        Pagina(destino: .figurasFiltros) {
            Text(
                "Edita la lista de figuras asociadas a este filtro. Cada casilla representa una figura; escribe sus valores separados por «-» (p. ej. 8-5-4-3-1-1-1-1)."
            )
            .foregroundStyle(Color.textoSecundario)
            Tarjeta("Casillas de figuras") {
                // M11 (#28): casillas de 96 de ancho y letra 15.
                LazyVGrid(columns: Array(repeating: GridItem(.fixed(96)), count: 6), alignment: .leading) {
                    ForEach(casillas.indices, id: \.self) { i in
                        VStack(alignment: .leading, spacing: 2) {
                            Text("\(i + 1)").font(.caption).foregroundStyle(Color.textoSecundario)
                            TextField("", text: $casillas[i]).multilineTextAlignment(.center).font(.system(size: 15))
                        }
                    }
                }
                Text(
                    sesion.figuras?.condicion == "SignosSeguidos"
                        ? "Cada figura son los tramos de signos seguidos, por ejemplo 3-2-2-1-1 o 32211 (se ordenan de mayor a menor)."
                        : "Cada valor de la figura, de 0 a 15, separado del siguiente por «-»."
                )
                .font(.caption).foregroundStyle(Color.textoSecundario)
            }
            HStack {
                Button("Aceptar") {
                    sesion.figuras?.resultado = CasosCondiciones.aceptarFiguras(casillas, condicion: sesion.figuras?.condicion ?? "")
                    sesion.router.atras()
                }
                .buttonStyle(EstiloPrincipal()).keyboardShortcut(.defaultAction)
                Button("Abrir…") {
                    Task {
                        let ext = CasosCondiciones.extensionFiguras(sesion.figuras?.condicion ?? "")
                        guard let url = await SelectorFicheros.abrirVarios([ext, "*"], multiple: false).first else { return }
                        do throws(ErrorDominio) {
                            pintar(try CasosCondiciones.leerFiguras(url, entorno: sesion.entorno))
                        } catch {
                            sesion.alertas.error(error)
                        }
                    }
                }
                Button("Borrar") { casillas = casillas.map { _ in "" } }
                Button("Cancelar") { sesion.router.atras() }
            }
        }
        .onAppear {
            guard !cargado else { return }
            cargado = true
            pintar(sesion.figuras?.lista ?? [])
        }
    }
}
