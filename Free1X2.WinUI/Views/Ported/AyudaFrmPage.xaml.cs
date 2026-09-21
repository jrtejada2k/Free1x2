// Free1X2 · WinUI 3 — WIN3
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Free1X2.WinUI.Services;
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
    private const string UrlFaq = "https://clubprogol.com/DocWK/index.php?title=FAQ";

    // Mensajes exactos del legacy AyudaFrm (:23,:48). Allí el literal llevaba la barra
    // invertida escapada, así que en pantalla se leía "\n" tal cual; aquí es un salto real.
    private const string MsgAyudaOffline =
        "Online help disabled for offline operation." + "\n" + "Please refer to local documentation.";
    private const string MsgNotificaciones = "Notifications system disabled for performance.";

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
        // Legacy AyudaFrm.linkManual_LinkClicked (:21-24): MessageBox informativo.
        AppServices.MostrarInfo(MsgAyudaOffline);
    }

    private void ArticulosLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.linkArticulos_LinkClicked (:26-29): MessageBox informativo.
        AppServices.MostrarInfo(MsgAyudaOffline);
    }

    private void RecursosLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.linkRecursos_LinkClicked (:41-44): MessageBox informativo.
        AppServices.MostrarInfo(MsgAyudaOffline);
    }

    private void ForoLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.linkForo_LinkClicked (:31-34): MessageBox informativo.
        AppServices.MostrarInfo(MsgAyudaOffline);
    }

    private void NotificacionesLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.lnkNotificaciones_LinkClicked (:46-49): MessageBox informativo.
        AppServices.MostrarInfo(MsgNotificaciones);
    }

    private void FacebookLink_Click(object sender, RoutedEventArgs e)
    {
        // Legacy AyudaFrm.lnkFacebook_LinkClicked (:51-54): MessageBox informativo.
        AppServices.MostrarInfo(MsgAyudaOffline);
    }

    private static async System.Threading.Tasks.Task AbrirUriSeguraAsync(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            await Launcher.LaunchUriAsync(uri);
        }
    }
}
