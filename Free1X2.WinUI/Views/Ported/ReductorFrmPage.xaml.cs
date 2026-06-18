// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>ReductorFrm</c> (Free1X2/UI/ReductorFrm.cs).
/// Reduce un archivo de columnas según método y nivel de reducción. La lógica de
/// dominio (IReduccion y la selección/lectura de archivos) queda como TODO en el ViewModel.
/// </summary>
public sealed partial class ReductorFrmPage : Page
{
    public ReductorFrmViewModel ViewModel { get; } = new();

    public ReductorFrmPage()
    {
        this.InitializeComponent();
    }
}
