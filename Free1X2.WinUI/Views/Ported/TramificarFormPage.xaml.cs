using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>TramificarForm</c> ("Tramificar").
/// Reparte el universo de columnas (3^14) en tramos según su valoración, usando los
/// datos de premios de la L.A.E. de una jornada (temporada, jornada, recaudación e
/// importes 14..10), calcula la probabilidad acumulada y permite filtrar por las
/// posiciones mínimas/máximas de cada categoría de premio y localizar columnas
/// concretas. Toda la lógica de dominio (cálculo, escrutinio, persistencia y apertura
/// de otros forms) está marcada como TODO en el ViewModel.
/// </summary>
public sealed partial class TramificarFormPage : Page
{
    public TramificarFormViewModel ViewModel { get; } = new();

    public TramificarFormPage()
    {
        this.InitializeComponent();
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
    }
}
