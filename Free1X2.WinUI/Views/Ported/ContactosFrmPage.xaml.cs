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
/// al <c>FiltroContactos</c> al Aceptar. La edición de figuras y la persistencia quedan como TODO.
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
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.CargarDesdeGrupo();
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
