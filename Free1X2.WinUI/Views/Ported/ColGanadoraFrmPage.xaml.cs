using Microsoft.UI.Xaml.Controls;

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

        // TODO[contexto-en-pantalla]: la rama "Analizar combinación en pantalla" requiere el
        //   contexto del flujo legacy (constructor ColGanadoraFrm): nPartidos, nombreCombi,
        //   analizador (Free1X2.MotorCalculo.Analizador) y listaPronosticos (string[]). Cuando
        //   la pantalla llamante pase estos datos por navegación (e.Parameter), reenviarlos en
        //   OnNavigatedTo a ViewModel.EstablecerContexto(...). La rama "Abrir combinación" ya
        //   funciona de forma autónoma (carga el .comb desde el picker).
    }

    private void OnAnalizarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.AnalizarCommand.Execute(null);
    }
}
