using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>BuscaLimsFrm</c> ("Buscador límites").
/// Recorre todas las columnas posibles y calcula los valores mínimo/máximo de
/// Sumas y Productos para las columnas cuyos aciertos respecto a la columna base
/// caen dentro del rango indicado. Muestra columnas Procesadas, Admitidas y Tiempo.
/// La lógica de cálculo está portada: la valoración procede de la rejilla
/// PorcentajesControl (== valors1.RetVals()) y alimenta AnalizarColumnas/UtilColumnas.
/// </summary>
public sealed partial class BuscaLimsFrmPage : Page
{
    public BuscaLimsFrmViewModel ViewModel { get; } = new();

    public BuscaLimsFrmPage()
    {
        this.InitializeComponent();
    }
}
