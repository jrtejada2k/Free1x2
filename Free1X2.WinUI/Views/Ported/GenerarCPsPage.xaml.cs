// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada desde el WinForms "GenerarCPs" (Generador de Columnas Probables).
/// La lógica de dominio (CPs/IO/Porcentajes) está implementada en el VM.
/// </summary>
public sealed partial class GenerarCPsPage : Page
{
    public GenerarCPsViewModel ViewModel { get; } = new();

    public GenerarCPsPage()
    {
        InitializeComponent();
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }
}
