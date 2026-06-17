using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del WinForms <c>Anastatics</c> (ventana "Estadísticas 0.3.4").
/// Calcula estadísticas sobre un fichero de columnas (combinaciones de 14 signos 1/X/2):
/// el usuario elige un modo de análisis, selecciona el fichero origen, calcula y muestra
/// los resultados en una ventana específica por modo.
///
/// Cableado al motor real: lectura con Free1X2.EntradaSalida.ArchivoColumnasTexto y cálculo
/// con Free1X2.MotorCalculo.Estadisticas.{Dibujos,DibRepes,StaInter,StaSigSeg}. Cada modo abre
/// su ventana de resultados igual que el legacy Mostrar() (dib.Show()/dibrep.Show()/...): los
/// modos "Variantes,X,2" y "Sus coincidencias" navegan a DibFormPage / DibRepFrmPage (legacy
/// DibForm / DibRepFrm) entregando la matriz por el handoff estático
/// DibFormViewModel.MatrizEntrada / DibRepFrmViewModel.MatrizEntrada; los modos "Interrupciones"
/// y "Signos seguidos" rellenan StaInterFrmViewModel / StaSSFormViewModel.
/// </summary>
public partial class AnastaticsViewModel : ObservableObject
{
    /// <summary>
    /// Callback de navegación inyectado por la Page (AnastaticsPage cablea Frame.Navigate),
    /// igual que MainPageViewModel.Navegar. Equivale a dib.Show()/dibrep.Show() del legacy
    /// Mostrar(): abrir la ventana de resultados del modo seleccionado.
    /// </summary>
    public Action<Type>? Navegar { get; set; }

    // Matriz de resultados (legacy: int[,] rsl = new int[15,15]) y nº de columnas leídas.
    private readonly int[,] _rsl = new int[15, 15];
    private string[] _columnas = Array.Empty<string>();
    private int _numcol;
    private string _rutaOrigen = string.Empty;

    // Modos de análisis = los 4 RadioButton del GroupBox "Condiciones" del form legacy.
    // Índices: 0 rdib, 1 rdibrep, 2 rinter, 3 rsigseg (orden visual del legacy).
    public IReadOnlyList<string> Modos { get; } = new[]
    {
        "Variantes, X, 2",      // rdib   -> Dibujos
        "Sus coincidencias",    // rdibrep-> DibRepes
        "Interrupciones",       // rinter -> StaInter
        "Signos seguidos",      // rsigseg-> StaSigSeg
    };

    // rdib.Checked = true por defecto en el legacy => índice 0 seleccionado.
    [ObservableProperty]
    private int _modoSeleccionado;

    // lFileIn.Text — nombre del fichero origen elegido (placeholder legacy "Fichero origen").
    [ObservableProperty]
    private string _ficheroOrigen = "Fichero origen";

    // lColOrg.Text — número de columnas leídas del fichero (placeholder legacy "Columnas").
    [ObservableProperty]
    private string _columnasTexto = "Columnas";

    // gbConds.Enabled / bCalcular.Enabled — activos sólo tras seleccionar un origen válido.
    [ObservableProperty]
    private bool _origenSeleccionado;

    // bMostrar.Enabled — activo sólo tras un cálculo correcto.
    [ObservableProperty]
    private bool _resultadosListos;

    // --- Resultados (rellenados por MostrarResultados según el modo) ---

    /// <summary>Resultado del modo "Interrupciones" (StaInter). Null hasta calcularlo.</summary>
    [ObservableProperty]
    private StaInterFrmViewModel? _resultadoInter;

    /// <summary>Resultado del modo "Signos seguidos" (StaSigSeg). Null hasta calcularlo.</summary>
    [ObservableProperty]
    private StaSSFormViewModel? _resultadoSigSeg;

