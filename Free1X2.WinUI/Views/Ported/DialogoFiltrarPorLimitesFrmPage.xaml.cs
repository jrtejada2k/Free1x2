using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "DialogoFiltrarPorLimitesFrm".
/// Permite, por cada tramo de aciertos, fijar el rango de diferencias cuyas
/// columnas deben eliminarse (matriz legacy int[10,4] 'extremos').
/// </summary>
public sealed partial class DialogoFiltrarPorLimitesFrmPage : Page
{
    public DialogoFiltrarPorLimitesFrmViewModel ViewModel { get; } = new();

    public DialogoFiltrarPorLimitesFrmPage()
    {
        InitializeComponent();
    }

    private void OnAceptarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Legacy btAceptar_Click: PonerTextoEnVariables(); CoherenciarExtremos();
        // ValoresAceptados = true; Close(). El comando hace lo primero; aquí cerramos/regresamos.
        // El caller lee ViewModel.Extremos y ViewModel.ValoresAceptados tras el regreso.
        ViewModel.AceptarCommand.Execute(null);
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }

    private void OnCancelarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Legacy button1_Click: Close() sin marcar ValoresAceptados.
        ViewModel.CancelarCommand.Execute(null);
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
