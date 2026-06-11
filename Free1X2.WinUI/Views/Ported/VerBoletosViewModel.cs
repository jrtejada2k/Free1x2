using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del formulario legacy WinForms "VerBoletos" (título: "Ver Boletos").
///
/// Propósito: el usuario elige un fichero de columnas (TextBox 'fichero' + Button 'abrir'),
/// selecciona un criterio de ordenación (groupBox1 "Ordenar por ...": No ordenar / Variantes /
/// X / 2 / Interrupciones / Signos seguidos) y un sentido (groupBox2 "Tipo de ordenamiento":
/// ascendente / descendente), y al pulsar "Ok" se abre el boleto con esos parámetros.
///
/// En el form legacy el criterio se mapea al enum <c>OrdenarMatriz</c>:
///   No ordenar -> Signo, Variantes -> Variantes, X -> Equis, 2 -> Doses,
///   Interrupciones -> Interrupciones, Signos seguidos -> SignosSeguidos.
/// El sentido se mapea al enum <c>TipoOrden</c> (asc / desc).
///
/// Lógica de dominio (selección de fichero y apertura del boleto) NO portada:
/// ver clase legacy <c>Free1X2.UI.VerBoletos</c> y el control de boleto
/// <c>Free1X2.UI.BoletoFrm</c> (propiedades ArchivoCombinacion, ordenarPor, tipoOrden).
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

    /// <summary>Selecciona el fichero de columnas (Button 'abrir' del form legacy).</summary>
    [RelayCommand]
    private void SeleccionarFichero()
    {
        // TODO(dominio): replicar abrir_Click de Free1X2.UI.VerBoletos:
        //   abrir un FileOpenPicker (filtro *.txt / *.cols / *.*), fijar Fichero con la ruta,
        //   habilitar Ok y preparar el BoletoFrm:
        //     boleto = new BoletoFrm();
        //     boleto.ArchivoCombinacion = Fichero;
        //     boleto.ordenarPor = <OrdenarMatriz según radio>;
        //     boleto.tipoOrden  = <TipoOrden según radio>;
        // El dominio no está disponible aquí; sólo se simula la habilitación de Ok.
        OkHabilitado = true;
        Estado = "Fichero seleccionado";
    }

    /// <summary>Muestra el boleto (btnOk_Click del form legacy).</summary>
    [RelayCommand]
    private void MostrarBoleto()
    {
        // TODO(dominio): replicar btnOk_Click de Free1X2.UI.VerBoletos:
        //   crear/abrir BoletoFrm con ArchivoCombinacion = Fichero y los enums
        //   ordenarPor (OrdenarMatriz) y tipoOrden (TipoOrden) según los radios marcados,
        //   y llamar boleto.ShowDialog(). En WinUI sería navegar a BoletoFrmPage / BoletoControl.
        Estado = "Mostrando boleto";
    }

    /// <summary>Cancela / cierra (btnCancel_Click del form legacy).</summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO(dominio): el form legacy cerraba la ventana. La navegación la resuelve el code-behind.
        Estado = "Cancelado";
    }
}
