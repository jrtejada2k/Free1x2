using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "ColGanadoraFrm" (Análisis de fallos en combinación).
/// Permite introducir la columna ganadora y elegir si analizar la combinación en pantalla
/// o abrir otra desde archivo, para detectar los fallos frente a esa columna.
/// </summary>
public sealed partial class ColGanadoraFrmPage : Page
{
    public ColGanadoraFrmViewModel ViewModel { get; } = new();

    public ColGanadoraFrmPage()
    {
        InitializeComponent();

        // La VM navega al visor del árbol de fallos a través del ContentFrame (mismo patrón
        // que MainPage): Analizar -> AnalizarCombinacionFrmPage (handoff estático UltimoAnalisis).
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    /// <summary>
    /// Recibe el contexto del flujo legacy (constructor ColGanadoraFrm): nº de partidos, nombre de
    /// la combinación en pantalla, su Analizador y la lista de pronósticos. El form legacy lo
    /// inyectaba por el constructor antes de mostrarse; aquí llega como parámetro de navegación
    /// (mismo patrón que MejoresOpcionesFrmPage.OnNavigatedTo con e.Parameter). Sin él, la rama
    /// "Analizar combinación en pantalla" avisa y "Abrir combinación" funciona de forma autónoma.
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is ColGanadoraContexto ctx)
        {
            ViewModel.EstablecerContexto(ctx.NumPartidos, ctx.NombreComb, ctx.Analizador, ctx.ListaPronosticos);
        }
    }

    private void OnAnalizarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.AnalizarCommand.Execute(null);
    }
}
