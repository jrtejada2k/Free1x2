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

        // TODO[dominio]: recibir el contexto del flujo legacy (constructor ColGanadoraFrm):
        //   nPartidos, nombreCombi (string), analizador (Free1X2.MotorCalculo.Analizador) y
        //   listaPronosticos (string[]). En navegación WinUI pasar estos datos como parámetro
        //   de OnNavigatedTo y reenviarlos al ViewModel (EstablecerNumeroPartidos, etc.).
    }

    private void OnAnalizarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.AnalizarCommand.Execute(null);
    }
}
