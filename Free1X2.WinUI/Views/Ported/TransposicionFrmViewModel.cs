using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    private void SeleccionarEntrada()
    {
        // TODO[dominio]: abrir FileOpenPicker (*.txt) y asignar:
        //   _rutaEntrada = ruta seleccionada;
        //   NombreEntrada = Path.GetFileName(ruta);
        //   Legacy TransposicionFrm.SeleccionarFicheros() (rama del OpenFileDialog).
    }

    /// <summary>
    /// Selecciona el archivo de columnas de salida.
    /// Legacy: SeleccionarFicheros() -> SaveFileDialog (filtro ColumnasSalida *.txt).
    /// </summary>
    [RelayCommand]
    private void SeleccionarSalida()
    {
        // TODO[dominio]: abrir FileSavePicker (*.txt) y asignar:
        //   _rutaSalida = ruta seleccionada;
        //   NombreSalida = Path.GetFileName(ruta);
        //   Legacy TransposicionFrm.SeleccionarFicheros() (rama del SaveFileDialog).
    }

    /// <summary>
    /// Ejecuta la transposición. Valida que las 14 posiciones formen una permutación
    /// (cada valor 1-14 exactamente una vez) y reordena cada columna del archivo.
    /// Legacy: BTransponerClick -> Transponer() (con Verificar()).
    /// </summary>
    [RelayCommand]
    private void Transponer()
    {
        // TODO[dominio]: validar y procesar.
        //   Legacy TransposicionFrm.Verificar():
        //     int[] orde = new int[14]; orde[i] = (int)Pos{i+1} - 1;  // base 0
        //     Validar con BitArray(14): cada índice 0..13 debe quedar marcado exactamente
        //     una vez y estar en rango; si no -> Estado = "Error en condiciones" y abortar.
        //   Legacy TransposicionFrm.Transponer():
        //     PuedeTransponer = false; Estado = "Procesando...";
        //     using var sr = new StreamReader(_rutaEntrada);
        //     using var sw = new StreamWriter(_rutaSalida);
        //     while (sr.Peek() > 0) {
        //         string columna = sr.ReadLine();
        //         var aux = new char[14];
        //         for (int nr = 0; nr < 14; nr++) aux[nr] = columna[orde[nr]];
        //         sw.WriteLine(new string(aux));
        //     }
        //     PuedeTransponer = true; Estado = "Transposición completada.";
        //   Nota: en el acceso externo legacy (constructor con int[] s, FileIn, FileOut)
        //   se cerraba la ventana al terminar (if (AccesoExterno) Close();).
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
