using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del formulario legacy WinForms "ConfiguracionAnalisisFrm" (Configurar Análisis).
/// Controla qué tipos de análisis estadístico se ejecutan sobre los históricos de quiniela.
///
/// Dependencias entre opciones (replican la lógica del form legacy):
///   - <see cref="FigurasV1X2"/> sólo disponible si <see cref="Seguidos"/> está activo.
///   - <see cref="FigurasContactos"/> sólo disponible si <see cref="Contactos"/> está activo.
///   - <see cref="FigurasPesos"/> sólo disponible si <see cref="Pesos"/> está activo.
/// </summary>
public partial class ConfiguracionAnalisisFrmViewModel : ObservableObject
{
    // Bandera para evitar recursión al marcar/desmarcar en bloque (Todo / Nada).
    private bool _actualizandoEnBloque;

    [ObservableProperty]
    private bool _vX2;

    [ObservableProperty]
    private bool _seguidos;

    [ObservableProperty]
    private bool _figurasV1X2;

    [ObservableProperty]
    private bool _figurasV1X2Habilitado;

    [ObservableProperty]
    private bool _dibujos;

    [ObservableProperty]
    private bool _interrupciones;

    [ObservableProperty]
    private bool _gruposEquipos;

    [ObservableProperty]
    private bool _formatos;

    [ObservableProperty]
    private bool _simetrias;

    [ObservableProperty]
    private bool _diferencias;

    [ObservableProperty]
    private bool _pesos;

    [ObservableProperty]
    private bool _figurasPesos;

    [ObservableProperty]
    private bool _figurasPesosHabilitado;

    [ObservableProperty]
    private bool _valoracion;

    [ObservableProperty]
    private bool _columnasProbables;

    [ObservableProperty]
    private bool _distancias;

    [ObservableProperty]
    private bool _contactos;

    [ObservableProperty]
    private bool _figurasContactos;

    [ObservableProperty]
    private bool _figurasContactosHabilitado;

    [ObservableProperty]
    private bool _formatos123;

    [ObservableProperty]
    private bool _controlGrupos;

    [ObservableProperty]
    private bool _controlConjuntos;

    public ConfiguracionAnalisisFrmViewModel()
    {
        CargarConfiguracion();
    }

    // ---- Dependencias entre opciones (equivalentes a los CheckedChanged del form legacy) ----

    partial void OnSeguidosChanged(bool value)
    {
        FigurasV1X2Habilitado = value;
        if (!value) FigurasV1X2 = false;
    }

    partial void OnContactosChanged(bool value)
    {
        FigurasContactosHabilitado = value;
        if (!value) FigurasContactos = false;
    }

    partial void OnPesosChanged(bool value)
    {
        FigurasPesosHabilitado = value;
        if (!value) FigurasPesos = false;
    }

    /// <summary>Marca todas las opciones de análisis (botón "Todo").</summary>
    [RelayCommand]
    private void MarcarTodo() => EstablecerTodo(true);

    /// <summary>Desmarca todas las opciones de análisis (botón "Nada").</summary>
    [RelayCommand]
    private void DesmarcarTodo() => EstablecerTodo(false);

    private void EstablecerTodo(bool valor)
    {
        _actualizandoEnBloque = true;
        try
        {
            VX2 = valor;
            Seguidos = valor;
            FigurasV1X2Habilitado = valor;
            FigurasV1X2 = valor;
            Dibujos = valor;
            Interrupciones = valor;
            GruposEquipos = valor;
            Formatos = valor;
            Simetrias = valor;
            Diferencias = valor;
            Pesos = valor;
            FigurasPesosHabilitado = valor;
            FigurasPesos = valor;
            Valoracion = valor;
            ColumnasProbables = valor;
            Distancias = valor;
            Contactos = valor;
            FigurasContactosHabilitado = valor;
            FigurasContactos = valor;
            Formatos123 = valor;
            ControlGrupos = valor;
            ControlConjuntos = valor;
        }
        finally
        {
            _actualizandoEnBloque = false;
        }
    }

    /// <summary>Carga la configuración persistida de análisis.</summary>
    private void CargarConfiguracion()
    {
        // TODO(dominio): cargar valores reales con
        // Free1X2.EntradaSalida.AConfiguracion.ObtenConfiguracionAnalisis(ref ...).
        // El dominio aún no está disponible en Free1X2.Domain; por ahora se dejan los valores por defecto.
    }

    /// <summary>Guarda la configuración de análisis (botón "Guardar").</summary>
    [RelayCommand]
    private void Guardar()
    {
        // TODO(dominio): persistir con
        // Free1X2.EntradaSalida.AConfiguracion.GuardarConfiguracionAnalisis(
        //     todo, VX2, Seguidos, FigurasV1X2, Interrupciones, Dibujos, Simetrias,
        //     Formatos, Formatos123, Distancias, Contactos, FigurasContactos, Pesos,
        //     FigurasPesos, Valoracion, ColumnasProbables, GruposEquipos,
        //     ControlGrupos, ControlConjuntos, Diferencias);
        // y luego Free1X2.VariablesGlobales.ReinicializarVariables().
    }
}
