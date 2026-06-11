using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>SignosSeguidosFrm</c> (filtro "Signos Seguidos").
/// Permite introducir, para Var / 1 / X / 2, las cantidades (0–15) admitidas de cada
/// concepto SEGUIDO dentro de una combinación, y definir las "Figuras" asociadas a cada uno.
/// La lógica de dominio (FiltroSignosSeguidos, FigurasFiltrosFrm, ArchivoCondiciones,
/// CalculadorEstadisticas, Grupo) está marcada como TODO en el ViewModel.
/// </summary>
public sealed partial class SignosSeguidosFrmPage : Page
{
    public SignosSeguidosFrmViewModel ViewModel { get; } = new();

    public SignosSeguidosFrmPage()
    {
        this.InitializeComponent();
    }
}
