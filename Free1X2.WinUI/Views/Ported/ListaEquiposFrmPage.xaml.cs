using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page WinUI 3 portada del WinForms 'Free1X2.UI.ListaEquiposFrm'.
    /// Muestra la lista de equipos de una categoría y permite seleccionar uno.
    /// </summary>
    public sealed partial class ListaEquiposFrmPage : Page
    {
        public ListaEquiposFrmViewModel ViewModel { get; } = new();

        public ListaEquiposFrmPage()
        {
            InitializeComponent();

            // TODO [dominio]: el form legacy recibía la 'categoria' por constructor
            // (ListaEquiposFrm(TextBox miTxt, string categoria)) y cargaba el .dat en OnLoad.
            // Al integrar la navegación, pasar la categoría vía parámetro de Navigate y
            // luego invocar ViewModel.CargarCommand. No se implementa aquí.
        }
    }
}
