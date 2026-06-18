// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Analisis;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una agrupación mostrada en la lista de "Paso Fijo" o "Solapadas".
/// Legacy: cada fila producida por VisorAnalisisColumnasAbdonFrm.ObtenTextoAgrupaciones()
/// y pintada por el UserControl CtrlAgrupacion.
/// </summary>
public partial class VisorAbdonAgrupacion : ObservableObject
{
    /// <summary>Descripción textual completa de la agrupación (p. ej. "3-5+2+1").</summary>
    public string Texto { get; }

    /// <summary>Número/rango de columnas de la agrupación (legacy: columna "Número" = valores[0]).</summary>
    public string Numero { get; }

    /// <summary>Nº de elementos de la agrupación (legacy: columna "Elementos" = valores[1]).</summary>
    public string Elementos { get; }

    /// <summary>Nº de aciertos de la agrupación (legacy: columna "Aciertos" = valores[2]).</summary>
    public string Aciertos { get; }

    public VisorAbdonAgrupacion(string texto)
    {
        Texto = texto;
        // Réplica de CtrlAgrupacion.LlenarAgrupaciones(): el string se parte por '+' en
        // Número / Elementos / Aciertos. Se acota por longitud para no fabricar columnas.
        string[] partes = (texto ?? string.Empty).Split('+');
        Numero = partes.Length > 0 ? partes[0] : string.Empty;
        Elementos = partes.Length > 1 ? partes[1] : string.Empty;
        Aciertos = partes.Length > 2 ? partes[2] : string.Empty;
    }
}

/// <summary>
/// ViewModel de la pantalla "EstuCol - Visor de Informe" (legacy: VisorAnalisisColumnasAbdonFrm).
/// Muestra el análisis ABDON de las columnas generadas frente a las columnas ganadoras del
/// archivo, con dos modos (Global / Individual): rango de aciertos, agrupaciones paso fijo y
/// solapadas, escaleras (asc/desc/totales), sandwichs y suma total de aciertos. Permite además
/// generar un fichero de condición (filtro de columnas probables).
///
/// Cableado al motor real: consume el handoff estático EstucolFrmViewModel.UltimoInforme
/// (los tres argumentos del ctor legacy: informePorCols, informePorGans, columnas) y replica
/// 1:1 MostrarInformePorCols/MostrarInformePorGans, las navegaciones y GenerarCondicion.
/// </summary>
public partial class VisorAnalisisColumnasAbdonFrmViewModel : ObservableObject
{
    // ------- Datos de dominio (legacy: campos del form) -------

    private readonly List<InformeColumnasABDON> _informePorColumnas;
    private readonly List<InformeColumnasABDON> _informePorGanadoras;
    private readonly List<long> _columnas;

    private int _noColumna;          // legacy: noColumna
    private int _noColumnaGanadora;  // legacy: noColumnaGanadora

    // Contenedores de agrupaciones (legacy: List<int>[,] de tamaño [Columnas.Count+1, 15]).
    private List<int>[,] _contenedorAgrupacionesPasoFijoGlobal;
    private List<int>[,] _contenedorAgrupacionesSolapadasGlobal;
    private List<int>[,] _contenedorAgrupacionesPasoFijo;
    private List<int>[,] _contenedorAgrupacionesSolapadas;

    // ------- Estado de modo y navegación -------

    /// <summary>true = modo Global (rangos sobre todas las ganadoras); false = modo Individual.</summary>
    [ObservableProperty]
    private bool _modoGlobal = true;

    /// <summary>Etiqueta del botón de cambio de modo (legacy: btnModo.Text).</summary>
    public string TextoBotonModo => ModoGlobal ? "Cambiar a modo individual" : "Cambiar a modo global";

    /// <summary>Posición de la columna generada actual: "n / total" (legacy: lblColumna).</summary>
    [ObservableProperty]
    private string _columnaTexto = "- / -";

    /// <summary>Posición de la columna ganadora actual: "n / total" (legacy: lblGanadora).</summary>
    [ObservableProperty]
    private string _ganadoraTexto = "- / -";

    // Visibilidad por modo. Se bindea en CADA control hijo (no en el panel) para
    // respetar la regla anti-crash del XamlCompiler.
    /// <summary>Visible para los controles propios del modo Individual (ganadoras, suma opcional).</summary>
    public Visibility VisibilidadIndividual =>
        ModoGlobal ? Visibility.Collapsed : Visibility.Visible;

