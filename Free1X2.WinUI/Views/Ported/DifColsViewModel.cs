// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Diferencias entre columnas" (legacy: DifCols).
/// Genera columnas de 14 signos (las 4.782.969 triples internamente — legacy
/// MetodoInterno — o leídas de un fichero — legacy MetodoExterno) y acepta cada una
/// sólo si el número de diferencias respecto a la columna base / fichero de condiciones
/// y respecto a las columnas ya aceptadas cae dentro de los rangos configurados
/// (legacy PreparaDifs / Proceso / compara).
/// </summary>
public partial class DifColsViewModel : ObservableObject
{
    // Resultado del último cálculo (legacy: ArrayList aceptadas).
    private List<string> _aceptadas = new();

    // Cancelación del cálculo en curso (legacy: campo bool 'salida').
    private CancellationTokenSource? _cts;

    // -------- Generación por (legacy groupBox "Generación por": rb14T / rbFile) --------

    // 14 triples internamente (legacy: rb14T.Checked). Marcado por defecto.
    [ObservableProperty]
    private bool _generaTriples = true;

    // Generar leyendo de un fichero de entrada (legacy: rbFile.Checked).
    [ObservableProperty]
    private bool _generaFichero;

    // Nombre del fichero de entrada seleccionado/usado (legacy: lblFileIn).
    [ObservableProperty]
    private string _nombreFicheroEntrada = string.Empty;

    // -------- Condicionada por (legacy groupBox2: rbBase / rbColsB) --------

    // Condicionar respecto a una columna base escrita a mano (legacy: rbBase.Checked).
    [ObservableProperty]
    private bool _condicionColumnaBase = true;

    // Condicionar respecto a un fichero de condiciones (legacy: rbColsB.Checked).
    [ObservableProperty]
    private bool _condicionFichero;

    // Columna base de 14 signos (legacy: tbColbase.Text, MaxLength 14, mayúsculas).
    [ObservableProperty]
    private string _columnaBase = "1X1212X2111121";

    // Nombre del fichero de condiciones usado (legacy: lblFileCond).
    [ObservableProperty]
    private string _nombreFicheroCondiciones = string.Empty;

    // Rangos de diferencias admitidas respecto a la columna base, formato "min(-max)"
    // separado por comas, p.ej. "0-5,6,7-14" (legacy: tbdifbase.Text, MaxLength 20).
    [ObservableProperty]
    private string _diferenciasBase = "0-5,6,7-14";

    // Rangos de diferencias admitidas respecto a las columnas ya aceptadas
    // (legacy: tbdifresul.Text, MaxLength 20).
    [ObservableProperty]
    private string _diferenciasResultado = "6-14";

    // -------- Proceso (legacy groupBox3 "Proceso") --------

    // Límite de columnas a grabar (legacy: tblimcol.Text; al grabar, si vacío/erróneo
    // se usa 4.782.969). NumberBox.Value es double por contrato.
    [ObservableProperty]
    private double _limiteColumnas;

    // Nº de columnas admitidas (legacy: lblResul, aceptadas.Count). String para TextBlock.
    [ObservableProperty]
    private string _admitidasTexto = "0";

    // Tiempo de cálculo (legacy: lblTime, (time9-time0)). String para TextBlock.
    [ObservableProperty]
    private string _tiempoTexto = "—";

    // Habilita el botón Calcular (legacy: bCalc.Enabled; se desactiva durante el cálculo).
    [ObservableProperty]
    private bool _puedeCalcular = true;

    // Habilita el botón Grabar (legacy: bGrab.Enabled; arranca deshabilitado, se activa
    // tras un cálculo con resultados).
    [ObservableProperty]
    private bool _puedeGrabar;

    // Habilita el botón Cancelar (legacy: bCancelar; sólo tiene efecto durante el cálculo).
    [ObservableProperty]
    private bool _puedeCancelar;

