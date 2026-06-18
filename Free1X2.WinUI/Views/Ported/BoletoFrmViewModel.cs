// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Boleto" (visor/navegador de boletos).
/// Replica el WinForms <c>BoletoFrm</c>: abre un archivo de columnas, calcula cuántos
/// boletos imprimibles contiene y permite navegar entre ellos (primero / anterior /
/// ir a nº / siguiente / último).
///
/// La carga de columnas (Free1X2.EntradaSalida.ArchivoColumnasTexto), el cálculo de
/// boletos/apuestas (ControlBoleto.CreaMatriz) y la ordenación de la matriz
/// (ControlBoleto.OrdenarMatrizColumnas, que usa FiltroNoVariantes/FiltroInterrupciones)
/// SÍ están cableadas al motor real. El UserControl legacy
/// <c>Free1X2.UI.Controls.Boleto.ControlColumnaBoleto</c> se sustituyó por el
/// <c>BoletoMatrizControl</c> de WinUI; la sincronización visual está cableada: el ViewModel
/// emite el evento <c>BoletoCambiado</c> con las 8 columnas del boleto actual y la página host
/// las vuelca en el control.
/// </summary>
public partial class BoletoFrmViewModel : ObservableObject
{
    // ===== Matriz de columnas cargada (legacy: ControlBoleto.columna[] / apuestas) =====
    private string[] _columna = Array.Empty<string>();
    private int _apuestas;

    // Criterio/sentido de ordenación (legacy: BoletoFrm.ordenarPor / tipoOrden, fijados por VerBoletos).
    public OrdenarMatriz OrdenarPor { get; set; } = OrdenarMatriz.Signo;
    public TipoOrden TipoOrden { get; set; } = TipoOrden.asc;

    // Ruta del archivo de combinación de entrada (WinForms: ArchivoCombinacion).
    [ObservableProperty]
    private string _archivoCombinacion = "";

    // Total de boletos imprimibles del archivo (WinForms: campo 'boletos' / totalBoletos.Text).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalBoletosTexto))]
    private int _totalBoletos = 1;

    /// <summary>Proyección en cadena de <see cref="TotalBoletos"/> para enlazar a TextBlock.Text.</summary>
    public string TotalBoletosTexto => TotalBoletos.ToString();

