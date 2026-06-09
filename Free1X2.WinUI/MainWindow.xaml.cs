using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Free1X2.WinUI.Views;
using Free1X2.WinUI.Navigation;

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

        PoblarPantallasPortadas();
        ContentFrame.Navigate(typeof(HomePage));
    }

    // Puebla el NavigationView con las pantallas portadas desde WinForms (data-driven).
    // Cada ola de migración solo añade entradas a PortedPagesRegistry; aquí no se toca.
    private void PoblarPantallasPortadas()
    {
        string categoriaActual = null;
        Nav.MenuItems.Add(new NavigationViewItemSeparator());
        Nav.MenuItems.Add(new NavigationViewItemHeader { Content = "Pantallas portadas (WinUI)" });
        foreach (var p in PortedPagesRegistry.All)
        {
            if (p.Category != categoriaActual)
            {
                categoriaActual = p.Category;
                Nav.MenuItems.Add(new NavigationViewItemHeader { Content = p.Category });
            }
            var nvi = new NavigationViewItem { Content = p.Title, Tag = p.PageType };
            if (!string.IsNullOrEmpty(p.Glyph))
                nvi.Icon = new FontIcon { Glyph = p.Glyph };
            Nav.MenuItems.Add(nvi);
        }
    }

    private void Nav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
            return;
        }

        // Pantallas portadas: el Tag es el Type de la Page.
        if (args.SelectedItem is NavigationViewItem ni && ni.Tag is Type pageType)
        {
            ContentFrame.Navigate(pageType);
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
