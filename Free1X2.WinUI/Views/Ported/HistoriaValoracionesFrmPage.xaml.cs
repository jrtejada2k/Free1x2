using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page WinUI portada del WinForms legacy "HistoriaValoracionesFrm".
/// Guarda las valoraciones 1X2 de una jornada/temporada en el fichero historico.
/// </summary>
public sealed partial class HistoriaValoracionesFrmPage : Page
{
    public HistoriaValoracionesFrmViewModel ViewModel { get; } = new();

    public HistoriaValoracionesFrmPage()
    {
        InitializeComponent();
    }
}
