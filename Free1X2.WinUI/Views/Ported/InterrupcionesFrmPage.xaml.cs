using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>InterrupcionesFrm</c> (filtro "Interrupciones").
/// Una interrupción es cada cambio de signo a lo largo de la columna. La página
/// permite introducir, para Global / Var / 1 / X / 2, los valores admitidos (0–14)
/// tanto de interrupciones como de interrupciones seguidas. La lógica de dominio
/// (FiltroInterrupciones, ArchivoCondiciones, CalculadorEstadisticas) está marcada
/// como TODO en el ViewModel.
/// </summary>
public sealed partial class InterrupcionesFrmPage : Page
{
    public InterrupcionesFrmViewModel ViewModel { get; } = new();

    public InterrupcionesFrmPage()
    {
        this.InitializeComponent();
    }
}
