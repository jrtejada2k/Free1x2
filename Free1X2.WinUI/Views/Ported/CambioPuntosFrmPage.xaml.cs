// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "CambioPuntosFrm" (Utilidades).
/// Permite editar y guardar la puntuación asignada a Fijos, Dobles y Triples.
/// </summary>
public sealed partial class CambioPuntosFrmPage : Page
{
    public CambioPuntosFrmViewModel ViewModel { get; } = new();

    public CambioPuntosFrmPage()
    {
        InitializeComponent();
    }

    private void OnAceptarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.GuardarCommand.Execute(null);

        // Tras guardar, regresa (legacy CambioPuntosFrm.btnOK_Click -> GuardarPuntos() que termina
        // en Close()). En navegación WinUI equivale a Frame.GoBack().
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }

    private void OnCancelarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Descarta y regresa (legacy: btnCancel_Click -> Close()). En navegación WinUI, Frame.GoBack().
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
