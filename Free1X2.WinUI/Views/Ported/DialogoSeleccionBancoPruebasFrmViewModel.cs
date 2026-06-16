using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Utils;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del formulario legacy WinForms "DialogoSeleccionBancoPruebasFrm".
///
/// El diálogo legacy muestra una tabla (DataGrid) con 18 conceptos del escrutinio del
/// Banco de Pruebas (aciertos de 14..10, veces con premio, premios totales/medios,
/// premio acumulado y % de recuperación). Cada fila tiene:
///   - Concepto (solo lectura)
///   - Checked  (incluir/excluir el concepto en el criterio de selección)
///   - Mínimo   (double, editable)
///   - Máximo   (double, editable)
///
/// El usuario acota cada concepto entre un mínimo y un máximo; al Aceptar se seleccionan
/// las columnas cuyos valores caen dentro de esos rangos (legacy: ValoresMinimosyMaximos /
/// clase Free1X2.Utils.TablaDeSeleccion, propiedad Cancelado).
///
/// Toda la lógica de cálculo / persistencia / cierre de diálogo queda como TODO citando la
/// clase legacy correspondiente (no se implementa aquí).
/// </summary>
public partial class DialogoSeleccionBancoPruebasFrmViewModel : ObservableObject
{
    // Texto descriptivo (legacy: label1, en color Maroon).
    public string Descripcion =>
        "Se seleccionarán las columnas comprendidas entre los valores mínimo y máximo " +
        "de los conceptos seleccionados.";

    // Filas del criterio de selección (legacy: ArrayList ValoresMinimosyMaximos de
    // Free1X2.Utils.TablaDeSeleccion, mapeado al DataGrid dataGrid1).
    public ObservableCollection<ConceptoSeleccionItem> Conceptos { get; } = new();

    // Indica si el usuario canceló (legacy: campo público bool Cancelado).
    [ObservableProperty]
    private bool _cancelado;

    /// <summary>True si el usuario pulsó "Aceptar" (equivale a Cancelado == false del legacy).</summary>
    public bool Aceptado { get; private set; }

    /// <summary>
    /// Resultado leído por el llamador (BancoPruebasFrm): los 18 tripletes concepto/min/max ya
    /// editados, como ArrayList de Free1X2.Utils.TablaDeSeleccion (legacy: ValoresMinimosyMaximos).
    /// Se rellena al pulsar "Aceptar".
    /// </summary>
    public ArrayList ValoresMinimosyMaximos { get; } = new();

    /// <summary>Etiquetas de los 18 conceptos, en el mismo orden que el ctor legacy (pMinMax 0..17).</summary>
    private static readonly string[] _etiquetasConceptos =
    {
        "Nº de veces 14 aciertos",
        "Nº de veces 13 aciertos",
        "Nº de veces 12 aciertos",
        "Nº de veces 11 aciertos",
        "Nº de veces 10 aciertos",
        "Nº de veces con premio",
        "Premio total de 14",
        "Premio total de 13",
        "Premio total de 12",
        "Premio total de 11",
        "Premio total de 10",
        "Premio acumulado",
        "Premio medio de 14",
        "Premio medio de 13",
        "Premio medio de 12",
        "Premio medio de 11",
        "Premio medio de 10",
        "% de recuperación",
    };

    public DialogoSeleccionBancoPruebasFrmViewModel()
    {
        // Pobla los 18 conceptos (legacy InicializaGrid). Los mínimos/máximos quedan a 0 hasta
        // que el llamador invoque Inicializar(pMinMax).
        foreach (var c in _etiquetasConceptos)
        {
            Conceptos.Add(new ConceptoSeleccionItem(c));
        }
    }

    /// <summary>
    /// Carga los mínimos/máximos por concepto (legacy: ctor DialogoSeleccionBancoPruebasFrm(double[,] pMinMax)).
    /// pMinMax es un array [18,2] con [concepto, 0] = mínimo y [concepto, 1] = máximo.
    /// </summary>
    public void Inicializar(double[,] pMinMax)
    {
        if (pMinMax == null) return;
        int filas = Math.Min(Conceptos.Count, pMinMax.GetLength(0));
        for (int i = 0; i < filas; i++)
        {
            // Asignar directamente los campos para no marcar "Incluido" en la carga inicial
            // (en el legacy el ctor crea TablaDeSeleccion(concepto, min, max) sin marcar Checked).
            Conceptos[i].EstablecerRango(pMinMax[i, 0], pMinMax[i, 1]);
        }
    }

    // Aceptar (legacy: button1_Click -> Cancelado = false; this.Close()).
    [RelayCommand]
    private void Aceptar()
    {
        Cancelado = false;
        Aceptado = true;

        // Construye el resultado equivalente a ValoresMinimosyMaximos del legacy:
        // un TablaDeSeleccion por concepto con su Checked/Minimo/Maximo editados.
        ValoresMinimosyMaximos.Clear();
        foreach (var c in Conceptos)
        {
            var fila = new TablaDeSeleccion(c.Concepto, c.Minimo, c.Maximo)
            {
                // El ctor de TablaDeSeleccion no toca Checked; lo fijamos según la casilla.
                Checked = c.Incluido,
            };
            ValoresMinimosyMaximos.Add(fila);
        }
        // El cierre/navegación lo gestiona el code-behind de la página (Frame.GoBack).
    }

    // Cancelar (legacy: btCancelar_Click -> Cancelado = true; this.Close()).
    [RelayCommand]
    private void Cancelar()
    {
        Cancelado = true;
        Aceptado = false;
        // El form legacy cerraba el diálogo descartando los cambios (lo hace el code-behind).
    }
}

/// <summary>
/// Fila editable del criterio de selección (legacy: Free1X2.Utils.TablaDeSeleccion).
/// Concepto es de solo lectura; Checked, Minimo y Maximo son editables.
/// En el legacy, al fijar Minimo o Maximo se marcaba automáticamente Checked = true.
/// </summary>
public partial class ConceptoSeleccionItem : ObservableObject
{
    public ConceptoSeleccionItem(string concepto)
    {
        Concepto = concepto;
    }

    // Solo lectura (legacy: TablaDeSeleccion.Concepto sin setter).
    public string Concepto { get; }

    [ObservableProperty]
    private bool _incluido;

    [ObservableProperty]
    private double _minimo;

    [ObservableProperty]
    private double _maximo;

    // Evita marcar "Incluido" durante la carga inicial de rangos (legacy: el ctor de
    // TablaDeSeleccion asigna min/max sin tocar Checked; solo los setters lo marcan).
    private bool _cargando;

    // Legacy: el setter de Minimo/Maximo marcaba Checked = true al editarse por el usuario.
    partial void OnMinimoChanged(double value)
    {
        if (!_cargando) Incluido = true;
    }

    partial void OnMaximoChanged(double value)
    {
        if (!_cargando) Incluido = true;
    }

    /// <summary>
    /// Establece el rango (mínimo/máximo) sin marcar "Incluido", para la carga inicial desde
    /// el array pMinMax del llamador (legacy: ctor TablaDeSeleccion(concepto, min, max)).
    /// </summary>
    public void EstablecerRango(double minimo, double maximo)
    {
        _cargando = true;
        Minimo = minimo;
        Maximo = maximo;
        _cargando = false;
    }
}
