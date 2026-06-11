using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "DescargaBoletoFrm".
/// Permite elegir jornada y temporada para descargar el boleto oficial online.
/// </summary>
public sealed partial class DescargaBoletoFrmPage : Page
{
    public DescargaBoletoFrmViewModel ViewModel { get; } = new();

    public DescargaBoletoFrmPage()
    {
        InitializeComponent();
    }

    private void OnDescargarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.DescargarCommand.Execute(null);

        // TODO[dominio]: tras una descarga exitosa, publicar el boleto y cerrar/regresar.
        //   Legacy: DescargaBoletoFrm.btnActualizar_Click -> MainForm.BoletoOnline = boleto; Close();
        //   En navegación WinUI, decidir aquí Frame.GoBack() o cerrar el host contenedor
        //   y entregar el boleto descargado a la pantalla principal (BoletoPage / shell).
    }
}
