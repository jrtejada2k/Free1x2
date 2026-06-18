using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "VisorEstadisticas".
/// Visor de solo lectura que muestra una rejilla de dos columnas (Archivo,
/// Cumplimiento) con el porcentaje de cumplimiento de cada fichero analizado.
/// Equivale al DataGridView "dgEstadisticas" alimentado por LlenarEstadisticas().
/// </summary>
public sealed partial class VisorEstadisticasPage : Page
{
    public VisorEstadisticasViewModel ViewModel { get; } = new();

    public VisorEstadisticasPage()
    {
        InitializeComponent();
    }
}
