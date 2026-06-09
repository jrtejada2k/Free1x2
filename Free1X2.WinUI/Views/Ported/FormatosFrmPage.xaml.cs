using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms "FormatosFrm" (categoría Filtros).
///
/// Permite definir la condición de Formatos de signos: una o varias relaciones
/// navegables (1/N), cada una con hasta 30 líneas (secuencia de signos 1/X/2/V/*
/// + rango Mín-Máx de apariciones) y los límites Líneas/Global.
///
/// La UI y el estado en memoria viven en <see cref="FormatosFrmViewModel"/>.
/// La lógica de dominio (cálculo, persistencia, estadísticas) está marcada con TODO
/// porque aún no existe en Free1X2.Domain; se referencian los tipos legacy a invocar.
/// </summary>
public sealed partial class FormatosFrmPage : Page
{
    public FormatosFrmViewModel ViewModel { get; } = new();

    public FormatosFrmPage()
    {
        this.InitializeComponent();
    }

    // ===== Utilidades de formatos =====

    private void OnSacaFormatos(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio/UI): abrir el equivalente de Free1X2.UI.Filtros.CalculoFormatosFrm
        // (ShowDialog en el legacy) para extraer formatos.
    }

    private void OnPares(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio/UI): abrir el equivalente de Free1X2.UI.Filtros.ParejasFrm.
    }

    private void OnTrios(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio/UI): abrir el equivalente de Free1X2.UI.Filtros.TriosFrm.
    }

    private void OnSumasPares(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio/UI): abrir el equivalente de Free1X2.MotorCalculo.Estadisticas.AnalizadorJPM.
    }

    // ===== Acciones de la condición (legacy MenuCondiciones) =====

    private void OnAceptar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio): volcar ViewModel -> Free1X2.MotorCalculo.FiltroFormatosSignos
        // (lista de FormatosSignos con sus FormatoSignos) y activar el filtro vía
        // Grupo.ActivaFiltro(filtroFormatos); luego cerrar/navegar de vuelta.
    }

    private void OnEstadisticas(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio): construir un FiltroFormatosSignos temporal y llamar a
        // Free1X2.MotorCalculo.Estadisticas.CalculadorEstadisticas.EstadisticasFiltro(...)
        // para mostrar el visor de estadísticas.
    }

    private void OnGuardar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio): persistir vía Free1X2.EntradaSalida.ArchivoCondiciones.GuardaArchivo(filtro)
        // (formato *.fmt / *.xml) usando un FileSavePicker.
    }

    private void OnAbrir(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio): cargar vía Free1X2.EntradaSalida.ArchivoCondiciones.AbrirArchivoCombinacion(...)
        // + LeeCondicion() usando un FileOpenPicker, y recargar el ViewModel.
    }

    private void OnCopiar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio): copiar la condición actual a un fichero temporal (tmp.fmt)
        // como hace el legacy con ArchivoCondiciones.
    }

    private void OnPegar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio): pegar desde el fichero temporal (tmp.fmt) recargando el ViewModel.
    }

    private void OnBorrar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (dominio): reiniciar el FiltroFormatosSignos y recargar el ViewModel.
    }

    private void OnCancelar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // TODO (UI): cerrar/navegar de vuelta sin guardar.
    }
}
