// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para HistoriaValoracionesFrmPage.
/// Porta el WinForms legacy "HistoriaValoracionesFrm" (UI/HistoriaValoracionesFrm.cs).
/// Proposito: guardar las valoraciones 1X2 (double[14,3]) de una jornada/temporada
/// en un fichero historico de valoraciones.
///
/// En el WinForms la matriz llegaba por constructor (desde el Escrutinio). En WinUI se edita
/// en una rejilla <see cref="PorcentajesControl"/> (<see cref="Filas"/>). La grabación replica
/// fielmente Free1X2.Utils.Porcentajes.GuardarValoraciones(... pTemporada, pJornada) — clase del
/// proyecto WinForms no portada al dominio; aquí se transcribe su escritura/anexado de líneas
/// históricas (formato 44) usando <see cref="ArchivoColumnasTexto"/> del dominio, sin inventar lógica.
/// </summary>
public partial class HistoriaValoracionesFrmViewModel : ObservableObject
{
    // Temporada de inicio (legacy: int Temporada, txTemporada).
    [ObservableProperty]
    private double _temporada = 2004;

    // Jornada a guardar (legacy: int Jornada, txJornada). Rango legacy: 0..43.
    [ObservableProperty]
    private double _jornada = 1;

    // Nombre / ruta del fichero historico de salida (legacy: archivoSalida / txNombreFichero).
    [ObservableProperty]
    private string _nombreFichero = string.Empty;

    // Habilita el boton Guardar solo cuando hay fichero seleccionado (legacy: btGuardar.Enabled).
    [ObservableProperty]
    private bool _puedeGuardar;

    // Texto de la temporada siguiente (legacy: txTemporada2, deshabilitado, = Temporada+1).
    // Regla anti-crash #2: nunca bindear int/double directo a TextBlock.Text.
    public string TemporadaSiguienteTexto => ((int)Temporada + 1).ToString();

    // Ruta completa del fichero histórico (legacy: archivoSalida).
    private string _rutaSalida = string.Empty;

    /// <summary>
    /// Matriz editable de valoraciones (14 partidos x 1/X/2) que alimenta la
    /// <see cref="PorcentajesControl"/>. En el WinForms llegaba por constructor (Escrutinio).
    /// </summary>
    public ObservableCollection<FilaPorcentaje> Filas { get; } =
        PorcentajesHelper.Crear(VariablesGlobales.NumeroPartidos);

    // Mensaje de estado / feedback (sustituye los MessageBox).
    [ObservableProperty]
    private string _estado = string.Empty;

    partial void OnTemporadaChanged(double value) => OnPropertyChanged(nameof(TemporadaSiguienteTexto));

    /// <summary>
    /// Carga el contexto que el WinForms recibía por constructor
    /// (legacy: new HistoriaValoracionesFrm(v, temporada, jornada, archivoHistorico) desde
    /// TramificarForm.AfegirAlHistoric). Vuelca la matriz de valoraciones a la rejilla y fija
    /// temporada/jornada y, si llega, el fichero histórico de partida.
    /// </summary>
    public void Inicializar(double[,] valores, int temporada, int jornada, string? archivoHistorico)
    {
        PorcentajesHelper.CargarMatriz(Filas, valores);
        Temporada = temporada;
        Jornada = jornada;

        if (!string.IsNullOrEmpty(archivoHistorico))
        {
            _rutaSalida = archivoHistorico!;
            NombreFichero = Path.GetFileName(archivoHistorico);
            PuedeGuardar = true;
        }
    }

    // --- Spinners de temporada (legacy: btTemporadaAnterior_Click / btTemporadaSiguiente_Click) ---

    [RelayCommand]
    private void TemporadaAnterior()
    {
        Temporada -= 1;
    }

    [RelayCommand]
    private void TemporadaSiguiente()
    {
        Temporada += 1;
    }