    /// <summary>Visible para los controles propios del modo Global (generar condición, aviso).</summary>
    public Visibility VisibilidadGlobal =>
        ModoGlobal ? Visibility.Visible : Visibility.Collapsed;

    // ------- Rango de aciertos de la columna generada -------

    /// <summary>Mínimo de aciertos de la columna actual (legacy: lblAciertosMin).</summary>
    [ObservableProperty]
    private string _aciertosMin = "-";

    /// <summary>Máximo de aciertos de la columna actual (legacy: lblAciertosMax).</summary>
    [ObservableProperty]
    private string _aciertosMax = "-";

    // ------- Agrupaciones Paso Fijo -------

    /// <summary>Filtro: mostrar sólo agrupaciones de N elementos (0 = todas). Legacy: txtTipoAgrupacion.</summary>
    [ObservableProperty]
    private double _tipoAgrupacionPasoFijo;

    /// <summary>Lista de agrupaciones paso fijo (legacy: CtrlAgrupacion "CtrlAgrupacionesPasoFijo").</summary>
    public ObservableCollection<VisorAbdonAgrupacion> AgrupacionesPasoFijo { get; } = new();

    /// <summary>Resumen "Hay X agrupaciones" en paso fijo (legacy: lblHayPasoFijo).</summary>
    [ObservableProperty]
    private string _hayPasoFijo = "";

    /// <summary>Nº de elementos del último filtro paso fijo aplicado (legacy: lblNumElementosPasoFijo).</summary>
    [ObservableProperty]
    private string _numElementosPasoFijo = "";

    // ------- Agrupaciones Solapadas -------

    /// <summary>Filtro: mostrar sólo agrupaciones solapadas de N elementos. Legacy: txtTipoAgrupacionSolapada.</summary>
    [ObservableProperty]
    private double _tipoAgrupacionSolapada;

    /// <summary>Lista de agrupaciones solapadas (legacy: CtrlAgrupacion "CtrlAgrupacionesSolapadas").</summary>
    public ObservableCollection<VisorAbdonAgrupacion> AgrupacionesSolapadas { get; } = new();

    /// <summary>Resumen "Hay X agrupaciones" en solapadas (legacy: lblHaySolapadas).</summary>
    [ObservableProperty]
    private string _haySolapadas = "";

    /// <summary>Nº de elementos del último filtro solapado aplicado (legacy: lblNumElementosSolapadas).</summary>
    [ObservableProperty]
    private string _numElementosSolapadas = "";

    // ------- Suma de aciertos -------

    /// <summary>Suma total de la serie de aciertos (valor o rango). Legacy: lblSumaTotalAciertos.</summary>
    [ObservableProperty]
    private string _sumaTotalAciertos = "-";

    /// <summary>Columnas a sumar de forma opcional (modo individual). Legacy: txtSumaAciertosOpcional.</summary>
    [ObservableProperty]
    private string _sumaAciertosOpcionalEntrada = "";

    /// <summary>Resultado de la suma opcional de aciertos (legacy: lblSumaOpcional).</summary>
    [ObservableProperty]
    private string _sumaOpcional = "-";

    // ------- Escaleras y sandwichs -------

    /// <summary>Escaleras ascendentes (valor o rango). Legacy: lblNumEscalerasAsc.</summary>
    [ObservableProperty]
    private string _escalerasAsc = "-";

    /// <summary>Escaleras descendentes (valor o rango). Legacy: lblNumEscalerasDesc.</summary>
    [ObservableProperty]
    private string _escalerasDesc = "-";

    /// <summary>Escaleras totales (valor o rango). Legacy: lblNumEscalerasTotales.</summary>
    [ObservableProperty]
    private string _escalerasTotales = "-";

    /// <summary>Número de sandwichs (valor o rango). Legacy: lblNumSandwichs.</summary>
    [ObservableProperty]
    private string _sandwichs = "-";

