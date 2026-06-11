using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    public sealed partial class ImportadorCPsFrmPage : Page
    {
        public ImportadorCPsFrmViewModel ViewModel { get; } = new ImportadorCPsFrmViewModel();

        public ImportadorCPsFrmPage()
        {
            InitializeComponent();
        }
    }
}
