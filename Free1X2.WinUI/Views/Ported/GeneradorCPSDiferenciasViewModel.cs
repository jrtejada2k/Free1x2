using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la página portada de <c>Free1X2.UI.GeneradorCPSDiferencias</c>
/// (Generador de CPs por diferencias).
///
/// Replica las entradas del WinForms legacy:
///  - txtColumna1 -> ColumnaInicial (cadena de 14 signos 1/X/2, mayúsculas)
///  - txtColumna2 -> ColumnaAlternativa (cadena de 14 signos 1/X/2, mayúsculas)
///  - txtArchivo  -> ArchivoDestino
///  - groupBox1 (rFijos/rDobles)  -> TipoColumnaIndex (0 = Fijos, 1 = Dobles)
///  - groupBox2 (rDif1/rDif2)     -> NumeroDiferenciasIndex (0 = 1, 1 = 2)
///
/// La generación (button2_Click) y la combinación (combinarFijos1/2, combinarDobles1/2,
/// esValida) son lógica autocontenida de strings copiada literalmente del legacy. La
/// persistencia usa Free1X2.EntradaSalida.ArchivoColumnasTexto.GuardarColsComa, igual
/// que el WinForms.
/// </summary>
public partial class GeneradorCPSDiferenciasViewModel : ObservableObject
{
    [ObservableProperty]
    private string _columnaInicial = string.Empty;

    [ObservableProperty]
    private string _columnaAlternativa = string.Empty;

    [ObservableProperty]
    private string _archivoDestino = string.Empty;

    // groupBox1 "Columnas": 0 = Fijos, 1 = Dobles. Legacy default rDobles.Checked = true.
    [ObservableProperty]
    private int _tipoColumnaIndex = 1;

    // groupBox2 "Diferencias": 0 = 1, 1 = 2. Legacy default rDif2.Checked = true.
    [ObservableProperty]
    private int _numeroDiferenciasIndex = 1;

    // Mensaje de estado para la barra inferior (sustituye los MessageBox legacy).
    [ObservableProperty]
    private string _estadoTexto = string.Empty;

    public IReadOnlyList<string> TiposColumna { get; } = new[] { "Fijos", "Dobles" };

    public IReadOnlyList<string> NumerosDiferencias { get; } = new[] { "1", "2" };