    public VisorAnalisisColumnasAbdonFrmViewModel()
    {
        // Handoff del productor (EstucolFrmViewModel.UltimoInforme): equivale a los argumentos
        // del ctor legacy VisorAnalisisColumnasAbdonFrm(informePorCols, informePorGans, columnas).
        // Se consume y se limpia: abrir el visor directamente desde el menú (sin haber generado un
        // informe en EstuCol) muestra el estado vacío en lugar de un informe antiguo.
        var informe = EstucolFrmViewModel.UltimoInforme;
        EstucolFrmViewModel.UltimoInforme = null;
        _informePorColumnas = informe?.porCols ?? new List<InformeColumnasABDON>();
        _informePorGanadoras = informe?.porGans ?? new List<InformeColumnasABDON>();
        _columnas = informe?.columnas ?? new List<long>();

        // Pre-init idéntico al ctor legacy (NO pasar arrays vacíos: el motor escribe en ellos).
        _contenedorAgrupacionesPasoFijoGlobal = new List<int>[_columnas.Count + 1, 15];
        _contenedorAgrupacionesSolapadasGlobal = new List<int>[_columnas.Count + 1, 15];
        _contenedorAgrupacionesPasoFijo = new List<int>[_columnas.Count + 1, 15];
        _contenedorAgrupacionesSolapadas = new List<int>[_columnas.Count + 1, 15];

        MostrarInformePorCols();
        MostrarInformePorGans();
    }

    partial void OnModoGlobalChanged(bool value)
    {
        OnPropertyChanged(nameof(TextoBotonModo));
        OnPropertyChanged(nameof(VisibilidadIndividual));
        OnPropertyChanged(nameof(VisibilidadGlobal));
    }

    // ===== Informe por columnas (legacy: MostrarInformePorCols / ObtenInfoPorCols) =====

    private void MostrarInformePorCols()
    {
        if (_informePorColumnas.Count > 0)
        {
            ColumnaTexto = (_noColumna + 1) + " / " + _informePorColumnas.Count;
            ObtenInfoPorCols();
        }
        else
        {
            ColumnaTexto = "- / -";
            AciertosMin = "-";
            AciertosMax = "-";
        }
    }

    private void ObtenInfoPorCols()
    {
        if (_informePorColumnas.Count > 0 && _noColumna < _informePorColumnas.Count)
        {
            InformeColumnasABDON inf = _informePorColumnas[_noColumna];
            AciertosMin = inf.MinimoAciertos.ToString();
            AciertosMax = inf.MaximoAciertos.ToString();
        }
        else
        {
            AciertosMin = "-";
            AciertosMax = "-";
        }
    }

    // ===== Informe por ganadoras (legacy: MostrarInformePorGans) =====

    private void MostrarInformePorGans()
    {
        if (_informePorGanadoras.Count > 0)
        {
            if (!ModoGlobal)
            {
                GanadoraTexto = (_noColumnaGanadora + 1) + " / " + _informePorGanadoras.Count;
                ObtenInfoPorGans();
            }
            else
            {
                ObtenInfoPorGansGlobal();
            }
        }
        else
        {
            GanadoraTexto = "- / -";
        }
    }

    private void ObtenInfoPorGans()
    {
        if (_informePorGanadoras.Count > 0 && _noColumnaGanadora < _informePorGanadoras.Count)
        {
            MostrarAgrupacionesPasoFijoInterno(0);
            MostrarAgrupacionesSolapadasInterno(0);
            MostrarSumaTotalDeAciertos();
            MostrarEscaleras();
            MostrarSandwichs();
        }
    }

    private void ObtenInfoPorGansGlobal()
    {
        if (_informePorGanadoras.Count > 0)
        {
            MostrarAgrupacionesPasoFijoGlobalInterno(0);
            MostrarAgrupacionesSolapadasGlobalInterno(0);
            MostrarEscalerasGlobal();
            MostrarSandwichsGlobal();
            MostrarSumaAciertosGlobal();
        }
    }

    private void MostrarSumaTotalDeAciertos()
    {
        if (_informePorGanadoras.Count > 0)
        {
            SumaTotalAciertos = _informePorGanadoras[_noColumnaGanadora].SumaTotalDeAciertos.ToString();
        }
    }

    private void MostrarSandwichs()
    {
        if (_informePorGanadoras.Count > 0)
        {
            Sandwichs = _informePorGanadoras[_noColumnaGanadora].Sandwichs.Count.ToString();
        }
    }

