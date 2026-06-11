using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        [RelayCommand]
        private void Abrir()
        {
            // Legacy: ModificadorFrm.AbrirClick(...)
            //   OpenFileDialog (InitialDirectory "Columnas\\", filtro "*.txt").
            //   En WinUI usar FileOpenPicker.
            // TODO dominio: con el fichero elegido:
            //   IArchivoColumnas aCol = new ArchivoColumnasTexto(ruta);  // Free1X2.EntradaSalida
            //   noPartidos = aCol.ObtenNumSignos();                      // ajustar nº de partidos
            //   apuestas   = aCol.ObtenNumCols();                        // nº de columnas
            //   LeeFichero(ruta);   // rellena la matriz frecuencia[partido,signo] y la pasa a %
            //   -> volcar esos porcentajes a Partidos[i].Valor1/ValorX/Valor2
            //      (legacy AccionModificador(Escribir)).
            //   FicheroNombre = Path.GetFileName(ruta);
            //   FicheroCargado = true;
        }

        [RelayCommand]
        private void Aceptar()
        {
            // Legacy: ModificadorFrm.BtnOkClick(...)
            //   AccionModificador(Modificar) -> ModificarFrecuencia(...) por cada partido activo.
            //   AccionModificador(Escribir)  -> reescribe los porcentajes mostrados.
            //   Luego pregunta si grabar (-> BtnGrabarClick).
            //
            //   Mapeo DistribucionSeleccionada -> opcion:
            //     "Aleatorio"    => 1
            //     "Proporcional" => 2
            //     "Ordenado"     => 3
            //   OrdenarSignos -> checkOrdenar.Checked (solo aplica si opcion > 1).
            //
            // TODO dominio: portar ModificadorFrm.ModificarFrecuencia / procesarGrupos /
            //   cambiarSigno / ordenar, que reparten los signos de cada partido segun
            //   los porcentajes y el modo elegido sobre el array 'columnas'.
            //   Validar que cada partido sume 100 (avisos legacy con MessageBox).
        }

        [RelayCommand]
        private void Grabar()
        {
            // Legacy: ModificadorFrm.BtnGrabarClick(...)
            //   SaveFileDialog con nombre por defecto "<fichero>_modificado.txt", filtro "*.txt".
            //   En WinUI usar FileSavePicker.
            // TODO dominio: GuardaFichero(ruta):
            //   IArchivoColumnas archComb = new ArchivoColumnasTexto(ruta);
            //   archComb.GuardarTodasCols(columnas); archComb.Cerrar();
            //   Mostrar confirmación "Se ha grabado el archivo." (ContentDialog en WinUI).
        }

        [RelayCommand]
        private void Cancelar()
        {
            // Legacy: ModificadorFrm.BtnCancelClick(...) -> this.Close();
            // TODO dominio: cerrar la página / navegar atrás sin grabar.
        }
    }
}
