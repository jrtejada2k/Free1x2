using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Análisis de signos" (legacy: VSignosFrm).
/// Procesa un fichero de columnas y contabiliza, partido a partido, cuántas veces
/// aparece cada signo (1/X/2). Permite elegir el formato de presentación
/// (% enteros, % con decimales, o columnas/recuentos), introducir una columna
/// ganadora de referencia y fijar un nivel de "aspiración" para el escrutinio parcial.
///
/// Herramienta autónoma fichero→fichero. Usa el motor real
/// Free1X2.EntradaSalida.ArchivoColumnasTexto para dimensionar el nº de partidos.
///
/// LIMITACIÓN: la fila editable de "columna ganadora" (legacy: txts[] -> part[]) y la
/// rejilla de recuentos por partido NO están portadas en VSignosFrmPage.xaml (la propia
/// Page indica que la rejilla se mostrará "tras migrar el motor"). Como part[] no tiene
/// entrada de UI, queda en blanco (comodín), de modo que AcCB() puntúa todas las columnas
/// como válidas y se contabiliza el recuento global de signos — comportamiento equivalente
/// al legacy con columna ganadora vacía. Los recuentos se conservan internamente para
/// poder grabarlos (Grabar) aunque la rejilla aún no se muestre.
/// </summary>
public partial class VSignosFrmViewModel : ObservableObject
{
    // Nº de partidos del fichero (legacy: noPartidos). Por defecto 15 hasta abrir fichero.
    private int _noPartidos = 15;

    // Recuento por partido y signo (legacy: int[noPartidos,3] vals; 0=1, 1=X, 2=2).
    private int[,] _vals = new int[15, 3];

    // Columnas aceptadas (legacy: ArrayList aceptadas) para el modo "columnas" al grabar.
    private readonly List<string> _aceptadas = new();

    private bool _hayResultado;

    /// <summary>Ruta del fichero de columnas a procesar (legacy: archivo / lFileIn).</summary>
    [ObservableProperty]
    private string _ficheroEntrada = string.Empty;

    /// <summary>Nombre del fichero de entrada mostrado al usuario (legacy: lFileIn.Text).</summary>
    [ObservableProperty]
    private string _nombreFicheroEntrada = "Fichero a procesar";

    /// <summary>
    /// Modo de presentación seleccionado (legacy: rbcen0 / rbcen2 / rbcol).
    /// Bindea a ComboBox.SelectedItem; los items vienen de <see cref="ModosPresentacion"/>.
    /// </summary>
    [ObservableProperty]
    private string _modoPresentacion = "% enteros";

    /// <summary>Opciones del modo de presentación (legacy: grupo "Ver" con 3 radios).</summary>
    public IReadOnlyList<string> ModosPresentacion { get; } = new[]
    {
        "% enteros",      // rbcen0
        "% con decimales",// rbcen2
        "columnas",       // rbcol
    };

    /// <summary>Considerar el último partido como Pleno al 15 (legacy: chkPleno).</summary>
    [ObservableProperty]
    private bool _considerarPleno = true;

    /// <summary>
    /// Nivel de aspiración: nº mínimo de aciertos para contabilizar una columna
    /// en el escrutinio parcial (legacy: lbasp + botones bMas/bMenos).
    /// NumberBox.Value es double.
    /// </summary>
    [ObservableProperty]
    private double _aspiracion;

    /// <summary>Número de partidos en juego (legacy: noPartidos / lbasp tope).</summary>
    [ObservableProperty]
    private double _numeroPartidos = 15;

    // --- Resultados / estado del proceso (solo lectura para la vista) ---

    /// <summary>Columnas procesadas del fichero (legacy: lproc / ctproc).</summary>
    [ObservableProperty]
    private string _columnasProcesadasTexto = "Procesadas: -";

    /// <summary>Tiempo empleado en el cálculo (legacy: ltime).</summary>
    [ObservableProperty]
    private string _tiempoTexto = "Tiempo: -";

    /// <summary>Premios remanentes por categoría (legacy: lp100viu).</summary>
    [ObservableProperty]
    private string _premiosRemanentesTexto = "Premios remanentes: -";

