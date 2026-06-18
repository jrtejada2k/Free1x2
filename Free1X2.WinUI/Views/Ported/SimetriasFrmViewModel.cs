// Free1X2 · WinUI 3 — WIN3
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
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del filtro "Simetrías".
/// Se da una simetría entre dos o más partidos cuando comparten el mismo signo.
/// Cada simetría es una lista de partidos separados por comas (a,b), guiones (a-b)
/// o una mezcla (a,b-c). El filtro mantiene además un campo "Aciertos" (intervalos
/// individuales o rangos: "1,3,5-7"). Equivale a los CtrlSimetria + txtAciertos del
/// WinForms SimetriasFrm. Persistencia (Guardar/Abrir/Copiar/Pegar) vía ArchivoCondiciones
/// (.sim/.xml + Temp/tmp.sim) y Estadísticas vía CalculadorEstadisticas -> VisorEstadisticasPage.
/// </summary>
public partial class SimetriasFrmViewModel : ObservableObject
{
    // La fila de simetría se define como clase pública de nivel superior en
    // FilaSimetria.cs (necesario para x:DataType="vm:FilaSimetria" en el DataTemplate).

    // Colección de filas de simetría. El form legacy arranca con 30 controles y va
    // añadiendo más conforme el usuario rellena el último (Añadir_Enter).
    public ObservableCollection<FilaSimetria> Simetrias { get; } = new();

    // Campo "Aciertos": individuales o rangos separados por comas (txtAciertos legacy).
    [ObservableProperty]
    private string _aciertos = string.Empty;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    // Fichero temporal de copiar/pegar (legacy: StartupPath + "/Temp/tmp.sim").
    private static string RutaTemporal =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.sim");

    // Directorio de columnas ganadoras (legacy: StartupPath + "/Ganadoras/").
    private static string DirectorioGanadoras =>
        Path.Combine(AppContext.BaseDirectory, "Ganadoras") + Path.DirectorySeparatorChar;

    public SimetriasFrmViewModel()
    {
        // El form legacy crea 30 controles iniciales (LlenarControles(30)).
        for (int i = 1; i <= 30; i++)
        {
            Simetrias.Add(new FilaSimetria(i));
        }
    }

    /// <summary>
    /// Vuelca las simetrías del FiltroSimetrias del grupo en edición a las filas.
    /// Equivale a SimetriasFrm.MarcarValores() (Free1X2/UI/Filtros/SimetriasFrm.cs líneas 361-391).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroSimetrias)grupo.GetFiltro(Filtro.Simetrias.ToString());
        if (!filtro.ContieneDatos) return;

