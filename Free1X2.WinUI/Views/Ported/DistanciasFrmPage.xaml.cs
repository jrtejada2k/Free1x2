using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>DistanciasFrm</c> (filtro "Distancias").
/// Permite introducir, para Var / 1 / X / 2, las distancias máximas (0–15) admitidas
/// entre dos signos iguales. La lógica de dominio (FiltroDistancias, ArchivoCondiciones,
/// CalculadorEstadisticas) está marcada como TODO en el ViewModel.
/// </summary>
public sealed partial class DistanciasFrmPage : Page
{
    public DistanciasFrmViewModel ViewModel { get; } = new();

    public DistanciasFrmPage()
    {
        this.InitializeComponent();
    }
}
