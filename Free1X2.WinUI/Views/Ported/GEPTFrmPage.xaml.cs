// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "GEPTFrm" — columna estadística de Abdon (GEPT).
/// Permite registrar las dos últimas ocasiones (G/E/P) de cada partido y calcular
/// el pronóstico de signos resultante (1 / X / 2 / 12 / 1X / X2).
/// </summary>
public sealed partial class GEPTFrmPage : Page
{
    public GEPTFrmViewModel ViewModel { get; } = new();

    public GEPTFrmPage()
    {
        InitializeComponent();
    }
}
