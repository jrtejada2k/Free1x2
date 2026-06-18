// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "AlgebraColumnasFrm" (título "Álgebra").
/// Realiza álgebra de combinaciones entre archivos de columnas: eliminar
/// repetidas, sumar, intersección de comunes o resta. La lógica de dominio
/// (SumadorCombinaciones, selección de archivos) está implementada en el ViewModel.
/// </summary>
public sealed partial class AlgebraColumnasFrmPage : Page
{
    public AlgebraColumnasFrmViewModel ViewModel { get; } = new();

    public AlgebraColumnasFrmPage()
    {
        InitializeComponent();
    }
}
