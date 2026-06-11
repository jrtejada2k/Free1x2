using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>ControlGruposFrm</c> ("Control de Grupos").
/// Permite definir, para un <c>ControladorGrupos</c>, una lista de controles de
/// fallos sobre grupos de partidos y otra sobre conjuntos de grupos. Cada sección
/// se recorre con Anterior/Siguiente, muestra un contador "actual/total" y permite
/// eliminar el control actual. La lógica de copia, guardado y cálculo de grupos
/// libres (MotorCalculo) queda como TODO en el ViewModel hasta portar el dominio.
/// </summary>
public sealed partial class ControlGruposFrmPage : Page
{
    public ControlGruposFrmViewModel ViewModel { get; } = new();

    public ControlGruposFrmPage()
    {
        this.InitializeComponent();
    }
}