    /// <summary>
    /// Legacy GeneradorCPSDiferencias.button1_Click -> SaveFileDialog (fd). Abre un
    /// FileSavePicker con filtro "*.txt" y asigna la ruta a ArchivoDestino.
    /// </summary>
    [RelayCommand]
    private async Task Examinar()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            // Legacy fd.DefaultExt = ".txt".
            DefaultFileExtension = ".txt",
            SuggestedFileName = "doc1",
        };
        picker.FileTypeChoices.Add("Archivos de texto", new List<string> { ".txt" });
        picker.FileTypeChoices.Add("Todos los archivos", new List<string> { "." });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? archivo = await picker.PickSaveFileAsync();
        if (archivo is not null)
        {
            ArchivoDestino = archivo.Path;
        }
    }

    /// <summary>
    /// Legacy GeneradorCPSDiferencias.button2_Click: valida las dos columnas, genera la
    /// matriz de combinaciones según tipo (Fijos/Dobles) y nº de diferencias (1/2) y la
    /// guarda con ArchivoColumnasTexto.GuardarColsComa.
    /// </summary>
    [RelayCommand]
    private async Task Generar()
    {
        string cInicial = (ColumnaInicial ?? string.Empty).ToUpper();
        string cAlterna = (ColumnaAlternativa ?? string.Empty).ToUpper();

        // Validación idéntica al legacy.
        if (!EsValida(cInicial))
        {
            EstadoTexto = "La cadena inicial no es válida.";
            AppServices.MostrarError("La cadena inicial no es válida");
            return;
        }
        if (!EsValida(cAlterna))
        {
            EstadoTexto = "La cadena alternativa no es válida.";
            AppServices.MostrarError("La cadena alternativa no es válida");
            return;
        }
        if (!(ArchivoDestino.Length > 0))
        {
            EstadoTexto = "No se ha indicado el fichero.";
            AppServices.MostrarError("No se ha indicado el fichero.");
            return;
        }

        int tipoColumna = TipoColumnaIndex;          // 0 = Fijos, 1 = Dobles
        int numDif = NumeroDiferenciasIndex;         // 0 = 1, 1 = 2
        string archivo = ArchivoDestino;

        EstadoTexto = "Generando...";

        await Task.Run(() =>
        {
            string[]? columnas = null;
            IArchivoColumnas f = new ArchivoColumnasTexto(archivo);

            if (tipoColumna == 0) // rFijos.Checked
            {
                if (numDif == 0) // rDif1
                    columnas = CombinarFijos1(cInicial, cAlterna);
                else             // rDif2
                    columnas = CombinarFijos2(cInicial, cAlterna);
            }
            else // rDobles.Checked
            {
                if (numDif == 0) // rDif1
                    columnas = CombinarDobles1(cInicial, cAlterna);
                else             // rDif2
                    columnas = CombinarDobles2(cInicial, cAlterna);
            }

            // Guardamos las columnas de la matriz al archivo (igual que el legacy).
            for (int i = 0; i < columnas!.Length; i++)
            {
                f.GuardarColsComa(columnas[i]);
            }
            f.Cerrar();
        });

        EstadoTexto = "Columnas creadas.";
        AppServices.MostrarInfo("Columnas creadas");
    }

    /// <summary>
    /// Legacy GeneradorCPSDiferencias.button3_Click -> Close(). En WinUI limpia el estado;
    /// el cierre/navegación lo maneja el contenedor de la Page.
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        EstadoTexto = string.Empty;
    }

    // ===== Lógica autocontenida copiada literalmente del WinForms legacy =====

    // GeneradorCPSDiferencias.esValida
    private static bool EsValida(string cadena)
    {
        string valido = "1X2";
        if (cadena.Length != 14)
        {
            return false;
        }
        for (int i = 0; i < 14; i++)
        {
            if (Array.IndexOf(valido.ToCharArray(), cadena[i]) < 0)
            {
                return false;
            }
        }
        return true;
    }

    // GeneradorCPSDiferencias.combinarFijos1
    private static string[] CombinarFijos1(string c1, string c2)
    {
        string[] cadena = new string[15];
        for (int i = 0; i < 15; i++)
        {
            string tmp = "";
            for (int j = 0; j < 14; j++)
            {
                if (j == i)
                {
                    tmp += c2.Substring(j, 1) + ",";
                }
                else
                {
                    tmp += c1.Substring(j, 1) + ",";
                }
            }
            cadena[i] = tmp.Substring(0, tmp.Length - 1);
        }
        return cadena;
    }

    // GeneradorCPSDiferencias.combinarFijos2
    private static string[] CombinarFijos2(string c1, string c2)
    {
        string[] cadena = new string[92];
        int n = 0;
        string tmp = "";
        // Añadimos la cadena inicial
        for (int i = 0; i < 14; i++)
        {
            tmp += c1.Substring(i, 1) + ",";
        }
        cadena[n] = tmp.Substring(0, tmp.Length - 1);
        for (int i = 0; i < 15; i++)
        {
            for (int j = i + 1; j < 14; j++)
            {
                n++;
                tmp = "";
                for (int k = 0; k < 14; k++)
                {
                    if (k == i || k == j)
                    {
                        tmp += c2.Substring(k, 1) + ",";
                    }
                    else
                    {
                        tmp += c1.Substring(k, 1) + ",";
                    }
                }
                // Al asignar a la matriz, quitamos la ","
                cadena[n] = tmp.Substring(0, tmp.Length - 1);
            }
        }
        return cadena;
    }

    // GeneradorCPSDiferencias.combinarDobles1
    private static string[] CombinarDobles1(string c1, string c2)
    {
        string valido = "1X2";
        string cM = "";
        string[] cadena = new string[15];

        // Establecemos la cadena intermedia
        for (int i = 0; i < 14; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if ((c1.Substring(i, 1) != valido.Substring(j, 1)) && (c2.Substring(i, 1) != valido.Substring(j, 1)))
                {
                    cM += valido.Substring(j, 1);
                }
            }
        }
        for (int i = 0; i < 15; i++)
        {
            string tmp = "";
            for (int j = 0; j < 14; j++)
            {
                if (j == i)
                {
                    tmp += c2.Substring(j, 1) + cM.Substring(j, 1) + ",";
                }
                else
                {
                    tmp += c1.Substring(j, 1) + cM.Substring(j, 1) + ",";
                }
            }
            // Al asignar a la matriz, quitamos la ","
            cadena[i] = tmp.Substring(0, tmp.Length - 1);
        }
        return cadena;
    }

    // GeneradorCPSDiferencias.combinarDobles2
    private static string[] CombinarDobles2(string c1, string c2)
    {
        string valido = "1X2";
        string cM = "";
        string[] cadena = new string[92];
        int n = 0;

        // Establecemos la cadena intermedia
        for (int i = 0; i < 14; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if ((c1.Substring(i, 1) != valido.Substring(j, 1)) && (c2.Substring(i, 1) != valido.Substring(j, 1)))
                {
                    cM += valido.Substring(j, 1);
                }
            }
        }

        // Añadimos la cadena inicial
        string tmp = "";
        for (int k = 0; k < 14; k++)
        {
            tmp += c1.Substring(k, 1) + cM.Substring(k, 1) + ",";
        }
        // Al asignar a la matriz, quitamos la ","
        cadena[n] = tmp.Substring(0, tmp.Length - 1);

        for (int i = 0; i < 15; i++)
        {
            for (int j = i + 1; j < 14; j++)
            {
                n++;
                tmp = "";
                for (int k = 0; k < 14; k++)
                {
                    if (k == i || k == j)
                    {
                        tmp += c2.Substring(k, 1) + cM.Substring(k, 1) + ",";
                    }
                    else
                    {
                        tmp += c1.Substring(k, 1) + cM.Substring(k, 1) + ",";
                    }
                }
                // Al asignar a la matriz, quitamos la ","
                cadena[n] = tmp.Substring(0, tmp.Length - 1);
            }
        }
        return cadena;
    }
}
