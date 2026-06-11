using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Rentabilidad" (legacy: Free1X2.UI.RentabilidadFrm).
/// La utilidad obtiene la Esperanza Matemática de premio (EM) de las columnas a partir de
/// la probabilidad real y de las frecuencias apostadas, recorriendo o bien las 14 triples
/// completas o las columnas de un fichero, y graba el resultado filtrado por límites de EM.
/// También permite calcular la valoración (EM) de una sola columna concreta.
/// </summary>
public partial class RentabilidadFrmViewModel : ObservableObject
{
    // --- Origen de las columnas (legacy: rb14Triples / rbFichero, txFicheroEntrada) ---
    // OrigenEsFichero=false -> 14 triples (por defecto, legacy rb14Triples.Checked=true);
    // true -> fichero de entrada. OrigenEs14Triples es el inverso para el RadioButton.
    [ObservableProperty]
    private bool _origenEsFichero;

    public bool OrigenEs14Triples => !OrigenEsFichero;

    partial void OnOrigenEsFicheroChanged(bool value)
    {
        OnPropertyChanged(nameof(OrigenEs14Triples));
    }

    [ObservableProperty]
    private string _ficheroEntrada = string.Empty;

    // --- Salida de resultados (legacy: txFicheroSalida) ---
    [ObservableProperty]
    private string _ficheroSalida = string.Empty;

    // --- Recaudación considerada (legacy: textBox1 + textBox1_TextChanged) ---
    // En el legacy fija el PremioTope = Recaudacion * Porcentaje14 / 100.
    [ObservableProperty]
    private double _recaudacion = 8000000;

    // --- Límites de Esperanza Matemática (legacy: txEMmin / txEMmax) ---
    [ObservableProperty]
    private double _emMin = 0.133;

    [ObservableProperty]
    private double _emMax = 50;

    // --- Opciones de salida (legacy: chkOrdenar / chkPonerEM) ---
    [ObservableProperty]
    private bool _ordenarPorEm;

    [ObservableProperty]
    private bool _anadirEmAlFichero;

    // --- Cálculo de valoración de una columna (legacy: txtColumna + btnCalculoVal) ---
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CalcularValoracionColumnaCommand))]
    private string _columna = string.Empty;

    // Resultados de la valoración de la columna. Texto para no bindear double->TextBlock.Text.
    // Legacy: txProbApostada (premio estimado de 14), txProbReal, txEM.
    [ObservableProperty]
    private string _premioEstimado14Texto = "-";

    [ObservableProperty]
    private string _probabilidadRealTexto = "-";

    [ObservableProperty]
    private string _esperanzaMatematicaTexto = "-";

    // Estado de la operación (legacy: statusBarPanel6: "Leyendo...", "Calculando...", "Finalizado (N columnas)").
    [ObservableProperty]
    private string _estadoTexto = "Listo";

    private bool ColumnaValida => Columna != null && Columna.Length == 14;

    // ---------------------------------------------------------------------
    // Acciones. La lógica de dominio queda como TODO citando la clase legacy.
    // ---------------------------------------------------------------------

    /// <summary>
    /// Selecciona el fichero de columnas de entrada.
    /// Legacy: RentabilidadFrm.btOpenFicheroEntrada_Click (OpenFileDialog *.txt).
    /// </summary>
    [RelayCommand]
    private void SeleccionarFicheroEntrada()
    {
        // TODO: portar OpenFileDialog -> FileOpenPicker (filtro "*.txt").
        //       Legacy: RentabilidadFrm.btOpenFicheroEntrada_Click asigna FicheroEntrada.
    }

    /// <summary>
    /// Selecciona el fichero de salida de resultados.
    /// Legacy: RentabilidadFrm.button4_Click (SaveFileDialog *.txt; FilterIndex==2 -> salidaBinaria).
    /// </summary>
    [RelayCommand]
    private void SeleccionarFicheroSalida()
    {
        // TODO: portar SaveFileDialog -> FileSavePicker.
        //       Legacy: RentabilidadFrm.button4_Click asigna FicheroSalida (+ flag salidaBinaria).
    }

    /// <summary>
    /// Calcula la valoración (EM) de la columna introducida.
    /// Legacy: RentabilidadFrm.btnCalculoVal_Click -> CalcularValoracionColumna().
    /// </summary>
    [RelayCommand(CanExecute = nameof(ColumnaValida))]
    private void CalcularValoracionColumna()
    {
        // TODO: portar RentabilidadFrm.CalcularValoracionColumna():
        //       - cargar %apostados/reales (ControlPorcentajes -> Porcentajes.ValoresBase100)
        //       - calcular p14 apostada/real signo a signo de la columna
        //       - Premio = PremioDe14 / p14apostada (acotado por PremioTope)
        //       - Esperanza = Premio * p14real
        //       y volcar a PremioEstimado14Texto / ProbabilidadRealTexto / EsperanzaMatematicaTexto.
    }

    /// <summary>
    /// Calcula la rentabilidad de todas las columnas y graba el fichero de salida.
    /// Legacy: RentabilidadFrm.btnOK_Click (EncontrarDistantes1 + ordena + GrabacionColumnas).
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO: portar RentabilidadFrm.btnOK_Click:
        //       - origen: 14 triples (Bits.SetAll(true)) o fichero (LeerColumnas -> ArchivoColumnasTexto)
        //       - recorrer apuestas con EncontrarDistantes1 calculando EM
        //       - si OrdenarPorEm -> ordena() (quicksort por EM)
        //       - GrabacionColumnas(): filtrar por EMmin/EMmax, opcional AnadirEmAlFichero,
        //         ConvertidorDeBases + ArchivoColumnasTexto.GuardarCols.
        //       Actualizar EstadoTexto con el número de columnas grabadas.
    }
}
