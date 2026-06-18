// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Representación gráfica de la combinación" (legacy: GraficoColumnasFrm).
/// Abre un fichero de columnas (.txt), calcula los límites (mínimo/máximo) de la combinación
/// y la representa gráficamente en 10 franjas. La "Guía" muestra qué fila de la quiniela
/// corresponde a cada franja del gráfico.
///
/// Cableado al motor real (Free1X2.EntradaSalida.ArchivoColumnasTexto): se computan los datos
/// del gráfico (mínimo/máximo/escala, las coordenadas de cada línea por apuesta y los rectángulos
/// de relleno granate cuando la combinación es estrecha). El render GDI+ del legacy se sustituye por
/// WinUI Shapes sobre el Canvas en el code-behind, a partir de LineasGrafico y RectangulosRelleno.
/// </summary>
public partial class GraficoColumnasFrmViewModel : ObservableObject
{
    // Datos computados de la combinación (equivalen a los campos minimo/maximo del legacy).
    private int _minimo;
    private int _maximo;

    // Señal de cancelación del bucle de dibujo (legacy: bool para).
    private volatile bool _para;

    /// <summary>Ruta del fichero de columnas a representar (legacy: abreFicheroDialog.FileName).</summary>
    [ObservableProperty]
    private string _ficheroEntrada = string.Empty;

    /// <summary>Texto de estado / resumen tras abrir y representar la combinación.</summary>
    [ObservableProperty]
    private string _estadoTexto = "Selecciona un fichero de columnas y pulsa \"Abrir combinación\".";

    /// <summary>Valor mínimo de la combinación, como texto (legacy: campo 'minimo').</summary>
    [ObservableProperty]
    private string _minimoTexto = "—";

    /// <summary>Valor máximo de la combinación, como texto (legacy: campo 'maximo').</summary>
    [ObservableProperty]
    private string _maximoTexto = "—";

    /// <summary>Indica si hay una combinación ya representada (habilita la Guía).</summary>
    [ObservableProperty]
    private bool _hayCombinacion;

    /// <summary>
    /// Se solicita copiar/guardar la imagen del gráfico. Lo escucha el code-behind de la Page,
    /// que es quien tiene acceso al Canvas (UIElement) para hacer el RenderTargetBitmap. El VM
    /// no referencia controles de la vista (separación MVVM).
    /// </summary>
    public event EventHandler? CopiaImagenSolicitada;

    /// <summary>
    /// Líneas de la guía: para cada una de las 10 franjas, la fila de la quiniela representada
    /// (legacy: btnGuia_Click, deBase10 + reemplazos 1->X, 0->1).
    /// </summary>
    public ObservableCollection<string> LineasGuia { get; } = new();

    /// <summary>
    /// Coordenadas de las líneas verticales a dibujar (una por apuesta). Cada elemento es
    /// (X, Y0, Y1) en el mismo sistema del legacy (franjas de 25 px, 10 franjas).
    /// El renderizado real sobre el Canvas se hace en el code-behind.
    /// Equivale a las e.Graphics.DrawLine(myPen, ancho+10, alto*25+51, ancho+10, alto*25+74) del legacy.
    /// </summary>
    public ObservableCollection<(int X, int Y0, int Y1)> LineasGrafico { get; } = new();

    /// <summary>
    /// Rectángulos de relleno granate (legacy: Color.Maroon) que delimitan la zona sobrante cuando
    /// la combinación es estrecha (diferencia &lt; 9566). Cada elemento es (X, Y, W, H). Réplica de
    /// GraficoColumnasFrm_Paint (Free1X2/UI/GraficoColumnasFrm.cs líneas 268-273): un rectángulo
    /// parcial en la última franja útil + las franjas inferiores no usadas. El render real sobre el
    /// Canvas se hace en el code-behind.
    /// </summary>
    public ObservableCollection<(int X, int Y, int W, int H)> RectangulosRelleno { get; } = new();

    /// <summary>
    /// Abre el fichero de columnas y computa los datos de la representación gráfica.
    /// Equivale a GraficoColumnasFrm.btnAbrir_Click + la parte de cálculo de GraficoColumnasFrm_Paint.
    /// </summary>
    [RelayCommand]
    private async Task AbrirCombinacionAsync()
    {
        // Diálogo de fichero (legacy: OpenFileDialog "Columnas\\", filtro *.txt/*.*).
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        FicheroEntrada = file.Path;
        _para = false;
        EstadoTexto = "Procesando...";

        try
        {
            string ruta = FicheroEntrada;
            var (lineas, rellenos, minimo, maximo, apuestas) = await Task.Run(() => ComputarGrafico(ruta));

            _minimo = minimo;
            _maximo = maximo;
            MinimoTexto = minimo.ToString();
            MaximoTexto = maximo.ToString();

            LineasGrafico.Clear();
            foreach (var l in lineas) LineasGrafico.Add(l);

            RectangulosRelleno.Clear();
            foreach (var r in rellenos) RectangulosRelleno.Add(r);

            HayCombinacion = true;
            EstadoTexto = $"{apuestas} apuestas representadas (mín. {minimo}, máx. {maximo}).";
            // El render (marcos + líneas por apuesta + rectángulos de relleno granate) lo hace el
            // code-behind a partir de LineasGrafico y RectangulosRelleno, réplica de
            // GraficoColumnasFrm_Paint (Free1X2/UI/GraficoColumnasFrm.cs líneas 220-301).
        }
        catch (Exception ex)
        {
            HayCombinacion = false;
            EstadoTexto = "Error al abrir la combinación.";
            AppServices.MostrarError("No se ha podido representar la combinación: " + ex.Message);
        }
    }

