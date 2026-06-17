using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;
using Free1X2.WinUI.Views.Ported;

namespace Free1X2.WinUI.Views;

/// <summary>
/// Acción de la barra de herramientas que se ejecuta sobre la pantalla principal. La barra de
/// herramientas (MainWindow) enruta a <see cref="MainPage"/> con uno de estos tokens y la página
/// invoca el comando correspondiente en su ViewModel (réplica de los handlers del menú "Archivo"
/// del MainForm original: MNuevaComb / MAbrirCombClick / MGuardarComb / MGuardarCombComo /
/// MGuardarPartidosClick / MAbreBoleto / MBorrarCombsTemp / borrarInformesDeError…).
/// </summary>
public enum AccionInicio
{
    Ninguna,
    NuevaCombinacion,
    AbrirCombinacion,
    GuardarCombinacion,
    GuardarCombinacionComo,
    GuardarEquipos,
    AbrirEquipos,
    BorrarTemporales,
    BorrarInformes,
}

/// <summary>
/// ViewModel de la pantalla principal (réplica de <c>Free1X2.UI.MainForm</c>):
/// boleto base + rejilla de condiciones (con semáforos), navegación de grupos,
/// filtro de columnas general y acciones de combinación (Calcular / Reducir /
/// Nueva / Abrir / Guardar .comb). Opera sobre el estado compartido <see cref="AppState"/>.
/// </summary>
public partial class MainPageViewModel : ObservableObject
{
    private readonly AppState _estado = AppState.Instancia;

    /// <summary>
    /// Callback que la Page enlaza para navegar por el <c>Frame</c> (la VM no conoce el Frame).
    /// </summary>
    public Action<Type>? Navegar { get; set; }

    /// <summary>Referencia al boleto base (la fija la Page tras InitializeComponent).</summary>
    public BoletoBaseViewModel? Boleto { get; set; }

    public AppState Estado => _estado;

    /// <summary>Condiciones que mapean a un IFiltro del grupo (botón + semáforo en MainForm).</summary>
    public ObservableCollection<CondicionItem> Condiciones { get; } = new();

    /// <summary>Columna izquierda de la rejilla de condiciones (orden del MainForm original).</summary>
    public ObservableCollection<CondicionItem> CondicionesIzquierda { get; } = new();

    /// <summary>Columna derecha de la rejilla de condiciones (orden del MainForm original).</summary>
    public ObservableCollection<CondicionItem> CondicionesDerecha { get; } = new();

    // ---- Estado de grupo en pantalla ----

    [ObservableProperty]
    private string _tituloGrupo = "Boleto Base";

    [ObservableProperty]
    private string _infoGrupos = "1 grupo";

    [ObservableProperty]
    private bool _esBoletoBase = true;

    // ---- Semáforos de las condiciones especiales (If-Then, Control de grupos) ----

    [ObservableProperty]
    private EstadoSemaforo _estadoIfThen = EstadoSemaforo.Neutro;

    [ObservableProperty]
    private EstadoSemaforo _estadoControlGrupos = EstadoSemaforo.Neutro;

    // ---- Filtro de columnas general ----

    [ObservableProperty]
    private string _nombreFiltro = "(sin filtro)";

    [ObservableProperty]
    private EstadoSemaforo _estadoFiltro = EstadoSemaforo.Neutro;

    [ObservableProperty]
    private bool _tieneFiltro;

    // ---- Filtro parcial del grupo (gbFiltroParcial del original) ----

    [ObservableProperty]
    private string _nombreFiltroParcial = "(Selecciona)";

    [ObservableProperty]
    private EstadoSemaforo _estadoFiltroParcial = EstadoSemaforo.Neutro;

    // ---- Combinación ----

    [ObservableProperty]
    private string _nombreCombinacion = "(combinación nueva)";

    public MainPageViewModel()
    {
        ConstruirCondiciones();
        _estado.Cambiado += (_, _) => RefrescarPantalla();
    }

