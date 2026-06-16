using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Añadir Equipo".
/// Replica los campos de entrada del WinForms <c>AgregarEquipoFrm</c>:
/// un nombre de equipo y la categoría a la que pertenece (1ª, 2ª, 2ªB o Int).
///
/// En WinForms este formulario recibía las cuatro ListBox del GestorEquiposFrm y,
/// según el RadioButton activo, añadía el nombre a la lista correspondiente si no
/// existía. Como página independiente de WinUI no comparte ListBox con el gestor,
/// por lo que persiste directamente al fichero .dat de la categoría destino
/// (leer existentes -> añadir si falta -> reescribir), manteniendo el mismo
/// formato y codificación que GestorEquiposFrm.
/// </summary>
public partial class AgregarEquipoFrmViewModel : ObservableObject
{
    // Mismo directorio/ficheros que GestorEquiposFrm (legacy: StartupPath + "/Equipos/equiposX.dat").
    private static string DirEquipos =>
        AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

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
        // Legacy: if (txtEquipo.Text != "") { ... } Close();
        if (string.IsNullOrWhiteSpace(NombreEquipo))
        {
            Estado = "Introduce un nombre de equipo";
            return;
        }

        // Legacy mapeo RadioButton -> lista destino:
        //   rdbPrimera -> equipos1.dat, rdbSegunda -> equipos2.dat,
        //   rdbSegundaB -> equipos2b.dat, else (rdbInt) -> equiposInt.dat.
        string archivo = Categoria switch
        {
            "2ª" => DirEquipos + "/Equipos/equipos2.dat",
            "2ªB" => DirEquipos + "/Equipos/equipos2b.dat",
            "Int" => DirEquipos + "/Equipos/equiposInt.dat",
            _ => DirEquipos + "/Equipos/equipos1.dat",
        };

        try
        {
            var equipos = LeerEquipos(archivo);

            // Legacy: if (!listaX.Items.Contains(txtEquipo.Text)) listaX.Items.Add(...).
            if (equipos.Contains(NombreEquipo))
            {
                Estado = $"El equipo \"{NombreEquipo}\" ya existe en {Categoria}.";
                return;
            }

            equipos.Add(NombreEquipo);
            GuardarEquipos(archivo, equipos);

            Estado = $"Equipo \"{NombreEquipo}\" añadido a {Categoria}.";
            NombreEquipo = "";
        }
        catch (Exception ex)
        {
            Estado = $"Error al añadir el equipo: {ex.Message}";
            Services.AppServices.MostrarError($"No se pudo añadir el equipo: {ex.Message}");
        }
    }

    /// <summary>
    /// Lee los equipos de un .dat (mismo formato que GestorEquiposFrm.CargaEquipos).
    /// Encoding.Latin1 ≡ Encoding.Default de WinForms en el Windows español.
    /// </summary>
    private static List<string> LeerEquipos(string archivo)
    {
        var equipos = new List<string>();
        if (!File.Exists(archivo))
        {
            return equipos;
        }
        using var sr = new StreamReader(archivo, Encoding.Latin1);
        while (sr.Peek() != -1)
        {
            string? linea = sr.ReadLine();
            if (linea is not null)
            {
                equipos.Add(linea);
            }
        }
        return equipos;
    }

    /// <summary>Escribe los equipos a su .dat (mismo formato que GestorEquiposFrm.GuardarEquipos).</summary>
    private static void GuardarEquipos(string archivo, List<string> equipos)
    {
        string? dir = Path.GetDirectoryName(archivo);
        if (dir is not null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        using var sw = new StreamWriter(archivo, false, Encoding.Latin1);
        foreach (string equipo in equipos)
        {
            sw.WriteLine(equipo);
        }
    }
}
