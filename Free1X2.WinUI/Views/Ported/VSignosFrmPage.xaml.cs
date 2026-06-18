// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "VSignosFrm" (Análisis de signos).
/// Procesa un fichero de columnas y contabiliza, partido a partido, la frecuencia
/// de cada signo (1/X/2), con opciones de formato (% enteros, % decimales o recuentos),
/// columna ganadora de referencia y nivel de aspiración para el escrutinio parcial.
/// </summary>
public sealed partial class VSignosFrmPage : Page
{
    public VSignosFrmViewModel ViewModel { get; } = new();

    public VSignosFrmPage()
    {
        InitializeComponent();
    }
}
