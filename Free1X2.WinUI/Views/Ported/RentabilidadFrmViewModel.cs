using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

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
    private async Task SeleccionarFicheroEntrada()
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;
        FicheroEntrada = file.Path;
    }

    /// <summary>
    /// Selecciona el fichero de salida de resultados.
    /// Legacy: RentabilidadFrm.button4_Click (SaveFileDialog *.txt; FilterIndex==2 -> salidaBinaria).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarFicheroSalida()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Rentabilidad",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;
        FicheroSalida = file.Path;
    }

    /// <summary>
    /// Calcula la valoración (EM) de la columna introducida.
    /// Legacy: RentabilidadFrm.btnCalculoVal_Click -> CalcularValoracionColumna().
    /// </summary>
    [RelayCommand(CanExecute = nameof(ColumnaValida))]
    private void CalcularValoracionColumna()
    {
        // TODO: lógica en Free1X2/UI/RentabilidadFrm.cs línea 113 (CalcularValoracionColumna).
        //   Requiere las matrices de porcentajes apostados/reales que en el WinForms provee el
        //   UserControl Free1X2.UI.Controls.ControlPorcentajes (controlPorcentajesApostados /
        //   controlPorcentajesReales -> Porcentajes.ValoresBase100()). Ese control aún no está
        //   portado a WinUI (la propia Page lo señala), por lo que no hay datos de entrada que
        //   cablear al motor sin inventar valores. Los selectores de fichero ya están cableados.
        EstadoTexto = "Falta el control de porcentajes (ver RentabilidadFrm.cs línea 113)";
    }

    /// <summary>
    /// Calcula la rentabilidad de todas las columnas y graba el fichero de salida.
    /// Legacy: RentabilidadFrm.btnOK_Click (EncontrarDistantes1 + ordena + GrabacionColumnas).
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // Validación de ficheros (origen fichero requiere FicheroEntrada; salida siempre).
        if (string.IsNullOrEmpty(FicheroSalida) ||
            (OrigenEsFichero && string.IsNullOrEmpty(FicheroEntrada)))
        {
            EstadoTexto = "Faltan datos";
            return;
        }

        // TODO: lógica en Free1X2/UI/RentabilidadFrm.cs línea 852 (btnOK_Click) +
        //   EncontrarDistantes1 (línea 949), ordena() y GrabacionColumnas(). El cálculo de la
        //   Esperanza Matemática parte de las matrices de porcentajes apostados/reales del
        //   UserControl ControlPorcentajes, aún no portado a WinUI. Sin esos datos de entrada no
        //   es posible alimentar el motor sin inventar valores; se cablean solo los selectores.
        EstadoTexto = "Falta el control de porcentajes (ver RentabilidadFrm.cs línea 852)";
    }
}
