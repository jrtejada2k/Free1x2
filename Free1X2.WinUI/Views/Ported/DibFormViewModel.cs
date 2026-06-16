using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de la tabla de distribución del WinForms <c>DibForm</c>.
/// Expone SOLO propiedades string para poder bindearse con x:Bind en el DataTemplate
/// (regla anti-crash: nada de int/bool directo a TextBlock.Text).
///   - <see cref="Cabecera"/> = cabecera de signo de la fila (label2..label9 legacy: "2","1","3","0",...).
///   - <see cref="Celdas"/>   = celdas lt#### de la fila ya formateadas y unidas en una cadena.
///   - <see cref="TotalFila"/>= total de la fila (lx## legacy).
/// </summary>
public sealed class DibFila
{
    public string Cabecera { get; init; } = string.Empty;
    public string Celdas { get; init; } = string.Empty;
    public string TotalFila { get; init; } = string.Empty;
}

/// <summary>
/// ViewModel del WinForms <c>Free1X2.UI.Estadisticas.DibForm</c> (ventana "dibujos").
/// El form legacy recibe una matriz <c>int[15,15] rsl</c> y <c>int numcol</c> desde Anastatics,
/// y pinta una rejilla de etiquetas: celdas lt#### (matriz), totales de fila lx##,
/// totales de columna ld## y totales de diagonal/variantes lv##. El RadioButton "mostrar"
/// alterna entre Porcentajes() (rbPercent, por defecto) y PintaColumnas() (rbCols).
///
/// Es un sub-diálogo de SÓLO PRESENTACIÓN: no tiene E/S de fichero ni llama al motor; sólo
/// formatea la matriz que le entrega su form padre (Anastatics). En WinUI esa matriz se
/// entrega mediante el handoff estático <see cref="MatrizEntrada"/> / <see cref="NumColEntrada"/>,
/// que rellena el productor antes de navegar (igual patrón que AppState.GrupoEnEdicion).
/// Mientras Anastatics no lo rellene la tabla se muestra a 0 (TODO documentado).
/// </summary>
public partial class DibFormViewModel : ObservableObject
{
    /// <summary>
    /// Matriz int[15,15] entregada por el form padre (legacy: ctor DibForm(int[,] ofparent, int ncol)).
    /// Handoff estático de proceso, análogo a AppState.GrupoEnEdicion.
    /// TODO[dominio]: el productor (AnastaticsViewModel / pantalla padre) debe asignar esta matriz
    ///   y NumColEntrada antes de navegar a DibFormPage (Free1X2/UI/Estadisticas/DibForm.cs línea 244).
    /// </summary>
    public static int[,]? MatrizEntrada { get; set; }

    /// <summary>Total de columnas analizadas (legacy: numcol). Handoff estático.</summary>
    public static int NumColEntrada { get; set; }

    // Cabeceras de signo de las filas (label2..label9 + label238/label272/... del legacy).
    // Orden visual del legacy: "2","1","3","0","13","..." — aquí índice de fila 0..14.
    private static readonly IReadOnlyList<string> CabecerasFila = new[]
    {
        "0", "1", "2", "3", "4", "5", "6", "7",
        "8", "9", "10", "11", "12", "13", "14",
    };

    // Matriz y total de columnas tomados del handoff en construcción.
    private readonly int[,] _rsl;
    private readonly int _numcol;

    // rbPercent.Checked = true por defecto => MostrarColumnas = false (porcentajes).
    // ToggleSwitch.IsOn (On = columnas / Off = porcentajes).
    [ObservableProperty]
    private bool _mostrarColumnas;

    // lncol.Text = "" + numcol. Propiedad string (anti-crash: no bindear int a Text).
    [ObservableProperty]
    private string _numColTexto = "0";

    // Filas de la tabla de distribución (DibFila con propiedades string).
    public ObservableCollection<DibFila> Filas { get; } = new();

    public DibFormViewModel()
    {
        // Toma el handoff del form padre (legacy: rsl = ofparent; numcol = ncol).
        _rsl = MatrizEntrada ?? new int[15, 15];
        _numcol = NumColEntrada;
        Repintar();
    }

    // Reacciona al cambio del selector "mostrar" (rbCols.CheckedChanged -> PintaPantalla legacy).
    partial void OnMostrarColumnasChanged(bool value)
    {
        Repintar();
    }

    /// <summary>
    /// Equivale a DibForm.PintaPantalla() del legacy:
    ///   lncol.Text = "" + numcol;
    ///   if (rbPercent.Checked) Porcentajes(); else PintaColumnas();
    /// Con MostrarColumnas == false -> porcentaje (rsl[f,c]*10000/numcol)/1E2;
    /// Con MostrarColumnas == true  -> recuento bruto rsl[f,c]. El total de fila es la suma.
    /// </summary>
    private void Repintar()
    {
        NumColTexto = _numcol.ToString();
        Filas.Clear();

        int filas = _rsl.GetLength(0);
        int cols = _rsl.GetLength(1);
        int divisor = _numcol == 0 ? 1 : _numcol;
        var inv = CultureInfo.InvariantCulture;

        for (int f = 0; f < filas; f++)
        {
            var sbCeldas = new StringBuilder();
            long totalFila = 0;
            for (int c = 0; c < cols; c++)
            {
                int valor = _rsl[f, c];
                totalFila += valor;
                if (c > 0) sbCeldas.Append("  ");
                if (MostrarColumnas)
                {
                    // PintaColumnas(): recuento bruto (legacy: lt####.Text = "" + rsl[f,c]).
                    sbCeldas.Append(valor.ToString(inv));
                }
                else
                {
                    // Porcentajes(): (rsl[f,c]*10000/numcol)/1E2 (2 decimales).
                    double pct = (valor * 10000 / divisor) / 1E2;
                    sbCeldas.Append(pct.ToString(inv));
                }
            }

            string totalTexto = MostrarColumnas
                ? totalFila.ToString(inv)
                : ((totalFila * 10000 / divisor) / 1E2).ToString(inv);

            Filas.Add(new DibFila
            {
                Cabecera = f < CabecerasFila.Count ? CabecerasFila[f] : f.ToString(),
                Celdas = sbCeldas.ToString(),
                TotalFila = totalTexto,
            });
        }
    }
}
