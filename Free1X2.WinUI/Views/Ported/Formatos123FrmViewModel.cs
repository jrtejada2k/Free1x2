// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de formato 123 dentro de la lista (equivale al control legacy
/// Free1X2.UI.Controls.CtrlFormato123).
/// </summary>
public partial class Formato123Item : ObservableObject
{
    [ObservableProperty]
    private string _numero = string.Empty;

    [ObservableProperty]
    private string _formato = string.Empty;

    [ObservableProperty]
    private string _aciertosMin = string.Empty;

    [ObservableProperty]
    private string _aciertosMax = string.Empty;
}

/// <summary>
/// ViewModel portado de Free1X2.UI.Filtros.Formatos123Frm (WinForms).
/// Replica los campos de entrada: lista de formatos 123 (con min/max aciertos),
/// líneas acertadas, "ignorar repeticiones", la rejilla de porcentajes/valoración
/// y el traductor 1X2 -> 123. Persistencia (Guardar/Abrir/Copiar/Pegar) vía
/// ArchivoCondiciones (.123/.xml + Temp/tmp.123) y Estadísticas -> VisorEstadisticasPage.
/// </summary>
public partial class Formatos123FrmViewModel : ObservableObject
{
    public Formatos123FrmViewModel()
    {
        // Legacy: LlenarControles(100) precargaba 100 filas vacías.
        // Aquí arrancamos con unas pocas filas editables; AgregarFila añade más.
        for (int i = 1; i <= 8; i++)
        {
            Formatos.Add(new Formato123Item { Numero = i.ToString() });
        }
    }

    /// <summary>
    /// Filas de formato (legacy: cctrl con N CtrlFormato123).
    /// </summary>
    public ObservableCollection<Formato123Item> Formatos { get; } = new();

    /// <summary>
    /// Rejilla de valoraciones 1/X/2 por partido (reemplaza el UserControl WinForms
    /// ControlPorcentajes / controlPorcentajes1). PorcentajesHelper.AMatriz(Porcentajes)
    /// equivale a controlPorcentajes1.Valores (matriz double[NumeroPartidos,3]).
    /// </summary>
    public ObservableCollection<FilaPorcentaje> Porcentajes { get; } =
        PorcentajesHelper.Crear(VariablesGlobales.NumeroPartidos);

    /// <summary>
    /// Legacy: txtAciertos. Lista de líneas acertadas permitidas (ej. "10,12-14").
    /// </summary>
    [ObservableProperty]
    private string _aciertosPermitidos = string.Empty;

    /// <summary>
    /// Legacy: chckPasoFijo "Ignorar Repeticiones". Cuando está activo, la
    /// etiqueta de aciertos pasa de "Líneas Acertadas" a "Aciertos Globales"
    /// y se ignoran los min/max por fila.
    /// </summary>
    [ObservableProperty]
    private bool _ignorarRepeticiones;

    /// <summary>
    /// Etiqueta dinámica del grupo de aciertos (legacy: label41 cambia de texto
    /// en DeterminarRepeticiones).
    /// </summary>
    public string EtiquetaAciertos =>
        IgnorarRepeticiones ? "Aciertos Globales" : "Líneas Acertadas";

    partial void OnIgnorarRepeticionesChanged(bool value)
    {
        OnPropertyChanged(nameof(EtiquetaAciertos));
    }

    /// <summary>
    /// Legacy: txtColumna1x2 (entrada del traductor).
    /// </summary>
    [ObservableProperty]
    private string _columna1X2 = string.Empty;

    /// <summary>
    /// Legacy: txtColumna123 (salida del traductor, solo lectura).
    /// </summary>
    [ObservableProperty]
    private string _columna123 = string.Empty;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    /// <summary>Acción para navegar a otra página (la cablea la página con Frame.Navigate(tipo)).</summary>
    public Action<Type>? Navegar { get; set; }

    // Fichero temporal de copiar/pegar (legacy: StartupPath + "/Temp/tmp.123").
    private static string RutaTemporal =>
        Path.Combine(AppContext.BaseDirectory, "Temp", "tmp.123");

    // Directorio de columnas ganadoras (legacy: StartupPath + "/Ganadoras/").
    private static string DirectorioGanadoras =>
        Path.Combine(AppContext.BaseDirectory, "Ganadoras") + Path.DirectorySeparatorChar;

    /// <summary>
    /// Vuelca los formatos del FiltroFormatos123 del grupo en edición a la pantalla.
    /// Equivale a Formatos123Frm.MarcarValores() (Free1X2/UI/Filtros/Formatos123Frm.cs líneas 562-613).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroFormatos123)grupo.GetFiltro(Filtro.Formatos123.ToString());
        if (!filtro.ContieneDatos) return;

