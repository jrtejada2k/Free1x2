using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

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

        // TODO: impresión — Free1X2/UI/imprimirBoleto.cs Preparar() (línea 152) + Imprimir(PrintPageEventArgs) (línea 519).
        //   El dibujo de las marcas 1/X/2 sobre el boleto usa System.Drawing.Printing (PrintDocument +
        //   Graphics.FillRectangle/DrawString) y es específico de WinForms. En WinUI debe portarse con
        //   Windows.Graphics.Printing / PrintManager + un PrintDocument de la app.
        //   Datos ya disponibles para el render: _cols (columnas leídas), MargenSuperior/MargenIzquierdo,
        //   rango DesdeBoleto..HastaBoleto y GirarBoleto.
        AppServices.MostrarInfo("La impresión del boleto se portará con el sistema de impresión de WinUI (pendiente).");
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
        // TODO: selección de impresora — Free1X2/UI/imprimirBoleto.cs btnVerImpresoras_Click() (línea 564).
        //   El legacy abre el diálogo Free1X2.UI.ListaImpresoras(controlador) y, al cerrarse, copia
        //   controlador.MargenSuperior/MargenIzquierda/Modelo/Rotar a los campos del form.
        //   En WinUI esto requiere navegar a ListaImpresorasPage (ya portada) y recibir de vuelta el
        //   ControladorImpresion elegido (handoff de navegación), lo cual queda fuera del cableado de
        //   dominio de esta pantalla. La lista de impresoras soportadas se obtiene de
        //   Free1X2.MotorCalculo.ControladoresImpresion.ObtenListaImpresorasSoportadas().
        AppServices.MostrarInfo("La selección de impresora conocida se hará desde la lista de impresoras (pendiente de navegación).");
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
