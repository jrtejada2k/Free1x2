using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "ParejasFrm" (título "Pares").
/// Filtra columnas según el resultado de parejas de partidos: 14 filas de
/// "Niveles" (P1, P2 + nivel por combinación 11/1x/12/x1/xx/x2/21/2x/22),
/// 7 "Condiciones" (rango min-max de aciertos por nivel) y un "Análisis
/// Resultados" sobre una columna ganadora. La lógica de dominio (lectura/
/// escritura de *.par y *.txt, Valida/s1n/n1s, BitArray de columnas válidas)
/// queda como TODO en el ViewModel.
/// </summary>
public sealed partial class ParejasFrmPage : Page
{
    public ParejasFrmViewModel ViewModel { get; } = new();

    public ParejasFrmPage()
    {
        InitializeComponent();
    }
}
