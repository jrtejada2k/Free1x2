import AppKit
import Free1X2CasosDeUso
import SwiftUI

/// Botón de la barra: tooltip = nombre accesible (`MainWindow.xaml.cs:350-433`); `icono` = PNG original de 16×16 de
/// `Assets/Toolbar` (DT-77); `comando == nil` = deshabilitado.
struct Herramienta: Identifiable {
    let titulo: String
    let icono: String
    let comando: Comando?
    var id: String { titulo }
}

/// Los 6 grupos en su orden visual (`GrupoBarra`, :343) con los iconos originales (DT-77 sustituye a DT-50).
enum GrupoBarra: CaseIterable {
    case free, archivo, operaciones, combinacion, filtros, utilidades

    /// Posición en `parametros.free1x2` (`tsFree, tsFiltros, tsCombinacion, tsOperaciones, tsArchivo, tsUtilidades`).
    var indiceFichero: Int {
        switch self {
        case .free: 0
        case .filtros: 1
        case .combinacion: 2
        case .operaciones: 3
        case .archivo: 4
        case .utilidades: 5
        }
    }

    /// Nombre en Ver ▸ Barras de herramientas.
    var nombre: String {
        switch self {
        case .free: "Free1X2"
        case .archivo: "Archivo"
        case .operaciones: "Operaciones"
        case .combinacion: "Combinación"
        case .filtros: "Filtros"
        case .utilidades: "Utilidades"
        }
    }

    /// Orden del submenú (10 §1.3).
    static let ordenMenu: [GrupoBarra] = [.filtros, .free, .operaciones, .utilidades, .combinacion, .archivo]

    var herramientas: [Herramienta] {
        func n(_ t: String, _ i: String, _ d: Destino) -> Herramienta { Herramienta(titulo: t, icono: i, comando: .navegar(d)) }
        func a(_ t: String, _ i: String, _ c: Comando) -> Herramienta { Herramienta(titulo: t, icono: i, comando: c) }
        switch self {
        case .free:
            return [
                a("Salir", "bSalir", .salir), n("Configuración", "bConfig", .configuracion),
                n("Configurar Análisis", "bConfAnalisis", .configuracionAnalisis),
                n("Ayuda", "bAyuda", .ayuda), n("Acerca de ...", "bAcercaDe", .acercaDe),
            ]
        case .archivo:
            return [
                a("Guardar equipos", "bGuardarEquipos", .guardarEquipos),
                a("Nueva combinación", "bNuevo", .nuevaCombinacion),
                n("Obtener Boletos Online", "bObtenerBoletos", .descargaBoleto),
                a("Abrir combinación", "bAbrirCombinacion", .abrirCombinacion),
                a("Guardar combinación", "bGuardarCombinacion", .guardarCombinacion),
                a("Guardar combinación como", "bGuardarCombinacionComo", .guardarComo),
                a("Borrar archivos temporales", "bBorrarTemporales", .borrarTemporales), a("Abrir equipos", "bAbrirEquipos", .abrirEquipos),
                a("Borrar Informes de Error", "bBorrarInformes", .borrarInformes),
                n("Gestión de Equipos", "bGestorEquipos", .gestorEquipos),
            ]
        case .operaciones:
            return [
                n("Algebra", "bAlgebra", .algebraColumnas),
                n("Transposición", "bTransposicion", .transposicion), n("Multiplicación", "bMultiplicacion", .multiplicador),
                n("Fraccionar", "bFraccionador", .fraccionador),
                n("Rotación de signos", "bRotacion", .rotacionDeSignos),
            ]
        case .combinacion:
            return [
                n("Calcular combinación", "bCalcular", .calculaColumnas),
                n("Calcular múltiples combinaciones", "bCalcularM", .calculaColumnasMultiple),
                n("Ver boletos", "bVerBoletos", .verBoletos), n("Imprimir boletos", "bImprimirBoletos", .imprimirBoleto),
                n("Reducir", "bReducir", .reductor), n("Escrutinio", "bEscrutinio", .escrutinios),
                Herramienta(titulo: "Escrutar combinaciones", icono: "bEscrutarComb", comando: nil),
                n("Análisis de columnas", "bAnalisisColumnas", .analizarFichero),
                n("Análisis de fallos", "bAnalisisFallos", .colGanadora),
                n("Análisis de signos", "bAnalisisSignos", .vSignos), n("Probabilidades", "bProbabilidades", .probabilidadPremios),
                n("Estadísticas", "bEstadisticas", .anastatics), n("Añadir pleno al 15", "bP15", .agregaP15),
            ]
        case .filtros:
            return [
                n("Combinar filtros", "bCombinarFiltros", .combinarFiltros),
                n("Diferencias de filtros", "bDiferenciasFiltros", .diFiltros),
                n("Filtro de coincidencias", "bFiltroCoincidencias", .coincidencias), n("Filtro Aidomnou", "bFiltroAidomnou", .aidomnou),
                n("Filtro Pim", "bFiltroPim", .generaPim),
            ]
        case .utilidades:
            return [
                n("Subir categoría", "bSubeCategoria", .subirCategoria),
                n("Modificador de porcentajes", "bModificadorPct", .modificador),
                n("Generador de CPs", "bGeneradorCPs", .generarCPs),
                n("Diferencias entre columnas", "bDiferenciasColumnas", .difCols),
                n("Ordenar por probabilidad", "bProbabilidad", .ordenarPorProbabilidad),
                n("Selector JuanM", "bSelectorJuanM", .selecJM), n("Selector MarioSan", "bSelectorMarioSan", .selectorMS),
                n("Rentabilidad", "bRentabilidad", .rentabilidad), n("Columnas GEPT", "bColumnasGEPT", .gEPT),
                n("Tramificar", "bTramificar", .tramificar), n("Premiadas", "bPremiadas", .premiadas),
                n("Estimación de premios", "bEstimacion", .estimadorPremios),
                n("Banco de pruebas", "bBancoPruebas", .bancoPruebas),
                n("Importar / Exportar", "bImportExport", .importExport),
                n("Análisis de grupos", "bAnalisisGrupos", .anaCombi),
                n("Reducciones perfectas", "bRedPerfectas", .reducidasPerfectas),
                n("Dependencia lineal", "bDependenciaLineal", .dependenciaLineal),
            ]
        }
    }
}

