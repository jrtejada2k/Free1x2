using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms "FormatosFrm" (categoría Filtros).
///
/// Permite definir la condición de Formatos de signos: una o varias relaciones
/// navegables (1/N), cada una con hasta 30 líneas (secuencia de signos 1/X/2/V/*
/// + rango Mín-Máx de apariciones) y los límites Líneas/Global.
///
/// Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe los cambios de vuelta
/// al <c>FiltroFormatosSignos</c> al Aceptar. Las utilidades de formatos y la persistencia
/// en disco quedan como TODO.
/// </summary>
public sealed partial class FormatosFrmPage : Page
{
    public FormatosFrmViewModel ViewModel { get; } = new();

    public FormatosFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver del visor de estadísticas (NavigationMode.Back) no recargamos desde el grupo
        // para no perder los formatos en edición; en la entrada normal cargamos.
        if (e.NavigationMode != NavigationMode.Back)
        {
            ViewModel.CargarDesdeGrupo();
        }
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
        // Vuelca el ViewModel al FiltroFormatosSignos, lo activa y vuelve atrás.
        ViewModel.AceptarCommand.Execute(null);
    }

    private void OnEstadisticas(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // FiltroFormatosSignos temporal -> CalculadorEstadisticas -> VisorEstadisticasPage.
        ViewModel.EstadisticasCommand.Execute(null);
    }

    private void OnGuardar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // ArchivoCondiciones.GuardaArchivo(filtro) (*.fmt / *.xml) vía FileSavePicker.
        ViewModel.GuardarCommand.Execute(null);
    }

    private void OnAbrir(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // ArchivoCondiciones.AbrirArchivoCombinacion(...) + LeeCondicion() vía FileOpenPicker.
        ViewModel.AbrirCommand.Execute(null);
    }

    private void OnCopiar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Copia la condición al fichero temporal interno (Temp/tmp.fmt).
        ViewModel.CopiarCommand.Execute(null);
    }

    private void OnPegar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Pega la condición desde el fichero temporal interno (Temp/tmp.fmt).
        ViewModel.PegarCommand.Execute(null);
    }

    private void OnBorrar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Reinicia el FiltroFormatosSignos del grupo y recarga la pantalla.
        ViewModel.BorrarCommand.Execute(null);
    }

    private void OnCancelar(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Cierra/navega de vuelta sin guardar (CerrarVentana legacy).
        ViewModel.CancelarCommand.Execute(null);
    }
}
