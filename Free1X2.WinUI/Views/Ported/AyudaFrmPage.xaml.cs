// Free1X2 · WinUI 3 — WIN3
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port de WinForms AyudaFrm (categoría Ayuda).
/// Pantalla "Recursos de ayuda": enlaces a documentación (Manual, FAQ) y comunidad (Foro).
/// W11/DT-86: clubprogol.com es WordPress y las direcciones antiguas dan 404; Artículos,
/// Recursos, Notificaciones y Facebook se quitan (solo mostraban avisos en inglés).
/// Mismas direcciones que la app Mac (enum Web).
/// Pantalla sin parámetros de entrada → no requiere ViewModel.
/// </summary>
public sealed partial class AyudaFrmPage : Page
{
    // En minúsculas: WordPress normaliza así las direcciones.
    internal const string UrlManual = "https://clubprogol.com/docwk/";
    internal const string UrlFaq    = "https://clubprogol.com/docwk/faq/";
    internal const string UrlForo   = "https://clubprogol.com/foro/";

    public AyudaFrmPage()
    {
        this.InitializeComponent();
    }

    private async void ManualLink_Click(object sender, RoutedEventArgs e)
    {
        await AbrirUriSeguraAsync(UrlManual);
    }

    private async void FaqLink_Click(object sender, RoutedEventArgs e)
    {
        await AbrirUriSeguraAsync(UrlFaq);
    }

    private async void ForoLink_Click(object sender, RoutedEventArgs e)
    {
        await AbrirUriSeguraAsync(UrlForo);
    }

    private static async System.Threading.Tasks.Task AbrirUriSeguraAsync(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            await Launcher.LaunchUriAsync(uri);
        }
    }
}