    /// <summary>
    /// Construye las condiciones que mapean a un IFiltro del grupo. El orden replica el de
    /// <c>MainForm.ActualizaGrupoPantalla()</c>. Cada una navega a su página portada (que en M3
    /// recibe el Grupo vía <c>AppState.GrupoEnEdicion</c>).
    /// </summary>
    private void ConstruirCondiciones()
    {
        // Columna izquierda (X=21 en MainForm.Designer), de arriba a abajo:
        var noVariantes   = new CondicionItem("Variantes X y 2", Filtro.NoVariantes.ToString(), typeof(NoVariantesFrmPage), "");
        var signosSeg     = new CondicionItem("Signos Seguidos", Filtro.SignosSeguidos.ToString(), typeof(SignosSeguidosFrmPage), "");
        var dibujos       = new CondicionItem("Dibujos", Filtro.Dibujos.ToString(), typeof(DibujosFrmPage), "");
        var interrup      = new CondicionItem("Interrupciones", Filtro.NoInterrupciones.ToString(), typeof(InterrupcionesFrmPage), "");
        var gruposEquipos = new CondicionItem("Grupos Equipos", Filtro.GruposEquipos.ToString(), typeof(GruposEquiposFrmPage), "");
        var formatos      = new CondicionItem("Formatos", Filtro.FormatosSignos.ToString(), typeof(FormatosFrmPage), "");
        var simetrias     = new CondicionItem("Simetrías", Filtro.Simetrias.ToString(), typeof(SimetriasFrmPage), "");

        // Columna derecha (X=182 en MainForm.Designer), de arriba a abajo:
        var pesos         = new CondicionItem("Pesos Numéricos", Filtro.PesosNumericos.ToString(), typeof(PesosNumFrmPage), "");
        var valoracion    = new CondicionItem("Valoración", Filtro.ValoracionSignos.ToString(), typeof(ValoracionFrmPage), "");
        var colProbables  = new CondicionItem("Columnas Probables", Filtro.ColProbables.ToString(), typeof(ColProbablesFrmPage), "");
        var distancias    = new CondicionItem("Distancias", Filtro.Distancias.ToString(), typeof(DistanciasFrmPage), "");
        var contactos     = new CondicionItem("Contactos", Filtro.Contactos.ToString(), typeof(ContactosFrmPage), "");
        var formatos123   = new CondicionItem("Formatos 123", Filtro.Formatos123.ToString(), typeof(Formatos123FrmPage), "");
        var diferencias   = new CondicionItem("Diferencias", Filtro.Diferencias.ToString(), typeof(DiferenciasFrmPage), "");

        // Columnas para la rejilla (orden exacto del original).
        CondicionesIzquierda.Add(noVariantes);
        CondicionesIzquierda.Add(signosSeg);
        CondicionesIzquierda.Add(dibujos);
        CondicionesIzquierda.Add(interrup);
        CondicionesIzquierda.Add(gruposEquipos);
        CondicionesIzquierda.Add(formatos);
        CondicionesIzquierda.Add(simetrias);

        CondicionesDerecha.Add(pesos);
        CondicionesDerecha.Add(valoracion);
        CondicionesDerecha.Add(colProbables);
        CondicionesDerecha.Add(distancias);
        CondicionesDerecha.Add(contactos);
        CondicionesDerecha.Add(formatos123);
        CondicionesDerecha.Add(diferencias);

        // Lista plana: la usa RefrescarPantalla() para recalcular semáforos de todas.
        foreach (var c in CondicionesIzquierda) Condiciones.Add(c);
        foreach (var c in CondicionesDerecha) Condiciones.Add(c);
    }

