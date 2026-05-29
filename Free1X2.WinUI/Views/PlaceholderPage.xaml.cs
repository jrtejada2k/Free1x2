using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views;

public sealed partial class PlaceholderPage : Page
{
    public PlaceholderPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is string title && !string.IsNullOrEmpty(title))
            Titulo.Text = title;
    }
}
