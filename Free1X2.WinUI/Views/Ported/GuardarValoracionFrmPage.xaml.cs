using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Página WinUI portada del WinForms legacy Free1X2.UI.GuardarValoracionFrm.
    /// Permite elegir el formato del fichero y el separador de campos para
    /// exportar valoraciones a un .txt.
    /// </summary>
    public sealed partial class GuardarValoracionFrmPage : Page
    {
        public GuardarValoracionFrmViewModel ViewModel { get; } = new GuardarValoracionFrmViewModel();

        public GuardarValoracionFrmPage()
        {
            this.InitializeComponent();
        }
    }
}
