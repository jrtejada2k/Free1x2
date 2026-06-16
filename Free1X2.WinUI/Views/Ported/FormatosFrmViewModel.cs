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
/// Una línea de formato dentro de un <see cref="FormatosViewModel"/>:
/// la secuencia de signos (1/X/2/V/*) y su rango de apariciones Min-Max.
/// Equivale al tipo de dominio legacy Free1X2.MotorCalculo.FormatoSignos.
/// </summary>
public partial class LineaFormatoViewModel : ObservableObject
{
    [ObservableProperty]
    private string _formato = string.Empty;

    [ObservableProperty]
    private string _rangoAparicion = string.Empty;
}

/// <summary>
/// Un conjunto de líneas de formato (una "relación"), con sus límites
/// globales de Líneas y Global. Equivale a Free1X2.MotorCalculo.FormatosSignos.
/// </summary>
public partial class FormatosViewModel : ObservableObject
{
    public ObservableCollection<LineaFormatoViewModel> Lineas { get; } = new();

    /// <summary>Límite de líneas para esta relación de formatos.</summary>
    [ObservableProperty]
    private string _limiteLineas = string.Empty;

    /// <summary>Límite global para esta relación de formatos.</summary>
    [ObservableProperty]
    private string _global = string.Empty;

    public FormatosViewModel()
    {
        // El form legacy muestra 30 filas en blanco por relación.
        for (int i = 0; i < FilasPorRelacion; i++)
        {
            Lineas.Add(new LineaFormatoViewModel());
        }
    }

    public const int FilasPorRelacion = 30;
}

/// <summary>
/// ViewModel de la pantalla "Formatos (1,X,2,V,*)".
///
/// Un formato es una determinada secuencia de signos; esta condición controla
/// la repetición o aparición de diferentes formatos en las columnas generadas.
/// El usuario define una o varias relaciones de formatos navegables (1/N) y, por
/// cada una, hasta 30 líneas (secuencia + rango Min-Max) más los límites Líneas/Global.
///
/// Datos en memoria; la persistencia y el cálculo aún viven en el dominio legacy
/// (ver los TODO en FormatosFrmPage.xaml.cs).
/// </summary>
public partial class FormatosFrmViewModel : ObservableObject
{
    public ObservableCollection<FormatosViewModel> Relaciones { get; } = new();

    [ObservableProperty]
    private int _indiceRelacion;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    // Fichero temporal de copiar/pegar (legacy: StartupPath + "/Temp/tmp.fmt").
    private static string RutaTemporal =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.fmt");

    // Directorio de columnas ganadoras (legacy: StartupPath + "/Ganadoras/").
    private static string DirectorioGanadoras =>
        Path.Combine(AppContext.BaseDirectory, "Ganadoras") + Path.DirectorySeparatorChar;

    public FormatosFrmViewModel()
    {
        Relaciones.Add(new FormatosViewModel());
        IndiceRelacion = 0;
    }

    /// <summary>
    /// Vuelca las relaciones de formatos del FiltroFormatosSignos del grupo en edición.
    /// Equivale a FormatosFrm.InicializaDatos()/ObtenCopiaFormatos()/ActualizaDatosPantalla()
    /// (Free1X2/UI/Filtros/FormatosFrm.cs líneas 112-171).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroFormatosSignos)grupo.GetFiltro(Filtro.FormatosSignos.ToString());

        Relaciones.Clear();
        foreach (FormatosSignos formatos in filtro.FormatosSignos)
        {
            var rel = new FormatosViewModel();
            rel.Lineas.Clear();
            foreach (FormatoSignos linea in formatos.LineasFormatos)
            {
                rel.Lineas.Add(new LineaFormatoViewModel
                {
                    Formato = linea.Formato,
                    RangoAparicion = linea.RangoAparicion,
                });
            }
            // El form legacy completa hasta 30 filas en blanco para edición.
            while (rel.Lineas.Count < FormatosViewModel.FilasPorRelacion)
            {
                rel.Lineas.Add(new LineaFormatoViewModel());
            }
            rel.LimiteLineas = formatos.Lineas;
            rel.Global = formatos.Global;
            Relaciones.Add(rel);
        }

        if (Relaciones.Count == 0)
        {
            Relaciones.Add(new FormatosViewModel());
        }

