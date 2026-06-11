using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del WinForms <c>SelecJM</c> ("Selección 5+5+4 de JuanM").
/// Reparte los 14 partidos en 3 grupos (gr.1 = 5 partidos, gr.2 = 5 partidos,
/// gr.3 = 4 partidos), genera las tablas ordenadas de productos de cada grupo
/// (243 / 243 / 81), aplica límites por grupo y por total, y obtiene las
/// columnas válidas — bien por corte de rangos, bien por valor exacto.
/// También analiza columnas ganadoras leídas de fichero para ubicar su rango
/// dentro de cada grupo. Toda la lógica de cálculo/persistencia está como TODO.
/// </summary>
public partial class SelecJMViewModel : ObservableObject
{
    // ===== Grupos (g01..g14): a qué grupo 1/2/3 pertenece cada uno de los 14 partidos =====
    // En el form legacy son 14 TextBox de 1 carácter. Validación: exactamente 5 unos, 5 doses y 4 treses.
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
    // Rangos de posiciones admitidas dentro de cada tabla ordenada y del total. Formato "a-b,c,..."
    [ObservableProperty] private string _limiteGrupo1 = "1-243"; // legacy tbgr1
    [ObservableProperty] private string _limiteGrupo2 = "1-243"; // legacy tbgr2
    [ObservableProperty] private string _limiteGrupo3 = "1-81";  // legacy tbgr3
    [ObservableProperty] private string _limiteTotal = "1-567";  // legacy tbtot

    // ===== Resultados de % por grupo (readonly: lpc1/lpc2/lpc3) =====
    [ObservableProperty] private string _porcentaje1 = "%1";
    [ObservableProperty] private string _porcentaje2 = "%2";
    [ObservableProperty] private string _porcentaje3 = "%3";

    // ===== Modo de examen (GroupBox "Examinar": rbcorte / rbvalor) =====
    // ComboBox con dos opciones; default "Por corte".
    public IReadOnlyList<string> ModosExamen { get; } = new[] { "Por corte", "Por valor" };

    [ObservableProperty] private string _modoExamen = "Por corte";

    // ===== Fichero de condiciones (lfcond) =====
    [ObservableProperty] private string _ficheroCondiciones = "(ninguno)";

    // ===== Análisis de resultados (GroupBox "Análisis Resultados") =====
    // Fichero de columnas ganadoras y navegación por las mismas.
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

    // Habilita "Analizar" una vez cargado el fichero de ganadoras (bAnalizar.Enabled).
    [ObservableProperty] private bool _puedeAnalizar;

    // ===== Acciones =====

    [RelayCommand]
    private void Calcular()
    {
        // TODO: Dominio legacy — SelecJM.Calcular():
        //   RecuperaPantalla()  -> valida 5+5+4 grupos, parsea límites (tablim1/2/3/tt) y lee nvals = valors1.RetVals().
        //   Calgr1()/Calgr2()/Calgr3() -> generan pg1[243]/pg2[243]/pg3[81], ordenan (mxsort) y calculan lpc1/lpc2/lpc3.
        //   Si ModoExamen == "Por corte" -> BuscaCorte(); else BuscaValor();  -> rellenan BitArray validas[3^14] y ctval/ctproc.
        //   veureelmeu() -> vuelca tiempo, ctval -> Validas, ctproc -> Procesadas. (Timer legacy de 3 s.)
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO: Dominio legacy — SelecJM.BCancelarClick(): salida = true (aborta el bucle de cálculo en curso).
    }

    [RelayCommand]
    private void GrabarColumnas()
    {
        // TODO: Dominio legacy — SelecJM.GrabaCols():
        //   Escribe "tablasJM.xls" con pg1/pg2/pg3 y, vía SaveFileDialog, el fichero .txt con todas las columnas válidas (n2s).
        //   En WinUI usar FileSavePicker. -> FicheroSalida.
    }

    [RelayCommand]
    private void LeerCondiciones()
    {
        // TODO: Dominio legacy — SelecJM.LeeCondis():
        //   OpenFileDialog (*.cnd) -> 6 líneas: grupos(14 chars), límites gr1/gr2/gr3, total y modo (0=valor,1=corte).
        //   En WinUI usar FileOpenPicker; rellena Grupo01..14, LimiteGrupo1/2/3, LimiteTotal, ModoExamen, FicheroCondiciones.
    }

    [RelayCommand]
    private void SalvarCondiciones()
    {
        // TODO: Dominio legacy — SelecJM.SalvaCondis():
        //   SaveFileDialog (*.cnd) -> graba grupos+límites+modo. En WinUI usar FileSavePicker. -> FicheroCondiciones.
    }

    [RelayCommand]
    private void CargarGanadoras()
    {
        // TODO: Dominio legacy — SelecJM.EntraCGsR():
        //   OpenFileDialog (*.txt) -> lee columnas ganadoras (Pral.Normaliza, longitud >= 14) a colgsR[].
        //   Posiciona en la última: IndiceGanadora, ColumnaGanadora; PuedeAnalizar = true; FicheroGanadoras = nombre.
    }

    [RelayCommand]
    private void Analizar()
    {
        // TODO: Dominio legacy — SelecJM.Analizar():
        //   RecuperaPantalla + Calgr1/2/3; según ModoExamen ejecuta AnaCorte() o AnaValor().
        //   Ubica la columna ganadora (tbCG) en pg1/pg2/pg3 -> RangoGrupo1/2/3 y RangoTotal (suma de rangos).
    }

    [RelayCommand]
    private void GanadoraSiguiente()
    {
        // TODO: Dominio legacy — SelecJM.GRMas(): avanza nrfCGR en colgsR[] -> IndiceGanadora, ColumnaGanadora.
    }

    [RelayCommand]
    private void GanadoraAnterior()
    {
        // TODO: Dominio legacy — SelecJM.GRMenos(): retrocede nrfCGR en colgsR[] -> IndiceGanadora, ColumnaGanadora.
    }
}
