using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una relación "si ... entonces ..." ya añadida a una de las listas del filtro.
/// Equivale a un ListViewItem (texto + subitem) de listaCondiciones / listaGrupos del IfThenFrm legacy.
/// </summary>
public partial class RelacionIfThenViewModel : ObservableObject
{
    public RelacionIfThenViewModel(string si, string entonces)
    {
        Si = si;
        Entonces = entonces;
    }

    /// <summary>Parte "Si se produce ..." de la relación.</summary>
    public string Si { get; }

    /// <summary>Parte "... debe cumplirse que ..." de la relación.</summary>
    public string Entonces { get; }

    /// <summary>Marca de selección (la columna de checkboxes del ListView legacy, usada para borrar).</summary>
    [ObservableProperty]
    private bool _seleccionada;
}

/// <summary>
/// ViewModel de la pantalla "Condiciones relacionadas (if-then)".
/// Port del WinForms <c>IfThenFrm</c> (Free1X2/UI/IfThenFrm.cs). Opera sobre el
/// <see cref="Analizador"/> compartido (<c>AppState.Instancia.Analizador</c>):
///  - Condiciones sencillas: relaciona una condición de filtro (Si) con otra (Entonces).
///  - Grupos: relaciona un grupo de partidos (Si) con otro grupo (Entonces).
///
/// Aceptar construye un <see cref="ControladorIfThen"/> (guardarCondicion del legacy) y lo
/// asigna a <c>analizador.IfThen</c>. Guardar/Abrir/Copiar/Pegar usan
/// <see cref="ArchivoCondiciones"/> (.if/.xml) igual que el menú de condiciones legacy.
/// </summary>
public partial class IfThenFrmViewModel : ObservableObject
{
    // Lista de condiciones genéricas (comboCG_If / comboCG_Then del form legacy, método llenarCombo()).
    private static readonly IReadOnlyList<string> CondicionesGenericas = new[]
    {
        "Cantidad de signos",
        "Dibujos",
        "Signos Seguidos",
        "Interrupciones",
        "Pesos numéricos",
        "Distancias",
        "Contactos",
    };

    // Ruta del fichero temporal de copiar/pegar (legacy: StartupPath + "/Temp/tmp.if").
    private static string RutaTemporalIf =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.if");

    private readonly Analizador _analizador = AppState.Instancia.Analizador;

    /// <summary>Acción de cierre/volver (la cablea la página con Frame.GoBack()).</summary>
    public Action? Volver { get; set; }

    public IfThenFrmViewModel()
    {
        Relaciones = new ObservableCollection<RelacionIfThenViewModel>();
        RelacionesGrupos = new ObservableCollection<RelacionIfThenViewModel>();
        GruposDisponibles = new ReadOnlyCollection<string>(new List<string>());

        // Habilita Pegar si hay un fichero temporal previo (compruebaPegar() legacy).
        PuedePegar = File.Exists(RutaTemporalIf);
    }

    /// <summary>
    /// Rellena grupos disponibles desde el motor y carga el ControladorIfThen previo.
    /// Equivale a IfThenFrm.llenarGrupos() + cargarDatos(analizador.IfThen) (ctor legacy).
    /// La página la llama en OnNavigatedTo.
    /// </summary>
    public void CargarDesdeMotor()
    {
        LlenarGrupos();
        CargarDatos(_analizador.IfThen);
    }

    private void LlenarGrupos()
    {
        // legacy llenarGrupos(): "i - NombreGrupo" para i = 1..count-1.
        var lista = new List<string>();
        for (int i = 1; i < _analizador.GruposPartidos.Count; i++)
        {
            lista.Add(i + " - " + _analizador.GruposPartidos[i].NombreGrupo);
        }
        GruposDisponibles = new ReadOnlyCollection<string>(lista);
        // legacy: si solo hay un grupo, la pestaña de grupos no aplica.
        PestanaGruposDisponible = _analizador.GruposPartidos.Count > 1;
    }

