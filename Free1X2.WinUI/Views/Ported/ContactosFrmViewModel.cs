using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de la condición de Contactos: una pareja de signos (p.ej. "1X")
/// y la lista de números de contactos admitidos (cadena separada por comas, 0-14).
/// Equivale a un control OptionNumTol0_14 del ContactosFrm WinForms.
/// </summary>
public partial class FilaContactoViewModel : ObservableObject
{
    public FilaContactoViewModel(string pareja, string valores)
    {
        Pareja = pareja;
        _valores = valores;
    }

    /// <summary>Etiqueta de la pareja de signos: 1X, 12, X2, 11, XX, 22, 1V, XV, 2V, VV.</summary>
    public string Pareja { get; }

    [ObservableProperty]
    private string _valores = string.Empty;
}

/// <summary>
/// ViewModel de la pantalla de filtro "Contactos".
/// Mantiene las 10 filas de parejas de signos y el estado de las figuras.
/// La persistencia/cálculo se delega al dominio legacy (ver TODOs en la Page).
/// </summary>
public partial class ContactosFrmViewModel : ObservableObject
{
    // Rango admitido por cada OptionNumTol0_14 (Minimo=0, Maximo=15 en el form legacy; 0-14 contactos posibles).
    private const string ValoresPorDefecto = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";

    public ContactosFrmViewModel()
    {
        Filas = new ObservableCollection<FilaContactoViewModel>
        {
            new("1X", ValoresPorDefecto),
            new("12", ValoresPorDefecto),
            new("X2", ValoresPorDefecto),
            new("11", ValoresPorDefecto),
            new("XX", ValoresPorDefecto),
            new("22", ValoresPorDefecto),
            new("1V", ValoresPorDefecto),
            new("XV", ValoresPorDefecto),
            new("2V", ValoresPorDefecto),
            new("VV", ValoresPorDefecto),
        };

        // TODO(dominio): cargar valores reales desde FiltroContactos del Grupo activo.
        //   var filtro = (FiltroContactos)grupo.GetFiltro(Filtro.Contactos.ToString());
        //   Filas[0].Valores = filtro.GetNum1X(); ... GetNum12/GetNumX2/.../GetNumVV()
        //   (equivale a ContactosFrm.MarcarValores()).
    }

    public ObservableCollection<FilaContactoViewModel> Filas { get; }

    /// <summary>True si hay figuras asociadas a la condición (botón "Figuras" resaltado en el form legacy).</summary>
    [ObservableProperty]
    private bool _tieneFiguras;

    [RelayCommand]
    private void EditarFiguras()
    {
        // TODO(dominio): abrir el selector de figuras y persistir la lista.
        //   Legacy: new FigurasFiltrosFrm(figuras, 10, new FiltroContactos()).ShowDialog();
        //   Tras cerrar: TieneFiguras = figuras.Count > 0; (equivale a IndicarCondicionFiguras()).
    }

    [RelayCommand]
    private void Aceptar()
    {
        // TODO(dominio): guardar las filas en FiltroContactos, activar el filtro y cerrar.
        //   Legacy ContactosFrm.menuCondiciones1_BOk():
        //     guardarDatos(filtro); filtro.UsaFiguras();
        //     FormPadre.analizador.GruposPartidos[...].ActivaFiltro(filtro);
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO(dominio): calcular estadísticas del filtro temporal.
        //   Legacy ContactosFrm.menuCondiciones1_BEstadisticas():
        //     new CalculadorEstadisticas().EstadisticasFiltro(filtroTemp, ".../Ganadoras/")
        //     -> VisorEstadisticas.
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO(dominio): serializar la condición a archivo (.cont/.xml).
        //   Legacy: ArchivoCondiciones.GuardaArchivo(filtro).
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO(dominio): leer condición desde archivo y recargar las filas.
        //   Legacy: ArchivoCondiciones.AbrirArchivoCombinacion(...) + LeeCondicion().
    }

    [RelayCommand]
    private void Borrar()
    {
        // TODO(dominio): vaciar el filtro (Grupo.ActivaFiltro(new Grupo(), "Contactos", true)).
        foreach (var fila in Filas)
        {
            fila.Valores = string.Empty;
        }
        TieneFiguras = false;
    }
}
