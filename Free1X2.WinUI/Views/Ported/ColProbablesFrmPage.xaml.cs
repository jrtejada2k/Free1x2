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

            // Al volver del diálogo "Copiar Datos" (Frame.GoBack), aplicar el rango elegido sobre la
            // copia de trabajo en memoria — NO recargar desde el grupo, que descartaría la edición en
            // curso (réplica de la 2ª mitad de ColProbablesFrm.CopiaValoresCP). AplicarCopiaDatos sólo
            // actúa si esta pantalla había lanzado el diálogo; si no, recarga normal del grupo en edición.
            if (ViewModel.CopiaDatosPendiente)
            {
                ViewModel.AplicarCopiaDatos();
                return;
            }

            ViewModel.CargarDesdeGrupo();
        }
    }
}
