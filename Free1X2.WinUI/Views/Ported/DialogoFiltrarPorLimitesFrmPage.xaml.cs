// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "DialogoFiltrarPorLimitesFrm".
/// Permite, por cada tramo de aciertos, fijar el rango de diferencias cuyas
/// columnas deben eliminarse (matriz legacy int[10,4] 'extremos').
/// </summary>
public sealed partial class DialogoFiltrarPorLimitesFrmPage : Page
{
    /// <summary>
    /// Handoff estático de ENTRADA: la matriz de extremos que el caller pasaba por constructor
    /// (legacy: new DialogoFiltrarPorLimitesFrm(extremos)). La deja el productor (TramificarForm)
    /// antes de navegar y la consume <see cref="OnNavigatedTo"/> (ViewModel.Inicializar).
    /// </summary>
    public static int[,]? ExtremosEntrada { get; set; }

    /// <summary>
    /// Handoff estático de SALIDA: tras "Aceptar"/"Cancelar", deja el resultado que el caller
    /// legacy leía de las propiedades públicas (ValoresAceptados + extremos). El productor lo
    /// consume al volver (Frame.GoBack -> OnNavigatedTo en modo Back).
    /// </summary>
    public static (bool aceptado, int[,] extremos)? Resultado { get; set; }

    public DialogoFiltrarPorLimitesFrmViewModel ViewModel { get; } = new();

    public DialogoFiltrarPorLimitesFrmPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Legacy: el ctor recibía la matriz extremos. Aquí llega por el handoff estático.
        if (ExtremosEntrada is { } entrada)
        {
            ExtremosEntrada = null; // se consume una sola vez
            ViewModel.Inicializar(entrada);
        }
    }

    private void OnAceptarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Legacy btAceptar_Click: PonerTextoEnVariables(); CoherenciarExtremos();
        // ValoresAceptados = true; Close(). El comando hace lo primero; aquí publicamos el
        // resultado para el caller y regresamos (el caller lo lee al volver, modo Back).
        ViewModel.AceptarCommand.Execute(null);
        Resultado = (ViewModel.ValoresAceptados, ViewModel.Extremos);
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }

    private void OnCancelarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Legacy button1_Click: Close() sin marcar ValoresAceptados.
        ViewModel.CancelarCommand.Execute(null);
        Resultado = (false, ViewModel.Extremos);
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
