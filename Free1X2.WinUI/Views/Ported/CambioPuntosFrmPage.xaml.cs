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

        // TODO[dominio]: tras guardar, cerrar/regresar.
        //   En el flujo legacy CambioPuntosFrm.btnOK_Click llamaba GuardarPuntos() y Close().
        //   En navegación WinUI, decidir aquí Frame.GoBack() o cerrar el host contenedor.
    }

    private void OnCancelarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO[dominio]: descartar y cerrar/regresar (legacy: btnCancel_Click -> Close()).
        //   En navegación WinUI, invocar Frame.GoBack() o cerrar el host contenedor.
    }
}