    /// <summary>
    /// Refresca toda la pantalla a partir del estado del motor. Réplica de
    /// <c>MainForm.ActualizaGrupoPantalla()</c> (título, semáforos de condiciones, If-Then,
    /// control de grupos) + estado de filtro y combinación.
    /// </summary>
    public void RefrescarPantalla()
    {
        Boleto?.CargarDesdeMotor();

        var analizador = _estado.Analizador;
        int grupoPantalla = _estado.GrupoPantalla;
        Grupo grupo = _estado.GrupoActual;

        EsBoletoBase = grupoPantalla == 0;
        TituloGrupo = grupoPantalla == 0
            ? "Boleto Base"
            : $"Grupo {grupoPantalla} / {analizador.GruposPartidos.Count - 1}";

        // Sincroniza el título de la cabecera salmón del boleto (Pronosticos.lblTitulo).
        if (Boleto != null)
            Boleto.Titulo = TituloGrupo;
        InfoGrupos = analizador.GruposPartidos.Count == 1
            ? "1 grupo"
            : $"{analizador.GruposPartidos.Count} grupos";

        // Semáforos de las condiciones con IFiltro (PonerColorBotonCondicion).
        foreach (var c in Condiciones)
        {
            c.RefrescarDesdeGrupo(grupo);
        }

        // Control de grupos: verde si hay más de un control de grupos (MainForm: btnControlGrupos).
        EstadoControlGrupos = analizador.CtrlGrupos.ControlesGrupos.Count > 1
            ? EstadoSemaforo.Verde
            : EstadoSemaforo.Neutro;

        // If-Then: réplica de PonerColorBotonIfThen().
        if (analizador.IfThen != null && !analizador.IfThen.EsVacio)
        {
            EstadoIfThen = analizador.IfThen.EsActivo ? EstadoSemaforo.Verde : EstadoSemaforo.Rojo;
        }
        else
        {
            EstadoIfThen = EstadoSemaforo.Neutro;
        }

        // Filtro de columnas general.
        TieneFiltro = !string.IsNullOrEmpty(_estado.ArchivoFiltroCols);
        if (TieneFiltro)
        {
            NombreFiltro = Path.GetFileNameWithoutExtension(_estado.ArchivoFiltroCols);
            EstadoFiltro = File.Exists(_estado.ArchivoFiltroCols)
                ? EstadoSemaforo.Verde
                : EstadoSemaforo.Rojo;
        }
        else
        {
            NombreFiltro = "(sin filtro)";
            EstadoFiltro = EstadoSemaforo.Neutro;
        }

        // Combinación.
        NombreCombinacion = string.IsNullOrEmpty(_estado.NombreArchivoComb)
            ? "(combinación nueva)"
            : Path.GetFileNameWithoutExtension(_estado.NombreArchivoComb);

        // Filtro parcial del grupo (réplica de la sección "Indicar Filtro" de
        // ActualizaGrupoPantalla: ActivaFiltroColumnasParcial / DesactivarFiltroColumnasParcial).
        // El filtro parcial es por-grupo (grupo.ArchivoFiltroGrupo) y, en el original, solo se
        // muestra para grupos distintos del boleto base.
        if (grupoPantalla != 0 && !string.IsNullOrEmpty(grupo.ArchivoFiltroGrupo))
        {
            NombreFiltroParcial = Path.GetFileNameWithoutExtension(grupo.ArchivoFiltroGrupo);
            EstadoFiltroParcial = File.Exists(grupo.ArchivoFiltroGrupo)
                ? EstadoSemaforo.Verde
                : EstadoSemaforo.Rojo;
        }
        else
        {
            NombreFiltroParcial = "(Selecciona)";
            EstadoFiltroParcial = EstadoSemaforo.Neutro;
        }
    }

    // ============================================================
    //  Condiciones — navegación con handoff del Grupo (BtnXxxClick).
    // ============================================================

    /// <summary>
    /// Abre la página de una condición pasándole el Grupo actual vía el handoff estático
    /// (<c>AppState.GrupoEnEdicion</c>). Equivale, en MainForm, a
    /// <c>new XxxFrm(analizador.GruposPartidos[grupoPantalla]).ShowDialog()</c>.
    /// </summary>
    [RelayCommand]
    private void AbrirCondicion(CondicionItem? condicion)
    {
        if (condicion is null) return;
        AppState.GrupoEnEdicion = _estado.GrupoActual;
        Navegar?.Invoke(condicion.Pagina);
    }

    /// <summary>Abre la página If-Then (MainForm.btnIfThen_Click).</summary>
    [RelayCommand]
    private void AbrirIfThen()
    {
        AppState.GrupoEnEdicion = _estado.GrupoActual;
        Navegar?.Invoke(typeof(IfThenFrmPage));
    }

