using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de resultados (solo lectura): categoría de aciertos y su probabilidad de premio.
/// Legacy: labels "lbl_NN" generados dinámicamente en ProbabilidadPremios.ButtonClick,
/// con texto "Premio de NN: pp,pp %".
/// </summary>
public partial class ProbabilidadPremioItem : ObservableObject
{
    [ObservableProperty]
    private string _categoria = string.Empty;

    // Regla anti-crash 2: el valor numérico se expone ya formateado como string.
    [ObservableProperty]
    private string _probabilidadTexto = "-";
}

/// <summary>
/// ViewModel de la pantalla "Probabilidades de Premios" (legacy: ProbabilidadPremios).
/// Selecciona dos ficheros de columnas (madre/hijas), un rango de aciertos y, al calcular,
/// produce la probabilidad de premio por cada categoría dentro del rango.
/// Cableado al motor real Free1X2.Analisis.Analizador (ComparaCombinaciones +
/// ObtenProbabilidadPremios) y al parser Free1X2.Utils.UtilidadesEntradasValores.
/// </summary>
public partial class ProbabilidadPremiosViewModel : ObservableObject
{
    // --- Selección de ficheros (legacy: archivoColsBase / archivoCols, labels lblCombMadre / lblCombHija) ---

    // Ruta completa del fichero de columnas "madre"/base. Legacy: campo archivoColsBase.
    private string _archivoColsMadre = string.Empty;

    // Ruta completa del fichero de columnas "hijas". Legacy: campo archivoCols.
    private string _archivoColsHija = string.Empty;

    // Nombre mostrado de la combinación madre. Legacy: lblCombMadre.Text (sin extensión).
    [ObservableProperty]
    private string _nombreColumnasMadre = "(seleccionar)";

    // Nombre mostrado de la combinación hija. Legacy: lblCombHija.Text (sin extensión).
    [ObservableProperty]
    private string _nombreColumnasHija = "(seleccionar)";

    // --- Rango de premios (legacy: txtRango) ---
    [ObservableProperty]
    private string _rangoPremios = string.Empty;

    // --- Habilitación del botón Calcular (legacy: button.Enabled gestionado por ActivarBotonCalculo) ---
    // Regla anti-crash 1: este bool se bindea a IsEnabled de un Control (Button), no de un panel.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeCalcular))]
    private bool _madreSeleccionada;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeCalcular))]
    private bool _hijaSeleccionada;

    public bool PuedeCalcular => MadreSeleccionada && HijaSeleccionada;

    // --- Resultados (legacy: labels lbl_NN añadidos dinámicamente al form) ---
    public ObservableCollection<ProbabilidadPremioItem> Resultados { get; } = new();

    [RelayCommand]
    private async Task SeleccionarMadreAsync()
    {
        // Legacy: ProbabilidadPremios.btnSelectMadreClick (OpenFileDialog "Columnas (*.txt)").
        var ruta = await SeleccionarColumnasAsync();
        if (ruta == null) return;
        _archivoColsMadre = ruta;
        NombreColumnasMadre = Path.GetFileNameWithoutExtension(ruta);
        MadreSeleccionada = true; // legacy: ActivarBotonCalculo()
    }

    [RelayCommand]
    private async Task SeleccionarHijaAsync()
    {
        // Legacy: ProbabilidadPremios.btnSelectHijaClick (OpenFileDialog "Columnas (*.txt)").
        var ruta = await SeleccionarColumnasAsync();
        if (ruta == null) return;
        _archivoColsHija = ruta;
        NombreColumnasHija = Path.GetFileNameWithoutExtension(ruta);
        HijaSeleccionada = true; // legacy: ActivarBotonCalculo()
    }

    private static async Task<string?> SeleccionarColumnasAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        return file?.Path;
    }

    [RelayCommand]
    private async Task CalcularAsync()
    {
        // Legacy: ProbabilidadPremios.ButtonClick.
        // 1) Rango de premios: parsear RangoPremios; si vacío o error usar 10..14.
        int minimoPremio;
        int maximoPremio;
        if (!string.IsNullOrEmpty(RangoPremios))
        {
            try
            {
                List<int> premios = Free1X2.Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(RangoPremios);
                premios.Sort();
                minimoPremio = premios[0];
                maximoPremio = premios[premios.Count - 1];
            }
            catch
            {
                minimoPremio = 10;
                maximoPremio = 14;
            }
        }
        else
        {
            minimoPremio = 10;
            maximoPremio = 14;
        }

        Resultados.Clear();

        string madre = _archivoColsMadre;
        string hija = _archivoColsHija;

        try
        {
            // 2) Análisis comparativo + 3) probabilidades por categoría (fuera del hilo de UI).
            var filas = await Task.Run(() =>
            {
                var analizador = new Free1X2.Analisis.Analizador(minimoPremio);
                analizador.ComparaCombinaciones(madre, hija);

                var resultado = new List<(string Categoria, string Probabilidad)>();
                for (int i = minimoPremio; i <= maximoPremio; i++)
                {
                    double prob = analizador.ObtenProbabilidadPremios(i);
                    resultado.Add(($"Premio de {i}", prob.ToString("#,##0.00;0.00") + " %"));
                }
                return resultado;
            });

            foreach (var (categoria, probabilidad) in filas)
            {
                Resultados.Add(new ProbabilidadPremioItem
                {
                    Categoria = categoria,
                    ProbabilidadTexto = probabilidad,
                });
            }
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al calcular las probabilidades: " + ex.Message);
        }
    }
}
