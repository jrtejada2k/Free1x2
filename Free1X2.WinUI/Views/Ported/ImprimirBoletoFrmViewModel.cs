using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Descriptor de un boleto a representar: encabezado (fichero + nº de boleto) y la lista de
/// marcas (rectángulos 1/X/2 + pleno) en coordenadas relativas al origen del boleto. La Page lo
/// dibuja sobre un Canvas. Réplica de lo que el legacy pintaba por página en Imprimir(PrintPageEventArgs).
/// </summary>
public sealed class BoletoRender
{
    public BoletoRender(string encabezado, IReadOnlyList<(double X, double Y, double W, double H)> marcas)
    {
        Encabezado = encabezado;
        Marcas = marcas;
    }

    public string Encabezado { get; }
    public IReadOnlyList<(double X, double Y, double W, double H)> Marcas { get; }
}

// ViewModel para ImprimirBoletoFrmPage.
// Replica los inputs del WinForms legacy Free1X2.UI.ImprimirBoletoFrm (UI/imprimirBoleto.cs):
//   - tbmgsup / tbmgizq : margenes de impresion (superior / izquierdo)
//   - tbminbol / tbmaxbol: rango de boletos a imprimir (desde / hasta)
//   - lfile / lcols      : fichero de columnas leido y numero de columnas
//   - lblImpresora       : modelo de impresora seleccionada
//   - controlador.Rotar  : girar el boleto al imprimir
public partial class ImprimirBoletoFrmViewModel : ObservableObject
{
    // Configuracion de impresion (legacy: ControladorImpresion controlador).
    private readonly ControladorImpresion _controlador = new();

    // Columnas leidas del fichero (legacy: List<string> cols).
    private readonly List<string> _cols = new();

    // --- Posicion de impresion (margenes en puntos) ---
    [ObservableProperty]
    private double _margenSuperior = 100;

    [ObservableProperty]
    private double _margenIzquierdo = 30;

    // --- Rango de boletos ---
    [ObservableProperty]
    private double _desdeBoleto = 1;

    [ObservableProperty]
    private double _hastaBoleto = 1;

    // --- Girar boleto (legacy controlador.Rotar) ---
    [ObservableProperty]
    private bool _girarBoleto;

    // --- Estado del fichero de columnas leido ---
    [ObservableProperty]
    private string _ficheroEntrada = "-";

    [ObservableProperty]
    private int _numeroColumnas;

    // TextBlock no debe bindear int directo (regla anti-crash #2):
    // se expone como string.
    public string NumeroColumnasTexto => NumeroColumnas.ToString();

    partial void OnNumeroColumnasChanged(int value) => OnPropertyChanged(nameof(NumeroColumnasTexto));

    // --- Impresora seleccionada ---
    [ObservableProperty]
    private string _impresora = "(ninguna)";

    /// <summary>
    /// Boletos ya computados, listos para que la Page los dibuje sobre el Canvas (réplica del
    /// dibujo por página del legacy Imprimir(PrintPageEventArgs)). Se rellena al pulsar "Imprimir".
    /// </summary>
    public ObservableCollection<BoletoRender> BoletosARenderizar { get; } = new();

    /// <summary>
    /// Se solicita exportar/guardar como imagen el boleto ya computado. Lo escucha el code-behind
    /// de la Page, que dibuja BoletosARenderizar en un Canvas, lo captura con RenderTargetBitmap y
    /// ofrece guardarlo como PNG / copiarlo al portapapeles. El VM no referencia controles (MVVM).
    /// </summary>
    public event EventHandler? ExportacionSolicitada;

    /// <summary>
    /// Se solicita elegir una impresora conocida. Lo escucha el code-behind de la Page, que muestra
    /// un selector (ContentDialog) con los modelos soportados y llama a <see cref="AplicarImpresora"/>
    /// con el elegido. Réplica del btnVerImpresoras_Click del legacy (que abría el diálogo
    /// ListaImpresoras y copiaba la config del controlador a los campos del form).
    /// </summary>
    public event EventHandler? SeleccionImpresoraSolicitada;

    /// <summary>
    /// Aplica la configuración de una impresora conocida (modelo + margenes + girar) a los campos
    /// del formulario. Equivale a la copia que hacía btnVerImpresoras_Click tras cerrar el diálogo
    /// (controlador.MargenSuperior/MargenIzquierda/Modelo/Rotar -> tbmgsup/tbmgizq/lblImpresora/picture).
    /// </summary>
    public void AplicarImpresora(string modelo, int margenSuperior, int margenIzquierdo, bool girar)
    {
        Impresora = modelo;
        MargenSuperior = margenSuperior;
        MargenIzquierdo = margenIzquierdo;
        GirarBoleto = girar;
        _controlador.Rotar = girar;
    }

