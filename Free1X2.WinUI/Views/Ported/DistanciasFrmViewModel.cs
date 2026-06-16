using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del filtro "Distancias".
/// La distancia es el número máximo de partidos que separan dos signos iguales.
/// El filtro mantiene cuatro juegos de valores: Var (cualquier signo), 1, X y 2.
/// Cada propiedad almacena los valores admitidos (rango 0..15) tal como el control
/// legacy OptionNumTol0_14 (lista de tolerancias).
/// </summary>
public partial class DistanciasFrmViewModel : ObservableObject
{
    // Distancias para "Var" (cualquier signo). Equivale a filtro.GetIntVar()/SetNoIntVar().
    [ObservableProperty]
    private string _distanciasVar = string.Empty;

    // Distancias para el signo "1". Equivale a filtro.GetInt1()/SetNoInt1().
    [ObservableProperty]
    private string _distancias1 = string.Empty;

    // Distancias para el signo "X". Equivale a filtro.GetIntX()/SetNoIntX().
    [ObservableProperty]
    private string _distanciasX = string.Empty;

    // Distancias para el signo "2". Equivale a filtro.GetInt2()/SetNoInt2().
    [ObservableProperty]
    private string _distancias2 = string.Empty;

    /// <summary>
    /// Acción para volver a la pantalla anterior (la cablea la página con Frame.GoBack()).
    /// Equivale a CerrarVentana() del form legacy.
    /// </summary>
    public Action? Volver { get; set; }

    // true si hay algún valor introducido (control NecesitaGuardarDatos() del form legacy).
    public bool ContieneDatos =>
        !string.IsNullOrWhiteSpace(DistanciasVar) ||
        !string.IsNullOrWhiteSpace(Distancias1) ||
        !string.IsNullOrWhiteSpace(DistanciasX) ||
        !string.IsNullOrWhiteSpace(Distancias2);

    /// <summary>
    /// Vuelca los valores actuales del FiltroDistancias del grupo en edición a la pantalla.
    /// Equivale a DistanciasFrm.MarcarValores() (ctor del form legacy).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroDistancias)grupo.GetFiltro(Filtro.Distancias.ToString());
        DistanciasVar = filtro.GetIntVar();
        Distancias1 = filtro.GetInt1();
        DistanciasX = filtro.GetIntX();
        Distancias2 = filtro.GetInt2();
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a DistanciasFrm.menuCondiciones1_BOk -> guardarValores() + ActivaFiltro
        //   (Free1X2/UI/Filtros/DistanciasFrm.cs líneas 313-439).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroDistancias)grupo.GetFiltro(Filtro.Distancias.ToString());
        string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();

        filtro.ReinicializaValores();
        if (ContieneDatos)
        {
            if (filtro.ContieneDatos == false)
            {
                // Primera vez guardando datos: activar la condición.
                filtro.IsActive = true;
            }
            filtro.ContieneDatos = true;

            // Cada juego de valores: si está vacío, se rellena con "todos" (replica del legacy).
            filtro.SetNoIntVar(!string.IsNullOrWhiteSpace(DistanciasVar) ? DistanciasVar : todosValores);
            filtro.SetNoInt1(!string.IsNullOrWhiteSpace(Distancias1) ? Distancias1 : todosValores);
            filtro.SetNoIntX(!string.IsNullOrWhiteSpace(DistanciasX) ? DistanciasX : todosValores);
            filtro.SetNoInt2(!string.IsNullOrWhiteSpace(Distancias2) ? Distancias2 : todosValores);
        }
        else
        {
            filtro.IsActive = false;
            filtro.ContieneDatos = false;
        }

        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a borrarValores() del form legacy.
        DistanciasVar = string.Empty;
        Distancias1 = string.Empty;
        DistanciasX = string.Empty;
        Distancias2 = string.Empty;
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO[persistencia]: ArchivoCondiciones.GuardaArchivo(FiltroDistancias) a un .dist/.xml
        //   con un SaveFileDialog (equivale a menuCondiciones1_BGuardar -> guardar(),
        //   Free1X2/UI/Filtros/DistanciasFrm.cs líneas 462-497). Requiere portar el diálogo de
        //   archivo de WinUI; fuera del alcance del cableado de la condición (load + aceptar).
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO[persistencia]: ArchivoCondiciones.AbrirArchivoCombinacion(...) + LeeCondicion()
        //   y volcar los valores del FiltroDistancias leído a estas propiedades
        //   (equivale a menuCondiciones1_BAbrir -> abrir(), DistanciasFrm.cs líneas 446-485).
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO[estadísticas]: construir FiltroDistancias temporal (ObtenerFiltroTemporal) y llamar
        //   CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, ".../Ganadoras/") mostrando el
        //   VisorEstadisticas (equivale a menuCondiciones1_BEstadisticas, DistanciasFrm.cs líneas 546-556).
    }
}
