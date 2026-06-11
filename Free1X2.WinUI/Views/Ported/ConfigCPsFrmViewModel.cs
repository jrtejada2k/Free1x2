using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de la tabla "Configuración de CPs" del ConfigCPsFrm WinForms.
/// Cada fila define el rango de puntuación y las reglas de forzado de
/// fijos/dobles/triples para un tramo de la columna probable.
/// Columnas legacy: desde, Hasta, Forzar Fijos, Num Fijos, Forzar Dobles,
/// Num Dobles, Forzar Triples.
/// </summary>
public partial class ConfigCPFilaViewModel : ObservableObject
{
    public ConfigCPFilaViewModel(
        double desde,
        double hasta,
        bool forzarFijos,
        double numFijos,
        bool forzarDobles,
        double numDobles,
        bool forzarTriples)
    {
        _desde = desde;
        _hasta = hasta;
        _forzarFijos = forzarFijos;
        _numFijos = numFijos;
        _forzarDobles = forzarDobles;
        _numDobles = numDobles;
        _forzarTriples = forzarTriples;
    }

    /// <summary>Mínima puntuación para incluir el signo (columna "Desde").</summary>
    [ObservableProperty]
    private double _desde;

    /// <summary>Máxima puntuación para incluir el signo (columna "Hasta").</summary>
    [ObservableProperty]
    private double _hasta;

    /// <summary>Sólo columnas de fijos (los más fijos) — columna "Forzar Fijos".</summary>
    [ObservableProperty]
    private bool _forzarFijos;

    /// <summary>Nº de fijos en columna de fijos — columna "Nº Fijos".</summary>
    [ObservableProperty]
    private double _numFijos;

    /// <summary>Sólo columnas de dobles (los más dobles) — columna "Forzar Dobles".</summary>
    [ObservableProperty]
    private bool _forzarDobles;

    /// <summary>Nº de dobles en columna de dobles — columna "Nº Dobles".</summary>
    [ObservableProperty]
    private double _numDobles;

    /// <summary>Fuerza el triple si ningún signo se incluye en rango — columna "Forzar Triples".</summary>
    [ObservableProperty]
    private bool _forzarTriples;
}

/// <summary>
/// ViewModel de la pantalla "Configurar Columnas Probables"
/// (legacy: Free1X2.EntradaSalida.GenerarCPs.ConfigCPsFrm).
///
/// El form legacy mostraba dos tablas enlazadas (DataGrid con dos
/// DataGridTableStyle): "Tipos de CPs" (índice + nombre del tipo) y
/// "Configuración de CPs" (las reglas de rango y forzado por tramo).
/// Aquí se exponen como TipoSeleccionado + lista de tipos + filas de
/// configuración. La persistencia se delega al dominio legacy (ver TODOs).
/// </summary>
public partial class ConfigCPsFrmViewModel : ObservableObject
{
    public ConfigCPsFrmViewModel()
    {
        // Items de ejemplo. ComboBox con SelectedItem requiere ItemsSource
        // desde una propiedad IReadOnlyList<string> (regla anti-crash 3).
        Tipos = new[] { "Tipo 1", "Tipo 2", "Tipo 3" };
        _tipoSeleccionado = Tipos[0];

        Filas = new ObservableCollection<ConfigCPFilaViewModel>
        {
            new(desde: 0, hasta: 100, forzarFijos: false, numFijos: 0,
                forzarDobles: false, numDobles: 0, forzarTriples: false),
        };

        // TODO[dominio]: cargar los tipos y su configuración desde disco.
        //   Legacy: Free1X2.EntradaSalida.GenerarCPs.DatosHelper.ObtenerDatos()
        //   devuelve un dataset ColumnasProbables con las tablas
        //   "Tipos de CPs" y "Configuracion de CPs"; mapear cada fila a
        //   ConfigCPFilaViewModel. El dominio aún no está migrado.
    }

    /// <summary>Lista de tipos de columnas disponibles (tabla "Tipos de CPs").</summary>
    public IReadOnlyList<string> Tipos { get; }

    /// <summary>Tipo de columna actualmente seleccionado.</summary>
    [ObservableProperty]
    private string _tipoSeleccionado;

    /// <summary>Filas de configuración del tipo seleccionado (tabla "Configuracion de CPs").</summary>
    public ObservableCollection<ConfigCPFilaViewModel> Filas { get; }

    /// <summary>
    /// True cuando hay cambios sin guardar. En el form legacy el botón
    /// "Guardar" sólo se mostraba tras editar una celda (dgTipos_CurrentCellChanged).
    /// </summary>
    [ObservableProperty]
    private bool _hayCambiosSinGuardar;

    [RelayCommand]
    private void Guardar()
    {
        // TODO[dominio]: guardar datos a disco.
        //   Legacy ConfigCPsFrm.button3_Click:
        //     new DatosHelper().GuardarDatos(dsConfCol);
        //   Volcar Tipos/Filas al dataset ColumnasProbables antes de persistir.
        HayCambiosSinGuardar = false;
    }
}
