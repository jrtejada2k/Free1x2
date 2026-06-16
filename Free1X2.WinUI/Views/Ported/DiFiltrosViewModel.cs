using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Microsoft.UI.Dispatching;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Representa una fila del grid "Filtros" del WinForms legacy DiFiltros.
/// Columnas legacy: N (orden), Path (ruta completa, oculta), F (nombre archivo),
/// A (activo/bool), C (cols. contadas), D (difs, máscara "1111" de niveles 0-3),
/// M (min. admitidas), R (adm. = resultado), 14 (acierta 14), 13 (aciertos 13).
/// </summary>
public partial class DiFiltroFilaViewModel : ObservableObject
{
    [ObservableProperty]
    private int _numero;

    // Regla anti-crash #2: no bindear int directo a Text.
    public string NumeroTexto => Numero.ToString();

    partial void OnNumeroChanged(int value) => OnPropertyChanged(nameof(NumeroTexto));

    // Path legacy: ruta completa del archivo de columnas (oculta en el grid, ancho 0).
    [ObservableProperty]
    private string _ruta = string.Empty;

    // F legacy: nombre del archivo de columnas.
    [ObservableProperty]
    private string _nombre = string.Empty;

    // A legacy: filtro activo/inactivo (bool, editable haciendo clic en la fila).
    [ObservableProperty]
    private bool _activo = true;

    // C legacy: nº de columnas leídas del archivo tras el proceso.
    [ObservableProperty]
    private int _columnas;

    public string ColumnasTexto => Columnas.ToString();

    partial void OnColumnasChanged(int value) => OnPropertyChanged(nameof(ColumnasTexto));

    // D legacy: máscara de "difs" "1111" — habilita los niveles de diferencia 0/1/2/3
    // (tst0..tst3 en DiFiltros.Calcular). Editable; por defecto "1111".
    [ObservableProperty]
    private string _difs = "1111";

    // M legacy: nº mínimo de columnas admitidas para considerar válida (mincol). Editable.
    [ObservableProperty]
    private double _minimo = 1;

    // R legacy: resultado (columnas admitidas) tras Calcular(). Solo lectura.
    [ObservableProperty]
    private int _admitidas;

    public string AdmitidasTexto => Admitidas.ToString();

    partial void OnAdmitidasChanged(int value) => OnPropertyChanged(nameof(AdmitidasTexto));

    // "14" legacy: 1 si la columna ganadora (prm14) queda admitida, 0 si no.
    [ObservableProperty]
    private int _acierta14;

    public string Acierta14Texto => Acierta14.ToString();

    partial void OnAcierta14Changed(int value) => OnPropertyChanged(nameof(Acierta14Texto));

    // "13" legacy: nº de las 28 variantes a 13 (prm13) que quedan admitidas.
    [ObservableProperty]
    private int _aciertos13;

    public string Aciertos13Texto => Aciertos13.ToString();

    partial void OnAciertos13Changed(int value) => OnPropertyChanged(nameof(Aciertos13Texto));
}

/// <summary>
/// ViewModel portado del WinForms DiFiltros ("Diferencias entre filtros").
/// Carga una lista de archivos de columnas (filtros), toma el primero como base y, para
/// cada filtro siguiente, descarta de las "válidas" las columnas que no estén dentro de
/// una distancia (difs 0-3) configurable respecto a sus columnas, dejando las "diferencias".
/// Incluye un análisis de resultados sobre una columna ganadora (14) y sus 28 variantes a 13.
/// Réplica fiel de la lógica del form legacy (Free1X2/UI/DiFiltros.cs): BitArrays
/// filtro2/validas (3^14), Calcular()/Valida()/Escrutar()/Analizar(), s2n vía
/// ConvertidorDeBases y E/S con ArchivoColumnasTexto.
/// </summary>
public partial class DiFiltrosViewModel : ObservableObject
{
    // Constante 3^14 = 4782969 (tamaño del espacio de columnas de 14 partidos), igual que el legacy.
    private const int EspacioColumnas = 4782969;

    // pot[] legacy: potencias de 3 para cada partido.
    private static readonly int[] Pot =
        { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323 };

    // BitArrays de trabajo del legacy (dimensionados a 3^14).
    private readonly BitArray _filtro2 = new(EspacioColumnas);
    private readonly BitArray _validas = new(EspacioColumnas);

