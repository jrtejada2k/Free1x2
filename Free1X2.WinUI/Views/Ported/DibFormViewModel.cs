using System.Collections.Generic;
using System.Collections.ObjectModel;
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
/// Toda la lógica de dominio (la matriz rsl, numcol y el cálculo de porcentajes/columnas)
/// está marcada como TODO.
/// </summary>
public partial class DibFormViewModel : ObservableObject
{
    // Cabeceras de signo de las filas (label2..label9 + label238/label272/... del legacy).
    // Orden visual del legacy: "2","1","3","0","13","..." — aquí índice de fila 0..14.
    private static readonly IReadOnlyList<string> CabecerasFila = new[]
    {
        "0", "1", "2", "3", "4", "5", "6", "7",
        "8", "9", "10", "11", "12", "13", "14",
    };

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
        // Estructura de demostración: 15 filas vacías con su cabecera de signo.
        // El contenido real (Celdas / TotalFila) lo poblará la lógica de dominio.
        for (int f = 0; f < CabecerasFila.Count; f++)
        {
            Filas.Add(new DibFila
            {
                Cabecera = CabecerasFila[f],
                Celdas = string.Empty,
                TotalFila = string.Empty,
            });
        }
    }

    // Reacciona al cambio del selector "mostrar" (rbCols.CheckedChanged -> PintaPantalla legacy).
    partial void OnMostrarColumnasChanged(bool value)
    {
        Repintar();
    }

    private void Repintar()
    {
        // Equivale a DibForm.PintaPantalla() del legacy:
        //   lncol.Text = "" + numcol;
        //   if (rbPercent.Checked) Porcentajes(); else PintaColumnas();
        //
        // TODO: Dominio legacy — recibir desde Anastatics la matriz int[15,15] rsl y numcol
        //   (constructor legacy: DibForm(int[,] ofparent, int ncol)).
        //   Con MostrarColumnas == false -> Porcentajes():
        //       cada celda lt[f,c] = (rsl[f,c] * 10000 / numcol) / 1E2 (porcentaje con 2 decimales),
        //       lx## = suma de la fila f, ld## = suma de la columna c,
        //       lv## = sumas por diagonal (numvars[nf+nc] += rsl[nf,nc]).
        //   Con MostrarColumnas == true -> PintaColumnas():
        //       cada celda lt[f,c] = "" + rsl[f,c] (recuento bruto) y los mismos totales sin dividir.
        //   Volcar numcol a NumColTexto y reconstruir Filas (Celdas/TotalFila ya formateados).
    }
}
