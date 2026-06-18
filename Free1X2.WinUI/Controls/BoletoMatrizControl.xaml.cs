// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;

using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Control visual reutilizable del boleto de la Quiniela. Pinta la matriz de 8 columnas
/// del boleto actual (cada columna = nº de columna en la combinación + 16 apuestas de
/// signos 1/X/2 marcados + aciertos), replicando el WinForms
/// <c>Free1X2.UI.Controls.ControlBoleto</c> / <c>ControlColumnaBoleto</c> / <c>ControlApuestaBoleto</c>.
///
/// Es de sólo lectura (visor): la edición interactiva de un boleto la cubre el otro
/// control <c>BoletoControl</c>. Expone <see cref="ViewModel"/> para enlace x:Bind y
/// métodos de conveniencia <see cref="Llenar"/> / <see cref="Limpiar"/> para los
/// consumidores (BoletoFrmViewModel.LlenarBoleto, VerBoletos).
/// </summary>
public sealed partial class BoletoMatrizControl : UserControl
{
    public BoletoMatrizViewModel ViewModel { get; } = new();

    public BoletoMatrizControl()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Vuelca el boleto actual: hasta 8 cadenas de signos, los números de columna en la
    /// combinación y, opcionalmente, los aciertos por columna. Equivale a
    /// <c>ControlBoleto.LlenarBoleto</c>.
    /// </summary>
    public void Llenar(
        IReadOnlyList<string> signos,
        IReadOnlyList<int>? numerosColumna = null,
        IReadOnlyList<int>? aciertos = null)
        => ViewModel.Llenar(signos, numerosColumna, aciertos);

    /// <summary>Limpia el boleto completo (legacy <c>ControlBoleto.LimpiarBoleto</c>).</summary>
    public void Limpiar() => ViewModel.Limpiar();
}
