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
/// Una opción seleccionable de tolerancia numérica (0..5) o de figura.
/// Equivale a las Label-casilla "optN" / "lblFigXX" del WinForms <c>PesosNumFrm</c>,
/// que alternan entre Wheat (inactiva) y LightGreen (activa) al hacer clic.
/// </summary>
public partial class OpcionSeleccionableViewModel : ObservableObject
{
    public OpcionSeleccionableViewModel(string etiqueta, bool seleccionada = false)
    {
        Etiqueta = etiqueta;
        _seleccionada = seleccionada;
    }

    /// <summary>Texto visible de la casilla (p. ej. "3" o "2-1-1-1").</summary>
    public string Etiqueta { get; }

    /// <summary>True si la casilla está activa (legacy: BackColor == LightGreen).</summary>
    [ObservableProperty]
    private bool _seleccionada;
}

/// <summary>
/// ViewModel de la pantalla portada del WinForms "PesosNumFrm".
/// Mantiene los conjuntos de Pesos Numéricos (Global / Variantes / 1 / X / 2),
/// sus tolerancias homónimas, las tolerancias numéricas (0..5) y las figuras
/// seleccionables. La conversión texto→valores, el cálculo y la persistencia se
/// delegan al dominio legacy (ver TODOs).
/// </summary>
public partial class PesosNumFrmViewModel : ObservableObject
{
    public PesosNumFrmViewModel()
    {
        // Tolerancias numéricas admitidas (legacy: opt0..opt5).
        ToleranciasNumericas = new ObservableCollection<OpcionSeleccionableViewModel>
        {
            new("0"), new("1"), new("2"), new("3"), new("4"), new("5"),
        };

        // Figuras seleccionables (legacy: lblFig32 / lblFig311 / lblFig221 / lblFig2111 / lblFig11111).
        Figuras = new ObservableCollection<OpcionSeleccionableViewModel>
        {
            new("3-2"), new("3-1-1"), new("2-2-1"), new("2-1-1-1"), new("1-1-1-1-1"),
        };

    }

    // Lista de figuras del filtro (List<long>); se carga y preserva al Aceptar.
    private List<long> _figuras = new();

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    // Fichero temporal de copiar/pegar (legacy: StartupPath + "/Temp/tmp.pes").
    private static string RutaTemporal =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.pes");

    // Directorio de columnas ganadoras (legacy: StartupPath + "/Ganadoras/").
    private static string DirectorioGanadoras =>
        Path.Combine(AppContext.BaseDirectory, "Ganadoras") + Path.DirectorySeparatorChar;

