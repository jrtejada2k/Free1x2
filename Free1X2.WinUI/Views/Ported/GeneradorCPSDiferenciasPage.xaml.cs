// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página WinUI portada de <c>Free1X2.UI.GeneradorCPSDiferencias</c> (WinForms).
/// Replica las entradas y acciones del formulario "Generador de CPs por diferencias".
/// La lógica de dominio (validación, combinación y persistencia de columnas) queda como TODO
/// en <see cref="GeneradorCPSDiferenciasViewModel"/>, citando los métodos legacy.
/// </summary>
public sealed partial class GeneradorCPSDiferenciasPage : Page
{
    public GeneradorCPSDiferenciasViewModel ViewModel { get; } = new();

    public GeneradorCPSDiferenciasPage()
    {
        InitializeComponent();
    }
}
