// Free1X2 · WinUI 3 — WIN3
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port de WinForms AyudaFrm (categoría Ayuda).
/// Pantalla "Recursos de ayuda": enlaces a documentación (Manual, Artículos, FAQ,
/// Recursos) y comunidad (Foro, Notificaciones, Facebook).
/// Pantalla sin parámetros de entrada → no requiere ViewModel.
/// </summary>
public sealed partial class AyudaFrmPage : Page
{
    // Única URL que el form legacy abría directamente (linkFAQ_LinkClicked).
    private const string UrlFaq = "http://www.free1x2.com/DocWK/index.php?title=FAQ";

    public AyudaFrmPage()
    {
        this.InitializeComponent();
    }

    private async void FaqLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.linkFAQ_LinkClicked: Process.Start(UrlFaq).
        await AbrirUriSeguraAsync(UrlFaq);
    }

    private void ManualLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.linkManual_LinkClicked mostraba un MessageBox:
        //   "Online help disabled for offline operation. Please refer to local documentation."
        // TODO(dominio): abrir el manual local empaquetado (Assets/Documentacion)
        //   vía StorageFile + Launcher.LaunchFileAsync, o navegar a AcercaDeFrmPage/Manual.
    }

    private void ArticulosLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.linkArticulos_LinkClicked: MessageBox "Online help disabled…".
        // TODO(dominio): definir destino de "Artículos" (documentación local o ContentDialog).
    }

    private void RecursosLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.linkRecursos_LinkClicked: MessageBox "Online help disabled…".
        // TODO(dominio): definir destino de "Recursos" (documentación local o ContentDialog).
    }

    private void ForoLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.linkForo_LinkClicked: MessageBox "Online help disabled…".
        // TODO(dominio): abrir el foro de la comunidad (Launcher.LaunchUriAsync) cuando
        //   se restablezca el acceso online.
    }

    private void NotificacionesLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.lnkNotificaciones_LinkClicked: MessageBox
        //   "Notifications system disabled for performance."
        // TODO(dominio): integrar el sistema de notificaciones cuando esté habilitado.
    }

    private void FacebookLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.lnkFacebook_LinkClicked: MessageBox "Online help disabled…".
        // TODO(dominio): abrir la página de Facebook (Launcher.LaunchUriAsync) cuando
        //   se restablezca el acceso online.
    }

    private static async System.Threading.Tasks.Task AbrirUriSeguraAsync(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            await Launcher.LaunchUriAsync(uri);
        }
    }
}
