using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page portada del WinForms legacy "ColProbablesFrm" (Free1X2.UI.Filtros.ColProbablesFrm).
    /// Editor del filtro "Columnas Probables" con pestañas Columnas / Relaciones I-III / Control Fallos.
    /// Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe los cambios de vuelta
    /// al FiltroColProbables al Aceptar.
    /// </summary>
    public sealed partial class ColProbablesFrmPage : Page
    {
        public ColProbablesFrmViewModel ViewModel { get; } = new ColProbablesFrmViewModel();

        public ColProbablesFrmPage()
        {
            this.InitializeComponent();
            ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
            ViewModel.Navegar = (tipo, parametro) => Frame?.Navigate(tipo, parametro);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.CargarDesdeGrupo();
        }
    }
}
