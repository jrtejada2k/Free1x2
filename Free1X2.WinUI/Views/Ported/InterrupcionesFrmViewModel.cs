using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del filtro "Interrupciones" (port del WinForms <c>InterrupcionesFrm</c>).
/// Una interrupción es cada cambio de signo que se produce a lo largo de la columna.
/// El filtro mantiene dos bloques de valores admitidos (rango 0..14):
///   - Interrupciones (Global / Var / 1 / X / 2).
///   - Interrupciones seguidas (Global / Var / 1 / X / 2).
/// Cada propiedad almacena la lista de valores tal como el control legacy
/// OptionNumTol0_14 (lista separada por comas, p. ej. "0,1,2").
/// La lógica de dominio (FiltroInterrupciones, ArchivoCondiciones, CalculadorEstadisticas)
/// está marcada como TODO; aquí solo se replica el estado de pantalla.
/// </summary>
public partial class InterrupcionesFrmViewModel : ObservableObject
{
    // ===== Interrupciones (bloque superior) =====

    // Global. Equivale a filtro.GetIntGlobales()/SetNoIntGlobales().
    [ObservableProperty]
    private string _intGlobal = string.Empty;

    // Var (cualquier signo). Equivale a filtro.GetIntVar()/SetNoIntVar().
    [ObservableProperty]
    private string _intVar = string.Empty;

    // Signo 1. Equivale a filtro.GetInt1()/SetNoInt1().
    [ObservableProperty]
    private string _int1 = string.Empty;

    // Signo X. Equivale a filtro.GetIntX()/SetNoIntX().
    [ObservableProperty]
    private string _intX = string.Empty;

    // Signo 2. Equivale a filtro.GetInt2()/SetNoInt2().
    [ObservableProperty]
    private string _int2 = string.Empty;

    // ===== Interrupciones seguidas (bloque "Seguidas") =====

    // Global seguidas. Equivale a filtro.GetIntGlobalSeg()/SetNoIntGlobalSeg().
    [ObservableProperty]
    private string _segGlobal = string.Empty;

    // Var seguidas. Equivale a filtro.GetIntVarSeg()/SetNoIntVarSeg().
    [ObservableProperty]
    private string _segVar = string.Empty;

    // Signo 1 seguidas. Equivale a filtro.GetInt1Seg()/SetNoInt1Seg().
    [ObservableProperty]
    private string _seg1 = string.Empty;

    // Signo X seguidas. Equivale a filtro.GetIntXSeg()/SetNoIntXSeg().
    [ObservableProperty]
    private string _segX = string.Empty;

    // Signo 2 seguidas. Equivale a filtro.GetInt2Seg()/SetNoInt2Seg().
    [ObservableProperty]
    private string _seg2 = string.Empty;

    // El form legacy rellena los campos vacíos con "0,1,...,14" (InterrupcionesFrm.ActualizarDatos línea 489).
    private const string TodosValores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";

    // Nombre del filtro: en el enum es NoInterrupciones (el form usa la cadena "NoInterrupciones").
    private static string NombreFiltro => Filtro.NoInterrupciones.ToString();

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    // true si hay algún valor introducido (NecesitaGuardarDatos() del form legacy).
    public bool ContieneDatos =>
        !string.IsNullOrWhiteSpace(IntGlobal) ||
        !string.IsNullOrWhiteSpace(IntVar) ||
        !string.IsNullOrWhiteSpace(Int1) ||
        !string.IsNullOrWhiteSpace(IntX) ||
        !string.IsNullOrWhiteSpace(Int2) ||
        !string.IsNullOrWhiteSpace(SegGlobal) ||
        !string.IsNullOrWhiteSpace(SegVar) ||
        !string.IsNullOrWhiteSpace(Seg1) ||
        !string.IsNullOrWhiteSpace(SegX) ||
        !string.IsNullOrWhiteSpace(Seg2);

    /// <summary>
    /// Vuelca los valores del FiltroInterrupciones del grupo en edición a la pantalla.
    /// Equivale a InterrupcionesFrm.MarcarValores() (Free1X2/UI/Filtros/InterrupcionesFrm.cs líneas 90-103).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroInterrupciones)grupo.GetFiltro(NombreFiltro);
        IntGlobal = filtro.GetIntGlobales();
        IntVar = filtro.GetIntVar();
        Int1 = filtro.GetInt1();
        IntX = filtro.GetIntX();
        Int2 = filtro.GetInt2();

