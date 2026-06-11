using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de un partido en la tabla de Dependencia Lineal (legacy: labels lblNewBaseNN1/X/2
/// para el pronóstico y LblCoefNN para el coeficiente).
/// Cada partido aporta un signo (0/1/2) ponderado por su coeficiente al cálculo lineal
/// del partido a tratar.
/// </summary>
public partial class DependenciaPartidoViewModel : ObservableObject
{
    public DependenciaPartidoViewModel(int indice)
    {
        Indice = indice;
    }

    // Índice 0..13 del partido (legacy: Tag del label).
    public int Indice { get; }

    // Etiqueta visible "P1".."P14" (no se bindea int directo a Text -> regla anti-crash 2).
    public string Etiqueta => "P" + (Indice + 1).ToString();

    // Indica si este partido es el "Partido a tratar" (legacy: PartidoATratar).
    // Cuando es true se ignora su coeficiente y se recalcula su signo a partir de los demás.
    [ObservableProperty]
    private bool _esPartidoATratar;

    // Coeficiente del partido (legacy: Coef[Partido], cíclico 0/1/2). Es double porque
    // NumberBox.Value es double (regla anti-crash 7); el dominio lo trunca a byte 0..2.
    [ObservableProperty]
    private double _coeficiente;

    // Pronóstico del partido a tratar: signos 1/X/2 activos (legacy: Pronosticos[Partido,0..2]).
    [ObservableProperty]
    private bool _signo1;

    [ObservableProperty]
    private bool _signoX;

    [ObservableProperty]
    private bool _signo2;
}

/// <summary>
/// ViewModel de la pantalla "Dependencia Lineal" (legacy: FrmDependenciaLineal).
/// Recalcula el signo del "partido a tratar" como combinación lineal (mód. 3 ó 2)
/// de los signos del resto de partidos, ponderados por sus coeficientes, y reescribe
/// el archivo de columnas resultante.
/// Equivalente legacy: Free1X2.UI.FrmDependenciaLineal + ConvertidorDeBases + ArchivoColumnasTexto.
/// </summary>
public partial class FrmDependenciaLinealViewModel : ObservableObject
{
    public FrmDependenciaLinealViewModel()
    {
        var partidos = new ObservableCollection<DependenciaPartidoViewModel>();
        for (int i = 0; i < 14; i++)
        {
            partidos.Add(new DependenciaPartidoViewModel(i));
        }
        Partidos = partidos;
    }

    // 14 partidos de la quiniela (legacy: arrays Coef[15] / Pronosticos[15,3] / labels).
    public ObservableCollection<DependenciaPartidoViewModel> Partidos { get; }

    // Nombre del archivo de entrada mostrado (legacy: TxFicheroEntrada.Text).
    [ObservableProperty]
    private string _nombreFicheroEntrada = "(falta selección)";

    // Nombre del archivo de salida mostrado (legacy: TxFicheroSalida.Text).
    [ObservableProperty]
    private string _nombreFicheroSalida = "(falta selección)";

    // Mensaje de estado (legacy: statusBarPanel2.Text).
    [ObservableProperty]
    private string _mensajeEstado = "Falta seleccionar fichero de columnas";

    // Habilita el botón Aceptar tras leer un archivo válido (legacy: btAceptar.Enabled).
    [ObservableProperty]
    private bool _puedeAceptar;

    // Rutas completas seleccionadas (legacy: archivoEntrada / archivoSalida).
    private string _rutaEntrada = string.Empty;
    private string _rutaSalida = string.Empty;

    /// <summary>
    /// Selecciona el archivo de columnas de entrada y lo lee.
    /// Legacy: button1_Click -> OpenFileDialog (carpeta Columnas, *.txt) + LeerColumnas() + ProponerCoeficientes().
    /// </summary>
    [RelayCommand]
    private void SeleccionarEntrada()
    {
        // TODO[dominio]: abrir FileOpenPicker (carpeta "Columnas", filtro *.txt).
        //   Legacy FrmDependenciaLineal.button1_Click:
        //     - archivoEntrada = ruta; NombreFicheroEntrada = Path.GetFileName(ruta)
        //     - LeerColumnas(): IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
        //         recorre columnas -> ConvertidorDeBases.ConvColumnaANumero(), marca BitArray Bits,
        //         cuenta NumApuestas y acumula PorcentajesSignos[14,3] (ContarSignos + ConvertirAPorcentaje),
        //         vuelca a controlPorcentajesCombinacion.Valores.
        //     - ProponerCoeficientes(): Coef[i]=1 si el partido tiene los 3 signos; el primero sin triple
        //         pasa a ser PartidoATratar (Coef=0); rellena LblCoefNN y resalta el pronóstico.
        //     - Pronosticos[PartidoATratar, 0..2] = true (arranca a 1X2).
        //     - Si archivoSalida vacío: archivoSalida = archivoEntrada (misma ruta de salida por defecto).
        //     - MensajeEstado = "Seleccionar partido e indicar coeficientes"; PuedeAceptar = true.
    }

    /// <summary>
    /// Selecciona el archivo de columnas de salida.
    /// Legacy: button2_Click -> SaveFileDialog (carpeta Columnas, *.txt).
    /// </summary>
    [RelayCommand]
    private void SeleccionarSalida()
    {
        // TODO[dominio]: abrir FileSavePicker (carpeta "Columnas", filtro *.txt).
        //   Legacy FrmDependenciaLineal.button2_Click:
        //     - salidaBinaria = (FilterIndex == 2)
        //     - archivoSalida = ruta; NombreFicheroSalida = Path.GetFileName(ruta)
    }

    /// <summary>
    /// Marca un partido como "partido a tratar" (exclusivo). Legacy: GenericLabelPronostico_Click /
    /// InicializarLabelsPronostico (sólo un partido puede tener pronóstico activo a la vez).
    /// </summary>
    [RelayCommand]
    private void MarcarPartidoATratar(DependenciaPartidoViewModel partido)
    {
        if (partido is null)
        {
            return;
        }

        foreach (var p in Partidos)
        {
            bool esElegido = ReferenceEquals(p, partido);
            p.EsPartidoATratar = esElegido;
            if (!esElegido)
            {
                // Legacy: Pronosticos[Parti,0..2] = false para el resto de partidos.
                p.Signo1 = false;
                p.SignoX = false;
                p.Signo2 = false;
            }
        }

        // TODO[dominio]: legacy guarda PartidoATratar = (byte)partido.Indice y por defecto
        //   activa los 3 signos (1X2) en ese partido.
        partido.Signo1 = true;
        partido.SignoX = true;
        partido.Signo2 = true;

        MensajeEstado = "Seleccionar partido e indicar coeficientes";
    }

    /// <summary>
    /// Ejecuta el cálculo de dependencia lineal y graba el archivo resultante.
    /// Legacy: btAceptar_Click -> (TestSignosPartidoATratar) + bucle sobre 4.782.969 combinaciones + GrabarColumnas().
    /// </summary>
    [RelayCommand]
    private void Aceptar()
    {
        // TODO[dominio]: validar y ejecutar el cálculo lineal.
        //   Legacy FrmDependenciaLineal.btAceptar_Click + TestSignosPartidoATratar + GrabarColumnas:
        //     - TestSignosPartidoATratar(): según los signos activos del PartidoATratar fija
        //         (Modulo, TerminoCorrectorDobles, TerminoIndependiente):
        //           1X2 -> (3,1,0)   1X -> (2,1,0)   X2 -> (2,1,1)   12 -> (2,2,0)
        //         Si no es triple ni un doble válido:
        //           MensajeEstado = "Debe poner un triple o un doble en el partido a tratar"; return.
        //     - Para cada combinación i con Bits[i]:
        //           SigIni = (i / pot[PartidoATratar]) % 3;
        //           NuevoSigno = Σ Coef[Partido] * ((i / pot[Partido]) % 3)  (Partido != PartidoATratar);
        //           NuevoSigno = (NuevoSigno % Modulo) * TerminoCorrectorDobles + TerminoIndependiente;
        //           Indice = i + pot[PartidoATratar] * (NuevoSigno - SigIni);  BitsCambiados[Indice] = true.
        //         (pot = potencias de 3; Coef[] tomado de Partidos[i].Coeficiente truncado a byte 0..2).
        //     - GrabarColumnas(): IArchivoColumnas Cols = new ArchivoColumnasTexto(archivoSalida);
        //         guarda ConvertidorDeBases.ConvNumAColumna(i) por cada BitsCambiados[i];
        //         MensajeEstado = "Se han grabado N columnas"; recarga LeerColumnas().
    }

    /// <summary>
    /// Cierra/regresa sin ejecutar. Legacy: btCancelar -> Close().
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO[dominio]: navegación WinUI — Frame.GoBack() o cerrar el host contenedor
        //   (equivale a FrmDependenciaLineal.Close()).
    }
}