    /// <summary>
    /// Vuelca los valores del FiltroPesosNumericos del grupo en edición a la pantalla.
    /// Equivale a PesosNumFrm.MarcarValores() (Free1X2/UI/Filtros/PesosNumFrm.cs líneas 93-134).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroPesosNumericos)grupo.GetFiltro(Filtro.PesosNumericos.ToString());
        MarcarValores(filtro);
    }

    // Vuelca un FiltroPesosNumericos a la pantalla (MarcarValores legacy, líneas 93-134).
    private void MarcarValores(FiltroPesosNumericos filtro)
    {
        PesoGlobal = filtro.GetPNGlobal();
        PesoVariantes = filtro.GetPNVariantes();
        PesoUnos = filtro.GetPNUnos();
        PesoEquis = filtro.GetPNEquis();
        PesoDoses = filtro.GetPNDoses();

        PesoGlobalTol = filtro.GetPNGlobalTol();
        PesoVariantesTol = filtro.GetPNVariantesTol();
        PesoUnosTol = filtro.GetPNUnosTol();
        PesoEquisTol = filtro.GetPNEquisTol();
        PesoDosesTol = filtro.GetPNDosesTol();

        // Tolerancias numéricas seleccionadas (GetTolerancias devuelve p. ej. "0,2,5").
        var tols = new HashSet<string>((filtro.GetTolerancias() ?? "").Split(','));
        foreach (var t in ToleranciasNumericas) t.Seleccionada = tols.Contains(t.Etiqueta);

        // Figuras seleccionadas (MarcarFigurasSeleccionadas).
        _figuras = filtro.Figuras ?? new List<long>();
        foreach (var f in Figuras)
        {
            long fig = UtilidadesEntradasValores.ObtenerLongFiguraFromText(f.Etiqueta);
            f.Seleccionada = _figuras.Contains(fig);
        }
    }

    // ToleranciaValores: cadena con las tolerancias 0..5 seleccionadas (PesosNumFrm.ToleranciaValores get).
    private string ToleranciaValores()
    {
        var sel = new List<string>();
        foreach (var t in ToleranciasNumericas)
        {
            if (t.Seleccionada) sel.Add(t.Etiqueta);
        }
        return string.Join(",", sel);
    }

    // Reconstruye la lista de figuras a partir de las casillas seleccionadas (MarcarFigura).
    private List<long> ConstruirFiguras()
    {
        var lista = new List<long>();
        foreach (var f in Figuras)
        {
            if (f.Seleccionada)
            {
                lista.Add(UtilidadesEntradasValores.ObtenerLongFiguraFromText(f.Etiqueta));
            }
        }
        return lista;
    }

    // NecesitaGuardarDatos: hay PN principal o figuras (PesosNumFrm.NecesitaGuardarDatos líneas 403-413).
    private bool NecesitaGuardarDatos(List<long> figuras) =>
        !string.IsNullOrWhiteSpace(PesoGlobal) ||
        !string.IsNullOrWhiteSpace(PesoVariantes) ||
        !string.IsNullOrWhiteSpace(PesoUnos) ||
        !string.IsNullOrWhiteSpace(PesoEquis) ||
        !string.IsNullOrWhiteSpace(PesoDoses) ||
        figuras.Count > 0;

    // NecesitaGuardarDatosTol: hay alguna tolerancia homónima (líneas 415-427).
    private bool NecesitaGuardarDatosTol() =>
        !string.IsNullOrWhiteSpace(PesoGlobalTol) ||
        !string.IsNullOrWhiteSpace(PesoVariantesTol) ||
        !string.IsNullOrWhiteSpace(PesoUnosTol) ||
        !string.IsNullOrWhiteSpace(PesoEquisTol) ||
        !string.IsNullOrWhiteSpace(PesoDosesTol);

    private static string ValorOTodos(string v, string todos) =>
        !string.IsNullOrWhiteSpace(v) ? v : todos;

    // ===== Pesos Numéricos principales (legacy: stdGlobal/stdVariantes/stdUnos/stdEquis/stdDoses) =====
    // Cada cadena lista los dígitos 0..9 seleccionados en el control OptionNumsHoriz0_9 legacy.

    [ObservableProperty]
    private string _pesoGlobal = string.Empty;

    [ObservableProperty]
    private string _pesoVariantes = string.Empty;

    [ObservableProperty]
    private string _pesoUnos = string.Empty;

    [ObservableProperty]
    private string _pesoEquis = string.Empty;

    [ObservableProperty]
    private string _pesoDoses = string.Empty;

    // ===== Tolerancias homónimas (legacy: stdGlobalTol/stdVariantesTol/...) =====

    [ObservableProperty]
    private string _pesoGlobalTol = string.Empty;

    [ObservableProperty]
    private string _pesoVariantesTol = string.Empty;

    [ObservableProperty]
    private string _pesoUnosTol = string.Empty;

    [ObservableProperty]
    private string _pesoEquisTol = string.Empty;

    [ObservableProperty]
    private string _pesoDosesTol = string.Empty;

    /// <summary>Tolerancias numéricas 0..5 (legacy: opt0..opt5).</summary>
    public ObservableCollection<OpcionSeleccionableViewModel> ToleranciasNumericas { get; }

    /// <summary>Figuras seleccionables (legacy: lblFig*).</summary>
    public ObservableCollection<OpcionSeleccionableViewModel> Figuras { get; }

    /// <summary>Nota informativa (legacy: label33).</summary>
    public string NotaFiguras =>
        "No se incluyen las figuras 5 y 4-1, que darían 0 columnas.";

    /// <summary>Ayuda del form (legacy: ctrlAyuda1.TextoAyuda).</summary>
    public string TextoAyuda =>
        "El Peso Numérico de una columna es una representación numérica de la columna. " +
        "También se puede expresar el Peso Numérico de Variantes, 1, X y 2.";

    /// <summary>
    /// Vuelca los valores y figuras de pantalla a un FiltroPesosNumericos.
    /// Réplica de PesosNumFrm.ActualizarDatos() (PesosNumFrm.cs líneas 136-267).
    /// </summary>
    private void VolcarA(FiltroPesosNumericos filtro)
    {
        var figuras = ConstruirFiguras();
        string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();

        filtro.ReinicializaValores();
        if (NecesitaGuardarDatos(figuras))
        {
            if (filtro.ContieneDatos == false)
            {
                filtro.IsActive = true;
            }
            filtro.ContieneDatos = true;

            filtro.SetPNGlobal(ValorOTodos(PesoGlobal, todosValores));
            filtro.SetPNVar(ValorOTodos(PesoVariantes, todosValores));
            filtro.SetPNUnos(ValorOTodos(PesoUnos, todosValores));
            filtro.SetPNEquis(ValorOTodos(PesoEquis, todosValores));
            filtro.SetPNDoses(ValorOTodos(PesoDoses, todosValores));

            if (NecesitaGuardarDatosTol())
            {
                filtro.SetPNGlobalTol(PesoGlobalTol ?? "");
                filtro.SetPNVarTol(PesoVariantesTol ?? "");
                filtro.SetPNUnosTol(PesoUnosTol ?? "");
                filtro.SetPNEquisTol(PesoEquisTol ?? "");
                filtro.SetPNDosesTol(PesoDosesTol ?? "");

                string tol = ToleranciaValores();
                filtro.PonerTolerancia(!string.IsNullOrWhiteSpace(tol) ? tol : "0");
            }
            else
            {
                filtro.PonerTolerancia("0");
            }

            filtro.Figuras = figuras;
        }
        else
        {
            filtro.IsActive = false;
            filtro.ContieneDatos = false;
        }
    }

    // Construye un FiltroPesosNumericos temporal con los valores de pantalla (ObtenerFiltroTemporal legacy, línea 268).
    private FiltroPesosNumericos ObtenerFiltroTemporal()
    {
        var filtroTemp = new FiltroPesosNumericos();
        VolcarA(filtroTemp);
        return filtroTemp;
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a PesosNumFrm.menuCondiciones1_BOk -> ActualizarDatos() + ActivaFiltro
        //   (Free1X2/UI/Filtros/PesosNumFrm.cs líneas 136-267, 1086-1092).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroPesosNumericos)grupo.GetFiltro(Filtro.PesosNumericos.ToString());
        VolcarA(filtro);

        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    // Guarda en disco el filtro temporal (PesosNumFrm.guardar(), líneas 1137-1142).
    private void GuardarEn(string nombreArchivo)
    {
        var filtroTemp = ObtenerFiltroTemporal();
        var archComb = new ArchivoCondiciones { NombreArchivo = nombreArchivo };
        archComb.GuardaArchivo(filtroTemp);
    }

    // Abre la condición desde disco y vuelca sus valores (PesosNumFrm.abrir(), líneas 1125-1134).
    private void AbrirDesde(string nombreArchivo)
    {
        var archComb = new ArchivoCondiciones();
        if (archComb.AbrirArchivoCombinacion(nombreArchivo))
        {
            Grupo g = archComb.LeeCondicion();
            var filtro = (FiltroPesosNumericos)g.GetFiltro("PesosNumericos");
            MarcarValores(filtro);
        }
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // Equivale a PesosNumFrm.menuCondiciones1_BEstadisticas (PesosNumFrm.cs líneas 1244-1254).
        var filtroTemp = ObtenerFiltroTemporal();
        var calc = new CalculadorEstadisticas();
        List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, DirectorioGanadoras);

        VisorEstadisticasViewModel.UltimasEstadisticas = lista;
        Navegar?.Invoke(typeof(VisorEstadisticasPage));
    }

    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a PesosNumFrm.menuCondiciones1_BGuardar (PesosNumFrm.cs líneas 1115-1123).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "PesosNumericos",
        };
        picker.FileTypeChoices.Add("Pesos Numéricos", new List<string> { ".pes" });
        picker.FileTypeChoices.Add("Pesos Numéricos (XML)", new List<string> { ".xml" });
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
        // Equivale a PesosNumFrm.menuCondiciones1_BAbrir (PesosNumFrm.cs líneas 1104-1113).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".pes");
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
        // Equivale a PesosNumFrm.menuCondiciones1_BCopiar (PesosNumFrm.cs líneas ~1160).
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
        // Equivale a PesosNumFrm.menuCondiciones1_BPegar (PesosNumFrm.cs líneas ~1175).
        if (File.Exists(RutaTemporal))
        {
            AbrirDesde(RutaTemporal);
        }
    }

    [RelayCommand]
    private void Borrar()
    {
        // Legacy PesosNumFrm.menuCondiciones1_BBorrar(): reinicia el filtro y limpia la pantalla.
        PesoGlobal = string.Empty;
        PesoVariantes = string.Empty;
        PesoUnos = string.Empty;
        PesoEquis = string.Empty;
        PesoDoses = string.Empty;
        PesoGlobalTol = string.Empty;
        PesoVariantesTol = string.Empty;
        PesoUnosTol = string.Empty;
        PesoEquisTol = string.Empty;
        PesoDosesTol = string.Empty;
        foreach (var t in ToleranciasNumericas) t.Seleccionada = false;
        foreach (var f in Figuras) f.Seleccionada = false;
        _figuras = new List<long>();
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }
}
