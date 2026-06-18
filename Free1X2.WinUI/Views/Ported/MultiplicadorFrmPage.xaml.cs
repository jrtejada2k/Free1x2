// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "MultiplicadorFrm" (título "Multiplicador").
/// Multiplica (producto cartesiano) dos archivos de combinaciones de 14 signos,
/// concatenando cada par de columnas en 28 cifras y reordenándolas según una
/// plantilla de 14 índices (1..28) que permite transponer columnas a la vez.
/// La lógica de dominio (carga/multiplicación/grabado de archivos) queda como TODO.
/// </summary>
public sealed partial class MultiplicadorFrmPage : Page
{
    public MultiplicadorFrmViewModel ViewModel { get; } = new();

    public MultiplicadorFrmPage()
    {
        InitializeComponent();
    }
}
