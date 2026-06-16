using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una temporada seleccionable en el modo "Combinación única" (legacy chkTemporadas,
/// un CheckedListBox alimentado por la ArrayList ListaTemporadas leída del fichero de
/// valoraciones históricas).
/// </summary>
public partial class TemporadaSeleccionableViewModel : ObservableObject
{
    public TemporadaSeleccionableViewModel(string nombre, bool seleccionada = false)
    {
        _nombre = nombre;
        _seleccionada = seleccionada;
    }

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private bool _seleccionada;
}

/// <summary>
/// Una fila de la lista "Una combinación por jornada" (legacy clase Free1X2.Utils.Combinacion,
/// mostrada en el DataGrid dgListaFicheros con columnas Temporada / Jornada / Fichero).
/// </summary>
public partial class CombinacionFilaViewModel : ObservableObject
{
    public CombinacionFilaViewModel(string temporada, string jornada, string ruta)
    {
        _temporada = temporada;
        _jornada = jornada;
        _ruta = ruta;
    }

    [ObservableProperty]
    private string _temporada = string.Empty;

    [ObservableProperty]
    private string _jornada = string.Empty;

    [ObservableProperty]
    private string _ruta = string.Empty;
}

/// <summary>
/// ViewModel de la pantalla "Definición múltiple de jornadas y ficheros"
/// (legacy: Free1X2.UI.DialogoAnalisisMultipleDeTramosFrm).
///
/// Reúne los datos para lanzar un análisis múltiple de tramos. El usuario elige primero
/// un fichero de valoraciones históricas y luego trabaja en uno de dos modos:
///   • "Combinación única": analiza una misma combinación (14 triples o un fichero) sobre
///     todas las temporadas marcadas (legacy pestaña tab14T).
///   • "Una combinación por jornada": construye una lista de tripletes
///     temporada/jornada/fichero que se puede guardar y leer en disco (legacy tabFicheros).
/// Además define un criterio de selección de jornadas por rangos de recaudación e importes
/// de los premios de 14/13/12/11/10 (legacy grSeleccionJornada).
///
/// Motor de dominio usado: Free1X2.EntradaSalida.ArchivoColumnasTexto (lectura/escritura de
/// .txt y .lst vía SiguienteColumna/LeeColumnaSinComas/GuardarCols/Cerrar) y
/// Free1X2.Utils.Combinacion (triplete temporada/jornada/ruta). Replica las rutinas
/// CargarListaDeTemporadas, EsFicheroDeColumnas, btAdd/btGuardar/btLeer del form legacy.
/// </summary>
public partial class DialogoAnalisisMultipleDeTramosFrmViewModel : ObservableObject
{
    // ===== Fichero de valoraciones históricas (legacy txNombreFicheroValoraciones) =====
    [ObservableProperty]
    private string _ficheroValoraciones = string.Empty;

    // Ruta completa del fichero de valoraciones (legacy FicheroValoracionesHistoricas).
    // FicheroValoraciones muestra solo el nombre; esta guarda la ruta para el motor/llamador.
    private string _rutaValoraciones = string.Empty;

    // ===== Modo "Combinación única" (legacy tab14T) =====

    /// <summary>Temporadas leídas del fichero (legacy chkTemporadas / ListaTemporadas).</summary>
    public ObservableCollection<TemporadaSeleccionableViewModel> Temporadas { get; } = new();

    /// <summary>Origen de la combinación: true = 14 triples, false = fichero (legacy rb14Triples / rbFichero).</summary>
    [ObservableProperty]
    private bool _usar14Triples = true;

    // Mantiene Es14Triples en sync mientras la pestaña activa es "Combinación única"
    // (legacy: los CheckedChanged de rb14Triples/rbFichero + tabControl1_SelectedIndexChanged).
    partial void OnUsar14TriplesChanged(bool value)
    {
        if (PestanaActiva == 0)
        {
            Es14Triples = value ? (byte)0 : (byte)1;
        }
    }

    /// <summary>Fichero de combinación cuando NO se usan 14 triples (legacy txFichero / FicheroCombinación).</summary>
    [ObservableProperty]
    private string _ficheroCombinacion = string.Empty;

    // Ruta completa del fichero de combinación (legacy FicheroCombinación).
    private string _rutaCombinacion = string.Empty;

    // ===== Modo "Una combinación por jornada" (legacy tabFicheros) =====

    /// <summary>Lista de tripletes temporada/jornada/fichero (legacy ListaCombinaciones).</summary>
    public ObservableCollection<CombinacionFilaViewModel> Combinaciones { get; } = new();

    /// <summary>Fila seleccionada en la lista (legacy dgListaFicheros.CurrentCell/IsSelected).</summary>
    [ObservableProperty]
    private CombinacionFilaViewModel? _combinacionSeleccionada;

    /// <summary>Temporada de inicio (legacy txTemporada). Su +1 se muestra como temporada2.</summary>
    [ObservableProperty]
    private double _temporada = 2004;

    /// <summary>Jornada actual (legacy txJornada, rango 0..43).</summary>
    [ObservableProperty]
    private double _jornada = 1;

    /// <summary>Incluir el análisis de los ficheros dentro del desarrollo de 14 triples (legacy chkFicherosEn14T).</summary>
    [ObservableProperty]
    private bool _analizarFicherosEn14Triples;

    /// <summary>Texto "AAAA/AAAA+1" de la temporada (legacy txTemporada + txTemporada2). String para TextBlock (regla 2).</summary>
    public string TemporadaTexto => $"{(int)Temporada}/{(int)Temporada + 1}";

    partial void OnTemporadaChanged(double value) => OnPropertyChanged(nameof(TemporadaTexto));

    // ===== Criterio de selección de jornadas (legacy grSeleccionJornada) =====
    // NumberBox.Value es double (regla anti-crash 7).
    [ObservableProperty] private double _recaudacionMinima;
    [ObservableProperty] private double _recaudacionMaxima = 25000000;
    [ObservableProperty] private double _premioMinimoDe14;
    [ObservableProperty] private double _premioMaximoDe14 = 6000000;
    [ObservableProperty] private double _premioMinimoDe13;
    [ObservableProperty] private double _premioMaximoDe13 = 3000000;
    [ObservableProperty] private double _premioMinimoDe12;
    [ObservableProperty] private double _premioMaximoDe12 = 3000000;
    [ObservableProperty] private double _premioMinimoDe11;
    [ObservableProperty] private double _premioMaximoDe11 = 3000000;
    [ObservableProperty] private double _premioMinimoDe10;
    [ObservableProperty] private double _premioMaximoDe10 = 3000000;

    // ===== Contrato con el llamador (legacy: MainForm lee los campos públicos del Form) =====

    /// <summary>Legacy DialogoAnalisisMultipleDeTramosFrm.FicheroValoracionesHistoricas.</summary>
    public string FicheroValoracionesHistoricas => _rutaValoraciones;

    /// <summary>Legacy DialogoAnalisisMultipleDeTramosFrm.FicheroCombinación.</summary>
    public string FicheroCombinacionRuta => _rutaCombinacion;

    /// <summary>
    /// Legacy DialogoAnalisisMultipleDeTramosFrm.Es14Triples: 0 = 14 triples, 1 = fichero,
    /// 2 = una combinación por jornada (pestaña tabFicheros).
    /// </summary>
    public byte Es14Triples { get; private set; }

    /// <summary>Legacy DialogoAnalisisMultipleDeTramosFrm.ListaCombinaciones (Free1X2.Utils.Combinacion).</summary>
    public ArrayList ListaCombinaciones { get; } = new();

    /// <summary>True si el usuario pulsó "Aceptar".</summary>
    public bool Aceptado { get; private set; }

    /// <summary>Pestaña activa (0 = Combinación única, 1 = Una combinación por jornada).</summary>
    [ObservableProperty]
    private int _pestanaActiva;

    /// <summary>Se dispara al aceptar/cancelar para que el host cierre el diálogo.</summary>
    public event EventHandler? CierreSolicitado;

    public DialogoAnalisisMultipleDeTramosFrmViewModel()
    {
    }

    /// <summary>
    /// Permite al llamador inicializar el fichero de valoraciones (legacy: ctor
    /// DialogoAnalisisMultipleDeTramosFrm(pFicheroValoraciones), que asigna la ruta y llama
    /// a CargarListaDeTemporadas).
    /// </summary>
    public void Inicializar(string ficheroValoraciones)
    {
        if (string.IsNullOrEmpty(ficheroValoraciones)) return;
        _rutaValoraciones = ficheroValoraciones;
        FicheroValoraciones = Path.GetFileName(ficheroValoraciones);
        CargarListaDeTemporadas();
    }

    // ===== Comandos del fichero de valoraciones =====

    [RelayCommand]
    private async Task SeleccionarFicheroValoracionesAsync()
    {
        // Legacy btSeleccionarFichero_Click: OpenFileDialog (*.txt) en "Columnas\".
        var file = await ElegirArchivoAsync(".txt");
        if (file == null) return;

        _rutaValoraciones = file.Path;
        FicheroValoraciones = Path.GetFileName(file.Path);
        CargarListaDeTemporadas();
    }

    // ===== Comandos del modo "Combinación única" =====

    [RelayCommand]
    private async Task SeleccionarCombinacionAsync()
    {
        // Legacy btSeleccionarCombi_Click: OpenFileDialog (*.txt) en "Columnas\". Al elegir,
        // pone rbFichero.Checked = true (Usar14Triples = false) y Es14Triples = 1.
        var file = await ElegirArchivoAsync(".txt");
        if (file == null) return;

        _rutaCombinacion = file.Path;
        FicheroCombinacion = Path.GetFileName(file.Path);
        Usar14Triples = false;
        Es14Triples = 1;
    }

    [RelayCommand]
    private void MarcarTodasTemporadas()
    {
        // legacy btTodas_Click / MarcarTodasLasJornadas()
        foreach (var t in Temporadas)
        {
            t.Seleccionada = true;
        }
    }

    [RelayCommand]
    private void DesmarcarTodasTemporadas()
    {
        // legacy btNinguna_Click
        foreach (var t in Temporadas)
        {
            t.Seleccionada = false;
        }
    }

    // ===== Comandos del modo "Una combinación por jornada" =====

    public bool PuedeJornadaAnterior => Jornada > 0;
    public bool PuedeJornadaSiguiente => Jornada < 43;

    partial void OnJornadaChanged(double value)
    {
        OnPropertyChanged(nameof(PuedeJornadaAnterior));
        OnPropertyChanged(nameof(PuedeJornadaSiguiente));
        JornadaAnteriorCommand.NotifyCanExecuteChanged();
        JornadaSiguienteCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void TemporadaAnterior() => Temporada -= 1;  // legacy btTemporadaAnterior_Click

    [RelayCommand]
    private void TemporadaSiguiente() => Temporada += 1;  // legacy btTemporadaSiguiente_Click

    [RelayCommand(CanExecute = nameof(PuedeJornadaAnterior))]
    private void JornadaAnterior()
    {
        // legacy btJornadaAnterior_Click
        if (Jornada > 0)
        {
            Jornada -= 1;
        }
    }

    [RelayCommand(CanExecute = nameof(PuedeJornadaSiguiente))]
    private void JornadaSiguiente()
    {
        // legacy btJornadaSiguiente_Click
        if (Jornada < 43)
        {
            Jornada += 1;
        }
    }

    [RelayCommand]
    private async Task AnadirCombinacionAsync()
    {
        // Legacy btAdd_Click: OpenFileDialog (*.txt) en "Columnas\", valida con
        // EsFicheroDeColumnas y crea un Free1X2.Utils.Combinacion(TemporadaTexto, jornada con
        // padding, ruta). Después avanza la jornada.
        var file = await ElegirArchivoAsync(".txt");
        if (file == null) return;

        if (!EsFicheroDeColumnas(file.Path))
        {
            AppServices.MostrarError("El fichero seleccionado no es un fichero de columnas válido");
            return;
        }

        string jornadaPad = ((int)Jornada).ToString().PadLeft(2, '0');
        // Modelo de dominio (legacy: ListaCombinaciones.Add(new Combinacion(...))).
        var combi = new Combinacion(TemporadaTexto, jornadaPad, file.Path);
        ListaCombinaciones.Add(combi);
        // Vista (legacy: GridDataBind sobre ListaCombinaciones).
        Combinaciones.Add(new CombinacionFilaViewModel(combi.Temporada, combi.Jornada, combi.Path));

        Jornada += 1;  // legacy: Jornada++; txJornada.Text = Jornada.ToString();
    }

    [RelayCommand]
    private void EliminarCombinacion()
    {
        // Legacy btEliminar_Click: elimina de ListaCombinaciones las filas seleccionadas en el
        // grid. En el XAML solo hay una fila seleccionable enlazada (CombinacionSeleccionada);
        // se elimina esa de ambos modelos (dominio y vista) manteniendo su correspondencia.
        var fila = CombinacionSeleccionada;
        if (fila == null) return;

        int idx = Combinaciones.IndexOf(fila);
        if (idx >= 0)
        {
            Combinaciones.RemoveAt(idx);
            if (idx < ListaCombinaciones.Count)
            {
                ListaCombinaciones.RemoveAt(idx);
            }
        }
        CombinacionSeleccionada = null;
        // TODO: borrado multiselección (legacy recorría dgListaFicheros.IsSelected de N filas) —
        //       requiere enlazar ListView.SelectedItems; ver Free1X2/UI/DialogoAnalisisMultipleDeTramosFrm.cs línea 1044.
    }

    [RelayCommand]
    private async Task GuardarListaAsync()
    {
        // Legacy btGuardar_Click: SaveFileDialog (*.lst) en "Lista\". Vuelca cada Combinacion
        // ("Temporada Jornada Path") con ArchivoColumnasTexto.GuardarCols(...).
        var picker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Lista",
        };
        picker.FileTypeChoices.Add("Lista", new List<string> { ".lst" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            IArchivoColumnas comCols = new ArchivoColumnasTexto(file.Path);
            foreach (Combinacion combi in ListaCombinaciones)
            {
                comCols.GuardarCols(combi.Temporada + " " + combi.Jornada + " " + combi.Path);
            }
            comCols.Cerrar();
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo guardar la lista: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task LeerListaAsync()
    {
        // Legacy btLeer_Click: OpenFileDialog (*.lst) en "Lista\". Limpia ListaCombinaciones y
        // la rellena parseando cada línea (temporada 0..9, jornada 10..12, ruta resto) con
        // ArchivoColumnasTexto.
        var file = await ElegirArchivoAsync(".lst");
        if (file == null) return;

        try
        {
            ListaCombinaciones.Clear();
            Combinaciones.Clear();

            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(file.Path);
            while (comBaseCols.SiguienteColumna())
            {
                string aux = comBaseCols.LeeColumnaSinComas();
                // Mismos índices que el legacy: Substring(0,9) / (10,2) / (13, resto).
                var combi = new Combinacion(
                    aux.Substring(0, 9),
                    aux.Substring(10, 2),
                    aux.Substring(13, aux.Length - 13));
                ListaCombinaciones.Add(combi);
                Combinaciones.Add(new CombinacionFilaViewModel(combi.Temporada, combi.Jornada, combi.Path));
            }
            comBaseCols.Cerrar();
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo leer la lista: " + ex.Message);
        }
    }

    // ===== Acciones del diálogo =====

    [RelayCommand]
    private void Aceptar()
    {
        // Legacy btAceptar_Click: valida según la pestaña activa y, si hay datos, cierra.
        // Es14Triples queda fijado por la pestaña activa (legacy tabControl1_SelectedIndexChanged):
        //   pestaña 0 + rb14Triples -> 0; pestaña 0 + rbFichero -> 1; pestaña 1 -> 2.
        if (PestanaActiva == 0)
        {
            Es14Triples = Usar14Triples ? (byte)0 : (byte)1;
        }
        else
        {
            Es14Triples = 2;
        }

        bool faltanDatos = true;
        if (PestanaActiva == 0)
        {
            // tab14T: hay fichero de valoraciones y al menos una temporada marcada.
            bool algunaMarcada = false;
            foreach (var t in Temporadas)
            {
                if (t.Seleccionada) { algunaMarcada = true; break; }
            }
            if (_rutaValoraciones != "" && algunaMarcada) faltanDatos = false;
        }
        else
        {
            // tabFicheros: hay fichero de valoraciones (la lista puede estar vacía como en el legacy).
            if (_rutaValoraciones != "") faltanDatos = false;
        }

        if (faltanDatos)
        {
            AppServices.MostrarError("No se ha introducido toda la información necesaria");
            return;
        }

        Aceptado = true;
        CierreSolicitado?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Legacy btCancelar_Click: descarta el fichero de valoraciones y cierra sin devolver datos.
        _rutaValoraciones = "";
        FicheroValoraciones = "";
        Aceptado = false;
        CierreSolicitado?.Invoke(this, EventArgs.Empty);
    }

    // ===== Lógica de dominio (réplica de las rutinas del form legacy) =====

    /// <summary>
    /// Carga la lista de temporadas del fichero de valoraciones (legacy CargarListaDeTemporadas):
    /// lee con ArchivoColumnasTexto, exige 44 campos por columna y agrupa por temporada
    /// (campo 0). Devuelve false si el fichero no tiene el formato esperado.
    /// </summary>
    private bool CargarListaDeTemporadas()
    {
        Temporadas.Clear();
        bool res = true;
        if (string.IsNullOrEmpty(_rutaValoraciones)) return res;

        string temporadaAnt = "";
        try
        {
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(_rutaValoraciones);
            while (comBaseCols.SiguienteColumna())
            {
                string[] valorsJornada = comBaseCols.LeeColumnaSinComas().Split((char)9);
                if (valorsJornada.Length != 44)
                {
                    res = false;
                    break;
                }
                if (valorsJornada[0] == temporadaAnt) continue;
                temporadaAnt = valorsJornada[0];
                Temporadas.Add(new TemporadaSeleccionableViewModel(temporadaAnt));
            }
            comBaseCols.Cerrar();
        }
        catch (Exception ex)
        {
            res = false;
            System.Diagnostics.Debug.WriteLine("CargarListaDeTemporadas: " + ex.Message);
        }

        if (res)
        {
            // legacy: txTemporada.Text = temporadaAnt.Substring(0,4); txJornada = 1.
            if (temporadaAnt.Length >= 4 && int.TryParse(temporadaAnt.Substring(0, 4), out int t))
            {
                Temporada = t;
            }
            Jornada = 1;
        }
        else
        {
            _rutaValoraciones = "";
            Temporadas.Clear();
            AppServices.MostrarError("El fichero no es de valoraciones históricas");
        }
        return res;
    }

    /// <summary>
    /// Comprueba que la primera línea del fichero es una columna válida (legacy
    /// EsFicheroDeColumnas): al menos 14 caracteres y los 14 primeros pertenecen a "1xX2".
    /// </summary>
    private static bool EsFicheroDeColumnas(string nombreFichero)
    {
        try
        {
            using var srv = new StreamReader(nombreFichero);
            if (srv.Peek() <= -1) return false;

            string columna = (srv.ReadLine() ?? "").Trim();
            if (columna.Length < 14) return false;

            for (int i = 0; i < 14; i++)
            {
                if ("1xX2".IndexOf(columna[i]) == -1) return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Helper común para los OpenFileDialog (*.txt / *.lst) del form legacy.
    private static async Task<Windows.Storage.StorageFile?> ElegirArchivoAsync(string extension)
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(extension);
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);
        return await picker.PickSingleFileAsync();
    }
}
