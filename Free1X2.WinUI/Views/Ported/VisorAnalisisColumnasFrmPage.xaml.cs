// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    // Code-behind for the ported "VisorAnalisisColumnasFrm" (legacy Free1X2.UI.VisorAnalisisColumnasFrm).
    // Read-only tabbed analysis viewer. The ViewModel reads the domain payload
    // (ContenedorAnalisisGlobal + Grupo) from the static handoff set by the
    // AnalisisUi.MostrarVisor hook (App.xaml.cs) and builds the analysis sections.
    public sealed partial class VisorAnalisisColumnasFrmPage : Page
    {
        public VisorAnalisisColumnasFrmViewModel ViewModel { get; } = new VisorAnalisisColumnasFrmViewModel();

        public VisorAnalisisColumnasFrmPage()
        {
            InitializeComponent();
            // El constructor legacy recibía (ContenedorAnalisisGlobal contenedor, Grupo grupo) y
            // llamaba MostrarDatos(0) para construir las pestañas. Aquí esos parámetros de dominio
            // llegan al ViewModel por el handoff estático que deja AnalisisUi.MostrarVisor
            // (App.xaml.cs); la VM los consume en su constructor y reconstruye las secciones.
        }

        /// <summary>
        /// Oculta el título de un bloque de matriz cuando está vacío (las secciones de un solo
        /// bloque no llevan título; CPs/Diferencias/Formatos sí). Usado por x:Bind en el DataTemplate.
        /// </summary>
        public static Visibility VisibilidadTexto(string texto) =>
            string.IsNullOrEmpty(texto) ? Visibility.Collapsed : Visibility.Visible;
    }
}
