using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Pestañas Relaciones I/II/III y Control Fallos: cableadas al motor real replicando los
    /// manejadores del WinForms (Free1X2/UI/Filtros/ColProbablesFrm.cs):
    ///   - Relaciones I  -> ControladorRelacionesCP1 / RelacionCP1 (Columnas, SumaAciertos,
    ///     Recorridos, CantidadCP, CuantosAC). Edición registro a registro con navegación
    ///     anterior/siguiente y borrado, igual que el form (InicializaDatosRelacionesCP +
    ///     GuardarDatosRelacionesCP1, líneas 920-1111).
    ///   - Relaciones II -> ControladorRelacionesCP2 / RelacionCP2 (bloques Globales e
    ///     Individuales). Navegación 1-based (indiceNavRel2) replicando MostrarRelacion2 +
    ///     GuardarRelacion2EnPantalla (líneas 3573-3865).
    ///   - Relaciones III -> ControladorRelacionesCP3 / RelacionCP3 (Concepto, Columnas
    ///     implicadas, Escaleras Total/Asc/Desc, Sándwichs). Navegación 1-based (indiceNavRel3)
    ///     replicando MostrarRelacion3 + GuardarRelacion3EnPantalla (líneas 3651-3828). Las
    ///     rejillas de Agrupaciones Paso Fijo / Solapadas (DataSet) se conservan en round-trip
    ///     vía sus *String pero no se editan aquí (ver TODO).
    ///   - Control Fallos -> ControladorCPControlFallos / CPControlFallos: tabla editable
    ///     (Columnas / Tolerancias / Aciertos) + FallosPermitidos, replicando
    ///     InicializaDatosControlFallos + GuardarDatosControlFallos (líneas 1150-1275).
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

        // ===== Estado del motor (pestañas Relaciones / Control Fallos) =====

        // Relaciones I: copia de trabajo (legacy List<RelacionCP1> arrayRelaciones1) e índice 0-based
        // (legacy relCP1Pantalla).
        private List<RelacionCP1> _arrayRelaciones1 = new();
        private int _relCP1Pantalla;

        // Relaciones II: copia de trabajo (legacy arrayRelaciones2) e índice 1-based (legacy indiceNavRel2).
        private List<RelacionCP2> _arrayRelaciones2 = new();
        private int _indiceNavRel2 = 1;

        // Relaciones III: copia de trabajo (legacy arrayRelaciones3) e índice 1-based (legacy indiceNavRel3).
        private List<RelacionCP3> _arrayRelaciones3 = new();
        private int _indiceNavRel3 = 1;

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

        // Legacy: txtFallosCtrl ("Número de Fallos de Controles" = ControladorCPControlFallos.FallosPermitidos).
        [ObservableProperty]
        private string _numeroFallosControles = "";

        // Tabla de controles de fallo (legacy dgControlFallos: Columnas / Tolerancias / Aciertos).
        // Bindeada a un DataGrid/ListView editable en la página.
        public ObservableCollection<ControlFallosFila> ControlesFallo { get; } = new();

        /// <summary>Acción de cierre/volver (la cablea la página con Frame.GoBack()). Legacy: cerrar diálogo.</summary>
        public Action? Volver { get; set; }

        /// <summary>
        /// Acción para navegar a otra página, opcionalmente con parámetro (la cablea la página con
        /// Frame.Navigate(tipo, parametro)). Mismo patrón que PosiblesPremiosFrmPage.
        /// </summary>
        public Action<Type, object?>? Navegar { get; set; }

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

                _arrayRelaciones1 = new List<RelacionCP1>();
                _relCP1Pantalla = 0;
                ActualizaDatosPantRel1(_relCP1Pantalla);

                _arrayRelaciones2 = new List<RelacionCP2>();
                _indiceNavRel2 = 1;
                ReinicializarValoresRelaciones2();

                _arrayRelaciones3 = new List<RelacionCP3>();
                _indiceNavRel3 = 1;
                ReinicializarValoresRelaciones3();

                NumeroFallosControles = "";
                ControlesFallo.Clear();
                return;
            }

            _filtroCP = (FiltroColProbables)grupo.GetFiltro(Filtro.ColProbables.ToString());
            _grupoCP = ObtenCopiaCP(_filtroCP);
            _cpPantalla = 0;
            ActualizaDatosPantalla(_cpPantalla);

            // Pestañas de relaciones y control de fallos (legacy InicializaDatosRelacionesCP +
            // InicializaDatosControlFallos, ColProbablesFrm.cs líneas 920-949, 1150-1189).
            InicializaDatosRelacionesCP();
            InicializaDatosControlFallos();
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

            // Vuelca las demás pestañas (legacy GuardarFiltro, líneas 3150-3157:
            // GuardarDatos + GuardarDatosRelacionesCP1/2/3 + GuardarDatosControlFallos).
            GuardarDatosRelacionesCP1();
            GuardarDatosRelacionesCP2();
            GuardarDatosRelacionesCP3();
            GuardarDatosControlFallos();

            AppState.Instancia.NotificarCambio();
            Volver?.Invoke();
        }

        // ===== Acciones de la pestaña Columnas pendientes de portar UserControls/diálogos =====

        // Valores AC/ACS/FS capturados de la pantalla al abrir el diálogo de copia (legacy
        // CopiaValoresCP, líneas 764-783): se capturan ANTES de mostrar el diálogo y se aplican al
        // volver. Se guardan aquí porque la página del diálogo se navega aparte (handoff de vuelta).
        private string _copiaValoresAC = "";
        private string _copiaValoresACS = "";
        private string _copiaValoresFS = "";
        private bool _copiaPendiente;

        /// <summary>
        /// True entre que se lanza el diálogo "Copiar Datos" y se regresa de él. La página lo consulta
        /// en OnNavigatedTo para distinguir el retorno del diálogo (aplicar copia) de una recarga normal.
        /// </summary>
        public bool CopiaDatosPendiente => _copiaPendiente;

        [RelayCommand]
        private void CopiarDatos()
        {
            // Legacy ColProbablesFrm.CopiaValoresCP (Free1X2/UI/Filtros/ColProbablesFrm.cs línea 761):
            //   abría CopiarDatosCPFrm(cpPantalla + 1, grupoCP.Count) para elegir un rango [Desde,Hasta].
            // El rango inicial se pasa como parámetro de navegación, igual que el constructor legacy;
            // CopiarDatosCPFrmPage.OnNavigatedTo lo consume vía ValueTuple<double,double>.
            if (_grupoCP.Count == 0) return; // legacy: if (grupoCP.Count == 0) return;

            // Captura los valores AC/ACS/FS de la pantalla (legacy líneas 764-783); "" -> todos.
            string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();
            _copiaValoresAC = Aciertos != "" ? Aciertos : todosValores;
            _copiaValoresACS = AciertosSeguidos != "" ? AciertosSeguidos : todosValores;
            _copiaValoresFS = FallosSeguidos != "" ? FallosSeguidos : todosValores;
            _copiaPendiente = true;

            // Limpia un resultado previo y navega al diálogo; al volver, OnNavigatedTo de la página
            // invoca AplicarCopiaDatos con CopiarDatosCPFrmViewModel.Resultado (handoff estático).
            CopiarDatosCPFrmViewModel.Resultado = null;
            Navegar?.Invoke(
                typeof(CopiarDatosCPFrmPage),
                ((double)(_cpPantalla + 1), (double)_grupoCP.Count));
        }

        /// <summary>
        /// Aplica el resultado del diálogo "Copiar Datos" sobre la copia de trabajo de columnas.
        /// Réplica de la segunda mitad de ColProbablesFrm.CopiaValoresCP (líneas 787-809): copia los
        /// rangos AC/ACS/FS capturados a las CPs del rango [Desde-1, Hasta) y a la columna actual.
        /// La cablea ColProbablesFrmPage.OnNavigatedTo al regresar del diálogo.
        /// </summary>
        public void AplicarCopiaDatos()
        {
            // Sólo procede si esta pantalla había lanzado el diálogo (evita aplicar resultados ajenos).
            if (!_copiaPendiente) return;
            _copiaPendiente = false;

            var resultado = CopiarDatosCPFrmViewModel.Resultado;
            CopiarDatosCPFrmViewModel.Resultado = null;
            if (resultado is null) return; // El usuario volvió sin confirmar ni cancelar explícitamente.

            // legacy: int min = f.Desde - 1; if (min < 0) return; (cancelar deja Desde = -1).
            int min = resultado.Value.Desde - 1;
            if (min < 0) return;
            int max = resultado.Value.Hasta;

            ColumnaProbable cp;
            for (int i = min; i < max && i < _grupoCP.Count; i++)
            {
                cp = _grupoCP[i];
                cp.ReinicializaValores();
                cp.SetNoAciertos(_copiaValoresAC);
                cp.SetNoAciertosSeguidos(_copiaValoresACS);
                cp.SetNoFallosSeguidos(_copiaValoresFS);
            }

            // Repite la operación para la columna actual si no quedó dentro del rango (legacy 800-807).
            if ((_cpPantalla < min || _cpPantalla > max) && _cpPantalla < _grupoCP.Count)
            {
                cp = _grupoCP[_cpPantalla];
                cp.ReinicializaValores();
                cp.SetNoAciertos(_copiaValoresAC);
                cp.SetNoAciertosSeguidos(_copiaValoresACS);
                cp.SetNoFallosSeguidos(_copiaValoresFS);
            }

            ActualizaDatosPantalla(_cpPantalla); // legacy: refresca la pantalla con la CP actual.
        }

        [RelayCommand]
        private void CopiarColumnas()
        {
            // Legacy ColProbablesFrm.btnCopiarCols_Click (línea 3039): abría CopiarCPFrm(grupoCP, this)
            //   para copiar columnas a otros grupos. CopiarCPFrmPage toma el origen de
            //   AppState.GrupoEnEdicion (el mismo grupo en edición que esta pantalla), así que basta navegar.
            Navegar?.Invoke(typeof(CopiarCPFrmPage), null);
        }

        [RelayCommand]
        private void CambiarPuntuacion()
        {
            // Legacy ColProbablesFrm.btnPuntuacion_Click (línea 3092): CambioPuntosFrm f = new(); f.ShowDialog();
            Navegar?.Invoke(typeof(CambioPuntosFrmPage), null);
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

        // ================= Pestaña Relaciones I (RelacionCP1) =================

        // Legacy InicializaDatosRelacionesCP (líneas 920-949): clona las RelacionCP1 guardadas a la
        // copia de trabajo y muestra la primera. También inicializa las pestañas II y III.
        private void InicializaDatosRelacionesCP()
        {
            _arrayRelaciones1 = new List<RelacionCP1>();
            _indiceNavRel2 = 1;
            _indiceNavRel3 = 1;

            // Pestañas II y III (legacy MostrarPrimeraRelacion2 / MostrarPrimeraRelacion3).
            MostrarPrimeraRelacion2();
            MostrarPrimeraRelacion3();

            if (_filtroCP is not null)
            {
                List<RelacionCP1> relacionesCP = _filtroCP.RelacionesCP1.Relaciones;
                for (int i = 0; i < relacionesCP.Count; i++)
                {
                    RelacionCP1 relGuardada = relacionesCP[i];
                    var rel = new RelacionCP1
                    {
                        Columnas = relGuardada.Columnas,
                        SumaAciertos = relGuardada.SumaAciertos,
                        Recorridos = relGuardada.Recorridos,
                        CantidadCP = relGuardada.CantidadCP,
                        CuantosAC = relGuardada.CuantosAC,
                    };
                    _arrayRelaciones1.Add(rel);
                }
            }

            _relCP1Pantalla = 0;
            ActualizaDatosPantRel1(_relCP1Pantalla);
        }

        // Legacy ActualizaDatosPantRel1 (líneas 951-974).
        private void ActualizaDatosPantRel1(int relCP1)
        {
            if (_arrayRelaciones1.Count > 0 && relCP1 >= 0 && relCP1 < _arrayRelaciones1.Count)
            {
                RelacionCP1 rel = _arrayRelaciones1[relCP1];
                Rel1Columnas = rel.Columnas;
                Rel1Recorrido = rel.Recorridos;
                Rel1SumaAciertos = rel.SumaAciertos;
                Rel1CuantasCP = rel.CantidadCP;
                Rel1NumeroAciertos = rel.CuantosAC;
                PaginacionRel1 = (relCP1 + 1) + "/" + _arrayRelaciones1.Count;
            }
            else
            {
                Rel1Columnas = "";
                Rel1Recorrido = "";
                Rel1SumaAciertos = "";
                Rel1CuantasCP = "";
                Rel1NumeroAciertos = "";
                PaginacionRel1 = "1/1";
            }
        }

        // Legacy TieneRelacion1Datos (líneas 976-992).
        private bool TieneRelacion1Datos()
        {
            if (Rel1Columnas == "")
            {
                return false;
            }
            if (Rel1SumaAciertos == "" && Rel1Recorrido == "" && Rel1CuantasCP == "")
            {
                return false;
            }
            return true;
        }

        // Legacy CambiaRelCP1Selecionado (líneas 999-1025).
        private void CambiaRelCP1Selecionado(int relCP1)
        {
            GuardarRelCP1Actual();
            _relCP1Pantalla = relCP1;

            if (_arrayRelaciones1.Count < relCP1 + 1)
            {
                _arrayRelaciones1.Add(new RelacionCP1());
            }

            ActualizaDatosPantRel1(_relCP1Pantalla);
        }

        // Legacy GuardarRelCP1Actual (líneas 1027-1044).
        private void GuardarRelCP1Actual()
        {
            RelacionCP1 rel;
            if (_relCP1Pantalla < _arrayRelaciones1.Count)
            {
                rel = _arrayRelaciones1[_relCP1Pantalla];
                GuardaDatosRel1(rel);
            }
            else if (TieneRelacion1Datos())
            {
                rel = new RelacionCP1();
                _arrayRelaciones1.Add(rel);
                GuardaDatosRel1(rel);
            }
        }

        // Legacy GuardaDatosRel1 (líneas 1075-1085).
        private void GuardaDatosRel1(RelacionCP1 rel)
        {
            if (TieneRelacion1Datos())
            {
                rel.Columnas = Rel1Columnas;
                rel.SumaAciertos = Rel1SumaAciertos;
                rel.Recorridos = Rel1Recorrido;
                rel.CantidadCP = Rel1CuantasCP;
                rel.CuantosAC = Rel1NumeroAciertos;
            }
        }

        // Legacy NecesitaBorrarUltimaRel1 (líneas 1093-1111).
        private bool NecesitaBorrarUltimaRel1()
        {
            RelacionCP1 rel = _arrayRelaciones1[_arrayRelaciones1.Count - 1];
            if (rel.Columnas == "")
            {
                return true;
            }
            if (rel.SumaAciertos == "" && rel.Recorridos == "" && rel.CantidadCP == "")
            {
                return true;
            }
            return false;
        }

        // Legacy GuardarDatosRelacionesCP1 (líneas 1047-1074).
        private void GuardarDatosRelacionesCP1()
        {
            if (_filtroCP is null) return;

            GuardarRelCP1Actual();

            if (_arrayRelaciones1.Count > 0 && NecesitaBorrarUltimaRel1())
            {
                _arrayRelaciones1.RemoveAt(_arrayRelaciones1.Count - 1);
            }

            var relacionesCPFinal = new List<RelacionCP1>();
            for (int i = 0; i < _arrayRelaciones1.Count; i++)
            {
                RelacionCP1 rel = _arrayRelaciones1[i];
                if (rel.Columnas != "")
                {
                    relacionesCPFinal.Add(rel);
                }
            }

            _filtroCP.RelacionesCP1.Relaciones = relacionesCPFinal;
        }

        // Legacy btnPrevRel1_Click (línea 3098).
        [RelayCommand]
        private void Rel1Anterior()
        {
            CambiaRelCP1Selecionado(_relCP1Pantalla - 1);
        }

        // Legacy btnNextRel1_Click (líneas 3103-3109): sólo avanza si la pantalla tiene datos.
        [RelayCommand]
        private void Rel1Siguiente()
        {
            if (TieneRelacion1Datos())
            {
                CambiaRelCP1Selecionado(_relCP1Pantalla + 1);
            }
        }

        // Legacy btnEliminarRel1_Click (líneas 3111-3138).
        [RelayCommand]
        private void Rel1Eliminar()
        {
            if (_relCP1Pantalla == 0)
            {
                if (_arrayRelaciones1.Count > 0)
                {
                    _arrayRelaciones1.RemoveAt(_relCP1Pantalla);
                }
            }
            else
            {
                _arrayRelaciones1.RemoveAt(_relCP1Pantalla);
                _relCP1Pantalla = _relCP1Pantalla - 1;
            }

            if (_arrayRelaciones1.Count == 0)
            {
                _arrayRelaciones1.Add(new RelacionCP1());
            }

            ActualizaDatosPantRel1(_relCP1Pantalla);
        }

        // ================= Pestaña Relaciones II (RelacionCP2) =================

        // Legacy ReinicializarValoresRelaciones2 (líneas 3541-3556).
        private void ReinicializarValoresRelaciones2()
        {
            Rel2SumaColsA = "";
            Rel2SumaColsB = "";
            Rel2SumaNumAciertos = "";
            Rel2SumaConcepto = "AC";
            Rel2SumaMasMenos = "Más";

            Rel2IndColsA = "";
            Rel2IndColsB = "";
            Rel2IndNumAciertos = "";
            Rel2IndConcepto = "AC";
            Rel2IndMasMenos = "Más";

            AdaptarControlesDesplazamientoRelaciones2();
        }

        // Legacy MostrarPrimeraRelacion2 (líneas 3627-3649): la copia de trabajo es la lista del filtro.
        private void MostrarPrimeraRelacion2()
        {
            _arrayRelaciones2 = _filtroCP is not null
                ? _filtroCP.RelacionesCP2.Relaciones2
                : new List<RelacionCP2>();

            if (_arrayRelaciones2.Count > 0)
            {
                CargarRelacion2EnPantalla(_arrayRelaciones2[0]);
            }
            else
            {
                ReinicializarValoresRelaciones2();
            }
        }

        // Legacy MostrarRelacion2 (líneas 3605-3626).
        private void MostrarRelacion2()
        {
            if (_arrayRelaciones2.Count >= _indiceNavRel2)
            {
                CargarRelacion2EnPantalla(_arrayRelaciones2[_indiceNavRel2 - 1]);
            }
            else
            {
                ReinicializarValoresRelaciones2();
            }
        }

        private void CargarRelacion2EnPantalla(RelacionCP2 rel)
        {
            Rel2SumaColsA = rel.StrColsA;
            Rel2SumaColsB = rel.StrColsB;
            Rel2SumaNumAciertos = rel.StrAciertos;
            Rel2SumaConcepto = rel.Concepto;
            Rel2SumaMasMenos = rel.Cantidad;

            Rel2IndColsA = rel.StrColsA2;
            Rel2IndColsB = rel.StrColsB2;
            Rel2IndNumAciertos = rel.StrAciertos2;
            Rel2IndConcepto = rel.Concepto2;
            Rel2IndMasMenos = rel.Cantidad2;
        }

        // Legacy ComprobarRelacion2 (líneas 3733-3744).
        private static bool ComprobarRelacion2(RelacionCP2 rel)
        {
            if ((rel.ColumnasA.Count == 0 || rel.ColumnasB.Count == 0 || rel.Aciertos.Count == 0) &&
                (rel.ColumnasA2.Count == 0 || rel.ColumnasB2.Count == 0 || rel.Aciertos2.Count == 0))
            {
                return false;
            }
            return true;
        }

        // Legacy GuardarRelacion2 (líneas 3709-3719): sustituye la actual o añade al final.
        private void GuardarRelacion2(RelacionCP2 rel)
        {
            if (_arrayRelaciones2.Count >= _indiceNavRel2)
            {
                _arrayRelaciones2[_indiceNavRel2 - 1] = rel;
            }
            else
            {
                _arrayRelaciones2.Add(rel);
            }
        }

        // Legacy GuardarRelacion2EnPantalla (líneas 3757-3784).
        private void GuardarRelacion2EnPantalla()
        {
            var rel = new RelacionCP2
            {
                Aciertos = UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(Rel2SumaNumAciertos),
                ColumnasA = UtilidadesEntradasValores.ObtenerListaFromTxt(Rel2SumaColsA),
                ColumnasB = UtilidadesEntradasValores.ObtenerListaFromTxt(Rel2SumaColsB),
                Concepto = Rel2SumaConcepto,
                Cantidad = Rel2SumaMasMenos,
                StrAciertos = Rel2SumaNumAciertos,
                StrColsA = Rel2SumaColsA,
                StrColsB = Rel2SumaColsB,

                Aciertos2 = UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(Rel2IndNumAciertos),
                ColumnasA2 = UtilidadesEntradasValores.ObtenerListaFromTxt(Rel2IndColsA),
                ColumnasB2 = UtilidadesEntradasValores.ObtenerListaFromTxt(Rel2IndColsB),
                Concepto2 = Rel2IndConcepto,
                Cantidad2 = Rel2IndMasMenos,
                StrAciertos2 = Rel2IndNumAciertos,
                StrColsA2 = Rel2IndColsA,
                StrColsB2 = Rel2IndColsB,

                ColumnasProbables = _grupoCP,
            };

            if (ComprobarRelacion2(rel))
            {
                GuardarRelacion2(rel);
            }
        }

        // Legacy AdaptarControlesDesplazamientoRelaciones2 (líneas 3850-3865): sólo refresca la paginación.
        private void AdaptarControlesDesplazamientoRelaciones2()
        {
            PaginacionRel2 = _indiceNavRel2 + " / " + _arrayRelaciones2.Count;
        }

        // Legacy GuardarDatosRelacionesCP2 (líneas 3695-3701).
        private void GuardarDatosRelacionesCP2()
        {
            if (_filtroCP is null) return;

            GuardarRelacion2EnPantalla();

            _filtroCP.RelacionesCP2.ColumnasProbables = _grupoCP;
            _filtroCP.RelacionesCP2.Relaciones2 = _arrayRelaciones2;
        }

        // Legacy btnRel2Atras_Click (líneas 3840-3848).
        [RelayCommand]
        private void Rel2Anterior()
        {
            if (_indiceNavRel2 <= 1) return; // legacy: botón "atrás" deshabilitado en la 1ª.

            GuardarRelacion2EnPantalla();
            _indiceNavRel2--;
            AdaptarControlesDesplazamientoRelaciones2();
            MostrarRelacion2();
        }

        // Legacy btnRel2Adelante_Click (líneas 3830-3838).
        [RelayCommand]
        private void Rel2Siguiente()
        {
            GuardarRelacion2EnPantalla();
            _indiceNavRel2 += 1;
            MostrarRelacion2();
            AdaptarControlesDesplazamientoRelaciones2();
        }

        // Legacy btnEliminaRel2_Click + EliminaRelacion2EnPantalla (líneas 3883-3887, 3573-3587).
        [RelayCommand]
        private void Rel2Eliminar()
        {
            if (_arrayRelaciones2.Count > 0)
            {
                if (_arrayRelaciones2.Count >= _indiceNavRel2)
                {
                    _arrayRelaciones2.RemoveAt(_indiceNavRel2 - 1);
                    MostrarRelacion2();
                }
            }
            else
            {
                ReinicializarValoresRelaciones2();
            }
            AdaptarControlesDesplazamientoRelaciones2();
        }

        // ================= Pestaña Relaciones III (RelacionCP3) =================

        // Legacy ReinicializarValoresRelaciones3 (líneas 3557-3571). Las rejillas de agrupaciones
        // (paso fijo / solapadas) no se editan aquí; ver TODO en GuardarRelacion3EnPantalla.
        private void ReinicializarValoresRelaciones3()
        {
            Rel3Columnas = "";
            Rel3Sandwichs = "";
            Rel3EscalerasTotal = "";
            Rel3EscalerasAsc = "";
            Rel3EscalerasDesc = "";
            Rel3Concepto = "AC";

            AdaptarControlesDesplazamientoRelaciones3();
        }

        // Legacy MostrarPrimeraRelacion3 (líneas 3672-3693).
        private void MostrarPrimeraRelacion3()
        {
            _arrayRelaciones3 = _filtroCP is not null
                ? _filtroCP.RelacionesCP3.Relaciones
                : new List<RelacionCP3>();

            if (_arrayRelaciones3.Count > 0)
            {
                CargarRelacion3EnPantalla(_arrayRelaciones3[0]);
            }
            else
            {
                ReinicializarValoresRelaciones3();
            }
        }

        // Legacy MostrarRelacion3 (líneas 3651-3671).
        private void MostrarRelacion3()
        {
            if (_arrayRelaciones3.Count >= _indiceNavRel3)
            {
                CargarRelacion3EnPantalla(_arrayRelaciones3[_indiceNavRel3 - 1]);
            }
            else
            {
                ReinicializarValoresRelaciones3();
            }
        }

        private void CargarRelacion3EnPantalla(RelacionCP3 rel)
        {
            Rel3Columnas = rel.ColumnasImplicadasString;
            Rel3Sandwichs = rel.NumeroSandwichsPermitidosString;
            Rel3EscalerasTotal = rel.NumeroEscalerasTotalesPermitidasString;
            Rel3EscalerasAsc = rel.NumeroEscalerasASCPermitidasString;
            Rel3EscalerasDesc = rel.NumeroEscalerasDESCPermitidasString;
            Rel3Concepto = rel.ConceptoString;
            // TODO[grids]: las rejillas dgAgrupacionesPasoFijo / dgAgrpacionesSolapadas
            //   (ColProbablesFrm.cs líneas 3663-3665, InicializaDatosAgrupaciones*) no se editan
            //   en WinUI; sus datos se conservan en el round-trip vía las cadenas
            //   AgrupacionesPasoFijoPermitidasString / AgrupacionesSolapadasPermitidasString.
        }

        // Legacy ObtenerColumnasImplicadasRelCP3 (líneas 4098-4107). NOTA: replica el bucle exacto
        // del WinForms, que añade grupoCP[i] (índice posicional, no el valor de la lista de índices).
        private List<ColumnaProbable> ObtenerColumnasImplicadasRelCP3()
        {
            var lista = new List<ColumnaProbable>();
            List<int> indices = UtilidadesEntradasValores.ObtenerListaFromTxt(Rel3Columnas);
            for (int i = 0; i < indices.Count; i++)
            {
                if (i < _grupoCP.Count)
                {
                    lista.Add(_grupoCP[i]);
                }
            }
            return lista;
        }

        // Legacy ComprobarRelacion3 (líneas 3745-3754).
        private static bool ComprobarRelacion3(RelacionCP3 rel)
        {
            if (rel.AgrupacionesPasoFijoPermitidas == null && rel.AgrupacionesSolapadasPermitidas == null &&
                rel.ColumnasImplicadasString == "" && rel.NumeroEscalerasASCPermitidas == null &&
                rel.NumeroEscalerasDESCPermitidas == null && rel.NumeroEscalerasTotalesPermitidas == null &&
                rel.NumeroSandwichsPermitidos == null)
            {
                return false;
            }
            return true;
        }

        // Legacy GuardarRelacion3 (líneas 3720-3730).
        private void GuardarRelacion3(RelacionCP3 rel)
        {
            if (_arrayRelaciones3.Count >= _indiceNavRel3)
            {
                _arrayRelaciones3[_indiceNavRel3 - 1] = rel;
            }
            else
            {
                _arrayRelaciones3.Add(rel);
            }
        }

        // Legacy GuardarRelacion3EnPantalla (líneas 3785-3828). Sin edición de las rejillas de
        // agrupaciones; las cadenas de agrupaciones quedan nulas (no se editan en WinUI).
        private void GuardarRelacion3EnPantalla()
        {
            List<ColumnaProbable> lista = ObtenerColumnasImplicadasRelCP3();
            var rel = new RelacionCP3
            {
                Concepto = Rel3Concepto,
                ConceptoString = Rel3Concepto,
                Columnas = lista,
            };

            int longitudEscaleras = lista.Count / 3;
            int longitudSandwichs = lista.Count / 4;

            rel.ColumnasImplicadasString = Rel3Columnas;

            rel.NumeroEscalerasASCPermitidas = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Rel3EscalerasAsc, longitudEscaleras);
            rel.NumeroEscalerasASCPermitidasString = Rel3EscalerasAsc;

            rel.NumeroEscalerasDESCPermitidas = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Rel3EscalerasDesc, longitudEscaleras);
            rel.NumeroEscalerasDESCPermitidasString = Rel3EscalerasDesc;

            rel.NumeroEscalerasTotalesPermitidas = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Rel3EscalerasTotal, longitudEscaleras);
            rel.NumeroEscalerasTotalesPermitidasString = Rel3EscalerasTotal;

            rel.NumeroSandwichsPermitidos = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Rel3Sandwichs, longitudSandwichs);
            rel.NumeroSandwichsPermitidosString = Rel3Sandwichs;

            // TODO[grids]: ColProbablesFrm.cs líneas 3812-3820 leían las rejillas de Agrupaciones
            //   Paso Fijo / Solapadas (DataSet) y rellenaban AgrupacionesPasoFijoPermitidas(String)
            //   y AgrupacionesSolapadasPermitidas(String). Esas rejillas no están portadas; aquí
            //   quedan nulas. Portar dos DataGrid editables (Número / Elementos / Aciertos) para
            //   replicar ObtenArrayAgrupaciones* (líneas 4110-4205).

            if (ComprobarRelacion3(rel))
            {
                GuardarRelacion3(rel);
            }
        }

        // Legacy AdaptarControlesDesplazamientoRelaciones3 (líneas 3866-3881).
        private void AdaptarControlesDesplazamientoRelaciones3()
        {
            PaginacionRel3 = _indiceNavRel3 + " / " + _arrayRelaciones3.Count;
        }

        // Legacy GuardarDatosRelacionesCP3 (líneas 3702-3707).
        private void GuardarDatosRelacionesCP3()
        {
            if (_filtroCP is null) return;

            GuardarRelacion3EnPantalla();
            _filtroCP.RelacionesCP3.Relaciones = _arrayRelaciones3;
        }

        // Legacy btnAtrasRelacion3_Click (líneas 4207-4215).
        [RelayCommand]
        private void Rel3Anterior()
        {
            if (_indiceNavRel3 <= 1) return; // legacy: botón "atrás" deshabilitado en la 1ª.

            GuardarRelacion3EnPantalla();
            _indiceNavRel3--;
            AdaptarControlesDesplazamientoRelaciones3();
            MostrarRelacion3();
        }

        // Legacy btnAdelanteRelacion3_Click (líneas 4217-4225).
        [RelayCommand]
        private void Rel3Siguiente()
        {
            GuardarRelacion3EnPantalla();
            _indiceNavRel3++;
            MostrarRelacion3();
            AdaptarControlesDesplazamientoRelaciones3();
        }

        // Legacy btnEliminaRel3_Click + EliminaRelacion3EnPantalla (líneas 4227-4231, 3588-3603).
        [RelayCommand]
        private void Rel3Eliminar()
        {
            if (_arrayRelaciones3.Count > 0)
            {
                if (_arrayRelaciones3.Count >= _indiceNavRel3)
                {
                    _arrayRelaciones3.RemoveAt(_indiceNavRel3 - 1);
                    MostrarRelacion3();
                }
            }
            else
            {
                ReinicializarValoresRelaciones3();
            }
            AdaptarControlesDesplazamientoRelaciones3();
        }

        // ================= Pestaña Control Fallos (CPControlFallos) =================

        // Legacy InicializaDatosControlFallos (líneas 1150-1189). El WinForms rellenaba 50 filas en
        // blanco en una rejilla fija; en WinUI se usa una colección con filas reales + botón "añadir".
        private void InicializaDatosControlFallos()
        {
            ControlesFallo.Clear();

            if (_filtroCP is null)
            {
                NumeroFallosControles = "";
                return;
            }

            NumeroFallosControles = _filtroCP.ControlFallosCP.FallosPermitidos;
            List<CPControlFallos> controlesFallo = _filtroCP.ControlFallosCP.ControlesFallos;

            for (int i = 0; i < controlesFallo.Count; i++)
            {
                CPControlFallos guardada = controlesFallo[i];
                ControlesFallo.Add(new ControlFallosFila
                {
                    Columnas = guardada.Columnas,
                    Tolerancias = guardada.Tolerancias,
                    Aciertos = guardada.Aciertos,
                });
            }
        }

        // Legacy GuardarDatosControlFallos (líneas 1255-1275): conserva sólo las filas con Columnas.
        private void GuardarDatosControlFallos()
        {
            if (_filtroCP is null) return;

            var arrayControlesFalloFinal = new List<CPControlFallos>();
            foreach (ControlFallosFila fila in ControlesFallo)
            {
                if (!string.IsNullOrEmpty(fila.Columnas))
                {
                    var ctrFallos = new CPControlFallos
                    {
                        Columnas = fila.Columnas ?? "",
                        Tolerancias = fila.Tolerancias ?? "",
                        Aciertos = fila.Aciertos ?? "",
                    };
                    arrayControlesFalloFinal.Add(ctrFallos);
                }
            }

            _filtroCP.ControlFallosCP.ControlesFallos = arrayControlesFalloFinal;
            _filtroCP.ControlFallosCP.FallosPermitidos = NumeroFallosControles ?? "";
        }

        // Añade una fila vacía a la tabla de controles de fallo (no existe en el WinForms, donde la
        // rejilla traía 50 filas en blanco; en WinUI se crean bajo demanda).
        [RelayCommand]
        private void AnadirControlFallo()
        {
            ControlesFallo.Add(new ControlFallosFila());
        }

        // Elimina una fila concreta de la tabla de controles de fallo.
        [RelayCommand]
        private void EliminarControlFallo(ControlFallosFila? fila)
        {
            if (fila is not null)
            {
                ControlesFallo.Remove(fila);
            }
        }
    }

    /// <summary>
    /// Fila editable de la tabla de Control de Fallos (legacy dgControlFallos: columnas
    /// Columnas / Tolerancias / Aciertos del DataSet "CPControlFallos"). Se mapea de/ hacia
    /// <see cref="CPControlFallos"/> en InicializaDatosControlFallos / GuardarDatosControlFallos.
    /// </summary>
    public partial class ControlFallosFila : ObservableObject
    {
        [ObservableProperty]
        private string _columnas = "";

        [ObservableProperty]
        private string _tolerancias = "";

        [ObservableProperty]
        private string _aciertos = "";
    }
}
