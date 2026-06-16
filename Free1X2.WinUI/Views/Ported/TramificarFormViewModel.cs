using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de la rejilla de resultados (legacy dgResultados, alimentada por la ArrayList de
/// <see cref="Tramo"/>). Cada propiedad mapea una columna del DataGridTableStyle legacy
/// (ver TramificarForm.InicializaGridResultados): Nº, Col. Inf., Col. Sup., Nº Cols.,
/// Prob. Max., 14..10, Nº Premios, Imp. Premios, Ingresos-Gastos. Los valores se exponen como
/// string (regla anti-crash 2: enlazar ints/doubles directos a TextBlock peta el x:Bind).
/// </summary>
public partial class TramoFilaViewModel : ObservableObject
{
    public TramoFilaViewModel(Tramo tr)
    {
        var ci = CultureInfo.CurrentCulture;
        NumeroDeTramo = tr.NumeroDeTramo.ToString(ci);
        ColInferior = tr.ValorIzquierda.ToString(ci);
        ColSuperior = tr.ValorDerecha.ToString(ci);
        NumColumnas = tr.NumColumnasTramo.ToString(ci);
        ProbMax = tr.ProbAcumulada.ToString(ci);
        P14 = tr.P14.ToString(ci);
        P13 = tr.P13.ToString(ci);
        P12 = tr.P12.ToString(ci);
        P11 = tr.P11.ToString(ci);
        P10 = tr.P10.ToString(ci);
        NumPremios = tr.ColumnasPremiadas.ToString(ci);
        ImportePremios = tr.TotalImportePremios.ToString(ci);
        Balance = tr.Balance.ToString(ci);
    }

    public string NumeroDeTramo { get; }
    public string ColInferior { get; }
    public string ColSuperior { get; }
    public string NumColumnas { get; }
    public string ProbMax { get; }
    public string P14 { get; }
    public string P13 { get; }
    public string P12 { get; }
    public string P11 { get; }
    public string P10 { get; }
    public string NumPremios { get; }
    public string ImportePremios { get; }
    public string Balance { get; }
}

/// <summary>
/// ViewModel del WinForms <c>TramificarForm</c> ("Tramificar").
/// Reparte el universo de columnas (3^14 = 4.782.969) en tramos según su valoración,
/// usando los datos de premios de la L.A.E. de una jornada concreta, calcula la
/// probabilidad acumulada y permite filtrar por posiciones mínimas/máximas de cada
/// categoría de premio y localizar columnas concretas.
///
/// Motor de dominio usado: <see cref="Tramo"/> (acumula premios y balance de cada tramo),
/// <see cref="ArchivoColumnasTexto"/> (lectura/escritura de ficheros L.A.E. y de límites),
/// y —para el cálculo de tramos— el escrutinio sobre el array <c>ApuestaProbEscrutada[4782969]</c>.
///
/// El CÁLCULO de los tramos (escrutinio + ordenación + reparto) depende de la matriz de
/// valoraciones <c>v[14,3]</c> que el form legacy obtiene del UserControl <c>controlPorcentajes1</c>
/// (aún sin portar y fuera del alcance de esta tarea), por lo que esos comandos quedan como TODO
/// con la referencia exacta de línea. La misma decisión se tomó en el form hermano
/// <c>OrdenarPorProbabilidadFrm</c> (que usa el mismo array de 4.782.969 columnas), para no
/// inventar comportamiento. Lo que SÍ está cableado: la tabla de resultados (alimentada por la
/// lista de Tramos), la navegación con handoff a TramificarGraficas / DialogoGrabarTramos /
/// DialogoAnalisisMultipleDeTramos, la grabación de la jornada L.A.E. y leer/guardar límites.
/// </summary>
public partial class TramificarFormViewModel : ObservableObject
{
    // Precio de la apuesta (legacy PrecioApuesta, usado por MontaLinea para des-escalar importes).
    private const double PrecioApuesta = 0.5;

    // Universo de columnas de 14 partidos (legacy noColumnasIniciales = 3^14).
    private const int NoColumnasIniciales = 4782969;

    // Lista de tramos calculados (legacy ArrayList Tramos), origen del handoff a las gráficas.
    private readonly List<Tramo> _tramos = new();

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    /// <summary>Filas de la rejilla de resultados (legacy dgResultados sobre la lista de Tramos).</summary>
    public ObservableCollection<TramoFilaViewModel> Resultados { get; } = new();

    // ===== Definición del tramo (GroupBox "Definición tramo") =====

    // Columna inicial del rango a tramificar. Campo legacy txValMin (default "0").
    [ObservableProperty]
    private double _columnaInicial = 0;

    // Columna final del rango a tramificar. Campo legacy txValMax (default 4782969).
    [ObservableProperty]
    private double _columnaFinal = 4782969;

    // Columnas por tramo. Campo legacy txIntervalo (default "531441").
    [ObservableProperty]
    private double _columnasPorTramo = 531441;

    // Nº de tramos seleccionado. Campo legacy cmbNumTrams (ComboBox de potencias de 3).
    [ObservableProperty]
    private string _numTramos = "9";

    // Opciones del ComboBox de nº de tramos (regla anti-crash 3: ItemsSource desde el VM).
    public IReadOnlyList<string> OpcionesNumTramos { get; } = new[]
    {
        "9", "27", "81", "243", "729", "2187", "6561", "19683", "59049", "177147", "531441"
    };

    // ===== Datos L.A.E. (GroupBox "Datos L.A.E.") =====

    // Columna premiada (combinación ganadora). Campo legacy txColumna (default "1X1222X2X12121").
    [ObservableProperty]
    private string _columnaPremiada = "1X1222X2X12121";

    // Temporada (año de inicio). Campo legacy txTemporada.
    [ObservableProperty]
    private string _temporada = "2004";

    // Temporada de fin (solo lectura, txTemporada2 deshabilitado).
    [ObservableProperty]
    private string _temporadaFin = "2005";

    // Jornada. Campo legacy numJornada (NumericUpDown 1..70). NumberBox.Value es double.
    [ObservableProperty]
    private double _jornada = 7;

    // Recaudación de la jornada. Campo legacy txRecaudacion.
    [ObservableProperty]
    private string _recaudacion = "7768049";

    // Importes de premio por categoría (GroupBox "Premios"). Campos legacy tx14..tx10.
    [ObservableProperty]
    private string _premio14 = "1165207,35";

    [ObservableProperty]
    private string _premio13 = "29877,11";

    [ObservableProperty]
    private string _premio12 = "1753,51";

    [ObservableProperty]
    private string _premio11 = "165,38";

    [ObservableProperty]
    private string _premio10 = "22,79";

    // Bloqueo de cada categoría de premio (checkboxes chkey14..chkey10).
    // El form legacy guarda el estado de bloqueo en el array PremioBloqueado[5].
    [ObservableProperty]
    private bool _bloqueado14;

    [ObservableProperty]
    private bool _bloqueado13;

    [ObservableProperty]
    private bool _bloqueado12;

    [ObservableProperty]
    private bool _bloqueado11;

    [ObservableProperty]
    private bool _bloqueado10;

    // ===== Posiciones mínimas y máximas (GroupBox "Posiciones mínimas y máximas") =====
    // Para cada categoría (14..10) un mínimo y un máximo de posición admitida en el tramo.
    // Campos legacy txMin14/txMax14 ... txMin10/txMax10.
    [ObservableProperty]
    private string _min14 = string.Empty;

    [ObservableProperty]
    private string _max14 = string.Empty;

    [ObservableProperty]
    private string _min13 = string.Empty;

    [ObservableProperty]
    private string _max13 = string.Empty;

    [ObservableProperty]
    private string _min12 = string.Empty;

    [ObservableProperty]
    private string _max12 = string.Empty;

    [ObservableProperty]
    private string _min11 = string.Empty;

    [ObservableProperty]
    private string _max11 = string.Empty;

    [ObservableProperty]
    private string _min10 = string.Empty;

    [ObservableProperty]
    private string _max10 = string.Empty;

    // Nº de columnas que pasan el filtro (etiqueta lbColumnasAGrabar, default "0").
    [ObservableProperty]
    private string _columnasFiltro = "0";

    // ===== Ver columna (GroupBox "Ver columna") =====

    // Columna en la posición actual del tramo. Campo legacy TxColumnaEnPosicion.
    [ObservableProperty]
    private string _columnaEnPosicion = string.Empty;

    // Aciertos de la columna mostrada respecto a la premiada. Campo legacy txAciertos.
    [ObservableProperty]
    private string _aciertos = "0";

    // Valoración/probabilidad de la columna mostrada. Campo legacy txProbabilidad.
    [ObservableProperty]
    private string _probabilidad = "0";

    // Aciertos a buscar al saltar de columna. Campo legacy cmbAciertosABuscar (DropDownList).
    [ObservableProperty]
    private string _aciertosABuscar = "14";

    public IReadOnlyList<string> OpcionesAciertosABuscar { get; } = new[]
    {
        "14", "13", "12", "11", "10"
    };

    // ===== Otros parámetros =====

    // LN central (parámetro de valoración logarítmica). Campo legacy txLNCentral (default "0").
    [ObservableProperty]
    private string _lnCentral = "0";

    // Acumular resultados de jornadas distintas. Campo legacy chkAcumular (oculto por defecto).
    [ObservableProperty]
    private bool _acumular;

    // Mensaje de estado (StatusBar legacy, panel 4: "Faltan datos").
    [ObservableProperty]
    private string _estado = "Faltan datos";

    // ===== Acciones (botones del form legacy) =====

    [RelayCommand]
    private void Tramificar()
    {
        // TODO: Dominio legacy — TramificarForm.btTramificar_Click() / Tramifica() / Tramificar()
        //   (Free1X2/UI/TramificarForm.cs líneas 2092, 2276 y 2402).
        //   El reparto en tramos opera sobre el array Ap14T[4782969] (ApuestaProbEscrutada), que se
        //   rellena en Calcula14Triples()->Escrutar()/EncontrarDistantes1() (líneas 2362, 2303, 2563)
        //   a partir de la matriz de valoraciones v[14,3]. Esa matriz la entrega el UserControl
        //   controlPorcentajes1 (PonerValoracionEnVariables(), línea 2612), aún SIN portar y fuera
        //   del alcance de esta tarea. Sin valoraciones el escrutinio no tiene datos, por lo que
        //   transcribir aquí el bucle de construcción de Tramo (líneas 2420-2461) produciría tramos
        //   vacíos: se deja como TODO para no fabricar comportamiento (misma decisión que el form
        //   hermano OrdenarPorProbabilidadFrm, que comparte el mismo array de 4.782.969 columnas).
        //   Cuando se porte controlPorcentajes1, este comando debe: escrutar+ordenar Ap14T, repartir
        //   [ColumnaInicial..ColumnaFinal] en tramos de tamaño ColumnasPorTramo (o NumTramos tramos)
        //   con los importes Premio14..Premio10, poblar _tramos y volcarlos con RefrescarResultados().
        Estado = "Cálculo de tramos pendiente de portar (requiere controlPorcentajes1; ver TramificarForm.cs línea 2092).";
    }

    [RelayCommand]
    private void GrabarTramos()
    {
        // Legacy btGrabarTramos: TramificarForm.button1_Click() (línea 2674) abre DialogoGrabarTramosFrm
        //   y persiste a fichero las columnas de los tramos seleccionados en dgResultados.
        // Parte portable: navegar al diálogo de grabación de tramos (ya portado como página).
        // La SELECCIÓN de tramos y la grabación de sus columnas dependen de Ap14T (escrutinio), por lo
        // que la persistencia real queda ligada al TODO de Tramificar().
        Navegar?.Invoke(typeof(DialogoGrabarTramosFrmPage));
    }

    [RelayCommand]
    private void Filtrar()
    {
        // TODO: Dominio legacy — TramificarForm.btFiltrar_Click() (Free1X2/UI/TramificarForm.cs línea 3015):
        //   marca todas las columnas en el BitArray Bits, obtiene los extremos de posición con
        //   ObtenerExtremos() (línea 3055) a partir de Min14/Max14..Min10/Max10, abre
        //   DialogoFiltrarPorLimitesFrm (sin portar) y, con los extremos aceptados, elimina columnas
        //   por diferencias con EliminarColumnas() (línea 3089) sobre Ap14T/Bits, contando las que
        //   quedan en ColumnasFiltro (lbColumnasAGrabar). Depende del array escrutado Ap14T y del
        //   diálogo DialogoFiltrarPorLimitesFrm, ninguno disponible aquí.
        Estado = "Filtro por posiciones pendiente de portar (requiere Ap14T y DialogoFiltrarPorLimitesFrm; ver TramificarForm.cs línea 3015).";
    }

    [RelayCommand]
    private void GrabarFiltro()
    {
        // TODO: Dominio legacy — TramificarForm.button1_Click_1() (btGrabarFiltro,
        //   Free1X2/UI/TramificarForm.cs línea 3108): tras DialogoGuardar(), recorre el BitArray Bits
        //   y graba con ArchivoColumnasTexto.GuardarCols(ConvNumAColumna(nr)) las columnas que pasaron
        //   el filtro. Depende del Bits resultante de Filtrar() (Ap14T), no disponible aquí.
        Estado = "Grabar filtro pendiente de portar (requiere el resultado del filtro sobre Bits; ver TramificarForm.cs línea 3108).";
    }

    [RelayCommand]
    private void BuscarColumna()
    {
        // TODO: Dominio legacy — TramificarForm.button1_Click_2() (btBuscarColumna,
        //   Free1X2/UI/TramificarForm.cs línea 3161): convierte ColumnaEnPosicion a número con
        //   ConvertidorDeBases y la localiza recorriendo Ap14T[i].Columna para situar el cursor
        //   (numericUpDown1). Depende del array escrutado Ap14T, no disponible aquí.
        Estado = "Buscar columna pendiente de portar (requiere Ap14T escrutado; ver TramificarForm.cs línea 3161).";
    }

    [RelayCommand]
    private void ColumnaAnterior()
    {
        // TODO: Dominio legacy — TramificarForm.btAnterior_Click() (Free1X2/UI/TramificarForm.cs
        //   línea 2910): retrocede en Ap14T hasta la columna anterior con Aciertos >= NumAciertosABuscar
        //   y actualiza ColumnaEnPosicion/Probabilidad/Aciertos (numericUpDown1_ValueChanged, línea 2891).
        //   Depende del array escrutado Ap14T, no disponible aquí.
        Estado = "Navegación de columnas pendiente de portar (requiere Ap14T escrutado; ver TramificarForm.cs línea 2910).";
    }

    [RelayCommand]
    private void ColumnaSiguiente()
    {
        // TODO: Dominio legacy — TramificarForm.btSiguiente_Click() (Free1X2/UI/TramificarForm.cs
        //   línea 2924): avanza en Ap14T hasta la siguiente columna con Aciertos >= NumAciertosABuscar
        //   y actualiza ColumnaEnPosicion/Probabilidad/Aciertos. Depende del array escrutado Ap14T.
        Estado = "Navegación de columnas pendiente de portar (requiere Ap14T escrutado; ver TramificarForm.cs línea 2924).";
    }

    [RelayCommand]
    private async Task GuardarLimites()
    {
        // Legacy btGuardarLimites_Click (Free1X2/UI/TramificarForm.cs línea 3309): SaveFileDialog
        //   (*.txt) y vuelca, una por línea, txMin14/txMax14 ... txMin10/txMax10. Lógica pura de IO.
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Limites",
        };
        picker.FileTypeChoices.Add("Límites", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            // Mismo orden exacto que el legacy.
            using var sw = new StreamWriter(file.Path, append: false);
            sw.WriteLine(Min14);
            sw.WriteLine(Max14);
            sw.WriteLine(Min13);
            sw.WriteLine(Max13);
            sw.WriteLine(Min12);
            sw.WriteLine(Max12);
            sw.WriteLine(Min11);
            sw.WriteLine(Max11);
            sw.WriteLine(Min10);
            sw.WriteLine(Max10);
            Estado = "Límites guardados.";
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudieron guardar los límites: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task LeerLimites()
    {
        // Legacy btLeerLimites_Click (Free1X2/UI/TramificarForm.cs línea 3285): OpenFileDialog (*.txt)
        //   y lee, una por línea, txMin14/txMax14 ... txMin10/txMax10. Lógica pura de IO.
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            // Mismo orden exacto que el legacy; ReadLine() puede devolver null al final del fichero.
            using var sr = new StreamReader(file.Path);
            Min14 = sr.ReadLine() ?? string.Empty;
            Max14 = sr.ReadLine() ?? string.Empty;
            Min13 = sr.ReadLine() ?? string.Empty;
            Max13 = sr.ReadLine() ?? string.Empty;
            Min12 = sr.ReadLine() ?? string.Empty;
            Max12 = sr.ReadLine() ?? string.Empty;
            Min11 = sr.ReadLine() ?? string.Empty;
            Max11 = sr.ReadLine() ?? string.Empty;
            Min10 = sr.ReadLine() ?? string.Empty;
            Max10 = sr.ReadLine() ?? string.Empty;
            Estado = "Límites cargados.";
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudieron leer los límites: " + ex.Message);
        }
    }

    [RelayCommand]
    private void GuardarJornadaLae()
    {
        // Legacy menuItem7_Click (Free1X2/UI/TramificarForm.cs línea 2938): guarda los datos de premios
        //   de la jornada (L.A.E.) en Jornadas/InfoJornadasLAE.txt. Si la temporada+jornada ya existe,
        //   reemplaza su línea; si no, la añade. Se persiste con ArchivoColumnasTexto.GuardarColsComa.
        //   Lógica de dominio/IO portable tal cual (réplica de MontaLinea, línea 2982).
        try
        {
            string jornadaAGuardar = ((int)Jornada).ToString().PadLeft(2, '0');
            string temporadaDeLaJornada = Temporada + "/" + TemporadaFin;

            string nombreFichero = Path.Combine(AppContext.BaseDirectory, "Jornadas", "InfoJornadasLAE.txt");

            var jornadas = new List<string>();
            bool jornadaYaExiste = false;

            if (File.Exists(nombreFichero))
            {
                IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(nombreFichero);
                var linea = new StringBuilder();
                while (comBaseCols.SiguienteColumna())
                {
                    linea.Clear();
                    linea.Append(comBaseCols.LeeColumnaSinComas());
                    string[] valorsJornada = linea.ToString().Split((char)9);

                    if (valorsJornada.Length > 2 &&
                        valorsJornada[1] == temporadaDeLaJornada && valorsJornada[2] == jornadaAGuardar)
                    {
                        jornadaYaExiste = true;
                        linea.Clear();
                        linea.Append(MontaLinea(temporadaDeLaJornada, jornadaAGuardar));
                    }
                    jornadas.Add(linea.ToString());
                }
                comBaseCols.Cerrar();
            }

            if (!jornadaYaExiste)
            {
                jornadas.Add(MontaLinea(temporadaDeLaJornada, jornadaAGuardar));
            }

            Directory.CreateDirectory(Path.GetDirectoryName(nombreFichero)!);
            IArchivoColumnas comCols = new ArchivoColumnasTexto(nombreFichero);
            foreach (string str in jornadas)
            {
                comCols.GuardarColsComa(str);
            }
            comCols.Cerrar();
            Estado = "Datos de la jornada L.A.E. guardados.";
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo guardar la jornada L.A.E.: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Legacy btCancelar_Click (Free1X2/UI/TramificarForm.cs línea 2650): cierra el formulario.
        // En WinUI el cierre/navegación atrás es responsabilidad del host (Page.Frame.GoBack()).
        Volver?.Invoke();
    }

    /// <summary>Cierra/vuelve atrás (la cablea la página con Frame.GoBack()).</summary>
    public Action? Volver { get; set; }

    [RelayCommand]
    private void VerGraficas()
    {
        // Legacy menuItem10_Click_1 (Free1X2/UI/TramificarForm.cs línea 3457): si hay Tramos,
        //   abre TramificarGraficasFrm pasándole la lista de Tramos (handoff por ctor).
        // Handoff WinUI: el visor lee la lista por la propiedad estática TramosAnalizados.
        if (_tramos.Count == 0)
        {
            Estado = "No hay tramos calculados que representar.";
            return;
        }
        TramificarGraficasFrmViewModel.TramosAnalizados = _tramos.AsReadOnly();
        Navegar?.Invoke(typeof(TramificarGraficasFrmPage));
    }

    [RelayCommand]
    private void AnalisisMultiple()
    {
        // Legacy menuItem13_Click (Free1X2/UI/TramificarForm.cs línea 3243): abre
        //   DialogoAnalisisMultipleDeTramosFrm para definir el análisis múltiple de jornadas/ficheros.
        Navegar?.Invoke(typeof(DialogoAnalisisMultipleDeTramosFrmPage));
    }

    // ===== Lógica de dominio portable (réplica de rutinas del form legacy) =====

    /// <summary>
    /// Vuelca la lista de Tramos calculados a la rejilla de resultados (legacy GridDataBind() sobre
    /// dgResultados). Lo invocará el comando Tramificar una vez se porte el escrutinio.
    /// </summary>
    private void RefrescarResultados()
    {
        Resultados.Clear();
        foreach (var tr in _tramos)
        {
            Resultados.Add(new TramoFilaViewModel(tr));
        }
    }

    /// <summary>
    /// Compone la línea TAB-separada de la jornada L.A.E. (réplica exacta de
    /// TramificarForm.MontaLinea, Free1X2/UI/TramificarForm.cs línea 2982): columna premiada,
    /// temporada, jornada y los importes des-escalados por PrecioApuesta, con coma decimal.
    /// </summary>
    private string MontaLinea(string temporadaDeLaJornadaAGuardar, string jornadaAGuardar)
    {
        const char sep = (char)9;
        var ci = CultureInfo.CurrentCulture;

        string recaudacionTxt = string.IsNullOrEmpty(Recaudacion) ? "14000000" : Recaudacion;
        double recaudacion = ToDouble(recaudacionTxt) / PrecioApuesta;
        double paraEl14 = ToDouble(Premio14) / PrecioApuesta;
        double paraEl13 = ToDouble(Premio13) / PrecioApuesta;
        double paraEl12 = ToDouble(Premio12) / PrecioApuesta;
        double paraEl11 = ToDouble(Premio11) / PrecioApuesta;
        double paraEl10 = ToDouble(Premio10) / PrecioApuesta;

        var linea = new StringBuilder();
        linea.Append(ColumnaPremiada);
        linea.Append(sep);
        linea.Append(temporadaDeLaJornadaAGuardar);
        linea.Append(sep);
        linea.Append(jornadaAGuardar);
        linea.Append(sep);
        linea.Append(recaudacion.ToString(ci).Replace(".", ","));
        linea.Append(sep);
        linea.Append(paraEl14.ToString(ci).Replace(".", ","));
        linea.Append(sep);
        linea.Append(paraEl13.ToString(ci).Replace(".", ","));
        linea.Append(sep);
        linea.Append(paraEl12.ToString(ci).Replace(".", ","));
        linea.Append(sep);
        linea.Append(paraEl11.ToString(ci).Replace(".", ","));
        linea.Append(sep);
        linea.Append(paraEl10.ToString(ci).Replace(".", ","));
        return linea.ToString();
    }

    // Convierte un importe de texto (coma o punto decimal) a double sin reventar por la cultura.
    private static double ToDouble(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return 0;
        if (double.TryParse(valor, NumberStyles.Any, CultureInfo.CurrentCulture, out double r)) return r;
        if (double.TryParse(valor.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out r)) return r;
        return 0;
    }
}
