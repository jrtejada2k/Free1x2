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
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del filtro "Distancias".
/// La distancia es el número máximo de partidos que separan dos signos iguales.
/// El filtro mantiene cuatro juegos de valores: Var (cualquier signo), 1, X y 2.
/// Cada propiedad almacena los valores admitidos (rango 0..15) tal como el control
/// legacy OptionNumTol0_14 (lista de tolerancias).
/// </summary>
public partial class DistanciasFrmViewModel : ObservableObject
{
    // Distancias para "Var" (cualquier signo). Equivale a filtro.GetIntVar()/SetNoIntVar().
    [ObservableProperty]
    private string _distanciasVar = string.Empty;

    // Distancias para el signo "1". Equivale a filtro.GetInt1()/SetNoInt1().
    [ObservableProperty]
    private string _distancias1 = string.Empty;

    // Distancias para el signo "X". Equivale a filtro.GetIntX()/SetNoIntX().
    [ObservableProperty]
    private string _distanciasX = string.Empty;

    // Distancias para el signo "2". Equivale a filtro.GetInt2()/SetNoInt2().
    [ObservableProperty]
    private string _distancias2 = string.Empty;

    /// <summary>
    /// Acción para volver a la pantalla anterior (la cablea la página con Frame.GoBack()).
    /// Equivale a CerrarVentana() del form legacy.
    /// </summary>
    public Action? Volver { get; set; }

    /// <summary>
    /// Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).
    /// Se usa para abrir el visor de estadísticas (mismo patrón que MainPage.Navegar).
    /// </summary>
    public Action<Type>? Navegar { get; set; }

    // Fichero temporal de copiar/pegar (legacy: StartupPath + "/Temp/tmp.dist").
    private static string RutaTemporal =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.dist");

    // Directorio de columnas ganadoras (legacy: StartupPath + "/Ganadoras/").
    private static string DirectorioGanadoras =>
        Path.Combine(AppContext.BaseDirectory, "Ganadoras") + Path.DirectorySeparatorChar;

    // true si hay algún valor introducido (control NecesitaGuardarDatos() del form legacy).
    public bool ContieneDatos =>
        !string.IsNullOrWhiteSpace(DistanciasVar) ||
        !string.IsNullOrWhiteSpace(Distancias1) ||
        !string.IsNullOrWhiteSpace(DistanciasX) ||
        !string.IsNullOrWhiteSpace(Distancias2);

    /// <summary>
    /// Vuelca los valores actuales del FiltroDistancias del grupo en edición a la pantalla.
    /// Equivale a DistanciasFrm.MarcarValores() (ctor del form legacy).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroDistancias)grupo.GetFiltro(Filtro.Distancias.ToString());
        DistanciasVar = filtro.GetIntVar();
        Distancias1 = filtro.GetInt1();
        DistanciasX = filtro.GetIntX();
        Distancias2 = filtro.GetInt2();
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a DistanciasFrm.menuCondiciones1_BOk -> guardarValores() + ActivaFiltro
        //   (Free1X2/UI/Filtros/DistanciasFrm.cs líneas 313-439).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroDistancias)grupo.GetFiltro(Filtro.Distancias.ToString());
        string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();

        filtro.ReinicializaValores();
        if (ContieneDatos)
        {
            if (filtro.ContieneDatos == false)
            {
                // Primera vez guardando datos: activar la condición.
                filtro.IsActive = true;
            }
            filtro.ContieneDatos = true;

            // Cada juego de valores: si está vacío, se rellena con "todos" (replica del legacy).
            filtro.SetNoIntVar(!string.IsNullOrWhiteSpace(DistanciasVar) ? DistanciasVar : todosValores);
            filtro.SetNoInt1(!string.IsNullOrWhiteSpace(Distancias1) ? Distancias1 : todosValores);
            filtro.SetNoIntX(!string.IsNullOrWhiteSpace(DistanciasX) ? DistanciasX : todosValores);
            filtro.SetNoInt2(!string.IsNullOrWhiteSpace(Distancias2) ? Distancias2 : todosValores);
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
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a borrarValores() del form legacy.
        DistanciasVar = string.Empty;
        Distancias1 = string.Empty;
        DistanciasX = string.Empty;
        Distancias2 = string.Empty;
    }