    private void MostrarEscaleras()
    {
        if (_informePorGanadoras.Count > 0)
        {
            InformeColumnasABDON inf = _informePorGanadoras[_noColumnaGanadora];
            EscalerasDesc = inf.NumeroDeEscalerasDescendentes.ToString();
            EscalerasAsc = inf.NumeroDeEscalerasAscendentes.ToString();
            EscalerasTotales = inf.NumeroDeEscaleras.ToString();
        }
    }

    private void MostrarEscalerasGlobal()
    {
        int minASC = 0, minDESC = 0, minTOT = 0, maxASC = 0, maxDESC = 0, maxTOT = 0;
        if (_informePorGanadoras.Count > 0)
        {
            minASC = maxASC = _informePorGanadoras[0].NumeroDeEscalerasAscendentes;
            minDESC = maxDESC = _informePorGanadoras[0].NumeroDeEscalerasDescendentes;
            minTOT = maxTOT = _informePorGanadoras[0].NumeroDeEscaleras;

            for (int i = 0; i < _informePorGanadoras.Count; i++)
            {
                InformeColumnasABDON inf = _informePorGanadoras[i];
                if (inf.NumeroDeEscalerasAscendentes < minASC) minASC = inf.NumeroDeEscalerasAscendentes;
                if (inf.NumeroDeEscalerasAscendentes > maxASC) maxASC = inf.NumeroDeEscalerasAscendentes;
                if (inf.NumeroDeEscalerasDescendentes < minDESC) minDESC = inf.NumeroDeEscalerasDescendentes;
                if (inf.NumeroDeEscalerasDescendentes > maxDESC) maxDESC = inf.NumeroDeEscalerasDescendentes;
                if (inf.NumeroDeEscaleras < minTOT) minTOT = inf.NumeroDeEscaleras;
                if (inf.NumeroDeEscaleras > maxTOT) maxTOT = inf.NumeroDeEscaleras;
            }
        }
        EscalerasAsc = minASC + "-" + maxASC;
        EscalerasDesc = minDESC + "-" + maxDESC;
        EscalerasTotales = minTOT + "-" + maxTOT;
    }

    private void MostrarSandwichsGlobal()
    {
        int min = 0, max = 0;
        if (_informePorGanadoras.Count > 0)
        {
            min = max = _informePorGanadoras[0].Sandwichs.Count;
            for (int i = 0; i < _informePorGanadoras.Count; i++)
            {
                int c = _informePorGanadoras[i].Sandwichs.Count;
                if (c > max) max = c;
                if (c < min) min = c;
            }
        }
        Sandwichs = min + "-" + max;
    }

    private void MostrarSumaAciertosGlobal()
    {
        var suma = new List<int>();
        if (_informePorGanadoras.Count > 0)
        {
            for (int i = 0; i < _informePorGanadoras.Count; i++)
            {
                suma.Add(_informePorGanadoras[i].SumaTotalDeAciertos);
            }
        }
        suma.Sort();
        SumaTotalAciertos = suma.Count > 0 ? suma[0] + "-" + suma[suma.Count - 1] : "-";
    }

    // ===== Agrupaciones individuales (legacy: MostrarAgrupacionesPasoFijo / Solapadas) =====

    private int MostrarAgrupacionesPasoFijoInterno(int tipo)
    {
        int noAgrup = ObtenerListaAgrupacionesPasoFijo(tipo);
        VolcarAgrupaciones(_contenedorAgrupacionesPasoFijo, AgrupacionesPasoFijo);
        return noAgrup;
    }

    private int MostrarAgrupacionesSolapadasInterno(int tipo)
    {
        int noAgrup = ObtenerListaAgrupacionesSolapadas(tipo);
        VolcarAgrupaciones(_contenedorAgrupacionesSolapadas, AgrupacionesSolapadas);
        return noAgrup;
    }

    private int[] MostrarAgrupacionesPasoFijoGlobalInterno(int tipo)
    {
        int[] rangos = ObtenerListaAgrupacionesPasoFijoGlobal(tipo);
        VolcarAgrupaciones(_contenedorAgrupacionesPasoFijoGlobal, AgrupacionesPasoFijo);
        return rangos;
    }