    // --- Spinners de jornada (legacy: btJornadaAnterior_Click / btJornadaSiguiente_Click, rango 0..43) ---

    [RelayCommand]
    private void JornadaAnterior()
    {
        if (Jornada > 0)
            Jornada -= 1;
    }

    [RelayCommand]
    private void JornadaSiguiente()
    {
        if (Jornada < 43)
            Jornada += 1;
    }

    // --- Seleccionar fichero existente (legacy: button1_Click, OpenFileDialog) ---
    [RelayCommand]
    private async Task SeleccionarFichero()
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

        _rutaSalida = archivo.Path;
        NombreFichero = archivo.Name;
        PuedeGuardar = true;
    }

    // --- Crear fichero nuevo (legacy: btNuevo_Click, SaveFileDialog) ---
    [RelayCommand]
    private async Task NuevoFichero()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            DefaultFileExtension = ".txt",
            SuggestedFileName = "valoraciones_historicas",
        };
        picker.FileTypeChoices.Add("Valoraciones históricas", new List<string> { ".txt" });
        picker.FileTypeChoices.Add("Todos los archivos", new List<string> { "." });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? archivo = await picker.PickSaveFileAsync();
        if (archivo is null)
        {
            return;
        }

        _rutaSalida = archivo.Path;
        NombreFichero = archivo.Name;
        PuedeGuardar = true;
    }

    // --- Guardar valoraciones en el historico (legacy: btGuardar_Click) ---
    [RelayCommand]
    private async Task Guardar()
    {
        if (_rutaSalida == "")
        {
            return;
        }

        // Legacy btGuardar_Click: tempo = Temporada + "/" + (Temporada+1); jorna = Jornada.PadLeft(2,'0').
        int temporada = (int)Temporada;
        string tempo = temporada + "/" + (temporada + 1);
        string jorna = ((int)Jornada).ToString().PadLeft(2, '0');

        double[,] valores = PorcentajesHelper.AMatriz(Filas);

        try
        {
            await Task.Run(() => GuardarValoracionesHistorico(_rutaSalida, (char)9, valores, tempo, jorna));
        }
        catch (Exception ex)
        {
            Estado = "Error al guardar en el histórico: " + ex.Message;
            AppServices.MostrarError("Error al guardar en el histórico: " + ex.Message);
            return;
        }

        Estado = $"Guardada la jornada {jorna} ({tempo}) en {NombreFichero}.";
        AppServices.MostrarInfo("Valoraciones guardadas en el histórico");
    }

    // --- Cancelar (legacy: button2_Click -> Close) ---
    [RelayCommand]
    private void Cancelar()
    {
        // Legacy button2_Click: Close(). En WinUI sólo se limpia el feedback (sin ventana propia).
        Estado = "Cancelado.";
    }

    // ===== Lógica de dominio portada de Free1X2.Utils.Porcentajes =====

    /// <summary>
    /// Réplica de Porcentajes.GuardarValoraciones(nombre, sep, valores1X2, pTemporada, pJornada)
    /// (Free1X2/Utils/Porcentajes.cs línea 422): graba/anexa una línea de jornada en formato 44
    /// (temporada, jornada, 42 valores) en un fichero histórico, reemplazando la jornada si ya existe.
    /// </summary>
    private static void GuardarValoracionesHistorico(string nombreArchivo, char sep, double[,] valores1X2,
        string temporada, string jornada)
    {
        short formatoFichero;
        if (File.Exists(nombreArchivo))
        {
            using var srv = new StreamReader(nombreArchivo);
            formatoFichero = TestFichero(srv, ref sep);
        }
        else
        {
            formatoFichero = 44;
        }

        if (formatoFichero != 44)
        {
            throw new InvalidOperationException("El fichero no es un fichero de Valoraciones históricas");
        }

        var jornadas = new List<string>();
        bool jornadaYaExiste = false;

        if (File.Exists(nombreArchivo))
        {
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(nombreArchivo);
            var linea = new StringBuilder("");
            while (comBaseCols.SiguienteColumna())
            {
                linea.Clear();
                linea.Append(comBaseCols.LeeColumnaSinComas());
                string[] valorsJornada = linea.ToString().Split(sep);
                if (valorsJornada.Length >= 2 && valorsJornada[0] == temporada && valorsJornada[1] == jornada)
                {
                    jornadaYaExiste = true;
                    linea.Clear();
                    linea.Append(MontaLinea(valores1X2, sep, temporada, jornada));
                }
                jornadas.Add(linea.ToString());
            }
            comBaseCols.Cerrar();
        }

        if (!jornadaYaExiste)
        {
            jornadas.Add(MontaLinea(valores1X2, sep, temporada, jornada));
        }

        IArchivoColumnas comCols = new ArchivoColumnasTexto(nombreArchivo);
        foreach (string str in jornadas)
        {
            comCols.GuardarColsComa(str);
        }
        comCols.Cerrar();
    }

    /// <summary>Réplica de Porcentajes.MontaLinea (Free1X2/Utils/Porcentajes.cs línea 482).</summary>
    private static string MontaLinea(double[,] valores1X2, char separador, string temporada, string jornada)
    {
        var linea = new StringBuilder(temporada);
        linea.Append(separador);
        linea.Append(jornada);

        int n = VariablesGlobales.NumeroPartidos;
        for (int i = 0; i < n; i++)
        {
            for (int x = 0; x < 3; x++)
            {
                linea.Append(separador);
                linea.Append(valores1X2[i, x].ToString().Replace(".", ","));
            }
        }
        return linea.ToString();
    }

    // ===== Detección de formato de fichero (réplica de Porcentajes.TestFichero/TestSeparador) =====

    private static short TestFichero(StreamReader srv, ref char sep)
    {
        string lineaTexto = LeerLinia(srv);

        sep = ',';
        if (TestSeparador(lineaTexto, sep) != 2) return TestSeparador(lineaTexto, sep);
        sep = ' ';
        if (TestSeparador(lineaTexto, sep) != 2) return TestSeparador(lineaTexto, sep);
        sep = (char)9;
        if (TestSeparador(lineaTexto, sep) != 2) return TestSeparador(lineaTexto, sep);
        if (EsNumero(lineaTexto)) return 1;
        return 2;
    }

    private static short TestSeparador(string lineaTexto, char sep)
    {
        string[] aux = lineaTexto.Split(sep);
        if (aux.Length == 3)
        {
            if (EsNumero(aux[0]) && EsNumero(aux[1]) && EsNumero(aux[2])) return 3;
        }
        if (aux.Length == 42 || aux.Length == 45)
        {
            short retorno = 42;
            for (int i = 0; i < 42; i++)
            {
                if (!EsNumero(aux[i])) { retorno = 2; break; }
            }
            return retorno;
        }
        if (aux.Length == 43 || aux.Length == 46)
        {
            short retorno = 43;
            for (int i = 1; i < 43; i++)
            {
                if (!EsNumero(aux[i])) { retorno = 2; break; }
            }
            return retorno;
        }
        if (aux.Length == 44 || aux.Length == 47)
        {
            short retorno = 44;
            for (int i = 2; i < 44; i++)
            {
                if (!EsNumero(aux[i])) { retorno = 2; break; }
            }
            return retorno;
        }
        return 2;
    }

    private static string LeerLinia(StreamReader srv)
    {
        string lineaTexto = "";
        if (srv.Peek() > -1)
        {
            lineaTexto = (srv.ReadLine() ?? "").Trim();
            // (legacy: Replace no reasigna; se conserva el comportamiento)
            lineaTexto.Replace((char)9, ' ');
            lineaTexto.Replace("  ", " ");
        }
        return lineaTexto;
    }

    private static bool EsNumero(string valor)
    {
        try { Convert.ToDouble(valor); return true; }
        catch { return false; }
    }
}
