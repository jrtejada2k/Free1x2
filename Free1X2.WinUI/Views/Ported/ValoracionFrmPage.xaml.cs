using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>ValoracionFrm</c> (filtro "Valoración" /
/// <c>FiltroValoracionSignos</c>). Permite asignar a cada partido unas
/// valoraciones 1/X/2 (control de porcentajes) y filtrar columnas cuya
/// valoración Global, de Unos, Equis y Doses caiga dentro de los rangos
/// indicados, en modo "suma" o "multiplo" (productos x 3E7). Incluye el cálculo
/// de la valoración de una columna concreta y utilidades (buscar límite,
/// columnas base) y la barra de condiciones (Aceptar, Estadísticas, Guardar,
/// Abrir, Copiar, Pegar, Borrar, Cancelar). La lógica de dominio
/// (FiltroValoracionSignos, ControlPorcentajes, persistencia) está marcada como
/// TODO en el ViewModel.
/// </summary>
public sealed partial class ValoracionFrmPage : Page
{
    public ValoracionFrmViewModel ViewModel { get; } = new();

    public ValoracionFrmPage()
    {
        this.InitializeComponent();
    }
}
