using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del WinForms <c>Anastatics</c> (ventana "Estadísticas 0.3.4").
/// Calcula estadísticas sobre un fichero de columnas (combinaciones de 14 signos 1/X/2):
/// el usuario elige un modo de análisis, selecciona el fichero origen, calcula y muestra
/// los resultados en una ventana específica por modo.
/// Toda la lógica de dominio (lectura del fichero, Dibujos/DibRepes/StaInter/StaSigSeg,
/// y la apertura de DibForm/DibRepFrm/StaInterFrm/StaSSForm) está marcada como TODO.
/// </summary>
public partial class AnastaticsViewModel : ObservableObject
{
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

    [RelayCommand]
    private void SeleccionarOrigen()
    {
        // Equivale a SelOrigen() del Anastatics legacy.
        // TODO: Dominio legacy —
        //   OpenFileDialog (filtro "Columnas(*.txt)|*.txt|Todos (*.*)|*.*") -> usar FileOpenPicker.
        //   IArchivoColumnas ac = new ArchivoColumnasTexto(filein);
        //   while (ac.SiguienteColumna()) { validar con VerColumna(ac.LeeColumnaSinComas()); ... }
        //   Volcar nombre de fichero a FicheroOrigen y el contador a ColumnasTexto.
        // Tras leer correctamente: OrigenSeleccionado = true; ResultadosListos = false;
    }

    [RelayCommand]
    private void Calcular()
    {
        // Equivale a Proceso() del Anastatics legacy.
        // TODO: Dominio legacy — según ModoSeleccionado, instanciar
        //   0 -> Free1X2.MotorCalculo.Estadisticas.Dibujos
        //   1 -> Free1X2.MotorCalculo.Estadisticas.DibRepes
        //   2 -> Free1X2.MotorCalculo.Estadisticas.StaInter
        //   3 -> Free1X2.MotorCalculo.Estadisticas.StaSigSeg
        //   recorrer las columnas con .Procesar(col) y acumular en la matriz int[15,15] rsl.
        // Tras el cálculo: ResultadosListos = true;
    }

    [RelayCommand]
    private void MostrarResultados()
    {
        // Equivale a Mostrar() del Anastatics legacy.
        // TODO: Dominio legacy — según ModoSeleccionado abrir la ventana de resultados:
        //   0 -> DibForm(rsl, numcol)
        //   1 -> DibRepFrm(rsl, numcol)
        //   2 -> StaInterFrm(rsl, numcol)
        //   3 -> StaSSForm(rsl, numcol)
        // (en WinUI: navegar a la Page de resultados correspondiente).
    }
}
