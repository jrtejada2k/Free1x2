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
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de la condición de Contactos: una pareja de signos (p.ej. "1X")
/// y la lista de números de contactos admitidos (cadena separada por comas, 0-14).
/// Equivale a un control OptionNumTol0_14 del ContactosFrm WinForms.
/// </summary>
public partial class FilaContactoViewModel : ObservableObject
{
    public FilaContactoViewModel(string pareja, string valores)
    {
        Pareja = pareja;
        _valores = valores;
    }

    /// <summary>Etiqueta de la pareja de signos: 1X, 12, X2, 11, XX, 22, 1V, XV, 2V, VV.</summary>
    public string Pareja { get; }

    [ObservableProperty]
    private string _valores = string.Empty;
}

/// <summary>
/// ViewModel de la pantalla de filtro "Contactos".
/// Mantiene las 10 filas de parejas de signos y el estado de las figuras.
/// Hace el round-trip contra <c>FiltroContactos</c> del Grupo en edición
/// (AppState.GrupoEnEdicion). La edición de figuras y la persistencia en disco quedan como TODO.
/// </summary>
public partial class ContactosFrmViewModel : ObservableObject
{
    // Rango admitido por cada OptionNumTol0_14 (Minimo=0, Maximo=15 en el form legacy; 0-14 contactos posibles).
    private const string ValoresPorDefecto = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";

    // Lista de figuras asociada al filtro (se carga del filtro y se preserva al Aceptar).
    private List<long>? _figuras;

    public ContactosFrmViewModel()
    {
        Filas = new ObservableCollection<FilaContactoViewModel>
        {
            new("1X", ValoresPorDefecto),
            new("12", ValoresPorDefecto),
            new("X2", ValoresPorDefecto),
            new("11", ValoresPorDefecto),
            new("XX", ValoresPorDefecto),
            new("22", ValoresPorDefecto),
            new("1V", ValoresPorDefecto),
            new("XV", ValoresPorDefecto),
            new("2V", ValoresPorDefecto),
            new("VV", ValoresPorDefecto),
        };
    }

    public ObservableCollection<FilaContactoViewModel> Filas { get; }

    /// <summary>True si hay figuras asociadas a la condición (botón "Figuras" resaltado en el form legacy).</summary>
    [ObservableProperty]
    private bool _tieneFiguras;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    // Fichero temporal de copiar/pegar (legacy: StartupPath + "/Temp/tmp.cont").
    private static string RutaTemporal =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.cont");

    // Directorio de columnas ganadoras (legacy: StartupPath + "/Ganadoras/").
    private static string DirectorioGanadoras =>
        Path.Combine(AppContext.BaseDirectory, "Ganadoras") + Path.DirectorySeparatorChar;

    // true si alguna fila tiene valores o hay figuras (NecesitaGuardarDatos() del form legacy).
    private bool ContieneDatos()
    {
        foreach (var fila in Filas)
        {
            if (!string.IsNullOrWhiteSpace(fila.Valores)) return true;
        }
        return _figuras != null && _figuras.Count > 0;
    }

    /// <summary>
    /// Vuelca los valores del FiltroContactos del grupo en edición a las 10 filas.
    /// Equivale a ContactosFrm.MarcarValores() (Free1X2/UI/Filtros/ContactosFrm.cs líneas 94-113).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroContactos)grupo.GetFiltro(Filtro.Contactos.ToString());
        Filas[0].Valores = filtro.GetNum1X();
        Filas[1].Valores = filtro.GetNum12();
        Filas[2].Valores = filtro.GetNumX2();
        Filas[3].Valores = filtro.GetNum11();
        Filas[4].Valores = filtro.GetNumXX();
        Filas[5].Valores = filtro.GetNum22();
        Filas[6].Valores = filtro.GetNum1V();
        Filas[7].Valores = filtro.GetNumXV();
        Filas[8].Valores = filtro.GetNum2V();
        Filas[9].Valores = filtro.GetNumVV();

