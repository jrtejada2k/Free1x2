// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "AnalizarFicheroFrm".
/// Selecciona un fichero de columnas (.txt/.cols), muestra cuántas columnas contiene
/// y lanza el análisis de la combinación, con opción de incluir el pleno al 15.
/// </summary>
public sealed partial class AnalizarFicheroFrmPage : Page
{
    public AnalizarFicheroFrmViewModel ViewModel { get; } = new();

    public AnalizarFicheroFrmPage()
    {
        InitializeComponent();
    }

    private void OnCancelarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO[dominio]: descartar y cerrar/regresar (legacy: btnCancel_Click -> this.Close()).
        //   En navegación WinUI, invocar Frame.GoBack() o cerrar el host contenedor.
    }
}
