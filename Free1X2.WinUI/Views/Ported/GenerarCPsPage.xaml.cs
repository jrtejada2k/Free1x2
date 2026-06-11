using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada desde el WinForms "GenerarCPs" (Generador de Columnas Probables).
/// Réplica de UI; la lógica de dominio (CPs/IO/Porcentajes) queda como TODO en el VM.
/// </summary>
public sealed partial class GenerarCPsPage : Page
{
    public GenerarCPsViewModel ViewModel { get; } = new();

    public GenerarCPsPage()
    {
        InitializeComponent();
    }
}
