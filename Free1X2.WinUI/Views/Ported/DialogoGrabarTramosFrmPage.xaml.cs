// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port de Free1X2.UI.DialogoGrabarTramosFrm (WinForms) — diálogo "Grabar Tramos".
/// Captura un rango de columnas y el patrón "grabar N de cada P columnas".
/// </summary>
public sealed partial class DialogoGrabarTramosFrmPage : Page
{
    public DialogoGrabarTramosFrmViewModel ViewModel { get; } = new();

    public DialogoGrabarTramosFrmPage()
    {
        InitializeComponent();
    }
}
