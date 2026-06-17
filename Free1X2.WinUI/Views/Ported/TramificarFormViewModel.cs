using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Analisis;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Controls;
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
/// <see cref="Porcentajes"/> (valoraciones neperianas) y el escrutinio sobre el array
/// <c>ApuestaProbEscrutada[4782969]</c>.
///
/// El cálculo de los tramos (escrutinio + ordenación + reparto) se transcribe literalmente del
/// form legacy operando sobre <c>Ap14T = ApuestaProbEscrutada[4782969]</c>. La matriz de
/// valoraciones <c>v[14,3]</c> que el form legacy obtenía del UserControl <c>controlPorcentajes1</c>
/// la entrega ahora <see cref="PorcentajesControl"/> a través de <see cref="Porcentajes"/>
/// (misma sustitución que el form hermano <c>OrdenarPorProbabilidadFrm</c>).
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

    /// <summary>
    /// Rejilla de porcentajes/valoraciones editable (sustituye al UserControl WinForms
    /// <c>controlPorcentajes1</c>). El motor obtiene la matriz <c>v[14,3]</c> con
    /// <see cref="PorcentajesHelper.AMatriz"/>.
    /// </summary>
    public ObservableCollection<FilaPorcentaje> Porcentajes { get; } =
        PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

    public TramificarFormViewModel()
    {
        // Equivalente a controlPorcentajes1_Modificado (legacy línea 3466): cualquier edición de las
        // valoraciones invalida el escrutinio previo para recalcular probabilidades en el siguiente
        // Tramificar.
        foreach (var fila in Porcentajes) fila.PropertyChanged += (_, _) => _escrutado = false;
    }

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

    // cmbNumTrams_SelectedIndexChanged (legacy línea 2819): al cambiar el nº de tramos recalcula
    // columnas/tramo = txValMax / Ntrams y desmarca Acumular.
    partial void OnNumTramosChanged(string value)
    {
        if (int.TryParse(value, out int ntrams) && ntrams > 0)
        {
            ColumnasPorTramo = Math.Floor(ColumnaFinal / ntrams);
            Acumular = false;
        }
    }

    // txValMax_TextChanged (legacy línea 2656): al cambiar la columna final recalcula columnas/tramo.
    partial void OnColumnaFinalChanged(double value)
    {
        if (int.TryParse(NumTramos, out int ntrams) && ntrams > 0)
        {
            ColumnasPorTramo = Math.Floor(value / ntrams);
        }
    }

    // ===== Datos L.A.E. (GroupBox "Datos L.A.E.") =====

    // Columna premiada (combinación ganadora). Campo legacy txColumna (default "1X1222X2X12121").
    [ObservableProperty]
    private string _columnaPremiada = "1X1222X2X12121";

    // txColumna_TextChanged_1 (legacy línea 2665): cualquier cambio de la combinación invalida el
    // escrutinio anterior.
    partial void OnColumnaPremiadaChanged(string value) => _escrutado = false;

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

    // cmbAciertosABuscar_SelectedIndexChanged (legacy línea 3430).
    partial void OnAciertosABuscarChanged(string value)
    {
        if (short.TryParse(value, out short n)) _numAciertosABuscar = n;
    }

    // ===== Otros parámetros =====

    // LN central (parámetro de valoración logarítmica). Campo legacy txLNCentral (default "0").
    [ObservableProperty]
    private string _lnCentral = "0";

    // txLNCentral_TextChanged (legacy línea 3425).
    partial void OnLnCentralChanged(string value)
    {
        if (value != "-" && TryParseEsp(value, out double ln)) _LN = ln;
    }

    // Acumular resultados de jornadas distintas. Campo legacy chkAcumular (oculto por defecto).
    [ObservableProperty]
    private bool _acumular;

    // Criterio de ordenación: por productos (logaritmos neperianos) o por sumas. Equivale a los
    // menús legacy mnuPorProductos/mnuPorSumas; el Designer marca mnuPorProductos.Checked = true
    // (línea 1168), así que el valor por defecto es "por productos".
    [ObservableProperty]
    private bool _ordenarPorProductos = true;

    // Mensaje de estado (StatusBar legacy, panel 4: "Faltan datos").
    [ObservableProperty]
    private string _estado = "Faltan datos";

    // ===== Estado del motor (campos de instancia del form legacy) =====
    private readonly int[] pot = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323 };
    private float[,] p = new float[14, 3];
    private double[,] Cr = new double[14, 3];
    private double[,] v = new double[14, 3];
    private double[] Premios = new double[5];
    private ApuestaProbEscrutada[] Ap14T = Array.Empty<ApuestaProbEscrutada>();
    private BitArray Bits = new BitArray(NoColumnasIniciales, false);
    private int[,] MaxiMin = new int[5, 2];
    private int noColumnasIniciales = NoColumnasIniciales;
    private double LimiteInferiorAbsoluto;
    private double LimiteSuperiorAbsoluto = 4782969;
    private double Incremento;
    private double ProbabilidadAcumulada;
    private int Profundidad;
    private double _LN;
    private int c;
    private bool _escrutado;
    private bool _afegirTram;
    private short _numAciertosABuscar = 14;
    // Posición del cursor de "Ver columna" (legacy numericUpDown1.Value, 1-based).
    private int _posicionActual = 1;

    // Construye Ap14T perezosamente (el array de 4.782.969 structs son ~57 MB; no se asigna hasta
    // el primer cálculo, igual que el form legacy lo tiene como campo pero solo se llena al escrutar).
    private void AsegurarAp14T()
    {
        if (Ap14T.Length != NoColumnasIniciales)
        {
            Ap14T = new ApuestaProbEscrutada[NoColumnasIniciales];
        }
    }

    // ===== Acciones (botones del form legacy) =====

    [RelayCommand]
    private async Task Tramificar()
    {
        // Legacy btTramificar_Click (línea 2092) -> Tramifica() (línea 2276). Aquí se ejecuta el
        // caso de jornada simple (mnuJornadaSimple), que es el único alcanzable sin el flujo de
        // análisis múltiple; el cálculo pesado va a un hilo de fondo.
        // OrdenaPorSumas legacy clampa txValMax a noColumnasIniciales; se hace aquí, en el hilo de UI,
        // para no disparar PropertyChanged de ColumnaFinal desde el hilo de fondo.
        if (ColumnaFinal > noColumnasIniciales) ColumnaFinal = noColumnasIniciales;

        Estado = "Calculando ...";
        try
        {
            await Task.Run(() => Tramifica());
            // Las actualizaciones enlazadas a la UI se hacen al volver al hilo de UI (post-await):
            // vuelca MaxiMin a Min/Max (PonerMaximosiMinimos), refresca la rejilla y sitúa el cursor.
            PonerMaximosiMinimos(false);
            RefrescarResultados();
            // Tras tramificar, el cursor de "Ver columna" se sitúa en la posición 1 (numericUpDown1.Value = 1).
            _posicionActual = 1;
            ActualizarColumnaEnPosicion();
            Estado = "Finalizado";
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudieron calcular los tramos: " + ex.Message);
            Estado = "Error en el cálculo de tramos.";
        }
    }

    // Tramifica() (legacy línea 2276): orquesta escrutinio + ordenación + reparto.
    private void Tramifica()
    {
        if (Acumular)
        {
            _escrutado = false;
            _afegirTram = true;
        }
        else
        {
            _afegirTram = false;
            InicializarMaximosiMinimos();
        }

        // Tramifica() (legacy línea 2291): por productos (mnuPorProductos, default) o por sumas.
        // El volcado de MaxiMin a los TextBox (PonerMaximosiMinimos) lo hace el llamador en el hilo
        // de UI, tras este cálculo, para no disparar PropertyChanged desde el hilo de fondo.
        if (OrdenarPorProductos)
        {
            OrdenaPorProductos();
        }
        else
        {
            OrdenaPorSumas();
        }
    }

    // OrdenaPorProductos() (legacy línea 2318): ordena por probabilidad (logaritmos neperianos).
    private void OrdenaPorProductos()
    {
        int Partido;
        float Prob = 0;

        // Leer Porcentajes (PonerValoracionEnVariables: v = controlPorcentajes1.Valores).
        v = PorcentajesHelper.AMatriz(Porcentajes);
        Porcentajes Pct = new Porcentajes(v);
        p = Pct.ValoresNeperianos();

        // probabilidad del 14 de la 1ª apuesta (11111111111111) y valores complementarios.
        for (Partido = 0; Partido < 14; Partido++)
        {
            Prob += p[Partido, 0];
            Cr[Partido, 1] = p[Partido, 1] - p[Partido, 0];
            Cr[Partido, 2] = p[Partido, 2] - p[Partido, 0];
        }

        Calcula14Triples(Prob);
    }

    // OrdenaPorSumas() (legacy línea 2340).
    private void OrdenaPorSumas()
    {
        int Partido;
        float Prob = 0;

        // Leer Porcentajes (PonerValoracionEnVariables: v = controlPorcentajes1.Valores).
        v = PorcentajesHelper.AMatriz(Porcentajes);

        for (Partido = 0; Partido < 14; Partido++)
        {
            p[Partido, 0] = (float)v[Partido, 0];
            p[Partido, 1] = (float)v[Partido, 1];
            p[Partido, 2] = (float)v[Partido, 2];
            Prob += p[Partido, 0];
            Cr[Partido, 1] = p[Partido, 1] - p[Partido, 0];
            Cr[Partido, 2] = p[Partido, 2] - p[Partido, 0];
        }
        Calcula14Triples(Prob);
    }

    // Calcula14Triples() (legacy línea 2362).
    private void Calcula14Triples(double Prob)
    {
        if (_escrutado == false)
        {
            ActualizarEstadoMotor("Escrutando ...");
            Escrutar();

            ActualizarEstadoMotor("Calculando probabilidades ...");
            Profundidad = 0;
            EncontrarDistantes1(Prob, 0, 0, 14);
            Ap14T[0].Probabilidad = Math.Abs(Prob - _LN);
            Ap14T[0].Columna = 0;

            ActualizarEstadoMotor("Ordenando apuestas ...");
            EliminaColumnas();
            ordena(0, 4782968);
        }

        ActualizarEstadoMotor("Tramificando apuestas ...");
        if (_afegirTram) TramificarAdd(); else TramificarTramos();

        ActualizarEstadoMotor("Finalizado");
    }

    // Escrutar() (legacy línea 2303): rellena Aciertos respecto a la combinación ganadora.
    private void Escrutar()
    {
        AsegurarAp14T();
        for (int i = 0; i < 4782969; i++)
        {
            Ap14T[i].Aciertos = 0;
            Ap14T[i].Columna = i;
            Ap14T[i].Probabilidad = 0;
        }
        ConvertidorDeBases col = new ConvertidorDeBases();
        int Num = col.ConvColumnaANumero(ColumnaPremiada);
        Ap14T[Num].Aciertos = 14;
        ColumnasADistancia1(Num, 0, 4);
        _escrutado = true;
    }

    // EliminaColumnas() (legacy línea 2390): en modo fichero penaliza las columnas marcadas en Bits.
    // En jornada simple (14 triples) Bits está todo a false, por lo que es un no-op; se mantiene la
    // condición exacta del legacy (solo actúa si mnuFichero.Checked, que aquí no se expone).
    private void EliminaColumnas()
    {
        // mnuFichero no está expuesto en la página portada: la rama no se ejecuta.
    }

    // ColumnasADistancia1() (legacy línea 2541).
    private void ColumnasADistancia1(int IndiceInicial, int PosicionInicial, int pProfundidad)
    {
        Profundidad++;
        for (int Partido = PosicionInicial; Partido < 14; Partido++)
        {
            int SigIni = ((IndiceInicial / pot[Partido]) % 3);
            for (int z = 0; z < 3; z++)
            {
                if (z == SigIni) continue;
                int Indice = IndiceInicial + pot[Partido] * (z - SigIni);
                Ap14T[Indice].Aciertos = (byte)(14 - Profundidad);

                if (Profundidad < pProfundidad)
                {
                    ColumnasADistancia1(Indice, Partido + 1, pProfundidad);
                }
            }
        }
        Profundidad--;
    }

    // EncontrarDistantes1() (legacy línea 2563).
    private void EncontrarDistantes1(double pProb, int IndiceInicial, int PosicionInicial, int pProfundidad)
    {
        Profundidad++;
        for (int Partido = PosicionInicial; Partido < 14; Partido++)
        {
            for (int z = 1; z < 3; z++)
            {
                int Indice = IndiceInicial + pot[Partido] * z;
                double Prob = pProb + Cr[Partido, z];
                Ap14T[Indice].Columna = Indice;
                Ap14T[Indice].Probabilidad = Math.Abs(Prob - _LN);

                if (Profundidad < pProfundidad)
                {
                    EncontrarDistantes1(Prob, Indice, Partido + 1, pProfundidad);
                }
            }
        }
        Profundidad--;
    }

    // ordena() (legacy línea 2585): quicksort por Probabilidad.
    private void ordena(int izq, int der)
    {
        int i = izq;
        int j = der;
        ApuestaProbEscrutada x = Ap14T[(izq + der) / 2];

        do
        {
            while (Ap14T[i].Probabilidad < x.Probabilidad && j <= der) i++;
            while (x.Probabilidad < Ap14T[j].Probabilidad && j > izq) j--;

            if (i <= j)
            {
                ApuestaProbEscrutada aux = Ap14T[i];
                Ap14T[i] = Ap14T[j];
                Ap14T[j] = aux;
                i++; j--;
            }
        } while (i <= j);
        if (izq < j) ordena(izq, j);
        if (i < der) ordena(i, der);
    }

    // Tramificar() (legacy línea 2402): reparte el array ordenado en tramos.
    private void TramificarTramos()
    {
        int nr;
        int nc;

        LimiteInferiorAbsoluto = ColumnaInicial;
        LimiteSuperiorAbsoluto = ColumnaFinal;
        Incremento = ColumnasPorTramo;
        Premios[0] = ToDouble(Premio14);
        Premios[1] = ToDouble(Premio13);
        Premios[2] = ToDouble(Premio12);
        Premios[3] = ToDouble(Premio11);
        Premios[4] = ToDouble(Premio10);
        _tramos.Clear();

        ProbabilidadAcumulada = 0;

        for (nr = Convert.ToInt32(LimiteInferiorAbsoluto); nr < Convert.ToInt32(LimiteSuperiorAbsoluto); nr += Convert.ToInt32(Incremento))
        {
            Tramo tr = new Tramo(Premios);
            c = 0;
            int c2 = 0;
            tr.ValorIzquierda = nr;
            int[] Aciertos = new int[5];

            for (nc = nr; nc < nr + Incremento; nc++)
            {
                if (nc > noColumnasIniciales - 1) break;
                ProbabilidadAcumulada = -Ap14T[nc].Probabilidad;

                if (!Bits[Ap14T[nc].Columna])
                {
                    c++;

                    if (Ap14T[nc].Aciertos > 9)
                    {
                        int indiceAciertos = 14 - Ap14T[nc].Aciertos;
                        Aciertos[indiceAciertos]++;
                        tr.ColumnasPremiadas++;

                        if (MaxiMin[indiceAciertos, 0] < nc) MaxiMin[indiceAciertos, 0] = nc;
                        if (MaxiMin[indiceAciertos, 1] > nc) MaxiMin[indiceAciertos, 1] = nc;
                    }
                }
                else
                {
                    c2++;
                }
            }
            tr.ProbAcumulada = ProbabilidadAcumulada;
            tr.NumColumnasTramo = c;
            tr.ValorDerecha += tr.ValorIzquierda + c + c2 - 1;
            tr.PonerAciertos(Aciertos);
            tr.NumeroDeTramo = _tramos.Count + 1;
            tr.CalculaTotalImportePremios();
            _tramos.Add(tr);
        }
    }

    // TramificarAdd() (legacy línea 2476): acumula sobre los tramos ya calculados (chkAcumular).
    private void TramificarAdd()
    {
        int nr;
        int nc;
        LimiteInferiorAbsoluto = ColumnaInicial;
        LimiteSuperiorAbsoluto = ColumnaFinal;
        Incremento = ColumnasPorTramo;
        Premios[0] = ToDouble(Premio14);
        Premios[1] = ToDouble(Premio13);
        Premios[2] = ToDouble(Premio12);
        Premios[3] = ToDouble(Premio11);
        Premios[4] = ToDouble(Premio10);
        ProbabilidadAcumulada = 0;
        int NumTramo = 0;
        Tramo[] Trams = _tramos.ToArray();
        _tramos.Clear();

        for (nr = Convert.ToInt32(LimiteInferiorAbsoluto); nr < Convert.ToInt32(LimiteSuperiorAbsoluto); nr += Convert.ToInt32(Incremento))
        {
            Tramo tr = new Tramo(Premios);
            c = 0;
            int c2 = 0;
            tr.ValorIzquierda = nr;
            int[] Aciertos = new int[5];

            for (nc = nr; nc < nr + Incremento; nc++)
            {
                if (nc > 4782968) break;
                if (!Bits[Ap14T[nc].Columna])
                {
                    ProbabilidadAcumulada = -Ap14T[nc].Probabilidad;
                    c++;

                    if (Ap14T[nc].Aciertos > 9)
                    {
                        int indiceAciertos = 14 - Ap14T[nc].Aciertos;
                        Aciertos[indiceAciertos]++;
                        if (MaxiMin[indiceAciertos, 0] < nc) MaxiMin[indiceAciertos, 0] = nc;
                        if (MaxiMin[indiceAciertos, 1] > nc) MaxiMin[indiceAciertos, 1] = nc;
                        tr.ColumnasPremiadas++;
                    }
                }
                else
                {
                    c2++;
                }
            }
            tr.ProbAcumulada = ProbabilidadAcumulada;
            tr.NumColumnasTramo = c;
            tr.ValorDerecha += tr.ValorIzquierda + c + c2 - 1;
            tr.PonerAciertos(Aciertos);
            tr.CalculaTotalImportePremios();
            if (NumTramo < Trams.Length) tr.AddTramo(Trams[NumTramo]);
            tr.NumeroDeTramo = _tramos.Count + 1;

            _tramos.Add(tr);

            NumTramo++;
        }
    }

    [RelayCommand]
    private void GrabarTramos()
    {
        // Legacy btGrabarTramos: TramificarForm.button1_Click() (línea 2674) abre DialogoGrabarTramosFrm
        //   y persiste a fichero las columnas de los tramos seleccionados en dgResultados.
        // Parte portable: navegar al diálogo de grabación de tramos (ya portado como página).
        // La selección concreta de tramos en una rejilla con multiselección no está expuesta en la
        // página portada; el handoff abre el diálogo igual que el menú legacy.
        Navegar?.Invoke(typeof(DialogoGrabarTramosFrmPage));
    }

    [RelayCommand]
    private async Task Filtrar()
    {
        // Legacy btFiltrar_Click (línea 3015): marca en Bits las columnas presentes en Ap14T,
        //   obtiene los extremos de posición con ObtenerExtremos() a partir de Min14/Max14..Min10/Max10
        //   y elimina columnas por diferencias con EliminarColumnas(), contando las que quedan.
        //
        // El form legacy abre además DialogoFiltrarPorLimitesFrm para revisar/editar los extremos y la
        // profundidad de diferencias por categoría. En la página portada se aplican los valores por
        // defecto del Designer de ese diálogo (los mismos que usa DialogoFiltrarPorLimitesFrmViewModel),
        // que es lo que el usuario obtenía aceptando el diálogo sin cambios. El cálculo del filtro
        // (EliminarColumnas sobre Ap14T/Bits) es idéntico al legacy.
        if (Ap14T.Length != NoColumnasIniciales)
        {
            Estado = "Primero hay que tramificar para escrutar las columnas.";
            return;
        }

        Estado = "Eliminando columnas ...";
        try
        {
            int total = await Task.Run(() => EjecutarFiltro());
            ColumnasFiltro = total.ToString(CultureInfo.CurrentCulture);
            Estado = "Finalizado";
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo aplicar el filtro: " + ex.Message);
            Estado = "Error al filtrar.";
        }
    }

    // Núcleo de btFiltrar_Click (legacy línea 3015) + aplicación de extremos.
    private int EjecutarFiltro()
    {
        int total = 0;

        Bits.SetAll(true);
        for (int x = 0; x < noColumnasIniciales; x++) { Bits[Ap14T[x].Columna] = false; }
        int[,] extremos = ObtenerExtremos();

        // Profundidad de diferencias por defecto (Designer de DialogoFiltrarPorLimitesFrm):
        // extremos[i,2] = eliminar-rango (0 = no), extremos[i,3] = profundidad de diferencias.
        AplicarDiferenciasPorDefecto(extremos);

        for (int i = 0; i < 10; i++)
        {
            if (extremos[i, 0] > 0) extremos[i, 0]--;
            for (int x = extremos[i, 0]; x < extremos[i, 1]; x++)
            {
                if (extremos[i, 2] == 0) Bits[Ap14T[x].Columna] = true;
                if (extremos[i, 3] > 0) EliminarColumnas(Ap14T[x].Columna, 0, (short)extremos[i, 3]);
            }
        }

        for (int nr = 0; nr < 4782969; nr++) { if (Bits[nr] == false) total++; }
        return total;
    }

    // Valores por defecto de la columna de diferencias del Designer de DialogoFiltrarPorLimitesFrm
    // (idénticos a DialogoFiltrarPorLimitesFrmViewModel.difsPorDefecto): difMin en [,2], difMax en [,3].
    private static void AplicarDiferenciasPorDefecto(int[,] extremos)
    {
        int[,] difsPorDefecto =
        {
            { 0, 4 }, { 1, 3 }, { 1, 2 }, { 1, 1 }, { 1, 0 },
            { 1, 0 }, { 1, 1 }, { 1, 2 }, { 0, 0 }, { 0, 0 },
        };
        for (int i = 0; i < 10; i++)
        {
            extremos[i, 2] = difsPorDefecto[i, 0];
            extremos[i, 3] = difsPorDefecto[i, 1];
        }
    }

    // ObtenerExtremos() (legacy línea 3055).
    private int[,] ObtenerExtremos()
    {
        int[,] extremos = new int[10, 4];
        extremos[0, 0] = 0;
        extremos[0, 1] = ToInt(Min10) - 1;

        extremos[1, 0] = ToInt(Min10) - 1;
        extremos[1, 1] = ToInt(Min11) - 1;

        extremos[2, 0] = ToInt(Min11) - 1;
        extremos[2, 1] = ToInt(Min12) - 1;

        extremos[3, 0] = ToInt(Min12) - 1;
        extremos[3, 1] = ToInt(Min13) - 1;

        extremos[4, 0] = ToInt(Min13) - 1;
        extremos[4, 1] = ToInt(Min14) - 1;

        extremos[5, 0] = ToInt(Max14);
        extremos[5, 1] = ToInt(Max13);

        extremos[6, 0] = ToInt(Max13);
        extremos[6, 1] = ToInt(Max12);

        extremos[7, 0] = ToInt(Max12);
        extremos[7, 1] = ToInt(Max11);

        extremos[8, 0] = ToInt(Max11);
        extremos[8, 1] = ToInt(Max10);

        extremos[9, 0] = ToInt(Max10);
        extremos[9, 1] = noColumnasIniciales - 1;
        return extremos;
    }

    // EliminarColumnas() (legacy línea 3089).
    private void EliminarColumnas(int IndiceInicial, short PosicionInicial, short pProfundidad)
    {
        Profundidad++;
        for (short Partido = PosicionInicial; Partido < 14; Partido++)
        {
            int SigIni = ((IndiceInicial / pot[Partido]) % 3);
            for (short z = 0; z < 3; z++)
            {
                if (z == SigIni) continue;
                int Indice = IndiceInicial + pot[Partido] * (z - SigIni);
                Bits[Indice] = true; // las marcadas a true son las que eliminamos
                if (Profundidad < pProfundidad)
                { EliminarColumnas(Indice, (short)(Partido + 1), pProfundidad); }
            }
        }
        Profundidad--;
    }

    [RelayCommand]
    private async Task GrabarFiltro()
    {
        // Legacy button1_Click_1 (btGrabarFiltro, línea 3108): tras DialogoGuardar(), recorre el
        //   BitArray Bits y graba con ArchivoColumnasTexto.GuardarCols(ConvNumAColumna(nr)) las
        //   columnas que pasaron el filtro (Bits[nr] == false), aplicando el paso.
        if (Ap14T.Length != NoColumnasIniciales)
        {
            Estado = "Primero hay que tramificar y filtrar.";
            return;
        }

        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Columnas",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        string ruta = file.Path;
        Estado = "Guardando columnas ...";
        try
        {
            int grabadas = await Task.Run(() =>
            {
                int total = 0;
                ConvertidorDeBases col = new ConvertidorDeBases();
                IArchivoColumnas comCols = new ArchivoColumnasTexto(ruta);
                for (int nr = 0; nr < 4782969; nr++)
                {
                    if (Bits[nr] == false)
                    {
                        // Paso = 1 (DialogoGrabarTramos por defecto): se graban todas las filtradas.
                        comCols.GuardarCols(col.ConvNumAColumna(nr));
                        total++;
                    }
                }
                comCols.Cerrar();
                return total;
            });
            Estado = "Grabadas " + grabadas + " columnas";
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudieron grabar las columnas: " + ex.Message);
            Estado = "Error al grabar el filtro.";
        }
    }

    [RelayCommand]
    private void BuscarColumna()
    {
        // Legacy button1_Click_2 (btBuscarColumna, línea 3161): convierte ColumnaEnPosicion a número
        //   con ConvertidorDeBases y la localiza recorriendo Ap14T[i].Columna para situar el cursor.
        if (Ap14T.Length != NoColumnasIniciales)
        {
            Estado = "Primero hay que tramificar.";
            return;
        }
        if (string.IsNullOrEmpty(ColumnaEnPosicion)) return;

        ConvertidorDeBases col = new ConvertidorDeBases();
        int Num = col.ConvColumnaANumero(ColumnaEnPosicion);
        for (int i = 0; i < noColumnasIniciales; i++)
        {
            if (Ap14T[i].Columna == Num)
            {
                _posicionActual = i + 1;
                ActualizarColumnaEnPosicion();
                break;
            }
        }
    }

    [RelayCommand]
    private void ColumnaAnterior()
    {
        // Legacy btAnterior_Click (línea 2910): retrocede en Ap14T hasta la columna anterior con
        //   Aciertos >= NumAciertosABuscar y mueve el cursor (numericUpDown1).
        if (Ap14T.Length != NoColumnasIniciales) { Estado = "Primero hay que tramificar."; return; }

        int Posicio = _posicionActual - 2;
        for (int i = Posicio; i > 0; i--)
        {
            if (Bits[Ap14T[i].Columna]) continue;
            if (Ap14T[i].Aciertos >= _numAciertosABuscar)
            {
                _posicionActual = i + 1;
                ActualizarColumnaEnPosicion();
                break;
            }
        }
    }

    [RelayCommand]
    private void ColumnaSiguiente()
    {
        // Legacy btSiguiente_Click (línea 2924): avanza en Ap14T hasta la siguiente columna con
        //   Aciertos >= NumAciertosABuscar y mueve el cursor.
        if (Ap14T.Length != NoColumnasIniciales) { Estado = "Primero hay que tramificar."; return; }

        int Posicio = _posicionActual;
        for (int i = Posicio; i < noColumnasIniciales; i++)
        {
            if (Bits[Ap14T[i].Columna]) continue;
            if (Ap14T[i].Aciertos >= _numAciertosABuscar)
            {
                _posicionActual = i + 1;
                ActualizarColumnaEnPosicion();
                break;
            }
        }
    }

    // numericUpDown1_ValueChanged (legacy línea 2891): refleja la columna del cursor, su valoración y
    // los aciertos respecto a la combinación premiada (TxColumnaEnPosicion_TextChanged, línea 3189).
    private void ActualizarColumnaEnPosicion()
    {
        ConvertidorDeBases col = new ConvertidorDeBases();
        int Posicio = _posicionActual - 1;
        if (Posicio >= 0 && Posicio < noColumnasIniciales && Ap14T.Length == NoColumnasIniciales)
        {
            string columna = col.ConvNumAColumna(Ap14T[Posicio].Columna);
            ColumnaEnPosicion = columna;
            Probabilidad = (-Ap14T[Posicio].Probabilidad).ToString(CultureInfo.CurrentCulture);

            int aciertos = 0;
            for (int i = 0; i < columna.Length && i < ColumnaPremiada.Length; i++)
            {
                if (ColumnaPremiada[i] == columna[i]) aciertos++;
            }
            Aciertos = aciertos.ToString(CultureInfo.CurrentCulture);
        }
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

    // InicializarMaximosiMinimos() (legacy línea 2882).
    private void InicializarMaximosiMinimos()
    {
        for (int i = 0; i < 5; i++)
        {
            MaxiMin[i, 0] = 0;
            MaxiMin[i, 1] = noColumnasIniciales;
        }
    }

    // PonerMaximosiMinimos(false) (legacy línea 2852): vuelca MaxiMin a Min/Max (+1, en absoluto).
    private void PonerMaximosiMinimos(bool enTantoPorCiento)
    {
        var ci = CultureInfo.CurrentCulture;
        if (enTantoPorCiento)
        {
            Min14 = Math.Round((double)MaxiMin[0, 1] * 100 / noColumnasIniciales, 3) + "%";
            Max14 = Math.Round((double)MaxiMin[0, 0] * 100 / noColumnasIniciales, 3) + "%";
            Min13 = Math.Round((double)MaxiMin[1, 1] * 100 / noColumnasIniciales, 3) + "%";
            Max13 = Math.Round((double)MaxiMin[1, 0] * 100 / noColumnasIniciales, 3) + "%";
            Min12 = Math.Round((double)MaxiMin[2, 1] * 100 / noColumnasIniciales, 3) + "%";
            Max12 = Math.Round((double)MaxiMin[2, 0] * 100 / noColumnasIniciales, 3) + "%";
            Min11 = Math.Round((double)MaxiMin[3, 1] * 100 / noColumnasIniciales, 3) + "%";
            Max11 = Math.Round((double)MaxiMin[3, 0] * 100 / noColumnasIniciales, 3) + "%";
            Min10 = Math.Round((double)MaxiMin[4, 1] * 100 / noColumnasIniciales, 3) + "%";
            Max10 = Math.Round((double)MaxiMin[4, 0] * 100 / noColumnasIniciales, 3) + "%";
        }
        else
        {
            Min14 = (1 + MaxiMin[0, 1]).ToString(ci);
            Max14 = (1 + MaxiMin[0, 0]).ToString(ci);
            Min13 = (1 + MaxiMin[1, 1]).ToString(ci);
            Max13 = (1 + MaxiMin[1, 0]).ToString(ci);
            Min12 = (1 + MaxiMin[2, 1]).ToString(ci);
            Max12 = (1 + MaxiMin[2, 0]).ToString(ci);
            Min11 = (1 + MaxiMin[3, 1]).ToString(ci);
            Max11 = (1 + MaxiMin[3, 0]).ToString(ci);
            Min10 = (1 + MaxiMin[4, 1]).ToString(ci);
            Max10 = (1 + MaxiMin[4, 0]).ToString(ci);
        }
    }

    /// <summary>
    /// Vuelca la lista de Tramos calculados a la rejilla de resultados (legacy GridDataBind() sobre
    /// dgResultados).
    /// </summary>
    private void RefrescarResultados()
    {
        Resultados.Clear();
        foreach (var tr in _tramos)
        {
            Resultados.Add(new TramoFilaViewModel(tr));
        }
    }

    // statusBarPanel4.Text legacy -> Estado marshalado al hilo de UI.
    private void ActualizarEstadoMotor(string texto)
    {
        var disp = AppServices.UiDispatcher;
        if (disp is null) { Estado = texto; return; }
        disp.TryEnqueue(() => Estado = texto);
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

    // Variante para parseo "esp" (acepta coma o punto) usado por LN central.
    private static bool TryParseEsp(string texto, out double valor)
    {
        valor = 0;
        if (string.IsNullOrWhiteSpace(texto)) return false;
        if (double.TryParse(texto, NumberStyles.Any, CultureInfo.CurrentCulture, out valor)) return true;
        return double.TryParse(texto.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out valor);
    }

    // Convierte un entero de texto (Convert.ToInt32 legacy) tolerando vacío.
    private static int ToInt(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return 0;
        string t = valor.Trim().Replace("%", "");
        if (int.TryParse(t, NumberStyles.Any, CultureInfo.CurrentCulture, out int r)) return r;
        if (double.TryParse(t, NumberStyles.Any, CultureInfo.CurrentCulture, out double d)) return (int)d;
        return 0;
    }
}
