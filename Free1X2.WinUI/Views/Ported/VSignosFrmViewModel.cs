using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
/// Una casilla editable de la columna ganadora (legacy: TextBox tb01..tb16 -> part[i]).
/// Guarda el nº de partido y su signo (1 / X / 2 / -). "-" (o vacío) significa comodín.
/// </summary>
public partial class CasillaColumnaGanadora : ObservableObject
{
    [ObservableProperty] private int _partido;
    [ObservableProperty] private string _signo = "-";

    public CasillaColumnaGanadora(int partido) => _partido = partido;
}

/// <summary>
/// Una fila de la rejilla de recuentos por partido (solo lectura). Refleja una fila de
/// <c>vals[noPartidos,3]</c>: nº de partido y cuántas veces aparece cada signo (1 / X / 2)
/// entre las columnas contabilizadas. Es una proyección de presentación; no altera el motor.
/// </summary>
public sealed class RecuentoSignoFila
{
    public int Partido { get; }
    public int Unos { get; }
    public int Equis { get; }
    public int Doses { get; }

    public RecuentoSignoFila(int partido, int unos, int equis, int doses)
    {
        Partido = partido;
        Unos = unos;
        Equis = equis;
        Doses = doses;
    }
}

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
/// La columna ganadora editable (legacy: txts[] -> part[]) está portada como
/// <see cref="ColumnaGanadora"/> (una casilla por partido, "-" = comodín); AcCb() cuenta
/// los aciertos de cada columna del fichero frente a ella, igual que el legacy.
/// La rejilla de recuentos por partido (1/X/2) aún no se muestra, pero los recuentos se
/// conservan internamente para poder grabarlos (Grabar).
/// </summary>
public partial class VSignosFrmViewModel : ObservableObject
{
    // Nº de partidos del fichero (legacy: noPartidos). Por defecto 15 hasta abrir fichero.
    private int _noPartidos = 15;

    /// <summary>
    /// Columna ganadora editable (legacy: txts[] -> part[]). Una casilla por partido,
    /// con "-" como comodín. Se redimensiona al abrir fichero (AdaptarVistaAPartidos).
    /// </summary>
    public ObservableCollection<CasillaColumnaGanadora> ColumnaGanadora { get; } = new();

    /// <summary>
    /// Proyección de presentación (solo lectura) de <see cref="_vals"/>: una fila por partido
    /// con los recuentos de 1 / X / 2. Se rellena al calcular; alimenta la rejilla de la vista.
    /// </summary>
    public ObservableCollection<RecuentoSignoFila> Recuentos { get; } = new();

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

    public VSignosFrmViewModel()
    {
        // Legacy: VSignosFrm ctor -> AdaptarVistaAPartidos() crea las casillas para noPartidos.
        AdaptarColumnaGanadora();
    }

    /// <summary>
    /// Redimensiona la columna ganadora a <see cref="_noPartidos"/> casillas, todas en "-".
    /// Equivale a la parte de VSignosFrm.AdaptarVistaAPartidos() que muestra/oculta los txts[].
    /// </summary>
    private void AdaptarColumnaGanadora()
    {
        ColumnaGanadora.Clear();
        for (int i = 1; i <= _noPartidos; i++)
        {
            ColumnaGanadora.Add(new CasillaColumnaGanadora(i));
        }
    }

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
        // Legacy: AdaptarVistaAPartidos() redimensiona la columna ganadora editable.
        AdaptarColumnaGanadora();

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

        // Legacy: RecuperaPantalla() vuelca txts[i].Text a part[] ('-' = comodín si vacío).
        char[] part = RecuperaColumnaGanadora(noPartidos);

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

                    // Legacy: AcCB() — cuenta aciertos frente a la columna ganadora part[].
                    int aciertos = AcCb(columna, part, noPartidos, considerarPleno);
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

