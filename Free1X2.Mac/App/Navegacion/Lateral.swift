import AppKit
import SwiftUI

/// Una pantalla del panel lateral: `icono` = PNG original de la barra del WinUI (DT-77) o, si no lo había, un SF Symbol.
struct EntradaLateral: Identifiable {
    let titulo: String
    let destino: Destino
    let icono: String
    var id: Destino { destino }
}

/// Panel lateral (DT-82): las pantallas de los menús y de la antigua barra de 55 iconos, por nombre y por grupo, con buscador.
struct PanelLateral: View {
    @Environment(Sesion.self) private var sesion
    @State private var filtro = ""

    static let secciones: [(String, [EntradaLateral])] = {
        func e(_ t: String, _ d: Destino, _ i: String) -> EntradaLateral { EntradaLateral(titulo: t, destino: d, icono: i) }
        return [
            ("", [e("Inicio", .inicio, "house")]),
            (
                "Combinación",
                [
                    e("Calcular columnas", .calculaColumnas, "bCalcular"),
                    e("Calcular varias combinaciones", .calculaColumnasMultiple, "bCalcularM"),
                    e("Ver boletos", .verBoletos, "bVerBoletos"),
                    e("Ver boletos en editor de texto", .verBoletosEnEditor, "doc.plaintext"),
                    e("Imprimir boletos", .imprimirBoleto, "bImprimirBoletos"),
                    e("Reducir", .reductor, "bReducir"),
                    e("Escrutinios", .escrutinios, "bEscrutinio"),
                    e("Escrutar combinaciones", .escrutarCombinaciones, "bEscrutarComb"),
                    e("Máscaras", .mascaras, "theatermasks"),
                    e("Analizar combinación", .analizarCombinacion, "magnifyingglass"),
                    e("Análisis de columnas", .analizarFichero, "bAnalisisColumnas"),
                    e("Análisis de fallos", .colGanadora, "bAnalisisFallos"),
                    e("Análisis de signos", .vSignos, "bAnalisisSignos"),
                    e("Gráfico de columnas", .graficoColumnas, "chart.bar"),
                    e("Probabilidades", .probabilidadPremios, "bProbabilidades"),
                    e("Estadísticas", .anastatics, "bEstadisticas"),
                    e("Añadir pleno al 15", .agregaP15, "bP15"),
                ]
            ),
            (
                "Filtros",
                [
                    e("Combinar filtros", .combinarFiltros, "bCombinarFiltros"),
                    e("Diferencias entre filtros", .diFiltros, "bDiferenciasFiltros"),
                    e("Filtro Coincidencias", .coincidencias, "bFiltroCoincidencias"),
                    e("Filtro Aidomnou", .aidomnou, "bFiltroAidomnou"),
                    e("Filtro Pim", .generaPim, "bFiltroPim"),
                ]
            ),
            (
                "Operaciones",
                [
                    e("Álgebra de columnas", .algebraColumnas, "bAlgebra"),
                    e("Transposición", .transposicion, "bTransposicion"),
                    e("Multiplicador", .multiplicador, "bMultiplicacion"),
                    e("Fraccionador", .fraccionador, "bFraccionador"),
                    e("Rotación de signos", .rotacionDeSignos, "bRotacion"),
                ]
            ),
            (
                "Utilidades",
                [
                    e("Subir categoría", .subirCategoria, "bSubeCategoria"),
                    e("Modificador de porcentajes", .modificador, "bModificadorPct"),
                    e("Generador de CPs", .generarCPs, "bGeneradorCPs"),
                    e("Columnas GEPT", .gEPT, "bColumnasGEPT"),
                    e("Diferencias entre columnas", .difCols, "bDiferenciasColumnas"),
                    e("Ordenar por probabilidad", .ordenarPorProbabilidad, "bProbabilidad"),
                    e("Selector JuanM", .selecJM, "bSelectorJuanM"),
                    e("Selector MarioSan", .selectorMS, "bSelectorMarioSan"),
                    e("Rentabilidad", .rentabilidad, "bRentabilidad"),
                    e("Tramificar", .tramificar, "bTramificar"),
                    e("Premiadas", .premiadas, "bPremiadas"),
                    e("Estimación de premios", .estimadorPremios, "bEstimacion"),
                    e("Banco de pruebas", .bancoPruebas, "bBancoPruebas"),
                    e("Análisis de grupos", .anaCombi, "bAnalisisGrupos"),
                    e("Reducciones perfectas", .reducidasPerfectas, "bRedPerfectas"),
                    e("Dependencia lineal", .dependenciaLineal, "bDependenciaLineal"),
                    e("Compresor z3q", .compresor, "archivebox"),
                    e("EstuCol", .estucol, "tablecells"),
                ]
            ),
            (
                "Archivo",
                [
                    e("Gestión de equipos", .gestorEquipos, "bGestorEquipos"),
                    e("Importar / exportar columnas", .importExport, "bImportExport"),
                    e("Obtener boletos online", .descargaBoleto, "bObtenerBoletos"),
                ]
            ),
            (
                "Ajustes y ayuda",
                [
                    e("Configuración", .configuracion, "bConfig"),
                    e("Configurar análisis", .configuracionAnalisis, "bConfAnalisis"),
                    e("Listado de condiciones", .listadoCondiciones, "list.bullet.rectangle"),
                    e("Ayuda", .ayuda, "bAyuda"),
                    e("Acerca de", .acercaDe, "bAcercaDe"),
                ]
            ),
        ]
    }()

    var body: some View {
        // Marca la pantalla raíz de la pila: en una subpantalla (p. ej. una condición abierta desde Inicio) no marca nada.
        let seleccion = Binding<Destino?>(
            get: { sesion.router.pila.first ?? .inicio },
            set: { if let d = $0 { sesion.router.ir(d) } })
        List(selection: seleccion) {
            ForEach(Self.secciones, id: \.0) { nombre, entradas in
                let visibles = filtro.isEmpty ? entradas : entradas.filter { $0.titulo.localizedStandardContains(filtro) }
                if !visibles.isEmpty {
                    Section {
                        ForEach(visibles) { fila($0) }
                    } header: {
                        if !nombre.isEmpty { Text(nombre) }
                    }
                }
            }
        }
        .searchable(text: $filtro, placement: .sidebar, prompt: "Buscar pantalla")
        .navigationSplitViewColumnWidth(min: 190, ideal: 230, max: 320)
    }

    private func fila(_ e: EntradaLateral) -> some View {
        Label {
            Text(e.titulo)
        } icon: {
            if let png = NSImage(named: e.icono) {
                // Casi todos miden 16 px (nítidos sin suavizar); tres miden 24 o 32 y se reducen suavizados.
                Image(nsImage: png).resizable().interpolation(png.size.width > 16 ? .high : .none).frame(width: 16, height: 16)
            } else {
                Image(systemName: e.icono).foregroundStyle(Color.acento)
            }
        }
        .tag(e.destino)
        .accessibilityIdentifier("lateral-\(e.destino.rawValue)")
    }
}
