// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>SelecJM</c> ("Selección 5+5+4 de JuanM").
/// Reparte los 14 partidos en 3 grupos (5+5+4), genera las tablas ordenadas de
/// productos de cada grupo, aplica límites por grupo y por total, y calcula las
/// columnas válidas por corte o por valor. Incluye análisis de columnas ganadoras
/// leídas de fichero. El cálculo está portado y toma la valoración de la rejilla
/// PorcentajesControl (== valors1.RetVals()).
/// </summary>
public sealed partial class SelecJMPage : Page
{
    public SelecJMViewModel ViewModel { get; } = new();

    public SelecJMPage()
    {
        this.InitializeComponent();
    }
}
