// Las 110 superficies (ESPEC_15 §7): Inicio + 108 páginas del original + Máscaras (DT-84). El orden es el del smoke (10 §1.8).
// Generado desde la tabla de ESPEC_15 §7 y los títulos de `Views/Ported/*Page.xaml`.

enum Destino: String, Hashable, CaseIterable, Identifiable {
    case inicio
    case agregaP15
    case agregarEquipo
    case algebraColumnas
    case anaCombi
    case analisisFormatos123
    case analizadorJPM
    case analizarCombinacion
    case analizarFichero
    case anastatics
    case bancoPruebas
    case boleto
    case buscaLims
    case calculaColumnas
    case calculaColumnasMultiple
    case calculoFormatos
    case cambioPuntos
    case coincidencias
    case colGanadora
    case colProbables
    case columnasPremiadas
    case combinarFiltros
    case compresor
    case configCPs
    case configuracionAnalisis
    case configuracion
    case contactos
    case controlGrupos
    case controlTol
    case copiarCP
    case copiarDatosCP
    case crearGrupos
    case descargaBoleto
    case diFiltros
    case dialogoAnalisisMultipleDeTramos
    case dialogoFiltrarPorLimites
    case dialogoGrabarBancoPruebas
    case dialogoGrabarTramos
    case dib
    case dibRep
    case dibujos
    case difCols
    case diferencias
    case distancias
    case escrutarCombinaciones
    case escrutinios
    case estimadorPremios
    case estucol
    case exportadorCPs
    case figurasFiltros
    case filtroPorcenJB
    case formatos123
    case formatos
    case fraccionador
    case dependenciaLineal
    case reducidasPerfectas
    case gEPT
    case generaPim
    case generadorCPSDiferencias
    case generarCPs
    case gestorEquipos
    case graficoColumnas
    case gruposEquipos
    case guardarValoracion
    case historiaValoraciones
    case ifThen
    case importExport
    case importadorCPs
    case imprimirBoleto
    case interrupciones
    case listaImpresoras
    case listadoCondiciones
    case mejoresOpciones
    case modificador
    case multiplicador
    case noVariantes
    case ordenarPorProbabilidad
    case parejas
    case pesosNum
    case posiblesPremios
    case premiadas
    case probabilidadPremios
    case reductor
    case rentabilidad
    case resultadosCalculoMultiple
    case rotacionDeSignos
    case selecJM
    case selectorMS
    case signosSeguidos
    case simetrias
    case staInter
    case staSS
    case subirCategoria
    case tramificar
    case tramificarGraficas
    case transposicion
    case trios
    case vSignos
    case valoracion
    case verBoletos
    case verBoletosEnEditor
    case visorAnalisisColumnasAbdon
    case visorAnalisisColumnas
    case visorEstadisticas
    case visorPosiblesPremios
    case aidomnou
    case acercaDe
    case ayuda
    case creditos
    /// Nueva (DT-84): máscaras guardadas y su escrutinio histórico; no existe en el WinUI.
    case mascaras

    var id: String { rawValue }