    private int[] MostrarAgrupacionesSolapadasGlobalInterno(int tipo)
    {
        int[] rangos = ObtenerListaAgrupacionesSolapadasGlobal(tipo);
        VolcarAgrupaciones(_contenedorAgrupacionesSolapadasGlobal, AgrupacionesSolapadas);
        return rangos;
    }

    private void VolcarAgrupaciones(List<int>[,] contenedor, ObservableCollection<VisorAbdonAgrupacion> destino)
    {
        destino.Clear();
        string[] lista = ObtenTextoAgrupaciones(contenedor);
        for (int i = 0; i < lista.Length; i++)
        {
            destino.Add(new VisorAbdonAgrupacion(lista[i]));
        }
    }

    private int ObtenerListaAgrupacionesPasoFijo(int tipo)
    {
        int noAgrup = 0;
        if (_informePorGanadoras.Count > 0)
        {
            InformeColumnasABDON inf = _informePorGanadoras[_noColumnaGanadora];
            for (int j = 0; j < inf.AgrupacionesPasoFijo.GetLength(0); j++)
            {
                if (j == tipo || tipo == 0)
                {
                    for (int k = 0; k < inf.AgrupacionesPasoFijo.GetLength(1); k++)
                    {
                        if (_contenedorAgrupacionesPasoFijo[j, k] == null)
                            _contenedorAgrupacionesPasoFijo[j, k] = new List<int>();
                        int num = inf.AgrupacionesPasoFijo[j, k];
                        if (!_contenedorAgrupacionesPasoFijo[j, k].Contains(num))
                        {
                            noAgrup += num;
                            _contenedorAgrupacionesPasoFijo[j, k].Add(num);
                        }
                    }
                }
            }
        }
        return noAgrup;
    }

    private int ObtenerListaAgrupacionesSolapadas(int tipo)
    {
        int noAgrup = 0;
        if (_informePorGanadoras.Count > 0)
        {
            InformeColumnasABDON inf = _informePorGanadoras[_noColumnaGanadora];
            for (int j = 0; j < inf.AgrupacionesSolapadas.GetLength(0); j++)
            {
                if (j == tipo || tipo == 0)
                {
                    for (int k = 0; k < inf.AgrupacionesSolapadas.GetLength(1); k++)
                    {
                        if (_contenedorAgrupacionesSolapadas[j, k] == null)
                            _contenedorAgrupacionesSolapadas[j, k] = new List<int>();
                        int num = inf.AgrupacionesSolapadas[j, k];
                        if (!_contenedorAgrupacionesSolapadas[j, k].Contains(num))
                        {
                            noAgrup += num;
                            _contenedorAgrupacionesSolapadas[j, k].Add(num);
                        }
                    }
                }
            }
        }
        return noAgrup;
    }

    private int[] ObtenerListaAgrupacionesPasoFijoGlobal(int tipo)
    {
        int noAgrup = 0;
        var valores = new List<int>();
        if (_informePorGanadoras.Count > 0)
        {
            for (int i = 0; i < _informePorGanadoras.Count; i++)
            {
                InformeColumnasABDON inf = _informePorGanadoras[i];
                for (int j = 1; j < inf.AgrupacionesPasoFijo.GetLength(0); j++)
                {
                    if (j == tipo || tipo == 0)
                    {
                        for (int k = 0; k < inf.AgrupacionesPasoFijo.GetLength(1); k++)
                        {
                            if (_contenedorAgrupacionesPasoFijoGlobal[j, k] == null)
                                _contenedorAgrupacionesPasoFijoGlobal[j, k] = new List<int>();
                            int num = inf.AgrupacionesPasoFijo[j, k];
                            noAgrup += num;
                            if (!_contenedorAgrupacionesPasoFijoGlobal[j, k].Contains(num))
                                _contenedorAgrupacionesPasoFijoGlobal[j, k].Add(num);
                        }
                    }
                }
                for (int j = inf.AgrupacionesPasoFijo.GetLength(0); j < _contenedorAgrupacionesPasoFijoGlobal.GetLength(0); j++)
                {
                    for (int k = 0; k < inf.AgrupacionesPasoFijo.GetLength(1); k++)
                    {
                        if (_contenedorAgrupacionesPasoFijoGlobal[j, k] == null)
                        {
                            _contenedorAgrupacionesPasoFijoGlobal[j, k] = new List<int>();
                            _contenedorAgrupacionesPasoFijoGlobal[j, k].Add(0);
                        }
                        else if (_contenedorAgrupacionesPasoFijoGlobal[j, k].IndexOf(0) == -1)
                        {
                            _contenedorAgrupacionesPasoFijoGlobal[j, k].Add(0);
                        }
                    }
                }
                valores.Add(noAgrup);
                noAgrup = 0;
            }
        }
        return UtilidadesEntradasValores.ObtenerRangos(valores);
    }

