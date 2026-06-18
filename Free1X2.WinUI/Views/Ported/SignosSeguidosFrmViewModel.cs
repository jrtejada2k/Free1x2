// Free1X2 · WinUI 3 — WIN3
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del filtro "Signos Seguidos" (WinForms <c>SignosSeguidosFrm</c>).
/// El filtro mantiene cuatro juegos de valores que indican la cantidad admitida de
/// Variantes, signos 1, signos X y signos 2 SEGUIDOS dentro de cada combinación.
/// Cada propiedad almacena la lista de cantidades admitidas (rango 0..15) tal como
/// el control legacy <c>OptionNumTol0_14</c> (cadena de tolerancias separadas por comas).
/// Además, cada concepto puede tener una lista de "Figuras" asociada (List&lt;long&gt; en
/// el dominio legacy: FigurasV/Figuras1/FigurasX/Figuras2), gestionada vía FigurasFiltrosFrm.
/// </summary>
public partial class SignosSeguidosFrmViewModel : ObservableObject
{
    // Cantidades admitidas de "Variantes" seguidas. Equivale a filtro.GetVariantes()/SetNoVariantes().
    [ObservableProperty]
    private string _variantes = string.Empty;

    // Cantidades admitidas de signos "1" seguidos. Equivale a filtro.GetUnos()/SetNoUnos().
    [ObservableProperty]
    private string _unos = string.Empty;

    // Cantidades admitidas de signos "X" seguidos. Equivale a filtro.GetEquis()/SetNoEquis().
    [ObservableProperty]
    private string _equis = string.Empty;

    // Cantidades admitidas de signos "2" seguidos. Equivale a filtro.GetDoses()/SetNoDoses().
    [ObservableProperty]
    private string _doses = string.Empty;

