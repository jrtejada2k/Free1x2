using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

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
    private void SeleccionarFicheros()
    {
        // TODO (dominio): replicar BtnFileOrigClick de EscrutarCombinacionesFrm.
        // Abrir OpenFileDialog multiselección (*.comb, *.xml) en "Columnas\\" y volcar
        // los nombres en FicherosAEscrutar (legacy: archivosComb / listaFicheros).
    }

    [RelayCommand]
    private void SeleccionarFicheroReferencia()
    {
        // TODO (dominio): replicar BtnFileRefClick de EscrutarCombinacionesFrm.
        // Abrir OpenFileDialog (*.txt) en "Columnas\\" y asignar a FicheroReferencia
        // (legacy: archivoRef / lblFileRef + lblFileRef.Tag).
    }

    [RelayCommand]
    private void SeleccionarCarpeta()
    {
        // TODO (dominio): replicar btnBuscarCarpeta_Click de EscrutarCombinacionesFrm.
        // Abrir FolderBrowserDialog y asignar a CarpetaJornadas (legacy: lblCarpeta).
    }

    [RelayCommand]
    private void InsertarMarcadorTemporada()
    {
        // TODO (dominio): replicar incluirPrefijo_click (btnIntrTemp) — insertar "/t"
        // en PlantillaNombreArchivo (legacy: txtNombreArchBase).
    }

    [RelayCommand]
    private void InsertarMarcadorJornada()
    {
        // TODO (dominio): replicar incluirPrefijo_click (btnIntrJorn) — insertar "/j"
        // en PlantillaNombreArchivo (legacy: txtNombreArchBase).
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
        // TODO (dominio): replicar BtnComienzoClick + RealizaEscrutinio de
        // EscrutarCombinacionesFrm. Validar (SonDatosValidos), construir colAciertos
        // (RangosHelper), instanciar EscrutadorComb por modo (Simple/Fichero/Jornadas),
        // ObtenerPosiblesPremios + EscrutarCombinacion, acumular premios globales y
        // volcar a Resultados + TiempoTexto.
    }

    [RelayCommand]
    private void PosiblesPremios()
    {
        // TODO (dominio): replicar btnPosiblesPremios_Click de EscrutarCombinacionesFrm.
    }

    [RelayCommand]
    private void SeleccionarTodas()
    {
        // TODO (dominio): replicar btnEnableSel_Click — marcar todas las filas de dgResultados.
    }

    [RelayCommand]
    private void DeseleccionarTodas()
    {
        // TODO (dominio): replicar btnDisabSel_Click — desmarcar todas las filas de dgResultados.
    }

    [RelayCommand]
    private void GrabarColumnas()
    {
        // TODO (dominio): replicar btnGrabaCols_Click — grabar las columnas seleccionadas.
    }

    [RelayCommand]
    private void VerPremiadasAccion()
    {
        // TODO (dominio): replicar btnVerPremiadas_Click — mostrar las columnas premiadas.
    }
}
