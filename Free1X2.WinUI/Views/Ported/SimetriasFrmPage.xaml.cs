using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>SimetriasFrm</c> (filtro "Simetrías").
/// Permite definir simetrías —grupos de partidos que deben compartir el mismo signo—
/// y un campo de "Aciertos". Replica los CtrlSimetria dinámicos + txtAciertos y los
/// botones de menuCondiciones (Aceptar, Estadísticas, Guardar, Abrir, Copiar, Pegar,
/// Borrar, Cancelar). La lógica de dominio (FiltroSimetrias, Simetria,
/// ArchivoCondiciones, CalculadorEstadisticas) está marcada como TODO en el ViewModel.
/// </summary>
public sealed partial class SimetriasFrmPage : Page
{
    public SimetriasFrmViewModel ViewModel { get; } = new();

    public SimetriasFrmPage()
    {
        this.InitializeComponent();
    }
}