    /// Título de la página (el de su `*.xaml` actual).
    var titulo: String {
        switch self {
        case .inicio: "Inicio"
        case .agregaP15: "Añade P15 (Indeciso)"
        case .agregarEquipo: "Añadir Equipo"
        case .algebraColumnas: "Álgebra"
        case .anaCombi: "Análisis de grupos en combinación"
        case .analisisFormatos123: "Analizador Formatos123"
        case .analizadorJPM: "Sumas Pares Naturales (JPM)"
        case .analizarCombinacion: "Analizar combinación"
        case .analizarFichero: "Analizar fichero de columnas"
        case .anastatics: "Estadísticas 0.3.4"
        case .bancoPruebas: "Simulador de escrutinios (Banco de Pruebas)"
        case .boleto: "Boleto"
        case .buscaLims: "Buscador límites"
        case .calculaColumnas: "Calcular columnas"
        case .calculaColumnasMultiple: "Calcular Múltiples Combinaciones"
        case .calculoFormatos: "Saca Formatos (Indeciso)"
        case .cambioPuntos: "Cambiar Puntuación"
        case .coincidencias: "Coincidencias"
        case .colGanadora: "Análisis de fallos en combinación"
        case .colProbables: "Columnas Probables"
        case .columnasPremiadas: "Columnas Premiadas"
        case .combinarFiltros: "Combinación de Filtros"
        case .compresor: "Compresor *.z3q"
        case .configCPs: "Configurar Columnas Probables"
        case .configuracionAnalisis: "Configurar Análisis"
        case .configuracion: "Configuración"
        case .contactos: "Contactos"
        case .controlGrupos: "Control de Grupos"
        case .controlTol: "Control Tolerancias"
        case .copiarCP: "Copiar Columnas"
        case .copiarDatosCP: "Copiar Datos"
        case .crearGrupos: "Crear nuevos grupos"
        case .descargaBoleto: "Descarga de boleto"
        case .diFiltros: "Diferencias entre filtros"
        case .dialogoAnalisisMultipleDeTramos: "Definición múltiple de jornadas y ficheros"
        case .dialogoFiltrarPorLimites: "Filtrar por límites"
        case .dialogoGrabarBancoPruebas: "Grabar apuestas"
        case .dialogoGrabarTramos: "Grabar Tramos"
        case .dib: "Dibujos"
        case .dibRep: "Coincidencias"
        case .dibujos: "Dibujos X+2"
        case .difCols: "Diferencias entre columnas"
        case .diferencias: "Diferencias"
        case .distancias: "Distancias"
        case .escrutarCombinaciones: "Escrutar combinaciones"
        case .escrutinios: "Escrutinios"
        case .estimadorPremios: "Estimación de premios"
        case .estucol: "EstuCol - Generador / Analizador de Columnas Probables"
        case .mascaras: "Máscaras"
        case .exportadorCPs: "Exportador de Columnas Probables"
        case .figurasFiltros: "Figuras"
        case .filtroPorcenJB: "Separador Porcentajes Juan Bellas"
        case .formatos123: "Formatos 123"
        case .formatos: "Formatos (1, X, 2, V, *)"
        case .fraccionador: "Fraccionador"
        case .dependenciaLineal: "Dependencia Lineal"
        case .reducidasPerfectas: "Reducciones Perfectas"
        case .gEPT: "Columna estadística de ABDON (GEPT)"
        case .generaPim: "Filtro Pim"
        case .generadorCPSDiferencias: "Generador de CPs por diferencias"
        case .generarCPs: "Generador de Columnas Probables"
        case .gestorEquipos: "Gestión de Equipos"
        case .graficoColumnas: "Representación gráfica de la combinación"
        case .gruposEquipos: "Grupos de Equipos"
        case .guardarValoracion: "Guardar Valoración"
        case .historiaValoraciones: "Valoraciones históricas"
        case .ifThen: "Condiciones relacionadas (if-then)"
        case .importExport: "Importar / Exportar columnas"
        case .importadorCPs: "Importador de Columnas Probables"
        case .imprimirBoleto: "Impresión de boletos"
        case .interrupciones: "Interrupciones"
        case .listaImpresoras: "Lista Impresoras Conocidas"
        case .listadoCondiciones: "Listado de Condiciones"
        case .mejoresOpciones: "Mis Mejores Opciones"
        case .modificador: "Modificador de columnas"
        case .multiplicador: "Multiplicador"
        case .noVariantes: "Número de Variantes"
        case .ordenarPorProbabilidad: "Ordenación de columnas por probabilidad"
        case .parejas: "Pares"
        case .pesosNum: "Pesos Numéricos"
        case .posiblesPremios: "Posibles Premios"
        case .premiadas: "Análisis Premiadas"
        case .probabilidadPremios: "Probabilidades de Premios"
        case .reductor: "Reductor"
        case .rentabilidad: "Rentabilidad"
        case .resultadosCalculoMultiple: "Resultados de la Generación Múltiple"
        case .rotacionDeSignos: "Rotación de signos"
        case .selecJM: "Selección 5+5+4 de JuanM"
        case .selectorMS: "Selector MarioSan"
        case .signosSeguidos: "Signos Seguidos"
        case .simetrias: "Simetrías"
        case .staInter: "Interrupciones"
        case .staSS: "Signos seguidos 0.1"
        case .subirCategoria: "Subir Categoría"
        case .tramificar: "Tramificar"
        case .tramificarGraficas: "Gráficas de los resultados del análisis de tramos"
        case .transposicion: "Transposición de columnas"
        case .trios: "Tríos"
        case .vSignos: "Análisis de signos"
        case .valoracion: "Valoración"
        case .verBoletos: "Ver Boletos"
        case .verBoletosEnEditor: "Ver Boletos En Editor de Texto"
        case .visorAnalisisColumnasAbdon: "Visor de Informe"
        case .visorAnalisisColumnas: "Análisis"
        case .visorEstadisticas: "Estadísticas"
        case .visorPosiblesPremios: "Visor de Posibles Premios"
        case .aidomnou: "Filtro Aidomnou"
        case .acercaDe: "Acerca de"
        case .ayuda: "Recursos de ayuda"
        case .creditos: "Equipo Free1X2"
        }
    }

