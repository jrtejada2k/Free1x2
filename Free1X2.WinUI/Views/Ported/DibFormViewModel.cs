// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una celda de la tabla de distribución del WinForms <c>DibForm</c> (etiqueta lt#### legacy).
/// Se expone como string (regla anti-crash: nada de int/double directo a TextBlock.Text).
/// <see cref="Visibilidad"/> reproduce la forma triangular del legacy: la fila nf (nº de X)
/// sólo tiene celdas para columnas nc (nº de 2) con nf+nc &lt;= 14; el resto del rectángulo
/// 15x15 no existe en el form original (no hay label lt####) y queda como hueco (Collapsed).
/// </summary>
public sealed class DibCelda
{
    public string Texto { get; init; } = string.Empty;

    /// <summary>Visibilidad de la celda (Collapsed en el hueco triangular del legacy).</summary>
    public Visibility Visibilidad { get; init; } = Visibility.Visible;
}

/// <summary>
/// Una fila de la tabla de distribución del WinForms <c>DibForm</c>.
/// Expone propiedades string/colecciones para bindear con x:Bind (regla anti-crash):
///   - <see cref="Cabecera"/> = nº de X de la fila (0..14), encabezado izquierdo del legacy.
///   - <see cref="Celdas"/>   = celdas lt#### de la fila (matriz triangular), ya formateadas.
///   - <see cref="TotalFila"/>= total de la fila (lx## legacy = suma de toda la fila).
/// </summary>
public sealed class DibFila
{
    public string Cabecera { get; init; } = string.Empty;
    public IReadOnlyList<DibCelda> Celdas { get; init; } = System.Array.Empty<DibCelda>();
    public string TotalFila { get; init; } = string.Empty;
}

/// <summary>
/// ViewModel del WinForms <c>Free1X2.UI.Estadisticas.DibForm</c> (ventana "dibujos").
/// El form legacy recibe una matriz <c>int[15,15] rsl</c> y <c>int numcol</c> desde Anastatics,
/// y pinta una rejilla triangular de etiquetas: celdas lt#### (filas nf = nº de X 0..14,
/// columnas nc = nº de 2 0..14, con nf+nc &lt;= 14), totales de fila lx## (suma de cada fila),
/// totales de columna ld## (suma de cada columna) y totales de diagonal/variantes lv##
/// (suma de las celdas con nf+nc == v). El RadioButton "mostrar" alterna entre Porcentajes()
/// (rbPercent, por defecto) y PintaColumnas() (rbCols).
///
/// Es un sub-diálogo de SÓLO PRESENTACIÓN: no tiene E/S de fichero ni llama al motor; sólo
/// formatea la matriz que le entrega su form padre (Anastatics). En WinUI esa matriz se
/// entrega mediante el handoff estático <see cref="MatrizEntrada"/> / <see cref="NumColEntrada"/>,
/// que rellena el productor antes de navegar (igual patrón que VisorEstadisticas.UltimasEstadisticas).
/// Mientras Anastatics no lo rellene la tabla se muestra a 0.
/// </summary>
public partial class DibFormViewModel : ObservableObject
{
    /// <summary>Tamaño de la matriz del legacy (rsl = new int[15,15]).</summary>
    private const int N = 15;

    /// <summary>
    /// Matriz int[15,15] entregada por el form padre (legacy: ctor DibForm(int[,] ofparent, int ncol)).
    /// Handoff estático de proceso, análogo a VisorEstadisticas.UltimasEstadisticas. El productor
    /// (AnastaticsViewModel modo "Variantes, X, 2") la asigna y asigna NumColEntrada antes de
    /// navegar a DibFormPage (Free1X2/UI/Estadisticas/DibForm.cs línea 244).
    /// </summary>
    public static int[,]? MatrizEntrada { get; set; }

    /// <summary>Total de columnas analizadas (legacy: numcol). Handoff estático.</summary>
    public static int NumColEntrada { get; set; }

