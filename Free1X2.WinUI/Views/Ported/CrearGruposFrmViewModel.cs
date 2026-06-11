using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para CrearGruposFrmPage.
    /// Replica el propósito del WinForms legacy CrearGruposFrm:
    /// pedir cuántos grupos nuevos crear (NumericUpDown udNumGrupos) y
    /// habilitar "Crear" sólo cuando el valor es mayor que 0.
    /// </summary>
    public partial class CrearGruposFrmViewModel : ObservableObject
    {
        // Legacy: NumericUpDown udNumGrupos. NumberBox.Value es double.
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearCommand))]
        private double _numGrupos;

        /// <summary>
        /// Legacy: btnCrear.Enabled = (udNumGrupos.Value > 0).
        /// </summary>
        private bool PuedeCrear() => NumGrupos > 0;

        [RelayCommand(CanExecute = nameof(PuedeCrear))]
        private void Crear()
        {
            // TODO: lógica de dominio legacy (CrearGruposFrm.btnCrear_Click).
            // El form original hacía Hide() y el llamador leía udNumGrupos.Value
            // para crear ese número de grupos nuevos.
            // Implementar: crear (int)NumGrupos grupos nuevos a través del
            // servicio/repositorio de grupos correspondiente.
        }

        [RelayCommand]
        private void Cancelar()
        {
            // Legacy: btnCancelar_Click -> udNumGrupos.Value = 0; Close();
            NumGrupos = 0;
            // TODO: cerrar/navegar atrás según el host de navegación WinUI.
        }
    }
}
