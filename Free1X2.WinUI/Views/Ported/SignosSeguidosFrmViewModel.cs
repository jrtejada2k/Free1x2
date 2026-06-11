using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del filtro "Signos Seguidos" (WinForms <c>SignosSeguidosFrm</c>).
/// El filtro mantiene cuatro juegos de valores que indican la cantidad admitida de
/// Variantes, signos 1, signos X y signos 2 SEGUIDOS dentro de cada combinación.
/// Cada propiedad almacena la lista de cantidades admitidas (rango 0..15) tal como
/// el control legacy <c>OptionNumTol0_14</c> (cadena de tolerancias separadas por comas).
/// Además, cada concepto puede tener una lista de "Figuras" asociada (List&lt;long&gt; en
/// el dominio legacy: FigurasV/Figuras1/FigurasX/Figuras2), gestionada vía FigurasFiltrosFrm.
/// </summary>
public partial class SignosSeguidosFrmViewModel : ObservableObject
{
    // Cantidades admitidas de "Variantes" seguidas. Equivale a filtro.GetVariantes()/SetNoVariantes().
    [ObservableProperty]
    private string _variantes = string.Empty;

    // Cantidades admitidas de signos "1" seguidos. Equivale a filtro.GetUnos()/SetNoUnos().
    [ObservableProperty]
    private string _unos = string.Empty;

    // Cantidades admitidas de signos "X" seguidos. Equivale a filtro.GetEquis()/SetNoEquis().
    [ObservableProperty]
    private string _equis = string.Empty;

    // Cantidades admitidas de signos "2" seguidos. Equivale a filtro.GetDoses()/SetNoDoses().
    [ObservableProperty]
    private string _doses = string.Empty;

    // Indicadores de si cada concepto tiene "figuras" definidas (botón verde en el form legacy,
    // IndicarCondicionFiguras()). El contenido real (List<long>) lo gestiona el dominio legacy.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasVariantesVisibility))]
    private bool _figurasVariantesActivas;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasUnosVisibility))]
    private bool _figurasUnosActivas;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasEquisVisibility))]
    private bool _figurasEquisActivas;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasDosesVisibility))]
    private bool _figurasDosesActivas;

    // Visibilidad del indicador "Activas" por concepto. Se exponen como Visibility para no
    // bindear un bool directo a la UI (regla del proyecto).
    public Visibility FigurasVariantesVisibility =>
        FigurasVariantesActivas ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FigurasUnosVisibility =>
        FigurasUnosActivas ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FigurasEquisVisibility =>
        FigurasEquisActivas ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FigurasDosesVisibility =>
        FigurasDosesActivas ? Visibility.Visible : Visibility.Collapsed;

    // true si hay algún valor introducido (NecesitaGuardarDatos() del form legacy).
    public bool ContieneDatos =>
        !string.IsNullOrWhiteSpace(Variantes) ||
        !string.IsNullOrWhiteSpace(Unos) ||
        !string.IsNullOrWhiteSpace(Equis) ||
        !string.IsNullOrWhiteSpace(Doses);

    [RelayCommand]
    private void Aceptar()
    {
        // TODO: Dominio legacy — guardar valores en FiltroSignosSeguidos y activar la condición.
        //   FiltroSignosSeguidos.ReinicializaValores();
        //   FiltroSignosSeguidos.SetNoVariantes/SetNoUnos/SetNoEquis/SetNoDoses(...) con los valores de pantalla;
        //   si un campo está vacío usar "0,1,...,14" (todosValores).
        //   FiltroSignosSeguidos.FigurasV/Figuras1/FigurasX/Figuras2 = listas correspondientes (si Count > 0).
        //   FiltroSignosSeguidos.IsActive = ContieneDatos; FiltroSignosSeguidos.ContieneDatos = ContieneDatos;
        //   Grupo.ActivaFiltro(filtro); (equivale a menuCondiciones1_BOk -> ActualizarDatos() del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO: Navegación legacy — cerrar la ventana sin aplicar cambios
        //   (equivale a menuCondiciones1_BCancelar -> CerrarVentana() del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a menuCondiciones1_BBorrar -> reinstanciar FiltroSignosSeguidos + MarcarValores().
        Variantes = string.Empty;
        Unos = string.Empty;
        Equis = string.Empty;
        Doses = string.Empty;
        FigurasVariantesActivas = false;
        FigurasUnosActivas = false;
        FigurasEquisActivas = false;
        FigurasDosesActivas = false;
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO: Dominio legacy — ArchivoCondiciones.GuardaArchivo(FiltroSignosSeguidos)
        //   sobre un archivo *.seg (equivale a menuCondiciones1_BGuardar -> guardar() del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO: Dominio legacy — ArchivoCondiciones.AbrirArchivoCombinacion(...) + LeeCondicion()
        //   y volcar los valores del FiltroSignosSeguidos leído ("SignosSeguidos") a estas propiedades
        //   (equivale a menuCondiciones1_BAbrir -> abrir() del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void Copiar()
    {
        // TODO: Dominio legacy — ActualizarDatos() y guardar a "/Temp/tmp.seg"; habilitar Pegar
        //   (equivale a menuCondiciones1_BCopiar del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void Pegar()
    {
        // TODO: Dominio legacy — abrir "/Temp/tmp.seg" y volcar sus valores a estas propiedades
        //   (equivale a menuCondiciones1_BPegar del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO: Dominio legacy — construir FiltroSignosSeguidos temporal (ObtenerFiltroTemporal())
        //   y llamar CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, ".../Ganadoras/")
        //   mostrando el VisorEstadisticas (equivale a menuCondiciones1_BEstadisticas).
    }

    [RelayCommand]
    private void FigurasVariantes()
    {
        // TODO: Dominio legacy — abrir FigurasFiltrosFrm(figurasV, 10, new FiltroSignosSeguidos())
        //   y tras cerrar actualizar FigurasVariantesActivas = (figurasV.Count > 0)
        //   (equivale a btnFigurasV_Click del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void FigurasUnos()
    {
        // TODO: Dominio legacy — abrir FigurasFiltrosFrm(figuras1, 10, new FiltroSignosSeguidos())
        //   y tras cerrar actualizar FigurasUnosActivas = (figuras1.Count > 0)
        //   (equivale a btnFiguras1_Click del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void FigurasEquis()
    {
        // TODO: Dominio legacy — abrir FigurasFiltrosFrm(figurasX, 10, new FiltroSignosSeguidos())
        //   y tras cerrar actualizar FigurasEquisActivas = (figurasX.Count > 0)
        //   (equivale a btnFigurasX_Click del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void FigurasDoses()
    {
        // TODO: Dominio legacy — abrir FigurasFiltrosFrm(figuras2, 10, new FiltroSignosSeguidos())
        //   y tras cerrar actualizar FigurasDosesActivas = (figuras2.Count > 0)
        //   (equivale a btnFiguras2_Click del SignosSeguidosFrm legacy).
    }
}
