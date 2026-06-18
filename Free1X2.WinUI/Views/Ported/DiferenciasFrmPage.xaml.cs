using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>DiferenciasFrm</c> (filtro "Diferencias").
/// El usuario define grupos de partidos (una línea por grupo, partidos separados por
/// "," o "-") y, por concepto (Variantes / Equis / Doses / Dibujos / Interrupciones /
/// Formatos), la cantidad o intervalo de valores DISTINTOS entre los grupos. Permite
/// guardar varios conjuntos (Diferencias) y navegar entre ellos, generar grupos por
/// atajos (Dúos..Octetos). Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe
/// los cambios de vuelta al <c>FiltroDiferencias</c> al Aceptar. La persistencia en disco
/// queda como TODO en el ViewModel.
/// </summary>
public sealed partial class DiferenciasFrmPage : Page
{
    public DiferenciasFrmViewModel ViewModel { get; } = new();

    public DiferenciasFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver del visor de estadísticas (NavigationMode.Back) no recargamos desde el grupo
        // para no perder la posición de conjunto en edición; en la entrada normal sí cargamos.
        if (e.NavigationMode != NavigationMode.Back)
        {
            ViewModel.CargarDesdeGrupo();
        }
    }
}
