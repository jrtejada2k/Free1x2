using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una línea de formato dentro de un <see cref="FormatosViewModel"/>:
/// la secuencia de signos (1/X/2/V/*) y su rango de apariciones Min-Max.
/// Equivale al tipo de dominio legacy Free1X2.MotorCalculo.FormatoSignos.
/// </summary>
public partial class LineaFormatoViewModel : ObservableObject
{
    [ObservableProperty]
    private string _formato = string.Empty;

    [ObservableProperty]
    private string _rangoAparicion = string.Empty;
}

/// <summary>
/// Un conjunto de líneas de formato (una "relación"), con sus límites
/// globales de Líneas y Global. Equivale a Free1X2.MotorCalculo.FormatosSignos.
/// </summary>
public partial class FormatosViewModel : ObservableObject
{
    public ObservableCollection<LineaFormatoViewModel> Lineas { get; } = new();

    /// <summary>Límite de líneas para esta relación de formatos.</summary>
    [ObservableProperty]
    private string _limiteLineas = string.Empty;

    /// <summary>Límite global para esta relación de formatos.</summary>
    [ObservableProperty]
    private string _global = string.Empty;

    public FormatosViewModel()
    {
        // El form legacy muestra 30 filas en blanco por relación.
        for (int i = 0; i < FilasPorRelacion; i++)
        {
            Lineas.Add(new LineaFormatoViewModel());
        }
    }

    public const int FilasPorRelacion = 30;
}

/// <summary>
/// ViewModel de la pantalla "Formatos (1,X,2,V,*)".
///
/// Un formato es una determinada secuencia de signos; esta condición controla
/// la repetición o aparición de diferentes formatos en las columnas generadas.
/// El usuario define una o varias relaciones de formatos navegables (1/N) y, por
/// cada una, hasta 30 líneas (secuencia + rango Min-Max) más los límites Líneas/Global.
///
/// Datos en memoria; la persistencia y el cálculo aún viven en el dominio legacy
/// (ver los TODO en FormatosFrmPage.xaml.cs).
/// </summary>
public partial class FormatosFrmViewModel : ObservableObject
{
    public ObservableCollection<FormatosViewModel> Relaciones { get; } = new();

    [ObservableProperty]
    private int _indiceRelacion;

    public FormatosFrmViewModel()
    {
        // TODO (dominio): poblar a partir de
        //   Free1X2.MotorCalculo.FiltroFormatosSignos.FormatosSignos
        // obtenido de Grupo.GetFiltro("FormatosSignos"). Por ahora, una relación vacía.
        Relaciones.Add(new FormatosViewModel());
        IndiceRelacion = 0;
    }

    /// <summary>Relación de formatos actualmente visible.</summary>
    public FormatosViewModel? RelacionActual =>
        IndiceRelacion >= 0 && IndiceRelacion < Relaciones.Count
            ? Relaciones[IndiceRelacion]
            : null;

    /// <summary>Contador "N/Total" mostrado en la cabecera (legacy lblNoFormatos).</summary>
    public string ContadorTexto =>
        Relaciones.Count == 0 ? "0/0" : $"{IndiceRelacion + 1}/{Relaciones.Count}";

    public bool PuedeRetroceder => IndiceRelacion > 0;

    partial void OnIndiceRelacionChanged(int value)
    {
        OnPropertyChanged(nameof(RelacionActual));
        OnPropertyChanged(nameof(ContadorTexto));
        OnPropertyChanged(nameof(PuedeRetroceder));
        RetrocederCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(PuedeRetroceder))]
    private void Retroceder()
    {
        if (IndiceRelacion > 0)
        {
            IndiceRelacion--;
        }
    }

    [RelayCommand]
    private void Avanzar()
    {
        // El legacy crea una relación nueva al avanzar más allá de la última.
        if (IndiceRelacion + 1 >= Relaciones.Count)
        {
            Relaciones.Add(new FormatosViewModel());
            OnPropertyChanged(nameof(ContadorTexto));
        }
        IndiceRelacion++;
    }

    [RelayCommand]
    private void EliminarActual()
    {
        if (Relaciones.Count == 0)
        {
            return;
        }

        Relaciones.RemoveAt(IndiceRelacion);
        if (Relaciones.Count == 0)
        {
            Relaciones.Add(new FormatosViewModel());
        }

        if (IndiceRelacion >= Relaciones.Count)
        {
            IndiceRelacion = Relaciones.Count - 1;
        }

        OnPropertyChanged(nameof(RelacionActual));
        OnPropertyChanged(nameof(ContadorTexto));
        OnPropertyChanged(nameof(PuedeRetroceder));
        RetrocederCommand.NotifyCanExecuteChanged();
    }
}
