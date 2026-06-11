using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "SalirFrm".
/// Diálogo de confirmación de salida: pregunta "¿Salir del programa?",
/// ofrece la opción de no volver a mostrar la advertencia y botones Sí / Cancelar.
/// </summary>
public sealed partial class SalirFrmPage : Page
{
    public SalirFrmViewModel ViewModel { get; } = new();

    public SalirFrmPage()
    {
        InitializeComponent();
    }

    private void OnConfirmarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.ConfirmarSalidaCommand.Execute(null);

        // TODO[dominio]: tras confirmar, terminar la aplicación.
        //   Legacy: SalirFrm.btnOK_Click -> exit = true; this.Close();
        //   MainForm consumía SalirFrm.exit para cerrar la app.
    }

    private void OnCancelarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.CancelarSalidaCommand.Execute(null);

        // TODO[dominio]: cerrar el diálogo sin salir.
        //   Legacy: SalirFrm.btnCancel_Click -> exit = false; this.Close();
        //   En navegación WinUI, invocar Frame.GoBack() o cerrar el host contenedor.
    }
}
