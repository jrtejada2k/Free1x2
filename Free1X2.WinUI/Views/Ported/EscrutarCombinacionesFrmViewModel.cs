using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

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
/// Esta clase reproduce únicamente la UI y el estado de entrada. Toda la lógica de
/// dominio (lectura de ficheros, EscrutadorComb, cálculo de premios, persistencia) se
/// deja marcada como TODO citando la clase legacy.
/// </summary>
public partial class EscrutarCombinacionesFrmViewModel : ObservableObject
{
    public EscrutarCombinacionesFrmViewModel()
    {
        for (int i = 1; i <= 14; i++)
        {
            SignosGanadores.Add(new SignoGanadorViewModel { Partido = i });
        }
    }

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

    /// <summary>Filas de resultado del escrutinio (dgResultados).</summary>
    public ObservableCollection<string> Resultados { get; } = new();

    /// <summary>Visibilidad del mensaje "sin resultados" mientras la grilla esté vacía.</summary>
    public Visibility MensajeVacioVisibility =>
        Resultados.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

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
    private void VerArchivos()
    {
        // TODO (dominio): replicar btnVerArch_Click de EscrutarCombinacionesFrm
        // (previsualizar los archivos que casan con la plantilla en la carpeta).
    }

    [RelayCommand]
    private void Escrutar()
    {
        // Validación previa equivalente a SonDatosValidos() (legacy línea 502), subconjunto
        // soportable sin el dataset de jornadas.
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

        // TODO (dominio): ejecutar el escrutinio real (legacy BtnComienzoClick + RealizaEscrutinio,
        //   Free1X2/UI/EscrutarCombinacionesFrm.cs línea 220-348). Bloqueado: usa EscrutadorComb
        //   (ObtenerPosiblesPremios + EscrutarCombinacion), que NO está portado al dominio:
        //   vive en Free1X2/Escrutinio/EscrutadorComb.cs. Cuando se porte, replicar:
        //     - RangosHelper.ObtenIntArray(NoAciertos) -> colAciertos.
        //     - InicializaResultadosDataSet() (legacy línea 139): DataSet "Resultados" con columnas
        //       Seleccionado/LineaID/Columna/Archivo y P0..P(NumeroPartidos). NUNCA pasar un DataSet
        //       sin la tabla "Resultados": el motor escribe filas vía PonerPremios y lanzaría
        //       NullReferenceException (ver CrearDataSetResultados de EscrutiniosFrmViewModel).
        //     - new EscrutadorComb(colAciertos) por modo, ObtenerPosiblesPremios(colGan, colAciertos),
        //       EscrutarCombinacion(jornada), acumular premiosGlobales y volcar a Resultados + TiempoTexto.
        AppServices.MostrarInfo(
            "El escrutinio de combinaciones requiere EscrutadorComb, que aún no está portado al dominio. " +
            "Los ficheros y parámetros ya están preparados.");
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
        // TODO (dominio): replicar btnEnableSel_Click — marcar todas las filas de dgResultados.
        //   Bloqueado: las filas de resultado las produce el escrutinio con EscrutadorComb
        //   (no portado, Free1X2/Escrutinio/EscrutadorComb.cs).
    }

    [RelayCommand]
    private void DeseleccionarTodas()
    {
        // TODO (dominio): replicar btnDisabSel_Click — desmarcar todas las filas de dgResultados.
        //   Bloqueado igual que SeleccionarTodas (EscrutadorComb no portado).
    }

    [RelayCommand]
    private void GrabarColumnas()
    {
        // TODO (dominio): replicar btnGrabaCols_Click — grabar las columnas seleccionadas
        //   (Free1X2/UI/EscrutarCombinacionesFrm.cs línea 1717). Bloqueado: requiere las filas
        //   de resultado generadas por EscrutadorComb (no portado).
    }

    [RelayCommand]
    private void VerPremiadasAccion()
    {
        // TODO (dominio): replicar btnVerPremiadas_Click — mostrar las columnas premiadas
        //   (Free1X2/UI/EscrutarCombinacionesFrm.cs línea 1741). Bloqueado: la lista de premiadas
        //   la acumula EscrutadorComb (no portado).
    }
}
