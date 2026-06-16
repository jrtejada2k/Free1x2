using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.WinUI.Services;

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
/// Cableado al motor real: la carga de columnas usa Free1X2.EntradaSalida.ArchivoColumnasTexto
/// y los algoritmos puros del form legacy (TraducirColumna/TransformarValoracion/
/// DeterminaApariciones) se replican aquí. La valoración procede del control 'valors'
/// (no portado); mientras tanto se usa una valoración neutra (mapeo identidad 1->1/X->2/2->3).
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
        // La lista arranca vacía; la cabecera muestra 0/0 hasta cargar un archivo.
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

    /// <summary>
    /// Equivalente a EntradaFichero(): abre un archivo de columnas, lo lee con
    /// Free1X2.EntradaSalida.ArchivoColumnasTexto y vuelca las columnas a la lista.
    /// </summary>
    [RelayCommand]
    private async Task LeerArchivoAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add(".cols");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        string ruta = file.Path;
        try
        {
            var leidas = await Task.Run(() =>
            {
                var lista = new List<string>();
                IArchivoColumnas ac = new ArchivoColumnasTexto(ruta);
                while (ac.SiguienteColumna())
                {
                    string columna = ac.LeeColumnaSinComas();
                    if (columna.Length > 16)
                    {
                        // legacy: "Error leyendo columnas" + abortar (lista vacía).
                        lista.Clear();
                        break;
                    }
                    lista.Add(columna.Substring(0, columna.Length).ToUpper());
                }
                ac.Cerrar();
                return lista;
            });

            Columnas.Clear();
            foreach (var c in leidas) Columnas.Add(c);
            RefrescarTrasCarga();
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error leyendo columnas: " + ex.Message);
        }
    }

    /// <summary>
    /// Equivalente a ObtenerFormatos(ColumnaActual): cuenta apariciones de cada formato
    /// predefinido (ArrayFormatos) sobre la columna traducida.
    /// </summary>
    [RelayCommand]
    private void Analizar()
    {
        // TODO: ArrayFormatos — ver Free1X2/UI/AnalisisFormatos123Frm.cs línea 97-104
        //   (ObtenerFormatos recorre this.ArrayFormatos, una lista de Formato123 inyectada
        //   externamente desde quien abría el form; nunca se establece en el propio form).
        //   Sin esa lista de formatos predefinidos el análisis no tiene entrada; usar
        //   "Mostrar todos" mientras no exista el origen de ArrayFormatos.
        if (Columnas.Count == 0) return;
        AppServices.MostrarInfo("Indica los formatos predefinidos a analizar o usa \"Mostrar todos\".");
    }

    /// <summary>
    /// Equivalente a ObtenerTodosFormatos(ColumnaActual): genera todas las subsecuencias
    /// posibles de la columna traducida, agrupa las iguales y cuenta apariciones.
    /// </summary>
    [RelayCommand]
    private void MostrarTodos()
    {
        if (Columnas.Count == 0) return;
        string columna1x2 = ColumnaActual;

        // Traducción de la columna usando la valoración (neutra mientras 'valors' no esté portado).
        // TODO: valoración real — ver Free1X2/UI/AnalisisFormatos123Frm.cs línea 110
        //   (TransformarValoracion(valors1.RetVals())). El control 'valors' (rejilla de
        //   valoraciones 1/X/2 por partido) aún no está portado a WinUI; con valoración neutra
        //   el mapeo es la identidad 1->"1", X->"2", 2->"3".
        string[,] val = TransformarValoracion(ValoracionNeutra(columna1x2.Length));
        string columnaFormato = TraducirColumna(columna1x2, val);

        var todos = new List<(string Formato, int AciertosMax)>();
        for (int longitud = 1; longitud <= columna1x2.Length; longitud++)
        {
            for (int posicion = 0; posicion < columna1x2.Length; posicion++)
            {
                if ((posicion + longitud) <= columna1x2.Length)
                {
                    todos.Add((columnaFormato.Substring(posicion, longitud), 0));
                }
                else
                {
                    break;
                }
            }
        }

        // Agrupar iguales y contar (legacy: recorrido O(n^2) con RemoveAt).
        for (int i = 0; i < todos.Count; i++)
        {
            int aciertos = 1;
            for (int j = i + 1; j < todos.Count; j++)
            {
                if (todos[j].Formato == todos[i].Formato)
                {
                    aciertos++;
                    todos.RemoveAt(j);
                    j--;
                }
            }
            todos[i] = (todos[i].Formato, aciertos);
        }

        Informe.Clear();
        foreach (var (formato, aciertosMax) in todos)
        {
            Informe.Add(new Formato123FilaViewModel(formato, aciertosMax));
        }
    }

    // ---- Algoritmos puros del form legacy (sin dependencia de UI) ----

    /// <summary>Traduce una columna 1/X/2 a notación de formato según la valoración (legacy TraducirColumna).</summary>
    private static string TraducirColumna(string columna, string[,] val)
    {
        string columnaAFormato = "";
        for (int i = 0; i < columna.Length; i++)
        {
            if (columna[i] == '1') columnaAFormato += val[i, 0];
            else if (columna[i] == 'X') columnaAFormato += val[i, 1];
            else columnaAFormato += val[i, 2];
        }
        return columnaAFormato;
    }

    /// <summary>Ordena la valoración de cada partido a posiciones 1/2/3 (legacy TransformarValoracion).</summary>
    private static string[,] TransformarValoracion(double[,] valoracion)
    {
        string[,] valoresTransformados = new string[valoracion.GetLength(0), 3];
        for (int i = 0; i < valoracion.GetLength(0); i++)
        {
            double[] valor = { valoracion[i, 0], valoracion[i, 1], valoracion[i, 2] };
            if ((valor[0] >= valor[1]) && (valor[0] >= valor[2]))
            {
                if (valor[1] >= valor[2]) { valoresTransformados[i, 0] = "1"; valoresTransformados[i, 1] = "2"; valoresTransformados[i, 2] = "3"; }
                else if (valor[2] > valor[1]) { valoresTransformados[i, 0] = "1"; valoresTransformados[i, 1] = "3"; valoresTransformados[i, 2] = "2"; }
            }
            else if ((valor[1] > valor[0]) && (valor[1] >= valor[2]))
            {
                if (valor[0] >= valor[2]) { valoresTransformados[i, 0] = "2"; valoresTransformados[i, 1] = "1"; valoresTransformados[i, 2] = "3"; }
                else { valoresTransformados[i, 0] = "3"; valoresTransformados[i, 1] = "1"; valoresTransformados[i, 2] = "2"; }
            }
            else if ((valor[2] > valor[0]) && (valor[2] > valor[1]))
            {
                if (valor[0] >= valor[1]) { valoresTransformados[i, 0] = "2"; valoresTransformados[i, 1] = "3"; valoresTransformados[i, 2] = "1"; }
                else { valoresTransformados[i, 0] = "3"; valoresTransformados[i, 1] = "2"; valoresTransformados[i, 2] = "1"; }
            }
        }
        return valoresTransformados;
    }

    /// <summary>Valoración neutra (todos los partidos con valores iguales 3>2>1 por orden 1/X/2).</summary>
    private static double[,] ValoracionNeutra(int partidos)
    {
        // Con valores iguales por partido y orden estable, TransformarValoracion devuelve
        // la posición natural 1/2/3 -> el signo se traduce a su posición (identidad).
        double[,] v = new double[partidos, 3];
        for (int i = 0; i < partidos; i++)
        {
            v[i, 0] = 3; // 1 -> posición 1
            v[i, 1] = 2; // X -> posición 2
            v[i, 2] = 1; // 2 -> posición 3
        }
        return v;
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