        // Asegurar suficientes filas.
        while (Simetrias.Count < filtro.ArraySimetrias.Count)
        {
            Simetrias.Add(new FilaSimetria(Simetrias.Count + 1));
        }
        for (int i = 0; i < filtro.ArraySimetrias.Count; i++)
        {
            Simetrias[i].Partidos = filtro.ArraySimetrias[i].Partidos;
        }
        Aciertos = filtro.Aciertos;
    }

    // Valida una entrada de simetría (partidos 1..NumeroPartidos). SimetriasFrm.CompruebaEntradas, líneas 289-318.
    private static bool EntradaValida(string partidosSimetria)
    {
        if (string.IsNullOrWhiteSpace(partidosSimetria)) return false;
        try
        {
            bool esValida = false;
            foreach (string parte in partidosSimetria.Split(','))
            {
                foreach (string parte2 in parte.Split('-'))
                {
                    int a = Convert.ToInt32(parte2.Trim());
                    if (a <= 0 || a > VariablesGlobales.NumeroPartidos) return false;
                    esValida = true;
                }
            }
            return esValida;
        }
        catch
        {
            return false;
        }
    }

    // Construye la lista de Simetria (ObtenerSimetrias, líneas 253-277). Devuelve null si hay entrada inválida.
    private List<Simetria>? ObtenerSimetrias()
    {
        var lista = new List<Simetria>();
        foreach (var fila in Simetrias)
        {
            string txt = fila.Partidos;
            if (string.IsNullOrWhiteSpace(txt)) continue;
            if (!EntradaValida(txt)) return null;
            lista.Add(new Simetria(UtilidadesEntradasValores.ObtenerValoresSeparadosPorComas(txt)));
        }
        return lista;
    }

    // Parsea txtAciertos a List<int> (ObtenerAciertos, líneas 319-352). Devuelve null si el formato es inválido.
    private List<int>? ObtenerAciertos()
    {
        var lista = new List<int>();
        if (string.IsNullOrWhiteSpace(Aciertos)) return lista;
        try
        {
            foreach (string parte in Aciertos.Split(','))
            {
                string p = parte.Trim();
                if (p.LastIndexOf('-') == -1)
                {
                    int v = Convert.ToInt32(p);
                    if (v >= 0 && v <= 40) lista.Add(v);
                }
                else
                {
                    string[] intervalo = p.Split('-');
                    for (int j = Convert.ToInt32(intervalo[0]); j <= Convert.ToInt32(intervalo[1]); j++)
                    {
                        if (j >= 0 && j <= 40) lista.Add(j);
                    }
                }
            }
            return lista;
        }
        catch
        {
            return null;
        }
    }

    // true si hay alguna simetría introducida (filtro.ContieneDatos legacy).
    public bool ContieneDatos
    {
        get
        {
            foreach (var fila in Simetrias)
            {
                if (!string.IsNullOrWhiteSpace(fila.Partidos))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [RelayCommand]
    private void AnadirFila()
    {
        // Equivale a Añadir_Enter del form legacy: añade un nuevo CtrlSimetria al final.
        Simetrias.Add(new FilaSimetria(Simetrias.Count + 1));
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a SimetriasFrm.menuCondiciones1_BOk (Free1X2/UI/Filtros/SimetriasFrm.cs líneas 413-436).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var simetrias = ObtenerSimetrias();
        var aciertos = ObtenerAciertos();
        // Si hay una entrada inválida, el form legacy muestra un error y NO cierra.
        if (simetrias is null || aciertos is null) return;

        var filtro = (FiltroSimetrias)grupo.GetFiltro(Filtro.Simetrias.ToString());
        filtro.ArraySimetrias = simetrias;
        filtro.ArrayAciertos = aciertos;
        filtro.Aciertos = string.IsNullOrWhiteSpace(Aciertos) ? "0" : Aciertos;
        filtro.IsActive = filtro.ContieneDatos;

        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    /// <summary>
    /// Construye un FiltroSimetrias temporal con los valores de pantalla.
    /// Réplica de SimetriasFrm.ObtenerFiltroTemporal() (SimetriasFrm.cs líneas 392-412).
    /// Devuelve null si hay alguna entrada inválida (el legacy mostraba un MessageBox).
    /// </summary>
    private FiltroSimetrias? ObtenerFiltroTemporal()
    {
        var simetrias = ObtenerSimetrias();
        var aciertos = ObtenerAciertos();
        if (simetrias is null || aciertos is null)
        {
            AppServices.MostrarError(
                "Verifique que ha introducido una simetría correcta: (a,b) ó (a-b) ó (a,b-c), " +
                "y que ha introducido un número válido de aciertos");
            return null;
        }

        var filtroTemp = new FiltroSimetrias
        {
            ArraySimetrias = simetrias,
            ArrayAciertos = aciertos,
            Aciertos = string.IsNullOrWhiteSpace(Aciertos) ? "0" : Aciertos,
        };
        filtroTemp.IsActive = filtroTemp.ContieneDatos;
        return filtroTemp;
    }

    // Vuelca un FiltroSimetrias a las filas y al campo Aciertos (MarcarValores(FiltroSimetrias) legacy, líneas 371-391).
    private void MarcarValores(FiltroSimetrias filtro)
    {
        foreach (var fila in Simetrias) fila.Partidos = string.Empty;

        while (Simetrias.Count < filtro.ArraySimetrias.Count)
        {
            Simetrias.Add(new FilaSimetria(Simetrias.Count + 1));
        }
        for (int i = 0; i < filtro.ArraySimetrias.Count; i++)
        {
            Simetrias[i].Partidos = filtro.ArraySimetrias[i].Partidos;
        }
        Aciertos = filtro.Aciertos;
    }

    // Guarda en disco el filtro temporal (SimetriasFrm.guardar(), líneas 510-515).
    private void GuardarEn(string nombreArchivo)
    {
        var filtroTemp = ObtenerFiltroTemporal();
        if (filtroTemp is null) return;
        var archComb = new ArchivoCondiciones { NombreArchivo = nombreArchivo };
        archComb.GuardaArchivo(filtroTemp);
    }

    // Abre la condición desde disco y vuelca sus valores (SimetriasFrm.abrir(), líneas 489-499).
    private void AbrirDesde(string nombreArchivo)
    {
        var archComb = new ArchivoCondiciones();
        if (archComb.AbrirArchivoCombinacion(nombreArchivo))
        {
            Grupo g = archComb.LeeCondicion();
            var filtro = (FiltroSimetrias)g.GetFiltro("Simetrias");
            MarcarValores(filtro);
        }
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // Equivale a SimetriasFrm.menuCondiciones2_BEstadisticas (SimetriasFrm.cs líneas 566-576).
        var filtroTemp = ObtenerFiltroTemporal();
        if (filtroTemp is null) return;

        var calc = new CalculadorEstadisticas();
        List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, DirectorioGanadoras);

        VisorEstadisticasViewModel.UltimasEstadisticas = lista;
        Navegar?.Invoke(typeof(VisorEstadisticasPage));
    }

    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a SimetriasFrm.menuCondiciones1_BGuardar (SimetriasFrm.cs líneas 500-509).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Simetrias",
        };
        picker.FileTypeChoices.Add("Simetrias", new List<string> { ".sim" });
        picker.FileTypeChoices.Add("Simetrias (XML)", new List<string> { ".xml" });
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
        // Equivale a SimetriasFrm.menuCondiciones1_BAbrir (SimetriasFrm.cs líneas 472-488).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".sim");
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
        // Equivale a SimetriasFrm.menuCondiciones1_BCopiar (SimetriasFrm.cs líneas 516-523).
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
        // Equivale a SimetriasFrm.menuCondiciones1_BPegar (SimetriasFrm.cs líneas 524-540).
        if (File.Exists(RutaTemporal))
        {
            AbrirDesde(RutaTemporal);
        }
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a menuCondiciones1_BBorrar + LimpiarPantalla() del form legacy.
        foreach (var fila in Simetrias)
        {
            fila.Partidos = string.Empty;
        }
        Aciertos = string.Empty;
        // Equivale a menuCondiciones1_BBorrar: reinstanciar el filtro vacío en el grupo.
        var grupo = AppState.GrupoEnEdicion;
        grupo?.ActivaFiltro(new FiltroSimetrias());
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }
}
