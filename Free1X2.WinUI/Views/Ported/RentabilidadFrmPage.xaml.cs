// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "RentabilidadFrm".
/// Obtiene la Esperanza Matemática de premio (EM) de las columnas a partir de la probabilidad
/// real y de las frecuencias apostadas, recorriendo las 14 triples o un fichero de entrada, y
/// graba el resultado filtrado por límites de EM. Permite además valorar una columna concreta.
/// </summary>
public sealed partial class RentabilidadFrmPage : Page
{
    public RentabilidadFrmViewModel ViewModel { get; } = new();

    public RentabilidadFrmPage()
    {
        InitializeComponent();
    }
}
