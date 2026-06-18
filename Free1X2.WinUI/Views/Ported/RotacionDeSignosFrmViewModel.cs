// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Fila del mapeo de rotación de signos para un partido de la columna base.
    /// Cada signo original (1 / X / 2) se transforma en un nuevo signo seleccionable.
    /// Legacy: arrays Signos[], NuevosSignos[16,3] y las matrices de Labels en RotacionDeSignosFrm.
    /// </summary>
    public partial class FilaRotacionViewModel : ObservableObject
    {
        public int Numero { get; set; }

        [ObservableProperty]
        private string _signoBase = "1";

        [ObservableProperty]
        private string _nuevoUno = "1";

        [ObservableProperty]
        private string _nuevaX = "X";

        [ObservableProperty]
        private string _nuevoDos = "2";

        public IReadOnlyList<string> OpcionesSigno { get; } = new List<string> { "1", "X", "2" };

        public string NumeroTexto => Numero.ToString();
    }

    /// <summary>
    /// VM de la Page portada de "Rotación de signos" (legacy WinForms RotacionDeSignosFrm).
    /// Propósito: tomar un fichero de columnas (apuestas 1/X/2), aplicar una rotación/transposición
    /// de signos por partido (definida en la tabla "Cambios de signo") y escribir el resultado en
    /// un fichero de salida. Permite corrección de fallos (chkGiros) y trabajar con valoraciones de
    /// jornada.
    /// </summary>
    public partial class RotacionDeSignosFrmViewModel : ObservableObject
    {
        public RotacionDeSignosFrmViewModel()
        {
            // Legacy: partidos por defecto = 14 hasta que se elige un fichero de entrada.
            ReconstruirFilas(NumeroPartidos);
        }

        private const int NumeroPartidos = 14;

        // pot[] legacy: potencias de 3 usadas para extraer el signo de cada partido en el número
        // de la combinación.
        private static readonly int[] Pot =
        {
            1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907,
        };

        // Rutas legacy: archivoEntrada / archivoSalida.
        private string _archivoEntrada = string.Empty;
        private string _archivoSalida = string.Empty;

        // Nº de partidos del fichero de entrada (legacy: partidos = aCol.ObtenNumSignos()).
        private int _partidos = NumeroPartidos;

        [ObservableProperty]
        private string _ficheroEntrada = "(falta selección)";

        [ObservableProperty]
        private string _ficheroSalida = "(falta selección)";

        // Equivale a chkGiros (corrección de fallos = rotación por partido independiente).
        [ObservableProperty]
        private bool _correccionDeFallos;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EstadoTexto))]
        private bool _puedeCalcular;

        // Mensaje de estado (legacy: statusBarPanel2.Text).
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EstadoTexto))]
        private string _estado = "Faltan datos";

        public string EstadoTexto => string.IsNullOrEmpty(Estado)
            ? (PuedeCalcular ? "Preparado" : "Faltan datos")
            : Estado;

        public ObservableCollection<FilaRotacionViewModel> Filas { get; } = new();

        [RelayCommand]
        private async Task SeleccionarEntrada()
        {
            // Legacy button1_Click: OpenFileDialog "Columnas(*.txt)".
            var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
            picker.FileTypeFilter.Add(".txt");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSingleFileAsync();
            if (file == null) return;

            _archivoEntrada = file.Path;
            FicheroEntrada = Path.GetFileName(_archivoEntrada);

            // Determina partidos y reconstruye las filas (legacy: ObtenNumSignos + InicializarValores).
            try
            {
                IArchivoColumnas aCol = new ArchivoColumnasTexto(_archivoEntrada);
                _partidos = aCol.ObtenNumSignos();
                aCol.Cerrar();
                if (_partidos <= 0) _partidos = NumeroPartidos;
                ReconstruirFilas(_partidos);

                // Lee la primera columna como columna base (legacy: MostrarColumnaBase).
                string? primera = await Task.Run(() =>
                {
                    IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(_archivoEntrada);
                    string? col = comBaseCols.SiguienteColumna() ? comBaseCols.LeeColumnaSinComas() : null;
                    comBaseCols.Cerrar();
                    return col;
                });
                if (primera != null)
                {
                    for (int i = 0; i < Filas.Count && i < primera.Length; i++)
                        Filas[i].SignoBase = primera[i].ToString();
                }
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("Error al leer el archivo: " + ex.Message);
            }

            ActualizarPuedeCalcular();
        }

        [RelayCommand]
        private async Task SeleccionarSalida()
        {
            // Legacy button2_Click: SaveFileDialog "Columnas(*.txt)".
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "ColumnasRotadas",
            };
            picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;

            _archivoSalida = file.Path;
            FicheroSalida = Path.GetFileName(_archivoSalida);
            ActualizarPuedeCalcular();
        }

        [RelayCommand]
        private void Transponer()
        {
            // Legacy button3_Click: rota la matriz de "cambios de signo" un paso (1->X->2->1)
            // en cada fila, equivalente a aplicar GenericLabel_Click a las tres celdas.
            foreach (var fila in Filas)
            {
                fila.NuevoUno = SiguienteSigno(fila.NuevoUno);
                fila.NuevaX = SiguienteSigno(fila.NuevaX);
                fila.NuevoDos = SiguienteSigno(fila.NuevoDos);
            }
        }

        [RelayCommand]
        private async Task Aceptar()
        {
            if (string.IsNullOrEmpty(_archivoEntrada) || string.IsNullOrEmpty(_archivoSalida))
            {
                Estado = "Faltan datos";
                return;
            }

            // NuevosSignos[partido, signoOriginal] = signoDestino (0/1/2), derivado de las filas.
            int partidos = _partidos;
            var nuevosSignos = new int[partidos, 3];
            for (int i = 0; i < partidos; i++)
            {
                nuevosSignos[i, 0] = IndiceSigno(Filas[i].NuevoUno);
                nuevosSignos[i, 1] = IndiceSigno(Filas[i].NuevaX);
                nuevosSignos[i, 2] = IndiceSigno(Filas[i].NuevoDos);
            }

            bool correccion = CorreccionDeFallos;
            string rutaEntrada = _archivoEntrada;
            string rutaSalida = _archivoSalida;

            PuedeCalcular = false;
            Estado = "Procesando...";
            try
            {
                int grabadas = await Task.Run(() =>
                    Rotar(rutaEntrada, rutaSalida, partidos, nuevosSignos, correccion));
                Estado = "Se han grabado " + grabadas + " columnas";
            }
            catch (Exception ex)
            {
                Estado = "Error: " + ex.Message;
            }
            finally
            {
                ActualizarPuedeCalcular();
            }
        }

        [RelayCommand]
        private void Cancelar()
        {
            // Navegación WinUI (Frame.GoBack) es responsabilidad del host de la Page.
        }

        /// <summary>
        /// Núcleo de btAceptar_Click + LeerColumnas + GrabarColumnas legacy: lee columnas a un
        /// BitArray, aplica la rotación de signos y graba el resultado. Devuelve nº columnas grabadas.
        /// </summary>
        private static int Rotar(string rutaEntrada, string rutaSalida, int partidos,
            int[,] nuevosSignos, bool correccion)
        {
            var bits = new BitArray(14348907, false);
            var bitsCambiados = new BitArray(14348907, false);

            // LeerColumnas(): marca cada combinación leída en Bits.
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(rutaEntrada);
            var conv = new ConvertidorDeBases();
            while (comBaseCols.SiguienteColumna())
            {
                int num = conv.ConvColumnaANumero(comBaseCols.LeeColumnaSinComas());
                bits[num] = true;
            }
            comBaseCols.Cerrar();

            // btAceptar_Click(): aplica los cambios de signo.
            if (correccion)
            {
                // chkGiros: cada partido se rota de forma independiente.
                for (int partido = 0; partido < partidos; partido++)
                {
                    for (int i = 0; i < bits.Count; i++)
                    {
                        if (bits[i])
                        {
                            int sigIni = (i / Pot[partido]) % 3;
                            int z = nuevosSignos[partido, sigIni];
                            int indice = i + Pot[partido] * (z - sigIni);
                            bitsCambiados[indice] = true;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < bits.Count; i++)
                {
                    if (bits[i])
                    {
                        int indice = i;
                        for (int partido = 0; partido < partidos; partido++)
                        {
                            int sigIni = (indice / Pot[partido]) % 3;
                            int z = nuevosSignos[partido, sigIni];
                            indice += Pot[partido] * (z - sigIni);
                        }
                        bitsCambiados[indice] = true;
                    }
                }
            }

            // GrabarColumnas(): vuelca BitsCambiados al fichero de salida.
            var con = new ConvertidorDeBases((byte)partidos);
            IArchivoColumnas cols = new ArchivoColumnasTexto(rutaSalida, partidos);
            int c = 0;
            for (int i = 0; i < bitsCambiados.Count; i++)
            {
                if (bitsCambiados[i])
                {
                    cols.GuardarCols(con.ConvNumAColumna(i));
                    c++;
                }
            }
            cols.Cerrar();
            return c;
        }

        private void ReconstruirFilas(int partidos)
        {
            Filas.Clear();
            for (int i = 1; i <= partidos; i++)
                Filas.Add(new FilaRotacionViewModel { Numero = i });
        }

        // "1X2".IndexOf(texto) legacy: 1->0, X->1, 2->2.
        private static int IndiceSigno(string signo)
        {
            int idx = "1X2".IndexOf(signo, StringComparison.Ordinal);
            return idx < 0 ? 0 : idx;
        }

        // GenericLabel_Click legacy: 1 -> X -> 2 -> 1.
        private static string SiguienteSigno(string signo) => signo switch
        {
            "1" => "X",
            "X" => "2",
            _ => "1",
        };

        private void ActualizarPuedeCalcular()
        {
            // Legacy HabilitarCalcular(): habilita Aceptar si entrada y salida están seleccionadas.
            PuedeCalcular =
                !string.IsNullOrEmpty(_archivoEntrada) && !string.IsNullOrEmpty(_archivoSalida);
            Estado = PuedeCalcular ? "Preparado" : "Faltan datos";
        }
    }
}
