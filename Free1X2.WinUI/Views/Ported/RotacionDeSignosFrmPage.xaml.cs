// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page portada del WinForms RotacionDeSignosFrm ("Rotación de signos").
    /// </summary>
    public sealed partial class RotacionDeSignosFrmPage : Page
    {
        public RotacionDeSignosFrmViewModel ViewModel { get; } = new RotacionDeSignosFrmViewModel();

        public RotacionDeSignosFrmPage()
        {
            this.InitializeComponent();
        }
    }
}
