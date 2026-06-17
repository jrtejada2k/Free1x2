using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Escrutinio;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de resultados del escrutinio (legacy: cada DataRow de la tabla "Resultados"
/// que el Escrutador rellena vía PonerPremios). <see cref="Seleccionado"/> es editable
/// (toggle de la rejilla); el resto se expone como string para enlazar a TextBlock.Text.
/// </summary>
public partial class ResultadoEscrutinioItem : ObservableObject
{
    /// <summary>Casilla de selección (legacy: columna "Seleccionado", DataGridBoolColumn).</summary>
    [ObservableProperty]
    private bool _seleccionado;

    /// <summary>Nº de línea/columna (legacy: "LineaID").</summary>
    public string LineaId { get; init; } = string.Empty;

    /// <summary>Fichero de origen (legacy: "Archivo").</summary>
    public string Archivo { get; init; } = string.Empty;

    /// <summary>Texto de la columna (legacy: "Columna").</summary>
    public string Columna { get; init; } = string.Empty;

    /// <summary>Aciertos totales de la fila (legacy: "Ac. Totales").</summary>
    public string AcTotales { get; init; } = string.Empty;

    /// <summary>Premios por categoría de la fila, ya formateados ("13: 0  12: 1 ...").</summary>
    public string Premios { get; init; } = string.Empty;

    /// <summary>Resumen para mostrar en una sola línea de la lista.</summary>
    public string Resumen =>
        $"{(Archivo.Length > 0 ? Archivo + "  " : "")}{(LineaId.Length > 0 ? "#" + LineaId + "  " : "")}{Columna}    {Premios}".Trim();
}

/// <summary>
/// ViewModel de la pantalla "Escrutinios" (legacy: Free1X2.UI.EscrutiniosFrm).
/// Escruta uno o varios ficheros de columnas contra: (1) una columna ganadora
/// introducida a mano, (2) las columnas de un fichero de referencia o (3) la
/// temporada/jornada correspondiente leída del histórico de resultados.
/// El motor (Escrutador), la lectura de Jornadas/Resultados.txt, la grabación de
/// columnas (ArchivoColumnasTexto) y la lista de premiadas están cableados. Quedan
/// como TODO la actualización por servicio web (Free1X2WService) y la navegación a
/// PosiblesPremios/Cancelar (responsabilidad del shell de navegación).
/// </summary>
public partial class EscrutiniosFrmViewModel : ObservableObject
{
    /// <summary>
    /// Navegación a otra página del Frame, inyectada por la Page (mismo patrón que
    /// PosiblesPremiosFrmViewModel.Navegar). El form legacy abría PosiblesPremiosFrm sin estado.
    /// </summary>
    public Action<Type, object?>? Navegar { get; set; }

    /// <summary>
    /// Navegación atrás (Frame.GoBack), inyectada por la Page (legacy BtnCancelarClick -> Close()).
    /// </summary>
    public Action? Volver { get; set; }

    // Escrutador en curso (legacy: campo escrutador, reasignado por cada fichero). Se conserva
    // para que Detener/Cancelar puedan llamar a PararEscrutinio() sobre el que está corriendo.
    private Escrutador? _escrutadorActual;

    // Tipo de escrutinio según la pestaña activa (legacy: tipoEscrutinio; 1=simple, 2=fichero, 3=jornadas).
    // Se deriva de TabSeleccionada para mantener el binding en un solo sitio.
    public int TipoEscrutinio => TabSeleccionada + 1;

    // ===== Cabecera (común a todas las pestañas) =====

    // Rango de aciertos a mostrar como columnas del resultado (legacy: txtNoAciertos, "10-14").
    [ObservableProperty]
    private string _rangoAciertos = "10-14";

    // Activa el registro de columnas premiadas durante el escrutinio (legacy: chkVerPremiadas).
    [ObservableProperty]
    private bool _activarVerPremiadas;

    // Índice de pestaña seleccionada (legacy: tabControl1.SelectedIndex; 0=Simple, 1=Fichero, 2=Jornadas).
    [ObservableProperty]
    private int _tabSeleccionada;

    partial void OnTabSeleccionadaChanged(int value)
    {
        // Legacy seleccionarTab(): en "Jornadas" se oculta el selector de fichero a escrutar.
        OnPropertyChanged(nameof(MostrarFicheroAEscrutar));
        OnPropertyChanged(nameof(TipoEscrutinio));
    }

    // El fichero a escrutar sólo aplica a las pestañas 1 y 2 (legacy: btnFileOrig/listaFicheros.Visible).
    public bool MostrarFicheroAEscrutar => TabSeleccionada != 2;

    // ===== Fichero(s) a escrutar (común a pestañas Simple y Fichero) =====

    // Nombres de los ficheros de columnas seleccionados para escrutar (legacy: listaFicheros + archivosComb).
    public ObservableCollection<string> FicherosAEscrutar { get; } = new();

    // ===== Pestaña 1: Escrutinio simple =====

    // Columna ganadora introducida a mano (legacy: txtColGanadora; caracteres 1/X/2/*/S, mayúsculas).
    [ObservableProperty]
    private string _columnaGanadora = string.Empty;

    // ===== Pestaña 2: Escrutinio contra fichero =====

    // Nombre del fichero de referencia (legacy: lblFileRef; ruta completa guardada en su Tag).
    [ObservableProperty]
    private string _ficheroReferencia = "(selecciona)";

    // ===== Pestaña 3: Escrutinio contra jornadas =====

    // Plantilla del nombre de archivo con marcadores /t (temporada) y /j (jornada) (legacy: txtNombreArchBase).
    [ObservableProperty]
    private string _plantillaNombreArchivo = string.Empty;

    // Carpeta donde están los ficheros de columnas (legacy: lblCarpeta).
    [ObservableProperty]
    private string _carpeta = string.Empty;

    // Dígitos de la temporada en el nombre de archivo: 4 ó 2 (legacy: rt4/rt2).
    public IReadOnlyList<string> OpcionesDigitosTemporada { get; } = new[] { "4 dígitos", "2 dígitos" };

    // Índice seleccionado de dígitos de temporada (0 => 4, 1 => 2; legacy: rt4.Checked por defecto).
    [ObservableProperty]
    private int _digitosTemporada;

    // Dígitos de la jornada en el nombre de archivo: 2 ó 1 (legacy: rj2/rj1).
    public IReadOnlyList<string> OpcionesDigitosJornada { get; } = new[] { "2 dígitos", "1 dígito" };

    // Índice seleccionado de dígitos de jornada (0 => 2, 1 => 1; legacy: rj2.Checked por defecto).
    [ObservableProperty]
    private int _digitosJornada;

    // Temporadas disponibles leídas del histórico (legacy: lstTemporadas, MultiExtended).
    public ObservableCollection<string> Temporadas { get; } = new();

    // Temporadas marcadas por el usuario (legacy: lstTemporadas.SelectedIndices).
    public ObservableCollection<int> TemporadasSeleccionadas { get; } = new();

    // ===== Resultados =====

    // Mensaje de tiempo / estado del escrutinio (legacy: lblTime; "Calculando...", "Final = ...").
    [ObservableProperty]
    private string _estado = string.Empty;

    // Filas de resultados del escrutinio (legacy: dgResultados / resultadosDS "Resultados").
    public ObservableCollection<ResultadoEscrutinioItem> Resultados { get; } = new();

    // Histograma de premios globales: nº de columnas con N aciertos (legacy: premiosGlobales[]).
    public ObservableCollection<PremioHistograma> Histograma { get; } = new();

    // Columnas premiadas del último escrutinio (legacy: listaPremiadas -> ColumnasPremiadasFrm).
    public ObservableCollection<ColumnaPremiadaItem> Premiadas { get; } = new();

    // Visibilidad de la tarjeta de premiadas (sólo tras VerPremiadas con datos).
    [ObservableProperty]
    private bool _mostrarPremiadas;

    // Rutas completas de los ficheros a escrutar (las propiedades *Texto sólo muestran el nombre).
    private readonly List<string> _archivosComb = new();
    private string _archivoReferencia = "";

    // DataSet de resultados del último escrutinio (legacy: resultadosDS). Se conserva para
    // que Seleccionar/Deseleccionar/Grabar operen sobre las mismas filas (sincronizado con
    // la colección observable Resultados a través de la propiedad Seleccionado).
    private DataSet? _resultadosDS;

    // Lista de columnas premiadas acumulada en el último escrutinio (legacy: listaPremiadas).
    private readonly List<ColumnasPremiadas> _listaPremiadas = new();

    // DataSet con las jornadas del histórico (legacy: dsJornadas, leído de Resultados.txt).
    private DataSet? _dsJornadas;

    // ===== Estados de habilitación de botones =====

    // Habilita Seleccionar/Deseleccionar/Grabar tras un escrutinio (legacy: btnEnableSel/btnDisabSel.Enabled).
    [ObservableProperty]
    private bool _hayResultados;

    // Habilita "Ver premiadas" si hubo escrutinio con la opción activa (legacy: btnVerPremiadas.Enabled).
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(VerPremiadasEnVentanaCommand))]
    private bool _puedeVerPremiadas;

    public EscrutiniosFrmViewModel()
    {
        // Rango por defecto con el nº real de partidos (legacy ctor: "10-" + NumeroPartidos).
        RangoAciertos = "10-" + Free1X2.VariablesGlobales.NumeroPartidos;
        // Carga las temporadas del histórico (legacy ctor: crearDataset()). Requerido para tipo 3.
        CrearDataSetJornadas();
    }

    /// <summary>
    /// Crea el DataSet de jornadas leyendo Jornadas/Resultados.txt y rellena Temporadas
    /// en orden descendente. Réplica exacta de EscrutiniosFrm.crearDataset (esquema Temp/Jorn/
    /// Quiniela, separador TAB). Si el fichero no existe, deja la lista vacía (modo 3 avisará).
    /// </summary>
    private void CrearDataSetJornadas()
    {
        // Legacy: Application.StartupPath + "/Jornadas/Resultados.txt".
        string baseDir = AppContext.BaseDirectory.TrimEnd(
            Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string ruta = Path.Combine(baseDir, "Jornadas", "Resultados.txt");

        var ds = new DataSet("Resultados");
        var t = new DataTable("Resultados");
        t.Columns.Add("Temp", typeof(short));
        t.Columns.Add("Jorn", typeof(short));
        var colQ = new DataColumn("Quiniela", typeof(string)) { MaxLength = 15 };
        t.Columns.Add(colQ);
        ds.Tables.Add(t);
        _dsJornadas = ds;

        Temporadas.Clear();
        if (!File.Exists(ruta)) return;

        try
        {
            foreach (string linea in File.ReadLines(ruta))
            {
                // Legacy: linea.Split((char)9) -> [Temp, Jorn, Quiniela].
                string[] r = linea.Split('\t');
                if (r.Length < 3) continue;
                DataRow row = t.NewRow();
                row["Temp"] = Convert.ToInt16(r[0]);
                row["Jorn"] = Convert.ToInt16(r[1]);
                row["Quiniela"] = r[2];
                t.Rows.Add(row);
            }

            int nr = t.Rows.Count;
            if (nr == 0) return;
            // Legacy: recorre de la última temporada a la primera (descendente).
            for (int i = Convert.ToInt16(t.Rows[nr - 1]["Temp"]);
                 i >= Convert.ToInt16(t.Rows[0]["Temp"]); i--)
            {
                Temporadas.Add(i.ToString());
            }
        }
        catch (Exception ex)
        {
            // El histórico es opcional para los modos 1 y 2; sólo el modo 3 lo necesita.
            System.Diagnostics.Debug.WriteLine($"crearDataset (Jornadas) falló: {ex.Message}");
        }
    }

    /// <summary>
    /// Selecciona uno o varios ficheros de columnas a escrutar.
    /// Legacy: BtnFileOrigClick -> OpenFileDialog (carpeta Columnas, *.txt, Multiselect).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarFicherosAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var files = await picker.PickMultipleFilesAsync();
        if (files == null || files.Count == 0) return;

        _archivosComb.Clear();
        FicherosAEscrutar.Clear();
        foreach (var f in files)
        {
            _archivosComb.Add(f.Path);
            FicherosAEscrutar.Add(f.Name);
        }
    }

    /// <summary>
    /// Selecciona el fichero de referencia (pestaña 2).
    /// Legacy: BtnFileRefClick -> OpenFileDialog (carpeta Columnas, *.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarFicheroReferenciaAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        _archivoReferencia = file.Path;
        FicheroReferencia = file.Name;
    }

    /// <summary>
    /// Actualiza la columna ganadora desde Free1X2.com (pestaña 1).
    /// Legacy: btnActualizaCG_Click -> Free1X2WService.ObtenerJornadaActual() -> P1..P15.
    /// </summary>
    [RelayCommand]
    private void ActualizarColumnaGanadora()
    {
        // El servicio web Free1X2WService (SOAP) NO está disponible offline: en el WinForms original
        // ObtenerJornadaActual() es un stub que devuelve una JornadaActual con P1..P15 nulos
        // (Free1X2/Utils/ControlCompatibility.cs línea 32). El legacy concatena esos 15 signos en un
        // StringBuilder vacío y, al hacer Substring(0, NumeroPartidos), lanza excepción cuyo catch
        // rellena la columna con asteriscos: txtColGanadora.Text = "****...".Substring(0, NumeroPartidos).
        // Reproducimos ese mismo estado de runtime (sin datos reales del servicio): asteriscos.
        // (No se referencia la clase JornadaActual del proyecto WinForms: no forma parte de Free1X2.Domain.)
        int n = Free1X2.VariablesGlobales.NumeroPartidos;
        ColumnaGanadora = new string('*', n);

        Free1X2.Abstractions.UserDialogs.ShowInfo(
            "El servicio online de Free1X2.com no está disponible sin conexión: " +
            "no se ha podido obtener la columna ganadora de la jornada actual.");
    }

    /// <summary>
    /// Selecciona la carpeta de ficheros (pestaña 3).
    /// Legacy: btnBuscarCarpeta_Click -> FolderBrowserDialog.
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarCarpetaAsync()
    {
        // Legacy btnBuscarCarpeta_Click: FolderBrowserDialog -> lblCarpeta.Text.
        var picker = new FolderPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFolder? folder = await picker.PickSingleFolderAsync();
        if (folder is null) return;

        Carpeta = folder.Path;
    }

    /// <summary>
    /// Inserta el marcador de temporada /t en la plantilla (pestaña 3).
    /// Legacy: incluirPrefijo_click con btnIntrTemp ("/t").
    /// </summary>
    [RelayCommand]
    private void InsertarTemporada()
    {
        // TODO[dominio]: insertar "/t" en la posición del cursor de la plantilla. (legacy incluirPrefijo_click)
        PlantillaNombreArchivo += "/t";
    }

    /// <summary>
    /// Inserta el marcador de jornada /j en la plantilla (pestaña 3).
    /// Legacy: incluirPrefijo_click con btnIntrJorn ("/j").
    /// </summary>
    [RelayCommand]
    private void InsertarJornada()
    {
        // TODO[dominio]: insertar "/j" en la posición del cursor de la plantilla. (legacy incluirPrefijo_click)
        PlantillaNombreArchivo += "/j";
    }

    /// <summary>
    /// Ejecuta el escrutinio según la pestaña activa.
    /// Legacy: BtnComienzoClick -> SonDatosValidos() + RealizaEscrutinio().
    /// </summary>
    [RelayCommand]
    private async Task EscrutarAsync()
    {
        // Validación equivalente a SonDatosValidos() (subconjunto: tipos 1 y 2).
        if (_archivosComb.Count == 0 && TipoEscrutinio != 3)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("Falta fichero a escrutar.");
            return;
        }
        if (TipoEscrutinio == 2 && _archivoReferencia.Length == 0)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("Falta fichero de referencia.");
            return;
        }
        if (TipoEscrutinio == 3)
        {
            // Validación del modo jornadas (legacy SonDatosValidos, rama tipoEscrutinio==3).
            if (PlantillaNombreArchivo.Length == 0)
            {
                Free1X2.Abstractions.UserDialogs.ShowError("Falta plantilla de nombre de fichero.");
                return;
            }
            if (Carpeta.Length == 0)
            {
                Free1X2.Abstractions.UserDialogs.ShowError("Falta la carpeta de los ficheros.");
                return;
            }
            if (PlantillaNombreArchivo.IndexOf("/t", StringComparison.Ordinal) < 0)
            {
                Free1X2.Abstractions.UserDialogs.ShowError("No se ha puesto el indicador de temporada (/t).");
                return;
            }
            if (PlantillaNombreArchivo.IndexOf("/j", StringComparison.Ordinal) < 0)
            {
                Free1X2.Abstractions.UserDialogs.ShowError("No se ha puesto el indicador de jornada (/j).");
                return;
            }
        }

        // RangosHelper.ObtenIntArray: convierte "10-14" en el array de nº de aciertos a contar.
        int[] colAciertos = new RangosHelper().ObtenIntArray(RangoAciertos);
        bool verPremiadas = ActivarVerPremiadas;
        int tipo = TipoEscrutinio;
        string colGan = ColumnaGanadora;
        string archivoRef = _archivoReferencia;
        var archivos = new List<string>(_archivosComb);

        // Parámetros del modo 3 (capturados para el hilo de fondo).
        string plantilla = PlantillaNombreArchivo;
        string carpeta = Carpeta;
        int dt = DigitosTemporada == 0 ? 4 : 2;   // legacy rt4/rt2.
        int dj = DigitosJornada == 0 ? 2 : 1;     // legacy rj2/rj1.
        var temporadasSel = new List<int>(TemporadasSeleccionadas);

        Estado = "Calculando...";
        Resultados.Clear();
        Histograma.Clear();
        Premiadas.Clear();
        MostrarPremiadas = false;
        PuedeVerPremiadas = false;
        HayResultados = false;
        _listaPremiadas.Clear();

        var hora0 = DateTime.Now;

        // RealizaEscrutinio() corre el motor por cada fichero y acumula premiosGlobales.
        // Se ejecuta en un hilo de fondo; el DataSet de resultados queda en memoria y el
        // histograma se publica en la UI al terminar.
        var salida = await Task.Run(() =>
        {
            int[] globales = new int[Free1X2.VariablesGlobales.NumeroPartidos + 1];
            var premiadas = new List<ColumnasPremiadas>();
            // El Escrutador escribe filas en la tabla "Resultados" vía PonerPremios; hay que
            // crear su esquema ANTES de escrutar (réplica de EscrutiniosFrm.InicializaResultadosDataSet).
            // Sin esto, Tables["Resultados"] es null y PonerPremios lanza NullReferenceException
            // (y PremiosTotales nunca se acumula). Bug detectado en validación de paridad.
            var resultadosDS = CrearDataSetResultados();
            Escrutador? ultimo = null;

            if (tipo == 3)
            {
                // ===== Rama tipoEscrutinio==3 de RealizaEscrutinio (escrutinio contra jornadas) =====
                EscrutarContraJornadas(resultadosDS, colAciertos, verPremiadas, plantilla,
                    carpeta, dt, dj, temporadasSel, globales, premiadas, ref ultimo);
            }
            else
            {
                foreach (string archivo in archivos)
                {
                    var escrutador = new Escrutador(colAciertos)
                    {
                        ArchivoColumnas = archivo,
                        AñadirAGanadoras = verPremiadas,
                    };
                    // Publica el escrutador en curso para que Detener/Cancelar puedan pararlo
                    // (legacy: campo escrutador reasignado por cada fichero).
                    _escrutadorActual = escrutador;

                    if (tipo == 1)
                        escrutador.EscrutaCombConColumna(colGan, resultadosDS, Path.GetFileName(archivo));
                    else // tipo == 2
                        escrutador.EscrutaCombConTemporada(archivoRef, resultadosDS, Path.GetFileName(archivo));

                    if (verPremiadas)
                    {
                        foreach (var p in escrutador.ListaPremiadas)
                            premiadas.Add((ColumnasPremiadas)p);
                    }

                    int[] premios = escrutador.PremiosTotales;
                    for (int i = 0; i <= Free1X2.VariablesGlobales.NumeroPartidos; i++)
                        globales[i] += premios[i];
                    ultimo = escrutador;
                }
            }

            // Legacy: escrutador.AñadirPremiosGlobales(premiosGlobales) — fila resumen "TOTALES".
            ultimo?.AñadirPremiosGlobales(globales);

            return (globales, resultadosDS, premiadas);
        });

        int[] premiosGlobales = salida.globales;
        _resultadosDS = salida.resultadosDS;
        _listaPremiadas.AddRange(salida.premiadas);

        // Proyecta las filas del DataSet a la colección observable (legacy: dgResultados).
        ProyectarResultados(colAciertos);

        // Publica el histograma (nº de aciertos -> columnas) sólo para los rangos pedidos.
        var rangoOrden = (int[])colAciertos.Clone();
        Array.Sort(rangoOrden);
        for (int k = rangoOrden.Length - 1; k >= 0; k--)
        {
            int aciertos = rangoOrden[k];
            if (aciertos >= 0 && aciertos < premiosGlobales.Length)
                Histograma.Add(new PremioHistograma(aciertos, premiosGlobales[aciertos]));
        }

        var hora9 = DateTime.Now;
        string tiempo = "Final = " + (hora9 - hora0);
        if (tiempo.Length >= 18) tiempo = tiempo.Substring(0, 18);
        Estado = tiempo;

        HayResultados = Resultados.Count > 0;
        PuedeVerPremiadas = verPremiadas && _listaPremiadas.Count > 0;
    }

    /// <summary>
    /// Réplica de la rama tipoEscrutinio==3 de EscrutiniosFrm.RealizaEscrutinio: por cada
    /// temporada/jornada seleccionada localiza el fichero con la plantilla (/t, /j), lee la
    /// columna ganadora del histórico y escruta con un <see cref="Escrutador"/>.
    /// </summary>
    private void EscrutarContraJornadas(
        DataSet resultadosDS, int[] colAciertos, bool verPremiadas, string plantilla,
        string carpeta, int dt, int dj, List<int> temporadasSel,
        int[] premiosGlobales, List<ColumnasPremiadas> premiadas, ref Escrutador? ultimo)
    {
        if (_dsJornadas is null || _dsJornadas.Tables.Count == 0) return;

        const string numeros = "0123456789";
        var listaInicialArchivos = new List<string>();

        // Construye el filtro de temporadas seleccionadas (legacy: " or Temp=...").
        string consulta = "";
        foreach (int temp in temporadasSel)
            consulta += " or Temp=" + temp;
        if (consulta.Length > 0) consulta = consulta.Substring(4);

        var dv = new DataView(_dsJornadas.Tables[0]) { RowFilter = consulta };
        if (dv.Count == 0) return;

        // Busca todos los ficheros que casan con cada temporada/jornada.
        var di = new DirectoryInfo(carpeta + "/");
        for (int i = 0; i < dv.Count; i++)
        {
            int temporada = Convert.ToInt16(dv[i]["Temp"]);
            int jornada = Convert.ToInt16(dv[i]["Jorn"]);
            string temp = temporada.ToString("0000");
            if (dt == 2) temp = temp.Substring(2);
            string jorn = dj == 2 ? jornada.ToString("00") : jornada.ToString();
            string cons = plantilla.Replace("/t", temp).Replace("/j", jorn);
            if (!di.Exists) continue;
            FileInfo[] fi = di.GetFiles(cons);
            for (int j = 0; j < fi.Length; j++)
                listaInicialArchivos.Add(cons);
        }

        int posTemp = plantilla.IndexOf("/t", StringComparison.Ordinal);
        int posJorn = plantilla.IndexOf("/j", StringComparison.Ordinal);

        for (int j = 0; j < listaInicialArchivos.Count; j++)
        {
            string archivo = listaInicialArchivos[j];
            int temporada, jornada;
            string jorn;
            int djLocal = dj;
            if (posTemp > posJorn)
            {
                jorn = archivo.Substring(posJorn, 2);
                if (numeros.IndexOf(jorn.Substring(1), StringComparison.Ordinal) >= 0)
                    djLocal = 2;
                else
                {
                    djLocal = 1;
                    jorn = jorn.Substring(0, 1);
                }
                jornada = Convert.ToInt16(jorn);
                temporada = Convert.ToInt16(archivo.Substring(posTemp + djLocal - 2, dt));
            }
            else
            {
                temporada = Convert.ToInt16(archivo.Substring(posTemp, dt));
                jorn = archivo.Substring(posJorn + dt - 2, 2);
                if (numeros.IndexOf(jorn.Substring(1), StringComparison.Ordinal) < 0)
                    jorn = jorn.Substring(0, 1);
                jornada = Convert.ToInt16(jorn);
            }

            dv.RowFilter = "Temp=" + temporada + " and Jorn=" + jornada;
            if (dv.Count == 0) continue;
            string colGanJornada = dv[0]["Quiniela"].ToString() ?? "";

            // El fichero a abrir está dentro de la carpeta; el Escrutador usa la ruta tal cual
            // (igual que el legacy, que pasaba el nombre relativo a la carpeta de trabajo).
            string rutaArchivo = Path.Combine(carpeta, archivo);
            var escrutador = new Escrutador(colAciertos)
            {
                ArchivoColumnas = rutaArchivo,
                AñadirAGanadoras = verPremiadas,
            };
            // Publica el escrutador en curso para Detener/Cancelar (legacy: campo escrutador).
            _escrutadorActual = escrutador;
            escrutador.EscrutaCombConColumna(colGanJornada, resultadosDS, Path.GetFileName(archivo));

            if (verPremiadas)
            {
                foreach (var p in escrutador.ListaPremiadas)
                    premiadas.Add((ColumnasPremiadas)p);
            }

            int[] premios = escrutador.PremiosTotales;
            for (int i = 0; i <= Free1X2.VariablesGlobales.NumeroPartidos; i++)
                premiosGlobales[i] += premios[i];
            ultimo = escrutador;
        }
    }

    /// <summary>
    /// Proyecta las filas de la tabla "Resultados" del DataSet a la colección observable,
    /// formateando los premios sólo para el rango de aciertos pedido (legacy: dgResultados).
    /// </summary>
    private void ProyectarResultados(int[] colAciertos)
    {
        Resultados.Clear();
        if (_resultadosDS is null) return;
        var tabla = _resultadosDS.Tables["Resultados"];
        if (tabla is null) return;

        var orden = (int[])colAciertos.Clone();
        Array.Sort(orden);
        Array.Reverse(orden); // legacy mostraba las columnas de mayor a menor acierto.

        foreach (DataRow row in tabla.Rows)
        {
            var sb = new System.Text.StringBuilder();
            foreach (int a in orden)
            {
                string col = "P" + a;
                if (tabla.Columns.Contains(col) && row[col] != DBNull.Value)
                    sb.Append(a).Append(": ").Append(row[col]).Append("  ");
            }

            Resultados.Add(new ResultadoEscrutinioItem
            {
                Seleccionado = row["Seleccionado"] != DBNull.Value && (bool)row["Seleccionado"],
                LineaId = row["LineaID"] == DBNull.Value ? "" : row["LineaID"].ToString() ?? "",
                Archivo = row["Archivo"] == DBNull.Value ? "" : row["Archivo"].ToString() ?? "",
                Columna = row["Columna"] == DBNull.Value ? "" : row["Columna"].ToString() ?? "",
                AcTotales = tabla.Columns.Contains("Ac. Totales") && row["Ac. Totales"] != DBNull.Value
                    ? row["Ac. Totales"].ToString() ?? "" : "",
                Premios = sb.ToString().Trim(),
            });
        }
    }

    /// <summary>
    /// Crea el DataSet con la tabla "Resultados" que el Escrutador rellena (PonerPremios).
    /// Réplica exacta de EscrutiniosFrm.InicializaResultadosDataSet del WinForms original:
    /// columnas Seleccionado/LineaID/Columna/Archivo, P0..P(NumeroPartidos) y "Ac. Totales".
    /// </summary>
    private static DataSet CrearDataSetResultados()
    {
        var ds = new DataSet();
        var t = new DataTable("Resultados");
        t.Columns.Add("Seleccionado", typeof(bool));
        t.Columns.Add("LineaID", typeof(int));
        t.Columns.Add("Columna", typeof(string));
        t.Columns.Add("Archivo", typeof(string));
        for (int i = 0; i <= Free1X2.VariablesGlobales.NumeroPartidos; i++)
            t.Columns.Add("P" + i, typeof(int));
        t.Columns.Add("Ac. Totales", typeof(string));
        ds.Tables.Add(t);
        return ds;
    }

    /// <summary>
    /// Detiene un escrutinio en curso. Legacy: BtnComienzoClick (texto "Parar escrutinio!").
    /// </summary>
    [RelayCommand]
    private void Detener()
    {
        // Legacy BtnComienzoClick (rama "Parar escrutinio!", EscrutiniosFrm.cs:1187):
        //   escrutador.PararEscrutinio(). El bucle de EscrutaComb... comprueba la bandera
        //   pararEscrutinio entre columnas y corta. _escrutadorActual apunta al que corre ahora.
        _escrutadorActual?.PararEscrutinio();
    }

    /// <summary>
    /// Marca todas las filas de resultados. Legacy: btnEnableSel_Click -> PonerValorMarcadoGlobal(true).
    /// </summary>
    [RelayCommand]
    private void SeleccionarTodas() => PonerValorMarcadoGlobal(true);

    /// <summary>
    /// Desmarca todas las filas de resultados. Legacy: btnDisabSel_Click -> PonerValorMarcadoGlobal(false).
    /// </summary>
    [RelayCommand]
    private void DeseleccionarTodas() => PonerValorMarcadoGlobal(false);

    // Legacy PonerValorMarcadoGlobal: pone Seleccionado en todas las filas. Mantiene
    // sincronizados el DataSet (origen para Grabar) y la colección observable (la UI).
    private void PonerValorMarcadoGlobal(bool seleccionado)
    {
        foreach (var item in Resultados)
            item.Seleccionado = seleccionado;

        if (_resultadosDS?.Tables["Resultados"] is { } tabla)
        {
            foreach (DataRow row in tabla.Rows)
                row["Seleccionado"] = seleccionado;
        }
    }

    /// <summary>
    /// Graba las columnas marcadas a un fichero. Legacy: btnGrabaCols_Click -> SaveFileDialog + ArchivoColumnasTexto.
    /// </summary>
    [RelayCommand]
    private async Task GrabarColumnasAsync()
    {
        // La selección la mantiene la colección observable (la rejilla edita Seleccionado).
        var seleccionadas = new List<string>();
        foreach (var item in Resultados)
            if (item.Seleccionado && item.Columna.Length > 0)
                seleccionadas.Add(item.Columna);

        if (seleccionadas.Count == 0)
        {
            Free1X2.Abstractions.UserDialogs.ShowInfo("No hay columnas seleccionadas que grabar.");
            return;
        }

        // Legacy: SaveFileDialog filtro "Columnas(*.txt)|*.txt|...".
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "columnas",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSaveFileAsync();
        if (file is null) return;

        await Task.Run(() =>
        {
            // Legacy: IArchivoColumnas archivo = new ArchivoColumnasTexto(nombre);
            //   por cada fila seleccionada -> archivo.GuardarCols(columna); archivo.Cerrar();
            IArchivoColumnas archivo = new ArchivoColumnasTexto(file.Path);
            foreach (string columna in seleccionadas)
                archivo.GuardarCols(columna);
            archivo.Cerrar();
        });

        Free1X2.Abstractions.UserDialogs.ShowInfo($"Guardadas {seleccionadas.Count} columna(s) en {file.Name}.");
    }

    /// <summary>
    /// Muestra las columnas premiadas del último escrutinio. Legacy: btnVerPremiadas_Click ->
    /// ColumnasPremiadasFrm (rellenaba listaResumen con Fichero/Jornada/Columna/Premio/NoCol/NoBoleto).
    /// Aquí se proyectan a la tarjeta de premiadas de la propia página (sin navegación cruzada).
    /// </summary>
    [RelayCommand]
    private void VerPremiadas()
    {
        Premiadas.Clear();
        foreach (var p in _listaPremiadas)
        {
            // Legacy: NoBoleto + " (" + orden + ")", con orden = NoColumna % 8 (8 si 0).
            int orden = p.NoColumna % 8;
            if (orden == 0) orden = 8;
            Premiadas.Add(new ColumnaPremiadaItem
            {
                ArchivoColumnas = Path.GetFileName(p.Fichero),
                Jornada = p.Jornada.ToString(),
                Columna = p.Columna,
                Premio = p.Premio.ToString(),
                NumeroColumna = p.NoColumna.ToString(),
                NumeroBoleto = p.NoBoleto + " (" + orden + ")",
            });
        }
        MostrarPremiadas = true;
    }

    /// <summary>
    /// Abre la VENTANA de columnas premiadas (legacy fiel: btnVerPremiadas_Click ->
    /// new ColumnasPremiadasFrm(); rellenar listaResumen; form.ShowDialog()). Proyecta
    /// _listaPremiadas a filas ColumnaPremiadaItem, las deja en el handoff estático
    /// ColumnasPremiadasFrmPage.Entrada y navega a esa página (que ofrece además exportar
    /// todas / seleccionadas). Coexiste con VerPremiadas (tarjeta inline) como en la app.
    /// </summary>
    [RelayCommand(CanExecute = nameof(PuedeVerPremiadas))]
    private void VerPremiadasEnVentana()
    {
        var filas = new List<ColumnaPremiadaItem>();
        foreach (var p in _listaPremiadas)
        {
            // Legacy: NoBoleto + " (" + orden + ")", con orden = NoColumna % 8 (8 si 0).
            int orden = p.NoColumna % 8;
            if (orden == 0) orden = 8;
            filas.Add(new ColumnaPremiadaItem
            {
                ArchivoColumnas = Path.GetFileName(p.Fichero),
                Jornada = p.Jornada.ToString(),
                Columna = p.Columna,
                Premio = p.Premio.ToString(),
                NumeroColumna = p.NoColumna.ToString(),
                NumeroBoleto = p.NoBoleto + " (" + orden + ")",
            });
        }

        ColumnasPremiadasFrmPage.Entrada = filas;
        Navegar?.Invoke(typeof(ColumnasPremiadasFrmPage), null);
    }

    /// <summary>
    /// Abre la ventana de posibles premios. Legacy: btnPosiblesPremios_Click -> PosiblesPremiosFrm.
    /// </summary>
    [RelayCommand]
    private void PosiblesPremios()
    {
        // Legacy btnPosiblesPremios_Click -> new PosiblesPremiosFrm().Show(). En WinUI navega a
        // PosiblesPremiosFrmPage (su ViewModel ya está cableado), sin parámetro (se abría sin estado).
        Navegar?.Invoke(typeof(PosiblesPremiosFrmPage), null);
    }

    /// <summary>
    /// Cierra/regresa sin escrutar. Legacy: BtnCancelarClick -> PararEscrutinio + Close().
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // Legacy BtnCancelarClick (EscrutiniosFrm.cs:1156): si hay escrutador, PararEscrutinio();
        //   luego Close(). En WinUI se detiene el escrutinio en curso y se navega atrás (Frame.GoBack).
        _escrutadorActual?.PararEscrutinio();
        Volver?.Invoke();
    }
}

/// <summary>Una fila del histograma de premios: nº de aciertos y nº de columnas con ese acierto.</summary>
public sealed class PremioHistograma
{
    public PremioHistograma(int aciertos, int columnas)
    {
        Aciertos = aciertos;
        Columnas = columnas;
    }

    public int Aciertos { get; }
    public int Columnas { get; }

    // Texto para bindear directamente a TextBlock.Text (regla anti-crash: no bindear int crudo).
    public string AciertosTexto => Aciertos.ToString();
    public string ColumnasTexto => Columnas.ToString();
}
