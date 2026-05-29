using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
        TemaRadios.SelectedIndex = 2; // Sistema por defecto
    }

    private void TemaRadios_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (App.MainWindow?.Content is not FrameworkElement root) return;
        root.RequestedTheme = TemaRadios.SelectedIndex switch
        {
            0 => ElementTheme.Light,
            1 => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };
    }
}
