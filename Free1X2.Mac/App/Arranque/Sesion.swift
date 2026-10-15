import AppKit
import Free1X2CasosDeUso
import Observation

/// Lo que menús, barra y atajos piden (≡ `AccionInicio` + navegación, 10 §1.2; ESPEC_11 §8.2).
enum Comando: Hashable {
    case nuevaCombinacion, abrirCombinacion, guardarCombinacion, guardarComo, abrirEquipos, guardarEquipos
    case borrarTemporales, borrarInformes, salir
    case navegar(Destino)
}

/// Estado de la app (≡ `AppState`, ESPEC_11 §8.1) y el boleto base de Inicio (`BoletoBaseViewModel`).
@MainActor @Observable
final class Sesion {
    let entorno: Entorno
    let router = Router()
    let alertas = Alertas()
    /// DP-04: leída una vez al arrancar.
    let configuracion: Configuracion
    var combinacion: Combinacion
    /// `GrupoPantalla`; `grupoActual` lo reajusta a 0 fuera de rango (07:1162).
    var grupoPantalla = 0
    /// `NombreArchivoComb`.
    var rutaCombinacion: URL?
    /// `ArchivoFiltroCols` (ruta tal cual, como la guarda el `.comb`).
    var archivoFiltroCols = ""
    var jornadaActual: JornadaQuiniela?
    /// Local y visitante de cada partido tal como se escriben (sin recortar, B-03).
    var locales: [String]
    var visitantes: [String]
    /// `AppState.GrupoEnEdicion`: el grupo que edita la pantalla de condición abierta desde Inicio.
    var grupoEnEdicion = 0
    /// `AppState.UsarAnalizadorCompartido`: «Calcular» desde Inicio usa la combinación en pantalla.
    var usarCombinacionCompartida = false
    var barras: [Bool]
    /// `VisorEstadisticasViewModel.UltimasEstadisticas` (no se borra al leerla).
    var ultimasEstadisticas: [Estadistica]?
    /// `FigurasFiltrosFrmViewModel.FigurasEnEdicion`/`NombreCondicion` (DT-58: copia; `resultado` al aceptar).
    var figuras: FigurasEnEdicion?
    /// `GuardarValoracionFrmPage.ValoresIniciales`.
    var valoracionInicial: [[Double]]?
    /// `AnalisisFormatos123FrmPage.FormatosEntrada`.
    var formatos123Entrada: [Formato123]?
    /// Estáticos de las pantallas 30, 67 y 48 con «Columnas Probables» (`Resultado`, `ListaParaExportar`).
    var traspasoCP = TraspasoCP()
    /// `VisorAnalisisColumnasFrmViewModel.UltimoContenedor/UltimoGrupo` (`AnalisisUi.MostrarVisor`).
    var visorAnalisis: (contenedor: ContenedorAnalisisGlobal, grupo: GrupoConfig)?
    /// `ColumnasPremiadasFrmPage.Entrada`.
    var premiadasEntrada: [FilaPremiada]?
    /// `ResultadosCalculoMultipleFrmViewModel.UltimosResultados`.
    var resultadosLote: [ResumenLote]?
    /// `TemaApp` (10 §1.5): «Sistema» (defecto), «Claro», «Oscuro»; en `UserDefaults` (ESPEC_11 §9).
    var tema = UserDefaults.standard.string(forKey: "tema") ?? "Sistema" {
        didSet {
            UserDefaults.standard.set(tema, forKey: "tema")
            aplicarTema()
        }
    }
    /// `IdiomaApp`: «Espanol» (defecto) o «Ingles»; solo Créditos está localizada (10 §1.5).
    var idioma = UserDefaults.standard.string(forKey: "idioma") ?? "Espanol" {
        didSet { UserDefaults.standard.set(idioma, forKey: "idioma") }
    }

    var numeroPartidos: Int { configuracion.boleto.numeroPartidos }

    init(entorno: Entorno) {
        self.entorno = entorno
        let inicial = CasosInicio.arrancar(entorno: entorno)
        configuracion = inicial.configuracion
        jornadaActual = inicial.jornada
        let n = inicial.configuracion.boleto.numeroPartidos
        combinacion = Combinacion(numeroPartidos: n)
        locales = Array(repeating: "", count: n)
        visitantes = Array(repeating: "", count: n)
        barras = CasosInicio.leerBarrasVisibles(inicial.configuracion)
    }

    func aplicarTema() {
        NSApplication.shared.appearance =
            switch tema {
            case "Claro": NSAppearance(named: .aqua)
            case "Oscuro": NSAppearance(named: .darkAqua)
            default: nil
            }
    }

    var grupoActual: Int {
        combinacion.grupos.indices.contains(grupoPantalla) ? grupoPantalla : 0
    }

    // MARK: Boleto

    func signos(_ i: Int) -> [Bool] { CasosInicio.signos(combinacion.pronosticos?[safe: i] ?? nil) }

    /// Cambiar un signo vuelca **todo** el boleto al motor (`AlCambiarSigno`).
    func alternar(_ i: Int, _ s: Int) {
        var todos = (0..<numeroPartidos).map(signos)
        todos[i][s].toggle()
        combinacion.ponerPronosticos(todos.map(CasosInicio.pronostico))
    }

