using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Escrutinio;
using Free1X2.Utils;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Escrutinios" (legacy: Free1X2.UI.EscrutiniosFrm).
/// Escruta uno o varios ficheros de columnas contra: (1) una columna ganadora
/// introducida a mano, (2) las columnas de un fichero de referencia o (3) la
/// temporada/jornada correspondiente leída del histórico de resultados.
/// Toda la lógica de dominio (Escrutador, ArchivoColumnasTexto, lectura de
/// Jornadas/Resultados.txt, Free1X2WService) queda marcada como TODO.
/// </summary>
public partial class EscrutiniosFrmViewModel : ObservableObject
{
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
    public ObservableCollection<string> TemporadasSeleccionadas { get; } = new();

    // ===== Resultados =====

    // Mensaje de tiempo / estado del escrutinio (legacy: lblTime; "Calculando...", "Final = ...").
    [ObservableProperty]
    private string _estado = string.Empty;

    // Filas de resultados del escrutinio (legacy: dgResultados / resultadosDS "Resultados").
    public ObservableCollection<object> Resultados { get; } = new();

    // Histograma de premios globales: nº de columnas con N aciertos (legacy: premiosGlobales[]).
    public ObservableCollection<PremioHistograma> Histograma { get; } = new();

    // Rutas completas de los ficheros a escrutar (las propiedades *Texto sólo muestran el nombre).
    private readonly List<string> _archivosComb = new();
    private string _archivoReferencia = "";

    // ===== Estados de habilitación de botones =====

    // Habilita Seleccionar/Deseleccionar/Grabar tras un escrutinio (legacy: btnEnableSel/btnDisabSel.Enabled).
    [ObservableProperty]
    private bool _hayResultados;

    // Habilita "Ver premiadas" si hubo escrutinio con la opción activa (legacy: btnVerPremiadas.Enabled).
    [ObservableProperty]
    private bool _puedeVerPremiadas;

    public EscrutiniosFrmViewModel()
    {
        // Rango por defecto con el nº real de partidos (legacy ctor: "10-" + NumeroPartidos).
        RangoAciertos = "10-" + Free1X2.VariablesGlobales.NumeroPartidos;
        // TODO[dominio]: cargar Temporadas leyendo Jornadas/Resultados.txt en orden descendente
        //   (legacy crearDataset(): rellena dsJornadas y lstTemporadas) — requerido para tipo 3.
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
    /// Legacy: btnActualizaCG_Click -> Free1X2WService.ObtenerJornadaActual().
    /// </summary>
    [RelayCommand]
    private void ActualizarColumnaGanadora()
    {
        // TODO[dominio]: llamar al servicio web y volcar P1..P15 en ColumnaGanadora
        //   recortado a VariablesGlobales.NumeroPartidos. (legacy btnActualizaCG_Click)
    }

    /// <summary>
    /// Selecciona la carpeta de ficheros (pestaña 3).
    /// Legacy: btnBuscarCarpeta_Click -> FolderBrowserDialog.
    /// </summary>
    [RelayCommand]
    private void SeleccionarCarpeta()
    {
        // TODO[dominio]: abrir FolderPicker; Carpeta = ruta seleccionada. (legacy lblCarpeta)
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
            // El escrutinio contra jornadas requiere el dataset de Jornadas/Resultados.txt
            // (legacy crearDataset + recorrido temporada/jornada). Pendiente de portar.
            // TODO(escrutinio-jornadas): portar la rama tipoEscrutinio==3 de RealizaEscrutinio.
            Free1X2.Abstractions.UserDialogs.ShowError(
                "El escrutinio contra jornadas aún no está disponible en esta versión.");
            return;
        }

        // RangosHelper.ObtenIntArray: convierte "10-14" en el array de nº de aciertos a contar.
        int[] colAciertos = new RangosHelper().ObtenIntArray(RangoAciertos);
        bool verPremiadas = ActivarVerPremiadas;
        int tipo = TipoEscrutinio;
        string colGan = ColumnaGanadora;
        string archivoRef = _archivoReferencia;
        var archivos = new List<string>(_archivosComb);

        Estado = "Calculando...";
        Resultados.Clear();
        Histograma.Clear();

        var hora0 = DateTime.Now;

        // RealizaEscrutinio() corre el motor por cada fichero y acumula premiosGlobales.
        // Se ejecuta en un hilo de fondo; el DataSet de resultados queda en memoria y el
        // histograma se publica en la UI al terminar.
        int[] premiosGlobales = await Task.Run(() =>
        {
            int[] globales = new int[Free1X2.VariablesGlobales.NumeroPartidos + 1];
            var resultadosDS = new DataSet();

            foreach (string archivo in archivos)
            {
                var escrutador = new Escrutador(colAciertos)
                {
                    ArchivoColumnas = archivo,
                    AñadirAGanadoras = verPremiadas,
                };

                if (tipo == 1)
                    escrutador.EscrutaCombConColumna(colGan, resultadosDS, Path.GetFileName(archivo));
                else // tipo == 2
                    escrutador.EscrutaCombConTemporada(archivoRef, resultadosDS, Path.GetFileName(archivo));

                int[] premios = escrutador.PremiosTotales;
                for (int i = 0; i <= Free1X2.VariablesGlobales.NumeroPartidos; i++)
                    globales[i] += premios[i];
            }
            return globales;
        });

        // Publica el histograma (nº de aciertos -> columnas) sólo para los rangos pedidos.
        Array.Sort(colAciertos);
        for (int k = colAciertos.Length - 1; k >= 0; k--)
        {
            int aciertos = colAciertos[k];
            if (aciertos >= 0 && aciertos < premiosGlobales.Length)
                Histograma.Add(new PremioHistograma(aciertos, premiosGlobales[aciertos]));
        }

        var hora9 = DateTime.Now;
        string tiempo = "Final = " + (hora9 - hora0);
        if (tiempo.Length >= 18) tiempo = tiempo.Substring(0, 18);
        Estado = tiempo;

        HayResultados = true;
        PuedeVerPremiadas = verPremiadas;
    }

