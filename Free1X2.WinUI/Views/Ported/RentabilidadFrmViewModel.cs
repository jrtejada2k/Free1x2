using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Controls;
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
    // Rejillas de porcentajes por partido (1/X/2). Sustituyen a los dos UserControls WinForms
    // ControlPorcentajes (controlPorcentajesApostados / controlPorcentajesReales);
    // PorcentajesHelper.AMatriz(...) equivale a ControlPorcentajes.Valores (get).
    public ObservableCollection<FilaPorcentaje> PorcentajesApostados { get; } =
        PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

    public ObservableCollection<FilaPorcentaje> PorcentajesReales { get; } =
        PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

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
        // Legacy: RentabilidadFrm.CalcularValoracionColumna() (línea 113). v/p = .Valores de los dos
        // ControlPorcentajes; pa/pr = Porcentajes.ValoresBase100() (normalización a tanto-por-uno).
        // No depende del motor de 4,7M -> totalmente cableable.
        double precioApuesta = Free1X2.VariablesGlobales.PrecioApuesta;
        double pctPremio14 = Free1X2.VariablesGlobales.Porcentaje14;
        float premioDe14 = (float)precioApuesta * (float)pctPremio14 / 100;
        float premioTope = (float)Recaudacion * (float)pctPremio14 / 100;

        float[,] pa = ValoresBase100(PorcentajesHelper.AMatriz(PorcentajesApostados));
        float[,] pr = ValoresBase100(PorcentajesHelper.AMatriz(PorcentajesReales));

        string apuesta = Columna;
        float p14Apostada = 1, p14Real = 1;
        for (int partido = 0; partido < 14; partido++)
        {
            switch (apuesta[partido])
            {
                case '1': p14Apostada *= pa[partido, 0]; p14Real *= pr[partido, 0]; break;
                case '2': p14Apostada *= pa[partido, 2]; p14Real *= pr[partido, 2]; break;
                default: p14Apostada *= pa[partido, 1]; p14Real *= pr[partido, 1]; break;
            }
        }

        float premio = premioDe14 / p14Apostada;
        if (premio > premioTope) premio = premioTope;
        float esperanza = premio * p14Real;

        PremioEstimado14Texto = Math.Round(precioApuesta * pctPremio14 / 100 / p14Apostada, 2).ToString();
        ProbabilidadRealTexto = p14Real.ToString();
        EsperanzaMatematicaTexto = esperanza.ToString();
        EstadoTexto = "Valoración calculada";
    }

    // Legacy: Free1X2.Utils.Porcentajes.ValoresBase100() (Free1X2/Utils/Porcentajes.cs línea 341).
    // Normaliza cada fila (1/X/2) a tanto-por-uno dividiendo por la suma de la fila. Se replica aquí
    // porque ese helper vive en el proyecto WinForms y no está en Free1X2.Domain.
    private static float[,] ValoresBase100(double[,] valores)
    {
        var b100 = new float[14, 3];
        for (int i = 0; i < 14; i++)
        {
            float factor = (float)(valores[i, 0] + valores[i, 1] + valores[i, 2]);
            for (int j = 0; j < 3; j++)
                b100[i, j] = (float)(valores[i, j] / factor);
        }
        return b100;
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

        // La parte de matriz SÍ se cablea: v/p = .Valores de los dos ControlPorcentajes
        // (RentabilidadFrm.cs 889/890) y su normalización base-100 (895/897):
        float[,] pa = ValoresBase100(PorcentajesHelper.AMatriz(PorcentajesApostados));
        float[,] pr = ValoresBase100(PorcentajesHelper.AMatriz(PorcentajesReales));
        _ = (pa, pr);

        // TODO[motor]: replicar btnOK_Click (RentabilidadFrm.cs 852) + EncontrarDistantes1 (949) +
        //   ordena() + GrabacionColumnas(). Las matrices base-100 (pa/pr) ya están listas para
        //   alimentar el recorrido, pero el motor recorre Ap14T = ApuestaProbableCentral[4782969]
        //   (Bits, Cra/Crp, pot[]) y graba el fichero filtrado por EmMin/EmMax — no portado al dominio.
        EstadoTexto = "Matrices listas; falta el motor de recorrido de 14 triples (RentabilidadFrm.cs 852)";
    }
}