    /// `VolcarPronosticosAlMotor`.
    func volcarPronosticos() {
        combinacion.ponerPronosticos((0..<numeroPartidos).map { CasosInicio.pronostico(signos($0)) })
    }

    func reiniciarTriples() {
        combinacion.ponerPronosticos(Array(repeating: "1,X,2", count: numeroPartidos))
    }

    /// `DevolverEquipos`.
    var equipos: [String] {
        (0..<numeroPartidos).map { CasosInicio.normalizarEquipos(local: locales[$0], visitante: visitantes[$0]) }
    }

    /// `SetEquipos`.
    func ponerEquipos(_ e: [String]) {
        let actuales = (0..<numeroPartidos).map { CasosInicio.componerEquipos(local: locales[$0], visitante: visitantes[$0]) }
        for (i, x) in CasosInicio.aplicarEquipos(e, a: actuales).enumerated() {
            (locales[i], visitantes[i]) = CasosInicio.dividirEquipos(x)
        }
    }

    // MARK: Comandos

    func ejecutar(_ c: Comando) {
        Task { await ejecutarAsync(c) }
    }

    /// `NavegarConAccion`: las acciones de Inicio se ejecutan sobre la combinación viva; los errores van a `Alertas`.
    func ejecutarAsync(_ c: Comando) async {
        do {
            switch c {
            case .navegar(let d): router.navegar(d)
            case .salir: NSApplication.shared.terminate(nil)
            case .nuevaCombinacion: nuevaCombinacion()
            case .abrirCombinacion: try await abrirCombinacion()
            case .guardarCombinacion: try await guardarCombinacion()
            case .guardarComo:
                guard let url = await SelectorFicheros.guardar("Combinacion", [("Combinación", ".comb")]) else { return }
                rutaCombinacion = url
                try await guardarCombinacion()
            case .abrirEquipos:
                guard let url = await SelectorFicheros.abrir(".txt", "*") else { return }
                ponerEquipos(try CasosInicio.abrirEquipos(url, numeroPartidos: numeroPartidos, entorno: entorno))
            case .guardarEquipos:
                guard let url = await SelectorFicheros.guardar("Equipos", [("Equipos", ".txt")]) else { return }
                try CasosInicio.guardarEquipos(equipos, en: url, entorno: entorno)
            case .borrarTemporales:
                guard await alertas.confirmar("¿Borrar las combinaciones temporales?") else { return }
                _ = try CasosInicio.borrarTemporales(entorno: entorno)
            case .borrarInformes:
                guard await alertas.confirmar("¿Borrar todos los Informes generados?") else { return }
                _ = try CasosInicio.borrarInformes(entorno: entorno)
            }
        } catch {
            alertas.error(error)
        }
    }

    func nuevaCombinacion() {
        let n = CasosInicio.nuevaCombinacion(numeroPartidos: numeroPartidos)
        combinacion = n.combinacion
        grupoPantalla = 0
        rutaCombinacion = nil
        archivoFiltroCols = ""
        ponerEquipos(n.equipos)
    }

    func abrirCombinacion() async throws {
        guard let url = await SelectorFicheros.abrir(".comb", ".xml") else { return }
        let c = try CasosInicio.abrirCombinacion(url, numeroPartidos: numeroPartidos, entorno: entorno)
        combinacion = c.combinacion
        archivoFiltroCols = c.filtroColumnas
        rutaCombinacion = url
        grupoPantalla = 0
        ponerEquipos(c.equipos)
    }

    func guardarCombinacion() async throws {
        volcarPronosticos()
        var destino = rutaCombinacion
        if destino == nil {
            destino = await SelectorFicheros.guardar("Combinacion", [("Combinación", ".comb")])
        }
        guard let destino else { return }
        try CasosInicio.guardarCombinacion(
            combinacion, equipos: equipos, filtroColumnas: archivoFiltroCols, en: destino, entorno: entorno)
        rutaCombinacion = destino
    }

    /// Al cerrar la ventana (`GuardarBarrasHerramientas`): un fallo solo se registra.
    func guardarBarras() {
        do throws(ErrorDominio) {
            try CasosInicio.guardarBarrasVisibles(barras, entorno: entorno)
        } catch {
            entorno.registro.registrar("GuardarToolBarsVisibles: \(error.mensajeUsuario)", nivel: .error)
        }
    }
}

/// Lista de figuras que edita la pantalla 49 por encargo de una condición.
struct FigurasEnEdicion {
    var lista: [Int64]
    let condicion: String
    /// Cuál de las listas del padre (0…3 en Signos Seguidos).
    let clave: Int
    var resultado: [Int64]?
}

/// Ida y vuelta de «Columnas Probables» con Copiar datos (30), Importador (67) y Exportador (48).
struct TraspasoCP {
    /// `(Desde, Hasta)` con que se abre la pantalla 30.
    var rango = (desde: 1, hasta: 1)
    /// `CopiarDatosCPFrmViewModel.Resultado`; `nil` = sin confirmar.
    var copia: (desde: Int, hasta: Int)?
    /// `ImportadorCPsFrmViewModel.Resultado`.
    var importadas: [ColumnaProbableConfig]?
    /// `ExportadorCPsFrmViewModel.ListaParaExportar`.
    var exportar: [ColumnaProbableConfig] = []
}