    private int _mincol;
    private bool _tst0, _tst1, _tst2, _tst3;
    private bool _salida, _analisis;
    private int _ctFR;

    // Análisis de resultados (groupBox3 legacy).
    private readonly string[] _colgsR = new string[3000];
    private int _limcgsR, _nrfCGR;
    private int _prm14;
    private readonly int[] _prm13 = new int[28];

    // dsDatos "Filtros" legacy -> colección de filas.
    public ObservableCollection<DiFiltroFilaViewModel> Filtros { get; } = new();

    // ckMD legacy: "Activa / Desactiva" todas las filas a la vez (Checked por defecto).
    [ObservableProperty]
    private bool _activarTodas = true;

    // lTime legacy: tiempo transcurrido del proceso (formateado, solo lectura).
    [ObservableProperty]
    private string _tiempo = string.Empty;

    // lFileR legacy: nombre del archivo grabado con las columnas resultantes.
    [ObservableProperty]
    private string _archivoSalida = string.Empty;

    // bIniciar legacy: indica si el proceso está en curso (deshabilita acciones).
    [ObservableProperty]
    private bool _procesando;

    public bool HayFiltros => Filtros.Count > 0;

    // --- Sección "Analisis resultados" (groupBox3) ---

    // lFGR legacy: nombre del fichero de columnas ganadoras cargado.
    [ObservableProperty]
    private string _ficheroGanadoras = "Fichero ganadoras";

    // tbCG legacy: columna ganadora actual (texto de 14 signos, MaxLength=14).
    [ObservableProperty]
    private string _columnaGanadora = "Col. Ganadora";

    // lbCGR legacy: índice (1-based) de la columna ganadora mostrada (nrfCGR).
    [ObservableProperty]
    private int _indiceGanadora;

    public string IndiceGanadoraTexto => IndiceGanadora.ToString();

    partial void OnIndiceGanadoraChanged(int value) => OnPropertyChanged(nameof(IndiceGanadoraTexto));

    // bAnalizar legacy: habilitado solo tras cargar un fichero de ganadoras válido.
    [ObservableProperty]
    private bool _puedeAnalizar;