    private void CargarDatos(ControladorIfThen? ifThen)
    {
        // Equivale a IfThenFrm.cargarDatos(ifThen).
        if (ifThen == null) return;

        Relaciones.Clear();
        RelacionesGrupos.Clear();

        foreach (CondicionIfThen condicion in ifThen.ControlesCondiciones)
        {
            Relaciones.Add(new RelacionIfThenViewModel(condicion.CondIf, condicion.CondThen));
        }
        if (ifThen.ControlesCondiciones.Count > 0)
        {
            RangoCondiciones = ifThen.RangoAciertoCondiciones ?? string.Empty;
        }

        foreach (GrupoIfThen grupo in ifThen.ControlesGrupos)
        {
            // Si los grupos del fichero no existen en la combinación actual, se descartan.
            if (grupo.NumGrupoIf >= _analizador.GruposPartidos.Count ||
                grupo.NumGrupoThen >= _analizador.GruposPartidos.Count)
            {
                AppServices.MostrarError(
                    "Uno de los grupos del fichero no existe en la combinación actual. Se quita de la lista.");
                continue;
            }
            string si = grupo.NumGrupoIf + " - " + _analizador.GruposPartidos[grupo.NumGrupoIf].NombreGrupo;
            if (grupo.NoIf) si = "(NO) " + si;
            string entonces = grupo.NumGrupoThen + " - " + _analizador.GruposPartidos[grupo.NumGrupoThen].NombreGrupo;
            if (grupo.NoThen) entonces = "(NO) " + entonces;
            RelacionesGrupos.Add(new RelacionIfThenViewModel(si, entonces));
        }
        if (ifThen.ControlesGrupos.Count > 0)
        {
            RangoGrupos = ifThen.RangoAciertoGrupos ?? string.Empty;
        }

        OnPropertyChanged(nameof(CondicionesEnListaTexto));
        OnPropertyChanged(nameof(GruposEnListaTexto));
    }

    // ======================= Pestaña 1: condiciones sencillas =======================

    /// <summary>Opciones del desplegable "Condición genérica" (mismas para Si y Entonces).</summary>
    public IReadOnlyList<string> CondicionesGenericasDisponibles => CondicionesGenericas;

    // ---- Bloque "Si ..." ----

    [ObservableProperty]
    private string? _condicionGenericaIf;

    [ObservableProperty]
    private IReadOnlyList<string> _condicionesEspecificasIf = new List<string>();

    [ObservableProperty]
    private string? _condicionEspecificaIf;

    /// <summary>Valor numérico del bloque Si (upDown_If). True si la condición usa upDown.</summary>
    [ObservableProperty]
    private double _valorIf;

    private bool _usaValorIf = true;

    [ObservableProperty]
    private bool _negadoIf;

    // ---- Bloque "Entonces ..." ----

    [ObservableProperty]
    private string? _condicionGenericaThen;

    [ObservableProperty]
    private IReadOnlyList<string> _condicionesEspecificasThen = new List<string>();

    [ObservableProperty]
    private string? _condicionEspecificaThen;

    [ObservableProperty]
    private double _valorThen;

    private bool _usaValorThen = true;

    [ObservableProperty]
    private bool _negadoThen;

    /// <summary>Relaciones añadidas en la pestaña de condiciones sencillas (listaCondiciones).</summary>
    public ObservableCollection<RelacionIfThenViewModel> Relaciones { get; }

    [ObservableProperty]
    private string _rangoCondiciones = string.Empty;

    /// <summary>Texto del contador "Condiciones en lista" (txtCondsLista). Solo lectura en la UI.</summary>
    public string CondicionesEnListaTexto => Relaciones.Count.ToString();

    // ======================= Pestaña 2: grupos =======================

    [ObservableProperty]
    private IReadOnlyList<string> _gruposDisponibles;

    /// <summary>Indica si la pestaña de grupos aplica (hay más de un grupo).</summary>
    [ObservableProperty]
    private bool _pestanaGruposDisponible;

    /// <summary>Habilita el comando Pegar si existe el fichero temporal.</summary>
    [ObservableProperty]
    private bool _puedePegar;

    [ObservableProperty]
    private string? _grupoIf;

    [ObservableProperty]
    private bool _negadoGrupoIf;

    [ObservableProperty]
    private string? _grupoThen;

    [ObservableProperty]
    private bool _negadoGrupoThen;

    /// <summary>Relaciones de grupos añadidas (listaGrupos).</summary>
    public ObservableCollection<RelacionIfThenViewModel> RelacionesGrupos { get; }

    [ObservableProperty]
    private string _rangoGrupos = string.Empty;

    /// <summary>Texto del contador "Condiciones en lista" de grupos (txtGruposLista).</summary>
    public string GruposEnListaTexto => RelacionesGrupos.Count.ToString();

    // ======================= Cascada de combos (cambioCombo legacy) =======================

    partial void OnCondicionGenericaIfChanged(string? value)
    {
        CondicionesEspecificasIf = RellenarEspecificas(value, out bool usaValor);
        _usaValorIf = usaValor;
        CondicionEspecificaIf = null;
    }

