using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Álgebra" (legacy: AlgebraColumnasFrm).
/// Realiza operaciones de álgebra de combinaciones entre dos archivos de columnas:
/// eliminar repetidas, sumar, intersección de comunes o resta.
/// Equivalente legacy: Free1X2.Utils.SumadorCombinaciones + AlgebraCombTipo.
/// </summary>
public partial class AlgebraColumnasFrmViewModel : ObservableObject
{
    // Rutas de archivos seleccionados (legacy: archivoCols1 / archivoCols2 / archivoColsFinal).
    private string _rutaCombinacion1 = string.Empty;
    private string _rutaCombinacion2 = string.Empty;
    private string _rutaCombinacionFinal = string.Empty;

    // Número de partidos (signos) de cada archivo, para validar compatibilidad
    // (legacy: noSignos1 / noSignos2 vía IArchivoColumnas.ObtenNumSignos()).
    private int _noSignos1;
    private int _noSignos2;

    // Operaciones disponibles. El índice coincide con el enum legacy AlgebraCombTipo:
    // 0 = EliminaRepetidas, 1 = SumaEliminaRepetidas, 2 = SumaSoloComunes, 3 = RestaSegunda.
    public IReadOnlyList<string> Operaciones { get; } = new[]
    {
        "Elimina columnas repetidas de Combinación 1",
        "Suma Combinaciones: elimina columnas repetidas",
        "Suma Combinaciones: selecciona columnas comunes",
        "Resta combinaciones: (1)-(2)",
    };

    // Operación seleccionada (legacy: radOption1..radOption4 -> AlgebraCombTipo).
    [ObservableProperty]
    private int _operacionSeleccionada;

    // Nombre del archivo de la Combinación 1 mostrado en pantalla (legacy: lblComb1).
    [ObservableProperty]
    private string _nombreCombinacion1 = "(selecciona)";

    // Nombre del archivo de la Combinación 2 mostrado en pantalla (legacy: lblComb2).
    [ObservableProperty]
    private string _nombreCombinacion2 = "(selecciona)";

    // Nombre del archivo de resultado mostrado en pantalla (legacy: lblCombFinal).
    [ObservableProperty]
    private string _nombreCombinacionFinal = "(selecciona)";

    // Detalle (nº columnas) de la Combinación 1 (legacy: lblFiltro1).
    [ObservableProperty]
    private string _detalleCombinacion1 = string.Empty;

    // Detalle (nº columnas) de la Combinación 2 (legacy: lblFiltro2).
    [ObservableProperty]
    private string _detalleCombinacion2 = string.Empty;

    // Mensaje de resultado del cálculo (legacy: lblResultado / sumador.MensajeFinCalculo).
    [ObservableProperty]
    private string _resultado = string.Empty;

    // Habilita/deshabilita el botón Calcular durante el cálculo (legacy: btnCalcular.Enabled).
    [ObservableProperty]
    private bool _puedeCalcular = true;

    /// <summary>
    /// Selecciona el archivo de la Combinación 1.
    /// Legacy: BtnSelComb1Click -> OpenFileDialog (carpeta Columnas, *.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarCombinacion1Async()
    {
        var file = await AbrirSelectorColumnasAsync();
        if (file == null) return;

        _rutaCombinacion1 = file.Path;
        NombreCombinacion1 = Path.GetFileNameWithoutExtension(_rutaCombinacion1);

        // Lee número de partidos y columnas del archivo (legacy: ArchivoColumnasTexto).
        IArchivoColumnas archivo = new ArchivoColumnasTexto(_rutaCombinacion1);
        _noSignos1 = archivo.ObtenNumSignos();
        DetalleCombinacion1 = "Combinación 1: " + archivo.NumColumnas + " columnas.";
    }

    /// <summary>
    /// Selecciona el archivo de la Combinación 2.
    /// Legacy: BtnSelComb2Click -> OpenFileDialog (carpeta Columnas, *.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarCombinacion2Async()
    {
        var file = await AbrirSelectorColumnasAsync();
        if (file == null) return;

        _rutaCombinacion2 = file.Path;
        NombreCombinacion2 = Path.GetFileNameWithoutExtension(_rutaCombinacion2);

        IArchivoColumnas archivo = new ArchivoColumnasTexto(_rutaCombinacion2);
        _noSignos2 = archivo.ObtenNumSignos();
        DetalleCombinacion2 = "Combinación 2: " + archivo.NumColumnas + " columnas.";
    }

