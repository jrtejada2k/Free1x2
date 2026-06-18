// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

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

        // Flujo "Filtrar" legacy (btFiltrar_Click): el VM calcula los extremos y pide abrir
        // DialogoFiltrarPorLimitesFrm; aquí se pasa la matriz por el handoff estático y se navega
        // a la página del diálogo. El resultado se recoge al volver (OnNavigatedTo, modo Back).
        ViewModel.AbrirDialogoFiltrarPorLimites = extremos =>
        {
            DialogoFiltrarPorLimitesFrmPage.ExtremosEntrada = extremos;
            DialogoFiltrarPorLimitesFrmPage.Resultado = null;
            Frame?.Navigate(typeof(DialogoFiltrarPorLimitesFrmPage));
        };
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Al volver de DialogoFiltrarPorLimitesFrmPage (legacy: if (DialogoFiltrar.ValoresAceptados))
        // se aplica el filtro con los extremos editados; si se canceló, no se hace nada.
        if (DialogoFiltrarPorLimitesFrmPage.Resultado is { } r)
        {
            DialogoFiltrarPorLimitesFrmPage.Resultado = null;
            if (r.aceptado)
            {
                await ViewModel.AplicarFiltroConExtremos(r.extremos);
            }
        }
    }
}
