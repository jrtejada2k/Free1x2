// Free1X2 · WinUI 3 — WIN3
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Una apuesta (fila de un partido) dentro de una columna del boleto. Replica el
/// WinForms <c>Free1X2.UI.Controls.Boleto.ControlApuestaBoleto</c>: tres ranuras
/// (1 / X / 2) de las cuales sólo se marca la pronosticada. Cuando la apuesta está
/// oculta (partido inexistente para esa longitud de columna) toda la fila se atenúa,
/// igual que <c>OcultarApuesta</c> ponía el control invisible.
/// </summary>
public partial class ApuestaBoletoViewModel : ObservableObject
{
    /// <summary>Número de partido (1-based), sólo informativo.</summary>
    public int Partido { get; }

    public ApuestaBoletoViewModel(int partido)
    {
        Partido = partido;
        // Legacy ControlColumnaBoleto: añadía 1px de separación tras los partidos 4, 8, 11 y 14
        // (índices 3, 7, 10, 13) para el agrupamiento visual clásico de la quiniela.
        bool gap = partido is 4 or 8 or 11 or 14;
        MargenInferior = gap ? new Thickness(0, 0, 0, 3) : new Thickness(0);
    }

    /// <summary>Separación inferior de la fila para reproducir el agrupamiento legacy.</summary>
    public Thickness MargenInferior { get; }

    // Legacy ControlApuestaBoleto.Uno / Equis / Dos (Label.Visible).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Visibilidad1))]
    private bool _es1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VisibilidadX))]
    private bool _esX;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Visibilidad2))]
    private bool _es2;

    // Legacy: ControlApuestaBoleto.Visible (OcultarApuesta lo ponía a false para los
    // partidos que sobran cuando la columna tiene menos de 16 signos).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VisibilidadFila))]
    private bool _filaVisible = true;

    public Visibility Visibilidad1 => Es1 ? Visibility.Visible : Visibility.Collapsed;
    public Visibility VisibilidadX => EsX ? Visibility.Visible : Visibility.Collapsed;
    public Visibility Visibilidad2 => Es2 ? Visibility.Visible : Visibility.Collapsed;
    public Visibility VisibilidadFila => FilaVisible ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Restablece la fila a su estado vacío y visible.</summary>
    public void Limpiar()
    {
        Es1 = false;
        EsX = false;
        Es2 = false;
        FilaVisible = true;
    }
}

/// <summary>
/// Una columna del boleto (legacy <c>ControlColumnaBoleto</c>): cabecera con el número
/// de columna en la combinación, 16 apuestas (filas 1/X/2) y un pie con los aciertos.
/// </summary>
public partial class ColumnaBoletoViewModel : ObservableObject
{
    /// <summary>Número fijo de apuestas que pinta una columna (legacy: ControlColumnaBoleto(16)).</summary>
    public const int NumApuestas = 16;

    public ColumnaBoletoViewModel()
    {
        for (int i = 0; i < NumApuestas; i++)
        {
            Apuestas.Add(new ApuestaBoletoViewModel(i + 1));
        }
    }

    /// <summary>Las 16 apuestas de la columna (filas de partido).</summary>
    public ObservableCollection<ApuestaBoletoViewModel> Apuestas { get; } = new();

    // Legacy ControlColumnaBoleto.NumColumna (Label 'numero'): nº de columna dentro de la
    // combinación; vacío cuando vale 0 (columna inexistente en un boleto incompleto).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NumeroTexto))]
    private int _numero;

    public string NumeroTexto => Numero > 0 ? Numero.ToString() : "";

    // Legacy ControlColumnaBoleto.Aciertos (Label 'aciertos'): vacío si <=0; resaltado si >=10.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AciertosTexto))]
    [NotifyPropertyChangedFor(nameof(AciertosResaltados))]
    private int _aciertos;

    public string AciertosTexto => Aciertos > 0 ? Aciertos.ToString() : "";

    /// <summary>True cuando los aciertos llegan al pleno-zona resaltada (legacy: ForeColor azul si &gt;=10).</summary>
    public bool AciertosResaltados => Aciertos >= 10;