    /// <summary>
    /// Lanza el cálculo de columnas admitidas. Legacy: BCalcClick -> Calcular().
    /// </summary>
    [RelayCommand]
    private async Task Calcular()
    {
        // --- Construir condiciones (legacy: rbBase / rbColsB) en el hilo de UI ---
        var condis = new List<string>();
        if (CondicionColumnaBase)
        {
            string tmp = VerColumna(ColumnaBase, out _);
            if (tmp.Length == 0)
            {
                AppServices.MostrarError("columna errónea");
                return;
            }
            condis.Add(tmp);
        }
        else
        {
            var file = await AbrirTxtAsync();
            if (file == null) return;
            NombreFicheroCondiciones = Path.GetFileName(file.Path);
            try
            {
                foreach (var linea in await Task.Run(() => File.ReadAllLines(file.Path)))
                {
                    if (string.IsNullOrEmpty(linea)) continue;
                    string tmp = VerColumna(linea, out _);
                    if (tmp.Length == 0) { AppServices.MostrarError("columna errónea"); return; }
                    condis.Add(tmp);
                }
            }
            catch (Exception ex) { AppServices.MostrarError(ex.Message); return; }
        }

        // --- Fichero de entrada (legacy: MetodoExterno) elegido por adelantado ---
        string? rutaEntrada = null;
        if (GeneraFichero)
        {
            var file = await AbrirTxtAsync();
            if (file == null) return;
            rutaEntrada = file.Path;
            NombreFicheroEntrada = Path.GetFileName(rutaEntrada);
        }

        // PreparaDifs(): máscaras de rangos admitidos.
        string difbase, difresul;
        try
        {
            difbase = PreparaDif(DiferenciasBase);
            difresul = PreparaDif(DiferenciasResultado);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Rango de diferencias inválido: " + ex.Message);
            return;
        }

        _cts = new CancellationTokenSource();
        CancellationToken token = _cts.Token;
        bool generaTriples = GeneraTriples;

        PuedeCalcular = false;
        PuedeGrabar = false;
        PuedeCancelar = true;
        AdmitidasTexto = "calculando...";
        TiempoTexto = " ";
        var time0 = DateTime.Now;
        try
        {
            var aceptadas = await Task.Run(() =>
                Procesar(condis, difbase, difresul, generaTriples, rutaEntrada, token), token);
            _aceptadas = aceptadas;
            AdmitidasTexto = aceptadas.Count.ToString();
            PuedeGrabar = aceptadas.Count > 0;
        }
        catch (OperationCanceledException)
        {
            AdmitidasTexto = _aceptadas.Count.ToString();
            PuedeGrabar = _aceptadas.Count > 0;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error en el cálculo: " + ex.Message);
        }
        finally
        {
            TiempoTexto = (DateTime.Now - time0).ToString().Substring(0, 10);
            PuedeCalcular = true;
            PuedeCancelar = false;
            _cts.Dispose();
            _cts = null;
        }
    }

    /// <summary>
    /// Graba las columnas admitidas a un fichero de texto. Legacy: BGrabClick -> Grabar().
    /// </summary>
    [RelayCommand]
    private async Task Grabar()
    {
        if (_aceptadas.Count == 0) return;

        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Diferencias",
        };
        picker.FileTypeChoices.Add("F.Salida", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        // límite = LimiteColumnas si > 0; si 0 -> 4.782.969 (legacy try/catch).
        int limite = LimiteColumnas > 0 ? (int)LimiteColumnas : 4782969;
        string ruta = file.Path;
        var copia = _aceptadas;
        PuedeCalcular = false;
        PuedeGrabar = false;
        try
        {
            await Task.Run(() =>
            {
                using var sw = new StreamWriter(ruta);
                int ngrab = 0;
                foreach (var cps in copia)
                {
                    sw.WriteLine(cps.Replace('4', 'X'));
                    if (++ngrab >= limite) break;
                }
            });
            AppServices.MostrarInfo("Columnas grabadas.");
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al grabar: " + ex.Message);
        }
        finally
        {
            PuedeCalcular = true;
            PuedeGrabar = true;
        }
    }

