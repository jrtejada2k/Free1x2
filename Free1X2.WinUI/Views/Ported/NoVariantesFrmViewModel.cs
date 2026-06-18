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
/// ViewModel del filtro "Número de Variantes" (WinForms <c>NoVariantesFrm</c>).
/// El filtro mantiene tres juegos de valores que indican la cantidad admitida de
/// Variantes, de signos X y de signos 2 dentro de cada combinación.
/// Cada propiedad almacena la lista de cantidades admitidas (rango 0..15) tal como
/// el control legacy <c>OptionNumTol0_14</c> (cadena de tolerancias separadas por comas).
/// </summary>
public partial class NoVariantesFrmViewModel : ObservableObject
{
    // Cantidades admitidas de "Variantes". Equivale a filtro.GetVariantes()/SetNoVariantes().
    [ObservableProperty]
    private string _variantes = string.Empty;

    // Cantidades admitidas de signos "X". Equivale a filtro.GetEquis()/SetNoEquis().
    [ObservableProperty]
    private string _equis = string.Empty;

    // Cantidades admitidas de signos "2". Equivale a filtro.GetDoses()/SetNoDoses().
    [ObservableProperty]
    private string _doses = string.Empty;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    // Fichero temporal de copiar/pegar (legacy: StartupPath + "/Temp/tmp.vx2").
    private static string RutaTemporal =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.vx2");

    // Directorio de columnas ganadoras (legacy: StartupPath + "/Ganadoras/").
    private static string DirectorioGanadoras =>
        Path.Combine(AppContext.BaseDirectory, "Ganadoras") + Path.DirectorySeparatorChar;

    // true si hay algún valor introducido (NecesitaGuardarDatos() del form legacy).
    public bool ContieneDatos =>
        !string.IsNullOrWhiteSpace(Variantes) ||
        !string.IsNullOrWhiteSpace(Equis) ||
        !string.IsNullOrWhiteSpace(Doses);

    /// <summary>
    /// Vuelca los valores del FiltroNoVariantes del grupo en edición a la pantalla.
    /// Equivale a NoVariantesFrm.MarcarValores() (ctor del form legacy).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroNoVariantes)grupo.GetFiltro(Filtro.NoVariantes.ToString());
        Variantes = filtro.GetVariantes();
        Equis = filtro.GetEquis();
        Doses = filtro.GetDoses();
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a NoVariantesFrm.menuCondiciones1_BOk -> ActualizarDatos() + ActivaFiltro
        //   (Free1X2/UI/Filtros/NoVariantesFrm.cs líneas 238-341).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroNoVariantes)grupo.GetFiltro(Filtro.NoVariantes.ToString());
        string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();

        filtro.ReinicializaValores();
        if (ContieneDatos)
        {
            if (filtro.ContieneDatos == false)
            {
                filtro.IsActive = true;
            }
            filtro.ContieneDatos = true;

            filtro.SetNoVariantes(!string.IsNullOrWhiteSpace(Variantes) ? Variantes : todosValores);
            filtro.SetNoEquis(!string.IsNullOrWhiteSpace(Equis) ? Equis : todosValores);
            filtro.SetNoDoses(!string.IsNullOrWhiteSpace(Doses) ? Doses : todosValores);
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
        // Equivale a menuCondiciones1_BBorrar -> reinstanciar FiltroNoVariantes + MarcarValores().
        Variantes = string.Empty;
        Equis = string.Empty;
        Doses = string.Empty;
    }

    /// <summary>
    /// Construye un FiltroNoVariantes temporal con los valores de pantalla.
    /// Réplica de NoVariantesFrm.ObtenerFiltroTemporal() (NoVariantesFrm.cs líneas 286-333).
    /// </summary>
    private FiltroNoVariantes ObtenerFiltroTemporal()
    {
        var filtroTemp = new FiltroNoVariantes();
        string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();

        filtroTemp.ReinicializaValores();
        if (ContieneDatos)
        {
            if (filtroTemp.ContieneDatos == false)
            {
                filtroTemp.IsActive = true;
            }
            filtroTemp.ContieneDatos = true;
            filtroTemp.SetNoVariantes(!string.IsNullOrWhiteSpace(Variantes) ? Variantes : todosValores);
            filtroTemp.SetNoEquis(!string.IsNullOrWhiteSpace(Equis) ? Equis : todosValores);
            filtroTemp.SetNoDoses(!string.IsNullOrWhiteSpace(Doses) ? Doses : todosValores);
        }
        else
        {
            filtroTemp.IsActive = false;
            filtroTemp.ContieneDatos = false;
        }
        return filtroTemp;
    }

    // Vuelca un FiltroNoVariantes a las tres propiedades de pantalla (MarcarValores legacy).
    private void MarcarValores(FiltroNoVariantes filtro)
    {
        Variantes = filtro.GetVariantes();
        Equis = filtro.GetEquis();
        Doses = filtro.GetDoses();
    }

    // Guarda en disco el filtro temporal (NoVariantesFrm.guardar(), líneas 386-391).
    private void GuardarEn(string nombreArchivo)
    {
        var filtroTemp = ObtenerFiltroTemporal();
        var archComb = new ArchivoCondiciones { NombreArchivo = nombreArchivo };
        archComb.GuardaArchivo(filtroTemp);
    }

    // Abre la condición desde disco y vuelca sus valores (NoVariantesFrm.abrir(), líneas 374-384).
    private void AbrirDesde(string nombreArchivo)
    {
        var archComb = new ArchivoCondiciones();
        if (archComb.AbrirArchivoCombinacion(nombreArchivo))
        {
            Grupo g = archComb.LeeCondicion();
            var filtro = (FiltroNoVariantes)g.GetFiltro("NoVariantes");
            MarcarValores(filtro);
        }
    }

    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a NoVariantesFrm.menuCondiciones1_BGuardar (NoVariantesFrm.cs líneas 363-372).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "CantidadSignos",
        };
        picker.FileTypeChoices.Add("Cantidad de signos V, X y 2", new List<string> { ".vx2" });
        picker.FileTypeChoices.Add("Cantidad de signos V, X y 2 (XML)", new List<string> { ".xml" });
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
        // Equivale a NoVariantesFrm.menuCondiciones1_BAbrir (NoVariantesFrm.cs líneas 348-361).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".vx2");
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
        // Equivale a NoVariantesFrm.menuCondiciones1_BCopiar (NoVariantesFrm.cs líneas 409-417).
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
        // Equivale a NoVariantesFrm.menuCondiciones1_BPegar (NoVariantesFrm.cs líneas 419-429).
        if (File.Exists(RutaTemporal))
        {
            AbrirDesde(RutaTemporal);
        }
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // Equivale a NoVariantesFrm.menuCondiciones1_BEstadisticas (NoVariantesFrm.cs líneas 440-450).
        var filtroTemp = ObtenerFiltroTemporal();
        var calc = new CalculadorEstadisticas();
        List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, DirectorioGanadoras);

        VisorEstadisticasViewModel.UltimasEstadisticas = lista;
        Navegar?.Invoke(typeof(VisorEstadisticasPage));
    }
}
