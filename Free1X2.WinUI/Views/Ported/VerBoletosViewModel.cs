using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Free1X2;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del formulario legacy WinForms "VerBoletos" (título: "Ver Boletos").
///
/// Propósito: el usuario elige un fichero de columnas (TextBox 'fichero' + Button 'abrir'),
/// selecciona un criterio de ordenación (groupBox1 "Ordenar por ...": No ordenar / Variantes /
/// X / 2 / Interrupciones / Signos seguidos) y un sentido (groupBox2 "Tipo de ordenamiento":
/// ascendente / descendente), y al pulsar "Ok" se abre el boleto con esos parámetros.
///
/// El criterio se mapea al enum <c>OrdenarMatriz</c> y el sentido a <c>TipoOrden</c>,
/// exactamente como hacía el WinForms (oN_CheckedChanged / tN_CheckedChanged). La apertura
/// del boleto se publica vía <see cref="AbrirBoletoSolicitado"/> para que la página host
/// navegue a BoletoFrmPage con el fichero y los enums ya resueltos (el BoletoFrmViewModel
/// está cableado al motor: ArchivoColumnasTexto + ordenación real).
/// </summary>
public partial class VerBoletosViewModel : ObservableObject
{
    // ---- Fichero de columnas (TextBox 'fichero', ReadOnly) ----

    [ObservableProperty]
    private string _fichero = "(falta selección)";

    // btnOk estaba deshabilitado hasta elegir fichero (btnOk.Enabled = true en abrir_Click).
    [ObservableProperty]
    private bool _okHabilitado;

    // ---- Criterio de ordenación (groupBox1 "Ordenar por ...") ----
    // RadioButtons exclusivos. 'o0' (No ordenar) estaba marcado por defecto.

    [ObservableProperty]
    private bool _ordenarNoOrdenar = true;       // o0 -> OrdenarMatriz.Signo

    [ObservableProperty]
    private bool _ordenarVariantes;              // o1 -> OrdenarMatriz.Variantes

    [ObservableProperty]
    private bool _ordenarEquis;                  // o2 -> OrdenarMatriz.Equis

    [ObservableProperty]
    private bool _ordenarDoses;                  // o3 -> OrdenarMatriz.Doses

    [ObservableProperty]
    private bool _ordenarInterrupciones;         // o4 -> OrdenarMatriz.Interrupciones

    // o5 estaba deshabilitado (Enabled = false) en el form legacy.
    [ObservableProperty]
    private bool _ordenarSignosSeguidos;         // o5 -> OrdenarMatriz.SignosSeguidos

    // ---- Sentido de ordenación (groupBox2 "Tipo de ordenamiento") ----
    // 't1' (ascendente) estaba marcado por defecto.

    [ObservableProperty]
    private bool _sentidoAscendente = true;      // t1 -> TipoOrden.asc

    [ObservableProperty]
    private bool _sentidoDescendente;            // t2 -> TipoOrden.desc

    // ---- Estado ----

    [ObservableProperty]
    private string _estado = "Seleccione un fichero de columnas";

    // Indica si el boleto ya se ha cargado y debe mostrarse el visor embebido.
    [ObservableProperty]
    private bool _boletoVisible;

    /// <summary>
    /// Visor de boletos embebido. Reutiliza el <see cref="BoletoFrmViewModel"/> (que ya
    /// está cableado al motor real: ArchivoColumnasTexto + CreaMatriz +
    /// OrdenarMatrizColumnas) igual que el legacy hacía con <c>boleto.ShowDialog()</c>.
    /// La página enlaza el <c>BoletoMatrizControl</c> y la navegación a este sub-VM.
    /// </summary>
    public BoletoFrmViewModel Boleto { get; } = new();

    /// <summary>
    /// Solicitud de apertura del boleto (legacy: btnOk_Click -> boleto.ShowDialog()).
    /// Argumentos: ruta del fichero, criterio (OrdenarMatriz) y sentido (TipoOrden) ya resueltos.
    /// La página host reacciona mostrando el visor embebido (BoletoMatrizControl).
    /// </summary>
    public event EventHandler<(string fichero, OrdenarMatriz orden, TipoOrden tipo)>? AbrirBoletoSolicitado;

