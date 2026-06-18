// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Escrutinio;
using Free1X2.WinUI.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de un partido en la pantalla "Posibles Premios" (legacy: cada terna
/// etiqueta cabecera P1..P16 + TextBox txt1..txt16 con el signo de la columna
/// ganadora + las casillas lblP{n}C1..C8 con los signos de la columna jugada).
/// </summary>
public partial class PartidoPremioItem : ObservableObject
{
    /// <summary>Número de partido 1..16 (legacy: etiquetas P1..P16).</summary>
    public int Numero { get; init; }

    /// <summary>Etiqueta del número de partido como texto (regla anti-crash 2: no se bindea int a Text).</summary>
    public string NumeroTexto => Numero.ToString();

    /// <summary>
    /// Signo ganador introducido por el usuario para este partido
    /// (legacy: TextBox txt{n}; valores admitidos "1" / "X" / "2", o "*" para no definitivo).
    /// </summary>
    [ObservableProperty]
    private string _signoGanador = string.Empty;

    /// <summary>
    /// Signos de las hasta 8 columnas jugadas leídas del fichero
    /// (legacy: lblP{n}C1..lblP{n}C8). Se muestran como texto de solo lectura.
    /// </summary>
    public ObservableCollection<string> SignosColumnas { get; } = new();
}

/// <summary>
/// ViewModel de la pantalla "Posibles Premios" (legacy: PosiblesPremiosFrm).
///
/// Propósito: a partir de una columna ganadora introducida por el usuario y de uno o
/// varios boletos/columnas jugadas leídas de un fichero, calcula a cuántos premios
/// (pleno-16/15, 14, 13, 12, 11 y 10 aciertos) está optando cada columna y resume las
/// columnas con premio. Permite navegar entre boletos del fichero, ver/copiar/guardar
/// el resumen y abrir el diálogo de "Mis mejores opciones".
///
/// La lógica de escrutinio del WinForms original (Escrutar, ObtenGanadora,
/// AnalizarColumnas, AnalizarColumnasResumen, ObtenerResumen) es autocontenida en el
/// formulario y solo depende de Free1X2.EntradaSalida.ArchivoColumnasTexto y
/// Free1X2.Escrutinio.PosiblesPremiosContenedor/PosiblesPremiosComparer (en Domain).
/// Se replica aquí de forma fiel (ver Free1X2/UI/PosiblesPremiosFrm.cs).
/// </summary>
public partial class PosiblesPremiosFrmViewModel : ObservableObject
{
    // Columnas jugadas leidas del fichero (legacy: arrayColumnas).
    private readonly List<string> _arrayColumnas = new();

    // Listas de columnas premiadas por categoria (legacy: col16..col10).
    private readonly List<string> _col16 = new();
    private readonly List<string> _col15 = new();
    private readonly List<string> _col14 = new();
    private readonly List<string> _col13 = new();
    private readonly List<string> _col12 = new();
    private readonly List<string> _col11 = new();
    private readonly List<string> _col10 = new();

    // Todas las premiadas concatenadas para paginar por boletos (legacy: arrayPremiadas).
    private readonly List<string> _arrayPremiadas = new();

    // Resumen detallado de premios (legacy: resumen, List<PosiblesPremiosContenedor>).
    private readonly List<PosiblesPremiosContenedor> _resumen = new();

    // Columna ganadora compuesta a partir de los signos del usuario (legacy: columnaGanadora).
    private string _columnaGanadora = string.Empty;

    // Numero de signos no definitivos ('*') (legacy: signosNoDefinitivos).
    private int _signosNoDefinitivos;

    // Numero de partidos detectado del fichero (legacy: noPartidos).
    private int _noPartidos;

    // Boleto actualmente mostrado en la rejilla y nº total de boletos premiados (legacy: noBoleto/noBoletos).
    private int _noBoleto = 1;
    private int _noBoletos;

    public PosiblesPremiosFrmViewModel()
    {
        for (int i = 1; i <= 16; i++)
        {
            Partidos.Add(new PartidoPremioItem { Numero = i });
        }
    }

    /// <summary>
    /// Navegación a otra página del ContentFrame, inyectada por la Page (mismo patrón que
    /// MainPageViewModel.Navegar). El segundo argumento es el parámetro de navegación opcional
    /// (e.Parameter); para los handoffs estáticos basta con pasar null.
    /// </summary>
    public Action<Type, object?>? Navegar { get; set; }

    /// <summary>
    /// Los 16 partidos con su signo ganador editable y los signos de las columnas jugadas
    /// (legacy: rejilla txt1..txt16 + lblP{n}C1..C8).
    /// </summary>
    public ObservableCollection<PartidoPremioItem> Partidos { get; } = new();

    /// <summary>Opciones de signo para los ComboBox (regla anti-crash 3: ItemsSource desde el VM).</summary>
    public IReadOnlyList<string> SignosPosibles { get; } = new[] { "", "1", "X", "2", "*" };

    /// <summary>Nombre del fichero de columnas abierto (legacy: lblNombreArchivo.Text).</summary>
    [ObservableProperty]
    private string _nombreArchivo = string.Empty;

    /// <summary>
    /// Considerar el último partido como pleno al 15 (legacy: CheckBox chkPleno
    /// "Considerar último partido como pleno").
    /// </summary>
    [ObservableProperty]
    private bool _considerarPleno;

    /// <summary>Generar resumen al calcular (legacy: CheckBox ckbResumen "Resumen").</summary>
    [ObservableProperty]
    private bool _generarResumen;

    // --- Resultados (legacy: lblPremios16..lblPremios10 y lblColumnasConPremio) ---
    // Reglas anti-crash 2: se exponen como string para bindear a TextBlock.Text.

    [ObservableProperty]
    private string _optandoA16 = "Optando a 16: 0";

    [ObservableProperty]
    private string _optandoA15 = "Optando a 15: 0";

    [ObservableProperty]
    private string _optandoA14 = "Optando a 14: 0";

    [ObservableProperty]
    private string _optandoA13 = "Optando a 13: 0";

    [ObservableProperty]
    private string _optandoA12 = "Optando a 12: 0";

    [ObservableProperty]
    private string _optandoA11 = "Optando a 11: 0";

    [ObservableProperty]
    private string _optandoA10 = "Optando a 10: 0";

    /// <summary>Resumen de columnas con premio (legacy: lblColumnasConPremio.Text).</summary>
    [ObservableProperty]
    private string _columnasConPremio = "Columnas con premio: 0";

    /// <summary>
    /// Abre un fichero de columnas/boletos y carga el primer boleto en la rejilla
    /// (legacy: btnAbreArchivo_Click -> EntradaFichero).
    /// </summary>
    [RelayCommand]
    private async System.Threading.Tasks.Task AbrirArchivoAsync()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add(".cols");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        _arrayColumnas.Clear();
        // Legacy EntradaFichero(): IArchivoColumnas ac = new ArchivoColumnasTexto(file);
        //   noPartidos = ac.ObtenNumSignos(); recorre columnas y valida longitud.
        IArchivoColumnas ac = new ArchivoColumnasTexto(file.Path);
        _noPartidos = ac.ObtenNumSignos();
        bool error = false;
        while (ac.SiguienteColumna())
        {
            string columna = ac.LeeColumnaSinComas();
            if (columna.Length != _noPartidos)
            {
                AppServices.MostrarError("Error leyendo columnas");
                _arrayColumnas.Clear();
                NombreArchivo = string.Empty;
                error = true;
                break;
            }
            _arrayColumnas.Add(columna.Substring(0, _noPartidos).ToUpper());
        }
        ac.Cerrar();

        if (error) return;

        NombreArchivo = file.Name;
        AdaptarInterfaz(_noPartidos);
        // Tras abrir, limpia la rejilla de premiadas (legacy: aun no se ha calculado).
        _arrayPremiadas.Clear();
        _noBoleto = 1;
        _noBoletos = 0;
        LimpiarRejilla();
    }

    /// <summary>Avanza al siguiente boleto del fichero (legacy: btnAdelante_Click, ">").</summary>
    [RelayCommand]
    private void Adelante()
    {
        if (_noBoleto >= _noBoletos) return;
        _noBoleto++;
        MostrarColumnas(_noBoleto);
    }

    /// <summary>Retrocede al boleto anterior del fichero (legacy: btnAtras_Click, "<").</summary>
    [RelayCommand]
    private void Atras()
    {
        if (_noBoleto <= 1) return;
        _noBoleto--;
        MostrarColumnas(_noBoleto);
    }

    /// <summary>
    /// Calcula los premios a los que opta cada columna frente a la columna ganadora
    /// (legacy: btnCalcular_Click).
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        if (_arrayColumnas.Count == 0 || _noPartidos == 0)
        {
            AppServices.MostrarError("Primero abre un fichero de columnas.");
            return;
        }

        // Legacy btnCalcular_Click: VaciarArrays(); ObtenGanadora(); AnalizarColumnas();
        //   SumarColumnas(); noBoletos = ObtenerNoBoletos(); ...
        VaciarArrays();
        ObtenGanadora();
        AnalizarColumnas(_columnaGanadora);
        SumarColumnas();
        _noBoletos = ObtenerNoBoletos();
        int columnasPremiadas = _arrayPremiadas.Count;

        if (GenerarResumen)
        {
            _resumen.Clear();
            AnalizarColumnasResumen("", 0);
            if (_resumen.Count > 0)
            {
                _resumen.Sort(new PosiblesPremiosComparer());
            }
        }

        if (columnasPremiadas > 0)
        {
            _noBoleto = 1;
            MostrarColumnas(_noBoleto);
            ColumnasConPremio = "Columnas con premio: " + columnasPremiadas;
            MostrarOpciones();
        }
        else
        {
            ColumnasConPremio = "No hay premios :(";
            MostrarOpciones();
            LimpiarRejilla();
        }
    }

    /// <summary>Muestra el resumen detallado de premios (legacy: btnVer_Click -> VisorPosiblesPremios).</summary>
    [RelayCommand]
    private void Ver()
    {
        // Legacy btnVer_Click (Free1X2/UI/PosiblesPremiosFrm.cs línea 3487):
        //   new VisorPosiblesPremios(resumen).ShowDialog().
        // El resumen sólo existe si se calculó con 'Generar resumen' marcado.
        if (_resumen.Count == 0)
        {
            AppServices.MostrarInfo("No hay resumen que mostrar. Marca 'Generar resumen' y calcula.");
            return;
        }

        // Handoff al visor (mismo patrón que EstucolFrmViewModel.UltimoInforme): el
        // VisorPosiblesPremiosViewModel lee UltimoResumen en su constructor al navegar.
        VisorPosiblesPremiosViewModel.UltimoResumen = new List<PosiblesPremiosContenedor>(_resumen);
        Navegar?.Invoke(typeof(VisorPosiblesPremiosPage), null);
    }

    /// <summary>Copia el resumen al portapapeles (legacy: btnCopiar_Click).</summary>
    [RelayCommand]
    private void Copiar()
    {
        // Legacy btnCopiar_Click (Free1X2/UI/PosiblesPremiosFrm.cs línea 3467): compone 'info'
        //   con lblPremios16..lblPremios10.Text y Clipboard.SetDataObject(info, true).
        var sb = new StringBuilder();
        sb.Append("Posibles premios a falta de " + _signosNoDefinitivos + " partidos:");
        if (_noPartidos >= 16) sb.Append("\r\n" + OptandoA16);
        if (_noPartidos >= 15) sb.Append("\r\n" + OptandoA15);
        sb.Append("\r\n" + OptandoA14);
        sb.Append("\r\n" + OptandoA13);
        sb.Append("\r\n" + OptandoA12);
        sb.Append("\r\n" + OptandoA11);
        sb.Append("\r\n" + OptandoA10);

        var paquete = new DataPackage();
        paquete.SetText(sb.ToString());
        Clipboard.SetContent(paquete);
    }

    /// <summary>Guarda el resumen en un fichero de texto (legacy: btnGuardar_Click -> GrabarResumen).</summary>
    [RelayCommand]
    private async System.Threading.Tasks.Task GuardarAsync()
    {
        if (_resumen.Count == 0)
        {
            AppServices.MostrarInfo("No hay resumen que guardar. Marca 'Generar resumen' y calcula.");
            return;
        }

        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Resumen",
        };
        picker.FileTypeChoices.Add("Resumen", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        // Legacy GrabarResumen() (Free1X2/UI/PosiblesPremiosFrm.cs línea 3327).
        var sb = new StringBuilder();
        sb.AppendLine("Posibles Premios a falta de " + _signosNoDefinitivos + " partidos");
        for (int i = 0; i < _resumen.Count; i++)
        {
            PosiblesPremiosContenedor contenedor = _resumen[i];
            sb.AppendLine("----------------------------------------------");
            sb.AppendLine("Columna Ganadora: " + contenedor.ColGanadora);
            sb.AppendLine("----------------------------------------------");
            EscribirCategoria(sb, contenedor.Col14, "14 aciertos");
            EscribirCategoria(sb, contenedor.Col13, "13 aciertos");
            EscribirCategoria(sb, contenedor.Col12, "12 aciertos");
            EscribirCategoria(sb, contenedor.Col11, "11 aciertos");
            EscribirCategoria(sb, contenedor.Col10, "10 aciertos");
            sb.AppendLine("----------------------------------------------");
            sb.AppendLine(" ");
        }

        await Windows.Storage.FileIO.WriteTextAsync(file, sb.ToString());
    }

    private static void EscribirCategoria(StringBuilder sb, List<string> categoria, string etiqueta)
    {
        for (int j = 0; j < categoria.Count; j++)
        {
            string columna = categoria[j];
            // Legacy graba siempre los 14 primeros signos.
            int recorte = Math.Min(14, columna.Length);
            sb.AppendLine(columna.Substring(0, recorte) + " " + etiqueta);
        }
    }

    /// <summary>Abre el diálogo "Mis mejores opciones" (legacy: btnMejoresOpciones_Click).</summary>
    [RelayCommand]
    private void MejoresOpciones()
    {
        // Legacy btnMejoresOpciones_Click (Free1X2/UI/PosiblesPremiosFrm.cs línea 3501):
        //   ObtenGanadora(); new MejoresOpcionesFrm(chkPleno.Checked) con
        //   ArchivoColumnas = arrayColumnas y ColumnaGanadora = columnaGanadora.
        if (_arrayColumnas.Count == 0 || _noPartidos == 0)
        {
            AppServices.MostrarError("Primero abre un fichero de columnas.");
            return;
        }

        // Compone la columna ganadora a partir de los signos del usuario (legacy: ObtenGanadora()).
        ObtenGanadora();

        // El cálculo ya está cableado en MejoresOpcionesFrmViewModel (EstablecerContexto + Calcular);
        // aquí se pasa el contexto como parámetro de navegación (legacy: propiedades del form modal).
        var contexto = new MejoresOpcionesContexto(
            _columnaGanadora,
            new List<string>(_arrayColumnas),
            ConsiderarPleno);
        Navegar?.Invoke(typeof(MejoresOpcionesFrmPage), contexto);
    }

    // ===== Lógica de escrutinio (réplica fiel del WinForms) =====

    /// <summary>
    /// Compone la columna ganadora a partir de los signos del usuario; los partidos
    /// visibles sin signo válido se marcan como comodín '*' (legacy: ObtenGanadora).
    /// </summary>
    private void ObtenGanadora()
    {
        _signosNoDefinitivos = 0;
        _columnaGanadora = string.Empty;
        for (int i = 0; i < Partidos.Count; i++)
        {
            string s = Partidos[i].SignoGanador ?? string.Empty;
            if (s == "1" || s == "x" || s == "X" || s == "2")
            {
                _columnaGanadora += s.ToUpper();
            }
            else
            {
                // Solo cuentan los partidos visibles segun el nº de partidos del fichero.
                if (i < _noPartidos)
                {
                    _columnaGanadora += "*";
                    Partidos[i].SignoGanador = "*";
                    _signosNoDefinitivos++;
                }
            }
        }
    }

    /// <summary>Escruta una columna analizada contra la ganadora (legacy: Escrutar).</summary>
    private int Escrutar(string cAnalizada, string cGanadora)
    {
        int aciertos = 0;
        int posiblesAciertos = _noPartidos;
        for (int i = 0; i < _noPartidos - 1; i++)
        {
            if (posiblesAciertos < 10) { break; }
            if (cAnalizada[i] == cGanadora[i])
            {
                aciertos++;
            }
            else if (cGanadora[i].ToString() == "*")
            {
                aciertos++;
            }
            else
            {
                posiblesAciertos--;
            }
        }
        // Analiza el último partido (legacy).
        if (ConsiderarPleno)
        {
            if (aciertos == (_noPartidos - 1))
            {
                if (cAnalizada[_noPartidos - 1] == cGanadora[_noPartidos - 1] || cGanadora[_noPartidos - 1].ToString() == "*")
                {
                    aciertos++;
                }
            }
        }
        else
        {
            if (cAnalizada[cAnalizada.Length - 1] == cGanadora[cGanadora.Length - 1] || cGanadora[_noPartidos - 1].ToString() == "*")
            {
                aciertos++;
            }
        }
        return aciertos;
    }

    /// <summary>Clasifica cada columna jugada por nº de aciertos (legacy: AnalizarColumnas).</summary>
    private void AnalizarColumnas(string colGanadora)
    {
        for (int i = 0; i < _arrayColumnas.Count; i++)
        {
            string columnaAAnalizar = _arrayColumnas[i];
            int aciertos = Escrutar(columnaAAnalizar, colGanadora);
            if (aciertos > 9)
            {
                switch (aciertos)
                {
                    case 16: _col16.Add(columnaAAnalizar + aciertos); break;
                    case 15: _col15.Add(columnaAAnalizar + aciertos); break;
                    case 14: _col14.Add(columnaAAnalizar + aciertos); break;
                    case 13: _col13.Add(columnaAAnalizar + aciertos); break;
                    case 12: _col12.Add(columnaAAnalizar + aciertos); break;
                    case 11: _col11.Add(columnaAAnalizar + aciertos); break;
                    case 10: _col10.Add(columnaAAnalizar + aciertos); break;
                }
            }
        }
    }

    /// <summary>
    /// Expande la columna ganadora con comodines y obtiene el resumen para cada
    /// combinación concreta (legacy: AnalizarColumnasResumen).
    /// </summary>
    private void AnalizarColumnasResumen(string preString, int partidoNo)
    {
        string[] signos = { "1", "X", "2" };
        string newPreString;
        for (int i = 0; i < signos.Length; i++)
        {
            if (_columnaGanadora[partidoNo].ToString() == "*")
            {
                newPreString = preString + signos[i];
            }
            else
            {
                newPreString = preString + _columnaGanadora[partidoNo];
                i = 4;
            }

            if ((partidoNo < _columnaGanadora.Length - 1) && (partidoNo < _noPartidos - 1))
            {
                AnalizarColumnasResumen(newPreString, partidoNo + 1);
            }
            else
            {
                ObtenerResumen(newPreString);
            }
        }
    }

    /// <summary>Construye el contenedor de premios para una ganadora concreta (legacy: ObtenerResumen).</summary>
    private void ObtenerResumen(string cGanadora)
    {
        PosiblesPremiosContenedor contenedor = new PosiblesPremiosContenedor();
        contenedor.ColGanadora = cGanadora;
        for (int i = 0; i < _arrayColumnas.Count; i++)
        {
            int aciertos = Escrutar(_arrayColumnas[i], cGanadora);
            if (aciertos > 9)
            {
                switch (aciertos)
                {
                    case 16: contenedor.Col16.Add(_arrayColumnas[i] + "16"); break;
                    case 15: contenedor.Col15.Add(_arrayColumnas[i] + "15"); break;
                    case 14: contenedor.Col14.Add(_arrayColumnas[i] + "14"); break;
                    case 13: contenedor.Col13.Add(_arrayColumnas[i] + "13"); break;
                    case 12: contenedor.Col12.Add(_arrayColumnas[i] + "12"); break;
                    case 11: contenedor.Col11.Add(_arrayColumnas[i] + "11"); break;
                    case 10: contenedor.Col10.Add(_arrayColumnas[i] + "10"); break;
                }
            }
        }
        if (contenedor.Col16.Count > 0 || contenedor.Col15.Count > 0 || contenedor.Col14.Count > 0 ||
            contenedor.Col13.Count > 0 || contenedor.Col12.Count > 0 || contenedor.Col11.Count > 0 ||
            contenedor.Col10.Count > 0)
        {
            _resumen.Add(contenedor);
        }
    }

    /// <summary>Vacía las listas de premiadas (legacy: VaciarArrays).</summary>
    private void VaciarArrays()
    {
        _arrayPremiadas.Clear();
        _col16.Clear();
        _col15.Clear();
        _col14.Clear();
        _col13.Clear();
        _col12.Clear();
        _col11.Clear();
        _col10.Clear();
    }

    /// <summary>Concatena las premiadas en orden 16..10 (legacy: SumarColumnas).</summary>
    private void SumarColumnas()
    {
        _arrayPremiadas.Clear();
        _arrayPremiadas.AddRange(_col16);
        _arrayPremiadas.AddRange(_col15);
        _arrayPremiadas.AddRange(_col14);
        _arrayPremiadas.AddRange(_col13);
        _arrayPremiadas.AddRange(_col12);
        _arrayPremiadas.AddRange(_col11);
        _arrayPremiadas.AddRange(_col10);
    }

    /// <summary>Calcula el nº de boletos (8 columnas cada uno) (legacy: ObtenerNoBoletos).</summary>
    private int ObtenerNoBoletos()
    {
        int totalPartes = _arrayPremiadas.Count / 8;
        int resto = _arrayPremiadas.Count % 8;
        return resto > 0 ? totalPartes + 1 : totalPartes;
    }

    /// <summary>Muestra el boleto indicado en la rejilla (legacy: MostrarColumnas).</summary>
    private void MostrarColumnas(int numBoleto)
    {
        int noCol = (numBoleto * 8) - 8;
        LlenarColumnas(noCol);
    }

    /// <summary>
    /// Vuelca hasta 8 columnas premiadas a las casillas de la rejilla (legacy: LlenarColumnas).
    /// En WinUI cada partido tiene una colección SignosColumnas con un signo por columna.
    /// </summary>
    private void LlenarColumnas(int numCol)
    {
        LimpiarRejilla();
        if (_arrayPremiadas.Count == 0) return;

        int columnasQueQuedan = _arrayPremiadas.Count - numCol;
        int aMostrar = columnasQueQuedan <= 8 ? columnasQueQuedan : 8;

        for (int c = 0; c < aMostrar; c++)
        {
            string columnaPremiada = _arrayPremiadas[numCol + c];
            string columna = columnaPremiada.Substring(0, _noPartidos);
            for (int p = 0; p < _noPartidos && p < Partidos.Count; p++)
            {
                Partidos[p].SignosColumnas.Add(columna[p].ToString());
            }
        }
    }

    /// <summary>Limpia las casillas de columnas jugadas de la rejilla (legacy: LimpiarPantalla).</summary>
    private void LimpiarRejilla()
    {
        foreach (var p in Partidos)
        {
            p.SignosColumnas.Clear();
        }
    }

    /// <summary>Actualiza las etiquetas de premios (legacy: MostrarOpciones).</summary>
    private void MostrarOpciones()
    {
        int p16 = _col16.Count;
        int p15 = _col15.Count;
        int p14 = _col14.Count;
        int p13 = _col13.Count;
        int p12 = _col12.Count;
        int p11 = _col11.Count;
        int p10 = _col10.Count;

        if (ConsiderarPleno)
        {
            p14 += p15;
        }

        OptandoA16 = "Optando a 16: " + p16;
        OptandoA15 = "Optando a 15: " + p15;
        OptandoA14 = "Optando a 14: " + p14;
        OptandoA13 = "Optando a 13: " + p13;
        OptandoA12 = "Optando a 12: " + p12;
        OptandoA11 = "Optando a 11: " + p11;
        OptandoA10 = "Optando a 10: " + p10;
    }

    /// <summary>
    /// Ajusta la visibilidad de las casillas según el nº de partidos del fichero
    /// (legacy: AdaptarInterfaz; los partidos > noPartidos se ocultan limpiando su signo).
    /// La rejilla WinUI muestra siempre los 16; aquí se vacía el signo de los que sobran.
    /// </summary>
    private void AdaptarInterfaz(int partidos)
    {
        for (int i = partidos; i < Partidos.Count; i++)
        {
            Partidos[i].SignoGanador = string.Empty;
        }
    }
}
