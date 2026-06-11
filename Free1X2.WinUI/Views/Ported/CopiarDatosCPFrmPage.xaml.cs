using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>CopiarDatosCPFrm</c> ("Copiar Datos").
/// Diálogo modal que permite elegir un rango de columnas [Desde, Hasta] a copiar.
/// El rango inicial (desde, max) llegaba por el constructor del form; aquí se pasa
/// como parámetro de navegación (un par de valores) o se deja en el rango por defecto.
/// </summary>
public sealed partial class CopiarDatosCPFrmPage : Page
{
    public CopiarDatosCPFrmViewModel ViewModel { get; } = new();

    public CopiarDatosCPFrmPage()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Equivale al constructor <c>CopiarDatosCPFrm(long desde, long max)</c>:
    /// recibe el rango inicial como parámetro de navegación (desde, max).
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // El llamador legacy construía el form con (desde, max). Aquí esos valores
        // llegarían vía e.Parameter (p. ej. una tupla (double Desde, double Max)).
        if (e.Parameter is System.ValueTuple<double, double> rango)
        {
            ViewModel.Inicializar(rango.Item1, rango.Item2);
        }
    }
}
