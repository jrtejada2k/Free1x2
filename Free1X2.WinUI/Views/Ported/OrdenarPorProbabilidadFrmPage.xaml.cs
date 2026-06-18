using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página WinUI 3 portada del formulario WinForms "OrdenarPorProbabilidadFrm"
/// (título: "Ordenación de columnas por probabilidad").
/// Ordena o filtra las columnas de la quiniela en torno a un valor central
/// (por productos, por sumas o múltiple) y las escribe a un fichero de salida.
/// </summary>
public sealed partial class OrdenarPorProbabilidadFrmPage : Page
{
    public OrdenarPorProbabilidadFrmViewModel ViewModel { get; } = new();

    public OrdenarPorProbabilidadFrmPage()
    {
        this.InitializeComponent();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        // El form legacy cerraba la ventana; aquí se navega hacia atrás.
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