        _figuras = filtro.Figuras;
        TieneFiguras = _figuras != null && _figuras.Count > 0;
    }

    /// <summary>
    /// Vuelca al filtro indicado las 10 filas de pantalla y las figuras.
    /// Réplica de ContactosFrm.guardarDatos(FiltroContactos) (ContactosFrm.cs líneas 511-634).
    /// </summary>
    private void GuardarDatos(FiltroContactos filtro)
    {
        // El form legacy construye "todos" sobre el rango 0..15 del control (std1X.Minimo..Maximo).
        string todosValores = Free1X2.Utils.UtilidadesEntradasValores.ObtenerTodosValores(0, 15);

        if (ContieneDatos())
        {
            if (!filtro.ContieneDatos)
            {
                filtro.IsActive = true;
            }
            filtro.ContieneDatos = true;
            filtro.Inicializa();

            filtro.SetNum1X(ValorOTodos(Filas[0].Valores, todosValores));
            filtro.SetNum12(ValorOTodos(Filas[1].Valores, todosValores));
            filtro.SetNumX2(ValorOTodos(Filas[2].Valores, todosValores));
            filtro.SetNum11(ValorOTodos(Filas[3].Valores, todosValores));
            filtro.SetNumXX(ValorOTodos(Filas[4].Valores, todosValores));
            filtro.SetNum22(ValorOTodos(Filas[5].Valores, todosValores));
            filtro.SetNum1V(ValorOTodos(Filas[6].Valores, todosValores));
            filtro.SetNumXV(ValorOTodos(Filas[7].Valores, todosValores));
            filtro.SetNum2V(ValorOTodos(Filas[8].Valores, todosValores));
            filtro.SetNumVV(ValorOTodos(Filas[9].Valores, todosValores));

            if (_figuras != null && _figuras.Count > 0)
            {
                filtro.Figuras = _figuras;
            }
        }
        else
        {
            filtro.IsActive = false;
            filtro.ContieneDatos = false;
        }
    }

    // Vuelca al UI las 10 filas + figuras de un FiltroContactos (MarcarValores legacy, líneas 94-113).
    private void MarcarValores(FiltroContactos filtro)
    {
        Filas[0].Valores = filtro.GetNum1X();
        Filas[1].Valores = filtro.GetNum12();
        Filas[2].Valores = filtro.GetNumX2();
        Filas[3].Valores = filtro.GetNum11();
        Filas[4].Valores = filtro.GetNumXX();
        Filas[5].Valores = filtro.GetNum22();
        Filas[6].Valores = filtro.GetNum1V();
        Filas[7].Valores = filtro.GetNumXV();
        Filas[8].Valores = filtro.GetNum2V();
        Filas[9].Valores = filtro.GetNumVV();

        _figuras = filtro.Figuras;
        TieneFiguras = _figuras != null && _figuras.Count > 0;
    }

    [RelayCommand]
    private void EditarFiguras()
    {
        // Equivale a ContactosFrm.btnFiguras_Click (ContactosFrm.cs líneas 764-773):
        //   new FigurasFiltrosFrm(figuras, 10, new FiltroContactos()).ShowDialog(); IndicarCondicionFiguras().
        // El handoff comparte la MISMA referencia de lista, así que al volver _figuras refleja la edición
        // (FigurasFiltrosFrmViewModel.Aceptar muta esa lista). La página refresca TieneFiguras en OnNavigatedTo.
        _figuras ??= new List<long>();
        FigurasFiltrosFrmViewModel.FigurasEnEdicion = _figuras;
        // Indica al editor de figuras que la condición es de Contactos (DeterminarCondicion legacy).
        FigurasFiltrosFrmViewModel.NombreCondicion = "Contactos";
        Navegar?.Invoke(typeof(FigurasFiltrosFrmPage));
    }

    /// <summary>
    /// Refresca el semáforo de figuras al volver del editor de figuras (la página lo llama en OnNavigatedTo).
    /// </summary>
    public void RefrescarFiguras()
    {
        TieneFiguras = _figuras != null && _figuras.Count > 0;
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a ContactosFrm.menuCondiciones1_BOk -> guardarDatos() + UsaFiguras() + ActivaFiltro
        //   (Free1X2/UI/Filtros/ContactosFrm.cs líneas 636-644).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroContactos)grupo.GetFiltro(Filtro.Contactos.ToString());
        GuardarDatos(filtro);
        filtro.UsaFiguras();
        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    private static string ValorOTodos(string valor, string todos)
        => !string.IsNullOrWhiteSpace(valor) ? valor : todos;

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }

    // Guarda en disco la condición de Contactos del grupo en edición (ContactosFrm.guardar, líneas 692-698).
    private void GuardarEn(string nombreArchivo)
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroContactos)grupo.GetFiltro(Filtro.Contactos.ToString());
        GuardarDatos(filtro);
        var archComb = new ArchivoCondiciones { NombreArchivo = nombreArchivo };
        archComb.GuardaArchivo(filtro);
    }

    // Abre la condición desde disco y vuelca sus valores (ContactosFrm.abrir, líneas 681-690).
    private void AbrirDesde(string nombreArchivo)
    {
        var archComb = new ArchivoCondiciones();
        if (archComb.AbrirArchivoCombinacion(nombreArchivo))
        {
            var grupo = archComb.LeeCondicion();
            var filtro = (FiltroContactos)grupo.GetFiltro(Filtro.Contactos.ToString());
            MarcarValores(filtro);
        }
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // Equivale a ContactosFrm.menuCondiciones1_BEstadisticas (ContactosFrm.cs líneas 775-789):
        //   guardarDatos(filtroTemp) + UsaFiguras() -> CalculadorEstadisticas.EstadisticasFiltro -> VisorEstadisticas.
        var filtroTemp = new FiltroContactos();
        GuardarDatos(filtroTemp);
        filtroTemp.UsaFiguras();

        var calc = new CalculadorEstadisticas();
        List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, DirectorioGanadoras);

        VisorEstadisticasViewModel.UltimasEstadisticas = lista;
        Navegar?.Invoke(typeof(VisorEstadisticasPage));
    }

    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a ContactosFrm.menuCondiciones1_BGuardar (ContactosFrm.cs líneas 668-679).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Contactos",
        };
        picker.FileTypeChoices.Add("Contactos", new List<string> { ".cont" });
        picker.FileTypeChoices.Add("Contactos (XML)", new List<string> { ".xml" });
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
        // Equivale a ContactosFrm.menuCondiciones1_BAbrir (ContactosFrm.cs líneas 651-666).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".cont");
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
        // Equivale a ContactosFrm.menuCondiciones1_BCopiar (ContactosFrm.cs líneas 709-720).
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
        // Equivale a ContactosFrm.menuCondiciones1_BPegar (ContactosFrm.cs líneas 722-732).
        if (File.Exists(RutaTemporal))
        {
            AbrirDesde(RutaTemporal);
        }
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a menuCondiciones1_BBorrar -> reinstanciar el filtro + MarcarValores().
        foreach (var fila in Filas)
        {
            fila.Valores = string.Empty;
        }
        _figuras = null;
        TieneFiguras = false;
    }
}
