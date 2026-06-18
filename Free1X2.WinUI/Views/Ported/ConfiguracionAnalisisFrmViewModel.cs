// Free1X2 · WinUI 3 — WIN3
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;

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

    // Estado persistido del check "Todo" del form legacy (chkTodo). No tiene control propio
    // en la Page (Marcar/Desmarcar son acciones), pero se carga y se guarda fielmente.
    private bool _aTodo;

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
    private void MarcarTodo()
    {
        EstablecerTodo(true);
        _aTodo = true;   // legacy: chkTodo.Checked = true al pulsar "Marcar todo".
    }

    /// <summary>Desmarca todas las opciones de análisis (botón "Nada").</summary>
    [RelayCommand]
    private void DesmarcarTodo()
    {
        EstablecerTodo(false);
        _aTodo = false;  // legacy: chkTodo.Checked = false al pulsar "Desmarcar todo".
    }

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

    /// <summary>
    /// Carga la configuración persistida de análisis.
    /// Réplica de ConfiguracionAnalisisFrm_Load (Free1X2/UI/ConfiguracionAnalisisFrm.cs:142-166):
    /// AConfiguracion.ObtenConfiguracionAnalisis(ref ...) -> chk*.Checked.
    /// </summary>
    private void CargarConfiguracion()
    {
        try
        {
            // AConfiguracion() usa AppContext.BaseDirectory/parametros.free1x2. Si el archivo
            // no existe (entorno de desarrollo sin datos), se conservan los valores por defecto.
            var cfg = new AConfiguracion();

            bool aTodo = false, aVX2 = false, aSeguidos = false, aFigurasV1X2 = false,
                 aInterrupciones = false, aDibujos = false, aSimetrias = false, aFormatos = false,
                 aFormatos123 = false, aDistancias = false, aContactos = false, aFigurasContactos = false,
                 aPesos = false, aFigurasPesos = false, aValoracion = false, aCPs = false,
                 aGruposEquipos = false, aControlGrupos = false, aControlConjuntos = false, aDiferencias = false;

            cfg.ObtenConfiguracionAnalisis(ref aTodo, ref aVX2, ref aSeguidos, ref aFigurasV1X2,
                ref aInterrupciones, ref aDibujos, ref aSimetrias, ref aFormatos, ref aFormatos123,
                ref aDistancias, ref aContactos, ref aFigurasContactos, ref aPesos, ref aFigurasPesos,
                ref aValoracion, ref aCPs, ref aGruposEquipos, ref aControlGrupos, ref aControlConjuntos,
                ref aDiferencias);

            // Réplica fiel del orden de asignación del form legacy. chkTodo se guarda en _aTodo.
            // Los OnXxxChanged (equivalentes a los CheckedChanged) ajustan los habilitados.
            _aTodo = aTodo;
            VX2 = aVX2;
            Seguidos = aSeguidos;
            FigurasV1X2 = aFigurasV1X2;
            Dibujos = aDibujos;
            Interrupciones = aInterrupciones;
            Valoracion = aValoracion;
            Formatos = aFormatos;
            Contactos = aContactos;
            FigurasContactos = aFigurasContactos;
            Formatos123 = aFormatos123;
            Simetrias = aSimetrias;
            Diferencias = aDiferencias;
            Distancias = aDistancias;
            Pesos = aPesos;
            FigurasPesos = aFigurasPesos;
            ColumnasProbables = aCPs;   // legacy chkCPs (Columnas probables).
            GruposEquipos = aGruposEquipos;
            ControlGrupos = aControlGrupos;
            ControlConjuntos = aControlConjuntos;
        }
        catch
        {
            // Sin archivo de configuración accesible: se mantienen los valores por defecto.
        }
    }

    /// <summary>
    /// Guarda la configuración de análisis (botón "Guardar").
    /// Réplica de btnGuardar_Click (Free1X2/UI/ConfiguracionAnalisisFrm.cs:134-140):
    /// AConfiguracion.GuardarConfiguracionAnalisis(...) + VariablesGlobales.ReinicializarVariables().
    /// </summary>
    [RelayCommand]
    private void Guardar()
    {
        try
        {
            var cfg = new AConfiguracion();
            cfg.GuardarConfiguracionAnalisis(_aTodo, VX2, Seguidos, FigurasV1X2, Interrupciones,
                Dibujos, Simetrias, Formatos, Formatos123, Distancias, Contactos, FigurasContactos,
                Pesos, FigurasPesos, Valoracion, ColumnasProbables, GruposEquipos, ControlGrupos,
                ControlConjuntos, Diferencias);

            Free1X2.VariablesGlobales.ReinicializarVariables();
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError(
                "No se pudo guardar la configuración de análisis: " + ex.Message);
        }
    }
}
