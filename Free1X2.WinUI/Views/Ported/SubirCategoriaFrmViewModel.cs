using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Elemento seleccionable (un partido o un nivel) usado por la página
/// <see cref="SubirCategoriaFrmPage"/>. Reemplaza los CheckBox individuales
/// (checkBox1..16 / chkNiv_0..16) del WinForms <c>SubirCategoriaFrm</c>.
/// </summary>
public partial class CasillaSeleccionable : ObservableObject
{
    public CasillaSeleccionable(string etiqueta)
    {
        _etiqueta = etiqueta;
    }

    [ObservableProperty]
    private string _etiqueta;

    // Equivale a CheckBox.Checked.
    [ObservableProperty]
    private bool _marcada;

    // Equivale a CheckBox.Enabled (se habilita según el nº de signos del archivo).
    [ObservableProperty]
    private bool _habilitada;
}

/// <summary>
/// ViewModel para la pantalla "Subir Categoría".
/// Replica los parámetros de entrada del WinForms <c>SubirCategoriaFrm</c>:
/// a partir de un archivo de columnas de origen, el usuario elige qué partidos
/// están involucrados, cuántos signos van "seguidos", qué niveles de acierto
/// aplicar y, opcionalmente, una combinación externa; el resultado se calcula y
/// se graba en un archivo de salida (sube de categoría las apuestas).
/// </summary>
public partial class SubirCategoriaFrmViewModel : ObservableObject
{
    private const int MaxPartidos = 16;
    private const int MaxNiveles = 17; // niveles 0..16

    public SubirCategoriaFrmViewModel()
    {
        for (int i = 1; i <= MaxPartidos; i++)
        {
            var casilla = new CasillaSeleccionable(i.ToString());
            casilla.PropertyChanged += Partido_PropertyChanged;
            Partidos.Add(casilla);
        }

        // El WinForms etiqueta los niveles del 16 al 0 (chkNiv_16 arriba, chkNiv_0 abajo).
        for (int n = MaxNiveles - 1; n >= 0; n--)
        {
            Niveles.Add(new CasillaSeleccionable(n.ToString()));
        }

        DeshabilitarTodo();
    }

    // ===== Partidos involucrados (groupBox "Partidos Involucrados") =====
    public ObservableCollection<CasillaSeleccionable> Partidos { get; } = new();

    // ===== Niveles (groupBox "Niveles") =====
    public ObservableCollection<CasillaSeleccionable> Niveles { get; } = new();

    // ===== Archivos =====
    // textBoxIn (btnFileIn "Origen (txt)").
    [ObservableProperty]
    private string _archivoOrigen = "";

    // textBoxOut (btnFileOut "Salida (txt)").
    [ObservableProperty]
    private string _archivoSalida = "";

    // txCombinacionExterna (btFileExternas "Usar combinación").
    [ObservableProperty]
    private string _archivoExternas = "";

    // ===== Seguidos (txSeguidos) =====
    [ObservableProperty]
    private double _seguidos;

    // ===== Conteo de columnas (textBoxCount): "I=", "F=", "G=" en el WinForms =====
    [ObservableProperty]
    private string _conteoColumnas = "";

    // ===== Estado / habilitación de acciones =====
    // textBoxOut.Enabled / btnCalcular.Enabled tras elegir origen.
    [ObservableProperty]
    private bool _puedeElegirSalida;

    // btnCalcular.Enabled tras elegir archivo de salida.
    [ObservableProperty]
    private bool _puedeCalcular;

    // btnGrabar.Enabled tras calcular.
    [ObservableProperty]
    private bool _puedeGrabar;

    [ObservableProperty]
    private string _estado = "Selecciona un archivo de origen";

    /// <summary>
    /// Nº de signos detectado en el archivo de origen. Equivale a noSignos /
    /// NumPartidos del WinForms. Determina cuántas casillas se habilitan.
    /// </summary>
    private int _numPartidos = 14;

    // ===== Lógica de habilitación (portada tal cual desde el WinForms) =====

    /// <summary>Equivale a <c>AdaptarInterfaz(noPartidos)</c>.</summary>
    public void AdaptarInterfaz(int noPartidos)
    {
        _numPartidos = noPartidos;
        DeshabilitarTodo();
        for (int i = 0; i < noPartidos && i < Partidos.Count; i++)
        {
            Partidos[i].Habilitada = true;
            Partidos[i].Marcada = true;
        }
        AdaptarNiveles();
    }

    /// <summary>Equivale a <c>AdaptarNiveles()</c>.</summary>
    private void AdaptarNiveles()
    {
        DeshabilitarNiveles();
        // chkNiv_0 siempre habilitado; luego uno por cada partido marcado.
        CasillaSeleccionable? nivel0 = ObtenNivel(0);
        if (nivel0 is not null)
        {
            nivel0.Habilitada = true;
        }

        int nivel = 1;
        foreach (var partido in Partidos)
        {
            if (partido.Marcada)
            {
                CasillaSeleccionable? n = ObtenNivel(nivel);
                if (n is not null)
                {
                    n.Habilitada = true;
                }
                nivel++;
            }
        }
    }

