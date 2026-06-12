using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    // Port of legacy WinForms "VisorAnalisisColumnasFrm" (Free1X2.UI.VisorAnalisisColumnasFrm).
    // The legacy form is a read-only tabbed viewer: a TabControl whose visible tabs depend on
    // which "VariablesGlobales.Analizar*" flags are active. Each visible tab hosts a custom
    // analysis UserControl (CtrlAnalisisVX2, CtrlAnalisisSSeguidos, CtrlDibujos, etc.).
    //
    // This ViewModel models the list of analysis sections and which ones are active, plus the
    // currently selected section. It has NO input fields in the legacy form, but it does expose
    // toggles that mirror the VariablesGlobales.Analizar* switches so the section list can be
    // recomputed (the legacy form rebuilt/removed tabs in MostrarDatos based on those flags).

    // One analysis section (mirrors a legacy TabPage + its custom control).
    public partial class SeccionAnalisisItem : ObservableObject
    {
        public SeccionAnalisisItem(string clave, string titulo)
        {
            _clave = clave;
            _titulo = titulo;
        }

        [ObservableProperty]
        private string _clave;

        [ObservableProperty]
        private string _titulo;

        // Texto descriptivo / estado (p. ej. "No hay datos para analizar").
        [ObservableProperty]
        private string _detalle = "Datos de análisis cargados desde el dominio.";

        // Si el análisis correspondiente está activo (VariablesGlobales.Analizar*).
        [ObservableProperty]
        private bool _activo = true;

        // Marcador textual del estado (string para TextBlock.Text; regla anti-crash: no bindear bool).
        public string EstadoMarcador => Activo ? "●" : string.Empty;

        partial void OnActivoChanged(bool value)
        {
            OnPropertyChanged(nameof(EstadoMarcador));
        }
    }

    public partial class VisorAnalisisColumnasFrmViewModel : ObservableObject
    {
        // Payload que el dominio entrega a través del hook AnalisisUi.MostrarVisor
        // (contenedor = Free1X2.Analisis.ContenedorAnalisisGlobal, grupo = Free1X2.MotorCalculo.Grupo).
        // Se guarda de forma estática hasta que el cableado completo del visor lo consuma.
        // TODO(visor): consumir estos objetos para reconstruir el árbol de análisis.
        public static object? UltimoContenedor { get; set; }
        public static object? UltimoGrupo { get; set; }

        public VisorAnalisisColumnasFrmViewModel()
        {
            // Legacy MostrarDatos(): el orden y los textos de las pestañas provienen del Designer
            // y de VariablesGlobales.Analizar*. Aquí se siembran todas las secciones posibles;
            // en producción 'Activo' debe derivarse de VariablesGlobales y del ContenedorAnalisisGlobal.
            // TODO: poblar desde VariablesGlobales.Analizar* y ContenedorAnalisisGlobal/AnalisisGrupos
            //       (clase legacy: Free1X2.Analisis.ContenedorAnalisisGlobal).
            Secciones = new ObservableCollection<SeccionAnalisisItem>
            {
                new SeccionAnalisisItem("VX2", "VX2 — Variantes, Equis, Doses"),
                new SeccionAnalisisItem("SignosSeguidos", "Signos Seguidos"),
                new SeccionAnalisisItem("Dibujos", "Dibujos"),
                new SeccionAnalisisItem("Interrupciones", "Interrupciones"),
                new SeccionAnalisisItem("Simetrias", "Simetrías"),
                new SeccionAnalisisItem("Diferencias", "Diferencias"),
                new SeccionAnalisisItem("Pesos", "Pesos Numéricos"),
                new SeccionAnalisisItem("Valoracion", "Valoración"),
                new SeccionAnalisisItem("Distancias", "Distancias"),
                new SeccionAnalisisItem("Contactos", "Contactos"),
                new SeccionAnalisisItem("Formatos", "Formatos"),
                new SeccionAnalisisItem("ControlGrupos", "Control Grupos"),
                new SeccionAnalisisItem("ColumnasProbables", "Columnas Probables"),
                new SeccionAnalisisItem("ControlConjuntos", "Control Conjuntos"),
            };

            _seccionSeleccionada = Secciones.Count > 0 ? Secciones[0] : null;
        }

        public ObservableCollection<SeccionAnalisisItem> Secciones { get; }

        [ObservableProperty]
        private SeccionAnalisisItem? _seccionSeleccionada;

        // Título del detalle = título de la sección seleccionada (string para TextBlock.Text).
        public string DetalleTitulo => SeccionSeleccionada?.Titulo ?? "Selecciona un análisis";

        public string DetalleTexto => SeccionSeleccionada?.Detalle
            ?? "No hay ningún análisis seleccionado.";

        partial void OnSeccionSeleccionadaChanged(SeccionAnalisisItem? value)
        {
            OnPropertyChanged(nameof(DetalleTitulo));
            OnPropertyChanged(nameof(DetalleTexto));
        }

        [RelayCommand]
        private void Refrescar()
        {
            // Legacy: MostrarDatos(grupoActual) reconstruía las pestañas según VariablesGlobales.Analizar*.
            // TODO: recargar 'Secciones'/'Activo' desde VariablesGlobales y ContenedorAnalisisGlobal
            //       (legacy VisorAnalisisColumnasFrm.MostrarDatos / AñadirVX2 / AñadirSSeguidos / ...).
        }

        [RelayCommand]
        private void AbrirSeccion()
        {
            // Legacy: cada AñadirX() creaba el control de análisis correspondiente
            // (CtrlAnalisisVX2, CtrlAnalisisSSeguidos, CtrlDibujos, CtrlAnalisisInterrupciones,
            //  CtrlAnalisisContactos, CtrlAnalisisPesos, CtrlAnalisisDistancias, CtrlAnalisisSimetrias,
            //  CtrlAnalisisDiferencias, CtrlAnalisisFormatosSignos, CtrlAnalisisValoraciones,
            //  CtrlAnalisisCPs, CtrlAnalisisControlGrupos, CtrlAnalisisControlConjuntos) y lo
            //  insertaba en la pestaña. La construcción de esos controles queda pendiente de portar.
            // TODO: instanciar/mostrar el control de análisis de SeccionSeleccionada.
        }
    }
}
