using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página WinUI 3 portada del formulario WinForms "VerBoletos" (título: "Ver Boletos").
/// Permite seleccionar un fichero de columnas, elegir el criterio de ordenación
/// ("Ordenar por ...") y el sentido ("Tipo de ordenamiento", ascendente/descendente)
/// y mostrar el boleto resultante.
/// </summary>
public sealed partial class VerBoletosPage : Page
{
    public VerBoletosViewModel ViewModel { get; } = new();

    public VerBoletosPage()
    {
        this.InitializeComponent();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        // El form legacy cerraba la ventana (btnCancel_Click -> this.Close()).
        // Aquí se navega hacia atrás.
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
