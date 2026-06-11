using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página WinUI 3 portada del diálogo WinForms "DialogoSeleccionBancoPruebasFrm"
/// ("Selección de columnas").
///
/// Permite acotar entre un mínimo y un máximo cada uno de los 18 conceptos del escrutinio
/// del Banco de Pruebas; al Aceptar se seleccionan las columnas comprendidas en esos rangos.
/// La lógica de cálculo / persistencia / cierre de diálogo se delega al ViewModel como TODO
/// de dominio (legacy: ValoresMinimosyMaximos, clase Free1X2.Utils.TablaDeSeleccion).
/// </summary>
public sealed partial class DialogoSeleccionBancoPruebasFrmPage : Page
{
    public DialogoSeleccionBancoPruebasFrmViewModel ViewModel { get; } = new();

    public DialogoSeleccionBancoPruebasFrmPage()
    {
        this.InitializeComponent();
    }

    private void BtnAceptar_Click(object sender, RoutedEventArgs e)
    {
        // legacy: button1_Click cerraba el diálogo (Cancelado = false ya lo fija el comando).
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        // legacy: btCancelar_Click cerraba el diálogo (Cancelado = true ya lo fija el comando).
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