    /// <summary>
    /// Detiene un escrutinio en curso. Legacy: BtnComienzoClick (texto "Parar escrutinio!").
    /// </summary>
    [RelayCommand]
    private void Detener()
    {
        // TODO[dominio]: escrutador?.PararEscrutinio(). (legacy)
    }

    /// <summary>
    /// Marca todas las filas de resultados. Legacy: btnEnableSel_Click -> PonerValorMarcadoGlobal(true).
    /// </summary>
    [RelayCommand]
    private void SeleccionarTodas()
    {
        // TODO[dominio]: poner Seleccionado=true en todas las filas del dataset. (legacy)
    }

    /// <summary>
    /// Desmarca todas las filas de resultados. Legacy: btnDisabSel_Click -> PonerValorMarcadoGlobal(false).
    /// </summary>
    [RelayCommand]
    private void DeseleccionarTodas()
    {
        // TODO[dominio]: poner Seleccionado=false en todas las filas del dataset. (legacy)
    }

    /// <summary>
    /// Graba las columnas marcadas a un fichero. Legacy: btnGrabaCols_Click -> SaveFileDialog + ArchivoColumnasTexto.
    /// </summary>
    [RelayCommand]
    private void GrabarColumnas()
    {
        // TODO[dominio]: FileSavePicker (carpeta Columnas, *.txt) y guardar las filas con
        //   Seleccionado=true vía IArchivoColumnas.GuardarCols(...). (legacy btnGrabaCols_Click)
    }

    /// <summary>
    /// Abre la ventana de columnas premiadas. Legacy: btnVerPremiadas_Click -> ColumnasPremiadasFrm.
    /// </summary>
    [RelayCommand]
    private void VerPremiadas()
    {
        // TODO[dominio]: navegar/abrir el equivalente WinUI de ColumnasPremiadasFrm
        //   con la listaPremiadas acumulada en el último escrutinio. (legacy btnVerPremiadas_Click)
    }

    /// <summary>
    /// Abre la ventana de posibles premios. Legacy: btnPosiblesPremios_Click -> PosiblesPremiosFrm.
    /// </summary>
    [RelayCommand]
    private void PosiblesPremios()
    {
        // TODO[dominio]: navegar/abrir el equivalente WinUI de PosiblesPremiosFrm. (legacy btnPosiblesPremios_Click)
    }

    /// <summary>
    /// Cierra/regresa sin escrutar. Legacy: BtnCancelarClick -> PararEscrutinio + Close().
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO[dominio]: detener escrutinio en curso y navegar atrás (Frame.GoBack)
        //   o cerrar el host contenedor. (legacy BtnCancelarClick)
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