    [RelayCommand]
    private async Task SeleccionarOrigenAsync()
    {
        // Equivale a SelOrigen() del Anastatics legacy.
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        _rutaOrigen = file.Path;
        FicheroOrigen = file.Name;
        OrigenSeleccionado = false;
        ResultadosListos = false;

        try
        {
            // Lectura + validación VerColumna (14 signos de {1,2,X,x}).
            var (columnas, errorEn) = await Task.Run(() =>
            {
                var lista = new List<string>();
                int idxError = -1;
                IArchivoColumnas ac = new ArchivoColumnasTexto(_rutaOrigen);
                while (ac.SiguienteColumna())
                {
                    string tmp = VerColumna(ac.LeeColumnaSinComas());
                    if (tmp.Length == 0) { idxError = lista.Count; break; }
                    lista.Add(tmp);
                    if (lista.Count == 500000) break;
                }
                ac.Cerrar();
                return (lista, idxError);
            });

            if (errorEn >= 0)
            {
                AppServices.MostrarError("columna errónea = " + errorEn);
                return;
            }

            _columnas = columnas.ToArray();
            _numcol = _columnas.Length;
            ColumnasTexto = _numcol.ToString();
            OrigenSeleccionado = true;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido leer el fichero: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task CalcularAsync()
    {
        // Equivale a Proceso() del Anastatics legacy.
        if (_numcol == 0)
        {
            AppServices.MostrarError("No hay columnas que analizar.");
            return;
        }

        int modo = ModoSeleccionado;
        var columnas = _columnas;
        int numcol = _numcol;

        try
        {
            int[,] rsl = await Task.Run(() => Proceso(modo, columnas, numcol));
            // Vuelca el resultado calculado a la matriz miembro.
            Array.Clear(_rsl, 0, _rsl.Length);
            for (int f = 0; f < 15; f++)
                for (int c = 0; c < 15; c++)
                    _rsl[f, c] = rsl[f, c];

            ResultadosListos = true;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error en el cálculo: " + ex.Message);
        }
    }

    [RelayCommand]
    private void MostrarResultados()
    {
        // Equivale a Mostrar() del Anastatics legacy: según el RadioButton marcado, abre la
        // ventana de resultados del modo (dib.Show()/dibrep.Show()/interrup.Show()/ss.Show()).
        // En WinUI cada ventana es una Page: se siembra su handoff estático con la matriz
        // calculada y se navega a ella (mismo patrón que VisorEstadisticas.UltimasEstadisticas).
        if (!ResultadosListos) return;

        switch (ModoSeleccionado)
        {
            case 0: // rdib -> DibForm(rsl, numcol)
                DibFormViewModel.MatrizEntrada = ClonarMatriz(_rsl);
                DibFormViewModel.NumColEntrada = _numcol;
                Navegar?.Invoke(typeof(DibFormPage));
                break;
            case 1: // rdibrep -> DibRepFrm(rsl, numcol)
                // DibRepFrm sólo usa las 5 primeras filas (rsl[0..4, 0..14]); legacy: rsl es
                // int[5,15]. Aquí se entrega esa porción de la matriz [15,15] calculada.
                DibRepFrmViewModel.MatrizEntrada = ExtraerFilas(_rsl, 5);
                DibRepFrmViewModel.NumColEntrada = _numcol;
                Navegar?.Invoke(typeof(DibRepFrmPage));
                break;
            case 2: // rinter -> StaInterFrm(rsl, numcol)
                StaInterFrmViewModel.MatrizEntrada = ClonarMatriz(_rsl);
                StaInterFrmViewModel.NumColEntrada = _numcol;
                var inter = new StaInterFrmViewModel();
                inter.CargarDatos(_rsl, _numcol);
                ResultadoInter = inter;
                Navegar?.Invoke(typeof(StaInterFrmPage));
                break;
            case 3: // rsigseg -> StaSSForm(rsl, numcol)
                StaSSFormViewModel.MatrizEntrada = ClonarMatriz(_rsl);
                StaSSFormViewModel.NumColEntrada = _numcol;
                var ss = new StaSSFormViewModel();
                ss.CargarDatos(_rsl, _numcol);
                ResultadoSigSeg = ss;
                Navegar?.Invoke(typeof(StaSSFormPage));
                break;
        }
    }

    /// <summary>Copia defensiva de la matriz para el handoff (evita aliasing con _rsl).</summary>
    private static int[,] ClonarMatriz(int[,] origen) => (int[,])origen.Clone();

    /// <summary>Extrae las primeras <paramref name="nfilas"/> filas de la matriz [15,15].</summary>
    private static int[,] ExtraerFilas(int[,] origen, int nfilas)
    {
        int cols = origen.GetLength(1);
        var destino = new int[nfilas, cols];
        for (int f = 0; f < nfilas; f++)
            for (int c = 0; c < cols; c++)
                destino[f, c] = origen[f, c];
        return destino;
    }

    /// <summary>
    /// Réplica de Proceso() del Anastatics legacy: según el modo, recorre las columnas con
    /// la clase de estadística correspondiente y acumula en rsl[15,15].
    /// </summary>
    private static int[,] Proceso(int modo, string[] columnas, int numcol)
    {
        var rsl = new int[15, 15];
        int[] ind;

        switch (modo)
        {
            case 0: // rdib -> Dibujos
                var dibs = new Dibujos();
                for (int nr = 0; nr < numcol; nr++)
                {
                    ind = dibs.Procesar(columnas[nr]);
                    rsl[ind[3], ind[4]]++;
                }
                break;

            case 1: // rdibrep -> DibRepes
                {
                    int numx, num2;
                    int num1 = numx = num2 = 0;
                    var dibrep = new DibRepes();
                    for (int nr = 0; nr < numcol; nr++)
                    {
                        ind = dibrep.Procesar(columnas[nr]);
                        rsl[0, ind[0]]++;
                        if (num1 == ind[2]) rsl[1, ind[2]]++;
                        if (numx == ind[3]) rsl[2, ind[3]]++;
                        if (num2 == ind[4]) rsl[3, ind[4]]++;
                        if ((numx + num2) == (ind[3] + ind[4])) rsl[4, (numx + num2)]++;
                        num1 = ind[2]; numx = ind[3]; num2 = ind[4];
                    }
                }
                break;

            case 2: // rinter -> StaInter
                var inter = new StaInter();
                for (int nr = 0; nr < numcol; nr++)
                {
                    ind = inter.Procesar(columnas[nr]);
                    rsl[0, ind[0]]++;
                    rsl[1, ind[2]]++;
                    rsl[2, ind[3]]++;
                    rsl[3, ind[4]]++;
                    rsl[4, ind[1]]++;
                }
                break;

            case 3: // rsigseg -> StaSigSeg
                var sigseg = new StaSigSeg();
                for (int nr = 0; nr < numcol; nr++)
                {
                    ind = sigseg.Procesar(columnas[nr]);
                    rsl[0, ind[2]]++;
                    rsl[1, ind[3]]++;
                    rsl[2, ind[4]]++;
                    rsl[3, ind[1]]++;
                }
                break;
        }

        return rsl;
    }

    /// <summary>Valida una columna: 14 signos de {1,2,x,X}; devuelve "" si es errónea (legacy VerColumna).</summary>
    private static string VerColumna(string columna)
    {
        string chval = "12xX";
        if (columna.Length != 14) return "";
        for (int nr = 0; nr < 14; nr++)
        {
            char ch = columna[nr];
            if (chval.IndexOf(ch) < 0) return "";
        }
        return columna.Replace('x', 'X');
    }
}
