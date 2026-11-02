// Free1X2 · WinUI 3 — WIN3
using System.ComponentModel;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;
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
    public ContactosFrmViewModel ViewModel { get; private set; } = new();

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
        // Al volver del editor de figuras o de Estadísticas se recupera el ViewModel que había, con
        // las filas en edición y la lista de figuras que el editor acaba de rellenar (comparten la misma
        // referencia), y se refresca el semáforo. En el original el editor era un diálogo encima del
        // formulario (ContactosFrm.cs:771-772). En la entrada normal se cargan los valores del grupo.
        if (EdicionPendiente.Recuperar<ContactosFrmViewModel>(this, e) is { } pendiente)
        {
            pendiente.Volver = ViewModel.Volver;
            pendiente.Navegar = ViewModel.Navegar;
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            ViewModel = pendiente;
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            ViewModel.RefrescarFiguras();
            Bindings.Update();
        }
        else
        {
            ViewModel.CargarDesdeGrupo();
        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        // Esta instancia de la página ya no vuelve: suelta el ViewModel por si otra lo recupera.
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        EdicionPendiente.Guardar(this, ViewModel, e);
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
