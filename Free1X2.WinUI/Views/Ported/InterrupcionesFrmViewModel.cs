using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

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

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    // Fichero temporal de copiar/pegar (legacy: StartupPath + "/Temp/tmp.int").
    private static string RutaTemporal =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.int");

    // Directorio de columnas ganadoras (legacy: StartupPath + "/Ganadoras/").
    private static string DirectorioGanadoras =>
        Path.Combine(AppContext.BaseDirectory, "Ganadoras") + Path.DirectorySeparatorChar;

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

    /// <summary>
    /// Construye un FiltroInterrupciones temporal con los valores de pantalla.
    /// Réplica de InterrupcionesFrm.ObtenerFiltroTemporal() (InterrupcionesFrm.cs líneas 598-...).
    /// </summary>
    private FiltroInterrupciones ObtenerFiltroTemporal()
    {
        var filtroTemp = new FiltroInterrupciones();
        filtroTemp.ReinicializaValores();

        if (ContieneDatos)
        {
            if (filtroTemp.ContieneDatos == false)
            {
                filtroTemp.IsActive = true;
            }
            filtroTemp.ContieneDatos = true;

            filtroTemp.SetNoIntGlobales(!string.IsNullOrWhiteSpace(IntGlobal) ? IntGlobal : TodosValores);
            filtroTemp.SetNoIntVar(!string.IsNullOrWhiteSpace(IntVar) ? IntVar : TodosValores);
            filtroTemp.SetNoInt1(!string.IsNullOrWhiteSpace(Int1) ? Int1 : TodosValores);
            filtroTemp.SetNoIntX(!string.IsNullOrWhiteSpace(IntX) ? IntX : TodosValores);
            filtroTemp.SetNoInt2(!string.IsNullOrWhiteSpace(Int2) ? Int2 : TodosValores);

            filtroTemp.SetNoIntGlobalSeg(!string.IsNullOrWhiteSpace(SegGlobal) ? SegGlobal : TodosValores);
            filtroTemp.SetNoIntVarSeg(!string.IsNullOrWhiteSpace(SegVar) ? SegVar : TodosValores);
            filtroTemp.SetNoInt1Seg(!string.IsNullOrWhiteSpace(Seg1) ? Seg1 : TodosValores);
            filtroTemp.SetNoIntXSeg(!string.IsNullOrWhiteSpace(SegX) ? SegX : TodosValores);
            filtroTemp.SetNoInt2Seg(!string.IsNullOrWhiteSpace(Seg2) ? Seg2 : TodosValores);
        }
        else
        {
            filtroTemp.IsActive = false;
            filtroTemp.ContieneDatos = false;
        }
        return filtroTemp;
    }

    // Vuelca un FiltroInterrupciones a las propiedades de pantalla (MarcarValores legacy, líneas 90-103).
    private void MarcarValores(FiltroInterrupciones filtro)
    {
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

    // Guarda en disco el filtro temporal (InterrupcionesFrm.guardar(), líneas 762-766).
    private void GuardarEn(string nombreArchivo)
    {
        var filtroTemp = ObtenerFiltroTemporal();
        var archComb = new ArchivoCondiciones { NombreArchivo = nombreArchivo };
        archComb.GuardaArchivo(filtroTemp);
    }

    // Abre la condición desde disco y vuelca sus valores (InterrupcionesFrm.abrir(), líneas 750-758).
    private void AbrirDesde(string nombreArchivo)
    {
        var archComb = new ArchivoCondiciones();
        if (archComb.AbrirArchivoCombinacion(nombreArchivo))
        {
            Grupo g = archComb.LeeCondicion();
            var filtro = (FiltroInterrupciones)g.GetFiltro("NoInterrupciones");
            MarcarValores(filtro);
        }
    }

    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a InterrupcionesFrm.menuCondiciones1_BGuardar (InterrupcionesFrm.cs líneas 740-748).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Interrupciones",
        };
        picker.FileTypeChoices.Add("Interrupciones", new List<string> { ".int" });
        picker.FileTypeChoices.Add("Interrupciones (XML)", new List<string> { ".xml" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            GuardarEn(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo guardar: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task Abrir()
    {
        // Equivale a InterrupcionesFrm.menuCondiciones1_BAbrir (InterrupcionesFrm.cs líneas 729-738).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".int");
        picker.FileTypeFilter.Add(".xml");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            AbrirDesde(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo abrir: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Copiar()
    {
        // Equivale a InterrupcionesFrm.menuCondiciones1_BCopiar (InterrupcionesFrm.cs líneas ~785).
        try
        {
            string ruta = RutaTemporal;
            Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
            GuardarEn(ruta);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo copiar: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Pegar()
    {
        // Equivale a InterrupcionesFrm.menuCondiciones1_BPegar (InterrupcionesFrm.cs líneas ~798).
        if (File.Exists(RutaTemporal))
        {
            AbrirDesde(RutaTemporal);
        }
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // Equivale a InterrupcionesFrm.menuCondiciones1_BEstadisticas (InterrupcionesFrm.cs líneas 819-829).
        var filtroTemp = ObtenerFiltroTemporal();
        var calc = new CalculadorEstadisticas();
        List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, DirectorioGanadoras);

        VisorEstadisticasViewModel.UltimasEstadisticas = lista;
        Navegar?.Invoke(typeof(VisorEstadisticasPage));
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }
}
