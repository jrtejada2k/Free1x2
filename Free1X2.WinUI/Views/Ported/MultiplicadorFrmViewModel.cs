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
/// ViewModel de la pantalla "Multiplicador" (legacy: Free1X2.UI.MultiplicadorFrm).
/// Multiplica (producto cartesiano) dos archivos de combinaciones de 14 signos cada uno,
/// concatenando cada columna de la Entrada 1 con cada columna de la Entrada 2 (28 cifras)
/// y reordenándolas según una plantilla de 14 índices (1..28) que permite transponer
/// columnas al mismo tiempo. El resultado se puede grabar a un archivo de texto.
/// Equivalente legacy: MultiplicadorFrm.Entrada1/Entrada2/Multiplicar/Grabar/RecuperaPantalla.
/// </summary>
public partial class MultiplicadorFrmViewModel : ObservableObject
{
    // Rutas de archivos seleccionados (legacy: filein de Entrada1/Entrada2 + fileout de Grabar).
    private string _rutaEntrada1 = string.Empty;
    private string _rutaEntrada2 = string.Empty;

    // Columnas cargadas en memoria (legacy: ascols1/ascols2 con contadores ncols1/ncols2,
    // y el resultado ascols3 con ncols3). Se conservan como listas en el VM, igual que la
    // forma legacy mantenía los arrays en memoria entre clics.
    private readonly List<string> _ascols1 = new();
    private readonly List<string> _ascols2 = new();
    private List<string> _ascols3 = new();

    // Plantilla de 14 índices de transposición (legacy: tbcol01..tbcol14 -> indices[0..13]).
    // Cada valor debe estar en el rango 1..28 (RecuperaPantalla valida ese rango).
    // NumberBox.Value es double, por eso cada índice se expone como double.
    [ObservableProperty]
    private double _indice01 = 1;
    [ObservableProperty]
    private double _indice02 = 2;
    [ObservableProperty]
    private double _indice03 = 3;
    [ObservableProperty]
    private double _indice04 = 4;
    [ObservableProperty]
    private double _indice05 = 5;
    [ObservableProperty]
    private double _indice06 = 15;
    [ObservableProperty]
    private double _indice07 = 16;
    [ObservableProperty]
    private double _indice08 = 17;
    [ObservableProperty]
    private double _indice09 = 18;
    [ObservableProperty]
    private double _indice10 = 19;
    [ObservableProperty]
    private double _indice11 = 20;
    [ObservableProperty]
    private double _indice12 = 21;
    [ObservableProperty]
    private double _indice13 = 13;
    [ObservableProperty]
    private double _indice14 = 13;

    // Nombre del archivo de la Entrada 1 mostrado en pantalla (legacy: filein de Entrada1).
    [ObservableProperty]
    private string _nombreEntrada1 = "(selecciona)";

    // Nombre del archivo de la Entrada 2 mostrado en pantalla (legacy: filein de Entrada2).
    [ObservableProperty]
    private string _nombreEntrada2 = "(selecciona)";

    // Nº de columnas cargadas de la Entrada 1 como texto (legacy: lcols1.Text = ncols1).
    [ObservableProperty]
    private string _columnasEntrada1Texto = "0";

    // Nº de columnas cargadas de la Entrada 2 como texto (legacy: lcols2.Text = ncols2).
    [ObservableProperty]
    private string _columnasEntrada2Texto = "0";

    // Nº de columnas resultantes como texto (legacy: lcolsresul.Text = ncols3).
    [ObservableProperty]
    private string _columnasResultadoTexto = "0";

    // Mensaje de estado/errores (legacy: MessageBox.Show("error en plantilla"), etc.).
    [ObservableProperty]
    private string _mensaje = string.Empty;

    // Habilita/deshabilita los botones de entrada y multiplicar durante el proceso
    // (legacy: bEntra1/bEntra2/bMultiplica.Enabled = false/true).
    [ObservableProperty]
    private bool _puedeProcesar = true;

    // Habilita Grabar solo cuando hay resultado calculado (legacy: bGrabar.Enabled).
    [ObservableProperty]
    private bool _puedeGrabar;

    /// <summary>
    /// Selecciona y carga el archivo de la Entrada Comb-1.
    /// Legacy: MultiplicadorFrm.Entrada1() -> OpenFileDialog (*.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarEntrada1Async()
    {
        var file = await AbrirSelectorColumnasAsync();
        if (file == null) return;

        _rutaEntrada1 = file.Path;
        PuedeProcesar = false;
        try
        {
            await Task.Run(() => CargarColumnas(_rutaEntrada1, _ascols1));
            NombreEntrada1 = Path.GetFileName(_rutaEntrada1);
            ColumnasEntrada1Texto = _ascols1.Count.ToString();
        }
        finally
        {
            PuedeProcesar = true;
        }
    }

