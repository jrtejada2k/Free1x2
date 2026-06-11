using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de la rejilla de "signos seguidos". Replica una de las 4 filas de datos del
/// WinForms <c>StaSSForm</c> (cant.1 / cant.X / cant.2 / cant.V). Cada fila tiene una
/// etiqueta y 15 celdas (columnas 0..14). Las celdas se exponen como string para no
/// bindear int/double a TextBlock.Text (regla anti-crash 2).
/// </summary>
public sealed class FilaSSViewModel
{
    public FilaSSViewModel(string etiqueta, IReadOnlyList<string> celdas)
    {
        Etiqueta = etiqueta;
        Celdas = celdas;
    }

    // label85 "cant. 1" / label153 "cant. X" / label119 "cant. 2" / label136 "cant. V".
    public string Etiqueta { get; }

    // 15 valores (columnas 0..14), ya formateados a string.
    public IReadOnlyList<string> Celdas { get; }
}

/// <summary>
/// ViewModel del WinForms <c>StaSSForm</c> (ventana "signos seguidos 0.1").
/// Muestra una rejilla de 4 filas (cant.1 / cant.X / cant.2 / cant.V) x 15 columnas (0..14)
/// con los resultados del análisis de "signos seguidos" sobre un conjunto de columnas.
/// El conmutador "mostrar" (porcentajes / columnas) decide si cada celda se muestra como
/// porcentaje sobre el total de columnas o como conteo bruto (RadioButtons rbPercent/rbCols
/// del GroupBox "mostrar" del legacy; rbPercent.Checked = true por defecto).
///
/// Los datos provienen de la matriz legacy <c>int[,] rsl</c> (filas 0..3 = 1/X/2/V,
/// columnas 0..14) y del entero <c>numcol</c> (número total de columnas analizadas),
/// pasados por el constructor <c>StaSSForm(int[,] ofparent, int ncol)</c>.
/// Aquí se dejan placeholders; el volcado real de datos es lógica de dominio (TODO).
/// </summary>
public partial class StaSSFormViewModel : ObservableObject
{
    public const int NumColumnasRejilla = 15; // columnas 0..14
    public const int NumFilas = 4;            // 1 / X / 2 / V

    private static readonly string[] EtiquetasFila = { "cant. 1", "cant. X", "cant. 2", "cant. V" };

    public StaSSFormViewModel()
    {
        // Cabecera de columnas: "0".."14" (label5..label19 del legacy).
        var cabecera = new List<string>(NumColumnasRejilla);
        for (int c = 0; c < NumColumnasRejilla; c++)
            cabecera.Add(c.ToString());
        Cabecera = cabecera;

        // Opciones del conmutador "mostrar" (GroupBox legacy con rbPercent / rbCols).
        Modos = new[] { "porcentajes", "columnas" };

        // Filas placeholder vacías hasta que el dominio rellene rsl/numcol.
        Filas = new ObservableCollection<FilaSSViewModel>();
        for (int f = 0; f < NumFilas; f++)
        {
            var celdas = new string[NumColumnasRejilla];
            for (int c = 0; c < NumColumnasRejilla; c++)
                celdas[c] = "-";
            Filas.Add(new FilaSSViewModel(EtiquetasFila[f], celdas));
        }

        // TODO: Dominio legacy — el constructor recibe (int[,] rsl, int numcol) y llama a
        //   PintaPantalla(): si rbPercent.Checked -> Porcentajes(), si no -> PintaColumnas().
        //   Porcentajes():  celda = (rsl[fila,col] * 10000 / numcol) / 1E2   (porcentaje, 2 dec.)
        //   PintaColumnas(): celda = rsl[fila,col]                            (conteo bruto)
        //   ColumnasTexto = numcol.ToString()  (label lncol del legacy).
        //   Rellamar a Recalcular() al cambiar ModoSeleccionado (RbColsCheckedChanged).
    }

    // Cabecera de columnas 0..14 (fila de labels superior del legacy).
    public IReadOnlyList<string> Cabecera { get; }

    // ItemsSource del ComboBox "mostrar" (regla anti-crash 3: lista del VM, sin <x:String> inline).
    public IReadOnlyList<string> Modos { get; }

    // Las 4 filas de datos de la rejilla.
    public ObservableCollection<FilaSSViewModel> Filas { get; }

    // 0 = porcentajes (rbPercent, por defecto), 1 = columnas (rbCols).
    [ObservableProperty]
    private int _modoSeleccionado;

    // lncol.Text — número total de columnas analizadas (placeholder hasta volcar numcol).
    [ObservableProperty]
    private string _columnasTexto = "-";

    partial void OnModoSeleccionadoChanged(int value)
    {
        // Equivale a RbColsCheckedChanged -> PintaPantalla() del legacy.
        // TODO: Dominio legacy — recalcular las celdas de Filas según el modo
        //   (porcentaje vs conteo) usando rsl/numcol y refrescar Filas.
    }
}
