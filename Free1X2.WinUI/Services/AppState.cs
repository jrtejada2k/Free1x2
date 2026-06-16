using System;
using Free1X2.MotorCalculo;

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

    private AppState() { }

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
