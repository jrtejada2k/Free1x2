// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "TransposicionFrm" (título "Transposición de
/// columnas"). Reordena los 14 signos de cada columna del archivo de entrada según
/// una permutación que indica el usuario. La lógica de dominio (lectura/escritura de
/// archivos y reordenado) queda como TODO.
/// </summary>
public sealed partial class TransposicionFrmPage : Page
{
    public TransposicionFrmViewModel ViewModel { get; } = new();

    public TransposicionFrmPage()
    {
        InitializeComponent();
    }
}
