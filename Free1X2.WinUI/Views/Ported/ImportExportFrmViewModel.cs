// Free1X2 · WinUI 3 — WIN3
using System;
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
    /// ViewModel para ImportExportFrmPage.
    /// Porta el WinForms legacy "ImportExportFrm" (UI/ImportExportFrm.cs):
    /// herramienta de conversion de ficheros de columnas entre CSV y TXT.
    /// </summary>
    public partial class ImportExportFrmViewModel : ObservableObject
    {
        // Columnas leidas del fichero de entrada (legacy: string[] columnas).
        private string[]? _columnas;

        // Opciones de conversion (legacy: RadioButton rConv3 / rConv4).
        // Indice 0 -> CSV a TXT (legacy rConv3, conversion=2, checked por defecto).
        // Indice 1 -> TXT a CSV (legacy rConv4, conversion=3).
        public IReadOnlyList<string> OpcionesConversion { get; } = new[]
        {
            "Separadas por comas (*.csv) a TXT (*.txt)",
            "TXT (*.txt) a Separadas por comas (*.csv)"
        };

        [ObservableProperty]
        private int _conversionSeleccionada = 0;

        // Fichero de entrada (legacy: txFicheroEntrada).
        [ObservableProperty]
        private string _ficheroEntrada = string.Empty;

        // Fichero de salida (legacy: txtFicheroSalida).
        [ObservableProperty]
        private string _ficheroSalida = string.Empty;

        // Contadores informativos (legacy: lblColsEntrada / lblColsSalida).
        [ObservableProperty]
        private string _colsEntradaTexto = string.Empty;

        [ObservableProperty]
        private string _colsSalidaTexto = string.Empty;

        // Mensaje de estado para sustituir los MessageBox del legacy.
        [ObservableProperty]
        private string _estadoTexto = string.Empty;

        // Mapea el indice del ComboBox al valor 'conversion' del legacy:
        // indice 0 (csv->txt) -> conversion 2; indice 1 (txt->csv) -> conversion 3.
        // (Legacy cambiarConversion: rConv3 checked => 2, rConv4 => 3.)
        private int ConversionLegacy => ConversionSeleccionada == 0 ? 2 : 3;

        // Legacy: btnAbrirEntrada_Click. Abre OpenFileDialog filtrando por
        // csv/txt segun la conversion, lee las columnas y rellena el contador.
        [RelayCommand]
        private async Task AbrirEntradaAsync()
        {
            // Legacy: filtro segun conversion (2 -> csv, otro -> txt).
            string filtro = ConversionLegacy == 2 ? ".csv" : ".txt";
            var file = await AbrirSelectorAsync(filtro);
            if (file == null) return;

            FicheroEntrada = file.Path;
            ColsEntradaTexto = string.Empty;
            EstadoTexto = string.Empty;

            string rutaEntrada = file.Path;
            int conversion = ConversionLegacy;
            try
            {
                // La lectura recorre el fichero completo: fuera del hilo de UI.
                string[] columnas = await Task.Run(() =>
                {
                    IArchivoColumnas cols = new ArchivoColumnasTexto(rutaEntrada);
                    try
                    {
                        // Legacy: conversion==2 -> LeerTodasColsSeparadasPorComas(); else LeerTodasCols(true).
                        return conversion == 2
                            ? cols.LeerTodasColsSeparadasPorComas()
                            : cols.LeerTodasCols(true);
                    }
                    finally
                    {
                        cols.Cerrar();
                    }
                });

                _columnas = columnas;

                // Legacy: propone el fichero de salida cambiando la extension.
                FicheroSalida = conversion == 2
                    ? rutaEntrada.ToLower().Replace(".csv", ".txt")
                    : rutaEntrada.ToLower().Replace(".txt", ".csv");

                ColsEntradaTexto = columnas.Length + " columnas.";
            }
            catch
            {
                _columnas = null;
                ColsEntradaTexto = string.Empty;
                // Legacy: MessageBox "No se ha podido leer el fichero de entrada".
                AppServices.MostrarError("No se ha podido leer el fichero de entrada");
            }
        }

        // Legacy: btnAbrirSalida_Click. Selecciona el fichero de salida.
        [RelayCommand]
        private async Task AbrirSalidaAsync()
        {
            // Legacy btnAbrirSalida_Click usa 'conversion'; con el ComboBox actual el valor
            // efectivo es 2 (csv->txt, salida .txt) o 3 (txt->csv, salida .csv).
            string extension = ConversionLegacy == 2 ? ".txt" : ".csv";

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "Columnas",
            };
            picker.FileTypeChoices.Add("Columnas", new List<string> { extension });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;

            FicheroSalida = file.Path;
        }

        // Legacy: btnOk_Click ("Hacer"). Ejecuta la conversion y guarda.
        [RelayCommand]
        private async Task HacerAsync()
        {
            // Legacy: if (txtFicheroSalida.Text.Length == 0) return;
            if (string.IsNullOrEmpty(FicheroSalida)) return;

            // Legacy: avisa si no hay columnas cargadas.
            if (_columnas == null || _columnas.Length == 0)
            {
                AppServices.MostrarError("No se ha cargado el fichero de entrada o no tiene columnas.");
                return;
            }

            // Legacy: los archivos de entrada y salida deben ser distintos.
            if (FicheroEntrada == FicheroSalida)
            {
                AppServices.MostrarError("Los archivos de entrada y salida deben ser distintos");
                return;
            }

            ColsSalidaTexto = string.Empty;

            // Legacy: confirma sobreescritura si el fichero de salida ya existe.
            // (El FileSavePicker ya confirma; esto cubre el caso de ruta escrita a mano.)
            if (File.Exists(FicheroSalida))
            {
                bool sobreescribir = await ConfirmarSobreescrituraAsync();
                if (!sobreescribir) return;
            }

            string rutaSalida = FicheroSalida;
            int conversion = ConversionLegacy;
            string[] columnas = _columnas;
            try
            {
                long numCols = await Task.Run(() =>
                {
                    IArchivoColumnas cols = new ArchivoColumnasTexto(rutaSalida);
                    // Legacy: case 2 -> GuardarTodasCols(columnas, false) (TXT sin comas);
                    //         default (3) -> GuardarTodasCols(columnas, true) (CSV con comas).
                    cols.GuardarTodasCols(columnas, conversion != 2);
                    cols.Cerrar();
                    return cols.ObtenNumCols();
                });

                ColsSalidaTexto = numCols + " columnas.";
                EstadoTexto = "Transformación finalizada";
                AppServices.MostrarInfo("Transformación finalizada");
            }
            catch (Exception ex)
            {
                EstadoTexto = "Error: " + ex.Message;
                AppServices.MostrarError("No se ha podido guardar el fichero de salida");
            }
        }

        // Legacy: btnCancel_Click. Cierra el formulario.
        [RelayCommand]
        private void Cancelar()
        {
            // Legacy: this.Close(). En WinUI, la navegación atrás/cierre es responsabilidad
            // del host de la Page (Frame.GoBack), no del cableado de dominio.
        }

        /// <summary>
        /// Confirma la sobreescritura del fichero de salida con un ContentDialog.
        /// Equivale al MessageBox OKCancel del legacy (btnOk_Click). Se ejecuta en el hilo de UI
        /// (el comando ya se invoca desde la UI). Si no hay ventana/XamlRoot, asume confirmación.
        /// </summary>
        private static async Task<bool> ConfirmarSobreescrituraAsync()
        {
            var root = AppServices.MainWindow?.Content?.XamlRoot;
            if (root is null) return true; // sin UI disponible: comportamiento por defecto (proceder)

            var dlg = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = "Free1X2",
                Content = "Ya existe el fichero de salida. ¿Sobreescribir?",
                PrimaryButtonText = "Sobreescribir",
                CloseButtonText = "Cancelar",
                DefaultButton = Microsoft.UI.Xaml.Controls.ContentDialogButton.Close,
                XamlRoot = root,
            };
            var resultado = await dlg.ShowAsync();
            return resultado == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary;
        }

        /// <summary>Abre un FileOpenPicker para un tipo de archivo concreto (*.csv o *.txt).</summary>
        private static async Task<Windows.Storage.StorageFile?> AbrirSelectorAsync(string extension)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.FileTypeFilter.Add(extension);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);
            return await picker.PickSingleFileAsync();
        }
    }
}