    /// <summary>
    /// Solicita cancelar el cálculo en curso. Legacy: BCancelarClick -> salida = true.
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        _cts?.Cancel();
    }

    /// <summary>
    /// Abre el generador de CPs por diferencias. Legacy: btnDiferencias_Click ->
    /// new GeneradorCPSDiferencias().ShowDialog().
    /// </summary>
    [RelayCommand]
    private void AbrirCpsPorDiferencias()
    {
        // Navegación a la pantalla portada GeneradorCPSDiferencias: es responsabilidad del
        // host de la Page (Frame.Navigate), no del cableado del dominio.
    }

    // ---- Lógica portada de DifCols ----

    // VerColumna(): valida 14 signos de "124xXF" y devuelve la columna con x/X->4.
    // 'mask' indica las posiciones fijas ('F') que no cuentan en las comparaciones.
    private static string VerColumna(string columna, out string mask)
    {
        const string chval = "124xXF";
        mask = "";
        if (columna.Length != 14) return "";
        for (int nr = 0; nr < 14; nr++)
        {
            char ch = columna[nr];
            if (chval.IndexOf(ch) < 0) return "";
            mask += chval.IndexOf(ch) == 5 ? 'F' : 'V';
        }
        return columna.Replace('x', '4').Replace('X', '4');
    }

    // PreparaDifs(): construye una máscara de 15 chars ('A'=admitido, 'F'=fuera).
    private static string PreparaDif(string rangos)
    {
        string dif = "FFFFFFFFFFFFFFF";
        foreach (var trozo in rangos.Split(','))
        {
            var mgg = trozo.Split('-');
            int nv1 = Convert.ToInt32(mgg[0]);
            if (nv1 < 0) nv1 = 0;
            if (nv1 > 14) nv1 = 14;
            int nv2 = mgg.Length == 2 ? Convert.ToInt32(mgg[1]) : nv1;
            if (nv2 < 0) nv2 = 0;
            if (nv2 > 14) nv2 = 14;
            string tmp = "";
            for (int nr = 0; nr < nv1; nr++) tmp += dif[nr];
            for (int nr = nv1; nr <= nv2; nr++) tmp += 'A';
            for (int nr = nv2 + 1; nr < 15; nr++) tmp += dif[nr];
            dif = tmp;
        }
        return dif;
    }

    // compara(): cuenta diferencias entre dos columnas saltando posiciones 'F' de la máscara.
    // El criterio legacy es (nova[nr] & refer[nr]) == 48 ('0'): los signos '1','2','4' (binarios
    // 0x31/0x32/0x34) sólo dan AND == 0x30 cuando son distintos en sus bits 1/2/4.
    private static int Compara(string nova, string refer, string mask)
    {
        int nd = 0;
        for (int nr = 0; nr < 14; nr++)
        {
            if (mask[nr] == 'F') continue;
            if ((nova[nr] & refer[nr]) == 48) nd++;
        }
        return nd;
    }

    // Proceso(): acepta la columna si pasa todas las condiciones y diferencias con las aceptadas.
    // Devuelve la lista de columnas aceptadas (cada una con 'F' en las posiciones fijas).
    private static List<string> Procesar(List<string> condis, string difbase, string difresul,
        bool generaTriples, string? rutaEntrada, CancellationToken token)
    {
        var aceptadas = new List<string>();

        void Proceso(string columna, string mask)
        {
            foreach (var cnd in condis)
            {
                int nd = Compara(columna, cnd, mask);
                if (difbase[nd] == 'F') return;
            }
            foreach (var cnd in aceptadas)
            {
                int nd = Compara(columna, cnd, mask);
                if (difresul[nd] == 'F') return;
            }
            string tmp = "";
            for (int nr = 0; nr < 14; nr++) tmp += mask[nr] == 'F' ? 'F' : columna[nr];
            aceptadas.Add(tmp);
        }

        // La máscara aplicable depende de las posiciones fijas; en el legacy 'mask' se fija en
        // VerColumna de la última columna candidata. Aquí se calcula por candidata.
        if (generaTriples)
        {
            // MetodoInterno(): 14 bucles 1..3, '3'->'4'.
            var c = new char[14];
            string maskTriples = "VVVVVVVVVVVVVV"; // las triples no tienen posiciones fijas
            void Recurse(int idx)
            {
                if (token.IsCancellationRequested) throw new OperationCanceledException(token);
                if (idx == 14)
                {
                    Proceso(new string(c), maskTriples);
                    return;
                }
                for (int v = 1; v < 4; v++)
                {
                    c[idx] = v == 3 ? '4' : (char)('0' + v);
                    Recurse(idx + 1);
                }
            }
            Recurse(0);
        }
        else if (rutaEntrada != null)
        {
            // MetodoExterno(): VerColumna por línea del fichero de entrada.
            foreach (var linea in File.ReadLines(rutaEntrada))
            {
                if (token.IsCancellationRequested) throw new OperationCanceledException(token);
                if (string.IsNullOrEmpty(linea)) continue;
                string tmp = VerColumna(linea, out string mask);
                if (tmp.Length == 0) throw new InvalidOperationException("columna errónea");
                Proceso(tmp, mask);
            }
        }

        return aceptadas;
    }

    private static async Task<Windows.Storage.StorageFile?> AbrirTxtAsync()
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);
        return await picker.PickSingleFileAsync();
    }
}
