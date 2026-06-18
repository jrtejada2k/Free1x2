// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    public sealed partial class ImportadorCPsFrmPage : Page
    {
        public ImportadorCPsFrmViewModel ViewModel { get; } = new ImportadorCPsFrmViewModel();

        public ImportadorCPsFrmPage()
        {
            InitializeComponent();
            ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        }
    }
}