    /// <summary>Abre la página de Control de grupos (MainForm.BtnControlGruposClick).</summary>
    [RelayCommand]
    private void AbrirControlGrupos()
    {
        AppState.GrupoEnEdicion = _estado.GrupoActual;
        Navegar?.Invoke(typeof(ControlGruposFrmPage));
    }

    /// <summary>
    /// Tolerancias del grupo (MainForm.BtnTolGrupoClick, Free1X2/UI/MainForm.cs ~1358-1363):
    /// el original abre <c>new ControlTolFrm(analizador.GruposPartidos[grupoPantalla].ControladorTolerancias)</c>.
    /// Aquí navegamos a la página portada <see cref="ControlTolFrmPage"/>, que ya edita el
    /// <c>ControladorTol</c> del grupo actual (lee <c>AppState.GrupoActual.ControladorTolerancias</c>
    /// en OnNavigatedTo). Pasamos el grupo por el handoff estático para mantener el patrón.
    /// </summary>
    [RelayCommand]
    private void AbrirTolerancias()
    {
        AppState.GrupoEnEdicion = _estado.GrupoActual;
        Navegar?.Invoke(typeof(ControlTolFrmPage));
    }

    /// <summary>
    /// Filtro parcial del grupo (MainForm.btnAbreFiltroParcial_Click, Free1X2/UI/MainForm.cs
    /// ~1808-1840). El filtro elegido se aplica SOLO al grupo en pantalla
    /// (<c>grupo.ArchivoFiltroGrupo</c>). Réplica fiel:
    ///   - Si el botón estaba "abierto" (ya hay filtro parcial) → DesactivarFiltroColumnasParcial:
    ///     limpia el archivo del grupo.
    ///   - Si no → OpenFileDialog (.txt) + ActivaFiltroColumnasParcial: valida que el filtro tenga
    ///     EXACTAMENTE <c>VariablesGlobales.NumeroPartidos</c> signos (no más, no distinto) y, si es
    ///     válido, asigna <c>grupo.ArchivoFiltroGrupo</c>.
    /// El filtro parcial no aplica al boleto base (grupo 0), igual que el original (la sección
    /// "Indicar Filtro" de ActualizaGrupoPantalla se salta el grupo 0).
    /// </summary>
    [RelayCommand]
    private async Task AbrirFiltroParcialAsync()
    {
        Grupo grupo = _estado.GrupoActual;

        // El filtro parcial es por-grupo y el original no lo expone para el boleto base.
        if (_estado.GrupoPantalla == 0)
        {
            Free1X2.Abstractions.UserDialogs.ShowError(
                "El Filtro Parcial se aplica a un grupo. Selecciona o crea un grupo distinto del boleto base.");
            return;
        }

        // Toggle: si ya hay filtro parcial, desactivarlo (DesactivarFiltroColumnasParcial).
        if (!string.IsNullOrEmpty(grupo.ArchivoFiltroGrupo))
        {
            grupo.ArchivoFiltroGrupo = "";
            grupo.ReinicializaVariablesFiltroParcial();
            RefrescarPantalla();
            return;
        }

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        // ActivaFiltroColumnasParcial: el filtro parcial debe tener EXACTAMENTE NumeroPartidos signos.
        try
        {
            IArchivoColumnas cols = new ArchivoColumnasTexto(file.Path);
            int signos = cols.ObtenNumSignos();
            if (signos != VariablesGlobales.NumeroPartidos)
            {
                Free1X2.Abstractions.UserDialogs.ShowError(
                    "El Filtro Parcial para los grupos debe tener " +
                    VariablesGlobales.NumeroPartidos + " partidos.");
                return;
            }
        }
        catch
        {
            Free1X2.Abstractions.UserDialogs.ShowError("No se pudo leer el archivo de filtro parcial.");
            return;
        }

        // Indicar al grupo cuál es su filtro (igual que btnAbreFiltroParcial_Click).
        grupo.ArchivoFiltroGrupo = file.Path;
        grupo.ReinicializaVariablesFiltroParcial();
        RefrescarPantalla(); // actualiza nombre + semáforo de la tarjeta "Filtro Parcial del Grupo"
    }

    // ============================================================
    //  Navegación de grupos (botones de grupo de MainForm).
    // ============================================================

