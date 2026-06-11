using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Añadir Equipo".
/// Replica los campos de entrada del WinForms <c>AgregarEquipoFrm</c>:
/// un nombre de equipo y la categoría a la que pertenece (1ª, 2ª, 2ªB o Int).
/// Al confirmar, el equipo se añade a la lista de su categoría si no existe ya.
/// </summary>
public partial class AgregarEquipoFrmViewModel : ObservableObject
{
    /// <summary>
    /// Categorías disponibles (equivalen a los RadioButton rdbPrimera / rdbSegunda /
    /// rdbSegundaB / rdbInt del WinForms). Se exponen como <c>ItemsSource</c> para un
    /// ComboBox: declarar <c>&lt;x:String&gt;</c> en línea junto a un <c>SelectedItem</c>
    /// enlazado con x:Bind TwoWay hace fallar (crash opaco) al XamlCompiler de Windows
    /// App SDK 1.6 (MarkupCompilePass1).
    /// </summary>
    public IReadOnlyList<string> Categorias { get; } = new[] { "1ª", "2ª", "2ªB", "Int" };

    // ===== Nombre del equipo (txtEquipo) =====
    [ObservableProperty]
    private string _nombreEquipo = "";

    // ===== Categoría seleccionada (RadioButtons) =====
    // Por defecto "1ª", igual que rdbPrimera.Checked = true en el WinForms.
    [ObservableProperty]
    private string _categoria = "1ª";

    // ===== Estado / feedback =====
    [ObservableProperty]
    private string _estado = "Preparado";

    /// <summary>
    /// Equivale a <c>btnNuevoEquipo_Click</c> del WinForms: valida el nombre y, según
    /// la categoría elegida, añade el equipo a la lista correspondiente si no existe.
    /// </summary>
    [RelayCommand]
    private void Anadir()
    {
        if (string.IsNullOrWhiteSpace(NombreEquipo))
        {
            Estado = "Introduce un nombre de equipo";
            return;
        }

        // TODO(dominio): portar la lógica de btnNuevoEquipo_Click de
        //   Free1X2.UI.AgregarEquipoFrm.
        //   El WinForms recibía cuatro ListBox por constructor (lista1, lista2,
        //   lista2B, listaInt) y, según el RadioButton activo, hacía:
        //       if (!listaX.Items.Contains(txtEquipo.Text)) listaX.Items.Add(...);
        //   En WinUI esas listas pasarán a ser colecciones de dominio/persistencia
        //   (gestión de equipos por categoría). Mapear Categoria:
        //       "1ª" -> lista1, "2ª" -> lista2, "2ªB" -> lista2B, "Int" -> listaInt.
        //   Tras añadir, el WinForms cerraba el formulario (Close()).

        Estado = $"Equipo \"{NombreEquipo}\" añadido a {Categoria} (pendiente de portar dominio)";
        NombreEquipo = "";
    }
}
