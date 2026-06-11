using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    private string _rutaResultado = string.Empty;

    // Columnas cargadas en memoria (legacy: ascols1/ascols2 y sus contadores ncols1/ncols2,
    // y el resultado ascols3 con ncols3). El dominio real las mantendría; aquí solo se exponen
    // los contadores para la UI.

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
    private void SeleccionarEntrada1()
    {
        // TODO[dominio]: abrir FileOpenPicker (*.txt) y cargar columnas.
        //   Legacy MultiplicadorFrm.Entrada1():
        //     - PuedeProcesar = false (deshabilita bEntra1/bEntra2/bMultiplica).
        //     - Por cada línea: scol1 = (linea + "11111111111111").Substring(0,14).Trim();
        //       ascols1[ncols1++] = scol1.
        //     - NombreEntrada1 = Path.GetFileName(ruta);
        //       ColumnasEntrada1Texto = ncols1.ToString();
        //     - PuedeProcesar = true.
    }

    /// <summary>
    /// Selecciona y carga el archivo de la Entrada Comb-2.
    /// Legacy: MultiplicadorFrm.Entrada2() -> OpenFileDialog (*.txt).
    /// </summary>
    [RelayCommand]
    private void SeleccionarEntrada2()
    {
        // TODO[dominio]: abrir FileOpenPicker (*.txt) y cargar columnas.
        //   Legacy MultiplicadorFrm.Entrada2():
        //     - PuedeProcesar = false.
        //     - Por cada línea: scol2 = (linea + "11111111111111").Substring(0,14).Trim();
        //       ascols2[ncols2++] = scol2.
        //     - NombreEntrada2 = Path.GetFileName(ruta);
        //       ColumnasEntrada2Texto = ncols2.ToString();
        //     - PuedeProcesar = true.
    }

    /// <summary>
    /// Realiza el producto cartesiano de ambas entradas aplicando la plantilla de índices.
    /// Legacy: MultiplicadorFrm.Multiplicar() + RecuperaPantalla().
    /// </summary>
    [RelayCommand]
    private void Multiplicar()
    {
        // TODO[dominio]: validar plantilla y ejecutar la multiplicación.
        //   Legacy MultiplicadorFrm.RecuperaPantalla():
        //     - indices[0..13] = Convert.ToInt32(Indice01..Indice14).
        //     - Validar: cada indice en rango 1..28; si no, Mensaje = "error en plantilla" y abortar.
        //   Legacy MultiplicadorFrm.Multiplicar():
        //     - PuedeProcesar = false; ncols3 = 0.
        //     - Para cada (nr1 en ncols1, nr2 en ncols2):
        //         scol3 = ascols1[nr1] + ascols2[nr2];   // 28 cifras
        //         para nr3 en 0..13: aux[nr3] = scol3[indices[nr3]-1];
        //         ascols3[ncols3++] = new string(aux);
        //     - ColumnasResultadoTexto = ncols3.ToString();
        //     - PuedeGrabar = true; PuedeProcesar = true.
    }

    /// <summary>
    /// Graba las columnas resultantes a un archivo de texto.
    /// Legacy: MultiplicadorFrm.Grabar() -> SaveFileDialog (*.txt).
    /// </summary>
    [RelayCommand]
    private void Grabar()
    {
        // TODO[dominio]: abrir FileSavePicker (*.txt) y escribir el resultado.
        //   Legacy MultiplicadorFrm.Grabar():
        //     - PuedeGrabar = false; PuedeProcesar = false.
        //     - fileout = Path.GetFileName(ruta);
        //       StreamWriter sw -> por cada nr en 0..ncols3-1: sw.WriteLine(ascols3[nr]).
        //     - PuedeGrabar = true; PuedeProcesar = true.
    }
}