    /// <summary>Crea/avanza al grupo siguiente; si no existe, lo crea (CambiaGrupoSeleccionado).</summary>
    [RelayCommand]
    private void GrupoSiguiente()
    {
        Boleto?.VolcarPronosticosAlMotor();
        CambiarGrupo(_estado.GrupoPantalla + 1);
    }

    /// <summary>Retrocede al grupo anterior (BtnGrupoPrevClick).</summary>
    [RelayCommand]
    private void GrupoAnterior() => CambiarGrupo(_estado.GrupoPantalla - 1);

    /// <summary>Va al primer grupo / boleto base (btnGrupoInicio_Click).</summary>
    [RelayCommand]
    private void GrupoInicio() => CambiarGrupo(0);

    /// <summary>Va al último grupo (btnGrupoFin_Click).</summary>
    [RelayCommand]
    private void GrupoFin() => CambiarGrupo(_estado.Analizador.GruposPartidos.Count - 1);

    /// <summary>
    /// Elimina el grupo en pantalla (mEliminarGrupos → BorraGrupoEnPantalla). No permite borrar
    /// el boleto base (grupo 0). Réplica fiel de <c>MainForm.BorraGrupoEnPantalla</c>
    /// (Free1X2/UI/MainForm.cs ~1006-1052): borra el grupo y reindexa los grupos controlados de
    /// cada control de grupos (los mayores que el borrado bajan una posición; el igual al borrado
    /// se descarta).
    /// </summary>
    [RelayCommand]
    private void EliminarGrupo()
    {
        if (_estado.GrupoPantalla == 0) return; // no se borra el boleto base
        var analizador = _estado.Analizador;
        var grupos = analizador.GruposPartidos;
        if (grupos.Count <= 1) return;

        int grupoPantalla = _estado.GrupoPantalla;

        // Borrar el grupo del motor (en WinUI no hay pronosticos.grupoPronosticos: el boleto
        // recarga desde el motor al refrescar la pantalla).
        grupos.BorrarGrupo(grupoPantalla);

        // Reindexar los grupos controlados de cada control de grupos (BorraGrupoEnPantalla):
        // los grupos > grupoPantalla bajan una posición; el grupo == grupoPantalla se descarta.
        char[] separadores = new char[] { ',', '-' };
        foreach (var control in analizador.CtrlGrupos.ControlesGrupos)
        {
            string[] gControlados = control.ObtenGruposControlados().Split(separadores);
            int[] gControladosInt = control.GruposControlados;
            string temp = "";
            for (int j = 0; j < gControlados.Length; j++)
            {
                if (gControladosInt[j] > grupoPantalla)
                {
                    temp += (gControladosInt[j] - 1);
                    temp += ",";
                }
                else if (gControladosInt[j] < grupoPantalla)
                {
                    temp += gControladosInt[j];
                    temp += ",";
                }
            }
            if (temp.Length > 1)
            {
                string def = temp.Substring(0, temp.Length - 1);
                string[] defString = def.Split(',');
                gControladosInt = new int[defString.Length];
                for (int j = 0; j < gControladosInt.Length; j++)
                {
                    gControladosInt[j] = Convert.ToInt32(defString[j]);
                }
                control.PonerGruposControlados(def);
                control.GruposControlados = gControladosInt;
            }
        }

        CambiarGrupo(grupoPantalla - 1);
    }

    /// <summary>
    /// Cambia el grupo en pantalla, creándolo si no existe (réplica de
    /// <c>MainForm.CambiaGrupoSeleccionado</c>, parte de dominio). Volca antes los pronósticos.
    /// </summary>
    private void CambiarGrupo(int destino)
    {
        var grupos = _estado.Analizador.GruposPartidos;
        if (destino < 0) destino = 0;

        // Crea el grupo si el destino supera el último existente (igual que CambiaGrupoSeleccionado).
        if (destino > grupos.Count - 1)
        {
            var nuevo = new Grupo();
            grupos.AddGrupo(nuevo);
        }
        _estado.GrupoPantalla = destino; // dispara Cambiado → RefrescarPantalla
    }

    // ============================================================
    //  Filtro de columnas general (BtnAddFiltroColsClick / ActivaFiltroColumnas).
    // ============================================================

