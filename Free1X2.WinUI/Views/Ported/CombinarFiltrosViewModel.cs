// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Microsoft.UI.Dispatching;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Representa una fila del grid "Filtros" del WinForms legacy CombinarFiltros.
/// Columnas legacy: N (orden), F (nombre archivo), FP (ruta completa),
/// A (activo/bool), C (cols. contadas tras el proceso).
/// </summary>
public partial class FiltroFilaViewModel : ObservableObject
{
    [ObservableProperty]
    private int _numero;

    // Regla anti-crash #2: no bindear int directo a Text.
    public string NumeroTexto => Numero.ToString();

    partial void OnNumeroChanged(int value) => OnPropertyChanged(nameof(NumeroTexto));

    [ObservableProperty]
    private string _nombre = string.Empty;

    // FP legacy: ruta completa del archivo de columnas (no se muestra en el grid legacy).
    [ObservableProperty]
    private string _ruta = string.Empty;

    [ObservableProperty]
    private bool _activo = true;

    [ObservableProperty]
    private int _columnas;

    // Regla anti-crash #2: no bindear int directo a Text. Exponemos string.
    public string ColumnasTexto => Columnas.ToString();

    partial void OnColumnasChanged(int value) => OnPropertyChanged(nameof(ColumnasTexto));
}

/// <summary>
/// ViewModel portado del WinForms CombinarFiltros ("Combinación de Filtros").
/// Combina varios archivos de columnas (.txt) contando en cuántos filtros aparece cada
/// columna, y permite grabar/sumar las que caen en un rango [mínimo, máximo] de
/// "filtros acertados". Réplica fiel de la lógica del form legacy
/// (Free1X2/UI/CombinarFiltros.cs): conteo con htCols[] (3^partidos) + BitArray repes,
/// usando ArchivoColumnasTexto, Pral.Normaliza y el s2n/n2s propios del form.
/// </summary>
public partial class CombinarFiltrosViewModel : ObservableObject
{
    // Estado de dominio del form legacy.
    private int[] _htCols = Array.Empty<int>();
    private int _partidosEnJuego;
    private bool _salida;

    public ObservableCollection<FiltroFilaViewModel> Filtros { get; } = new();

    // tbmin / tbmax legacy (TextBox numéricos, valor por defecto "1").
    [ObservableProperty]
    private double _minimo = 1;

    [ObservableProperty]
    private double _maximo = 1;

    // ckMD legacy: "Activa / Desactiva" todas las filas a la vez.
    [ObservableProperty]
    private bool _activarTodas = true;

    // lCols legacy: total de columnas resultantes.
    [ObservableProperty]
    private int _totalColumnas;

    public string TotalColumnasTexto => TotalColumnas.ToString();

    partial void OnTotalColumnasChanged(int value) => OnPropertyChanged(nameof(TotalColumnasTexto));

    // lfilout legacy: nombre del archivo de salida grabado.
    [ObservableProperty]
    private string _archivoSalida = string.Empty;

    // bIniciar legacy: solo procede si hay filtros cargados (limite > 0).
    [ObservableProperty]
    private bool _procesando;

    public bool HayFiltros => Filtros.Count > 0;

