import Free1X2CasosDeUso
import SwiftUI

/// Botón de la barra: tooltip = nombre accesible (`MainWindow.xaml.cs:350-433`); `comando == nil` = deshabilitado.
struct Herramienta: Identifiable {
    let titulo: String
    let simbolo: String
    let comando: Comando?
    var id: String { titulo }
}

/// Los 6 grupos en su orden visual (`GrupoBarra`, :343) con los iconos de DT-50 (ESPEC_15 §1.4).
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
        func n(_ t: String, _ s: String, _ d: Destino) -> Herramienta { Herramienta(titulo: t, simbolo: s, comando: .navegar(d)) }
        func a(_ t: String, _ s: String, _ c: Comando) -> Herramienta { Herramienta(titulo: t, simbolo: s, comando: c) }
        switch self {
        case .free:
            return [
                a("Salir", "rectangle.portrait.and.arrow.right", .salir), n("Configuración", "gearshape", .configuracion),
                n("Configurar Análisis", "slider.horizontal.3", .configuracionAnalisis),
                n("Ayuda", "questionmark.circle", .ayuda), n("Acerca de ...", "info.circle", .acercaDe),
            ]
        case .archivo:
            return [
                a("Guardar equipos", "person.2.badge.gearshape", .guardarEquipos),
                a("Nueva combinación", "doc.badge.plus", .nuevaCombinacion),
                n("Obtener Boletos Online", "icloud.and.arrow.down", .descargaBoleto),
                a("Abrir combinación", "folder", .abrirCombinacion),
                a("Guardar combinación", "square.and.arrow.down", .guardarCombinacion),
                a("Guardar combinación como", "square.and.arrow.down.on.square", .guardarComo),
                a("Borrar archivos temporales", "trash", .borrarTemporales), a("Abrir equipos", "person.2", .abrirEquipos),
                a("Borrar Informes de Error", "doc.badge.ellipsis", .borrarInformes),
                n("Gestión de Equipos", "person.3", .gestorEquipos),
            ]
        case .operaciones:
            return [
                n("Algebra", "plus.forwardslash.minus", .algebraColumnas),
                n("Transposición", "arrow.left.arrow.right", .transposicion), n("Multiplicación", "multiply", .multiplicador),
                n("Fraccionar", "square.split.2x2", .fraccionador),
                n("Rotación de signos", "arrow.triangle.2.circlepath", .rotacionDeSignos),
            ]
        case .combinacion:
            return [
                n("Calcular combinación", "play.fill", .calculaColumnas),
                n("Calcular múltiples combinaciones", "play.square.stack", .calculaColumnasMultiple),
                n("Ver boletos", "list.bullet.rectangle", .verBoletos), n("Imprimir boletos", "printer", .imprimirBoleto),
                n("Reducir", "arrow.down.right.and.arrow.up.left", .reductor), n("Escrutinio", "checkmark.seal", .escrutinios),
                Herramienta(titulo: "Escrutar combinaciones", simbolo: "checkmark.seal.fill", comando: nil),
                n("Análisis de columnas", "tablecells", .analizarFichero),
                n("Análisis de fallos", "exclamationmark.triangle", .colGanadora),
                n("Análisis de signos", "textformat.123", .vSignos), n("Probabilidades", "percent", .probabilidadPremios),
                n("Estadísticas", "chart.bar", .anastatics), n("Añadir pleno al 15", "15.circle", .agregaP15),
            ]
        case .filtros:
            return [
                n("Combinar filtros", "line.3.horizontal.decrease.circle", .combinarFiltros),
                n("Diferencias de filtros", "minus.circle", .diFiltros),
                n("Filtro de coincidencias", "equal.circle", .coincidencias), n("Filtro Aidomnou", "6.circle", .aidomnou),
                n("Filtro Pim", "p.circle", .generaPim),
            ]
        case .utilidades:
            return [
                n("Subir categoría", "arrow.up.circle", .subirCategoria),
                n("Modificador de porcentajes", "slider.horizontal.below.square.and.square.filled", .modificador),
                n("Generador de CPs", "wand.and.stars", .generarCPs),
                n("Diferencias entre columnas", "arrow.left.and.right.square", .difCols),
                n("Ordenar por probabilidad", "arrow.up.arrow.down", .ordenarPorProbabilidad),
                n("Selector JuanM", "j.circle", .selecJM), n("Selector MarioSan", "m.circle", .selectorMS),
                n("Rentabilidad", "eurosign.circle", .rentabilidad), n("Columnas GEPT", "g.circle", .gEPT),
                n("Tramificar", "chart.bar.xaxis", .tramificar), n("Premiadas", "trophy", .premiadas),
                n("Estimación de premios", "chart.line.uptrend.xyaxis", .estimadorPremios),
                n("Banco de pruebas", "testtube.2", .bancoPruebas),
                n("Importar / Exportar", "arrow.up.arrow.down.square", .importExport),
                n("Análisis de grupos", "square.grid.3x3", .anaCombi),
                n("Reducciones perfectas", "checkmark.circle", .reducidasPerfectas),
                n("Dependencia lineal", "function", .dependenciaLineal),
            ]
        }
    }
}

/// La barra: los grupos visibles separados; lo que no cabe va al desbordamiento `»` de macOS.
struct BarraHerramientas: ToolbarContent {
    let sesion: Sesion

    var body: some ToolbarContent {
        ToolbarItemGroup(placement: .primaryAction) {
            ForEach(GrupoBarra.allCases.filter { sesion.barras[safe: $0.indiceFichero] ?? true }, id: \.self) { g in
                ForEach(g.herramientas) { h in
                    Button {
                        if let c = h.comando { sesion.ejecutar(c) }
                    } label: {
                        Label(h.titulo, systemImage: h.simbolo)
                    }
                    .help(h.titulo)
                    .disabled(h.comando == nil)
                }
                Divider()
            }
        }
    }
}
