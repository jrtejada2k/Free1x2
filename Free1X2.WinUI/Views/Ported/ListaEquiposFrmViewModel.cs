using System.Collections.ObjectModel;
using System.Collections.Generic;
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
        // Categoría recibida en el ctor del form legacy (ListaEquiposFrm(TextBox, string categoria)).
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

        // Lista completa de equipos cargada desde disco (legacy: contenido del .dat, Sorted=true).
        public ObservableCollection<string> Equipos { get; } = new();

        // Indica si hay una selección válida para confirmar.
        public bool HaySeleccion => !string.IsNullOrEmpty(EquipoSeleccionado);

        partial void OnEquipoSeleccionadoChanged(string? value) => OnPropertyChanged(nameof(HaySeleccion));

        partial void OnTextoBusquedaChanged(string value) => AplicarFiltro();

        /// <summary>
        /// Carga los equipos de la categoría actual.
        /// Legacy: ListaEquiposFrm_Load -> File.Exists(StartupPath + "/Equipos/equipos" + cat + ".dat"),
        /// lectura línea a línea con StreamReader (Encoding.Default), Trim por línea, ListBox.Sorted.
        /// </summary>
        [RelayCommand]
        private void Cargar()
        {
            Equipos.Clear();
            MensajeEstado = string.Empty;

            // TODO [dominio]: portar ListaEquiposFrm.ListaEquiposFrm_Load / HaySiguiente / LeeEquipos.
            // 1. Resolver ruta del fichero "Equipos/equipos{Categoria}.dat" (legacy usaba Application.StartupPath).
            // 2. Si no existe -> MensajeEstado = "No se encuentra el archivo ...".
            // 3. Leer líneas (Encoding.Default), Trim, añadir a Equipos y ordenar alfabéticamente.
            // No se implementa aquí: acceso a disco / persistencia es lógica de dominio.
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

            // TODO [dominio]: portar ListaEquiposFrm.listBox1_DoubleClick.
            // El form legacy volcaba EquipoSeleccionado en el TextBox padre (campo 'txt') y ocultaba la ventana.
            // En WinUI esto debe devolverse al llamante (p. ej. navegación/diálogo) — no se implementa aquí.
        }

        private void AplicarFiltro()
        {
            // TODO [dominio]: re-aplicar el filtro TextoBusqueda sobre la fuente completa de equipos
            // tras portar la carga real desde disco. Aquí solo se conserva la intención del filtro.
        }
    }
}