    // Indicadores de si cada concepto tiene "figuras" definidas (botón verde en el form legacy,
    // IndicarCondicionFiguras()). El contenido real (List<long>) lo gestiona el dominio legacy.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasVariantesVisibility))]
    private bool _figurasVariantesActivas;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasUnosVisibility))]
    private bool _figurasUnosActivas;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasEquisVisibility))]
    private bool _figurasEquisActivas;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasDosesVisibility))]
    private bool _figurasDosesActivas;

    // Visibilidad del indicador "Activas" por concepto. Se exponen como Visibility para no
    // bindear un bool directo a la UI (regla del proyecto).
    public Visibility FigurasVariantesVisibility =>
        FigurasVariantesActivas ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FigurasUnosVisibility =>
        FigurasUnosActivas ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FigurasEquisVisibility =>
        FigurasEquisActivas ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FigurasDosesVisibility =>
        FigurasDosesActivas ? Visibility.Visible : Visibility.Collapsed;

    // El form legacy rellena los campos vacíos con "0,1,...,14" (SignosSeguidosFrm.ActualizarDatos línea 345).
    private const string TodosValores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";

    // Listas de figuras cargadas del filtro; se preservan al Aceptar y se editan vía EditarFiguras
    // (navega a FigurasFiltrosFrmPage compartiendo la referencia de lista).
    private List<long>? _figurasV;
    private List<long>? _figuras1;
    private List<long>? _figurasX;
    private List<long>? _figuras2;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    // Fichero temporal de copiar/pegar (legacy: StartupPath + "/Temp/tmp.seg").
    private static string RutaTemporal =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.seg");

    // Directorio de columnas ganadoras (legacy: StartupPath + "/Ganadoras/").
    private static string DirectorioGanadoras =>
        Path.Combine(AppContext.BaseDirectory, "Ganadoras") + Path.DirectorySeparatorChar;

    // true si hay algún valor introducido (NecesitaGuardarDatos() del form legacy).
    public bool ContieneDatos =>
        !string.IsNullOrWhiteSpace(Variantes) ||
        !string.IsNullOrWhiteSpace(Unos) ||
        !string.IsNullOrWhiteSpace(Equis) ||
        !string.IsNullOrWhiteSpace(Doses);

    /// <summary>
    /// Vuelca los valores del FiltroSignosSeguidos del grupo en edición a la pantalla.
    /// Equivale a SignosSeguidosFrm.MarcarValores() (Free1X2/UI/Filtros/SignosSeguidosFrm.cs líneas 75-87).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroSignosSeguidos)grupo.GetFiltro(Filtro.SignosSeguidos.ToString());
        Variantes = filtro.GetVariantes();
        Unos = filtro.GetUnos();
        Equis = filtro.GetEquis();
        Doses = filtro.GetDoses();

        _figurasV = filtro.FigurasV;
        _figuras1 = filtro.Figuras1;
        _figurasX = filtro.FigurasX;
        _figuras2 = filtro.Figuras2;
        FigurasVariantesActivas = _figurasV != null && _figurasV.Count > 0;
        FigurasUnosActivas = _figuras1 != null && _figuras1.Count > 0;
        FigurasEquisActivas = _figurasX != null && _figurasX.Count > 0;
        FigurasDosesActivas = _figuras2 != null && _figuras2.Count > 0;
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a SignosSeguidosFrm.menuCondiciones1_BOk -> ActualizarDatos() + ActivaFiltro
        //   (Free1X2/UI/Filtros/SignosSeguidosFrm.cs líneas 343-534).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroSignosSeguidos)grupo.GetFiltro(Filtro.SignosSeguidos.ToString());
        filtro.ReinicializaValores();

        if (ContieneDatos)
        {
            if (filtro.ContieneDatos == false)
            {
                filtro.IsActive = true;
            }
            filtro.ContieneDatos = true;

            filtro.SetNoVariantes(!string.IsNullOrWhiteSpace(Variantes) ? Variantes : TodosValores);
            filtro.SetNoUnos(!string.IsNullOrWhiteSpace(Unos) ? Unos : TodosValores);
            filtro.SetNoEquis(!string.IsNullOrWhiteSpace(Equis) ? Equis : TodosValores);
            filtro.SetNoDoses(!string.IsNullOrWhiteSpace(Doses) ? Doses : TodosValores);

            if (_figurasV != null && _figurasV.Count > 0) filtro.FigurasV = _figurasV;
            if (_figuras1 != null && _figuras1.Count > 0) filtro.Figuras1 = _figuras1;
            if (_figurasX != null && _figurasX.Count > 0) filtro.FigurasX = _figurasX;
            if (_figuras2 != null && _figuras2.Count > 0) filtro.Figuras2 = _figuras2;
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
        // Equivale a menuCondiciones1_BBorrar -> reinstanciar FiltroSignosSeguidos + MarcarValores().
        Variantes = string.Empty;
        Unos = string.Empty;
        Equis = string.Empty;
        Doses = string.Empty;
        _figurasV = _figuras1 = _figurasX = _figuras2 = null;
        FigurasVariantesActivas = false;
        FigurasUnosActivas = false;
        FigurasEquisActivas = false;
        FigurasDosesActivas = false;
    }

    /// <summary>
    /// Construye un FiltroSignosSeguidos temporal con los valores y figuras de pantalla.
    /// Réplica de SignosSeguidosFrm.ObtenerFiltroTemporal() (SignosSeguidosFrm.cs líneas 434-...).
    /// </summary>
    private FiltroSignosSeguidos ObtenerFiltroTemporal()
    {
        var filtroTemp = new FiltroSignosSeguidos();
        filtroTemp.ReinicializaValores();

        if (ContieneDatos)
        {
            if (filtroTemp.ContieneDatos == false)
            {
                filtroTemp.IsActive = true;
            }
            filtroTemp.ContieneDatos = true;
            filtroTemp.SetNoVariantes(!string.IsNullOrWhiteSpace(Variantes) ? Variantes : TodosValores);
            filtroTemp.SetNoUnos(!string.IsNullOrWhiteSpace(Unos) ? Unos : TodosValores);
            filtroTemp.SetNoEquis(!string.IsNullOrWhiteSpace(Equis) ? Equis : TodosValores);
            filtroTemp.SetNoDoses(!string.IsNullOrWhiteSpace(Doses) ? Doses : TodosValores);

            if (_figurasV != null && _figurasV.Count > 0) filtroTemp.FigurasV = _figurasV;
            if (_figuras1 != null && _figuras1.Count > 0) filtroTemp.Figuras1 = _figuras1;
            if (_figurasX != null && _figurasX.Count > 0) filtroTemp.FigurasX = _figurasX;
            if (_figuras2 != null && _figuras2.Count > 0) filtroTemp.Figuras2 = _figuras2;
        }
        else
        {
            filtroTemp.IsActive = false;
            filtroTemp.ContieneDatos = false;
        }
        return filtroTemp;
    }

    // Vuelca un FiltroSignosSeguidos a las propiedades de pantalla (MarcarValores legacy, líneas 75-87).
    private void MarcarValores(FiltroSignosSeguidos filtro)
    {
        Variantes = filtro.GetVariantes();
        Unos = filtro.GetUnos();
        Equis = filtro.GetEquis();
        Doses = filtro.GetDoses();

        _figurasV = filtro.FigurasV;
        _figuras1 = filtro.Figuras1;
        _figurasX = filtro.FigurasX;
        _figuras2 = filtro.Figuras2;
        ActualizarIndicadoresFiguras();
    }

    // Recalcula los 4 indicadores de figuras activas (IndicarCondicionFiguras legacy).
    private void ActualizarIndicadoresFiguras()
    {
        FigurasVariantesActivas = _figurasV != null && _figurasV.Count > 0;
        FigurasUnosActivas = _figuras1 != null && _figuras1.Count > 0;
        FigurasEquisActivas = _figurasX != null && _figurasX.Count > 0;
        FigurasDosesActivas = _figuras2 != null && _figuras2.Count > 0;
    }

    /// <summary>Refresca los indicadores de figuras al volver del editor (lo llama la página en OnNavigatedTo).</summary>
    public void RefrescarFiguras() => ActualizarIndicadoresFiguras();

    // Guarda en disco el filtro temporal (SignosSeguidosFrm.guardar(), líneas 579-584).
    private void GuardarEn(string nombreArchivo)
    {
        var filtroTemp = ObtenerFiltroTemporal();
        var archComb = new ArchivoCondiciones { NombreArchivo = nombreArchivo };
        archComb.GuardaArchivo(filtroTemp);
    }

    // Abre la condición desde disco y vuelca sus valores (SignosSeguidosFrm.abrir(), líneas 567-577).
    private void AbrirDesde(string nombreArchivo)
    {
        var archComb = new ArchivoCondiciones();
        if (archComb.AbrirArchivoCombinacion(nombreArchivo))
        {
            Grupo g = archComb.LeeCondicion();
            var filtro = (FiltroSignosSeguidos)g.GetFiltro("SignosSeguidos");
            MarcarValores(filtro);
        }
    }

    // Navega al editor de figuras compartiendo la lista por referencia (btnFiguras*_Click legacy).
    private void EditarFiguras(ref List<long>? lista)
    {
        lista ??= new List<long>();
        FigurasFiltrosFrmViewModel.FigurasEnEdicion = lista;
        FigurasFiltrosFrmViewModel.NombreCondicion = "SignosSeguidos";
        Navegar?.Invoke(typeof(FigurasFiltrosFrmPage));
    }

    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a SignosSeguidosFrm.menuCondiciones1_BGuardar (SignosSeguidosFrm.cs líneas 558-565).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "SignosSeguidos",
        };
        picker.FileTypeChoices.Add("Signos seguidos", new List<string> { ".seg" });
        picker.FileTypeChoices.Add("Signos seguidos (XML)", new List<string> { ".xml" });
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
        // Equivale a SignosSeguidosFrm.menuCondiciones1_BAbrir (SignosSeguidosFrm.cs líneas 547-556).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".seg");
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
        // Equivale a SignosSeguidosFrm.menuCondiciones1_BCopiar (SignosSeguidosFrm.cs líneas ~600).
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
        // Equivale a SignosSeguidosFrm.menuCondiciones1_BPegar (SignosSeguidosFrm.cs líneas ~617).
        if (File.Exists(RutaTemporal))
        {
            AbrirDesde(RutaTemporal);
        }
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // Equivale a SignosSeguidosFrm.menuCondiciones1_BEstadisticas (SignosSeguidosFrm.cs líneas 744-754).
        var filtroTemp = ObtenerFiltroTemporal();
        var calc = new CalculadorEstadisticas();
        List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, DirectorioGanadoras);

        VisorEstadisticasViewModel.UltimasEstadisticas = lista;
        Navegar?.Invoke(typeof(VisorEstadisticasPage));
    }

    [RelayCommand]
    private void FigurasVariantes()
    {
        // Equivale a btnFigurasV_Click (SignosSeguidosFrm.cs líneas 700-709).
        EditarFiguras(ref _figurasV);
    }

    [RelayCommand]
    private void FigurasUnos()
    {
        // Equivale a btnFiguras1_Click (SignosSeguidosFrm.cs líneas 711-720).
        EditarFiguras(ref _figuras1);
    }

    [RelayCommand]
    private void FigurasEquis()
    {
        // Equivale a btnFigurasX_Click (SignosSeguidosFrm.cs líneas 722-731).
        EditarFiguras(ref _figurasX);
    }

    [RelayCommand]
    private void FigurasDoses()
    {
        // Equivale a btnFiguras2_Click (SignosSeguidosFrm.cs líneas 733-742).
        EditarFiguras(ref _figuras2);
    }
}