    /// <summary>
    /// Computa las coordenadas de las líneas del gráfico y los límites de la combinación.
    /// Réplica exacta del cálculo de GraficoColumnasFrm_Paint (sin la parte de dibujo).
    /// </summary>
    private (System.Collections.Generic.List<(int X, int Y0, int Y1)> Lineas,
             System.Collections.Generic.List<(int X, int Y, int W, int H)> Rellenos,
             int Minimo, int Maximo, int Apuestas) ComputarGrafico(string ruta)
    {
        var lineas = new System.Collections.Generic.List<(int X, int Y0, int Y1)>();
        var rellenos = new System.Collections.Generic.List<(int X, int Y, int W, int H)>();

        IArchivoColumnas archComb = new ArchivoColumnasTexto(ruta);
        int[] matrizColumnas = archComb.LeerTodasColsANumero();
        int apuestas = matrizColumnas.Length;
        archComb.Cerrar();

        if (apuestas == 0) return (lineas, rellenos, 0, 0, 0);

        // Límites inferior y superior de la combinación.
        Array.Sort(matrizColumnas);
        int minimo = matrizColumnas[0];
        int maximo = matrizColumnas[apuestas - 1];
        int diferencia = maximo - minimo;

        double escala;
        if (diferencia < 9566)
        {
            // Combinación estrecha: hay menos columnas que líneas representables (960*10). El exceso
            // se pinta en granate para delimitar la zona. Réplica de las FillRectangle del legacy
            // (Free1X2/UI/GraficoColumnasFrm.cs líneas 262-273).
            escala = 1;
            int altoRelleno = Convert.ToInt16((diferencia * 10) / 9580);
            // Rectángulo parcial en la última franja útil: (maximo+1, altoRelleno*25+50, 958-maximo, 25).
            rellenos.Add((maximo + 1, (altoRelleno * 25) + 50, 958 - maximo, 25));
            // Franjas inferiores no usadas: (9, i*25+50, 958, 25) para i = altoRelleno+1 .. 9.
            for (int i = altoRelleno + 1; i < 10; i++)
            {
                rellenos.Add((9, (i * 25) + 50, 958, 25));
            }
        }
        else
        {
            escala = diferencia / 9565.938;
        }

        for (int i = 0; i < apuestas; i++)
        {
            if (_para) break;
            double num = (matrizColumnas[i] - minimo) / escala;
            // Dividimos en 10 barras 4782969/(500*957)=10.
            int alto = Convert.ToInt16((num / 958) - 0.5);
            num -= (alto * 958);
            int ancho = Convert.ToInt16(num);
            // Línea vertical: (ancho+10, alto*25+51) -> (ancho+10, alto*25+74).
            lineas.Add((ancho + 10, (alto * 25) + 51, (alto * 25) + 74));
        }

        return (lineas, rellenos, minimo, maximo, apuestas);
    }

    /// <summary>
    /// Detiene la representación en curso (legacy: btnSalir_Click -> para = true).
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        _para = true; // legacy: corta el bucle de pintado de líneas.
    }

    /// <summary>
    /// Copia/guarda la imagen del gráfico. El legacy GraficoColumnasFrm no tenía esta acción;
    /// se añade como equivalente funcional WinUI (capturar el lienzo nativo a un mapa de bits y
    /// llevarlo al portapapeles / guardarlo como PNG), reutilizando el render ya pintado en el Canvas.
    /// El VM sólo dispara el evento; el RenderTargetBitmap (que necesita el UIElement) lo hace la Page.
    /// </summary>
    [RelayCommand]
    private void Copiar()
    {
        if (!HayCombinacion)
        {
            EstadoTexto = "No hay ninguna combinación representada que copiar.";
            return;
        }
        CopiaImagenSolicitada?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Muestra la guía: qué fila de la quiniela corresponde a cada una de las 10 franjas.
    /// Equivale a GraficoColumnasFrm.btnGuia_Click.
    /// </summary>
    [RelayCommand]
    private void MostrarGuia()
    {
        LineasGuia.Clear();
        int diferencia = _maximo - _minimo;
        for (int i = 1; i < 11; i++)
        {
            string tmp = DeBase10(((i - 1) * (diferencia / 10)) + _minimo, 3, 14);
            tmp = tmp.Replace("1", "X");
            tmp = tmp.Replace("0", "1");
            LineasGuia.Add(i.ToString("00") + ")  " + tmp);
        }
    }

    /// <summary>Conversión de base 10 a base 3 con longitud fija (legacy: GraficoColumnasFrm.deBase10).</summary>
    private static string DeBase10(int numero, int nBase, int longitud)
    {
        string num = "";
        int resto;
        int divisor = 0;

        if (numero < nBase)
        {
            resto = numero % nBase;
            num = resto.ToString();
        }
        if (numero == nBase)
        {
            divisor = numero / nBase;
        }
        while (numero >= nBase)
        {
            resto = numero % nBase;
            divisor = numero / nBase;
            num = resto + num;
            numero = divisor;
        }
        num = divisor + num;
        while (num.Length < longitud)
        {
            num = "0" + num;
        }
        return num;
    }
}