    public DiFiltrosViewModel()
    {
        Filtros.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HayFiltros));
    }

    private static DispatcherQueue? Disp => AppServices.UiDispatcher;

    // s2n legacy de DiFiltros (usa ConvertidorDeBases, distinto al de CombinarFiltros).
    private static int S2n(string ax)
    {
        var conv = new ConvertidorDeBases();
        return conv.ConvColumnaANumero(ax);
    }

    // ====== btnCargarFiltro -> CargarFiltro() legacy ======
    [RelayCommand]
    private async Task CargarFiltro()
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
        if (files == null || files.Count == 0) return;

        foreach (StorageFile file in files)
        {
            Filtros.Add(new DiFiltroFilaViewModel
            {
                Numero = Filtros.Count + 1,
                Ruta = file.Path,
                Nombre = Path.GetFileName(file.Path),
                Activo = true,
                Columnas = 0,
                Difs = "1111",
                Minimo = 1,
            });
        }
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
        try
        {
            // legacy: líneas "Path;F;A;D;M" separadas por ';'.
            foreach (string linea in File.ReadLines(file.Path))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                string[] aux = linea.Split(';');
                if (aux.Length < 5) continue;

                Filtros.Add(new DiFiltroFilaViewModel
                {
                    Numero = Filtros.Count + 1,
                    Ruta = aux[0],
                    Nombre = aux[1],
                    Activo = aux[2].Equals("True", StringComparison.OrdinalIgnoreCase) || aux[2] == "1",
                    Columnas = 0,
                    Difs = aux[3],
                    Minimo = double.TryParse(aux[4], out double m) ? m : 1,
                    Admitidas = 0,
                    Acierta14 = 0,
                    Aciertos13 = 0,
                });
            }
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
            foreach (DiFiltroFilaViewModel fila in Filtros)
            {
                // legacy escribe "Path;F;A;D;M".
                lineas.Add(fila.Ruta + ";" + fila.Nombre + ";" + fila.Activo + ";" + fila.Difs + ";" + (int)fila.Minimo);
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
        await IniciarProcesoAsync();
    }

    private async Task IniciarProcesoAsync()
    {
        if (Filtros.Count == 0) return;

        _salida = false;
        Procesando = true;
        var filas = new List<DiFiltroFilaViewModel>(Filtros);
        bool analisis = _analisis;
        DateTime dt0 = DateTime.Now;

        try
        {
            await Task.Run(() =>
            {
                _validas.SetAll(false);

                for (int nf = 0; nf < filas.Count; nf++)
                {
                    if (_salida) break;
                    DiFiltroFilaViewModel fila = filas[nf];
                    Disp?.TryEnqueue(() => fila.Columnas = 0);

                    if (!fila.Activo) continue;

                    IArchivoColumnas sr;
                    try { sr = new ArchivoColumnasTexto(fila.Ruta); }
                    catch { Disp?.TryEnqueue(() => fila.Activo = false); continue; }

                    _filtro2.SetAll(false);
                    int ctcols = 0;
                    while (sr.SiguienteColumna())
                    {
                        if (_salida) break;
                        string columna = sr.LeeColumnaSinComas();
                        ctcols++;
                        if (columna.Length < 14) break; // legacy: error de longitud
                        int indice = S2n(columna);
                        if (nf == 0) _validas[indice] = true;
                        else _filtro2[indice] = true;
                    }
                    sr.Cerrar();
                    int ctcolsFinal = ctcols;
                    Disp?.TryEnqueue(() => fila.Columnas = ctcolsFinal);

                    if (nf == 0)
                    {
                        _ctFR = ctcols;
                        Disp?.TryEnqueue(() => fila.Admitidas = ctcolsFinal);
                    }
                    else
                    {
                        Calcular(fila);
                    }

                    if (analisis) Escrutar(fila);
                }
            });

            // veureelmeu(): formatea el tiempo transcurrido como el legacy (10 chars).
            string temp = (DateTime.Now - dt0) + "0000000000";
            Tiempo = temp.Substring(0, 10);
        }
        finally
        {
            Procesando = false;
        }
    }

    // ====== Calcular(nf) legacy ======
    private void Calcular(DiFiltroFilaViewModel fila)
    {
        _mincol = (int)fila.Minimo;
        if (_mincol < 1) _mincol = 1;
        if (_mincol > 3305) _mincol = 3305;

        string tmp = fila.Difs;
        if (tmp.Length < 4) tmp = (tmp + "0000").Substring(0, 4);
        _tst0 = tmp[0] == '1';
        _tst1 = tmp[1] == '1';
        _tst2 = tmp[2] == '1';
        _tst3 = tmp[3] == '1';

        for (int nc = 0; nc < EspacioColumnas; nc++)
        {
            if (_salida) break;
            if (_validas[nc] && !Valida(nc))
            {
                _validas[nc] = false;
                _ctFR--;
            }
        }
        int ctFRFinal = _ctFR;
        Disp?.TryEnqueue(() => fila.Admitidas = ctFRFinal);
    }

    // ====== Valida(nsel) legacy ======
    private bool Valida(int nsel)
    {
        if (_filtro2[nsel] && _tst0) return true;
        if (_tst1 || _tst2 || _tst3)
        {
            int na13, na12, na11;
            na13 = na12 = na11 = 0;
            for (int nr = 0; nr < 14; nr++)
            {
                int sign1 = (nsel / Pot[nr]) % 3;
                for (int z1 = 0; z1 < 3; z1++)
                {
                    if (z1 == sign1) continue;
                    int col1 = nsel + Pot[nr] * (z1 - sign1);
                    if (_filtro2[col1] && _tst1) na13++;
                    if (_tst2 || _tst3)
                    {
                        for (int nr2 = nr + 1; nr2 < 14; nr2++)
                        {
                            int sign2 = (col1 / Pot[nr2]) % 3;
                            for (int z2 = 0; z2 < 3; z2++)
                            {
                                if (z2 == sign2) continue;
                                int col2 = col1 + Pot[nr2] * (z2 - sign2);
                                if (_filtro2[col2] && _tst2) na12++;
                                if (_tst3)
                                {
                                    for (int nr3 = nr2 + 1; nr3 < 14; nr3++)
                                    {
                                        int sign3 = (col2 / Pot[nr3]) % 3;
                                        for (int z3 = 0; z3 < 3; z3++)
                                        {
                                            if (z3 == sign3) continue;
                                            int col3 = col2 + Pot[nr3] * (z3 - sign3);
                                            if (_filtro2[col3]) na11++;
                                        }
                                    }
                                    if ((na11 + na12 + na13) >= _mincol) return true;
                                }
                            }
                            if ((na11 + na12 + na13) >= _mincol) return true;
                        }
                    }
                    if ((na11 + na12 + na13) >= _mincol) return true;
                }
            }
        }
        return false;
    }

    // ====== Escrutar(nf) legacy ======
    private void Escrutar(DiFiltroFilaViewModel fila)
    {
        int ct13 = 0;
        int a14 = _validas[_prm14] ? 1 : 0;
        for (int nr = 0; nr < 28; nr++)
        {
            int n = _prm13[nr];
            if (_validas[n]) ct13++;
        }
        int ct13Final = ct13;
        Disp?.TryEnqueue(() =>
        {
            fila.Acierta14 = a14;
            fila.Aciertos13 = ct13Final;
        });
    }

    // ====== bCancelar -> salida = true ======
    [RelayCommand]
    private void Cancelar()
    {
        // legacy BCancelarClick: pone 'salida' para abortar el bucle.
        _salida = true;
        Procesando = false;
    }

    // ====== bGrabar -> Grabar() legacy ======
    [RelayCommand]
    private async Task Grabar()
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

        string ruta = file.Path;
        try
        {
            await Task.Run(() =>
            {
                IArchivoColumnas sw = new ArchivoColumnasTexto(ruta);
                sw.GuardarTodasCols(_validas);
                sw.Cerrar();
            });
            ArchivoSalida = Path.GetFileName(ruta);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo grabar: " + ex.Message);
        }
    }

    // ====== ckMD -> MarcaDesmarca() legacy ======
    [RelayCommand]
    private void MarcarDesmarcar()
    {
        foreach (DiFiltroFilaViewModel fila in Filtros)
        {
            fila.Activo = ActivarTodas;
        }
    }

    // ====== bFG -> EntraCGsR() legacy ======
    [RelayCommand]
    private async Task CargarGanadoras()
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            _limcgsR = 0;
            foreach (string lineaRaw in File.ReadLines(file.Path))
            {
                string tmp = VerColumna(lineaRaw);
                if (tmp.Length == 0)
                {
                    AppServices.MostrarError("col.G. errónea");
                    return;
                }
                if (_limcgsR >= _colgsR.Length) break;
                _colgsR[_limcgsR] = tmp;
                _limcgsR++;
            }
            if (_limcgsR == 0) return;

            _nrfCGR = _limcgsR;
            FicheroGanadoras = Path.GetFileName(file.Path);
            IndiceGanadora = _nrfCGR;
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
            PuedeAnalizar = true;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo cargar el fichero de ganadoras: " + ex.Message);
        }
    }

    // VerColumna legacy: valida una columna de 14 signos en "12xX".
    private static string VerColumna(string columna)
    {
        const string chval = "12xX";
        string xcol = columna.Trim();
        if (xcol.Length < 14) return "";
        xcol = xcol.Substring(0, 14);
        for (int nr = 0; nr < 14; nr++)
        {
            if (chval.IndexOf(xcol[nr]) < 0) return "";
        }
        return xcol;
    }

    // ====== bAnalizar -> Analizar() legacy ======
    [RelayCommand]
    private async Task Analizar()
    {
        // legacy Analizar(): prm14 = s2n(colGanadora); 28 variantes a 13; analisis=true; IniciarProceso().
        _prm14 = S2n(ColumnaGanadora);
        int idx = 0;
        for (int nr = 0; nr < 14; nr++)
        {
            int sign1 = (_prm14 / Pot[nr]) % 3;
            for (int z1 = 0; z1 < 3; z1++)
            {
                if (z1 == sign1) continue;
                _prm13[idx] = _prm14 + Pot[nr] * (z1 - sign1);
                idx++;
            }
        }

        _analisis = true;
        try
        {
            await IniciarProcesoAsync();
        }
        finally
        {
            _analisis = false;
        }
    }

    // ====== bMasR -> GRMas() legacy ======
    [RelayCommand]
    private void GanadoraSiguiente()
    {
        if (_nrfCGR < _limcgsR)
        {
            _nrfCGR++;
            IndiceGanadora = _nrfCGR;
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    // ====== bMenosR -> GRMenos() legacy ======
    [RelayCommand]
    private void GanadoraAnterior()
    {
        if (_nrfCGR > 1)
        {
            _nrfCGR--;
            IndiceGanadora = _nrfCGR;
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }
}
