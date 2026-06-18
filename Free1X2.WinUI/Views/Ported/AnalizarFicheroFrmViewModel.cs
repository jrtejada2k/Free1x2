// Free1X2 · WinUI 3 — WIN3
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Analizar fichero de columnas" (legacy: AnalizarFicheroFrm).
/// Permite seleccionar un fichero de columnas (.txt/.cols), leer cuántas columnas
/// contiene y lanzar el análisis de la combinación, con opción de incluir el pleno al 15.
/// Cableado al motor real (Free1X2.EntradaSalida.ArchivoColumnasTexto y
/// Free1X2.MotorCalculo.Analizador); el visor de resultados lo gestiona el hook
/// Free1X2.Abstractions.AnalisisUi.MostrarVisor a nivel de app.
/// </summary>
public partial class AnalizarFicheroFrmViewModel : ObservableObject
{
    // Columnas leídas del fichero (legacy: campo string[] columnas).
    private string[] _columnas = Array.Empty<string>();

    /// <summary>Ruta del fichero de columnas de entrada (legacy: txFicheroEntrada).</summary>
    [ObservableProperty]
    private string _ficheroEntrada = string.Empty;

    /// <summary>Texto informativo con el nº de columnas leídas (legacy: lblColsEntrada).</summary>
    [ObservableProperty]
    private string _resumenColumnas = string.Empty;

    /// <summary>Incluir pleno al 15 en el análisis (legacy: chkPleno).</summary>
    [ObservableProperty]
    private bool _incluirPleno;

    /// <summary>
    /// Habilita el check "Incluir pleno al 15" solo cuando las columnas tienen 15 signos
    /// (legacy: chkPleno.Enabled = (columnas[0].Length == 15)).
    /// </summary>
    [ObservableProperty]
    private bool _puedeIncluirPleno;

    /// <summary>
    /// Habilita el botón Analizar solo cuando hay columnas cargadas
    /// (legacy: btnOk.Enabled = (columnas.Length > 0)).
    /// </summary>
    [ObservableProperty]
    private bool _puedeAnalizar;

    /// <summary>
    /// Selecciona el fichero de columnas y lee cuántas columnas contiene.
    /// Equivale a AnalizarFicheroFrm.btnAbrirEntrada_Click.
    /// </summary>
    [RelayCommand]
    private async Task AbrirFicheroAsync()
    {
        // Diálogo de fichero (legacy: abreFiltroDialog, OpenFileDialog *.txt/*.cols/*.*).
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add(".cols");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        FicheroEntrada = file.Path;

        // Lectura de columnas (legacy: cols = new ArchivoColumnasTexto(ruta);
        //                              columnas = cols.LeerTodasCols(false); cols.Cerrar();).
        try
        {
            IArchivoColumnas cols = new ArchivoColumnasTexto(FicheroEntrada);
            _columnas = cols.LeerTodasCols(false);
            cols.Cerrar();
            ResumenColumnas = _columnas.Length + " columnas.";
        }
        catch
        {
            _columnas = Array.Empty<string>();
            ResumenColumnas = string.Empty;
            AppServices.MostrarError("No se ha podido leer el fichero de columnas.");
            return;
        }

        // Habilitación de botones (legacy: btnOk.Enabled / chkPleno.Enabled).
        if (_columnas.Length > 0)
        {
            PuedeAnalizar = true;
            PuedeIncluirPleno = _columnas[0].Length == 15;
        }
        else
        {
            PuedeAnalizar = false;
            IncluirPleno = false;
            PuedeIncluirPleno = false;
        }
    }

    /// <summary>
    /// Lanza el análisis de la combinación del fichero seleccionado.
    /// Equivale a AnalizarFicheroFrm.btnOk_Click.
    /// </summary>
    [RelayCommand]
    private async Task AnalizarAsync()
    {
        if (FicheroEntrada.Length == 0) return;
        if (_columnas.Length == 0)
        {
            AppServices.MostrarError("No se ha cargado el fichero de entrada o no tiene columnas.");
            return;
        }

        string ruta = FicheroEntrada;

        try
        {
            await Task.Run(() =>
            {
                // Legacy: número de signos del fichero -> dimensiona el Analizador.
                IArchivoColumnas aCol = new ArchivoColumnasTexto(ruta);
                int partidos = aCol.ObtenNumSignos();
                aCol.Cerrar();

                var analizador = new Free1X2.MotorCalculo.Analizador(partidos);
                analizador.ArchivoColumnasBase = ruta;
                // Inicializa los pronósticos a "1,X,2" (combinación abierta).
                for (int i = 0; i < partidos; i++)
                {
                    analizador.SetPronostico(i, "1,X,2");
                }
                // esAnalisisExterno = (true, true): análisis de fichero externo. El visor de
                // resultados lo dispara Free1X2.Abstractions.AnalisisUi.MostrarVisor.
                analizador.AnalizaCombinacion(true, true);
            });
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al analizar el fichero: " + ex.Message);
        }
    }
}
