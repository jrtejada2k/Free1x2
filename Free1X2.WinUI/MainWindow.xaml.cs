using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.UI.Dispatching;
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
        ContentFrame.Navigate(typeof(MainPage));

        // Smoke test permanente: solo se activa con FREE1X2_SMOKE=1. En ejecucion
        // normal es un no-op. Recorre todas las pantallas portadas (+ MainPage/HomePage),
        // registra fallos en %TEMP%\free1x2_smoke.log y cierra la app con exit 0.
        if (Environment.GetEnvironmentVariable("FREE1X2_SMOKE") == "1")
        {
            IniciarSmokeTest();
        }
    }

    // ===== Smoke test gated por FREE1X2_SMOKE (permanente, no-op sin la env var) =====

    private DispatcherQueueTimer? _smokeTimer;
    private List<Type>? _smokeRuta;
    private int _smokeIndice;
    private int _smokeOk;
    private int _smokeFail;
    private string _smokeLog = "";

    private void IniciarSmokeTest()
    {
        _smokeLog = Path.Combine(Path.GetTempPath(), "free1x2_smoke.log");
        try { File.WriteAllText(_smokeLog, "SMOKE START\r\n"); } catch { }

        // Ruta: MainPage + HomePage + todas las pantallas portadas del registro.
        _smokeRuta = new List<Type> { typeof(MainPage), typeof(HomePage) };
        foreach (var p in PortedPagesRegistry.All)
            _smokeRuta.Add(p.PageType);

        _smokeIndice = 0;
        _smokeOk = 0;
        _smokeFail = 0;

        _smokeTimer = DispatcherQueue.CreateTimer();
        _smokeTimer.Interval = TimeSpan.FromMilliseconds(120);
        _smokeTimer.Tick += SmokeTimer_Tick;
        _smokeTimer.Start();
    }

    private void SmokeTimer_Tick(DispatcherQueueTimer sender, object args)
    {
        if (_smokeRuta == null) { return; }

        if (_smokeIndice >= _smokeRuta.Count)
        {
            _smokeTimer?.Stop();
            int total = _smokeOk + _smokeFail;
            SmokeAppend($"SMOKE DONE total={total} ok={_smokeOk} fail={_smokeFail}");
            Application.Current.Exit();
            return;
        }

        Type pageType = _smokeRuta[_smokeIndice++];
        try
        {
            ContentFrame.Navigate(pageType);
            _smokeOk++;
        }
        catch (Exception ex)
        {
            _smokeFail++;
            SmokeAppend($"FAIL {pageType.FullName}: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private void SmokeAppend(string linea)
    {
        try { File.AppendAllText(_smokeLog, linea + "\r\n"); } catch { }
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
                "home"         => typeof(MainPage),
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
