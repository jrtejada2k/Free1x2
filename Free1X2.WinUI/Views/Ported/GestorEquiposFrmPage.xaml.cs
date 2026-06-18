using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>GestorEquiposFrm</c> ("Gestión de Equipos").
/// Muestra los equipos de las cuatro categorías (1ª, 2ª, 2ªB e Internacionales)
/// y permite moverlos entre ellas, eliminarlos, dar de alta nuevos y guardar.
/// La carga/guardado de los ficheros .dat y la apertura de AgregarEquipoFrm
/// quedan como TODO de dominio en el ViewModel.
/// </summary>
public sealed partial class GestorEquiposFrmPage : Page
{
    public GestorEquiposFrmViewModel ViewModel { get; } = new();

    public GestorEquiposFrmPage()
    {
        this.InitializeComponent();
        // Legacy btnNuevoEquipo_Click: new AgregarEquipoFrm(...).ShowDialog(). El VM navega a
        // AgregarEquipoFrmPage (form de alta de equipo).
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }
}
