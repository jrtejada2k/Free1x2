// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>TriosFrm</c> ("Tríos").
/// Analiza un fichero de columnas de entrada validando cada columna contra una
/// tabla de "Límites" de 5 niveles (mínimo / máximo por nivel) y una matriz de
/// "Niveles" de sólo lectura. Muestra columnas procesadas / válidas, el tiempo y
/// un resultado por nivel. La lógica de validación, lectura de fichero, cálculo de
/// la matriz y persistencia de condiciones está implementada en el ViewModel.
/// </summary>
public sealed partial class TriosFrmPage : Page
{
    public TriosFrmViewModel ViewModel { get; } = new();

    public TriosFrmPage()
    {
        this.InitializeComponent();
    }
}