        MarcarValores(filtro);
    }

    // Vuelca un FiltroFormatos123 a la pantalla (MarcarValores(FiltroFormatos123) legacy, líneas 574-580).
    private void MarcarValores(FiltroFormatos123 filtro)
    {
        // Volcar la valoración del filtro a la rejilla de porcentajes
        // (equivale a Formatos123Frm.MarcarValoracion(filtro.Valoracion), línea 632-635).
        if (filtro.Valoracion is { } val && val.GetLength(0) > 0)
            PorcentajesHelper.CargarMatriz(Porcentajes, val);

        Formatos.Clear();
        int n = 1;
        foreach (Formato123 f in filtro.ArrayFormatos)
        {
            Formatos.Add(new Formato123Item
            {
                Numero = n.ToString(),
                Formato = f.Formato,
                AciertosMin = f.AciertosMin.ToString(),
                AciertosMax = f.AciertosMax.ToString(),
            });
            n++;
        }
        // Filas en blanco extra para edición (el form legacy precarga muchas).
        for (int i = 0; i < 8; i++)
        {
            Formatos.Add(new Formato123Item { Numero = (Formatos.Count + 1).ToString() });
        }

        AciertosPermitidos = filtro.AciertosFiltro ?? string.Empty;
        IgnorarRepeticiones = filtro.PasoFijo;
    }

    [RelayCommand]
    private void AgregarFila()
    {
        // Legacy: Añadir_Enter agregaba un CtrlFormato123 nuevo al final.
        Formatos.Add(new Formato123Item { Numero = (Formatos.Count + 1).ToString() });
    }

    // Solo se admiten dígitos 1/2/3 en el formato (Formatos123Frm.CompruebaFormato línea 156).
    private static bool FormatoEsValido(string formato)
    {
        if (formato.Length == 0 || formato.Length > 14) return false;
        foreach (char c in formato)
        {
            if (c != '1' && c != '2' && c != '3') return false;
        }
        return true;
    }

    // Construye la lista de formatos válidos (CompruebaEntradas + AñadirFormato, líneas 116-155).
    private List<Formato123> ConstruirFormatos()
    {
        var lista = new List<Formato123>();
        foreach (var item in Formatos)
        {
            string formato = (item.Formato ?? "").Trim();
            if (formato == "") continue;
            if (!FormatoEsValido(formato)) continue;

            var f = new Formato123 { Formato = formato };
            if (!IgnorarRepeticiones)
            {
                // Si no se ignoran repeticiones, min/max deben ser enteros válidos.
                if (!int.TryParse((item.AciertosMin ?? "").Trim(), out int min)) continue;
                if (!int.TryParse((item.AciertosMax ?? "").Trim(), out int max)) continue;
                f.AciertosMin = min;
                f.AciertosMax = max;
            }
            else
            {
                f.AciertosMin = 0;
                f.AciertosMax = 0;
            }
            if (!lista.Contains(f)) lista.Add(f);
        }
        return lista;
    }

    // Parsea txtAciertos (individuales o intervalos "a-b") a List<int> (ObtenerAciertosPermitidos, líneas 231-255).
    private List<int> ConstruirAciertos()
    {
        var aciertos = new List<int>();
        if (string.IsNullOrWhiteSpace(AciertosPermitidos)) return aciertos;

        foreach (string parte in AciertosPermitidos.Split(','))
        {
            string p = parte.Trim();
            if (p == "") continue;
            if (p.LastIndexOf('-') == -1)
            {
                if (int.TryParse(p, out int v) && v >= 0 && v <= 40) aciertos.Add(v);
            }
            else
            {
                string[] intervalo = p.Split('-');
                if (intervalo.Length == 2 &&
                    int.TryParse(intervalo[0].Trim(), out int desde) &&
                    int.TryParse(intervalo[1].Trim(), out int hasta))
                {
                    for (int j = desde; j <= hasta; j++)
                    {
                        if (j >= 0 && j <= 40) aciertos.Add(j);
                    }
                }
            }
        }
        return aciertos;
    }

    // Valida una columna 1/X/2 de NumeroPartidos signos (Formatos123Frm.CompruebaColumna, líneas 173-193).
    private static bool CompruebaColumna(string columna)
    {
        if (columna.Length != VariablesGlobales.NumeroPartidos) return false;
        foreach (char c in columna.ToUpper())
        {
            if (c != '1' && c != 'X' && c != '2') return false;
        }
        return true;
    }

    [RelayCommand]
    private void Traducir()
    {
        // Equivale a Formatos123Frm.btnTraducir_Click() (líneas 910-923).
        string entrada = (Columna1X2 ?? "").ToUpper();
        if (CompruebaColumna(entrada))
        {
            double[,] nvals = PorcentajesHelper.AMatriz(Porcentajes);  // == controlPorcentajes1.Valores
            var filtroTmp = new FiltroFormatos123
            {
                ValoracionTranformada = TransformarValoracion(nvals),
                Valores = nvals,
            };
            Columna123 = filtroTmp.Col1X2ToCol123(entrada);
        }
        else
        {
            Columna123 = "Error!!!!";
        }
    }

    [RelayCommand]
    private void Analizar()
    {
        // Equivale a Formatos123Frm.btnAnalisis_Click (Free1X2/UI/Filtros/Formatos123Frm.cs
        //   líneas 925-937): si el filtro tiene formatos, abre AnalisisFormatos123Frm con
        //   analisisff.ArrayFormatos = this.filtro.ArrayFormatos; si no, avisa.
        var arrayFormatos = ConstruirFormatos();
        if (arrayFormatos.Count == 0)
        {
            // Legacy: MessageBox.Show("Guarde el Filtro Primero!!!").
            AppServices.MostrarInfo("Guarde el Filtro Primero!!!");
            return;
        }

        // Handoff estático de los formatos (mismo patrón que VerBoletos -> BoletoFrmPage) y
        // navegación a la página portada del analizador.
        AnalisisFormatos123FrmPage.FormatosEntrada = arrayFormatos;
        Navegar?.Invoke(typeof(AnalisisFormatos123FrmPage));
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a Formatos123Frm.menuCondiciones1_BOk -> volcar al filtro + ActivaFiltro
        //   (Free1X2/UI/Filtros/Formatos123Frm.cs líneas 744-772).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroFormatos123)grupo.GetFiltro(Filtro.Formatos123.ToString());

        // Volcar la valoración de la rejilla de porcentajes (equivale a
        // Formatos123Frm.menuCondiciones1_BOk líneas 752-753).
        double[,] nvals = PorcentajesHelper.AMatriz(Porcentajes);  // == controlPorcentajes1.Valores
        filtro.ValoracionTranformada = TransformarValoracion(nvals);
        filtro.Valoracion = nvals;

        var arrayFormatos = ConstruirFormatos();
        var arrayAciertos = ConstruirAciertos();

        if (arrayAciertos.Count > 0)
        {
            arrayAciertos.Sort();
            filtro.AciertosMax = arrayAciertos[arrayAciertos.Count - 1];
            filtro.AciertosMin = arrayAciertos[0];
        }
        filtro.ArrayAciertos = arrayAciertos;
        filtro.ArrayFormatos = arrayFormatos;
        filtro.PasoFijo = IgnorarRepeticiones;
        filtro.AciertosFiltro = AciertosPermitidos ?? string.Empty;
        filtro.IsActive = filtro.NecesitaGuardar();
        if (filtro.ContieneDatos == false)
        {
            filtro.ContieneDatos = filtro.NecesitaGuardar();
        }

        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    // Equivale a Formatos123Frm.TransformarValoracion() (líneas 637-690): a partir de la
    // matriz de porcentajes 1/X/2 produce el byte[N,3] con el orden de signos 1/2/3.
    private static byte[,] TransformarValoracion(double[,] valoracion)
    {
        var valoresTransformados = new byte[VariablesGlobales.NumeroPartidos, 3];
        for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
        {
            double[] valor = { valoracion[i, 0], valoracion[i, 1], valoracion[i, 2] };
            if ((valor[0] >= valor[1]) && (valor[0] >= valor[2]))
            {
                if (valor[1] >= valor[2])
                {
                    valoresTransformados[i, 0] = 4; // "1"
                    valoresTransformados[i, 1] = 2; // "2"
                    valoresTransformados[i, 2] = 1; // "3"
                }
                else if (valor[2] > valor[1])
                {
                    valoresTransformados[i, 0] = 4; // "1"
                    valoresTransformados[i, 1] = 1; // "3"
                    valoresTransformados[i, 2] = 2; // "2"
                }
            }
            else if ((valor[1] > valor[0]) && (valor[1] >= valor[2]))
            {
                if (valor[0] >= valor[2])
                {
                    valoresTransformados[i, 0] = 2; // "2"
                    valoresTransformados[i, 1] = 4; // "1"
                    valoresTransformados[i, 2] = 1; // "3"
                }
                else
                {
                    valoresTransformados[i, 0] = 1; // "3"
                    valoresTransformados[i, 1] = 4; // "1"
                    valoresTransformados[i, 2] = 2; // "2"
                }
            }
            else if ((valor[2] > valor[0]) && (valor[2] > valor[1]))
            {
                if (valor[0] >= valor[1])
                {
                    valoresTransformados[i, 0] = 2; // "2"
                    valoresTransformados[i, 1] = 1; // "3"
                    valoresTransformados[i, 2] = 4; // "1"
                }
                else
                {
                    valoresTransformados[i, 0] = 1; // "3"
                    valoresTransformados[i, 1] = 2; // "2"
                    valoresTransformados[i, 2] = 4; // "1"
                }
            }
        }
        return valoresTransformados;
    }

    /// <summary>
    /// Construye un FiltroFormatos123 temporal con los valores de pantalla.
    /// Réplica de Formatos123Frm.ObtenerFiltroTemporal() (Formatos123Frm.cs líneas 714-739).
    /// </summary>
    private FiltroFormatos123 ObtenerFiltroTemporal()
    {
        double[,] nvals = PorcentajesHelper.AMatriz(Porcentajes);  // == controlPorcentajes1.Valores
        var filtroTemp = new FiltroFormatos123
        {
            ValoracionTranformada = TransformarValoracion(nvals),
            Valoracion = nvals,
        };

        var arrayFormatos = ConstruirFormatos();
        var arrayAciertos = ConstruirAciertos();
        if (arrayAciertos.Count > 0)
        {
            arrayAciertos.Sort();
            filtroTemp.AciertosMax = arrayAciertos[arrayAciertos.Count - 1];
            filtroTemp.AciertosMin = arrayAciertos[0];
        }
        filtroTemp.ArrayAciertos = arrayAciertos;
        filtroTemp.ArrayFormatos = arrayFormatos;
        filtroTemp.PasoFijo = IgnorarRepeticiones;
        filtroTemp.AciertosFiltro = AciertosPermitidos ?? string.Empty;
        filtroTemp.IsActive = filtroTemp.NecesitaGuardar();
        if (filtroTemp.ContieneDatos == false)
        {
            filtroTemp.ContieneDatos = filtroTemp.NecesitaGuardar();
        }
        return filtroTemp;
    }

    // Guarda en disco el filtro temporal (Formatos123Frm.guardar(), líneas 850-855).
    private void GuardarEn(string nombreArchivo)
    {
        var filtroTemp = ObtenerFiltroTemporal();
        var archComb = new ArchivoCondiciones { NombreArchivo = nombreArchivo };
        archComb.GuardaArchivo(filtroTemp);
    }

    // Abre la condición desde disco y vuelca sus valores (Formatos123Frm.abrir(), líneas 856-866).
    private void AbrirDesde(string nombreArchivo)
    {
        var archComb = new ArchivoCondiciones();
        if (archComb.AbrirArchivoCombinacion(nombreArchivo))
        {
            Grupo g = archComb.LeeCondicion();
            var filtro = (FiltroFormatos123)g.GetFiltro("Formatos123");
            MarcarValores(filtro);
        }
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // Equivale a Formatos123Frm.menuCondiciones2_BEstadisticas (Formatos123Frm.cs líneas 953-963).
        var filtroTemp = ObtenerFiltroTemporal();
        var calc = new CalculadorEstadisticas();
        List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, DirectorioGanadoras);

        VisorEstadisticasViewModel.UltimasEstadisticas = lista;
        Navegar?.Invoke(typeof(VisorEstadisticasPage));
    }

    [RelayCommand]
    private async Task Guardar()
    {
        // Equivale a Formatos123Frm.menuCondiciones1_BGuardar (Formatos123Frm.cs líneas 840-848).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Formatos123",
        };
        picker.FileTypeChoices.Add("Formatos 123", new List<string> { ".123" });
        picker.FileTypeChoices.Add("Formatos 123 (XML)", new List<string> { ".xml" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            GuardarEn(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo guardar: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task Abrir()
    {
        // Equivale a Formatos123Frm.menuCondiciones1_BAbrir (Formatos123Frm.cs líneas 823-838).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".123");
        picker.FileTypeFilter.Add(".xml");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            AbrirDesde(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo abrir: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Copiar()
    {
        // Equivale a Formatos123Frm.menuCondiciones1_BCopiar (Formatos123Frm.cs líneas 867-876).
        try
        {
            string ruta = RutaTemporal;
            Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
            GuardarEn(ruta);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo copiar: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Pegar()
    {
        // Equivale a Formatos123Frm.menuCondiciones1_BPegar (Formatos123Frm.cs líneas 878-...).
        if (File.Exists(RutaTemporal))
        {
            AbrirDesde(RutaTemporal);
        }
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a Formatos123Frm.menuCondiciones1_BBorrar: limpia la pantalla (los datos se
        //   aplican al filtro real sólo al Aceptar).
        Formatos.Clear();
        for (int i = 1; i <= 8; i++)
        {
            Formatos.Add(new Formato123Item { Numero = i.ToString() });
        }
        AciertosPermitidos = string.Empty;
        IgnorarRepeticiones = false;
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }
}