    private int[] ObtenerListaAgrupacionesSolapadasGlobal(int tipo)
    {
        int noAgrup = 0;
        var valores = new List<int>();
        if (_informePorGanadoras.Count > 0)
        {
            for (int i = 0; i < _informePorGanadoras.Count; i++)
            {
                InformeColumnasABDON inf = _informePorGanadoras[i];
                for (int j = 1; j < inf.AgrupacionesSolapadas.GetLength(0); j++)
                {
                    if (j == tipo || tipo == 0)
                    {
                        for (int k = 0; k < inf.AgrupacionesSolapadas.GetLength(1); k++)
                        {
                            if (_contenedorAgrupacionesSolapadasGlobal[j, k] == null)
                                _contenedorAgrupacionesSolapadasGlobal[j, k] = new List<int>();
                            int num = inf.AgrupacionesSolapadas[j, k];
                            noAgrup += num;
                            if (!_contenedorAgrupacionesSolapadasGlobal[j, k].Contains(num))
                                _contenedorAgrupacionesSolapadasGlobal[j, k].Add(num);
                        }
                    }
                }
                for (int j = inf.AgrupacionesSolapadas.GetLength(0); j < _contenedorAgrupacionesSolapadasGlobal.GetLength(0); j++)
                {
                    for (int k = 0; k < inf.AgrupacionesSolapadas.GetLength(1); k++)
                    {
                        if (_contenedorAgrupacionesSolapadasGlobal[j, k] == null)
                            _contenedorAgrupacionesSolapadasGlobal[j, k] = new List<int>();
                        if (!_contenedorAgrupacionesSolapadasGlobal[j, k].Contains(0))
                            _contenedorAgrupacionesSolapadasGlobal[j, k].Add(0);
                    }
                }
                valores.Add(noAgrup);
                noAgrup = 0;
            }
        }
        return UtilidadesEntradasValores.ObtenerRangos(valores);
    }

    private static string[] ObtenTextoAgrupaciones(List<int>[,] lista)
    {
        var temp = new List<string>();
        for (int i = 1; i < lista.GetLength(0); i++)
        {
            for (int j = 0; j < lista.GetLength(1); j++)
            {
                if (lista[i, j] != null)
                {
                    if (lista[i, j].Count > 1)
                    {
                        lista[i, j].Sort();
                        string fila = lista[i, j][0] + "-" + lista[i, j][lista[i, j].Count - 1] + "+" + i + "+" + j;
                        temp.Add(fila);
                    }
                    else
                    {
                        if (lista[i, j][0] != 0)
                        {
                            string fila = lista[i, j][0] + "+" + i + "+" + j;
                            temp.Add(fila);
                        }
                    }
                }
            }
        }
        return temp.ToArray();
    }

    // ===== Comandos =====

    /// <summary>
    /// Alterna entre modo Global e Individual y refresca el informe de ganadoras.
    /// Legacy: VisorAnalisisColumnasAbdonFrm.btnModo_Click + MostrarInformePorGans().
    /// </summary>
    [RelayCommand]
    private void CambiarModo()
    {
        ModoGlobal = !ModoGlobal;
        // Limpieza de filtros igual que el legacy al cambiar de modo.
        HayPasoFijo = "";
        HaySolapadas = "";
        NumElementosPasoFijo = "";
        NumElementosSolapadas = "";
        TipoAgrupacionPasoFijo = 0;
        TipoAgrupacionSolapada = 0;
        MostrarInformePorGans();
    }

    /// <summary>Avanza a la siguiente columna generada. Legacy: btnAdelante_Click.</summary>
    [RelayCommand]
    private void AdelanteColumna()
    {
        if (_noColumna < _informePorColumnas.Count - 1)
        {
            _noColumna++;
            MostrarInformePorCols();
        }
    }

