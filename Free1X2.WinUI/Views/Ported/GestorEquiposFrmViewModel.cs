using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Gestión de Equipos".
/// Replica el WinForms <c>GestorEquiposFrm</c>: cuatro listas de equipos por
/// categoría (1ª, 2ª, 2ªB e Internacionales) que se cargan desde los ficheros
/// <c>equipos1.dat</c>, <c>equipos2.dat</c>, <c>equipos2b.dat</c> y
/// <c>equiposInt.dat</c>. Permite mover equipos entre categorías, eliminarlos,
/// dar de alta nuevos y guardar los cambios.
///
/// La carga/guardado real (StreamReader/StreamWriter sobre los .dat) y la
/// apertura de <c>AgregarEquipoFrm</c> quedan como TODO de dominio.
/// </summary>
public partial class GestorEquiposFrmViewModel : ObservableObject
{
    // ===== Listas por categoría (lbEquipos1 / lbEquipos2 / lbEquipos2B / lbEquiposInt) =====
    public ObservableCollection<string> EquiposPrimera { get; } = new();
    public ObservableCollection<string> EquiposSegunda { get; } = new();
    public ObservableCollection<string> EquiposSegundaB { get; } = new();
    public ObservableCollection<string> EquiposInt { get; } = new();

    // ===== Selección actual de cada lista (ListView.SelectedItem) =====
    [ObservableProperty]
    private string? _seleccionPrimera;

    [ObservableProperty]
    private string? _seleccionSegunda;

    [ObservableProperty]
    private string? _seleccionSegundaB;

    [ObservableProperty]
    private string? _seleccionInt;

    // ===== Estado / feedback =====
    [ObservableProperty]
    private string _estado = "Preparado";

    public GestorEquiposFrmViewModel()
    {
        // TODO(dominio): portar el constructor de Free1X2.UI.GestorEquiposFrm,
        //   que llamaba a CargaEquipos(lbEquiposX, archivoEquiposX) para cada
        //   categoría. CargaEquipos abría el .dat con StreamReader (Encoding.Default)
        //   y añadía cada línea a la ListBox. Rellenar aquí EquiposPrimera /
        //   EquiposSegunda / EquiposSegundaB / EquiposInt desde la capa de
        //   persistencia. Las listas WinForms tenían Sorted = true.
    }

    // ===== Mover 1ª -> 2ª (btnASegunda_Click) =====
    [RelayCommand]
    private void MoverAPrimeraASegunda()
    {
        Mover(EquiposPrimera, EquiposSegunda, SeleccionPrimera);
    }

    // ===== Mover 2ª -> 1ª (btnAPrimera_Click) =====
    [RelayCommand]
    private void MoverASegundaAPrimera()
    {
        Mover(EquiposSegunda, EquiposPrimera, SeleccionSegunda);
    }

    // ===== Mover 2ª -> 2ªB (btnASegundaB_Click) =====
    [RelayCommand]
    private void MoverASegundaASegundaB()
    {
        Mover(EquiposSegunda, EquiposSegundaB, SeleccionSegunda);
    }

    // ===== Mover 2ªB -> 2ª (btnASegundaSube_Click) =====
    [RelayCommand]
    private void MoverASegundaBASegunda()
    {
        Mover(EquiposSegundaB, EquiposSegunda, SeleccionSegundaB);
    }

    // ===== Eliminar de cada categoría (btnEliminaDeX_Click) =====
    [RelayCommand]
    private void EliminarDePrimera() => Eliminar(EquiposPrimera, SeleccionPrimera);

    [RelayCommand]
    private void EliminarDeSegunda() => Eliminar(EquiposSegunda, SeleccionSegunda);

    [RelayCommand]
    private void EliminarDeSegundaB() => Eliminar(EquiposSegundaB, SeleccionSegundaB);

    [RelayCommand]
    private void EliminarDeInt() => Eliminar(EquiposInt, SeleccionInt);

    // ===== Nuevo equipo (btnNuevoEquipo_Click) =====
    [RelayCommand]
    private void NuevoEquipo()
    {
        // TODO(dominio): portar btnNuevoEquipo_Click de Free1X2.UI.GestorEquiposFrm,
        //   que abría AgregarEquipoFrm como diálogo pasándole las cuatro ListBox
        //   (new AgregarEquipoFrm(lbEquipos1, lbEquipos2, lbEquipos2B, lbEquiposInt)
        //    .ShowDialog()). En WinUI esto debe navegar a AgregarEquipoFrmPage o
        //   mostrar un ContentDialog que añada el equipo a la colección destino.
        Estado = "Nuevo equipo: pendiente de portar (AgregarEquipoFrm).";
    }

    // ===== Guardar todas las categorías (btnGuardar_Click) =====
    [RelayCommand]
    private void Guardar()
    {
        // TODO(dominio): portar GuardarEquiposTodasCategorias de
        //   Free1X2.UI.GestorEquiposFrm, que escribía cada lista a su .dat con
        //   StreamWriter (Encoding.Default): equipos1.dat, equipos2.dat,
        //   equipos2b.dat, equiposInt.dat.
        Estado = "Guardar archivos: pendiente de portar persistencia (.dat).";
    }

    // ----- Helpers de manipulación de listas (lógica de UI pura, sin dominio) -----

    /// <summary>
    /// Mueve el equipo seleccionado de <paramref name="origen"/> a
    /// <paramref name="destino"/> sin duplicar (igual que los btnA*_Click del
    /// WinForms, que comprobaban Items.Contains antes de Add y luego Remove).
    /// </summary>
    private void Mover(ObservableCollection<string> origen, ObservableCollection<string> destino, string? seleccion)
    {
        if (string.IsNullOrEmpty(seleccion)) { Estado = "Selecciona un equipo."; return; }
        if (!destino.Contains(seleccion)) destino.Add(seleccion);
        origen.Remove(seleccion);
        Estado = $"Movido \"{seleccion}\".";
    }

    private void Eliminar(ObservableCollection<string> lista, string? seleccion)
    {
        if (string.IsNullOrEmpty(seleccion)) { Estado = "Selecciona un equipo."; return; }
        lista.Remove(seleccion);
        Estado = $"Eliminado \"{seleccion}\".";
    }
}
