// Free1X2 · WinUI 3 — WIN3
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Rejilla editable de porcentajes/valoraciones por partido (1 / X / 2). Reemplaza a los
/// UserControls WinForms <c>valors</c> y <c>ControlPorcentajes</c>. El ViewModel anfitrión
/// posee la colección <see cref="Filas"/> y obtiene la matriz <c>double[N,3]</c> que consume
/// el motor con <see cref="PorcentajesHelper.AMatriz"/> (equivalente a RetVals/Valores).
/// </summary>
public sealed partial class PorcentajesControl : UserControl
{
    public PorcentajesControl()
    {
        this.InitializeComponent();
    }

    /// <summary>Colección de filas a editar (una por partido). La aporta el ViewModel anfitrión.</summary>
    public ObservableCollection<FilaPorcentaje> Filas
    {
        get => (ObservableCollection<FilaPorcentaje>)GetValue(FilasProperty);
        set => SetValue(FilasProperty, value);
    }

    public static readonly DependencyProperty FilasProperty = DependencyProperty.Register(
        nameof(Filas),
        typeof(ObservableCollection<FilaPorcentaje>),
        typeof(PorcentajesControl),
        new PropertyMetadata(null));
}