    partial void OnCondicionGenericaThenChanged(string? value)
    {
        CondicionesEspecificasThen = RellenarEspecificas(value, out bool usaValor);
        _usaValorThen = usaValor;
        CondicionEspecificaThen = null;
    }

    /// <summary>
    /// Rellena las condiciones específicas para una genérica. Equivale a IfThenFrm.cambioCombo().
    /// <paramref name="usaValor"/> indica si la genérica usa el upDown de valor (los Dibujos no).
    /// </summary>
    private static IReadOnlyList<string> RellenarEspecificas(string? generica, out bool usaValor)
    {
        usaValor = true;
        var items = new List<string>();
        switch (generica)
        {
            case "Cantidad de signos":
                items.Add("Cantidad de Variantes");
                items.Add("Cantidad de X");
                items.Add("Cantidad de 2");
                break;
            case "Dibujos":
                for (int i = 0; i <= VariablesGlobales.NumeroPartidos; i++)
                {
                    for (int j = 0; j <= VariablesGlobales.NumeroPartidos; j++)
                    {
                        if ((i + j) <= VariablesGlobales.NumeroPartidos)
                        {
                            items.Add("Dibujo " + i + "+" + j);
                        }
                    }
                }
                usaValor = false; // los Dibujos no llevan valor numérico.
                break;
            case "Signos Seguidos":
                items.Add("Signos Seguidos de Variantes");
                items.Add("Signos Seguidos de 1");
                items.Add("Signos Seguidos de X");
                items.Add("Signos Seguidos de 2");
                break;
            case "Interrupciones":
                items.Add("Interrupciones Globales");
                items.Add("Interrupciones de Variantes");
                items.Add("Interrupciones de 1");
                items.Add("Interrupciones de X");
                items.Add("Interrupciones de 2");
                items.Add("Interrupciones Seguidas Globales");
                items.Add("Interrupciones Seguidas de Variantes");
                items.Add("Interrupciones Seguidas de 1");
                items.Add("Interrupciones Seguidas de X");
                items.Add("Interrupciones Seguidas de 2");
                break;
            case "Pesos numéricos":
                items.Add("Peso numérico Global");
                items.Add("Peso numérico de Variantes");
                items.Add("Peso numérico de 1");
                items.Add("Peso numérico de X");
                items.Add("Peso numérico de 2");
                break;
            case "Distancias":
                items.Add("Distancia de Variantes");
                items.Add("Distancia de 1");
                items.Add("Distancia de X");
                items.Add("Distancia de 2");
                break;
            case "Contactos":
                items.Add("Contactos de 1X");
                items.Add("Contactos de 12");
                items.Add("Contactos de X2");
                items.Add("Contactos de 11");
                items.Add("Contactos de XX");
                items.Add("Contactos de 22");
                items.Add("Contactos de 1V");
                items.Add("Contactos de XV");
                items.Add("Contactos de 2V");
                items.Add("Contactos de VV");
                break;
        }
        return items;
    }

    // ======================= Comandos: condiciones sencillas =======================

    [RelayCommand]
    private void AnadirCondicion()
    {
        // Equivale a IfThenFrm.btnAdd_Click().
        string? txt = ComponerCondicion(CondicionGenericaIf, CondicionEspecificaIf, ValorIf, _usaValorIf, NegadoIf);
        string? txt2 = ComponerCondicion(CondicionGenericaThen, CondicionEspecificaThen, ValorThen, _usaValorThen, NegadoThen);

        if (txt == null || txt2 == null) return;

        // Las condiciones deben ser diferentes; dos Dibujos a la vez no se permiten.
        if (txt == txt2)
        {
            AppServices.MostrarError("Las condiciones deben ser diferentes");
            return;
        }
        if (txt.IndexOf("Dibujo", StringComparison.Ordinal) >= 0 &&
            txt2.IndexOf("Dibujo", StringComparison.Ordinal) >= 0)
        {
            AppServices.MostrarError("Las condiciones deben ser diferentes");
            return;
        }

        Relaciones.Add(new RelacionIfThenViewModel(txt, txt2));
        OnPropertyChanged(nameof(CondicionesEnListaTexto));
        ResetEntradaCondicion();
    }