        SegGlobal = filtro.GetIntGlobalSeg();
        SegVar = filtro.GetIntVarSeg();
        Seg1 = filtro.GetInt1Seg();
        SegX = filtro.GetIntXSeg();
        Seg2 = filtro.GetInt2Seg();
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a InterrupcionesFrm.menuCondiciones1_BOk -> ActualizarDatos() + ActivaFiltro
        //   (Free1X2/UI/Filtros/InterrupcionesFrm.cs líneas 487-717).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroInterrupciones)grupo.GetFiltro(NombreFiltro);
        filtro.ReinicializaValores();

        if (ContieneDatos)
        {
            if (filtro.ContieneDatos == false)
            {
                filtro.IsActive = true;
            }
            filtro.ContieneDatos = true;

            filtro.SetNoIntGlobales(!string.IsNullOrWhiteSpace(IntGlobal) ? IntGlobal : TodosValores);
            filtro.SetNoIntVar(!string.IsNullOrWhiteSpace(IntVar) ? IntVar : TodosValores);
            filtro.SetNoInt1(!string.IsNullOrWhiteSpace(Int1) ? Int1 : TodosValores);
            filtro.SetNoIntX(!string.IsNullOrWhiteSpace(IntX) ? IntX : TodosValores);
            filtro.SetNoInt2(!string.IsNullOrWhiteSpace(Int2) ? Int2 : TodosValores);

            filtro.SetNoIntGlobalSeg(!string.IsNullOrWhiteSpace(SegGlobal) ? SegGlobal : TodosValores);
            filtro.SetNoIntVarSeg(!string.IsNullOrWhiteSpace(SegVar) ? SegVar : TodosValores);
            filtro.SetNoInt1Seg(!string.IsNullOrWhiteSpace(Seg1) ? Seg1 : TodosValores);
            filtro.SetNoIntXSeg(!string.IsNullOrWhiteSpace(SegX) ? SegX : TodosValores);
            filtro.SetNoInt2Seg(!string.IsNullOrWhiteSpace(Seg2) ? Seg2 : TodosValores);
        }
        else
        {
            filtro.IsActive = false;
            filtro.ContieneDatos = false;
        }

        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a menuCondiciones1_BBorrar (reinicia el FiltroInterrupciones y MarcarValores()).
        IntGlobal = string.Empty;
        IntVar = string.Empty;
        Int1 = string.Empty;
        IntX = string.Empty;
        Int2 = string.Empty;
        SegGlobal = string.Empty;
        SegVar = string.Empty;
        Seg1 = string.Empty;
        SegX = string.Empty;
        Seg2 = string.Empty;
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO: Dominio legacy — ActualizarDatos() + ArchivoCondiciones.GuardaArchivo(filtro)
        //   con extensiones *.int / *.xml (equivale a menuCondiciones1_BGuardar -> guardar()).
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO: Dominio legacy — ArchivoCondiciones.AbrirArchivoCombinacion(...) + LeeCondicion(),
        //   obtener filtro "NoInterrupciones" y volcar sus valores a estas propiedades (MarcarValores()).
        //   (equivale a menuCondiciones1_BAbrir -> abrir() del InterrupcionesFrm legacy).
    }

    [RelayCommand]
    private void Copiar()
    {
        // TODO: Dominio legacy — ActualizarDatos() y guardar a fichero temporal "Temp/tmp.int";
        //   habilita el botón Pegar (equivale a menuCondiciones1_BCopiar).
    }

    [RelayCommand]
    private void Pegar()
    {
        // TODO: Dominio legacy — abrir el fichero temporal "Temp/tmp.int" y volcar valores
        //   (equivale a menuCondiciones1_BPegar -> abrir()).
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO: Dominio legacy — construir FiltroInterrupciones temporal (ObtenerFiltroTemporal())
        //   y llamar CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, ".../Ganadoras/")
        //   y mostrar el VisorEstadisticas (equivale a menuCondiciones1_BEstadisticas).
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }
}
