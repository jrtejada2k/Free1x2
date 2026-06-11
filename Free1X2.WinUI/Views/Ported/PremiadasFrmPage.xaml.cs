using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "PremiadasFrm" (título "Análisis Premiadas").
///
/// A partir de un fichero de columnas ganadoras, calcula cuántas columnas del espacio
/// total resultarían premiadas con 10/11/12/13 aciertos y muestra las frecuencias.
/// Permite grabar a fichero las columnas de una frecuencia seleccionada (legacy: bGrabar)
/// y analizar en qué jornadas aparece dicha frecuencia (legacy: bAnaliza).
///
/// La lógica de cálculo/persistencia se deja como TODO en el ViewModel citando PremiadasFrm.
/// </summary>
public sealed partial class PremiadasFrmPage : Page
{
    public PremiadasFrmViewModel ViewModel { get; } = new();

    public PremiadasFrmPage()
    {
        InitializeComponent();
    }
}
