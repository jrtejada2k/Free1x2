using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy
/// "Free1X2.EntradaSalida.GenerarCPs.ConfigCPsFrm" (Configurar Columnas Probables).
/// Permite editar los tipos de columnas probables y las reglas de forzado
/// (fijos/dobles/triples) por tramo de puntuación, y guardarlas a disco.
/// </summary>
public sealed partial class ConfigCPsFrmPage : Page
{
    public ConfigCPsFrmViewModel ViewModel { get; } = new();

    public ConfigCPsFrmPage()
    {
        InitializeComponent();
    }

    private void OnGuardarClick(object sender, RoutedEventArgs e)
    {
        ViewModel.GuardarCommand.Execute(null);

        // TODO[dominio]: tras guardar, ocultar el botón / notificar éxito.
        //   Legacy ConfigCPsFrm.button3_Click ocultaba el botón (button3.Visible = false)
        //   tras DatosHelper.GuardarDatos(dsConfCol).
    }

    private void OnVolverClick(object sender, RoutedEventArgs e)
    {
        // TODO[dominio]: cerrar/regresar (legacy ConfigCPsFrm.button1_Click -> this.Close()).
        //   En navegación WinUI, invocar Frame.GoBack() o cerrar el host contenedor.
    }
}
