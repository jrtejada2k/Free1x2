using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    // true si hay algún valor introducido (control NecesitaGuardarDatos() del form legacy).
    public bool ContieneDatos =>
        !string.IsNullOrWhiteSpace(DistanciasVar) ||
        !string.IsNullOrWhiteSpace(Distancias1) ||
        !string.IsNullOrWhiteSpace(DistanciasX) ||
        !string.IsNullOrWhiteSpace(Distancias2);

    [RelayCommand]
    private void Aceptar()
    {
        // TODO: Dominio legacy — guardar valores en FiltroDistancias y activar la condición.
        //   El Grupo a editar llega de la MainPage por el handoff estático
        //   AppState.GrupoEnEdicion (análogo a new DistanciasFrm(analizador.GruposPartidos[grupoPantalla])).
        //   Pasos:
        //     var grupo = Free1X2.WinUI.Services.AppState.GrupoEnEdicion;
        //     var filtro = (FiltroDistancias)grupo.GetFiltro(Filtro.Distancias.ToString());
        //     FiltroDistancias.SetNoIntVar/SetNoInt1/SetNoIntX/SetNoInt2(...) con los valores de pantalla.
        //     filtro.IsActive = ContieneDatos; filtro.ContieneDatos = ContieneDatos;
        //     grupo.ActivaFiltro(filtro); AppState.Instancia.NotificarCambio();
        //   (equivale a menuCondiciones1_BOk del DistanciasFrm legacy + ActualizaGrupoPantalla).
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
        // TODO: Dominio legacy — ArchivoCondiciones.GuardaArchivo(FiltroDistancias)
        //   (equivale a menuCondiciones1_BGuardar -> guardar() del DistanciasFrm legacy).
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO: Dominio legacy — ArchivoCondiciones.AbrirArchivoCombinacion(...) + LeeCondicion()
        //   y volcar los valores del FiltroDistancias leído a estas propiedades
        //   (equivale a menuCondiciones1_BAbrir -> abrir() del DistanciasFrm legacy).
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO: Dominio legacy — construir FiltroDistancias temporal y llamar
        //   CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, ".../Ganadoras/")
        //   y mostrar el VisorEstadisticas (equivale a menuCondiciones1_BEstadisticas).
    }
}
