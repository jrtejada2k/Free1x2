using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

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
    /// Edita la lista de tolerancias (Tolerancia/Aciertos) y los fallos permitidos en
    /// los controles, gestionados por el <see cref="ControladorTol"/> del grupo actual.
    /// En el WinForms se abre con <c>analizador.GruposPartidos[grupoPantalla].ControladorTolerancias</c>
    /// (MainForm.BtnTolGrupoClick); aquí se toma el grupo actual de
    /// <c>AppState.Instancia.GrupoActual.ControladorTolerancias</c>.
    /// </summary>
    public partial class ControlTolFrmViewModel : ObservableObject
    {
        // Número mínimo de filas en blanco que mostraba el form legacy.
        private const int LineasMinimas = 30;

        // Controlador real de tolerancias del grupo actual (legacy: ctrlTolerancias).
        private ControladorTol _ctrlTolerancias = AppState.Instancia.GrupoActual.ControladorTolerancias;

        /// <summary>Acción de cierre/volver (la cablea la página con Frame.GoBack()).</summary>
        public Action? Volver { get; set; }

        [ObservableProperty]
        private string _fallosPermitidos = string.Empty;

        public ObservableCollection<ToleranciaItem> Tolerancias { get; } = new();

        public ControlTolFrmViewModel()
        {
            // Carga inicial con lo que haya en el motor en el momento de construir el VM.
            CargarDesdeMotor();
        }

        /// <summary>
        /// Vuelca al grid las tolerancias del grupo actual y los fallos permitidos.
        /// Equivale a ControlTolFrm.InicializaDatosDG() + asignación de txtFallosControles
        /// (ctor del form legacy). La página la llama en OnNavigatedTo.
        /// </summary>
        public void CargarDesdeMotor()
        {
            _ctrlTolerancias = AppState.Instancia.GrupoActual.ControladorTolerancias;

            Tolerancias.Clear();

            List<ToleranciaFiltros> tolerancias = _ctrlTolerancias.Tolerancias;
            int noLineas = tolerancias.Count;
            // legacy: asegurar un mínimo de 30 líneas en blanco
            if (noLineas < LineasMinimas)
            {
                noLineas = LineasMinimas;
            }

            for (int i = 0; i < noLineas; i++)
            {
                var item = new ToleranciaItem();
                if (i < tolerancias.Count)
                {
                    item.LetrasTol = tolerancias[i].LetrasTol;
                    item.Aciertos = tolerancias[i].Aciertos;
                }
                Tolerancias.Add(item);
            }

            FallosPermitidos = _ctrlTolerancias.FallosPermitidos;
        }

        /// <summary>
        /// Equivale a ControlTolFrm.BtnOKClick: construye la lista final de
        /// ToleranciaFiltros tomando solo las filas con Aciertos != "" y la asigna al
        /// ControladorTol del grupo, junto con los fallos permitidos. Luego cierra.
        /// </summary>
        [RelayCommand]
        private void Guardar()
        {
            var toleranciasFinal = new List<ToleranciaFiltros>();

            foreach (ToleranciaItem item in Tolerancias)
            {
                // legacy: solo se conservan filas con Aciertos no vacío.
                if (!string.IsNullOrEmpty(item.Aciertos))
                {
                    var tol = new ToleranciaFiltros
                    {
                        LetrasTol = item.LetrasTol,
                        Aciertos = item.Aciertos, // el setter inicializa el array interno
                    };
                    toleranciasFinal.Add(tol);
                }
            }

            _ctrlTolerancias.Tolerancias = toleranciasFinal;
            _ctrlTolerancias.FallosPermitidos = FallosPermitidos;

            AppState.Instancia.NotificarCambio();
            Volver?.Invoke();
        }

        /// <summary>Equivale a ControlTolFrm.BtnCancelClick — cerrar sin guardar.</summary>
        [RelayCommand]
        private void Cancelar()
        {
            Volver?.Invoke();
        }
    }
}
