using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

// ViewModel para FiltroPorcenJBPage.
// Porta el WinForms legacy Free1X2.UI.FiltroPorcenJB ("Separador Porcentajes Juan Bellas").
//
// CABLEADO REAL: la persistencia de rangos/límites (*.jb7) y la navegación de columnas
// ganadoras se cablean al sistema de ficheros con pickers de WinUI.
//
// LIMITACIÓN DE DOMINIO: el cálculo (Calcular), el análisis (Analizar) y la exportación
// (Exportar) dependen de la matriz de porcentajes 'cps[14,7]' que el form legacy obtiene de
// 'valors1.RetVals()' — el UserControl Free1X2.UI.Controls.valors, que NO está portado a WinUI
// (no existe en FiltroPorcenJBPage.xaml). Sin esos porcentajes scps[] queda vacío y Valida()
// no puede clasificar columnas. Por eso Calcular/Analizar/Exportar quedan como TODO con la
// referencia exacta; el resto (rangos, límites, ganadoras) sí está cableado.
public partial class FiltroPorcenJBViewModel : ObservableObject
{
    // Columnas ganadoras cargadas (legacy: string[3000] colgsR) y navegación (limcgsR / nrfCGR).
    private readonly List<string> _colgsR = new();
    private int _nrfCGR;

    private bool _salida;

    // --- Limites de banda (legacy tblim1..tblim6, "<=" cut-offs). Defaults del Designer. ---
    [ObservableProperty]
    private double _limite1 = 15;

    [ObservableProperty]
    private double _limite2 = 22;

    [ObservableProperty]
    private double _limite3 = 29;

    [ObservableProperty]
    private double _limite4 = 36;

    [ObservableProperty]
    private double _limite5 = 48;

    [ObservableProperty]
    private double _limite6 = 61;

    // --- Rangos "min-max" por banda (legacy tbrank1..tbrank7), texto libre tipo "0-3". ---
    [ObservableProperty]
    private string _rango1 = "0-3";

    [ObservableProperty]
    private string _rango2 = "0-3";

    [ObservableProperty]
    private string _rango3 = "0-3";

    [ObservableProperty]
    private string _rango4 = "0-3";

    [ObservableProperty]
    private string _rango5 = "0-3";

    [ObservableProperty]
    private string _rango6 = "0-3";

    [ObservableProperty]
    private string _rango7 = "0-3";

    // Rango de recorrido max-min (legacy tbmgreco), "0-14".
    [ObservableProperty]
    private string _recorrido = "0-14";

    // --- Columna ganadora a analizar (legacy tbCG). ---
    [ObservableProperty]
    private string _columnaGanadora = "COL.GANADORA";

    // --- Etiquetas de resultado (legacy labels read-only). string para bindear a TextBlock.Text. ---
    [ObservableProperty]
    private string _columnasProcesadas = "procesadas";

    [ObservableProperty]
    private string _columnasAdmitidas = "admitidas";

    [ObservableProperty]
    private string _tiempo = "tiempo";

    [ObservableProperty]
    private string _ficheroResultado = "fichero";

    [ObservableProperty]
    private string _ficheroRangos = "fichero";

    [ObservableProperty]
    private string _ficheroGanadoras = "fichero ganadoras";

    [ObservableProperty]
    private string _contadorGanadoras = "-";

    // Resultado del analisis por banda (legacy lrk1..lrk7 + lreco).
    [ObservableProperty]
    private string _analisisBanda1 = "-";

    [ObservableProperty]
    private string _analisisBanda2 = "-";

    [ObservableProperty]
    private string _analisisBanda3 = "-";

    [ObservableProperty]
    private string _analisisBanda4 = "-";

    [ObservableProperty]
    private string _analisisBanda5 = "-";

    [ObservableProperty]
    private string _analisisBanda6 = "-";

    [ObservableProperty]
    private string _analisisBanda7 = "-";

    [ObservableProperty]
    private string _analisisRecorrido = "-";

    // --- Acciones ---

    [RelayCommand]
    private void Calcular()
    {
        // Legacy: FiltroPorcenJB.Calcular() — RecuperaPantalla() (que llama valors1.RetVals()),
        // PintaPantalla() y filtra el fichero de entrada con Valida().
        // TODO[dominio]: depende del UserControl 'valors' (matriz de porcentajes cps[14,7]) que
        //   NO está portado en FiltroPorcenJBPage.xaml. Sin él scps[] queda vacío y Valida() no
        //   puede clasificar columnas. Portar primero la rejilla de valoraciones 'valors'
        //   (Free1X2/UI/Controls/valors + FiltroPorcenJB.cs RecuperaPantalla línea 224, Valida 465).
        AppServices.MostrarInfo("El cálculo requiere la rejilla de valoraciones ('valors'), aún no portada.");
    }

    [RelayCommand]
    private void GrabarResultado()
    {
        // Legacy: FiltroPorcenJB.GrabaCols() — guarda las columnas admitidas (validas[]) a *.txt.
        // TODO[dominio]: depende del resultado de Calcular() (lista 'validas'), que a su vez
        //   depende de la rejilla 'valors' no portada (FiltroPorcenJB.cs GrabaCols línea 398).
        AppServices.MostrarInfo("No hay resultado que grabar: el cálculo aún no está disponible (falta 'valors').");
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Legacy: FiltroPorcenJB.BCancelarClick -> salida=true (aborta el bucle de Calcular).
        _salida = true;
    }

