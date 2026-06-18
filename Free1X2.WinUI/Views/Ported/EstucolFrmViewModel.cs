using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.Analisis;
using Free1X2.Escrutinio;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "EstuCol - Generador / Analizador de Columnas Probables"
/// (legacy: EstucolFrm). El usuario selecciona dos archivos de texto (columnas reducidas y
/// columnas ganadoras), elige un modo de agrupación/emparejamiento de columnas y genera un
/// informe del escrutinio de las columnas emparejadas frente a las ganadoras.
///
/// Cableado al motor real: empareja las columnas reducidas (suma de bits via UtilColumnas),
/// escruta cada emparejamiento contra cada ganadora con Escrutador.EscrutaApuestaMultiple y
/// construye los informes InformeColumnasABDON por columna y por ganadora. Los resultados se
/// dejan en el handoff estático para el visor (VisorAnalisisColumnasAbdonFrmViewModel).
/// </summary>
public partial class EstucolFrmViewModel : ObservableObject
{
    /// <summary>
    /// Handoff estático con los resultados del escrutinio ABDON para el visor.
    /// Equivale a los argumentos del ctor legacy VisorAnalisisColumnasAbdonFrm(informePorCols,
    /// informePorGans, columnas). El visor lo lee al navegar (patrón AppState.GrupoEnEdicion).
    /// </summary>
    public static (List<InformeColumnasABDON> porCols, List<InformeColumnasABDON> porGans, List<long> columnas)? UltimoInforme { get; set; }

    // Total de columnas ganadoras leídas del fichero (legacy: noTotalGanadoras).
    private int _noTotalGanadoras;

    // Columnas emparejadas (legacy: List<long> ColumnasEmparejadas).
    private readonly List<long> _columnasEmparejadas = new();

    // Matriz de aciertos [emparejada, ganadora] (legacy: int[,] contenedorAciertos).
    private int[,]? _contenedorAciertos;

    /// <summary>Acción para navegar al visor de informe (la cablea la página).</summary>
    public Action? AbrirVisor { get; set; }

    /// <summary>Ruta del archivo de columnas reducidas (legacy: pathReducidas).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreArchivoReducidas))]
    [NotifyPropertyChangedFor(nameof(PuedeComenzar))]
    private string _pathReducidas = string.Empty;

    /// <summary>Ruta del archivo de columnas ganadoras (legacy: pathGanadoras).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreArchivoGanadoras))]
    [NotifyPropertyChangedFor(nameof(PuedeComenzar))]
    private string _pathGanadoras = string.Empty;

    /// <summary>
    /// Modo de agrupación seleccionado (legacy: rdbAgrupacionA/B/C -> ModoEmparejamientoColumnasABDON).
    /// A = "1,2 - 3,4..." (por defecto), B = "1,2 - 2,3...", C = "1,3 - 2,4...".
    /// </summary>
    [ObservableProperty]
    private bool _agrupacionA = true;

    /// <summary>Modo de agrupación B (legacy: rdbAgrupacionB).</summary>
    [ObservableProperty]
    private bool _agrupacionB;

    /// <summary>Modo de agrupación C (legacy: rdbAgrupacionC).</summary>
    [ObservableProperty]
    private bool _agrupacionC;

    /// <summary>Texto de estado del proceso (legacy: lblEstado, status strip).</summary>
    [ObservableProperty]
    private string _estado = "Listo";

    /// <summary>Nombre del archivo de reducidas para mostrar (legacy: lblNombreArchivoReducidas.Text).</summary>
    public string NombreArchivoReducidas =>
        string.IsNullOrEmpty(PathReducidas) ? "Sin archivo" : System.IO.Path.GetFileName(PathReducidas);

    /// <summary>Nombre del archivo de ganadoras para mostrar (legacy: lblArchivoGanadoras.Text).</summary>
    public string NombreArchivoGanadoras =>
        string.IsNullOrEmpty(PathGanadoras) ? "Sin archivo" : System.IO.Path.GetFileName(PathGanadoras);