        IndiceRelacion = 0;
        OnPropertyChanged(nameof(RelacionActual));
        OnPropertyChanged(nameof(ContadorTexto));
        OnPropertyChanged(nameof(PuedeRetroceder));
        RetrocederCommand.NotifyCanExecuteChanged();
    }

    // Caracteres permitidos en un formato (FormatosFrm.CompruebaFomato línea 489).
    private static readonly char[] CaracteresFormato = { '1', 'X', '2', 'V', '*' };

    private static bool FormatoEsValido(string formato)
    {
        if (formato.Length > VariablesGlobales.NumeroPartidos) return false;
        foreach (char c in formato)
        {
            if (Array.IndexOf(CaracteresFormato, c) < 0) return false;
        }
        return true;
    }

    /// <summary>
    /// Construye la List&lt;FormatosSignos&gt; a partir de las relaciones de pantalla,
    /// replicando ObtenDatosGrid + GuardarDatosFormatos + NecesitaBorrarUltimoFormato del form.
    /// </summary>
    private List<FormatosSignos> ConstruirFormatos()
    {
        var resultado = new List<FormatosSignos>();
        foreach (var rel in Relaciones)
        {
            var formatos = new FormatosSignos();
            var lineas = new List<FormatoSignos>();
            foreach (var linea in rel.Lineas)
            {
                string formato = (linea.Formato ?? "").Trim().ToUpper();
                string rangos = (linea.RangoAparicion ?? "").Trim();
                if (FormatoEsValido(formato) && formato != "" && rangos != "")
                {
                    lineas.Add(new FormatoSignos { Formato = formato, RangoAparicion = rangos });
                }
            }
            formatos.LineasFormatos = lineas;
            // Lineas/Global solo si son numéricos (GuardarDatosFormatos líneas 331-347).
            string limLineas = (rel.LimiteLineas ?? "").Trim();
            formatos.Lineas = UtilidadesEntradasValores.SonTodosNumeros(limLineas) ? limLineas : "";
            string limGlobal = (rel.Global ?? "").Trim();
            formatos.Global = UtilidadesEntradasValores.SonTodosNumeros(limGlobal) ? limGlobal : "";
            resultado.Add(formatos);
        }

        // Borrar la última relación si no tiene ninguna línea con formato (NecesitaBorrarUltimoFormato).
        if (resultado.Count > 0)
        {
            var ultima = resultado[resultado.Count - 1];
            bool tieneFormato = false;
            foreach (var l in ultima.LineasFormatos)
            {
                if (l.Formato != "") { tieneFormato = true; break; }
            }
            if (!tieneFormato) resultado.RemoveAt(resultado.Count - 1);
        }

        return resultado;
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a FormatosFrm.menuCondiciones1_BOk -> GuardarDatos() + ActivaFiltro
        //   (Free1X2/UI/Filtros/FormatosFrm.cs líneas 352-374, 917-922).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroFormatosSignos)grupo.GetFiltro(Filtro.FormatosSignos.ToString());
        var grupoFormatos = ConstruirFormatos();

        if (filtro.ContieneDatos == false && grupoFormatos.Count > 0)
        {
            filtro.ContieneDatos = true;
            filtro.IsActive = true;
        }
        if (grupoFormatos.Count == 0)
        {
            filtro.ContieneDatos = false;
            filtro.IsActive = false;
        }
        filtro.FormatosSignos = grupoFormatos;

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

    /// <summary>Relación de formatos actualmente visible.</summary>
    public FormatosViewModel? RelacionActual =>
        IndiceRelacion >= 0 && IndiceRelacion < Relaciones.Count
            ? Relaciones[IndiceRelacion]
            : null;

    /// <summary>Contador "N/Total" mostrado en la cabecera (legacy lblNoFormatos).</summary>
    public string ContadorTexto =>
        Relaciones.Count == 0 ? "0/0" : $"{IndiceRelacion + 1}/{Relaciones.Count}";

    public bool PuedeRetroceder => IndiceRelacion > 0;

    partial void OnIndiceRelacionChanged(int value)
    {
        OnPropertyChanged(nameof(RelacionActual));
        OnPropertyChanged(nameof(ContadorTexto));
        OnPropertyChanged(nameof(PuedeRetroceder));
        RetrocederCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(PuedeRetroceder))]
    private void Retroceder()
    {
        if (IndiceRelacion > 0)
        {
            IndiceRelacion--;
        }
    }

    [RelayCommand]
    private void Avanzar()
    {
        // El legacy crea una relación nueva al avanzar más allá de la última.
        if (IndiceRelacion + 1 >= Relaciones.Count)
        {
            Relaciones.Add(new FormatosViewModel());
            OnPropertyChanged(nameof(ContadorTexto));
        }
        IndiceRelacion++;
    }

    [RelayCommand]
    private void EliminarActual()
    {
        if (Relaciones.Count == 0)
        {
            return;
        }

        Relaciones.RemoveAt(IndiceRelacion);
        if (Relaciones.Count == 0)
        {
            Relaciones.Add(new FormatosViewModel());
        }

        if (IndiceRelacion >= Relaciones.Count)
        {
            IndiceRelacion = Relaciones.Count - 1;
        }

        OnPropertyChanged(nameof(RelacionActual));
        OnPropertyChanged(nameof(ContadorTexto));
        OnPropertyChanged(nameof(PuedeRetroceder));
        RetrocederCommand.NotifyCanExecuteChanged();
    }

    // ===== Persistencia y estadísticas (menuCondiciones legacy) =====

    /// <summary>
    /// Construye un FiltroFormatosSignos temporal con las relaciones de pantalla.
    /// Réplica de FormatosFrm.ObtenerFiltroTemporal() (FormatosFrm.cs líneas 376-...).
    /// </summary>
    private FiltroFormatosSignos ObtenerFiltroTemporal()
    {
        var filtroTemp = new FiltroFormatosSignos();
        var grupoFormatos = ConstruirFormatos();
        if (grupoFormatos.Count > 0)
        {
            filtroTemp.ContieneDatos = true;
            filtroTemp.IsActive = true;
        }
        filtroTemp.FormatosSignos = grupoFormatos;
        return filtroTemp;
    }

    // Vuelca una lista de FormatosSignos al filtro del grupo en edición.
    private FiltroFormatosSignos? ObtenerFiltroGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return null;
        return (FiltroFormatosSignos)grupo.GetFiltro(Filtro.FormatosSignos.ToString());
    }

    // Guarda en disco el filtro temporal (FormatosFrm.guardar(), líneas 968-978).
    private void GuardarEn(string nombreArchivo)
    {
        var filtroTemp = ObtenerFiltroTemporal();
        var archComb = new ArchivoCondiciones { NombreArchivo = nombreArchivo };
        if (filtroTemp.FormatosSignos.Count > 0)
        {
            filtroTemp.ContieneDatos = true;
            filtroTemp.IsActive = true;
        }
        archComb.GuardaArchivo(filtroTemp);
    }

    // Abre la condición desde disco, vuelca al filtro del grupo y recarga la pantalla
    // (FormatosFrm.abrir(), líneas 955-966).
    private void AbrirDesde(string nombreArchivo)
    {
        var filtroGrupo = ObtenerFiltroGrupo();
        if (filtroGrupo is null) return;

        var archComb = new ArchivoCondiciones();
        if (!archComb.AbrirArchivoCombinacion(nombreArchivo)) return;

        Grupo g = archComb.LeeCondicion();
        var leido = (FiltroFormatosSignos)g.GetFiltro("FormatosSignos");
        filtroGrupo.FormatosSignos = leido.FormatosSignos;
        filtroGrupo.ContieneDatos = leido.ContieneDatos;
        filtroGrupo.IsActive = leido.IsActive;

        CargarDesdeGrupo();
    }

    /// <summary>Calcula estadísticas del filtro temporal de formatos (menuCondiciones1_BEstadisticas).</summary>
    [RelayCommand]
    private void Estadisticas()
    {
        // Equivale a FormatosFrm.menuCondiciones1_BEstadisticas (FormatosFrm.cs líneas 1028-1038).
        var filtroTemp = ObtenerFiltroTemporal();
        var calc = new CalculadorEstadisticas();
        List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, DirectorioGanadoras);

        VisorEstadisticasViewModel.UltimasEstadisticas = lista;
        Navegar?.Invoke(typeof(VisorEstadisticasPage));
    }

    /// <summary>Guarda la condición de formatos a un archivo .fmt/.xml.</summary>
    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a FormatosFrm.menuCondiciones1_BGuardar (FormatosFrm.cs líneas 944-953).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Formatos",
        };
        picker.FileTypeChoices.Add("Formatos", new List<string> { ".fmt" });
        picker.FileTypeChoices.Add("Formatos (XML)", new List<string> { ".xml" });
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

    /// <summary>Abre una condición de formatos desde un archivo .fmt/.xml.</summary>
    [RelayCommand]
    private async Task Abrir()
    {
        // Equivale a FormatosFrm.menuCondiciones1_BAbrir (FormatosFrm.cs líneas 929-942).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".fmt");
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

    /// <summary>Copia la condición de formatos al fichero temporal interno.</summary>
    [RelayCommand]
    private void Copiar()
    {
        // Equivale a FormatosFrm.menuCondiciones1_BCopiar (FormatosFrm.cs líneas 993-1001).
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

    /// <summary>Pega la condición de formatos desde el fichero temporal interno.</summary>
    [RelayCommand]
    private void Pegar()
    {
        // Equivale a FormatosFrm.menuCondiciones1_BPegar (FormatosFrm.cs líneas 1003-1013).
        if (File.Exists(RutaTemporal))
        {
            AbrirDesde(RutaTemporal);
        }
    }

    /// <summary>Borra los datos del filtro de formatos (menuCondiciones1_BBorrar).</summary>
    [RelayCommand]
    private void Borrar()
    {
        // Equivale a FormatosFrm.menuCondiciones1_BBorrar (FormatosFrm.cs líneas 980-991):
        //   reinstancia el FiltroFormatosSignos del grupo y recarga la pantalla vacía.
        var filtroGrupo = ObtenerFiltroGrupo();
        if (filtroGrupo is not null)
        {
            filtroGrupo.FormatosSignos = new List<FormatosSignos>();
            filtroGrupo.ContieneDatos = false;
            filtroGrupo.IsActive = false;
        }
        CargarDesdeGrupo();
    }
}
