// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    // Page portada del WinForms "DibRepFrm" (this.Text = "coincidencias").
    // Muestra una tabla de 15 posiciones (0..14) x filas de categoria con los conteos
    // de coincidencias, alternando entre porcentajes y conteo de columnas.
    public sealed partial class DibRepFrmPage : Page
    {
        public DibRepFrmViewModel ViewModel { get; } = new DibRepFrmViewModel();

        public DibRepFrmPage()
        {
            InitializeComponent();
        }
    }
}
