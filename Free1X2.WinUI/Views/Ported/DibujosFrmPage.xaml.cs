using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>DibujosFrm</c> (categoría Filtros).
/// Filtro de "dibujos X+2": el usuario selecciona en una malla las figuras (NumX + Num2)
/// que deben cumplir las columnas. Equivale al filtro <c>FiltroDibujos</c> del dominio.
/// </summary>
public sealed partial class DibujosFrmPage : Page
{
    public DibujosFrmViewModel ViewModel { get; } = new();

    public DibujosFrmPage()
    {
        this.InitializeComponent();
    }
}
