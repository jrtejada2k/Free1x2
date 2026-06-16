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
        // Legacy: SalirFrm.btnOK_Click -> exit = true; this.Close().
        // El comando guarda la preferencia y llama Application.Current.Exit().
        ViewModel.ConfirmarSalidaCommand.Execute(null);
    }

    private void OnCancelarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Legacy: SalirFrm.btnCancel_Click -> exit = false; this.Close().
        ViewModel.CancelarSalidaCommand.Execute(null);

        // Cerrar el diálogo sin salir: si esta página está dentro de un Frame, retroceder.
        if (Frame is { CanGoBack: true })
        {
            Frame.GoBack();
        }
    }
}