        // Proyecta vals[noPartidos,3] a la rejilla de recuentos (solo presentación).
        Recuentos.Clear();
        for (int nr = 0; nr < noPartidos; nr++)
        {
            Recuentos.Add(new RecuentoSignoFila(nr + 1, vals[nr, 0], vals[nr, 1], vals[nr, 2]));
        }

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
        foreach (var casilla in ColumnaGanadora)
        {
            casilla.Signo = "-";
        }
    }

    /// <summary>
    /// Fija el signo "1" de la columna ganadora para el partido de la fila pulsada
    /// (legacy: VSignosFrm.lbl_Click sobre la etiqueta de la columna "1" -> txts[indice].Text = "1").
    /// Pulsar una celda de la rejilla de recuentos rellena la casilla correspondiente.
    /// </summary>
    [RelayCommand]
    private void FijarUno(RecuentoSignoFila? fila) => FijarSigno(fila, "1");

    /// <summary>
    /// Fija el signo "X" de la columna ganadora para el partido de la fila pulsada
    /// (legacy: lbl_Click sobre la etiqueta de la columna "X" -> txts[indice].Text = "X").
    /// </summary>
    [RelayCommand]
    private void FijarEquis(RecuentoSignoFila? fila) => FijarSigno(fila, "X");

    /// <summary>
    /// Fija el signo "2" de la columna ganadora para el partido de la fila pulsada
    /// (legacy: lbl_Click sobre la etiqueta de la columna "2" -> txts[indice].Text = "2").
    /// </summary>
    [RelayCommand]
    private void FijarDos(RecuentoSignoFila? fila) => FijarSigno(fila, "2");

    /// <summary>
    /// Vuelca el signo pulsado a la casilla editable de la columna ganadora del partido
    /// correspondiente (legacy: VSignosFrm.lbl_Click -> txts[partido - 1].Text = signo).
    /// </summary>
    private void FijarSigno(RecuentoSignoFila? fila, string signo)
    {
        if (fila == null) return;
        int indice = fila.Partido - 1; // legacy: indice = partido - 1
        if (indice < 0 || indice >= ColumnaGanadora.Count) return;
        ColumnaGanadora[indice].Signo = signo;
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
    /// Vuelca la columna ganadora editable a un char[] (legacy: RecuperaPantalla() -> part[]).
    /// Casilla vacía o "-" => comodín ('-'); en otro caso, su primer carácter (1/X/2).
    /// </summary>
    private char[] RecuperaColumnaGanadora(int noPartidos)
    {
        var part = new char[noPartidos];
        for (int i = 0; i < noPartidos; i++)
        {
            string txt = i < ColumnaGanadora.Count ? (ColumnaGanadora[i].Signo ?? string.Empty) : string.Empty;
            part[i] = string.IsNullOrEmpty(txt) ? '-' : txt[0];
        }
        return part;
    }

    /// <summary>
    /// Cuenta los aciertos de una columna frente a la columna ganadora part[]
    /// (legacy: VSignosFrm.AcCB, líneas 269-316). Una posición cuenta como acierto si su signo
    /// coincide con el de part[] o si part[] es comodín ('-' u otro carácter distinto de 1/X/2).
    /// </summary>
    private static int AcCb(string columna, char[] part, int noPartidos, bool considerarPleno)
    {
        int rsl = 0;
        // Primeros noPartidos-1 partidos.
        for (int nr = 0; nr < noPartidos - 1; nr++)
        {
            char ch = part[nr];
            if (ch == columna[nr]) rsl++;
            else if (ch == '1' || ch == 'X' || ch == '2') { /* signo fijado que no acierta */ }
            else rsl++; // comodín
        }

        // Último partido (pleno).
        char chUlt = part[part.Length - 1];
        if (considerarPleno)
        {
            // Solo se evalúa el pleno si los demás partidos han acertado todos (legacy).
            if (rsl == noPartidos - 1)
            {
                if (chUlt == columna[columna.Length - 1]) rsl++;
                else if (chUlt == '1' || chUlt == 'X' || chUlt == '2') { }
                else rsl++;
            }
        }
        else
        {
            // Ignora la condición de pleno: el último partido cuenta como uno más.
            if (chUlt == columna[columna.Length - 1]) rsl++;
            else if (chUlt == '1' || chUlt == 'X' || chUlt == '2') { }
            else rsl++;
        }
        return rsl;
    }
}
