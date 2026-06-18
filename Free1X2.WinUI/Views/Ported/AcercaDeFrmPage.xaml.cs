// Free1X2 · WinUI 3 — WIN3
using System;
using System.IO;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.System;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port de WinForms AcercaDeFrm (categoría Ayuda).
/// Diálogo "Acerca de": identidad de la app (logo, nombre, versión "Rarotonga"),
/// lema y enlaces a documentación / sitio web / foro / créditos.
/// Pantalla sin parámetros de entrada → no requiere ViewModel.
/// </summary>
public sealed partial class AcercaDeFrmPage : Page
{
    // URLs externas (idénticas a las del form legacy AcercaDeFrm).
    private const string UrlDescarga = "https://clubprogol.com/Descargas/Descargas.aspx";
    private const string UrlForo     = "https://clubprogol.com/foros/index.php";
    private const string UrlSitioWeb = "https://clubprogol.com";
    private const string UrlManual   = "https://clubprogol.com/DocWK";

    public AcercaDeFrmPage()
    {
        this.InitializeComponent();
        this.Loaded += AcercaDeFrmPage_Loaded;
    }

    private void AcercaDeFrmPage_Loaded(object sender, RoutedEventArgs e)
    {
        // El form legacy hacía: lblVersion.Text = "Versión " + Application.ProductVersion + " Rarotonga";
        // Application.ProductVersion en WinForms equivale a la FileVersion del ensamblado; aquí
        // usamos la versión del ensamblado (csproj <Version>/<FileVersion>), conservando " Rarotonga".
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.81.2";
        VersionTextBlock.Text = $"Versión {version} Rarotonga";

        // Logo real de la app (imgLogo en WinForms, resources.GetObject("imgLogo.Image")).
        // El recurso legacy (AcercaDeFrm.resx, JPEG 110x110) se extrajo a Assets/logo.jpg y se
        // empaqueta como Content (ms-appx:///Assets/logo.jpg).
        try
        {
            LogoImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/logo.jpg"));
        }
        catch
        {
            // Si el recurso faltara, se deja el recuadro vacío (no se inventa logo).
        }
    }

    private async void LicenciaLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy: Process.Start(Application.StartupPath + "/Documentacion/licencia.txt").
        // Abre el documento de licencia empaquetado junto al exe (Documentacion/licencia.txt),
        // copia de LICENSE (GPLv3). Si faltara, cae al sitio web.
        await AbrirDocumentoLocalAsync("Documentacion/licencia.txt", UrlSitioWeb);
    }

    private async void GplLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy: Process.Start(Application.StartupPath + "/Documentacion/GPL.txt").
        // Abre el documento GPL empaquetado junto al exe (Documentacion/GPL.txt),
        // copia de LICENSE (GPLv3). Si faltara, cae al sitio web.
        await AbrirDocumentoLocalAsync("Documentacion/GPL.txt", UrlSitioWeb);
    }

    private async void DescargaLink_Click(object sender, RoutedEventArgs e)
    {
        await AbrirUriSeguraAsync(UrlDescarga);
    }

    private async void ManualLink_Click(object sender, RoutedEventArgs e)
    {
        await AbrirUriSeguraAsync(UrlManual);
    }

    private async void ForoLink_Click(object sender, RoutedEventArgs e)
    {
        await AbrirUriSeguraAsync(UrlForo);
    }

    private async void SitioWebLink_Click(object sender, RoutedEventArgs e)
    {
        await AbrirUriSeguraAsync(UrlSitioWeb);
    }

    private void CreditosLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy: new CreditosFrm().ShowDialog();
        // Navega a la pantalla de Créditos portada (CreditosFrmPage) en el Frame de
        // contenido, mismo patrón que el resto de páginas (p. ej. VerBoletosPage).
        Frame?.Navigate(typeof(CreditosFrmPage));
    }

    private static async System.Threading.Tasks.Task AbrirUriSeguraAsync(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            await Launcher.LaunchUriAsync(uri);
        }
    }

    /// <summary>
    /// Abre un documento empaquetado junto al ejecutable (rutas relativas a AppContext.BaseDirectory,
    /// como hacía el legacy con Application.StartupPath). Replica el Process.Start(rutaLocal) del
    /// WinForms usando StorageFile + Launcher.LaunchFileAsync. Si el fichero no existe o falla la
    /// apertura, cae a la URL del sitio web (robustez).
    /// </summary>
    private static async System.Threading.Tasks.Task AbrirDocumentoLocalAsync(string rutaRelativa, string urlFallback)
    {
        try
        {
            string ruta = Path.Combine(AppContext.BaseDirectory, rutaRelativa.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(ruta))
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(ruta);
                bool ok = await Launcher.LaunchFileAsync(file);
                if (ok) return;
            }
        }
        catch
        {
            // Si no se puede abrir el documento local, se intenta el sitio web.
        }

        await AbrirUriSeguraAsync(urlFallback);
    }
}