    /// <summary>
    /// True si ambos archivos están seleccionados (legacy: ActivaBtnComenzar / ComprobarEntradas:
    /// btnComenzar.Enabled = pathGanadoras != "" &amp;&amp; pathReducidas != "").
    /// </summary>
    public bool PuedeComenzar =>
        !string.IsNullOrEmpty(PathReducidas) && !string.IsNullOrEmpty(PathGanadoras);

    /// <summary>Abre el selector de archivo de columnas reducidas (legacy: btnAbreArchivoReducidas_Click).</summary>
    [RelayCommand]
    private async Task AbrirArchivoReducidasAsync()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;
        PathReducidas = file.Path;
    }

    /// <summary>Abre el selector de archivo de columnas ganadoras (legacy: btnAbreArchivoGanadoras_Click).</summary>
    [RelayCommand]
    private async Task AbrirArchivoGanadorasAsync()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;
        PathGanadoras = file.Path;

        // Legacy: tras seleccionar, cuenta el total de líneas para noTotalGanadoras.
        try
        {
            _noTotalGanadoras = 0;
            using var sr = new StreamReader(file.Path);
            while (sr.Peek() != -1) { sr.ReadLine(); _noTotalGanadoras++; }
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido leer el fichero de ganadoras: " + ex.Message);
        }
    }

    /// <summary>
    /// Ejecuta el proceso completo de generación de informe (legacy: btnComenzar_Click).
    /// </summary>
    [RelayCommand]
    private async Task ComenzarAsync()
    {
        Estado = "Comprobando Entradas";
        if (!PuedeComenzar)
        {
            AppServices.MostrarError("Debe especificar los dos archivos");
            Estado = "Listo";
            return;
        }

        ModoEmparejamientoColumnasABDON modo = DeterminarMetodoOrdenacion();
        string rutaReducidas = PathReducidas;
        string rutaGanadoras = PathGanadoras;

        try
        {
            Estado = "Emparejando columnas";
            await Task.Run(() => EmparejarColumnasReducidas(modo, rutaReducidas));

            Estado = "Escrutando Columnas";
            await Task.Run(() => EscrutarColumnasEmparejadas(rutaGanadoras));

            Estado = "Generando Informe";
            var (porCols, porGans) = await Task.Run(GenerarInforme);

            UltimoInforme = (porCols, porGans, new List<long>(_columnasEmparejadas));
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al generar el informe: " + ex.Message);
            Estado = "Listo";
            return;
        }

        Estado = "Listo";

        // Navega al visor (legacy: new VisorAnalisisColumnasAbdonFrm(...).Show()).
        // El handoff queda en UltimoInforme; VisorAnalisisColumnasAbdonFrmViewModel lo lee
        // al navegar y puebla los informes (ya cableado).
        AbrirVisor?.Invoke();
    }

    // ===== Lógica de dominio cableada al motor real (1:1 del WinForms EstucolFrm) =====

    /// <summary>
    /// Empareja las columnas reducidas según el modo (legacy: EmparejarColumnasReducidas).
    /// Suma columnas con OR de bits (UtilColumnas.ConvStrToLong + SumaColumnas = colA | colB).
    /// </summary>
    private void EmparejarColumnasReducidas(ModoEmparejamientoColumnasABDON modo, string rutaReducidas)
    {
        _columnasEmparejadas.Clear();
        long colTemp = 0, colTemp2 = 0;
        int noCol = 0;

        using var sr = new StreamReader(rutaReducidas);
        string? linea;
        while ((linea = sr.ReadLine()) != null)
        {
            long columnaLeidaLong = UtilColumnas.ConvStrToLong(linea);
            noCol++;
            switch (modo)
            {
                case ModoEmparejamientoColumnasABDON.A:
                    if (noCol % 2 == 0)
                    {
                        long colResultado = colTemp | columnaLeidaLong;
                        if (colResultado != columnaLeidaLong) _columnasEmparejadas.Add(colResultado);
                    }
                    colTemp = columnaLeidaLong;
                    break;
                case ModoEmparejamientoColumnasABDON.B:
                    long colRes = colTemp | columnaLeidaLong;
                    if (colRes != columnaLeidaLong) _columnasEmparejadas.Add(colRes);
                    colTemp = columnaLeidaLong;
                    break;
                case ModoEmparejamientoColumnasABDON.C:
                    if (noCol % 2 != 0)
                    {
                        long colResultado = colTemp | columnaLeidaLong;
                        if (colResultado != columnaLeidaLong) _columnasEmparejadas.Add(colResultado);
                        colTemp = columnaLeidaLong;
                    }
                    else
                    {
                        long colResultado = colTemp2 | columnaLeidaLong;
                        if (colResultado != columnaLeidaLong) _columnasEmparejadas.Add(colResultado);
                        colTemp2 = columnaLeidaLong;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Escruta cada columna emparejada contra cada ganadora (legacy: EscrutarColumnasEmparejadas).
    /// Rellena la matriz contenedorAciertos[emparejada, ganadora] con EscrutaApuestaMultiple.
    /// </summary>
    private void EscrutarColumnasEmparejadas(string rutaGanadoras)
    {
        if (_columnasEmparejadas.Count == 0)
        {
            _contenedorAciertos = new int[0, 0];
            return;
        }

        _contenedorAciertos = new int[_columnasEmparejadas.Count, _noTotalGanadoras];
        var esc = new Escrutador();
        int noColGanadora = 0;

        using var sr = new StreamReader(rutaGanadoras);
        string? linea;
        while ((linea = sr.ReadLine()) != null)
        {
            if (noColGanadora >= _noTotalGanadoras) break;
            long colGan = UtilColumnas.ConvStrToLong(linea);
            for (int i = 0; i < _columnasEmparejadas.Count; i++)
            {
                int aciertos = esc.EscrutaApuestaMultiple(_columnasEmparejadas[i], colGan);
                _contenedorAciertos[i, noColGanadora] = aciertos;
            }
            noColGanadora++;
        }
    }

    /// <summary>
    /// Construye los informes por columna y por ganadora (legacy: ObtenDatosUnaColTodasGan +
    /// ObtenDatosUnaGanTodasCol + GenerarInforme).
    /// </summary>
    private (List<InformeColumnasABDON> porCols, List<InformeColumnasABDON> porGans) GenerarInforme()
    {
        var porCols = new List<InformeColumnasABDON>();
        var porGans = new List<InformeColumnasABDON>();
        if (_contenedorAciertos == null) return (porCols, porGans);

        int nCols = _contenedorAciertos.GetLength(0);
        int nGans = _contenedorAciertos.GetLength(1);

        // Una columna, todas las ganadoras.
        for (int i = 0; i < nCols; i++)
        {
            var serie = new int[nGans];
            for (int j = 0; j < nGans; j++) serie[j] = _contenedorAciertos[i, j];
            porCols.Add(new InformeColumnasABDON(serie));
        }

        // Una ganadora, todas las columnas.
        for (int i = 0; i < nGans; i++)
        {
            var serie = new int[nCols];
            for (int j = 0; j < nCols; j++) serie[j] = _contenedorAciertos[j, i];
            porGans.Add(new InformeColumnasABDON(serie));
        }

        return (porCols, porGans);
    }

    /// <summary>Determina el modo según los radios (legacy: DeterminarMetodoOrdenacion).</summary>
    private ModoEmparejamientoColumnasABDON DeterminarMetodoOrdenacion()
    {
        if (AgrupacionB) return ModoEmparejamientoColumnasABDON.B;
        if (AgrupacionC) return ModoEmparejamientoColumnasABDON.C;
        return ModoEmparejamientoColumnasABDON.A;
    }
}
