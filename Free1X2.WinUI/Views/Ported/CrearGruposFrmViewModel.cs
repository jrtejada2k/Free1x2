// Free1X2 · WinUI 3 — WIN3
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para CrearGruposFrmPage.
    /// Port del WinForms legacy CrearGruposFrm: pide cuántos grupos nuevos crear
    /// (NumericUpDown udNumGrupos) y los crea en la combinación actual.
    ///
    /// En WinForms, CrearGruposFrm sólo recogía la cantidad y el llamador
    /// (CopiarCPFrm.btnCrearGrupos_Click, Free1X2/UI/Filtros/CopiarCPFrm.cs líneas 386-403)
    /// ejecutaba el bucle de creación: por cada grupo nuevo crea un <see cref="Grupo"/>
    /// con todos los partidos activos y lo añade vía GruposPartidos.AddGrupo. Aquí, al ser
    /// una página independiente, el comando Crear realiza ese mismo bucle sobre
    /// <c>AppState.Instancia.Analizador.GruposPartidos</c>.
    /// </summary>
    public partial class CrearGruposFrmViewModel : ObservableObject
    {
        // Legacy: NumericUpDown udNumGrupos. NumberBox.Value es double.
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CrearCommand))]
        private double _numGrupos;

        /// <summary>Acción de cierre/volver (la cablea la página con Frame.GoBack()).</summary>
        public Action? Volver { get; set; }

        /// <summary>Legacy: btnCrear.Enabled = (udNumGrupos.Value > 0).</summary>
        private bool PuedeCrear() => NumGrupos > 0;

        [RelayCommand(CanExecute = nameof(PuedeCrear))]
        private void Crear()
        {
            // Equivale a CopiarCPFrm.btnCrearGrupos_Click (líneas 392-402):
            //   por cada grupo nuevo, crea un Grupo con los 14 partidos activos y lo
            //   añade al motor; tras el bucle se refresca la UI (NotificarCambio).
            int gruposNuevos = (int)NumGrupos;
            GrupoPartidos grupos = AppState.Instancia.Analizador.GruposPartidos;

            for (int i = 0; i < gruposNuevos; i++)
            {
                var grupo = new Grupo();
                grupo.PonerPartidosActivos("1,2,3,4,5,6,7,8,9,10,11,12,13,14");
                grupos.AddGrupo(grupo);
            }

            AppState.Instancia.NotificarCambio();
            NumGrupos = 0;
            Volver?.Invoke();
        }

        [RelayCommand]
        private void Cancelar()
        {
            // Legacy: btnCancelar_Click -> udNumGrupos.Value = 0; Close();
            NumGrupos = 0;
            Volver?.Invoke();
        }
    }
}
