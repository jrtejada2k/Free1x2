using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Exportador de Columnas Probables".
/// Replica el WinForms <c>ExportadorCPsFrm</c>, un diálogo que recibe una lista de
/// <c>ColumnaProbable</c> y ofrece exportarla a fichero (más cancelar):
///   - Simple: sólo las columnas/pronósticos a un .txt (btnExportarSimples).
///   - Con aciertos: pronósticos + aciertos, aciertos seguidos y fallos seguidos
///     a un .clm (btnExportarClm).
/// La escritura usa el motor real (ArchivoColumnasTexto y ColumnaProbable), igual que el
/// legacy. La lista de origen se inyecta vía <see cref="EstablecerColumnas"/> al navegar.
/// </summary>
public partial class ExportadorCPsFrmViewModel : ObservableObject
{
    // Lista de CPs a exportar (legacy: constructor ExportadorCPsFrm(List<ColumnaProbable> lista)).
    private List<ColumnaProbable> _lista = new();

    // ===== Estado / feedback (sustituye al cierre del diálogo) =====
    [ObservableProperty]
    private string _estado = "Elige el formato de exportación de las columnas probables.";

    [ObservableProperty]
    private string _numeroColumnasTexto = "0 columnas para exportar.";

    public ExportadorCPsFrmViewModel()
    {
    }

    /// <summary>
    /// Inyecta la lista de columnas a exportar. El WinForms la recibía por constructor; en
    /// WinUI la pasa quien navegue a la página (p. ej. ColProbablesFrm.ExportaColumnas,
    /// ver Free1X2/UI/Filtros/ColProbablesFrm.cs línea 748).
    /// </summary>
    public void EstablecerColumnas(List<ColumnaProbable> lista)
    {
        _lista = lista ?? new List<ColumnaProbable>();
        ActualizarResumen();
    }

    /// <summary>
    /// btnExportarSimples_Click del WinForms: exporta sólo las columnas (PronosticosString)
    /// a un fichero de texto (*.txt) usando ArchivoColumnasTexto.GuardarTodasCols(.., true).
    /// </summary>
    [RelayCommand]
    private async Task ExportarSimples()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            DefaultFileExtension = ".txt",
            SuggestedFileName = "columnas",
        };
        picker.FileTypeChoices.Add("Columnas Simples", new List<string> { ".txt" });
        picker.FileTypeChoices.Add("Todos los archivos", new List<string> { "." });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? archivo = await picker.PickSaveFileAsync();
        if (archivo is null)
        {
            return;
        }

        string ruta = archivo.Path;
        List<ColumnaProbable> lista = _lista;

        await Task.Run(() =>
        {
            string[] columnas = new string[lista.Count];
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(ruta);
            for (int i = 0; i < lista.Count; i++)
            {
                columnas[i] = lista[i].PronosticosString;
            }
            comBaseCols.GuardarTodasCols(columnas, true);
            comBaseCols.Cerrar();
        });

        Estado = $"Exportadas {lista.Count} columnas simples a {Path.GetFileName(ruta)}.";
    }

    /// <summary>
    /// btnExportarClm_Click del WinForms: exporta cada columna con sus aciertos (Ac),
    /// aciertos seguidos (Acs) y fallos seguidos (Fs) a un *.clm, una línea por columna.
    /// </summary>
    [RelayCommand]
    private async Task ExportarConAciertos()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            DefaultFileExtension = ".clm",
            SuggestedFileName = "columnas",
        };
        picker.FileTypeChoices.Add("Columnas Con Aciertos", new List<string> { ".clm" });
        picker.FileTypeChoices.Add("Todos los archivos", new List<string> { "." });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? archivo = await picker.PickSaveFileAsync();
        if (archivo is null)
        {
            return;
        }

        string ruta = archivo.Path;
        List<ColumnaProbable> lista = _lista;

        await Task.Run(() =>
        {
            using var sw = new StreamWriter(ruta);
            for (int i = 0; i < lista.Count; i++)
            {
                ColumnaProbable cp = lista[i];
                string linea = cp.PronosticosString + "#" + cp.GetAciertos() + "#" +
                               cp.GetAciertosSeguidos() + "#" + cp.GetFallosSeguidos();
                sw.WriteLine(linea);
            }
            sw.Close();
        });

        Estado = $"Exportadas {lista.Count} columnas con aciertos a {Path.GetFileName(ruta)}.";
    }

    /// <summary>
    /// btnCancelar_Click del WinForms (Close()).
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        Estado = "Cancelado.";
        // TODO (navegación): el WinForms hacía Close(); en WinUI el cierre/navegación atrás lo
        //   gestiona el contenedor de la Page.
    }

    private void ActualizarResumen()
    {
        int n = _lista.Count;
        NumeroColumnasTexto = n == 1 ? "1 columna para exportar." : $"{n} columnas para exportar.";
    }
}
