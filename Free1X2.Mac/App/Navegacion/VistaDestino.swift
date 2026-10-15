import SwiftUI

/// Una vista por `Destino` (ESPEC_15 §7).
struct VistaDestino: View {
    let destino: Destino

    var body: some View {
        switch destino {
        case .inicio: VistaInicio()
        case .acercaDe: VistaAcercaDe()
        case .ayuda: VistaAyuda()
        case .creditos: VistaCreditos()
        case .configuracion: VistaConfiguracion()
        case .configuracionAnalisis: VistaConfiguracionAnalisis()
        case .cambioPuntos: VistaCambioPuntos()
        case .visorEstadisticas: VistaVisorEstadisticas()
        case .noVariantes: VistaNoVariantes()
        case .signosSeguidos: VistaSignosSeguidos()
        case .figurasFiltros: VistaFigurasFiltros()
        case .simetrias: VistaSimetrias()
        case .pesosNum: VistaPesosNumericos()
        case .valoracion: VistaValoracion()
        case .distancias: VistaDistancias()
        case .interrupciones: VistaInterrupciones()
        case .contactos: VistaContactos()
        case .dibujos: VistaDibujos()
        case .diferencias: VistaDiferencias()
        case .formatos123: VistaFormatos123()
        case .formatos: VistaFormatos()
        case .gruposEquipos: VistaGruposEquipos()
        case .ifThen: VistaIfThen()
        case .controlGrupos: VistaControlGrupos()
        case .controlTol: VistaControlTol()
        case .colProbables: VistaColProbables()
        case .copiarDatosCP: VistaCopiarDatosCP()
        case .copiarCP: VistaCopiarCP()
        case .importadorCPs: VistaImportadorCPs()
        case .exportadorCPs: VistaExportadorCPs()
        case .calculaColumnas: VistaCalculaColumnas()
        case .visorAnalisisColumnas: VistaVisorAnalisisColumnas()
        case .reductor: VistaReductor()
        case .escrutinios: VistaEscrutinios()
        case .columnasPremiadas: VistaColumnasPremiadas()
        case .crearGrupos: VistaCrearGrupos()
        case .calculaColumnasMultiple: VistaCalculaColumnasMultiple()
        case .resultadosCalculoMultiple: VistaResultadosCalculoMultiple()
        case .analizarFichero: VistaAnalizarFichero()
        case .gestorEquipos: VistaGestorEquipos()
        case .agregarEquipo: VistaAgregarEquipo()
        case .descargaBoleto: VistaDescargaBoleto()
        case .colGanadora: VistaColGanadora()
        case .analizarCombinacion: VistaAnalizarCombinacion()
        case .verBoletos: VistaVerBoletos()
        case .boleto: VistaBoleto()
        case .verBoletosEnEditor: VistaVerBoletosEnEditor()
        default: PantallaPendiente(destino: destino)
        }
    }
}

/// ponytail: hueco de las pantallas que aún no están portadas (M5 en curso); desaparece con la última.
struct PantallaPendiente: View {
    let destino: Destino

    var body: some View {
        Pagina(destino: destino) {
            ContentUnavailableView(destino.titulo, systemImage: "hammer", description: Text(destino.clase))
        }
    }
}