    /// <summary>
    /// Vuelca una columna de signos (p. ej. "1X2112...") sobre las apuestas, marcando
    /// la ranura correspondiente. Las apuestas que sobran (longitud &lt; 16) se ocultan,
    /// igual que <c>ControlBoleto.LlenarColumna</c> + <c>OcultarApuesta</c>.
    /// </summary>
    public void Llenar(string signos, int numColumna, int aciertos)
    {
        // Legacy LimpiarColumna antes de rellenar.
        foreach (var ap in Apuestas)
        {
            ap.Limpiar();
        }

        signos ??= "";
        int noPartidos = signos.Length;

        for (int j = 0; j < NumApuestas; j++)
        {
            var ap = Apuestas[j];
            if (j < noPartidos)
            {
                // Legacy ControlColumnaBoleto.LlenarApuesta(j+1, signo).
                char signo = signos[j];
                ap.Es1 = signo == '1';
                ap.EsX = signo == 'X' || signo == 'x';
                ap.Es2 = signo == '2';
                ap.FilaVisible = true;
            }
            else
            {
                // Legacy OcultarApuesta: partidos que no existen en esta columna.
                ap.FilaVisible = false;
            }
        }

        Numero = numColumna;
        Aciertos = aciertos;
    }

    /// <summary>Limpia la columna completa (legacy <c>ControlBoleto.LimpiarColumna</c>).</summary>
    public void Limpiar()
    {
        foreach (var ap in Apuestas)
        {
            ap.Limpiar();
        }
        Numero = 0;
        Aciertos = 0;
    }
}

/// <summary>
/// ViewModel del boleto visual completo: las 8 columnas que el WinForms
/// <c>ControlBoleto</c> pintaba en paralelo (legacy: 8 ControlColumnaBoleto en una fila).
/// Es el modelo que consume <see cref="BoletoMatrizControl"/>.
/// </summary>
public partial class BoletoMatrizViewModel : ObservableObject
{
    /// <summary>Número de columnas que muestra un boleto (legacy: bucle 1..8 de PrepararBoleto).</summary>
    public const int NumColumnas = 8;

    public BoletoMatrizViewModel()
    {
        for (int i = 0; i < NumColumnas; i++)
        {
            Columnas.Add(new ColumnaBoletoViewModel());
        }
    }

    /// <summary>Las 8 columnas del boleto.</summary>
    public ObservableCollection<ColumnaBoletoViewModel> Columnas { get; } = new();

    /// <summary>
    /// Rellena el boleto a partir de las 8 cadenas de signos del boleto actual, sus
    /// números de columna en la combinación y, opcionalmente, sus aciertos. Replica
    /// <c>ControlBoleto.LlenarBoleto</c> (Free1X2/UI/Controls/ControlBoleto.cs líneas 129-146).
    /// </summary>
    /// <param name="signos">Hasta 8 cadenas de signos; cadena vacía = columna inexistente.</param>
    /// <param name="numerosColumna">Nº de columna en la combinación para cada posición (0 = oculto).</param>
    /// <param name="aciertos">Aciertos por columna (0 en el visor de boletos del legacy).</param>
    public void Llenar(
        System.Collections.Generic.IReadOnlyList<string> signos,
        System.Collections.Generic.IReadOnlyList<int>? numerosColumna = null,
        System.Collections.Generic.IReadOnlyList<int>? aciertos = null)
    {
        for (int i = 0; i < NumColumnas; i++)
        {
            string s = i < signos.Count ? signos[i] ?? "" : "";
            int num = numerosColumna is not null && i < numerosColumna.Count ? numerosColumna[i] : 0;
            int ac = aciertos is not null && i < aciertos.Count ? aciertos[i] : 0;

            // Legacy: LlenarColumna(i+1, "", 0, 0) cuando la columna no existe en este boleto.
            Columnas[i].Llenar(s, string.IsNullOrEmpty(s) ? 0 : num, ac);
        }
    }

    /// <summary>Limpia todas las columnas (legacy <c>ControlBoleto.LimpiarBoleto</c>).</summary>
    public void Limpiar()
    {
        foreach (var c in Columnas)
        {
            c.Limpiar();
        }
    }
}
