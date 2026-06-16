using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del filtro "Número de Variantes" (WinForms <c>NoVariantesFrm</c>).
/// El filtro mantiene tres juegos de valores que indican la cantidad admitida de
/// Variantes, de signos X y de signos 2 dentro de cada combinación.
/// Cada propiedad almacena la lista de cantidades admitidas (rango 0..15) tal como
/// el control legacy <c>OptionNumTol0_14</c> (cadena de tolerancias separadas por comas).
/// </summary>
public partial class NoVariantesFrmViewModel : ObservableObject
{
    // Cantidades admitidas de "Variantes". Equivale a filtro.GetVariantes()/SetNoVariantes().
    [ObservableProperty]
    private string _variantes = string.Empty;

    // Cantidades admitidas de signos "X". Equivale a filtro.GetEquis()/SetNoEquis().
    [ObservableProperty]
    private string _equis = string.Empty;

    // Cantidades admitidas de signos "2". Equivale a filtro.GetDoses()/SetNoDoses().
    [ObservableProperty]
    private string _doses = string.Empty;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    // true si hay algún valor introducido (NecesitaGuardarDatos() del form legacy).
    public bool ContieneDatos =>
        !string.IsNullOrWhiteSpace(Variantes) ||
        !string.IsNullOrWhiteSpace(Equis) ||
        !string.IsNullOrWhiteSpace(Doses);

    /// <summary>
    /// Vuelca los valores del FiltroNoVariantes del grupo en edición a la pantalla.
    /// Equivale a NoVariantesFrm.MarcarValores() (ctor del form legacy).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroNoVariantes)grupo.GetFiltro(Filtro.NoVariantes.ToString());
        Variantes = filtro.GetVariantes();
        Equis = filtro.GetEquis();
        Doses = filtro.GetDoses();
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a NoVariantesFrm.menuCondiciones1_BOk -> ActualizarDatos() + ActivaFiltro
        //   (Free1X2/UI/Filtros/NoVariantesFrm.cs líneas 238-341).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroNoVariantes)grupo.GetFiltro(Filtro.NoVariantes.ToString());
        string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();

        filtro.ReinicializaValores();
        if (ContieneDatos)
        {
            if (filtro.ContieneDatos == false)
            {
                filtro.IsActive = true;
            }
            filtro.ContieneDatos = true;

            filtro.SetNoVariantes(!string.IsNullOrWhiteSpace(Variantes) ? Variantes : todosValores);
            filtro.SetNoEquis(!string.IsNullOrWhiteSpace(Equis) ? Equis : todosValores);
            filtro.SetNoDoses(!string.IsNullOrWhiteSpace(Doses) ? Doses : todosValores);
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
        // Equivale a menuCondiciones1_BBorrar -> reinstanciar FiltroNoVariantes + MarcarValores().
        Variantes = string.Empty;
        Equis = string.Empty;
        Doses = string.Empty;
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO[persistencia]: ArchivoCondiciones.GuardaArchivo(FiltroNoVariantes) sobre un *.vx2/.xml
        //   con SaveFileDialog (equivale a menuCondiciones1_BGuardar -> guardar(),
        //   Free1X2/UI/Filtros/NoVariantesFrm.cs líneas 363-391). Requiere portar el diálogo de archivo.
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO[persistencia]: ArchivoCondiciones.AbrirArchivoCombinacion(...) + LeeCondicion()
        //   y volcar los valores leídos a estas propiedades (menuCondiciones1_BAbrir -> abrir(),
        //   NoVariantesFrm.cs líneas 348-384).
    }

    [RelayCommand]
    private void Copiar()
    {
        // TODO[persistencia]: ActualizarDatos() y guardar a "/Temp/tmp.vx2"; habilitar Pegar
        //   (equivale a menuCondiciones1_BCopiar, NoVariantesFrm.cs líneas 409-417).
    }

    [RelayCommand]
    private void Pegar()
    {
        // TODO[persistencia]: abrir "/Temp/tmp.vx2" y volcar sus valores a estas propiedades
        //   (equivale a menuCondiciones1_BPegar, NoVariantesFrm.cs líneas 419-429).
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO[estadísticas]: construir FiltroNoVariantes temporal (ObtenerFiltroTemporal()) y llamar
        //   CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, ".../Ganadoras/") mostrando el
        //   VisorEstadisticas (equivale a menuCondiciones1_BEstadisticas, NoVariantesFrm.cs líneas 440-450).
    }
}