    // --- Acciones ---

    /// <summary>Selecciona el fichero de columnas de entrada (legacy: BFileInClick -> EntradaFichero).</summary>
    [RelayCommand]
    private async Task AbrirFicheroAsync()
    {
        // Legacy: VSignosFrm.EntradaFichero() — OpenFileDialog *.txt + dimensionar noPartidos.
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        FicheroEntrada = file.Path;
        NombreFicheroEntrada = Path.GetFileName(file.Path);

        // Legacy: IArchivoColumnas aCol = new ArchivoColumnasTexto(ruta);
        //         noPartidos = aCol.ObtenNumSignos(); aCol.Cerrar();
        try
        {
            IArchivoColumnas aCol = new ArchivoColumnasTexto(file.Path);
            _noPartidos = aCol.ObtenNumSignos();
            aCol.Cerrar();
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido leer el fichero de columnas: " + ex.Message);
            return;
        }

        if (_noPartidos < 10) _noPartidos = 15;
        NumeroPartidos = _noPartidos;
        // Legacy: AdaptarVariables() -> vals = new int[noPartidos,3].
        _vals = new int[_noPartidos, 3];

        // Legacy: EntradaFichero() llama a Calcular() automáticamente tras abrir.
        await CalcularAsync();
    }

    /// <summary>Procesa el fichero y contabiliza los signos por partido (legacy: BCalcularClick -> Calcular).</summary>
    [RelayCommand]
    private async Task CalcularAsync()
    {
        if (string.IsNullOrEmpty(FicheroEntrada))
        {
            AppServices.MostrarError("Seleccione primero un fichero de columnas.");
            return;
        }

        var time0 = DateTime.Now;
        int ctproc = 0;
        _aceptadas.Clear();
        // Premios remanentes (legacy: premis[noPartidos-9]).
        int tamPremis = Math.Max(1, _noPartidos - 9);
        var premis = new int[tamPremis];
        var vals = new int[_noPartidos, 3];
        int limac = (int)Aspiracion;
        bool considerarPleno = ConsiderarPleno;
        int noPartidos = _noPartidos;
        string ruta = FicheroEntrada;

        try
        {
            await Task.Run(() =>
            {
                using var sr = new StreamReader(ruta);
                string? linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string columna = linea.Trim().ToUpper();
                    if (columna.Length < noPartidos) continue;
                    ctproc++;

                    // Legacy: AcCB() — sin columna ganadora (part[] en blanco), todos los
                    // partidos cuentan como acierto (rama "else rsl++").
                    int aciertos = AcCb(columna, noPartidos, considerarPleno);
                    if (aciertos >= limac)
                    {
                        // Legacy: Contabiliza().
                        for (int nr = 0; nr < noPartidos; nr++)
                        {
                            char ch = columna[nr];
                            if (ch == '1') vals[nr, 0]++;
                            else if (ch == '2') vals[nr, 2]++;
                            else vals[nr, 1]++;
                        }
                        _aceptadas.Add(columna);
                    }
                    if (aciertos > 9 && aciertos <= noPartidos)
                    {
                        int idx = noPartidos - aciertos;
                        if (idx >= 0 && idx < premis.Length) premis[idx]++;
                    }
                }
            });
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al procesar el fichero: " + ex.Message);
            return;
        }

        _vals = vals;
        _hayResultado = true;

        ColumnasProcesadasTexto = "Procesadas: " + ctproc;
        string temp = (DateTime.Now - time0) + "0000000000";
        TiempoTexto = "Tiempo: " + temp.Substring(0, Math.Min(10, temp.Length));

