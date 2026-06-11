using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Representa una fila del grid "Filtros" del WinForms legacy DiFiltros.
/// Columnas legacy: N (orden), Path (ruta completa, oculta), F (nombre archivo),
/// A (activo/bool), C (cols. contadas), D (difs, máscara "1111" de niveles 0-3),
/// M (min. admitidas), R (adm. = resultado), 14 (acierta 14), 13 (aciertos 13).
/// </summary>
public partial class DiFiltroFilaViewModel : ObservableObject
{
    [ObservableProperty]
    private int _numero;

    // Regla anti-crash #2: no bindear int directo a Text.
    public string NumeroTexto => Numero.ToString();

    partial void OnNumeroChanged(int value) => OnPropertyChanged(nameof(NumeroTexto));

    // Path legacy: ruta completa del archivo de columnas (oculta en el grid, ancho 0).
    [ObservableProperty]
    private string _ruta = string.Empty;

    // F legacy: nombre del archivo de columnas.
    [ObservableProperty]
    private string _nombre = string.Empty;

    // A legacy: filtro activo/inactivo (bool, editable haciendo clic en la fila).
    [ObservableProperty]
    private bool _activo = true;

    // C legacy: nº de columnas leídas del archivo tras el proceso.
    [ObservableProperty]
    private int _columnas;

    public string ColumnasTexto => Columnas.ToString();

    partial void OnColumnasChanged(int value) => OnPropertyChanged(nameof(ColumnasTexto));

    // D legacy: máscara de "difs" "1111" — habilita los niveles de diferencia 0/1/2/3
    // (tst0..tst3 en DiFiltros.Calcular). Editable; por defecto "1111".
    [ObservableProperty]
    private string _difs = "1111";

    // M legacy: nº mínimo de columnas admitidas para considerar válida (mincol). Editable.
    // NumberBox.Value es double (regla anti-crash #7).
    [ObservableProperty]
    private double _minimo = 1;

    // R legacy: resultado (columnas admitidas) tras Calcular(). Solo lectura.
    [ObservableProperty]
    private int _admitidas;

    public string AdmitidasTexto => Admitidas.ToString();

    partial void OnAdmitidasChanged(int value) => OnPropertyChanged(nameof(AdmitidasTexto));

    // "14" legacy: 1 si la columna ganadora (prm14) queda admitida, 0 si no.
    [ObservableProperty]
    private int _acierta14;

    public string Acierta14Texto => Acierta14.ToString();

    partial void OnAcierta14Changed(int value) => OnPropertyChanged(nameof(Acierta14Texto));

    // "13" legacy: nº de las 28 variantes a 13 (prm13) que quedan admitidas.
    [ObservableProperty]
    private int _aciertos13;

    public string Aciertos13Texto => Aciertos13.ToString();

    partial void OnAciertos13Changed(int value) => OnPropertyChanged(nameof(Aciertos13Texto));
}

/// <summary>
/// ViewModel portado del WinForms DiFiltros ("Diferencias entre filtros").
/// Carga una lista de archivos de columnas (filtros), toma el primero como base
/// y, para cada filtro siguiente, descarta de las "válidas" las columnas que no
/// estén dentro de una distancia (difs 0-3) configurable respecto a sus columnas,
/// dejando las "diferencias". Incluye un análisis de resultados sobre una columna
/// ganadora (14) y sus 28 variantes a 13.
/// Toda la lógica de dominio (lectura/escritura de archivos, cálculo de diferencias,
/// escrutinio) está marcada como TODO citando la clase legacy.
/// </summary>
public partial class DiFiltrosViewModel : ObservableObject
{
    // dsDatos "Filtros" legacy -> colección de filas.
    public ObservableCollection<DiFiltroFilaViewModel> Filtros { get; } = new();

    // ckMD legacy: "Activa / Desactiva" todas las filas a la vez (Checked por defecto).
    [ObservableProperty]
    private bool _activarTodas = true;

    // lTime legacy: tiempo transcurrido del proceso (formateado, solo lectura).
    [ObservableProperty]
    private string _tiempo = string.Empty;

    // lFileR legacy: nombre del archivo grabado con las columnas resultantes.
    [ObservableProperty]
    private string _archivoSalida = string.Empty;

    // bIniciar legacy: indica si el proceso está en curso (deshabilita acciones).
    [ObservableProperty]
    private bool _procesando;

    public bool HayFiltros => Filtros.Count > 0;

    // --- Sección "Analisis resultados" (groupBox3) ---

    // lFGR legacy: nombre del fichero de columnas ganadoras cargado.
    [ObservableProperty]
    private string _ficheroGanadoras = "Fichero ganadoras";

    // tbCG legacy: columna ganadora actual (texto de 14 signos, MaxLength=14).
    [ObservableProperty]
    private string _columnaGanadora = "Col. Ganadora";

    // lbCGR legacy: índice (1-based) de la columna ganadora mostrada (nrfCGR).
    [ObservableProperty]
    private int _indiceGanadora;

    public string IndiceGanadoraTexto => IndiceGanadora.ToString();

