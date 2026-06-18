using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page portada del WinForms "Filtro Pim" (clase legacy Free1X2.UI.GeneraPim).
    /// </summary>
    public sealed partial class GeneraPimPage : Page
    {
        public GeneraPimViewModel ViewModel { get; } = new();

        public GeneraPimPage()
        {
            this.InitializeComponent();
        }
    }
}
