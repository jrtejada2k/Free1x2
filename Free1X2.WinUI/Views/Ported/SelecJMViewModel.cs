using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del WinForms <c>SelecJM</c> ("Selección 5+5+4 de JuanM", archivo UI/SelectorJM.cs).
/// Reparte los 14 partidos en 3 grupos (gr.1 = 5 partidos, gr.2 = 5 partidos,
/// gr.3 = 4 partidos), genera las tablas ordenadas de productos de cada grupo
/// (243 / 243 / 81), aplica límites por grupo y por total, y obtiene las
/// columnas válidas — bien por corte de rangos, bien por valor exacto.
/// También analiza columnas ganadoras leídas de fichero para ubicar su rango.
/// Los selectores de ficheros (.cnd / ganadoras) están cableados; el cálculo
/// queda como TODO porque depende del control de valoraciones (valors1), no portado.
/// </summary>
public partial class SelecJMViewModel : ObservableObject
{
    // Fichero de ganadoras navegable (legacy: colgsR / nrfCGR / limcgsR).
    private readonly List<string> _colgsR = new();
    private int _nrfCGR;

    // ===== Grupos (g01..g14): a qué grupo 1/2/3 pertenece cada uno de los 14 partidos =====
    [ObservableProperty] private string _grupo01 = "1";
    [ObservableProperty] private string _grupo02 = "1";
    [ObservableProperty] private string _grupo03 = "1";
    [ObservableProperty] private string _grupo04 = "1";
    [ObservableProperty] private string _grupo05 = "1";
    [ObservableProperty] private string _grupo06 = "2";
    [ObservableProperty] private string _grupo07 = "2";
    [ObservableProperty] private string _grupo08 = "2";
    [ObservableProperty] private string _grupo09 = "2";
    [ObservableProperty] private string _grupo10 = "2";
    [ObservableProperty] private string _grupo11 = "3";
    [ObservableProperty] private string _grupo12 = "3";
    [ObservableProperty] private string _grupo13 = "3";
    [ObservableProperty] private string _grupo14 = "3";

    // ===== Límites (GroupBox "Límites") =====
    [ObservableProperty] private string _limiteGrupo1 = "1-243"; // legacy tbgr1
    [ObservableProperty] private string _limiteGrupo2 = "1-243"; // legacy tbgr2
    [ObservableProperty] private string _limiteGrupo3 = "1-81";  // legacy tbgr3
    [ObservableProperty] private string _limiteTotal = "1-567";  // legacy tbtot

    // ===== Resultados de % por grupo (readonly: lpc1/lpc2/lpc3) =====
    [ObservableProperty] private string _porcentaje1 = "%1";
    [ObservableProperty] private string _porcentaje2 = "%2";
    [ObservableProperty] private string _porcentaje3 = "%3";

    // ===== Modo de examen (GroupBox "Examinar": rbcorte / rbvalor) =====
    public IReadOnlyList<string> ModosExamen { get; } = new[] { "Por corte", "Por valor" };

    [ObservableProperty] private string _modoExamen = "Por corte";

    // ===== Fichero de condiciones (lfcond) =====
    [ObservableProperty] private string _ficheroCondiciones = "(ninguno)";

    // ===== Análisis de resultados (GroupBox "Análisis Resultados") =====
    [ObservableProperty] private string _ficheroGanadoras = "Fichero ganadoras"; // legacy lFGR
    [ObservableProperty] private string _columnaGanadora = string.Empty;          // legacy tbCG (max 14)
    [ObservableProperty] private string _indiceGanadora = string.Empty;           // legacy lbCGR

    // Rangos calculados de la columna analizada dentro de cada grupo (ang1/ang2/ang3/angt).
    [ObservableProperty] private string _rangoGrupo1 = "-";
    [ObservableProperty] private string _rangoGrupo2 = "-";
    [ObservableProperty] private string _rangoGrupo3 = "-";
    [ObservableProperty] private string _rangoTotal = "-";

    // ===== Estado del proceso (lproc / lvalidas / ltime / lfile) =====
    [ObservableProperty] private string _procesadas = "Procesadas";
    [ObservableProperty] private string _validas = "Válidas";
    [ObservableProperty] private string _tiempo = "Tiempo";
    [ObservableProperty] private string _ficheroSalida = "(ninguno)";

    [ObservableProperty] private bool _puedeAnalizar;

    // ===== Acciones =====

    [RelayCommand]
    private void Calcular()
    {
        // TODO: lógica en Free1X2/UI/SelectorJM.cs línea 168 (Calcular) + RecuperaPantalla (246),
        //   Calgr1/2/3, BuscaCorte/BuscaValor. El cálculo parte de la matriz de valoraciones del
        //   control 'valors1' (RecuperaPantalla -> valors1.RetVals(), línea 248), un UserControl de
        //   valoraciones aún no portado a WinUI; sin él no hay datos que cablear al motor sin
        //   inventar valores. Los selectores de condiciones y ganadoras sí están cableados.
        Procesadas = "Requiere control de valoraciones (ver SelectorJM.cs línea 168)";
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Legacy BCancelarClick: salida = true (aborta el bucle de cálculo). El cálculo no está
        // portado aún, por lo que no hay bucle que cancelar.
    }

