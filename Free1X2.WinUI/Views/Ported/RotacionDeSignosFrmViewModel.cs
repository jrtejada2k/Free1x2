// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Analisis;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Controls;
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

            // «Valoraciones JORNADA»: al cambiar cualquier valor se propone la rotación
            // (legacy controlPorcentajesJornada_Modificado -> PropuestaAutomatica).
            ValoracionJornada.CollectionChanged += OnJornadaColeccionCambiada;
            PorcentajesHelper.CargarMatriz(ValoracionJornada, new double[NumeroPartidos, 3]);
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

        // W3 (C2, DT-79): paneles del legacy recuperados.
        // «% COMBINACIÓN»: frecuencia de cada signo en las columnas de la entrada (solo lectura).
        public ObservableCollection<FilaPorcentaje> PorcentajesCombinacion { get; } = new();

        // «Valoraciones JORNADA»: la escribe o abre el usuario.
        public ObservableCollection<FilaPorcentaje> ValoracionJornada { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SinPorcentajes))]
        [NotifyCanExecuteChangedFor(nameof(TransposicionAutomaticaCommand))]
        private bool _hayPorcentajes;

        public bool SinPorcentajes => !HayPorcentajes;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(TransposicionAutomaticaCommand))]
        private bool _ocupado;

        [RelayCommand]
        private async Task SeleccionarEntrada()
        {
            // Legacy button1_Click: OpenFileDialog "Columnas(*.txt)".
            var file = await PickerHelper.AbrirAsync(".txt");
            if (file == null) return;

            await LeerEntradaAsync(file.Path);
        }

        /// <summary>
        /// Legacy LeerColumnas: partidos, columna base y «% COMBINACIÓN» de la entrada.
        /// </summary>
        private async Task LeerEntradaAsync(string ruta)
        {
            _archivoEntrada = ruta;
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

                // «% COMBINACIÓN» (legacy ContarSignos + ConvertirAPorcentaje).
                int partidos = _partidos;
                double[,] pct = await Task.Run(() => RotacionSignos.Porcentajes(ruta, partidos));
                PorcentajesHelper.CargarMatriz(PorcentajesCombinacion, pct);
                HayPorcentajes = true;
                if (ValoracionJornada.Count != partidos)
                    PorcentajesHelper.CargarMatriz(ValoracionJornada, new double[partidos, 3]);
            }
            catch (Exception ex)
            {
                PorcentajesCombinacion.Clear();
                HayPorcentajes = false;
                AppServices.MostrarError("Error al leer el archivo: " + ex.Message);
            }

            ActualizarPuedeCalcular();
        }

        [RelayCommand]
        private async Task SeleccionarSalida()
        {
            // Legacy button2_Click: SaveFileDialog "Columnas(*.txt)".
            var file = await PickerHelper.GuardarAsync("ColumnasRotadas", ("Columnas", ".txt"));
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

        /// <summary>
        /// «Abrir valoración…» del panel «Valoraciones JORNADA» (legacy ControlPorcentajes):
        /// lee el fichero con <see cref="Porcentajes"/> (formatos 1/3/42/43/44), como «Importar %».
        /// </summary>
        [RelayCommand]
        private async Task AbrirValoracion()
        {
            var archivo = await PickerHelper.AbrirAsync(".txt", "*");
            if (archivo == null) return;

            try
            {
                double[,] leidos = await Task.Run(() => new Porcentajes(archivo.Path).valores);
                var m = new double[_partidos, 3];
                for (int i = 0; i < _partidos && i < leidos.GetLength(0); i++)
                    for (int j = 0; j < 3; j++) m[i, j] = leidos[i, j];
                PorcentajesHelper.CargarMatriz(ValoracionJornada, m);
                Proponer();
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("Error al leer la valoración: " + ex.Message);
            }
        }

        private bool PuedeTransposicionAutomatica() =>
            !string.IsNullOrEmpty(_archivoEntrada) && HayPorcentajes && !Ocupado;

        /// <summary>
        /// «Transposición automática» (legacy button3_Click, RotacionDeSignosFrm.cs:2264-2286):
        /// TransponedorAutomatico con la jornada y la combinación, transposición a la salida (o a
        /// «&lt;entrada&gt;T») y esta pasa a ser la entrada. En vez del diálogo de Transposición del
        /// legacy pide confirmación con el orden y el fichero (DT-79), como la app Mac.
        /// </summary>
        [RelayCommand(CanExecute = nameof(PuedeTransposicionAutomatica))]
        private async Task TransposicionAutomatica()
        {
            if (string.IsNullOrEmpty(_archivoEntrada)) return;

            int[]? orden = RotacionSignos.TrasposicionAutomatica(
                PorcentajesHelper.AMatriz(ValoracionJornada), PorcentajesHelper.AMatriz(PorcentajesCombinacion));
            if (orden == null)
            {
                AppServices.MostrarError("La transposición automática necesita 14 partidos con valoración y columnas leídas.");
                return;
            }

            string entrada = _archivoEntrada;
            string destino = !string.IsNullOrEmpty(_archivoSalida)
                ? _archivoSalida
                : Path.Combine(Path.GetDirectoryName(entrada) ?? string.Empty,
                    Path.GetFileNameWithoutExtension(entrada) + "T" + Path.GetExtension(entrada));
            if (destino == entrada) return;

            string lista = string.Join(", ", orden.Select(o => o + 1));
            if (!await AppServices.ConfirmarAsync(
                    "Orden de transposición: " + lista + ".\nSe grabará en " + Path.GetFileName(destino) + ". ¿Transponer?"))
                return;

            Ocupado = true;
            try
            {
                await Task.Run(() => RotacionSignos.TransponerColumnas(entrada, destino, orden));
                _archivoSalida = destino;
                FicheroSalida = Path.GetFileName(destino);
                await LeerEntradaAsync(destino);
                Proponer();
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("Error al transponer: " + ex.Message);
            }
            finally
            {
                Ocupado = false;
            }
        }

        /// <summary>
        /// Legacy PropuestaAutomatica: cada partido rota sus signos según la valoración de la
        /// jornada. Como el legacy, solo con valoración: si está vacía se deja la rotación del usuario.
        /// </summary>
        private void Proponer()
        {
            if (!HayPorcentajes || ValoracionJornada.All(f => f.Uno == 0 && f.Equis == 0 && f.Dos == 0)) return;

            int[,] nuevos = RotacionSignos.PropuestaAutomatica(
                PorcentajesHelper.AMatriz(PorcentajesCombinacion), PorcentajesHelper.AMatriz(ValoracionJornada));
            for (int i = 0; i < Filas.Count && i < nuevos.GetLength(0); i++)
            {
                Filas[i].NuevoUno = Signos[nuevos[i, 0]];
                Filas[i].NuevaX = Signos[nuevos[i, 1]];
                Filas[i].NuevoDos = Signos[nuevos[i, 2]];
            }
        }

        private static readonly string[] Signos = { "1", "X", "2" };

        // Recarga de la jornada (CargarMatriz crea filas nuevas): se escucha cada fila.
        private void OnJornadaColeccionCambiada(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (FilaPorcentaje f in e.OldItems) f.PropertyChanged -= OnJornadaFilaCambiada;
            if (e.NewItems != null)
                foreach (FilaPorcentaje f in e.NewItems) f.PropertyChanged += OnJornadaFilaCambiada;
        }

        private void OnJornadaFilaCambiada(object? sender, PropertyChangedEventArgs e) => Proponer();

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