    /// <summary>Criterio de ordenación resuelto a partir de los radios (legacy: campo ordenarPor).</summary>
    public OrdenarMatriz OrdenSeleccionado
    {
        get
        {
            // Legacy oN_CheckedChanged: cada radio fija ordenarPor.
            if (OrdenarVariantes) return OrdenarMatriz.Variantes;
            if (OrdenarEquis) return OrdenarMatriz.Equis;
            if (OrdenarDoses) return OrdenarMatriz.Doses;
            if (OrdenarInterrupciones) return OrdenarMatriz.Interrupciones;
            if (OrdenarSignosSeguidos) return OrdenarMatriz.SignosSeguidos;
            return OrdenarMatriz.Signo; // o0 "No ordenar" por defecto.
        }
    }

    /// <summary>Sentido resuelto a partir de los radios (legacy: campo tipoOrden).</summary>
    public TipoOrden TipoSeleccionado =>
        SentidoDescendente ? TipoOrden.desc : TipoOrden.asc; // t1 asc por defecto.

    /// <summary>Selecciona el fichero de columnas (Button 'abrir' del form legacy).</summary>
    [RelayCommand]
    private async Task SeleccionarFicheroAsync()
    {
        // Legacy abrir_Click: OpenFileDialog filtro
        //   "Columnas(*.txt)|*.txt|Columnas(*.cols)|*.cols|Todos (*.*)|*.*",
        //   InitialDirectory "Columnas\".
        try
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".cols");
            picker.FileTypeFilter.Add("*");

            WinRT.Interop.InitializeWithWindow.Initialize(picker, Services.AppServices.WindowHandle);
            StorageFile? file = await picker.PickSingleFileAsync();
            if (file is null)
            {
                return;
            }

            // Legacy: fichero.Text = archivoEntrada; btnOk.Enabled = true; preparar BoletoFrm.
            Fichero = file.Path;
            OkHabilitado = true;
            Estado = "Fichero seleccionado";
        }
        catch (Exception ex)
        {
            Estado = $"Error al seleccionar el fichero: {ex.Message}";
            Services.AppServices.MostrarError($"No se pudo seleccionar el fichero: {ex.Message}");
        }
    }

    /// <summary>Muestra el boleto (btnOk_Click del form legacy).</summary>
    [RelayCommand]
    private async Task MostrarBoletoAsync()
    {
        if (!OkHabilitado || string.IsNullOrEmpty(Fichero) || Fichero == "(falta selección)")
        {
            Estado = "Seleccione un fichero de columnas";
            return;
        }

        // Legacy btnOk_Click: boleto.ArchivoCombinacion = fichero.Text;
        //   boleto.ordenarPor = ordenarPor; boleto.tipoOrden = tipoOrden; boleto.ShowDialog().
        // Aquí se reutiliza el BoletoFrmViewModel embebido (cableado al motor real) y se
        // carga el boleto en el visor BoletoMatrizControl de la propia página.
        Estado = "Mostrando boleto";
        Boleto.ArchivoCombinacion = Fichero;
        Boleto.OrdenarPor = OrdenSeleccionado;
        Boleto.TipoOrden = TipoSeleccionado;

        // Avisa a la página (suscribe Boleto.BoletoCambiado antes de cargar).
        AbrirBoletoSolicitado?.Invoke(this, (Fichero, OrdenSeleccionado, TipoSeleccionado));

        await Boleto.CargarCommand.ExecuteAsync(null);
        BoletoVisible = true;
        Estado = Boleto.Estado;
    }

    /// <summary>Cancela / cierra (btnCancel_Click del form legacy).</summary>
    [RelayCommand]
    private void Cancelar()
    {
        // El form legacy cerraba la ventana; la navegación la resuelve el code-behind.
        Estado = "Cancelado";
    }
}
