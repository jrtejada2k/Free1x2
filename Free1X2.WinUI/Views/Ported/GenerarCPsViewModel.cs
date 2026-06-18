// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.EntradaSalida.GenerarCPs;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de valoración por partido (signos 1 / X / 2 en porcentaje).
/// Réplica del UserControl legacy Free1X2.UI.Controls.GeneradorOptions,
/// que exponía Valor_1 / Valor_X / Valor_2 (string) + NumeroPartido.
/// </summary>
public partial class ValoracionPartidoItem : ObservableObject
{
    [ObservableProperty]
    private int _numeroPartido;

    [ObservableProperty]
    private double _valor1;

    [ObservableProperty]
    private double _valorX;

    [ObservableProperty]
    private double _valor2;

    // Texto de cabecera del partido para el TextBlock (regla anti-crash 2:
    // no se bindea el int directo a Text).
    public string NumeroPartidoTexto => NumeroPartido.ToString();
}

/// <summary>
/// ViewModel de la Page portada desde el WinForms "GenerarCPs"
/// (Generador de Columnas Probables).
///
/// La generación (btnOK_Click) está cableada al motor real (Free1X2.EntradaSalida.GenerarCPs:
/// DatosHelper, ColumnasProbables, CPs, IO). La importación de % usa Free1X2.Utils.Porcentajes
/// (ya portada al dominio: detección de formato 1/3/42/43/44 valores por fila). La navegación
/// a ConfigCPsFrm / FiltroPorcenJB / GeneradorCPSDiferencias se cablea con la acción Navegar.
/// </summary>
public partial class GenerarCPsViewModel : ObservableObject
{
    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    public GenerarCPsViewModel()
    {
        Valoraciones = new ObservableCollection<ValoracionPartidoItem>();
        for (int i = 1; i <= VariablesGlobales.NumeroPartidos; i++)
        {
            Valoraciones.Add(new ValoracionPartidoItem { NumeroPartido = i });
        }
    }

    /// <summary>
    /// Lista editable de valoraciones (1/X/2) por partido. Equivale al
    /// panelPartidos con N controles GeneradorOptions del legacy.
    /// </summary>
    public ObservableCollection<ValoracionPartidoItem> Valoraciones { get; }

    // Jornada destino. En el legacy era el NumTextBox 'numJornada'.
    [ObservableProperty]
    private double _jornada;

    // Mensaje de estado para feedback al usuario (sustituye los MessageBox).
    [ObservableProperty]
    private string _estado = string.Empty;

    /// <summary>
    /// btnOK_Click del legacy: valida la jornada, copia la valoración a la
    /// matriz y genera/guarda los ficheros de columnas probables en Condiciones/.
    /// </summary>
    [RelayCommand]
    private async Task Generar()
    {
        int jornada = (int)Jornada;
        if (jornada <= 0)
        {
            Estado = "Especifique un número de jornada.";
            AppServices.MostrarError("Especifique un número de jornada");
            return;
        }

        // Copiamos la valoración a la matriz (legacy CopiarValoracion()).
        Valoracion[] valores = CopiarValoracion();

        Estado = "Generando columnas...";

        try
        {
            int ficheros = await Task.Run(() =>
            {
                // Cargamos los datos de las columnas en memoria.
                var dh = new DatosHelper();
                ColumnasProbables dsConfCol = dh.ObtenerDatos();

                // Carpeta Condiciones bajo el directorio base de la app (igual semántica que
                // el legacy Application.StartupPath + "/Condiciones/").
                string baseDir = AppContext.BaseDirectory.TrimEnd(
                    Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string carpeta = Path.Combine(baseDir, "Condiciones");
                Directory.CreateDirectory(carpeta);

                int generados = 0;
                IO f2;
                for (int i = 0; i < dsConfCol.Tables["Tipos de CPs"]!.Rows.Count; i++)
                {
                    var dv = new DataView(dsConfCol.Tables["Configuracion de CPs"]);
                    string filtro = "Tipo = " + dsConfCol.Tables["Tipos de CPs"]!.Rows[i]["Tipo"];
                    dv.RowFilter = filtro;
                    DataSet ds = LlenarDataset(dv, valores);
                    string txt = LlenarTxtColumnas(ds);
                    string nombre = dsConfCol.Tables["Tipos de CPs"]!.Rows[i]["Nombre"].ToString()!;
                    string fichero = Path.Combine(carpeta, nombre + "_j" + jornada + ".txt");
                    fichero.Replace(" ", "_"); // (literal del legacy: no reasigna; se conserva el comportamiento)
                    f2 = new IO(fichero);
                    f2.GuardarTexto(txt);
                    f2.Cerrar();
                    generados++;
                }
                return generados;
            });

            Estado = $"Fichero(s) guardado(s) correctamente en la carpeta Condiciones ({ficheros}).";
            AppServices.MostrarInfo("Fichero(s) guardado(s) correctamente en la carpeta Condiciones");
        }
        catch (Exception ex)
        {
            Estado = "Error al generar las columnas: " + ex.Message;
            AppServices.MostrarError("Error al generar las columnas: " + ex.Message);
        }
    }

    /// <summary>
    /// btnImportarVal_Click del legacy: abre un .txt de valoración. El parseo se hacía con
    /// Free1X2.Utils.Porcentajes (detección de formato), no portada al dominio.
    /// </summary>
    [RelayCommand]
    private async Task ImportarPorcentajes()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? archivo = await picker.PickSingleFileAsync();
        if (archivo is null)
        {
            return;
        }

        // Réplica de btnImportarVal_Click (Free1X2/UI/GenerarCPs.cs línea 271):
        //   new Porcentajes(fileName) detecta el formato (1/3/42/43/44 valores por fila) al
        //   leer; luego se vuelca Pct.valores[i,0..2] a la valoración por partido. La clase
        //   Free1X2.Utils.Porcentajes vive ahora en el dominio (Free1X2.Domain/Utils).
        try
        {
            double[,] leidos = await Task.Run(() =>
            {
                var pct = new Porcentajes(archivo.Path);
                return pct.valores;
            });

            // Equivalente a PonerValoracionPantalla(valores) (línea 295): se vuelca a la
            // colección que pinta la rejilla de partidos. La matriz Valoraciones se reconstruye
            // luego en Generar() vía CopiarValoracion(), igual que el legacy CopiarValoracion().
            for (int i = 0; i < Valoraciones.Count && i < VariablesGlobales.NumeroPartidos; i++)
            {
                Valoraciones[i].Valor1 = leidos[i, 0];
                Valoraciones[i].ValorX = leidos[i, 1];
                Valoraciones[i].Valor2 = leidos[i, 2];
            }

            Estado = $"Valoración importada de {archivo.Name}.";
        }
        catch (Exception ex)
        {
            Estado = "Error al importar la valoración: " + ex.Message;
            AppServices.MostrarError("Error al importar la valoración: " + ex.Message);
        }
    }

