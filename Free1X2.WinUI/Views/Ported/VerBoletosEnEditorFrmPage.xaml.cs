using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Port WinUI de Free1X2.UI.VerBoletosEnEditorFrm.
    /// Muestra las columnas de apuesta formateadas como boletos verticales
    /// (editor de texto de solo lectura, fuente monoespaciada) y permite imprimirlas.
    /// </summary>
    public sealed partial class VerBoletosEnEditorFrmPage : Page
    {
        public VerBoletosEnEditorFrmViewModel ViewModel { get; } = new VerBoletosEnEditorFrmViewModel();

        public VerBoletosEnEditorFrmPage()
        {
            this.InitializeComponent();

            // TODO Legacy: el constructor de VerBoletosEnEditorFrm recibía string[] columnas
            // y las formateaba. Aquí las columnas deben llegar por navegación (OnNavigatedTo)
            // y pasarse a ViewModel.CargarColumnas(columnas) cuando se conecte el dominio.
            // ViewModel.CargarColumnas(columnas);
        }
    }
}
