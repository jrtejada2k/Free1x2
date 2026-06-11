using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página WinUI 3 portada del formulario WinForms "BancoPruebasFrm"
/// (Simulador de escrutinios — Banco de Pruebas).
///
/// Asistente de 4 pasos: Ficheros, Valoraciones, Simular 14's y Escrutinios.
/// La lógica de cálculo / persistencia se delega al ViewModel como TODO de dominio.
/// </summary>
public sealed partial class BancoPruebasFrmPage : Page
{
    public BancoPruebasFrmViewModel ViewModel { get; } = new();

    public BancoPruebasFrmPage()
    {
        this.InitializeComponent();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        // El form legacy cerraba la ventana (btnCancel). Aquí se navega hacia atrás si es posible.
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