    /// <summary>
    /// Compone el texto de una condición igual que IfThenFrm.btnAdd_Click(): genérica obligatoria,
    /// específica si existe (con ": " salvo "Dibujo"), valor del upDown si aplica y prefijo "(NO) ".
    /// Devuelve null si faltan datos obligatorios (equivale a los "return" del legacy).
    /// </summary>
    private static string? ComponerCondicion(string? generica, string? especifica, double valor, bool usaValor, bool negado)
    {
        if (string.IsNullOrEmpty(generica)) return null;

        string txt;
        if (!string.IsNullOrEmpty(especifica))
        {
            txt = especifica;
            if (especifica.IndexOf("Dibujo", StringComparison.Ordinal) < 0)
            {
                txt += ": ";
            }
        }
        else
        {
            // Sin específica: usa la genérica como prefijo; requiere valor.
            txt = generica + ": ";
        }

        if (usaValor || string.IsNullOrEmpty(especifica))
        {
            txt += ((int)valor).ToString();
        }

        if (negado) txt = "(NO) " + txt;
        return txt;
    }

    [RelayCommand]
    private void BorrarCondicionesSeleccionadas()
    {
        // Equivale a IfThenFrm.btnBorrar_Click(): quita las filas marcadas.
        for (int i = Relaciones.Count - 1; i >= 0; i--)
        {
            if (Relaciones[i].Seleccionada)
            {
                Relaciones.RemoveAt(i);
            }
        }
        OnPropertyChanged(nameof(CondicionesEnListaTexto));
    }

    // ======================= Comandos: grupos =======================

    [RelayCommand]
    private void AnadirGrupo()
    {
        // Equivale a IfThenFrm.btnAddGrupo_Click(): los grupos deben ser diferentes.
        if (string.IsNullOrWhiteSpace(GrupoIf) || string.IsNullOrWhiteSpace(GrupoThen)) return;
        if (GrupoIf == GrupoThen)
        {
            AppServices.MostrarError("Los grupos deben ser diferentes");
            return;
        }

        string si = (NegadoGrupoIf ? "(NO) " : string.Empty) + GrupoIf;
        string entonces = (NegadoGrupoThen ? "(NO) " : string.Empty) + GrupoThen;
        RelacionesGrupos.Add(new RelacionIfThenViewModel(si, entonces));
        OnPropertyChanged(nameof(GruposEnListaTexto));

        GrupoIf = null;
        GrupoThen = null;
        NegadoGrupoIf = false;
        NegadoGrupoThen = false;
    }

    [RelayCommand]
    private void BorrarGruposSeleccionados()
    {
        // Equivale a IfThenFrm.btnBorrarGrupo_Click().
        for (int i = RelacionesGrupos.Count - 1; i >= 0; i--)
        {
            if (RelacionesGrupos[i].Seleccionada)
            {
                RelacionesGrupos.RemoveAt(i);
            }
        }
        OnPropertyChanged(nameof(GruposEnListaTexto));
    }

    // ======================= Construcción del ControladorIfThen (guardarCondicion) =======================

    /// <summary>
    /// Construye un <see cref="ControladorIfThen"/> a partir de las listas y rangos.
    /// Equivale a IfThenFrm.guardarCondicion(). Devuelve null si falta un rango obligatorio.
    /// </summary>
    private ControladorIfThen? GuardarCondicion()
    {
        if (string.IsNullOrEmpty(RangoCondiciones) && Relaciones.Count > 0)
        {
            AppServices.MostrarError("Debe indicarse la cantidad de condiciones que se deben cumplir.");
            return null;
        }
        if (string.IsNullOrEmpty(RangoGrupos) && RelacionesGrupos.Count > 0)
        {
            AppServices.MostrarError("Debe indicarse la cantidad de grupos que se deben cumplir.");
            return null;
        }

        var ifThen = new ControladorIfThen();

        // Condiciones
        foreach (RelacionIfThenViewModel rel in Relaciones)
        {
            var cond = new CondicionIfThen { CondIf = rel.Si, CondThen = rel.Entonces };
            ifThen.AddCondiciones(cond);
        }
        if (Relaciones.Count > 0)
        {
            ifThen.RangoAciertoCondiciones = RangoCondiciones;
        }

        // Grupos
        foreach (RelacionIfThenViewModel rel in RelacionesGrupos)
        {
            var gr = new GrupoIfThen();
            string txt = rel.Si;
            if (txt.StartsWith("(NO)", StringComparison.Ordinal))
            {
                gr.NoIf = true;
                txt = txt.Substring(5);
            }
            int posicion = txt.IndexOf(" ", StringComparison.Ordinal);
            int numGrupo = Convert.ToInt16(txt.Substring(0, posicion));
            gr.NumGrupoIf = numGrupo;
            Grupo grupoIf = _analizador.GruposPartidos[numGrupo];

            txt = rel.Entonces;
            if (txt.StartsWith("(NO)", StringComparison.Ordinal))
            {
                gr.NoThen = true;
                txt = txt.Substring(5);
            }
            posicion = txt.IndexOf(" ", StringComparison.Ordinal);
            numGrupo = Convert.ToInt16(txt.Substring(0, posicion));
            gr.NumGrupoThen = numGrupo;
            Grupo grupoThen = _analizador.GruposPartidos[numGrupo];

            gr.GrupoIf = grupoIf;
            gr.GrupoThen = grupoThen;
            ifThen.AddGrupos(gr);
        }
        if (RelacionesGrupos.Count > 0)
        {
            ifThen.RangoAciertoGrupos = RangoGrupos;
        }

        return ifThen;
    }

