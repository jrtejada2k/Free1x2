// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Página WinUI portada del WinForms legacy Free1X2.UI.ModificadorFrm
    /// ("Modificador de columnas"). Carga un fichero de columnas, muestra el
    /// porcentaje 1/X/2 de cada partido y redistribuye los signos según el modo
    /// (aleatorio / proporcional / ordenado) antes de grabar el resultado.
    /// </summary>
    public sealed partial class ModificadorFrmPage : Page
    {
        public ModificadorFrmViewModel ViewModel { get; } = new ModificadorFrmViewModel();

        public ModificadorFrmPage()
        {
            this.InitializeComponent();
        }
    }
}
