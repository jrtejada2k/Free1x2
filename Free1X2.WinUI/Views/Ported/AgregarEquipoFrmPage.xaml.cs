// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>AgregarEquipoFrm</c> ("Añadir Equipo").
/// Captura el nombre de un equipo y su categoría (1ª, 2ª, 2ªB o Int) y lo
/// añade a la lista correspondiente. La lógica de persistencia queda como TODO
/// en el ViewModel hasta portar el dominio.
/// </summary>
public sealed partial class AgregarEquipoFrmPage : Page
{
    public AgregarEquipoFrmViewModel ViewModel { get; } = new();

    public AgregarEquipoFrmPage()
    {
        this.InitializeComponent();
    }
}
