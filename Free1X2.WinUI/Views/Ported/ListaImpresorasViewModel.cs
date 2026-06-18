// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Free1X2.MotorCalculo;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para ListaImpresorasPage.
/// Porta el WinForms legacy Free1X2.UI.ListaImpresoras.
/// Muestra la lista de impresoras conocidas (modelos soportados leídos del XML
/// Impresion/impresoras.cfg vía <see cref="ControladoresImpresion"/>) y permite
/// seleccionar una (doble clic en el legacy) para aplicar su configuración al
/// controlador de impresión activo.
/// </summary>
public partial class ListaImpresorasViewModel : ObservableObject
{
    // Legacy: List<ControladorImpresion> arrayImpresoras (modelos soportados del XML).
    private List<ControladorImpresion> _arrayImpresoras = new();

    // Legacy: ControladorImpresion cont (el controlador activo que recibe la config).
    // El form lo recibía por constructor desde MainForm; aquí se mantiene un controlador
    // propio cuyos valores quedan expuestos como propiedades observables para que la
    // página host lo consuma tras la selección.
    private readonly ControladorImpresion _controladorActivo = new();

    public ListaImpresorasViewModel()
    {
        // Legacy: ListaImpresoras ctor -> CargarListaImpresoras() dentro de try/catch.
        _impresoras = new ObservableCollection<string>();
        CargarListaImpresoras();
        ActualizarMensaje();
    }

    /// <summary>Modelos de impresoras conocidas (legacy: listBox1, Sorted).</summary>
    [ObservableProperty]
    private ObservableCollection<string> _impresoras;

    /// <summary>Impresora seleccionada en la lista (legacy: listBox1.SelectedItem).</summary>
    [ObservableProperty]
    private string? _impresoraSeleccionada;

    /// <summary>
    /// Mensaje de estado.
    /// Legacy lblMensaje: "Doble clic sobre una impresora de la lista" /
    /// "No hay impresoras" / "No se han encontrado impresoras".
    /// </summary>
    [ObservableProperty]
    private string _mensaje = "Doble clic sobre una impresora de la lista";

    // Configuración aplicada del controlador activo (legacy: cont.Modelo/MargenSuperior/...).
    // Expuesta para que la página host la lea tras aplicar la selección.

    /// <summary>Modelo aplicado al controlador activo (legacy: cont.Modelo).</summary>
    [ObservableProperty]
    private string _modeloAplicado = "";

    /// <summary>Margen superior aplicado (legacy: cont.MargenSuperior).</summary>
    [ObservableProperty]
    private int _margenSuperiorAplicado;

    /// <summary>Margen izquierdo aplicado (legacy: cont.MargenIzquierda).</summary>
    [ObservableProperty]
    private int _margenIzquierdaAplicado;

    /// <summary>Indica si la impresión debe rotarse (legacy: cont.Rotar).</summary>
    [ObservableProperty]
    private bool _rotarAplicado;

    /// <summary>Controlador de impresión activo con la última configuración aplicada.</summary>
    public ControladorImpresion ControladorActivo => _controladorActivo;

    /// <summary>
    /// Carga la lista real de modelos soportados (legacy: CargarListaImpresoras).
    /// Lee Impresion/impresoras.cfg vía ControladoresImpresion. Si falla, replica el
    /// catch del legacy: "No se han encontrado impresoras".
    /// </summary>
    private void CargarListaImpresoras()
    {
        try
        {
            Impresoras.Clear();
            var lista = new ControladoresImpresion();
            _arrayImpresoras = lista.Impresoras;
            if (_arrayImpresoras.Count > 0)
            {
                // Legacy: listBox1 está ordenado (Sorted = true).
                _arrayImpresoras.Sort((a, b) =>
                    string.Compare(a.Modelo, b.Modelo, StringComparison.CurrentCulture));
                foreach (var controlador in _arrayImpresoras)
                {
                    Impresoras.Add(controlador.Modelo);
                }
            }
            else
            {
                // Legacy: lblMensaje = "No hay impresoras"; cont = null;
                Mensaje = "No hay impresoras";
            }
        }
        catch
        {
            // Legacy: catch -> lblMensaje.Text = "No se han encontrado impresoras";
            _arrayImpresoras = new List<ControladorImpresion>();
            Mensaje = "No se han encontrado impresoras";
        }
    }

    partial void OnImpresoraSeleccionadaChanged(string? value) => ActualizarMensaje();

    private void ActualizarMensaje()
    {
        if (Impresoras.Count == 0)
        {
            // Conservamos el mensaje de error de carga si lo hubo.
            if (Mensaje != "No se han encontrado impresoras")
            {
                Mensaje = "No hay impresoras";
            }
            return;
        }

        Mensaje = ImpresoraSeleccionada is { Length: > 0 }
            ? $"Impresora seleccionada: {ImpresoraSeleccionada}"
            : "Doble clic sobre una impresora de la lista";
    }

    /// <summary>
    /// Selecciona la impresora activa (legacy: listBox1_DoubleClick).
    /// Busca el controlador por modelo y copia su configuración al controlador activo.
    /// </summary>
    [RelayCommand]
    private void SeleccionarImpresora(string? modelo)
    {
        if (modelo is null or { Length: 0 })
        {
            return;
        }

        ImpresoraSeleccionada = modelo;

        // Legacy: ControladorImpresion controladorImp = BuscarImpresora(...);
        var controladorImp = BuscarImpresora(modelo);

        // Legacy: cont.Modelo/MargenSuperior/MargenIzquierda/Rotar = controladorImp.*
        _controladorActivo.Modelo = controladorImp.Modelo;
        _controladorActivo.MargenSuperior = controladorImp.MargenSuperior;
        _controladorActivo.MargenIzquierda = controladorImp.MargenIzquierda;
        _controladorActivo.Rotar = controladorImp.Rotar;

        // Reflejar la configuración aplicada en las propiedades observables.
        ModeloAplicado = controladorImp.Modelo;
        MargenSuperiorAplicado = controladorImp.MargenSuperior;
        MargenIzquierdaAplicado = controladorImp.MargenIzquierda;
        RotarAplicado = controladorImp.Rotar;

        // Deja el controlador elegido en el handoff estático para que la página productora
        // (ImprimirBoletoFrmPage) copie su config al volver, igual que el btnVerImpresoras_Click
        // legacy leía 'controlador' tras cerrar el diálogo. Luego solicita el cierre (legacy: Close()).
        SeleccionResultado = _controladorActivo;
        CierreSolicitado?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handoff estático con el controlador elegido (legacy: el form recibía/escribía 'controlador'
    /// por referencia y el productor lo leía tras ShowDialog). La página productora lo consume al
    /// volver por Frame.GoBack. null = el usuario no seleccionó ninguna impresora.
    /// </summary>
    public static ControladorImpresion? SeleccionResultado { get; set; }

    /// <summary>
    /// Se solicita cerrar la página (legacy: ListaImpresoras.Close() tras listBox1_DoubleClick).
    /// Lo escucha el code-behind de la página, que hace Frame.GoBack para volver al productor.
    /// </summary>
    public event EventHandler? CierreSolicitado;

    /// <summary>
    /// Busca un controlador por modelo dentro de la lista cargada (legacy: BuscarImpresora).
    /// </summary>
    private ControladorImpresion BuscarImpresora(string modelo)
    {
        var control = new ControladorImpresion();
        for (int i = 0; i < _arrayImpresoras.Count; i++)
        {
            control = _arrayImpresoras[i];
            if (control.Modelo == modelo)
            {
                break;
            }
        }
        return control;
    }
}
