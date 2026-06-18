// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "ProbabilidadPremios" (Free1X2/UI/ProbabilidadPremios.cs).
/// Compara un fichero de columnas "madre" (base) con uno de columnas "hijas" y calcula, para
/// cada categoría de aciertos dentro del rango indicado, la probabilidad de premio.
/// El botón Calcular solo se habilita cuando ambos ficheros han sido seleccionados.
/// </summary>
public sealed partial class ProbabilidadPremiosPage : Page
{
    public ProbabilidadPremiosViewModel ViewModel { get; } = new();

    public ProbabilidadPremiosPage()
    {
        InitializeComponent();
    }
}
