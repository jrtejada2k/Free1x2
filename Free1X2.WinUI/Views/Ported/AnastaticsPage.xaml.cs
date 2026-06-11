using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>Anastatics</c> (ventana "Estadísticas 0.3.4").
/// Permite elegir un modo de análisis, seleccionar un fichero de columnas origen,
/// calcular estadísticas y mostrar los resultados. La lógica de dominio
/// (ArchivoColumnasTexto, Dibujos/DibRepes/StaInter/StaSigSeg y las ventanas de
/// resultados DibForm/DibRepFrm/StaInterFrm/StaSSForm) está marcada como TODO en el ViewModel.
/// </summary>
public sealed partial class AnastaticsPage : Page
{
    public AnastaticsViewModel ViewModel { get; } = new();

    public AnastaticsPage()
    {
        this.InitializeComponent();
    }
}
