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
        Condiciones.Add(new CondicionItem("No Variantes", Filtro.NoVariantes.ToString(), typeof(NoVariantesFrmPage)));
        Condiciones.Add(new CondicionItem("Signos Seguidos", Filtro.SignosSeguidos.ToString(), typeof(SignosSeguidosFrmPage)));
        Condiciones.Add(new CondicionItem("Dibujos", Filtro.Dibujos.ToString(), typeof(DibujosFrmPage)));
        Condiciones.Add(new CondicionItem("Col. Probables", Filtro.ColProbables.ToString(), typeof(ColProbablesFrmPage)));
        Condiciones.Add(new CondicionItem("Pesos Numéricos", Filtro.PesosNumericos.ToString(), typeof(PesosNumFrmPage)));
        Condiciones.Add(new CondicionItem("Valoración Signos", Filtro.ValoracionSignos.ToString(), typeof(ValoracionFrmPage)));
        Condiciones.Add(new CondicionItem("No Interrupciones", Filtro.NoInterrupciones.ToString(), typeof(InterrupcionesFrmPage)));
        Condiciones.Add(new CondicionItem("Distancias", Filtro.Distancias.ToString(), typeof(DistanciasFrmPage)));
        Condiciones.Add(new CondicionItem("Contactos", Filtro.Contactos.ToString(), typeof(ContactosFrmPage)));
        Condiciones.Add(new CondicionItem("Formatos Signos", Filtro.FormatosSignos.ToString(), typeof(FormatosFrmPage)));
        Condiciones.Add(new CondicionItem("Formatos 123", Filtro.Formatos123.ToString(), typeof(Formatos123FrmPage)));
        Condiciones.Add(new CondicionItem("Simetrías", Filtro.Simetrias.ToString(), typeof(SimetriasFrmPage)));
        Condiciones.Add(new CondicionItem("Diferencias", Filtro.Diferencias.ToString(), typeof(DiferenciasFrmPage)));
        Condiciones.Add(new CondicionItem("Grupos Equipos", Filtro.GruposEquipos.ToString(), typeof(GruposEquiposFrmPage)));
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
}
