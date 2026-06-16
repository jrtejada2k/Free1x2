using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.WinUI.Services;

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
    ///   - Relaciones I/II/III: condiciones entre columnas (no portadas; ver TODOs).
    ///   - Control Fallos: tabla (Columnas / Tolerancias / Aciertos) + puntuación (no portada).
    ///
    /// Cableado al motor real (pestaña Columnas): la lista de columnas probables se carga del
    /// <see cref="FiltroColProbables"/> del grupo en edición (<c>AppState.GrupoEnEdicion</c>),
    /// se edita columna a columna (navegación) y se vuelca de vuelta al filtro al Aceptar
    /// (GuardarDatos). Se usan ColumnaProbable / FiltroColProbables / Grupo / Filtro del dominio.
    ///
    /// Las pestañas Relaciones I/II/III y Control Fallos dependen de controladores y rejillas
    /// (RelacionCP1/2/3, CPControlFallos, DataSet/grids) y UserControls no portados; sus comandos
    /// quedan con TODO preciso a la línea del WinForms.
    /// </summary>
    public partial class ColProbablesFrmViewModel : ObservableObject
    {
        // Opciones para los ComboBox (legacy: cbbConcepto/cbbConcepto2/cbbConceptoRel3 = AC/ACS/FS,
        // cbbMasMenos/cbbMasMenos2 = Más/Menos). Reglas anti-crash: ItemsSource desde IReadOnlyList<string>.
        public IReadOnlyList<string> OpcionesConcepto { get; } = new[] { "AC", "ACS", "FS" };
        public IReadOnlyList<string> OpcionesMasMenos { get; } = new[] { "Más", "Menos" };

        // ===== Estado del motor (pestaña Columnas) =====

        // Filtro de columnas probables del grupo en edición (legacy: filtroCP).
        private FiltroColProbables? _filtroCP;

        // Copia de trabajo de las columnas probables (legacy: List<ColumnaProbable> grupoCP).
        private List<ColumnaProbable> _grupoCP = new();

        // Índice de la columna en pantalla (legacy: int cpPantalla).
        private int _cpPantalla;

        // Evita que la sincronización pantalla<->modelo se dispare mientras cargamos una columna.
        private bool _cargando;

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

        /// <summary>Acción de cierre/volver (la cablea la página con Frame.GoBack()). Legacy: cerrar diálogo.</summary>
        public Action? Volver { get; set; }

        public ColProbablesFrmViewModel()
        {
            CargarDesdeGrupo();
        }

        // ================= Carga / guardado contra el motor =================

        /// <summary>
        /// Carga la copia de trabajo de columnas probables del grupo en edición.
        /// Equivale a ColProbablesFrm.InicializaDatos() + ObtenCopiaCP() (líneas 266-302).
        /// </summary>
        public void CargarDesdeGrupo()
        {
            Grupo? grupo = AppState.GrupoEnEdicion;
            if (grupo is null)
            {
                _filtroCP = null;
                _grupoCP = new List<ColumnaProbable>();
                _cpPantalla = 0;
                ActualizaDatosPantalla(0);
                return;
            }

            _filtroCP = (FiltroColProbables)grupo.GetFiltro(Filtro.ColProbables.ToString());
            _grupoCP = ObtenCopiaCP(_filtroCP);
            _cpPantalla = 0;
            ActualizaDatosPantalla(_cpPantalla);
        }

        /// <summary>Réplica de ColProbablesFrm.ObtenCopiaCP (clona cada ColumnaProbable del filtro).</summary>
        private static List<ColumnaProbable> ObtenCopiaCP(FiltroColProbables filtro)
        {
            var copia = new List<ColumnaProbable>();
            foreach (ColumnaProbable cp in filtro.ColProbables)
            {
                var cpCopia = new ColumnaProbable
                {
                    Pronosticos = cp.Pronosticos,
                };
                cpCopia.SetNoAciertos(cp.GetAciertos());
                cpCopia.SetNoAciertosSeguidos(cp.GetAciertosSeguidos());
                cpCopia.SetNoFallosSeguidos(cp.GetFallosSeguidos());
                cpCopia.SetPuntos(cp.GetPuntos());

                if (cp.ToleranciaLocalActiva)
                {
                    cpCopia.SetACTol(cp.GetACTol());
                    cpCopia.SetACSTol(cp.GetACSTol());
                    cpCopia.SetFSTol(cp.GetFSTol());
                    cpCopia.SetTolerancias(cp.GetTolerancias());
                }
                copia.Add(cpCopia);
            }
            return copia;
        }

        /// <summary>
        /// Vuelca la columna en pantalla al modelo (legacy ActualizaDatosPantalla, líneas 309-345).
        /// Convierte el pronóstico por partido (string[]) a la cadena de 14 signos de la pantalla.
        /// </summary>
        private void ActualizaDatosPantalla(int noCP)
        {
            _cargando = true;
            try
            {
                ColumnaProbable cp;
                if (_grupoCP.Count > 0 && noCP >= 0 && noCP < _grupoCP.Count)
                {
                    cp = _grupoCP[noCP];
                    PaginacionColumnas = (noCP + 1) + "/" + _grupoCP.Count;
                }
                else
                {
                    cp = new ColumnaProbable();
                    PaginacionColumnas = "1/1";
                }

                // Pronóstico: la pantalla usa una cadena de 14 signos (un carácter por partido).
                // NOTA: la TextBox sólo admite signos simples (MaxLength=14); para dobles/triples
                // se concatena la representación del partido, que puede exceder 14 caracteres.
                string[] pronosticos = cp.Pronosticos;
                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < pronosticos.Length; i++)
                {
                    sb.Append(pronosticos[i]);
                }
                Pronostico = sb.ToString();

                Aciertos = cp.GetAciertos();
                AciertosSeguidos = cp.GetAciertosSeguidos();
                FallosSeguidos = cp.GetFallosSeguidos();
                Puntos = cp.GetPuntos();

                AciertosTol = cp.GetACTol();
                AciertosSeguidosTol = cp.GetACSTol();
                FallosSeguidosTol = cp.GetFSTol();

                CargarTolerancias(cp.GetTolerancias());
            }
            finally
            {
                _cargando = false;
            }
        }

        // Legacy ToleranciaValores (set): marca tol0..tol3 según la cadena "0,1,..".
        private void CargarTolerancias(string valores)
        {
            Tolerancia0 = Tolerancia1 = Tolerancia2 = Tolerancia3 = false;
            foreach (string val in (valores ?? "").Split(','))
            {
                switch (val)
                {
                    case "0": Tolerancia0 = true; break;
                    case "1": Tolerancia1 = true; break;
                    case "2": Tolerancia2 = true; break;
                    case "3": Tolerancia3 = true; break;
                }
            }
        }

        // Legacy ToleranciaValores (get): construye la cadena "0,1,.." de las tolerancias activas.
        private string ObtenerTolerancias()
        {
            var partes = new List<string>();
            if (Tolerancia0) partes.Add("0");
            if (Tolerancia1) partes.Add("1");
            if (Tolerancia2) partes.Add("2");
            if (Tolerancia3) partes.Add("3");
            return string.Join(",", partes);
        }

        // Legacy TieneColumnaDatos(): true si el pronóstico de pantalla tiene algún signo.
        private bool TieneColumnaDatos() => !string.IsNullOrEmpty(Pronostico);

        // Legacy NecesitaGuardarDatosTol(): hay tolerancias que persistir.
        private bool NecesitaGuardarDatosTol() =>
            AciertosTol != "" || AciertosSeguidosTol != "" || FallosSeguidosTol != "";

        /// <summary>
        /// Vuelca la pantalla a la ColumnaProbable correspondiente.
        /// Equivale a ColProbablesFrm.GuardarCPActual + GuardaDatosCP (líneas 441-567).
        /// </summary>
        private void GuardarCPActual()
        {
            ColumnaProbable cp;
            if (_cpPantalla < _grupoCP.Count)
            {
                cp = _grupoCP[_cpPantalla];
                GuardaDatosCP(cp);
            }
            else if (TieneColumnaDatos())
            {
                cp = new ColumnaProbable();
                _grupoCP.Add(cp);
                GuardaDatosCP(cp);
            }
        }

        private void GuardaDatosCP(ColumnaProbable cp)
        {
            cp.ReinicializaValores();

            // Pronóstico por partido: un carácter por partido (signos simples).
            string[] pronosticos = cp.Pronosticos;
            string pron = Pronostico ?? "";
            for (int i = 0; i < pronosticos.Length; i++)
            {
                pronosticos[i] = i < pron.Length ? pron[i].ToString() : "";
            }
            cp.Pronosticos = pronosticos;

            string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();

            if (TieneColumnaDatos())
            {
                cp.SetPuntos(Puntos ?? "");

                cp.SetNoAciertos(Aciertos != "" ? Aciertos : todosValores);
                cp.SetNoAciertosSeguidos(AciertosSeguidos != "" ? AciertosSeguidos : todosValores);
                cp.SetNoFallosSeguidos(FallosSeguidos != "" ? FallosSeguidos : todosValores);

                if (NecesitaGuardarDatosTol())
                {
                    cp.SetACTol(AciertosTol != "" ? AciertosTol : todosValores);
                    cp.SetACSTol(AciertosSeguidosTol != "" ? AciertosSeguidosTol : todosValores);
                    cp.SetFSTol(FallosSeguidosTol != "" ? FallosSeguidosTol : todosValores);

                    string tols = ObtenerTolerancias();
                    cp.SetTolerancias(tols != "" ? tols : "0");
                }
            }
            else if (_cpPantalla < _grupoCP.Count)
            {
                _grupoCP.RemoveAt(_cpPantalla);
            }
        }

        // Legacy NecesitaBorrarUltimaCP(): la última columna no tiene ningún pronóstico.
        private bool NecesitaBorrarUltimaCP()
        {
            if (_grupoCP.Count == 0) return false;
            ColumnaProbable cp = _grupoCP[_grupoCP.Count - 1];
            foreach (string p in cp.Pronosticos)
            {
                if (!string.IsNullOrEmpty(p)) return false;
            }
            return true;
        }

        // Legacy CambiaCPSelecionado(int noCP) (líneas 621-642): guarda actual, mueve, crea si falta.
        private void CambiaCPSelecionado(int noCP)
        {
            if (noCP < 0) noCP = 0;
            if (noCP > _grupoCP.Count) noCP = _grupoCP.Count;

            GuardarCPActual();
            _cpPantalla = noCP;

            if (_grupoCP.Count < noCP + 1)
            {
                _grupoCP.Add(new ColumnaProbable());
            }

            ActualizaDatosPantalla(noCP);
        }

        // ================= Comandos: pestaña Columnas =================

        // Legacy btnInicioCP / btnPrev3CP / btnPrevCP / btnNextCP / btnNext3CP / btnFinCP.
        [RelayCommand]
        private void IrPrimeraColumna()
        {
            CambiaCPSelecionado(0);
        }

        [RelayCommand]
        private void ColumnasAtras3()
        {
            CambiaCPSelecionado(_cpPantalla - VariablesGlobales.Desplazamiento);
        }

        [RelayCommand]
        private void ColumnaAnterior()
        {
            CambiaCPSelecionado(_cpPantalla - 1);
        }

        [RelayCommand]
        private void ColumnaSiguiente()
        {
            CambiaCPSelecionado(_cpPantalla + 1);
        }

        [RelayCommand]
        private void ColumnasAdelante3()
        {
            CambiaCPSelecionado(_cpPantalla + VariablesGlobales.Desplazamiento);
        }

        [RelayCommand]
        private void IrUltimaColumna()
        {
            CambiaCPSelecionado(_grupoCP.Count > 0 ? _grupoCP.Count - 1 : 0);
        }

        // Legacy btnEliminarActual (BorrarCP de la CP en pantalla).
        [RelayCommand]
        private void EliminarColumnaActual()
        {
            if (_grupoCP.Count > 0 && _cpPantalla < _grupoCP.Count)
            {
                _grupoCP.RemoveAt(_cpPantalla);
                if (_cpPantalla >= _grupoCP.Count) _cpPantalla = Math.Max(0, _grupoCP.Count - 1);
                ActualizaDatosPantalla(_cpPantalla);
            }
        }

        // Legacy PonerTodosATriple (líneas 823-842): pone la columna en pantalla a 1X2 en todos los partidos.
        [RelayCommand]
        private void TodosATriple()
        {
            ColumnaProbable cp;
            if (_grupoCP.Count > _cpPantalla && _cpPantalla >= 0)
            {
                cp = _grupoCP[_cpPantalla];
            }
            else
            {
                cp = new ColumnaProbable();
                _grupoCP.Add(cp);
                _cpPantalla = _grupoCP.Count - 1;
            }
            cp.TodosATriple();
            ActualizaDatosPantalla(_cpPantalla);
        }

        // Legacy BorraPronosticosColumnaPantalla (líneas 811-821).
        [RelayCommand]
        private void BorrarPronosticos()
        {
            if (_grupoCP.Count > _cpPantalla && _cpPantalla >= 0)
            {
                _grupoCP[_cpPantalla].BorraPronosticos();
            }
            ActualizaDatosPantalla(_cpPantalla);
        }

        // Botón global: guardar / aceptar el filtro (legacy GuardarDatos, líneas 413-439).
        [RelayCommand]
        private void Aceptar()
        {
            if (_filtroCP is null)
            {
                Volver?.Invoke();
                return;
            }

            GuardarCPActual();

            // Borrar última CP si quedó vacía (legacy NecesitaBorrarUltimaCP).
            if (_grupoCP.Count > 0 && NecesitaBorrarUltimaCP())
            {
                _grupoCP.RemoveAt(_grupoCP.Count - 1);
            }

            // Primera vez con datos: activa la condición (legacy líneas 429-435).
            if (!_filtroCP.ContieneDatos && _grupoCP.Count > 0)
            {
                _filtroCP.ContieneDatos = true;
                _filtroCP.IsActive = true;
            }

            // Vuelca la copia actualizada al filtro (legacy filtroCP.ColProbables = grupoCP).
            _filtroCP.ColProbables = _grupoCP;

            AppState.Instancia.NotificarCambio();
            Volver?.Invoke();
        }

        // ===== Acciones de la pestaña Columnas pendientes de portar UserControls/diálogos =====

        [RelayCommand]
        private void CopiarDatos()
        {
            // TODO[navegación]: ColProbablesFrm.CopiaValoresCP (Free1X2/UI/Filtros/ColProbablesFrm.cs línea 761)
            //   abría CopiarDatosCPFrm para elegir un rango [Desde,Hasta] y copiaba los rangos AC/ACS/FS
            //   de la columna en pantalla a ese rango. En WinUI: navegar a CopiarDatosCPFrmPage (que ya
            //   devuelve el rango con ResultadoConfirmado) y aplicar el bucle de copia sobre _grupoCP.
        }

        [RelayCommand]
        private void CopiarColumnas()
        {
            // TODO[navegación]: ColProbablesFrm btnCopiarCols abría CopiarCPFrm para copiar columnas a
            //   otros grupos. En WinUI existe CopiarCPFrmPage (ya cableada al motor): navegar a ella
            //   tras fijar AppState.GrupoEnEdicion con el grupo de origen.
        }

        [RelayCommand]
        private void CambiarPuntuacion()
        {
            // TODO[dominio]: ColProbablesFrm btnPuntuacion ajustaba la puntuación (puntos fijos/dobles/
            //   triples) vía FiltroColProbables.InicializaPuntosCP — requiere el diálogo de puntuación
            //   del WinForms (no portado).
        }

        [RelayCommand]
        private void ImportarCPs()
        {
            // TODO[navegación]: ColProbablesFrm.ImportaColumnas (línea 675) abría ImportadorCPsFrm y
            //   fusionaba/sustituía las columnas importadas. En WinUI existe ImportadorCPsFrmPage:
            //   navegar a ella y volcar el resultado en _grupoCP (sustituir/añadir según el caso).
        }

        [RelayCommand]
        private void ExportarCPs()
        {
            // TODO[navegación]: ColProbablesFrm.ExportaColumnas (línea 748) abría ExportadorCPsFrm con
            //   un FiltroColProbables temporal. En WinUI existe ExportadorCPsFrmPage: navegar a ella
            //   pasando una copia de _grupoCP.
        }

        // ===== Pestañas Relaciones I/II/III (controladores no portados) =====

        [RelayCommand]
        private void Rel1Anterior()
        {
            // TODO[dominio]: ColProbablesFrm btnPrevRel1 -> CambiaRelCP1Selecionado(relCP1Pantalla-1).
            //   Requiere portar RelacionCP1 / ControladorRelacionesCP1 a la pantalla (no portado).
        }

        [RelayCommand]
        private void Rel1Siguiente()
        {
            // TODO[dominio]: ColProbablesFrm btnNextRel1 -> CambiaRelCP1Selecionado(relCP1Pantalla+1).
        }

        [RelayCommand]
        private void Rel1Eliminar()
        {
            // TODO[dominio]: ColProbablesFrm btnEliminarRel1 (BorrarRel1) sobre ControladorRelacionesCP1.
        }

        [RelayCommand]
        private void Rel2Anterior()
        {
            // TODO[dominio]: ColProbablesFrm btnRel2Atras (AdaptarControlesDesplazamientoRelaciones2),
            //   sobre ControladorRelacionesCP2 / RelacionCP2 (no portado).
        }

        [RelayCommand]
        private void Rel2Siguiente()
        {
            // TODO[dominio]: ColProbablesFrm btnRel2Adelante.
        }

        [RelayCommand]
        private void Rel2Eliminar()
        {
            // TODO[dominio]: ColProbablesFrm btnEliminaRel2.
        }

        [RelayCommand]
        private void Rel3Anterior()
        {
            // TODO[dominio]: ColProbablesFrm btnAtrasRelacion3 (AdaptarControlesDesplazamientoRelaciones3),
            //   sobre ControladorRelacionesCP3 / RelacionCP3 (no portado).
        }

        [RelayCommand]
        private void Rel3Siguiente()
        {
            // TODO[dominio]: ColProbablesFrm btnAdelanteRelacion3.
        }

        [RelayCommand]
        private void Rel3Eliminar()
        {
            // TODO[dominio]: ColProbablesFrm btnEliminaRel3.
        }
    }
}