    /// <summary>Retrocede a la columna generada anterior. Legacy: btnAtras_Click.</summary>
    [RelayCommand]
    private void AtrasColumna()
    {
        if (_noColumna > 0)
        {
            _noColumna--;
            MostrarInformePorCols();
        }
    }

    /// <summary>Avanza a la siguiente columna ganadora (modo individual). Legacy: btnAdelanteGanadoras_Click.</summary>
    [RelayCommand]
    private void AdelanteGanadora()
    {
        if (_noColumnaGanadora < _informePorGanadoras.Count - 1)
        {
            _noColumnaGanadora++;
            // Reinicio de contenedores individuales (legacy: new List<int>[Columnas.Count+1,15]).
            _contenedorAgrupacionesSolapadas = new List<int>[_columnas.Count + 1, 15];
            _contenedorAgrupacionesPasoFijo = new List<int>[_columnas.Count + 1, 15];
            MostrarInformePorGans();
        }
    }

    /// <summary>Retrocede a la columna ganadora anterior (modo individual). Legacy: btnAtrasGanadoras_Click.</summary>
    [RelayCommand]
    private void AtrasGanadora()
    {
        if (_noColumnaGanadora > 0)
        {
            _noColumnaGanadora--;
            _contenedorAgrupacionesSolapadas = new List<int>[_columnas.Count + 1, 15];
            _contenedorAgrupacionesPasoFijo = new List<int>[_columnas.Count + 1, 15];
            MostrarInformePorGans();
        }
    }

    /// <summary>
    /// Calcula y muestra las agrupaciones paso fijo del tipo/elementos indicado.
    /// Legacy: btnMostrarAgrupaciones_Click -> MostrarAgrupacionesPasoFijo[Global].
    /// </summary>
    [RelayCommand]
    private void MostrarAgrupacionesPasoFijo()
    {
        int tipo = (int)TipoAgrupacionPasoFijo;
        if (!ModoGlobal)
        {
            _contenedorAgrupacionesPasoFijo = new List<int>[_columnas.Count + 1, 15];
            int noTotal = MostrarAgrupacionesPasoFijoInterno(tipo);
            NumElementosPasoFijo = tipo.ToString();
            HayPasoFijo = noTotal.ToString();
        }
        else
        {
            _contenedorAgrupacionesPasoFijoGlobal = new List<int>[_columnas.Count + 1, 15];
            int[] rangos = MostrarAgrupacionesPasoFijoGlobalInterno(tipo);
            NumElementosPasoFijo = tipo.ToString();
            HayPasoFijo = rangos.Length >= 2 ? rangos[0] + "-" + rangos[1] : "";
        }
    }

    /// <summary>
    /// Calcula y muestra las agrupaciones solapadas del tipo/elementos indicado.
    /// Legacy: btnMostrarAgrupacionesSolapadas_Click -> MostrarAgrupacionesSolapadas[Global].
    /// </summary>
    [RelayCommand]
    private void MostrarAgrupacionesSolapadas()
    {
        int tipo = (int)TipoAgrupacionSolapada;
        if (!ModoGlobal)
        {
            _contenedorAgrupacionesSolapadas = new List<int>[_columnas.Count + 1, 15];
            int noTotal = MostrarAgrupacionesSolapadasInterno(tipo);
            NumElementosSolapadas = tipo.ToString();
            HaySolapadas = noTotal.ToString();
        }
        else
        {
            _contenedorAgrupacionesSolapadasGlobal = new List<int>[_columnas.Count + 1, 15];
            int[] rangos = MostrarAgrupacionesSolapadasGlobalInterno(tipo);
            NumElementosSolapadas = tipo.ToString();
            HaySolapadas = rangos.Length >= 2 ? rangos[0] + "-" + rangos[1] : "";
        }
    }

    /// <summary>
    /// Suma los aciertos de las columnas indicadas (modo individual).
    /// Legacy: btnSumaOpcional_Click -> MostrarSumaOpcionalDeAciertos().
    /// </summary>
    [RelayCommand]
    private void CalcularSumaOpcional()
    {
        if (SumaAciertosOpcionalEntrada == null) return;
        List<int> columnas = UtilidadesEntradasValores.ObtenerListaFromTxt(SumaAciertosOpcionalEntrada);
        int suma = 0;
        if (_informePorGanadoras.Count > 0)
        {
            InformeColumnasABDON inf = _informePorGanadoras[_noColumnaGanadora];
            for (int i = 0; i < inf.SerieAciertos.Length; i++)
            {
                if (columnas.Contains(i)) suma += inf.SerieAciertos[i];
            }
        }
        SumaOpcional = suma.ToString();
    }

