// Free1X2 · WinUI 3 — WIN3
using System;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.77.2";
        VersionTextBlock.Text = $"Versión {version} Rarotonga";

        // TODO(dominio/recursos): cargar el logo real de la app.
        //   En WinForms era imgLogo con resources.GetObject("imgLogo.Image").
        //   Migrar el recurso a Assets/ y asignar:
        //   LogoImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png"));
    }

    private async void LicenciaLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy: Process.Start(Application.StartupPath + "/Documentacion/licencia.txt")
        // TODO(dominio): localizar y abrir el documento empaquetado de licencia
        //   (Documentacion/licencia.txt) vía StorageFile + Launcher.LaunchFileAsync.
        await AbrirUriSeguraAsync(UrlSitioWeb);
    }

    private async void GplLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy: Process.Start(Application.StartupPath + "/Documentacion/GPL.txt")
        // TODO(dominio): abrir el documento empaquetado GPL.txt vía StorageFile + Launcher.
        await AbrirUriSeguraAsync(UrlSitioWeb);
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
}
