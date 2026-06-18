using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page portada del WinForms legacy "aidomnou" (Free1X2.UI.aidomnou).
    /// Filtro que genera 6 columnas probables a partir de las valoraciones de los 14 partidos,
    /// define límites de aciertos por columna (más suma y recorrido) y filtra un fichero de
    /// columnas que cumplan esas condiciones, además de analizar columnas ganadoras.
    /// </summary>
    public sealed partial class aidomnouPage : Page
    {
        public aidomnouViewModel ViewModel { get; } = new aidomnouViewModel();

        public aidomnouPage()
        {
            this.InitializeComponent();
        }
    }
}
