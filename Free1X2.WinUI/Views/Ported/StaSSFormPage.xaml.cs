using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>StaSSForm</c> (ventana "signos seguidos 0.1").
/// Muestra una rejilla de 4 filas (cant.1 / cant.X / cant.2 / cant.V) x 15 columnas (0..14)
/// con la distribución de "signos seguidos" del conjunto de columnas analizadas, y un
/// conmutador "mostrar" (porcentajes / columnas). Los datos provienen del constructor legacy
/// <c>StaSSForm(int[,] rsl, int numcol)</c>; el volcado real está marcado como TODO en el ViewModel.
/// </summary>
public sealed partial class StaSSFormPage : Page
{
    public StaSSFormViewModel ViewModel { get; } = new();

    public StaSSFormPage()
    {
        this.InitializeComponent();
    }
}