    /// <summary>Selecciona un archivo de filtro de columnas (.txt) o lo limpia si ya hay uno.</summary>
    [RelayCommand]
    private async Task FiltroColumnasAsync()
    {
        if (TieneFiltro)
        {
            // Desactivar (DesactivarFiltroColumnas).
            _estado.ArchivoFiltroCols = "";
            return;
        }

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        // ActivaFiltroColumnas: valida nº de signos contra VariablesGlobales.NumeroPartidos.
        try
        {
            IArchivoColumnas cols = new ArchivoColumnasTexto(file.Path);
            int signos = cols.ObtenNumSignos();
            if (signos > VariablesGlobales.NumeroPartidos)
            {
                Free1X2.Abstractions.UserDialogs.ShowError(
                    "El filtro tiene más signos que partidos del boleto.");
                return;
            }
        }
        catch
        {
            // Si no se puede leer, igualmente se asigna (el semáforo quedará en rojo si no existe).
        }

        _estado.ArchivoFiltroCols = file.Path; // dispara Cambiado → RefrescarPantalla
    }

    // ============================================================
    //  Acciones de combinación.
    // ============================================================

    /// <summary>
    /// Calcular columnas (MCalcular → AbreCalculoColumnasFrm). Réplica de
    /// <c>MainForm.AbreCalculoColumnasFrm</c> (Free1X2/UI/MainForm.cs ~528-550):
    /// vuelca pronósticos al motor (ObtenPronosticos/ObtenPartidosGrupos ya hechos por el boleto y la
    /// navegación de grupos), fija el archivo base si el filtro está activo y válido, y navega a la
    /// página de cálculo activando el handoff para que use el <see cref="AppState.Analizador"/>
    /// compartido (con boleto, grupos, condiciones e If-Then ya cargados).
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        Boleto?.VolcarPronosticosAlMotor();
        var analizador = _estado.Analizador;
        // AbreCalculoColumnasFrm: fija el archivo base si hay filtro de columnas válido (verde).
        analizador.ArchivoColumnasBase =
            (TieneFiltro && EstadoFiltro == EstadoSemaforo.Verde) ? _estado.ArchivoFiltroCols : "";

