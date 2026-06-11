using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para la Page portada de "ColProbablesFrm"
    /// (WinForms legacy: Free1X2.UI.Filtros.ColProbablesFrm).
    ///
    /// Propósito legacy: editor del filtro "Columnas Probables" (FiltroColProbables).
    /// Una Columna Probable es un segundo pronóstico sobre un grupo de partidos. El form
    /// gestiona, mediante pestañas:
    ///   - Columnas: pronóstico 1/X/2 por partido + rangos de Aciertos / Ac. Seguidos /
    ///     Fallos Seguidos, puntos y tolerancias locales.
    ///   - Relaciones I: condiciones entre columnas (suma aciertos, recorrido, cuántas CP, nº aciertos).
    ///   - Relaciones II: sumas o individuales de AC/ACS/FS entre rangos de columnas (Más/Menos).
    ///   - Relaciones III: escaleras, sándwichs y agrupaciones de aciertos.
    ///   - Control Fallos: tabla (Columnas / Tolerancias / Aciertos) + puntuación.
    ///
    /// Toda la lógica de dominio (FiltroColProbables, ColumnaProbable, RelacionCP1/2/3,
    /// CPControlFallos, import/export, persistencia) queda como TODO; aquí solo se replican
    /// los campos de entrada y las acciones de la UI.
    /// </summary>
    public partial class ColProbablesFrmViewModel : ObservableObject
    {
        // Opciones para los ComboBox (legacy: cbbConcepto/cbbConcepto2/cbbConceptoRel3 = AC/ACS/FS,
        // cbbMasMenos/cbbMasMenos2 = Más/Menos). Reglas anti-crash: ItemsSource desde IReadOnlyList<string>.
        public IReadOnlyList<string> OpcionesConcepto { get; } = new[] { "AC", "ACS", "FS" };
        public IReadOnlyList<string> OpcionesMasMenos { get; } = new[] { "Más", "Menos" };

        // ---------------- Pestaña Columnas ----------------

        // Pronóstico de la columna en pantalla (14 signos 1/X/2). Legacy: controles Prono1X2 por partido.
        [ObservableProperty]
        private string _pronostico = "";

        // Rangos de la columna actual. Legacy: optAC / optACS / optFS (OptionNumTol0_14, lista de valores 0..14).
        [ObservableProperty]
        private string _aciertos = "";

        [ObservableProperty]
        private string _aciertosSeguidos = "";

        [ObservableProperty]
        private string _fallosSeguidos = "";

        // Puntos de la columna. Legacy: txtPuntos.
        [ObservableProperty]
        private string _puntos = "";

        // Tolerancias locales: valores de AC/ACS/FS y nº de tolerancias seleccionadas (0..3).
        // Legacy: optACTol / optACSTol / optFSTol (OptionNumsHoriz) y los labels tol0..tol3 conmutables.
        [ObservableProperty]
        private string _aciertosTol = "";

        [ObservableProperty]
        private string _aciertosSeguidosTol = "";

        [ObservableProperty]
        private string _fallosSeguidosTol = "";

        [ObservableProperty]
        private bool _tolerancia0;

        [ObservableProperty]
        private bool _tolerancia1;

        [ObservableProperty]
        private bool _tolerancia2;

        [ObservableProperty]
        private bool _tolerancia3;

        // Paginación de columnas. Legacy: lblNoCP "n/total". String para no bindear int a Text.
        [ObservableProperty]
        private string _paginacionColumnas = "1/1";

        // ---------------- Pestaña Relaciones I ----------------

        // Legacy: txtColumnas / txtSumaAC / txtRecorridoAC / txtCuantasCP / txtNoAciertos.
        [ObservableProperty]
        private string _rel1Columnas = "";

        [ObservableProperty]
        private string _rel1SumaAciertos = "";

        [ObservableProperty]
        private string _rel1Recorrido = "";

        [ObservableProperty]
        private string _rel1CuantasCP = "";

        [ObservableProperty]
        private string _rel1NumeroAciertos = "";

        // Legacy: lblNoRel1 "n/total".
        [ObservableProperty]
        private string _paginacionRel1 = "1/1";

        // ---------------- Pestaña Relaciones II ----------------

        // Bloque "Sumas de AC/ACS/FS". Legacy: txtColsA / cbbConcepto / txtNumAc / cbbMasMenos / txtColsB.
        [ObservableProperty]
        private string _rel2SumaColsA = "";

        [ObservableProperty]
        private string _rel2SumaConcepto = "AC";

        [ObservableProperty]
        private string _rel2SumaNumAciertos = "";

        [ObservableProperty]
        private string _rel2SumaMasMenos = "Más";

        [ObservableProperty]
        private string _rel2SumaColsB = "";

        // Bloque "AC/ACS/FS individuales". Legacy: txtColsA2 / cbbConcepto2 / txtNumAc2 / cbbMasMenos2 / txtColsB2.
        [ObservableProperty]
        private string _rel2IndColsA = "";

        [ObservableProperty]
        private string _rel2IndConcepto = "AC";

        [ObservableProperty]
        private string _rel2IndNumAciertos = "";

        [ObservableProperty]
        private string _rel2IndMasMenos = "Más";

        [ObservableProperty]
        private string _rel2IndColsB = "";

        // Legacy: lblRel2Paginacion "1 / 1".
        [ObservableProperty]
        private string _paginacionRel2 = "1 / 1";

        // ---------------- Pestaña Relaciones III ----------------

        // Legacy: txtColsImplicadas / cbbConceptoRel3.
        [ObservableProperty]
        private string _rel3Columnas = "";

        [ObservableProperty]
        private string _rel3Concepto = "AC";

        // Escaleras. Legacy: txtNoEscaleras / txtNoEscalerasAsc / txtNoEscalerasDesc.
        [ObservableProperty]
        private string _rel3EscalerasTotal = "";

        [ObservableProperty]
        private string _rel3EscalerasAsc = "";

        [ObservableProperty]
        private string _rel3EscalerasDesc = "";

        // Sándwichs. Legacy: txtNoSandwichs.
        [ObservableProperty]
        private string _rel3Sandwichs = "";

        // Legacy: lblRel3Paginacion "1 / 1".
        [ObservableProperty]
        private string _paginacionRel3 = "1 / 1";

        // ---------------- Pestaña Control Fallos ----------------

        // Legacy: txtFallosCtrl ("Número de Fallos de Controles") y txtPuntos del bloque puntuación.
        [ObservableProperty]
        private string _numeroFallosControles = "";

        [ObservableProperty]
        private string _puntosControl = "";

        // ================= Comandos (lógica de dominio = TODO) =================

        // Pestaña Columnas: navegación. Legacy: btnInicioCP / btnPrev3CP / btnPrevCP / btnNextCP / btnNext3CP / btnFinCP.
        [RelayCommand]
        private void IrPrimeraColumna()
        {
            // TODO: portar Free1X2.UI.Filtros.ColProbablesFrm.CambiaCPSelecionado(0) (btnInicioCP).
        }

        [RelayCommand]
        private void ColumnasAtras3()
        {
            // TODO: portar ColProbablesFrm btnPrev3CP (desplazamiento de VariablesGlobales.Desplazamiento hacia atrás).
        }

        [RelayCommand]
        private void ColumnaAnterior()
        {
            // TODO: portar ColProbablesFrm btnPrevCP -> CambiaCPSelecionado(cpPantalla - 1).
        }

        [RelayCommand]
        private void ColumnaSiguiente()
        {
            // TODO: portar ColProbablesFrm btnNextCP -> CambiaCPSelecionado(cpPantalla + 1).
        }

        [RelayCommand]
        private void ColumnasAdelante3()
        {
            // TODO: portar ColProbablesFrm btnNext3CP.
        }

        [RelayCommand]
        private void IrUltimaColumna()
        {
            // TODO: portar ColProbablesFrm btnFinCP.
        }

        // Pestaña Columnas: acciones. Legacy: btnCopiarValores / btnCopiarCols / btnEliminarActual /
        // btnATriple / btnBorraPronosticos / btnPuntuacion / btnImportar / btnExportador.
        [RelayCommand]
        private void CopiarDatos()
        {
            // TODO: portar ColProbablesFrm.CopiaValoresCP (abre CopiarDatosCPFrm).
        }

        [RelayCommand]
        private void CopiarColumnas()
        {
            // TODO: portar ColProbablesFrm btnCopiarCols.
        }

        [RelayCommand]
        private void EliminarColumnaActual()
        {
            // TODO: portar ColProbablesFrm btnEliminarActual (BorrarCP de la CP en pantalla).
        }

        [RelayCommand]
        private void TodosATriple()
        {
            // TODO: portar ColProbablesFrm.PonerTodosATriple.
        }

        [RelayCommand]
        private void BorrarPronosticos()
        {
            // TODO: portar ColProbablesFrm.BorraPronosticosColumnaPantalla.
        }

        [RelayCommand]
        private void CambiarPuntuacion()
        {
            // TODO: portar ColProbablesFrm btnPuntuacion.
        }

        [RelayCommand]
        private void ImportarCPs()
        {
            // TODO: portar ColProbablesFrm.ImportaColumnas (abre ImportadorCPsFrm).
        }

        [RelayCommand]
        private void ExportarCPs()
        {
            // TODO: portar ColProbablesFrm.ExportaColumnas (abre ExportadorCPsFrm).
        }

        // Pestaña Relaciones I. Legacy: btnPrevRel1 / btnNextRel1 / btnEliminarRel1.
        [RelayCommand]
        private void Rel1Anterior()
        {
            // TODO: portar ColProbablesFrm btnPrevRel1 -> CambiaRelCP1Selecionado(relCP1Pantalla - 1).
        }

        [RelayCommand]
        private void Rel1Siguiente()
        {
            // TODO: portar ColProbablesFrm btnNextRel1 -> CambiaRelCP1Selecionado(relCP1Pantalla + 1).
        }

        [RelayCommand]
        private void Rel1Eliminar()
        {
            // TODO: portar ColProbablesFrm btnEliminarRel1 (BorrarRel1).
        }

        // Pestaña Relaciones II. Legacy: btnRel2Atras / btnRel2Adelante / btnEliminaRel2.
        [RelayCommand]
        private void Rel2Anterior()
        {
            // TODO: portar ColProbablesFrm btnRel2Atras (AdaptarControlesDesplazamientoRelaciones2).
        }

        [RelayCommand]
        private void Rel2Siguiente()
        {
            // TODO: portar ColProbablesFrm btnRel2Adelante.
        }

        [RelayCommand]
        private void Rel2Eliminar()
        {
            // TODO: portar ColProbablesFrm btnEliminaRel2.
        }

        // Pestaña Relaciones III. Legacy: btnAtrasRelacion3 / btnAdelanteRelacion3 / btnEliminaRel3.
        [RelayCommand]
        private void Rel3Anterior()
        {
            // TODO: portar ColProbablesFrm btnAtrasRelacion3 (AdaptarControlesDesplazamientoRelaciones3).
        }

        [RelayCommand]
        private void Rel3Siguiente()
        {
            // TODO: portar ColProbablesFrm btnAdelanteRelacion3.
        }

        [RelayCommand]
        private void Rel3Eliminar()
        {
            // TODO: portar ColProbablesFrm btnEliminaRel3.
        }

        // Botón global: guardar / aceptar (legacy MenuCondiciones1: aceptar/cancelar/copiar/pegar el filtro).
        [RelayCommand]
        private void Aceptar()
        {
            // TODO: portar ColProbablesFrm.GuardarDatos + GuardarDatosRelacionesCP1 (+ rel2/rel3/controlFallos)
            //       y volcar grupoCP en filtroCP.ColProbables. Cierra el diálogo en el legacy.
        }
    }
}