    /// <summary>Equivale a <c>DeshabilitarTodo()</c>.</summary>
    private void DeshabilitarTodo()
    {
        foreach (var p in Partidos)
        {
            p.Habilitada = false;
            p.Marcada = false;
        }
        DeshabilitarNiveles();
    }

    /// <summary>Equivale a <c>DeshabilitarNiveles()</c>.</summary>
    private void DeshabilitarNiveles()
    {
        foreach (var n in Niveles)
        {
            n.Habilitada = false;
            n.Marcada = false;
        }
    }

    // La colección se muestra de 16 a 0; este helper localiza por número de nivel.
    private CasillaSeleccionable? ObtenNivel(int numero)
    {
        string clave = numero.ToString();
        foreach (var n in Niveles)
        {
            if (n.Etiqueta == clave)
            {
                return n;
            }
        }
        return null;
    }

    private void Partido_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // checkBox_CheckedChanged -> AdaptarNiveles().
        if (e.PropertyName == nameof(CasillaSeleccionable.Marcada))
        {
            AdaptarNiveles();
        }
    }

    /// <summary>Equivale a <c>HayAlgunNivelSeleccionado()</c>.</summary>
    private bool HayAlgunNivelSeleccionado()
    {
        foreach (var n in Niveles)
        {
            if (n.Marcada)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Equivale a <c>BtnCalcularClick</c> del WinForms.
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        if (!HayAlgunNivelSeleccionado())
        {
            Estado = "No se ha seleccionado ningún nivel";
            return;
        }

        // Limita "seguidos" al nº de partidos (txSeguidos_TextChanged).
        if (Seguidos > _numPartidos)
        {
            Seguidos = _numPartidos;
        }

        Estado = "Calculando...";

        // TODO(dominio): portar BtnCalcularClick de Free1X2.UI.SubirCategoriaFrm.
        //   - Free1X2.SubirCategoria.Calculos.Calcular(bool[] partidos, bool[] niveles,
        //       int seguidos, string archivoExternas, int noSignos).
        //   - bool[] partidos  = Partidos.Select(p => p.Marcada).
        //   - bool[] niveles   = Niveles ordenados 0..16 .Select(n => n.Marcada).
        //   - El resultado actualiza ConteoColumnas = "F=" + Calculos.NoColumnas.
        //   Tipos en el proyecto WinForms (pendientes de mover a Free1X2.Domain).

        ConteoColumnas = "F=...";
        PuedeGrabar = true;
        Estado = "Calculado (pendiente de portar dominio)";
    }

    /// <summary>
    /// Equivale a <c>BtnGrabarClick</c> del WinForms.
    /// </summary>
    [RelayCommand]
    private void Grabar()
    {
        Estado = "Grabando...";

        // TODO(dominio): portar BtnGrabarClick de Free1X2.UI.SubirCategoriaFrm.
        //   - Free1X2.SubirCategoria.Calculos.Grabar(string nombreSalida).
        //   - Tras grabar: ConteoColumnas = "G=" + Calculos.NoColumnas.

        ConteoColumnas = "G=...";
        Estado = "Grabado (pendiente de portar dominio)";
    }

    /// <summary>
    /// Equivale a la apertura del archivo de origen (BtnFileInClick):
    /// detecta el nº de signos y habilita la interfaz. La lectura real del
    /// archivo se delega al dominio.
    /// </summary>
    public void OnArchivoOrigenSeleccionado(string ruta)
    {
        ArchivoOrigen = ruta;
        ConteoColumnas = "leyendo...";

        // TODO(dominio): portar la lectura de BtnFileInClick de SubirCategoriaFrm.
        //   - Free1X2.EntradaSalida.ArchivoColumnasTexto(ruta).ObtenNumSignos()
        //     -> establece noSignos / NumPartidos.
        //   - new Free1X2.SubirCategoria.Calculos(ruta) -> ConteoColumnas = "I=" + NoColumnas.
        //   Por ahora se asume el valor por defecto de partidos.

        AdaptarInterfaz(_numPartidos);
        PuedeElegirSalida = true;
        PuedeCalcular = false;
        PuedeGrabar = false;
        ConteoColumnas = "I=...";
        Estado = "Archivo de origen cargado";
    }

    /// <summary>Equivale a btnFileOutClick: tras elegir salida se puede calcular.</summary>
    public void OnArchivoSalidaSeleccionado(string ruta)
    {
        ArchivoSalida = ruta;
        PuedeCalcular = true;
        Estado = "Listo para calcular";
    }

    /// <summary>Equivale a btFileExternas_Click.</summary>
    public void OnArchivoExternasSeleccionado(string ruta)
    {
        ArchivoExternas = ruta;
    }
}
