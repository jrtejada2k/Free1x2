using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del port WinUI 3 del WinForms <c>CalculaColumnasMultipleFrm</c>.
///
/// Propósito (legacy): procesar EN LOTE varios archivos de combinaciones
/// (*.comb / *.xml) de la carpeta "Combinaciones", aplicar el reductor/analizador
/// a cada uno y volcar las columnas resultantes en la carpeta "Columnas" como .txt.
/// Durante el proceso muestra estadísticas en vivo (columnas procesadas/aceptadas,
/// coste, porcentaje, estimadas, máximas) y tiempos de comienzo/fin.
///
/// Toda la lógica de dominio (Analizador, ArchivoCombinacion, ArchivoColumnasTexto,
/// VariablesGlobales, ResultadosCalculoMultipleFrm) queda marcada como TODO.
/// </summary>
public partial class CalculaColumnasMultipleFrmViewModel : ObservableObject
{
    // Archivos de combinaciones seleccionados (ListBox listaFicheros del form legacy).
    // En el legacy se almacena la ruta completa en 'combinaciones' y el nombre en el ListBox.
    public ObservableCollection<string> Ficheros { get; } = new();

    // Índice seleccionado en la lista (listaFicheros.SelectedIndex).
    [ObservableProperty]
    private int _ficheroSeleccionado = -1;

    // true cuando hay al menos un fichero (btnCalcular.Enabled del legacy).
    public bool HayFicheros => Ficheros.Count > 0;

    // ===== Estadísticas en vivo (labels del form legacy) =====
    // Se exponen como string porque el dominio aporta long/double con formato
    // "#,##0" y "€ #,##0.00" (regla anti-crash: no bindear numérico directo a Text).

    // lblColsProcesadas / colProcesadaCoste
    [ObservableProperty] private string _columnasProcesadas = "0";
    [ObservableProperty] private string _costeProcesadas = "€ 0,00";

    // lblColsAdmitidas / colAceptadaCoste / lblPorcentaje
    [ObservableProperty] private string _columnasAceptadas = "0";
    [ObservableProperty] private string _costeAceptadas = "€ 0,00";
    [ObservableProperty] private string _porcentaje = "0,00 %";

    // lblColsEstimadas / colEstimadasCoste
    [ObservableProperty] private string _columnasEstimadas = "0";
    [ObservableProperty] private string _costeEstimadas = "€ 0,00";

    // lblColsMaximo / colMaximoCoste
    [ObservableProperty] private string _columnasMaximas = "0";
    [ObservableProperty] private string _costeMaximas = "€ 0,00";

    // horaComienzo / horaFinal / lblSeg
    [ObservableProperty] private string _horaComienzo = "00:00:00";
    [ObservableProperty] private string _horaFinal = "00:00:00";
    [ObservableProperty] private string _tiempoTotal = "0,0";

    // ===== Progreso (progressBar / progressBarArchivos del legacy) =====
    [ObservableProperty] private double _progresoColumnas;   // 0..100 (archivo en curso)
    [ObservableProperty] private double _progresoArchivos;   // 0..100 (lote completo)
    [ObservableProperty] private bool _progresoVisible;      // visibilidad de ambas barras

    // btnCalcular.Enabled: hay ficheros y no hay un proceso en marcha.
    [ObservableProperty] private bool _procesoEnMarcha;

    public bool PuedeCalcular => HayFicheros && !ProcesoEnMarcha;

    partial void OnProcesoEnMarchaChanged(bool value) => OnPropertyChanged(nameof(PuedeCalcular));

    [RelayCommand]
    private void SeleccionarArchivos()
    {
        // TODO: Dominio legacy — btnSelArch_Click del CalculaColumnasMultipleFrm:
        //   OpenFileDialog (Multiselect) sobre "{StartupPath}/Combinaciones/",
        //   filtro "Combinaciones(*.comb)|*.comb|Combinaciones(*.xml)|*.xml|...".
        //   En WinUI usar Windows.Storage.Pickers.FileOpenPicker. Tras elegir:
        //     - ordenar las rutas (combinaciones.Sort())
        //     - rellenar Ficheros con Path.GetFileName de cada ruta
        //   y habilitar Calcular (HayFicheros / PuedeCalcular).
        OnPropertyChanged(nameof(HayFicheros));
        OnPropertyChanged(nameof(PuedeCalcular));
    }

    [RelayCommand]
    private void Calcular()
    {
        // TODO: Dominio legacy — BtnCalcularClick del CalculaColumnasMultipleFrm:
        //   1. Carpeta destino = "{StartupPath}/Columnas/"; cada fichero de salida
        //      es el nombre de origen con extensión .txt (.comb/.xml -> .txt).
        //   2. Si el destino existe, pedir confirmación de reemplazo (ContentDialog).
        //   3. Por cada combinación:
        //        archComb = new ArchivoCombinacion(); archComb.AbrirArchivoCombinacion(origen);
        //        analizador = new Analizador(); archComb.CargaControladorGrupos(analizador.CtrlGrupos);
        //        analizador.ArchivoColumnasBase = archComb.LeeFiltroColumnas();
        //        analizador.Pronosticos = archComb.LeePronosticos();
        //        actualizaColumnasPrevistas();  // calcula colsMaximas (2^dobles * 3^triples
        //                                        // o ObtenNumCols() del filtro base)
        //        analizador.AnalizaCombinacion(destino);
        //   4. Refrescar estadísticas (ActualizaDatosCalculo) vía un Timer/DispatcherTimer
        //      mientras procesoEnMarcha == true:
        //        ColumnasProcesadas = analizador.ColsAnalizadas; ColumnasAceptadas = analizador.ColsAceptadas;
        //        coste = cols * VariablesGlobales.PrecioApuesta; porcentaje = aceptadas*100/analizadas;
        //        estimadas = round(colsMaximas*porcentaje/100); ProgresoColumnas = analizadas*100/colsMaximas.
        //   5. Al terminar el lote, registrar HoraFinal y TiempoTotal y abrir
        //      ResultadosCalculoMultipleFrm (resumen por archivo: origen, destino,
        //      analizadas, aceptadas, tiempo).
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO: Dominio legacy — BtnCancelarClick: analizador.PararAnalisis() y cerrar.
        ProcesoEnMarcha = false;
        ProgresoVisible = false;
    }
}
