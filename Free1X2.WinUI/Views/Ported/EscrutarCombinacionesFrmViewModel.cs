using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Escrutinio;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una columna premiada del escrutinio de combinaciones (legacy:
/// EscrutadorComb.ListaEscrutadasConPremio -> ColumnasPremiadasComb). Es la fila
/// seleccionable/grababable de la rejilla de resultados (el grid del WinForms original
/// nunca llegaba a poblarse porque EscrutadorComb.escrutinioDS era null).
/// <see cref="Seleccionado"/> es editable; el resto se expone como string.
/// </summary>
public partial class CombinacionPremiadaItem : ObservableObject
{
    [ObservableProperty]
    private bool _seleccionado;

    /// <summary>Texto de la columna (1/X/2) (legacy: ColumnasPremiadasComb.ColumnaTexto).</summary>
    public string Columna { get; init; } = string.Empty;

    /// <summary>Fichero de origen (legacy: ColumnasPremiadasComb.Fichero).</summary>
    public string Archivo { get; init; } = string.Empty;

    /// <summary>Jornada (legacy: ColumnasPremiadasComb.Jornada).</summary>
    public string Jornada { get; init; } = string.Empty;

    /// <summary>Categoría de premio obtenida (legacy: ColumnasPremiadasComb.Premio).</summary>
    public string Premio { get; init; } = string.Empty;

    /// <summary>Resumen para una sola línea de la lista.</summary>
    public string Resumen =>
        $"{Columna}   {Premio} ac.   {(Archivo.Length > 0 ? Archivo : "")}{(Jornada.Length > 0 ? "  j" + Jornada : "")}".Trim();
}

/// <summary>
/// Una casilla de un signo (1/X/2) de la columna ganadora introducida manualmente.
/// Réplica de cada TextBox txtCG1..txtCG14 del WinForms "EscrutarCombinacionesFrm"
/// (MaxLength=1, mayúsculas, vacío = comodín '*').
/// </summary>
public partial class SignoGanadorViewModel : ObservableObject
{
    /// <summary>Número de partido (1..14), mostrado como etiqueta del toggle.</summary>
    public int Partido { get; init; }

    /// <summary>Etiqueta visible (número de partido).</summary>
    public string Etiqueta => Partido.ToString();

    // Estado de cada signo. Sólo uno (o ninguno = comodín) puede estar activo.
    // En el WinForms el valor se tecleaba directo en el TextBox; aquí se usan toggles.

    [ObservableProperty]
    private bool _es1;

    [ObservableProperty]
    private bool _esX;

    [ObservableProperty]
    private bool _es2;
}

/// <summary>
/// ViewModel de la página portada del WinForms "EscrutarCombinacionesFrm".
/// Escruta (puntúa) uno o varios ficheros de combinaciones contra columna/s ganadora/s,
/// en tres modos: escrutinio simple (columna manual), contra fichero de referencia y
/// contra jornadas de una/varias temporadas.
///
/// La lógica de dominio (EscrutadorComb: ObtenerPosiblesPremios + EscrutarCombinacion,
/// lectura de ficheros, cálculo de premios, grabación y lista de premiadas) está cableada.
/// Sólo queda como TODO la navegación a PosiblesPremiosFrmPage (responsabilidad del shell).
/// </summary>
public partial class EscrutarCombinacionesFrmViewModel : ObservableObject
{
    public EscrutarCombinacionesFrmViewModel()
    {
        for (int i = 1; i <= 14; i++)
        {
            SignosGanadores.Add(new SignoGanadorViewModel { Partido = i });
        }
        // Legacy ctor: crearDataset() — carga el histórico de jornadas (modo 3).
        CrearDataSetJornadas();
    }

    // DataSet con las jornadas del histórico (legacy: dsJornadas).
    private System.Data.DataSet? _dsJornadas;

    /// <summary>
    /// Carga Jornadas/Resultados.txt y rellena <see cref="Temporadas"/> en orden descendente.
    /// Réplica de EscrutarCombinacionesFrm.crearDataset (esquema Temp/Jorn/Quiniela, TAB).
    /// </summary>
    private void CrearDataSetJornadas()
    {
        string baseDir = AppContext.BaseDirectory.TrimEnd(
            Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string ruta = Path.Combine(baseDir, "Jornadas", "Resultados.txt");

        var ds = new System.Data.DataSet("Resultados");
        var t = new System.Data.DataTable("Resultados");
        t.Columns.Add("Temp", typeof(short));
        t.Columns.Add("Jorn", typeof(short));
        var colQ = new System.Data.DataColumn("Quiniela", typeof(string)) { MaxLength = 15 };
        t.Columns.Add(colQ);
        ds.Tables.Add(t);
        _dsJornadas = ds;

        Temporadas.Clear();
        if (!File.Exists(ruta)) return;

        try
        {
            foreach (string linea in File.ReadLines(ruta))
            {
                string[] r = linea.Split('\t');
                if (r.Length < 3) continue;
                var row = t.NewRow();
                row["Temp"] = Convert.ToInt16(r[0]);
                row["Jorn"] = Convert.ToInt16(r[1]);
                row["Quiniela"] = r[2];
                t.Rows.Add(row);
            }

            int nr = t.Rows.Count;
            if (nr == 0) return;
            for (int i = Convert.ToInt16(t.Rows[nr - 1]["Temp"]);
                 i >= Convert.ToInt16(t.Rows[0]["Temp"]); i--)
            {
                Temporadas.Add(i.ToString());
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"crearDataset (Jornadas) falló: {ex.Message}");
        }
    }

    // Temporadas marcadas por el usuario (legacy: lstTemporadas.SelectedIndices, MultiExtended).
    public ObservableCollection<int> TemporadasSeleccionadas { get; } = new();

    // ===== Modo de escrutinio (TabControl tpSimple / tbFichero / tbTemporada) =====
    // 0 = Simple (Tag "1"), 1 = Contra fichero (Tag "2"), 2 = Contra jornadas (Tag "3").
    /// <summary>Opciones de modo equivalentes a las 3 pestañas del TabControl legacy.</summary>
    public IReadOnlyList<string> ModosEscrutinio { get; } = new[]
    {
        "Escrutinio simple",
        "Escrutinio contra fichero",
        "Escrutinio contra jornadas"
    };

    [ObservableProperty]
    private int _modoSeleccionadoIndex;

    // Visibilidad por modo. Se bindea en CADA control hijo (no en el panel) para
    // respetar la regla anti-crash del XamlCompiler.
    /// <summary>Visible cuando el modo activo es "Escrutinio simple".</summary>
    public Visibility ModoSimpleVisibility =>
        ModoSeleccionadoIndex == 0 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Visible cuando el modo activo es "Escrutinio contra fichero".</summary>
    public Visibility ModoFicheroVisibility =>
        ModoSeleccionadoIndex == 1 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Visible cuando el modo activo es "Escrutinio contra jornadas".</summary>
    public Visibility ModoJornadasVisibility =>
        ModoSeleccionadoIndex == 2 ? Visibility.Visible : Visibility.Collapsed;

    partial void OnModoSeleccionadoIndexChanged(int value)
    {
        OnPropertyChanged(nameof(ModoSimpleVisibility));
        OnPropertyChanged(nameof(ModoFicheroVisibility));
        OnPropertyChanged(nameof(ModoJornadasVisibility));
    }

    // ===== Comunes =====
    /// <summary>Rango de número de aciertos a escrutar (txtNoAciertos, p.ej. "10-14").</summary>
    [ObservableProperty]
    private string _noAciertos = "10-14";

    /// <summary>Activa el registro de columnas premiadas (chkVerPremiadas).</summary>
    [ObservableProperty]
    private bool _verPremiadas;

    /// <summary>Ficheros de combinaciones seleccionados a escrutar (listaFicheros).</summary>
    public ObservableCollection<string> FicherosAEscrutar { get; } = new();

    // Rutas completas de los ficheros a escrutar (legacy: archivosComb, string[]).
    private readonly List<string> _archivosComb = new();

    // Ruta completa del fichero de referencia (legacy: archivoRef / lblFileRef.Tag).
    private string _archivoRef = string.Empty;

    // ===== Modo simple: columna ganadora =====
    /// <summary>14 casillas de signo de la columna ganadora (txtCG1..txtCG14).</summary>
    public ObservableCollection<SignoGanadorViewModel> SignosGanadores { get; } = new();

    /// <summary>Columna ganadora completa introducida como texto (txtColGanadora).</summary>
    [ObservableProperty]
    private string _columnaGanadoraTexto = string.Empty;

    // ===== Modo contra fichero =====
    /// <summary>Nombre del fichero de referencia seleccionado (lblFileRef).</summary>
    [ObservableProperty]
    private string _ficheroReferencia = "(selecciona)";

    // ===== Modo contra jornadas =====
    /// <summary>Plantilla de nombre de archivo con marcadores /t y /j (txtNombreArchBase).</summary>
    [ObservableProperty]
    private string _plantillaNombreArchivo = string.Empty;

    /// <summary>Carpeta donde buscar los ficheros de jornadas (lblCarpeta).</summary>
    [ObservableProperty]
    private string _carpetaJornadas = string.Empty;

    /// <summary>Dígitos de la temporada: false = 4 (rt4, por defecto), true = 2 (rt2).</summary>
    [ObservableProperty]
    private bool _temporadaDosDigitos;

    /// <summary>Dígitos de la jornada: false = 1 (rj1), true = 2 (rj2, por defecto).</summary>
    [ObservableProperty]
    private bool _jornadaDosDigitos = true;

    /// <summary>Temporadas disponibles para selección múltiple (lstTemporadas).</summary>
    public ObservableCollection<string> Temporadas { get; } = new();

    // ===== Estado de resultados =====
    /// <summary>Texto del cronómetro / estado del escrutinio (lblTime).</summary>
    [ObservableProperty]
    private string _tiempoTexto = string.Empty;

    /// <summary>
    /// Filas de resultado del escrutinio (dgResultados). Cada fila es una columna premiada
    /// con casilla de selección, ya que el grid de combinaciones legacy nunca se poblaba
    /// (EscrutadorComb.escrutinioDS quedaba null); las columnas premiadas
    /// (ListaEscrutadasConPremio) son la salida útil que se selecciona/graba.
    /// </summary>
    public ObservableCollection<CombinacionPremiadaItem> Resultados { get; } = new();

    /// <summary>Histograma de premios por categoría (nº de columnas con N aciertos).</summary>
    public ObservableCollection<string> Histograma { get; } = new();

    /// <summary>Visibilidad del mensaje "sin resultados" mientras la grilla esté vacía.</summary>
    public Visibility MensajeVacioVisibility =>
        Resultados.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    partial void OnTiempoTextoChanged(string value) => OnPropertyChanged(nameof(MensajeVacioVisibility));

    // ===== Acciones =====

    [RelayCommand]
    private async Task SeleccionarFicherosAsync()
    {
        // Legacy BtnFileOrigClick: OpenFileDialog multiselección (*.comb, *.xml) en "Columnas\\".
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".comb");
        picker.FileTypeFilter.Add(".xml");
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

    [RelayCommand]
    private async Task SeleccionarFicheroReferenciaAsync()
    {
        // Legacy BtnFileRefClick: OpenFileDialog (*.txt) en "Columnas\\".
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        _archivoRef = file.Path;
        FicheroReferencia = file.Name;
    }

    [RelayCommand]
    private async Task SeleccionarCarpetaAsync()
    {
        // Legacy btnBuscarCarpeta_Click: FolderBrowserDialog -> lblCarpeta.
        var picker = new FolderPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var folder = await picker.PickSingleFolderAsync();
        if (folder == null) return;

        CarpetaJornadas = folder.Path;
    }

    [RelayCommand]
    private void InsertarMarcadorTemporada()
    {
        // Legacy incluirPrefijo_click (btnIntrTemp): inserta "/t" en txtNombreArchBase.
        PlantillaNombreArchivo += "/t";
    }

    [RelayCommand]
    private void InsertarMarcadorJornada()
    {
        // Legacy incluirPrefijo_click (btnIntrJorn): inserta "/j" en txtNombreArchBase.
        PlantillaNombreArchivo += "/j";
    }

    [RelayCommand]
    private async Task VerArchivosAsync()
    {
        // Legacy btnVerArch_Click: OpenFileDialog y, si se elige un archivo, txtNombreArchBase = su nombre.
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSingleFileAsync();
        if (file is null) return;
        PlantillaNombreArchivo = file.Name;
    }

    [RelayCommand]
    private async Task EscrutarAsync()
    {
        // Validación previa equivalente a SonDatosValidos() (legacy línea 502).
        int tipoEscrutinio = ModoSeleccionadoIndex + 1; // 1=Simple, 2=Fichero, 3=Jornadas (legacy Tag).
        if (_archivosComb.Count == 0 && tipoEscrutinio != 3)
        {
            AppServices.MostrarError("Falta fichero a escrutar.");
            return;
        }
        if (tipoEscrutinio == 1)
        {
            string col = ObtenerColumnaGanadora();
            if (col.Replace("*", "") == string.Empty)
            {
                AppServices.MostrarError("Falta columna ganadora.");
                return;
            }
        }
        else if (tipoEscrutinio == 2 && _archivoRef.Length == 0)
        {
            AppServices.MostrarError("Falta fichero referencia.");
            return;
        }
        else if (tipoEscrutinio == 3)
        {
            if (PlantillaNombreArchivo.Length == 0)
            {
                AppServices.MostrarError("Falta plantilla de nombre de fichero.");
                return;
            }
            if (CarpetaJornadas.Length == 0)
            {
                AppServices.MostrarError("Falta la carpeta de los ficheros.");
                return;
            }
            if (PlantillaNombreArchivo.IndexOf("/t", StringComparison.Ordinal) < 0)
            {
                AppServices.MostrarError("No se ha puesto el indicador de temporada (/t).");
                return;
            }
            if (PlantillaNombreArchivo.IndexOf("/j", StringComparison.Ordinal) < 0)
            {
                AppServices.MostrarError("No se ha puesto el indicador de jornada (/j).");
                return;
            }
        }

        // RangosHelper.ObtenIntArray: convierte "10-14" en el array de nº de aciertos.
        int[] colAciertos = new RangosHelper().ObtenIntArray(NoAciertos);
        bool verPremiadas = VerPremiadas;
        int tipo = tipoEscrutinio;
        string colGan = ObtenerColumnaGanadora();
        string archivoRef = _archivoRef;
        var archivos = new List<string>(_archivosComb);
        string plantilla = PlantillaNombreArchivo;
        string carpeta = CarpetaJornadas;
        int dt = TemporadaDosDigitos ? 2 : 4;   // legacy rt4 (4) / rt2 (2).
        int dj = JornadaDosDigitos ? 2 : 1;      // legacy rj2 (2) / rj1 (1).
        var temporadasSel = new List<int>(TemporadasSeleccionadas);

        TiempoTexto = "Calculando...";
        Resultados.Clear();
        Histograma.Clear();
        OnPropertyChanged(nameof(MensajeVacioVisibility));

        var hora0 = DateTime.Now;

        // RealizaEscrutinio (legacy líneas 220-347): por modo crea EscrutadorComb, calcula los
        // posibles premios de la columna ganadora y escruta cada fichero de combinaciones.
        // NOTA: el legacy llamaba a AñadirPremiosGlobales (escribe en escrutinioDS, que en
        // EscrutadorComb es null) — lanzaría NullReferenceException; se omite a propósito.
        var salida = await Task.Run(() =>
        {
            // Histograma por categoría de aciertos. EscrutadorComb.PremiosTotales se indexa por
            // la posición de la categoría en su array Premios (que ObtenerPosiblesPremios ordena
            // descendente in-place), no por el nº de aciertos; por eso se agrega emparejando
            // Premios[i] (categoría) con PremiosTotales[i] (conteo).
            var globales = new Dictionary<int, int>();
            var premiadas = new List<ColumnasPremiadasComb>();

            if (tipo == 3)
            {
                EscrutarCombinacionesContraJornadas(colAciertos, verPremiadas, plantilla,
                    carpeta, dt, dj, temporadasSel, globales, premiadas);
            }
            else
            {
                foreach (string archivoComb in archivos)
                {
                    var escrutador = new EscrutadorComb(colAciertos)
                    {
                        ArchivoColumnas = archivoComb,
                        AñadirAGanadoras = verPremiadas,
                    };
                    // El motor añade premiadas a ListaEscrutadasConPremio; hay que inicializarla
                    // (en EscrutadorComb es null por defecto -> NullReferenceException si no).
                    if (verPremiadas) escrutador.ListaEscrutadasConPremio = new ArrayList();

                    if (tipo == 1)
                    {
                        escrutador.ObtenerPosiblesPremios(colGan, colAciertos);
                        escrutador.EscrutarCombinacion(0);
                    }
                    else // tipo == 2
                    {
                        IArchivoColumnas arch = new ArchivoColumnasTexto(archivoRef);
                        string[] ganadoras = arch.LeerTodasCols(false);
                        for (int jorn = 1; jorn <= ganadoras.Length; jorn++)
                        {
                            escrutador.ObtenerPosiblesPremios(ganadoras[jorn - 1], colAciertos);
                            escrutador.EscrutarCombinacion(jorn);
                        }
                    }

                    // PremiosTotales es acumulativo en el escrutador; se lee una vez al final
                    // (el legacy lo sumaba dentro del bucle, lo que duplicaba en el modo 2).
                    AcumularPremios(globales, escrutador);

                    if (verPremiadas && escrutador.ListaEscrutadasConPremio != null)
                    {
                        foreach (var p in escrutador.ListaEscrutadasConPremio)
                            premiadas.Add((ColumnasPremiadasComb)p);
                    }
                }
            }

            return (globales, premiadas);
        });

        // Premiadas -> filas de resultado seleccionables.
        _premiadas.Clear();
        foreach (var p in salida.premiadas)
        {
            _premiadas.Add(p);
            Resultados.Add(new CombinacionPremiadaItem
            {
                Columna = p.ColumnaTexto,
                Archivo = Path.GetFileName(p.Fichero ?? ""),
                Jornada = p.Jornada.ToString(),
                Premio = p.Premio.ToString(),
            });
        }

        // Histograma de premios por categoría (premiosTotales acumulado), de mayor a menor.
        var categorias = new List<int>(salida.globales.Keys);
        categorias.Sort();
        categorias.Reverse();
        foreach (int a in categorias)
        {
            Histograma.Add($"{a} aciertos: {salida.globales[a]}");
        }

        var hora9 = DateTime.Now;
        string tiempo = "Final = " + (hora9 - hora0);
        if (tiempo.Length >= 18) tiempo = tiempo.Substring(0, 18);
        TiempoTexto = tiempo;
        OnPropertyChanged(nameof(MensajeVacioVisibility));
    }

    // Lista de premiadas del último escrutinio (legacy: listaEscrutadasConPremio).
    private readonly List<ColumnasPremiadasComb> _premiadas = new();

    // Acumula PremiosTotales del escrutador en el histograma por categoría de aciertos.
    // EscrutadorComb empareja Premios[i] (categoría) con PremiosTotales[i] (conteo).
    private static void AcumularPremios(Dictionary<int, int> globales, EscrutadorComb escrutador)
    {
        int[] cats = escrutador.Premios;
        int[] tot = escrutador.PremiosTotales;
        int n = Math.Min(cats.Length, tot.Length);
        for (int i = 0; i < n; i++)
        {
            int cat = cats[i];
            globales[cat] = globales.TryGetValue(cat, out int v) ? v + tot[i] : tot[i];
        }
    }

    /// <summary>
    /// Rama tipoEscrutinio==3 de RealizaEscrutinio (combinaciones contra jornadas):
    /// localiza ficheros por plantilla /t /j, lee la columna ganadora del histórico y
    /// escruta con EscrutadorComb. Réplica de EscrutarCombinacionesFrm.cs líneas 238-318.
    /// </summary>
    private void EscrutarCombinacionesContraJornadas(
        int[] colAciertos, bool verPremiadas, string plantilla, string carpeta,
        int dt, int dj, List<int> temporadasSel, Dictionary<int, int> premiosGlobales,
        List<ColumnasPremiadasComb> premiadas)
    {
        if (_dsJornadas is null || _dsJornadas.Tables.Count == 0) return;

        const string numeros = "0123456789";
        var listaInicialArchivos = new List<string>();

        string consulta = "";
        foreach (int temp in temporadasSel)
            consulta += " or Temp=" + temp;
        if (consulta.Length > 0) consulta = consulta.Substring(4);

        var dv = new System.Data.DataView(_dsJornadas.Tables[0]) { RowFilter = consulta };
        if (dv.Count == 0) return;

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
                else { djLocal = 1; jorn = jorn.Substring(0, 1); }
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

            string rutaArchivo = Path.Combine(carpeta, archivo);
            var escrutador = new EscrutadorComb(colAciertos)
            {
                ArchivoColumnas = rutaArchivo,
                AñadirAGanadoras = verPremiadas,
            };
            if (verPremiadas) escrutador.ListaEscrutadasConPremio = new ArrayList();
            escrutador.ObtenerPosiblesPremios(colGanJornada, colAciertos);
            escrutador.EscrutarCombinacion(jornada);
            AcumularPremios(premiosGlobales, escrutador);

            if (verPremiadas && escrutador.ListaEscrutadasConPremio != null)
            {
                foreach (var p in escrutador.ListaEscrutadasConPremio)
                    premiadas.Add((ColumnasPremiadasComb)p);
            }
        }
    }

    /// <summary>
    /// Compone la columna ganadora a partir de los toggles de signos (legacy: ObtenColGanadora,
    /// Free1X2/UI/EscrutarCombinacionesFrm.cs línea 370). Vacío => comodín '*'.
    /// </summary>
    private string ObtenerColumnaGanadora()
    {
        // Si el usuario tecleó la columna como texto, tiene prioridad (legacy txtColGanadora).
        if (!string.IsNullOrWhiteSpace(ColumnaGanadoraTexto))
        {
            return ColumnaGanadoraTexto.Trim().ToUpper();
        }

        var sb = new System.Text.StringBuilder();
        foreach (var s in SignosGanadores)
        {
            if (s.Es1) sb.Append('1');
            else if (s.EsX) sb.Append('X');
            else if (s.Es2) sb.Append('2');
            else sb.Append('*');
        }
        return sb.ToString();
    }

    [RelayCommand]
    private void PosiblesPremios()
    {
        // Legacy btnPosiblesPremios_Click (Free1X2/UI/EscrutarCombinacionesFrm.cs línea 2014):
        //   new PosiblesPremiosFrm().Show().
        // TODO[navegación]: navegar a PosiblesPremiosFrmPage (su ViewModel ya está cableado).
        //   El host de navegación entre páginas no está en el alcance de este lote.
        AppServices.MostrarInfo("Posibles premios: la navegación entre páginas se cableará con el shell.");
    }

    [RelayCommand]
    private void SeleccionarTodas()
    {
        // Legacy btnEnableSel_Click -> PonerValorMarcadoGlobal(true).
        foreach (var item in Resultados) item.Seleccionado = true;
    }

    [RelayCommand]
    private void DeseleccionarTodas()
    {
        // Legacy btnDisabSel_Click -> PonerValorMarcadoGlobal(false).
        foreach (var item in Resultados) item.Seleccionado = false;
    }

    [RelayCommand]
    private async Task GrabarColumnasAsync()
    {
        // Legacy btnGrabaCols_Click: graba las columnas seleccionadas (rowView["Columna"]).
        var seleccionadas = new List<string>();
        foreach (var item in Resultados)
            if (item.Seleccionado && item.Columna.Length > 0)
                seleccionadas.Add(item.Columna);

        if (seleccionadas.Count == 0)
        {
            AppServices.MostrarInfo("No hay columnas seleccionadas que grabar.");
            return;
        }

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
            //   archivo.GuardarCols(columna) por fila; archivo.Cerrar().
            IArchivoColumnas archivo = new ArchivoColumnasTexto(file.Path);
            foreach (string columna in seleccionadas)
                archivo.GuardarCols(columna);
            archivo.Cerrar();
        });

        AppServices.MostrarInfo($"Guardadas {seleccionadas.Count} columna(s) en {file.Name}.");
    }

    /// <summary>Columnas premiadas a mostrar (legacy btnVerPremiadas_Click -> ColumnasPremiadasFrm).</summary>
    public ObservableCollection<CombinacionPremiadaItem> Premiadas { get; } = new();

    /// <summary>Visibilidad de la tarjeta de premiadas (sólo tras Ver Premiadas con datos).</summary>
    [ObservableProperty]
    private bool _mostrarPremiadas;

    [RelayCommand]
    private void VerPremiadasAccion()
    {
        // Legacy btnVerPremiadas_Click: vuelca listaPremiadas (ColumnasPremiadasComb) a la lista.
        Premiadas.Clear();
        foreach (var p in _premiadas)
        {
            Premiadas.Add(new CombinacionPremiadaItem
            {
                Columna = p.ColumnaTexto,
                Archivo = Path.GetFileName(p.Fichero ?? ""),
                Jornada = p.Jornada.ToString(),
                Premio = p.Premio.ToString(),
            });
        }
        if (Premiadas.Count == 0)
        {
            AppServices.MostrarInfo("No hay columnas premiadas. Activa «Ver Premiadas» antes de escrutar.");
            return;
        }
        MostrarPremiadas = true;
    }
}
