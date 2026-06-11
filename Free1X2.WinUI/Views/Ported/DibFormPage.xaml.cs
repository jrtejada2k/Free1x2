using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>Free1X2.UI.Estadisticas.DibForm</c> (ventana "dibujos").
/// Muestra la distribución resultante del análisis "Variantes, X, 2" (modo Dibujos de Anastatics)
/// como una tabla 15x15 de celdas (etiquetas legacy lt####), con totales por fila (lx##),
/// por columna (ld##) y por diagonal/variantes (lv##). Un selector "mostrar" alterna entre
/// porcentajes (rbPercent) y número de columnas (rbCols).
/// La lógica de dominio (recibir la matriz int[15,15] rsl y numcol desde Anastatics y poblar
/// las celdas con Porcentajes()/PintaColumnas()) está marcada como TODO en el ViewModel.
/// </summary>
public sealed partial class DibFormPage : Page
{
    public DibFormViewModel ViewModel { get; } = new();

    public DibFormPage()
    {
        this.InitializeComponent();
    }
}