    /// <summary>
    /// Selecciona y carga el archivo de la Entrada Comb-2.
    /// Legacy: MultiplicadorFrm.Entrada2() -> OpenFileDialog (*.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarEntrada2Async()
    {
        var file = await AbrirSelectorColumnasAsync();
        if (file == null) return;

        _rutaEntrada2 = file.Path;
        PuedeProcesar = false;
        try
        {
            await Task.Run(() => CargarColumnas(_rutaEntrada2, _ascols2));
            NombreEntrada2 = Path.GetFileName(_rutaEntrada2);
            ColumnasEntrada2Texto = _ascols2.Count.ToString();
        }
        finally
        {
            PuedeProcesar = true;
        }
    }

    /// <summary>
    /// Realiza el producto cartesiano de ambas entradas aplicando la plantilla de índices.
    /// Legacy: MultiplicadorFrm.Multiplicar() + RecuperaPantalla().
    /// </summary>
    [RelayCommand]
    private async Task MultiplicarAsync()
    {
        // RecuperaPantalla() legacy: plantilla de 14 índices con rango 1..28.
        var indices = new[]
        {
            (int)Indice01, (int)Indice02, (int)Indice03, (int)Indice04, (int)Indice05,
            (int)Indice06, (int)Indice07, (int)Indice08, (int)Indice09, (int)Indice10,
            (int)Indice11, (int)Indice12, (int)Indice13, (int)Indice14,
        };
        for (int nr = 0; nr < 14; nr++)
        {
            if (indices[nr] < 1 || indices[nr] > 28)
            {
                Mensaje = "error en plantilla";
                AppServices.MostrarError("error en plantilla");
                return;
            }
        }

        Mensaje = string.Empty;
        PuedeProcesar = false;
        try
        {
            // Producto cartesiano: concatena cada columna de E1 con cada una de E2 (28 cifras)
            // y reordena según la plantilla (legacy: Multiplicar()).
            var resultado = await Task.Run(() =>
            {
                var lista = new List<string>(_ascols1.Count * _ascols2.Count);
                var aux = new char[14];
                foreach (var scol1 in _ascols1)
                {
                    foreach (var scol2 in _ascols2)
                    {
                        string scol3 = scol1 + scol2;
                        for (int nr3 = 0; nr3 < 14; nr3++) aux[nr3] = scol3[indices[nr3] - 1];
                        lista.Add(new string(aux));
                    }
                }
                return lista;
            });
            _ascols3 = resultado;
            ColumnasResultadoTexto = _ascols3.Count.ToString();
            PuedeGrabar = _ascols3.Count > 0;
        }
        catch (Exception ex)
        {
            Mensaje = "Error: " + ex.Message;
        }
        finally
        {
            PuedeProcesar = true;
        }
    }

    /// <summary>
    /// Graba las columnas resultantes a un archivo de texto.
    /// Legacy: MultiplicadorFrm.Grabar() -> SaveFileDialog (*.txt).
    /// </summary>
    [RelayCommand]
    private async Task GrabarAsync()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Resultado",
        };
        picker.FileTypeChoices.Add("Fichero resultados", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        string ruta = file.Path;
        var copia = _ascols3;
        PuedeGrabar = false;
        PuedeProcesar = false;
        try
        {
            await Task.Run(() =>
            {
                using var sw = new StreamWriter(ruta);
                foreach (var col in copia) sw.WriteLine(col);
            });
            Mensaje = "Resultado grabado.";
        }
        catch (Exception ex)
        {
            Mensaje = "Error: " + ex.Message;
        }
        finally
        {
            PuedeGrabar = copia.Count > 0;
            PuedeProcesar = true;
        }
    }

    /// <summary>Carga columnas normalizadas a 14 cifras (legacy: (linea+"11111111111111").Substring(0,14).Trim()).</summary>
    private static void CargarColumnas(string ruta, List<string> destino)
    {
        destino.Clear();
        using var sr = new StreamReader(ruta);
        while (sr.Peek() > 0)
        {
            string scol = (sr.ReadLine() + "11111111111111").Substring(0, 14);
            destino.Add(scol.Trim());
        }
    }

    /// <summary>Abre un FileOpenPicker para archivos de columnas (*.txt).</summary>
    private static async Task<Windows.Storage.StorageFile?> AbrirSelectorColumnasAsync()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);
        return await picker.PickSingleFileAsync();
    }
}
