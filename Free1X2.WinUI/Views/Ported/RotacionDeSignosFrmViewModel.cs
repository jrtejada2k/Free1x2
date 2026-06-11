using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    /// un fichero de salida. Permite corrección de fallos y trabajar con valoraciones de jornada.
    /// </summary>
    public partial class RotacionDeSignosFrmViewModel : ObservableObject
    {
        public RotacionDeSignosFrmViewModel()
        {
            // Legacy: InicializarValores() / partidos por defecto = 14.
            for (int i = 1; i <= NumeroPartidos; i++)
            {
                Filas.Add(new FilaRotacionViewModel { Numero = i });
            }
        }

        private const int NumeroPartidos = 14;

        [ObservableProperty]
        private string _ficheroEntrada = "(falta selección)";

        [ObservableProperty]
        private string _ficheroSalida = "(falta selección)";

        [ObservableProperty]
        private bool _correccionDeFallos;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EstadoTexto))]
        private bool _puedeCalcular;

        public string EstadoTexto => PuedeCalcular ? "Preparado" : "Faltan datos";

        public ObservableCollection<FilaRotacionViewModel> Filas { get; } = new();

        [RelayCommand]
        private void SeleccionarEntrada()
        {
            // TODO Legacy RotacionDeSignosFrm.button1_Click:
            //   - OpenFileDialog filtro "Columnas(*.txt)".
            //   - archivoEntrada = ruta; ObtenNumSignos() via ArchivoColumnasTexto (IArchivoColumnas).
            //   - InicializarValores(); LeerColumnas(); PropuestaAutomatica(); HabilitarCalcular().
            ActualizarPuedeCalcular();
        }

        [RelayCommand]
        private void SeleccionarSalida()
        {
            // TODO Legacy RotacionDeSignosFrm.button2_Click:
            //   - SaveFileDialog filtro "Columnas(*.txt)"; FilterIndex==2 => salidaBinaria.
            //   - archivoSalida = ruta; HabilitarCalcular().
            ActualizarPuedeCalcular();
        }

        [RelayCommand]
        private void Transponer()
        {
            // TODO Legacy RotacionDeSignosFrm.button3_Click: transponer la matriz de cambios de signo.
        }

        [RelayCommand]
        private void Aceptar()
        {
            // TODO Legacy RotacionDeSignosFrm.btAceptar_Click:
            //   - PonerValoresEnVariables(); PonerValoracionEnVariables().
            //   - Recorrer BitArray Bits/BitsCambiados sustituyendo 1/X/2 por NuevosSignos[Partido,n].
            //   - Si chkGiros (correcciónDeFallos) aplica corrección de fallos.
            //   - Escribir archivoSalida (texto o binario según salidaBinaria).
        }

        [RelayCommand]
        private void Cancelar()
        {
            // TODO Legacy RotacionDeSignosFrm.btCancelar_Click: cerrar el formulario / cancelar proceso.
        }

        private void ActualizarPuedeCalcular()
        {
            // Legacy HabilitarCalcular(): habilita Aceptar si entrada y salida están seleccionadas.
            PuedeCalcular =
                FicheroEntrada != "(falta selección)" && !string.IsNullOrEmpty(FicheroEntrada) &&
                FicheroSalida != "(falta selección)" && !string.IsNullOrEmpty(FicheroSalida);
        }
    }
}
