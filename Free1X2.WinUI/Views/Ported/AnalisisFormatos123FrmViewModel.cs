using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila del informe de formatos: la secuencia de signos (en notación 1/2/3
/// según la valoración del partido) y cuántas veces aparece en la columna actual.
/// Equivale al control legacy Free1X2.UI.Controls.CtrlFormatos123Analisis
/// alimentado por Free1X2.MotorCalculo.Formato123.
/// </summary>
public partial class Formato123FilaViewModel : ObservableObject
{
    public Formato123FilaViewModel(string formato, int apariciones)
    {
        _formato = formato;
        _apariciones = apariciones;
    }

    [ObservableProperty]
    private string _formato = string.Empty;

    [ObservableProperty]
    private int _apariciones;

    /// <summary>Texto seguro para bindear en un TextBlock (regla anti-crash 2).</summary>
    public string AparicionesTexto => Apariciones.ToString();

    partial void OnAparicionesChanged(int value) => OnPropertyChanged(nameof(AparicionesTexto));
}

/// <summary>
/// ViewModel de la pantalla "Analizador Formatos123".
///
/// El "Formato123" traduce cada signo de una columna (1/X/2) a su posición en la
/// valoración del partido (1 = más probable, 2 = media, 3 = menos probable) y, sobre
/// esa traducción, cuenta cuántas veces aparece cada subsecuencia ("formato"). El
/// usuario carga un archivo de columnas, navega columna a columna y lanza el análisis
/// de los formatos predefinidos ("Analizar") o de todos los formatos posibles
/// ("Mostrar todos"). El resultado es un informe de formato + nº de apariciones.
///
/// Datos en memoria; la traducción, el conteo y la persistencia siguen en el dominio
/// legacy (ver los TODO en AnalisisFormatos123FrmPage.xaml.cs).
/// </summary>
public partial class AnalisisFormatos123FrmViewModel : ObservableObject
{
    /// <summary>Columnas leídas del archivo (legacy arrayColumnas).</summary>
    public ObservableCollection<string> Columnas { get; } = new();

    /// <summary>Filas del informe de formatos (legacy contenido del ContainerControl cctrl).</summary>
    public ObservableCollection<Formato123FilaViewModel> Informe { get; } = new();

    /// <summary>Índice de la columna actualmente analizada (legacy noColumna).</summary>
    [ObservableProperty]
    private int _indiceColumna;

    public AnalisisFormatos123FrmViewModel()
    {
        // TODO (dominio): poblar Columnas desde un archivo .txt/.cols mediante
        //   Free1X2.EntradaSalida.ArchivoColumnasTexto (legacy EntradaFichero()).
        // Por ahora la lista arranca vacía; la cabecera muestra 0/0.
    }

    /// <summary>Columna actualmente seleccionada (legacy arrayColumnas[noColumna]).</summary>
    public string ColumnaActual =>
        IndiceColumna >= 0 && IndiceColumna < Columnas.Count
            ? Columnas[IndiceColumna]
            : string.Empty;

    /// <summary>Contador "N/Total" (legacy lblNumCol). String para TextBlock (regla 2).</summary>
    public string ContadorTexto =>
        Columnas.Count == 0 ? "0/0" : $"{IndiceColumna + 1}/{Columnas.Count}";

    public bool HayColumnas => Columnas.Count > 0;
    public bool PuedeRetroceder => IndiceColumna > 0;
    public bool PuedeAvanzar => IndiceColumna < Columnas.Count - 1;

    partial void OnIndiceColumnaChanged(int value)
    {
        OnPropertyChanged(nameof(ColumnaActual));
        OnPropertyChanged(nameof(ContadorTexto));
        OnPropertyChanged(nameof(PuedeRetroceder));
        OnPropertyChanged(nameof(PuedeAvanzar));
        RetrocederCommand.NotifyCanExecuteChanged();
        AvanzarCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(PuedeRetroceder))]
    private void Retroceder()
    {
        // legacy DecrementarContador()
        if (IndiceColumna > 0)
        {
            IndiceColumna--;
        }
    }

    [RelayCommand(CanExecute = nameof(PuedeAvanzar))]
    private void Avanzar()
    {
        // legacy IncrementarContador()
        if (IndiceColumna < Columnas.Count - 1)
        {
            IndiceColumna++;
        }
    }

    [RelayCommand]
    private void LeerArchivo()
    {
        // TODO (dominio): equivalente de EntradaFichero() — abrir un FileOpenPicker
        // (filtros *.txt / *.cols), leer con Free1X2.EntradaSalida.ArchivoColumnasTexto
        // (SiguienteColumna / LeeColumnaSinComas), validar longitud <= 16, y volcar a
        // Columnas. Después refrescar el contador.
        // RefrescarTrasCarga(); // llamar tras poblar Columnas
    }

    [RelayCommand]
    private void Analizar()
    {
        // TODO (dominio): equivalente de ObtenerFormatos(ColumnaActual) —
        // 1) TransformarValoracion(valoración del control valors) -> matriz de rangos 1/2/3,
        // 2) TraducirColumna(ColumnaActual, ...) -> cadena en notación de formato,
        // 3) por cada Free1X2.MotorCalculo.Formato123 predefinido, contar apariciones con
        //    DeterminaApariciones(...) y poblar Informe con Formato123FilaViewModel.
    }

    [RelayCommand]
    private void MostrarTodos()
    {
        // TODO (dominio): equivalente de ObtenerTodosFormatos(ColumnaActual) — generar
        // todas las subsecuencias posibles de la columna traducida, agrupar iguales,
        // contar apariciones y volcar el resultado a Informe.
    }

    /// <summary>Helper para refrescar la cabecera tras cargar columnas (legacy MostrarContador()).</summary>
    public void RefrescarTrasCarga()
    {
        IndiceColumna = 0;
        OnPropertyChanged(nameof(HayColumnas));
        OnPropertyChanged(nameof(ColumnaActual));
        OnPropertyChanged(nameof(ContadorTexto));
        OnPropertyChanged(nameof(PuedeRetroceder));
        OnPropertyChanged(nameof(PuedeAvanzar));
        RetrocederCommand.NotifyCanExecuteChanged();
        AvanzarCommand.NotifyCanExecuteChanged();
    }
}
