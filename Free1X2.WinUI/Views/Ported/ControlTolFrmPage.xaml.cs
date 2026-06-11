using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    public sealed partial class ControlTolFrmPage : Page
    {
        public ControlTolFrmViewModel ViewModel { get; } = new ControlTolFrmViewModel();

        public ControlTolFrmPage()
        {
            this.InitializeComponent();
        }
    }
}