    /// <summary>
    /// btnConfigurar_Click del legacy: abría el diálogo ConfigCPsFrm.
    /// </summary>
    [RelayCommand]
    private void ConfigurarColumnas()
    {
        // Legacy: ConfigCPsFrm f = new ConfigCPsFrm(); f.ShowDialog();
        Navegar?.Invoke(typeof(ConfigCPsFrmPage));
    }

    /// <summary>
    /// btnSeparador_Click del legacy: abría el diálogo FiltroPorcenJB.
    /// </summary>
    [RelayCommand]
    private void SeparadorPorcentajes()
    {
        // Legacy: FiltroPorcenJB separador = new FiltroPorcenJB(); separador.ShowDialog();
        Navegar?.Invoke(typeof(FiltroPorcenJBPage));
    }

    /// <summary>
    /// btnDiferencias_Click del legacy: abría GeneradorCPSDiferencias.
    /// </summary>
    [RelayCommand]
    private void CpsPorDiferencias()
    {
        // Legacy: GeneradorCPSDiferencias f = new GeneradorCPSDiferencias(); f.ShowDialog();
        Navegar?.Invoke(typeof(GeneradorCPSDiferenciasPage));
    }

    // ===== Lógica de dominio replicada del WinForms GenerarCPs =====

    /// <summary>Legacy CopiarValoracion(): vuelca las valoraciones de pantalla a la matriz.</summary>
    private Valoracion[] CopiarValoracion()
    {
        var valores = new Valoracion[VariablesGlobales.NumeroPartidos];
        foreach (ValoracionPartidoItem item in Valoraciones)
        {
            int numPartido = item.NumeroPartido - 1;
            if (numPartido < 0 || numPartido >= valores.Length) continue;
            var valTemp = new Valoracion
            {
                Uno = item.Valor1,
                Equis = item.ValorX,
                Dos = item.Valor2,
            };
            valores[numPartido] = valTemp;
        }
        return valores;
    }

    /// <summary>Legacy LlenarDataset(DataView): genera las CPs con CPs.CrearCPs y arma el DataSet.</summary>
    private static DataSet LlenarDataset(DataView dv, Valoracion[] valores)
    {
        var cps = new CPs();
        string[,] columnas = cps.CrearCPs(dv, valores);
        return LlenarDataset(columnas);
    }

    /// <summary>Legacy LlenarDataset(string[,]): convierte la matriz de columnas en un DataSet.</summary>
    private static DataSet LlenarDataset(string[,] columnas)
    {
        int i;
        int longitud = columnas.Length / VariablesGlobales.NumeroPartidos;
        var myDataTable = new DataTable();
        for (i = 0; i < longitud; i++)
        {
            var myDataColumn = new DataColumn
            {
                DataType = Type.GetType("System.String")!,
                ColumnName = i.ToString(),
            };
            myDataTable.Columns.Add(myDataColumn);
        }
        for (i = 0; i < VariablesGlobales.NumeroPartidos; i++)
        {
            DataRow myDataRow = myDataTable.NewRow();
            for (int j = 0; j < longitud; j++)
            {
                string col = columnas[i, j] ?? " ";
                myDataRow[j.ToString()] = col;
            }
            myDataTable.Rows.Add(myDataRow);
        }

        var myDataSet = new DataSet();
        myDataSet.Tables.Add(myDataTable);
        return myDataSet;
    }

    /// <summary>Legacy LlenarTxtColumnas(): serializa el DataSet de columnas a texto (CSV por partido).</summary>
    private static string LlenarTxtColumnas(DataSet dsCPs)
    {
        string txt = "";
        const string nl = "\r\n";
        for (int i = 0; i < dsCPs.Tables[0].Columns.Count; i++)
        {
            for (int j = 0; j < VariablesGlobales.NumeroPartidos; j++)
            {
                txt += dsCPs.Tables[0].Rows[j][i].ToString();
                if (j < VariablesGlobales.NumeroPartidos - 1)
                {
                    txt += ",";
                }
            }
            txt += nl;
        }
        return txt;
    }
}