    partial void OnIndiceGanadoraChanged(int value) => OnPropertyChanged(nameof(IndiceGanadoraTexto));

    // bAnalizar legacy: habilitado solo tras cargar un fichero de ganadoras válido.
    [ObservableProperty]
    private bool _puedeAnalizar;

    public DiFiltrosViewModel()
    {
        Filtros.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HayFiltros));
    }

    // btnCargarFiltro -> CargarFiltro() legacy.
    [RelayCommand]
    private void CargarFiltro()
    {
        // TODO[dominio]: portar DiFiltros.CargarFiltro():
        //   OpenFileDialog multiselección (*.txt), directorio Application.StartupPath.
        //   Por cada archivo añade una fila: N=orden, Path=ruta, F=nombre,
        //   A=true, C=0, D="1111", M=1. En WinUI usar Windows.Storage.Pickers.FileOpenPicker.
    }

    // bCargaLista -> CargarLista() legacy.
    [RelayCommand]
    private void CargarLista()
    {
        // TODO[dominio]: portar DiFiltros.CargarLista():
        //   limpia la tabla, abre un .lst (líneas "Path;F;A;D;M" separadas por ';'),
        //   reconstruye una fila por línea (R/14/13 a 0).
    }

    // bSalvaLista -> SalvarLista() legacy.
    [RelayCommand]
    private void SalvarLista()
    {
        // TODO[dominio]: portar DiFiltros.SalvarLista():
        //   SaveFileDialog (*.lst), escribe por fila "Path;F;A;D;M".
    }

    // bIniciar -> IniciarProceso() legacy.
    [RelayCommand]
    private void Iniciar()
    {
        // TODO[dominio]: portar DiFiltros.IniciarProceso():
        //   pone validas a false; el primer filtro activo es la base (marca validas),
        //   los demás se leen en filtro2 y se llama Calcular(nf) que descarta de validas
        //   (DiFiltros.Valida) las columnas sin suficientes vecinas a distancia 0-3
        //   (máscara D, umbral M=mincol). Lee columnas vía
        //   Free1X2.EntradaSalida.ArchivoColumnasTexto + Free1X2.Utils.ConvertidorDeBases.s2n.
        //   Si 'analisis' está activo, llama Escrutar(nf) por fila. Cronometra (lTime).
    }

    // bCancelar -> salida = true en BCancelarClick legacy.
    [RelayCommand]
    private void Cancelar()
    {
        // TODO[dominio]: portar DiFiltros: poner el flag 'salida' a true para abortar
        //   el bucle de IniciarProceso()/Calcular().
        Procesando = false;
    }

    // bGrabar -> Grabar() legacy.
    [RelayCommand]
    private void Grabar()
    {
        // TODO[dominio]: portar DiFiltros.Grabar():
        //   SaveFileDialog (*.txt, dir "Filtros\\"), guarda el BitArray 'validas' vía
        //   Free1X2.EntradaSalida.ArchivoColumnasTexto.GuardarTodasCols(validas);
        //   actualiza ArchivoSalida con el nombre del archivo.
    }

    // ckMD -> MarcaDesmarca() legacy.
    [RelayCommand]
    private void MarcarDesmarcar()
    {
        foreach (var fila in Filtros)
        {
            fila.Activo = ActivarTodas;
        }
        // TODO[dominio]: equivalente a DiFiltros.MarcaDesmarca() sobre el DataSet legacy.
    }

    // bFG -> EntraCGsR() legacy.
    [RelayCommand]
    private void CargarGanadoras()
    {
        // TODO[dominio]: portar DiFiltros.EntraCGsR():
        //   OpenFileDialog (*.txt) de columnas ganadoras; valida cada línea con
        //   VerColumna (14 signos en "12xX"); guarda en colgsR[], fija nrfCGR=limcgsR,
        //   FicheroGanadoras=nombre, IndiceGanadora=nrfCGR, ColumnaGanadora=última col,
        //   PuedeAnalizar=true.
    }

    // bAnalizar -> Analizar() legacy.
    [RelayCommand]
    private void Analizar()
    {
        // TODO[dominio]: portar DiFiltros.Analizar():
        //   prm14 = s2n(ColumnaGanadora); calcula las 28 variantes a 13 (prm13)
        //   cambiando un signo; activa 'analisis' y llama IniciarProceso();
        //   Escrutar() rellena las columnas "14"/"13" de cada fila.
    }

    // bMasR -> GRMas() legacy.
    [RelayCommand]
    private void GanadoraSiguiente()
    {
        // TODO[dominio]: portar DiFiltros.GRMas():
        //   if (nrfCGR < limcgsR) { nrfCGR++; IndiceGanadora=nrfCGR;
        //     ColumnaGanadora = colgsR[nrfCGR-1]; }
    }

    // bMenosR -> GRMenos() legacy.
    [RelayCommand]
    private void GanadoraAnterior()
    {
        // TODO[dominio]: portar DiFiltros.GRMenos():
        //   if (nrfCGR > 1) { nrfCGR--; IndiceGanadora=nrfCGR;
        //     ColumnaGanadora = colgsR[nrfCGR-1]; }
    }
}
