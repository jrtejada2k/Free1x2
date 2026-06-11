using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    // true si hay algún valor introducido (NecesitaGuardarDatos() del form legacy).
    public bool ContieneDatos =>
        !string.IsNullOrWhiteSpace(Variantes) ||
        !string.IsNullOrWhiteSpace(Equis) ||
        !string.IsNullOrWhiteSpace(Doses);

    [RelayCommand]
    private void Aceptar()
    {
        // TODO: Dominio legacy — guardar valores en FiltroNoVariantes y activar la condición.
        //   FiltroNoVariantes.ReinicializaValores();
        //   FiltroNoVariantes.SetNoVariantes/SetNoEquis/SetNoDoses(...) con los valores de pantalla.
        //   FiltroNoVariantes.IsActive = ContieneDatos; FiltroNoVariantes.ContieneDatos = ContieneDatos;
        //   Grupo.ActivaFiltro(filtro); (equivale a menuCondiciones1_BOk -> ActualizarDatos() del NoVariantesFrm legacy).
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO: Navegación legacy — cerrar la ventana sin aplicar cambios
        //   (equivale a menuCondiciones1_BCancelar -> CerrarVentana() del NoVariantesFrm legacy).
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
        // TODO: Dominio legacy — ArchivoCondiciones.GuardaArchivo(FiltroNoVariantes)
        //   sobre un archivo *.vx2 (equivale a menuCondiciones1_BGuardar -> guardar() del NoVariantesFrm legacy).
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO: Dominio legacy — ArchivoCondiciones.AbrirArchivoCombinacion(...) + LeeCondicion()
        //   y volcar los valores del FiltroNoVariantes leído ("NoVariantes") a estas propiedades
        //   (equivale a menuCondiciones1_BAbrir -> abrir() del NoVariantesFrm legacy).
    }

    [RelayCommand]
    private void Copiar()
    {
        // TODO: Dominio legacy — ActualizarDatos() y guardar a "/Temp/tmp.vx2"; habilitar Pegar
        //   (equivale a menuCondiciones1_BCopiar del NoVariantesFrm legacy).
    }

    [RelayCommand]
    private void Pegar()
    {
        // TODO: Dominio legacy — abrir "/Temp/tmp.vx2" y volcar sus valores a estas propiedades
        //   (equivale a menuCondiciones1_BPegar del NoVariantesFrm legacy).
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO: Dominio legacy — construir FiltroNoVariantes temporal (ObtenerFiltroTemporal())
        //   y llamar CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, ".../Ganadoras/")
        //   mostrando el VisorEstadisticas (equivale a menuCondiciones1_BEstadisticas).
    }
}