    public CombinarFiltrosViewModel()
    {
        Filtros.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HayFiltros));
    }

    private static DispatcherQueue? Disp => AppServices.UiDispatcher;

    // ====== s2n / n2s propios de CombinarFiltros (NO ConvertidorDeBases) ======
    // X=0, 1=+1, 2=+2, dígito más significativo a la izquierda (legacy CombinarFiltros).
    private static int S2n(string ax)
    {
        int nx = 0;
        for (int nr = 0; nr < ax.Length; nr++)
        {
            nx *= 3;
            if (ax[nr] == '1') nx += 1;
            else if (ax[nr] == '2') nx += 2;
        }
        return nx;
    }

    private static string N2s(int nx)
    {
        string ax = "";
        for (int nr = 0; nr < 14; nr++)
        {
            int nx2 = nx % 3; nx /= 3;
            if (nx2 == 1) ax = "1" + ax;
            else if (nx2 == 2) ax = "2" + ax;
            else ax = "X" + ax;
        }
        return ax;
    }

    private void AdaptarANumeroDePartidos()
    {
        // legacy AdaptarANumeroDePartidos(): htCols/repes dimensionados a 3^partidos.
        _htCols = new int[Convert.ToInt32(Math.Pow(3, _partidosEnJuego))];
    }

    // ====== btnCargarFiltro -> CargarFiltro() legacy ======
    [RelayCommand]
    private async Task CargarFiltro()
    {
        // OpenFileDialog multiselección (*.txt). Valida que todos los filtros tengan el
        // mismo número de partidos (ArchivoColumnasTexto.ObtenNumSignos).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
        if (files == null || files.Count == 0) return;

        foreach (StorageFile file in files)
        {
            IArchivoColumnas aCol;
            try { aCol = new ArchivoColumnasTexto(file.Path); }
            catch { continue; }

            int numP = aCol.ObtenNumSignos();
            aCol.Cerrar();

            if (_partidosEnJuego == 0)
            {
                _partidosEnJuego = numP;
            }
            else if (_partidosEnJuego != numP)
            {
                AppServices.MostrarError("Los filtros deben tener el mismo número de partidos");
                return;
            }

            Filtros.Add(new FiltroFilaViewModel
            {
                Numero = Filtros.Count + 1,
                Nombre = Path.GetFileName(file.Path),
                Ruta = file.Path,
                Activo = true,
                Columnas = 0,
            });
        }
        AdaptarANumeroDePartidos();
    }

    // ====== bCargaLista -> CargarLista() legacy ======
    [RelayCommand]
    private async Task CargarLista()
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".lst");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSingleFileAsync();
        if (file == null) return;

        Filtros.Clear();
        _partidosEnJuego = 0;
        int lineasLeidas = 0;

        try
        {
            foreach (string ruta in File.ReadLines(file.Path))
            {
                if (string.IsNullOrWhiteSpace(ruta)) continue;
                IArchivoColumnas aCol = new ArchivoColumnasTexto(ruta);
                int numP = aCol.ObtenNumSignos();
                aCol.Cerrar();

                if (lineasLeidas == 0 && _partidosEnJuego == 0)
                {
                    _partidosEnJuego = numP;
                }
                else if (numP != _partidosEnJuego)
                {
                    AppServices.MostrarError("Los archivos no tienen el mismo número de partidos");
                    return;
                }
                lineasLeidas++;

                Filtros.Add(new FiltroFilaViewModel
                {
                    Numero = Filtros.Count + 1,
                    Nombre = Path.GetFileName(ruta),
                    Ruta = ruta,
                    Activo = true,
                    Columnas = 0,
                });
            }
            AdaptarANumeroDePartidos();
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo cargar la lista: " + ex.Message);
        }
    }

    // ====== bSalvaLista -> SalvarLista() legacy ======
    [RelayCommand]
    private async Task SalvarLista()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Lista",
        };
        picker.FileTypeChoices.Add("SalvarLista", new List<string> { ".lst" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            var lineas = new List<string>();
            foreach (FiltroFilaViewModel fila in Filtros)
            {
                lineas.Add(fila.Ruta); // legacy escribe la columna FP (ruta) de cada fila.
            }
            await FileIO.WriteLinesAsync(file, lineas);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo guardar la lista: " + ex.Message);
        }
    }

    // ====== bIniciar -> IniciarProceso() legacy ======
    [RelayCommand]
    private async Task Iniciar()
    {
        // legacy BIniciarClick: solo si limite > 0.
        if (Filtros.Count == 0) return;

        _salida = false;
        Procesando = true;

        // Captura inmutable de las filas activas (ruta) para el hilo de trabajo.
        var activos = new List<FiltroFilaViewModel>(Filtros);

        try
        {
            await Task.Run(() =>
            {
                int[] htCols = _htCols;
                for (int nr = 0; nr < htCols.Length; nr++) htCols[nr] = 0;

                var repes = new BitArray(htCols.Length);
                foreach (FiltroFilaViewModel fila in activos)
                {
                    if (_salida) break;
                    int ctcols = 0;
                    FiltroFilaViewModel filaLocal = fila;
                    Disp?.TryEnqueue(() => filaLocal.Columnas = 0);

                    if (!fila.Activo) continue;

                    IArchivoColumnas sr;
                    try { sr = new ArchivoColumnasTexto(fila.Ruta); }
                    catch { continue; }

                    repes.SetAll(false);
                    while (sr.SiguienteColumna())
                    {
                        if (_salida) break;
                        string columna = Pral.Normaliza(sr.LeeColumnaSinComas());
                        ctcols++;
                        if (columna.Length < 14) break; // legacy: error de longitud
                        int indice = S2n(columna);
                        if (!repes[indice])
                        {
                            htCols[indice]++;
                            repes[indice] = true;
                        }
                    }
                    sr.Cerrar();
                    int ctcolsFinal = ctcols;
                    Disp?.TryEnqueue(() => filaLocal.Columnas = ctcolsFinal);
                }
            });
        }
        finally
        {
            Procesando = false;
        }
    }

    // ====== bCancelar -> salida = true ======
    [RelayCommand]
    private void Cancelar()
    {
        // legacy BCancelarClick: pone el flag 'salida' para abortar el bucle.
        _salida = true;
        Procesando = false;
    }

    // ====== btnGrabarFiltro -> Grabar() legacy ======
    [RelayCommand]
    private async Task GrabarColumnas()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Columnas",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSaveFileAsync();
        if (file == null) return;

        int min = (int)Minimo;
        int max = (int)Maximo;
        int limite = Filtros.Count;
        if (min < 1) min = 1;
        if (max > limite) max = limite;

        int[] htCols = _htCols;
        int partidos = _partidosEnJuego;
        string ruta = file.Path;

        try
        {
            int tot = await Task.Run(() =>
            {
                int total = 0;
                IArchivoColumnas sw = new ArchivoColumnasTexto(ruta);
                int tope = Convert.ToInt32(Math.Pow(3, partidos));
                for (int nr = 0; nr < tope; nr++)
                {
                    int qnt = htCols[nr];
                    if (qnt >= min && qnt <= max)
                    {
                        sw.GuardarCols(N2s(nr));
                        total++;
                    }
                }
                sw.Cerrar();
                return total;
            });

            TotalColumnas = tot;
            ArchivoSalida = Path.GetFileName(ruta);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo grabar: " + ex.Message);
        }
    }

    // ====== bSumaCols -> SumaCols() legacy ======
    [RelayCommand]
    private void SumarColumnas()
    {
        int min = (int)Minimo;
        int max = (int)Maximo;
        int limite = Filtros.Count;
        if (min < 1) min = 1;
        if (max > limite) max = limite;

        int tot = 0;
        int tope = Convert.ToInt32(Math.Pow(3, _partidosEnJuego));
        for (int nr = 0; nr < tope && nr < _htCols.Length; nr++)
        {
            int qnt = _htCols[nr];
            if (qnt >= min && qnt <= max) tot++;
        }
        TotalColumnas = tot;
    }

    // ====== btnReiniciaTodo -> btnReiniciaTodo_Click legacy ======
    [RelayCommand]
    private void ReiniciarTodo()
    {
        Filtros.Clear();
        TotalColumnas = 0;
        ArchivoSalida = string.Empty;
        _partidosEnJuego = 0;
        _htCols = Array.Empty<int>();
    }

    // ====== ckMD -> MarcaDesmarca() legacy ======
    [RelayCommand]
    private void MarcarDesmarcar()
    {
        foreach (FiltroFilaViewModel fila in Filtros)
        {
            fila.Activo = ActivarTodas;
        }
    }
}
