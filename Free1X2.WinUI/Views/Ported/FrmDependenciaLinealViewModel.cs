using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de un partido en la tabla de Dependencia Lineal (legacy: labels lblNewBaseNN1/X/2
/// para el pronóstico y LblCoefNN para el coeficiente).
/// Cada partido aporta un signo (0/1/2) ponderado por su coeficiente al cálculo lineal
/// del partido a tratar.
/// </summary>
public partial class DependenciaPartidoViewModel : ObservableObject
{
    public DependenciaPartidoViewModel(int indice)
    {
        Indice = indice;
    }

    // Índice 0..13 del partido (legacy: Tag del label).
    public int Indice { get; }

    // Etiqueta visible "P1".."P14" (no se bindea int directo a Text -> regla anti-crash 2).
    public string Etiqueta => "P" + (Indice + 1).ToString();

    // Indica si este partido es el "Partido a tratar" (legacy: PartidoATratar).
    // Cuando es true se ignora su coeficiente y se recalcula su signo a partir de los demás.
    [ObservableProperty]
    private bool _esPartidoATratar;

    // Coeficiente del partido (legacy: Coef[Partido], cíclico 0/1/2). Es double porque
    // NumberBox.Value es double (regla anti-crash 7); el dominio lo trunca a byte 0..2.
    [ObservableProperty]
    private double _coeficiente;

    // Pronóstico del partido a tratar: signos 1/X/2 activos (legacy: Pronosticos[Partido,0..2]).
    [ObservableProperty]
    private bool _signo1;

    [ObservableProperty]
    private bool _signoX;

    [ObservableProperty]
    private bool _signo2;
}

/// <summary>
/// ViewModel de la pantalla "Dependencia Lineal" (legacy: FrmDependenciaLineal).
/// Recalcula el signo del "partido a tratar" como combinación lineal (mód. 3 ó 2)
/// de los signos del resto de partidos, ponderados por sus coeficientes, y reescribe
/// el archivo de columnas resultante.
///
/// Cableado al motor real: lectura/escritura de columnas con
/// <see cref="ArchivoColumnasTexto"/> + <see cref="ConvertidorDeBases"/> (Free1X2.Domain),
/// más FileOpenPicker/FileSavePicker (WinUI). El cálculo recorre las 4.782.969 combinaciones
/// posibles de 14 partidos (3^14) en un hilo de fondo (Task.Run) y refresca el estado en el
/// hilo de UI con el DispatcherQueue.
/// </summary>
public partial class FrmDependenciaLinealViewModel : ObservableObject
{
    // 3^14 = 4.782.969 combinaciones posibles de 14 partidos (legacy: tamaño de los BitArray).
    private const int TotalCombinaciones = 4782969;

    // Potencias de 3 por partido (legacy: int[] pot).
    private static readonly int[] Pot =
        { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323 };

    // Combinaciones presentes en el fichero de entrada (legacy: BitArray Bits).
    private BitArray _bits = new(TotalCombinaciones, false);

    public FrmDependenciaLinealViewModel()
    {
        var partidos = new ObservableCollection<DependenciaPartidoViewModel>();
        for (int i = 0; i < 14; i++)
        {
            partidos.Add(new DependenciaPartidoViewModel(i));
        }
        Partidos = partidos;
    }

    // 14 partidos de la quiniela (legacy: arrays Coef[15] / Pronosticos[15,3] / labels).
    public ObservableCollection<DependenciaPartidoViewModel> Partidos { get; }

    /// <summary>Acción de cierre/volver (la cablea la página con Frame.GoBack()). Legacy: Close().</summary>
    public Action? Volver { get; set; }

    // Nombre del archivo de entrada mostrado (legacy: TxFicheroEntrada.Text).
    [ObservableProperty]
    private string _nombreFicheroEntrada = "(falta selección)";

    // Nombre del archivo de salida mostrado (legacy: TxFicheroSalida.Text).
    [ObservableProperty]
    private string _nombreFicheroSalida = "(falta selección)";

    // Mensaje de estado (legacy: statusBarPanel2.Text).
    [ObservableProperty]
    private string _mensajeEstado = "Falta seleccionar fichero de columnas";

    // Habilita el botón Aceptar tras leer un archivo válido (legacy: btAceptar.Enabled).
    [ObservableProperty]
    private bool _puedeAceptar;

    // Rutas completas seleccionadas (legacy: archivoEntrada / archivoSalida).
    private string _rutaEntrada = string.Empty;
    private string _rutaSalida = string.Empty;

    /// <summary>
    /// Selecciona el archivo de columnas de entrada y lo lee.
    /// Legacy: button1_Click -> OpenFileDialog (carpeta Columnas, *.txt) + LeerColumnas() + ProponerCoeficientes().
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarEntrada()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? archivo = await picker.PickSingleFileAsync();
        if (archivo is null)
        {
            return;
        }

        _rutaEntrada = archivo.Path;
        NombreFicheroEntrada = archivo.Name;

        MensajeEstado = "Leyendo columnas...";
        double[,] porcentajes;
        try
        {
            porcentajes = await Task.Run(() => LeerColumnas(_rutaEntrada));
        }
        catch (Exception ex)
        {
            MensajeEstado = "Error al leer las columnas: " + ex.Message;
            AppServices.MostrarError("Error al leer las columnas: " + ex.Message);
            return;
        }

        // ProponerCoeficientes(): Coef=1 para los partidos con los 3 signos; el primero sin triple
        // pasa a ser PartidoATratar (Coef=0) y arranca con pronóstico 1X2.
        ProponerCoeficientes(porcentajes);

        if (_rutaSalida == "")
        {
            _rutaSalida = _rutaEntrada;
            NombreFicheroSalida = NombreFicheroEntrada;
        }

        MensajeEstado = "Seleccionar partido e indicar coeficientes";
        PuedeAceptar = true;
    }

    /// <summary>
    /// Selecciona el archivo de columnas de salida.
    /// Legacy: button2_Click -> SaveFileDialog (carpeta Columnas, *.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarSalida()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            DefaultFileExtension = ".txt",
            SuggestedFileName = "columnas",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        picker.FileTypeChoices.Add("Todos los archivos", new List<string> { "." });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? archivo = await picker.PickSaveFileAsync();
        if (archivo is null)
        {
            return;
        }

        _rutaSalida = archivo.Path;
        NombreFicheroSalida = archivo.Name;
    }

    /// <summary>
    /// Marca un partido como "partido a tratar" (exclusivo). Legacy: GenericLabelPronostico_Click /
    /// InicializarLabelsPronostico (sólo un partido puede tener pronóstico activo a la vez).
    /// </summary>
    [RelayCommand]
    private void MarcarPartidoATratar(DependenciaPartidoViewModel partido)
    {
        if (partido is null)
        {
            return;
        }

        foreach (var p in Partidos)
        {
            bool esElegido = ReferenceEquals(p, partido);
            p.EsPartidoATratar = esElegido;
            if (!esElegido)
            {
                // Legacy: Pronosticos[Parti,0..2] = false para el resto de partidos.
                p.Signo1 = false;
                p.SignoX = false;
                p.Signo2 = false;
            }
        }

        // Legacy: por defecto el partido a tratar arranca con los 3 signos (1X2).
        partido.Signo1 = true;
        partido.SignoX = true;
        partido.Signo2 = true;

        MensajeEstado = "Seleccionar partido e indicar coeficientes";
    }

    /// <summary>
    /// Ejecuta el cálculo de dependencia lineal y graba el archivo resultante.
    /// Legacy: btAceptar_Click -> (TestSignosPartidoATratar) + bucle sobre 4.782.969 combinaciones + GrabarColumnas().
    /// </summary>
    [RelayCommand]
    private async Task Aceptar()
    {
        // Localizar el partido a tratar (legacy: byte PartidoATratar).
        int partidoATratar = -1;
        for (int i = 0; i < Partidos.Count; i++)
        {
            if (Partidos[i].EsPartidoATratar) { partidoATratar = i; break; }
        }
        if (partidoATratar < 0)
        {
            MensajeEstado = "Seleccione el partido a tratar";
            return;
        }

        // TestSignosPartidoATratar(): determina (Modulo, TerminoCorrectorDobles, TerminoIndependiente)
        // según los signos activos del partido a tratar; falla si no es triple ni doble válido.
        var ata = Partidos[partidoATratar];
        if (!TestSignosPartidoATratar(ata.Signo1, ata.SignoX, ata.Signo2,
                out int modulo, out int terminoCorrectorDobles, out int terminoIndependiente))
        {
            MensajeEstado = "Debe poner un triple o un doble en el partido a tratar";
            return;
        }

        // Coeficientes truncados a byte 0..2 (legacy: byte[] Coef).
        var coef = new int[14];
        for (int i = 0; i < 14; i++)
        {
            int c = (int)Partidos[i].Coeficiente;
            if (c < 0) c = 0;
            if (c > 2) c = 2;
            coef[i] = c;
        }

        MensajeEstado = "Calculando columnas...";

        int columnasGrabadas;
        try
        {
            columnasGrabadas = await Task.Run(() =>
            {
                // Recorre las combinaciones presentes y recalcula el signo del partido a tratar.
                var bitsCambiados = new BitArray(TotalCombinaciones, false);
                for (int i = 0; i < TotalCombinaciones; i++)
                {
                    if (_bits[i])
                    {
                        int sigIni = (i / Pot[partidoATratar]) % 3;
                        int nuevoSigno = 0;
                        for (int partido = 0; partido < 14; partido++)
                        {
                            if (partido == partidoATratar) continue;
                            nuevoSigno += coef[partido] * ((i / Pot[partido]) % 3);
                        }
                        nuevoSigno %= modulo;
                        nuevoSigno *= terminoCorrectorDobles;
                        nuevoSigno += terminoIndependiente;
                        int indice = i + Pot[partidoATratar] * (nuevoSigno - sigIni);
                        bitsCambiados[indice] = true;
                    }
                }

                // GrabarColumnas(): vuelca las combinaciones recalculadas al fichero de salida.
                return GrabarColumnas(bitsCambiados, _rutaSalida);
            });
        }
        catch (Exception ex)
        {
            MensajeEstado = "Error al calcular/grabar las columnas: " + ex.Message;
            AppServices.MostrarError("Error al calcular/grabar las columnas: " + ex.Message);
            return;
        }

        MensajeEstado = "Se han grabado " + columnasGrabadas + " columnas";

        // Legacy GrabarColumnas() relee las columnas al terminar (recarga porcentajes).
        try
        {
            double[,] porcentajes = await Task.Run(() => LeerColumnas(_rutaSalida));
            _ = porcentajes; // los porcentajes recalculados se mostrarían en el control de % (no portado aquí)
        }
        catch
        {
            // Relectura no crítica: el fichero ya se grabó.
        }
    }

    /// <summary>
    /// Cierra/regresa sin ejecutar. Legacy: btCancelar -> Close().
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        Volver?.Invoke();
    }

    // ===== Lógica de dominio portada de FrmDependenciaLineal =====

    /// <summary>
    /// Legacy LeerColumnas(): lee el fichero de columnas, marca <see cref="_bits"/> con las
    /// combinaciones presentes y devuelve los porcentajes de signos por partido (double[14,3]).
    /// </summary>
    private double[,] LeerColumnas(string ruta)
    {
        IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(ruta);
        _bits = new BitArray(TotalCombinaciones, false);
        var porcentajes = new double[14, 3];

        var col = new ConvertidorDeBases();
        while (comBaseCols.SiguienteColumna())
        {
            string columna = comBaseCols.LeeColumnaSinComas();
            int num = col.ConvColumnaANumero(columna);
            _bits[num] = true;
            // ContarSignos(num): acumula el reparto de signos por partido.
            for (int partido = 0; partido < 14; partido++)
            {
                porcentajes[partido, (num / Pot[partido]) % 3]++;
            }
        }
        comBaseCols.Cerrar();

        // ConvertirAPorcentaje(): normaliza a base 100 (legacy ConvertirAPorcentaje).
        for (int i = 0; i < 14; i++)
        {
            double suma = porcentajes[i, 0] + porcentajes[i, 1] + porcentajes[i, 2];
            if (suma == 0) continue;
            porcentajes[i, 2] = Math.Round(porcentajes[i, 2] * 100 / suma, 0);
            porcentajes[i, 1] = Math.Round(porcentajes[i, 1] * 100 / suma, 0);
            porcentajes[i, 0] = 100 - porcentajes[i, 2] - porcentajes[i, 1];
        }

        return porcentajes;
    }

    /// <summary>
    /// Legacy ProponerCoeficientes(): propone Coef=1 a los partidos con los 3 signos presentes;
    /// el primero que no tenga triple pasa a ser el partido a tratar (Coef=0, pronóstico 1X2).
    /// </summary>
    private void ProponerCoeficientes(double[,] porcentajes)
    {
        int partidoATratar = -1;
        for (int i = 0; i < 14; i++)
        {
            DependenciaPartidoViewModel p = Partidos[i];
            if (porcentajes[i, 0] * porcentajes[i, 1] * porcentajes[i, 2] != 0)
            {
                p.Coeficiente = 1;
            }
            else
            {
                p.Coeficiente = 0;
                if (partidoATratar == -1) partidoATratar = i;
            }
        }

        // Reinicia banderas de pronóstico/partido a tratar.
        foreach (var p in Partidos)
        {
            p.EsPartidoATratar = false;
            p.Signo1 = false;
            p.SignoX = false;
            p.Signo2 = false;
        }

        if (partidoATratar == -1) partidoATratar = 0;
        var ata = Partidos[partidoATratar];
        ata.EsPartidoATratar = true;
        ata.Signo1 = true;
        ata.SignoX = true;
        ata.Signo2 = true;
    }

    /// <summary>
    /// Legacy TestSignosPartidoATratar(): fija módulo y términos del recálculo según el
    /// pronóstico del partido a tratar (1X2 / 1X / X2 / 12); devuelve false si no es válido.
    /// </summary>
    private static bool TestSignosPartidoATratar(bool s1, bool sX, bool s2,
        out int modulo, out int terminoCorrectorDobles, out int terminoIndependiente)
    {
        modulo = 3;
        terminoCorrectorDobles = 1;
        terminoIndependiente = 0;

        // 1X2 (triple)
        if (s1 && sX && s2)
        {
            modulo = 3; terminoCorrectorDobles = 1; terminoIndependiente = 0;
            return true;
        }
        // 1X
        if (s1 && sX && !s2)
        {
            modulo = 2; terminoCorrectorDobles = 1; terminoIndependiente = 0;
            return true;
        }
        // X2
        if (!s1 && sX && s2)
        {
            modulo = 2; terminoCorrectorDobles = 1; terminoIndependiente = 1;
            return true;
        }
        // 12
        if (s1 && !sX && s2)
        {
            modulo = 2; terminoCorrectorDobles = 2; terminoIndependiente = 0;
            return true;
        }
        return false;
    }

    /// <summary>Legacy GrabarColumnas(): graba al fichero de salida las combinaciones recalculadas.</summary>
    private static int GrabarColumnas(BitArray bitsCambiados, string rutaSalida)
    {
        var con = new ConvertidorDeBases();
        IArchivoColumnas cols = new ArchivoColumnasTexto(rutaSalida);
        int c = 0;
        for (int i = 0; i < TotalCombinaciones; i++)
        {
            if (bitsCambiados[i])
            {
                cols.GuardarCols(con.ConvNumAColumna(i));
                c++;
            }
        }
        cols.Cerrar();
        return c;
    }
}
