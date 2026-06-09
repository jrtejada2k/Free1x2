using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Free1X2.WinUI.Views;

namespace Free1X2.WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        // Mica + barra de título integrada (look Win11 nativo)
        this.SystemBackdrop = new MicaBackdrop();
        this.ExtendsContentIntoTitleBar = true;
        this.SetTitleBar(AppTitleBar);
        this.Title = "Free1X2";

        ContentFrame.Navigate(typeof(HomePage));
    }

    private void Nav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
            return;
        }

        if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag)
        {
            Type page = tag switch
            {
                "home"         => typeof(HomePage),
                "boleto"       => typeof(BoletoPage),
                "componentes"  => typeof(ComponentesPage),
                "filtros"      => typeof(PlaceholderPage),
                "operaciones"  => typeof(PlaceholderPage),
                "estadisticas" => typeof(PlaceholderPage),
                "escrutinio"   => typeof(PlaceholderPage),
                _              => typeof(HomePage),
            };
            ContentFrame.Navigate(page, item.Content?.ToString());
        }
    }
}
