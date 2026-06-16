using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>CopiarCPFrm</c> ("Copiar Columnas").
/// Captura los índices de columnas probables a copiar (txtCP) y los grupos
/// destino (cmbGrupo, ListBox multi-selección) y copia las columnas a esos
/// grupos. La lógica de cálculo/persistencia queda como TODO en el ViewModel
/// hasta portar el dominio (MotorCalculo / FiltroColProbables / CrearGruposFrm).
/// </summary>
public sealed partial class CopiarCPFrmPage : Page
{
    public CopiarCPFrmViewModel ViewModel { get; } = new();

    public CopiarCPFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Refresca los grupos destino y el origen de columnas con el estado actual del motor.
        ViewModel.CargarDesdeGrupo();
    }

    /// <summary>
    /// Mantiene <see cref="CopiarCPFrmViewModel.GruposSeleccionados"/> sincronizada
    /// con la selección múltiple del ListView. WinUI no permite enlazar (x:Bind)
    /// directamente la selección múltiple de un ListView a una colección del VM,
    /// así que se replica aquí (equivale a cmbGrupo.GetSelected(i) del WinForms).
    /// </summary>
    private void GruposListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.GruposSeleccionados.Clear();
        if (sender is ListView lv)
        {
            foreach (var item in lv.SelectedItems)
            {
                if (item is string grupo)
                {
                    ViewModel.GruposSeleccionados.Add(grupo);
                }
            }
        }
    }
}