        // Handoff: la página de cálculo usará el Analizador compartido (no uno propio), igual que
        // MainForm pasa su analizador al constructor de CalculaColumnasFrm.
        AppState.UsarAnalizadorCompartido = true;
        Navegar?.Invoke(typeof(CalculaColumnasFrmPage));
    }

    /// <summary>Reducir (MReducir → AbreReducciones). Navega al reductor (ya cableado).</summary>
    [RelayCommand]
    private void Reducir()
    {
        Boleto?.VolcarPronosticosAlMotor();
        Navegar?.Invoke(typeof(ReductorFrmPage));
    }

    /// <summary>Nueva combinación (MNuevaComb): reinicia motor, grupo, filtro y boleto.</summary>
    [RelayCommand]
    private void NuevaCombinacion()
    {
        _estado.NuevaCombinacion();   // dispara Cambiado → RefrescarPantalla + recarga boleto
        Boleto?.VaciarEquipos();      // SetEquiposVacio()
        Boleto?.ReiniciarTriples();   // 14 triples, igual que pronosticos.Reiniciar14Triples()
    }

    /// <summary>Abrir combinación .comb (AbreCombinacion).</summary>
    [RelayCommand]
    private async Task AbrirCombinacionAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".comb");
        picker.FileTypeFilter.Add(".xml");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            var archComb = new ArchivoCombinacion();
            archComb.AbrirArchivoCombinacion(file.Path);

            string[] equipos = archComb.LeeEquipos();
            string archFiltroCols = archComb.LeeFiltroColumnas();

            var analizador = new Analizador();
            archComb.CargaControladorGrupos(analizador.CtrlGrupos);
            analizador.IfThen = archComb.CargaIfThen();

            // Pronósticos de la columna base.
            string[] pronosticos = archComb.LeePronosticos();
            for (int i = 0; i < pronosticos.Length; i++)
            {
                analizador.SetPronostico(i, pronosticos[i]);
            }

            _estado.Analizador = analizador;
            _estado.ArchivoFiltroCols = archFiltroCols ?? "";
            _estado.NombreArchivoComb = file.Path;
            _estado.GrupoPantalla = 0;

            // Vuelca equipos al boleto y refresca.
            Boleto?.SetEquipos(equipos);
            RefrescarPantalla();
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("No se pudo abrir la combinación: " + ex.Message);
        }
    }

    /// <summary>Guardar combinación .comb (GuardaCombinacion).</summary>
    [RelayCommand]
    private async Task GuardarCombinacionAsync()
    {
        Boleto?.VolcarPronosticosAlMotor();
        var analizador = _estado.Analizador;

        string destino = _estado.NombreArchivoComb;
        if (string.IsNullOrEmpty(destino))
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "Combinacion",
            };
            picker.FileTypeChoices.Add("Combinación", new List<string> { ".comb" });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;
            destino = file.Path;
        }

        try
        {
            var archComb = new ArchivoCombinacion
            {
                NombreArchivo = destino,
                Equipos = Boleto?.DevolverEquipos() ?? Array.Empty<string>(),
                Pronosticos = analizador.Pronosticos,
                ArchivoColumnasFiltro = _estado.ArchivoFiltroCols,
                Grupos = analizador.GruposPartidos,
                CtrlGrupos = analizador.CtrlGrupos,
                IfThen = analizador.IfThen,
            };
            archComb.GuardaArchivo();
            _estado.NombreArchivoComb = destino;
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("No se pudo guardar la combinación: " + ex.Message);
        }
    }

    /// <summary>
    /// Guardar combinación como… (MainForm.MGuardarCombComo, Free1X2/UI/MainForm.cs:401-415):
    /// SIEMPRE pide un archivo nuevo (SaveFileDialog) y, si se confirma, guarda en él. A diferencia
    /// de <see cref="GuardarCombinacionAsync"/>, no reutiliza el nombre actual.
    /// </summary>
    [RelayCommand]
    private async Task GuardarCombinacionComoAsync()
    {
        var picker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Combinacion",
        };
        picker.FileTypeChoices.Add("Combinación", new List<string> { ".comb" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        _estado.NombreArchivoComb = file.Path;   // fija el destino y reutiliza el guardado normal
        await GuardarCombinacionAsync();
    }

    // ============================================================
    //  Equipos del boleto base (menú Archivo: Guardar / Abrir equipos).
    // ============================================================

    /// <summary>
    /// Guardar equipos (MainForm.MGuardarPartidosClick → Pronosticos.CrearArchivoBoleto,
    /// Free1X2/UI/MainForm.cs:247-254, Free1X2/UI/Controls/Pronosticos.cs:548-570):
    /// SaveFileDialog (.txt) y escribe una línea por partido con los nombres de equipos
    /// (<c>DevolverEquipos()</c>), igual que el StreamWriter del original.
    /// </summary>
    [RelayCommand]
    private async Task GuardarEquiposAsync()
    {
        if (Boleto is null) return;
        string[] equipos = Boleto.DevolverEquipos();

        var picker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Equipos",
        };
        picker.FileTypeChoices.Add("Equipos", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            // Una línea por partido (igual que CrearArchivoBoleto: foreach equipos -> WriteLine).
            File.WriteAllLines(file.Path, equipos, System.Text.Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("No se pudieron guardar los equipos: " + ex.Message);
        }
    }

    /// <summary>
    /// Abrir equipos (MainForm.MAbreBoleto → Pronosticos.LeerBoletoBase,
    /// Free1X2/UI/MainForm.cs:238-245, Free1X2/UI/Controls/Pronosticos.cs:501-521):
    /// OpenFileDialog (.txt), lee <c>NumeroPartidos</c> líneas (las que falten -> "? - ?") y las
    /// vuelca al boleto con <c>SetEquipos()</c>, igual que el StreamReader del original.
    /// </summary>
    [RelayCommand]
    private async Task AbrirEquiposAsync()
    {
        if (Boleto is null) return;

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            int numPartidos = VariablesGlobales.NumeroPartidos;
            string[] lineas = File.ReadAllLines(file.Path);
            var partBol = new string[numPartidos];
            for (int i = 0; i < numPartidos; i++)
            {
                // Igual que LeerBoletoBase: si no hay línea para el partido, "? - ?".
                partBol[i] = (i < lineas.Length && lineas[i] != null) ? lineas[i] : "? - ?";
            }
            Boleto.SetEquipos(partBol);
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("No se pudieron abrir los equipos: " + ex.Message);
        }
    }

    // ============================================================
    //  Borrado de archivos (menú/barra Archivo: temporales e informes de error).
    // ============================================================

    /// <summary>
    /// Borrar archivos temporales (MainForm.MBorrarCombsTemp, Free1X2/UI/MainForm.cs:417-428):
    /// confirma y borra los ficheros <c>*_tmp.comb</c> de la carpeta <c>Temp/</c> (legacy:
    /// <c>Application.StartupPath + "/Temp/"</c> → aquí <c>AppContext.BaseDirectory/Temp</c>).
    /// </summary>
    [RelayCommand]
    private async Task BorrarTemporalesAsync()
    {
        if (!await AppServices.ConfirmarAsync("¿Borrar las combinaciones temporales?")) return;
        try
        {
            string dir = Path.Combine(AppContext.BaseDirectory, "Temp");
            if (!Directory.Exists(dir)) return;
            foreach (string f in Directory.GetFiles(dir))
            {
                if (Path.GetFileName(f).IndexOf("_tmp.comb", StringComparison.Ordinal) >= 0)
                    File.Delete(f);
            }
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("No se pudieron borrar los temporales: " + ex.Message);
        }
    }

    /// <summary>
    /// Borrar Informes de Error (MainForm.borrarInformesDeErrorToolStripMenuItem_Click,
    /// Free1X2/UI/MainForm.cs:430-447): confirma y borra los ficheros cuyo nombre empieza por
    /// "Informe" de la carpeta <c>Informes/</c> (legacy: <c>Application.StartupPath + "/Informes/"</c>
    /// → aquí <c>AppContext.BaseDirectory/Informes</c>).
    /// </summary>
    [RelayCommand]
    private async Task BorrarInformesAsync()
    {
        if (!await AppServices.ConfirmarAsync("¿Borrar todos los Informes generados?")) return;
        try
        {
            string dir = Path.Combine(AppContext.BaseDirectory, "Informes");
            if (!Directory.Exists(dir)) return;
            foreach (string f in Directory.GetFiles(dir))
            {
                string nombre = Path.GetFileName(f);
                if (nombre.Length > 7 && nombre.Substring(0, 7) == "Informe")
                    File.Delete(f);
            }
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("No se pudieron borrar los informes: " + ex.Message);
        }
    }

    /// <summary>
    /// Ejecuta la acción solicitada por un botón de la barra de herramientas (la MainPage la recibe
    /// como parámetro de navegación y la delega aquí). Cada token mapea a su comando, que replica el
    /// handler equivalente del menú "Archivo" del MainForm original.
    /// </summary>
    public Task EjecutarAccionAsync(AccionInicio accion)
    {
        switch (accion)
        {
            case AccionInicio.NuevaCombinacion:
                NuevaCombinacion();   // comando síncrono (réplica de MNuevaComb)
                return Task.CompletedTask;
            default:
                return EjecutarAccionAsyncCore(accion);
        }
    }

    private Task EjecutarAccionAsyncCore(AccionInicio accion) => accion switch
    {
        AccionInicio.AbrirCombinacion       => AbrirCombinacionCommand.ExecuteAsync(null),
        AccionInicio.GuardarCombinacion     => GuardarCombinacionCommand.ExecuteAsync(null),
        AccionInicio.GuardarCombinacionComo => GuardarCombinacionComoCommand.ExecuteAsync(null),
        AccionInicio.GuardarEquipos         => GuardarEquiposCommand.ExecuteAsync(null),
        AccionInicio.AbrirEquipos           => AbrirEquiposCommand.ExecuteAsync(null),
        AccionInicio.BorrarTemporales       => BorrarTemporalesCommand.ExecuteAsync(null),
        AccionInicio.BorrarInformes         => BorrarInformesCommand.ExecuteAsync(null),
        _ => Task.CompletedTask,
    };
}