    /// <summary>
    /// Selecciona el archivo de resultado donde se guardará la operación.
    /// Legacy: BtnSelCombFinalClick -> SaveFileDialog (carpeta Columnas, *.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarCombinacionFinalAsync()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "ColumnasResultado",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        _rutaCombinacionFinal = file.Path;
        NombreCombinacionFinal = Path.GetFileNameWithoutExtension(_rutaCombinacionFinal);
    }

    /// <summary>
    /// Ejecuta la operación de álgebra de combinaciones seleccionada.
    /// Legacy: BtnCalcularClick -> Calcula() + SonDatosEntradaValidos().
    /// </summary>
    [RelayCommand]
    private async Task CalcularAsync()
    {
        var tipo = (AlgebraCombTipo)OperacionSeleccionada;

        // Validación de rutas requeridas según la operación (legacy: SonDatosEntradaValidos).
        // EliminaRepetidas solo necesita Combinación 1 + Final; el resto necesita las dos + Final.
        bool rutasOk = tipo == AlgebraCombTipo.EliminaRepetidas
            ? !string.IsNullOrEmpty(_rutaCombinacion1) && !string.IsNullOrEmpty(_rutaCombinacionFinal)
            : !string.IsNullOrEmpty(_rutaCombinacion1) && !string.IsNullOrEmpty(_rutaCombinacion2)
              && !string.IsNullOrEmpty(_rutaCombinacionFinal);

        if (!rutasOk)
        {
            Resultado = "No ha seleccionado los archivos";
            return;
        }

        // Validación de compatibilidad: mismo número de partidos y distinto de cero
        // (legacy: noSignos2 == noSignos1 && noSignos1 != 0 && noSignos2 != 0).
        // Para EliminaRepetidas solo participa la Combinación 1, por lo que solo se exige noSignos1 != 0.
        bool partidosOk = tipo == AlgebraCombTipo.EliminaRepetidas
            ? _noSignos1 != 0
            : _noSignos1 == _noSignos2 && _noSignos1 != 0 && _noSignos2 != 0;

        if (!partidosOk)
        {
            NombreCombinacion1 = "(selecciona)";
            NombreCombinacion2 = "(selecciona)";
            NombreCombinacionFinal = "(selecciona)";
            _rutaCombinacion1 = string.Empty;
            _rutaCombinacion2 = string.Empty;
            _rutaCombinacionFinal = string.Empty;
            AppServices.MostrarError("Los archivos tienen distinto número de partidos");
            return;
        }

        // Para EliminaRepetidas el motor usa noPartidos de la Combinación 1.
        int noPartidos = tipo == AlgebraCombTipo.EliminaRepetidas ? _noSignos1 : _noSignos2;
        string ruta1 = _rutaCombinacion1;
        string ruta2 = _rutaCombinacion2;
        string rutaFinal = _rutaCombinacionFinal;

        Resultado = string.Empty;
        PuedeCalcular = false;
        try
        {
            // El cálculo recorre archivos completos: ejecuta fuera del hilo de UI.
            string mensaje = await Task.Run(() =>
            {
                var sumador = new SumadorCombinaciones(noPartidos)
                {
                    ArchivoCols1 = ruta1,
                    ArchivoCols2 = ruta2,
                    ArchivoColsFinal = rutaFinal,
                };
                sumador.Calcula(tipo);
                return sumador.MensajeFinCalculo;
            });
            Resultado = mensaje;
        }
        catch (Exception ex)
        {
            Resultado = "Error: " + ex.Message;
        }
        finally
        {
            PuedeCalcular = true;
        }
    }

    /// <summary>
    /// Cierra/regresa sin ejecutar la operación. Legacy: BtnCancelarClick -> Close().
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // Navegación WinUI (Frame.GoBack) es responsabilidad del host de la Page, no del
        // cableado del dominio; equivale a AlgebraColumnasFrm.Close().
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
