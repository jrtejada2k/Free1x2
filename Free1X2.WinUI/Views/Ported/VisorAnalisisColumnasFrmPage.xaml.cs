using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    // Code-behind for the ported "VisorAnalisisColumnasFrm" (legacy Free1X2.UI.VisorAnalisisColumnasFrm).
    // Read-only tabbed analysis viewer. ViewModel holds the analysis sections; domain wiring is TODO.
    public sealed partial class VisorAnalisisColumnasFrmPage : Page
    {
        public VisorAnalisisColumnasFrmViewModel ViewModel { get; } = new VisorAnalisisColumnasFrmViewModel();

        public VisorAnalisisColumnasFrmPage()
        {
            InitializeComponent();
            // TODO: el constructor legacy recibía (ContenedorAnalisisGlobal contenedor, Grupo grupo)
            //       y llamaba MostrarDatos(0) para construir las pestañas activas. Falta cablear
            //       esos parámetros de dominio al ViewModel.
        }

        /// <summary>
        /// Oculta el título de un bloque de matriz cuando está vacío (las secciones de un solo
        /// bloque no llevan título; CPs/Diferencias/Formatos sí). Usado por x:Bind en el DataTemplate.
        /// </summary>
        public static Visibility VisibilidadTexto(string texto) =>
            string.IsNullOrEmpty(texto) ? Visibility.Collapsed : Visibility.Visible;
    }
}
