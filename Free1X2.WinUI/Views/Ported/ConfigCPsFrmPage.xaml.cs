// Free1X2 · WinUI 3 — WIN3
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
        // Guarda los datos a disco. Fiel al legacy ConfigurarCPs.button3_Click, que tras
        // DatosHelper.GuardarDatos(dsConfCol) ocultaba el botón Guardar (button3.Visible = false)
        // y NO cerraba el form; el cierre era responsabilidad del botón Volver (OnVolverClick).
        ViewModel.GuardarCommand.Execute(null);
    }

    private void OnVolverClick(object sender, RoutedEventArgs e)
    {
        // Cierra/regresa (legacy ConfigurarCPs.button1_Click -> this.Close()). En navegación
        // WinUI equivale a Frame.GoBack().
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
