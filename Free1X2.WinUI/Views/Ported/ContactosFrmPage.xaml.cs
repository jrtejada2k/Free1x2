// Free1X2 · WinUI 3 — WIN3
using System.ComponentModel;
using Free1X2.WinUI.Controls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "ContactosFrm" (filtro de Contactos).
/// Permite definir, para cada pareja de signos (1X, 12, X2, 11, XX, 22, 1V, XV, 2V, VV),
/// los números de contactos admitidos, más una condición opcional de figuras.
/// Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe los cambios de vuelta
/// al <c>FiltroContactos</c> al Aceptar. La edición de figuras (navega a
/// FigurasFiltrosFrmPage) y la persistencia al filtro están implementadas en el ViewModel.
/// </summary>
public sealed partial class ContactosFrmPage : Page
{
    public ContactosFrmViewModel ViewModel { get; } = new();

    public ContactosFrmPage()
    {
        this.InitializeComponent();

        // Reevaluar el semáforo de figuras cuando cambie el estado en el ViewModel.
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        // La VM navega al editor de figuras / visor de estadísticas a través del Frame.
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver del editor de figuras (NavigationMode.Back) sólo refrescamos el semáforo:
        // recargar desde el grupo borraría las filas/figuras en edición. En la entrada normal
        // (New / navegación hacia delante) sí cargamos los valores del grupo.
        if (e.NavigationMode == NavigationMode.Back)
        {
            ViewModel.RefrescarFiguras();
        }
        else
        {
            ViewModel.CargarDesdeGrupo();
        }
    }

    /// <summary>Estado del semáforo que refleja si la condición tiene figuras asociadas.</summary>
    public EstadoSemaforo EstadoFiguras =>
        ViewModel.TieneFiguras ? EstadoSemaforo.Verde : EstadoSemaforo.Neutro;

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ContactosFrmViewModel.TieneFiguras))
        {
            Bindings.Update();
        }
    }
}
