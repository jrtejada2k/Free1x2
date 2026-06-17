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

        // Sincroniza el visor embebido con el BoletoFrmViewModel reutilizado: cada vez que
        // cambia el boleto (carga / navegación) se vuelca en el BoletoMatrizControl.
        ViewModel.Boleto.BoletoCambiado += OnBoletoCambiado;

        // Legacy btnOk_Click: new BoletoFrm{...}.ShowDialog() abría el boleto como ventana
        // separada. Aquí se navega a BoletoFrmPage (mismo patrón static-handoff que EstucolFrm
        // -> VisorAnalisisColumnasAbdonFrm): VerBoletosViewModel publica AbrirBoletoSolicitado
        // con (fichero, orden, tipo) y la página los deja en BoletoFrmPage.ParametrosBoleto
        // antes de Frame.Navigate, igual que el legacy fijaba ArchivoCombinacion/ordenarPor/tipoOrden.
        ViewModel.AbrirBoletoSolicitado += OnAbrirBoletoSolicitado;
    }

    private void OnAbrirBoletoSolicitado(object? sender, (string fichero, Free1X2.OrdenarMatriz orden, Free1X2.TipoOrden tipo) e)
    {
        BoletoFrmPage.ParametrosBoleto = (e.fichero, e.orden, e.tipo);
        Frame?.Navigate(typeof(BoletoFrmPage));
    }

    private void OnBoletoCambiado(object? sender, (string[] signos, int[] numerosColumna) e)
    {
        BoletoVisual.Llenar(e.signos, e.numerosColumna);
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        // El form legacy cerraba la ventana (btnCancel_Click -> this.Close()).
        // Aquí se navega hacia atrás.
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