    /// Clase de la pantalla actual, para el log del smoke.
    var clase: String {
        switch self {
        case .inicio: "MainPage"
        case .agregaP15: "AgregaP15Frm"
        case .agregarEquipo: "AgregarEquipoFrm"
        case .algebraColumnas: "AlgebraColumnasFrm"
        case .anaCombi: "AnaCombi"
        case .analisisFormatos123: "AnalisisFormatos123Frm"
        case .analizadorJPM: "AnalizadorJPM"
        case .analizarCombinacion: "AnalizarCombinacionFrm"
        case .analizarFichero: "AnalizarFicheroFrm"
        case .anastatics: "Anastatics"
        case .bancoPruebas: "BancoPruebasFrm"
        case .boleto: "BoletoFrm"
        case .buscaLims: "BuscaLimsFrm"
        case .calculaColumnas: "CalculaColumnasFrm"
        case .calculaColumnasMultiple: "CalculaColumnasMultipleFrm"
        case .calculoFormatos: "CalculoFormatosFrm"
        case .cambioPuntos: "CambioPuntosFrm"
        case .coincidencias: "Coincidencias"
        case .colGanadora: "ColGanadoraFrm"
        case .colProbables: "ColProbablesFrm"
        case .columnasPremiadas: "ColumnasPremiadasFrm"
        case .combinarFiltros: "CombinarFiltros"
        case .compresor: "Compresor"
        case .configCPs: "ConfigCPsFrm"
        case .configuracionAnalisis: "ConfiguracionAnalisisFrm"
        case .configuracion: "ConfiguracionFrm"
        case .contactos: "ContactosFrm"
        case .controlGrupos: "ControlGruposFrm"
        case .controlTol: "ControlTolFrm"
        case .copiarCP: "CopiarCPFrm"
        case .copiarDatosCP: "CopiarDatosCPFrm"
        case .crearGrupos: "CrearGruposFrm"
        case .descargaBoleto: "DescargaBoletoFrm"
        case .diFiltros: "DiFiltros"
        case .dialogoAnalisisMultipleDeTramos: "DialogoAnalisisMultipleDeTramosFrm"
        case .dialogoFiltrarPorLimites: "DialogoFiltrarPorLimitesFrm"
        case .dialogoGrabarBancoPruebas: "DialogoGrabarBancoPruebasFrm"
        case .dialogoGrabarTramos: "DialogoGrabarTramosFrm"
        case .dib: "DibForm"
        case .dibRep: "DibRepFrm"
        case .dibujos: "DibujosFrm"
        case .difCols: "DifCols"
        case .diferencias: "DiferenciasFrm"
        case .distancias: "DistanciasFrm"
        case .escrutarCombinaciones: "EscrutarCombinacionesFrm"
        case .escrutinios: "EscrutiniosFrm"
        case .estimadorPremios: "EstimadorPremiosFrm"
        case .estucol: "EstucolFrm"
        case .mascaras: "MascarasPage"
        case .exportadorCPs: "ExportadorCPsFrm"
        case .figurasFiltros: "FigurasFiltrosFrm"
        case .filtroPorcenJB: "FiltroPorcenJB"
        case .formatos123: "Formatos123Frm"
        case .formatos: "FormatosFrm"
        case .fraccionador: "FraccionadorFrm"
        case .dependenciaLineal: "FrmDependenciaLineal"
        case .reducidasPerfectas: "FrmReducidasPerfectas"
        case .gEPT: "GEPTFrm"
        case .generaPim: "GeneraPim"
        case .generadorCPSDiferencias: "GeneradorCPSDiferencias"
        case .generarCPs: "GenerarCPs"
        case .gestorEquipos: "GestorEquiposFrm"
        case .graficoColumnas: "GraficoColumnasFrm"
        case .gruposEquipos: "GruposEquiposFrm"
        case .guardarValoracion: "GuardarValoracionFrm"
        case .historiaValoraciones: "HistoriaValoracionesFrm"
        case .ifThen: "IfThenFrm"
        case .importExport: "ImportExportFrm"
        case .importadorCPs: "ImportadorCPsFrm"
        case .imprimirBoleto: "ImprimirBoletoFrm"
        case .interrupciones: "InterrupcionesFrm"
        case .listaImpresoras: "ListaImpresoras"
        case .listadoCondiciones: "ListadoCondicionesFrm"
        case .mejoresOpciones: "MejoresOpcionesFrm"
        case .modificador: "ModificadorFrm"
        case .multiplicador: "MultiplicadorFrm"
        case .noVariantes: "NoVariantesFrm"
        case .ordenarPorProbabilidad: "OrdenarPorProbabilidadFrm"
        case .parejas: "ParejasFrm"
        case .pesosNum: "PesosNumFrm"
        case .posiblesPremios: "PosiblesPremiosFrm"
        case .premiadas: "PremiadasFrm"
        case .probabilidadPremios: "ProbabilidadPremios"
        case .reductor: "ReductorFrm"
        case .rentabilidad: "RentabilidadFrm"
        case .resultadosCalculoMultiple: "ResultadosCalculoMultipleFrm"
        case .rotacionDeSignos: "RotacionDeSignosFrm"
        case .selecJM: "SelecJM"
        case .selectorMS: "SelectorMS"
        case .signosSeguidos: "SignosSeguidosFrm"
        case .simetrias: "SimetriasFrm"
        case .staInter: "StaInterFrm"
        case .staSS: "StaSSForm"
        case .subirCategoria: "SubirCategoriaFrm"
        case .tramificar: "TramificarForm"
        case .tramificarGraficas: "TramificarGraficasFrm"
        case .transposicion: "TransposicionFrm"
        case .trios: "TriosFrm"
        case .vSignos: "VSignosFrm"
        case .valoracion: "ValoracionFrm"
        case .verBoletos: "VerBoletos"
        case .verBoletosEnEditor: "VerBoletosEnEditorFrm"
        case .visorAnalisisColumnasAbdon: "VisorAnalisisColumnasAbdonFrm"
        case .visorAnalisisColumnas: "VisorAnalisisColumnasFrm"
        case .visorEstadisticas: "VisorEstadisticas"
        case .visorPosiblesPremios: "VisorPosiblesPremios"
        case .aidomnou: "aidomnou"
        case .acercaDe: "AcercaDeFrm"
        case .ayuda: "AyudaFrm"
        case .creditos: "CreditosFrm"
        }
    }
}
