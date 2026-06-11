using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
/// Una fila de la lista "Una combinación por jornada" (legacy clase Free1X2.Combinacion,
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
/// ViewModel de la pantalla "Definición múltiple de jornadas y ficheros".
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
/// Los datos viven en memoria; la lectura/escritura de ficheros y la validación siguen en
/// el dominio legacy (ver los TODO en DialogoAnalisisMultipleDeTramosFrmPage.xaml.cs).
/// </summary>
public partial class DialogoAnalisisMultipleDeTramosFrmViewModel : ObservableObject
{
    // ===== Fichero de valoraciones históricas (legacy txNombreFicheroValoraciones) =====
    [ObservableProperty]
    private string _ficheroValoraciones = string.Empty;

    // ===== Modo "Combinación única" (legacy tab14T) =====

    /// <summary>Temporadas leídas del fichero (legacy chkTemporadas / ListaTemporadas).</summary>
    public ObservableCollection<TemporadaSeleccionableViewModel> Temporadas { get; } = new();

    /// <summary>Origen de la combinación: true = 14 triples, false = fichero (legacy rb14Triples / rbFichero).</summary>
    [ObservableProperty]
    private bool _usar14Triples = true;

    /// <summary>Fichero de combinación cuando NO se usan 14 triples (legacy txFichero / FicheroCombinación).</summary>
    [ObservableProperty]
    private string _ficheroCombinacion = string.Empty;

    // ===== Modo "Una combinación por jornada" (legacy tabFicheros) =====

    /// <summary>Lista de tripletes temporada/jornada/fichero (legacy ListaCombinaciones).</summary>
    public ObservableCollection<CombinacionFilaViewModel> Combinaciones { get; } = new();

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

    public DialogoAnalisisMultipleDeTramosFrmViewModel()
    {
        // TODO (dominio): si se recibe un fichero de valoraciones inicial, asignarlo a
        // FicheroValoraciones y poblar Temporadas mediante CargarListaDeTemporadas()
        // (legacy: lee con Free1X2.EntradaSalida.ArchivoColumnasTexto, valida 44 campos
        // por columna y agrupa por temporada).
    }

    // ===== Comandos del fichero de valoraciones =====

    [RelayCommand]
    private void SeleccionarFicheroValoraciones()
    {
        // TODO (dominio): equivalente de btSeleccionarFichero_Click — abrir un
        // FileOpenPicker (filtro *.txt) en la carpeta "Columnas\\", asignar la ruta a
        // FicheroValoraciones y luego CargarListaDeTemporadas() para poblar Temporadas.
    }

    // ===== Comandos del modo "Combinación única" =====

    [RelayCommand]
    private void SeleccionarCombinacion()
    {
        // TODO (dominio): equivalente de btSeleccionarCombi_Click — abrir un FileOpenPicker
        // (filtro *.txt) en "Columnas\\", asignar la ruta a FicheroCombinacion y poner
        // Usar14Triples = false (legacy Es14Triples = 1).
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
    private void AnadirCombinacion()
    {
        // TODO (dominio): equivalente de btAdd_Click — abrir un FileOpenPicker multiselect
        // (filtro *.txt) en "Columnas\\", validar cada fichero con EsFicheroDeColumnas(...)
        // (las 14 primeras posiciones deben ser 1/x/X/2) y, por cada válido, crear un
        // Free1X2.Combinacion(TemporadaTexto, jornada con padding, ruta) y añadirlo a
        // Combinaciones; después avanzar la jornada.
    }

    [RelayCommand]
    private void EliminarCombinacion()
    {
        // TODO (dominio/UI): equivalente de btEliminar_Click — eliminar de Combinaciones
        // las filas seleccionadas en la lista (legacy recorría dgListaFicheros.IsSelected).
    }

    [RelayCommand]
    private void GuardarLista()
    {
        // TODO (dominio): equivalente de btGuardar_Click — abrir un FileSavePicker
        // (filtro *.lst) en "Lista\\" y volcar cada Combinacion ("Temporada Jornada Path")
        // con Free1X2.EntradaSalida.ArchivoColumnasTexto.GuardarCols(...).
    }

    [RelayCommand]
    private void LeerLista()
    {
        // TODO (dominio): equivalente de btLeer_Click — abrir un FileOpenPicker (filtro
        // *.lst) en "Lista\\", limpiar Combinaciones y rellenarla parseando cada línea
        // (temporada 0..9, jornada 10..12, ruta resto) con ArchivoColumnasTexto.
    }

    // ===== Acciones del diálogo =====

    [RelayCommand]
    private void Aceptar()
    {
        // TODO (dominio): equivalente de btAceptar_Click — validar que hay fichero de
        // valoraciones y, según el modo activo, que hay al menos una temporada marcada
        // ("Combinación única") o que la lista de combinaciones no está vacía
        // ("Una combinación por jornada"); si falta información avisar, si no cerrar el
        // diálogo devolviendo los datos al MainForm.
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO (dominio): equivalente de btCancelar_Click — descartar el fichero de
        // valoraciones y cerrar el diálogo sin devolver datos.
    }
}
