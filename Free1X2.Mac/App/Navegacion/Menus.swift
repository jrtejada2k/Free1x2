import AppKit
import SwiftUI

/// Barra de menús (ESPEC_15 §2.2; 10 §1.3) y atajos (§5). Todo emite `Comando` sobre la `Sesion`.
struct Menus: Commands {
    let sesion: Sesion

    private func item(_ t: String, _ d: Destino) -> some View {
        Button(t) { sesion.ejecutar(.navegar(d)) }
    }

    private func tecla(_ f: Int) -> KeyEquivalent { KeyEquivalent(Character(UnicodeScalar(f) ?? " ")) }

    var body: some Commands {
        CommandGroup(replacing: .appInfo) {
            item("Acerca de Free1X2", .acercaDe)
            item("Créditos…", .creditos)
        }
        CommandGroup(replacing: .appSettings) {
            item("Ajustes…", .configuracion).keyboardShortcut(",")
            item("Configurar análisis…", .configuracionAnalisis)
        }
        CommandGroup(replacing: .newItem) {
            Button("Nueva combinación") { sesion.ejecutar(.nuevaCombinacion) }.keyboardShortcut("n")
            Button("Abrir combinación…") { sesion.ejecutar(.abrirCombinacion) }.keyboardShortcut("o")
        }
        CommandGroup(replacing: .saveItem) {
            Button("Guardar combinación") { sesion.ejecutar(.guardarCombinacion) }.keyboardShortcut("s")
            Button("Guardar combinación como…") { sesion.ejecutar(.guardarComo) }.keyboardShortcut("s", modifiers: [.command, .shift])
            Divider()
            Button("Abrir equipos…") { sesion.ejecutar(.abrirEquipos) }
            Button("Guardar equipos…") { sesion.ejecutar(.guardarEquipos) }
            Divider()
            item("Boleto / combinación (Inicio)", .inicio)
            item("Gestión de equipos…", .gestorEquipos)
            item("Importar / exportar columnas…", .importExport)
            item("Obtener boletos online…", .descargaBoleto)
            Divider()
            Button("Borrar temporales…") { sesion.ejecutar(.borrarTemporales) }
            Button("Borrar informes…") { sesion.ejecutar(.borrarInformes) }
        }
        CommandGroup(replacing: .printItem) {}
        CommandGroup(before: .toolbar) {
            item("Inicio", .inicio)
            item("Ver boletos…", .verBoletos)
            item("Gráfico de columnas…", .graficoColumnas)
            item("Estadísticas…", .anastatics)
            item("Configuración…", .configuracion)
            Divider()
            Menu("Barras de herramientas") {
                ForEach(GrupoBarra.ordenMenu, id: \.self) { g in
                    Toggle(
                        g.nombre,
                        isOn: Binding(
                            get: { sesion.barras[safe: g.indiceFichero] ?? true },
                            set: { if sesion.barras.indices.contains(g.indiceFichero) { sesion.barras[g.indiceFichero] = $0 } }))
                }
            }
            item("Listado de condiciones", .listadoCondiciones)
            Divider()
            Picker("Tema", selection: Binding(get: { sesion.tema }, set: { sesion.tema = $0 })) {
                Text("Claro").tag("Claro")
                Text("Oscuro").tag("Oscuro")
                Text("Sistema").tag("Sistema")
            }
            Picker("Idioma", selection: Binding(get: { sesion.idioma }, set: { sesion.idioma = $0 })) {
                Text("Español").tag("Espanol")
                Text("English").tag("Ingles")
            }
            Divider()
            Button("Atrás") { sesion.router.atras() }.keyboardShortcut("[")
            Divider()
        }
        CommandMenu("Combinación") {
            item("Calcular…", .calculaColumnas).keyboardShortcut(tecla(NSF5FunctionKey), modifiers: [])
            item("Calcular varias…", .calculaColumnasMultiple)
            item("Ver boletos…", .verBoletos)
            item("Ver boletos en editor de texto…", .verBoletosEnEditor)
            item("Imprimir boletos…", .imprimirBoleto)
            item("Reducir…", .reductor)
            item("Escrutinios…", .escrutinios)
            item("Escrutar combinaciones…", .escrutarCombinaciones)
            item("Analizar combinación…", .analizarCombinacion)
            item("Gráfico de columnas…", .graficoColumnas)
            item("Probabilidades…", .probabilidadPremios)
            item("Estadísticas…", .anastatics)
            item("Añadir Pleno al 15…", .agregaP15)
        }
        CommandMenu("Filtros") {
            item("Combinar filtros…", .combinarFiltros)
            item("Diferencias entre filtros…", .diFiltros)
            item("Filtro Coincidencias…", .coincidencias)
            item("Filtro Aidomnou…", .aidomnou)
            item("Filtro Pim…", .generaPim)
        }
        CommandMenu("Operaciones") {
            item("Álgebra de columnas…", .algebraColumnas)
            item("Transposición…", .transposicion)
            item("Multiplicador…", .multiplicador)
            item("Fraccionador…", .fraccionador)
            item("Rotación de signos…", .rotacionDeSignos)
        }
        CommandMenu("Utilidades") {
            item("Sube categoría…", .subirCategoria)
            item("Modificador %…", .modificador)
            Divider()
            item("Generador CP…", .generarCPs)
            item("Columnas GEPT…", .gEPT)
            Divider()
            item("Diferencias entre columnas…", .difCols)
            item("Ordenar por probabilidad…", .ordenarPorProbabilidad)
            Divider()
            item("Selector JuanM…", .selecJM)
            item("Selector MarioSan…", .selectorMS)
            Divider()
            item("Rentabilidad…", .rentabilidad)
            item("Tramificar…", .tramificar)
            item("Premiadas…", .premiadas)
            item("Estimación de premios…", .estimadorPremios)
            item("Banco de pruebas…", .bancoPruebas)
            Divider()
            item("Compresor z3q…", .compresor)
            item("EstuCol…", .estucol)
        }
        CommandGroup(replacing: .help) {
            item("Recursos de ayuda", .ayuda).keyboardShortcut("?")
            item("Ayuda (F1)", .ayuda).keyboardShortcut(tecla(NSF1FunctionKey), modifiers: [])
        }
    }
}
