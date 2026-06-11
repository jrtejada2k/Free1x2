using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page portada del WinForms legacy "CalculoFormatosFrm" (Free1X2.UI.CalculoFormatosFrm).
    /// </summary>
    public sealed partial class CalculoFormatosFrmPage : Page
    {
        public CalculoFormatosFrmViewModel ViewModel { get; } = new CalculoFormatosFrmViewModel();

        public CalculoFormatosFrmPage()
        {
            this.InitializeComponent();
        }
    }
}