    /// <summary>
    /// Genera y guarda un fichero de condición (filtro de columnas probables) a partir de los
    /// valores globales mostrados. Legacy: btnGenerarCondicion_Click -> GenerarCondicion().
    /// </summary>
    [RelayCommand]
    private async Task GenerarCondicion()
    {
        if (_columnas.Count == 0 || _informePorColumnas.Count < _columnas.Count)
        {
            AppServices.MostrarError("No hay informe de columnas para generar la condición.");
            return;
        }

        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Condicion",
        };
        picker.FileTypeChoices.Add("Columnas probables", new List<string> { ".cps" });
        picker.FileTypeChoices.Add("Columnas probables (XML)", new List<string> { ".xml" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            // Construcción 1:1 de VisorAnalisisColumnasAbdonFrm.GenerarCondicion() (siempre valores globales).
            string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();
            var cols = new List<ColumnaProbable>();
            var filtro = new FiltroColProbables();

            for (int i = 0; i < _columnas.Count; i++)
            {
                int min = _informePorColumnas[i].MinimoAciertos;
                int max = _informePorColumnas[i].MaximoAciertos;

                string pronosticos = UtilColumnas.ObtenerStringFromLongApuestaMultiple(_columnas[i]);
                var cp = new ColumnaProbable();
                cp.Pronosticos = pronosticos.Split(',');
                cp.SetNoAciertos(UtilidadesEntradasValores.ObtenerTodosValores(min, max));
                cp.SetNoAciertosSeguidos(todosValores);
                cp.SetNoFallosSeguidos(todosValores);
                cp.SetPuntos("");

                filtro.ColProbables.Add(cp);
                cols.Add(cp);
            }

            var rel1 = new RelacionCP1();
            rel1.Columnas = "1-" + _columnas.Count;
            rel1.SumaAciertos = SumaTotalAciertos;
            rel1.Recorridos = "";
            rel1.CantidadCP = "";
            rel1.CuantosAC = "";
            filtro.RelacionesCP1.Relaciones.Add(rel1);

            var rel = new RelacionCP3();
            rel.ColumnasImplicadasString = "1-" + _columnas.Count;
            rel.Columnas = cols;
            rel.Concepto = "AC";
            rel.ConceptoString = "AC";
            int longitudEsc = cols.Count / 3;
            int longitudSandwichs = cols.Count / 4;

            rel.NumeroEscalerasASCPermitidas = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(EscalerasAsc, longitudEsc);
            rel.NumeroEscalerasASCPermitidasString = EscalerasAsc;
            rel.NumeroEscalerasDESCPermitidas = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(EscalerasDesc, longitudEsc);
            rel.NumeroEscalerasDESCPermitidasString = EscalerasDesc;
            rel.NumeroEscalerasTotalesPermitidas = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(EscalerasTotales, longitudEsc);
            rel.NumeroEscalerasTotalesPermitidasString = EscalerasTotales;
            rel.NumeroSandwichsPermitidos = UtilidadesEntradasValores.ObtenerBoolArrayFromTxt(Sandwichs, longitudSandwichs);
            rel.NumeroSandwichsPermitidosString = Sandwichs;

            rel.AgrupacionesPasoFijoPermitidasString = ObtenTextoAgrupaciones(_contenedorAgrupacionesPasoFijoGlobal);
            rel.AgrupacionesSolapadasPermitidasString = ObtenTextoAgrupaciones(_contenedorAgrupacionesSolapadasGlobal);

            filtro.RelacionesCP3.Relaciones.Add(rel);
            filtro.ContieneDatos = true;

            string ruta = file.Path;
            await Task.Run(() =>
            {
                var archComb = new ArchivoCondiciones();
                archComb.NombreArchivo = ruta;
                archComb.GuardaArchivo(filtro);
            });

            AppServices.MostrarInfo("Filtro guardado");
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo generar la condición: " + ex.Message);
        }
    }
}