    // Número de boleto actualmente mostrado (1-based, como el WinForms 'boletoActual').
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BoletoActualTexto))]
    private int _boletoActual = 1;

    /// <summary>Proyección en cadena de <see cref="BoletoActual"/> (x:Bind no convierte int -&gt; string).</summary>
    public string BoletoActualTexto => BoletoActual.ToString();

    // Destino del cuadro "Ir a nº" (WinForms: boletoActual TextBox editable). NumberBox -> double.
    [ObservableProperty]
    private double _irABoleto = 1;

    // Mensaje de estado / errores de rango (WinForms: MessageBox "fuera de rango").
    [ObservableProperty]
    private string _estado = "Selecciona un archivo de combinación para ver sus boletos.";

    [ObservableProperty]
    private bool _cargando;

    /// <summary>
    /// Columnas (signos crudos) del boleto actualmente mostrado: hasta 8 columnas,
    /// equivalentes a las 8 ControlColumnaBoleto que el WinForms rellenaba en LlenarBoleto.
    /// Vacío en posiciones sin columna (boleto incompleto), igual que el legacy.
    /// </summary>
    public ObservableCollection<string> ColumnasBoletoActual { get; } = new();

    /// <summary>
    /// Se dispara cada vez que cambia el boleto mostrado (<see cref="LlenarBoleto"/>).
    /// La página host lo recibe y vuelca los datos en el <c>BoletoMatrizControl</c>
    /// visual (control.Llenar), reemplazando lo que el WinForms hacía rellenando las 8
    /// ControlColumnaBoleto. Args: signos por columna y nº de columna en la combinación.
    /// </summary>
    public event EventHandler<(string[] signos, int[] numerosColumna)>? BoletoCambiado;

    /// <summary>
    /// Equivale a <c>BoletoFrm_Load</c>: lee el archivo, crea la matriz de columnas,
    /// calcula el total de boletos, ordena y muestra el primero.
    /// </summary>
    [RelayCommand]
    private async Task CargarAsync()
    {
        if (string.IsNullOrEmpty(ArchivoCombinacion))
        {
            Estado = "No se ha seleccionado ningún archivo.";
            return;
        }

        Cargando = true;
        Estado = "Cargando boletos...";

        try
        {
            string archivoEntrada = ArchivoCombinacion;
            OrdenarMatriz orden = OrdenarPor;
            TipoOrden tipo = TipoOrden;

            // Lectura + cálculo + ordenación en hilo de fondo (legacy lo hacía con WaitCursor).
            var (columnas, apuestas, boletos) = await Task.Run(() =>
            {
                // Legacy: IArchivoColumnas archComb = new ArchivoColumnasTexto(archivoEntrada);
                IArchivoColumnas archComb = new ArchivoColumnasTexto(archivoEntrada);

                // Legacy: controlBoleto1.CreaMatriz(Convert.ToInt32(archComb.ObtenNumCols()));
                int capacidad = Convert.ToInt32(archComb.ObtenNumCols());
                string[] cols = new string[capacidad];

                // Legacy: ControlBoleto.CreaMatriz -> boletos = ceil(capacidad/8); apuestas = capacidad.
                int nBoletos = (capacidad % 8) == 0
                    ? capacidad / 8
                    : Convert.ToInt32((capacidad / 8) + 0.5000001);

                // Legacy: for (i=0; i<apuestas; i++) if (SiguienteColumna()) columna[i] = LeeColumnaSinComas();
                for (int i = 0; i < capacidad; i++)
                {
                    if (archComb.SiguienteColumna())
                    {
                        cols[i] = archComb.LeeColumnaSinComas();
                    }
                    else
                    {
                        cols[i] = "";
                    }
                }
                archComb.Cerrar();

                // Legacy: controlBoleto1.OrdenarMatrizColumnas(ordenarPor, tipoOrden).
                OrdenarMatrizColumnas(cols, orden, tipo);

                return (cols, capacidad, nBoletos);
            });

            _columna = columnas;
            _apuestas = apuestas;
            TotalBoletos = boletos <= 0 ? 1 : boletos;

            // Legacy: Text = Path.GetFileName(archivoEntrada).
            string nombre = System.IO.Path.GetFileName(archivoEntrada);

            // Legacy: LlenarBoleto(0).
            LlenarBoleto(0);
            Estado = $"{nombre}: {TotalBoletos} boleto(s).";
        }
        catch (Exception ex)
        {
            Estado = $"Error al cargar el boleto: {ex.Message}";
            Services.AppServices.MostrarError($"No se pudo cargar el boleto: {ex.Message}");
        }
        finally
        {
            Cargando = false;
        }
    }

    /// <summary>Equivale a <c>btnPrimero_Click</c>: muestra el primer boleto.</summary>
    [RelayCommand]
    private void Primero() => LlenarBoleto(0);

    /// <summary>Equivale a <c>btnAnterior_Click</c>: retrocede un boleto.</summary>
    [RelayCommand]
    private void Anterior() => LlenarBoleto(BoletoActual - 2);

    /// <summary>Equivale a <c>btnSiguiente_Click</c>: avanza un boleto.</summary>
    [RelayCommand]
    private void Siguiente() => LlenarBoleto(BoletoActual);

    /// <summary>Equivale a <c>btnUltimo_Click</c>: muestra el último boleto.</summary>
    [RelayCommand]
    private void Ultimo() => LlenarBoleto(TotalBoletos - 1);

    /// <summary>Equivale a <c>btnIr_Click</c>: salta al boleto indicado, validando el rango.</summary>
    [RelayCommand]
    private void Ir()
    {
        int destino = (int)IrABoleto;
        if (destino < 1 || destino > TotalBoletos)
        {
            // Legacy: MessageBox.Show("El número de boleto está fuera de rango").
            Estado = "El número de boleto está fuera de rango.";
            return;
        }
        LlenarBoleto(destino - 1);
    }

    /// <summary>
    /// Equivale a <c>ControlBoleto.LlenarBoleto(int)</c>: vuelca las 8 columnas del boleto
    /// (0-based) en <see cref="ColumnasBoletoActual"/> y actualiza el contador 1-based.
    /// </summary>
    private void LlenarBoleto(int numBol)
    {
        if (numBol < 0) numBol = 0;
        if (numBol > TotalBoletos - 1) numBol = TotalBoletos - 1;

        // Legacy: int numColumna = (boleto*8)+1; for (i=0..7) si ((numColumna+i-1) < apuestas)
        //   LlenarColumna(i+1, columna[numColumna+i-1], numColumna+i, 0) else LlenarColumna(i+1, "", 0, 0).
        ColumnasBoletoActual.Clear();
        var signos = new string[8];
        var numerosColumna = new int[8];
        int numColumna = (numBol * 8) + 1;
        for (int i = 0; i < 8; i++)
        {
            int idx = numColumna + i - 1;
            string col = idx < _apuestas && idx >= 0 ? (_columna[idx] ?? "") : "";
            signos[i] = col;
            // Legacy LlenarColumna: numColumnaEnCombinacion = numColumna + i (0 si no existe).
            numerosColumna[i] = string.IsNullOrEmpty(col) ? 0 : numColumna + i;
            ColumnasBoletoActual.Add(col);
        }

        // Pinta el boleto visual (8 columnas × 16 apuestas) reemplazando lo que el WinForms
        // hacía con las 8 ControlColumnaBoleto. Aciertos = 0 como en el visor legacy
        // (ControlBoleto.LlenarColumna recibía ac=0). Ref Free1X2/UI/Controls/ControlBoleto.cs
        // líneas 129-146. La página host vuelca esto en el BoletoMatrizControl.
        BoletoCambiado?.Invoke(this, (signos, numerosColumna));

        BoletoActual = numBol + 1;
        IrABoleto = BoletoActual;
    }

    /// <summary>
    /// Porta <c>ControlBoleto.OrdenarMatrizColumnas</c> (Free1X2/UI/Controls/ControlBoleto.cs
    /// líneas 163-250). Ordena la matriz de columnas según el criterio/sentido usando los
    /// filtros reales del motor (FiltroNoVariantes / FiltroInterrupciones).
    /// </summary>
    private static void OrdenarMatrizColumnas(string[] columna, OrdenarMatriz orden, TipoOrden tipo)
    {
        if (columna.Length == 0) return;

        // Legacy: OrdenarMatriz.Signo -> Array.Sort (y Reverse si desc).
        if (orden == OrdenarMatriz.Signo)
        {
            Array.Sort(columna);
            if (tipo == TipoOrden.desc)
            {
                Array.Reverse(columna);
            }
            return;
        }

        var fVar = new FiltroNoVariantes();
        var fInt = new FiltroInterrupciones();
        int[] matrizTmp = new int[columna.Length];
        string[] matrizSalida = new string[columna.Length];

        for (int i = 0; i < columna.Length; i++)
        {
            switch (orden)
            {
                case OrdenarMatriz.Variantes:
                    fVar.AnalizaColumna(columna[i]);
                    matrizTmp[i] = fVar.NoVariantes;
                    break;
                case OrdenarMatriz.Equis:
                    fVar.AnalizaColumna(columna[i]);
                    matrizTmp[i] = fVar.NoEquis;
                    break;
                case OrdenarMatriz.Doses:
                    fVar.AnalizaColumna(columna[i]);
                    matrizTmp[i] = fVar.NoDoses;
                    break;
                case OrdenarMatriz.SignosSeguidos:
                    // Legacy: deshabilitado (el cálculo estaba comentado en ControlBoleto).
                    break;
                case OrdenarMatriz.Interrupciones:
                    fInt.AnalizaColumna(columna[i]);
                    matrizTmp[i] = fInt.NoInterrupciones;
                    break;
            }
        }

        int j = 0;
        if (tipo == TipoOrden.asc)
        {
            for (int i = 0; i <= columna[0].Length; i++)
            {
                int pos = Array.IndexOf(matrizTmp, i);
                while (pos >= 0)
                {
                    matrizSalida[j] = columna[pos];
                    j++;
                    pos = Array.IndexOf(matrizTmp, i, pos + 1);
                }
            }
        }
        else
        {
            for (int i = 0; i <= columna[0].Length; i++)
            {
                int pos = Array.IndexOf(matrizTmp, columna[0].Length - i);
                while (pos >= 0)
                {
                    matrizSalida[j] = columna[pos];
                    j++;
                    pos = Array.IndexOf(matrizTmp, columna[0].Length - i, pos + 1);
                }
            }
        }
        Array.Copy(matrizSalida, columna, matrizSalida.Length);
    }
}
