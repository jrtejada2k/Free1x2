using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>GruposEquiposFrm</c> ("Grupos de Equipos").
/// Define agrupaciones de equipos para el filtro <c>FiltroGruposEquipos</c>:
/// en la pestaña "Grupos Equipos" se marcan los equipos (casa/fuera) de los 14
/// partidos y se fijan victorias / empates / derrotas / suma de puntos; en
/// "Relaciones" se combinan grupos por índice con sus sumas. La lógica de
/// cálculo y persistencia se marca como TODO en el ViewModel.
/// </summary>
public sealed partial class GruposEquiposFrmPage : Page
{
    public GruposEquiposFrmViewModel ViewModel { get; } = new();

    public GruposEquiposFrmPage()
    {
        this.InitializeComponent();
    }
}
