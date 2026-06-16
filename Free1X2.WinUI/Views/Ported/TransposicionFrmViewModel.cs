using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Transposición de columnas" (legacy: TransposicionFrm,
/// archivo UI\TransponedorFrm.cs).
///
/// Propósito: reordena los 14 signos de cada columna de un archivo de combinaciones.
/// El usuario indica, para cada una de las 14 posiciones de SALIDA, de qué posición
/// de la ENTRADA (1-14) debe tomarse el signo. El conjunto de valores debe ser una
/// permutación: cada número del 1 al 14 usado exactamente una vez (legacy: Verificar()
/// con BitArray verif de 14 bits + array int[] orde).
///
/// El procesado legacy (Transponer()) lee el archivo de entrada línea a línea y, para
/// cada columna, construye una nueva con aux[nr] = columna[orde[nr]].
/// </summary>
public partial class TransposicionFrmViewModel : ObservableObject
{
    // Rutas de archivos (legacy: filein / fileout).
    private string _rutaEntrada = string.Empty;
    private string _rutaSalida = string.Empty;

    // Las 14 posiciones de la permutación (legacy: textboxes tbc1..tbc14 -> orde[0..13]).
    // NumberBox.Value es double, por lo que las propiedades deben ser double (regla anti-crash 7).
    // Valor por defecto = identidad (1..14): no altera el orden hasta que el usuario lo cambie.
    [ObservableProperty]
    private double _pos1 = 1;
    [ObservableProperty]
    private double _pos2 = 2;
    [ObservableProperty]
    private double _pos3 = 3;
    [ObservableProperty]
    private double _pos4 = 4;
    [ObservableProperty]
    private double _pos5 = 5;
    [ObservableProperty]
    private double _pos6 = 6;
    [ObservableProperty]
    private double _pos7 = 7;
    [ObservableProperty]
    private double _pos8 = 8;
    [ObservableProperty]
    private double _pos9 = 9;
    [ObservableProperty]
    private double _pos10 = 10;
    [ObservableProperty]
    private double _pos11 = 11;
    [ObservableProperty]
    private double _pos12 = 12;
    [ObservableProperty]
    private double _pos13 = 13;
    [ObservableProperty]
    private double _pos14 = 14;

    // Nombre del archivo de entrada mostrado en pantalla.
    [ObservableProperty]
    private string _nombreEntrada = "(selecciona)";

    // Nombre del archivo de salida mostrado en pantalla.
    [ObservableProperty]
    private string _nombreSalida = "(selecciona)";

    // Mensaje de estado/resultado (legacy: bTransponer.Text "Procesando..." y MessageBox de error).
    [ObservableProperty]
    private string _estado = string.Empty;

    // Habilita/deshabilita el botón Transponer durante el proceso (legacy: bTransponer.Enabled).
    [ObservableProperty]
    private bool _puedeTransponer = true;

    /// <summary>
    /// Selecciona el archivo de columnas de entrada.
    /// Legacy: SeleccionarFicheros() -> OpenFileDialog (filtro ColumnasEntrada *.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarEntradaAsync()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;
        _rutaEntrada = file.Path;
        NombreEntrada = Path.GetFileName(_rutaEntrada);
    }

    /// <summary>
    /// Selecciona el archivo de columnas de salida.
    /// Legacy: SeleccionarFicheros() -> SaveFileDialog (filtro ColumnasSalida *.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarSalidaAsync()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "ColumnasTranspuestas",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;
        _rutaSalida = file.Path;
        NombreSalida = Path.GetFileName(_rutaSalida);
    }

    /// <summary>
    /// Ejecuta la transposición. Valida que las 14 posiciones formen una permutación
    /// (cada valor 1-14 exactamente una vez) y reordena cada columna del archivo.
    /// Legacy: BTransponerClick -> Transponer() (con Verificar()).
    /// </summary>
    [RelayCommand]
    private async Task TransponerAsync()
    {
        // Verificar() legacy: las 14 posiciones deben formar una permutación 1..14.
        var orde = new int[14];
        var verif = new BitArray(14);
        var pos = new[]
        {
            Pos1, Pos2, Pos3, Pos4, Pos5, Pos6, Pos7,
            Pos8, Pos9, Pos10, Pos11, Pos12, Pos13, Pos14,
        };
        bool valido = true;
        for (int i = 0; i < 14 && valido; i++)
        {
            int nx = (int)pos[i] - 1; // base 0 (legacy: Convert.ToInt32(tbcN)-1)
            if (nx < 0 || nx > 13) { valido = false; break; }
            verif[nx] = true;
            orde[i] = nx;
        }
        if (valido)
        {
            for (int nr = 0; nr < 14; nr++)
            {
                if (!verif[nr]) { valido = false; break; }
            }
        }
        if (!valido)
        {
            Estado = "Error en condiciones";
            AppServices.MostrarError("Error en condiciones");
            return;
        }

        if (string.IsNullOrEmpty(_rutaEntrada) || string.IsNullOrEmpty(_rutaSalida))
        {
            Estado = "Seleccione los archivos de entrada y salida.";
            return;
        }

        string rutaEntrada = _rutaEntrada;
        string rutaSalida = _rutaSalida;

        PuedeTransponer = false;
        Estado = "Procesando...";
        try
        {
            // Transponer() legacy: reordena los signos de cada columna (aux[nr] = columna[orde[nr]]).
            await Task.Run(() =>
            {
                using var sr = new StreamReader(rutaEntrada);
                using var sw = new StreamWriter(rutaSalida);
                while (sr.Peek() > 0)
                {
                    string columna = sr.ReadLine() ?? string.Empty;
                    var aux = new char[14];
                    for (int nr = 0; nr < 14; nr++) aux[nr] = columna[orde[nr]];
                    sw.WriteLine(new string(aux));
                }
            });
            Estado = "Transposición completada.";
        }
        catch (Exception ex)
        {
            Estado = "Error: " + ex.Message;
        }
        finally
        {
            PuedeTransponer = true;
        }
    }

    /// <summary>
    /// Cierra/regresa sin ejecutar. Legacy: cierre del formulario (Close()).
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO[dominio]: navegación WinUI — Frame.GoBack() o cerrar el host contenedor
        //   (equivale a TransposicionFrm.Close()).
    }
}
