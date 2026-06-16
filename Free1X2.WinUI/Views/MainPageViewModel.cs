using CommunityToolkit.Mvvm.ComponentModel;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views;

/// <summary>
/// ViewModel de la pantalla principal (réplica de <c>Free1X2.UI.MainForm</c>).
/// En M2 expone el estado compartido; en M3 se amplía con la rejilla de condiciones,
/// la navegación de grupos, el filtro de columnas y la fila de acciones.
/// El boleto base lo gestiona el propio <c>BoletoBaseControl</c> (su ViewModel),
/// que también opera sobre <see cref="AppState"/>.
/// </summary>
public partial class MainPageViewModel : ObservableObject
{
    public AppState Estado => AppState.Instancia;
}