    /// <summary>
    /// Construye un FiltroDistancias temporal con los valores de pantalla.
    /// Réplica de DistanciasFrm.ObtenerFiltroTemporal() (DistanciasFrm.cs líneas 370-428).
    /// </summary>
    private FiltroDistancias ObtenerFiltroTemporal()
    {
        var filtroTemp = new FiltroDistancias();
        string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();

        filtroTemp.ReinicializaValores();
        if (ContieneDatos)
        {
            if (filtroTemp.ContieneDatos == false)
            {
                filtroTemp.IsActive = true;
            }
            filtroTemp.ContieneDatos = true;
            filtroTemp.SetNoIntVar(!string.IsNullOrWhiteSpace(DistanciasVar) ? DistanciasVar : todosValores);
            filtroTemp.SetNoInt1(!string.IsNullOrWhiteSpace(Distancias1) ? Distancias1 : todosValores);
            filtroTemp.SetNoIntX(!string.IsNullOrWhiteSpace(DistanciasX) ? DistanciasX : todosValores);
            filtroTemp.SetNoInt2(!string.IsNullOrWhiteSpace(Distancias2) ? Distancias2 : todosValores);
        }
        else
        {
            filtroTemp.IsActive = false;
            filtroTemp.ContieneDatos = false;
        }
        return filtroTemp;
    }

    // Vuelca un FiltroDistancias a las cuatro propiedades de pantalla (MarcarValores legacy).
    private void MarcarValores(FiltroDistancias filtro)
    {
        DistanciasVar = filtro.GetIntVar();
        Distancias1 = filtro.GetInt1();
        DistanciasX = filtro.GetIntX();
        Distancias2 = filtro.GetInt2();
    }

    // Guarda el filtro temporal en disco (DistanciasFrm.guardar(), líneas 487-497).
    private void GuardarEn(string nombreArchivo)
    {
        var filtroTemp = ObtenerFiltroTemporal();
        var archComb = new ArchivoCondiciones { NombreArchivo = nombreArchivo };
        if (filtroTemp.NoDistancias > 0)
        {
            filtroTemp.ContieneDatos = true;
            filtroTemp.IsActive = true;
        }
        archComb.GuardaArchivo(filtroTemp);
    }

    // Abre el filtro desde disco y vuelca sus valores (DistanciasFrm.abrir(), líneas 474-485).
    private void AbrirDesde(string nombreArchivo)
    {
        var archComb = new ArchivoCondiciones();
        if (archComb.AbrirArchivoCombinacion(nombreArchivo))
        {
            var grupo = archComb.LeeCondicion();
            var filtro = (FiltroDistancias)grupo.GetFiltro("Distancias");
            MarcarValores(filtro);
        }
    }

    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a menuCondiciones1_BGuardar -> guardar() (DistanciasFrm.cs líneas 462-497).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Distancias",
        };
        picker.FileTypeChoices.Add("Distancias", new List<string> { ".dist" });
        picker.FileTypeChoices.Add("Distancias (XML)", new List<string> { ".xml" });
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
        // Equivale a menuCondiciones1_BAbrir -> abrir() (DistanciasFrm.cs líneas 446-485).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".dist");
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
        // Equivale a menuCondiciones1_BCopiar -> guardar(Temp/tmp.dist) (DistanciasFrm.cs líneas 509-518).
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
        // Equivale a menuCondiciones1_BPegar -> abrir(Temp/tmp.dist) (DistanciasFrm.cs líneas 520-531).
        if (File.Exists(RutaTemporal))
        {
            AbrirDesde(RutaTemporal);
        }
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // Equivale a menuCondiciones1_BEstadisticas (DistanciasFrm.cs líneas 546-556):
        //   ObtenerFiltroTemporal -> CalculadorEstadisticas.EstadisticasFiltro -> VisorEstadisticas.
        var filtroTemp = ObtenerFiltroTemporal();
        var calc = new CalculadorEstadisticas();
        List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, DirectorioGanadoras);

        VisorEstadisticasViewModel.UltimasEstadisticas = lista;
        Navegar?.Invoke(typeof(VisorEstadisticasPage));
    }
}