/// La barra (`ToolbarPanel`): franja bajo el título con los grupos visibles separados por una raya y que salta de línea
/// como el `WrapPanel` del original, en vez de la barra de la ventana, que tiñe los iconos y esconde casi todos en `»`.
struct BarraHerramientas: View {
    let sesion: Sesion

    var body: some View {
        Flujo(espacio: 2) {
            ForEach(GrupoBarra.allCases.filter { sesion.barras[safe: $0.indiceFichero] ?? true }, id: \.self) { g in
                ForEach(g.herramientas) { h in
                    Button {
                        if let c = h.comando { sesion.ejecutar(c) }
                    } label: {
                        // Casi todos miden 16 px (nítidos sin suavizar); tres miden 24 o 32 y se reducen suavizados.
                        let imagen = NSImage(named: h.icono) ?? NSImage()
                        Image(nsImage: imagen)
                            .resizable()
                            .interpolation(imagen.size.width > 16 ? .high : .none)
                            .frame(width: 16, height: 16)
                            .frame(width: 26, height: 26)
                            .contentShape(.rect)
                    }
                    .buttonStyle(EstiloHerramienta())
                    .help(h.titulo)
                    .accessibilityLabel(h.titulo)
                    .disabled(h.comando == nil)
                }
                Rectangle().fill(Color.borde).frame(width: 1, height: 18).padding(.horizontal, 4).padding(.vertical, 4)
            }
        }
        .padding(.horizontal, 8)
        .padding(.vertical, 3)
        .frame(maxWidth: .infinity, alignment: .leading)
        .background(Color.superficieAlt)
        .overlay(alignment: .bottom) { Rectangle().fill(Color.borde).frame(height: 1) }
    }
}

/// Botón transparente de 26×26 de la barra; resalte al pasar el ratón y atenuado si está deshabilitado.
struct EstiloHerramienta: ButtonStyle {
    @Environment(\.isEnabled) private var activo
    @State private var encima = false

    func makeBody(configuration: Configuration) -> some View {
        configuration.label
            .background(Color.acento.opacity(configuration.isPressed ? 0.25 : encima ? 0.12 : 0), in: .rect(cornerRadius: 5))
            .opacity(activo ? 1 : 0.35)
            .onHover { encima = $0 }
    }
}

/// `WrapPanel`: coloca las vistas en fila y salta de línea cuando no caben.
struct Flujo: Layout {
    var espacio: CGFloat = 0

    func sizeThatFits(proposal: ProposedViewSize, subviews: Subviews, cache: inout ()) -> CGSize {
        let filas = colocar(ancho: proposal.width ?? .infinity, subviews)
        return CGSize(width: filas.map(\.maxX).max() ?? 0, height: filas.map(\.maxY).max() ?? 0)
    }

    func placeSubviews(in bounds: CGRect, proposal: ProposedViewSize, subviews: Subviews, cache: inout ()) {
        for (v, r) in zip(subviews, colocar(ancho: bounds.width, subviews)) {
            v.place(at: CGPoint(x: bounds.minX + r.minX, y: bounds.minY + r.minY), proposal: ProposedViewSize(r.size))
        }
    }

    private func colocar(ancho: CGFloat, _ subviews: Subviews) -> [CGRect] {
        var (x, y, alto) = (CGFloat(0), CGFloat(0), CGFloat(0))
        return subviews.map { v in
            let t = v.sizeThatFits(.unspecified)
            if x > 0 && x + t.width > ancho { (x, y, alto) = (0, y + alto + espacio, 0) }
            defer { (x, alto) = (x + t.width + espacio, max(alto, t.height)) }
            return CGRect(origin: CGPoint(x: x, y: y), size: t)
        }
    }
}