        var sb = new System.Text.StringBuilder("Premios remanentes: ");
        for (int i = 0; i < premis.Length; i++)
        {
            sb.Append(premis[i]);
            if (i < premis.Length - 1) sb.Append('-');
        }
        PremiosRemanentesTexto = sb.ToString();
    }

    /// <summary>Graba el resultado (valoración o columnas aceptadas) a fichero (legacy: BGrabarClick -> Grabar).</summary>
    [RelayCommand]
    private async Task GrabarAsync()
    {
        if (!_hayResultado)
        {
            AppServices.MostrarError("Calcule primero el análisis antes de grabar.");
            return;
        }

        bool modoColumnas = ModoPresentacion == "columnas";
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = modoColumnas ? "Columnas" : "Valoracion",
        };
        picker.FileTypeChoices.Add(modoColumnas ? "Columnas" : "Valoración", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        string ruta = file.Path;
        var inv = CultureInfo.InvariantCulture;
        try
        {
            await Task.Run(() =>
            {
                using var sw = new StreamWriter(ruta);
                if (modoColumnas)
                {
                    // Legacy: foreach (string col in aceptadas) sw.WriteLine(col).
                    foreach (string col in _aceptadas) sw.WriteLine(col);
                }
                else
                {
                    // Legacy: porcentaje por partido (f0 enteros / f2 decimales).
                    bool decimales = ModoPresentacion == "% con decimales";
                    int filas = _vals.GetLength(0);
                    for (int nr = 0; nr < filas; nr++)
                    {
                        double sum = _vals[nr, 0] + _vals[nr, 1] + _vals[nr, 2];
                        if (sum == 0) sum = 1;
                        double v1 = (_vals[nr, 0] / sum) * 1e2;
                        double vx = (_vals[nr, 1] / sum) * 1e2;
                        double v2 = (_vals[nr, 2] / sum) * 1e2;
                        string fmt = decimales ? "f2" : "f0";
                        sw.WriteLine(string.Format(inv, "{0}, {1}, {2}",
                            v1.ToString(fmt, inv), vx.ToString(fmt, inv), v2.ToString(fmt, inv)));
                    }
                }
            });
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al grabar el resultado: " + ex.Message);
        }
    }

    /// <summary>Reinicia la columna ganadora a guiones (legacy: BLimpClick -> ReinicializaColumnaGanadora).</summary>
    [RelayCommand]
    private void LimpiarColumnaGanadora()
    {
        // Legacy: VSignosFrm.ReinicializaColumnaGanadora() -> txts[i].Text = "-".
        // La fila editable de columna ganadora no está portada en la Page; al no haber
        // estado de columna ganadora aquí, no hay nada que reiniciar por ahora.
        // TODO[dominio]: cuando se porte la fila editable de "columna ganadora" (txts[]),
        //   reiniciar sus 14/15 signos a "-" (Free1X2/UI/VSignosFrm.cs línea 381).
    }

    /// <summary>Sube en 1 el nivel de aspiración (legacy: BMasClick -> AspMas).</summary>
    [RelayCommand]
    private void SubirAspiracion()
    {
        // Legacy: VSignosFrm.AspMas() -> if (naux < noPartidos) lbasp = naux + 1.
        if (Aspiracion < NumeroPartidos)
        {
            Aspiracion += 1;
        }
    }

    /// <summary>Baja en 1 el nivel de aspiración (legacy: BMenosClick -> AspMenos).</summary>
    [RelayCommand]
    private void BajarAspiracion()
    {
        // Legacy: VSignosFrm.AspMenos() -> if (naux > 0) lbasp = naux - 1.
        if (Aspiracion > 0)
        {
            Aspiracion -= 1;
        }
    }

    /// <summary>
    /// Cuenta los aciertos de una columna frente a la columna ganadora (legacy: VSignosFrm.AcCB).
    /// Como part[] (la columna ganadora) no tiene UI portada, todas las posiciones se tratan
    /// como comodín, sumando un acierto en cada partido (ramas "else rsl++" del legacy).
    /// </summary>
    private static int AcCb(string columna, int noPartidos, bool considerarPleno)
    {
        // part[] no informado -> cada posición es comodín -> rsl++ en todos los partidos
        // salvo el último, que sigue la misma rama comodín (con/sin pleno da el mismo resultado).
        int rsl = noPartidos - 1; // los primeros noPartidos-1 partidos.
        // Último partido (pleno): rama comodín -> +1 en ambos casos (con o sin pleno).
        if (considerarPleno)
        {
            if (rsl == noPartidos - 1) rsl++;
        }
        else
        {
            rsl++;
        }
        return rsl;
    }
}
