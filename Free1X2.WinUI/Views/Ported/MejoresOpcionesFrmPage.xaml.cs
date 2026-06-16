using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "MejoresOpcionesFrm" ("Mis Mejores Opciones").
///
/// A partir de una columna ganadora conocida (posiblemente con comodines), el usuario
/// marca que partidos pueden variar ("Partidos Involucrados", legacy ckb1..ckb16) e indica
/// cuantos resultados mostrar (legacy txtLimite). Al pulsar "Calcular" (legacy button1_Click)
/// se generan las columnas ganadoras posibles, se escrutan las columnas jugadas y se muestra
/// el resumen ordenado por premios (legacy txtResumen).
/// </summary>
public sealed partial class MejoresOpcionesFrmPage : Page
{
    public MejoresOpcionesFrmViewModel ViewModel { get; } = new();

    public MejoresOpcionesFrmPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Recibe el contexto del flujo legacy (PosiblesPremiosFrm.btnMejoresOpciones_Click):
    /// columna ganadora, columnas jugadas y si se contempla el pleno al 15. El form legacy lo
    /// inyectaba por propiedades antes de ShowDialog y, en OnLoad, llamaba a
    /// AdaptarInterfaz(ColumnaGanadora.Length). Aquí llega como parámetro de navegación
    /// (mismo patrón que CopiarDatosCPFrmPage.OnNavigatedTo con e.Parameter).
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is MejoresOpcionesContexto ctx)
        {
            // EstablecerContexto ya recalcula AdaptarInterfaz con la longitud real de la columna.
            ViewModel.EstablecerContexto(ctx.ColumnaGanadora, ctx.ArchivoColumnas, ctx.ContemplaPleno);
        }
    }
}
