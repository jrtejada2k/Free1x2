// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida.GenerarCPs;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de la tabla "Configuración de CPs" del ConfigCPsFrm WinForms.
/// Cada fila define el rango de puntuación y las reglas de forzado de
/// fijos/dobles/triples para un tramo de la columna probable.
/// Columnas legacy: Desde, Hasta, Forzar Fijos, Num Fijos, Forzar Dobles,
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

    /// <summary>Nº de fijos en columna de fijos — columna "Num Fijos".</summary>
    [ObservableProperty]
    private double _numFijos;

    /// <summary>Sólo columnas de dobles (los más dobles) — columna "Forzar Dobles".</summary>
    [ObservableProperty]
    private bool _forzarDobles;

    /// <summary>Nº de dobles en columna de dobles — columna "Num Dobles".</summary>
    [ObservableProperty]
    private double _numDobles;

    /// <summary>Fuerza el triple si ningún signo se incluye en rango — columna "Forzar Triples".</summary>
    [ObservableProperty]
    private bool _forzarTriples;
}

/// <summary>
/// ViewModel de la pantalla "Configurar Columnas Probables"
/// (legacy: Free1X2.EntradaSalida.GenerarCPs.ConfigCPsFrm, en Free1X2/UI/ConfigurarCPs.cs).
///
/// El form legacy enlazaba el dataset tipado ColumnasProbables a un DataGrid con dos
/// DataGridTableStyle: "Tipos de CPs" (índice + nombre) y "Configuracion de CPs" (reglas
/// por tramo). Aquí se carga el dataset con DatosHelper.ObtenerDatos(); al elegir un tipo se
/// vuelcan sus filas de configuración a <see cref="Filas"/>, y al guardar se vuelve a escribir
/// en el dataset y se persiste con DatosHelper.GuardarDatos().
/// </summary>
public partial class ConfigCPsFrmViewModel : ObservableObject
{
    private const string TablaTipos = "Tipos de CPs";
    private const string TablaConfig = "Configuracion de CPs";

    // Dataset tipado cargado en memoria (legacy: dsConfCol).
    private readonly ColumnasProbables _dsConfCol;

    // Mapa nombre de tipo -> valor de la columna "Tipo" (entero), para filtrar la config.
    private readonly Dictionary<string, int> _tipoPorNombre = new();

    public ConfigCPsFrmViewModel()
    {
        // Legacy ConfigCPsFrm ctor: new DatosHelper().ObtenerDatos().
        var dh = new DatosHelper();
        _dsConfCol = dh.ObtenerDatos();

        var nombres = new List<string>();
        DataTable? tablaTipos = _dsConfCol.Tables[TablaTipos];
        if (tablaTipos is not null)
        {
            foreach (DataRow fila in tablaTipos.Rows)
            {
                string nombre = fila["Nombre"]?.ToString() ?? string.Empty;
                int tipo = fila["Tipo"] is int t ? t : Convert.ToInt32(fila["Tipo"]);
                if (!_tipoPorNombre.ContainsKey(nombre))
                {
                    _tipoPorNombre[nombre] = tipo;
                    nombres.Add(nombre);
                }
            }
        }

        Tipos = nombres;
        Filas = new ObservableCollection<ConfigCPFilaViewModel>();

        // Selecciona el primer tipo (si lo hay) y carga su configuración.
        _tipoSeleccionado = nombres.Count > 0 ? nombres[0] : string.Empty;
        CargarFilasDelTipo(_tipoSeleccionado);
    }

    /// <summary>Lista de tipos de columnas disponibles (tabla "Tipos de CPs").</summary>
    public IReadOnlyList<string> Tipos { get; }

    /// <summary>Tipo de columna actualmente seleccionado.</summary>
    [ObservableProperty]
    private string _tipoSeleccionado;

    /// <summary>Filas de configuración del tipo seleccionado (tabla "Configuracion de CPs").</summary>
    public ObservableCollection<ConfigCPFilaViewModel> Filas { get; }

    /// <summary>
    /// True cuando hay cambios sin guardar. En el form legacy el botón "Guardar" sólo se
    /// mostraba tras editar una celda (dgTipos_CurrentCellChanged).
    /// </summary>
    [ObservableProperty]
    private bool _hayCambiosSinGuardar;

    partial void OnTipoSeleccionadoChanged(string value)
    {
        CargarFilasDelTipo(value);
    }

    private void CargarFilasDelTipo(string nombreTipo)
    {
        Filas.Clear();

        DataTable? tablaConfig = _dsConfCol.Tables[TablaConfig];
        if (tablaConfig is null || string.IsNullOrEmpty(nombreTipo) ||
            !_tipoPorNombre.TryGetValue(nombreTipo, out int tipo))
        {
            return;
        }

        var dv = new DataView(tablaConfig)
        {
            RowFilter = "Tipo = " + tipo,
        };

        foreach (DataRowView fila in dv)
        {
            Filas.Add(new ConfigCPFilaViewModel(
                desde: ADouble(fila["Desde"]),
                hasta: ADouble(fila["Hasta"]),
                forzarFijos: ABool(fila["Forzar Fijos"]),
                numFijos: ADouble(fila["Num Fijos"]),
                forzarDobles: ABool(fila["Forzar Dobles"]),
                numDobles: ADouble(fila["Num Dobles"]),
                forzarTriples: ABool(fila["Forzar Triples"])));
        }
    }

    [RelayCommand]
    private void Guardar()
    {
        DataTable? tablaConfig = _dsConfCol.Tables[TablaConfig];
        if (tablaConfig is not null && _tipoPorNombre.TryGetValue(TipoSeleccionado ?? string.Empty, out int tipo))
        {
            // Volcar las filas editadas de vuelta al dataset, en orden, sobre las filas
            // existentes del tipo seleccionado (legacy: el DataGrid editaba el dataset in situ).
            var dv = new DataView(tablaConfig)
            {
                RowFilter = "Tipo = " + tipo,
            };

            int n = Math.Min(dv.Count, Filas.Count);
            for (int i = 0; i < n; i++)
            {
                DataRow row = dv[i].Row;
                ConfigCPFilaViewModel fila = Filas[i];
                row["Desde"] = (int)fila.Desde;
                row["Hasta"] = (int)fila.Hasta;
                row["Forzar Fijos"] = fila.ForzarFijos;
                row["Num Fijos"] = (int)fila.NumFijos;
                row["Forzar Dobles"] = fila.ForzarDobles;
                row["Num Dobles"] = (int)fila.NumDobles;
                row["Forzar Triples"] = fila.ForzarTriples;
            }
            _dsConfCol.AcceptChanges();
        }

        // Legacy ConfigCPsFrm.button3_Click: new DatosHelper().GuardarDatos(dsConfCol).
        var datosHelper = new DatosHelper();
        datosHelper.GuardarDatos(_dsConfCol);

        HayCambiosSinGuardar = false;
    }

    private static double ADouble(object valor)
        => valor is null || valor == DBNull.Value ? 0 : Convert.ToDouble(valor);

    private static bool ABool(object valor)
        => valor is not null && valor != DBNull.Value && Convert.ToBoolean(valor);
}