    // Ruta del fichero de configuracion (legacy: StartupPath + "/Impresion/imprebol.cfg").
    private static string RutaConfig =>
        Path.Combine(
            AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
            "Impresion", "imprebol.cfg");

    public ImprimirBoletoFrmViewModel()
    {
        // Legacy ctor: if (!GetConfig()) SetConfig(); -> intenta leer y, si no, deja los defaults.
        RecuperarConfiguracionInterno();
    }

    // ---------------------------------------------------------------------
    // Acciones (botones del form legacy).
    // ---------------------------------------------------------------------

    [RelayCommand]
    private async Task LeerColumnasAsync()
    {
        // Legacy LeerCols(): OpenFileDialog filtrando *.txt en /Columnas/.
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        FicheroEntrada = "-";
        NumeroColumnas = 0;
        string ruta = file.Path;

        try
        {
            // Lee todas las columnas sin comas (legacy: while(ac.SiguienteColumna()) cols.Add(ac.LeeColumnaSinComas())).
            var columnas = await Task.Run(() =>
            {
                var lista = new List<string>();
                IArchivoColumnas ac = new ArchivoColumnasTexto(ruta);
                try
                {
                    while (ac.SiguienteColumna())
                    {
                        lista.Add(ac.LeeColumnaSinComas());
                    }
                }
                finally
                {
                    ac.Cerrar();
                }
                return lista;
            });

            _cols.Clear();
            _cols.AddRange(columnas);

            FicheroEntrada = Path.GetFileName(ruta);
            NumeroColumnas = _cols.Count;

            // Legacy: boletos = (cols.Count - 1) / 8 + 1; tbminbol="1"; tbmaxbol=boletos.
            int boletos = (_cols.Count - 1) / 8 + 1;
            if (boletos < 1) boletos = 1;
            DesdeBoleto = 1;
            HastaBoleto = boletos;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido leer el fichero de columnas: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Imprimir()
    {
        // Validacion (legacy Preparar(): if ((cols.Count > 0) && (cols[0] != null)) ... else error).
        if (_cols.Count == 0 || _cols[0] == null)
        {
            AppServices.MostrarError("No se ha seleccionado el archivo");
            return;
        }

        // Equivalente funcional WinUI de Preparar() + Imprimir(PrintPageEventArgs)
        // (Free1X2/UI/imprimirBoleto.cs líneas 152 y 519): el legacy usaba System.Drawing.Printing
        // (PrintDocument + Graphics.FillRectangle/DrawString). Aquí se computan las MISMAS marcas
        // 1/X/2 (con idéntica geometría) y la Page las dibuja sobre un Canvas, lo captura con
        // RenderTargetBitmap y ofrece guardarlo como imagen PNG / copiarlo al portapapeles.
        BoletosARenderizar.Clear();
        foreach (var boleto in ComputarBoletos())
        {
            BoletosARenderizar.Add(boleto);
        }

        if (BoletosARenderizar.Count == 0)
        {
            AppServices.MostrarError("No hay columnas en el rango de boletos seleccionado.");
            return;
        }

        ExportacionSolicitada?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Computa los boletos del rango seleccionado replicando la geometría exacta del legacy
    /// Imprimir(PrintPageEventArgs) (Free1X2/UI/imprimirBoleto.cs líneas 519-559): por cada boleto
    /// (hasta 8 columnas) se calcula, para cada apuesta y cada uno de los 14 partidos, la posición
    /// de la marca 1/X/2, más la marca del pleno al 15. Las marcas se dan en coordenadas relativas
    /// al origen del boleto (la Page apila los boletos verticalmente).
    /// </summary>
    private IEnumerable<BoletoRender> ComputarBoletos()
    {
        // Margenes (legacy: mgy = tbmgsup, mgx = tbmgizq).
        float mgy = (float)MargenSuperior;
        float mgx = (float)MargenIzquierdo;

        // Rango de boletos (legacy: minbol/maxbol; cada boleto = 8 columnas).
        int minbol = (int)DesdeBoleto; if (minbol < 1) minbol = 1;
        int maxbol = (int)HastaBoleto; if (maxbol < minbol) maxbol = minbol;
        int maxcol = maxbol * 8; if (maxcol > _cols.Count) maxcol = _cols.Count;
        int mincol = (minbol - 1) * 8; if (mincol == maxcol) mincol--;
        if (mincol < 0) mincol = 0;

        int nwcol = mincol;
        int actubol = minbol;

        while (nwcol < maxcol)
        {
            var marcas = new List<(double X, double Y, double W, double H)>();

            // Pleno al 15 del primer column del boleto (legacy: p15 = columna[14] si len>14 si no '1').
            string primera = _cols[nwcol];
            char p15 = primera.Length > 14 ? primera[14] : '1';

            // 8 columnas por boleto (legacy: for nw8 0..7).
            for (int nw8 = 0; nw8 < 8; nw8++)
            {
                string columna = _cols[nwcol++];
                int partidos = Math.Min(14, columna.Length);
                for (int pa = 0; pa < partidos; pa++)
                {
                    char ch = columna[pa];
                    double cx = mgx - 29 + (14 - pa) * 19.685;
                    double inc = ch == '1' ? 0 : (ch == 'X' ? 14.764 : 29.528);
                    double cy = mgy + 83 + nw8 * 44.291 + inc;
                    marcas.Add((cx, cy, 6, 4));
                }

                if (nwcol == _cols.Count) break;
                if (nwcol + 2 == _cols.Count && nw8 == 6) break;
            }

            // Marca del pleno al 15 (legacy: cx = mgx-30; cy según p15).
            double cxp = mgx - 30;
            double cyp = p15 == '1' ? mgy + 245 : (p15 == '2' ? mgy + 305 : mgy + 275);
            marcas.Add((cxp, cyp, 6, 4));

            string encabezado = $"{FicheroEntrada}   Boleto {actubol}";
            yield return new BoletoRender(encabezado, marcas);
            actubol++;
        }
    }

    [RelayCommand]
    private void GuardarConfiguracion()
    {
        // Legacy SetConfig(): escribe 4 líneas en /Impresion/imprebol.cfg.
        try
        {
            _controlador.Rotar = GirarBoleto;
            string dir = Path.GetDirectoryName(RutaConfig)!;
            Directory.CreateDirectory(dir);
            using var sw = new StreamWriter(RutaConfig);
            sw.WriteLine("margen superior=" + ((int)MargenSuperior));
            sw.WriteLine("margen izquierdo=" + ((int)MargenIzquierdo));
            sw.WriteLine("modelo=" + Impresora);
            sw.WriteLine("girar=" + (GirarBoleto ? "si" : "no"));
            AppServices.MostrarInfo("Configuración guardada");
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido guardar la configuración: " + ex.Message);
        }
    }

    [RelayCommand]
    private void RecuperarConfiguracion()
    {
        // Legacy GetConfig(): lee 4 líneas de /Impresion/imprebol.cfg.
        if (!RecuperarConfiguracionInterno())
        {
            AppServices.MostrarError("No se ha podido leer la configuración guardada");
        }
    }

    /// <summary>
    /// Lee /Impresion/imprebol.cfg y puebla MargenSuperior, MargenIzquierdo, Impresora y GirarBoleto.
    /// Réplica de GetConfig() del legacy. Devuelve false si no se pudo leer.
    /// </summary>
    private bool RecuperarConfiguracionInterno()
    {
        try
        {
            if (!File.Exists(RutaConfig)) return false;
            string[] lineas = File.ReadAllLines(RutaConfig);
            if (lineas.Length < 4) return false;

            // Cada línea tiene formato "clave=valor"; el legacy toma aux[1].
            MargenSuperior = ParsearValorDouble(lineas[0], MargenSuperior);
            MargenIzquierdo = ParsearValorDouble(lineas[1], MargenIzquierdo);
            Impresora = ParsearValor(lineas[2], Impresora);
            string girar = ParsearValor(lineas[3], "no");
            GirarBoleto = girar == "si";
            _controlador.Rotar = GirarBoleto;
            return true;
        }
        catch
        {
            return false;
        }
    }

    [RelayCommand]
    private void ImpresorasConocidas()
    {
        // Equivalente WinUI de btnVerImpresoras_Click (Free1X2/UI/imprimirBoleto.cs línea 564): el
        // legacy abría ListaImpresoras(controlador) y, al cerrar, copiaba la config del controlador
        // a los campos del form. Aquí la Page muestra un selector (ContentDialog) con los modelos
        // soportados (Free1X2.MotorCalculo.ControladoresImpresion) y, al elegir, llama a
        // AplicarImpresora(...) para volcar margenes/modelo/girar.
        SeleccionImpresoraSolicitada?.Invoke(this, EventArgs.Empty);
    }

    // Extrae el valor (lo que sigue al primer '=') de una línea "clave=valor".
    private static string ParsearValor(string linea, string porDefecto)
    {
        int i = linea.IndexOf('=');
        return i >= 0 && i + 1 <= linea.Length ? linea.Substring(i + 1) : porDefecto;
    }

    private static double ParsearValorDouble(string linea, double porDefecto)
    {
        string v = ParsearValor(linea, string.Empty);
        return double.TryParse(v, out double d) ? d : porDefecto;
    }
}
