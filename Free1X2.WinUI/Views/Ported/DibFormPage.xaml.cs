using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>Free1X2.UI.Estadisticas.DibForm</c> (ventana "dibujos").
/// Muestra la distribución resultante del análisis "Variantes, X, 2" (modo Dibujos de Anastatics)
/// como una tabla triangular de celdas (etiquetas legacy lt####, filas = nº de X, columnas =
/// nº de 2, con X+2 &lt;= 14), con totales por fila (lx##), por columna (ld##) y por nº de
/// variantes (lv##). Un selector "mostrar" alterna entre porcentajes (rbPercent) y número de
/// columnas (rbCols). La matriz int[15,15] rsl y numcol los entrega Anastatics por el handoff
/// estático DibFormViewModel.MatrizEntrada / NumColEntrada antes de navegar a esta Page.
/// </summary>
public sealed partial class DibFormPage : Page
{
    public DibFormViewModel ViewModel { get; } = new();

    public DibFormPage()
    {
        this.InitializeComponent();
    }
}
