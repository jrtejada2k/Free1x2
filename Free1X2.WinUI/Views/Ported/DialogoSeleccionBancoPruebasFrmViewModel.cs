using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    public DialogoSeleccionBancoPruebasFrmViewModel()
    {
        // Los 18 conceptos del diálogo legacy (InicializaGrid + carga de pMinMax).
        // TODO(dominio): los mínimos/máximos reales provienen del array double[,] pMinMax
        //                que el form legacy recibe en su constructor. Aquí se inicializan a 0.
        string[] conceptos =
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

        foreach (var c in conceptos)
            Conceptos.Add(new ConceptoSeleccionItem(c));
    }

    // Aceptar (legacy: button1_Click -> Cancelado = false; this.Close()).
    [RelayCommand]
    private void Aceptar()
    {
        Cancelado = false;
        // TODO(dominio): el form legacy devolvía ValoresMinimosyMaximos (los rangos editados)
        //                al llamador y cerraba el diálogo. Reconectar al flujo de navegación /
        //                al consumidor de BancoPruebasFrm cuando exista el equivalente WinUI.
    }

    // Cancelar (legacy: btCancelar_Click -> Cancelado = true; this.Close()).
    [RelayCommand]
    private void Cancelar()
    {
        Cancelado = true;
        // TODO(dominio): el form legacy cerraba el diálogo descartando los cambios.
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

    // Legacy: el setter de Minimo/Maximo marcaba Checked = true al editarse.
    partial void OnMinimoChanged(double value) => Incluido = true;

    partial void OnMaximoChanged(double value) => Incluido = true;
}