    // Encabezados de fila (nº de X de la fila, 0..14): label izquierdo del legacy.
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

    // Encabezados de columna (nº de 2, 0..14): fila de labels superior del legacy.
    public IReadOnlyList<string> CabecerasColumna { get; } = CabecerasFila;

    // Filas de la tabla de distribución (DibFila con celdas y total de fila).
    public ObservableCollection<DibFila> Filas { get; } = new();

    // Totales por columna (ld## legacy = suma de toda la columna nc), ya formateados.
    public ObservableCollection<string> TotalesColumna { get; } = new();

    // Totales por variantes (lv## legacy = suma de las celdas con nf+nc == v), v 0..14.
    public ObservableCollection<string> TotalesVariantes { get; } = new();

    public DibFormViewModel()
    {
        // Toma el handoff del form padre (legacy: rsl = ofparent; numcol = ncol).
        _rsl = MatrizEntrada ?? new int[N, N];
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
    /// Porcentajes():  celda = (rsl[f,c]*10000/numcol)/1E2  (porcentaje, 2 decimales)
    /// PintaColumnas(): celda = rsl[f,c]                     (conteo bruto)
    /// Los totales lx## (fila), ld## (columna) y lv## (variantes) se calculan igual que el legacy.
    /// </summary>
    private void Repintar()
    {
        NumColTexto = _numcol.ToString(CultureInfo.InvariantCulture);
        Filas.Clear();
        TotalesColumna.Clear();
        TotalesVariantes.Clear();

        // Filas + total de fila (lx## = suma de los 15 elementos de la fila nf).
        for (int nf = 0; nf < N; nf++)
        {
            var celdas = new List<DibCelda>(N);
            long sumaFila = 0;
            for (int nc = 0; nc < N; nc++)
            {
                int valor = _rsl[nf, nc];
                sumaFila += valor;
                // Forma triangular del legacy: sólo existe label lt#### si nf+nc <= 14.
                bool visible = nf + nc < N;
                celdas.Add(new DibCelda
                {
                    Texto = visible ? Formatear(valor) : string.Empty,
                    Visibilidad = visible ? Visibility.Visible : Visibility.Collapsed,
                });
            }
            Filas.Add(new DibFila
            {
                Cabecera = CabecerasFila[nf],
                Celdas = celdas,
                TotalFila = Formatear(sumaFila),
            });
        }

        // Totales por columna (ld## = suma de los 15 elementos de la columna nc).
        for (int nc = 0; nc < N; nc++)
        {
            long suma = 0;
            for (int nf = 0; nf < N; nf++) suma += _rsl[nf, nc];
            TotalesColumna.Add(Formatear(suma));
        }

        // Totales por variantes (lv## = suma de las celdas con nf+nc == v), v 0..14.
        // Legacy: numvars[nf+nc] += rsl[nf,nc] (array de 29; se muestran lv00..lv14).
        var numvars = new long[2 * N - 1];
        for (int nf = 0; nf < N; nf++)
            for (int nc = 0; nc < N; nc++)
                numvars[nf + nc] += _rsl[nf, nc];
        for (int v = 0; v < N; v++)
            TotalesVariantes.Add(Formatear(numvars[v]));
    }

    /// <summary>
    /// Formatea un valor según el modo: porcentaje (rsl*10000/numcol)/1E2 o conteo bruto.
    /// Réplica de la aritmética entera del legacy (division entera antes de /1E2).
    /// </summary>
    private string Formatear(long valor)
    {
        var inv = CultureInfo.InvariantCulture;
        if (MostrarColumnas)
            return valor.ToString(inv); // PintaColumnas(): conteo bruto.

        // Porcentajes(): (valor*10000/numcol)/1E2 con la misma división entera del legacy.
        int divisor = _numcol == 0 ? 1 : _numcol;
        double pct = (valor * 10000 / divisor) / 1E2;
        return pct.ToString(inv);
    }
}
