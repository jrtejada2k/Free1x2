using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>NoVariantesFrm</c> (filtro "Número de Variantes").
/// Permite introducir, para Var / X / 2, las cantidades (0–15) admitidas de cada
/// concepto dentro de una combinación. La lógica de dominio (FiltroNoVariantes,
/// ArchivoCondiciones, CalculadorEstadisticas, Grupo) está marcada como TODO en el ViewModel.
/// </summary>
public sealed partial class NoVariantesFrmPage : Page
{
    public NoVariantesFrmViewModel ViewModel { get; } = new();

    public NoVariantesFrmPage()
    {
        this.InitializeComponent();
    }
}
