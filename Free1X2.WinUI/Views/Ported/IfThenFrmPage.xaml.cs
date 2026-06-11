using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "IfThenFrm" — "Condiciones relacionadas (if-then)".
///
/// Permite definir relaciones del tipo "si se produce una condición, entonces debe
/// cumplirse otra", en dos pestañas:
///  - Condiciones sencillas: relaciona condiciones de filtro (genérica + específica + valor + negación).
///  - Grupos: relaciona grupos de partidos entre sí.
///
/// La lógica de dominio (ControladorIfThen / CondicionIfThen / GrupoIfThen, persistencia
/// con ArchivoCondiciones, y la integración con el Analizador) está marcada como TODO
/// en el ViewModel; aún no existe en el dominio portado.
/// </summary>
public sealed partial class IfThenFrmPage : Page
{
    public IfThenFrmViewModel ViewModel { get; } = new();

    public IfThenFrmPage()
    {
        this.InitializeComponent();
    }
}
