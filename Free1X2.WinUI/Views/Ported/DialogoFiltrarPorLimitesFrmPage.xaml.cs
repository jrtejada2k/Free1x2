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
        ViewModel.AceptarCommand.Execute(null);

        // TODO[dominio]: tras aceptar, devolver la matriz 'extremos' al caller y cerrar/regresar.
        //   Legacy: DialogoFiltrarPorLimitesFrm.btAceptar_Click ->
        //     PonerTextoEnVariables(); CoherenciarExtremos(); ValoresAceptados = true; Close();
        //   En navegación WinUI, decidir aquí Frame.GoBack() o cerrar el host contenedor.
    }

    private void OnCancelarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO[dominio]: descartar y cerrar/regresar (legacy: button1_Click -> Close() sin marcar ValoresAceptados).
        //   En navegación WinUI, invocar Frame.GoBack() o cerrar el host contenedor.
    }
}
