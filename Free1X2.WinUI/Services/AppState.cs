// Free1X2 · WinUI 3 — WIN3
using System;
using Free1X2.MotorCalculo;
using Free1X2.Online;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Estado compartido del motor de cálculo para toda la app WinUI. Es el análogo de los
/// campos de instancia de <c>Free1X2.UI.MainForm</c> (WinForms):
///   - <see cref="Analizador"/>            ↔ MainForm.analizador
///   - <see cref="GrupoPantalla"/>          ↔ MainForm.grupoPantalla
///   - <see cref="NombreArchivoComb"/>      ↔ MainForm.nombreArchivoComb
///   - <see cref="ArchivoFiltroCols"/>      ↔ MainForm.archivoFiltroCols
///
/// Todas las páginas (boleto, condiciones, grupos, filtros) leen/escriben el MISMO
/// <see cref="Analizador"/>, de modo que el estado (boleto base, grupos y condiciones)
/// es coherente entre pantallas. Cuando una página de filtro edita un <see cref="Grupo"/>,
/// se publica <see cref="Cambiado"/> para que la MainPage refresque los semáforos de
/// las condiciones (equivalente a MainForm.ActualizaGrupoPantalla tras cerrar un diálogo).
///
/// Singleton de proceso: una sola ventana, igual que MainForm.
/// </summary>
public sealed class AppState
{
    private static readonly Lazy<AppState> _instancia = new(() => new AppState());

    /// <summary>Instancia única compartida.</summary>
    public static AppState Instancia => _instancia.Value;

    private Analizador _analizador = new();
    private int _grupoPantalla;
    private string _nombreArchivoComb = "";
    private string _archivoFiltroCols = "";
    private JornadaQuiniela? _jornadaActual;

    private AppState() { }

    /// <summary>
    /// Jornada de la Quiniela descargada del servicio online (clubprogol.com), o <c>null</c>
    /// si aún no se ha descargado ninguna (modo offline/manual). Es la FUENTE COMPARTIDA de
    /// los nombres reales de los equipos: el boleto (Controls/BoletoViewModel) y la pantalla
    /// "Grupos de Equipos" (GruposEquiposFrmViewModel) leen de aquí los equipos local/visitante
    /// cuando hay jornada cargada, y caen a los nombres por defecto/muestra cuando es null.
    /// La fija <c>DescargaBoletoFrmViewModel</c> tras una descarga online correcta.
    /// </summary>
    public JornadaQuiniela? JornadaActual
    {
        get => _jornadaActual;
        set
        {
            _jornadaActual = value;
            JornadaCambiada?.Invoke(this, EventArgs.Empty);
            NotificarCambio();
        }
    }

    /// <summary>
    /// Se dispara cuando cambia <see cref="JornadaActual"/> (descarga online correcta). Las
    /// vistas que muestran nombres de equipo (boleto, Grupos de Equipos) se suscriben para
    /// refrescar los nombres reales sin recrearse.
    /// </summary>
    public event EventHandler? JornadaCambiada;

    /// <summary>
    /// Grupo que una página de filtro está editando, entregado por la MainPage al navegar.
    /// Sigue el patrón de "handoff" estático de la app (cf.
    /// <c>VisorAnalisisColumnasFrmViewModel.UltimoGrupo</c>): la página de filtro lo lee en
    /// su constructor/OnNavigatedTo para operar sobre el <see cref="Grupo"/> real del motor.
    /// Es el análogo del <c>analizador.GruposPartidos[grupoPantalla]</c> que MainForm pasa al
    /// constructor de cada Frm de condición.
    /// </summary>
    public static Grupo? GrupoEnEdicion { get; set; }

    /// <summary>
    /// Handoff para el cálculo de columnas: cuando la MainPage navega a "Calcular columnas",
    /// pone esta bandera a <c>true</c> para que <c>CalculaColumnasFrmViewModel</c> use el
    /// <see cref="Analizador"/> compartido (que ya lleva el boleto, grupos, condiciones e
    /// If-Then) en lugar de crear uno propio. Equivale a que <c>MainForm.AbreCalculoColumnasFrm</c>
    /// pase su <c>analizador</c> al constructor de <c>CalculaColumnasFrm</c>. La página de cálculo
    /// la consume en su constructor y la reinicia a <c>false</c>, de modo que si se abre de forma
    /// independiente desde la navegación seguirá creando su propio Analizador editable.
    /// </summary>
    public static bool UsarAnalizadorCompartido { get; set; }

    /// <summary>
    /// Se dispara cuando cambia algo del estado del motor (combinación nueva/abierta,
    /// cambio de grupo, edición de filtros, filtro de columnas). La MainPage se suscribe
    /// para refrescar el boleto y los semáforos de condiciones.
    /// </summary>
    public event EventHandler? Cambiado;

    /// <summary>Instancia viva del motor de cálculo (MainForm.analizador).</summary>
    public Analizador Analizador
    {
        get => _analizador;
        set
        {
            _analizador = value;
            NotificarCambio();
        }
    }

    /// <summary>Índice del grupo actualmente en pantalla (MainForm.grupoPantalla).</summary>
    public int GrupoPantalla
    {
        get => _grupoPantalla;
        set
        {
            if (_grupoPantalla == value) return;
            _grupoPantalla = value;
            NotificarCambio();
        }
    }

    /// <summary>Ruta del .comb cargado/guardado (MainForm.nombreArchivoComb). "" si no hay.</summary>
    public string NombreArchivoComb
    {
        get => _nombreArchivoComb;
        set
        {
            _nombreArchivoComb = value ?? "";
            NotificarCambio();
        }
    }

    /// <summary>Ruta del archivo de filtro de columnas general (MainForm.archivoFiltroCols). "" si no hay.</summary>
    public string ArchivoFiltroCols
    {
        get => _archivoFiltroCols;
        set
        {
            _archivoFiltroCols = value ?? "";
            NotificarCambio();
        }
    }

    /// <summary>Grupo actualmente en pantalla, o el boleto base si el índice se ha quedado fuera de rango.</summary>
    public Grupo GrupoActual
    {
        get
        {
            var grupos = _analizador.GruposPartidos;
            if (_grupoPantalla < 0 || _grupoPantalla >= grupos.Count)
            {
                _grupoPantalla = 0;
            }
            return grupos[_grupoPantalla];
        }
    }

    /// <summary>
    /// Crea una combinación nueva: reinicia el Analizador, el grupo en pantalla y las rutas.
    /// Equivale a MainForm.MNuevaComb (parte de dominio).
    /// </summary>
    public void NuevaCombinacion()
    {
        _analizador = new Analizador();
        _grupoPantalla = 0;
        _nombreArchivoComb = "";
        _archivoFiltroCols = "";
        NotificarCambio();
    }

    /// <summary>Publica <see cref="Cambiado"/>. Llamar tras mutar el motor desde otra página.</summary>
    public void NotificarCambio() => Cambiado?.Invoke(this, EventArgs.Empty);
}