    // ======================= Comandos: menú (MenuCondiciones legacy) =======================

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a IfThenFrm.menuCondiciones1_BOk(): guardarCondicion() + analizador.IfThen = ifThen.
        ControladorIfThen? ifThen = GuardarCondicion();
        if (ifThen == null) return;

        _analizador.IfThen = ifThen;
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a IfThenFrm.menuCondiciones1_BGuardar(): valida + SaveFileDialog (.if/.xml).
        ControladorIfThen? ifThen = GuardarCondicion();
        if (ifThen == null || ifThen.EsVacio) return;

        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Condiciones",
        };
        picker.FileTypeChoices.Add("Condiciones relacionadas", new List<string> { ".if" });
        picker.FileTypeChoices.Add("Condiciones relacionadas (XML)", new List<string> { ".xml" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        Windows.Storage.StorageFile? file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            var archComb = new ArchivoCondiciones { NombreArchivo = file.Path };
            archComb.GuardaArchivo(ifThen);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo guardar: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task Abrir()
    {
        // Equivale a IfThenFrm.menuCondiciones1_BAbrir(): OpenFileDialog (.if/.xml) + abrir().
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".if");
        picker.FileTypeFilter.Add(".xml");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        Windows.Storage.StorageFile? file = await picker.PickSingleFileAsync();
        if (file == null) return;

        AbrirDesde(file.Path);
    }

    private void AbrirDesde(string nombreArchivo)
    {
        // Equivale a IfThenFrm.abrir(): limpiar() + ArchivoCondiciones.LeeIfThen() + cargarDatos().
        Limpiar();
        try
        {
            var archComb = new ArchivoCondiciones();
            if (archComb.AbrirArchivoCombinacion(nombreArchivo))
            {
                ControladorIfThen ifThen = archComb.LeeIfThen();
                CargarDatos(ifThen);
            }
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo abrir: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Copiar()
    {
        // Equivale a IfThenFrm.menuCondiciones1_BCopiar(): guarda en Temp/tmp.if y habilita Pegar.
        ControladorIfThen? ifThen = GuardarCondicion();
        if (ifThen == null || ifThen.EsVacio) return;

        try
        {
            string ruta = RutaTemporalIf;
            Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
            var archComb = new ArchivoCondiciones { NombreArchivo = ruta };
            archComb.GuardaArchivo(ifThen);
            PuedePegar = true;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo copiar: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Pegar()
    {
        // Equivale a IfThenFrm.menuCondiciones1_BPegar(): abre Temp/tmp.if.
        if (File.Exists(RutaTemporalIf))
        {
            AbrirDesde(RutaTemporalIf);
        }
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a IfThenFrm.limpiar() (menuCondiciones1_BBorrar): vacía listas y rangos.
        Limpiar();
    }

    // ======================= Auxiliares =======================

    private void Limpiar()
    {
        ResetEntradaCondicion();
        Relaciones.Clear();
        RangoCondiciones = string.Empty;
        RelacionesGrupos.Clear();
        RangoGrupos = string.Empty;
        OnPropertyChanged(nameof(CondicionesEnListaTexto));
        OnPropertyChanged(nameof(GruposEnListaTexto));
    }

    private void ResetEntradaCondicion()
    {
        // Equivale a IfThenFrm.resetConds().
        CondicionGenericaIf = null;
        CondicionEspecificaIf = null;
        CondicionesEspecificasIf = new List<string>();
        ValorIf = 0;
        NegadoIf = false;
        _usaValorIf = true;

        CondicionGenericaThen = null;
        CondicionEspecificaThen = null;
        CondicionesEspecificasThen = new List<string>();
        ValorThen = 0;
        NegadoThen = false;
        _usaValorThen = true;
    }
}