    [RelayCommand]
    private async Task SalvarRangosAsync()
    {
        // Legacy: FiltroPorcenJB.SalvarConds() — persiste 7 rangos + recorrido + 6 límites a *.jb7.
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "RangosJB",
        };
        picker.FileTypeChoices.Add("Condiciones", new List<string> { ".jb7" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        var inv = CultureInfo.InvariantCulture;
        try
        {
            using var sw = new StreamWriter(file.Path);
            sw.WriteLine(Rango1);
            sw.WriteLine(Rango2);
            sw.WriteLine(Rango3);
            sw.WriteLine(Rango4);
            sw.WriteLine(Rango5);
            sw.WriteLine(Rango6);
            sw.WriteLine(Rango7);
            sw.WriteLine(Recorrido);
            sw.WriteLine(string.Join(',', new[]
            {
                ((int)Limite1).ToString(inv), ((int)Limite2).ToString(inv), ((int)Limite3).ToString(inv),
                ((int)Limite4).ToString(inv), ((int)Limite5).ToString(inv), ((int)Limite6).ToString(inv),
            }));
            FicheroRangos = Path.GetFileName(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se han podido guardar los rangos: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task LeerRangosAsync()
    {
        // Legacy: FiltroPorcenJB.LeerConds() — carga 7 rangos + recorrido + 6 límites desde *.jb7.
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".jb7");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            string[] lineas = await File.ReadAllLinesAsync(file.Path);
            if (lineas.Length >= 1) Rango1 = lineas[0];
            if (lineas.Length >= 2) Rango2 = lineas[1];
            if (lineas.Length >= 3) Rango3 = lineas[2];
            if (lineas.Length >= 4) Rango4 = lineas[3];
            if (lineas.Length >= 5) Rango5 = lineas[4];
            if (lineas.Length >= 6) Rango6 = lineas[5];
            if (lineas.Length >= 7) Rango7 = lineas[6];
            if (lineas.Length >= 8) Recorrido = lineas[7];
            if (lineas.Length >= 9)
            {
                string[] lims = lineas[8].Split(',');
                if (lims.Length >= 1) Limite1 = ADouble(lims[0]);
                if (lims.Length >= 2) Limite2 = ADouble(lims[1]);
                if (lims.Length >= 3) Limite3 = ADouble(lims[2]);
                if (lims.Length >= 4) Limite4 = ADouble(lims[3]);
                if (lims.Length >= 5) Limite5 = ADouble(lims[4]);
                if (lims.Length >= 6) Limite6 = ADouble(lims[5]);
            }
            FicheroRangos = Path.GetFileName(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se han podido leer los rangos: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Analizar()
    {
        // Legacy: FiltroPorcenJB.Analizar() — cuenta coincidencias de ColumnaGanadora por banda.
        // TODO[dominio]: depende de scps[] (matriz de porcentajes 'cps') derivado del UserControl
        //   'valors' no portado. Sin él no se puede analizar (FiltroPorcenJB.cs Analizar línea 536).
        AppServices.MostrarInfo("El análisis requiere la rejilla de valoraciones ('valors'), aún no portada.");
    }

    [RelayCommand]
    private void Exportar()
    {
        // Legacy: FiltroPorcenJB.ExporCols() — exporta la rejilla de signos por banda (cps) a CSV.
        // TODO[dominio]: depende de la matriz 'cps' derivada del UserControl 'valors' no portado
        //   (FiltroPorcenJB.cs ExporCols línea 602).
        AppServices.MostrarInfo("La exportación requiere la rejilla de valoraciones ('valors'), aún no portada.");
    }

    [RelayCommand]
    private async Task CargarGanadorasAsync()
    {
        // Legacy: FiltroPorcenJB.EntraCGsR() — carga el fichero de ganadoras y selecciona la última.
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            _colgsR.Clear();
            using (var sr = new StreamReader(file.Path))
            {
                string? linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string tmp = linea.Trim().ToUpper();
                    if (tmp.Length < 14)
                    {
                        AppServices.MostrarError("col.G. errónea=" + tmp);
                        return;
                    }
                    _colgsR.Add(tmp);
                }
            }
            if (_colgsR.Count == 0)
            {
                AppServices.MostrarError("El fichero de ganadoras está vacío.");
                return;
            }
            _nrfCGR = _colgsR.Count;
            FicheroGanadoras = Path.GetFileName(file.Path);
            ContadorGanadoras = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido leer el fichero de ganadoras: " + ex.Message);
        }
    }

    [RelayCommand]
    private void GanadoraSiguiente()
    {
        // Legacy: FiltroPorcenJB.GRMas() — avanza a la siguiente columna ganadora.
        if (_nrfCGR < _colgsR.Count)
        {
            _nrfCGR++;
            ContadorGanadoras = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    [RelayCommand]
    private void GanadoraAnterior()
    {
        // Legacy: FiltroPorcenJB.GRMenos() — retrocede a la columna ganadora anterior.
        if (_nrfCGR > 1)
        {
            _nrfCGR--;
            ContadorGanadoras = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    private static double ADouble(string? s) => double.TryParse(s, out double v) ? v : 0;
}
