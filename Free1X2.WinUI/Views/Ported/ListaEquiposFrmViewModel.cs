using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para la Page portada de WinForms 'ListaEquiposFrm'.
    /// El form original mostraba un ListBox ordenado con los equipos cargados
    /// desde el fichero "Equipos/equipos{categoria}.dat" y, al hacer doble clic,
    /// volcaba el nombre seleccionado en un TextBox del formulario padre.
    /// </summary>
    public partial class ListaEquiposFrmViewModel : ObservableObject
    {
        // Origen completo cargado de disco (antes de aplicar el filtro de búsqueda).
        private readonly List<string> _fuenteCompleta = new();

        // Categoría recibida en el ctor del form legacy (ListaEquiposFrm(TextBox, string categoria)).
        // Es el sufijo del fichero: "1", "2", "2b", "Int".
        [ObservableProperty]
        private string _categoria = string.Empty;

        // Equipo actualmente seleccionado en la lista (legacy: listBox1.SelectedItem).
        [ObservableProperty]
        private string? _equipoSeleccionado;

        // Texto de filtro para localizar equipos en la lista (mejora sobre el ListBox legacy).
        [ObservableProperty]
        private string _textoBusqueda = string.Empty;

        // Mensaje de estado / error (legacy mostraba un MessageBox si no existía el .dat).
        [ObservableProperty]
        private string _mensajeEstado = string.Empty;

        // Lista (posiblemente filtrada) mostrada en pantalla (legacy: contenido del .dat, Sorted=true).
        public ObservableCollection<string> Equipos { get; } = new();

        // Indica si hay una selección válida para confirmar.
        public bool HaySeleccion => !string.IsNullOrEmpty(EquipoSeleccionado);

        /// <summary>
        /// Resultado confirmado por el usuario (legacy: txt.Text = listBox1.SelectedItem).
        /// La página host lo consume para volcarlo al control destino y cerrar/ocultar.
        /// </summary>
        public string? ResultadoSeleccionado { get; private set; }

        /// <summary>
        /// Se dispara al confirmar una selección (legacy: this.Hide() tras volcar el texto).
        /// El argumento es el nombre del equipo elegido.
        /// </summary>
        public event EventHandler<string>? EquipoConfirmado;

        partial void OnEquipoSeleccionadoChanged(string? value) => OnPropertyChanged(nameof(HaySeleccion));

        partial void OnTextoBusquedaChanged(string value) => AplicarFiltro();

        partial void OnCategoriaChanged(string value) => Cargar();

        /// <summary>
        /// Ruta del fichero de equipos de la categoría actual.
        /// Legacy: Application.StartupPath + "/Equipos/equipos" + cat + ".dat".
        /// En WinUI el directorio de inicio equivale a AppContext.BaseDirectory.
        /// </summary>
        private string ArchivoCategoria =>
            AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + "/Equipos/equipos" + Categoria + ".dat";

        /// <summary>
        /// Carga los equipos de la categoría actual.
        /// Legacy: ListaEquiposFrm_Load -> File.Exists(StartupPath + "/Equipos/equipos" + cat + ".dat"),
        /// lectura línea a línea con StreamReader (Encoding.Default), Trim por línea, ListBox.Sorted.
        /// </summary>
        [RelayCommand]
        private void Cargar()
        {
            _fuenteCompleta.Clear();
            Equipos.Clear();
            MensajeEstado = string.Empty;

            string fichero = ArchivoCategoria;

            // Legacy: if (File.Exists(...)) { leer } else { MessageBox "No se encuentra el archivo" }.
            if (!File.Exists(fichero))
            {
                MensajeEstado = "No se encuentra el archivo " + fichero;
                return;
            }

            try
            {
                // Legacy HaySiguiente/LeeEquipos: StreamReader(fichero, Encoding.Default), Trim por línea.
                // Encoding.Latin1 ≡ Encoding.Default de WinForms en el Windows español.
                using var sr = new StreamReader(fichero, Encoding.Latin1);
                while (sr.Peek() >= 0)
                {
                    string? linea = sr.ReadLine();
                    if (linea is not null)
                    {
                        _fuenteCompleta.Add(linea.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                MensajeEstado = "Error al leer " + fichero + ": " + ex.Message;
                return;
            }

            AplicarFiltro();
        }

        /// <summary>
        /// Confirma la selección actual.
        /// Legacy: listBox1_DoubleClick -> txt.Text = listBox1.SelectedItem.ToString(); this.Hide();
        /// </summary>
        [RelayCommand]
        private void Seleccionar()
        {
            if (!HaySeleccion)
                return;

            // Legacy: volcaba el nombre al TextBox padre y ocultaba la ventana. En WinUI lo
            // publicamos para que la página host lo entregue al control destino y cierre/oculte.
            ResultadoSeleccionado = EquipoSeleccionado;
            EquipoConfirmado?.Invoke(this, EquipoSeleccionado!);
        }

        /// <summary>
        /// Reaplica el filtro de búsqueda sobre la fuente completa y mantiene el orden
        /// alfabético (legacy: ListBox.Sorted = true).
        /// </summary>
        private void AplicarFiltro()
        {
            IEnumerable<string> resultado = _fuenteCompleta;

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                resultado = resultado.Where(e =>
                    e.Contains(TextoBusqueda, StringComparison.CurrentCultureIgnoreCase));
            }

            // Legacy: listBox1.Sorted = true.
            var ordenados = resultado
                .OrderBy(e => e, StringComparer.CurrentCulture)
                .ToList();

            Equipos.Clear();
            foreach (string equipo in ordenados)
            {
                Equipos.Add(equipo);
            }
        }
    }
}
