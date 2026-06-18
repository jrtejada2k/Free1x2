// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Fila por partido del modificador de columnas.
    /// Porta el UserControl legacy Free1X2.UI.Controls.ModificadorOptions:
    /// un CheckBox que activa el partido y 3 cajas de porcentaje (1 / X / 2)
    /// que solo se editan cuando el partido esta activo.
    /// </summary>
    public partial class ModificadorPartido : ObservableObject
    {
        // Legacy: ModificadorOptions.NumeroPartido (1..14).
        public int Numero { get; }

        // Anti-crash regla 2: no bindear int directo a TextBlock.Text -> exponer string.
        public string NumeroTexto => Numero.ToString();

        public ModificadorPartido(int numero)
        {
            Numero = numero;
        }

        // Legacy: ModificadorOptions.chckActive (PartidoActivo). Al activarse, habilita
        // las 3 cajas de valor (ChckActiveCheckedChanged en el control legacy).
        [ObservableProperty]
        private bool _activo;

        // Legacy: ModificadorOptions.Valor_1 / Valor_X / Valor_2 (string, MaxLength 3).
        // Representan el porcentaje de cada signo en el partido.
        [ObservableProperty]
        private double _valor1;

        [ObservableProperty]
        private double _valorX;

        [ObservableProperty]
        private double _valor2;
    }

    /// <summary>
    /// ViewModel para ModificadorFrmPage.
    /// Porta el WinForms legacy Free1X2.UI.ModificadorFrm ("Modificador de columnas").
    /// Carga un fichero de columnas, muestra el porcentaje 1/X/2 de cada uno de los 14
    /// partidos y permite redistribuir los signos segun un modo (aleatorio / proporcional
    /// / ordenado), opcionalmente ordenando los signos, para luego grabar el resultado.
    /// </summary>
    public partial class ModificadorFrmViewModel : ObservableObject
    {
        public ModificadorFrmViewModel()
        {
            // Legacy: PonerControles() crea 14 ModificadorOptions (noPartidos = 14).
            // El numero real de partidos lo determina el fichero (aCol.ObtenNumSignos()).
            for (int i = 1; i <= 14; i++)
            {
                Partidos.Add(new ModificadorPartido(i));
            }
        }

        // ----- Estado del dominio (legacy: campos de ModificadorFrm) -----
        private int _noPartidos = 14;
        private int _apuestas;
        private int[,] _frecuencia = new int[14, 3];
        private string[] _columnas = Array.Empty<string>();
        private string _rutaEntrada = string.Empty;

        // Legacy: gbPartidos con un ModificadorOptions por partido.
        public ObservableCollection<ModificadorPartido> Partidos { get; } = new();

        // Legacy: TextBox 'fichero' (ReadOnly) que muestra el nombre del fichero abierto.
        [ObservableProperty]
        private string _ficheroNombre = "(ningún fichero cargado)";

        // Legacy: btnOk.Enabled = false hasta que se abre un fichero (AbrirClick).
        [ObservableProperty]
        private bool _ficheroCargado;

        // ----- Distribución de los signos -----
        // Legacy: GroupBox "Distribución de los signos" con 3 RadioButton:
        //   radioDistribucion1 "Aleatorio"     -> opcion = 1
        //   radioDistribucion2 "Proporcional"  -> opcion = 2 (Checked por defecto)
        //   radioDistribucion3 "Ordenado"      -> opcion = 3
        // Anti-crash regla 3: ItemsSource desde propiedad del VM, no <x:String> inline.
        public IReadOnlyList<string> DistribucionOpciones { get; } = new[]
        {
            "Aleatorio",
            "Proporcional",
            "Ordenado",
        };

        // Legacy: radioDistribucion2 (Proporcional) es la opcion por defecto.
        [ObservableProperty]
        private string _distribucionSeleccionada = "Proporcional";

        // Legacy: CheckBox 'checkOrdenar' ("Ordenar signos"), Checked por defecto.
        // Solo se habilita cuando la opcion es Proporcional u Ordenado (opcion > 1);
        // se deshabilita para Aleatorio (ver radioDistribucionX_CheckedChanged).
        [ObservableProperty]
        private bool _ordenarSignos = true;

        // Anti-crash regla 1: el ToggleSwitch hijo lee este flag en su IsEnabled,
        // nunca un panel contenedor.
        public bool OrdenarHabilitado => DistribucionSeleccionada != "Aleatorio";

        partial void OnDistribucionSeleccionadaChanged(string value)
        {
            OnPropertyChanged(nameof(OrdenarHabilitado));
        }

        // Mapea la opción de distribución al entero legacy (opcion 1/2/3).
        private int Opcion => DistribucionSeleccionada switch
        {
            "Aleatorio" => 1,
            "Ordenado" => 3,
            _ => 2,
        };

        [RelayCommand]
        private async Task Abrir()
        {
            // Legacy AbrirClick: OpenFileDialog (*.txt).
            var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
            picker.FileTypeFilter.Add(".txt");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSingleFileAsync();
            if (file == null) return;

            _rutaEntrada = file.Path;
            try
            {
                // ObtenNumSignos + ObtenNumCols + LeeFichero (en hilo de fondo).
                await Task.Run(() =>
                {
                    IArchivoColumnas aCol = new ArchivoColumnasTexto(_rutaEntrada);
                    _noPartidos = aCol.ObtenNumSignos();
                    _apuestas = Convert.ToInt32(aCol.ObtenNumCols());
                    aCol.Cerrar();
                    LeeFichero(_rutaEntrada);
                });

                // PonerControles + AccionModificador(Escribir): vuelca los porcentajes a las filas.
                ReconstruirPartidos();
                EscribirFrecuencias();

                FicheroNombre = Path.GetFileName(_rutaEntrada);
                FicheroCargado = true;
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("Error al leer el archivo: " + ex.Message);
            }
        }

        [RelayCommand]
        private void Aceptar()
        {
            // Legacy BtnOkClick: AccionModificador(Modificar) + AccionModificador(Escribir).
            if (!FicheroCargado) return;
            foreach (var partido in Partidos)
            {
                if (partido.Activo)
                {
                    ModificarFrecuencia(partido, partido.Numero - 1);
                }
            }
            EscribirFrecuencias();
        }

        [RelayCommand]
        private async Task Grabar()
        {
            // Legacy BtnGrabarClick: SaveFileDialog con nombre "<fichero>_modificado.txt".
            if (!FicheroCargado) return;

            string baseNombre = Path.GetFileNameWithoutExtension(_rutaEntrada);
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = baseNombre + "_modificado",
            };
            picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;

            string ruta = file.Path;
            var cols = _columnas;
            try
            {
                await Task.Run(() =>
                {
                    // GuardaFichero(): vuelca el array de columnas.
                    IArchivoColumnas archComb = new ArchivoColumnasTexto(ruta);
                    archComb.GuardarTodasCols(cols);
                    archComb.Cerrar();
                });
                AppServices.MostrarInfo("Se ha grabado el archivo.");
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("Error al grabar: " + ex.Message);
            }
        }

        [RelayCommand]
        private void Cancelar()
        {
            // Navegación WinUI (Frame.GoBack) es responsabilidad del host de la Page.
        }

        // ---- Lógica portada de ModificadorFrm ----

        private void ReconstruirPartidos()
        {
            // Ajusta el nº de filas al nº de partidos del fichero (legacy PonerControles).
            if (Partidos.Count == _noPartidos) return;
            Partidos.Clear();
            for (int i = 1; i <= _noPartidos; i++) Partidos.Add(new ModificadorPartido(i));
        }

        // AccionModificador(Escribir): muestra la frecuencia (porcentaje) de cada partido.
        private void EscribirFrecuencias()
        {
            for (int i = 0; i < Partidos.Count && i < _noPartidos; i++)
            {
                Partidos[i].Valor1 = _frecuencia[i, 0];
                Partidos[i].ValorX = _frecuencia[i, 1];
                Partidos[i].Valor2 = _frecuencia[i, 2];
            }
        }

        // LeeFichero(): carga columnas y calcula el porcentaje 1/X/2 de cada partido.
        private void LeeFichero(string archivoEntrada)
        {
            long contador = 0;
            IArchivoColumnas archComb = new ArchivoColumnasTexto(archivoEntrada);

            _columnas = new string[_apuestas];
            _frecuencia = new int[_noPartidos, 3];

            while (archComb.SiguienteColumna())
            {
                _columnas[contador] = archComb.LeeColumnaSinComas();
                for (int i = 0; i < _noPartidos; i++)
                {
                    char signo = _columnas[contador][i];
                    switch (signo)
                    {
                        case '1': _frecuencia[i, 0]++; break;
                        case 'X': _frecuencia[i, 1]++; break;
                        case '2': _frecuencia[i, 2]++; break;
                    }
                }
                contador++;
            }
            archComb.Cerrar();

            // pasa la frecuencia a porcentajes
            for (int i = 0; i < _noPartidos; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    _frecuencia[i, j] = Convert.ToInt32((_frecuencia[i, j] * 100 / Convert.ToDouble(_apuestas)) + 0.4);
                }
                while ((_frecuencia[i, 0] + _frecuencia[i, 1] + _frecuencia[i, 2]) < 100)
                {
                    _frecuencia[i, 0]++;
                }
                while ((_frecuencia[i, 0] + _frecuencia[i, 1] + _frecuencia[i, 2]) > 100)
                {
                    if (_frecuencia[i, 2] > 0) _frecuencia[i, 2]--;
                    else if (_frecuencia[i, 1] > 0) _frecuencia[i, 1]--;
                }
            }
        }

        // ModificarFrecuencia(): reparte los signos del partido según los porcentajes y el modo.
        private void ModificarFrecuencia(ModificadorPartido partido, int numPartido)
        {
            var signo = new char[3];
            var pct = new double[3];
            var inicial = new int[3];
            var total = new int[3];
            var grupo = new int[3];

            pct[0] = partido.Valor1;
            pct[1] = partido.ValorX;
            pct[2] = partido.Valor2;

            // Comprueba que sume 100; si no, convierte a % (legacy avisa con MessageBox -> aquí
            // se asume continuar para no bloquear el flujo del cálculo).
            double pctSuma = pct[0] + pct[1] + pct[2];
            if (pctSuma != 100 && pctSuma != 0)
            {
                pct[0] *= 100 / pctSuma;
                pct[1] *= 100 / pctSuma;
                pct[2] *= 100 / pctSuma;
            }

            inicial[0] = Convert.ToInt32(Convert.ToDouble(_apuestas) * pct[0] / 100);
            inicial[1] = Convert.ToInt32(Convert.ToDouble(_apuestas) * pct[1] / 100);
            inicial[2] = Convert.ToInt32(Convert.ToDouble(_apuestas) * pct[2] / 100);
            while ((inicial[0] + inicial[1] + inicial[2]) < _apuestas) inicial[0]++;
            while ((inicial[0] + inicial[1] + inicial[2]) > _apuestas) inicial[2]--;

            string temporal = Ordenar(pct);
            int opcion = Opcion;

            if (OrdenarSignos && opcion > 1)
            {
                signo[0] = temporal[0];
                signo[1] = temporal[1];
                signo[2] = temporal[2];
                for (int i = 0; i < 3; i++)
                {
                    switch (signo[i])
                    {
                        case '1': total[i] = inicial[0]; break;
                        case 'X': total[i] = inicial[1]; break;
                        case '2': total[i] = inicial[2]; break;
                    }
                }
            }
            else
            {
                signo[0] = '1';
                signo[1] = 'X';
                signo[2] = '2';
                for (int i = 0; i < 3; i++) total[i] = inicial[i];
            }

            switch (opcion)
            {
                case 1:
                    signo[0] = '1';
                    signo[1] = 'X';
                    signo[2] = '2';
                    for (int numColumna = 0; numColumna < _apuestas; numColumna++)
                    {
                        int aleatorio;
                        do
                        {
                            aleatorio = Convert.ToInt32(Math.Pow(DateTime.Now.Millisecond, 3)) % 3;
                        } while (total[aleatorio] == 0);
                        _columnas[numColumna] = CambiarSigno(_columnas[numColumna], numPartido, signo[aleatorio]);
                        total[aleatorio]--;
                    }
                    break;
                case 2:
                    for (int i = 0; i < 3; i++)
                    {
                        if (total[2] == 0)
                        {
                            grupo[i] = total[1] == 0 ? total[i] : total[i] / total[1];
                        }
                        else
                        {
                            grupo[i] = total[i] / total[2];
                        }
                    }
                    ProcesarGrupos(total, grupo, signo, numPartido);
                    break;
                case 3:
                    for (int i = 0; i < 3; i++) grupo[i] = total[i];
                    ProcesarGrupos(total, grupo, signo, numPartido);
                    break;
            }

            for (int i = 0; i < 3; i++) _frecuencia[numPartido, i] = Convert.ToInt32(pct[i]);
            partido.Activo = false;
        }

        private void ProcesarGrupos(int[] total, int[] grupo, char[] signo, int numPartido)
        {
            var quedan = new int[3];
            int procesados = 0;
            while (procesados < _apuestas)
            {
                for (int i = 0; i < 3; i++)
                {
                    quedan[i] = grupo[i];
                    if (quedan[i] > total[i]) quedan[i] = total[i];
                    while (quedan[i] > 0 && total[i] > 0)
                    {
                        _columnas[procesados] = CambiarSigno(_columnas[procesados], numPartido, signo[i]);
                        quedan[i]--;
                        total[i]--;
                        procesados++;
                    }
                }
            }
        }

        private string CambiarSigno(string columna, int partido, char signo)
        {
            string nuevoSigno = Convert.ToString(signo);
            string nuevaColumna = "";
            for (int i = 0; i < _noPartidos; i++)
            {
                nuevaColumna += i == partido ? nuevoSigno : columna[i].ToString();
            }
            return nuevaColumna;
        }

        private static string Ordenar(double[] porcentaje)
        {
            // Ordena los 3 signos de mayor a menor.
            var orden = new char[3];
            if (porcentaje[0] >= porcentaje[1] && porcentaje[0] >= porcentaje[2])
            {
                orden[0] = '1';
                if (porcentaje[1] >= porcentaje[2]) { orden[1] = 'X'; orden[2] = '2'; }
                else { orden[1] = '2'; orden[2] = 'X'; }
            }
            if (porcentaje[1] >= porcentaje[0] && porcentaje[1] >= porcentaje[2])
            {
                orden[0] = 'X';
                if (porcentaje[0] >= porcentaje[2]) { orden[1] = '1'; orden[2] = '2'; }
                else { orden[1] = '2'; orden[2] = '1'; }
            }
            if (porcentaje[2] >= porcentaje[1] && porcentaje[2] >= porcentaje[0])
            {
                orden[0] = '2';
                if (porcentaje[0] >= porcentaje[1]) { orden[1] = '1'; orden[2] = 'X'; }
                else { orden[1] = 'X'; orden[2] = '1'; }
            }
            return string.Concat(orden[0], orden[1], orden[2]);
        }
    }
}
