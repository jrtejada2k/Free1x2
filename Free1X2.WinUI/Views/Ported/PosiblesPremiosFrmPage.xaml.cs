using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "PosiblesPremiosFrm" (título "Posibles Premios").
///
/// Propósito: el usuario introduce la columna ganadora (un signo 1/X/2 — o "*" para no
/// definitivo — por cada uno de los 16 partidos), abre un fichero de boletos y navega
/// entre ellos, y calcula a cuántos premios (16/15/14/13/12/11/10 aciertos) opta cada
/// columna jugada, con opción de tratar el último partido como pleno y de generar un
/// resumen exportable/copiable.
/// </summary>
public sealed partial class PosiblesPremiosFrmPage : Page
{
    public PosiblesPremiosFrmViewModel ViewModel { get; } = new();

    /// <summary>
    /// Pasarela a las opciones de signo del VM, para enlazarlas desde el ItemTemplate de la
    /// rejilla de partidos vía Binding ElementName=PageRoot (regla anti-crash 3: ItemsSource
    /// del ComboBox desde una propiedad, no items inline).
    /// </summary>
    public IReadOnlyList<string> SignosPosibles => ViewModel.SignosPosibles;

    public PosiblesPremiosFrmPage()
    {
        InitializeComponent();
    }
}