    [RelayCommand]
    private async Task GrabarColumnas()
    {
        // El legacy GrabaCols() escribe "tablasJM.xls" + las columnas de validas[]; ambos requieren
        // el cálculo previo (no portado). Aquí se cablea solo el selector de salida.
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "ResultadosJM",
        };
        picker.FileTypeChoices.Add("Resultados", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;
        FicheroSalida = Path.GetFileName(file.Path);
        // TODO: volcar validas[] a 'file.Path' (Free1X2/UI/SelectorJM.cs línea 195, GrabaCols)
        //   tras portar el cálculo (depende del control de valoraciones valors1).
    }

    [RelayCommand]
    private async Task LeerCondiciones()
    {
        // LeeCondis() legacy: lee un .cnd de 6 líneas (grupos, límites gr1/gr2/gr3, total, modo).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".cnd");
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            var buff = await Task.Run(() => File.ReadAllLines(file.Path));
            if (buff.Length < 6) { AppServices.MostrarError("fichero de condiciones erróneo"); return; }

            string grupos = buff[0];
            if (grupos.Length < 14) { AppServices.MostrarError("fichero de condiciones erróneo"); return; }
            Grupo01 = grupos[0].ToString(); Grupo02 = grupos[1].ToString();
            Grupo03 = grupos[2].ToString(); Grupo04 = grupos[3].ToString();
            Grupo05 = grupos[4].ToString(); Grupo06 = grupos[5].ToString();
            Grupo07 = grupos[6].ToString(); Grupo08 = grupos[7].ToString();
            Grupo09 = grupos[8].ToString(); Grupo10 = grupos[9].ToString();
            Grupo11 = grupos[10].ToString(); Grupo12 = grupos[11].ToString();
            Grupo13 = grupos[12].ToString(); Grupo14 = grupos[13].ToString();
            LimiteGrupo1 = buff[1];
            LimiteGrupo2 = buff[2];
            LimiteGrupo3 = buff[3];
            LimiteTotal = buff[4];
            ModoExamen = buff[5] == "0" ? "Por valor" : "Por corte";
            FicheroCondiciones = Path.GetFileName(file.Path);
        }
        catch (Exception ex) { AppServices.MostrarError(ex.Message); }
    }

    [RelayCommand]
    private async Task SalvarCondiciones()
    {
        // SalvaCondis() legacy: graba grupos + límites + modo a un .cnd (6 líneas).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Condiciones",
        };
        picker.FileTypeChoices.Add("F.Condiciones", new List<string> { ".cnd" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        string grupos = Grupo01 + Grupo02 + Grupo03 + Grupo04 + Grupo05 + Grupo06 + Grupo07 +
            Grupo08 + Grupo09 + Grupo10 + Grupo11 + Grupo12 + Grupo13 + Grupo14;
        var lineas = new[]
        {
            grupos,
            LimiteGrupo1,
            LimiteGrupo2,
            LimiteGrupo3,
            LimiteTotal,
            ModoExamen == "Por corte" ? "1" : "0",
        };
        try
        {
            await Task.Run(() => File.WriteAllLines(file.Path, lineas));
            FicheroCondiciones = Path.GetFileName(file.Path);
        }
        catch (Exception ex) { AppServices.MostrarError(ex.Message); }
    }

    [RelayCommand]
    private async Task CargarGanadoras()
    {
        // EntraCGsR() legacy: lee columnas ganadoras (>=14 chars) a colgsR[].
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        _colgsR.Clear();
        try
        {
            foreach (var linea in await Task.Run(() => File.ReadAllLines(file.Path)))
            {
                string tmp = linea.Trim().ToUpper();
                if (tmp.Length == 0) continue;
                if (tmp.Length < 14) { AppServices.MostrarError("col.G. errónea=" + (_colgsR.Count + 1)); break; }
                _colgsR.Add(tmp);
            }
        }
        catch (Exception ex) { AppServices.MostrarError(ex.Message); return; }

        if (_colgsR.Count == 0) return;
        _nrfCGR = _colgsR.Count;
        FicheroGanadoras = Path.GetFileName(file.Path);
        IndiceGanadora = _nrfCGR.ToString();
        ColumnaGanadora = _colgsR[_nrfCGR - 1];
        PuedeAnalizar = true;
    }

    [RelayCommand]
    private void Analizar()
    {
        // TODO: lógica en Free1X2/UI/SelectorJM.cs línea 599 (Analizar): RecuperaPantalla + Calgr1/2/3
        //   + AnaCorte/AnaValor. Depende del control de valoraciones (valors1), no portado a WinUI;
        //   ubica la columna ganadora en pg1/pg2/pg3 -> RangoGrupo1/2/3 y RangoTotal.
        RangoTotal = "Requiere control de valoraciones (ver SelectorJM.cs línea 599)";
    }

    [RelayCommand]
    private void GanadoraSiguiente()
    {
        // GRMas() legacy: avanza nrfCGR en colgsR[].
        if (_nrfCGR < _colgsR.Count)
        {
            _nrfCGR++;
            IndiceGanadora = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    [RelayCommand]
    private void GanadoraAnterior()
    {
        // GRMenos() legacy: retrocede nrfCGR en colgsR[].
        if (_nrfCGR > 1)
        {
            _nrfCGR--;
            IndiceGanadora = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }
}
