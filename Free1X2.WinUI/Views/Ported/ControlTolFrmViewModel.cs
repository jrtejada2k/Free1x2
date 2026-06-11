using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Una fila de la rejilla de tolerancias.
    /// Legacy: ToleranciaFiltros (Free1X2.MotorCalculo) — columnas LetrasTol y Aciertos.
    /// </summary>
    public partial class ToleranciaItem : ObservableObject
    {
        [ObservableProperty]
        private string _letrasTol = string.Empty;

        [ObservableProperty]
        private string _aciertos = string.Empty;
    }

    /// <summary>
    /// Port de ControlTolFrm (Free1X2.UI.Filtros.ControlTolFrm).
    /// Edita la lista de tolerancias (Tolerancia/Aciertos) y los fallos permitidos
    /// en los controles, gestionados por el legacy ControladorTol.
    /// </summary>
    public partial class ControlTolFrmViewModel : ObservableObject
    {
        // Número mínimo de filas en blanco que mostraba el form legacy.
        private const int LineasMinimas = 30;

        [ObservableProperty]
        private string _fallosPermitidos = string.Empty;

        public ObservableCollection<ToleranciaItem> Tolerancias { get; } = new();

        public ControlTolFrmViewModel()
        {
            // TODO dominio: cargar desde ControladorTol.Tolerancias y
            // ControladorTol.FallosPermitidos (Free1X2.MotorCalculo.ControladorTol).
            // El legacy garantiza un mínimo de 30 líneas en blanco.
            for (int i = 0; i < LineasMinimas; i++)
            {
                Tolerancias.Add(new ToleranciaItem());
            }
        }

        [RelayCommand]
        private void Guardar()
        {
            // TODO dominio: replicar ControlTolFrm.BtnOKClick:
            //  - construir la lista final de ToleranciaFiltros tomando solo las filas
            //    con Aciertos != "" (descartando las líneas en blanco).
            //  - ctrlTolerancias.Tolerancias = toleranciasFinal;
            //  - ctrlTolerancias.FallosPermitidos = FallosPermitidos;
            //  - cerrar la página/diálogo.
        }

        [RelayCommand]
        private void Cancelar()
        {
            // TODO dominio: replicar ControlTolFrm.BtnCancelClick — cerrar sin guardar.
        }
    }
}
