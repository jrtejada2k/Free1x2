// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

// Page portada del WinForms legacy Free1X2.UI.FiltroPorcenJB
// ("Separador Porcentajes Juan Bellas").
public sealed partial class FiltroPorcenJBPage : Page
{
    public FiltroPorcenJBViewModel ViewModel { get; } = new();

    public FiltroPorcenJBPage()
    {
        InitializeComponent();
    }
}
